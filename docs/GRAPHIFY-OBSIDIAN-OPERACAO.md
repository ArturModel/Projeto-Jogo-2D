# Manual de Operação e Governança — Graphify + Obsidian + Antigravity

**Projeto:** Santa Rita — A Noite das 03:17  
**Data:** 26/10/2026  
**Versão:** 1.0  

---

## 1. Visão Geral do Ecossistema

Este ecossistema conecta três camadas de inteligência arquitetural ao projeto:
1. **Graphify (`graphify-out/`):** Índice estrutural de código C# e contratos em grafo com busca BFS/DFS via CLI (`graphify query`, `graphify path`).
2. **Obsidian (`docs/architecture-vault/`):** Cofre visual navegável com notas atômicas em `generated/`, notas de design perenes em `manual/` e canvas visual (`graph.canvas`).
3. **Antigravity AI (`docs/ai-memory/` + `.agents/`):** Memória persistente e regras de projeto que forçam qualquer agente a consultar a arquitetura antes de alterar código.

---

## 2. Início de uma Nova Sessão de Agente

Ao iniciar uma nova sessão ou ao receber uma tarefa complexa:
1. **Recuperar Contexto:** O agente deve ler [`docs/ai-memory/INDEX.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CURRENT_STATE.md).
2. **Formular Pergunta e Consultar o Grafo:**
   ```bash
   graphify query "Onde o Minigame de Sinuca salva a vitoria no GameState?"
   graphify path "SinucaMinigameUI" "GameState" --undirected
   graphify explain "PayphoneCheckpoint"
   ```
3. **Validar no Código-Fonte:** O agente consulta as linhas exatas indicadas pelo grafo em `Projeto Jogo 2D/Assets/Scripts/` para confirmar assinaturas e fluxos reais antes de qualquer edição.

---

## 3. Como Abrir e Utilizar no Obsidian

1. Abra o Obsidian no Linux (`flatpak run md.obsidian.Obsidian` ou pelo lançador gráfico).
2. Selecione **"Abrir pasta como cofre"** (*Open folder as vault*).
3. Selecione a pasta:
   ```text
   /home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/architecture-vault
   ```
4. **Visualizações Recomendadas:**
   - **Canvas:** Clique duas vezes em `generated/graph.canvas` para ver os cartões interativos.
   - **Graph View:** Pressione `Ctrl + G` para ver o grafo geral de conexões.
   - **Notas Manuais:** Crie suas notas em `manual/` usando `[[NomeDaClasse]]` para linkar diretamente às classes do jogo.

---

## 4. Atualização do Grafo (Pós-Alteração de Código)

Sempre que adicionar ou alterar scripts C# ou regras de jogo:
```bash
./scripts/graphify-refresh.sh
```
O script atualiza o grafo via AST (sem custo de API de LLM), sincroniza as notas do Obsidian e atualiza o `graph.html`.

Para validar a integridade após a atualização:
```bash
./scripts/graphify-validate.sh
```

---

## 5. Política de Versionamento Git

Para manter o repositório enxuto e seguro:

### Arquivos Versionados no Git:
- `docs/ai-memory/` (toda a memória de contexto do agente).
- `docs/architecture-vault/manual/` (notas manuais da equipe).
- `docs/architecture-vault/README.md` (manual do cofre).
- `.agents/` (regras e workflows do Antigravity).
- `scripts/` (scripts utilitários).
- `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md` e `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`.
- `graphify-out/graph.json` e `graphify-out/GRAPH_REPORT.md` (compartilháveis com outros membros e IAs).

### Arquivos Ignorados no Git (`.gitignore`):
- `graphify-out/.graphify_*` (arquivos temporários de pipeline).
- `docs/architecture-vault/.obsidian/workspace*.json` (preferências locais da interface do Obsidian).
- `docs/architecture-vault/.obsidian/cache/` (cache interno do Obsidian).

---

## 6. Procedimento de Rollback e Recuperação de Falhas

Se o índice ou o cofre sofrerem qualquer inconsistência:
1. **Recuperação simples:**
   ```bash
   rm -rf graphify-out/.graphify_*
   ./scripts/graphify-refresh.sh
   ./scripts/graphify-validate.sh
   ```
2. **Reversão total da branch:**
   Caso queira descartar todas as configurações de IA e voltar ao estado original da `main`:
   ```bash
   git checkout main
   git branch -D chore/graphify-obsidian-setup
   ```
   *Nota: O código do jogo e as cenas Unity nunca são afetados por este rollback.*
