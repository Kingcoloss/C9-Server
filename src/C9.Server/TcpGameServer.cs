using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using C9.Protocol;

namespace C9.Server;

public sealed class TcpGameServer(ServerOptions options, IServerEvents events) : IAsyncDisposable
{
    private sealed record Session(TcpClient Client, TaskCompletionSource Completion);
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
                    events.Emit(new ServerEvent("connection_limit"));
                    client.Dispose();
                    continue;
                }
                long id = Interlocked.Increment(ref sessionNumber);
                TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
                sessions[id] = new Session(client, completion);
                _ = HandleSessionAsync(id, client, completion, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (SocketException) when (cancellationToken.IsCancellationRequested) { }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
    }

    private async Task HandleSessionAsync(long id, TcpClient client, TaskCompletionSource completion,
        CancellationToken cancellationToken)
    {
        PipeReader? reader = null;
        PipeWriter? writer = null;
        try
        {
            NetworkStream stream = client.GetStream();
            reader = PipeReader.Create(stream, new StreamPipeReaderOptions(bufferSize: 4096,
                minimumReadSize: 512, leaveOpen: true));
            writer = PipeWriter.Create(stream, new StreamPipeWriterOptions(leaveOpen: true));
            await processor.RunAsync(id, reader, writer, cancellationToken);
        }
        catch (SocketException)
        { events.Emit(new ServerEvent("session_error") { Session = id, Reason = "ConnectionError" }); }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
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
