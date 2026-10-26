# Guia de Trabalho com IA (Graphify + Obsidian) — Santa Rita

**Atualizado em:** 26/10/2026  
**Público-alvo:** Desenvolvedores, artistas e designers do projeto *Santa Rita — A Noite das 03:17*.

---

## 1. Como o Graphify, o Obsidian e a IA Trabalham Juntos

Neste repositório, nenhuma IA (Claude Code, Cursor, Antigravity, Copilot, Codex, Windsurf) deve adivinhar a arquitetura ou ler arquivos às cegas. O fluxo é guiado pelo arquivo **`AGENTS.md`** (na raiz do repositório) e pelas regras em **`.agents/rules/`**:

1. **Antes de responder a qualquer prompt técnico:**
   - A IA lê o estado atual em `docs/ai-memory/INDEX.md` e `docs/ai-memory/CURRENT_STATE.md`.
   - A IA consulta as notas manuais do cofre Obsidian em `docs/architecture-vault/manual/`.
   - A IA executa consultas estruturais no grafo com `graphify query`, `graphify path` ou `graphify explain`.
2. **Durante a implementação:**
   - A IA abre apenas os arquivos C# exatos apontados pelo grafo e respeita os contratos canônicos (Save Manual exclusivo em `[[PayphoneCheckpoint]]`, estado em RAM em `[[GameState]]`, escala `PPU 32` e `[[UIFactory]]`).
3. **Ao finalizar qualquer alteração:**
   - A IA executa `./scripts/graphify-refresh.sh` para atualizar o grafo (`graphify-out/`) e regenerar as notas atômicas do Obsidian (`docs/architecture-vault/generated/`).
   - A IA atualiza as notas de arquitetura em `docs/architecture-vault/manual/` e a memória em `docs/ai-memory/` (`CURRENT_STATE.md`, `CHANGELOG_AI.md`, `ARCHITECTURE.md`, `DECISIONS.md`).
   - A IA valida tudo com `./scripts/graphify-validate.sh`.

---

## 2. Comandos Úteis do Graphify no Dia a Dia

Você (ou a IA) pode rodar estes comandos no terminal na raiz do repositório:

```bash
# Fazer uma pergunta aberta sobre a arquitetura (busca BFS no grafo)
graphify query "Como funciona o sistema de save manual no orelhao?"

# Descobrir o menor caminho entre duas classes ou conceitos
graphify path "SinucaMinigameUI" "CheckpointSystem" --undirected

# Explicar todas as conexões de entrada e saída de uma classe específica
graphify explain "Capitulo1Director"

# Atualizar o grafo, o cofre do Obsidian e o graph.html após editar código C#
./scripts/graphify-refresh.sh

# Validar se o grafo, o canvas do Obsidian e as regras de exclusão estão íntegros
./scripts/graphify-validate.sh
```

---

## 3. Como Explorar o Projeto no Obsidian

1. Abra o Obsidian e clique em **Open folder as vault** (*Abrir pasta como cofre*).
2. Escolha a pasta `docs/architecture-vault` dentro do repositório.
3. **Onde ler e escrever:**
   - **`manual/`**: Contém as notas escritas para humanos e IAs lerem rapidamente:
     - [[Visao_Geral_Arquitetura]]
     - [[Catalogo_Completo_Scripts]]
     - [[Mapa_Cenas_Zonas_E_Fluxo_Cap1]]
     - [[Guia_Trabalho_Com_IA]]
   - **`generated/`**: Contém mais de 1.600 notas geradas automaticamente (uma para cada classe, método, enum e documento) e o mapa visual `generated/graph.canvas`. **Nunca edite arquivos manualmente dentro de `generated/`**, pois eles são recriados pelo `graphify-refresh.sh`.
4. **Dica de Ouro (Graph View):** Pressione `Ctrl + G` no Obsidian para abrir a visualização em constelação de todo o código do jogo.
