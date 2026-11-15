<#
.SYNOPSIS
  End-to-end smoke test of a running platform: TCP login/lobby flows, portal API, packet trace, exports,
  parameters, control, audit, Prometheus, Grafana and Loki. Exit code 0 = all passed.
#>
[CmdletBinding()]
param([string]$State)
$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
if (-not $State) { $State = if ($env:C9_STATE_DIR) { $env:C9_STATE_DIR } else { Join-Path $root 'artifacts/docker-state' } }
dotnet run --project (Join-Path $root 'tests/C9.Platform.Smoke') -c Release -- --state $State
exit $LASTEXITCODE
