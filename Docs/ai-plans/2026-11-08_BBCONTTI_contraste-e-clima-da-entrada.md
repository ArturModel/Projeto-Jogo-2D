# Contraste e clima da Rodovia de Entrada

Registro de @BBCONTTI, em 08/11/2026, no Unity 6000.6.0f1 com URP 2D. Implementado e conferido no Play do Editor.

O pedido priorizou contraste geral, atmosfera e profundidade, com clarão ocasional e reflexos nas poças da entrada. A referência foi a captura do autor e a montagem da Rodovia já registrada no mapa do C1 e na memória do projeto.

- Checkpoint local anterior às mudanças em `68ccb0f7`. O envio aguarda confirmação após bloqueio da revisão automática.
- `SantaRitaRuaNorte.cs` configura o visual da Rodovia, alimenta os shaders com a posição dos postes e o quadro do protagonista e agenda o raio por tempo de caminhada.
- `LuzVolumetrica.shader` escurece asfalto e fundo, modula a penumbra atrás dos postes e projeta a imagem animada do protagonista na água do Tilemap.
- `Nevoeiro.shader` acrescenta faixas de névoa fria, dispersão localizada junto às lâmpadas e a silhueta do raio.
- `PlayerController.cs` aplica uma pausa de 0,22 s no movimento durante o susto.

O Editor compilou os scripts e os dois shaders passaram em `ShaderHasError`. As capturas foram feitas na câmera de jogo em Play, com o clarão forçado em fixture e a duração da reação verificada em execução. O gatilho aleatório completo com input real e o executável permanecem pendentes. As duas capturas estão em `Projeto Jogo 2D/Docs/Evidencias/EntradaDaCidade`. A captura de referência anterior foi fornecida pelo autor.

O refresh do Graphify parou porque a dependência não estava disponível e a tentativa de instalação falhou no DNS do PyPI. O validador identificou três fontes ausentes no grafo existente.
