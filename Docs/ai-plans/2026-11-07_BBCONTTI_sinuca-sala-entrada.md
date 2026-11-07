# Sala e entrada da sinuca

Autor @BBCONTTI · 07/11/2026 · Unity 6000.6.0f1 · `melhoria-minigames`  
Status: implementado e aprovado pelo autor após teste em jogo.

## Pedido e diagnóstico

O autor pediu um salão 2D mais largo, com a decoração e a atmosfera da sala em primeira pessoa, e uma passagem mais elaborada ao começar a sinuca. O índice e o mapa de cenas do Obsidian apontam para o Bar do Zé dentro de `EntradaDaCidade`. O código confirmou que `SantaRitaBarLayout` monta o salão, `Capitulo1Director` ajusta a luz ambiente e `InteracaoCapitulo1` inicia o duelo após a conversa. A consulta ao Graphify não rodou porque o executável não estava disponível.

## Execução

- [x] Ampliar de 9,5 para 12,5 unidades a largura explorável, afastar móveis laterais e alinhar as colisões com o piso. Mesa central, soleira e escala dos sprites foram mantidas.
- [x] Reaproveitar a madeira e o lambril. Adicionar frisos, arandelas, pendente verde, luz de balcão e sombras de contato e projetadas. O ambiente mais frio deixa os pontos quentes conduzirem a leitura da sala.
- [x] Fechar a íris sobre a lâmpada no 2D, revelar o pendente em primeira pessoa e baixar o olhar até o feltro. Após a revisão do autor, o fechamento passou a usar o grão do próprio piso sob luz âmbar discreta. A escolha dos grupos aparece depois da entrada de cerca de 3,7 s.
- [x] Usar a transição suave quando a preferência de reduzir efeitos intensos estiver ativa.

## Arquivos

`SantaRitaBarLayout.cs`, `SantaRitaBarLighting.cs`, `Capitulo1Director.cs` e respectivos metadados cuidam da exploração. `InteracaoCapitulo1.cs`, `SceneFader.cs`, `SinucaMinigameUI.cs`, `SinucaRoom3D.cs`, `SinucaRoom3D.Scenery.cs`, `MenuPausa.cs` e `EntradaSinuca.shader` implementam a apresentação e o bloqueio de entradas. A verificação `Sinuca3DRuntimeVerification.cs` aguarda o fim da entrada.

## Verificação

- Compilação C# com referências da Unity: 0 erros, 3 avisos externos ao escopo.
- Unity Editor: piso de 12,5 unidades, cinco luzes Sprite-Lit e 36 renderizadores de sombra. Shader final compilado e renderizado separadamente em 640×360.
- O autor informou ter testado e aprovado o resultado final em jogo.
- A cópia isolada em modo batch encontrou problemas de licenciamento do Editor. Graphify refresh indisponível e validate apontou fontes fora do índice existente.

As capturas locais estão em `Docs/Evidencias/SinucaEntrada/`: referência inicial, vista em primeira pessoa durante a partida e render isolado da textura no fechamento da íris. O repositório ignora imagens nessa pasta.
