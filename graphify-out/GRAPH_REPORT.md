# Graph Report - Projeto-Jogo-2D  (2026-11-04)

## Corpus Check
- 162 files · ~109,822 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2134 nodes · 7195 edges · 115 communities (53 shown, 62 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 309 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `064d6182`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- unityengine
- Capitulo1Director
- SinucaMinigameUI
- BochaSimulation
- SantaRitaChapter1Builder
- .Scenes
- Legenda
- PrologoDirector
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- RectTransform
- .Marcar
- .Marcar
- VideoSettingsController
- SpriteRenderer
- BochaMinigameUI
- SantaRitaInventory
- PayphoneCheckpoint
- Material
- TilePiso
- MarcusNotebookUI
- Chuva
- SantaRitaBarLayout
- Capitulo2Portal
- AudioSource
- graphify-project.py
- ObjetoCarro
- Vector2
- SinucaBrasileiraRules
- SinucaTableView
- BochaShot
- SinucaRoom3D
- .Sync
- SinucaSpinSelector
- .Dizer
- SinucaSpinSelector
- SinuquinhaRules
- PlayerController
- .Run
- PointerEventData
- TileAleatorio
- Chapter2RuntimeVerification
- MonoBehaviour
- .Capture
- Fase
- EstadoPartida
- SantaRitaBarAudio
- c2-workstations.lua
- .Run
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
- SceneFader
- AudioSettingsController
- CheckpointSystem
- .Apply
- DispositivoCarga
- MapaInterativoUI
- ChuvaRolando
- SantaRitaNpcRoutine
- PrologoVideo
- BochaCameraView
- CamadaDeLuz
- LimpadorParabrisa
- bar-patrons.lua
- ZonaId
- Kind
- MovelLoja
- CamadaParallax
- .Executar
- EstradaScroller
- .Play
- AudioAmbiente
- SantaRitaMcpSetup
- fitas.lua

## God Nodes (most connected - your core abstractions)
1. `SinucaMinigameUI` - 143 edges
2. `Chapter2RuntimeVerification` - 105 edges
3. `PlayerController` - 83 edges
4. `Capitulo1Director` - 78 edges
5. `SinucaRoom3D` - 68 edges
6. `InteracaoCapitulo1` - 66 edges
7. `BochaCourtView` - 60 edges
8. `BochaMinigameUI` - 59 edges
9. `Chuva` - 53 edges
10. `SinucaTableView` - 51 edges

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

## Communities (115 total, 62 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.06
Nodes (7): SantaRitaArtImporter, Flags, MarcusNotebookCatalog, MarcusNotebookContent, MarcusNotebookEntry, SantaRitaItemAction, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.10
Nodes (6): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.09
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaSimulation"
Cohesion: 0.20
Nodes (11): BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor, BochaSide (+3 more)

### Community 6 - ".Scenes"
Cohesion: 0.17
Nodes (5): Chapter2BuildVerification, Evidence, ForcarDevelopment, MilaProductionVerification, SantaRitaProductionVerification

### Community 7 - "Legenda"
Cohesion: 0.22
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 9 - "BochaCourtView"
Cohesion: 0.12
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "RectTransform"
Cohesion: 0.16
Nodes (4): MenuPausa, Aberto, ReduzirEfeitosIntensos, BolaMesa

### Community 13 - ".Marcar"
Cohesion: 0.06
Nodes (15): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha, CelularCarro (+7 more)

### Community 14 - ".Marcar"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.09
Nodes (6): HorizontalStepperNavigation, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 17 - "BochaMinigameUI"
Cohesion: 0.17
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "SantaRitaInventory"
Cohesion: 0.06
Nodes (15): MilaRuntimeVerification, SantaRitaInventory, ClosedFrame, Instance, IsOpen, SantaRitaTapeDeck, ClosedFrame, Instance (+7 more)

### Community 19 - "PayphoneCheckpoint"
Cohesion: 0.07
Nodes (9): GameMessageUI, Instance, IsShowing, MainMenuActions, PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt (+1 more)

### Community 20 - "Material"
Cohesion: 0.12
Nodes (5): SantaRitaRuralPaths, Stretch, SinucaRef7Authoring, SantaRitaAtmosphere, SantaRitaRimLight

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "MarcusNotebookUI"
Cohesion: 0.11
Nodes (9): MarcusNotebookUI, HasPendingNotice, IsOpen, MarkedEntry, PageCount, Spread, UpdateCount, UIFactory (+1 more)

### Community 25 - "Chuva"
Cohesion: 0.13
Nodes (5): Camada, Chuva, Gota, Poca, Respingo

### Community 26 - "SantaRitaBarLayout"
Cohesion: 0.14
Nodes (8): Fileira, SantaRitaBarHost, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt

### Community 27 - "Capitulo2Portal"
Cohesion: 0.15
Nodes (11): BochaBall, Centro, Velocidade, BochaContact, BochaContactType, Bochas, Madeira, Saibro (+3 more)

### Community 28 - "AudioSource"
Cohesion: 0.17
Nodes (3): Camada, Capitulo2Audio, MenuItemSelector

### Community 32 - "graphify-project.py"
Cohesion: 0.11
Nodes (10): command(), fingerprint(), main(), run(), sources(), validate(), fetch(), main() (+2 more)

### Community 33 - "ObjetoCarro"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "Vector2"
Cohesion: 0.07
Nodes (10): SantaRitaChapter2Authoring, CameraFollow, LimiteMax, LimiteMin, OlharPrimeiraPessoa, Deslocamento, Prop, SantaRitaGeography (+2 more)

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "SinucaTableView"
Cohesion: 0.11
Nodes (4): SinucaTableView, CameraModo, Room, Zoom

### Community 37 - "BochaShot"
Cohesion: 0.18
Nodes (9): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa (+1 more)

### Community 38 - "SinucaRoom3D"
Cohesion: 0.08
Nodes (13): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+5 more)

