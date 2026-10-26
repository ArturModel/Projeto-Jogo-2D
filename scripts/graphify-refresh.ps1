# scripts/graphify-refresh.ps1 — Atualização incremental do grafo e exportação do cofre (Windows PowerShell)
$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $PSScriptRoot
Set-Location $RootDir

Write-Host "==> Atualizando grafo estrutural incrementalmente (AST C#)..." -ForegroundColor Cyan

if (-not (Get-Command graphify -ErrorAction SilentlyContinue)) {
    $uvBin = Join-Path $HOME ".local\bin"
    if (Test-Path $uvBin) {
        $env:PATH = "$uvBin;$env:PATH"
    }
}

if (-not (Get-Command graphify -ErrorAction SilentlyContinue)) {
    Write-Error "ERRO: 'graphify' não encontrado. Execute .\scripts\graphify-setup.ps1 primeiro."
    exit 1
}

# 1. Atualiza AST localmente (sem custo de LLM)
graphify update .

# 2. Regenera cofre do Obsidian preservando manual/
Write-Host "==> Regenerando cofre Obsidian em docs/architecture-vault/generated/..." -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path "docs/architecture-vault/generated" | Out-Null
graphify export obsidian --dir docs/architecture-vault/generated

# 3. Regenera visualização HTML
Write-Host "==> Atualizando visualização HTML em graphify-out/graph.html..." -ForegroundColor Cyan
graphify export html

Write-Host "==> Grafo e cofre atualizados com sucesso." -ForegroundColor Green
