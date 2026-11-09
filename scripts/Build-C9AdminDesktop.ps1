[CmdletBinding()]
param(
    [ValidateSet('win-arm64', 'win-x64')][string]$RuntimeIdentifier,
    [switch]$Launch
)

$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
if (-not $RuntimeIdentifier) {
    $RuntimeIdentifier = if ([Runtime.InteropServices.RuntimeInformation]::OSArchitecture -eq 'Arm64') { 'win-arm64' } else { 'win-x64' }
}
$candidates = @((Join-Path $env:USERPROFILE '.dotnet/dotnet.exe')) +
    @(Get-Command dotnet.exe -All -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
$dotnet = $candidates | Where-Object {
    (Test-Path -LiteralPath $_ -PathType Leaf) -and (& $_ --list-sdks | Where-Object { $_ -match '^10\.' })
} | Select-Object -First 1
if (-not $dotnet) { throw 'Install the .NET 10 SDK before building C9 Server Manager.' }

$output = Join-Path $workspace "artifacts/admin-desktop/$RuntimeIdentifier"
$project = Join-Path $workspace 'src/C9.AdminDesktop/C9.AdminDesktop.csproj'
Push-Location -LiteralPath $workspace
try {
    & $dotnet publish $project -c Release -p:RuntimeIdentifierOverride=$RuntimeIdentifier -o $output --nologo
    if ($LASTEXITCODE -ne 0) { throw 'C9 Server Manager build failed. Check that the maui-windows workload is installed.' }
    & $dotnet publish (Join-Path $workspace 'src/C9.Server/C9.Server.csproj') -c Release -o (Join-Path $output 'game') --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Game server build failed.' }
} finally { Pop-Location }
$executable = Join-Path $output 'C9.AdminDesktop.exe'
Write-Host "C9 Server Manager $executable"
if ($Launch) { Start-Process -FilePath $executable -WorkingDirectory $output }
