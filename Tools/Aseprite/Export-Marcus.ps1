param([string]$Root = (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent))
$ErrorActionPreference = 'Stop'
$aseprite = Join-Path $env:USERPROFILE '.santarita/aseprite-ninja/bin/aseprite.exe'
$art = Join-Path $Root 'Arte/Producao/Marcus'
$assets = Join-Path $Root 'Projeto Jogo 2D/Assets/StreamingAssets'
$logs = Join-Path ([IO.Path]::GetTempPath()) 'SantaRitaMarcusExport'
New-Item -ItemType Directory -Force -Path $logs | Out-Null
function Invoke-MarcusExport([string]$Script, [string]$Source) {
    $arguments = @('--batch', '--script-param', ('"root=' + $art + '"'), '--script-param', ('"source=' + $Source + '"'), '--script', ('"' + (Join-Path $PSScriptRoot $Script) + '"'))
    $process = Start-Process -FilePath $aseprite -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru -RedirectStandardOutput (Join-Path $logs ($Script + '.out.txt')) -RedirectStandardError (Join-Path $logs ($Script + '.err.txt'))
    if ($process.ExitCode -ne 0) { throw (Get-Content -LiteralPath (Join-Path $logs ($Script + '.err.txt')) -Raw) }
}
Invoke-MarcusExport 'marcus-walk.lua' (Join-Path $assets 'MarcusWalk.png')
Invoke-MarcusExport 'marcus-flashlight.lua' (Join-Path $art 'MarcusWalkFluido.png')
foreach ($name in @('MarcusWalkFluido.png', 'MarcusLanterna.png')) {
    $bytes = [IO.File]::ReadAllBytes((Join-Path $art $name))
    $width = ([int]$bytes[16] -shl 24) -bor ([int]$bytes[17] -shl 16) -bor ([int]$bytes[18] -shl 8) -bor $bytes[19]
    $height = ([int]$bytes[20] -shl 24) -bor ([int]$bytes[21] -shl 16) -bor ([int]$bytes[22] -shl 8) -bor $bytes[23]
    if ($width -ne 1088 -or $height -ne 192) { throw "Folha incompleta: $name ${width}x${height}" }
}
# Publicar somente depois que ambas as exportacoes terminarem. Preservar GUIDs/metas.
foreach ($name in @('MarcusWalkFluido.png', 'MarcusLanterna.png')) {
    Copy-Item -LiteralPath (Join-Path $art $name) -Destination (Join-Path $assets $name) -Force
}
Write-Output 'Marcus: duas folhas completas exportadas, fontes e GUIDs preservados.'
