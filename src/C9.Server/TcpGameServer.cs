using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using C9.Protocol;

namespace C9.Server;

public sealed class TcpGameServer(ServerOptions options, IServerEvents events, ServiceRuntime? runtime = null) : IAsyncDisposable
{
    private sealed record Session(TcpClient Client, TaskCompletionSource Completion, SessionContext Context);
    private readonly ConcurrentDictionary<long, Session> sessions = new();
    private readonly SemaphoreSlim slots = new(options.MaxConnections, options.MaxConnections);
    private readonly SessionProcessor processor = new(options, events);
    private CancellationTokenSource? lifetime;
    private TcpListener? listener;
    private Task? acceptLoop;
    private long sessionNumber;
    private bool disposed;
    public int Port => listener?.LocalEndpoint is IPEndPoint endpoint ? endpoint.Port : 0;
    public int ActiveSessions => sessions.Count;
    public Task Completion => acceptLoop ?? Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (lifetime is not null) throw new InvalidOperationException("Server already started");
        options.Validate();
        _ = new ConnectionCodec(options.MaxFrame);
        lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        listener = new TcpListener(IPAddress.Parse(options.Host), options.Port);
        listener.Start();
        events.Emit(new ServerEvent("listening")
        {
            Host = options.Host, Port = Port, Profile = options.Profile,
            ClientCompatibility = "unverified", CipherEpochLimit = InitialCipherDirection.FrameLimit, Runtime = "dotnet"
        });
        acceptLoop = AcceptLoopAsync(lifetime.Token);
        return Task.CompletedTask;
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await listener!.AcceptTcpClientAsync(cancellationToken);
                client.NoDelay = true;
                if (!slots.Wait(0))
                {
                    events.Emit(new ServerEvent("connection_limit") { Remote = runtime is null ? null : client.Client.RemoteEndPoint?.ToString() });
                    runtime?.ConnectionRejected();
                    client.Dispose();
                    continue;
                }
                SessionContext context = runtime?.CreateSession(client.Client.RemoteEndPoint?.ToString(), Port)
                    ?? SessionContext.Legacy(Interlocked.Increment(ref sessionNumber));
                long id = context.Number;
                TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
                sessions[id] = new Session(client, completion, context);
                _ = HandleSessionAsync(context, client, completion, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (SocketException) when (cancellationToken.IsCancellationRequested) { }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
    }

    private async Task HandleSessionAsync(SessionContext context, TcpClient client, TaskCompletionSource completion,
        CancellationToken cancellationToken)
    {
        long id = context.Number;
        PipeReader? reader = null;
        PipeWriter? writer = null;
        try
        {
            NetworkStream stream = client.GetStream();
            reader = PipeReader.Create(stream, new StreamPipeReaderOptions(bufferSize: 4096,
                minimumReadSize: 512, leaveOpen: true));
            writer = PipeWriter.Create(stream, new StreamPipeWriterOptions(leaveOpen: true));
            await processor.RunAsync(context, reader, writer, cancellationToken);
        }
        catch (SocketException)
        { events.Emit(new ServerEvent("session_error") { Session = id, SessionKey = context.Keyed ? context.Key : null, Reason = "ConnectionError" }); }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
        catch (ObjectDisposedException)
        { events.Emit(new ServerEvent("session_error") { Session = id, SessionKey = context.Keyed ? context.Key : null, Reason = "Disconnected" }); }
        finally
        {
            // Close the transport before completing adapters, so cleanup cannot
            // retry a blocked network flush after a session deadline has expired.
            client.Dispose();
            try
            {
                if (reader is not null) await reader.CompleteAsync();
                if (writer is not null) await writer.CompleteAsync();
            }
            catch (IOException) { }
            catch (ObjectDisposedException) { }
            finally
            {
                sessions.TryRemove(id, out _);
                slots.Release();
                completion.TrySetResult();
            }
        }
    }

    /// <summary>Closes one connection; its session ends with a connection error.</summary>
    public bool Disconnect(long number)
    {
        if (!sessions.TryGetValue(number, out Session? session)) return false;
        session.Context.MarkDisconnectRequested();
        session.Client.Dispose();
        return true;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (lifetime is null || disposed) return;
        await lifetime.CancelAsync();
        listener!.Stop();
        if (acceptLoop is not null) await acceptLoop.WaitAsync(cancellationToken);
        Session[] active = sessions.Values.ToArray();
        foreach (Session session in active) session.Client.Dispose();
        await Task.WhenAll(active.Select(s => s.Completion.Task)).WaitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed) return;
        await StopAsync();
        disposed = true;
        lifetime?.Dispose();
        slots.Dispose();
    }
}
