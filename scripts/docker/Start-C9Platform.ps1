<#
.SYNOPSIS
  Builds and starts the C9 platform with Docker Compose (game services, Admin Portal, Prometheus, Loki, Alloy, Grafana).
.DESCRIPTION
  Requires Docker Desktop (WSL2 backend). Every port is published on 127.0.0.1 only.
  -GameData also starts PostgreSQL (profile "gamedata") and points the Accounts/World modules at it.
  -NoBuild reuses the existing c9-platform:local image.
.EXAMPLE
  ./scripts/docker/Start-C9Platform.ps1
  ./scripts/docker/Start-C9Platform.ps1 -GameData
#>
[CmdletBinding()]
param([switch]$GameData, [switch]$NoBuild, [string]$CaBundle)
$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
Push-Location $root
try {
    docker version --format '{{.Server.Version}}' | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Docker Desktop is not running. Start Docker Desktop and try again.' }
    foreach ($port in 14000, 14001) {
        if (Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue | Where-Object { $_.OwningProcess -ne 0 -and (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue).ProcessName -notmatch 'docker|wsl|vpnkit|com.docker' }) {
            throw "Port $port is already used by another program (for example L9 started its own server). Close it first."
        }
    }
    if (-not $NoBuild) {
        $build = @('build', '-f', 'docker/Dockerfile', '-t', 'c9-platform:local', '.')
        if ($CaBundle) { $build = @('build', '--secret', "id=build_ca,src=$CaBundle") + $build[1..($build.Length - 1)] }
        docker @build
        if ($LASTEXITCODE -ne 0) { throw 'docker build failed' }
    }
    $compose = @('compose')
    if ($GameData) {
        $compose += @('--profile', 'gamedata')
        $env:C9_DB_CONNECTION = 'Host=c9-db;Port=5432;Database=c9;Username=c9;Gss Encryption Mode=Disable'
    }
    docker @compose up -d --no-build
    if ($LASTEXITCODE -ne 0) { throw 'docker compose up failed' }
    docker compose ps
    Write-Host ''
    Write-Host 'Admin Portal : http://127.0.0.1:14880  (key: ./scripts/docker/Get-C9PortalKey.ps1)'
    Write-Host 'Grafana      : http://127.0.0.1:3000/d/c9-overview'
    Write-Host 'Prometheus   : http://127.0.0.1:9090'
    Write-Host 'Game client  : connects to 127.0.0.1:14000 as before'
}
finally { Pop-Location }
