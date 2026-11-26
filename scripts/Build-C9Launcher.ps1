[CmdletBinding()]
param(
    [ValidateSet('win-arm64', 'win-x64')][string]$RuntimeIdentifier,
    [switch]$Installer,
    [switch]$Launch
)

# Publishes the MAUI Client Launcher to artifacts/launcher/<runtime> in the compact layout:
#   L9.exe          small starter (src/C9.LocalLauncher.Package/L9Starter.cs)
#   L9.local.json   local configuration and L9.*.log files (kept across rebuilds)
#   bin\            the MAUI launcher (bin\L9.exe) and all runtime files
# -Installer also builds artifacts/launcher/L9-Setup-<version>-<runtime>.exe with Inno Setup 6.
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
$runtime = Join-Path $output 'bin'
$project = Join-Path $workspace 'src/C9.LocalLauncher.Maui/C9.LocalLauncher.Maui.csproj'
$package = Join-Path $workspace 'src/C9.LocalLauncher.Package'
Push-Location -LiteralPath $workspace
try {
    & $dotnet publish $project -c Release -p:RuntimeIdentifierOverride=$RuntimeIdentifier -o $runtime --nologo
    if ($LASTEXITCODE -ne 0) { throw 'C9 Local Launcher build failed. Check that the maui-windows workload is installed.' }
} finally { Pop-Location }
if (-not (Test-Path -LiteralPath (Join-Path $runtime 'L9.exe') -PathType Leaf)) { throw "Expected launcher not found: $runtime\L9.exe" }

# Older builds published flat into $output: remove top-level copies of runtime files (config and logs stay).
Get-ChildItem -LiteralPath $runtime -Force | Where-Object Name -ne 'L9.exe' | ForEach-Object {
    $stale = Join-Path $output $_.Name
    if (Test-Path -LiteralPath $stale) { Remove-Item -LiteralPath $stale -Recurse -Force }
}

# Starter L9.exe, compiled with the .NET Framework 4.x compiler that ships with Windows; uses the launcher icon.
$csc = @('Framework64', 'Framework') | ForEach-Object { Join-Path $env:WINDIR "Microsoft.NET/$_/v4.0.30319/csc.exe" } |
    Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
if (-not $csc) { throw 'The .NET Framework 4.x compiler (csc.exe) was not found in Windows.' }
$starter = Join-Path $output 'L9.exe'
$icon = Join-Path ([IO.Path]::GetTempPath()) "L9-starter-$PID.ico"
try {
    Add-Type -AssemblyName System.Drawing
    $stream = [IO.File]::Create($icon)
    try { [Drawing.Icon]::ExtractAssociatedIcon((Join-Path $runtime 'L9.exe')).Save($stream) } finally { $stream.Dispose() }
    & $csc /nologo /target:winexe /optimize+ /codepage:65001 /r:System.Windows.Forms.dll "/win32icon:$icon" "/out:$starter" (Join-Path $package 'L9Starter.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Building the starter L9.exe failed.' }
} finally { Remove-Item -LiteralPath $icon -ErrorAction SilentlyContinue }
Write-Host "C9 Local Launcher $starter (runtime in $runtime)"

if ($Installer) {
    $iscc = @((Get-Command ISCC.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source),
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6/ISCC.exe'),
        (Join-Path $env:LOCALAPPDATA 'Programs/Inno Setup 6/ISCC.exe')) |
        Where-Object { $_ -and (Test-Path -LiteralPath $_ -PathType Leaf) } | Select-Object -First 1
    if (-not $iscc) { throw 'Install Inno Setup 6 (winget install JRSoftware.InnoSetup) to build the installer.' }
    $version = ([xml](Get-Content -LiteralPath $project -Raw)).Project.PropertyGroup.ApplicationDisplayVersion | Where-Object { $_ } | Select-Object -First 1
    $architecture = if ($RuntimeIdentifier -eq 'win-arm64') { 'arm64' } else { 'x64compatible' }
    $setupName = "L9-Setup-$version-$RuntimeIdentifier"
    & $iscc /Qp "/DSourceDir=$output" "/DAppVersion=$version" "/DArch=$architecture" "/DSetupName=$setupName" `
        "/O$(Join-Path $workspace 'artifacts/launcher')" (Join-Path $package 'L9Setup.iss')
    if ($LASTEXITCODE -ne 0) { throw 'Building the L9 installer failed.' }
    Write-Host "C9 Local Launcher installer $(Join-Path $workspace "artifacts/launcher/$setupName.exe")"
}
if ($Launch) { Start-Process -FilePath $starter -WorkingDirectory $output }
