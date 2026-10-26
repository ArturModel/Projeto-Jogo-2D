# scripts/graphify-validate.ps1 — Validação de integridade do Grafo e Cofre Obsidian (Windows PowerShell)
$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $PSScriptRoot
Set-Location $RootDir

Write-Host "==> Executando validações de integridade (PowerShell)..." -ForegroundColor Cyan

$errors = 0

# 1. Verifica existência de graph.json e GRAPH_REPORT.md
if ((Test-Path "graphify-out/graph.json") -and ((Get-Item "graphify-out/graph.json").Length -gt 0)) {
    Write-Host "  [OK] graphify-out/graph.json presente e populado." -ForegroundColor Green
} else {
    Write-Host "  [FALHA] graphify-out/graph.json ausente ou vazio." -ForegroundColor Red
    $errors++
}

if (Test-Path "graphify-out/GRAPH_REPORT.md") {
    Write-Host "  [OK] graphify-out/GRAPH_REPORT.md presente." -ForegroundColor Green
} else {
    Write-Host "  [FALHA] graphify-out/GRAPH_REPORT.md ausente." -ForegroundColor Red
    $errors++
}

# 2. Valida JSON do Canvas do Obsidian
$canvas = "docs/architecture-vault/generated/graph.canvas"
if (Test-Path $canvas) {
    try {
        Get-Content $canvas -Raw -Encoding UTF8 | ConvertFrom-Json | Out-Null
        Write-Host "  [OK] $canvas é um JSON válido." -ForegroundColor Green
    } catch {
        Write-Host "  [FALHA] $canvas contém JSON corrompido." -ForegroundColor Red
        $errors++
    }
} else {
    Write-Host "  [FALHA] $canvas ausente." -ForegroundColor Red
    $errors++
}

# 3. Testa consulta no grafo via CLI
if (Get-Command graphify -ErrorAction SilentlyContinue) {
    try {
        graphify query "GameState" | Out-Null
        Write-Host "  [OK] graphify query respondeu com sucesso." -ForegroundColor Green
    } catch {
        Write-Host "  [FALHA] graphify query falhou ao consultar 'GameState'." -ForegroundColor Red
        $errors++
    }
} else {
    Write-Host "  [FALHA] comando 'graphify' não acessível para validação." -ForegroundColor Red
    $errors++
}

# 4. Verifica se mídias binárias vazaram para o grafo
$graphContent = Get-Content "graphify-out/graph.json" -Raw -Encoding UTF8
if ($graphContent -match "\.png") {
    Write-Host "  [FALHA] Arquivos .png foram encontrados em graphify-out/graph.json (exclusão violada)." -ForegroundColor Red
    $errors++
} else {
    Write-Host "  [OK] Nenhuma imagem .png detectada no grafo (isolamento mantido)." -ForegroundColor Green
}

if ($graphContent -match "\.mp4") {
    Write-Host "  [FALHA] Arquivos .mp4 foram encontrados em graphify-out/graph.json (exclusão violada)." -ForegroundColor Red
    $errors++
} else {
    Write-Host "  [OK] Nenhum vídeo .mp4 detectado no grafo (isolamento mantido)." -ForegroundColor Green
}

# 5. Verifica preservação do cofre manual
if (Test-Path "docs/architecture-vault/manual/Visao_Geral_Arquitetura.md") {
    Write-Host "  [OK] Notas perenes em docs/architecture-vault/manual/ preservadas." -ForegroundColor Green
} else {
    Write-Host "  [FALHA] Pasta manual do cofre não encontrada." -ForegroundColor Red
    $errors++
}

if ($errors -eq 0) {
    Write-Host "==> Todas as validações passaram com sucesso!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "==> Validação concluída com $errors erro(s)." -ForegroundColor Red
    exit 1
}
