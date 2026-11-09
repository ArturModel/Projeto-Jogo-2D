# Folha de detalhes da Rodovia

@BBCONTTI · 09/11/2026 09:10 (America/Sao_Paulo) · Santa Rita, Unity 6000.6.0f1 / URP 2D  
Status — arte-fonte gerada e conferida.

## Pedido e contexto

Gerar uma única folha de pequenos sprites para compor a primeira cena. A direção de arte, o mapa do C1 no Obsidian e a composição em `SantaRitaRuaNorte.Chegada.cs` orientaram a escolha. As capturas do trecho da placa e do orelhão serviram para avaliar onde cabem os detalhes. O Graphify CLI estava indisponível.

## Execução e validação

- [x] Gerar com o ImageGen integrado uma prancha modular, com paleta discreta e sem sombras projetadas na arte.
- [x] Preservar a fonte em `Arte/Producao/EntradaDaCidade/Detalhes-Rodovia-ImageGen.png` e registrar a procedência em `ASSET_SOURCES.csv`.
- [x] Inspecionar a prancha, verificar PNG de 1254 × 1254 em RGBA e confirmar um recorte isolado por célula no alfa. A cópia no repositório tem o mesmo hash do original.
- [x] Executar refresh e validação do Graphify. Refresh indisponível sem graphifyy 0.9.96. O validador encontrou scripts preexistentes fora do índice local.

## Montagem futura

Linha superior da esquerda para a direita — cano junto ao abrigo, saída de drenagem na base do muro, limitador de roda no recuo do carro. Linha inferior — hera para a transição entre pedra e terra, papel úmido junto ao banco, caixa elétrica para fixação no abrigo. Recortar e dimensionar as peças individualmente na densidade da cena, preservando a passagem e deixando a projeção de sombras para os materiais do jogo. A folha é arte-fonte e ainda aguarda aplicação no Unity.

## Prompt usado

Use case: stylized-concept. Generate ONE production-ready 3-column by 2-row sprite sheet for the rainy Brazilian town of Santa Rita (southern Brazil, 2004). Reference: the attached Unity game screenshot in the recent conversation shows a wet gray sidewalk, a stone retaining wall with olive weeds, the green town sign, warm street lamps, parked brown car, and a bus shelter. Use that screenshot only to match the muted, material-driven realistic pixel art and elevated almost frontal orthographic view. The output consists exclusively of the following six independent small modular sprites on genuine transparent alpha. Centers in exact equal grid, one object per cell, very generous empty gutters and margins, no overlap. Row 1 left to right: (1) short gray downpipe with three dark metal clips and bent outlet, straight-on frontal wall mount, a narrow vertical silhouette; (2) short dark cast-iron wall weep-hole frame with two visible bars and a tiny dark interior, isolated cutout only, without surrounding masonry; (3) compact small chipped concrete wheel stop, low horizontal profile, believable top and front faces, muted gray with a short worn desaturated ochre stripe. Row 2 left to right: (4) a few little wet ivy runners growing horizontally with clustered muted dark olive leaves, isolated, sparse irregular silhouette for the base of a wall; (5) a small pale damp folded paper scrap with one curled corner lying flat, seen slightly from above, slate cream without printed text; (6) a discreet little wall-mounted charcoal gray sheet-metal utility junction box, soft rust at fasteners, square-ish with a short conduit stub. Consistent orthographic elevated frontal viewpoint, edges align horizontally, no isometric diamonds, minimal side faces. Hand-clustered low-res realistic 32-bit era pixel art: bold intentionally placed square pixels, integer-looking pixel edges, compact material clusters, low contrast, no fine-grain noise, no cartoony black outlines. Equal pixel density despite different object proportions. Damp stone gray, smoke-gray iron, olive, aged paper. Diffuse neutral albedo with small material highlights for readability, no strong directional lighting, no cast or contact shadows outside any cutout, no ground or scene, no wall textures behind pieces, no wet reflection, no rain overlays, no labels, no lettering, no UI, no frames, no grid lines, no border. Deliver a single square sprite atlas with true transparency including negative space between objects.


## Aplicação na Rodovia

A revisão seguinte recortou e instalou os seis tipos a 32 PPU. A placa, a parada e o orelhão continuam com a máscara direcional. Cano, drenos e caixa compartilham a sombra do suporte. Os detalhes decorativos mantêm a passagem sem colisores novos. [Captura de Play Mode](../Evidencias/RodoviaModular/rodovia-detalhes-orelhao.png).
