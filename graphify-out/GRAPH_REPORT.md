# Graph Report - Projeto-Jogo-2D  (2026-10-26)

## Corpus Check
- 61 files · ~65,349 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 9 file(s) not represented in the graph (top: (none) 3, .csv 1, .mixer 1)

## Summary
- 1639 nodes · 3077 edges · 69 communities (59 shown, 10 thin omitted)
- Extraction: 98% EXTRACTED · 2% INFERRED · 0% AMBIGUOUS · INFERRED: 62 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `56f4ccb0`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- InteracaoCapitulo1
- .CreateCanvas
- Capitulo1Director
- SinucaMinigameUI
- PlayerController
- MapaInterativoUI
- manifest.json
- dependencies
- Chuva
- MonoBehaviour
- MenuPausa
- com.unity.burst
- packages-lock.json
- com.unity.2d.common
- TipoInteracao
- SceneFader
- OptionsPanelController
- CheckpointSystem
- com.unity.modules.unitywebrequest
- com.unity.modules.physics
- GameState
- GameMessageUI
- CamadaDeLuz
- com.unity.test-framework
- com.unity.modules.jsonserialize
- TilePiso
- com.unity.modules.imgui
- unityengine
- com.unity.modules.audio
- MainMenuActions
- PayphoneCheckpoint
- VideoSettingsController
- com.unity.settings-manager
- PrologoDirector
- com.unity.modules.ui
- PrologoVideo
- MenuItemSelector
- AudioSettingsController
- AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente)
- SubtitleSettingsController
- com.unity.2d.tilemap
- com.unity.modules.imageconversion
- Legenda
- InteractionPrompt
- .SaveSlot
- com.unity.ext.nunit
- com.unity.modules.tetgen
- EstadoPartida
- dependencies
- .CreateLabel
- com.unity.mathematics
- ZonaId
- com.unity.collab-proxy
- TileAleatorio
- .AoClicarSlot
- Plano de Implementação — Setup Graphify + Obsidian + Antigravity
- MovelLoja
- com.unity.2d.tooling
- com.unity.modules.accessibility
- dependencies
- com.unity.modules.umbra
- Cadeia de Investigação e Pistas do Capítulo 1
- Manual de Operação e Governança — Graphify + Obsidian + Agentes de IA
- graphify-refresh.sh script
- graphify-setup.sh script
- graphify-validate.sh script

## God Nodes (most connected - your core abstractions)
1. `Capitulo1Director` - 72 edges
2. `SinucaMinigameUI` - 61 edges
3. `Chuva` - 49 edges
4. `MapaInterativoUI` - 49 edges
5. `InteracaoCapitulo1` - 47 edges
6. `TipoInteracao` - 36 edges
7. `MenuPausa` - 34 edges
8. `PayphoneCheckpoint` - 34 edges
9. `PlayerController` - 33 edges
10. `DialogoLinha` - 32 edges

## Surprising Connections (you probably didn't know these)
- `7. Critérios de Aceite e Resultados dos Testes (Fase 7)` --references--> `PlayerController`  [INFERRED]
  docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md → Projeto Jogo 2D/Assets/Scripts/PlayerController.cs
- `2. Regras Inegociáveis de Domínio do Jogo (Revisão Canônica R1)` --references--> `CameraFollow`  [INFERRED]
  AGENTS.md → Projeto Jogo 2D/Assets/Scripts/CameraFollow.cs
- `Escala Visual e Pixel Perfect (PPU 32, Canvas 640x360)` --conceptually_related_to--> `CameraFollow`  [INFERRED]
  Docs/SANTA_RITA_PROJECT_AUDIT.md → Projeto Jogo 2D/Assets/Scripts/CameraFollow.cs
- `2. Regras Inegociáveis de Domínio do Jogo (Revisão Canônica R1)` --references--> `GameStateSnapshot`  [INFERRED]
  AGENTS.md → Projeto Jogo 2D/Assets/Scripts/GameState.cs
- `Contrato de Save Manual Exclusivo no Orelhão` --references--> `PayphoneCheckpoint`  [EXTRACTED]
  Docs/SANTA_RITA_PROJECT_AUDIT.md → Projeto Jogo 2D/Assets/Scripts/PayphoneCheckpoint.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Núcleo de Persistência Manual e Estado em RAM** — docs_santa_rita_project_audit_save_manual_orelhao, docs_santa_rita_vertical_slice_report_gamestate_ram, projeto_jogo_2d_assets_scripts_checkpointsystem_checkpointsystem, projeto_jogo_2d_assets_scripts_gamestate_gamestate, projeto_jogo_2d_assets_scripts_payphonecheckpoint_payphonecheckpoint [EXTRACTED 1.00]
