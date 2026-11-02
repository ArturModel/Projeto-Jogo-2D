# Graph Report - Projeto-Jogo-2D  (2026-11-02)

## Corpus Check
- 144 files · ~105,190 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 1937 nodes · 6442 edges · 105 communities (53 shown, 52 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 308 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `ae407965`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- unityengine
- Capitulo1Director
- SinucaMinigameUI
- BochaSimulation
- SantaRitaChapter1Builder
- Vector2
- PayphoneCheckpoint
- Chuva
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- RectTransform
- InteracaoCapitulo1
- .Marcar
- VideoSettingsController
- SpriteRenderer
- BochaMinigameUI
- SpriteRenderer
- PrologoDirector
- Material
- TilePiso
- .Tem
- Kind
- SantaRitaBarLayout
- Capitulo2Portal
- MenuItemSelector
- SinucaSpinSelector
- GameObject
- AudioSettingsController
- graphify-project.py
- ObjetoCarro
- AudioSettingsController
- SinucaBrasileiraRules
- GameMessageUI
- BochaShot
- SinucaRoom3D
- SinucaTableView
- SantaRitaActorElevation
- .Dizer
- CamadaParallax
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- AudioSource
- TileAleatorio
- .Scenes
- PointerEventData
- .Stretch
- Fase
- EstadoPartida
- Legenda
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
- SinucaCueSpin
- .Apply
- PrologoVideo
- DispositivoCarga
- ItemDeCena
- .Run
- MainMenuActions
- Capitulo2Director
- BochaCameraView
- .Executar
- bar-patrons.lua
- ChuvaRolando

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

## Communities (105 total, 52 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.06
Nodes (4): SantaRitaArtImporter, SantaRitaMcpSetup, SantaRitaItemAction, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.07
Nodes (24): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona, MovelLoja, CaixaMercado (+16 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.08
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaSimulation"
Cohesion: 0.19
Nodes (11): BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor, BochaSide (+3 more)

### Community 6 - "Vector2"
Cohesion: 0.08
Nodes (9): SantaRitaChapter2Authoring, CameraFollow, LimiteMax, LimiteMin, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography, SantaRitaRoomLayout (+1 more)

### Community 7 - "PayphoneCheckpoint"
Cohesion: 0.18
Nodes (5): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher

### Community 8 - "Chuva"
Cohesion: 0.06
Nodes (10): CamadaDeLuz, Nome, Camada, Chuva, Gota, Poca, Respingo, SantaRitaRoomCutaway (+2 more)

### Community 9 - "BochaCourtView"
Cohesion: 0.14
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "RectTransform"
Cohesion: 0.16
Nodes (4): MenuPausa, Aberto, ReduzirEfeitosIntensos, BolaMesa

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.19
Nodes (3): InteracaoCapitulo1, Prompt, Tipo

### Community 14 - ".Marcar"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.09
Nodes (6): HorizontalStepperNavigation, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.20
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.15
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "SpriteRenderer"
Cohesion: 0.11
Nodes (10): SantaRitaBarHost, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt, SantaRitaNpcWork, StationIndex (+2 more)

### Community 19 - "PrologoDirector"
Cohesion: 0.08
Nodes (10): AudioAmbiente, Instance, Camada, EstradaScroller, Curvatura, Velocidade, LimpadorParabrisa, Ativo (+2 more)

### Community 20 - "Material"
Cohesion: 0.15
Nodes (4): SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaRimLight

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - ".Tem"
Cohesion: 0.14
Nodes (6): C2DialogueBranch, Disponivel, C2InteractionData, ChegadaEmSantaRita, DentroDaCasaDoAvo, DialogoLinha

### Community 25 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 26 - "SantaRitaBarLayout"
Cohesion: 0.17
Nodes (3): Prop, SantaRitaBarLayout, Actor

### Community 27 - "Capitulo2Portal"
Cohesion: 0.15
Nodes (11): BochaBall, Centro, Velocidade, BochaContact, BochaContactType, Bochas, Madeira, Saibro (+3 more)

### Community 29 - "SinucaSpinSelector"
Cohesion: 0.19
Nodes (4): BochaMapGraphic, Simulation, SinucaSpinSelector, Contact

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.15
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "AudioSettingsController"
Cohesion: 0.13
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "GameMessageUI"
Cohesion: 0.16
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 37 - "BochaShot"
Cohesion: 0.20
Nodes (9): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa (+1 more)

### Community 38 - "SinucaRoom3D"
Cohesion: 0.11
Nodes (8): SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile, Wood

### Community 39 - "SinucaTableView"
Cohesion: 0.08
Nodes (8): SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior, SinucaTableView, CameraModo, Room, Zoom

### Community 40 - "SantaRitaActorElevation"
Cohesion: 0.22
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 43 - "SinuquinhaRules"
Cohesion: 0.11
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.12
Nodes (5): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada, SantaRitaExitThreshold

### Community 45 - "CheckpointSystem"
Cohesion: 0.07
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 46 - "AudioSource"
Cohesion: 0.14
Nodes (4): Capitulo2Audio, SantaRitaBarAudio, SantaRitaSound, EventsPlayed

### Community 49 - ".Scenes"
Cohesion: 0.09
Nodes (10): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification, SinucaRef7Authoring, Capitulo2Portal, Prompt, SpawnPoint (+2 more)

### Community 51 - ".Stretch"
Cohesion: 0.12
Nodes (3): SantaRitaInventory, UIFactory, Font

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - "Legenda"
Cohesion: 0.15
Nodes (6): CelularCarro, Disponivel, Fala, Legenda, Instance, RadioCarro

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 84 - "Interactable"
Cohesion: 0.07
Nodes (10): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance, SceneFader (+2 more)

### Community 85 - "SinucaCueSpin"
Cohesion: 0.29
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 87 - ".Apply"
Cohesion: 0.14
Nodes (5): SantaRitaArtProduction, Capitulo2Interactable, Prompt, Fileira, SantaRitaFootsteps

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 94 - ".Run"
Cohesion: 0.23
Nodes (5): BochaPerspectiveVerification, Chapter2Verification, MarcusGait, Quadro, QuadroDetalhado

### Community 96 - "Capitulo2Director"
Cohesion: 0.24
Nodes (4): C2ContentData, Capitulo2Director, Instance, ProntoParaBocha

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

## Knowledge Gaps
- **258 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+253 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 430 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **52 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `Vector2`, `Chuva`, `RectTransform`, `SpriteRenderer`, `.CancelarCargaAtiva`, `.ConstruirUI`, `.Tem`, `SinucaSpinSelector`, `GameObject`, `GameMessageUI`, `SinucaTableView`, `SinuquinhaRules`, `AudioSource`, `.Stretch`, `EstadoPartida`, `.Run`, `.SimularFisicaMesa`, `SinucaCueSpin`, `.IniciarTurnoAtual`, `DispositivoCarga`, `RuntimeInitializeOnLoadMethod`, `.AtualizarVisualTacoEMira`?**
  _High betweenness centrality (0.135) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _258 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.05799275090613673 - nodes in this community are weakly interconnected._
- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `IEnumerator`, `unityengine`, `Capitulo1Director`, `.ObterEntradasDiario`, `GameMessageUI`, `SantaRitaChapter1Builder`, `TipoInteracao`, `SpriteRenderer`, `Interactable`, `.Marcar`, `Legenda`, `.Tem`, `GameObject`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.06635318704284221 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `BochaMinigameUI` to `unityengine`, `GameMessageUI`, `BochaSimulation`, `BochaShot`, `Vector2`, `Chuva`, `BochaCourtView`, `RectTransform`, `AudioSource`, `SpriteRenderer`, `.Stretch`, `Fase`, `Capitulo2Portal`, `SinucaSpinSelector`, `GameObject`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.08235294117647059 - nodes in this community are weakly interconnected._