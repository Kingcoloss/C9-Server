[CmdletBinding()]
param(
    [ValidateRange(1, 65535)][int]$Port = 14000,
    [ValidateRange(10, 600)][int]$Seconds = 180,
    [switch]$IncludeHandoff
)

$ErrorActionPreference = 'Stop'
$projectDirectory = Split-Path -Parent $PSScriptRoot
$captureTool = Join-Path $env:ProgramFiles 'Wireshark\dumpcap.exe'
if (-not (Test-Path -LiteralPath $captureTool)) {
    $available = Get-Command dumpcap.exe -ErrorAction SilentlyContinue
    if (-not $available) { throw 'Wireshark dumpcap.exe was not found.' }
    $captureTool = $available.Source
}
$interfaces = & $captureTool -D 2>&1 | Out-String
if ($LASTEXITCODE -ne 0 -or $interfaces -notmatch [regex]::Escape('\Device\NPF_Loopback')) {
    throw 'Npcap loopback capture is not available. No network settings were changed.'
}
$directory = Join-Path $projectDirectory 'artifacts\captures'
New-Item -ItemType Directory -Path $directory -Force | Out-Null
$stem = Join-Path $directory ('c9-login-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
$captureFile = $stem + '.pcapng'
$portFilter = if ($IncludeHandoff) { "(tcp port $Port or tcp port 14001)" } else { "tcp port $Port" }
$filter = "$portFilter and host 127.0.0.1"
$arguments = '-i "\Device\NPF_Loopback" -f "{0}" -s 8192 -c 512 -a duration:{1} -a filesize:1024 -w "{2}"' -f $filter, $Seconds, $captureFile
$captureProcess = Start-Process -FilePath $captureTool -ArgumentList $arguments -WindowStyle Hidden `
    -RedirectStandardOutput ($stem + '.stdout.log') -RedirectStandardError ($stem + '.stderr.log') -PassThru
for ($attempt = 0; $attempt -lt 20; $attempt++) {
    $captureProcess.Refresh()
    if ($captureProcess.HasExited) { throw "Capture exited. Inspect $stem.stderr.log" }
    if (Test-Path -LiteralPath $captureFile) { break }
    Start-Sleep -Milliseconds 250
}
if (-not (Test-Path -LiteralPath $captureFile)) { throw 'Capture did not create its output file.' }
$record = [ordered]@{
    pid = $captureProcess.Id
    start_time_utc = $captureProcess.StartTime.ToUniversalTime().ToString('o')
    interface = '\Device\NPF_Loopback'
    filter = $filter
    duration_seconds = $Seconds
    maximum_packets = 512
    maximum_file_kib = 1024
    file = $captureFile
    stderr = $stem + '.stderr.log'
}
$record | ConvertTo-Json | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $projectDirectory 'artifacts\capture.local.json')
Write-Host "Capturing $filter for up to $Seconds seconds: $captureFile"
