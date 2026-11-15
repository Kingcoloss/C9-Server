[CmdletBinding()]
param(
    [ValidateSet('All', 'Server', 'Client')][string]$Target = 'All',
    [ValidateSet('win-arm64', 'win-x64')][string]$RuntimeIdentifier,
    [ValidateSet('WinForms', 'Maui')][string]$ServerUi = 'WinForms',
    [switch]$FrameworkDependent,
    [switch]$Launch,
    [switch]$NoZip
)

$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
if ($Launch -and $Target -eq 'All') { throw 'Use -Target Server or -Target Client with -Launch.' }
if ($Launch -and $Target -eq 'Client') { throw 'Install the client release in the game folder using install_local_launcher.py before launching it.' }
if (-not $RuntimeIdentifier) {
    $architecture = [Runtime.InteropServices.RuntimeInformation]::OSArchitecture
    $RuntimeIdentifier = switch ($architecture.ToString()) {
        'Arm64' { 'win-arm64' }
        'X64' { 'win-x64' }
        default { throw "Unsupported Windows architecture: $architecture" }
    }
}

$candidates = @((Join-Path $env:USERPROFILE '.dotnet/dotnet.exe')) +
    @(Get-Command dotnet.exe -All -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
$dotnet = $candidates | Where-Object {
    (Test-Path -LiteralPath $_ -PathType Leaf) -and (& $_ --list-sdks | Where-Object { $_ -match '^10\.' })
} | Select-Object -First 1
if (-not $dotnet) { throw 'Install the .NET 10 SDK before publishing C9.' }

$releaseRoot = [IO.Path]::GetFullPath((Join-Path $workspace "artifacts/release/$RuntimeIdentifier"))
$runId = (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$staging = Join-Path $releaseRoot ('.staging-' + $runId)
$backup = Join-Path $releaseRoot ('.previous/' + $runId)
$targets = if ($Target -eq 'All') { @('Server', 'Client') } else { @($Target) }

function Invoke-Publish([string]$Name, [string[]]$Arguments) {
    Write-Host "[PUBLISH] $Name"
    & $dotnet @Arguments --disable-build-servers -m:1 -p:UseSharedCompilation=false | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Publish failed: $Name (exit code $LASTEXITCODE)" }
}

function Assert-ReleaseFile([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Expected release file not found: $Path" }
}

function Assert-ReleasePath([string]$Path) {
    $root = (Resolve-Path -LiteralPath $releaseRoot).ProviderPath.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    $full = [IO.Path]::GetFullPath($Path)
    if (-not $full.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) { throw "Path outside release root: $full" }
    if (Test-Path -LiteralPath $full) {
        $item = Get-Item -LiteralPath $full -Force
        if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw "Release path is a link: $full" }
        $resolved = (Resolve-Path -LiteralPath $full).ProviderPath
        if (-not $resolved.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) { throw "Resolved path outside release root: $resolved" }
    }
}

Push-Location -LiteralPath $workspace
try {
    [void][IO.Directory]::CreateDirectory($releaseRoot)
    foreach ($parent in @((Join-Path $workspace 'artifacts'), (Join-Path $workspace 'artifacts/release'), $releaseRoot)) {
        if (((Get-Item -LiteralPath $parent -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
            throw "Release directory is a link: $parent"
        }
    }
    $previousRoot = Join-Path $releaseRoot '.previous'
    if ((Test-Path -LiteralPath $previousRoot) -and ((Get-Item -LiteralPath $previousRoot -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw "Release backup directory is a link: $previousRoot"
    }
    Assert-ReleasePath $staging
    [void][IO.Directory]::CreateDirectory($staging)

    foreach ($part in $targets) {
        $output = Join-Path $staging $part.ToLowerInvariant()
        if ($part -eq 'Server') {
            if ($ServerUi -eq 'Maui') {
                if ($FrameworkDependent) { throw 'Use WinForms for a framework-dependent desktop release.' }
                Invoke-Publish 'C9.AdminDesktop (MAUI)' @('publish', 'src/C9.AdminDesktop/C9.AdminDesktop.csproj', '-c', 'Release', "-p:RuntimeIdentifierOverride=$RuntimeIdentifier", '-o', $output, '--nologo')
            } else {
                $adminArguments = @('publish', 'src/C9.AdminDesktop.WinForms/C9.AdminDesktop.WinForms.csproj', '-c', 'Release', '-p:UseAppHost=true', '-o', $output, '--nologo')
                if ($FrameworkDependent) { $adminArguments += @('--self-contained', 'false') }
                else { $adminArguments += @('-r', $RuntimeIdentifier, '--self-contained', 'true', '--source', 'https://api.nuget.org/v3/index.json') }
                Invoke-Publish 'C9.AdminDesktop (WinForms)' $adminArguments
            }
            Assert-ReleaseFile (Join-Path $output 'C9.AdminDesktop.exe')
            $gameOutput = Join-Path $output 'game'
            $serverArguments = @('publish', 'src/C9.Server/C9.Server.csproj', '-c', 'Release', '-p:UseAppHost=true', '-o', $gameOutput, '--nologo')
            if ($FrameworkDependent) { $serverArguments += @('--self-contained', 'false') }
            else { $serverArguments += @('-r', $RuntimeIdentifier, '--self-contained', 'true', '--source', 'https://api.nuget.org/v3/index.json') }
            Invoke-Publish 'C9.Server' $serverArguments
            Assert-ReleaseFile (Join-Path $gameOutput 'C9.Server.exe')
            Assert-ReleaseFile (Join-Path $gameOutput 'C9.Server.dll')
        } else {
            $clientArguments = @('publish', 'src/C9.LocalLauncher/C9.LocalLauncher.csproj', '-c', 'Release', '-p:UseAppHost=true', '-o', $output, '--nologo')
            if ($FrameworkDependent) { $clientArguments += @('--self-contained', 'false', '-p:PublishSingleFile=false') }
            else { $clientArguments += @('-r', $RuntimeIdentifier, '--self-contained', 'true', '-p:PublishSingleFile=true', '-p:IncludeNativeLibrariesForSelfExtract=true', '--source', 'https://api.nuget.org/v3/index.json') }
            Invoke-Publish 'C9.LocalLauncher' $clientArguments
            Assert-ReleaseFile (Join-Path $output 'L9.exe')
        }
        if (-not $NoZip) {
            $archive = Join-Path $staging "C9-$part-$RuntimeIdentifier.zip"
            Compress-Archive -Path (Join-Path $output '*') -DestinationPath $archive -CompressionLevel Optimal
            Assert-ReleaseFile $archive
        }
    }

    # Preserve previously published files; never recursively delete an output directory.
    foreach ($part in $targets) {
        $name = $part.ToLowerInvariant()
        $items = @($name)
        $items += "C9-$part-$RuntimeIdentifier.zip"
        foreach ($itemName in $items) {
            $source = Join-Path $staging $itemName
            $destination = Join-Path $releaseRoot $itemName
            $old = Join-Path $backup $itemName
            Assert-ReleasePath $source
            Assert-ReleasePath $destination
            Assert-ReleasePath $old
            if (Test-Path -LiteralPath $destination) {
                [void][IO.Directory]::CreateDirectory($backup)
                Move-Item -LiteralPath $destination -Destination $old
            }
            if (Test-Path -LiteralPath $source) { Move-Item -LiteralPath $source -Destination $destination }
        }
        Write-Host "[RELEASE] $(Join-Path $releaseRoot $name)"
    }
    if ($Launch) {
        $relative = if ($Target -eq 'Server') { 'server/C9.AdminDesktop.exe' } else { 'client/L9.exe' }
        $executable = Join-Path $releaseRoot $relative
        Start-Process -FilePath $executable -WorkingDirectory (Split-Path -Parent $executable)
    }
} finally {
    Pop-Location
}
