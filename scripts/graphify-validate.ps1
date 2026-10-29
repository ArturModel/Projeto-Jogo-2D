# Shared local Graphify lifecycle. Propagate native failures to the caller.
$ErrorActionPreference = 'Stop'
$graphifyScript = Join-Path $PSScriptRoot 'graphify-project.py'

# Ensure uv and local bin paths are present in PATH if available
$localBin = Join-Path $env:USERPROFILE '.local\bin'
if ((Test-Path $localBin) -and ($env:Path -notlike "*$localBin*")) {
    $env:Path = "$localBin;$env:Path"
}
$wingetUvDir = Join-Path $env:LOCALAPPDATA 'Microsoft\WinGet\Packages\astral-sh.uv_Microsoft.Winget.Source_8wekyb3d8bbwe'
if ((Test-Path $wingetUvDir) -and ($env:Path -notlike "*$wingetUvDir*")) {
    $env:Path = "$wingetUvDir;$env:Path"
}

$graphifyPython = Get-Command python -ErrorAction SilentlyContinue
if ($graphifyPython) {
    try {
        & $graphifyPython.Source --version *>$null
        if ($LASTEXITCODE -ne 0) { $graphifyPython = $null }
    } catch {
        $graphifyPython = $null
    }
}

if ($graphifyPython) {
    & $graphifyPython.Source $graphifyScript validate
} elseif (Get-Command py -ErrorAction SilentlyContinue) {
    & py -3 $graphifyScript validate
} elseif (Get-Command uv -ErrorAction SilentlyContinue) {
    & uv run --no-project --python 3.12 python $graphifyScript validate
} else {
    throw 'Python 3.10+ or uv is required. See docs/GRAPHIFY-OBSIDIAN-OPERACAO.md.'
}
if ($LASTEXITCODE -ne 0) { throw "Graphify validate failed (exit $LASTEXITCODE)." }