- **Sistemas e Regras do Confronto com Zé (Sinuca)** — docs_santa_rita_project_audit_sinuca_rules, docs_santa_rita_vertical_slice_report_sinuca_invariants, projeto_jogo_2d_assets_scripts_sinucaminigameui_sinucaminigameui [EXTRACTED 1.00]

## Communities (69 total, 10 thin omitted)

### Community 0 - "InteracaoCapitulo1"
Cohesion: 0.07
Nodes (14): CelularCarro, Disponivel, IEnumerator, Collider2D, IEnumerator, InteracaoCapitulo1, Prompt, Tipo (+6 more)

### Community 1 - ".CreateCanvas"
Cohesion: 0.16
Nodes (11): Escala Visual e Pixel Perfect (PPU 32, Canvas 640x360), Canvas, CanvasScaler, Image, Sprite, Transform, Vector2, UIFactory (+3 more)

### Community 2 - "Capitulo1Director"
Cohesion: 0.10
Nodes (27): Func, InfoZona, MovelLoja, Capitulo1Director, EstaEmInterior, Instance, ZonaAtual, AudioClip (+19 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.08
Nodes (24): BolaMesa, Regras da Sinuca Autoral: A Bola Marcada (Melhor de 3), Invariantes de Física e Pontuação da Sinuca, EstadoPartida, BolaMesa, Action, Button, Color (+16 more)

### Community 4 - "PlayerController"
Cohesion: 0.05
Nodes (28): Animator, CameraFollow, LimiteMax, LimiteMin, BoxCollider2D, Camera, SpriteRenderer, Transform (+20 more)

### Community 5 - "MapaInterativoUI"
Cohesion: 0.08
Nodes (27): Sistema de Mapa Interativo (Tecla M / Caderno de Bolso), Diário Investigativo e Abas de Fichas do Clube, PontoMapa, Button, Canvas, Color, Dictionary, GameObject (+19 more)

### Community 6 - "manifest.json"
Cohesion: 0.04
Nodes (54): com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.director, com.unity.modules.imageconversion, com.unity.modules.imgui (+46 more)

### Community 7 - "dependencies"
Cohesion: 0.04
Nodes (55): dependencies, com.unity.2d.animation, com.unity.2d.aseprite, com.unity.2d.psdimporter, com.unity.2d.sprite, com.unity.2d.spriteshape, com.unity.2d.tilemap, com.unity.2d.tilemap.extras (+47 more)

### Community 8 - "Chuva"
Cohesion: 0.09
Nodes (24): AudioLowPassFilter, AudioMixerGroup, Camada, Gota, NativeArray, PixelPerfectCamera, Poca, Camada (+16 more)

### Community 9 - "MonoBehaviour"
Cohesion: 0.06
Nodes (29): Fluxo Narrativo e Cênico do Capítulo 1, MenuItem, MonoBehaviour, BoxCollider2D, Color, GameObject, Material, Sprite (+21 more)

### Community 10 - "MenuPausa"
Cohesion: 0.10
Nodes (26): EventSystem, Graphic, InputSystemUIInputModule, Action, AudioClip, AudioMixer, AudioSource, Button (+18 more)

### Community 11 - "com.unity.burst"
Cohesion: 0.05
Nodes (42): dependencies, depth, source, version, dependencies, depth, source, version (+34 more)

### Community 12 - "packages-lock.json"
Cohesion: 0.05
Nodes (39): com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.director, com.unity.modules.imageconversion, com.unity.modules.imgui (+31 more)

### Community 13 - "com.unity.2d.common"
Cohesion: 0.06
Nodes (38): dependencies, depth, source, url, version, dependencies, depth, source (+30 more)

### Community 14 - "TipoInteracao"
Cohesion: 0.06
Nodes (36): TipoInteracao, BalcaoBarDoZe, BombaCombustivelPosto, CadernoFiadoMercadinho, CaixaCorreioAvo, CaminhoneteClubeBar, CarroEstacionadoPosto, CarroMarcus (+28 more)

### Community 15 - "SceneFader"
Cohesion: 0.12
Nodes (13): Action, Canvas, CanvasGroup, CanvasScaler, Coroutine, IEnumerator, Image, LoadSceneMode (+5 more)

### Community 16 - "OptionsPanelController"
Cohesion: 0.14
Nodes (12): CanvasGroup, GameObject, IEnumerator, RectTransform, Vector2, MenuPanelTransition, CanvasGroup, GameObject (+4 more)

### Community 17 - "CheckpointSystem"
Cohesion: 0.21
Nodes (8): Contrato de Save Manual Exclusivo no Orelhão, CheckpointSystem, BackupFilePath, CheckpointScene, HasCheckpoint, PrimaryFilePath, UltimoSlotUsado, ManualSaveData

### Community 18 - "com.unity.modules.unitywebrequest"
Cohesion: 0.09
Nodes (25): dependencies, depth, source, version, dependencies, depth, source, version (+17 more)

### Community 19 - "com.unity.modules.physics"
Cohesion: 0.08
Nodes (25): dependencies, depth, source, version, dependencies, depth, source, version (+17 more)

### Community 20 - "GameState"
Cohesion: 0.16
Nodes (13): Arquitetura de Estado em RAM na Sessão (GameState), HashSet, IReadOnlyList, Dictionary, List, EntradaDiario, Flags, GameState (+5 more)

### Community 21 - "GameMessageUI"
Cohesion: 0.14
Nodes (14): Action, CanvasGroup, Coroutine, Dictionary, GameObject, IEnumerator, Image, List (+6 more)

### Community 22 - "CamadaDeLuz"
Cohesion: 0.23
Nodes (4): CamadaDeLuz, Nome, Light2D, List

### Community 23 - "com.unity.test-framework"
Cohesion: 0.10
Nodes (22): dependencies, dependencies, depth, source, url, version, dependencies, depth (+14 more)

### Community 24 - "com.unity.modules.jsonserialize"
Cohesion: 0.10
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 25 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 26 - "com.unity.modules.imgui"
Cohesion: 0.10
Nodes (20): dependencies, depth, source, url, version, dependencies, depth, source (+12 more)

### Community 27 - "unityengine"
Cohesion: 0.05
Nodes (36): LayerMask, Color, SpriteRenderer, ObjetoCarro, Apontado, Disponivel, Camera, PonteiroCarro (+28 more)

### Community 28 - "com.unity.modules.audio"
Cohesion: 0.11
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 30 - "PayphoneCheckpoint"
Cohesion: 0.10
Nodes (18): PayphoneSlotInputWatcher, AudioClip, AudioSource, Button, Color, GameObject, GraphicRaycaster, Image (+10 more)

### Community 31 - "VideoSettingsController"
Cohesion: 0.18
Nodes (8): List, RawImage, Slider, TMP_Dropdown, TMP_Text, Toggle, VideoSettingsController, Resolution

### Community 32 - "com.unity.settings-manager"
Cohesion: 0.12
Nodes (17): dependencies, depth, source, version, dependencies, depth, source, url (+9 more)

### Community 33 - "PrologoDirector"
Cohesion: 0.06
Nodes (23): FalaMarcada, Fileira, AudioAmbiente, Instance, Camada, AudioClip, AudioSource, Dictionary (+15 more)

### Community 34 - "com.unity.modules.ui"
Cohesion: 0.12
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, version (+8 more)

### Community 35 - "PrologoVideo"
Cohesion: 0.17
Nodes (10): AspectRatioFitter, Cânone da Linha do Tempo (1990, 1998, 2004), Fala, IEnumerator, Image, RawImage, Fala, PrologoVideo (+2 more)

### Community 36 - "MenuItemSelector"
Cohesion: 0.18
Nodes (10): BaseEventData, IPointerEnterHandler, ISelectHandler, PointerEventData, AudioClip, AudioSource, GameObject, RectTransform (+2 more)

### Community 37 - "AudioSettingsController"
Cohesion: 0.28
Nodes (4): AudioSettingsController, AudioMixer, RuntimeInitializeOnLoadMethod, Slider

### Community 38 - "AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente)"
Cohesion: 0.14
Nodes (12): 0. Princípios Fundamentais e Segurança, 1. ANTES de Responder ou Alterar Qualquer Código (Início de Todo Prompt), 3. AO FINAL de Cada Execução (Pós-Modificação Obrigatória), AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente), Passo 1.1 — Ler a Memória Persistente e o Cofre Obsidian, Passo 1.2 — Executar Busca no Graphify, Passo 1.3 — Confirmar no Código-Fonte C# Real, Passo 3.1 — Atualizar o Graphify e o Cofre Gerado do Obsidian (+4 more)

