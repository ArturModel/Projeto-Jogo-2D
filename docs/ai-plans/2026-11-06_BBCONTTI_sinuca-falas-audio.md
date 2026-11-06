# Sinuca: falas reativas de Zé, legenda e equilíbrio sonoro

Autor: @BBCONTTI (git config user.name); execução Codex. Data: 06/11/2026, America/Sao_Paulo.
Ambiente: Windows, C:/Temp/SantaRita, Unity 6000.6.0f1, branch melhoria-minigames.
Status: implementado e validado por compilação/testes C#; render, audição e Player pendentes.

## Solicitação

Baixar um pouco a música, destacar taco/bolas e fazer Zé reagir à partida (encaçapadas e proximidade da vitória), com onomatopeia sem palavras. Complemento do autor: legenda maior, centralizada na parte inferior, escrita aos poucos e depois desaparecendo.

## Contexto e diagnóstico

Consultados AGENTS.md, INDEX/CURRENT_STATE, visão geral, catálogo e mapa do cofre, direção/critérios de produção, skills locais santa-rita-audio/narrativa/qa, Bíblia R1 (Zé e conflito antes da mesa), textos reais de Notebook/Entries e implementação atual. Zé usa a partida para adiar sua resposta sobre Adílio; as falas não revelam a consequência ou informações novas. Rotina: dono do bar/adversário; vínculo: Marcus já conversa com ele antes do duelo; informação: somente estado público da mesa e combinado já existente; nenhuma mudança persistente por falar.

Graphify query indisponível nesta máquina. Fontes reais SinucaMinigameUI, SinuquinhaRules e Capitulo1Director confirmadas diretamente; índices não tratados como implementação. MCP respondeu no_unity_session; não houve alteração de cenas ou tentativa de salvar trabalho aberto. Alterações locais preexistentes, incluindo mãos e apresentação da sinuca, preservadas.

## Executado

- [x] Volume musical da sinuca .31 → .23, mantendo fade e continuidade da faixa do autor.
- [x] Taco .18–.42 → .23–.54; bola .44 → .56; tabela .30 → .35; caçapa .34 → .40. Giz preservado; intensidade física, preferências de efeitos e AudioListener geral continuam aplicáveis.
- [x] Reações adjudicadas após a tacada, distinguindo Marcus/Zé, conversão válida/falta/marcada. Variações, silêncio entre defesas, duas bolas e preta final com marcos únicos por jogador/mão.
- [x] Legenda independente embaixo, centralizada, fonte 16 no canvas nativo 640×360; painel 520×48 a 64 unidades da borda inferior, acima do rodapé. Entrada por caracteres, pausa na pontuação, leitura mínima 2,2 s, fade final .3 s. Não intercepta cliques nem altera controles.
- [x] Quatro murmúrios originais sintetizados, 75 ms, fundamental 112–139 Hz, sem gravação/voz inteligível. Fonte dedicada com volume .20 e intervalo ≥95 ms; não interrompe impactos.
- [x] Fala/texto suspensos em pausa, perda de foco e painéis; retomada no mesmo caractere. Treino sem falas; fechamento/desativação/nova mão limpam legenda, fila e fonte. Fila limitada e prioridade para marcada/proximidade da vitória.
- [x] Testes, proveniência e memória registrados. Nenhuma alteração de flags, pistas, saves, física, cenas ou callbacks narrativos.

## Arquivos

- Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs: interface, som abstrato e seletor transitório SinucaZeComments (no mesmo arquivo).
- Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs: volume da trilha durante a sinuca.
- Tools/QA/verify-sinuca-dialogue.py: harness executável sobre métodos/modelo C# reais, com doubles de TMP/AudioSource/tempo; artefatos temporários fora de Assets.
- ASSET_SOURCES.csv; este plano; memória/cofre; evidências em Docs/Evidencias/SinucaMecanicas/*falas*.

## Validação e limites

Assembly-CSharp compilado: **0 erros, 17 avisos CS0649 preexistentes**. A falta transitória de SinucaZeComments registrada por outro trabalho durante esta edição foi resolvida com a inclusão do seletor; compilação final sem CS0246.

**27 PASS**: seleção e variação de reações, dois jogadores perto da vitória/preta, falta/marcada com precedência, reinício de mão, silêncio/encerramento, texto progressivo, som, pausa/modal/retomada, mute em tempo real, leitura/desaparecimento, prioridade/limpeza, treino e quatro sinais sonoros finitos/sem clipping/bordas suaves. O teste inicial detectou NaN na última amostra: sin(π) em float ligeiramente negativo elevado a 1,5; corrigido com limite inferior zero e reexecutado integralmente.

**46 PASS** na regressão numérica de mecânicas/áudio existente. git diff --check específico sem erros de whitespace. Harness não executa render TMP, saída de som nativa ou Player; posição/leitura e equilíbrio percebido ainda precisam de conferência em jogo. Sem alegação de aprovação artística/auditiva.

Refresh Graphify executado e falhou por CLI ausente; validate executado e falhou por hashes antigos (C# files changed since refresh). Logs preservados; índices não falsificados. Nenhum commit/push.

## Pendências

Conferir legenda nas resoluções do jogo e ouvir mix/murmúrio no Unity/Player quando houver sessão disponível. Recuperar CLI Graphify e atualizar os índices. Nenhuma alteração de progressão ou persistência necessária.
