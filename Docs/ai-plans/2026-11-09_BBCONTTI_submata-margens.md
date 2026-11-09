# Submata e margem sul nas três cenas asfaltadas

@BBCONTTI · 09/11/2026 · Unity 6000.6.0f1 / URP 2D

Os prints de inspeção indicavam copas isoladas, uma margem sul vazia e diferenças de luminosidade no próprio albedo. A revisão usa a topologia do C1 registrada no Obsidian e as coordenadas das calçadas, da travessia e da mala no diretor. Não há mudança nos gatilhos.

- Gerar uma folha modular com duas árvores jovens, moita, samambaia, folhiço e galho caído. Recortar a 32 PPU, remover a franja de alfa e guardar a folha-fonte.
- Distribuir núcleos com copas sobrepostas, arbustos baixos e chão de floresta. A composição sul usa bolsões específicos para cada zona e mantém claros de acesso na travessia e junto à mala.
- Harmonizar a cor-base em luz linear com base nas medianas medidas nas regiões de folhas. A mesma correção distingue a casca e limita o efeito quente de transmissão às folhas iluminadas pelos postes. Árvores, arbustos e samambaias usam perfis de relevo.
- Aplicar terra e cobertura orgânica apenas sob o gramado da margem sul, antes do passeio. Folhiço e galho caído são recortes soltos, posicionados no chão.

Implementação em `SantaRitaRuaNorte.Vegetacao.cs` e `LuzVolumetrica.shader`. Origem e recortes registrados em `ASSET_SOURCES.csv`.

## Prompt da folha

One transparent 3-column by 2-row production sprite atlas of six independent modular pieces for a wet southern Brazilian roadside woodland in Santa Rita, 2004. Use the latest game screenshots as visual reference only. Slightly elevated frontal orthographic perspective, authentic clustered 32-pixel-per-world-unit realistic pixel art, subdued consistent olive green, gray-brown bark, dark rich soil. Diffuse neutral albedo with medium-low contrast, same apparent leaf luminance and hues for all vegetation, no baked spotlight, no directional shadow, no pale outline or gray washed-out leaves. Exact row 1 left to right: a short young native camboatá tree with a rounded dense irregular broadleaf crown and visible split trunk, two groups of layered leaves; a short young native bracatinga with feathery dense spreading low canopy, spreading branches and medium-brown trunk; a horizontal low dense native shrub joining several round foliage masses with gaps and an irregular bottom, seamless silhouette with transparent taper. Row 2: a low fan of clustered ferns with some small thin stems and dry leaves at the roots, roughly horizontal; an irregular FLAT carpet decal of scattered damp brown leaves, dark dirt and twigs, very sparse transparent edges, no rectangular foundation; a short mossy fallen branch with split exposed wood and moss, lying flat along the horizontal axis, no background. True alpha with generous fully transparent margins around each piece, especially between cells. No scene, pavement, contiguous strip, mountain, labels, glow, text or grid lines. Keep all six pieces completely separated and render only one atlas.

## Verificação

Em Play Mode, foram conferidos oito grupos de margem na Rodovia e nove no Posto e no Mercadinho. Os pontos amostrados junto à mala, à travessia e ao gatilho do Bairro estavam livres. A captura auxiliar, na mesma escala de câmera do jogo, registrou as três margens sul. O Graphify permaneceu indisponível, e o índice local tem fontes pendentes.

- [Rodovia](../Evidencias/MercadinhoComposicao/mata-sul-rodovia.png)
- [Posto](../Evidencias/MercadinhoComposicao/mata-sul-posto.png)
- [Mercadinho](../Evidencias/MercadinhoComposicao/mata-sul-mercadinho.png)
