# Manual de Operação e Governança — Graphify + Obsidian + Agentes de IA

**Projeto:** Santa Rita — A Noite das 03:17  
**Data:** 26/10/2026  
**Versão:** 2.0 (Compatível com Windows PowerShell, Linux Bash, macOS e WSL2)  

---

## 1. Visão Geral do Ecossistema e Separação de Camadas

Este ecossistema conecta três camadas de inteligência arquitetural ao projeto:
1. **Graphify (`graphify-out/`):** Índice estrutural de código C# e contratos em grafo com busca BFS/DFS via CLI (`graphify query`, `graphify path`, `graphify explain`).
2. **Obsidian (`docs/architecture-vault/`):** Cofre visual navegável com notas atômicas regeneráveis em `generated/` (`graph.canvas` incluso) e notas de arquitetura perenes em `manual/`.
3. **Governança de Agentes de IA (`AGENTS.md`, `CLAUDE.md`, `.agents/rules/`, `docs/ai-memory/`):** Instruções canônicas e memória persistente que obrigam qualquer agente a consultar o Graphify e o Obsidian no início de cada prompt e atualizá-los ao final.

### Três Conceitos que Evitam Erros Comuns:
| Camada | Onde roda | Comandos Principais | Observação |
| :--- | :--- | :--- | :--- |
| **CLI `graphify`** | Terminal (PowerShell / Bash) | `graphify query`, `graphify path`, `graphify explain`, `graphify update .`, `graphify export obsidian`, `graphify export html` | Pacote oficial no PyPI: **`graphifyy`** (com dois "y"). Atualização AST de código C# é 100% local e sem custo de tokens. |
| **Skill do Agente** | Chat da IA | `/graphify .` (Claude Code, Antigravity, Cursor) ou `$graphify .` (Codex) | Constrói/atualiza a parte semântica (docs Markdown). Aceita flags da skill como `--update`, `--obsidian`, `--mode deep`. |
| **Integração por Plataforma** | Terminal (1x por agente) | `graphify antigravity install`, `graphify claude install`, `graphify codex install`, `graphify cursor install` | Grava regras e hooks nativos para o agente que cada desenvolvedor preferir usar. |

---

## 2. Início de Todo Novo Prompt (Ciclo Obrigatório da IA)

Conforme definido em [`AGENTS.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/AGENTS.md) e [`CLAUDE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/CLAUDE.md):
1. **Recuperar Contexto:** Ler [`docs/ai-memory/INDEX.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/ai-memory/CURRENT_STATE.md), além das notas manuais do cofre em [`docs/architecture-vault/manual/`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/docs/architecture-vault/manual/).
2. **Formular Pergunta e Consultar o Grafo:**
   ```bash
   graphify query "Onde o Minigame de Sinuca salva a vitoria no GameState?"
   graphify path "SinucaMinigameUI" "GameState" --undirected
   graphify explain "PayphoneCheckpoint"
   ```
3. **Confirmar no Código-Fonte:** Consultar as linhas exatas indicadas pelo grafo em `Projeto Jogo 2D/Assets/Scripts/` antes de qualquer edição. O código C# é sempre a fonte da verdade.

---

## 3. Como Abrir e Utilizar no Obsidian

1. Abra o Obsidian Desktop (Windows, macOS ou Linux).
2. Selecione **"Abrir pasta como cofre"** (*Open folder as vault*).
3. Selecione a pasta `docs/architecture-vault` dentro do repositório.
4. **Camadas do Cofre:**
   - **`manual/` (Sagrada / Humana & Arquitetural):** Contém `Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md` e `Guia_Trabalho_Com_IA.md`. Nunca é sobrescrita por scripts automáticos.
   - **`generated/` (Recriável):** Contém as 1.700+ notas atômicas e `generated/graph.canvas`.

---

## 4. Atualização e Validação Pós-Execução (Linux/macOS e Windows)

Sempre que adicionar ou alterar scripts C# ou documentação arquitetural:

### No Linux / macOS / WSL (Bash):
```bash
./scripts/graphify-refresh.sh
./scripts/graphify-validate.sh
```

### No Windows (PowerShell):
```powershell
.\scripts\graphify-refresh.ps1
.\scripts\graphify-validate.ps1
```

---

## 5. Integração com Outros Agentes da Equipe

O repositório já traz `.agents/` (Antigravity), `AGENTS.md` (Codex/Cursor/Windsurf/Copilot) e `CLAUDE.md` (Claude Code). Se um colega quiser ativar os hooks nativos do agente específico dele:
- **Claude Code:** `graphify claude install`
- **OpenAI Codex:** `graphify codex install`
- **Cursor:** `graphify cursor install`
- **GitHub Copilot / VS Code:** `graphify copilot install` ou `graphify vscode install`
*(Sempre revisar o `git diff` após rodar comandos de instalação para preservar o bloco `<!-- BEGIN memoria-projeto -->` do `AGENTS.md`).*

---

## 6. Solução de Problemas Comuns (Troubleshooting)

| Sintoma | Causa Provável | Solução |
| :--- | :--- | :--- |
| `graphify: command not found` | Diretório de binários do `uv` fora do `PATH` | Rode `uv tool update-shell` e reabra o terminal (ou adicione `~/.local/bin` ao `PATH`). |
| Script `.ps1` bloqueado no Windows | `ExecutionPolicy` restrita do PowerShell | Rode `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` ou execute `powershell -ExecutionPolicy Bypass -File .\scripts\graphify-refresh.ps1`. |
| Mudanças no código C# não aparecem no grafo | Esqueceu de rodar o refresh após editar | Rode `./scripts/graphify-refresh.sh` (Bash) ou `.\scripts\graphify-refresh.ps1` (PowerShell). |
| Repositório lento no WSL2 | Projeto clonado dentro de `/mnt/c/...` | Se usar Windows nativo, use PowerShell nativo; se usar WSL2, clone dentro do sistema de arquivos Linux (`~/...`). |

---

## 7. Procedimento de Rollback e Recuperação

1. **Recuperação simples do índice:**
   ```bash
   rm -rf graphify-out/.graphify_*
   ./scripts/graphify-refresh.sh && ./scripts/graphify-validate.sh
   ```
2. **Reversão total da branch:**
   ```bash
   git checkout main
   git branch -D chore/graphify-obsidian-setup
   ```
