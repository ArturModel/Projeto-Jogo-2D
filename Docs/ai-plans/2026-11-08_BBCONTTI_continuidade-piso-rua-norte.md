# Continuidade do piso da Rua Norte

Autor de referência @BBCONTTI · 08/11/2026 · Unity 6000.6.0f1 / URP 2D
Status: aplicado na Rua Norte de EntradaDaCidade, verificado em Play Mode.

## Solicitação e diagnóstico

O autor pediu uma passada de imersão no exterior do Bar do Zé e, na revisão, concentrou o acabamento nas emendas de grama, asfalto e calçada. A memória e o mapa do Obsidian apontaram para a montagem em SantaRitaRuaNorte. O código e o atlas confirmaram o Tilemap de 38 × 22 células. Os mapas auxiliares acentuavam o contraste de células com trincas e poças, além da descontinuidade do atlas.

## Execução

- [x] Construir o mapa de vizinhança junto com o Tilemap e misturar albedo, rugosidade, cavidades, altura e inclinação nos encontros.
- [x] Variar a largura das transições no mundo, suavizar o contraste dos mapas de relevo e manter o traço de tinta no tile de origem.
- [x] Derivar trincas e poças do mesmo asfalto de base e limitar a mudança de altura aos detalhes.
- [x] Na primeira passada, concentrar a luz quente na entrada do bar, reduzir luz periférica e agrupar as cadeiras e a vegetação.

Alterações em Assets/Scripts/SantaRitaRuaNorte.cs, Assets/Resources/Art/RuaNorte/LuzVolumetrica.shader, Assets/Resources/Art/RuaNorte/Relevo.png e Arte/Producao/RuaNorte/gerar-relevo-rua-norte.py. O atlas de cor, os metadados e as cenas serializadas foram preservados.

## Verificação

O Unity recompilou sem erro de C#. O shader foi importado e usado em Play Mode na Rua Norte. A malha lógica, o mapa de vizinhança de 38 × 22 e a porta estavam presentes. [Antes](../Evidencias/RuaNorte/Continuidade/antes.png) e [depois](../Evidencias/RuaNorte/Continuidade/depois.png) mostram a câmera de jogo em posições próximas, com chuva em instantes diferentes. A rodada não gerou build independente. Nenhum slot foi gravado para o teste.

Graphify query e refresh indisponíveis por ausência do executável. Validate executado e apontou BarDoZe3D.cs e SinucaRoom3D.Bar.cs ausentes no grafo anterior. As notas manuais foram atualizadas.
