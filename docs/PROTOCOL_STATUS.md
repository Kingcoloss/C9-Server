# Protocol implementation evidence

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
