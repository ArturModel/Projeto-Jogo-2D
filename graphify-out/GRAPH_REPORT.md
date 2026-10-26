# Graph Report - Projeto-Jogo-2D  (2026-10-26)

## Corpus Check
- 121 files · ~85,522 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 9 file(s) not represented in the graph (top: (none) 3, .toml 1, .csv 1)

## Summary
- 1529 nodes · 4864 edges · 83 communities (40 shown, 43 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 214 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `86b7c02d`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- IEnumerator
- unityengine
- Capitulo1Director
- SinucaMinigameUI
- BochaSimulation
- PlayerController
- Vector2
- MapaInterativoUI
- Chuva
- BochaCourtView
- TipoInteracao
- SantaRitaC2Consolidation
- RectTransform
- InteracaoCapitulo1
- VideoSettingsController
- SpriteRenderer
- BochaMinigameUI
- Sprite
- PrologoDirector
- Material
- GameMessageUI
- TilePiso
- .GarantirModalCriado
- CanvasGroup
- Capitulo2Director
- SceneFader
- Capitulo2Portal
- PayphoneCheckpoint
- AudioSource
- Interactable
- .Scenes
- graphify-project.py
- ObjetoCarro
- AudioSettingsController
- .Exteriors
- Legenda
- InteractionPrompt
- CamadaDeLuz
- BochaMapGraphic
- .Dizer
- MonoBehaviour
- ZonaId
- BochaShot
- MenuItemSelector
- PrologoVideo
- TileAleatorio
- EstradaScroller
- LimpadorParabrisa
- Fase
- MainMenuActions
- SantaRitaSound
- c2-workstations.lua
- CamadaParallax
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

## Communities (83 total, 43 thin omitted)

### Community 0 - "IEnumerator"
Cohesion: 0.06
Nodes (25): Chapter2RuntimeVerification, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 1 - "unityengine"
Cohesion: 0.07
Nodes (3): SantaRitaArtImporter, SantaRitaMcpSetup, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.06
Nodes (17): SantaRitaChapter1Builder, Capitulo1Director, EstaEmInterior, Instance, ZonaAtual, InfoZona, MovelLoja, CaixaMercado (+9 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.07
Nodes (17): BolaMesa, EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo (+9 more)

### Community 4 - "BochaSimulation"
Cohesion: 0.08
Nodes (27): BochaPerspectiveVerification, Chapter2Verification, BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaHandResult, BochaMatch, Iniciador (+19 more)

### Community 5 - "PlayerController"
Cohesion: 0.05
Nodes (14): CameraFollow, LimiteMax, LimiteMin, MarcusGait, Quadro, PlayerController, DirecaoAtual, LanternaVisivel (+6 more)

### Community 6 - "Vector2"
Cohesion: 0.11
Nodes (6): SantaRitaChapter2Authoring, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 7 - "MapaInterativoUI"
Cohesion: 0.11
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 8 - "Chuva"
Cohesion: 0.10
Nodes (5): Camada, Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.11
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "RectTransform"
Cohesion: 0.17
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.16
Nodes (4): DialogoLinha, InteracaoCapitulo1, Prompt, Tipo

### Community 15 - "VideoSettingsController"
Cohesion: 0.11
Nodes (5): SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.19
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaPlaceDressing

### Community 17 - "BochaMinigameUI"
Cohesion: 0.18
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.15
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, Fileira

### Community 19 - "PrologoDirector"
Cohesion: 0.14
Nodes (5): AudioAmbiente, Instance, Camada, FalaMarcada, PrologoDirector

### Community 20 - "Material"
Cohesion: 0.15
Nodes (4): SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaRimLight

### Community 21 - "GameMessageUI"
Cohesion: 0.20
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 25 - "Capitulo2Director"
Cohesion: 0.16
Nodes (7): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 26 - "SceneFader"
Cohesion: 0.20
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 27 - "Capitulo2Portal"
Cohesion: 0.12
Nodes (6): Capitulo2Gate, Capitulo2Portal, Prompt, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 28 - "PayphoneCheckpoint"
Cohesion: 0.20
Nodes (4): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt

### Community 30 - "Interactable"
Cohesion: 0.14
Nodes (7): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, ItemDeCena, Prompt

### Community 31 - ".Scenes"
Cohesion: 0.22
Nodes (4): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 35 - ".Exteriors"
Cohesion: 0.19
Nodes (5): SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex, Working, Station

### Community 36 - "Legenda"
Cohesion: 0.23
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 41 - ".Dizer"
Cohesion: 0.22
Nodes (3): CelularCarro, Disponivel, ChegadaEmSantaRita

### Community 42 - "MonoBehaviour"
Cohesion: 0.22
Nodes (4): Capitulo2Perception, ChuvaRolando, DentroDaCasaDoAvo, PayphoneSlotInputWatcher

### Community 43 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 44 - "BochaShot"
Cohesion: 0.27
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 48 - "EstradaScroller"
Cohesion: 0.39
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

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
  These have ≤1 connection - possible missing edges. (Counts symbols only; 345 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **43 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `Capitulo1Director`, `Vector2`, `MapaInterativoUI`, `Chuva`, `MonoBehaviour`, `RectTransform`, `.Marcar`, `RuntimeInitializeOnLoadMethod`, `GameMessageUI`?**
  _High betweenness centrality (0.085) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _193 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.05896358543417367 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `InteractionPrompt`, `MapaInterativoUI`, `.Dizer`, `MonoBehaviour`, `ZonaId`, `.Marcar`, `RuntimeInitializeOnLoadMethod`, `Sprite`, `TilePiso`, `Capitulo2Portal`, `PayphoneCheckpoint`, `AudioSource`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.06910336950314107 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `BochaMinigameUI` to `unityengine`, `Capitulo1Director`, `SinucaMinigameUI`, `BochaSimulation`, `Vector2`, `MapaInterativoUI`, `Chuva`, `BochaCourtView`, `MonoBehaviour`, `BochaMapGraphic`, `.CancelarCarga`, `BochaShot`, `RectTransform`, `Fase`, `GameMessageUI`, `PayphoneCheckpoint`, `AudioSource`?**
  _High betweenness centrality (0.066) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.0627062706270627 - nodes in this community are weakly interconnected._