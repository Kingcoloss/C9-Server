<#
.SYNOPSIS
  Stops the C9 platform containers. State in artifacts/docker-state and the metric/log volumes are kept.
.PARAMETER RemoveData
  Also deletes the Prometheus/Loki/Grafana/PostgreSQL volumes (asks for confirmation). artifacts/docker-state is never deleted.
#>
[CmdletBinding(SupportsShouldProcess)]
param([switch]$RemoveData)
$ErrorActionPreference = 'Stop'
Push-Location (Resolve-Path (Join-Path $PSScriptRoot '../..'))
try {
    if ($RemoveData) {
        if ($PSCmdlet.ShouldProcess('Docker volumes of project c9 (metrics, logs, dashboards state, game database)', 'Remove')) {
            docker compose --profile gamedata down -v
        }
    }
    else { docker compose --profile gamedata down }
}
finally { Pop-Location }
