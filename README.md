# C9 Server

C#/.NET 10 server runtime and reverse-engineering tools for the selected C9
Client build. Python is retained as a protocol reference and RE toolchain.
The port supports encrypted login/server-list **fixtures**. Original-game
compatibility, authentication semantics and successful game login remain unverified.

The original game, Launcher and Patcher must stay closed until the project
owner explicitly authorizes a debug session. Commands below run our own code.

## Run on this Mac

From the repository root, using the installed .NET 10 SDK:

```bash
dotnet build C9.slnx -c Release --disable-build-servers -m:1 -p:UseSharedCompilation=false

# Verify configuration, managed crypto and embedded tables without opening a socket.
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --check-runtime

# Passive TCP endpoint; validates packets and logs metadata.
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll

# Explicit synthetic response scenario.
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --config config/lab-fixture.json
```

Both bind `127.0.0.1:14000`. Use `--port 0` for an OS-assigned local port, or
`--port 14001` to choose another. Stop with Ctrl-C. Only numeric loopback bind
addresses are accepted, with packet, connection, frame and timeout limits.
The server uses the .NET/ASP.NET Core 10 shared frameworks and no external NuGet
packages. Managed RIPEMD160/RC6 and embedded payload tables remove Python,
OpenSSL legacy-provider and Client-folder dependencies from the server runtime.
The current loopback restriction remains while original-Client protocol and auth
validation are pending.

The fixture profile sends a constructed type-4 greeting, accepts a fixed-length
`0x2EE1` payload, returns an opaque `0x2EE2` fixture, then answers `0x2EE5` with a
one-entry `0x2EE6` fixture. These choices exercise static layouts and transport.
They do not authenticate credentials or establish the original handshake.
The five DWORD values in the login response and the server entry are synthetic.
Entry text encoding and most entry fields remain unresolved.

## Validate

```bash
# Socket-free .NET protocol/session validation.
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll --offline

# Add Python comparisons and cross-language stdio fixture flow.
python3 re/scripts/validate_dotnet_port.py --offline

# Full TCP integration suite, in an environment that permits loopback sockets.
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll
python3 re/scripts/validate_dotnet_port.py
```

The .NET validation executable is a package-free test harness. Offline mode runs
24 cases and explicitly skips 10 socket cases. It checks 960 Python payload
vectors, 64 sequential wire vectors and state tables, independent RC6/RIPEMD160
vectors, framing, session isolation, timeout, packet limits and backpressure.
The same session processor is used by TCP and in-memory/stdio adapters.

In the current execution environment, outbound loopback connection attempts
returned `SocketException (13): Permission denied`. The .NET TCP suite and the
Linux deployment target therefore remain unverified. Socket-free checks passing
do not establish TCP or original-game compatibility.

## Publish

```bash
dotnet publish src/C9.Server/C9.Server.csproj -c Release --no-restore -o artifacts/server
dotnet artifacts/server/C9.Server.dll --check-runtime
dotnet artifacts/server/C9.Server.dll --config config/lab-fixture.json
```

This is framework-dependent output; the target needs the .NET/ASP.NET Core 10
shared frameworks. Publish/runtime checks have been exercised on macOS ARM64.

## Implemented scope

| Component | Current behavior |
|---|---|
| TCP host | Sockets, Pipelines, Generic Host, connection limits and graceful shutdown; socket integration remains pending in this environment |
| Session runtime | Shared processor with bounded framing, deadlines, backpressure and metadata logs |
| Header/checksum | 13-byte decoded header, signed-byte checksum, uint32 wrap |
| Payload transform | All 80 observed selectors, encode/decode, four length branches |
| Initial outer cipher | Recovered fixed seed, RIPEMD160, RC6 with a 16-byte key |
| Outer wrapper | Independent blocks, zero fill, clear padding-count footer |
| Sequence | Separate send/receive counters; plain frames do not advance them |
| Cipher rotation | Initial 256 transformed frames per direction; later epochs rejected |
| Client send adjustment | Bounded search over 80 transforms; one checksum-valid plaintext |
| Messages | Opaque 52-byte server-list entries, explicit fixture response rules |
| Authentication / game world | Pending protocol and runtime evidence |

The cipher implementation is grounded in the selected Client's static code.
RC6 vectors validate the primitive, and differential/basis checks validate the
recovered payload maps. They do not replace an original-Client trace. This is a
research server, with no database, character service, world simulation or admin
interface yet.

## Relocated Client folder

The Client has been moved out of this repository. RE scripts resolve its location
from `--client-dir`, then `C9_CLIENT_DIR`, then `config/research.local.json`.
Relative paths are interpreted from the project root. A historical `52C9GL`
directory is used only if no new location is configured and that directory exists.

```bash
export C9_CLIENT_DIR="/absolute/path/to/C9 Client"
python3 re/scripts/check_client_integrity.py
```

Alternatively, copy `config/research.example.json` to
`config/research.local.json` and replace its placeholder. The local file is
ignored by Git. Integrity checks read the Client and write reports outside its
folder. Ghidra `--process` exports use the existing research database and do not
require the original Client directory. The server never needs this setting.

## Project map

- `src/C9.Protocol/`: managed binary codec, crypto, payload transforms and message layouts.
- `src/C9.Server/`: primary .NET TCP host, session processor and configuration.
- `tests/C9.Validation/`: .NET validation executable, offline and TCP suites.
- `c9server/`: Python reference implementation and canonical extracted payload data.
- `config/lab-fixture.json`: explicit synthetic response scenario.
- `tests/fixtures/`: independent primitive vectors and Python port vectors.
- `re/scripts/`: static extraction and offline inspection tools.
- `re/evidence/`: static exports, provenance and validation records.
- `re/C9_RE_PLAN_TH.md`: milestones and acceptance criteria.
- `re/C9_RE_Phase5_DotNet_Port_TH.md`: port report, validation and remaining gaps.

Rebuild payload maps with `re/scripts/build_twister_tables.py` after the bounded
Ghidra exports are available. Extraction needs the installed Clang; normal
server execution uses portable JSON tables and never loads the Windows PE.

Next work is subsequent key rotation, remaining message schemas and the first
original-Client debug session after the owner permits opening the game. M4's
original-Client acceptance criterion remains pending.
