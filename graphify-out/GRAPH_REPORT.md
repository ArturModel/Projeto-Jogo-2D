# Graph Report - Projeto-Jogo-2D  (2026-11-06)

## Corpus Check
- 154 files · ~114,260 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 2129 nodes · 7091 edges · 124 communities (62 shown, 62 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 316 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `879a90c4`
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
- .Apply
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
- GameState
- SinucaTableView
- MonoBehaviour
- .Add
- .Roteiro
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- MenuAmbienteGraphic
- TileAleatorio
- Chapter2RuntimeVerification
- .Scenes
- PrologoRuntimeVerification
- RectTransform
- Fase
- EstadoPartida
- Legenda
- c2-workstations.lua
- PayphoneCheckpoint
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
- PayphoneRegistroUI
- SceneFader
- .Sync
- .Marcar
- MarcusGait
- Material
- CameraFollow
- DispositivoCarga
- CamadaDeLuz
- .Awake
- AudioSource
- HorizontalStepperNavigation
- ZonaId
- BochaCameraView
- BochaBall
- bar-patrons.lua
- ItemDeCena
- Camada
- SantaRitaRoomCutaway
- MovelLoja
- Interactable
- MenuAmbiente
- .BeginIsolatedTests
- List
- .Run
- SubtitleSettingsController
- SantaRitaBarAudio
- DoorTrigger
- MenuItemSelector
- ChuvaRolando
- .Executar

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

## Communities (124 total, 62 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.05
Nodes (8): MenuAmbienteAuthoring, MenuAmbienteTextureImporter, PrologoTextureImporter, SantaRitaArtImporter, SantaRitaMcpSetup, CamadaParallax, SantaRitaItemAction, SantaRitaWorldLighting

### Community 2 - "SinucaRoom3D"
Cohesion: 0.10
Nodes (9): Actor, SinucaRoom3D, DarkWood, FloorTile, FrameGeometryBuilds, StaticRendererCount, Target, WallTile (+1 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.07
Nodes (11): BicoCacapa, SinucaMinigameUI, CameraAtual, EfeitoAtual, EmModoPratica, EventosAudioDisparados, Instance, IsShowing (+3 more)

### Community 4 - "BochaMatch"
Cohesion: 0.17
Nodes (12): BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor, BochaSide (+4 more)

### Community 6 - "Vector2"
Cohesion: 0.08
Nodes (10): SantaRitaChapter2Authoring, PontoMapa, OlharPrimeiraPessoa, Deslocamento, Prop, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition (+2 more)

### Community 8 - "Chuva"
Cohesion: 0.14
Nodes (4): Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.12
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.15
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.07
Nodes (11): C2DialogueBranch, Disponivel, DialogoLinha, GameMessageUI, Instance, IsShowing, InteracaoCapitulo1, Prompt (+3 more)

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 16 - "SpriteRenderer"
Cohesion: 0.18
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.17
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.15
Nodes (7): Fileira, SantaRitaBarLayout, SantaRitaBarPatron, Prompt, SantaRitaBarPose, SantaRitaBarProp, Prompt

### Community 19 - "PrologoDirector"
Cohesion: 0.08
Nodes (9): AudioAmbiente, Instance, EstradaScroller, Curvatura, Velocidade, LimpadorParabrisa, Ativo, FalaMarcada (+1 more)

### Community 20 - ".Apply"
Cohesion: 0.13
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, SantaRitaFootsteps

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "Capitulo1Director"
Cohesion: 0.11
Nodes (6): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona

### Community 25 - "Kind"
Cohesion: 0.25
Nodes (8): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall

### Community 26 - "ObjetoCarro"
Cohesion: 0.19
Nodes (5): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo

### Community 27 - "BochaSimulation"
Cohesion: 0.16
Nodes (8): BochaContact, BochaContactType, Bochas, Madeira, Saibro, BochaSimulation, IsMoving, Evento

### Community 28 - "PointerEventData"
Cohesion: 0.12
Nodes (3): SantaRitaItemHover, SinucaSpinSelector, Contact

### Community 32 - "graphify-project.py"
Cohesion: 0.18
Nodes (7): command(), fingerprint(), main(), run(), sources(), validate(), preparar()

### Community 33 - "PrologoJogavel"
Cohesion: 0.08
Nodes (20): PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador, Finalizado (+12 more)

### Community 34 - "MapaInterativoUI"
Cohesion: 0.15
Nodes (3): MapaInterativoUI, Instance, IsShowing

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.10
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 36 - "Capitulo2Director"
Cohesion: 0.15
Nodes (6): C2ContentData, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha, SantaRitaExitThreshold

### Community 37 - "BochaShot"
Cohesion: 0.18
Nodes (8): BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaLaunch, BochaThrowType, Ponto, Rafa, Volo

### Community 38 - "GameState"
Cohesion: 0.15
Nodes (9): EntradaDiario, Flags, GameState, LanternaLigada, ObjetivoAtual, TemProgressoNaoSalvo, GameStateSnapshot, ParNumero (+1 more)

### Community 39 - "SinucaTableView"
Cohesion: 0.07
Nodes (8): SinucaCameraView, LinhaTaco, Mesa3D, VistaSuperior, SinucaTableView, CameraModo, Room, Zoom

### Community 40 - "MonoBehaviour"
Cohesion: 0.07
Nodes (13): Capitulo2Gate, ChegadaEmSantaRita, DentroDaCasaDoAvo, SantaRitaActorElevation, VisualDepth, VisualScale, SantaRitaBarHost, SantaRitaNpcWork (+5 more)

### Community 42 - ".Roteiro"
Cohesion: 0.23
Nodes (3): MenuAmbienteRuntimeQA, Resultado, Rodando

### Community 43 - "SinuquinhaRules"
Cohesion: 0.11
Nodes (16): Sequence, FreeColor, MustOn, OptionalColor, Start, SinuquinhaRules, Finished, MarcusGroup (+8 more)

### Community 44 - "PlayerController"
Cohesion: 0.14
Nodes (4): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada

### Community 45 - "CheckpointSystem"
Cohesion: 0.12
Nodes (14): CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot, PrefKeyPrimary (+6 more)

### Community 49 - ".Scenes"
Cohesion: 0.20
Nodes (4): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification

### Community 51 - "RectTransform"
Cohesion: 0.11
Nodes (3): SantaRitaInventory, UIFactory, Font

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - "Legenda"
Cohesion: 0.15
Nodes (5): Fala, Legenda, Instance, Fala, PrologoVideo

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 56 - "PayphoneCheckpoint"
Cohesion: 0.17
Nodes (5): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 83 - "PayphoneRegistroUI"
Cohesion: 0.17
Nodes (5): PayphoneRegistroUI, Ocupado, RegistroSelecionado, SomenteCarregar, TemArte

### Community 84 - "SceneFader"
Cohesion: 0.21
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 85 - ".Sync"
Cohesion: 0.23
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 86 - ".Marcar"
Cohesion: 0.13
Nodes (3): CelularCarro, Disponivel, RadioCarro

### Community 87 - "MarcusGait"
Cohesion: 0.53
Nodes (3): MarcusGait, Quadro, QuadroDetalhado

### Community 88 - "Material"
Cohesion: 0.12
Nodes (5): SantaRitaRuralPaths, Stretch, SinucaRef7Authoring, SantaRitaAtmosphere, SantaRitaRimLight

### Community 89 - "CameraFollow"
Cohesion: 0.25
Nodes (3): CameraFollow, LimiteMax, LimiteMin

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 97 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 98 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 99 - "BochaBall"
Cohesion: 0.27
Nodes (5): BochaPerspectiveVerification, Chapter2Verification, BochaBall, Centro, Velocidade

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 105 - "Camada"
Cohesion: 0.67
Nodes (3): Camada, Ambiente, Leitura

### Community 108 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

### Community 109 - "Interactable"
Cohesion: 0.17
Nodes (5): Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance

### Community 110 - "MenuAmbiente"
Cohesion: 0.10
Nodes (5): MainMenuActions, MenuAmbiente, FocoAtual, Pronto, Saindo

### Community 113 - "List"
Cohesion: 0.20
Nodes (3): Camada, SantaRitaShallowStairs, Result

### Community 115 - "SubtitleSettingsController"
Cohesion: 0.29
Nodes (4): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho

## Knowledge Gaps
- **289 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+284 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 470 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **62 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `IEnumerator`, `unityengine`, `SinucaRoom3D`, `TipoInteracao`, `Interactable`, `Chapter2RuntimeVerification`, `Sprite`, `.Marcar`, `.UpdateCasaDoAvoScene`, `Transform`?**
  _High betweenness centrality (0.107) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _289 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.050041934582052 - nodes in this community are weakly interconnected._
- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `SinucaRoom3D`, `Vector2`, `Image`, `Image`, `InteracaoCapitulo1`, `.AtualizarVisualTacoEMira`, `PointerEventData`, `GameState`, `SinucaTableView`, `MonoBehaviour`, `SinuquinhaRules`, `CheckpointSystem`, `Chapter2RuntimeVerification`, `RectTransform`, `EstadoPartida`, `.Sync`, `DispositivoCarga`, `AudioSource`, `.AtualizarVisualTacoEMira`, `RuntimeInitializeOnLoadMethod`, `List`, `.Run`?**
  _High betweenness centrality (0.099) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.0963265306122449 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `SinucaRoom3D`, `SantaRitaChapter1Builder`, `Vector2`, `Image`, `Sprite`, `TilePiso`, `Transform`, `MonoBehaviour`, `RectTransform`, `PayphoneCheckpoint`, `.Marcar`, `CameraFollow`, `AudioSource`, `ZonaId`, `MovelLoja`, `RuntimeInitializeOnLoadMethod`, `Scene`, `.UpdateCasaDoAvoScene`?**
  _High betweenness centrality (0.067) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.07180851063829788 - nodes in this community are weakly interconnected._