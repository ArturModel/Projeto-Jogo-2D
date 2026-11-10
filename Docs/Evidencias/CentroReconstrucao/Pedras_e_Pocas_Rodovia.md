# Matrizes de cor, relevo e poças

As capturas do autor servem de referência anterior. Os resultados abaixo são de inspeção dos rasters e de verificação estática, sem captura posterior em Play Mode.

| Material | Fonte ImageGen | Sprite exportado |
| --- | --- | --- |
| Pedregulho | [Fonte](../../../Arte/Producao/Capitulo2/Pedregulho-Matriz-ImageGen.png) | [128×128](../../../Projeto%20Jogo%202D/Assets/Resources/Art/Cap2/Remodelagem/Matriz/Pedregulho.png) |
| Tijolinho | [Fonte](../../../Arte/Producao/Capitulo2/Tijolinho-Matriz-ImageGen.png) | [128×128](../../../Projeto%20Jogo%202D/Assets/Resources/Art/Cap2/Remodelagem/Matriz/LajeGranito.png) |

Dois pedidos independentes ao ImageGen integrado. A referência visual foi o atlas de terreno da Rua Norte. O trecho comum de ambos os prompts foi:

> Generate a single square seamless top-down pixel art game texture with FULL BLEED texture only for Santa Rita. Use attached real terrain atlas for the muted chromatic palette and weathering, but this image is an UNLIT BASE COLOR texture used under a dark nighttime renderer. Make it materially much brighter than the dark asphalt sample: average RGB approximately 118,112,99, readable warm gray midtones, highlights around 150, shadows mostly above 74. No dark vignette, no black joints, no overhead cast shadows, no lighting gradient, no objects, no grid overlaid, no text, no framing. Flat straight down orthographic, seamless periodic tile on all sides. Clear pixel clusters and features which survive reduction to 128x128.

Pedregulho

> Fine irregular crushed warm gray granite pebbles packed into compact taupe stone dust, individual stone sizes approximately 3–9 pixels in a 128 px image, crisp readable organic shapes, varied rounded stones with subtle cooler gray and olive mineral variation, small distinct rounded bevels and narrow sediment joints. No asphalt tar, no polygon mosaic, no cobbles bigger than 9 px at target scale.

Tijolinho

> Old Brazilian church forecourt paving in small rectangular stone setts, running bond offset a half brick every row. Rough worn gray-beige granite, chipped rounded corners, narrow dark gray mortar, coherent diffuse stone microtexture, pale warm taupe high points and subtle cool variations between blocks. Aim for 8 stones across and 16 horizontal rows across the full square so a block is approximately 16x8 pixels at final 128x128 size. No strong grid of black lines. Seamless row phase and mortar alignment on opposite edges.
