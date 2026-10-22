# Fontes da revisão visual

Ferramenta de criação: ImageGen integrado, sem API externa ou plugin de geração adicional. As saídas originais estão nesta pasta. Corte, escala nearest e preparação de fontes editáveis: Aseprite compilado localmente, scripts em `Tools/Aseprite`. Imagens geradas não são declaradas CC0.

## NPCs de C2

Referência: MarcusWalk.png. Atlas 4×2 transparente com Tonão, Bento, Clarice, Alcina, Dario, Mila, Celina e Nair. Adultos de Santa Rita em 2004, perspectiva 3/4 de cima, contornos escuros, grupos de pixels definidos, cores contidas e personalidades nas roupas/silhuetas. Mesma escala útil de Marcus; nenhuma pintura suave, texto ou cenário. Cada corpo completo, com pés visíveis e margem entre células. Exportação: célula 64×64, corpo 48, pés a 8 pixels da base.

## Materiais

Atlas com asfalto escuro sem linhas embutidas, grama/solo, terra úmida, concreto, pedra basáltica, madeira, cerâmica e água. Direção de pixel art sóbria, materiais legíveis, luz neutra, sem sombras dramáticas. Variantes de marcação, cor de cerâmica e areia foram preparadas pelo Lua. Macros de asfalto/terra: 128×128 divididos em 16 células contínuas. Grama em revisão adicional nativa pelo script `grass-material.lua`, para reduzir aparência interpolada.

## Flora

Referência principal: imagem ambiental fornecida pelo autor em 21/10, conservada em Docs/Referencias. Atlas transparente: duas árvores de copa irregular, araucária, arbusto, flores rosadas, capim com pedras, cerca de tábuas e samambaia. Folhagem detalhada em pequenos grupos de pixels, cores oliva/ocre, troncos gastos e perspectiva 3/4. Sem chão, sombra destacada ou iluminação de poste pintada na imagem. O corte foi ajustado ao corredor transparente entre árvores e plantas baixas; não dividir a imagem ao meio cegamente.

## NPCs de C1 e cotidiano

Referências: imagem ambiental do autor e Marcus. Frentista de uniforme azul, atendente com avental, Ramiro de suéter/cap, Zé de colete, frequentador do bar, Chico de chapéu, passageira com mala e motorista com horário dobrado. Atlas transparente 4×2, roupas individuais, proporções adultas, mesma linguagem de pixel art e exportação 64×64.

## Fachadas e caminhada

Oito fachadas de C1: casa do avô, três casas rurais, mercadinho, Bar do Zé, cobertura do posto e venda. Atlas original com arquitetura de madeira/reboco, telhas de barro, janelas quentes e perspectiva da referência ambiental. Fontes em Fachadas; exportação 256×256 a 32 PPU. Caminhada de Marcus recebeu acabamento nativo pelo Lua, sem gerar outro rosto ou outra roupa; fontes e GIFs em Marcus. Grama macro final produzida nativamente pelo grass-material.lua.


Asfalto final refeito nativamente em pixels por asphalt-material.lua: agregado fino, contraste reduzido, fissuras e pintura dupla; o macro gerado antes, que lembrava pedra grande na câmera, foi substituído após inspeção da build.

