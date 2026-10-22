param([Parameter(Mandatory=$true)][string]$Source, [Parameter(Mandatory=$true)][string]$Output)
$ErrorActionPreference = 'Stop'
$aseprite = Join-Path $env:USERPROFILE '.santarita\aseprite-ninja\bin\aseprite.exe'
if (!(Test-Path -LiteralPath $aseprite)) { throw 'Aseprite local não encontrado; consulte Docs/Producao_Artistica_Execucao.md.' }
$sourcePath = (Resolve-Path -LiteralPath $Source).Path
$outputPath = [IO.Path]::GetFullPath($Output)
New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
& $aseprite --batch $sourcePath --sheet-type rows --sheet (Join-Path $outputPath 'sheet.png') --data (Join-Path $outputPath 'sheet.json') --format json-array --list-tags 2>&1 | Out-String | Write-Output
if ($LASTEXITCODE -ne 0) { throw 'Aseprite falhou na exportação.' }
