# Prólogo e chegada à Rodovia

Autor do registro @BBCONTTI · 09/11/2026 · America/Sao_Paulo · Unity 6000.6.0f1, URP 2D

A revisão leva o acabamento da Rodovia para o prólogo, reforça o temporal e liga o ponto onde Marcus estaciona ao início da exploração.

## Diagnóstico

A memória de produção, o mapa de cenas do Obsidian e a direção de arte apontam para a Rodovia como continuação direta de `Prologo`. O código anterior terminava a viagem em terra. A Rodovia já tinha um recuo pavimentado, calçada, placa, parada e postes. A consulta Graphify ficou indisponível por falta da versão 0.9.96. As posições, o piso e o spawn foram conferidos no código.

## Execução

- [x] Reusar Terreno, espécies de árvore, arbustos, postes e a folha de objetos da Entrada. O trecho final lê as mesmas células de piso que a cena 2D.
- [x] Voltar ao asfalto antes da manobra. Usar a posição do sedã e as posições da placa, da parada, do orelhão e dos postes na projeção. Orientar o sedã para leste.
- [x] Escurecer a paisagem, engrossar a névoa baixa e compor a chuva com duas cortinas e gotas aderidas ao vidro.
- [x] Projetar o recorte dos objetos no chão a partir dos postes ou dos faróis e aplicar sombras de contato. Manter iluminação fria e focos âmbar.
- [x] Atualizar a verificação da posição de parada e registrar evidência no Editor.

## Arquivos

`PrologoJogavel.cs`, `PrologoEstradaGraphic.cs`, `AsfaltoMolhado.shader`, `SantaRitaRuaNorte.cs`, `Capitulo1Director.cs` e `PrologoRuntimeVerification.cs`. Os recursos visuais vêm de `Assets/Resources/Art/RuaNorte`. As cenas serializadas seguem no fluxo existente.

## Validação

- A [evidência do Editor](../Evidencias/PrologoRodovia) registra asfalto, terra, aproximação, parada e desembarque. A fixture avançou o relógio para amostrar os quadros e executou a atualização e transição do jogo. A distância permaneceu em 468, a lateral em −4,18 e a velocidade em zero. Marcus surgiu em `saida_carro` com a conclusão em RAM. A captura `00-antes-do-clima.png` mostra a primeira revisão antes da névoa e da chuva reforçadas.
- A build Windows Development foi gerada. A execução automatizada do player não produziu o relatório da fixture. A verificação de visual e chegada desta rodada foi feita no Editor.
- O refresh do Graphify ficou indisponível sem o executável 0.9.96. O validador apontou fontes já existentes que faltavam no índice anterior.
