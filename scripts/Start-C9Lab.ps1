[CmdletBinding()]
param(
    [string]$ClientDirectory,
    [ValidateRange(1, 65535)][int]$UpdatePort = 18080,
    [ValidateRange(1, 65535)][int]$GamePort = 14000,
    [ValidateSet('probe', 'fixture')][string]$GameProfile = 'probe',
    [string]$HandoffConfig,
    [switch]$OfflineLauncher,
    [switch]$OpenLauncher
)

$ErrorActionPreference = 'Stop'
$projectDirectory = Split-Path -Parent $PSScriptRoot
if (-not $ClientDirectory) {
    $localConfig = Join-Path $projectDirectory 'config\research.local.json'
    if (-not (Test-Path -LiteralPath $localConfig)) {
        throw 'Pass -ClientDirectory with the installed C9 folder, or configure config/research.local.json.'
    }
    $ClientDirectory = (Get-Content -Raw -Encoding UTF8 -LiteralPath $localConfig | ConvertFrom-Json).client_directory
}
$ClientDirectory = (Resolve-Path -LiteralPath $ClientDirectory).Path
$launcher = Join-Path $ClientDirectory 'C9Launcher.exe'
$localInfo = Join-Path $ClientDirectory 'localinfo.dat'
if (-not (Test-Path -LiteralPath $launcher) -or -not (Test-Path -LiteralPath $localInfo)) {
    throw 'The Client folder must contain C9Launcher.exe and localinfo.dat.'
}
if (-not $OfflineLauncher -and $UpdatePort -eq $GamePort) { throw 'UpdatePort and GamePort must differ.' }
$handoffPort = 14001
if ($GameProfile -eq 'fixture' -and ($GamePort -eq $handoffPort -or (-not $OfflineLauncher -and $UpdatePort -eq $handoffPort))) {
    throw 'The fixture reserves port 14001 for the handoff endpoint.'
}
if ($HandoffConfig -and $GameProfile -ne 'fixture') { throw 'HandoffConfig requires -GameProfile fixture.' }
if ($GameProfile -eq 'fixture') {
    if (-not $HandoffConfig) { $HandoffConfig = Join-Path $projectDirectory 'config\lab-handoff-probe.json' }
    $HandoffConfig = (Resolve-Path -LiteralPath $HandoffConfig).Path
    $handoffProfile = (Get-Content -Raw -Encoding UTF8 -LiteralPath $HandoffConfig | ConvertFrom-Json).profile
}

$dotnetCommand = Get-Command dotnet.exe -ErrorAction SilentlyContinue
$dotnet = if ($dotnetCommand) { $dotnetCommand.Source } else { Join-Path $env:USERPROFILE '.dotnet\dotnet.exe' }
if (-not (Test-Path -LiteralPath $dotnet)) { throw 'Install the .NET 10 SDK before starting the lab.' }
$serverDirectory = Join-Path $projectDirectory 'artifacts\server'
$serverDll = Join-Path $serverDirectory 'C9.Server.dll'
if (-not (Test-Path -LiteralPath $serverDll)) {
    Push-Location $projectDirectory
    try {
        & $dotnet publish src/C9.Server/C9.Server.csproj -c Release -o $serverDirectory --disable-build-servers -m:1 -p:UseSharedCompilation=false
        if ($LASTEXITCODE -ne 0) { throw 'Server publish failed.' }
    } finally { Pop-Location }
}
$handoffDll = $serverDll
$existingRecordPath = Join-Path $projectDirectory 'artifacts\lab-processes.local.json'
if (Test-Path -LiteralPath $existingRecordPath) {
    $existingRecord = Get-Content -Raw -Encoding UTF8 -LiteralPath $existingRecordPath | ConvertFrom-Json
    if ($existingRecord.handoff_probe.assembly_path) {
        $candidate = [IO.Path]::GetFullPath($existingRecord.handoff_probe.assembly_path)
        $artifactRoot = [IO.Path]::GetFullPath((Join-Path $projectDirectory 'artifacts')) + [IO.Path]::DirectorySeparatorChar
        if (-not $candidate.StartsWith($artifactRoot, [StringComparison]::OrdinalIgnoreCase) -or
            [IO.Path]::GetFileName($candidate) -ne 'C9.Server.dll' -or -not (Test-Path -LiteralPath $candidate)) {
            throw 'The recorded handoff assembly is outside the lab artifacts or is missing.'
        }
        $handoffDll = $candidate
    }
}
$logDirectory = Join-Path $projectDirectory 'artifacts\logs'
New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

