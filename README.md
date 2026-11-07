# C9 Server

C#/.NET 10 server runtime and reverse-engineering tools for the selected C9
Client build. Python is retained as a protocol reference and RE toolchain.
The [development plan](docs/DEVELOPMENT_PLAN_TH.md) includes the complete-map
survey and planned Admin Portal features, dependencies and acceptance criteria.
The [independent local launcher](docs/LOCAL_LAUNCHER_TH.md) preserves the official
updater for the newly supplied client. After official updates, signed build 55350
reaches world selection through VFUN on the Windows ARM64 VM. With the corrected
`fromVLauncher` marker and synthetic launch fields, it also reaches our loopback
server and sends a plain `0x2F04` request with a 1,092-byte payload. After the
`0x2EE2` response, the live Client sent `0x2EF6` with four payload bytes. With
the local-account `0x2EF7` reply it refreshed the world list and sent `0x2EEC`
when a world was selected. The [loopback fixture](config/lab-official-login-fixture.json)
now replies with a proposed handoff to port 14001. The gameplay
results below apply to the older lab Client, not builds 52893 or 55350.
The port supports encrypted login/server-list **fixtures**. The selected game
has exchanged its initial login/list messages with these fixtures, displayed
`C9 Local` with status `Good`, and followed a synthetic TCP handoff to a second
loopback endpoint. The character profile creates and persists a local character,
reloads its appearance, and enters Waterford Village in the original Client.
The zero-based sub-map lookup and missing actor-class field are fixed. The
Client reaches C9StageGame and opens Inventory; stats, bag capacity, skills,
connection lifecycle and server gameplay remain incomplete. Credential validation
is still unimplemented.

The owner authorized opening the original game and Launcher for the Windows
debug session on 3 November 2026. The local update service and protocol probe
are described in [the Windows session notes](docs/WINDOWS_LAUNCHER_TH.md).
The [Client startup notes](docs/WINDOWS_CLIENT_STARTUP_TH.md) document the
reversible two-byte wrapper fix and the Client's required SERVER manifest fields.

## Run the protocol server

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
Repeated list requests receive the same list without resetting the session.
Selecting the fixture server returns a constructed `0x2EED` endpoint reply;
`scripts/Start-C9Lab.ps1 -GameProfile fixture` also starts the observer on 14001.
The `observer` profile sends one plain type-4 mode-1 greeting, then validates
framing, crypto and sequence while recording metadata for all packet types.
It accepts interleaved keepalives/list requests without application replies.
The separate `character_lobby` profile implements a single synthetic account's
three character slots and local JSON persistence. Run
`scripts/Start-C9Lab.ps1 -OfflineLauncher -GameProfile fixture -HandoffConfig config/lab-character-lobby.json`
with the patched Launcher; see [character test notes](docs/WINDOWS_CHARACTER_LOBBY_TH.md).
Authentication and a UDP service remain unimplemented.
They do not authenticate credentials or establish the original authentication semantics.
The five DWORD values in the login response and the server entry are synthetic.
The entry includes a short UTF-16LE name at +5 and a synthetic nonzero selection
gate at +51, based on selected-Client readers. Most entry fields remain unresolved.
See [the Windows handshake evidence](docs/WINDOWS_HANDSHAKE_TH.md).

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
26 cases and explicitly skips 15 socket cases. It checks 960 Python payload
vectors, 64 sequential wire vectors and state tables, independent RC6/RIPEMD160
vectors, framing, session isolation, timeout, packet limits and backpressure.
The same session processor is used by TCP and in-memory/stdio adapters.

On Windows ARM64, the full .NET suite passed all 43 cases (6,280 assertions), including all catalogued map routes,
character persistence, the TCP suite and the local launcher HTTP contract, on 6 November 2026. The Python
protocol/server suite passed all 19 cases. The Windows half-close test retains
the connected NetworkStream before shutting down the sending socket.
The earlier macOS environment rejected outbound loopback connects with
`SocketException (13)`. Linux deployment and original-game authentication
remain unverified.

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
| TCP host | Sockets, Pipelines, Generic Host, connection limits and graceful shutdown; TCP suite passes on Windows ARM64 |
| Launcher update host | Loopback HTTP manifest based on the installed version.dat; no patch archives or Client executables are served |
| Independent local UI | Own Windows UI, profile-checked new-Client arguments, automatic loopback observer startup, separate official updater entry |
| Session runtime | Shared processor with bounded framing, deadlines, backpressure and metadata logs |
| Header/checksum | 13-byte decoded header, signed-byte checksum, uint32 wrap |
| Payload transform | All 80 observed selectors, encode/decode, four length branches |
| Initial outer cipher | Recovered fixed seed, RIPEMD160, RC6 with a 16-byte key |
| Outer wrapper | Independent blocks, zero fill, clear padding-count footer |
| Sequence | Separate send/receive counters; plain frames do not advance them |
| Cipher rotation | Initial 256 transformed frames per direction; later epochs rejected |
| Client send adjustment | Bounded search over 80 transforms; one checksum-valid plaintext |
| Messages | Opaque 52-byte server-list entries, explicit fixture response rules |
| Character lobby | Create, persist and reload a synthetic character; verified with the original Client |
| Map catalog | 257 map IDs / 516 sub-maps; 450 routes have matching geometry/marker prefixes, with one configurable entry builder |
| Initial world transition | Older lab Client enters Waterford and opens Inventory; vitals, lifecycle and gameplay remain incomplete |
| Authentication / playable world | Unimplemented; real credential validation and gameplay remain unverified |

The cipher implementation is grounded in the selected Client's static code.
RC6 vectors validate the primitive, and differential/basis checks validate the
recovered payload maps. They do not replace an original-Client trace. This is a
research server with a local JSON character store. It has no production database,
world simulation or admin interface yet.

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

The Windows session fixes the retired launcher update dependency and establishes
initial encrypted game/fixture exchange and TCP handoff. Subsequent key rotation, authentication,
remaining message schemas and M4's full acceptance remain pending.
