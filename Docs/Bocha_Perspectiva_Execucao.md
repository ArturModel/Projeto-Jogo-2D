# Bocha — perspectiva, controle e física

23/10/2026. Pedido do autor: aproximar a experiência de https://texasboccecourts.com/pages/play-bocce-ball-online, preservando a identidade da Sociedade Beira-Rio. Retirar expressamente a mecânica de água/atrito por faixa. Não transportar o cenário comercial nem seus assets/código.

## Base e preservação

Branch `capitulo_2`, alterações anteriores presentes. Patch e cópias dos arquivos afetados em `%USERPROFILE%/.santarita/backups/20261023_Bocha`. Hashes pessoais preservados no mesmo backup. Autor autorizou descartar a partida de teste que estava aberta no Editor. Não alterar política de saves, recompensa, ligações entre regiões ou a regra autoral de quatro bochas/duelo até seis.

## Investigação e marcos

- [x] Referência aberta e jogada pelo navegador: perspectiva desde a cabeceira, arraste para direção/força, Ponto/Rafa/Volo, rolagem com rastro e mapa superior expansível. Observar os três modos e a consulta antes de fechar a pesquisa.
- [x] Falha física reproduzida no kernel real, sem tocar a partida: bocha em `(-0,05;10)` a 14 m/s passa raspando por outra em `(0;10,3595)` com raios 0,18; zero impactos/transferência no passo de 1/120. Teste de contato varrido deve detectar esse caso.
- [x] Física: contatos contínuos, impulso/massa, atrito e rotação, lançamento alto/queda, rebotes e repouso; mesmo kernel para jogo e IA. Medir, não apenas ajustar aparência.
- [x] Apresentação: geometria em perspectiva, madeira/saibro/luzes do salão, sombras, giro e rastros de contato; vistas de lançamento, alvo e superior, zoom e minimapa sempre legível.
- [x] Controle (revisão de 23/10): Ponto/Rafa/Volo; cursor define direção/comprimento da mira; segurar clique ou Espaço carrega força triangular de 4–100%, subindo em 1,5 s e descendo em 1,5 s; soltar lança. Sem slider/botão LANÇAR. Botão direito/C, foco, vista, tipo e release fora da área cancelam.
- [x] Integração: eliminar faixa de água da física/IA/interface e das falas instrutivas; preservar fatos históricos ligados à inundação. Recompensa única, prática sem progresso, escolha explícita de save após derrota.
- [x] Validar física, input, vistas, UI, resolução, IA, derrota/vitória e build final com saves isolados. Índice de evidências e limitações no fechamento.

## Decisões de apresentação

A cancha continua sendo a da Sociedade: saibro contido por madeira, circulação lateral, bancos, suporte e lâmpadas quentes. A mudança de eixo na câmera é somente uma vista do minigame, sem mover o edifício ou a cancha do mapa. Projeção em perspectiva de geometria e bolas com altura real no kernel; consulta superior usa as mesmas coordenadas da partida. A apresentação não exige trocar o renderer do mundo 2D nem a engine.

## Evidências

Entrega técnica validada; QA/PO e limites em [Relatorio](Evidencias/BochaPerspectiva/Relatorio.md). Capturas/filmagens locais em `Docs/Evidencias/BochaPerspectiva/`, ignoradas pelo Git. Conservar no repositório fontes editáveis, resultados finais selecionados e este índice. Qualidade artística, diversão e audição não são inferidas da contagem de testes.


## Revisão de clique — 23/10/2026

Reprodução: Project(-1,0,8).x = +23,99 e Project(+1,0,8).x = -23,99: câmera espelhada, enquanto o minimapa usava x crescente. Corrigida a base da câmera; A/D já alteravam x corretamente. Carga antiga dependia de OnDrag/distância do gesto. Nova carga usa tempo sem depender do deslocamento e repete enquanto pressionada. Mira e carga têm informação separada: linha indica ponto visado, força define alcance. Consulta superior é uma câmera de inspeção; minimapa mantém a orientação da cabeceira com base embaixo.

Melhorias de leitura: bocha de saída visível inteira, cabeceira próxima baixa; luz quente localizada por vértice e escurecimento junto às guias, atlas Aseprite refinado e madeira com filetes. Trata-se de iluminação da projeção UI, não novas Light2D/SSAO do mundo.

Validação final: 28 regressões físicas/carga, 117 verificações gerais e 70 verificações no Player QA passaram. Builds QA/comum com zero erros e mesmo hash de conteúdo. Saves pessoais preservados. As primeiras rodadas revelaram premissas ausentes da fixture (entrada C2/F ligada), ajustadas no harness. Limites e inspeção da build comum estão no relatório.


## Sala fechada e orientação final

Pedido adicional do autor aplicado: vista geral lança da esquerda para a direita. A Sociedade mantém sua geografia; uma divisória separa salão/copa da sala da cancha, com passagem física, circulação e bancos internos. Autoria dirigida preserva os objetos/progresso existentes. Bancada estreita e parede cobrindo Marcus foram detectadas e corrigidas na validação. Fonte Aseprite exporta o mesmo saibro/madeira para a projeção e o ambiente. Nenhum capítulo novo ou C1 reaberto.
