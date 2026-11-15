<#
.SYNOPSIS
  Prints (or copies) the Admin Portal operator key created on first start.
.DESCRIPTION
  The key signs in as Owner. Keep it private; delete artifacts/docker-state/portal/access-key.txt and restart
  c9-portal to rotate it (the old key and its sessions stop working).
#>
[CmdletBinding()]
param([switch]$Copy)
$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
$state = if ($env:C9_STATE_DIR) { $env:C9_STATE_DIR } else { Join-Path $root 'artifacts/docker-state' }
$file = Join-Path $state 'portal/access-key.txt'
if (-not (Test-Path $file)) { throw "No key yet at $file. Start the platform first (Start-C9Platform.ps1)." }
$key = (Get-Content -Raw $file).Trim()
if ($Copy) { Set-Clipboard -Value $key; Write-Host 'Operator key copied to the clipboard.' } else { $key }
