# Protocol implementation evidence

5 November 2026 character lobby: the original Client created `Lab`, a level-1
Fighter in slot 0, and loaded its name, class and appearance after the server
restarted. Capture validation confirms `0BBB`/`0BBC` and `0EFB`/`0EFC` exchanges.
The creation request carries template ID 0 for Fighter; its asset job code is 1.
The list must use the unsigned 64-bit -1 sentinel for an active character's
deletion time. `2EEA`/`2EEB` returns a populated character list to Choose World.
The full .NET suite passes 40/40 cases (2,976 assertions).
See [character lobby notes](WINDOWS_CHARACTER_LOBBY_TH.md).

6 November 2026: `0BC3` payload +4 is a zero-based sub-map index, loaded into
Player +0x2320 and checked by 004B5890 over 0x88-byte vector elements. Waterford
has only index 0 (`_VILLAGE`); sending 1 caused the former Map Key dialog.
With index 0 the Client sends `ED1D` (nine requests), `0E4B`, `0BC4`, plain `0EFD`
and `0EB1`, then exits with Windows access violation `0xC0000005`.
The complete validated capture metadata is in
`re/evidence/static/windows_world_zero_submap_validation.json`.
Later the same day, native exception monitoring recovered guest x86 fault
0078F5BF in CharacterLODState. The class DWORD at `0BC0` payload +12 was zero,
so 00DA42DF skipped actor creation. Sending Fighter class 1 lets `Lab` enter
Waterford Village, with C9StageGame active and Inventory visible. Capture
metadata is in `windows_waterford_entry_validation.json`; the sanitized crash
chain is in `windows_world_crash_analysis.json`. Inventory capacity and stats
were still zero, and the session later ended with connection lost.
Gameplay systems and lifecycle handling remain incomplete.

The runtime asset survey covers all 257 map IDs, 136 folders and 516 sub-map
records. All 1,887 MapData CFS indices (22,466 entries) validate; matching geometry
and marker prefixes are available for 450 routes. The configurable entry builder
uses this embedded catalog and rejects unknown/missing routes before startup.
See [the complete map report](MAP_CATALOG_TH.md). The .NET suite passes 41/41
cases with 6,226 assertions; the bounded CFS reader passes six additional tests.

5 November 2026: the selected launcher reads the installed `version.ini` after
applying the reversible executable patch. PLAY reached the original game's Log In
screen with the HTTP updater stopped. See [offline launcher notes](WINDOWS_LAUNCHER_TH.md).

4 November 2026 resume: .NET full validation passes 39/39 (2,919 assertions),
Python protocol/server passes 19/19. A separate greeting-only `observer`
profile accepts interleaved valid packets without application replies; malformed
wire frames still fail validation. See [resume notes](WINDOWS_RESUME_20261104_TH.md).

Windows session update (3 November 2026): the owner authorized opening the game.
The .NET full suite passes 38/38 cases on Windows ARM64, including TCP; the
launcher now downloads its local version manifest and shows
`Patch success complete.`. See [Windows session notes](WINDOWS_LAUNCHER_TH.md)
and `re/evidence/static/windows_launcher_validation.json`. This supersedes the
historical environment/authorization limitations below for this session.
The selected game has exchanged initial encrypted login/list frames with the
synthetic profile, displayed C9 Local / Good, and followed a synthetic TCP
handoff to loopback 14001. A mode-1 greeting there produces the expected
49-byte 1771 request with fresh sequence 0. Authentication semantics, the game-endpoint reply
and subsequent cipher epochs remain unverified. See
[Windows handshake evidence](WINDOWS_HANDSHAKE_TH.md).

Afternoon continuation: a reversible, hash-locked two-byte promotional-dialog
fix in PhysXLoader.dll and the recovered SERVER/IP1/PORT1 manifest fields let
the Client reach its Log In screen. C9.exe remains unchanged. See
[Client startup notes](WINDOWS_CLIENT_STARTUP_TH.md). Displaying the login UI
does not establish successful authentication or game-server interoperability.

Target SHA-256:
`b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`

Addresses below are preferred-image VAs for the selected PE32 Client.

