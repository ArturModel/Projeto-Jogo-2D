# Proporções de Marcus e acabamento do inventário
Autor: @PatrickDotto / Patrick Dotto; execução Codex. Data: 01/11/2026. Ambiente: Unity 6000.6.0f1 Windows. Status: implementação e Player QA validados; sessão restaurada.

Pedido: pernas mais encorpadas e barra mais bonita/coerente. Referência: screenshot do autor e menu de pausa.

Contexto consultado: INDEX/CURRENT_STATE e três notas manuais Obsidian, Graphify SantaRitaInventory/MarcusGait, skills pixel-art e QA, direção/produção e fontes reais. O ciclo completo anterior resolveu o corte mas manteve pernas finas; o painel da pausa fatiado não funcionou na altura da barra.

- [x] Alargar segmentos do jeans em dois pixels mantendo pose, cintura, altura e cores autorais; aplicar também ao repouso e regenerar lanterna/fontes Aseprite.
- [x] Substituir moldura fatiada por construção nativa de UI: contorno fino de latão, relevo superior e fundo escuro. Cinco slots sem fichas.
- [x] Apresentação conferida no Player, 141 verificações + encerramento PASS na repetição sem build paralela; movimento/contatos/repouso. Builds sem erros e hash idêntico 1816e1777ab30a16df4b1f879672378cb04c1175c2842a7ad72c1400993a6e7e.
- [x] Refresh/validate PASS, 112 fontes C#; diff check exit 0.

Arquivos: Tools/Aseprite/marcus-walk.lua; fontes Arte/Producao/Marcus e StreamingAssets MarcusWalkFluido/MarcusLanterna; SantaRitaInventory.cs. Sem novas classes, cenas ou regras de progressão. Assets derivados do Marcus existente; sem download externo. Aprovação artística pertence ao autor.

QA inicial: builds QA/normal sem erros; captura do inventário conferida. Movimento e contatos passaram, mas repouso falhou durante build paralela (janela de desaceleração em tempo real). Repetição sem build em andamento; causa de carga é hipótese, não conclusão. Registro intermediário local runtime-primeira-rodada.txt preservado.

Repetição: repouso PASS sem mudar o código. Primeira falha registrada, sem assumir causa confirmada. Capturas de inventário e sequência de caminhada ficam locais, relatórios finais em Docs/Evidencias/Proporcoes.

Encerramento: seis saves intactos, RAM/posição/indicador de progresso restaurados no Centro. Nenhum commit/push. QA técnico não equivale à aprovação artística.
