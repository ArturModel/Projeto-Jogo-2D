# Caderno de Marcus e acabamento do mapa

Autor: Patrick Dotto (identificação Git local; gh fora do PATH). Data: 03/11/2026. Ambiente: Unity 6000.6.0f1 / Windows. Status: concluído tecnicamente; avaliação artística pelo autor permanece aberta.

## Pedido
Isolar scroll dos painéis, nome Mapa da Cidade, entorno com campos/morros/rio no estilo do jogo, fichas só adquiridas e em ordem sem cupom; substituir diário técnico por caderno manuscrito, paginado, com voz de Marcus, crescimento narrativo e notificação de atualização. Rabiscos coerentes com descobertas.

## Contexto
INDEX/CURRENT_STATE, notas manuais Obsidian, consulta Graphify de MapaInterativoUI/SantaRitaAtlasView/GameState, fontes reais, Bíblia/direção de produção e skills narrativa/pixel-art/QA consultados. Sessão do autor encontrada em Play EntradaDaCidade; snapshot RAM, posição, zona e indicador de progresso exportados fora do repositório antes de parar. Seis arquivos de save têm hashes de referência externos. Alterações anteriores do atlas preservadas.

## Plano
- [x] Separar entrada de mapa/painéis/caderno.
- [x] Filtrar fichas por posse, preservar flags e retirar cupom.
- [x] Caderno com paginação e textos autorais em primeira pessoa, fonte manuscrita e papel Aseprite editável.
- [x] Eventos de diário para aviso, sem notificar carga de snapshot ou repetição idêntica.
- [x] Arte de entorno contextual com materiais/árvores do jogo, sem alterar cartas ou cenas.
- [x] Gerar/importar fonte e arte, revisar visualmente.
- [x] Regressão Unity/Player, builds e restauração da sessão.
- [x] Índices Graphify/Obsidian, memória e relatório final.

## Dependência compartilhada
GameState recebe eventos de atualização material do diário e restauração do snapshot. Não muda formato v2 ou regras de save manual; nenhum autosave. A projeção editorial lê registros existentes e não reescreve fontes/diálogos/objetivos. Fonte Kalam usa OFL 1.1, licença distribuída em StreamingAssets/Licenses; imagens do caderno e rabiscos são originais Aseprite, paisagem reutiliza arte do próprio jogo.

## Evidências e limites
Docs/Evidencias/Caderno. Avaliação artística/leitura subjetiva pelo autor permanece distinta do QA técnico. Paisagem nova representa apenas o entorno ilustrativo, sem prometer áreas jogáveis, acessos ou detalhes de capítulos futuros.


## Arquivos e verificações em curso

Novos: MarcusNotebookContent.cs, MarcusNotebookUI.cs, NotebookRuntimeVerification.cs, SantaRitaNotebookAuthoring.cs, Resources/Notebook (64 textos/variantes; Book/Leaf/Sketch), fonte Kalam e TMP estático/licença, Tools/Aseprite/caderno.lua e fontes Arte/Producao/Caderno. Modificados: MapaInterativoUI, GameState, SantaRitaAtlasView, dispatcher Chapter2RuntimeVerification, ASSET_SOURCES.csv e documentação pertinente. Nenhuma cena reautora.

Editor final: 30 verificações específicas + PASS e 237 verificações Atlas + PASS. Player específico: 30 verificações + PASS. Fonte e papel conferidos em capturas 1280×720. Duplicata editorial da anotação da rodoviária removida antes das builds finais (64 textos/variantes). Primeiro QA do Editor sofreu interferência de dispositivos sintéticos remanescentes; fixture passou a possuir/limpar os dispositivos desde o início. Game View estreito não serviu como evidência final de layout.

Refresh e validate Graphify concluídos: PASS, 120 C#. Seis arquivos pessoais de save conferidos sem mudança por SHA256. Compilação/build final, regressão Player e restauração da sessão em consolidação. Relatório com evidências e limites em Docs/Evidencias/Caderno/Relatorio.md.


Conferência manual no normal revelou nota legada do prólogo ainda genérica; adicionada versão pessoal e conferidos todos os IDs literais dos registros ativos (nenhum sem projeção). Catálogo final: 64 textos/variantes sem condição duplicada. Teste de mouse: carregar slot explícito, item mapa, abrir caderno e virar página. Helper não confirmou teclas rápidas de Windows; não considerar teclado físico homologado por esse smoke. Configuração runInBackground alterada pelo ensaio do Editor é restaurada para o valor de produção antes das builds finais.


## Fechamento

Editor: 30 verificações de caderno + 237 de Atlas e encerramentos PASS. Player: 30 de caderno final + 237 de Atlas e encerramentos PASS; última adição editorial do prólogo não altera mecânica e está na build final específica. Builds normal/QA Succeeded, zero erros, mesmo SHA256 `59052d449a474eb18124d41d4808935fb762d903b66ace9fd3f0f34e99896d6b`. Assembly normal contém o caderno e exclui runner/ferramentas de autoria.

Sessão restaurada em Play/EntradaDaCidade/zona original, posição original, mapa aberto e progresso não salvo preservado. JSON da RAM restaurada comparado estruturalmente: idêntico ao backup. Seis arquivos pessoais continuam idênticos por SHA256. Cenas e ProjectSettings sem diff. Configuração de execução em background restaurada. `git diff --check`: PASS; nada staged, commit/push não realizados. Índices refresh/validate PASS (120 C#). Evidências locais e relatório discriminam fixtures, leitura artística e limites de teclado físico.

Próximos passos: revisão subjetiva pelo autor do caderno e da paisagem. Sem dependência técnica pendente deste pedido. Ferramentas e corpus disponíveis para continuidade pelos demais desenvolvedores.


Observação de integridade após restauração: TMP escreveu cache de glifos em VT323-Regular Bitmap.asset, único asset posterior às builds. Hash completo atual difere pelo cache; builds QA/normal têm o mesmo hash entre si. Código/cenas/corpus/arte não foram alterados depois das builds. Preservado cache e trabalho anterior, sem reset para igualar artificialmente. Limite explicitado no relatório.