function Start-LabEndpoint([string]$Name, [int]$Port, [string]$Options) {
    $endpointDll = if ($Name -eq 'handoff-probe') { $handoffDll } else { $serverDll }
    $listeners = @(Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue)
    if ($listeners.Count -gt 0) {
        $owners = @($listeners | Select-Object -ExpandProperty OwningProcess -Unique)
        if ($owners.Count -ne 1) { throw "Port $Port has more than one owner." }
        $existing = Get-CimInstance Win32_Process -Filter "ProcessId=$($owners[0])"
        $isUpdate = $existing.CommandLine -match '(?:^|\s)--launcher-update(?:\s|$)'
        if (-not $existing.CommandLine -or
            $existing.CommandLine.IndexOf($endpointDll, [StringComparison]::OrdinalIgnoreCase) -lt 0 -or
            ($Name -eq 'launcher-update') -ne $isUpdate -or
            @($listeners | Where-Object LocalAddress -ne '127.0.0.1').Count -gt 0) {
            throw "Port $Port belongs to another service. Its process was left running."
        }
        if ($Name -eq 'game-probe') {
            $usesConfig = $existing.CommandLine -match '(?:^|\s)--config(?:\s|$)'
            $fixturePath = Join-Path $projectDirectory 'config\lab-fixture.json'
            if (($GameProfile -eq 'fixture') -ne $usesConfig -or
                ($usesConfig -and $existing.CommandLine.IndexOf($fixturePath, [StringComparison]::OrdinalIgnoreCase) -lt 0)) {
                throw 'The running lab uses a different game profile. Stop it with Stop-C9Lab.ps1 before switching profiles.'
            }
        }
        if ($Name -eq 'handoff-probe' -and
            ($existing.CommandLine -notmatch '(?:^|\s)--config(?:\s|$)' -or
             $existing.CommandLine.IndexOf($HandoffConfig, [StringComparison]::OrdinalIgnoreCase) -lt 0)) {
            throw 'Port 14001 uses a different configuration. Stop the lab before switching configurations.'
        }
        return Get-Process -Id $owners[0]
    }
    $arguments = '"{0}" {1}' -f $endpointDll, $Options
    $started = Start-Process -FilePath $dotnet -ArgumentList $arguments -WorkingDirectory $projectDirectory -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $logDirectory "$Name.stdout.log") `
        -RedirectStandardError (Join-Path $logDirectory "$Name.stderr.log") -PassThru
    for ($attempt = 0; $attempt -lt 40; $attempt++) {
        if ($started.HasExited) { throw "$Name exited; inspect artifacts/logs/$Name.stderr.log." }
        $ready = @(Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue |
            Where-Object { $_.OwningProcess -eq $started.Id -and $_.LocalAddress -eq '127.0.0.1' })
        if ($ready.Count -gt 0) { return $started }
        Start-Sleep -Milliseconds 250
        $started.Refresh()
    }
    throw "$Name did not start listening; inspect artifacts/logs."
}

$recordPath = Join-Path $projectDirectory 'artifacts\lab-processes.local.json'
$versionBytes = [IO.File]::ReadAllBytes((Join-Path $ClientDirectory 'version.dat'))
if ($versionBytes.Length -ne 4) { throw 'version.dat must contain a 4-byte installed version.' }
$installedVersion = [BitConverter]::ToUInt32($versionBytes, 0)
$updateBase = "http://127.0.0.1:$UpdatePort/C9Update/EN/"
$updateEntry = $null
if ($OfflineLauncher) {
    $offlineHash = 'a90bfc010cbf189e3795fb6ed6266c02721165274ebdfa7dad0c93c4b9e38834'
    if ((Get-FileHash -LiteralPath $launcher -Algorithm SHA256).Hash -ne $offlineHash) {
        throw 'Apply re/scripts/patch_launcher_offline.py before using -OfflineLauncher.'
    }
    $manifest = [IO.File]::ReadAllText((Join-Path $ClientDirectory 'version.ini'))
    if (Test-Path -LiteralPath $recordPath) {
        $previous = Get-Content -Raw -Encoding UTF8 -LiteralPath $recordPath | ConvertFrom-Json
        if ($previous.launcher_update -and (Get-Process -Id $previous.launcher_update.pid -ErrorAction SilentlyContinue)) {
            $updateEntry = $previous.launcher_update
        }
    }
} else {
    $updateProcess = Start-LabEndpoint 'launcher-update' $UpdatePort ('--launcher-update --client-dir "{0}" --update-port {1} --login-port {2}' -f $ClientDirectory, $UpdatePort, $GamePort)
    $response = Invoke-WebRequest -Uri ($updateBase + 'version.ini') -UseBasicParsing -TimeoutSec 5
    $manifest = $response.Content
    if ($response.StatusCode -ne 200 -or $manifest -notmatch [regex]::Escape($updateBase)) {
        throw 'The update service does not match this Client folder.'
    }
    $updateEntry = @{ pid = $updateProcess.Id; start_time_utc = $updateProcess.StartTime.ToUniversalTime().ToString('o'); port = $UpdatePort }
}
if ($manifest -notmatch "(?m)^LatestVersion=$installedVersion\r?$" -or
    $manifest -notmatch "(?m)^PORT1=$GamePort\r?$" -or
    $manifest -notmatch '(?m)^IP1=127\.0\.0\.1\r?$') {
    throw 'The launcher manifest does not match the installed version and local game port.'
}
$gameOptions = "--port $GamePort"
$handoffProcess = $null
if ($GameProfile -eq 'fixture') {
    $handoffProcess = Start-LabEndpoint 'handoff-probe' $handoffPort ('--config "{0}" --port {1}' -f $HandoffConfig, $handoffPort)
    $gameOptions = '--config "{0}" --port {1}' -f (Join-Path $projectDirectory 'config\lab-fixture.json'), $GamePort
}
$gameProcess = Start-LabEndpoint 'game-probe' $GamePort $gameOptions

if (-not $OfflineLauncher) {
    $originalInfo = [IO.File]::ReadAllText($localInfo)
    $newInfo = $originalInfo
    foreach ($setting in @(@('VersionUrl', ($updateBase + 'version.ini')), @('MaintenanceUrl', ($updateBase + 'maintenance.ini')))) {
        $pattern = '(?im)^' + $setting[0] + '=[^\r\n]*'
        $line = $setting[0] + '=' + $setting[1]
        if ($newInfo -match $pattern) { $newInfo = [regex]::Replace($newInfo, $pattern, $line) }
        else { $newInfo = $newInfo.TrimEnd("`r", "`n") + "`r`n" + $line + "`r`n" }
    }
    if ($newInfo -ne $originalInfo) {
        $backupDirectory = Join-Path $projectDirectory ('artifacts\client-backups\' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
        New-Item -ItemType Directory -Path $backupDirectory | Out-Null
        Copy-Item -LiteralPath $localInfo -Destination (Join-Path $backupDirectory 'localinfo.dat')
        [IO.File]::WriteAllText($localInfo, $newInfo, [Text.Encoding]::ASCII)
        Write-Host "Original launcher configuration saved in $backupDirectory"
    }
}

$processRecord = [ordered]@{
    client_directory = $ClientDirectory
    game_probe = @{ pid = $gameProcess.Id; start_time_utc = $gameProcess.StartTime.ToUniversalTime().ToString('o'); port = $GamePort; profile = $GameProfile }
}
if ($updateEntry) { $processRecord.launcher_update = $updateEntry }
if ($handoffProcess) {
    $processRecord.handoff_probe = @{ pid = $handoffProcess.Id; start_time_utc = $handoffProcess.StartTime.ToUniversalTime().ToString('o'); port = $handoffPort; profile = $handoffProfile; assembly_path = $handoffDll }
}
$processRecord | ConvertTo-Json -Depth 4 | Set-Content -Encoding UTF8 -LiteralPath $recordPath
if ($OfflineLauncher) { Write-Host "Offline launcher ready: local version.ini (installed version $installedVersion)" }
else { Write-Host "Launcher update ready: $($updateBase)version.ini (installed version $installedVersion)" }
Write-Host "Game endpoint ready: 127.0.0.1:$GamePort (profile $GameProfile). Game authentication is still unverified."
if ($GameProfile -eq 'fixture') {
    Write-Host 'Synthetic greeting/login/server-list/handoff replies are enabled for local protocol testing.'
    Write-Host "Handoff endpoint ready: 127.0.0.1:$handoffPort (profile $handoffProfile)."
}
if ($OpenLauncher) {
    $running = @(Get-CimInstance Win32_Process -Filter "Name='C9Launcher.exe'" |
        Where-Object { $_.ExecutablePath -eq $launcher })
    if ($running.Count -eq 0) { Start-Process -FilePath $launcher -WorkingDirectory $ClientDirectory -WindowStyle Normal }
    else { Write-Host 'The C9 launcher is already running.' }
}
