# Perfil global de áudio e legendas dos minigames

Autor do projeto @BBCONTTI · execução Codex  
07/11/2026 11:07 America/Sao_Paulo · Windows · Unity 6000.6.0f1  
Implementado, com verificação técnica no Editor e audição em partida pendente.

## Solicitação e fonte

Usar a mixagem e as legendas da sinuca na bocha e nos demais minigames. O ajuste do autor passa a representar 100% em cada controle de áudio do projeto. O print foi confirmado com uma leitura das seis preferências na sessão ativa do Unity. Os ganhos anteriores eram geral 1, música 0, fala 1, chuva 0.0661297441, taco e bolas 1 e outros efeitos 0.078125.

Foram consultados AGENTS.md, memória do projeto, notas manuais do Obsidian, direção de áudio, critérios de produção, matriz do capítulo 2 e skills santa-rita-audio e santa-rita-qa. A consulta ao Graphify falhou porque o executável está ausente. A implementação foi conferida no código, nas cenas e no Editor.

## Executado

- [x] Fazer os seis controles partirem de 100% no novo perfil. Persistir a posição relativa em chaves próprias e manter as chaves de áudio antigas disponíveis. Geral passa pelo listener. Música permanece muda conforme a referência.
- [x] Encaminhar fontes diretas e mixers para os mesmos ganhos, incluindo orelhão, passos, chuva, fala, ambiência e efeitos de interface.
- [x] Reusar na Sociedade Beira-Rio o clipe de chuva abafada, volume de cena e filtro do Bar do Zé. Igualar a redução de trilha nos minigames e a escala dos impactos equivalentes. Na bocha, lançamento, rolagem e colisões respondem a Taco e Bolas.
- [x] Extrair o estilo de legenda e o murmúrio da sinuca em MinigameDialogue. Zé, Tonão e Marcus usam a mesma apresentação, com tom do locutor, tamanho, fundo e ativação configuráveis. O rodapé da bocha mantém espaço para os comandos.

## Arquivos

- AudioSettingsController, AudioAmbiente, Capitulo1Director, Capitulo2Audio, Capitulo2Perception, MenuPausa, PayphoneCheckpoint, SantaRitaBarAudio e SantaRitaSound.
- MinigameDialogue, SinucaMinigameUI e seu parcial de percepção, BochaMinigameUI e seus parciais de Tonão e percepção.
- Cena SociedadeBeiraRio, expectativas das verificações de áudio do capítulo 2 e documentação de arquitetura/memória.

## Evidência de QA

- Compilação com referências reais do Unity e importação nativa no Editor. Zero erros no console depois da recompilação.
- Sessão Unity, MainMenu. Seis verificações de ganhos efetivos e seis de posição normalizada dos sliders. Chuva e Outros Efeitos conferidos contra os valores exatos do autor.
- Fixture temporária em Canvas real, sem tocar áudio nem modificar preferências. Legenda de Marcus usou VT323 em 16 pt, revelou o último caractere de 8 para 9, aplicou ganho de fala 0.20, alcançou alfa 0.8, ocultou ao pausar e limpou ao terminar. Fundo e ativação conferidos pela configuração atual. Um ensaio inicial sem Canvas falhou na geração da malha TMP, corrigido no ensaio com Canvas real.
- Auditoria de ligações. Parâmetros de chuva idênticos nas duas cenas, três entradas de diálogo usando o componente compartilhado e nenhuma leitura de ganhos legados no runtime. git diff --check passou.
- Graphify refresh executado sem sucesso porque o executável está ausente. Validate executado e apontou fontes atuais fora do índice antigo.

## Pendências

Audição humana da mixagem durante uma partida completa e conferência no Player.
