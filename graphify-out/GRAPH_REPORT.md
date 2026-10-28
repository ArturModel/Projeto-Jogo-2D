# Graph Report - Projeto-Jogo-2D  (2026-10-28)

## Corpus Check
- 121 files · ~86,072 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 9 file(s) not represented in the graph (top: (none) 3, .toml 1, .csv 1)

## Summary
- 1535 nodes · 4901 edges · 83 communities (42 shown, 41 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 214 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `d1b3a4da`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- InteracaoCapitulo1
- .CreateCanvas
- Capitulo1Director
- SinucaMinigameUI
- PlayerController
- PlayerController
- manifest.json
- dependencies
- Chuva
- MonoBehaviour
- MenuPausa
- com.unity.burst
- packages-lock.json
- InteracaoCapitulo1
- SceneFader
- OptionsPanelController
- CheckpointSystem
- com.unity.modules.unitywebrequest
- com.unity.modules.physics
- GameState
- GameMessageUI
- CamadaDeLuz
- RectTransform
- com.unity.modules.jsonserialize
- Capitulo2Director
- com.unity.modules.imgui
- SantaRitaActorElevation
- com.unity.modules.audio
- SantaRitaBarAudio
- Interactable
- VideoSettingsController
- com.unity.settings-manager
- PrologoDirector
- .Dizer
- SantaRitaNpcRoutine
- Legenda
- EstadoPartida
- AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente)
- com.unity.2d.tilemap
- Legenda
- InteractionPrompt
- .SaveSlot
- com.unity.ext.nunit
- com.unity.modules.tetgen
- EstadoPartida
- dependencies
- com.unity.mathematics
- com.unity.collab-proxy
- TileAleatorio
- AudioSource
- Plano de Implementação — Setup Graphify + Obsidian + Antigravity
- com.unity.2d.tooling
- com.unity.modules.accessibility
- graphify-setup.ps1
- com.unity.modules.umbra
- Cadeia de Investigação e Pistas do Capítulo 1
- Manual de Operação e Governança — Graphify + Obsidian + Agentes de IA
- graphify-validate.ps1
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

## Communities (83 total, 41 thin omitted)

### Community 0 - "InteracaoCapitulo1"
Cohesion: 0.05
Nodes (25): Chapter2RuntimeVerification, CheckpointSystem, BackupFilePath, CheckpointScene, EmArmazenamentoDeTeste, HasCheckpoint, PrefKeyBackup, PrefKeyLastSlot (+17 more)

