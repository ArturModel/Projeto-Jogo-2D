# Cenas, zonas e conexões — C1/C2

Atualizado em 26/10/2026. Nome do arquivo mantido para preservar links; o escopo agora inclui C2. [Geografia](../../../Projeto%20Jogo%202D/Assets/Scripts/SantaRitaGeography.cs).

C1: MainMenu → Prologo → EntradaDaCidade, com zonas Rodovia/Posto/Encruzilhada/Bairro/Rua Norte e interiores de conveniência, mercadinho, casa do avô e Bar do Zé gerenciados por Capitulo1Director. CasaDoAvo e BarDoZe também existem como cenas legadas; conferir uso real antes de editar ou excluir.

Progressão: breve abertura → carro próprio marrom (mapa/lanterna) → casa de Adílio/pistas → mercadinho e Bar do Zé → sinuca/ficha → Centro. Relações e revelações seguem Bíblia e direção atual; este índice não substitui condições no código.

C2: CentroSantaRita liga IgrejaMatriz, PrefeituraSantaRita, RodoviariaSantaRita, MercadinhoDeposito, OficinaRadio, CasaCostureira, SociedadeBeiraRio e CemiterioSantaRita; AbrigoCemiterio pertence ao circuito do cemitério. Cemitério não é somente um trecho do Centro.

Retornos C2 → C1 pelo mercadinho (depósito/serviço) e caminho norte do Bar do Zé. Preservar pontos de entrada/saída, flags e compatibilidade de saves anteriores. Escola continua futura.

Sociedade: salão e cancha na mesma cena, acesso por escada rasa sem porta/novo carregamento. O minigame Bocha tem câmera/apresentação própria, minimapa/vista geral e consequência imediata; não avança automaticamente para C3.

Interação E próxima de portas/objetos; transições bloqueadas durante fade. Saídas usam soleira/capacho. Carregar mostra slots; não deslocar para último save automaticamente ao abrir minigame.

## Cenas habilitadas em EditorBuildSettings
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Prologo.unity`
- `Assets/Scenes/Cidade/EntradaDaCidade.unity`
- `Assets/Scenes/Cidade/CasaDoAvo.unity`
- `Assets/Scenes/Cidade/BarDoZe.unity`
- `Assets/Scenes/GameplayScene.unity`
- `Assets/Scenes/Cidade/CentroSantaRita.unity`
- `Assets/Scenes/Cidade/IgrejaMatriz.unity`
- `Assets/Scenes/Cidade/PrefeituraSantaRita.unity`
- `Assets/Scenes/Cidade/RodoviariaSantaRita.unity`
- `Assets/Scenes/Cidade/MercadinhoDeposito.unity`
- `Assets/Scenes/Cidade/OficinaRadio.unity`
- `Assets/Scenes/Cidade/CasaCostureira.unity`
- `Assets/Scenes/Cidade/SociedadeBeiraRio.unity`
- `Assets/Scenes/Cidade/AbrigoCemiterio.unity`
- `Assets/Scenes/Cidade/CemiterioSantaRita.unity`

## Revisão R4 — itens e saídas

Carro entrega lanterna; mapa novo é retirado nos folhetos de orientação da Rodoviária C2. Diário continua disponível antes do mapa; saves antigos mantêm mapa adquirido. Recibo passa por examinar → PEGAR → inventário → ENTREGAR no mercadinho → testemunho. Interior4_BarDoZe usa a planta compartilhada SantaRitaBarLayout. Saídas C1/C2 exigem os pés no tapete; depósito mantém seu retorno ao mercadinho em EntradaDaCidade, sem forçar retorno ao Centro.

Ref7 (31/10/2026): Interior4_BarDoZe mantém a conexão e a soleira existentes; mobiliário, limites de circulação e paredes usam a planta compacta comum da sala 3D. O minigame não carrega outra cena nem muda o fluxo narrativo. Renderer/authoring são descritos em Visao_Geral_Arquitetura.

Correção posterior de identidade 2D: Interior4_BarDoZe usa os sprites originais e WorldLit; a sinuca reaproveita a mesma arte em perspectiva. O layout físico e a soleira continuam compartilhados; não carregar Baked/ na exploração.

Sinuquinha15 (31/10/2026): no mesmo interior, Zé oferece lisas/listradas ou 1–7/9–15 e a branca começa pronta. Melhor de três e consequência narrativa preservadas. Capacho na saída, sem tapete sob a mesa; nenhuma nova cena ou alteração de geografia.
