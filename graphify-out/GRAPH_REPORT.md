# Graph Report - Projeto-Jogo-2D  (2026-11-03)

## Corpus Check
- 153 files · ~106,736 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2029 nodes · 6848 edges · 120 communities (59 shown, 61 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 313 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `be433eaa`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- unityengine
- Capitulo1Director
- SinucaMinigameUI
- BochaSimulation
- SantaRitaChapter1Builder
- SantaRitaChapter2Authoring
- PayphoneCheckpoint
- PrologoDirector
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- RectTransform
- InteracaoCapitulo1
- .Marcar
- VideoSettingsController
- SpriteRenderer
- BochaMinigameUI
- SpawnPoint
- GameMessageUI
- Material
- TilePiso
- Image
- MarcusNotebookUI
- Chuva
- SantaRitaBarLayout
- Capitulo2Portal
- AudioSource
- .Tem
- graphify-project.py
- ObjetoCarro
- MapaInterativoUI
- SinucaBrasileiraRules
- SinucaTableView
- BochaShot
- SinucaRoom3D
- Vector2
- SinucaSpinSelector
- .Dizer
- HorizontalStepperNavigation
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- PointerEventData
- TileAleatorio
- Chapter2RuntimeVerification
- Capitulo2Portal
- MapaInterativoUI
- Fase
- EstadoPartida
- SinucaCameraView
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
- Interactable
- SceneFader
- C2InteractionData
- .Apply
- Legenda
- DispositivoCarga
- CameraFollow
- MarcusGait
- MainMenuActions
- PrologoVideo
- TransicaoDeZonaTrigger
- BochaCameraView
- SantaRitaFootsteps
- LimpadorParabrisa
- bar-patrons.lua
- DoorTrigger
- ZonaId
- Kind
- MenuItemSelector
- MovelLoja
- CamadaParallax
- MonoBehaviour
- Capitulo2Director
- SantaRitaBarAudio
- SantaRitaRoomCutaway
- SantaRitaExitThreshold
- .Play
- SinucaCueSpin

## God Nodes (most connected - your core abstractions)
1. `SinucaMinigameUI` - 143 edges
2. `Chapter2RuntimeVerification` - 105 edges
3. `PlayerController` - 83 edges
4. `Capitulo1Director` - 78 edges
5. `SinucaRoom3D` - 68 edges
6. `InteracaoCapitulo1` - 65 edges
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

## Communities (120 total, 61 thin omitted)

### Community 2 - "Capitulo1Director"
Cohesion: 0.14
Nodes (5): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.08
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaSimulation"
Cohesion: 0.16
Nodes (13): BochaPerspectiveVerification, Chapter2Verification, BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador (+5 more)

### Community 7 - "PayphoneCheckpoint"
Cohesion: 0.22
Nodes (4): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt

### Community 8 - "PrologoDirector"
Cohesion: 0.16
Nodes (4): AudioAmbiente, Instance, FalaMarcada, PrologoDirector

### Community 9 - "BochaCourtView"
Cohesion: 0.13
Nodes (10): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+2 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "RectTransform"
Cohesion: 0.15
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.18
Nodes (4): DialogoLinha, InteracaoCapitulo1, Prompt, Tipo

### Community 14 - ".Marcar"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.11
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.17
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.15
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "SpawnPoint"
Cohesion: 0.29
Nodes (3): SpawnPoint, DirecaoInicial, Id

### Community 19 - "GameMessageUI"
Cohesion: 0.15
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 20 - "Material"
Cohesion: 0.10
Nodes (6): SantaRitaRuralPaths, Stretch, SinucaRef7Authoring, SantaRitaAtmosphere, SantaRitaRimLight, SantaRitaWorldLighting

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 23 - "Image"
Cohesion: 0.13
Nodes (3): SantaRitaInventory, UIFactory, Font

### Community 24 - "MarcusNotebookUI"
Cohesion: 0.17
Nodes (6): MarcusNotebookUI, HasPendingNotice, IsOpen, PageCount, Spread, UpdateCount

### Community 25 - "Chuva"
Cohesion: 0.06
Nodes (14): CamadaDeLuz, Nome, Camada, Chuva, Gota, Poca, Respingo, EstradaScroller (+6 more)

### Community 26 - "SantaRitaBarLayout"
Cohesion: 0.11
Nodes (11): SantaRitaBarHost, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt, SantaRitaNpcWork (+3 more)

### Community 27 - "Capitulo2Portal"
Cohesion: 0.14
Nodes (11): BochaBall, Centro, Velocidade, BochaContact, BochaContactType, Bochas, Madeira, Saibro (+3 more)

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.12
Nodes (8): CelularCarro, Disponivel, ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "MapaInterativoUI"
Cohesion: 0.07
Nodes (10): SantaRitaAtlasAuthoring, AtlasAccess, AtlasArrival, AtlasLandmark, AtlasRegion, AtlasSurvey, SantaRitaAtlas, Survey (+2 more)

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "SinucaTableView"
Cohesion: 0.12
Nodes (4): SinucaTableView, CameraModo, Room, Zoom

### Community 37 - "BochaShot"
Cohesion: 0.18
Nodes (9): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa (+1 more)

### Community 38 - "SinucaRoom3D"
Cohesion: 0.10
Nodes (9): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+1 more)

