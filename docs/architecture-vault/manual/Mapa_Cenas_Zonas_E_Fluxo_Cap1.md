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

06/11 — Prologo passa a viagem jogável. Manobra/parada no recuo pavimentado da Rodovia de Entrada antecede a saída em `saida_carro`. Placa, banco e orelhão são marcos de continuidade, igreja apenas paisagem distante. Fluxo/cenas e coordenadas C1 preservados.

Decisão posterior do autor (06/11): remover do prólogo a imagem de placa/banco/orelhão, que parecia uma plataforma na estrada. Somente a estrada e um recuo lateral discreto permanecem visíveis. O carro encosta, desacelera e para antes da transição; os marcos são vistos apenas na próxima cena. A imagem descartada foi arquivada fora de Assets, em Arte/Producao/PrologoJogavel/Descartados. Testes visuais devem priorizar Play, sem nova build por este ajuste.

06/11 — Prólogo rural: PrologoJogavel fornece CentroVia/TerraEm por distância, duas curvas, recuo de terra e parada 44 s/transição 51 s. PrologoEstradaGraphic projeta atlas de cenário e três superfícies do mesmo mundo; UV1 distingue terra e solo vegetal. AudioAmbiente.ConfigurarLoop reaproveita a fonte do motor e adiciona pneusTerra ao mixer comum; alvo/pitch acompanham velocidade/superfície. Próxima cena/saida_carro e saves manuais preservados. Plano: docs/ai-plans/2026-11-06_ismaelweinertb_prologo-rural.md; evidências: Docs/Evidencias/PrologoRural/Relatorio.md.
