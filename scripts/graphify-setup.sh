#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
export PATH="$HOME/.local/bin:$PATH"
if command -v python3 >/dev/null 2>&1; then
    python3 "$SCRIPT_DIR/graphify-project.py" setup
elif command -v python >/dev/null 2>&1; then
    python "$SCRIPT_DIR/graphify-project.py" setup
elif command -v uv >/dev/null 2>&1; then
    uv run --no-project --python 3.12 python "$SCRIPT_DIR/graphify-project.py" setup
else
    echo 'Python 3.10+ or uv required; see docs/GRAPHIFY-OBSIDIAN-OPERACAO.md.' >&2
    exit 1
fi
