#!/usr/bin/env python3
"""Run the installed Ghidra on a separate research project with scoped caches."""
import argparse
from pathlib import Path
import subprocess
import tempfile
from c9_paths import resolve_client_file


def main():
    root = Path(__file__).resolve().parents[2]
    parser = argparse.ArgumentParser()
    parser.add_argument('--target', default='C9.exe')
    parser.add_argument('--client-dir', type=Path, help='Client directory override; otherwise use C9_CLIENT_DIR or research.local.json')
    parser.add_argument('--output', type=Path, default=root / 're/evidence/ghidra')
    parser.add_argument('--project', default='C9_RE_Phase1')
    parser.add_argument('--ghidra', type=Path, default=root / 'ghidra_12.2_PUBLIC')
    parser.add_argument('--java', type=Path, default=Path('/Library/Java/JavaVirtualMachines/jdk-25.jdk/Contents/Home/bin/java'))
    parser.add_argument('--timeout', type=int, default=180)
    parser.add_argument('--process', action='store_true', help='Use the already imported program')
    parser.add_argument('--no-analysis', action='store_true')
    parser.add_argument('--functions', nargs='+', help='Export only the given function addresses')
    parser.add_argument('--recover-functions', action='store_true', help='Define missing functions at explicitly selected addresses in the research database')
    parser.add_argument('--function-range', nargs=2, metavar=('START', 'END'), help='Export defined functions in a bounded address range')
    parser.add_argument('--export-prefix', default='packet_framing')
    args = parser.parse_args()
    out = args.output.resolve()
    (out / 'projects').mkdir(parents=True, exist_ok=True)
    cache = Path(tempfile.gettempdir()) / ('c9-re-' + args.project)
    command = [str(args.java), '-Xmx2G', '-Xshare:off', '-Djava.awt.headless=true',
               '-Djava.system.class.loader=ghidra.GhidraClassLoader',
               f'-Dapplication.settingsdir={cache}/settings',
               f'-Dapplication.cachedir={cache}/cache', f'-Dapplication.tempdir={cache}/temp',
               '--enable-native-access=ALL-UNNAMED', '--sun-misc-unsafe-memory-access=allow',
               '-cp', str(args.ghidra.resolve() / 'Ghidra/Framework/Utility/lib/Utility.jar'),
               'ghidra.Ghidra', 'ghidra.app.util.headless.AnalyzeHeadless',
               str(out / 'projects'), args.project]
    if args.process:
        command += ['-process', args.target]
    else:
        try:
            target = resolve_client_file(args.target, args.client_dir, root)
        except ValueError as error:
            parser.error(str(error))
        command += ['-import', str(target)]
    if args.no_analysis:
        command += ['-noanalysis']
    command += ['-analysisTimeoutPerFile', str(args.timeout), '-max-cpu', '2',
                '-scriptPath', str(root / 're/scripts'), '-postScript']
    if args.functions and args.function_range:
        parser.error('Choose --functions or --function-range')
    if args.function_range:
        command += ['ExportC9Range.java', str(out / args.export_prefix)] + args.function_range
    elif args.functions:
        command += ['RecoverC9Functions.java' if args.recover_functions else 'ExportC9Functions.java', str(out / args.export_prefix)] + args.functions
    else:
        command += ['ExportC9Network.java', str(out)]
        seeds = out / 'seed_strings.tsv'
        if seeds.exists(): command.append(str(seeds))
    command += ['-log', str(out / 'analysis.log'), '-scriptlog', str(out / 'script.log')]
    raise SystemExit(subprocess.call(command, cwd=root))


if __name__ == '__main__':
    main()
