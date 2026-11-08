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


Atlas (02/11/2026): cenas/zonas aprovadas não foram reconstruídas. O levantamento registra 19 cartas (9 zonas C1 + 10 cenas C2), 38 acessos reais, inclusive o LimiarCentro de InteracaoCapitulo1. Frente e serviço do Mercadinho compartilham identidade, mantendo suas coordenadas locais e origem exterior de entrada. Sem Escola ou interiores futuros.


04/11/2026 — Anexos de fitas, sem mudança na topologia: SantaRitaTapePlacement usa RetratoFamiliaCasaAvo na cena C1, foto_igreja na Igreja e cordas na Sociedade; Mila entrega a fita da oficina ao concluir o reparo. São anexos runtime em pontos existentes, não novas salas/cenas. Escola e fita futura de Amaro permanecem adiadas. [Plano](../../ai-plans/2026-11-04_PatrickDotto_mila-inventario-caderno.md).

08/11/2026 — O exterior da Rua Norte é montado por SantaRitaRuaNorte no fluxo normal de EntradaDaCidade. O Tilemap, a luz e a névoa têm recursos próprios. A porta, o orelhão e as rotas pertencem ao Capitulo1Director. [Plano](../../ai-plans/2026-11-08_BBCONTTI_rua-norte-bar-ze.md).


A entrada atual do Bar do Zé é a porta da Rua Norte em `EntradaDaCidade`. A zona muda durante a cobertura da máscara, e `BarDoZe3D` reutiliza `SinucaRoom3D` para exploração e duelo. A porta de saída devolve Marcus à calçada. A cena legada `BarDoZe` continua disponível para cargas antigas e abre a mesma exploração em 3D.
