# Falas do posto no padrão da entrada

Autor @BBCONTTI (Git local), com Codex. 08/11/2026. Aplicado no Unity 6000.6.0f1 em EntradaDaCidade.

O autor confirmou que o pedido se limita ao posto, dentro e fora. O mapa de zonas do Obsidian e a memória do projeto identificam Zona2_Posto e Interior_Posto como partes de EntradaDaCidade. A consulta ao Graphify ficou indisponível por falta do executável.

- SantaRitaFalasEntrada.cs reaproveita MinigameDialogue para as falas do posto. O texto aparece na faixa inferior com o murmúrio sintético original. Os tons são .77 para Marcus, .96 para o frentista e .62 para o motorista.
- InteracaoCapitulo1.cs usa esse fluxo de fala no posto. Bomba, caderno e TV conservam o conteúdo escrito da inspeção. Os comentários de Marcus passam para a faixa inferior ao fechar a inspeção.
- Posto3D.cs encaminha E à legenda durante a conversa e suspende o deslocamento até a conclusão.

O Unity compilou com zero erros. A fixture usou as interações do jogo no exterior e no interior e confirmou a faixa inferior, a ausência da caixa de diálogo e o pitch aplicado ao AudioSource para cada locutor. A audição humana fica a cargo do autor.

Refresh falhou porque o executável Graphify está indisponível. Validação falhou por fontes ausentes no índice.