# Integração Graphify/Obsidian ao progresso C1/C2

26/10/2026. Merge iniciado pelo autor: `main` local `86b7c02` com `origin/chore/graphify-obsidian-setup` (`6a9d7c1`). Progresso local soberano; ferramentas/documentação da branch preservadas e reconciliadas. Sem push.

## Conflitos e escopo

- `.gitignore`: união das exclusões Graphify/segredos com as regras locais de evidências finais; prints/logs intermediários permanecem locais.
- `AGENTS.md`: regras de produção preservadas integralmente e ciclo de índice acrescentado. Mapa/Bíblia, direção atual, C2 existente, Escola adiada, save manual e validação honesta prevalecem sobre levantamento antigo.
- Nenhum C#, cena, asset, pacote ou configuração Unity alterado nesta integração. A modificação anterior de `Kenney-Interface-License.txt` continua fora do stage/merge, com diff idêntico ao capturado no início.
- README, memória e notas curadas atualizados para C1/C2, Bocha, retornos, cemitério separado, lanterna F, movimento/passos, música/efeitos, ambientes, escada/recorte/bancos, slots e portas E. Sem inventar aprovação artística ou nova campanha testada.
- Inventário passa de 44 arquivos do levantamento antigo a 93 C# atuais. Relatórios históricos de QA permanecem vinculados com data e limites.

## Ferramentas e reconstrução

Graphify instalado localmente via uv com versão fixada `graphifyy==0.9.96`. Nenhum hook de agente, watcher ou serviço remoto foi instalado. O Obsidian usa a pasta do cofre; não precisa estar aberto para exportar notas/canvas.

Os wrappers PowerShell/Bash compartilham `scripts/graphify-project.py`. Refresh executa atualização AST local, exporta cofre/HTML, confirma preservação das notas curadas e gera `graphify-out/scan-state.json` com hashes C#. Validate verifica cobertura integral dos C#, caminhos portáveis existentes, hashes, canvas/HTML/relatório e quatro consultas reais. Falhas propagam para o terminal.

Primeira reconstrução revelou autoindexação: regras `docs/...` não excluíam a pasta real `Docs/...` no scan Windows, e os blocos de código das notas eram interpretados como fontes. Interrompida e arquivada; exclusões abrangem Markdown e saídas por padrão independente do prefixo. Reconstrução limpa removeu conceitos semânticos antigos que descreviam só C1. Índice recebido e primeira tentativa arquivados em backup externo, preservando `manual/` no repositório. O índice final não contém fontes Markdown, mídias, saves ou caches.

Resultado: **1.529 nós, 4.864 relações, 83 comunidades; 93/93 fontes C# cobertas**, além de ferramentas de autoria. Os 11 arquivos de Lua/PowerShell sem símbolos são limite informado pelo extrator, não falha de C#. Índice AST não prova iluminação, cenas YAML, narrativa ou qualidade visual.

## Verificação desta integração

- Refresh completo: PASS, saída 0; exportou 1.612 notas (nós + comunidades) e canvas.
- Setup e validate PowerShell: PASS, Graphify 0.9.96, consultas GameState/Capitulo2Portal/BochaSimulation/SantaRitaShallowStairs.
- Caso negativo: hash deliberadamente desatualizado no manifesto temporário foi recusado; wrapper PowerShell retornou erro. Manifesto restaurado e validação passou novamente.
- Bash do Git for Windows: sintaxe dos três wrappers e validate executado, PASS. Linux/macOS nativos não executados nesta máquina.
- Notas `manual/` preservadas byte a byte durante exportação. Links ativos, conflitos e diff final conferidos ao concluir.
- Projeto Unity comparado com `HEAD` anterior ao merge: scripts/cenas/autoria/configurações/pacotes sem diferenças. Saves e sessão do Unity não foram acessados ou modificados.
- Não houve nova build ou QA do jogo: integração restrita a documentação e ferramentas fora de Assets/. Resultados de gameplay citados pertencem às execuções anteriores, não a esta tarefa.

## Uso futuro

`./scripts/graphify-refresh.ps1` e `./scripts/graphify-validate.ps1` no Windows; equivalentes `.sh` no Bash. Regras de agentes solicitam isso após alterações estruturais. Revise separadamente as notas de texto afetadas: AST não atualiza semântica de documentação. Consulte [operação](GRAPHIFY-OBSIDIAN-OPERACAO.md).
