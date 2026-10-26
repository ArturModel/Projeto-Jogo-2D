# AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente)

Este repositório (**Santa Rita — A Noite das 03:17**, Unity `6000.6.0f1` / URP 2D) utiliza um ecossistema integrado de conhecimento arquitetural composto por **Graphify** (`graphify-out/`), **Cofre Obsidian** (`docs/architecture-vault/`) e **Memória Persistente de IA** (`docs/ai-memory/`).

> **ATENÇÃO AGENTE DE IA (Antigravity, Claude Code, Cursor, GitHub Copilot, Windsurf, Codex):**  
> As regras abaixo são **OBRIGATÓRIAS** em **TODO NOVO PROMPT** recebido neste repositório, sem exceção.

---

## 1. ANTES de Responder ou Alterar Qualquer Código (Início de Todo Prompt)

Em **todo novo prompt** (seja uma pergunta, investigação de bug, nova feature ou refatoração), você **DEVE** executar os 3 passos abaixo **antes** de vasculhar pastas às cegas ou editar arquivos:

### Passo 1.1 — Ler a Memória Persistente e o Cofre Obsidian
1. Leia [`docs/ai-memory/INDEX.md`](docs/ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](docs/ai-memory/CURRENT_STATE.md).
2. Consulte as notas arquiteturais do **Obsidian** em [`docs/architecture-vault/manual/`](docs/architecture-vault/manual/):
   - [`Visao_Geral_Arquitetura.md`](docs/architecture-vault/manual/Visao_Geral_Arquitetura.md) — Mapa dos subsistemas e contratos de domínio.
   - [`Catalogo_Completo_Scripts.md`](docs/architecture-vault/manual/Catalogo_Completo_Scripts.md) — Inventário de todos os 44 scripts C#, classes, structs e enums.
   - [`Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`](docs/architecture-vault/manual/Mapa_Cenas_Zonas_E_Fluxo_Cap1.md) — Topologia das 5 cenas, 9 zonas/interiores e cadeia de flags.
   - Se precisar inspecionar as conexões de um símbolo específico no Obsidian, consulte sua nota atômica em `docs/architecture-vault/generated/<Simbolo>.md`.

### Passo 1.2 — Executar Busca no Graphify
Formule uma consulta específica sobre os símbolos, classes ou fluxos envolvidos no prompt e execute o **Graphify CLI** (se `graphify` não estiver no `PATH` padrão, inclua `export PATH="$HOME/.local/bin:$PATH"`):

```bash
# Busca em largura (BFS) por conceitos, classes ou métodos relacionados à tarefa
graphify query "<pergunta ou palavras-chave da tarefa>"

# Menor caminho entre duas classes/entidades no grafo
graphify path "<ClasseOuSimboloA>" "<ClasseOuSimboloB>" --undirected

# Explicação detalhada das conexões de um nó específico
graphify explain "<NomeDaClasseOuMetodo>"
```

### Passo 1.3 — Confirmar no Código-Fonte C# Real
- Use os caminhos e linhas (`src=... loc=L...`) retornados pelo Graphify e pelo Obsidian para abrir **apenas os trechos necessários** dos arquivos C# originais em `Projeto Jogo 2D/Assets/Scripts/` ou `Assets/Editor/`.
- **Fonte da Verdade:** Nunca altere código baseando-se apenas no resumo do grafo; confirme assinaturas e tipos no arquivo `.cs`.

---

## 2. Regras Inegociáveis de Domínio do Jogo (Revisão Canônica R1)

Ao escrever ou modificar código neste projeto, respeite sempre:
1. **Save Manual Exclusivo no Orelhão:** `GameState.cs` opera **100% em RAM** durante a sessão. Jamais grave progresso da partida automaticamente em `PlayerPrefs` ou disco ao marcar flags, coletar pistas, trocar de cena ou vencer o boss. Apenas `CheckpointSystem.SaveSlot()` / `Save()` (acionado nos Orelhões via `PayphoneCheckpoint.cs`) persiste o `GameStateSnapshot`.
2. **Proibição de Spoiler de Lucas no Capítulo 1:** Nunca adicione textos, diálogos, nomes de flags visíveis ou entradas de diário revelando que Lucas sobreviveu a 1998.
3. **Linha do Tempo R1:** 1990 (tragédia de Júlia), 1998 (acidente às 03:17 no Clube e partida às 03:19), 2004 (retorno de Marcus em carro próprio porque o avô Adílio parou de atender **no mesmo dia**).
4. **Escala Visual e UI:** `PPU = 32`, câmera `512×288` (`CameraFollow` com `AplicarClampBordas`), Canvas `640×360` via `UIFactory.cs`.
5. **Isolamento de Mídias no Grafo:** Nunca remova as regras do `.graphifyignore` (os 1.000+ arquivos `.png` e `.mp4` jamais devem entrar no scan semântico do Graphify).

---

## 3. AO FINAL de Cada Execução (Pós-Modificação Obrigatória)

Sempre que você criar, editar ou remover qualquer script C#, documentação ou configuração durante a execução de um prompt, você **DEVE** executar este checklist antes de encerrar sua resposta ao usuário:

### Passo 3.1 — Atualizar o Graphify e o Cofre Gerado do Obsidian
Execute o script idempotente de atualização (que roda `graphify update .`, reexporta `docs/architecture-vault/generated/` com o `graph.canvas` e atualiza `graphify-out/graph.html`, sem custo de API):

```bash
./scripts/graphify-refresh.sh
```

### Passo 3.2 — Atualizar as Notas Manuais do Obsidian e a Memória Persistente
1. **Cofre Obsidian (`docs/architecture-vault/manual/`):**
   - Se criou/alterou classes, métodos públicos, cenas, zonas ou flags, atualize [`Catalogo_Completo_Scripts.md`](docs/architecture-vault/manual/Catalogo_Completo_Scripts.md), [`Visao_Geral_Arquitetura.md`](docs/architecture-vault/manual/Visao_Geral_Arquitetura.md) e/ou [`Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`](docs/architecture-vault/manual/Mapa_Cenas_Zonas_E_Fluxo_Cap1.md) mantendo os wikilinks `[[NomeDaClasse]]`.
2. **Memória de IA (`docs/ai-memory/`):**
   - Atualize [`docs/ai-memory/CURRENT_STATE.md`](docs/ai-memory/CURRENT_STATE.md) com o novo estado do projeto.
   - Registre a intervenção no [`docs/ai-memory/CHANGELOG_AI.md`](docs/ai-memory/CHANGELOG_AI.md) (data, objetivo, arquivos tocados e validação).
   - Se houve mudança estrutural ou nova decisão técnica, atualize [`docs/ai-memory/ARCHITECTURE.md`](docs/ai-memory/ARCHITECTURE.md) e [`docs/ai-memory/DECISIONS.md`](docs/ai-memory/DECISIONS.md).

### Passo 3.3 — Validar a Integridade
Rode o validador automático e confirme que todos os testes passam (`0 erros`):

```bash
./scripts/graphify-validate.sh
```
