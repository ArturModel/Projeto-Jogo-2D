# Graphify + Obsidian — operação atual

Atualizado em 26/10/2026 após integração ao C2. [Estado atual](ai-memory/CURRENT_STATE.md) e [regras de produção](../AGENTS.md) prevalecem sobre o levantamento inicial da branch.

## Instalar e abrir

Dependências: Git, Python 3.10+ ou uv. Versão fixada e exercitada nesta integração: **graphifyy 0.9.96** (executável `graphify`). Origem: [Graphify-Labs](https://github.com/Graphify-Labs/graphify), [PyPI](https://pypi.org/project/graphifyy/0.9.96/).

```powershell
uv tool install graphifyy==0.9.96
uv tool update-shell
.\scripts\graphify-setup.ps1
```

Feche/reabra o terminal após mudar PATH. Os scripts também procuram o executável em `~/.local/bin`; não dependem da pasta pessoal do autor da branch. Não instalam hooks nem sobrescrevem regras de agentes. Se houver outra versão do Graphify, revise a atualização antes de alterar a versão fixada.

Instale o [Obsidian](https://obsidian.md/download) e escolha **Abrir pasta como cofre** → `docs/architecture-vault`. `manual/` contém notas curadas de C1/C2; `generated/` contém símbolos e `graph.canvas`. Ctrl+G mostra conexões. O app não precisa estar aberto para o cofre ser atualizado.

## Atualizar após mudanças estruturais de código

Windows, na raiz:

```powershell
.\scripts\graphify-refresh.ps1
.\scripts\graphify-validate.ps1
```

Bash (Linux/macOS/WSL):

```bash
bash scripts/graphify-setup.sh
bash scripts/graphify-refresh.sh
bash scripts/graphify-validate.sh
```

Os wrappers usam `scripts/graphify-project.py`: atualização AST local, exportação Obsidian/HTML, preservação byte a byte das notas manuais durante exportação, manifesto de hashes dos C# e validação de cobertura/caminhos. Uma falha retorna erro; não há mensagem de sucesso sem execução válida.

A rotina de agentes em AGENTS.md solicita refresh após alterações estruturais. Não há watcher ou serviço em segundo plano instalado. Atualização só de Markdown exige revisão das notas curadas; **AST não extrai semântica de documentos**. A reconstrução desta integração removeu do índice as notas semânticas antigas e seus fatos desatualizados. Backup externo da versão recebida foi conservado.

## Consultar

```bash
graphify query "Capitulo2Portal" --budget 500
graphify query "BochaSimulation" --budget 500
graphify explain "SantaRitaShallowStairs"
graphify path "BochaMinigameUI" "GameState" --undirected
```

Se um nome for ambíguo, use o ID/caminho apresentado. Confirme a informação no código/cena/dado atual. O cofre não determina geografia nem aprova arte, áudio ou campanha.

## Validação e preservação

- Todos os C# de Assets/Scripts e Assets/Editor devem aparecer como fonte no grafo; não apenas os 44 arquivos do levantamento antigo.
- Caminhos de fontes são relativos, existentes e internos ao repositório. Hashes devem refletir o código atual.
- Canvas JSON, HTML, relatório e consultas de GameState/portas/Bocha/escada devem funcionar.
- `.graphifyignore` exclui arte, mídias, cenas YAML, caches, builds, saves, credenciais e saídas do próprio índice. Referências a uma imagem dentro de código não significam indexação do binário; validação examina fonte dos nós.
- `manual/` e `docs/ai-memory` são mantidos separadamente, com fontes e limites. Não apagar notas humanas para regenerar símbolos.
- Ferramentas e documentação permanecem fora de Assets/ e da build. Esta validação não recompila nem testa o jogo.

## Recuperação

Se refresh falhar, mantenha os fontes e notas curadas, corrija o erro exibido e reexecute. Antes de substituir um índice incompatível, faça backup externo de `graphify-out` e regenere somente os derivados. Não use comandos de exclusão de branch ou de cenas para recuperar documentação. Nunca use `--force` para esconder uma perda de cobertura; investigue primeiro.