### Community 39 - "Vector2"
Cohesion: 0.07
Nodes (9): OlharPrimeiraPessoa, Deslocamento, Prop, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition, BolaMesa, SinucaAimGuideData (+1 more)

### Community 40 - "SinucaSpinSelector"
Cohesion: 0.19
Nodes (4): BochaMapGraphic, Simulation, SinucaSpinSelector, Contact

### Community 43 - "SinuquinhaRules"
Cohesion: 0.11
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.16
Nodes (4): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada

### Community 45 - "CheckpointSystem"
Cohesion: 0.07
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 49 - "Capitulo2Portal"
Cohesion: 0.10
Nodes (9): Chapter2BuildVerification, Evidence, ForcarDevelopment, Capitulo2Gate, Capitulo2Portal, Prompt, SantaRitaActorElevation, VisualDepth (+1 more)

### Community 51 - "MapaInterativoUI"
Cohesion: 0.33
Nodes (3): MapaInterativoUI, Instance, IsShowing

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - "SinucaCameraView"
Cohesion: 0.18
Nodes (4): SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 83 - "Interactable"
Cohesion: 0.19
Nodes (5): Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance

### Community 84 - "SceneFader"
Cohesion: 0.08
Nodes (4): AudioSettingsController, SceneFader, Instance, IsTransitioning

### Community 85 - "C2InteractionData"
Cohesion: 0.67
Nodes (3): C2DialogueBranch, Disponivel, C2InteractionData

### Community 87 - ".Apply"
Cohesion: 0.13
Nodes (4): SantaRitaArtProduction, SantaRitaProductionVerification, Capitulo2Interactable, Prompt

### Community 89 - "Legenda"
Cohesion: 0.25
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 93 - "CameraFollow"
Cohesion: 0.35
Nodes (3): CameraFollow, LimiteMax, LimiteMin

### Community 94 - "MarcusGait"
Cohesion: 0.53
Nodes (3): MarcusGait, Quadro, QuadroDetalhado

### Community 97 - "TransicaoDeZonaTrigger"
Cohesion: 0.36
Nodes (3): InfoZona, TransicaoDeZonaTrigger, Prompt

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

### Community 112 - "MonoBehaviour"
Cohesion: 0.13
Nodes (7): Capitulo2Perception, ChegadaEmSantaRita, ChuvaRolando, DentroDaCasaDoAvo, PayphoneSlotInputWatcher, SantaRitaItemAction, SantaRitaShallowStairs

### Community 113 - "Capitulo2Director"
Cohesion: 0.33
Nodes (4): C2ContentData, Capitulo2Director, Instance, ProntoParaBocha

### Community 118 - "SinucaCueSpin"
Cohesion: 0.33
Nodes (4): SinucaCueSpin, Avancar, Normal, Recuar

## Knowledge Gaps
- **265 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+260 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 446 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **61 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `RectTransform`, `GameMessageUI`, `.CancelarCargaAtiva`, `Image`, `Chuva`, `AudioSource`, `Button`, `SinucaTableView`, `Vector2`, `SinucaSpinSelector`, `SinuquinhaRules`, `CheckpointSystem`, `EstadoPartida`, `SinucaCameraView`, `.Run`, `SceneFader`, `.SimularFisicaMesa`, `DispositivoCarga`, `MonoBehaviour`, `SinucaCueSpin`?**
  _High betweenness centrality (0.116) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _265 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.06082289803220036 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `SantaRitaChapter1Builder`, `PayphoneCheckpoint`, `SpawnPoint`, `TilePiso`, `Image`, `SantaRitaBarLayout`, `AudioSource`, `Transform`, `Button`, `.Tem`, `Capitulo2Portal`, `SceneFader`, `.RunAtlas`, `TransicaoDeZonaTrigger`, `SantaRitaFootsteps`, `ZonaId`, `MovelLoja`, `BoxCollider2D`, `.TeleportarPara`, `MonoBehaviour`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.13768115942028986 - nodes in this community are weakly interconnected._
- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `IEnumerator`, `unityengine`, `MapaInterativoUI`, `TipoInteracao`, `CheckpointSystem`, `BoxCollider2D`, `.Marcar`, `GameMessageUI`, `Interactable`, `SantaRitaBarLayout`, `Transform`, `Button`, `.Tem`?**
  _High betweenness centrality (0.054) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.0821256038647343 - nodes in this community are weakly interconnected._