# Prólogo rural: fundo integrado, terra e áudio

Autor: @ismaelweinertb — Ismael Weinert (perfil e credencial GitHub verificados).
Data: 06/11/2026, America/Sao_Paulo. Status: implementado e verificado tecnicamente em Play; avaliação artística/audição pelo autor pendentes.
Ambiente: Windows, Unity 6000.6.0f1, branch feature/prologo-jogavel.

## Pedido e contexto

Fundo sem casas/postes presos à imagem; duas curvas suaves e entrada antecipada na terra; viagem mais curta e parada no destino. Correções adicionais do autor: separar o material do chão sob as árvores das duas pistas e criar transição entre eles; melhorar o motor e tornar perceptível o ruído de estrada de chão.

Memória INDEX/CURRENT_STATE, notas manuais de arquitetura/catálogo/fluxo, consulta Graphify PrologoJogavel/PrologoEstradaGraphic, direção de arte/narrativa/áudio e skills locais consultados. Fontes reais confirmadas nos dois scripts de prólogo, mixer e próxima cena. O pedido recente prevalece sobre os registros anteriores de recuo pavimentado. Não alterar cenas/geografia da campanha para a revisão de apresentação.

## Executado

- [x] Horizonte sem objetos próximos, atlas de árvores/casas/arbustos com projeção e posição no mundo; movimento integrado com estrada e rumo.
- [x] Duas curvas contínuas, terra em aproximadamente 12–14 s, parada aos 44 s, término aos 51 s. Recuo lateral de terra livre de arbustos/cerca.
- [x] Três materiais de solo e acostamento irregular com transparência. UVs em espaço; Repeat e mipmaps apenas nas superfícies.
- [x] Motor e rodagem de terra reais CC0, editados com fonte de autoria reproduzível; mistura por superfície/velocidade e pitch de desaceleração, pause/foco/efeitos no mixer comum.
- [x] Teste em Play via MCP com projeto confirmado; sem builds. Fixtures e limites registrados em Docs/Evidencias/PrologoRural.
- [x] Refresh e validate Graphify PASS: 116 fontes C#, hashes/caminhos/Obsidian/HTML íntegros.

## Arquivos

PrologoJogavel.cs, PrologoEstradaGraphic.cs, AudioAmbiente.cs, PrologoRuntimeVerification.cs, Editor/PrologoJogavelVerification.cs; shader AsfaltoMolhado; imagens Horizonte/CenarioRural/Terra/SoloVegetacao e metas; Resources/Audio/Prologo com dois loops e créditos; Tools/Audio/prepare-prologo-loops.py. Procedência em ASSET_SOURCES.csv e Arte/Producao/PrologoJogavel. Índices e memória atualizados. Cabine/interações e fluxo para EntradaDaCidade/saida_carro preservados.

## Validação e limites

Oito texturas presentes, shader sem erro; malhas em seis posições finitas e abaixo de 18 mil vértices, canais independentes de solo/terra; alvos de áudio em quatro fases e pausa/retomada verificados. Dois loops PCM estéreo sem clipping próprio (picos 0.62); medidas de costura documentadas. Captura de terra inspecionada. Chegada em EntradaDaCidade com PrologoConcluido em RAM observada; o primeiro observador expirou aos 30 s de uma viagem reiniciada (antes dos 51 s), depois a chegada foi conferida e registrada. Console final: zero entradas de erro. Play encerrado; cenas não salvas.

A fixture usa reflexão e inicialização explícita para inspecionar fases; não representa campanha completa. Captura preta assíncrona da parada não é prova visual. Nenhum save pessoal usado. Naturalidade/mixagem e aprovação artística dependem de avaliação do autor no Play. As melhorias desta revisão permanecem locais sem commit. A pedido do autor, o commit anterior foi regravado apenas para corrigir autoria: dc7b5841 → 451c5606, mesma árvore 76a146310649ae93ee2d86f306ae85381dcac9d0 e mensagem, push com lease explícito. Configuração Git local agora usa Ismael Weinert e noreply verificado; credencial selecionada ismaelweinertb. A identidade histórica dos documentos anteriores foi preservada como registro do fallback usado então.
