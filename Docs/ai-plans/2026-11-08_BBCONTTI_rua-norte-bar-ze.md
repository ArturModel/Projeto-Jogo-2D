# Exterior do Bar do Zé na Rua Norte

Autor do projeto @BBCONTTI · 08/11/2026 · America/Sao_Paulo · Unity 6000.6.0f1 · `melhoria-minigames`

Implementação validada em Play Mode.

## Pedido e diagnóstico

O pedido reuniu uma nova direção de pixel art no exterior, escala de objetos guiada por Marcus, porta única fechada, revisão do asfalto, da calçada e da grama, árvores com vento, névoa à frente dos objetos, luz ambiente com profundidade e relevo no terreno. O mapa de cenas e a consulta Graphify ligaram o exterior a `ConstruirZona5_RuaNorteBar` em `EntradaDaCidade`. `BarDoZe.unity` representa o interior no fluxo legado.

## Execução

- [x] A Rua Norte passou a montar 38 × 22 células usando 13 famílias com quatro variações de 64 px. As emendas juntam grama, pedra, terra e asfalto, com juntas, musgo, desgaste, poças e pintura viária.
- [x] Fachada, picape, mobiliário, orelhão, postes e vegetação têm sprites da Rua Norte. A porta de uma folha fecha o ponto de entrada. O importador respeita a densidade de 64 PPU e os pivôs de pé desta pasta. Marcus mede 1,50 unidade na pose observada, e a cadeira mede 0,81.
- [x] Quatro silhuetas da flora regional ocupam 17 posições. O vento inclina as copas no shader. Raízes e colisores permanecem ancorados no mundo.
- [x] Dois véus de névoa em espaço de mundo passam sobre cenário, personagens e copas. A densidade varia ao redor da fachada e da soleira. O vento e os véus obedecem à preferência de efeitos reduzidos.
- [x] Luz ambiente comum esfria e diminui no fundo. Os postes e a entrada iluminam volumes XYZ, com sombras projetadas. A textura complementar guarda normal XY, rugosidade e cavidades. A resposta especular usa GGX para os três materiais e poças. Sombras de contato no plano do chão usam as pegadas dos volumes e acompanham Marcus.

## Recursos e autoria

A montagem está em `Assets/Scripts/SantaRitaRuaNorte.cs` e entra pelo `Capitulo1Director`. `SantaRitaArtImporter` respeita os metadados locais. `SantaRitaAtmosphere` e `SantaRitaRimLight` suprimem os efeitos legados na Rua Norte. `Assets/Resources/Art/RuaNorte` reúne sprites, texturas e shaders. As fontes editáveis ficam em `Arte/Producao/RuaNorte` e a procedência em `ASSET_SOURCES.csv`.

Direção dos prompts do ImageGen integrado

- Fachada do bar rural do sul brasileiro em 2004, vista frontal elevada de RPG, reboco creme gasto, lambril azul petróleo, telhas de barro, letreiro BAR DO ZÉ e luz âmbar discreta. A edição focou na porta azul fechada de uma folha e maçaneta lateral.
- Folhas de objetos em pixel art de resolução moderada com transparência e paleta de ardósia, oliva e terracota. Picape escura, orelhão laranja, poste, contentor, mesa redonda, cadeira plástica, engradado, portão e caixotes.
- Asfalto úmido de carvão azulado, calçada de pedra marrom-acinzentada com rejuntes gastos e grama oliva escura com folhas e terra exposta. Os três campos de estudo foram amostrados e encaixados na grade, com bordas produzidas para o Tilemap.
- Folha transparente de canela, araucária, pitangueira e bracatinga, com silhuetas distintas, troncos expostos e raízes em grama oliva escura. Os cortes e pivôs foram exportados na escala do jogo.

## Validação

Em Play Mode, usando a câmera da cena e reposicionamento temporário para as capturas. Comparação no mesmo enquadramento para conferir textura, escala, névoa, vento, relevo e sombras. As capturas ficam em `Docs/Evidencias/RuaNorte`. Os dois shaders compilaram no Editor. A folha auxiliar foi importada em espaço linear. A névoa usou as ordens 2030 e 2031, acima de Marcus, da fachada e dos objetos. O comparativo A/B no mesmo frame confirmou a resposta visual do relevo, dos postes, das sombras de contato, do vento e da névoa.

A interação de porta, o checkpoint do orelhão e as rotas continuam no diretor de capítulo. A fixture conferiu os componentes e a posição da porta e do orelhão. A campanha e o executável distribuível ficam fora da validação visual desta iteração.
