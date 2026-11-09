# Inspeções e orelhão na exploração

- Autor @BBCONTTI com Codex
- Data 09/11/2026, America/Sao_Paulo
- Ambiente Unity 6000.6.0f1, branch melhoria-minigames
- Status implementado com testes focados no Editor

## Pedido

Revisar as interações da Rodovia ao Bar do Zé, incluindo interiores em 3D. Apresentar inspeções com aproximação lateral da câmera, leitura digitada, fundo escuro da legenda e efeito sonoro de papel. Padronizar as falas com legenda no rodapé e os tons dos personagens. Usar a mesma aproximação no orelhão e posicionar seus controles junto ao aparelho real na cena.

## Contexto

Memória persistente, mapa de cenas, Bíblia, direção artística, relatório Graphify existente e código C# do projeto em execução. O mural de regras apontava para uma posição legada do Bar 3D. A leitura de rastros do Bairro passou a ter um decalque na posição interativa.

## Execução

- [x] Vincular interações exteriores aos renderers do cenário e usar os focos físicos nos interiores.
- [x] Reusar a legenda e os tons da sinuca, incluindo Marcus e Zé.
- [x] Aplicar a curva de aproximação e de retorno, painel discreto de leitura e digitação com cadência de pontuação.
- [x] Integrar os controles do orelhão à câmera da rua. Manter confirmação de gravação e carga por registro.
- [x] Testar os dois orelhões de C1, incluindo carga cruzada, em armazenamento de teste isolado.

## Arquivos

`CameraFollow.cs`, `SantaRitaInspecao.cs`, `SantaRitaFalasEntrada.cs`, `MinigameDialogue.cs`, `Legenda.cs`, `InteracaoCapitulo1.cs`, `Capitulo1Director.cs`, `Posto3D.cs`, `BarDoZe3D.cs`, `SinucaRoom3D.Bar.cs`, `SinucaRoom3D.Decoration.cs`, `SinucaMinigameUI.cs`, `SantaRitaRuaNorte.Posto.cs`, `SantaRitaRuaNorte.Bairro.cs`, `PayphoneCheckpoint.cs` e `PayphoneJogoUI.cs`.

## Validação e limites

Unity compilou sem erros. A fixture do orelhão passou em 15 verificações, com captura de enquadramento, conferência dos cinco registros, gravação explícita e carga de um orelhão para o outro. A fixture posicionou Marcus perto de cada aparelho e acionou a interação e os botões pelo código do jogo. Nenhum slot pessoal foi usado. O ensaio integral em executável e a audição permanecem pendentes. O refresh de Graphify encontrou a CLI ausente.


Fixtures de inspeção passaram em 16 verificações externas nas cinco zonas, nove verificações nos interiores 3D de Posto, Mercadinho e casa de Adílio, e uma verificação dos quadros no Bar 3D. Captura e registro do orelhão em Docs/Evidencias/OrelhaoEmCena. O validador de Graphify confirmou que o índice anterior está desatualizado. Sessão original restaurada em RAM após QA.

