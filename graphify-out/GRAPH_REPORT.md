# Graph Report - Projeto-Jogo-2D  (2026-11-06)

## Corpus Check
- 149 files · ~108,356 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2025 nodes · 6700 edges · 112 communities (55 shown, 57 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 311 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `dc7b5841`
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
- Image
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
- Material
- TilePiso
- PrologoEstradaGraphic
- Capitulo1Director
- Kind
- ObjetoCarro
- BochaSimulation
- PointerEventData
- BochaMapGraphic
- AudioSettingsController
- graphify-project.py
- PrologoJogavel
- MapaInterativoUI
- SinucaBrasileiraRules
- Capitulo2Director
- BochaShot
- SinucaTableView
- SantaRitaActorElevation
- .Add
- CamadaParallax
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- PayphoneCheckpoint
- TileAleatorio
- Chapter2RuntimeVerification
- Capitulo2Portal
- PrologoRuntimeVerification
- RectTransform
- Fase
- EstadoPartida
- .Dizer
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
- .Sync
- .Marcar
- .Run
- CameraFollow
- DispositivoCarga
- AudioSource
- .Awake
- HorizontalStepperNavigation
- ZonaId
- BochaCameraView
- InteractionPrompt
- bar-patrons.lua
- TransicaoDeZonaTrigger
- SantaRitaBarAudio
- SantaRitaRoomCutaway
- MenuItemSelector
- MovelLoja
- .Play
- MainMenuActions
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
10. `PrologoJogavel` - 51 edges

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

## Communities (112 total, 57 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.06
Nodes (8): PrologoTextureImporter, SantaRitaArtImporter, SantaRitaMcpSetup, C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, SantaRitaWorldLighting

### Community 2 - "SinucaRoom3D"
Cohesion: 0.08
Nodes (9): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+1 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.09
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaMatch"
Cohesion: 0.20
Nodes (11): BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor, BochaSide (+3 more)

### Community 5 - "SantaRitaChapter1Builder"
Cohesion: 0.10
Nodes (8): SantaRitaChapter1Builder, DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, ItemDeCena, Prompt

### Community 6 - "Vector2"
Cohesion: 0.09
Nodes (8): SantaRitaChapter2Authoring, PontoMapa, OlharPrimeiraPessoa, Deslocamento, Prop, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 8 - "Chuva"
Cohesion: 0.07
Nodes (8): CamadaDeLuz, Nome, Camada, Chuva, Gota, Poca, Respingo, SantaRitaShallowStairs

### Community 9 - "BochaCourtView"
Cohesion: 0.12
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.16
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.08
Nodes (7): DialogoLinha, GameMessageUI, Instance, IsShowing, InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.11
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.17
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.17
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.11
Nodes (12): Fileira, SantaRitaBarHost, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt (+4 more)

### Community 19 - "PrologoDirector"
Cohesion: 0.05
Nodes (12): PrologoJogavelVerification, AudioAmbiente, Instance, EstradaScroller, Curvatura, Velocidade, LimpadorParabrisa, Ativo (+4 more)

### Community 20 - "Material"
Cohesion: 0.09
Nodes (6): SantaRitaArtProduction, SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaFootsteps, SantaRitaRimLight

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "Capitulo1Director"
Cohesion: 0.11
Nodes (5): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual

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
Cohesion: 0.13
Nodes (3): SantaRitaItemHover, SinucaSpinSelector, Contact

### Community 32 - "graphify-project.py"
Cohesion: 0.18
Nodes (7): command(), fingerprint(), main(), run(), sources(), validate(), preparar()

### Community 33 - "PrologoJogavel"
Cohesion: 0.08
Nodes (20): PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador, Finalizado (+12 more)

### Community 34 - "MapaInterativoUI"
Cohesion: 0.13
Nodes (3): MapaInterativoUI, Instance, IsShowing

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "Capitulo2Director"
Cohesion: 0.17
Nodes (4): Capitulo2Director, Instance, ProntoParaBocha, SantaRitaExitThreshold

### Community 37 - "BochaShot"
Cohesion: 0.18
Nodes (9): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa (+1 more)

### Community 39 - "SinucaTableView"
Cohesion: 0.09
Nodes (8): SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior, SinucaTableView, CameraModo, Room, Zoom

### Community 40 - "SantaRitaActorElevation"
Cohesion: 0.16
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 43 - "SinuquinhaRules"
Cohesion: 0.10
Nodes (17): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished (+9 more)

### Community 44 - "PlayerController"
Cohesion: 0.16
Nodes (4): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada

### Community 45 - "CheckpointSystem"
Cohesion: 0.06
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 46 - "PayphoneCheckpoint"
Cohesion: 0.18
Nodes (5): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher

### Community 49 - "Capitulo2Portal"
Cohesion: 0.08
Nodes (10): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification, SinucaRef7Authoring, Capitulo2Portal, Prompt, SpawnPoint (+2 more)

### Community 51 - "RectTransform"
Cohesion: 0.13
Nodes (3): SantaRitaInventory, UIFactory, Font

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - ".Dizer"
Cohesion: 0.19
Nodes (6): ChegadaEmSantaRita, DentroDaCasaDoAvo, Fala, Legenda, Instance, SantaRitaItemAction

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
Cohesion: 0.15
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 85 - ".Sync"
Cohesion: 0.33
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 87 - ".Run"
Cohesion: 0.21
Nodes (5): BochaPerspectiveVerification, Chapter2Verification, MarcusGait, Quadro, QuadroDetalhado

### Community 89 - "CameraFollow"
Cohesion: 0.25
Nodes (3): CameraFollow, LimiteMax, LimiteMin

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 94 - ".Awake"
Cohesion: 0.22
Nodes (4): CelularCarro, Disponivel, PortaLuvasCarro, RadioCarro

### Community 97 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 103 - "TransicaoDeZonaTrigger"
Cohesion: 0.36
Nodes (3): InfoZona, TransicaoDeZonaTrigger, Prompt

### Community 108 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

## Knowledge Gaps
- **278 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+273 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 458 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **57 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `Vector2`, `Image`, `Chuva`, `BochaCourtView`, `Image`, `InteracaoCapitulo1`, `.AtualizarVisualTacoEMira`, `PointerEventData`, `.SimularFisicaMesa`, `SinucaTableView`, `SinuquinhaRules`, `CheckpointSystem`, `Chapter2RuntimeVerification`, `RectTransform`, `EstadoPartida`, `.Dizer`, `.Run`, `.OnMesaPointerDown`, `.Sync`, `.ResolverFimDaTacada`, `DispositivoCarga`, `AudioSource`, `RuntimeInitializeOnLoadMethod`, `.OnMesaPointerDown`?**
  _High betweenness centrality (0.113) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _278 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.05655794587092297 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `BochaMinigameUI` to `unityengine`, `BochaMatch`, `BochaShot`, `Vector2`, `Image`, `Chuva`, `BochaCourtView`, `Image`, `InteracaoCapitulo1`, `Chapter2RuntimeVerification`, `BochaMapGraphic`, `RectTransform`, `Fase`, `.Dizer`, `PrologoEstradaGraphic`, `BochaSimulation`, `AudioSource`?**
  _High betweenness centrality (0.071) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.08215488215488216 - nodes in this community are weakly interconnected._
- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `IEnumerator`, `unityengine`, `SantaRitaChapter1Builder`, `TipoInteracao`, `CheckpointSystem`, `Chapter2RuntimeVerification`, `Sprite`, `.Marcar`, `Transform`?**
  _High betweenness centrality (0.065) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.09113300492610837 - nodes in this community are weakly interconnected._