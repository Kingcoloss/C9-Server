# ADR 0001: Stack for the deployable C9 server

Date: 2026-11-02 (Asia/Bangkok)
Status: Accepted for the C#/.NET 10 server runtime; the owner authorized the port.
Database/deployment choices remain planned work. The migrated runtime's current
validation and remaining protocol gaps are recorded in the Phase 5 port report.

## Context

The intended deliverable is a server that communicates with the original Client
and can operate with persistent player data. The Python implementation currently
provides a protocol reference and synthetic TCP fixture scenario. Its passing
functional tests establish neither original-Client compatibility nor capacity.
The current Mac has .NET SDK 10.0.302 installed.

## Baseline

| Layer | Choice / scope |
|---|---|
| Server language/runtime | C# / .NET 10 LTS |
| Network | System.Net.Sockets and System.IO.Pipelines for the recovered TCP framing; keep a transport interface for other paths established by evidence |
| Binary codec | Explicit widths/endianness, bounded buffers and checked parsing; unchecked uint32 arithmetic only where the recovered algorithm requires it |
| Legacy cipher compatibility | Managed RIPEMD160/RC6 implemented for the initial epoch; verify additional implementations per primitive and OS as rotation is added |
| Process architecture | One deployable host with separate protocol, session, auth, lobby, world and persistence modules; introduce a separate process only when protocol, isolation or measured load requires it |
| Persistence | PostgreSQL 18 as the proposed database; evaluate Npgsql and transaction/migration design when persistence is introduced |
| Development | macOS ARM64 |
| Deployment target | Linux x64 first, subject to codec/transport dependencies passing on the target OS; deployment artifacts and database are not created by this note |
| RE and reference verification | Existing Python/Ghidra scripts, fixtures and validation records |
| Operations | Configuration, bounded queues/backpressure, structured logs, metrics, graceful shutdown and tested database recovery |

.NET 10 is an active LTS release supported through November 2028. Keep the chosen
runtime and dependencies patched; the installed SDK version is environment
evidence rather than a permanent production patch pin.

## Rationale and alternatives

.NET provides a practical single server language for protocol work, persistent
services and subsequent operational APIs. System.IO.Pipelines directly addresses
buffering and parsing streaming input. This is an engineering choice, not a
benchmark claim that .NET is fastest or sufficient for an unspecified player count.

Rust remains an option if measured memory/latency requirements justify its
ownership/concurrency engineering cost. Go is also viable; no protocol requirement
currently forces its selection. Keep one primary server runtime initially so
cross-language deployment and debugging do not expand ahead of evidence.

Python remains useful as a reference for byte-level comparisons. Cross-language
agreement must be supplemented with independent primitive vectors and actual
original-Client wire/plaintext fixtures; matching two implementations can preserve
the same mistaken specification.

## Work order and acceptance

1. The .NET solution and initial codec/session runtime have been ported. Preserve
   byte-for-byte differential tests and complete TCP/target-OS validation; the
   current environment denies outbound loopback connections.
2. Complete subsequent key rotation and required transport/auth schemas. The
   modulo-80 search is a research fallback to replace with an evidenced rule or
   a measured and explicitly accepted bounded strategy for deployment.
3. After the owner permits opening the game, validate login, server list and
   handoff with the original Client before calling the server compatible.
4. Add persistent characters, inventory and world behavior using verified flows
   and explicit server-side rules. Client RE does not provide all authoritative
   server logic, economy, combat formulas, AI or transaction semantics.
5. Set concurrent-player, packet-rate and tick/latency budgets, then test load,
   long sessions across key rotations, recovery and deployment on the target OS.

Keep game, Launcher and Patcher closed until the owner's explicit authorization.
This constraint does not prevent static RE, .NET implementation or synthetic tests.

## Sources

- [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
- [System.IO.Pipelines](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines)
- [PostgreSQL versioning/support policy](https://www.postgresql.org/support/versioning/)
- Project evidence and current scope: `re/C9_RE_Phase4_Server_TH.md` and `docs/PROTOCOL_STATUS.md`.
