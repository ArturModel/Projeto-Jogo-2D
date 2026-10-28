# Diagnóstico e Resolução de Conflito de Porta (8080) MCP Unity — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_diagnostico-conflito-porta-8080-mcp.md`](../ai-plans/2026-10-28_lucamontini_diagnostico-conflito-porta-8080-mcp.md)
- **Resumo:** Diagnosticada a causa do erro de conexão do MCP no Unity: a porta `8080` estava sendo ocupada pelo backend Java Spring Boot (`BffKickstartApplication`, PID 124682) do desenvolvedor. A sonda TCP do Unity detectava a porta aberta pelo Spring Boot, assumia falsamente que o servidor MCP estava pronto e tentava handshake WebSocket em `/hub/plugin`, recebendo HTTP 404 da aplicação Java. Alterada a porta padrão do MCP em `SantaRitaMcpSetup.cs` para a porta dedicada `8088`, eliminando qualquer colisão com servidores locais de trabalho. Validação Graphify e Obsidian com status PASS.

---

# Correção de Race Condition e Suporte Multiplataforma MCP Unity — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_correcao-race-condition-mcp-linux.md`](../ai-plans/2026-10-27_lucamontini_correcao-race-condition-mcp-linux.md)
- **Resumo:** Refatorado `SantaRitaMcpSetup.cs` para suportar nativamente Linux/macOS e Windows. Eliminada a condição de corrida (*race condition*) que causava o erro vermelho `[WebSocket] Connection failed` e aviso `Failed to start MCP transport: Http` na abertura do Unity. O bootstrap agora apenas ajusta o ambiente (`~/.local/bin/uvx`, separador `:`, remoção de `.exe`) e delega a conexão ordenada ao `HttpAutoStartHandler` nativo do pacote MCP. Validação Graphify e Obsidian com status PASS.

---

# Correção de Erros e Avisos de Abertura no Unity 6 (Linux) — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_correcao-warnings-e-erros-unity6.md`](../ai-plans/2026-10-27_lucamontini_correcao-warnings-e-erros-unity6.md)
- **Resumo:** Criação da branch isolada `fix/unity6-startup-warnings-errors`. Resolvidos avisos de API obsoleta `FindObjectsSortMode` (CS0618) no Unity 6, campos não lidos (CS0414), diretiva `DEVELOPMENT_BUILD` (UAC0009 migrada para `DEBUG`), aviso UAC0005 no MCP setup, metadados da fonte TextMeshPro (v4), transcodificação de vídeo para Linux e eliminação dos erros críticos e NullReferenceExceptions do Plastic SCM / Unity Version Control (configurado `m_Mode: Visible Meta Files`, removido `com.unity.collab-proxy` e pasta residual `.plastic/`). Validação completa Graphify e Obsidian com status PASS.

---

# Resolução de Espelho APT, Instalação do Git LFS e Checkout de Assets — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_instalacao-git-lfs-e-pull-assets.md`](../ai-plans/2026-10-27_lucamontini_instalacao-git-lfs-e-pull-assets.md)
- **Resumo:** Resolução de erro HTTP 403 do espelho APT `mirror.uepg.br`. Baixado pacote oficial diretamente de `archive.ubuntu.com`, extraído e instalado `git-lfs` em `~/.local/bin/git-lfs`. Inicializado LFS (`git lfs install`), baixados 1.438 objetos (167 MB) via `git lfs pull` e realizado checkout de 1.769 objetos (172 MB) via `git lfs checkout`. Texturas e áudios validados como binários reais intactos. Projeto 100% pronto para abertura no Unity 6000.6.0f1. Validação de governança Graphify e Obsidian com status PASS.

---

# Verificação de Branch e Pré-Requisitos para Abertura no Unity — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_verificacao-branch-e-pre-requisitos-unity.md`](../ai-plans/2026-10-27_lucamontini_verificacao-branch-e-pre-requisitos-unity.md)
- **Resumo:** Verificação da branch ativa (`main`) e auditoria de pré-requisitos antes de abrir o Unity. Detectada ausência do pacote `git-lfs` no Linux Mint com assets binários presentes apenas como ponteiros de ~130 bytes; estruturado passo a passo de correção (`sudo apt install git-lfs`, `git lfs install`, `git lfs pull`) para evitar corrupção de importação na biblioteca do Unity 6000.6.0f1. Validação de governança Graphify e Obsidian concluída com PASS.

---

# Auditoria Pós-Merge, Governança Reforçada e Histórico de Planos — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_merge-main-auditoria-governanca.md`](../ai-plans/2026-10-27_lucamontini_merge-main-auditoria-governanca.md)
- **Resumo:** Pull da `main` realizado após merge de Patrick Dotto. Análise de diferenças confirmou que nenhum código ou asset foi perdido (C1 + C2 intactos com 93 scripts). Corrigida a diluição das regras de governança que havia ocorrido no merge: restaurado checklist obrigatório em `AGENTS.md` e regras de agente (`.agents/rules/`). Implementado sistema obrigatório de registro de planos executados por prompt em `docs/ai-plans/` contendo autor do GitHub e data. Graphify local atualizado para `0.9.96` e validado com sucesso (`./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` PASS).

---

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
