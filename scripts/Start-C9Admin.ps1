[CmdletBinding()]
param(
    [ValidateRange(1, 65535)][int]$Port = 14880,
    [string]$ClientDirectory
)

$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
if ($Port -in 14000, 14001) { throw 'Use a separate port for the Admin Console.' }
if (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue) { throw "Console port $Port is already in use." }
$dotnetCandidates = @((Join-Path $env:USERPROFILE '.dotnet\dotnet.exe')) +
    @(Get-Command dotnet.exe -All -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
$dotnet = $null
foreach ($candidate in $dotnetCandidates) {
    if ((Test-Path -LiteralPath $candidate -PathType Leaf) -and (& $candidate --list-sdks | Where-Object { $_ -match '^10\.' })) {
        $dotnet = $candidate
        break
    }
}
if (-not $dotnet) { throw 'Install the .NET 10 SDK before starting the Admin Console.' }
$state = Join-Path $workspace 'artifacts\admin-console'
$gameDirectory = Join-Path $state 'game'
$appDirectory = Join-Path $state 'app'

& $dotnet publish (Join-Path $workspace 'src\C9.Server\C9.Server.csproj') -c Release -o $gameDirectory --nologo
if ($LASTEXITCODE -ne 0) { throw 'Game server build failed.' }
& $dotnet publish (Join-Path $workspace 'src\C9.Admin\C9.Admin.csproj') -c Release -o $appDirectory --nologo
if ($LASTEXITCODE -ne 0) { throw 'Admin Console build failed.' }

$options = @('--workspace', $workspace, '--port', "$Port", '--dotnet-host', $dotnet)
if ($ClientDirectory) { $options += @('--client-dir', (Resolve-Path -LiteralPath $ClientDirectory).Path) }
Write-Host "Admin Console: http://127.0.0.1:$Port/"
Write-Host "Access key file: $(Join-Path $state 'access-key.local.txt')"
Push-Location -LiteralPath $appDirectory
try {
    & $dotnet (Join-Path $appDirectory 'C9.Admin.dll') @options
    if ($LASTEXITCODE -ne 0) { throw 'Admin Console exited with an error.' }
} finally { Pop-Location }
