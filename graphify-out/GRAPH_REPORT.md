# Graph Report - Projeto-Jogo-2D  (2026-10-28)

## Corpus Check
- 121 files · ~86,770 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 1544 nodes · 4931 edges · 84 communities (40 shown, 44 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 215 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `fe8c0988`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- unityengine
- Capitulo1Director
- SinucaMinigameUI
- BochaMatch
- PlayerController
- Vector2
- Interactable
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
- Sprite
- PrologoDirector
- Material
- GameState
- TilePiso
- .GarantirModalCriado
- CanvasGroup
- Capitulo2Director
- SceneFader
- BochaSimulation
- CheckpointSystem
- ItemDeCena
- .Scenes
- graphify-project.py
- ObjetoCarro
- MapaInterativoUI
- .EncloseCourt
- Legenda
- BochaShot
- AudioSettingsController
- InteractionPrompt
- SantaRitaShallowStairs
- MonoBehaviour
- ZonaId
- .Paint
- HorizontalStepperNavigation
- PrologoVideo
- TileAleatorio
- EstradaScroller
- CamadaParallax
- LimpadorParabrisa
- MarcusGait
- Fase
- Capitulo2Interactable
- PayphoneCheckpoint
- c2-workstations.lua
- BochaCameraView
- c2-npc-work.lua
- marcus-flashlight.lua
- marcus-walk.lua
- pool-table.lua
- graphify-refresh.sh
- graphify-setup.sh
- graphify-validate.sh
- bocha-court-atlas.lua
- market-stock.lua
- SantaRitaMcpSetup

## God Nodes (most connected - your core abstractions)
1. `PlayerController` - 76 edges
2. `Capitulo1Director` - 71 edges
3. `SinucaMinigameUI` - 65 edges
4. `Chapter2RuntimeVerification` - 63 edges
5. `BochaCourtView` - 60 edges
6. `BochaMinigameUI` - 59 edges
7. `Chuva` - 53 edges
8. `InteracaoCapitulo1` - 49 edges
9. `MapaInterativoUI` - 49 edges
10. `BochaSimulation` - 42 edges

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

## Communities (84 total, 44 thin omitted)

### Community 1 - "unityengine"
Cohesion: 0.07
Nodes (5): SantaRitaArtImporter, BochaAI, UltimaDecisaoMs, UltimosCandidatos, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.06
Nodes (16): SantaRitaChapter1Builder, Capitulo1Director, EstaEmInterior, Instance, ZonaAtual, InfoZona, MovelLoja, CaixaMercado (+8 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.05
Nodes (19): GameMessageUI, Instance, IsShowing, EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota (+11 more)

### Community 4 - "BochaMatch"
Cohesion: 0.15
Nodes (12): Chapter2Verification, BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador, Vencedor (+4 more)

### Community 5 - "PlayerController"
Cohesion: 0.11
Nodes (7): PlayerController, DirecaoAtual, LanternaVisivel, QuadroCaminhada, SantaRitaExitThreshold, SantaRitaRoomCutaway, Opacity

### Community 6 - "Vector2"
Cohesion: 0.08
Nodes (9): SantaRitaChapter2Authoring, CameraFollow, LimiteMax, LimiteMin, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography, SantaRitaRoomLayout (+1 more)

### Community 7 - "Interactable"
Cohesion: 0.14
Nodes (7): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, TransicaoDeZonaTrigger, Prompt

### Community 8 - "Chuva"
Cohesion: 0.11
Nodes (4): Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.07
Nodes (11): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, BochaMapGraphic, Simulation (+3 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "RectTransform"
Cohesion: 0.15
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.15
Nodes (4): DialogoLinha, InteracaoCapitulo1, Prompt, Tipo

### Community 15 - "VideoSettingsController"
Cohesion: 0.11
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.16
Nodes (4): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine, SantaRitaPlaceDressing

### Community 17 - "BochaMinigameUI"
Cohesion: 0.15
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 19 - "PrologoDirector"
Cohesion: 0.15
Nodes (4): AudioAmbiente, Instance, FalaMarcada, PrologoDirector

### Community 20 - "Material"
Cohesion: 0.12
Nodes (5): SantaRitaActorElevation, VisualDepth, VisualScale, SantaRitaAtmosphere, SantaRitaRimLight

### Community 21 - "GameState"
Cohesion: 0.11
Nodes (11): IsolatedSaveScope, Fileira, EntradaDiario, Flags, GameState, LanternaLigada, ObjetivoAtual, TemProgressoNaoSalvo (+3 more)

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 23 - ".GarantirModalCriado"
Cohesion: 0.17
Nodes (3): BolaMesa, UIFactory, Font

### Community 25 - "Capitulo2Director"
Cohesion: 0.16
Nodes (7): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 26 - "SceneFader"
Cohesion: 0.15
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 27 - "BochaSimulation"
Cohesion: 0.13
Nodes (12): BochaPerspectiveVerification, BochaBall, Centro, Velocidade, BochaContact, BochaContactType, Bochas, Madeira (+4 more)

### Community 29 - "CheckpointSystem"
Cohesion: 0.13
Nodes (14): CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot, PrefKeyPrimary (+6 more)

### Community 31 - ".Scenes"
Cohesion: 0.15
Nodes (5): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification, Capitulo2Gate

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "MapaInterativoUI"
Cohesion: 0.16
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 35 - ".EncloseCourt"
Cohesion: 0.15
Nodes (6): Capitulo2Portal, Prompt, SantaRitaNpcWork, StationIndex, Working, Station

### Community 36 - "Legenda"
Cohesion: 0.23
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 37 - "BochaShot"
Cohesion: 0.23
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 38 - "AudioSettingsController"
Cohesion: 0.15
Nodes (3): AudioSettingsController, CamadaDeLuz, Nome

### Community 42 - "MonoBehaviour"
Cohesion: 0.13
Nodes (6): Capitulo2Perception, ChegadaEmSantaRita, ChuvaRolando, DentroDaCasaDoAvo, MainMenuActions, PayphoneSlotInputWatcher

### Community 43 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 48 - "EstradaScroller"
Cohesion: 0.39
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 54 - "PayphoneCheckpoint"
Cohesion: 0.06
Nodes (10): Camada, Capitulo2Audio, MenuItemSelector, PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, SantaRitaBarAudio (+2 more)

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 57 - "BochaCameraView"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

## Knowledge Gaps
- **193 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+188 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 343 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **44 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `.Ir`, `MonoBehaviour`, `ZonaId`, `.Marcar`, `Sprite`, `Material`, `PayphoneCheckpoint`, `.GarantirModalCriado`, `TilePiso`, `SceneFader`?**
  _High betweenness centrality (0.070) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _193 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.06721611721611721 - nodes in this community are weakly interconnected._
- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `Capitulo1Director`, `Vector2`, `MonoBehaviour`, `.Marcar`, `GameState`, `PayphoneCheckpoint`, `.GarantirModalCriado`?**
  _High betweenness centrality (0.065) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.0642593671131819 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `BochaMinigameUI` to `unityengine`, `Capitulo1Director`, `SinucaMinigameUI`, `BochaMatch`, `BochaShot`, `Vector2`, `BochaCourtView`, `MonoBehaviour`, `Fase`, `GameState`, `PayphoneCheckpoint`, `.GarantirModalCriado`, `BochaSimulation`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.053946053946053944 - nodes in this community are weakly interconnected._