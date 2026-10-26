# Integração ao progresso local — 26/10/2026

Resolução do merge Graphify/Obsidian sobre main local `86b7c02`: união de gitignore, preservação das regras de produção, atualização da memória/cofre para C2 até Bocha e consequência, inventário dos 93 C#, índice AST limpo e ferramentas portáveis com validação real e propagação de falhas. Escola adiada, retornos C1, saves/slots, lanterna F, escada/ambientes e correção E preservados. Nenhum arquivo Unity alterado; licença de áudio preexistente mantida fora do merge. [Resultado e testes](../GRAPHIFY-INTEGRACAO-C2.md).

---

> Registro histórico da branch de preparação, baseada em C1 anterior. Alegações antigas de completude/compatibilidade não comprovam o estado atual. Para a integração ao C2, consulte [estado atual](CURRENT_STATE.md) e o relatório de integração.

# Registro de Alterações por Agentes de IA (CHANGELOG_AI)

**Atualizado em:** 26/10/2026
**Por:** Antigravity AI
**Base:** Verificado nos logs de execução e Git history

---

## [2026-10-26] — Alinhamento Integral com Roteiro Universal v2.0 e Prompt Mestre v2 (Multiplataforma Windows + Linux)

### Objetivo da Tarefa
Verificar os documentos de referência do usuário (*Roteiro Universal v2.0* e *Prompt Mestre v2*) e incorporar todos os requisitos faltantes para que os colegas (especialmente em Windows PowerShell e múltiplos agentes como Claude Code, Codex e Cursor) tenham suporte completo.

### Arquivos Criados / Modificados
- **Scripts PowerShell para Windows (`scripts/*.ps1`):**
  - `scripts/graphify-setup.ps1`, `scripts/graphify-refresh.ps1`, `scripts/graphify-validate.ps1`.
- **Governança e Segurança (`AGENTS.md`, `CLAUDE.md`, `.graphifyignore`, `.gitignore`):**
  - Adicionados marcadores `<!-- BEGIN/END memoria-projeto -->` em `AGENTS.md`, diretiva `@AGENTS.md` em `CLAUDE.md`, regras de segurança contra prompt injection, distinção CLI vs. Skill e exclusão de segredos/certificados (`.env*`, `.pem`, `.key`, `.p12`, `.pfx`, `**/.obsidian/workspace*.json`).
- **Documentação (`README.md`, `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`):**
  - Adicionados comandos de instalação via `winget` / PowerShell no Windows, matriz de integração por agente e tabela de solução de problemas.

### Validações Realizadas
1. Execução de `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` (0 erros).
2. Commit e push atualizados na branch `chore/graphify-obsidian-setup`.

---

## [2026-10-26] — Detalhamento Completo dos 44 Scripts no Obsidian/Graphify, Criação do AGENTS.md e Publicação da Branch no GitHub

### Objetivo da Tarefa
Analisar minuciosamente todos os arquivos do projeto do jogo (44 scripts C# / ~15.010 linhas, 5 cenas, 9 zonas/interiores), documentar tudo em detalhes no Graphify e no cofre do Obsidian, criar o arquivo `AGENTS.md` (e `CLAUDE.md`) para obrigar qualquer IA a buscar no Graphify/Obsidian no início de todo prompt e atualizar ambos ao final, atualizar o `README.md` com o passo a passo de instalação/uso para os colegas e publicar a nova branch no GitHub.

### Arquivos Criados / Modificados
- **Governança de Agentes na Raiz:**
  - `AGENTS.md`: Instruções universais obrigatórias de pré-busca (Graphify + Obsidian + `docs/ai-memory/`) no início de cada prompt e atualização/validação ao final.
  - `CLAUDE.md`: Espelho de governança para compatibilidade nativa com Claude Code.
  - `.agents/rules/project-context.md`: Atualizado para exigir leitura das novas notas manuais do Obsidian e execução do ciclo completo em todo prompt.
- **Documentação Detalhada no Cofre Obsidian (`docs/architecture-vault/`):**
  - `docs/architecture-vault/manual/Catalogo_Completo_Scripts.md`: Inventário completo de todos os 44 scripts C#, suas classes, structs, enums e responsabilidades, conectados via wikilinks às notas geradas.
  - `docs/architecture-vault/manual/Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`: Mapa completo das 5 cenas Unity, das 9 zonas/interiores do `Capitulo1Director`, coordenadas UV do radar e cadeia de investigação (`Flags`).
  - `docs/architecture-vault/manual/Guia_Trabalho_Com_IA.md`: Guia prático de comandos e fluxo de trabalho da equipe com IA.
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md` e `manual/README.md` e `docs/architecture-vault/README.md`: Expandidos com todos os 7 subsistemas e instruções multiplataforma.
- **Memória Persistente e Guia do Repositório:**
  - `README.md`: Adicionada seção completa *"Desenvolvimento com IA: Graphify + Obsidian + Memória Persistente"* explicando instalação (`uv tool install graphifyy`, abertura do vault no Obsidian) e fluxo automático com IA.
  - `docs/ai-memory/ARCHITECTURE.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/ai-memory/CHANGELOG_AI.md`: Atualizados com o inventário integral dos 44 scripts.

### Validações Realizadas
1. Execução de `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` com 0 erros.
2. Nenhum arquivo de código C# ou cena do Unity foi alterado indevidamente pela documentação.
3. Commit e push da branch `chore/graphify-obsidian-setup` para o repositório remoto no GitHub.

---

## [2026-10-26] — Setup Inicial: Antigravity + Graphify + Obsidian

### Objetivo da Tarefa
Estruturar o projeto com o grafo de conhecimento Graphify, cofre Markdown navegável para Obsidian e memória persistente em disco para que futuros agentes de IA operem com contexto preservado e leitura prévia estruturada.

### Arquivos Criados / Modificados
- **Configurações e Governança:**
  - `.graphifyignore`: Filtro de segurança descartando 1.001 imagens e 18 vídeos da indexação em massa.
  - `.agents/rules/graphify.md`: Regra nativa do Antigravity para priorização do grafo.
  - `.agents/workflows/graphify.md`: Definição de workflow `/graphify`.
  - `.agents/rules/project-context.md`: Regra de governança de contexto e preservação do código.
- **Grafo e Índices (`graphify-out/`):**
  - `graphify-out/graph.json`, `graphify-out/GRAPH_REPORT.md`, `graphify-out/graph.html`.
- **Cofre Obsidian (`docs/architecture-vault/`):**
  - `docs/architecture-vault/README.md`, `manual/` e `generated/` (`graph.canvas` + notas atômicas).
- **Memória Persistente (`docs/ai-memory/`):**
  - `INDEX.md`, `PROJECT_CONTEXT.md`, `ARCHITECTURE.md`, `DECISIONS.md`, `CURRENT_STATE.md`, `CHANGELOG_AI.md`.
- **Scripts de Operação (`scripts/`):**
  - `graphify-setup.sh`, `graphify-refresh.sh`, `graphify-validate.sh`.
- **Documentação:**
  - `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md`, `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`.
