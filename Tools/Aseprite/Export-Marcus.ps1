param([string]$Root = (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent))
$ErrorActionPreference = 'Stop'
$art = Join-Path $Root 'Arte/Producao/Marcus/MarcusCaminhada.png'
$assets = Join-Path $Root 'Projeto Jogo 2D/Assets/StreamingAssets'
$bytes = [IO.File]::ReadAllBytes($art)
$width = ([int]$bytes[16] -shl 24) -bor ([int]$bytes[17] -shl 16) -bor ([int]$bytes[18] -shl 8) -bor $bytes[19]
$height = ([int]$bytes[20] -shl 24) -bor ([int]$bytes[21] -shl 16) -bor ([int]$bytes[22] -shl 8) -bor $bytes[23]
if ($width -ne 192 -or $height -ne 256) { throw 'A folha de Marcus deve ter três quadros de 64 px por quatro direções.' }
Copy-Item -LiteralPath $art -Destination (Join-Path $assets 'MarcusWalkFluido.png') -Force
