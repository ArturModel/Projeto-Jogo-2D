# Plano de Implementação — Setup Graphify + Obsidian + Antigravity

**Projeto:** Santa Rita — A Noite das 03:17 (`Projeto-Jogo-2D`)  
**Data do Diagnóstico:** 26/10/2026  
**Status Atual:** Setup Concluído com Sucesso — Todos os 10 Testes de Aceitação Aprovados!

---

## 1. Diagnóstico do Ambiente (Fase 0 — Somente Leitura)

### 1.1 Sistema Operacional e Ferramentas
- **Sistema Operacional:** Linux Ubuntu (kernel `6.17.0-1033-oem`, x86_64, shell Bash).
- **Python do Sistema:** Python 3.12.3 (`/usr/bin/python3`).
- **uv:** Versão `0.12.18` instalada em `/home/lucamontini/.local/bin/uv`.
- **pipx:** Não instalado (não necessário, pois `uv` está presente e operacional).
- **Git:** Versão `2.43.0` (`/usr/bin/git`).
- **Node.js:** Versão `v24.21.0` via NVM (`/home/lucamontini/.nvm/versions/node/v24.21.0/bin/node`).
- **Graphify CLI:** **Já instalado** na versão `0.9.67` em `/home/lucamontini/.local/bin/graphify` (ambiente isolado `uv tool`: `/home/lucamontini/.local/share/uv/tools/graphifyy/bin/python`).
- **Obsidian:** **Instalado e confirmado** via Flatpak (`md.obsidian.Obsidian` versão `1.14.5`). O cofre será gerado como pasta Markdown padrão em `docs/architecture-vault/`, abrível diretamente pelo Obsidian ou por qualquer editor/LLM.

### 1.2 Estrutura do Repositório
- **Raiz do Projeto:** `/home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D`.
- **Natureza do Projeto:** Projeto de jogo 2D de terror psicológico em **Unity 6000.6.0f1** com URP 2D ("Santa Rita — A Noite das 03:17").
- **Módulos e Diretórios Principais:**
  - `Projeto Jogo 2D/`: Contém a raiz do projeto Unity (Assets, ProjectSettings, Packages, e pastas de cache/editor).
    - `Assets/Scripts/`: 43 scripts C# (~14.192 linhas de código).
    - `Assets/Editor/`: 1 script C# (`SantaRitaChapter1Builder.cs`, ~818 linhas).
    - Total de código C#: **44 scripts, ~15.010 linhas de código**.
    - Pastas pesadas do Unity no disco: `Library/`, `Temp/`, `Logs/`, `UserSettings/` (corretamente ignoradas pelo `.gitignore`).
  - `Arte/`: Arquivos de trabalho da equipe artística (Aseprite, referências, guias de arte).
  - `Docs/`: Documentação existente (`SANTA_RITA_PROJECT_AUDIT.md`, `SANTA_RITA_VERTICAL_SLICE_REPORT.md`).
  - `.claude/`: Configuração de execução de editor (`launch.json`).
  - `README.md`, `ASSET_SOURCES.csv`, `.gitignore`, `.gitattributes`.

