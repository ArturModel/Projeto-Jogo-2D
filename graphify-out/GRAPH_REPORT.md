# Graph Report - Projeto-Jogo-2D  (2026-10-29)

## Corpus Check
- 122 files · ~88,965 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 1581 nodes · 5089 edges · 76 communities (40 shown, 36 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 201 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `60ff6c52`
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
- GameMessageUI
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
- .Tem
- TilePiso
- RectTransform
- CanvasGroup
- Capitulo2Director
- SceneFader
- PayphoneCheckpoint
- BochaMapGraphic
- .Dizer
- .Scenes
- graphify-project.py
- ObjetoCarro
- MapaInterativoUI
- .Exteriors
- Legenda
- BochaShot
- MainMenuActions
- .CreateCanvas
- Capitulo2Portal
- .ApplyC1
- MonoBehaviour
- ZonaId
- PrologoVideo
- TileAleatorio
- EstradaScroller
- LimpadorParabrisa
- Fase
- c2-workstations.lua
- .Construir
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
1. `Capitulo1Director` - 78 edges
2. `PlayerController` - 76 edges
3. `SinucaMinigameUI` - 65 edges
4. `Chapter2RuntimeVerification` - 63 edges
5. `InteracaoCapitulo1` - 61 edges
6. `BochaCourtView` - 60 edges
7. `BochaMinigameUI` - 59 edges
8. `Chuva` - 53 edges
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

## Communities (76 total, 36 thin omitted)

### Community 0 - "IEnumerator"
Cohesion: 0.06
Nodes (25): Chapter2RuntimeVerification, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 1 - "unityengine"
Cohesion: 0.07
Nodes (3): SantaRitaArtImporter, SantaRitaMcpSetup, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.06
Nodes (18): SantaRitaChapter1Builder, Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona, MovelLoja (+10 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.07
Nodes (16): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+8 more)

### Community 4 - "BochaMatch"
Cohesion: 0.08
Nodes (27): BochaPerspectiveVerification, Chapter2Verification, BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaHandResult, BochaMatch, Iniciador (+19 more)

### Community 5 - "PlayerController"
Cohesion: 0.06
Nodes (13): CameraFollow, LimiteMax, LimiteMin, MarcusGait, Quadro, PlayerController, DirecaoAtual, LanternaVisivel (+5 more)

### Community 6 - "Vector2"
Cohesion: 0.10
Nodes (6): SantaRitaChapter2Authoring, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 7 - "GameMessageUI"
Cohesion: 0.16
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 8 - "Chuva"
Cohesion: 0.07
Nodes (7): CamadaDeLuz, Nome, Camada, Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.08
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.16
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.16
Nodes (3): InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.09
Nodes (6): AudioSettingsController, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 17 - "BochaMinigameUI"
Cohesion: 0.27
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.18
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, Fileira

### Community 19 - "PrologoDirector"
Cohesion: 0.16
Nodes (4): AudioAmbiente, Instance, FalaMarcada, PrologoDirector

### Community 20 - "Material"
Cohesion: 0.15
Nodes (4): SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaRimLight

### Community 21 - ".Tem"
Cohesion: 0.11
Nodes (3): ChegadaEmSantaRita, ItemDeCena, Prompt

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 25 - "Capitulo2Director"
Cohesion: 0.18
Nodes (8): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha, DialogoLinha

### Community 26 - "SceneFader"
Cohesion: 0.06
Nodes (10): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance, SceneFader (+2 more)

### Community 28 - "PayphoneCheckpoint"
Cohesion: 0.06
Nodes (12): Camada, Capitulo2Audio, MenuItemSelector, HorizontalStepperNavigation, PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt (+4 more)

### Community 29 - "BochaMapGraphic"
Cohesion: 0.20
Nodes (3): BochaMapGraphic, Simulation, SantaRitaShallowStairs

### Community 31 - ".Scenes"
Cohesion: 0.14
Nodes (6): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification, Capitulo2Portal, Prompt

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "MapaInterativoUI"
Cohesion: 0.14
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 35 - ".Exteriors"
Cohesion: 0.19
Nodes (5): SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex, Working, Station

### Community 36 - "Legenda"
Cohesion: 0.25
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 37 - "BochaShot"
Cohesion: 0.21
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 40 - "Capitulo2Portal"
Cohesion: 0.22
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 42 - "MonoBehaviour"
Cohesion: 0.15
Nodes (4): CamadaParallax, Capitulo2Perception, ChuvaRolando, DentroDaCasaDoAvo

### Community 43 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 48 - "EstradaScroller"
Cohesion: 0.33
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 57 - ".Construir"
Cohesion: 0.24
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

## Knowledge Gaps
- **195 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+190 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 349 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **36 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `unityengine`, `Capitulo1Director`, `Vector2`, `Chuva`, `MonoBehaviour`, `Image`, `.Tem`, `RectTransform`, `SceneFader`?**
  _High betweenness centrality (0.084) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _195 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.06225741344511722 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `PlayerController`, `Vector2`, `.CreateCanvas`, `MonoBehaviour`, `ZonaId`, `Sprite`, `.Tem`, `TilePiso`, `RectTransform`, `SceneFader`, `PayphoneCheckpoint`, `.Dizer`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.06577323223417386 - nodes in this community are weakly interconnected._
- **Why does `PlayerController` connect `PlayerController` to `IEnumerator`, `unityengine`, `MapaInterativoUI`, `Capitulo1Director`, `.Exteriors`, `Vector2`, `Chuva`, `Capitulo2Portal`, `MonoBehaviour`, `SantaRitaC2Consolidation`, `SpriteRenderer`, `Sprite`, `Material`, `Capitulo2Director`, `SceneFader`, `PayphoneCheckpoint`, `.Scenes`?**
  _High betweenness centrality (0.061) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.062123859444768004 - nodes in this community are weakly interconnected._