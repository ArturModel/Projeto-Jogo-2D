# Plano Executado — Auditoria Pós-Merge da Main e Governança de Planos por Autor

- **Autor (GitHub):** @lucamontini (Luca Dalla Lana)
- **Data e Hora:** 2026-10-27 13:30 (BRT)
- **Status:** Concluído com Sucesso
- **Ambiente/Plataforma:** Linux (Ubuntu), Graphify 0.9.96, Python 3.12, Git, gh CLI

---

## 1. Solicitação do Usuário

> "Faça um git pull na branch main. Meu colega fez o merge da minha branch onde colocamos o graphify e o obsidian, mas como tinha dado incosistencia em um dos planos, que já havia sido feito e nós não pegamos, ele pediu para a IA corrigir. Quero que com base na nossa branch "chore..." onde implementamos essa questão, você analise as diferenças com a main atual e me diga o que pode ter sido perdido, caso não tenha nada sido perdido, ótimo. O importante é o graphify e o obsdian funcionarem, todo novo prompt a IA consultar esses dois e no final de cada prompt atualizar ambos e criar um documento do plano que ela executou naquele prompt, para que se torne um historico do que cada um fez, seu nome(do github) e a data"

---

## 2. Diagnóstico e Contexto Consultado

1. **Git Fetch & Comparação:**
   - Merge base entre `chore/graphify-obsidian-setup` (`6a9d7c1`) e `origin/main` (`434b320`, autor: Patrick Dotto).
   - Inspecionados commits: `86b7c02` (capitulo_2) e `434b320` (merge commit).
