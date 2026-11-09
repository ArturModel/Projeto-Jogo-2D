# Graph Report - SantaRita  (2026-11-08)

## Corpus Check
- 205 files · ~139,279 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2769 nodes · 9279 edges · 138 communities (66 shown, 72 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 524 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `b89b5e57`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- system
- SinucaRoom3D
- SinucaMinigameUI
- .RunNotebook
- SantaRitaChapter1Builder
- Vector2
- SantaRitaTapeDeck
- Chuva
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- Image
- InteracaoCapitulo1
- DicaDeTecla
- VideoSettingsController
- BoxCollider2D
- BochaMinigameUI
- MeshRenderer
- PrologoDirector
- Sprite
- TilePiso
- Transform
- Kind
- ObjetoCarro
- BochaSimulation
- BochaRoom3D
- BochaRoom3D
- AudioSettingsController
- json
- PrologoJogavel
- .Capture
- SinucaBrasileiraRules
- BochaShot
- SantaRitaInventory
- SinucaTableView
- SantaRitaShallowStairs
- .Add
- .Roteiro
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- Color
- TileAleatorio
- MenuAmbienteGraphic
- .Update
- .RunAtlas
- MarcusNotebookUI
- Fase
- EstadoPartida
- SinucaRoom3D
- c2-workstations.lua
- Ground
- GameObject
- c2-npc-work.lua
- marcus-flashlight.lua
- marcus-walk.lua
- pool-table.lua
- graphify-refresh.sh
- graphify-setup.sh
- graphify-validate.sh
- bocha-court-atlas.lua
- market-stock.lua
- PayphoneCheckpoint
- SceneFader
- RectTransform
- fitas.lua
- Capitulo2Director
- SantaRitaRuaNorte
- DispositivoCarga
- .Marcar
- AudioSource
- MenuItemSelector
- BochaCameraView
- .SincronizarMesa3D
- bar-patrons.lua
- MinigameDialogue
- CamadaDeLuz
- SinucaSpinSelector
- AudioClip
- MonoBehaviour
- AudioAmbiente
- Button
- MenuAmbienteAuthoring.cs
- SpriteRenderer
- SantaRitaBarLighting
- Vector2
- SantaRitaBarAudio
- Scene
- EstradaScroller
- SantaRitaAtmosphere
- LimpadorParabrisa
- Dictionary
- CamadaParallax
- .Build
- Event
- BochaMapGraphic
- OlharPrimeiraPessoa
- SinucaCameraView
- SantaRitaMcpSetup
- Alucinacao
- .CriarCoracao
- .Prepare
- ChuvaRolando
- BochaAI
- .Executar
- Camada

## God Nodes (most connected - your core abstractions)
1. `SinucaMinigameUI` - 247 edges
2. `SinucaRoom3D` - 161 edges
3. `Chapter2RuntimeVerification` - 106 edges
4. `BochaMinigameUI` - 104 edges
5. `PlayerController` - 86 edges
6. `Capitulo1Director` - 78 edges
7. `BochaRoom3D` - 66 edges
8. `InteracaoCapitulo1` - 66 edges
9. `Chuva` - 53 edges
10. `SinucaTableView` - 53 edges

## Surprising Connections (you probably didn't know these)
- `SantaRitaChapter2Authoring` --references--> `Capitulo2Director`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Editor/SantaRitaChapter2Authoring.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/Capitulo2Director.cs
- `Capitulo1Director` --references--> `PayphoneCheckpoint`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs → Projeto Jogo 2D/Assets/Scripts/PayphoneCheckpoint.cs
- `BochaRoom3D` --references--> `BochaCameraView`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaRoom3D.Perception.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs
- `BochaCourtView` --references--> `BochaRoom3D`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaRoom3D.Perception.cs
- `BochaCourtView` --references--> `BochaShot`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaCourtView.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaShot.cs

## Import Cycles
- None detected.

## Communities (138 total, 72 thin omitted)

### Community 2 - "SinucaRoom3D"
Cohesion: 0.05
Nodes (20): SinucaRoom3D, OpponentAtShot, CanInspectUnderTable, FalseBallVisible, GripShake, HauntedOpponentInFocus, InspectingUnderTable, LookingAtFalseBall (+12 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.05
Nodes (20): BicoCacapa, SinucaMinigameUI, ConsoleSinucaAberto, FalaPercepcaoOcupada, OlhandoDebaixo, PercepcaoSobMesaAtiva, CameraAtual, EfeitoAtual (+12 more)

### Community 4 - ".RunNotebook"
Cohesion: 0.08
Nodes (14): C2ContentData, C2InteractionData, EntradaDiario, Flags, GameState, LanternaLigada, ObjetivoAtual, TemProgressoNaoSalvo (+6 more)

### Community 5 - "SantaRitaChapter1Builder"
Cohesion: 0.08
Nodes (10): SantaRitaChapter1Builder, DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance (+2 more)

### Community 7 - "SantaRitaTapeDeck"
Cohesion: 0.14
Nodes (10): SantaRitaTapeDeck, ClosedFrame, Instance, IsOpen, Playing, Position, Records, TapeCatalog (+2 more)

### Community 8 - "Chuva"
Cohesion: 0.11
Nodes (4): Camada, Chuva, Gota, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.07
Nodes (7): BochaCourtView, AimDistance, Room, TrackCount, View, Zoom, SantaRitaItemHover

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.13
Nodes (4): MenuPausa, Aberto, ClosedFrame, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.12
Nodes (6): C2DialogueBranch, Disponivel, DialogoLinha, InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.13
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "BoxCollider2D"
Cohesion: 0.16
Nodes (6): SantaRitaLivingWorldAuthoring, Capitulo2Director, Instance, ProntoParaBocha, Capitulo2Portal, Prompt

### Community 17 - "BochaMinigameUI"
Cohesion: 0.10
Nodes (3): Instance, IsShowing, BochaMinigameUI

### Community 20 - "Sprite"
Cohesion: 0.11
Nodes (7): SantaRitaArtProduction, SantaRitaProductionVerification, SantaRitaRuralPaths, Stretch, Capitulo2Interactable, Prompt, SantaRitaFootsteps

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "Transform"
Cohesion: 0.06
Nodes (25): SantaRitaNotebookAuthoring, Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona, MovelLoja (+17 more)

### Community 25 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 26 - "ObjetoCarro"
Cohesion: 0.19
Nodes (5): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo

### Community 27 - "BochaSimulation"
Cohesion: 0.09
Nodes (24): BochaPerspectiveVerification, Chapter2Verification, BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador (+16 more)

### Community 29 - "BochaRoom3D"
Cohesion: 0.10
Nodes (7): AtLaunch, Pitch, StandingPosition, TrackCount, Yaw, BochaRoom3D, FiguraEmFoco

### Community 30 - "BochaRoom3D"
Cohesion: 0.18
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 31 - "AudioSettingsController"
Cohesion: 0.11
Nodes (8): AudioSettingsController, EmMinigame, VolumeChuva, VolumeChuvaAmbiente, VolumeEfeitos, VolumeFala, VolumeImpactos, VolumeMusica

### Community 32 - "json"
Cohesion: 0.05
Nodes (18): command(), fingerprint(), main(), run(), sources(), validate(), flamula(), frame() (+10 more)

### Community 33 - "PrologoJogavel"
Cohesion: 0.08
Nodes (21): PortaLuvasCarro, PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador (+13 more)

### Community 34 - ".Capture"
Cohesion: 0.08
Nodes (15): SantaRitaAtlasAuthoring, MapaInterativoUI, Instance, IsShowing, PontoMapa, AtlasAccess, AtlasArrival, AtlasLandmark (+7 more)

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 37 - "BochaShot"
Cohesion: 0.25
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 38 - "SantaRitaInventory"
Cohesion: 0.21
Nodes (4): SantaRitaInventory, ClosedFrame, Instance, IsOpen

### Community 39 - "SinucaTableView"
Cohesion: 0.08
Nodes (5): Sinuca3DVerification, SinucaTableView, CameraModo, Room, Zoom

### Community 42 - ".Roteiro"
Cohesion: 0.23
Nodes (3): MenuAmbienteRuntimeQA, Resultado, Rodando

### Community 43 - "SinuquinhaRules"
Cohesion: 0.09
Nodes (18): Reaction, SinucaZeComments, Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules (+10 more)

### Community 44 - "PlayerController"
Cohesion: 0.08
Nodes (12): CameraFollow, LimiteMax, LimiteMin, MarcusGait, Quadro, QuadroDetalhado, PlayerController, DirecaoAtual (+4 more)

### Community 45 - "CheckpointSystem"
Cohesion: 0.09
Nodes (16): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+8 more)

### Community 49 - ".Update"
Cohesion: 0.08
Nodes (7): FalaHistoria, MinigamePerceptionSchedule, Active, Age, IsForced, Remaining, SinucaPerceptionSchedule

### Community 51 - "MarcusNotebookUI"
Cohesion: 0.17
Nodes (7): MarcusNotebookUI, HasPendingNotice, IsOpen, MarkedEntry, PageCount, Spread, UpdateCount

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 56 - "Ground"
Cohesion: 0.14
Nodes (14): Ground, Crack, Curb, Dirt, DirtRoad, DirtVerge, Drain, Grass (+6 more)

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 83 - "PayphoneCheckpoint"
Cohesion: 0.07
Nodes (11): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher, PayphoneRegistroTecla, PayphoneRegistroUI, Ocupado (+3 more)

### Community 84 - "SceneFader"
Cohesion: 0.16
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 85 - "RectTransform"
Cohesion: 0.13
Nodes (3): BolaMesa, UIFactory, Font

### Community 86 - "fitas.lua"
Cohesion: 0.83
Nodes (3): col(), rect(), save()

### Community 88 - "Capitulo2Director"
Cohesion: 0.11
Nodes (8): Chapter2BuildVerification, Evidence, ForcarDevelopment, MilaProductionVerification, Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 94 - ".Marcar"
Cohesion: 0.09
Nodes (8): CelularCarro, Disponivel, Fala, Legenda, Instance, Fala, PrologoVideo, RadioCarro

### Community 98 - "BochaCameraView"
Cohesion: 0.29
Nodes (5): BochaCameraView, Alvo, Caminhando, Lancamento, Superior

### Community 99 - ".SincronizarMesa3D"
Cohesion: 0.22
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 108 - "MonoBehaviour"
Cohesion: 0.07
Nodes (12): ChegadaEmSantaRita, DentroDaCasaDoAvo, SantaRitaBarHost, SantaRitaForeground, SantaRitaItemAction, SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex (+4 more)

### Community 109 - "AudioAmbiente"
Cohesion: 0.22
Nodes (3): AudioAmbiente, Instance, Camada

### Community 110 - "Button"
Cohesion: 0.10
Nodes (5): MainMenuActions, MenuAmbiente, FocoAtual, Pronto, Saindo

### Community 112 - "MenuAmbienteAuthoring.cs"
Cohesion: 0.22
Nodes (3): MenuAmbienteAuthoring, MenuAmbienteTextureImporter, SantaRitaArtImporter

### Community 113 - "SpriteRenderer"
Cohesion: 0.09
Nodes (10): SantaRitaBochaAmbiente, Fileira, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt (+2 more)

### Community 115 - "Vector2"
Cohesion: 0.12
Nodes (5): Poca, Prop, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 117 - "Scene"
Cohesion: 0.22
Nodes (3): SantaRitaTapePickup, Prompt, SantaRitaTapePlacement

### Community 118 - "EstradaScroller"
Cohesion: 0.39
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 124 - "Event"
Cohesion: 0.29
Nodes (7): Event, Blackout, FalseBall, HauntedZe, None, UnderTable, WindowKnock

### Community 125 - "BochaMapGraphic"
Cohesion: 0.43
Nodes (3): BochaMapGraphic, BochaOculta, Simulation

### Community 127 - "SinucaCameraView"
Cohesion: 0.40
Nodes (5): SinucaCameraView, Caminhando3D, LinhaTaco, Mesa3D, VistaSuperior

### Community 129 - "Alucinacao"
Cohesion: 0.50
Nodes (4): Alucinacao, Buraco, FiguraSentada, Nenhuma

### Community 135 - "BochaAI"
Cohesion: 0.67
Nodes (3): BochaAI, UltimaDecisaoMs, UltimosCandidatos

### Community 137 - "Camada"
Cohesion: 0.67
Nodes (3): Camada, Ambiente, Leitura

## Knowledge Gaps
- **373 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `VolumeMusica`, `VolumeEfeitos` (+368 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 598 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **72 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `IEnumerator`, `DicaDeTecla`, `.AtualizarVisualTacoEMira`, `system_collections`, `.ComecarNovaSerie`, `BochaRoom3D`, `BochaRoom3D`, `json`, `.Capture`, `unityengine`, `SinucaTableView`, `SinuquinhaRules`, `.Update`, `MarcusNotebookUI`, `EstadoPartida`, `RectTransform`, `DispositivoCarga`, `unityengine_ui`, `.Marcar`, `AudioSource`, `.SincronizarMesa3D`, `.OnMesaPointerDown`, `MinigameDialogue`, `SinucaSpinSelector`, `AudioClip`, `MonoBehaviour`, `RuntimeInitializeOnLoadMethod`, `Vector2`, `Dictionary`, `SinucaCameraView`?**
  _High betweenness centrality (0.117) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _373 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.1094017094017094 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `BochaMinigameUI` to `system`, `Alucinacao`, `BochaCourtView`, `system_collections`, `BochaSimulation`, `BochaRoom3D`, `BochaRoom3D`, `json`, `.Capture`, `BochaShot`, `Color`, `.Update`, `MarcusNotebookUI`, `Fase`, `GameObject`, `RectTransform`, `.MostrarModal`, `AudioSource`, `MinigameDialogue`, `AudioClip`, `MonoBehaviour`, `Button`, `Vector2`, `BochaMapGraphic`?**
  _High betweenness centrality (0.084) - this node is a cross-community bridge._
- **Should `system` be split into smaller, more focused modules?**
  _Cohesion score 0.10852713178294573 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Transform` to `SantaRitaChapter1Builder`, `MonoBehaviour`, `PlayerController`, `RuntimeInitializeOnLoadMethod`, `.RunAtlas`, `MarcusNotebookUI`, `PayphoneCheckpoint`, `RectTransform`, `Scene`, `TilePiso`, `Sprite`, `Dictionary`, `unityengine_ui`, `BochaRoom3D`, `.Marcar`, `AudioSource`?**
  _High betweenness centrality (0.083) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.05084745762711865 - nodes in this community are weakly interconnected._