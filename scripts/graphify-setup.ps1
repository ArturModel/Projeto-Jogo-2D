# scripts/graphify-setup.ps1 — Configuração e verificação do ambiente Graphify + Obsidian (Windows PowerShell)
$ErrorActionPreference = "Stop"

Write-Host "==> Verificando dependências do ambiente (Windows PowerShell)..." -ForegroundColor Cyan

# 1. Verifica Git
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "ERRO: 'git' não encontrado no PATH. Instale o Git for Windows."
    exit 1
}

# 2. Verifica Python / uv
$hasUv = [bool](Get-Command uv -ErrorAction SilentlyContinue)
$hasPy = [bool](Get-Command python -ErrorAction SilentlyContinue) -or [bool](Get-Command py -ErrorAction SilentlyContinue)
if (-not $hasUv -and -not $hasPy) {
    Write-Error "ERRO: Nem 'uv' nem 'python' foram encontrados no PATH. Instale o uv: winget install --id astral-sh.uv -e"
    exit 1
}

# 3. Verifica ou instala graphify CLI via uv tool
if (-not (Get-Command graphify -ErrorAction SilentlyContinue)) {
    Write-Host "==> 'graphify' não encontrado no PATH. Tentando instalar via uv tool..." -ForegroundColor Yellow
    if ($hasUv) {
        uv tool install graphifyy
        uv tool update-shell
    } else {
        Write-Error "ERRO: Instale a ferramenta com: uv tool install graphifyy (ou pip install graphifyy)"
        exit 1
    }
}

$graphifyVer = try { graphify --version 2>$null } catch { "instalado" }
Write-Host "==> Graphify detectado: $graphifyVer" -ForegroundColor Green

# 4. Verifica integração do Antigravity
Write-Host "==> Verificando integração do Antigravity (.agents/)..."
if (-not (Test-Path ".agents/rules/graphify.md")) {
    Write-Host "==> Configurando regras do Antigravity..."
    graphify antigravity install
} else {
    Write-Host "==> Regras do Antigravity já presentes (.agents/rules/graphify.md)." -ForegroundColor Green
}

# 5. Verifica estrutura de diretórios do projeto
New-Item -ItemType Directory -Force -Path "graphify-out" | Out-Null
New-Item -ItemType Directory -Force -Path "docs/architecture-vault/generated" | Out-Null
New-Item -ItemType Directory -Force -Path "docs/architecture-vault/manual" | Out-Null
New-Item -ItemType Directory -Force -Path "docs/ai-memory" | Out-Null

Write-Host "==> Setup concluído com sucesso! Abra 'docs/architecture-vault' como cofre no Obsidian." -ForegroundColor Green
