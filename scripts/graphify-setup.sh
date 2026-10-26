#!/usr/bin/env bash
# scripts/graphify-setup.sh — Configuração e verificação do ambiente Graphify + Obsidian
set -euo pipefail

echo "==> Verificando dependências do ambiente..."

# 1. Verifica Git
if ! command -v git >/dev/null 2>&1; then
    echo "ERRO: 'git' não encontrado no PATH." >&2
    exit 1
fi

# 2. Verifica Python / uv
if ! command -v uv >/dev/null 2>&1 && ! command -v python3 >/dev/null 2>&1; then
    echo "ERRO: Nem 'uv' nem 'python3' foram encontrados no PATH." >&2
    exit 1
fi

# 3. Verifica ou instala graphify CLI via uv tool
if ! command -v graphify >/dev/null 2>&1; then
    echo "==> 'graphify' não encontrado no PATH. Tentando instalar via uv tool..."
    if command -v uv >/dev/null 2>&1; then
        uv tool install graphifyy
        export PATH="$HOME/.local/bin:$PATH"
    else
        echo "ERRO: Instale a ferramenta com: pip install graphifyy ou uv tool install graphifyy" >&2
        exit 1
    fi
fi

GRAPHIFY_VER=$(graphify --version 2>/dev/null || echo "instalado")
echo "==> Graphify detectado: $GRAPHIFY_VER"

# 4. Verifica integração do Antigravity
echo "==> Verificando integração do Antigravity (.agents/)..."
if [ ! -f ".agents/rules/graphify.md" ]; then
    echo "==> Configurando regras do Antigravity..."
    graphify antigravity install
else
    echo "==> Regras do Antigravity já presentes (.agents/rules/graphify.md)."
fi

# 5. Verifica estrutura de diretórios do projeto
mkdir -p graphify-out
mkdir -p docs/architecture-vault/generated
mkdir -p docs/architecture-vault/manual
mkdir -p docs/ai-memory

# 6. Verifica Obsidian (Flatpak ou nativo)
if command -v obsidian >/dev/null 2>&1 || (command -v flatpak >/dev/null 2>&1 && flatpak info md.obsidian.Obsidian >/dev/null 2>&1); then
    echo "==> Obsidian detectado no sistema."
else
    echo "AVISO: Obsidian não detectado no PATH/Flatpak. O cofre Markdown continuará disponível em docs/architecture-vault/."
fi

echo "==> Setup concluído com sucesso."
