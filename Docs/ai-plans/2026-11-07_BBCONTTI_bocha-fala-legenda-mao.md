# Fala, legenda e mão na bocha

Autor do projeto @BBCONTTI · execução Codex

07/11/2026, 11h54 em São Paulo · Windows · Unity 6000.6.0f1

Implementado e verificado em fixture no Editor.

## Solicitação

Restabelecer o murmúrio de Tonão no padrão da sinuca, retirar os avisos de espera, baixar as legendas e espelhar horizontalmente a mão de Marcus.

## Diagnóstico e execução

- [x] Consultar a memória e o mapa de subsistemas no Obsidian, a direção do jogo, a matriz do capítulo 2 e as skills locais de áudio, narrativa, pixel art e QA. A consulta ao Graphify falhou porque o executável está ausente.
- [x] Corrigir a referência do rodapé e a busca dos painéis após a mudança para `HudBocha`. A busca antiga retornava nulo em `AtualizarTela`, que também roda ao abrir a primeira mão. Tonão passa pelo `MinigameDialogue` compartilhado com a sinuca e pelo ganho de fala já configurado.
- [x] Remover as instruções para observar a jogada ou caminhar enquanto a vez está bloqueada. Baixar o centro das duas legendas de 118 para 94 unidades da borda inferior, com a base alinhada ao topo da faixa de comandos. Os textos de câmera e percepção cedem espaço enquanto a legenda está ativa.
- [x] Aplicar escala horizontal de −1 ao desenho da mão no Canvas. A inversão é feita após o recorte do quadro e mantém o pivô, o tamanho e a posição usados pelo gesto.

## Arquivos

`BochaMinigameUI.cs`, `BochaMinigameUI.Tonao.cs` e registros de execução e memória.

## Verificação

- Fixture no Play Mode em sessão de teste. Abriu o duelo pelos botões e avançou a simulação a partir de um lançamento controlado de Marcus. `Update` iniciou o turno de Tonão e tocou `SR_Minigame_Murmurio_0` com a fonte em reprodução e ganho 0,20. O clipe apresentou pico de amostra de 0,412. A legenda revelou caracteres e a altura das duas caixas foi confirmada em 94. O aviso no rodapé ficou vazio durante a rolagem.
- Nenhum erro no Console após o teste de fala. `git diff --check` aplicado aos arquivos da correção.
- Compilação isolada com as referências do Unity concluída com zero erros e três avisos de referência ou campo preexistente.
- Refresh e validate do Graphify executados. O executável está ausente e o índice disponível aponta fontes da bocha e da sinuca ainda fora do grafo.
- Audição em partida completa e verificação no Player pendentes.
