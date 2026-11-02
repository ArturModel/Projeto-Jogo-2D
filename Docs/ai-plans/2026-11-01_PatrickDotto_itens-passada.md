# Inventário e passada de Marcus
Autor: @PatrickDotto / Patrick Dotto; execução Codex. Data: 01/11/2026. Ambiente: Windows, Unity 6000.6.0f1. Status: implementação e QA técnico concluídos; avaliação artística pelo autor.

Pedido: fichas somente na aba do mapa, cinco slots agrupados com tema do menu e caminhada coerente de corpo inteiro.

Contexto: INDEX/CURRENT_STATE, visão/catálogo/cenas Obsidian, query Graphify MarcusGait/SantaRitaInventory; skills pixel-art/QA e direção de produção. Código real Inventory, PlayerController, MarcusGait e scripts Aseprite. Diagnóstico: pernas procedurais desenhadas depois do recorte do torso apagavam continuidade do jeans/quadris.

- [x] Fichas deixam de ocupar os slots, sem apagar flags/saves nem aba do mapa.
- [x] Barra agrupada, moldura do mesmo asset da pausa, fundo escuro e divisórias discretas; cinco espaços, três itens atuais.
- [x] Exportar poses completas autorais, sem recorte na cintura; preservar contorno/paleta/roupa. Regenerar versão de lanterna e fontes Aseprite/GIFs.
- [x] Player QA: 141 verificações + encerramento PASS; captura da barra conferida, sequência de 16 quadros em caminhada real, contatos 0/4 e repouso sem passos extras. Builds QA/normal sem erros, hash idêntico c503b8f48616c35d82562794bf8488404b3c22f642d98c3513dac3a0ba3085c9.
- [x] Refresh/validate PASS, 112 fontes; diff --check exit 0. Seis saves intactos; RAM, posição no Cemitério e indicador de progresso não salvo restaurados.

Arquivos: SantaRitaInventory.cs, AudioC2RuntimeVerification.cs (regressão atualizada), Tools/Aseprite/marcus-walk.lua, StreamingAssets MarcusWalkFluido/MarcusLanterna e fontes Arte/Producao/Marcus. Não há alterações de geografia ou progressão.

Origem das poses: MarcusWalk.png existente do projeto. Nenhuma arte externa. Qualidade artística depende de avaliação do autor; teste automatizado não a aprova.

Evidências: Docs/Evidencias/ItensPassada/PlayerQA/runtime-qa.txt e PlayerNormal/build-windows.txt. Capturas/sequência e GIFs Aseprite ficam locais. QA usa fixtures e não comprova campanha integral nem aprovação artística. Ensaio completo no Player Development; Editor usado para compilação e restauração.
