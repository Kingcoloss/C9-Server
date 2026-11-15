[CmdletBinding()]
param(
    [ValidateSet('win-arm64', 'win-x64')][string]$RuntimeIdentifier,
    [switch]$Launch
)

# Publishes the MAUI Client Launcher (L9.exe) to artifacts/launcher/<runtime>.
# Requires the .NET 10 SDK with the maui-windows workload (same as C9 Server Manager).
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
if (-not $dotnet) { throw 'Install the .NET 10 SDK before building the C9 Local Launcher.' }

$output = Join-Path $workspace "artifacts/launcher/$RuntimeIdentifier"
$project = Join-Path $workspace 'src/C9.LocalLauncher.Maui/C9.LocalLauncher.Maui.csproj'
Push-Location -LiteralPath $workspace
try {
    # L9.local.json and logs live beside L9.exe; keep them across rebuilds.
    & $dotnet publish $project -c Release -p:RuntimeIdentifierOverride=$RuntimeIdentifier -o $output --nologo
    if ($LASTEXITCODE -ne 0) { throw 'C9 Local Launcher build failed. Check that the maui-windows workload is installed.' }
} finally { Pop-Location }
$executable = Join-Path $output 'L9.exe'
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) { throw "Expected launcher not found: $executable" }
Write-Host "C9 Local Launcher $executable"
if ($Launch) { Start-Process -FilePath $executable -WorkingDirectory $output }
