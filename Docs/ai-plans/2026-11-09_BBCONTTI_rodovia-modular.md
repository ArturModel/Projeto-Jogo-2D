# Rodovia de entrada modular e iluminação dos sprites

Autor do registro @BBCONTTI · 09/11/2026 · America/Sao_Paulo · Unity 6000.6.0f1 · URP 2D  
Status: implementado e verificado em Play Mode.

## Pedido e diagnóstico

Compor toda a primeira área explorável com sprites e texturas independentes, mantendo a rodovia rural e os reflexos. A segunda revisão pediu menos desgaste visível e luz direcional com sombras. O print final expôs um quadro cinza com o inventário à frente.

O fluxo C1 documentado no Obsidian monta a Rodovia dentro de EntradaDaCidade em runtime. A investigação confirmou que a inicialização genérica podia correr novamente após a montagem da Rodovia. Ela aplicava WorldLit à névoa e sobrescrevia sua ordenação, convertendo o quadrilátero transparente em um plano opaco na frente do cenário. O shader de relevo estava compilado.

## Execução

- [x] Retirar as duas faixas de arte e montar recuo, muros de arrimo, parada, orelhão, vala e acostamento com peças separadas.
- [x] Limpar a pista e o passeio. Manter folhas em quatro pontos de drenagem e terra discreta na vegetação.
- [x] Integrar mapas lineares de relevo e rugosidade para metal, pedra, madeira, folhagem e objetos da folha de entrada. Manter a leitura em Point.
- [x] Projetar sombra dos muros, suportes, defensas, telhado e mobiliário com volumes de altura. Manter o brilho da água sob os postes e o reflexo do sedã e de Marcus.
- [x] Proteger os materiais e a ordem de desenho da Rodovia na reaplicação da decoração geral. A montagem do cenário ocorre uma vez por instância.

## Arquivos

- `Assets/Scripts/SantaRitaRuaNorte.cs` e `SantaRitaRuaNorte.Chegada.cs` montam o cenário, o piso, a fiação e os volumes.
- `SantaRitaPlaceDressing.cs` e `SantaRitaWorldLighting.cs` preservam a propriedade do render da Rodovia.
- `Assets/Resources/Art/RuaNorte/LuzVolumetrica.shader`, `Entrada/Modulos/` e `Entrada/Relevos/` contêm os ajustes e a arte.
- `ASSET_SOURCES.csv` registra procedência. A montagem continua no fluxo de `EntradaDaCidade` em runtime.

## Validação

- Reprodução do quadro cinza em Play Mode, com o material incorreto identificado na névoa.
- Após a correção, a névoa usa SantaRita/RuaNorte/Nevoeiro no nível 2031. O abrigo usa LuzVolumetrica com relevo ativo e a zona tem 48 volumes incluindo Marcus. O Game View voltou a exibir cenário e HUD e o Console retornou zero erros.
- A camada de chuva estava ativa e produzia 2.056 pixels com alfa na amostra inspecionada.
- [Captura do Game View](../Evidencias/RodoviaModular/rodovia-corrigida-game.png). Validação feita no Editor.
- Graphify refresh indisponível sem o executável 0.9.96. O validador encontrou fontes ausentes no índice antigo, incluindo a nova partial da Rodovia.

## Prompts dos sprites e texturas

Geração raster pelo ImageGen integrado. As duas folhas foram recortadas em módulos individuais e amostradas na densidade do jogo. Os mapas lineares vieram dos sprites com perfis de superfície e rugosidade por material.

### Folha de objetos

Use case: stylized-concept. Game asset production sheet on a genuinely transparent background. Create exactly twelve separate, fully isolated cutout sprites laid out on an evenly spaced invisible 4-column by 3-row grid, in the exact row order below, centered in each cell, with large transparent gutters. No ground shared between sprites, no labels, text, lines or grid, no background, no scenery or buildings. These are modular objects for a 2004 southern Brazilian rainy rural roadside in the realistic pixel-art style of a serious top-down 2D adventure. Elevated orthographic three-quarter front angle with horizontal ground edges; rooted bases and contact areas are visible. They must read at 32 pixels per world unit. Sharp square pixel clusters and recognizable silhouettes, no outline cartoon, no antialias blur. Materials are aged concrete, wet iron, small clay-red hints, desaturated cool olive-green, warm charcoal, beige weathered patches. Ambient light from upper left, no painted lamp beams or drop shadows. Row 1, left to right: a low irregular dry-stacked stone retaining wall straight section with squared ends and dark cap; a separate short capped stone wall end pier with chipped plaster and exposed stones; a low wet galvanized steel roadside guardrail section with two short posts; a small rusty rectangular stormwater grate flat on the ground, straight overhead perspective. Row 2: a compact low dense wet native hedge with small dark leaves; a broader clump of fern, vines, and dark grass that grows against a wall; a small tuft of long roadside grass with bare dirt around roots kept transparent; a slim tall wood utility pole with crossbar and ceramic insulators with no long extending wires. Row 3: a plain early-2000s concrete roadside bus shelter structure with thin tiled roof and open front, no bench, see-through opening only above low back wall, no text; a lidded iron street litter bin dark grey; a pair of plain short cylindrical roadside concrete bollards with faded white tips; a cluster of wet small stones with scattered leaves, low and flat. Each item centered in its own cell and scaled to mostly occupy its cell. Keep generous transparent padding so the images can be sliced and positioned separately in Unity.

### Folha de detalhes do chão

Create a production sprite sheet of FOUR independent isolated ground decal textures for a rainy Brazilian roadside in a serious realistic pixel art 2D adventure. 2 by 2 equal square cells, each texture fully contained in its quadrant with generous transparent gutters. Truly transparent background outside the decal and through scattered gaps, no canvas color, no shadow. Perfect vertical overhead view, restrained night palette, low contrast, physically believable, crisp pixel texture, no reflections baked into the image because the engine will light these. Top left: a thin irregular charcoal asphalt repair strip, very wide horizontally, asphalt aggregate and thin dark tar seam, edges melt into fine scattered asphalt grains. Top right: a mostly horizontal patch of muddy grass with exposed damp earth, shallow pebble speckles and irregular thin rooted grass breaking up all edges, desaturated forest green with warm brown. Bottom left: sparse wet golden-brown dead leaves and tiny gravel collected in a narrow ragged horizontal gutter, largely transparent gaps between clumps. Bottom right: worn shallow concrete paver cracks and water stains shaped in an irregular flat patch, sparse moss only where cracks meet, old desaturated taupe. No text, no cast shadows, no scene, no person, no surroundings. Each cutout will be sized independently to one to three Unity world units at 32px per world unit.
