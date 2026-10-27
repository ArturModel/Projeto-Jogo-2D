# Plano Executado — Verificação de Branch e Pré-Requisitos para Abertura no Unity

- **Autor (GitHub):** @lucamontini (Luca Dalla Lana)
- **Data e Hora:** 2026-10-27 14:20 (BRT)
- **Status:** Concluído com Sucesso
- **Ambiente/Plataforma:** Linux Mint 22.3 (Zena / Ubuntu Noble), Unity 6000.6.0f1 target, Git, Graphify 0.9.96, Python 3.12

---

## 1. Solicitação do Usuário

> "vou abrir o jogo na unity, como posso saber se estou na branch main? Eu preciso rodar algum comando antes de abrir o projeto na unity?"

---

## 2. Diagnóstico e Contexto Consultado

1. **Ciclo de Entrada Obrigatório:**
   - Memória persistente: [`docs/ai-memory/INDEX.md`](../ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](../ai-memory/CURRENT_STATE.md).
   - Cofre Obsidian: [`docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`](../architecture-vault/manual/Visao_Geral_Arquitetura.md) e [`Catalogo_Completo_Scripts.md`](../architecture-vault/manual/Catalogo_Completo_Scripts.md).
   - Graphify CLI: `graphify query "configuracao unity git" --budget 100` executado com sucesso.
2. **Inspeção de Git e Ambiente:**
   - Comando `git branch --show-current`: confirmou que o repositório já está na branch `main`.
   - Comando `git status -s`: confirmou repositório limpo (apenas 2 arquivos de licença com diferença de quebras CRLF/LF).
   - Versão do Unity conferida em `Projeto Jogo 2D/ProjectSettings/ProjectVersion.txt`: `6000.6.0f1`.
3. **Detecção Crítica de Git LFS:**
   - Inspeção de `.gitattributes`: extensões binárias (`.png`, `.ogg`, `.wav`, etc.) estão sob filtro LFS.
   - Verificação no sistema operacional: `git-lfs` não estava instalado no Linux Mint (`which git-lfs` retornou exit code 1).
   - Verificação nos arquivos do projeto: arquivos como `bocha.ogg` e sprites `.png` em `Projeto Jogo 2D/Assets/` estavam como ponteiros de texto de ~130 bytes.
   - **Risco identificado:** A abertura direta do projeto na Unity sem puxar os binários LFS importaria os arquivos de áudio e texturas corrompidos/ausentes.

---

## 3. Ações Realizadas

1. **Mapeamento das Instruções de Abertura:**
   - Comandos para checagem de branch (`git branch`, `git status`).
   - Instruções de instalação do `git-lfs` via `apt` no Linux Mint.
   - Comandos `git lfs install` e `git lfs pull` para obter os binários reais antes do Unity importar a pasta `Library`.
   - Instruções sobre a versão exata do Unity Editor (`6000.6.0f1`) e pasta correta (`Projeto Jogo 2D/`).
   - Recomendação de configuração do `UnityYAMLMerge` para merge de cenas.
2. **Ciclo de Saída de Governança:**
   - Execução de `./scripts/graphify-refresh.sh`.
   - Execução de `./scripts/graphify-validate.sh` (93 scripts C# validados).
   - Atualização do [`docs/ai-memory/CHANGELOG_AI.md`](../ai-memory/CHANGELOG_AI.md).

---

## 4. Arquivos Modificados / Criados

- `docs/ai-plans/2026-10-27_lucamontini_verificacao-branch-e-pre-requisitos-unity.md` (este documento).
- `docs/ai-memory/CHANGELOG_AI.md` (registro da intervenção e referência a este plano).

---

## 5. Validação e Testes

- `./scripts/graphify-refresh.sh`: PASS (AST e cofre gerado atualizados).
- `./scripts/graphify-validate.sh`: PASS (93 scripts C# cobertos, caminhos portáveis, integridade de hashes).

---

## 6. Próximos Passos Recomendados para o Usuário

1. Rodar no terminal:
   ```bash
   sudo apt update && sudo apt install git-lfs
   git lfs install
   git lfs pull
   ```
2. No Unity Hub, adicionar o projeto selecionando a pasta `Projeto Jogo 2D/` e garantindo o Editor na versão `6000.6.0f1`.