### Community 2 - "Capitulo1Director"
Cohesion: 0.07
Nodes (17): SantaRitaChapter1Builder, Capitulo1Director, EstaEmInterior, Instance, ZonaAtual, InfoZona, MovelLoja, CaixaMercado (+9 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.11
Nodes (4): SinucaMinigameUI, Instance, IsShowing, MaoEmCurso

### Community 4 - "PlayerController"
Cohesion: 0.08
Nodes (27): BochaPerspectiveVerification, Chapter2Verification, BochaAI, UltimaDecisaoMs, UltimosCandidatos, BochaHandResult, BochaMatch, Iniciador (+19 more)

### Community 5 - "PlayerController"
Cohesion: 0.06
Nodes (13): CameraFollow, LimiteMax, LimiteMin, MarcusGait, Quadro, PlayerController, DirecaoAtual, LanternaVisivel (+5 more)

### Community 6 - "manifest.json"
Cohesion: 0.11
Nodes (6): SantaRitaChapter2Authoring, OlharPrimeiraPessoa, Deslocamento, SantaRitaGeography, SantaRitaRoomLayout, SceneTransition

### Community 7 - "dependencies"
Cohesion: 0.12
Nodes (4): MapaInterativoUI, Instance, IsShowing, PontoMapa

### Community 8 - "Chuva"
Cohesion: 0.10
Nodes (5): Camada, Chuva, Gota, Poca, Respingo

### Community 9 - "MonoBehaviour"
Cohesion: 0.08
Nodes (9): BochaCourtView, AimDistance, mainTexture, TrackCount, View, Zoom, Face, Track (+1 more)

### Community 10 - "MenuPausa"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 12 - "packages-lock.json"
Cohesion: 0.15
Nodes (3): MenuPausa, Aberto, ReduzirEfeitosIntensos

### Community 13 - "InteracaoCapitulo1"
Cohesion: 0.16
Nodes (4): DialogoLinha, InteracaoCapitulo1, Prompt, Tipo

### Community 15 - "SceneFader"
Cohesion: 0.09
Nodes (6): AudioSettingsController, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho, VideoSettingsController

### Community 16 - "OptionsPanelController"
Cohesion: 0.20
Nodes (4): SantaRitaLivingWorldAuthoring, Capitulo2Portal, Prompt, SantaRitaForeground

### Community 17 - "CheckpointSystem"
Cohesion: 0.20
Nodes (3): BochaMinigameUI, Instance, IsShowing

### Community 18 - "com.unity.modules.unitywebrequest"
Cohesion: 0.17
Nodes (4): SantaRitaArtProduction, Capitulo2Interactable, Prompt, Fileira

### Community 19 - "com.unity.modules.physics"
Cohesion: 0.14
Nodes (4): AudioAmbiente, Instance, FalaMarcada, PrologoDirector

### Community 20 - "GameState"
Cohesion: 0.15
Nodes (4): SantaRitaRuralPaths, Stretch, SantaRitaAtmosphere, SantaRitaRimLight

### Community 21 - "GameMessageUI"
Cohesion: 0.20
Nodes (3): GameMessageUI, Instance, IsShowing

### Community 22 - "CamadaDeLuz"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 23 - "RectTransform"
Cohesion: 0.11
Nodes (5): InteractionPrompt, Instance, BolaMesa, UIFactory, Font

### Community 25 - "Capitulo2Director"
Cohesion: 0.16
Nodes (7): C2ContentData, C2DialogueBranch, Disponivel, C2InteractionData, Capitulo2Director, Instance, ProntoParaBocha

### Community 26 - "com.unity.modules.imgui"
Cohesion: 0.20
Nodes (3): SceneFader, Instance, IsTransitioning

### Community 27 - "SantaRitaActorElevation"
Cohesion: 0.16
Nodes (4): Capitulo2Gate, SantaRitaActorElevation, VisualDepth, VisualScale

### Community 28 - "com.unity.modules.audio"
Cohesion: 0.20
Nodes (5): PayphoneCheckpoint, IsSaveUIOpen, PayphoneId, Prompt, PayphoneSlotInputWatcher

### Community 30 - "Interactable"
Cohesion: 0.14
Nodes (7): DoorTrigger, Prompt, Interactable, InteractionRadius, Prompt, ItemDeCena, Prompt

### Community 31 - "VideoSettingsController"
Cohesion: 0.21
Nodes (4): Chapter2BuildVerification, Evidence, ForcarDevelopment, SantaRitaProductionVerification

### Community 32 - "com.unity.settings-manager"
Cohesion: 0.26
Nodes (6): command(), fingerprint(), main(), run(), sources(), validate()

### Community 33 - "PrologoDirector"
Cohesion: 0.16
Nodes (6): ObjetoCarro, Apontado, Disponivel, PonteiroCarro, Ativo, PortaLuvasCarro

### Community 34 - ".Dizer"
Cohesion: 0.18
Nodes (3): CelularCarro, Disponivel, ChegadaEmSantaRita

### Community 35 - "SantaRitaNpcRoutine"
Cohesion: 0.14
Nodes (6): SantaRitaNpcRoutine, SantaRitaNpcWork, StationIndex, Working, Station, SantaRitaPlaceDressing

### Community 36 - "Legenda"
Cohesion: 0.23
Nodes (4): Fala, Legenda, Instance, RadioCarro

### Community 37 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 42 - "Legenda"
Cohesion: 0.12
Nodes (5): CamadaParallax, Capitulo2Perception, ChuvaRolando, DentroDaCasaDoAvo, SantaRitaShallowStairs

### Community 43 - "InteractionPrompt"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 44 - ".SaveSlot"
Cohesion: 0.21
Nodes (6): BochaLaunch, BochaShot, BochaThrowType, Ponto, Rafa, Volo

### Community 48 - "dependencies"
Cohesion: 0.39
Nodes (3): EstradaScroller, Curvatura, Velocidade

### Community 52 - "com.unity.collab-proxy"
Cohesion: 0.33
Nodes (6): Fase, Carregando, Mira, Resultado, Rolagem, Tonando

### Community 54 - "AudioSource"
Cohesion: 0.21
Nodes (4): Camada, Capitulo2Audio, SantaRitaSound, EventsPlayed

### Community 55 - "Plano de Implementação — Setup Graphify + Obsidian + Antigravity"
Cohesion: 0.53
Nodes (4): box(), c(), rect(), table()

### Community 57 - "com.unity.2d.tooling"
Cohesion: 0.50
Nodes (4): BochaCameraView, Alvo, Lancamento, Superior

### Community 63 - "Cadeia de Investigação e Pistas do Capítulo 1"
Cohesion: 0.83
Nodes (3): color(), dot(), rect()

## Knowledge Gaps
- **193 isolated node(s):** `ForcarDevelopment`, `Evidence`, `Instance`, `Nome`, `LimiteMin` (+188 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 344 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **41 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SinucaMinigameUI` connect `SinucaMinigameUI` to `InteracaoCapitulo1`, `.CreateCanvas`, `Capitulo1Director`, `EstadoPartida`, `manifest.json`, `dependencies`, `Chuva`, `Legenda`, `packages-lock.json`, `.CreateLabel`, `GameMessageUI`, `RectTransform`?**
  _High betweenness centrality (0.073) - this node is a cross-community bridge._
- **What connects `ForcarDevelopment`, `Evidence`, `Instance` to the rest of the system?**
  _193 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `InteracaoCapitulo1` be split into smaller, more focused modules?**
  _Cohesion score 0.05462134764460346 - nodes in this community are weakly interconnected._
- **Why does `Capitulo1Director` connect `Capitulo1Director` to `.CreateCanvas`, `.Dizer`, `manifest.json`, `Legenda`, `InteractionPrompt`, `.Marcar`, `.CreateLabel`, `com.unity.modules.unitywebrequest`, `AudioSource`, `RectTransform`, `Scene`, `CamadaDeLuz`, `com.unity.modules.audio`, `SantaRitaBarAudio`?**
  _High betweenness centrality (0.070) - this node is a cross-community bridge._
- **Should `.CreateCanvas` be split into smaller, more focused modules?**
  _Cohesion score 0.07188118811881188 - nodes in this community are weakly interconnected._
- **Why does `BochaMinigameUI` connect `CheckpointSystem` to `.CreateCanvas`, `Capitulo1Director`, `PlayerController`, `manifest.json`, `dependencies`, `Chuva`, `MonoBehaviour`, `Legenda`, `com.unity.2d.tilemap`, `packages-lock.json`, `.SaveSlot`, `.Construir`, `.CancelarCarga`, `com.unity.collab-proxy`, `GameMessageUI`, `AudioSource`, `RectTransform`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.06679035250463822 - nodes in this community are weakly interconnected._