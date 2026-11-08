# Bar do Zé em 3D pela porta da Rua Norte

Autor de referência: @BBCONTTI. Data: 08/11/2026. Ambiente: Unity 6000.6.0f1, Editor Windows. Implementado e revisto após retorno visual do autor. A paleta final foi escurecida depois desse retorno.

## Pedido e contexto

Entrar em primeira pessoa pela interação da porta, explorar e falar com Zé no mesmo salão da sinuca. Após as revisões, Marcus abre a porta pela maçaneta externa direita. Zé fica junto à mesa. A Rua Norte mantém os efeitos até a máscara escura cobrir todo o quadro. A câmera 3D revela a mesma maçaneta. O autor pediu a máscara um pouco mais escura, com névoa animada.

Consultadas a memória do projeto, as notas de cenas e arquitetura do Obsidian, a Bíblia R1, os critérios de arte e o código da porta, diálogo e sinuca. O Graphify não estava disponível para consulta ou refresh. O validador informa que as duas novas unidades de C# ainda não constam do grafo.

## Implementação

- `Capitulo1Director` encaminha a porta para o salão 3D e recupera a Rua Norte ao sair. Cargas com referência ao bar legado usam a exploração 3D. O piso e a parede compartilham a pintura original, sem construir o antigo salão 2D na cidade.
- `SceneFader` e `EntradaSinuca.shader` fecham e abrem no mesmo ponto, com máscara escura e névoa. A zona e a câmera só mudam depois de um quadro totalmente coberto. Efeitos reduzidos usam fade simples.
- `BarDoZe3D`, `SinucaRoom3D.Bar`, `SinucaRoom3D` e seus parciais de arquitetura conduzem a câmera, a abertura pela dobradiça esquerda, o sprite `MarcusMaoMacaneta.png`, a exploração e os pontos de interação. O sprite usa a paleta e a escala do jogo com punho escurecido para Marcus.
- `PlayerController` bloqueia input 2D durante a transição e exploração do bar. `SinucaMinigameUI` e `SinucaTableView` compartilham a sala com a exploração. A conversa aciona o fluxo existente da sinuca, e ao fechar o duelo a exploração retoma a posição anterior.

## Verificação

Unity compilou sem erros após as correções. Foi observado em Play Mode que a cena legada entra no salão 3D e que a entrada da Rua Norte chega à zona do bar. O autor avaliou a transição em jogo e pediu escurecimento adicional. A rodada automatizada de captura não completou a sequência de evidências. A confirmação visual da última paleta, a regressão integral de input e o Player ficam pendentes. Nenhuma cena foi salva durante a verificação.
