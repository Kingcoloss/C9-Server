#!/usr/bin/env python3
"""Validate the .NET port against Python over loopback; never starts the game."""
import argparse
import asyncio
import hashlib
import json
import os
import re
import select
import signal
import subprocess
import sys
from contextlib import contextmanager
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT))
from c9server.protocol import ConnectionCodec, FrameSplitter, Packet
from c9server.messages import parse_server_list


def run(command, env):
    result = subprocess.run(command, cwd=ROOT, env=env, capture_output=True, text=True, timeout=45)
    if result.returncode:
        raise RuntimeError(f'Validation process failed ({result.returncode}):\n{result.stdout}\n{result.stderr}')
    return result


@contextmanager
def server_process(command, env, records, name):
    process = subprocess.Popen(command, cwd=ROOT, env=env, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    first = None
    try:
        if not select.select([process.stdout], [], [], 10)[0]:
            raise RuntimeError(f'{name} did not report startup')
        line = process.stdout.readline()
        if not line:
            raise RuntimeError(f'{name} exited before startup')
        first = json.loads(line)
        if first.get('event') != 'listening' or first.get('profile') != 'fixture':
            raise RuntimeError(f'{name} unexpected startup event')
        yield first['port']
    finally:
        if process.poll() is None:
            process.send_signal(signal.SIGINT)
        try:
            output, errors = process.communicate(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            process.communicate()
            raise RuntimeError(f'{name} did not stop within the deadline')
        if process.returncode or errors:
            raise RuntimeError(f'{name} exit={process.returncode}: {errors}')
        events = ([first] if first else []) + [json.loads(line) for line in output.splitlines() if line]
        records.append({'name': name, 'exit_code': process.returncode, 'server_stopped': True, 'events': events})


async def python_client(port):
    for coalesced in [False, True]:
        reader, writer = await asyncio.open_connection('127.0.0.1', port)
        codec = ConnectionCodec()

        async def receive():
            prefix = await asyncio.wait_for(reader.readexactly(2), 3)
            size = int.from_bytes(prefix, 'little')
            if not 13 <= size <= 8192:
                raise RuntimeError('Invalid .NET reply size')
            return codec.decode(prefix + await asyncio.wait_for(reader.readexactly(size - 2), 3))

        try:
            hello = await receive()
            assert hello.kind == 4 and hello.flag == 0 and hello.payload == b'\0'
            marker = b'PRIVATE-FIXTURE-DO-NOT-LOG'
            login = codec.encode(Packet(0x2EE1, marker + bytes(2162 - len(marker))))
            listing = codec.encode(Packet(0x2EE5))
            if coalesced:
                writer.write(login + listing)
                await writer.drain()
            else:
                for part in [login[:1], login[1:7], login[7:1024], login[1024:]]:
                    writer.write(part)
                    await writer.drain()
                    await asyncio.sleep(0)
            reply = await receive()
            assert reply.kind == 0x2EE2 and reply.sequence == 0 and len(reply.payload) == 20
            if not coalesced:
                writer.write(listing)
                await writer.drain()
            reply = await receive()
            entries, trailing = parse_server_list(reply)
            assert reply.sequence == 1 and len(entries) == 1 and trailing == b''
            assert int.from_bytes(entries[0][1:5], 'little') == 1
        finally:
            writer.close()
            await writer.wait_closed()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='/opt/homebrew/bin/dotnet')
    parser.add_argument('--server', type=Path, default=ROOT / 'src/C9.Server/bin/Release/net10.0/C9.Server.dll')
    parser.add_argument('--validation', type=Path, default=ROOT / 'tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll')
    parser.add_argument('--offline', action='store_true', help='Use memory/stdio validation without any socket connections')
    args = parser.parse_args()
    env = dict(os.environ, DOTNET_CLI_HOME='/tmp/c9-dotnet-cli', DOTNET_NOLOGO='1', DOTNET_CLI_TELEMETRY_OPTOUT='1')
    report = {'recorded_at_utc': datetime.now(timezone.utc).isoformat(),
              'original_game_executed': False, 'original_client_compatibility': 'unverified',
              'scope': 'Runtime port parity; authentication/rotation beyond initial epoch remain pending', 'processes': []}
    result = run([args.dotnet, str(args.validation)] + (['--offline'] if args.offline else []), env)
    report['dotnet_validation'] = json.loads(result.stdout.splitlines()[-1])
    assert report['dotnet_validation']['status'] == 'passed'
    if args.offline:
        codec = ConnectionCodec()
        requests = codec.encode(Packet(0x2EE1, bytes(2162))) + codec.encode(Packet(0x2EE5))
        process = subprocess.run([args.dotnet, str(args.validation), '--stdio-fixture'], input=requests,
                                 capture_output=True, cwd=ROOT, env=env, timeout=15)
        if process.returncode:
            raise RuntimeError(process.stderr.decode('utf-8', 'replace'))
        splitter = FrameSplitter(8192)
        replies = [codec.decode(wire) for wire in splitter.feed(process.stdout)]
        splitter.finish()
        assert [packet.kind for packet in replies] == [4, 0x2EE2, 0x2EE6]
        entries, trailing = parse_server_list(replies[-1])
        assert len(entries) == 1 and trailing == b''
        events = [json.loads(line) for line in process.stderr.decode().splitlines() if line]
        report['cross_language_stdio'] = {'python_client_to_dotnet_session': 'passed',
                                          'reply_types': ['0x0004', '0x2ee2', '0x2ee6'],
                                          'server_entries': len(entries), 'events': events, 'process_exit_code': process.returncode}
        report['tcp_validation'] = {'status': 'not_verified',
            'reason': 'Socket tests are omitted in explicit --offline mode',
            'network_tests_skipped': report['dotnet_validation']['skipped']}
    else:
        with server_process([args.dotnet, str(args.server), '--config', 'config/lab-fixture.json', '--port', '0'],
                            env, report['processes'], 'dotnet_server') as port:
            asyncio.run(python_client(port))
        with server_process([sys.executable, '-m', 'c9server', '--config', 'config/lab-fixture.json', '--port', '0'],
                            env, report['processes'], 'python_reference_server') as port:
            result = run([args.dotnet, str(args.validation), '--client', '127.0.0.1', str(port)], env)
            assert json.loads(result.stdout.splitlines()[-1])['event'] == 'interop_client_passed'
        report['cross_language_tcp'] = {'python_client_to_dotnet_server': 'passed',
                                         'dotnet_client_to_python_server': 'passed'}
    report['python_suites'] = []
    for folder, pattern in [('tests', 'test_protocol.py' if args.offline else 'test_*.py'), ('re/scripts', 'test_c9_protocol_offline.py'), ('re/scripts', 'test_c9_paths.py')]:
        result = run([sys.executable, '-m', 'unittest', 'discover', '-s', folder, '-p', pattern, '-v'], env)
        match = re.search(r'Ran (\d+) tests?', result.stderr)
        if not match:
            raise RuntimeError('Missing Python test count')
        report['python_suites'].append({'directory': folder, 'passed': int(match.group(1)), 'exit_code': result.returncode})
    fixture = ROOT / 'tests/fixtures/python_port_vectors.json'
    report['reference_fixture_sha256'] = hashlib.sha256(fixture.read_bytes()).hexdigest()
    report['runtime_python_dependency'] = False
    destination = ROOT / 're/evidence/static/phase5_dotnet_validation.json'
    destination.write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps({'dotnet': report['dotnet_validation'], 'cross_language': report.get('cross_language_tcp', report.get('cross_language_stdio', {}).get('python_client_to_dotnet_session')),
                      'python_suites': report['python_suites'], 'all_server_processes_stopped': True}))


if __name__ == '__main__':
    main()
