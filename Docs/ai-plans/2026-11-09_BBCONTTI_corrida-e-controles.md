# Corrida e controles de exploração

09/11/2026 · BBCONTTI com Codex · Unity 6000.6.0f1 · Implementado no projeto Santa Rita

O pedido foi melhorar a caminhada e os controles, impedir que E interrompa o movimento e oferecer uma corrida sutil em 2D e 3D.

O catálogo do Obsidian e o relatório estrutural existente apontaram para `PlayerController` e o sistema de interação. A fonte confirmou que o temporizador de 0,6 s bloqueava o movimento mesmo quando E era pressionado sem alvo. O posto, o mercadinho e a casa de Adílio usam `Posto3D`. O salão do bar usa `BarDoZe3D`.

- [x] Associar a corrida a ambos os Shift, com velocidade 25% acima da caminhada.
- [x] Usar aceleração e frenagem graduais, velocidade diagonal normalizada, colisão do Rigidbody2D na rua e deslocamentos curtos nas cápsulas dos interiores.
- [x] Separar a pose de interação do controle de movimento. A tecla E atua no pressionamento e as interfaces modais bloqueiam o deslocamento.
- [x] Atualizar a verificação de caminhada para ler a velocidade física.

Arquivos de código alterados em `Projeto Jogo 2D/Assets/Scripts`: `PlayerController.cs`, `Posto3D.cs`, `BarDoZe3D.cs`, `SinucaRoom3D.cs` e `Capitulo2/Chapter2RuntimeVerification.cs`.

No Unity Editor, uma cena física temporária com entrada pelo Input System verificou aceleração, caminhada com E mantido, corrida com ambos os Shift, diagonal, retorno à caminhada, frenagem e colisão. A mesma execução conferiu os alvos 3D de 3,06 no posto e 2,06 no bar. Em fixtures isoladas, os métodos reais de deslocamento 3D confirmaram 25% a mais de distância no mesmo intervalo no posto e no bar. A cápsula do posto parou diante de uma parede de teste. A sequência completa de entrada, exploração e saída nos interiores e a build independente seguem pendentes.

O refresh do Graphify foi executado e falhou por falta do CLI. A validação apontou arquivos C# ausentes no índice preexistente. O relatório estrutural disponível foi consultado.
