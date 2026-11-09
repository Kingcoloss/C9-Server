$ErrorActionPreference = 'Stop'
$workspace = Split-Path -Parent $PSScriptRoot
$snapshot = Join-Path $workspace 'config/migration/local-state'
$files = [ordered]@{
    'official-55350-characters.json' = 'artifacts/official-55350-characters.local.json'
    'lab-characters.json' = 'artifacts/lab-characters.local.json'
    'parameters.json' = 'artifacts/admin-console/parameters.local.json'
}

foreach ($entry in $files.GetEnumerator()) {
    $source = Join-Path $snapshot $entry.Key
    $target = Join-Path $workspace $entry.Value
    if ([IO.File]::Exists($target)) {
        Write-Host "Kept existing $($entry.Value)"
        continue
    }

    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($target))
    $temporary = "$target.$([Guid]::NewGuid().ToString('N')).tmp"
    try {
        [IO.File]::Copy($source, $temporary)
        [IO.File]::Move($temporary, $target)
        Write-Host "Restored $($entry.Value)"
    } finally {
        [IO.File]::Delete($temporary)
    }
}
