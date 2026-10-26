# Política de Contexto do Projeto — Santa Rita (Ciclo Obrigatório em Todo Prompt)

Para qualquer agente de IA que atue neste repositório, cumpra rigorosamente o contrato abaixo (também espelhado em [`AGENTS.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/AGENTS.md)):

1. **Busca Obrigatória no Início de Todo Prompt (Graphify + Obsidian + Memória):**
   - Comece lendo [`docs/ai-memory/INDEX.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CURRENT_STATE.md).
   - Consulte as notas estruturadas do **Obsidian** em [`docs/architecture-vault/manual/`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/architecture-vault/manual/) (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`) e, quando útil, as notas atômicas em `docs/architecture-vault/generated/<Simbolo>.md`.
   - Execute **obrigatoriamente** uma busca no **Graphify** antes de vasculhar o código-fonte:
     - `graphify query "<pergunta>"`
     - `graphify path "<SímboloA>" "<SímboloB>" --undirected`
     - `graphify explain "<Símbolo>"`
   - Identifique os nós, métodos e arquivos realmente relevantes antes de abrir arquivos `.cs`.

2. **Verificação de Atualidade:**
   - Verifique se o grafo está atualizado comparando com `git status` e `git log -1`. Se estiver defasado, rode `./scripts/graphify-refresh.sh`.

3. **Fonte da Verdade e Contratos R1:**
   - Leia apenas os trechos necessários dos arquivos C# originais. **Nunca altere código com base exclusivamente no resumo do grafo**: confirme assinaturas, tipos e fluxos no código-fonte atual.
   - Respeite estritamente: Save Manual exclusivo no Orelhão (`PayphoneCheckpoint` -> `CheckpointSystem`), estado da sessão em RAM (`GameState`), proibição de spoilers de Lucas no Cap. 1 e escala `PPU = 32` / Canvas `640×360` (`UIFactory`).

4. **Atualização Obrigatória ao Final de Cada Execução:**
   - Após criar ou alterar qualquer código ou arquitetura:
     1. Execute `./scripts/graphify-refresh.sh` para atualizar o grafo (`graphify-out/`) e o cofre Obsidian (`docs/architecture-vault/generated/` e `graphify-out/graph.html`).
     2. Atualize as notas manuais do Obsidian em [`docs/architecture-vault/manual/`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/architecture-vault/manual/) caso classes, scripts, zonas ou fluxos tenham mudado.
     3. Atualize [`docs/ai-memory/CURRENT_STATE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CURRENT_STATE.md), [`docs/ai-memory/CHANGELOG_AI.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CHANGELOG_AI.md), [`docs/ai-memory/ARCHITECTURE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/ARCHITECTURE.md) e [`docs/ai-memory/DECISIONS.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/DECISIONS.md).
     4. Execute `./scripts/graphify-validate.sh` para validar a integridade do grafo e do cofre.

5. **Segurança e Privacidade:**
   - Não envie segredos nem dados sensíveis a modelos remotos. Respeite estritamente o `.gitignore` e o `.graphifyignore`.
