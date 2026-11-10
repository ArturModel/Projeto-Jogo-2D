# Praça da Matriz — fontes de arte

- Praca-da-Matriz-ImageGen.png — folha original de seis sprites, com recortes individuais e escala final de 32 px/u. Prompt: mobiliário cívico brasileiro de 2004 em pixel art, granito úmido, ferro escuro, folhagem nativa, frontal oblíquo, com transparência e sem sombras no piso.
- Igreja-Matriz-ImageGen.png — fachada original da revisão. Prompt: igreja católica colonial brasileira, torre central, telhado cerâmico molhado, reboco creme, porta de madeira arqueada e vitrais âmbar. Silhueta isolada com fundo transparente.
- Pedregulho.png, LajeGranito.png e GuiaGranito.png — padrões produzidos para o projeto. Pedregulho e laje repetem em 128 px; a guia tem módulo de 32 px. A praça reaproveita as texturas de asfalto, grama e terra de Art/Macros, e a iluminação e vegetação da Rua Norte.

## Mureta superior

- Mureta-Matriz-ImageGen.png é a fonte do ImageGen integrado. O sprite de jogo é MuretaPedra.png, com recorte do conteúdo e exportação por vizinho mais próximo para 80×32. Módulo e pivô de base iguais aos do trecho anterior. A fonte PNG permite refazer a exportação.
- Prompt: gerar um módulo reto e repetível em pixel art para a praça de igreja brasileira, com duas fiadas alternadas de granito aparelhado cinza-bege, capeamento corrido de pedra, argamassa envelhecida e musgo discreto nas juntas. Usar a captura como referência de estilo e o módulo antigo como referência de geometria e câmera. Vista frontal levemente elevada, alpha transparente, sem chão, sombra desenhada, pilares ou adornos.
- Exportação: FFmpeg com o filtro `format=rgba,crop=1791:401:96:192,scale=80:32:flags=neighbor`. O importador usa Point, sem compressão, 32 PPU e pivô em (0,5; 0).