### Community 39 - "SubtitleSettingsController"
Cohesion: 0.21
Nodes (8): Image, Slider, TMP_Text, Toggle, SubtitleSettingsController, FundoEscuroAtivo, LegendasAtivas, MultiplicadorTamanho

### Community 40 - "com.unity.2d.tilemap"
Cohesion: 0.14
Nodes (15): dependencies, depth, dependencies, depth, source, url, version, source (+7 more)

### Community 41 - "com.unity.modules.imageconversion"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 42 - "Legenda"
Cohesion: 0.19
Nodes (10): CanvasGroup, CanvasScaler, Coroutine, IEnumerator, List, TextMeshProUGUI, Vector2, Fala (+2 more)

### Community 43 - "InteractionPrompt"
Cohesion: 0.19
Nodes (7): CanvasGroup, LoadSceneMode, RectTransform, Scene, TextMeshProUGUI, InteractionPrompt, Instance

### Community 45 - "com.unity.ext.nunit"
Cohesion: 0.17
Nodes (12): dependencies, depth, source, version, dependencies, depth, source, url (+4 more)

### Community 46 - "com.unity.modules.tetgen"
Cohesion: 0.18
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 47 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 48 - "dependencies"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, version, dependencies, depth, source, url (+3 more)

### Community 49 - ".CreateLabel"
Cohesion: 0.28
Nodes (6): Button, GraphicRaycaster, Image, RectTransform, UnityAction, TextMeshProUGUI

