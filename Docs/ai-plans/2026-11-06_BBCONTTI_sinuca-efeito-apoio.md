# Sinuca — ajuste de efeito e mão de apoio

Autor: @BBCONTTI (git config user.name); execução: OpenAI Codex.
Data: 06/11/2026, America/Sao_Paulo. Ambiente: Windows / Unity 6000.6.0f1.
Status: implementado e compilado; testes técnicos no Editor. Player/input físico e aprovação visual do autor pendentes.

## Solicitação
Segurar botão direito e arrastar para escolher o efeito: pequeno zoom, contato na branca, taco mudando de ângulo, indicador UI com bola/ponto vermelho/taco. Em seguida, corrigir a mão de apoio atravessada pelo taco inclinado, conforme captura do autor.

## Diagnóstico e fontes
AGENTS.md, memória INDEX/CURRENT_STATE, notas manuais de arquitetura/catálogo/cenas, direção/critérios de produção e skill santa-rita-qa consultados. Código atual confirmado nos quatro scripts abaixo e shader MaoFlutuante. O seletor e a física contínua já existiam. A mão girava em projeção, porém seu pivô continuava em um ponto fixo que não pertencia ao novo eixo do taco.
Graphify query falhou por executável ausente. Refresh por scripts/graphify-project.py também falhou: graphify unavailable. Validate falhou por fontes ausentes no índice, incluindo arquivos preexistentes. Não declarar índice atualizado.

## Plano executado
- [x] Reutilizar contato contínuo e limite radial de 0,75 da branca.
- [x] Capturar direito + arraste com sensibilidade em unidades do Canvas; impedir mira/carga simultâneas.
- [x] Mostrar UI contextual com bola, ponto de contato, taco e ângulos.
- [x] Aplicar foco suave de câmera sem sobrescrever zoom-base; restaurar ao soltar/suspender.
- [x] Orientar/deslocar taco em função do contato e preservar a física existente.
- [x] Inicializar sala antes de aplicar contato, corrigindo referência nula encontrada na abertura da fixture.
- [x] Corrigir apoio por interseção da haste real com plano fixo atrás da branca. Palma/polegar mantêm máscaras complementares e o mesmo pivô UV. Puxada desliza a haste sem arrastar a mão.
- [x] Evitar que a inclinação de efeito alto leve a traseira do taco para baixo do feltro.
- [x] Documentar resultados e limites, preservando alterações anteriores e sem salvar cenas/saves.

## Arquivos
- Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs: roteamento/bloqueio de entradas, sincronização e ciclo de suspensão.
- Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.Spin.cs (+ meta Unity): novo controlador parcial de efeito e UI.
- Projeto Jogo 2D/Assets/Scripts/SinucaSpinSelector.cs: representação UI do taco.
- Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.cs: contato, ângulo, foco e apoio alinhado ao eixo real.
- Este plano e acréscimos em memória/notas manuais.

## Validação
Unity compilado sem erros após importação explícita. Falhas transitórias de importação dos scripts e referência nula da primeira abertura foram corrigidas antes da verificação final.
Harness Tools/QA/verify-sinuca-mechanics.py: 136 PASS (física numérica, sem input/render Player).
Fixture no Unity: entrada de ajuste, alteração do ângulo de taco em 9,84 graus, mira preservada, esquerdo bloqueado, soltura preservando contato, saída do ajuste, limite radial e suspensão: PASS. Foco deslocou câmera 0,31 m na fixture.
Após correção do apoio: 7 contatos (centro, laterais, alto/baixo e diagonais) x 2 cargas x palma/polegar = 28 verificações; erro máximo de alinhamento projetado 0,000061 pixels; deslocamento máximo do apoio ao puxar 0,00000012 m.
Captura UI anterior à correção do apoio: Projeto Jogo 2D/Captures/SpinEffect/spin-ui.png. Mostra a UI, não comprova a correção posterior. Captura bridge-corrected.png pegou estado de exploração; não serve como prova do apoio. Validação visual posterior no ângulo do autor continua pendente. Fixtures invocaram métodos via reflexão, não equivalem a teste físico de mouse nem campanha.

## Pendências
Revisão visual do autor nos extremos do efeito e na puxada. Sem nova build Player nesta tarefa. Graphify permanece desatualizado/ferramenta ausente. Não houve gravação de cenas nem uso de slots pessoais.
