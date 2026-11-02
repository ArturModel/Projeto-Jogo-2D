# Lanterna oculta de costas
Autor: @PatrickDotto / Patrick Dotto; execução Codex. Data: 02/11/2026. Ambiente Unity 6000.6.0f1 Windows. Status: concluído tecnicamente.

Pedido: corrigir somente a lanterna visível quando Marcus está de costas.
Contexto: INDEX/CURRENT_STATE, três notas Obsidian, Graphify PlayerController/PrepararPosesLanterna, skills arte/QA e código real.

- [x] Uma condição em PlayerController seleciona o corpo sem objeto na direção de costas. Estado da lanterna e feixe preservados.
- [x] Adequar a expectativa do QA já existente para costas equipadas; demais direções mantidas.
- [x] Player QA 33 verificações + PASS; captura de costas equipada mostra corpo sem objeto e feixe ativo. Outras direções passaram. Builds QA/normal sem erros/hash igual a9a43b7c105138318c5eae2b7d35cdb242a175b043591990b1b4d9e9da26ba3a. Seis saves intactos; sessão restaurada no encerramento.
- [x] Refresh/validate PASS (112 fontes), diff check exit 0. Relatórios em Docs/Evidencias/LanternaCostas; captura repouso-W-True local.

Arquivos de gameplay: apenas PlayerController.cs, além da expectativa em AudioC2RuntimeVerification.cs. Nenhum sprite, ciclo, velocidade, áudio, cena ou regra alterados.
