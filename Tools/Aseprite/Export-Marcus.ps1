param([string]$Root = (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent))
$ErrorActionPreference = 'Stop'
$art = Join-Path $Root 'Arte/Producao/Marcus/MarcusCaminhadaV20.png'
if (!(Test-Path -LiteralPath $art)) { $art = Join-Path $Root 'Arte/Producao/Marcus/MarcusCaminhada.png' }
$assets = Join-Path $Root 'Projeto Jogo 2D/Assets/StreamingAssets'
$bytes = [IO.File]::ReadAllBytes($art)
$width = ([int]$bytes[16] -shl 24) -bor ([int]$bytes[17] -shl 16) -bor ([int]$bytes[18] -shl 8) -bor $bytes[19]
$height = ([int]$bytes[20] -shl 24) -bor ([int]$bytes[21] -shl 16) -bor ([int]$bytes[22] -shl 8) -bor $bytes[23]
if ($height -ne 256 -or $width -notin 192, 576) { throw 'A folha de Marcus deve ter quatro linhas e três ou nove colunas de 64 px.' }
Copy-Item -LiteralPath $art -Destination (Join-Path $assets 'MarcusWalkFluido.png') -Force
$pyLanterna = Join-Path $PSScriptRoot 'gerar-marcus-lanterna.py'
if ($width -eq 576 -and (Test-Path -LiteralPath $pyLanterna)) {
    & python $pyLanterna
    if ($LASTEXITCODE -ne 0) { throw "Falha ao gerar MarcusLanterna.png via $pyLanterna" }
}
