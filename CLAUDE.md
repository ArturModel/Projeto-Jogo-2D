# CLAUDE.md — Instruções para Agentes de IA

Leia e siga integralmente todas as instruções definidas em [`AGENTS.md`](AGENTS.md) na raiz deste repositório.

## Resumo do Ciclo Obrigatório em Todo Prompt:
1. **Início do Prompt (Busca Obrigatória):**
   - Leia `docs/ai-memory/INDEX.md` e `docs/ai-memory/CURRENT_STATE.md`.
   - Consulte a documentação do cofre Obsidian em `docs/architecture-vault/manual/` (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`).
   - Execute `graphify query "<pergunta>"`, `graphify path "<A>" "<B>" --undirected` ou `graphify explain "<Simbolo>"` antes de explorar arquivos de código.
2. **Fim do Prompt (Atualização Obrigatória):**
   - Se qualquer código ou arquitetura foi alterado, execute `./scripts/graphify-refresh.sh` para atualizar o Graphify (`graphify-out/`) e o cofre gerado do Obsidian (`docs/architecture-vault/generated/`).
   - Atualize as notas manuais do Obsidian (`docs/architecture-vault/manual/`) e a memória de IA (`docs/ai-memory/CURRENT_STATE.md`, `CHANGELOG_AI.md`, `ARCHITECTURE.md`, `DECISIONS.md`).
   - Execute `./scripts/graphify-validate.sh` para comprovar a integridade.
