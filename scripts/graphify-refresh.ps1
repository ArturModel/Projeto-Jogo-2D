# Shared local Graphify lifecycle. Propagate native failures to the caller.
$ErrorActionPreference = 'Stop'
$graphifyScript = Join-Path $PSScriptRoot 'graphify-project.py'
$graphifyPython = Get-Command python -ErrorAction SilentlyContinue
if ($graphifyPython) {
    & $graphifyPython.Source $graphifyScript refresh
} elseif (Get-Command py -ErrorAction SilentlyContinue) {
    & py -3 $graphifyScript refresh
} elseif (Get-Command uv -ErrorAction SilentlyContinue) {
    & uv run --no-project --python 3.12 python $graphifyScript refresh
} else {
    throw 'Python 3.10+ or uv is required. See docs/GRAPHIFY-OBSIDIAN-OPERACAO.md.'
}
if ($LASTEXITCODE -ne 0) { throw "Graphify refresh failed (exit $LASTEXITCODE)." }
