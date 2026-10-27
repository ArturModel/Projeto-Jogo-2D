# Planos de Execução de IA — Histórico por Autor e Data

Este diretório (`docs/ai-plans/`) registra o histórico rastreável de todas as execuções e tarefas realizadas por agentes de IA e desenvolvedores no projeto **Santa Rita — A Noite das 03:17**.

---

## 1. Regra Obrigatória de Governança

> **OBRIGATÓRIO:** Ao final de **TODO NOVO PROMPT**, o agente de IA **DEVE** criar um arquivo neste diretório documentando o plano executado, conforme estipulado em [`AGENTS.md`](../../AGENTS.md).

### Padrão de Nomenclatura do Arquivo
```
docs/ai-plans/YYYY-MM-DD_<usuario-github>_<slug-da-tarefa>.md
```
*Exemplo:* `2026-10-27_lucamontini_merge-main-auditoria-governanca.md`

Para obter o usuário do GitHub:
- No Linux/macOS: `gh api user -q .login 2>/dev/null || git config user.name`
- No Windows PowerShell: `gh api user -q .login; if (!$?) { git config user.name }`

---

## 2. Template Obrigatório de Documento de Plano

Todo documento de plano gerado deve seguir a estrutura abaixo:

```markdown
# Plano Executado — <Título Conciso da Tarefa>

- **Autor (GitHub):** @<usuario-github> (<Nome Real se disponível>)
- **Data e Hora:** AAAA-MM-DD HH:MM (Fuso local)
- **Status:** Concluído / Em Andamento / Bloqueado
- **Ambiente/Plataforma:** Linux / Windows / macOS (versão do Graphify: 0.9.96)

---

## 1. Solicitação do Usuário
<Texto integral ou transcrição do prompt do usuário>

---

## 2. Diagnóstico e Contexto Consultado
- **Memória e Obsidian:** Quais notas de `docs/ai-memory/` e `docs/architecture-vault/manual/` foram lidas.
- **Graphify CLI:** Comandos executados (ex.: `graphify query "<termo>"`, `graphify path`, `graphify explain`) e nós relevantes identificados.
- **Fontes C# reais:** Arquivos inspecionados em `Projeto Jogo 2D/Assets/Scripts/`.

---

## 3. Plano de Ação Executado
1. [x] Passo 1 executado...
2. [x] Passo 2 executado...
3. [x] Passo 3 executado...

---

## 4. Arquivos Modificados / Criados
- `arquivo1.cs` — Descrição da alteração
- `arquivo2.md` — Descrição da documentação adicionada

---

## 5. Validação e Testes
- **Refresh Graphify + Obsidian:** Status do `./scripts/graphify-refresh.sh` ou `.\scripts\graphify-refresh.ps1`.
- **Validador de Integridade:** Status do `./scripts/graphify-validate.sh` ou `.\scripts\graphify-validate.ps1` (cobertura integral dos 93 C#, 0 erros).
- **Testes Manuais / Unity (se aplicável):** Comprovações no Editor ou logs de execução.

---

## 6. Próximos Passos e Recomendações
- Próximas tarefas recomendadas para os colegas de equipe.
```
