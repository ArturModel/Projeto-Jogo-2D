# Bloqueio da avenida e fundo do bosque

@BBCONTTI · 09/11/2026 · Unity 6000.6.0f1 / URP 2D

Os prints mostraram que a vala parecia uma faixa de água artificial e que a parte alta atrás do mercadinho revelava grandes trechos nus. A topologia do C1 preserva o acesso ao Bairro pela estrada de terra e a orientação ao Centro pelo portão da Rua Norte.

- Substituir a vala por uma árvore caída. Montar tronco, raiz exposta, ramos e pedras como peças independentes. Preservar o asfalto e sujar apenas a área em contato com a madeira. Manter o aviso que explica a interdição.
- Acompanhar o tronco e os maciços das margens com um colisor contínuo. Manter livre o caminho ao Bairro.
- Plantar copas de fundo nas três zonas. Na Encruzilhada há novos grupos atrás do mercadinho e nas duas laterais da subida, além da primeira camada visível da avenida. Um preenchimento difuso discreto dá leitura às copas distantes.
- Arquivos principais em `SantaRitaRuaNorte.Mercadinho.cs`, `SantaRitaRuaNorte.Vegetacao.cs`, `Capitulo1Director.cs` e `LuzVolumetrica.shader`. Fonte em `Arte/Producao/EntradaDaCidade/Bloqueio-Encruzilhada-ImageGen.png`. Recortes e mapas em `Assets/Resources/Art/RuaNorte/Mercadinho`.

## Prompt da folha

Production sprite atlas, two columns by two rows, four fully isolated pieces with very generous transparent gutters for a realistic 2D pixel-art game in rainy southern Brazil, 2004. No scenery, no painted ground, no drop shadows, no glow, no labels, no road cones, no orange construction colors. Slightly elevated orthographic view, read well at native 32 pixels per world unit. Low-contrast wet weathered olive-green leaves, gray/brown bark and dark earth. Main TOP LEFT object: one long thick tree trunk that has fallen, laid flat on the ground, viewed from slightly above. It runs steeply diagonally from lower-left to upper-right, about 25 degrees to the right of vertical, has a clearly visible wide broken root end at the upper right, a crooked tapering broken end at lower left, a few torn secondary branches and a few restrained clusters of wet olive leaves attached at the sides. Long narrow silhouette with diagonal directional clarity, isolated. TOP RIGHT: a short standalone uprooted soil-covered root ball with exposed roots and a little moss for placing beside the top of the long trunk, chunky realistic irregular shape. BOTTOM LEFT: a small mostly flat irregular handful of snapped wet branches, leaves and twigs, sparse contour breaking into transparency, no rectangle. BOTTOM RIGHT: a low dark rubble bank of a few irregular stones and wet soil with sparse tufts, in a horizontal cluster. All pieces fully separate, true alpha, muted natural materials with tactile clusters and credible wet bark textures, no cast shadow outside the art. The four objects will be placed individually by a game level compositor.

## Verificação

Play Mode em EntradaDaCidade. A árvore e seus recortes carregaram no acabamento normal. O limite interceptou dez raios horizontais ao longo da pista e das margens. A faixa, o entorno da mala e o acesso pela subida não encontraram colisores novos nos pontos amostrados. Foram conferidas 10 copas de fundo na Rodovia, 12 no Posto e 18 na Encruzilhada. Os materiais de 109, 114 e 120 sprites de vegetação respectivamente apresentaram perfis de relevo e compensação de albedo. Shader sem erros. As capturas de comparação usam uma câmera auxiliar com escala de jogo e não reposicionam Marcus.

- [Árvore caída e calçadas](../Evidencias/MercadinhoComposicao/mercadinho-bloqueio-tronco.png)
- [Bosque atrás do mercadinho e na subida](../Evidencias/MercadinhoComposicao/mercadinho-mata-de-fundo.png)

O Graphify não foi regenerado porque graphifyy 0.9.96 estava ausente. A validação reportou fontes ausentes no índice local.
