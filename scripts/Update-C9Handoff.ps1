[CmdletBinding()]
param()

# Update only while the Client is back at Choose World. Keep login alive.
$ErrorActionPreference = 'Stop'
$projectDirectory = Split-Path -Parent $PSScriptRoot
$recordPath = Join-Path $projectDirectory 'artifacts\lab-processes.local.json'
$record = Get-Content -Raw -Encoding UTF8 -LiteralPath $recordPath | ConvertFrom-Json
$entry = $record.handoff_probe
if (-not $entry -or $entry.profile -ne 'character_lobby') { throw 'Start the character lobby lab first.' }
if (@(Get-NetTCPConnection -State Established -LocalPort 14001 -ErrorAction SilentlyContinue).Count) {
    throw 'Return the Client to Choose World before updating the character service.'
}
$oldAssembly = if ($entry.assembly_path) { [IO.Path]::GetFullPath($entry.assembly_path) } else { Join-Path $projectDirectory 'artifacts\server\C9.Server.dll' }
$artifactRoot = [IO.Path]::GetFullPath((Join-Path $projectDirectory 'artifacts')) + [IO.Path]::DirectorySeparatorChar
if (-not $oldAssembly.StartsWith($artifactRoot, [StringComparison]::OrdinalIgnoreCase) -or
    [IO.Path]::GetFileName($oldAssembly) -ne 'C9.Server.dll') { throw 'The recorded assembly is outside lab artifacts.' }
$old = Get-Process -Id $entry.pid -ErrorAction Stop
$command = (Get-CimInstance Win32_Process -Filter "ProcessId=$($old.Id)").CommandLine
if ($old.StartTime.ToUniversalTime().Ticks -ne ([datetime]$entry.start_time_utc).ToUniversalTime().Ticks -or
    -not $command -or $command.IndexOf($oldAssembly, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
    throw 'The recorded character service no longer matches; it was left running.'
}
$dotnetCommand = Get-Command dotnet.exe -ErrorAction SilentlyContinue
$dotnet = if ($dotnetCommand) { $dotnetCommand.Source } else { Join-Path $env:USERPROFILE '.dotnet\dotnet.exe' }
$buildDirectory = Join-Path $projectDirectory ('artifacts\handoff-builds\' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
& $dotnet publish (Join-Path $projectDirectory 'src\C9.Server\C9.Server.csproj') -c Release -o $buildDirectory --disable-build-servers -m:1 -p:UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Build failed; the running service was left intact.' }
if (@(Get-NetTCPConnection -State Established -LocalPort 14001 -ErrorAction SilentlyContinue).Count) {
    throw 'The Client connected during the build; the running service was left intact.'
}
# Verify identity again after the build, before stopping just this process.
$old.Refresh()
if ($old.HasExited -or $old.StartTime.ToUniversalTime().Ticks -ne ([datetime]$entry.start_time_utc).ToUniversalTime().Ticks) {
    throw 'The character service changed during the build.'
}
Stop-Process -Id $old.Id
$assembly = Join-Path $buildDirectory 'C9.Server.dll'
$config = Join-Path $projectDirectory 'config\lab-character-lobby.json'
$logs = Join-Path $projectDirectory 'artifacts\logs'
$started = Start-Process -FilePath $dotnet -ArgumentList ('"{0}" --config "{1}" --port 14001' -f $assembly, $config) `
    -WorkingDirectory $projectDirectory -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput (Join-Path $logs 'handoff-probe.stdout.log') `
    -RedirectStandardError (Join-Path $logs 'handoff-probe.stderr.log')
$ready = $false
for ($attempt = 0; $attempt -lt 40; $attempt++) {
    $started.Refresh()
    if ($started.HasExited) { throw 'The updated character service exited; inspect its error log.' }
    $listener = @(Get-NetTCPConnection -State Listen -LocalPort 14001 -ErrorAction SilentlyContinue |
        Where-Object { $_.OwningProcess -eq $started.Id -and $_.LocalAddress -eq '127.0.0.1' })
    if ($listener.Count) { $ready = $true; break }
    Start-Sleep -Milliseconds 250
}
if (-not $ready) { throw 'The updated character service did not become ready.' }
$record.handoff_probe = [pscustomobject]@{
    pid = $started.Id; start_time_utc = $started.StartTime.ToUniversalTime().ToString('o')
    port = 14001; profile = 'character_lobby'; assembly_path = $assembly
}
$record | ConvertTo-Json -Depth 4 | Set-Content -Encoding UTF8 -LiteralPath $recordPath
Write-Host 'Character service updated. Connect to C9 Local again; the login service was kept running.'