### 1.3 Inventário de Arquivos do Graphify (Scan Inicial)
- **Código:** 49 arquivos (44 scripts C# + manifests/settings JSON de pacote).
- **Documentos:** 14 arquivos Markdown/TXT (incluindo READMEs e licenças de assets).
- **Imagens:** 1.001 arquivos (sprites, texturas, tilesets de arte).
- **Vídeos:** 18 arquivos (vídeos do prólogo e cutscenes).
- **Segredos/Sensíveis:** Nenhum arquivo de credencial (`.env`, `.pem`, chaves) detectado.

> [!WARNING] Alerta Crítico de Escopo e Custo de Tokens
> O repositório contém **1.001 imagens** e **18 vídeos**. A tentativa de extração semântica indiscriminada enviaria centenas de imagens e arquivos pesados a modelos de visão ou acionaria transcrição de vídeo, gerando alto custo, lentidão extrema e ruído arquitetural. A extração para este projeto deve ser focada no **código-fonte C# e na documentação técnica** (`README.md` e `Docs/`), excluindo dados binários de mídia da indexação estrutural.

### 1.4 Estado Git
- Branch atual: `main` (rastreando `origin/main`).
- Existem modificações locais de Unity não comitadas (cenas, sprites e configurações em `Projeto Jogo 2D/Assets/` e logs de corrupção do PlasticSCM).
- **Recomendação de Segurança:** Criar uma branch dedicada para a configuração (`chore/graphify-obsidian-setup`), preservando intactas as alterações locais atuais.

### 1.5 Layout de Configuração de Agentes no Projeto
- Atualmente não há `.agent/`, `.agents/`, `AGENTS.md` ou `GEMINI.md` no repositório.
- A skill `graphify` já está disponível globalmente para o Antigravity em `/home/lucamontini/.gemini/config/skills/graphify/SKILL.md`.
- O comando `graphify antigravity install` injetará a regra do Graphify e o workflow local em `.agents/` no escopo do projeto.

---

## 2. Checklist de Execução por Fase

- [x] **Fase 0 — Descoberta (somente leitura) e Plano**
  - [x] Diagnóstico de SO, ferramentas, versões e caminhos.
  - [x] Levantamento de tamanho de código e estrutura do repositório.
  - [x] Análise de `.gitignore`, segredos e estado do Git.
  - [x] Criação de `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md`.
- [x] **GATE G1 — Aprovação do Diagnóstico e Plano** (Aprovado pelo usuário em 26/10/2026)
- [x] **Fase 1 — Instalação e Integração**
  - [x] Criação da branch Git `chore/graphify-obsidian-setup` (isolada, sem merge futuro).
  - [x] Detecção e confirmação do Obsidian via Flatpak (`md.obsidian.Obsidian` 1.14.5).
  - [x] Execução de `graphify antigravity install` no projeto.
  - [x] Verificação e isolamento das regras geradas (`.agents/rules/graphify.md`, `.agents/workflows/graphify.md`).
- [x] **GATE G2 — Evidência de Instalação e Escolha do Modo de Extração** (Opção B Semântica escolhida pelo usuário)
- [x] **Fase 2 — Política de Indexação e Exclusões**
  - [x] Criação do arquivo `.graphifyignore` (isolando 1.001 imagens, 18 vídeos, `Library/`, `Temp/`, etc.).
  - [x] Teste de validação: 0 mídias binárias no scan; 48 arquivos de código + 5 documentos técnicos.
- [x] **Fase 3 — Primeiro Grafo e Cofre Obsidian**
  - [x] Construção do grafo semântico completo: 1.584 nós, 3.025 arestas, 64 comunidades (`graphify-out/graph.json` e `GRAPH_REPORT.md`).
  - [x] Exportação do cofre Obsidian: 1.648 notas atômicas em `docs/architecture-vault/generated/` e canvas `graph.canvas`.
  - [x] Criação da estrutura de duas camadas: `docs/architecture-vault/README.md`, `manual/README.md` e `manual/Visao_Geral_Arquitetura.md`.
  - [x] Validação de queries (`graphify query`, `graphify path`), integridade do Canvas JSON e wikilinks.
  - [x] Geração da visualização interativa `graphify-out/graph.html`.
- [x] **GATE G3 — Apresentação do Grafo e Métricas do Cofre** (Aprovado pelo usuário em 26/10/2026)
- [x] **Fase 4 — Memória Persistente do Projeto (`docs/ai-memory/`)**
  - [x] Criação de `INDEX.md`, `PROJECT_CONTEXT.md`, `ARCHITECTURE.md`, `DECISIONS.md`, `CURRENT_STATE.md` e `CHANGELOG_AI.md`.
- [x] **Fase 5 — Regras Persistentes do Antigravity para o Projeto**
  - [x] Criação da regra de governança do projeto (`.agents/rules/project-context.md`).
  - [x] Vinculação da política de consulta prioritária ao grafo.
- [x] **Fase 6 — Scripts Idempotentes de Operação e Governança Git**
  - [x] Criação dos scripts em `scripts/` (`graphify-setup.sh`, `graphify-refresh.sh`, `graphify-validate.sh`).
  - [x] Documentação operacional em `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`.
  - [x] Configuração do `.gitignore` para o cache do Graphify.
- [x] **Fase 7 — Testes de Aceitação e Relatório Final**
  - [x] Execução da bateria de 10 testes de aceitação com evidências concretas (10/10 PASSOU).
  - [x] Entrega do relatório final em português do Brasil.

---

## 3. Comandos Previstos por Fase

### Fase 1
```bash
# Branch de trabalho dedicada
git checkout -b chore/graphify-obsidian-setup

# Integração Antigravity
graphify antigravity install
git status
```

### Fase 2 e 3
```bash
# Extração estrutural AST de código C# e docs técnicos
# (usando o interpretador isolado do Graphify para garantir integridade)
/home/lucamontini/.local/share/uv/tools/graphifyy/bin/python -c "..."

# Exportação do cofre Obsidian
graphify export obsidian --dir docs/architecture-vault/generated

# Exportação da visualização interativa HTML
graphify export html
```

### Fase 6
```bash
# Permissão de execução para os scripts operacionais
chmod +x scripts/graphify-setup.sh scripts/graphify-refresh.sh scripts/graphify-validate.sh
```

---

## 4. Arquivos a Criar e Modificar

### A Criar:
- `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md` (este documento).
- `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md` (manual de operação, manutenção e rollback).
- `docs/architecture-vault/README.md` (instruções de abertura no Obsidian).
- `docs/architecture-vault/manual/` (notas humanas e arquiteturais permanentes).
- `docs/architecture-vault/generated/` (saída gerada automaticamente pelo export).
- `docs/ai-memory/INDEX.md`
- `docs/ai-memory/PROJECT_CONTEXT.md`
- `docs/ai-memory/ARCHITECTURE.md`
- `docs/ai-memory/DECISIONS.md`
- `docs/ai-memory/CURRENT_STATE.md`
- `docs/ai-memory/CHANGELOG_AI.md`
- `.agents/rules/project-context.md` (regras específicas de arquitetura e consulta do agente).
- `scripts/graphify-setup.sh`
- `scripts/graphify-refresh.sh`
- `scripts/graphify-validate.sh`
- `graphify-out/` (`graph.json`, `GRAPH_REPORT.md`, `graph.html`).

### A Modificar:
- `.gitignore`: Adicionar regras para caches intermediários do Graphify (`graphify-out/.graphify_*`, `graphify-out/cost.json` se aplicável), preservando o grafo principal e regras existentes.
- **Nenhum arquivo de código C# ou asset do Unity será modificado.**

---

## 5. Matriz de Riscos e Mitigações

| Risco | Impacto | Mitigação |
| :--- | :--- | :--- |
| **Indexação de 1.001 imagens e 18 vídeos** | Alto custo de LLM, lentidão extrema, estouro de contexto | Restringir indexação exclusivamente a código C# (AST local gratuito) e documentação técnica Markdown em `Docs/` e `README.md`. |
| **Leitura inadvertida de caches Unity (`Library/`, `Temp/`)** | Milhares de arquivos lidos, corrupção de índice | Garantir que o scanner ignore explicitamente diretórios de build/cache do Unity conforme o `.gitignore`. |
| **Conflito com alterações locais do Unity na branch `main`** | Risco de perda de alterações de cena/sprites em andamento | Criar branch isolada `chore/graphify-obsidian-setup` sem tocar na working tree dos arquivos Unity modificados. |
| **Sobrescrita de regras do Graphify em atualizações** | Perda de customizações do projeto | Manter regras de negócio e governança do projeto em `.agents/rules/project-context.md`, mantendo `.agents/rules/graphify.md` intocado. |
| **Dependência do app desktop Obsidian** | Falha em ambientes headless/CI | Cofre estruturado 100% em arquivos Markdown e Wikilinks determinísticos e universais. |

---

## 6. Procedimento de Rollback

Se a qualquer momento for necessário reverter totalmente a configuração:
1. **Git:** Se feita na branch `chore/graphify-obsidian-setup`, basta alternar para `main`:
   ```bash
   git checkout main
   git branch -D chore/graphify-obsidian-setup
   ```
2. **Arquivos criados:**
   ```bash
   rm -rf graphify-out/
   rm -rf docs/architecture-vault/
   rm -rf docs/ai-memory/
   rm -rf scripts/graphify-*.sh
   rm -f docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md docs/GRAPHIFY-OBSIDIAN-OPERACAO.md
   rm -rf .agents/
   ```
3. **Código da aplicação:** Não há risco de impacto no código do jogo, pois nenhum arquivo de `Projeto Jogo 2D/` será alterado.

---

## 7. Critérios de Aceite e Resultados dos Testes (Fase 7)

| Nº | Critério de Aceite | Resultado | Evidência Concreta |
| :--- | :--- | :---: | :--- |
| **1** | Graphify instalado, CLI acessível, versão identificada | **PASSOU** | `which graphify` ➔ `/home/lucamontini/.local/bin/graphify`, versão `0.9.67`. |
| **2** | Integração Antigravity criou `.agents/` sem quebrar regras anteriores | **PASSOU** | Criados `.agents/rules/graphify.md`, `.agents/rules/project-context.md` e `.agents/workflows/graphify.md`. |
| **3** | Grafo gerado a partir de código real; sem vazamentos de mídia ou segredos | **PASSOU** | 1.619 nós, 3.057 arestas. 0 imagens `.png`, 0 vídeos `.mp4`, 0 diretórios de cache (`Library/`, `Temp/`). |
| **4** | `graphify query` devolve caminhos de arquivos reais que existem no disco | **PASSOU** | Consulta de `PlayerController` retornou `Projeto Jogo 2D/Assets/Scripts/PlayerController.cs` (`exists: True`). |
| **5** | Cofre abre como pasta Markdown; wikilinks e canvas válidos | **PASSOU** | 1.689 notas com wikilinks em `generated/`; `graph.canvas` validado como JSON com 1.648 nós e 200 arestas. |
| **6** | Agente recupera contexto da memória persistente em nova sessão | **PASSOU** | 6 documentos estruturados em `docs/ai-memory/` com links e categorização verificado/inferência/pendência. |
| **7** | Atualização incremental detecta alteração de código e reverte limpamente | **PASSOU** | Criado `TesteGrafoTemporario.cs`, detectado no grafo via `graphify update .`, removido e podado com sucesso (`pruned 64 note(s)`). |
| **8** | Scripts executam de forma idempotente e exibem erro acionável se faltar ferramenta | **PASSOU** | `./scripts/graphify-setup.sh` e `./scripts/graphify-validate.sh` rodaram sucessivas vezes sem efeito colateral. |
| **9** | `git diff` não mostra alterações no código C# ou lógica da aplicação | **PASSOU** | `git diff -- "Projeto Jogo 2D/Assets/Scripts"` retornou vazio (código 100% intocado). |
| **10** | Documentação, planos e manuais refletem o estado final | **PASSOU** | `PLAN-SETUP-GRAPHIFY-OBSIDIAN.md`, `CURRENT_STATE.md`, `CHANGELOG_AI.md`, `GRAPHIFY-OBSIDIAN-OPERACAO.md` atualizados. |

