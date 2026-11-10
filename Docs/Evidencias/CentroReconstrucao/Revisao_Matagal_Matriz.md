# Revisão de matagal e pisos da Matriz

Referência anterior de câmera em [Matriz detalhada](matriz-detalhada.png) e nas duas imagens do autor. A captura histórica precede esta revisão.

- Sprites raster de [pavimento miúdo](../../../Projeto%20Jogo%202D/Assets/Resources/Art/Cap2/Remodelagem/Matriz/LajeGranito.png) e [pedregulho](../../../Projeto%20Jogo%202D/Assets/Resources/Art/Cap2/Remodelagem/Matriz/Pedregulho.png). Cada imagem corresponde aos quatro quadrantes do respectivo material no atlas.
- Comparação Pillow dos três atlas de 256×960 confirmou igualdade pixel a pixel das treze linhas reaproveitadas. As duas linhas novas são distintas em todos os mapas. O ponto (58,16) da arte do poste tem cor âmbar e corresponde ao ponto (13,16) na imagem espelhada, com coordenadas de imagem a partir do canto superior esquerdo.
- Compilação C# com zero erros. A instância do Editor ficou indisponível via Unity CLI. Captura posterior, percepção das junções, densidade em câmera normal e alinhamento visual dos cones aguardam Play Mode.
