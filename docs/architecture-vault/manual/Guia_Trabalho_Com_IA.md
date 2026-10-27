# Guia de Trabalho com IA (Graphify + Obsidian + Planos por Autor) — Santa Rita

**Atualizado em:** 27/10/2026  
**Público-alvo:** Desenvolvedores, artistas, designers e agentes de IA do projeto *Santa Rita — A Noite das 03:17*.

---

## 1. O Ecossistema Integrado de IA

Neste repositório, nenhuma IA (Antigravity, Claude Code, Cursor, Copilot, Codex, Windsurf) atua no código às cegas ou inventa arquitetura. O fluxo de trabalho é guiado por [`AGENTS.md`](../../../AGENTS.md) e composto por 4 pilares:

1. **Grafo Estrutural (Graphify):** Mapeia os 93 scripts C#, classes, métodos e relações em `graphify-out/` sem custo de LLM (extração AST local ultrarrápida).
2. **Cofre Obsidian (`docs/architecture-vault/`):**
   - `manual/`: Notas arquiteturais curadas por humanos e mantidas intactas durante updates.
   - `generated/`: Mais de 1.600 notas atômicas geradas para classes/métodos e o `graph.canvas` interativo.
3. **Memória Persistente (`docs/ai-memory/`):** Estado atual, arquitetura, decisões técnicas e changelog central.
4. **Histórico de Planos por Autor (`docs/ai-plans/`):** Cada prompt executado por IA gera um documento registrando quem pediu, quem executou (usuário GitHub), data/hora, diagnóstico e resultado dos testes.

---

## 2. Ciclo de Trabalho por Prompt (Obrigatório)

### 2.1 — Início do Prompt
1. Ler `docs/ai-memory/INDEX.md` e `CURRENT_STATE.md`.
2. Consultar as notas do cofre em `docs/architecture-vault/manual/`.
3. Fazer buscas direcionadas no terminal:
   ```bash
   graphify query "Como funciona o minigame de Bocha?" --budget 300
   graphify path "BochaMinigameUI" "GameState" --undirected
   graphify explain "PayphoneCheckpoint"
   ```
4. Confirmar no código C# real (`Projeto Jogo 2D/Assets/Scripts/`).

### 2.2 — Durante a Implementação
- Respeitar contratos de domínio: Save manual exclusivo no Orelhão em RAM (`GameState`), proibição de spoilers de Lucas no Cap. 1, C2 já implementado até Bocha, Marcus como âncora de arte.

### 2.3 — Fechamento do Prompt
1. Executar o refresh para atualizar Graphify e Obsidian:
   - Linux/macOS: `./scripts/graphify-refresh.sh`
   - Windows: `.\scripts\graphify-refresh.ps1`
2. Validar a integridade:
   - Linux/macOS: `./scripts/graphify-validate.sh`
   - Windows: `.\scripts\graphify-validate.ps1`
3. Criar o documento de plano em `docs/ai-plans/YYYY-MM-DD_<usuario-github>_<slug>.md`.
4. Atualizar `docs/ai-memory/CHANGELOG_AI.md`.

---

## 3. Como Explorar o Projeto no Obsidian

1. Abra o Obsidian e clique em **Open folder as vault** (*Abrir pasta como cofre*).
2. Selecione a pasta `docs/architecture-vault`.
3. Abra `manual/Visao_Geral_Arquitetura.md` ou `manual/Catalogo_Completo_Scripts.md` para navegação estruturada.
4. Pressione `Ctrl + G` para abrir o **Graph View** e navegar na constelação interativa de nós do jogo.
