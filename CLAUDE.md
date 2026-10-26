@AGENTS.md

<!-- BEGIN memoria-projeto -->
# CLAUDE.md — Instruções para o Claude Code e Agentes de IA

Siga integralmente todas as regras importadas de [`AGENTS.md`](AGENTS.md) na raiz deste repositório.

## Resumo do Ciclo Obrigatório em Todo Novo Prompt:
1. **Início do Prompt (Busca Obrigatória no Graphify + Obsidian):**
   - Leia `docs/ai-memory/INDEX.md` e `docs/ai-memory/CURRENT_STATE.md`.
   - Consulte a documentação do cofre Obsidian em `docs/architecture-vault/manual/` (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`).
   - Execute `graphify query "<pergunta>"`, `graphify path "<A>" "<B>" --undirected` ou `graphify explain "<Simbolo>"` antes de explorar arquivos de código C#.
   - Confirme assinaturas e tipos diretamente nos arquivos `.cs` antes de editar.
2. **Fim do Prompt (Atualização Obrigatória do Graphify + Obsidian):**
   - Se qualquer código ou arquitetura foi alterado, execute `./scripts/graphify-refresh.sh` (ou `.\scripts\graphify-refresh.ps1` no Windows) para atualizar `graphify-out/` e `docs/architecture-vault/generated/`.
   - Atualize as notas manuais do Obsidian (`docs/architecture-vault/manual/`) e a memória persistente (`docs/ai-memory/CURRENT_STATE.md`, `CHANGELOG_AI.md`, `ARCHITECTURE.md`, `DECISIONS.md`).
   - Execute `./scripts/graphify-validate.sh` (ou `.\scripts\graphify-validate.ps1` no Windows) para comprovar a integridade.
<!-- END memoria-projeto -->
