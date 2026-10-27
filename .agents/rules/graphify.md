---
trigger: always_on
description: Índice estrutural Graphify + Obsidian local de Santa Rita com ciclo por prompt.
---

## graphify

Este projeto possui um grafo estrutural local em `graphify-out/` e um cofre Obsidian em `docs/architecture-vault/`.

Regras obrigatórias:
- **No início de todo prompt:** execute `graphify query "<pergunta>"` (CLI) ou MCP, `graphify path "<A>" "<B>" --undirected` ou `graphify explain "<classe>"`. Consulte também as notas em `docs/architecture-vault/manual/`.
- Nunca altere código baseando-se apenas em resumos de grafo; confirme assinaturas e lógica no código C# real (`Projeto Jogo 2D/Assets/Scripts/`).
- **Ao final de cada prompt:** execute `./scripts/graphify-refresh.sh` (Linux) ou `.\scripts\graphify-refresh.ps1` (Windows) para manter o grafo e o cofre atualizados. Em seguida, valide com `./scripts/graphify-validate.sh` ou `.ps1`.
- Crie o registro do plano executado em `docs/ai-plans/` conforme [`AGENTS.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/AGENTS.md).
