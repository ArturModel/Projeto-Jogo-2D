# Reforma da Rodovia de Entrada

Autor do projeto @BBCONTTI · 08/11/2026 às 20:45 · America/Sao_Paulo · Unity 6000.6.0f1, URP 2D

A Rodovia de `EntradaDaCidade` recebeu a linguagem visual da Rua Norte do Bar do Zé. A revisão inclui a legenda das falas do Marcus, a correção do passeio junto à mala e o ajuste das sombras.

## Diagnóstico

O [guia visual](../Guia_Visual_Santa_Rita.md), a memória e o mapa do Obsidian foram comparados com a cena ativa e com o código. A zona é montada por `Capitulo1Director`. O primeiro protótipo ainda exibia caixotes na interação da mala e uma faixa de barro interrompia o passeio. Caixas de oclusão criavam projeções retangulares sob o sedã e outros objetos. A VT323 compartilhada usava material Bitmap, inclusive nos textos serializados do menu.

## Implementação

- A zona usa o atlas de piso, os mapas de superfície e relevo, as árvores, os postes e o orelhão da Rua Norte. A planta da entrada tem suas próprias margens, recuo do carro, três pontos de luz e névoa de fundo. O passeio sul é contínuo na posição da mala.
- A [prancha de produção](../../Arte/Producao/EntradaDaCidade/Fontes.md) usa a picape como referência. O jogo importa uma folha a 64 PPU com cinco recortes. Sedã 270 × 116, letreiro 223 × 162, banco 128 × 72, quadro 107 × 128 e mala 60 × 56 pixels. Os pivôs ficam na base.
- O sedã recebe contato suave sob os pneus e uma projeção arredondada única que combina carroceria e cabine. O banco, a lixeira e os objetos pequenos recebem contato elíptico. As árvores da entrada não contribuem para a sombra. A placa principal mantém a projeção.
- `SantaRitaFalasEntrada` compartilha com a sinuca a caixa preta, a revelação de texto, a síntese de sílabas, o tom do Marcus e o volume base 0,20 multiplicado pela preferência de fala. A câmera aproxima 2,5% na fala espontânea e 6% quando ela parte de uma interação. A tecla E menor aparece perto dos objetos e pode avançar a fala durante uma interação. O quadro mantém a leitura do documento e a fala vem ao fechar.
- A VT323 usa SDF no mesmo asset, com os mesmos vínculos no menu e na interface. A legenda de exploração usa o material sem camada de sombra tipográfica.
- A lanterna, o ID do checkpoint da entrada, as rotas e as flags do carro continuam no fluxo normal do capítulo. A reforma roda na montagem da zona, sem uma nova cena serializada.

## Arquivos

- `Assets/Scripts/` reúne as alterações de montagem, câmera, controles, indicação, diálogo e materiais.
- `Assets/Resources/Art/RuaNorte/` contém os recursos reutilizados, a folha `Entrada/ObjetosEntrada.png` e o shader compartilhado.
- `Assets/Resources/Fonts/VT323-Regular Bitmap.asset` contém o material SDF.
- `Arte/Producao/EntradaDaCidade/` contém a fonte editável, as imagens de geração e os prompts. `ASSET_SOURCES.csv` registra a procedência.

## Validação

- Play Mode na cena ativa. O Unity importou os cinco sprites a 64 PPU, carregou o material SDF e adicionou os caracteres em português da amostra. O tile sob a mala pertence à família Walk.
- A revisão do carro foi conferida no material da cena pelo perfil Veiculo. O perfil calcula o contato e a visibilidade da luz separadamente.
- Capturas da câmera do jogo em [evidências da entrada](../Evidencias/EntradaDaCidade).
- A primeira tentativa automatizada de fala perdeu o foco do Game View e não concluiu a medição de áudio ou zoom. O executável e a campanha completa ficaram fora desta rodada de QA.
- O refresh de Graphify ficou indisponível sem o executável 0.9.96. A validação do grafo anterior encontrou scripts ausentes da revisão do Bar e da entrada. A inspeção usou o código, o Obsidian e o Editor.
