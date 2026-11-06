[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$projectDirectory = Split-Path -Parent $PSScriptRoot
$recordPath = Join-Path $projectDirectory 'artifacts\lab-processes.local.json'
if (-not (Test-Path -LiteralPath $recordPath)) { Write-Host 'No recorded lab processes.'; return }
$record = Get-Content -Raw -Encoding UTF8 -LiteralPath $recordPath | ConvertFrom-Json
$serverDll = Join-Path $projectDirectory 'artifacts\server\C9.Server.dll'
foreach ($name in @('launcher_update', 'game_probe', 'handoff_probe')) {
    $entry = $record.$name
    if (-not $entry) { continue }
    $endpointDll = $serverDll
    if ($entry.assembly_path) {
        $endpointDll = [IO.Path]::GetFullPath($entry.assembly_path)
        $artifactRoot = [IO.Path]::GetFullPath((Join-Path $projectDirectory 'artifacts')) + [IO.Path]::DirectorySeparatorChar
        if (-not $endpointDll.StartsWith($artifactRoot, [StringComparison]::OrdinalIgnoreCase) -or
            [IO.Path]::GetFileName($endpointDll) -ne 'C9.Server.dll') {
            Write-Warning "The recorded $name assembly is outside the lab; it was left running."
            continue
        }
    }
    $process = Get-Process -Id $entry.pid -ErrorAction SilentlyContinue
    if (-not $process) { continue }
    $command = (Get-CimInstance Win32_Process -Filter "ProcessId=$($process.Id)").CommandLine
    $recordedStartTicks = ([datetime]$entry.start_time_utc).ToUniversalTime().Ticks
    if ($process.StartTime.ToUniversalTime().Ticks -ne $recordedStartTicks -or
        -not $command -or $command.IndexOf($endpointDll, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        Write-Warning "The recorded $name process no longer matches; it was left running."
        continue
    }
    Stop-Process -Id $process.Id
    Write-Host "Stopped $name (process $($process.Id))."
}
