# Alucinações aleatórias na bocha

Autor do projeto @BBCONTTI · 07/11/2026 às 01h10 (São Paulo)  
Estado implementado · Unity 6000.6.0f1, Editor Windows

## Solicitação

Levar à bocha o sorteio da sinuca. Marcus vê uma figura sentada que some após o olhar sustentado e uma bocha que cai num buraco e volta ao mesmo lugar. Tonão segue o jogo sem perceber as ocorrências.

## Decisões e execução

- [x] Conferir a memória, as notas manuais do Obsidian, a Bíblia R1, as direções de produção e a matriz do capítulo 2. A consulta ao Graphify falhou porque a ferramenta não estava disponível.
- [x] Extrair a agenda comum sem mudar a ordem de sorteio, os intervalos ou o contrato público da sinuca. A bocha habilita a agenda durante o duelo, a partir de uma vez de Marcus elegível. A agenda é transitória e exclusiva.
- [x] Reaproveitar o sprite sentado existente em `Art/Characters/Npc_Frequentador_Sentado`, da produção Bar2004 registrada em `ASSET_SOURCES.csv`. Figura no primeiro banco esquerdo. O medo requer 0,42 s de olhar contínuo. Depois do susto, a figura permanece por 2,45 s. Uma aparição que Marcus não percebe expira em 26 s. A mão e a mira tremem, com um tremor residual reservado à primeira preparação. A preferência de reduzir efeitos estabiliza ambas.
- [x] Recortar a abertura no saibro e no assoalho apenas na sala da bocha. Durante 1,85 s, manter a simulação na posição e velocidade do instante da abertura e animar só a representação da bocha. Após fechar o buraco, reapresentar a posição original e retomar a simulação.
- [x] Usar legendas de Marcus nos dois eventos. Tonão mantém sua rotina e comentários normais sobre a partida. O batimento é síntese original de pulso duplo a 22.050 Hz. A queda reutiliza o foley de madeira e saibro do minigame com pitch reduzido.
- [x] Aplicar a configuração de volume de efeitos e suspender o áudio transitório nos bloqueios do minigame. Exercitar a figura, a restauração e o encerramento em uma fixture isolada.

## Arquivos

- `Assets/Scripts/Capitulo2/BochaMinigameUI.cs` e `BochaMinigameUI.Perception.cs`
- `Assets/Scripts/Capitulo2/BochaRoom3D.cs` e `BochaRoom3D.Perception.cs`
- `Assets/Scripts/Capitulo2/BochaCourtView.cs`
- `Assets/Scripts/SinucaPerceptionSchedule.cs`
- `Assets/Resources/Art/Sinuca/SalaLit.shader`
- Evidências, notas de arquitetura e memória abaixo.

## Verificação

- Unity Play Mode com fixture isolada e sorteios direcionados. 37 verificações, com 2.048 disparos de agenda de dois eventos, olhar, tremor e acessibilidade, Ponto/Rafa/Volo, rejeição de Tonão, posição restaurada e trajetória posterior comparada a um controle. [Resultado](../Evidencias/BochaPercepcoes/qa-bocha.txt).
- RenderTexture da câmera do minigame, sem HUD. [Figura](../Evidencias/BochaPercepcoes/01-figura-focada.png), [banco vazio](../Evidencias/BochaPercepcoes/02-banco-vazio.png), [queda](../Evidencias/BochaPercepcoes/03-abertura-e-queda.png), [abertura](../Evidencias/BochaPercepcoes/04-buraco-aberto.png) e [restauração](../Evidencias/BochaPercepcoes/05-bocha-restaurada.png).
- Console do Unity com zero erros após a validação. Harness existente da sinuca compilado com o arquivo de produção e 4.131.611 verificações em 1.000 sementes e 24.000 eventos.
- Refresh do Graphify executado. Ferramenta `graphify` ausente. Validate executado. O índice existente não inclui os arquivos novos e outros scripts preexistentes.

## Pendências

Audição e ritmo em partida completa, input físico e build Player. A fixture deixa o áudio mudo para não interferir na sessão do editor.
