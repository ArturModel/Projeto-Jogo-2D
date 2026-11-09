# Graph Report - SantaRita  (2026-11-09)

## Corpus Check
- 232 files · ~182,466 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 3286 nodes · 11355 edges · 156 communities (83 shown, 73 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 756 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `ebdce216`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- SinucaRoom3D
- SinucaMinigameUI
- .Project
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
- BoxCollider2D
- BochaMinigameUI
- Posto3D
- BarDoZe3D
- Sprite
- TilePiso
- Capitulo1Director
- .Capture
- ObjetoCarro
- BochaSimulation
- .ComecarNovaSerie
- BochaRoom3D
- BochaRoom3D
- AudioSettingsController
- json
- PrologoJogavel
- MapaInterativoUI
- SinucaBrasileiraRules
- BochaShot
- SantaRitaInventory
- SinucaTableView
- .Add
- .Roteiro
- SinuquinhaRules
- PlayerController
- CheckpointSystem
- List
- TileAleatorio
- Color
- .Update
- .Scenes
- PayphoneJogoUI
- Action
- EstadoPartida
- MonoBehaviour
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
- .Rebuild
- SceneFader
- RectTransform
- fitas.lua
- SantaRitaActorElevation
- SantaRitaRuaNorte
- DispositivoCarga
- .Marcar
- PrologoDirector
- PayphoneCheckpoint
- .SincronizarMesa3D
- bar-patrons.lua
- Interactable
- SantaRitaBarLighting
- MarcusNotebookUI
- EstradaScroller
- SpawnPoint
- AudioAmbiente
- Button
- AssetPostprocessor
- SantaRitaBarLayout
- Vector2
- .Dizer
- PrologoVideo
- SantaRitaAtmosphere
- LimpadorParabrisa
- CamadaParallax
- .Build
- Event
- SantaRitaTapePickup
- Shader
- MenuItem
- pathlib
- Bake-SpriteSurfaces.py
- .CriarCoracao
- Fase
- ChuvaRolando
- BochaAI
- DoorTrigger
- .RunAtlas
- SantaRitaNpcRoutine
- SpriteRenderer
- TransicaoDeZonaTrigger
- .Apply
- save-patrons-aseprite.py
- .Run
- MenuAmbienteAuthoring
- ItemDeCena
- ZonaId
- graphify-project.py
- prepare-sinuca-impacts.py
- SantaRitaBochaAmbiente
- Local
- Sombra
- SantaRitaNotebookAuthoring
- Light
- MovelLoja
- Interior

## God Nodes (most connected - your core abstractions)
1. `SinucaMinigameUI` - 247 edges
2. `SinucaRoom3D` - 190 edges
3. `SantaRitaRuaNorte` - 130 edges
4. `PlayerController` - 110 edges
5. `Chapter2RuntimeVerification` - 106 edges
6. `BochaMinigameUI` - 104 edges
7. `Posto3D` - 99 edges
8. `Capitulo1Director` - 87 edges
9. `Capitulo2Interior3D` - 79 edges
10. `InteracaoCapitulo1` - 77 edges

## Surprising Connections (you probably didn't know these)
- `SantaRitaChapter2Authoring` --references--> `Capitulo2Director`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Editor/SantaRitaChapter2Authoring.cs → Projeto Jogo 2D/Assets/Scripts/Capitulo2/Capitulo2Director.cs
- `BarDoZe3D` --references--> `Chuva`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/BarDoZe3D.cs → Projeto Jogo 2D/Assets/Scripts/Chuva.cs
- `BarDoZe3D` --references--> `InteracaoCapitulo1`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/BarDoZe3D.cs → Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs
- `BarDoZe3D` --references--> `SinucaRoom3D`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/BarDoZe3D.cs → Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.Actors.cs
- `Capitulo1Director` --references--> `PayphoneCheckpoint`  [EXTRACTED]
  Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs → Projeto Jogo 2D/Assets/Scripts/PayphoneCheckpoint.cs

## Import Cycles
- None detected.

## Communities (156 total, 73 thin omitted)

### Community 2 - "SinucaRoom3D"
Cohesion: 0.04
Nodes (24): SinucaRoom3D, FocoDaPortaDoBar, OlharExploracao, PosicaoExploracao, OpponentAtShot, PatronVisual, CanInspectUnderTable, FalseBallVisible (+16 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.05
Nodes (20): BicoCacapa, SinucaMinigameUI, ConsoleSinucaAberto, FalaPercepcaoOcupada, OlhandoDebaixo, PercepcaoSobMesaAtiva, CameraAtual, EfeitoAtual (+12 more)

### Community 4 - ".Project"
Cohesion: 0.27
Nodes (3): MarcusNotebookCatalog, MarcusNotebookContent, MarcusNotebookEntry

### Community 8 - "Chuva"
Cohesion: 0.07
Nodes (7): Camada, Chuva, Gota, Poca, Respingo, SantaRitaFootsteps, SantaRitaShallowStairs

### Community 9 - "BochaCourtView"
Cohesion: 0.07
Nodes (11): BochaCameraView, Alvo, Caminhando, Lancamento, Superior, BochaCourtView, AimDistance, Room (+3 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.05
Nodes (37): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+29 more)

### Community 12 - "Image"
Cohesion: 0.15
Nodes (4): MenuPausa, Aberto, ClosedFrame, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.16
Nodes (3): InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.13
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "BoxCollider2D"
Cohesion: 0.23
Nodes (3): SantaRitaLivingWorldAuthoring, Capitulo2Portal, Prompt

### Community 17 - "BochaMinigameUI"
Cohesion: 0.08
Nodes (7): Instance, IsShowing, Alucinacao, Buraco, FiguraSentada, Nenhuma, BochaMinigameUI

### Community 18 - "Posto3D"
Cohesion: 0.14
Nodes (5): Posto3D, Aberto, Casa, FocoDaPorta, PontoDeChegada

### Community 19 - "BarDoZe3D"
Cohesion: 0.09
Nodes (5): BarDoZe3D, Aberto, FocoDaPorta, SantaRitaInteriorAudio, Ligado

### Community 20 - "Sprite"
Cohesion: 0.12
Nodes (7): SantaRitaArtProduction, SantaRitaRuralPaths, Stretch, Capitulo2Interactable, Prompt, SantaRitaBarPatron, Prompt

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 24 - "Capitulo1Director"
Cohesion: 0.09
Nodes (5): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual

### Community 25 - ".Capture"
Cohesion: 0.19
Nodes (6): SantaRitaAtlasAuthoring, AtlasAccess, AtlasArrival, AtlasLandmark, AtlasRegion, AtlasSurvey

### Community 26 - "ObjetoCarro"
Cohesion: 0.19
Nodes (5): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo

### Community 27 - "BochaSimulation"
Cohesion: 0.10
Nodes (19): BochaPerspectiveVerification, Chapter2Verification, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor (+11 more)

### Community 28 - ".ComecarNovaSerie"
Cohesion: 0.12
Nodes (5): MinigamePerceptionSchedule, Active, Age, IsForced, Remaining

### Community 29 - "BochaRoom3D"
Cohesion: 0.10
Nodes (7): AtLaunch, Pitch, StandingPosition, TrackCount, Yaw, BochaRoom3D, FiguraEmFoco

### Community 30 - "BochaRoom3D"
Cohesion: 0.14
Nodes (6): C2DialogueBranch, Disponivel, DialogoLinha, GameMessageUI, Instance, IsShowing

### Community 31 - "AudioSettingsController"
Cohesion: 0.13
Nodes (8): AudioSettingsController, EmMinigame, VolumeChuva, VolumeChuvaAmbiente, VolumeEfeitos, VolumeFala, VolumeImpactos, VolumeMusica

### Community 32 - "json"
Cohesion: 0.15
Nodes (4): fetch(), main(), main(), main()

### Community 33 - "PrologoJogavel"
Cohesion: 0.07
Nodes (22): PortaLuvasCarro, PrologoJogavel, Curva, Direcao, Distancia, Estacionamento, FarolAlto, FaseLimpador (+14 more)

### Community 34 - "MapaInterativoUI"
Cohesion: 0.14
Nodes (5): MapaInterativoUI, Instance, IsShowing, SantaRitaAtlasView, Interior

### Community 35 - "SinucaBrasileiraRules"
Cohesion: 0.11
Nodes (16): Result, Sequence, FreeColor, MustOn, OptionalColor, Start, SinucaBrasileiraRules, Finished (+8 more)

### Community 37 - "BochaShot"
Cohesion: 0.14
Nodes (11): BochaHandResult, BochaSide, Marcus, Nenhum, Tonao, BochaLaunch, BochaShot, BochaThrowType (+3 more)

### Community 38 - "SantaRitaInventory"
Cohesion: 0.06
Nodes (15): MilaRuntimeVerification, SantaRitaInventory, ClosedFrame, Instance, IsOpen, SantaRitaTapeDeck, ClosedFrame, Instance (+7 more)

### Community 39 - "SinucaTableView"
Cohesion: 0.07
Nodes (9): SinucaCameraView, Caminhando3D, LinhaTaco, Mesa3D, VistaSuperior, SinucaTableView, CameraModo, Room (+1 more)

### Community 42 - ".Roteiro"
Cohesion: 0.23
Nodes (3): MenuAmbienteRuntimeQA, Resultado, Rodando

### Community 43 - "SinuquinhaRules"
Cohesion: 0.08
Nodes (19): Reaction, SinucaZeComments, Result, Sequence, FreeColor, MustOn, OptionalColor, Start (+11 more)

### Community 44 - "PlayerController"
Cohesion: 0.07
Nodes (12): MarcusGait, Quadro, QuadroDetalhado, PlayerController, CaminhandoParaEventoAmbiental, DirecaoAtual, EspelharCaminhadaDireita, LanternaVisivel (+4 more)

### Community 45 - "CheckpointSystem"
Cohesion: 0.07
Nodes (25): SinucaPixelPreviewSession, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 46 - "List"
Cohesion: 0.07
Nodes (17): CamadaDeLuz, Nome, Arte, Araucaria, Arbusto, Arvore, Bracatinga, Entrada (+9 more)

### Community 48 - "Color"
Cohesion: 0.06
Nodes (13): BochaMapGraphic, BochaOculta, Simulation, Atual, Capitulo2Interior3D, Capitulo2PublicRecordsDoor3D, Camada, Ambiente (+5 more)

### Community 50 - ".Scenes"
Cohesion: 0.20
Nodes (4): Chapter2BuildVerification, Evidence, ForcarDevelopment, MilaProductionVerification

### Community 51 - "PayphoneJogoUI"
Cohesion: 0.18
Nodes (4): PayphoneJogoUI, Busy, FoneForaDoGancho, Ready

### Community 52 - "Action"
Cohesion: 0.05
Nodes (16): CameraFollow, LimiteMax, LimiteMin, Fala, Legenda, Instance, MinigameDialogue, Ocupada (+8 more)

### Community 53 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 54 - "MonoBehaviour"
Cohesion: 0.05
Nodes (13): ChuvaMinigamesSmoke, Capitulo2Audio, Capitulo2Perception, ChegadaEmSantaRita, ChuvaMinigame, DentroDaCasaDoAvo, MenuItemSelector, HorizontalStepperNavigation (+5 more)

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 56 - "Ground"
Cohesion: 0.14
Nodes (14): Ground, Crack, Curb, Dirt, DirtRoad, DirtVerge, Drain, Grass (+6 more)

### Community 57 - "GameObject"
Cohesion: 0.12
Nodes (4): InteractionPrompt, Instance, MenuPanelTransition, OptionsPanelController

### Community 62 - "marcus-walk.lua"
Cohesion: 0.83
Nodes (3): c(), leg(), stroke()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

### Community 83 - ".Rebuild"
Cohesion: 0.23
Nodes (3): PontoMapa, SantaRitaAtlas, Survey

### Community 84 - "SceneFader"
Cohesion: 0.14
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 85 - "RectTransform"
Cohesion: 0.13
Nodes (3): BolaMesa, UIFactory, Font

### Community 86 - "fitas.lua"
Cohesion: 0.83
Nodes (3): col(), rect(), save()

### Community 88 - "SantaRitaActorElevation"
Cohesion: 0.20
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 89 - "SantaRitaRuaNorte"
Cohesion: 0.09
Nodes (6): SantaRitaRuaNorte, Active, AvenidaMolhada, EntradaAtiva, RuaDoBar, ZonaAtiva

### Community 91 - "DispositivoCarga"
Cohesion: 0.50
Nodes (4): DispositivoCarga, MouseEsquerdo, Nenhum, TeclaEspaco

### Community 94 - ".Marcar"
Cohesion: 0.13
Nodes (5): C2ContentData, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 96 - "PayphoneCheckpoint"
Cohesion: 0.05
Nodes (16): PayphoneCheckpoint, ClosedFrame, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher, Presentation, List (+8 more)

### Community 99 - ".SincronizarMesa3D"
Cohesion: 0.24
Nodes (6): SinucaAimGuideData, SinucaCueSpin, Avancar, Normal, Recuar, SinucaVisualBall

### Community 101 - "bar-patrons.lua"
Cohesion: 0.70
Nodes (4): box(), c(), limb(), line()

### Community 103 - "Interactable"
Cohesion: 0.17
Nodes (6): Interactable, InteractionRadius, Prompt, RenderizadorDoIndicador, SantaRitaBarProp, Prompt

### Community 106 - "MarcusNotebookUI"
Cohesion: 0.13
Nodes (7): MarcusNotebookUI, HasPendingNotice, IsOpen, MarkedEntry, PageCount, Spread, UpdateCount

### Community 107 - "EstradaScroller"
Cohesion: 0.33
Nodes (4): EstradaScroller, Curvatura, Velocidade, Fileira

### Community 108 - "SpawnPoint"
Cohesion: 0.29
Nodes (3): SpawnPoint, DirecaoInicial, Id

### Community 109 - "AudioAmbiente"
Cohesion: 0.22
Nodes (3): AudioAmbiente, Instance, Camada

### Community 110 - "Button"
Cohesion: 0.11
Nodes (5): MainMenuActions, MenuAmbiente, FocoAtual, Pronto, Saindo

### Community 112 - "AssetPostprocessor"
Cohesion: 0.25
Nodes (3): MenuAmbienteTextureImporter, PrologoTextureImporter, SantaRitaArtImporter

### Community 113 - "SantaRitaBarLayout"
Cohesion: 0.09
Nodes (15): Kind, Actor, Chair, Counter, Decor, Shelf, Table, Wall (+7 more)

### Community 115 - "Vector2"
Cohesion: 0.11
Nodes (6): OlharPrimeiraPessoa, Deslocamento, Prop, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 124 - "Event"
Cohesion: 0.18
Nodes (8): Event, Blackout, FalseBall, HauntedZe, None, UnderTable, WindowKnock, SinucaPerceptionSchedule

### Community 126 - "SantaRitaTapePickup"
Cohesion: 0.43
Nodes (3): SantaRitaTapePickup, Prompt, SantaRitaTapePlacement

### Community 128 - "MenuItem"
Cohesion: 0.17
Nodes (3): SantaRitaMcpSetup, SantaRitaProductionVerification, SinucaRef7Authoring

### Community 129 - "pathlib"
Cohesion: 0.12
Nodes (5): flamula(), frame(), mural(), placar(), gerar_marcus_lanterna()

### Community 130 - "Bake-SpriteSurfaces.py"
Cohesion: 0.25
Nodes (9): bake(), bake_sheet(), slope(), link(), main(), neighbor(), profile(), texture_meta() (+1 more)

### Community 133 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 135 - "BochaAI"
Cohesion: 0.67
Nodes (3): BochaAI, UltimaDecisaoMs, UltimosCandidatos

### Community 139 - "SpriteRenderer"
Cohesion: 0.15
Nodes (4): Capitulo2Exterior, SantaRitaBarPose, SantaRitaForeground, SantaRitaRimLight

### Community 140 - "TransicaoDeZonaTrigger"
Cohesion: 0.36
Nodes (3): InfoZona, TransicaoDeZonaTrigger, Prompt

### Community 142 - "save-patrons-aseprite.py"
Cohesion: 0.24
Nodes (3): aseprite(), chunk(), string()

### Community 146 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 148 - "graphify-project.py"
Cohesion: 0.42
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 151 - "Local"
Cohesion: 0.25
Nodes (8): Local, Bar, Casa, Jardim, Mercadinho, Posto, Salao, Sociedade

### Community 152 - "Sombra"
Cohesion: 0.25
Nodes (8): Sombra, Contato, EdificioSuave, Nenhuma, Projetada, ProjetadaSuave, Recorte, Veiculo

### Community 156 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

### Community 160 - "Interior"
Cohesion: 0.50
Nodes (4): Interior, CasaDoAvo, Mercadinho, Posto

## Knowledge Gaps
- **435 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Output`, `Instance`, `VolumeMusica` (+430 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 679 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **73 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `IEnumerator`, `Image`, `DicaDeTecla`, `.Run`, `BarDoZe3D`, `.AtualizarVisualTacoEMira`, `.ComecarNovaSerie`, `SinucaTableView`, `SinuquinhaRules`, `CheckpointSystem`, `List`, `Color`, `.Update`, `Action`, `EstadoPartida`, `MonoBehaviour`, `RectTransform`, `DispositivoCarga`, `unityengine_ui`, `.SincronizarMesa3D`, `.OnMesaPointerDown`, `RuntimeInitializeOnLoadMethod`, `GameObject`, `Vector2`, `Event`?**
  _High betweenness centrality (0.160) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Output` to the rest of the system?**
  _435 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.13333333333333333 - nodes in this community are weakly interconnected._
- **Why does `SinucaRoom3D` connect `SinucaRoom3D` to `TipoInteracao`, `SpriteRenderer`, `.Run`, `Posto3D`, `BarDoZe3D`, `Sprite`, `Light`, `.ComecarNovaSerie`, `BochaRoom3D`, `unityengine`, `SinucaTableView`, `CheckpointSystem`, `List`, `.Update`, `Action`, `RectTransform`, `Vector3`, `SantaRitaRuaNorte`, `.SincronizarMesa3D`, `SantaRitaBarLayout`, `GameObject`, `Vector2`, `PrologoVideo`, `Transform`?**
  _High betweenness centrality (0.064) - this node is a cross-community bridge._
- **Should `SinucaRoom3D` be split into smaller, more focused modules?**
  _Cohesion score 0.04044943820224719 - nodes in this community are weakly interconnected._
- **Why does `SantaRitaRuaNorte` connect `SantaRitaRuaNorte` to `Light2D`, `Chuva`, `SpriteRenderer`, `SantaRitaC2Consolidation`, `Posto3D`, `Sprite`, `Sombra`, `Capitulo1Director`, `BochaRoom3D`, `unityengine`, `SpriteRenderer`, `.Add`, `PlayerController`, `List`, `MonoBehaviour`, `Ground`, `RawImage`, `GameObject`, `Vector2`, `PrologoVideo`, `Transform`, `.BuildTilemap`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.054098360655737705 - nodes in this community are weakly interconnected._