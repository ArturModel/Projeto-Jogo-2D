#!/usr/bin/env bash
# scripts/graphify-refresh.sh — Atualização incremental do grafo e exportação do cofre
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "==> Atualizando grafo estrutural incrementalmente (AST C#)..."

if ! command -v graphify >/dev/null 2>&1; then
    export PATH="$HOME/.local/bin:$PATH"
fi

if ! command -v graphify >/dev/null 2>&1; then
    echo "ERRO: 'graphify' não encontrado. Execute ./scripts/graphify-setup.sh primeiro." >&2
    exit 1
fi

# Atualiza AST localmente (sem custo de LLM)
graphify update .

# Regenera cofre do Obsidian preservando manual/
echo "==> Regenerando cofre Obsidian em docs/architecture-vault/generated/..."
mkdir -p docs/architecture-vault/generated
graphify export obsidian --dir docs/architecture-vault/generated

# Regenera visualização HTML
echo "==> Atualizando visualização HTML em graphify-out/graph.html..."
graphify export html

echo "==> Grafo e cofre atualizados com sucesso."