| Implemented rule | Static evidence |
|---|---|
| Clear length/flag prefix, encoded body starts at +3 | 00563C10, 00563D00 |
| Header length/type/checksum/sequence offsets 0/3/5/9 | Same encode/decode paths |
| Signed-byte checksum, seed BD136D48, multiplier 03FAA1FF | 00564140, 006669D0, MOVSX in assembly |
| Checksum field is 100 while computing body checksum | 00563C10, 00563D00 |
| Registry keys 0..99; observed lookups use modulo 80 | 00410AE0, 00412130, 004121D0, 00412230 |
| Initial seed pointer/length | 00DCECB0, data 012FC7F4, length at 012FC818 = 32 |
| Hash selection and digest byte for factory | 00DD5470, recovered switch at 00DD5513 / table 00DD5A30 |
| Initial hash is RIPEMD160, digest byte 5 modulo 10 is 5 | Branch 7 at 00DD5784, vtable 011D93C4, RTTI, seed computation |
| Initial factory chooses RC6 | 00DD5070 selector 5 -> 00DD3000 -> vtable 011D9E2C |
| Initial key uses first 16 digest bytes; rotation threshold 256 | 00DD3060: key length 10h, object +4 = 100h |
| RC6 key schedule and block functions | 00E6F950, 00E6FB30, 00E6FC30 |
| Zero fill / separate count footer / 16-byte block wrapper | 00DD3990, 00DD30A0; return values checked in assembly |
| Independent direction state, table, counters | 00DCECB0, 00DCEEC0, 00DCEF50, 00DCED50 |
| Rotation after the 256th transformed frame | Counter initialized to 1, compared to property 100h |
| TCP sequence initialization and increment | 00563820, 00A38850, 00563E40 |
| Server list count/stride | 00A621F0 type 2EE6: count +13, entry +14, stride 34h |
| Opaque entry copy / lookup key | 00614D40 copies 13 DWORDs; 00A63630 compares DWORD at entry +1 |
| Entry display name | 00A621F0 passes entry +5 to 0045DFE0; WORD scan and WideCharToMultiByte at IAT 00F99308 establish UTF-16LE |
| Entry selection gate | 00B1D7CD / 00B1D7F1 read byte +33h; zero takes the error path, nonzero reaches 00A631B0; complete status enum remains unresolved |
| Server selection request | 00A64480 / 00A631B0 construct 2EEC with 20-byte payload; captured game request has key 1 at payload +8 and sequence 14 |
| Handoff TCP endpoint | 2EED reader 00A62E36..00A62E58 uses packet +1Ah/+1Eh; 0056386A..00563882 applies htons to the port and copies IPv4 bytes into sockaddr |
| Periodic 9999 request | 00A6B3F0 constructs decoded length 15h; 00A68740 sends after 3000 ms and writes GetTickCount plus a DWORD flag; captured payload is 8 bytes |

The fixed seed is 32 bytes. Its RIPEMD160 digest is
`6f749a998ed7373b8413712b6b3a201381850d31`; digest byte 5 is D7h,
selecting factory branch 5. Both initial directions use this seed. The server
never reads the Client PE at runtime.

Decompiler prototypes and stack variables around 00DD5470 are misleading.
In branch 7, the finalize destination resolves to the same buffer passed to
00DD5070. Assembly at 00DD57BD..00DD57EC accounts for pushes before LEA and
confirms selection at digest offset 5. The apparent `bStack_*` names alone are
insufficient evidence for the pointer relationship.

Client send path 00563C10 adds a per-type value to the payload selector. This
prototype tries the bounded 80-entry registry and requires one distinct
checksum-valid plaintext. Identical plaintext from several selectors counts
once. This is a research implementation strategy, not a recovered server
routine or proof of the original server's adjustment table.

## Validation boundaries

- `phase4_all_twisters.c/.asm.txt`: 160 statically recovered functions.
- `phase4_twisters_reference.c`: newly compiled C transcription with host-width
  pointer declarations and unsigned byte-shift storage.
- `build_twister_tables.py`: rejects non-bijective/non-affine mappings; checks
  zero/every basis bit, recovered inverse maps and seeded random inputs at
  boundaries. It does not execute the original PE.
- `phase4_twister_validation.json`: 67,520 differential/inverse basis checks,
  plus extraction basis vectors, for 80 selectors / 640 length branches.
- `tests/fixtures/rc6_kats.json`: six independent RC6 vectors from the
  [Crypto++ repository](https://github.com/weidai11/cryptopp/blob/master/TestData/rc6val.dat).
- `tests/test_server.py`: TCP fixture exchanges between newly written
  components, not captured traffic from the game.

## Pending

The server runtime is now C#/.NET 10 in `src/`; Python remains the RE/reference
implementation. Managed RIPEMD160 and RC6, embedded PacketTwister data, sequence,
configuration and session rules have been ported. See the Phase 5 port report.
The runtime can be checked with `--check-runtime` without Python, the Client
folder or any socket connection. The selected Client's frozen hash remains the
target; the relocated directory must be configured and rechecked before further
imports are attributed to it.

The current .NET socket tests encountered `SocketException (13): Permission
denied` on outbound loopback connect. Memory/stdio validation exercises the same
session processor, with socket-specific tests explicitly skipped. The earlier
Phase 4 Python TCP results do not establish that .NET TCP integration passed.

Subsequent cipher epochs, other cipher implementations, the full per-type
adjustment table, login/auth semantics, name encoding and entry layout, handoff,
BaseCat envelopes, character/world protocols and original-Client runtime
validation remain pending. Fixture greeting/replies are constructed test data
and do not prove successful game login. Opening the original game requires the
owner's explicit authorization.