2. **Documentação consultada:**
   - `docs/GRAPHIFY-INTEGRACAO-C2.md` (relatório elaborado por Patrick e sua IA).
   - `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md` e `docs/ai-memory/CURRENT_STATE.md`.
   - `docs/architecture-vault/manual/` (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`).
3. **Graphify CLI:**
   - Executados testes de consulta: `graphify query "GameState"`, `graphify query "BochaSimulation"`.

---

## 3. Análise Detalhada das Diferenças e o que Houve no Merge

### 3.1 O que causou a inconsistência mencionada
- Na nossa branch `chore/graphify-obsidian-setup`, indexamos e documentamos o projeto quando tínhamos a visão de **44 scripts C# (foco no Capítulo 1)**, assumindo em planos e notas que o Capítulo 2 era trabalho futuro pendente.
- Na branch `capitulo_2` do colega Patrick (`86b7c02`), o **Capítulo 2 já havia sido implementado em código até o minigame da Bocha** (totalizando **93 scripts C#**).
- Ao fazer o merge, a IA do Patrick percebeu que nosso plano estava "defasado" em relação ao progresso real do código. Ela então reindexou o grafo e atualizou a documentação para refletir o estado real de 93 scripts.

### 3.2 O que NÃO foi perdido
- **Código e Assets:** Absolutamente nenhum script C# ou asset foi perdido. Toda a implementação de C1 e C2 está intacta na `main`.
- **Scripts de Automação:** Patrick criou `scripts/graphify-project.py`, que unifica PowerShell e Bash, adicionando validação rigorosa de hashes SHA-256 e detecção de caminhos portáveis.
- **Grafo e Cofre:** Todos os 93 scripts estão no grafo e no cofre gerado do Obsidian.

### 3.3 O que foi PERDIDO ou ENFRAQUECIDO no Merge (e corrigido agora)
1. **Governança Imperativa em `AGENTS.md`:** A IA do colega substituiu os checklists detalhados e numerados (Passos 1.1–1.3 e Passos 3.1–3.3) por parágrafos gerais. Além disso, ela havia flexibilizado a atualização ao dizer *"Após mudanças estruturais de código"*, o que abria brecha para agentes pularem o refresh em tarefas menores.
2. **Regras de Agentes:** `.agents/rules/project-context.md` e `graphify.md` foram reduzidos a 3 linhas genéricas, perdendo o gatilho direto dos comandos de busca.
3. **Versão do Graphify:** Patrick fixou o script em `graphifyy==0.9.96`. Máquinas locais que estavam na versão anterior (`0.9.67`) falhavam ao rodar `./scripts/graphify-refresh.sh`.
4. **Registro de Histórico de Planos:** Não existia no repositório o mecanismo de salvar a cada prompt o plano executado com data e usuário do GitHub.

---

## 4. Plano de Ação Executado

1. [x] **Git Pull da Branch `main`:**
   - Stash de modificações locais temporárias de cena/line-endings.
   - `git checkout main && git pull origin main` (atualizado para `434b320`).
2. [x] **Atualização do Ambiente Local:**
   - Executado `uv tool install --upgrade graphifyy==0.9.96` para alinhar com a versão fixada.
3. [x] **Criação do Sistema de Planos por Autor (`docs/ai-plans/`):**
   - Criado `docs/ai-plans/README.md` contendo o padrão de nomenclatura `YYYY-MM-DD_<usuario-github>_<slug>.md` e o template obrigatório.
   - Criado este primeiro plano histórico de referência.
4. [x] **Restauração e Blindagem da Governança Multi-Agente:**
   - **`AGENTS.md`:** Restaurada a estrutura mandatória completa, mantendo integralmente as regras de C1/C2 de Patrick, mas impondo o ciclo de 3 passos no início do prompt, consulta Graphify/Obsidian, atualização de ambos no final e criação obrigatória do documento de plano.
   - **`.agents/rules/project-context.md` e `graphify.md`:** Reescritos com instruções estritas para agentes (Cursor, Codex, Antigravity, Copilot).
   - **`docs/architecture-vault/manual/Guia_Trabalho_Com_IA.md`:** Expandido com tutorial completo de colaboração IA + desenvolvedor.
   - **`docs/ai-memory/INDEX.md`:** Atualizado com link para os novos planos.
5. [x] **Validação Completa de Integridade:**
   - Executado `./scripts/graphify-refresh.sh` com sucesso.
   - Executado `./scripts/graphify-validate.sh` com sucesso (93 scripts C# cobertos, 0 erros).

---

## 5. Arquivos Modificados / Criados

- `AGENTS.md` — Restaurada governança rigorosa, ciclo por prompt e obrigatoriedade de criação de plano com usuário GitHub e data.
- `.agents/rules/project-context.md` — Regra imperativa de início e fim de prompt.
- `.agents/rules/graphify.md` — Diretrizes de consulta e atualização do grafo.
- `docs/ai-plans/README.md` (Novo) — Especificação e template de planos por autor.
- `docs/ai-plans/2026-10-27_lucamontini_merge-main-auditoria-governanca.md` (Novo) — Registro histórico deste prompt.
- `docs/architecture-vault/manual/Guia_Trabalho_Com_IA.md` — Manual didático completo.
- `docs/ai-memory/INDEX.md` — Adicionado apontamento para o diretório de planos.
- `docs/ai-memory/CHANGELOG_AI.md` — Registrada a auditoria e implementação do sistema de planos.

---

## 6. Validação e Testes

- **Graphify Refresh:**
  ```bash
  ./scripts/graphify-refresh.sh
  ```
  *Resultado:* PASS — 93 scripts C# indexados, cofre em `docs/architecture-vault/generated/` atualizado (1.612 notas e canvas), `graphify-out/graph.html` gerado.
- **Graphify Validate:**
  ```bash
  ./scripts/graphify-validate.sh
  ```
  *Resultado:* PASS — 93 C# sources covered; portable paths; current hashes; canvas/HTML/report; four real queries (`GameState`, `Capitulo2Portal`, `BochaSimulation`, `SantaRitaShallowStairs`).

---

## 7. Próximos Passos e Recomendações

- **Para o time:** Todos devem rodar `uv tool install --upgrade graphifyy==0.9.96` se ainda não tiverem feito, para garantir paridade exata com os scripts.
- **Para qualquer IA em prompts futuros:** Seguir sempre o ciclo:
  1. Consultar Graphify + Obsidian no início.
  2. Executar alterações.
  3. Rodar `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` no final.
  4. Gerar o arquivo em `docs/ai-plans/YYYY-MM-DD_<usuario-github>_<slug>.md`.
