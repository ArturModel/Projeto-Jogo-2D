# Ajuste do volume da chuva

Autor do projeto @BBCONTTI · execução Codex  
07/11/2026 11:16 America/Sao_Paulo · Windows · Unity 6000.6.0f1  
Ajuste aplicado e reprodução verificada no Editor.

## Solicitação e diagnóstico

Subir levemente a chuva, que parecia ausente. As fontes de chuva do menu, dos ambientes de jogo e do bar usam AudioSettingsController.VolumeChuva. A referência de 6,61% deixava as duas camadas discretas, especialmente em interiores. Na primeira amostra da sessão as fontes estavam paradas. Em sessão ativa após a recompilação, ambas estavam tocando em loop, com o listener ativo e sem pausa. O motivo da primeira amostra não foi determinado.

## Execução

- [x] Subir o ganho de referência em AudioSettingsController.cs de 0.0661297441 para 0.10, cerca de 3,6 dB. A posição do controle continua multiplicando o ganho.
- [x] Conferir as ligações do menu, de Chuva e de SantaRitaBarAudio.
- [x] Atualizar memória e registrar a validação.

## Evidência

Compilação C# com referências do Unity terminou sem erros. Em Play Mode, EntradaDaCidade, o ganho retornado foi 0.10. As fontes SomChuvaA e SomChuvaB estavam ativas, sem mute, carregadas, em loop e tocando, com volumes instantâneos 0.03946 e 0.02368 e tempos de reprodução diferentes. A partida do autor não foi interrompida. Audição na bocha continua pendente.

Graphify query e refresh indisponíveis por falta do executável. Validate apontou scripts atuais ausentes no índice existente.
