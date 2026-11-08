# Relevo adicional no exterior do Bar do Zé

Autor do projeto @BBCONTTI · 08/11/2026 · America/Sao_Paulo · Unity 6000.6.0f1 / URP 2D

Implementado e conferido no Editor em Play Mode.

## Pedido e diagnóstico

Acrescentar relevo e resposta à luz em todas as superfícies do exterior, preservando os presets existentes. A direção de arte, o plano anterior da Rua Norte e o mapa do C1 identificam o Tilemap montado por `SantaRitaRuaNorte` em `EntradaDaCidade`. O código confirma que `Terreno` fornece a cor e `Superficie` já fornece as normais, a rugosidade e a oclusão de base.

## Execução

- [x] Gerar `Relevo.png`, atlas linear de altura, gradientes XY e oclusão local para as 52 peças. A fonte reproduzível é `Arte/Producao/RuaNorte/gerar-relevo-rua-norte.py`.
- [x] Combinar os gradientes com a normal existente, levar a altura ao cálculo das luzes e aplicar oclusão nas cavidades. Rejuntes, pedriscos, tufos, terra, meio-fio, ralo, rachaduras e bordas de poça usam perfis distintos.
- [x] Carregar a camada extra apenas com atlas compatível. Manter os presets `Terreno.png` e `Superficie.png` com hashes idênticos aos originais.
- [x] Comparar a intensidade 0 e 1 no mesmo enquadramento. A escala da câmera, as lâmpadas e os elementos da rua foram compartilhados entre os quadros.

## Verificação

O Unity carregou o atlas como textura linear `R8G8B8A8_UNorm`, filtragem Point, 256 × 832. O material montado em Play Mode ficou com a intensidade 1 e os dois atlas auxiliares vinculados. O shader foi aceito pelo Editor e o console não registrou erros do projeto.

As imagens foram renderizadas com uma câmera temporária na escala de jogo enquanto Marcus estava no interior. Assim a comparação do terreno usa o mesmo quadro sem depender da sessão do jogador. [Preset original](../Evidencias/RuaNorte/relevo-adicional-antes.png) · [Com a nova camada](../Evidencias/RuaNorte/relevo-adicional-depois.png).

O relevo é visual e participa da iluminação. A malha de circulação do Tilemap continua a mesma. A verificação cobre o Editor em Play Mode. A build distribuível não foi executada nesta iteração.

O CLI do Graphify não estava instalado. A validação do índice anterior indicou duas fontes C# do interior ausentes. As notas manuais e o código real serviram de referência.
