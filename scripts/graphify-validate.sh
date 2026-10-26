#!/usr/bin/env bash
# scripts/graphify-validate.sh — Validação de integridade do Grafo e Cofre Obsidian
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "==> Executando validações de integridade..."

ERRORS=0

# 1. Verifica existência de graph.json e GRAPH_REPORT.md
if [ -f "graphify-out/graph.json" ] && [ -s "graphify-out/graph.json" ]; then
    echo "  [OK] graphify-out/graph.json presente e populado."
else
    echo "  [FALHA] graphify-out/graph.json ausente ou vazio." >&2
    ERRORS=$((ERRORS + 1))
fi

if [ -f "graphify-out/GRAPH_REPORT.md" ]; then
    echo "  [OK] graphify-out/GRAPH_REPORT.md presente."
else
    echo "  [FALHA] graphify-out/GRAPH_REPORT.md ausente." >&2
    ERRORS=$((ERRORS + 1))
fi

# 2. Valida JSON do Canvas do Obsidian
CANVAS="docs/architecture-vault/generated/graph.canvas"
if [ -f "$CANVAS" ]; then
    if python3 -c "import json; json.loads(open('$CANVAS').read())" 2>/dev/null; then
        echo "  [OK] $CANVAS é um JSON válido."
    else
        echo "  [FALHA] $CANVAS contém JSON corrompido." >&2
        ERRORS=$((ERRORS + 1))
    fi
else
    echo "  [FALHA] $CANVAS ausente." >&2
    ERRORS=$((ERRORS + 1))
fi

# 3. Testa consulta no grafo via CLI
if command -v graphify >/dev/null 2>&1 || [ -x "$HOME/.local/bin/graphify" ]; then
    export PATH="$HOME/.local/bin:$PATH"
    if graphify query "GameState" >/dev/null 2>&1; then
        echo "  [OK] graphify query respondeu com sucesso."
    else
        echo "  [FALHA] graphify query falhou ao consultar 'GameState'." >&2
        ERRORS=$((ERRORS + 1))
    fi
else
    echo "  [FALHA] comando 'graphify' não acessível para validação." >&2
    ERRORS=$((ERRORS + 1))
fi

# 4. Verifica se mídias binárias vazaram para o grafo
if grep -q "\.png" graphify-out/graph.json 2>/dev/null; then
    echo "  [FALHA] Arquivos .png foram encontrados em graphify-out/graph.json (exclusão violada)." >&2
    ERRORS=$((ERRORS + 1))
else
    echo "  [OK] Nenhuma imagem .png detectada no grafo (isolamento mantido)."
fi

if grep -q "\.mp4" graphify-out/graph.json 2>/dev/null; then
    echo "  [FALHA] Arquivos .mp4 foram encontrados em graphify-out/graph.json (exclusão violada)." >&2
    ERRORS=$((ERRORS + 1))
else
    echo "  [OK] Nenhum vídeo .mp4 detectado no grafo (isolamento mantido)."
fi

# 5. Verifica preservação do cofre manual
if [ -f "docs/architecture-vault/manual/Visao_Geral_Arquitetura.md" ]; then
    echo "  [OK] Notas perenes em docs/architecture-vault/manual/ preservadas."
else
    echo "  [FALHA] Pasta manual do cofre não encontrada." >&2
    ERRORS=$((ERRORS + 1))
fi

if [ "$ERRORS" -eq 0 ]; then
    echo "==> Todas as validações passaram com sucesso!"
    exit 0
else
    echo "==> Validação concluída com $ERRORS erro(s)." >&2
    exit 1
fi