### Community 50 - "com.unity.mathematics"
Cohesion: 0.09
Nodes (22): dependencies, depth, source, url, version, dependencies, depth, source (+14 more)

### Community 51 - "ZonaId"
Cohesion: 0.20
Nodes (10): ZonaId, Interior_BarDoZe, Interior_CasaDoAvo, Interior_Mercadinho, Interior_Posto, Zona1_Rodovia, Zona2_Posto, Zona3_Encruzilhada (+2 more)

### Community 52 - "com.unity.collab-proxy"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.collab-proxy

### Community 53 - "TileAleatorio"
Cohesion: 0.28
Nodes (7): ColliderType, ITilemap, Sprite, TileAleatorio, TileBase, TileData, Vector3Int

### Community 55 - "Plano de Implementação — Setup Graphify + Obsidian + Antigravity"
Cohesion: 0.11
Nodes (18): 1.1 Sistema Operacional e Ferramentas, 1.2 Estrutura do Repositório, 1.3 Inventário de Arquivos do Graphify (Scan Inicial), 1.4 Estado Git, 1.5 Layout de Configuração de Agentes no Projeto, 1. Diagnóstico do Ambiente (Fase 0 — Somente Leitura), 2. Checklist de Execução por Fase, 3. Comandos Previstos por Fase (+10 more)

### Community 56 - "MovelLoja"
Cohesion: 0.33
Nodes (6): MovelLoja, CaixaMercado, CaixaPosto, Geladeira, Hortifruti, Prateleira

### Community 57 - "com.unity.2d.tooling"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.2d.tooling

### Community 58 - "com.unity.modules.accessibility"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.accessibility

### Community 60 - "dependencies"
Cohesion: 0.10
Nodes (21): dependencies, depth, source, version, dependencies, depth, source, version (+13 more)

### Community 62 - "com.unity.modules.umbra"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.umbra

### Community 64 - "Manual de Operação e Governança — Graphify + Obsidian + Agentes de IA"
Cohesion: 0.17
Nodes (11): 1. Visão Geral do Ecossistema e Separação de Camadas, 2. Início de Todo Novo Prompt (Ciclo Obrigatório da IA), 3. Como Abrir e Utilizar no Obsidian, 4. Atualização e Validação Pós-Execução (Linux/macOS e Windows), 5. Integração com Outros Agentes da Equipe, 6. Solução de Problemas Comuns (Troubleshooting), 7. Procedimento de Rollback e Recuperação, Manual de Operação e Governança — Graphify + Obsidian + Agentes de IA (+3 more)

## Knowledge Gaps
- **575 isolated node(s):** `Instance`, `Nome`, `LimiteMin`, `LimiteMax`, `Disponivel` (+570 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 776 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **10 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Capitulo1Director` connect `Capitulo1Director` to `InteracaoCapitulo1`, `MonoBehaviour`, `.CreateLabel`, `ZonaId`, `MovelLoja`, `TilePiso`, `unityengine`, `PayphoneCheckpoint`?**
  _High betweenness centrality (0.083) - this node is a cross-community bridge._
- **Why does `dependencies` connect `dependencies` to `com.unity.burst`, `packages-lock.json`, `com.unity.2d.common`, `com.unity.modules.unitywebrequest`, `com.unity.modules.physics`, `com.unity.test-framework`, `com.unity.modules.jsonserialize`, `com.unity.modules.imgui`, `com.unity.modules.audio`, `com.unity.settings-manager`, `com.unity.modules.ui`, `com.unity.2d.tilemap`, `com.unity.modules.imageconversion`, `com.unity.ext.nunit`, `com.unity.modules.tetgen`, `dependencies`, `com.unity.mathematics`, `com.unity.collab-proxy`, `com.unity.2d.tooling`, `com.unity.modules.accessibility`, `com.unity.modules.umbra`?**
  _High betweenness centrality (0.074) - this node is a cross-community bridge._
- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `Capitulo1Director`, `PlayerController`, `MonoBehaviour`, `TipoInteracao`, `unityengine`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **What connects `Instance`, `Nome`, `LimiteMin` to the rest of the system?**
  _575 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `InteracaoCapitulo1` be split into smaller, more focused modules?**
  _Cohesion score 0.07228070175438596 - nodes in this community are weakly interconnected._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.09811181044057757 - nodes in this community are weakly interconnected._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.07785547785547786 - nodes in this community are weakly interconnected._