---
trigger: always_on
description: Contexto obrigatório de Santa Rita — ciclo de consulta e atualização por prompt com histórico de planos.
---

# Governança de Contexto de IA — Santa Rita (Ciclo Obrigatório em Todo Prompt)

Qualquer agente de IA atuando neste repositório DEVE cumprir o ciclo formal estipulado em [`AGENTS.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/AGENTS.md):

1. **Início de Todo Prompt (Consulta Obrigatória):**
   - Ler [`docs/ai-memory/INDEX.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CURRENT_STATE.md).
   - Consultar o cofre Obsidian em [`docs/architecture-vault/manual/`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/architecture-vault/manual/).
   - Executar consultas no **Graphify** (`graphify query`, `graphify path`, `graphify explain`) antes de ler arquivos de código.
   - Confirmar regras de domínio: Save manual exclusivo no Orelhão em RAM (`GameState`), proibição de spoilers de Lucas no Cap. 1, C2 já implementado até Bocha (93 scripts C#), Marcus como referência de arte.

2. **Final de Cada Prompt (Ciclo Obrigatório de Fechamento):**
   - Atualizar Graphify e Obsidian com `./scripts/graphify-refresh.sh` (Linux) ou `.\scripts\graphify-refresh.ps1` (Windows).
   - Validar integridade com `./scripts/graphify-validate.sh` (Linux) ou `.\scripts\graphify-validate.ps1` (Windows).
   - **Criar documento do plano executado** em `docs/ai-plans/YYYY-MM-DD_<usuario-github>_<slug>.md` registrando o usuário GitHub (`gh api user -q .login` ou `git config user.name`), data, prompt, diagnóstico, ações e validação.
   - Registrar no [`docs/ai-memory/CHANGELOG_AI.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CHANGELOG_AI.md).
