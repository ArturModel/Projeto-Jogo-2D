# Tonão no minigame da bocha

Autor @BBCONTTI (Git BBCONTTI). Execução Codex. 07/11/2026, 01h10, America/Sao_Paulo. Unity 6000.6.0f1 e URP no Windows. Implementado e verificado em fixture no Editor.

## Solicitação

Dar ao adversário da bocha o detalhamento e a presença que Zé tem na sinuca. Tonão deve andar até a posição escolhida pela IA, fazer o gesto por ângulos diferentes, reagir à partida e falar com legendas e murmúrios. A revisão do autor apontou que o primeiro sprite trocava a bocha de mão no arremesso.

## Diagnóstico e decisões

Consultados a Bíblia R1, a investigação em `Assets/Resources/Cap2/Investigation.json`, a direção de produção, a memória e as notas de arquitetura. Tonão é o campeão local e homem dos reparos da Sociedade Beira-Rio. Costuma insistir numa nova tentativa quando o primeiro plano falha. As falas de jogo usam essa teimosia e preservam o testemunho narrativo para o diálogo que já o entrega.

O chat de ajustes da bocha forneceu a base de áudio, iluminação e vista superior. O código corrente também tinha as percepções de Marcus. A integração dá prioridade à legenda de Marcus quando a percepção está falando. A câmera mantém o controle do jogador durante o arremesso de Tonão.

- [x] Manter as três posições físicas em todos os tipos de lançamento.
- [x] Percorrer a faixa livre até o `x` escolhido e só então preparar o gesto.
- [x] Usar frente, costas e dois lados do perfil. Desenhar os dois perfis de arremesso para que a direita faça o gesto quando o jogador muda de lado.
- [x] Manter a bocha na direita nos três primeiros quadros. Inserir a bocha no simulador no mesmo passo que seleciona o quarto quadro, com a palma direita aberta.
- [x] Integrar com luz e sombra de contato da cancha. Exibir comentários contextuais com escrita progressiva, pausa e volume de fala.

## Arquivos

- `BochaAI.cs`, `BochaRoom3D.cs` e `BochaRoom3D.Tonao.cs` integram as posições, a caminhada, os quadros e a luz.
- `BochaMinigameUI.cs`, `BochaMinigameUI.Tonao.cs` e `BochaMinigameUI.Perception.cs` conectam o momento da soltura, a legenda, o áudio e sua limpeza.
- `Assets/Resources/Bocha/TonaoBocha.png` é o atlas de 512 × 896, com 28 células de 128. `Arte/Producao/TonaoBocha` contém fontes geradas, os prompts e a animação editável. `Tools/Aseprite/export-tonao-bocha.py` reproduz a exportação. `ASSET_SOURCES.csv` registra a procedência.
- [Capturas animadas da fixture](../Evidencias/TonaoBocha/Arremesso.gif) registram os quatro enquadramentos da mesma ação. A comparação de ângulos usa uma sala isolada e câmera fixa.

## Verificação

- Unity importou o atlas com filtro Point e um nível de mip. A fixture executou 27 combinações de posição, tipo de lançamento e passo de tempo, checando caminhada, três poses com bocha, momento de soltura, consumo único do estoque e origem física.
- Dezesseis capturas reais do Editor conferiram os quadros de arremesso em quatro enquadramentos. A legenda passou por início, escrita progressiva, pausa, retomada e desaparecimento. O murmúrio foi criado pela fonte independente de voz. A IA avaliou 18 candidatos em uma amostra com bola de Marcus válida.
- A consulta e o refresh do Graphify ficaram indisponíveis porque a CLI está ausente. O validador executou e acusou fontes fora do índice, incluindo alterações anteriores a este trabalho.
- Avaliação em partida completa, audição humana e build Player pendentes.
