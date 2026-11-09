# Interface e orelhão da exploração

Autor do repositório @BBCONTTI. Execução em 08/11/2026 às 23:40 BRT, no Unity 6000.6.0f1, projeto C:/Temp/SantaRita. Implementação integrada.

## Pedido e diagnóstico

Atualizar inventário e menus com a identidade visual de Santa Rita. Criar uma interação própria para salvar e carregar nos orelhões da exploração. O aparelho frontal do menu era reutilizado no jogo. A bolsa e os atalhos ocupavam áreas separadas da tela. Memória, notas do cofre, direção de arte, sprite original e código dos pontos de entrada foram consultados. A consulta Graphify não executou porque o CLI não estava disponível no shell.

## Execução

- [x] Mover os cinco atalhos para o canto superior esquerdo e abrir a bolsa logo abaixo. Preservar posse, ordem e ações de objetos.
- [x] Aplicar contraste frio, filetes âmbar e entrada suave na pausa. Simplificar tipografia, largura dos alvos e hierarquia no menu principal.
- [x] Separar a cena de exploração do aparelho do menu principal e da lista de carga acessada pela pausa e pelos resultados de minigame.
- [x] Encaixar cinco teclas na placa do telefone. Selecionar o registro e confirmar são ações separadas. Mostrar nome do local e data para identificar o registro. A mão segue o mouse e reage ao clique.
- [x] Integrar arte com camisa escura de manga comprida, Marcus perto do aparelho e braço livre oculto. Fundo genérico desfocado. Chuva e ondulações animadas sobre a arte.
- [x] Preservar compatibilidade com saves, confirmação manual exclusiva no orelhão e fluxo de carga pelo SceneFader.

## Arquivos

Em Assets/Scripts: PayphoneJogoUI, PayphoneCheckpoint, MenuAmbiente, MenuAmbienteGraphic, MenuPausa, SantaRitaInventory e MenuAmbienteRuntimeQA. Em Assets/Editor: MenuAmbienteAuthoring. Arte e importação em Assets/Resources/UI/OrelhaoJogo, procedência em ASSET_SOURCES.csv. Nenhuma cena precisou ser salva.

## Geração de arte

ImageGen integrado. Para Rua.png: “Editar a cena pixel art em 16:9. Preservar o enquadramento do orelhão ocre e a placa escura vazia. Marcus de costas em três quartos, junto do aparelho, usa camisa preta de manga comprida até o pulso e jeans conforme MarcusWalk.png. Mostrar a mão no monofone e ocultar completamente o braço livre atrás do tronco. Manter rua genérica muito desfocada, piso úmido e reflexos âmbar. Sem chuva, respingos, letras ou interface na imagem.”

Para Mao.png: “Sprite de cursor em alpha, mão direita de Marcus vista de trás, pele parda, indicador estendido à direita e os outros dedos recolhidos. Pixel art em grupos nítidos, luz âmbar contida, somente mão e punho, sem fundo ou objetos.”

## Validação

Roslyn compilou as assemblies de jogo e Editor na cópia de trabalho com as referências Unity do projeto. O primeiro teste revelou incompatibilidade Action/UnityAction no botão de desligar, corrigida antes da transferência. A advertência de BochaMinigameUI já existia. A fixture MenuAmbienteRuntimeQA usa Input System e saves isolados.

A validação em Play Mode e a atualização do índice estrutural são registradas abaixo.

## Revisões de interface e clima

Pausa e configurações passaram para o centro. Inventário, pausa e foco do menu principal usam uma textura de metal pintado com desgaste fino, margens fatiadas e transparência. O aviso do caderno fica no canto superior direito. As teclas de salvar e carregar foram incorporadas à placa do orelhão.

A camada de clima ficou invisível na primeira implementação porque o objeto não tinha CanvasRenderer, confirmado na sessão de Play. MenuAmbienteGraphic agora declara o componente obrigatório. A captura enviada pelo autor na rodada seguinte mostrou chuva e névoa sobre a cena. Ajuste final mantém 238 riscos animados, reduz opacidade e saturação da chuva, remove as ondulações do piso e conserva respingos sobre a concha. O shader OrelhaoGrade aplica uma dessaturação de 28% ao cenário, separada da interface. C# recompilado com as referências do Unity. A verificação visual da última calibração ficou pendente. A revisão automática de aprovação bloqueou a captura de QA por limite de uso. A sessão de Play foi preservada.

Graphify refresh falhou por ausência do CLI; o índice estrutural não foi atualizado nesta rodada.