### Community 39 - ".Sync"
Cohesion: 0.27
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 43 - "SinuquinhaRules"
Cohesion: 0.09
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.11
Nodes (5): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada, SantaRitaExitThreshold

### Community 45 - ".Run"
Cohesion: 0.21
Nodes (5): BochaPerspectiveVerification, Chapter2Verification, MarcusGait, Quadro, QuadroDetalhado

### Community 49 - "MonoBehaviour"
Cohesion: 0.10
Nodes (7): Capitulo2Gate, Capitulo2Portal, Prompt, SantaRitaActorElevation, VisualDepth, VisualScale, SantaRitaShallowStairs

### Community 51 - ".Capture"
Cohesion: 0.08
Nodes (12): SantaRitaAtlasAuthoring, AtlasAccess, AtlasArrival, AtlasLandmark, AtlasRegion, AtlasSurvey, SantaRitaAtlas, Survey (+4 more)

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 84 - "SceneFader"
Cohesion: 0.06
Nodes (15): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance, SantaRitaTapePickup (+7 more)

### Community 86 - "CheckpointSystem"
Cohesion: 0.07
Nodes (24): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+16 more)

### Community 87 - ".Apply"
Cohesion: 0.13
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, SantaRitaFootsteps

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 92 - "MapaInterativoUI"
Cohesion: 0.18
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 94 - "SantaRitaNpcRoutine"
Cohesion: 0.10
Nodes (10): ChegadaEmSantaRita, DentroDaCasaDoAvo, SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex, Working, Station, SpawnPoint (+2 more)

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 105 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 106 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 108 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

### Community 116 - "EstradaScroller"
Cohesion: 0.33
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 123 - "fitas.lua"
Cohesion: 0.83
Nodes (3): col(), rect(), save()

## Knowledge Gaps
- **276 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+271 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 468 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **62 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `InteracaoCapitulo1` connect `.Marcar` to `IEnumerator`, `unityengine`, `TipoInteracao`, `GameObject`, `.Capture`, `PayphoneCheckpoint`, `SceneFader`, `CheckpointSystem`, `.UpdateCasaDoAvoScene`, `SantaRitaBarLayout`, `Transform`, `Button`?**
  _High betweenness centrality (0.107) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _276 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.05521783181357649 - nodes in this community are weakly interconnected._
- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `BochaCourtView`, `RectTransform`, `.Marcar`, `SantaRitaInventory`, `PayphoneCheckpoint`, `.CancelarCargaAtiva`, `Color`, `MarcusNotebookUI`, `Chuva`, `AudioSource`, `Button`, `Vector2`, `SinucaTableView`, `SinucaRoom3D`, `.Sync`, `SinucaSpinSelector`, `SinuquinhaRules`, `Chapter2RuntimeVerification`, `.EncontrarPosicaoLivreDeterministica`, `EstadoPartida`, `.Run`, `.OnMesaPointerDown`, `DispositivoCarga`, `SantaRitaNpcRoutine`, `GameObject`, `.CancelarCargaAoSuspender`?**
  _High betweenness centrality (0.086) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.10084033613445378 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `SantaRitaChapter1Builder`, `ZonaId`, `MovelLoja`, `.Marcar`, `GameObject`, `MonoBehaviour`, `SantaRitaInventory`, `PayphoneCheckpoint`, `SceneFader`, `TilePiso`, `MarcusNotebookUI`, `.UpdateCasaDoAvoScene`, `.Stretch`, `SantaRitaBarLayout`, `AudioSource`, `Transform`, `SantaRitaNpcRoutine`?**
  _High betweenness centrality (0.081) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.09206349206349207 - nodes in this community are weakly interconnected._