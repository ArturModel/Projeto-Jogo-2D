# Composição da estrada e da mata do Bairro

Autor @BBCONTTI. Data 09/11/2026. Unity 6000.6.0f1, URP 2D, EntradaDaCidade.

O pedido inclui vegetação em todo o perímetro da quarta zona, balizas de madeira da subida anterior, folha de arte exclusiva e acabamento para a estrada de chão. O mapa do C1, as notas de arquitetura, a memória e as orientações de pixel art e QA foram consultados. A cena já usava trilhas em cinco segmentos e passeios de pedra na frente das casas. A mata deixava grandes espaços entre as árvores e o piso tinha pouca variação localizada.

- Mantidos os segmentos, acessos, interações e gatilhos do diretor.
- Copas adultas e jovens em estratos, submata de samambaias e moitas, folhiço e margem orgânica no sul, no centro, nos fundos e nos dois extremos da zona.
- Quatro balizas quentes reaproveitadas. O shader compartilhado aplica a luz local também na Zona4.
- Oito recortes independentes em `Assets/Resources/Art/RuaNorte/Bairro/Detalhes/`, com mapas de material. O plano de composição fica em `SantaRitaRuaNorte.Bairro.cs`. A caixa do nº 40 conserva seu ponto de interação. O muro baixo reutiliza as bases físicas das antigas cercas.
- Chão com sulcos e sedimento, bordas de poça, entrada em pedra, cascalho e drenagem.

## Fonte da arte

OpenAI ImageGen integrado. Folha preservada em `Arte/Producao/EntradaDaCidade/Bairro-Detalhes-ImageGen.png`.

> Transparent PNG sprite atlas for Santa Rita rainy southern Brazilian rural residential dirt lane, circa 2004. Four by two strict equal rectangular grid of eight independent isolated pixel-art cutouts, ample clear gutters, each item entirely centered in its cell. Top row: mossy rough stone and brick driveway lip, worn concrete roadside culvert mouth with rocks, a low old rural wooden mailbox with post, two terracotta pots with dark leafy plants. Bottom row: paired muddy tire ruts with scattered gravel viewed from above, irregular shallow silty puddle with broken gravel shoreline viewed from above, irregular wet packed gravel alluvial patch with sparse brown leaves viewed from above, low eroded grassy road edge and exposed soil with stones viewed from above. Objects have the slightly elevated frontal view of a top-down 2D realistic pixel game; earth decals lie flat. Muted cool brown, olive, charcoal, faded brick, realistic compact pixel clusters, comparable to 32 pixels per game unit. No scene, no captions, no baked lamp light or cast shadows. True transparent background with soft broken contour alpha limited to dirt decals. Wide landscape atlas.


## Ajuste após revisão visual

Reduzidos a opacidade, a escala e o contraste dos detalhes sobre o barro. As margens de grama ficaram mais espaçadas e alinhadas à estrada. Os sulcos contínuos e as manchas úmidas ficaram no material do chão. Os postes altos receberam o halo, a dispersão e a projeção direcional da Encruzilhada. A intensidade dos reflexos leva em conta a terra e a água de cada ponto. A correção da resolução dos módulos do atlas confirmou os cinco muros de pedra.

## Validação

- Play Mode em EntradaDaCidade, com transição do diretor à Zona4 como fixture. Capturas antes e depois em câmera auxiliar com escala do jogo, mais captura da Game View em chuva, em Docs/Evidencias/BairroEstradaMata.
- Confirmados 127 sprites de árvore, perfis de folha nos 127, quatro balizas com fonte local, cinco postes altos com cone registrado, cinco muros e três gatilhos originais. Shaders de terreno e névoa sem erros na consulta ao Editor. A campanha e a build standalone não foram exercitadas.
- Graphify query/refresh indisponíveis por falta da CLI. O validador sinalizou fontes ausentes no grafo existente.
