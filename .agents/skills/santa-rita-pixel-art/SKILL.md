---
name: santa-rita-pixel-art
description: Produzir ou revisar sprites, animações, tilesets, interiores e iluminação de Santa Rita com Marcus como referência visual.
---

Leia `Docs/Direcao_Arte_Narrativa_Audio.md`, seção Arte, e a procedência em `ASSET_SOURCES.csv`. Use Marcus em `Projeto Jogo 2D/Assets/StreamingAssets/MarcusWalk.png` como referência visual obrigatória para novos personagens; inspecione a imagem antes de produzir variantes.

Consulte `Docs/Matriz_Tecnicas_Arte.md` para distinguir técnica implementada de orientação. Para cenário/interior, escreva o uso de cada setor, mantenha circulação e escala, e confira mobiliário em câmera normal. Não aprovar sala por quantidade de props. Praça cívica, cancha e comércio devem parecer seus usos reais; não montar exposições para a chegada do protagonista. O autor autorizou liberdade de composição preservando geografia e conexões.

Escolha Aseprite para acabamento, quadros, paleta e fontes editáveis; ImageGen para criação raster com referências explícitas. Não declare sprites coerentes apenas por terem a mesma dimensão de arquivo. Compare contorno, escala da cabeça/corpo, direção da luz e tamanho de cada grupo de pixels.

Preserve GUIDs/meta ao substituir artes autorizadas. Exporte fontes e sprite sheet com tags; conferir pivôs nos pés, ausência de recortes e animação sem deslizamento. Para terrenos, use Tilemap para estrutura e camada de detalhes para desgaste. Antes de aplicar efeitos, confirme URP 2D e materiais lit. Verifique iluminação, colisão e ordenação dentro da câmera normal.

Registre evidência antes/depois e origem das imagens. Não trate um atlas bonito como animação pronta ou imagem gerada como asset CC0.
