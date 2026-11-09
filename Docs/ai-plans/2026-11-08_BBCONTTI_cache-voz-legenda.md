# Voz da legenda após descarte do áudio

Autor do projeto @BBCONTTI · 08/11/2026 às 20:49 · America/Sao_Paulo · Unity 6000.6.0f1 · corrigido e verificado no Play Mode

A voz da legenda de Marcus havia sumido. O Editor mostrou o volume de fala e o volume geral em 1, com um AudioListener ativo. O cache estático continha quatro referências cujo clipe nativo já havia sido descartado. A montagem da Rodovia e o fluxo da fala estão descritos no mapa C1 do Obsidian e no plano da reforma.

- MinigameDialogue.cs passa a reconstruir cada sílaba descartada no primeiro uso. Preserva sequência, síntese, tom e mix compartilhados com a sinuca.
- O Unity compilou sem erros. O teste em Play Mode substituiu temporariamente uma entrada do cache por um clipe descartado. A legenda o recriou e disparou o AudioSource com PCM válido, pico 0,412, volume 0,20 e pitch 0,77.
- Graphify segue indisponível por falta do executável 0.9.96. O validador do índice anterior ainda aponta as lacunas documentadas na reforma.
