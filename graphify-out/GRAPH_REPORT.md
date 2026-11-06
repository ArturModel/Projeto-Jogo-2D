# Graph Report - Projeto-Jogo-2D  (2026-11-06)

## Corpus Check
- 153 files · ~112,222 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2095 nodes · 6963 edges · 110 communities (54 shown, 56 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 308 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `ed02f7a3`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- unityengine
- SinucaRoom3D
- SinucaMinigameUI
- BochaMatch
- SantaRitaChapter1Builder
- Vector2
- Chuva
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- Image
- InteracaoCapitulo1
- DicaDeTecla
- VideoSettingsController
- SpriteRenderer
- BochaMinigameUI
- Sprite
- PrologoDirector
- .Apply
- TilePiso
- PrologoEstradaGraphic
- Texture2D
- Kind
- ObjetoCarro
- BochaSimulation
- PointerEventData
- BochaMapGraphic
- Transform
- AudioSettingsController
- graphify-project.py
- PrologoJogavel
- MapaInterativoUI
- SinucaBrasileiraRules
- Capitulo2Director
- BochaShot
- SinucaTableView
- Light2D
- .Add
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- MenuAmbienteGraphic
- TileAleatorio
- Chapter2RuntimeVerification
- .EncloseCourt
- PrologoRuntimeVerification
- RectTransform
- Fase
- EstadoPartida
- .Dizer
- c2-workstations.lua
- SantaRitaNpcRoutine
- CanvasGroup
- c2-npc-work.lua
- marcus-flashlight.lua
- marcus-walk.lua
- pool-table.lua
- graphify-refresh.sh
- graphify-setup.sh
- graphify-validate.sh
- bocha-court-atlas.lua
- market-stock.lua
- Interactable
- .Sync
- .Tem
- MarcusGait
- .ConfigurarCenaAtual
- .AplicarEstadoDaZona
- DispositivoCarga
- CamadaDeLuz
- .Awake
- .Dizer
- MonoBehaviour
- ZonaId
- BochaCameraView
- .Paint
- bar-patrons.lua
- ItemDeCena
- Camada
- SantaRitaRoomCutaway
- MovelLoja
- GameMessageUI
- MenuAmbiente

## God Nodes (most connected - your core abstractions)
1. `SinucaMinigameUI` - 143 edges
2. `Chapter2RuntimeVerification` - 99 edges
3. `PlayerController` - 81 edges
4. `Capitulo1Director` - 78 edges
5. `SinucaRoom3D` - 68 edges
6. `InteracaoCapitulo1` - 64 edges
7. `BochaCourtView` - 60 edges
8. `BochaMinigameUI` - 59 edges
9. `Chuva` - 53 edges
10. `PrologoJogavel` - 52 edges

## Surprising Connections (you probably didn't know these)
- `SantaRitaChapter2Authoring` --references--> `Capitulo2Director`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Editor/SantaRitaChapter2Authoring.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/Capitulo2Director.cs
- `Capitulo1Director` --references--> `PayphoneCheckpoint`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs → Projeto Jogo 2D/Assets/Scripts/PayphoneCheckpoint.cs
- `BochaCourtView` --references--> `BochaShot`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaShot.cs
- `BochaCourtView` --references--> `BochaSimulation`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaSimulation.cs
- `BochaMinigameUI` --references--> `BochaCourtView`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaMinigameUI.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs

## Import Cycles
- None detected.

## Communities (110 total, 56 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.06
Nodes (5): MenuAmbienteAuthoring, MenuAmbienteTextureImporter, PrologoTextureImporter, SantaRitaArtImporter, SantaRitaWorldLighting

### Community 2 - "SinucaRoom3D"
Cohesion: 0.08
Nodes (9): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+1 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.11
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaMatch"
Cohesion: 0.16
Nodes (13): BochaPerspectiveVerification, Chapter2Verification, BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador (+5 more)

### Community 6 - "Vector2"
Cohesion: 0.06
Nodes (15): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaChapter2Authoring, PontoMapa, OlharPrimeiraPessoa, Deslocamento, Prop (+7 more)

### Community 8 - "Chuva"
Cohesion: 0.05
Nodes (14): Camada, Capitulo2Audio, Chuva, Gota, Poca, Respingo, PayphoneCheckpoint, IsSaveUIOpen (+6 more)

### Community 9 - "BochaCourtView"
Cohesion: 0.12
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.15
Nodes (4): MenuPausa, Aberto, ReduzirEfeitosIntensos, BolaMesa

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.19
Nodes (3): InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.11
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.19
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaBarPose, SantaRitaForeground

### Community 17 - "BochaMinigameUI"
Cohesion: 0.15
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.12
Nodes (6): Fileira, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarProp, Prompt

### Community 19 - "PrologoDirector"
Cohesion: 0.05
Nodes (12): PrologoJogavelVerification, AudioAmbiente, Instance, EstradaScroller, Curvatura, Velocidade, LimpadorParabrisa, Ativo (+4 more)

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 25 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 26 - "ObjetoCarro"
Cohesion: 0.19
Nodes (5): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo

### Community 27 - "BochaSimulation"
Cohesion: 0.15
Nodes (11): BochaBall, Centro, Velocidade, BochaContact, BochaContactType, Bochas, Madeira, Saibro (+3 more)

### Community 28 - "PointerEventData"
Cohesion: 0.12
Nodes (3): SantaRitaItemHover, SinucaSpinSelector, Contact

### Community 30 - "Transform"
Cohesion: 0.24
Nodes (5): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual

### Community 32 - "graphify-project.py"
Cohesion: 0.18
Nodes (7): command(), fingerprint(), main(), run(), sources(), validate(), preparar()

### Community 33 - "PrologoJogavel"
Cohesion: 0.09
Nodes (20): PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador, Finalizado (+12 more)

### Community 34 - "MapaInterativoUI"
Cohesion: 0.13
Nodes (3): MapaInterativoUI, Instance, IsShowing

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "Capitulo2Director"
Cohesion: 0.16
Nodes (7): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 37 - "BochaShot"
Cohesion: 0.16
Nodes (9): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa (+1 more)

### Community 39 - "SinucaTableView"
Cohesion: 0.06
Nodes (12): Sinuca3DVerification, CameraFollow, LimiteMax, LimiteMin, SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior (+4 more)

### Community 40 - "Light2D"
Cohesion: 0.09
Nodes (6): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale, SantaRitaRimLight, SantaRitaShallowStairs

### Community 43 - "SinuquinhaRules"
Cohesion: 0.10
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.10
Nodes (6): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada, SantaRitaExitThreshold, SantaRitaRoomLayout

### Community 45 - "CheckpointSystem"
Cohesion: 0.07
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 49 - ".EncloseCourt"
Cohesion: 0.16
Nodes (3): SantaRitaMcpSetup, SantaRitaProductionVerification, SinucaRef7Authoring

### Community 51 - "RectTransform"
Cohesion: 0.12
Nodes (4): SantaRitaInventory, SantaRitaItemAction, UIFactory, Font

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - ".Dizer"
Cohesion: 0.39
Nodes (3): Fala, Legenda, Instance

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 56 - "SantaRitaNpcRoutine"
Cohesion: 0.15
Nodes (5): SantaRitaBarHost, SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex, Working

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 84 - "Interactable"
Cohesion: 0.05
Nodes (12): Capitulo2Interactable, Prompt, DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt (+4 more)

### Community 85 - ".Sync"
Cohesion: 0.29
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 87 - "MarcusGait"
Cohesion: 0.50
Nodes (3): MarcusGait, Quadro, QuadroDetalhado

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 96 - "MonoBehaviour"
Cohesion: 0.09
Nodes (6): CamadaParallax, Capitulo2Perception, ChuvaRolando, DentroDaCasaDoAvo, MenuItemSelector, HorizontalStepperNavigation

### Community 97 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 105 - "Camada"
Cohesion: 0.67
Nodes (3): Camada, Ambiente, Leitura

### Community 108 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

### Community 109 - "GameMessageUI"
Cohesion: 0.17
Nodes (4): DialogoLinha, GameMessageUI, Instance, IsShowing

### Community 110 - "MenuAmbiente"
Cohesion: 0.07
Nodes (8): MainMenuActions, MenuAmbiente, FocoAtual, Pronto, Saindo, MenuAmbienteRuntimeQA, Resultado, Rodando

## Knowledge Gaps
- **285 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+280 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 464 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **56 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `IEnumerator`, `unityengine`, `.Marcar`, `TipoInteracao`, `MenuAmbiente`, `Chapter2RuntimeVerification`, `Sprite`, `Interactable`, `.Tem`, `Transform`, `.Dizer`?**
  _High betweenness centrality (0.105) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _285 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.055220564900031736 - nodes in this community are weakly interconnected._
- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `SinucaRoom3D`, `Vector2`, `Image`, `Chuva`, `Image`, `.AtualizarVisualTacoEMira`, `PointerEventData`, `.SimularFisicaMesa`, `SinucaTableView`, `SinuquinhaRules`, `Chapter2RuntimeVerification`, `EstadoPartida`, `.CancelarCargaAoSuspender`, `Interactable`, `.Sync`, `.Tem`, `DispositivoCarga`, `MonoBehaviour`, `.AtualizarVisualTacoEMira`, `MenuAmbiente`?**
  _High betweenness centrality (0.091) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.08469449485783424 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Transform` to `MonoBehaviour`, `unityengine`, `SinucaRoom3D`, `MapaInterativoUI`, `ZonaId`, `Image`, `Chuva`, `MovelLoja`, `Sprite`, `RectTransform`, `Interactable`, `.Tem`, `TilePiso`, `Texture2D`, `.AplicarEstadoDaZona`, `.ConfigurarCenaAtual`, `.Dizer`?**
  _High betweenness centrality (0.066) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.10826210826210826 - nodes in this community are weakly interconnected._