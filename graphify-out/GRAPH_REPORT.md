# Graph Report - SantaRitaPR6Merge  (2026-11-06)

## Corpus Check
- 173 files · ~119,077 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 11 file(s) not represented in the graph (top: (none) 5, .toml 1, .csv 1)

## Summary
- 2325 nodes · 7844 edges · 115 communities (60 shown, 55 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 317 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `429157c1`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- unityengine
- SinucaRoom3D
- SinucaMinigameUI
- BochaMatch
- Texture2D
- Vector2
- RectTransform
- Chuva
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- Image
- .Marcar
- DicaDeTecla
- List
- SpriteRenderer
- BochaMinigameUI
- Sprite
- PrologoDirector
- Material
- TilePiso
- Color
- Transform
- Kind
- ObjetoCarro
- BochaSimulation
- SinucaSpinSelector
- SantaRitaTapeDeck
- GameMessageUI
- AudioSettingsController
- graphify-project.py
- PrologoJogavel
- MapaInterativoUI
- SinucaBrasileiraRules
- MilaRuntimeVerification
- BochaShot
- SantaRitaInventory
- SinucaTableView
- SantaRitaActorElevation
- .Add
- .Roteiro
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- MenuAmbienteGraphic
- TileAleatorio
- .RunNotebook
- MarcusNotebookUI
- Fase
- EstadoPartida
- HorizontalStepperNavigation
- c2-workstations.lua
- PayphoneCheckpoint
- MonoBehaviour
- c2-npc-work.lua
- marcus-flashlight.lua
- marcus-walk.lua
- pool-table.lua
- graphify-refresh.sh
- graphify-setup.sh
- graphify-validate.sh
- bocha-court-atlas.lua
- market-stock.lua
- PayphoneRegistroUI
- SceneFader
- SinucaCueSpin
- fitas.lua
- MarcusGait
- .Paint
- BochaAI
- DispositivoCarga
- CamadaDeLuz
- .Awake
- AudioSource
- MenuItemSelector
- BochaCameraView
- BochaBall
- bar-patrons.lua
- SantaRitaRoomCutaway
- Interactable
- MenuAmbiente
- Light2D
- .Run
- SubtitleSettingsController
- SantaRitaBarAudio
- Scene
- CamadaParallax
- .Executar

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

## Communities (115 total, 55 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.05
Nodes (6): MenuAmbienteAuthoring, MenuAmbienteTextureImporter, PrologoTextureImporter, SantaRitaArtImporter, SantaRitaMcpSetup, SantaRitaWorldLighting

### Community 2 - "SinucaRoom3D"
Cohesion: 0.10
Nodes (9): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+1 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.10
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaMatch"
Cohesion: 0.20
Nodes (11): BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor, BochaSide (+3 more)

### Community 6 - "Vector2"
Cohesion: 0.05
Nodes (19): Chapter2BuildVerification, Evidence, ForcarDevelopment, MilaProductionVerification, SantaRitaAtlasAuthoring, SantaRitaChapter2Authoring, OlharPrimeiraPessoa, Deslocamento (+11 more)

### Community 8 - "Chuva"
Cohesion: 0.10
Nodes (5): Camada, Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.09
Nodes (10): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+2 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.13
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - ".Marcar"
Cohesion: 0.06
Nodes (18): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha, CelularCarro (+10 more)

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "List"
Cohesion: 0.10
Nodes (4): Fala, Legenda, Instance, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.17
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.15
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.10
Nodes (12): Fileira, SantaRitaBarHost, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt (+4 more)

### Community 19 - "PrologoDirector"
Cohesion: 0.05
Nodes (12): PrologoJogavelVerification, AudioAmbiente, Instance, EstradaScroller, Curvatura, Velocidade, LimpadorParabrisa, Ativo (+4 more)

### Community 20 - "Material"
Cohesion: 0.09
Nodes (5): SantaRitaArtProduction, SantaRitaProductionVerification, SinucaRef7Authoring, SantaRitaAtmosphere, SantaRitaFootsteps

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "Transform"
Cohesion: 0.06
Nodes (27): CameraFollow, LimiteMax, LimiteMin, Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual (+19 more)

### Community 25 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 26 - "ObjetoCarro"
Cohesion: 0.19
Nodes (5): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo

### Community 27 - "BochaSimulation"
Cohesion: 0.17
Nodes (8): BochaContact, BochaContactType, Bochas, Madeira, Saibro, BochaSimulation, IsMoving, Evento

### Community 29 - "SantaRitaTapeDeck"
Cohesion: 0.14
Nodes (10): SantaRitaTapeDeck, ClosedFrame, Instance, IsOpen, Playing, Position, Records, TapeCatalog (+2 more)

### Community 30 - "GameMessageUI"
Cohesion: 0.16
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 32 - "graphify-project.py"
Cohesion: 0.10
Nodes (11): command(), fingerprint(), main(), run(), sources(), validate(), preparar(), fetch() (+3 more)

### Community 33 - "PrologoJogavel"
Cohesion: 0.09
Nodes (20): PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador, Finalizado (+12 more)

### Community 34 - "MapaInterativoUI"
Cohesion: 0.08
Nodes (9): MapaInterativoUI, Instance, IsShowing, PontoMapa, AtlasRegion, SantaRitaAtlas, Survey, SantaRitaAtlasView (+1 more)

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 37 - "BochaShot"
Cohesion: 0.31
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 38 - "SantaRitaInventory"
Cohesion: 0.17
Nodes (4): SantaRitaInventory, ClosedFrame, Instance, IsOpen

### Community 39 - "SinucaTableView"
Cohesion: 0.07
Nodes (8): SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior, SinucaTableView, CameraModo, Room, Zoom

### Community 40 - "SantaRitaActorElevation"
Cohesion: 0.22
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 42 - ".Roteiro"
Cohesion: 0.23
Nodes (3): MenuAmbienteRuntimeQA, Resultado, Rodando

### Community 43 - "SinuquinhaRules"
Cohesion: 0.11
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.11
Nodes (5): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada, SantaRitaExitThreshold

### Community 45 - "CheckpointSystem"
Cohesion: 0.07
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 46 - "MenuAmbienteGraphic"
Cohesion: 0.22
Nodes (6): BochaMapGraphic, Simulation, Camada, Ambiente, Leitura, MenuAmbienteGraphic

### Community 51 - "MarcusNotebookUI"
Cohesion: 0.09
Nodes (12): MarcusNotebookCatalog, MarcusNotebookContent, MarcusNotebookEntry, MarcusNotebookUI, HasPendingNotice, IsOpen, MarkedEntry, PageCount (+4 more)

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 56 - "PayphoneCheckpoint"
Cohesion: 0.14
Nodes (5): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher

### Community 57 - "MonoBehaviour"
Cohesion: 0.12
Nodes (6): ChegadaEmSantaRita, DentroDaCasaDoAvo, InteractionPrompt, Instance, MenuPanelTransition, OptionsPanelController

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 83 - "PayphoneRegistroUI"
Cohesion: 0.15
Nodes (5): PayphoneRegistroUI, Ocupado, RegistroSelecionado, SomenteCarregar, TemArte

### Community 84 - "SceneFader"
Cohesion: 0.18
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 85 - "SinucaCueSpin"
Cohesion: 0.29
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 86 - "fitas.lua"
Cohesion: 0.83
Nodes (3): col(), rect(), save()

### Community 87 - "MarcusGait"
Cohesion: 0.50
Nodes (3): MarcusGait, Quadro, QuadroDetalhado

### Community 89 - "BochaAI"
Cohesion: 0.67
Nodes (3): BochaAI, UltimaDecisaoMs, UltimosCandidatos

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 95 - "AudioSource"
Cohesion: 0.20
Nodes (4): Camada, Capitulo2Audio, SantaRitaSound, EventsPlayed

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 99 - "BochaBall"
Cohesion: 0.21
Nodes (5): BochaPerspectiveVerification, Chapter2Verification, BochaBall, Centro, Velocidade

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 109 - "Interactable"
Cohesion: 0.13
Nodes (7): Capitulo2Interactable, Prompt, Capitulo2Portal, Prompt, Interactable, InteractionRadius, Prompt

### Community 110 - "MenuAmbiente"
Cohesion: 0.11
Nodes (5): MainMenuActions, MenuAmbiente, FocoAtual, Pronto, Saindo

### Community 115 - "SubtitleSettingsController"
Cohesion: 0.29
Nodes (4): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho

### Community 117 - "Scene"
Cohesion: 0.22
Nodes (3): SantaRitaTapePickup, Prompt, SantaRitaTapePlacement

## Knowledge Gaps
- **307 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+302 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 508 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **55 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `IEnumerator`, `unityengine`, `Vector2`, `RectTransform`, `Image`, `List`, `.IniciarTurnoAtual`, `SinucaSpinSelector`, `GameMessageUI`, `SinucaTableView`, `SinuquinhaRules`, `CheckpointSystem`, `GameObject`, `.CancelarCargaAoSuspender`, `MarcusNotebookUI`, `EstadoPartida`, `MonoBehaviour`, `SinucaCueSpin`, `DispositivoCarga`, `AudioSource`, `.AtualizarVisualTacoEMira`, `RuntimeInitializeOnLoadMethod`, `.Run`?**
  _High betweenness centrality (0.086) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _307 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.12937853107344632 - nodes in this community are weakly interconnected._
- **Why does `InteracaoCapitulo1` connect `.Marcar` to `IEnumerator`, `unityengine`, `Texture2D`, `Vector2`, `TipoInteracao`, `CheckpointSystem`, `Interactable`, `GameObject`, `Sprite`, `Scene`, `Transform`, `GameMessageUI`?**
  _High betweenness centrality (0.065) - this node is a cross-community bridge._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.05342281879194631 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Transform` to `unityengine`, `Texture2D`, `Vector2`, `RectTransform`, `.Marcar`, `RuntimeInitializeOnLoadMethod`, `GameObject`, `.RunNotebook`, `MarcusNotebookUI`, `Sprite`, `Scene`, `TilePiso`, `PayphoneCheckpoint`, `MonoBehaviour`, `AudioSource`?**
  _High betweenness centrality (0.058) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.09797979797979799 - nodes in this community are weakly interconnected._