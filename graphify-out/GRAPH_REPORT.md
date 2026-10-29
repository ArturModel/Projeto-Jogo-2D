# Graph Report - Projeto-Jogo-2D  (2026-10-29)

## Corpus Check
- 122 files · ~89,929 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: (none) 4, .toml 1, .csv 1)

## Summary
- 1591 nodes · 5120 edges · 79 communities (42 shown, 37 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 201 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `d057788f`
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
- SantaRitaChapter1Builder
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
- BochaSimulation.cs
- AudioSource
- BochaMapGraphic
- .Tem
- PayphoneCheckpoint
- graphify-project.py
- ObjetoCarro
- MapaInterativoUI
- SantaRitaNpcWork
- Legenda
- BochaShot
- HorizontalStepperNavigation
- .CreateCanvas
- Capitulo2Portal
- .Add
- MonoBehaviour
- ZonaId
- PrologoVideo
- TileAleatorio
- EstradaScroller
- SantaRitaBarAudio
- LimpadorParabrisa
- BochaAI
- Fase
- c2-workstations.lua
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

## Communities (79 total, 37 thin omitted)

### Community 0 - "IEnumerator"
Cohesion: 0.05
Nodes (25): Chapter2RuntimeVerification, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 1 - "unityengine"
Cohesion: 0.07
Nodes (3): SantaRitaArtImporter, SantaRitaMcpSetup, SantaRitaWorldLighting

### Community 2 - "Capitulo1Director"
Cohesion: 0.09
Nodes (14): Capitulo1Director, EstaEmInterior, Instance, PainelFechamentoAberto, ZonaAtual, InfoZona, MovelLoja, CaixaMercado (+6 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.05
Nodes (19): GameMessageUI, Instance, IsShowing, EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota (+11 more)

### Community 4 - "BochaMatch"
Cohesion: 0.13
Nodes (18): BochaPerspectiveVerification, Chapter2Verification, BochaHandResult, BochaMatch, Iniciador, Mao, Resolvida, UltimoLancador (+10 more)

### Community 5 - "PlayerController"
Cohesion: 0.06
Nodes (13): CameraFollow, LimiteMax, LimiteMin, MarcusGait, Quadro, PlayerController, DirecaoAtual, LanternaVisivel (+5 more)

### Community 6 - "Vector2"
Cohesion: 0.07
Nodes (13): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaChapter2Authoring, SantaRitaProductionVerification, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography (+5 more)

### Community 8 - "Chuva"
Cohesion: 0.07
Nodes (7): CamadaDeLuz, Nome, Camada, Chuva, Gota, Poca, Respingo

### Community 9 - "BochaCourtView"
Cohesion: 0.07
Nodes (13): BochaCameraView, Alvo, Lancamento, Superior, BochaCourtView, AimDistance, mainTexture, TrackCount (+5 more)

### Community 10 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "Image"
Cohesion: 0.16
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.18
Nodes (3): InteracaoCapitulo1, Prompt, Tipo

### Community 14 - "DicaDeTecla"
Cohesion: 0.21
Nodes (3): DicaDeTecla, Instance, Pedido

### Community 15 - "VideoSettingsController"
Cohesion: 0.09
Nodes (6): AudioSettingsController, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "SpriteRenderer"
Cohesion: 0.18
Nodes (3): SantaRitaLivingWorldAuthoring, SantaRitaForeground, SantaRitaNpcRoutine

### Community 17 - "BochaMinigameUI"
Cohesion: 0.13
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "Sprite"
Cohesion: 0.15
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, Fileira

### Community 19 - "PrologoDirector"
Cohesion: 0.15
Nodes (5): AudioAmbiente, Instance, Camada, FalaMarcada, PrologoDirector

### Community 20 - "Material"
Cohesion: 0.15
Nodes (4): SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaRimLight

### Community 21 - ".Tem"
Cohesion: 0.14
Nodes (3): ChegadaEmSantaRita, ItemDeCena, Prompt

### Community 22 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 25 - "Capitulo2Director"
Cohesion: 0.19
Nodes (5): C2ContentData, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 26 - "SceneFader"
Cohesion: 0.07
Nodes (10): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, InteractionPrompt, Instance, SceneFader (+2 more)

### Community 27 - "BochaSimulation.cs"
Cohesion: 0.16
Nodes (6): BochaContact, BochaContactType, Bochas, Madeira, Saibro, Evento

### Community 28 - "AudioSource"
Cohesion: 0.17
Nodes (4): Capitulo2Audio, MenuItemSelector, SantaRitaSound, EventsPlayed

### Community 29 - "BochaMapGraphic"
Cohesion: 0.22
Nodes (3): BochaMapGraphic, Simulation, SantaRitaShallowStairs

### Community 30 - ".Tem"
Cohesion: 0.35
Nodes (3): C2DialogueBranch, Disponivel, DialogoLinha

### Community 31 - "PayphoneCheckpoint"
Cohesion: 0.23
Nodes (4): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt

### Community 32 - "graphify-project.py"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "ObjetoCarro"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - "MapaInterativoUI"
Cohesion: 0.14
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 35 - "SantaRitaNpcWork"
Cohesion: 0.33
Nodes (4): SantaRitaNpcWork, StationIndex, Working, Station

### Community 36 - "Legenda"
Cohesion: 0.16
Nodes (6): CelularCarro, Disponivel, Fala, Legenda, Instance, RadioCarro

### Community 37 - "BochaShot"
Cohesion: 0.31
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 40 - "Capitulo2Portal"
Cohesion: 0.12
Nodes (6): Capitulo2Gate, Capitulo2Portal, Prompt, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 42 - "MonoBehaviour"
Cohesion: 0.11
Nodes (6): CamadaParallax, Capitulo2Perception, ChuvaRolando, DentroDaCasaDoAvo, MainMenuActions, PayphoneSlotInputWatcher

### Community 43 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 48 - "EstradaScroller"
Cohesion: 0.33
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 51 - "BochaAI"
Cohesion: 0.67
Nodes (3): BochaAI, UltimaDecisaoMs, UltimosCandidatos

### Community 52 - "Fase"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 55 - "c2-workstations.lua"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 63 - "pool-table.lua"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

## Knowledge Gaps
- **195 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+190 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 349 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **37 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `IEnumerator`, `unityengine`, `Vector2`, `Chuva`, `MonoBehaviour`, `Image`, `RuntimeInitializeOnLoadMethod`, `RectTransform`, `CanvasGroup`?**
  _High betweenness centrality (0.086) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _195 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `IEnumerator` be split into smaller, more focused modules?**
  _Cohesion score 0.0542571931884909 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `unityengine`, `PlayerController`, `SantaRitaChapter1Builder`, `Capitulo2Portal`, `MonoBehaviour`, `ZonaId`, `RuntimeInitializeOnLoadMethod`, `PrologoVideo`, `Sprite`, `.Tem`, `TilePiso`, `RectTransform`, `CanvasGroup`, `SceneFader`, `AudioSource`, `.Tem`, `PayphoneCheckpoint`?**
  _High betweenness centrality (0.075) - this node is a cross-community bridge._
- **Should `unityengine` be split into smaller, more focused modules?**
  _Cohesion score 0.06684636118598383 - nodes in this community are weakly interconnected._
- **Why does `PlayerController` connect `PlayerController` to `IEnumerator`, `unityengine`, `MapaInterativoUI`, `Capitulo1Director`, `Vector2`, `Capitulo2Portal`, `Chuva`, `MonoBehaviour`, `SantaRitaC2Consolidation`, `SpriteRenderer`, `Sprite`, `Material`, `Capitulo2Director`, `SceneFader`, `PayphoneCheckpoint`?**
  _High betweenness centrality (0.071) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.09336016096579478 - nodes in this community are weakly interconnected._