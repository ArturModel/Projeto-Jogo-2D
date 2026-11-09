# Bairro das Casas

08/11/2026 · Santa Rita · Unity 6000.6.0f1

O pedido reaplica o padrão visual da Entrada, do Posto e do Mercadinho ao trecho acessado pela estrada de chão. O mapa do Obsidian e o código confirmam a Zona4_Bairro e a casa nº 40 de Adílio como os destinos.

- Exterior em Tilemap, com ruas de barro contínuas, sulcos, calçadas, postes, vegetação, raio e reflexos do personagem nas poças. Fachadas recebem oclusão de contorno suave.
- Sprites novos para as casas 14, 22, 32, 40 e 48 e para a van da casa 14. Gerados pelo ImageGen integrado com as fachadas originais, a casa da Encruzilhada e a picape do Mercadinho como referências. Exportação transparente em 64 PPU, filtro Point.
- Interior 3D feito em paralelo, com cozinha, sala, quarto, lanterna, objetos de investigação e animação da porta com névoa. A transição reaproveita a estrutura do posto. As pistas e o recibo usam os gatilhos existentes.
- Legenda inferior com tom próprio para Lurdes e Ramiro. Pensamentos de Marcus aparecem após fechar a inspeção. Documentos mantêm a leitura própria.

Autor BBCONTTI com Codex. Aplicado à branch melhoria-minigames.

A verificação incluiu compilação do C#, mensagens de importação DX11 e inspeção da rua, dos alvos da casa e da ida e volta pelo mesmo acesso em Play Mode, com reposicionamento por fixture. A tela magenta vinha de erros de compilação HLSL na amostragem do barro, corrigidos antes da verificação. Build independente e avaliação artística do autor pendentes.

Arquivos principais: Capitulo1Director, Posto3D e Posto3D.CasaDoAvo, SantaRitaRuaNorte e SantaRitaRuaNorte.Bairro, LuzVolumetrica e Nevoeiro, SantaRitaFalasEntrada e InteracaoCapitulo1. As alterações compartilhadas de Posto e Mercadinho entram como dependências da mesma reforma.

Refresh do Graphify indisponível: Graphify refresh failed (exit 1). Índice desatualizado: Graphify validate failed (exit 1).