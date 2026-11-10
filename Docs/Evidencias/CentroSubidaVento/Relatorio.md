# Centro — chegada, vento e sarjetas

- [Antes, chegada](Antes-Chegada.png) e [depois, chegada](Chegada.png).
- [Antes, rua](Antes-Rua.png), [rua oeste revisada](Rua-Oeste.png) e [rua leste revisada](Rua-Leste.png).
- [Vento ligado](Vento-Ligado.png) e [vento desligado no mesmo enquadramento](Vento-Desligado.png).

As capturas fornecidas pelo autor mostram a câmera com HUD. As capturas novas são da câmera de jogo, 1024 × 576, em Play no projeto principal. Marcus foi reposicionado temporariamente. A terra atravessa a abertura no passeio, deixa sulcos e cascalho sobre a entrada do asfalto e passa a aparecer na câmera da chegada. As poças fora da luz direta ficam visíveis pelo reflexo frio. As grelhas, tampas e pontas de arrimo ficam mais próximas da pista, com sobreposição parcial das copas.

A verificação passou no frame 1.054, com 1.205 árvores ativas ancoradas nos pés, os mapas de relevo da vegetação e 28 peças PBR. A abertura do passeio foi identificada como Terra no mapa visual e a amostragem física em 42 pontos não encontrou sólidos entre o ponto de chegada e o caminho. O console principal terminou sem erros. Uma revisão concorrente dos canteiros da praça esconde algumas árvores naquele setor em Play. Nenhuma posição de árvore do prefab mudou nesta revisão.

A sessão principal foi reiniciada para carregar a composição. O snapshot em RAM e a posição anterior de Marcus foram restaurados, sem usar slots de save. A tentativa inicial de cópia de teste foi interrompida a pedido do autor. A validação acima usou o projeto principal. O executável de distribuição ficou pendente.

