# Mercadinho, subida de terra e vegetação das três cenas

@BBCONTTI · 09/11/2026 · Unity 6000.6.0f1 / URP 2D  
Escopo — Zona3_Encruzilhada e vegetação da Rodovia, Posto e Encruzilhada.

## Pedido e contexto

Compor o Mercadinho e a subida com módulos próprios, corrigir a travessia, aumentar a casa da esquina, retirar a cerca e substituir a barricada de cones. A revisão ampliou o escopo para as árvores e a submata das três zonas. O mapa do C1 e a construção em `Capitulo1Director.cs` confirmam o acesso ao Bairro pela subida e a interdição da avenida do Centro.

## Execução

- [x] Gerar duas folhas de seis sprites com o ImageGen integrado. As peças de loja e de subida foram recortadas individualmente a 32 PPU e recebem luz de cena. Cascalho, sulcos e água são decalques planos. Bueiro, capim e balizas ficam nas margens.
- [x] Alinhar a faixa à porta, prolongar a pintura pela parte asfaltada até os passeios e indicar os dois rebaixos no material. Integrar a boca da subida ao asfalto com transição de terra e preservar o gatilho do Bairro.
- [x] Remover a cerca e os cones e ampliar a casa em 30%. A vala de obra recorta o pavimento e as calçadas, tem margem física contínua e preserva a leitura do aviso sobre o acesso pela Rua Norte.
- [x] Gerar quatro espécies com ImageGen. Recortar e importar pitangueira, bracatinga, araucária e camboatá com pivôs na base dos troncos. Distribuir grupos e submata nas três zonas, com clareiras e estrada abertas.
- [x] Calcular perfis diferentes para copa e casca. Cada árvore usa inclinação local, rugosidade e transmissão de luz limitada às folhas. Reutilizar a máscara de sombras da cena e a oscilação das copas.

## Arquivos

- Montagem em `SantaRitaRuaNorte.Mercadinho.cs` e `SantaRitaRuaNorte.Vegetacao.cs`, com ajustes no diretor, no acabamento comum e em `LuzVolumetrica.shader`.
- Fontes `Mercadinho-Detalhes-ImageGen.png`, `Subida-Detalhes-ImageGen.png` e `Arvores-Rodovia-ImageGen.png` em `Arte/Producao/EntradaDaCidade`.
- Recortes, metadados e mapas lineares em `Assets/Resources/Art/RuaNorte/Mercadinho` e `Vegetacao`. Procedência em `ASSET_SOURCES.csv`.

## Prompts de geração

### Loja

Create one transparent square 3×2 production sprite atlas for Santa Rita, a rainy 2004 small town in southern Brazil, realistic low-resolution pixel art with elevated front-on orthographic view and controlled gray/olive/brown palette. Use the user’s mercadinho Game View reference only to match camera/material/pixel style. Six isolated modular sprites, generously separated in equal grid cells, true alpha behind each, no ground shadow or baked light. Row 1 left to right: a low old metal and timber produce rack with only a few desaturated oranges in shallow trays; a shuttered galvanized utility meter box with short conduit for a wall; a small low street litter bin with dark olive hinged lid. Row 2 left to right: a narrow storefront folding sandwich board in charcoal wood with a few illegible muted marks, no words; a flat narrow dark cast iron trench grate (front-on elevated view, very horizontal); a wide low worn concrete planter with a sparse broad-leaf plant. Render with natural material clusters, restrained contrast, crisp readable square pixels, neutral diffuse albedo, no noise, no rim light, no shadows outside the art, no buildings or surroundings, no labels or grid lines. These are small supplementary props for composing an actual game level.

### Subida

Create one genuine transparent square 3×2 game sprite atlas. Six individual modular accents for the muddy sloped unpaved side road in a rainy southern Brazilian town in 2004, Santa Rita. Match the realistic orthographic low-res pixel art of the attached Mercadinho game reference, in restrained dark earth, slate gray, olive, and pale gray wet stone. Even invisible grid, one completely separated asset in each cell with generous transparent gutters, no captions, no ground plane. Top row: (1) small flattened loose cluster of wet angular gray gravel, irregular thin near-top-down decal, its edges break into a few individual stones separated by alpha; (2) one dark broken-width thin muddy shallow rut running VERTICALLY up the sprite, slight irregular offset, only the inner rut, very low contrast, tapered and transparent edges; (3) small flat irregular blue-brown muddy water patch with soft irregular edge stepping in pixel clusters, faint sky-gray streaks only with no bright reflection. Bottom row: (4) low sparse tussock of native grass rooted around 3 little stones for the road shoulder, no foundation rectangle; (5) short weathered wooden culvert headwall and a dark small round metal pipe opening inset between a few stones, viewed from slightly above and straight on, no building; (6) short old timber roadside delineator stake, pale faded top, worn brown bottom, thin and vertical. Pixel art with tactile clustered materials, square crisply placed pixels, restrained grain, neutral albedo and no painted directional shadows, no outlines, no rain graphics, no scenery. The asset sheet is only the isolated pieces, for individual slicing and composition in a game.

### Árvores

Use case stylized concept, production game sprite atlas. Generate a single 2×2 grid of four entirely separate whole native tree sprites, for a rainy small town roadside in southern Brazil in 2004. True transparent alpha between and around all cutouts, generous gutters. No floor, ground blob, glow, shadows, rain graphics, border or lettering. Elevated orthographic nearly straight-on front view, at the angle of a realistic 2D pixel-art game, with only a little of the upper leaf surfaces visible. Tactile deliberately arranged square pixel clusters, readable at 32 pixels per world unit. Neutral diffuse material albedo, no fixed direction to the light. Separate limbs and foliage silhouettes, gaps revealing branches and true transparency within each crown. Top left: pitangueira with a rounded irregular densely layered medium-sized crown, small dark olive leaves and two stems, no conspicuous fruit. Top right: bracatinga with an open asymmetrical fine-leaved canopy and a narrow forked gray trunk, enough negative space between groups to cast believable shadows. Bottom left: native Brazilian araucaria with a gray straight tall trunk and tiered umbrella arms with dense dark conifer foliage, strongly recognizable silhouette, no snow. Bottom right: mature camboatá with wide slightly spreading uneven heavy crown, dark olive and sage leaf groups and a twisting split mottled bark trunk. Muted rainy palette, leaf groups visibly distinct from wood, bark rough rather than black, no sharp white rim or high-contrast grain. Use true alpha, avoid colored haze or baked shadow. Realistic human-relative proportions for a roadside scene; game depth is added by the engine.


## Verificação e revisão do limite

Em Play Mode, os doze tipos novos da loja e da subida carregaram a 32 PPU com material local. A travessia manteve o eixo da porta e a subida manteve o gatilho do Bairro. O Graphify estava indisponível e a validação detectou fontes ausentes no índice. A vala deste primeiro acabamento foi substituída em uma revisão por uma árvore caída. [Travessia atual](../Evidencias/MercadinhoComposicao/mercadinho-travessia-final.png), [revisão do bloqueio](2026-11-09_BBCONTTI_bloqueio-fundo-mata.md).
