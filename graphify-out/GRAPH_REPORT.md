# Graph Report - Projeto-Jogo-2D  (2026-10-26)

## Corpus Check
- 58 files · ~63,980 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 9 file(s) not represented in the graph (top: (none) 3, .csv 1, .mixer 1)

## Summary
- 1633 nodes · 3074 edges · 68 communities (61 shown, 7 thin omitted)
- Extraction: 98% EXTRACTED · 2% INFERRED · 0% AMBIGUOUS · INFERRED: 62 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `aebf3dc1`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- InteracaoCapitulo1
- CameraFollow
- Capitulo1Director
- SinucaMinigameUI
- PlayerController
- MapaInterativoUI
- manifest.json
- dependencies
- Chuva
- SantaRitaChapter1Builder
- MenuPausa
- com.unity.render-pipelines.core
- packages-lock.json
- com.unity.2d.common
- TipoInteracao
- SceneFader
- OptionsPanelController
- CheckpointSystem
- com.unity.modules.audio
- dependencies
- GameState
- GameMessageUI
- Interactable
- com.unity.test-framework
- com.unity.modules.jsonserialize
- TilePiso
- com.unity.modules.uielements
- unityengine
- com.unity.modules.animation
- MonoBehaviour
- PayphoneCheckpoint
- VideoSettingsController
- com.unity.settings-manager
- PrologoDirector
- com.unity.modules.imgui
- PrologoVideo
- MenuItemSelector
- AudioSettingsController
- 1. ANTES de Responder ou Alterar Qualquer Código (Início de Todo Prompt)
- SubtitleSettingsController
- com.unity.2d.tilemap
- com.unity.modules.imageconversion
- Legenda
- EstradaScroller
- .SaveSlot
- .DetectarLimitesDaRegiao
- com.unity.modules.wind
- EstadoPartida
- .ConsumirEntrada
- AudioAmbiente
- com.unity.modules.physics2d
- ZonaId
- com.unity.collab-proxy
- TileAleatorio
- .AoClicarSlot
- Plano de Implementação — Setup Graphify + Obsidian + Antigravity
- LimpadorParabrisa
- OlharPrimeiraPessoa
- com.unity.modules.accessibility
- com.unity.burst
- com.unity.modules.ai
- com.unity.modules.tetgen
- com.unity.modules.umbra
- Cadeia de Investigação e Pistas do Capítulo 1
- Manual de Operação e Governança — Graphify + Obsidian + Antigravity
- graphify-refresh.sh
- graphify-setup.sh
- graphify-validate.sh

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
- `Escala Visual e Pixel Perfect (PPU 32, Canvas 640x360)` --conceptually_related_to--> `CameraFollow`  [INFERRED]
  Docs/SANTA_RITA_PROJECT_AUDIT.md → Projeto Jogo 2D/Assets/Scripts/CameraFollow.cs
- `Contrato de Save Manual Exclusivo no Orelhão` --implements--> `CheckpointSystem`  [EXTRACTED]
  Docs/SANTA_RITA_PROJECT_AUDIT.md → Projeto Jogo 2D/Assets/Scripts/CheckpointSystem.cs
- `2. Regras Inegociáveis de Domínio do Jogo (Revisão Canônica R1)` --references--> `GameStateSnapshot`  [INFERRED]
  AGENTS.md → Projeto Jogo 2D/Assets/Scripts/GameState.cs
- `Cânone da Linha do Tempo (1990, 1998, 2004)` --rationale_for--> `PrologoDirector`  [EXTRACTED]
  Docs/SANTA_RITA_PROJECT_AUDIT.md → Projeto Jogo 2D/Assets/Scripts/PrologoDirector.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Núcleo de Persistência Manual e Estado em RAM** — docs_santa_rita_project_audit_save_manual_orelhao, docs_santa_rita_vertical_slice_report_gamestate_ram, projeto_jogo_2d_assets_scripts_checkpointsystem_checkpointsystem, projeto_jogo_2d_assets_scripts_gamestate_gamestate, projeto_jogo_2d_assets_scripts_payphonecheckpoint_payphonecheckpoint [EXTRACTED 1.00]
- **Sistemas e Regras do Confronto com Zé (Sinuca)** — docs_santa_rita_project_audit_sinuca_rules, docs_santa_rita_vertical_slice_report_sinuca_invariants, projeto_jogo_2d_assets_scripts_sinucaminigameui_sinucaminigameui [EXTRACTED 1.00]

## Communities (68 total, 7 thin omitted)

### Community 0 - "InteracaoCapitulo1"
Cohesion: 0.07
Nodes (14): CelularCarro, Disponivel, IEnumerator, Collider2D, IEnumerator, LoadSceneMode, Scene, InteracaoCapitulo1 (+6 more)

### Community 1 - "CameraFollow"
Cohesion: 0.27
Nodes (8): 2. Regras Inegociáveis de Domínio do Jogo (Revisão Canônica R1), CameraFollow, LimiteMax, LimiteMin, Camera, Transform, Vector2, Vector3

### Community 2 - "Capitulo1Director"
Cohesion: 0.09
Nodes (33): Func, InfoZona, MovelLoja, Capitulo1Director, EstaEmInterior, Instance, ZonaAtual, AudioClip (+25 more)

### Community 3 - "SinucaMinigameUI"
Cohesion: 0.08
Nodes (24): BolaMesa, Regras da Sinuca Autoral: A Bola Marcada (Melhor de 3), Invariantes de Física e Pontuação da Sinuca, EstadoPartida, BolaMesa, Action, Button, Color (+16 more)

### Community 4 - "PlayerController"
Cohesion: 0.21
Nodes (6): Sprite, SpriteRenderer, Transform, Vector2, PlayerController, Rigidbody2D

### Community 5 - "MapaInterativoUI"
Cohesion: 0.08
Nodes (28): Sistema de Mapa Interativo (Tecla M / Caderno de Bolso), Diário Investigativo e Abas de Fichas do Clube, PontoMapa, Button, Canvas, Color, Dictionary, GameObject (+20 more)

### Community 6 - "manifest.json"
Cohesion: 0.04
Nodes (54): com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.director, com.unity.modules.imageconversion, com.unity.modules.imgui (+46 more)

### Community 7 - "dependencies"
Cohesion: 0.04
Nodes (55): dependencies, com.unity.2d.animation, com.unity.2d.aseprite, com.unity.2d.psdimporter, com.unity.2d.sprite, com.unity.2d.spriteshape, com.unity.2d.tilemap, com.unity.2d.tilemap.extras (+47 more)

### Community 8 - "Chuva"
Cohesion: 0.09
Nodes (24): AudioLowPassFilter, AudioMixerGroup, Camada, Gota, NativeArray, PixelPerfectCamera, Poca, Camada (+16 more)

### Community 9 - "SantaRitaChapter1Builder"
Cohesion: 0.08
Nodes (20): MenuItem, BoxCollider2D, Color, GameObject, Material, Sprite, SpriteRenderer, Texture2D (+12 more)

### Community 10 - "MenuPausa"
Cohesion: 0.10
Nodes (25): EventSystem, Graphic, InputSystemUIInputModule, Action, AudioClip, AudioMixer, AudioSource, Button (+17 more)

### Community 11 - "com.unity.render-pipelines.core"
Cohesion: 0.07
Nodes (30): dependencies, depth, source, version, depth, source, version, dependencies (+22 more)

### Community 12 - "packages-lock.json"
Cohesion: 0.05
Nodes (39): com.unity.2d.sprite, com.unity.2d.tilemap, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.director, com.unity.modules.imageconversion, com.unity.modules.imgui (+31 more)

### Community 13 - "com.unity.2d.common"
Cohesion: 0.06
Nodes (34): dependencies, depth, source, url, version, dependencies, depth, source (+26 more)

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
Cohesion: 0.22
Nodes (7): CheckpointSystem, BackupFilePath, CheckpointScene, HasCheckpoint, PrimaryFilePath, UltimoSlotUsado, ManualSaveData

### Community 18 - "com.unity.modules.audio"
Cohesion: 0.07
Nodes (35): dependencies, depth, source, version, dependencies, depth, source, version (+27 more)

### Community 19 - "dependencies"
Cohesion: 0.09
Nodes (26): dependencies, depth, source, version, dependencies, depth, source, version (+18 more)

### Community 20 - "GameState"
Cohesion: 0.13
Nodes (13): Arquitetura de Estado em RAM na Sessão (GameState), HashSet, IReadOnlyList, Dictionary, List, EntradaDiario, Flags, GameState (+5 more)

### Community 21 - "GameMessageUI"
Cohesion: 0.14
Nodes (14): Action, CanvasGroup, Coroutine, Dictionary, GameObject, IEnumerator, Image, List (+6 more)

### Community 22 - "Interactable"
Cohesion: 0.15
Nodes (6): Animator, List, Vector2, Interactable, InteractionRadius, Prompt

### Community 23 - "com.unity.test-framework"
Cohesion: 0.07
Nodes (27): dependencies, depth, source, version, dependencies, depth, source, url (+19 more)

### Community 24 - "com.unity.modules.jsonserialize"
Cohesion: 0.06
Nodes (35): dependencies, depth, source, version, dependencies, depth, source, version (+27 more)

### Community 25 - "TilePiso"
Cohesion: 0.10
Nodes (20): TilePiso, AsfaltoH_Buraco, AsfaltoH_Poca, AsfaltoH_Trinca, AsfaltoParaTerra, AsfaltoV, CalcadaNorte, CalcadaSul (+12 more)

### Community 26 - "com.unity.modules.uielements"
Cohesion: 0.12
Nodes (16): dependencies, depth, source, url, version, dependencies, depth, source (+8 more)

### Community 27 - "unityengine"
Cohesion: 0.05
Nodes (36): LayerMask, Color, SpriteRenderer, ObjetoCarro, Apontado, Disponivel, Camera, PonteiroCarro (+28 more)

### Community 28 - "com.unity.modules.animation"
Cohesion: 0.11
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 29 - "MonoBehaviour"
Cohesion: 0.11
Nodes (9): Fluxo Narrativo e Cênico do Capítulo 1, MonoBehaviour, CamadaParallax, Vector3, ChegadaEmSantaRita, ChuvaRolando, Vector3, DentroDaCasaDoAvo (+1 more)

### Community 30 - "PayphoneCheckpoint"
Cohesion: 0.10
Nodes (19): Contrato de Save Manual Exclusivo no Orelhão, PayphoneSlotInputWatcher, AudioClip, AudioSource, Button, Color, GameObject, GraphicRaycaster (+11 more)

### Community 31 - "VideoSettingsController"
Cohesion: 0.10
Nodes (12): CamadaDeLuz, Nome, Light2D, List, List, RawImage, Slider, TMP_Dropdown (+4 more)

### Community 32 - "com.unity.settings-manager"
Cohesion: 0.12
Nodes (17): dependencies, depth, source, version, dependencies, depth, source, url (+9 more)

### Community 33 - "PrologoDirector"
Cohesion: 0.25
Nodes (5): FalaMarcada, IEnumerator, Image, FalaMarcada, PrologoDirector

### Community 34 - "com.unity.modules.imgui"
Cohesion: 0.06
Nodes (32): dependencies, depth, source, version, dependencies, depth, source, version (+24 more)

### Community 35 - "PrologoVideo"
Cohesion: 0.17
Nodes (9): AspectRatioFitter, Cânone da Linha do Tempo (1990, 1998, 2004), IEnumerator, Image, RawImage, Fala, PrologoVideo, RenderTexture (+1 more)

### Community 36 - "MenuItemSelector"
Cohesion: 0.18
Nodes (10): BaseEventData, IPointerEnterHandler, ISelectHandler, PointerEventData, AudioClip, AudioSource, GameObject, RectTransform (+2 more)

### Community 37 - "AudioSettingsController"
Cohesion: 0.28
Nodes (4): AudioSettingsController, AudioMixer, RuntimeInitializeOnLoadMethod, Slider

### Community 38 - "1. ANTES de Responder ou Alterar Qualquer Código (Início de Todo Prompt)"
Cohesion: 0.15
Nodes (11): 1. ANTES de Responder ou Alterar Qualquer Código (Início de Todo Prompt), 3. AO FINAL de Cada Execução (Pós-Modificação Obrigatória), AGENTS.md — Governança Obrigatória de IA (Graphify + Obsidian + Memória Persistente), Passo 1.1 — Ler a Memória Persistente e o Cofre Obsidian, Passo 1.2 — Executar Busca no Graphify, Passo 1.3 — Confirmar no Código-Fonte C# Real, Passo 3.1 — Atualizar o Graphify e o Cofre Gerado do Obsidian, Passo 3.2 — Atualizar as Notas Manuais do Obsidian e a Memória Persistente (+3 more)

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
Cohesion: 0.05
Nodes (35): Escala Visual e Pixel Perfect (PPU 32, Canvas 640x360), Fala, Button, GraphicRaycaster, Image, RectTransform, UnityAction, CanvasGroup (+27 more)

### Community 43 - "EstradaScroller"
Cohesion: 0.20
Nodes (9): Fileira, List, Sprite, SpriteRenderer, Transform, EstradaScroller, Curvatura, Velocidade (+1 more)

### Community 45 - ".DetectarLimitesDaRegiao"
Cohesion: 0.40
Nodes (4): BoxCollider2D, SpriteRenderer, Tilemap, TilemapRenderer

### Community 46 - "com.unity.modules.wind"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.wind

### Community 47 - "EstadoPartida"
Cohesion: 0.17
Nodes (12): EstadoPartida, BolasEmMovimento, CarregandoForca, FimDeMao, FimDeSerieDerrota, FimDeSerieVitoria, Inativo, MirandoJogador (+4 more)

### Community 49 - "AudioAmbiente"
Cohesion: 0.20
Nodes (6): AudioAmbiente, Instance, Camada, AudioClip, AudioSource, Dictionary

### Community 50 - "com.unity.modules.physics2d"
Cohesion: 0.12
Nodes (17): dependencies, depth, source, url, version, dependencies, depth, source (+9 more)

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

### Community 56 - "LimpadorParabrisa"
Cohesion: 0.31
Nodes (3): Transform, LimpadorParabrisa, Ativo

### Community 57 - "OlharPrimeiraPessoa"
Cohesion: 0.38
Nodes (4): Vector2, Vector3, OlharPrimeiraPessoa, Deslocamento

### Community 58 - "com.unity.modules.accessibility"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.accessibility

### Community 59 - "com.unity.burst"
Cohesion: 0.09
Nodes (23): dependencies, depth, source, version, dependencies, depth, source, version (+15 more)

### Community 60 - "com.unity.modules.ai"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.ai

### Community 61 - "com.unity.modules.tetgen"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.tetgen

### Community 62 - "com.unity.modules.umbra"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.umbra

### Community 64 - "Manual de Operação e Governança — Graphify + Obsidian + Antigravity"
Cohesion: 0.20
Nodes (9): 1. Visão Geral do Ecossistema, 2. Início de uma Nova Sessão de Agente, 3. Como Abrir e Utilizar no Obsidian, 4. Atualização do Grafo (Pós-Alteração de Código), 5. Política de Versionamento Git, 6. Procedimento de Rollback e Recuperação de Falhas, Arquivos Ignorados no Git (`.gitignore`):, Arquivos Versionados no Git: (+1 more)

## Knowledge Gaps
- **573 isolated node(s):** `Instance`, `Nome`, `LimiteMin`, `LimiteMax`, `Disponivel` (+568 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 771 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **7 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Capitulo1Director` connect `Capitulo1Director` to `InteracaoCapitulo1`, `SantaRitaChapter1Builder`, `Legenda`, `ZonaId`, `TilePiso`, `unityengine`, `MonoBehaviour`, `PayphoneCheckpoint`?**
  _High betweenness centrality (0.083) - this node is a cross-community bridge._
- **Why does `dependencies` connect `dependencies` to `com.unity.render-pipelines.core`, `packages-lock.json`, `com.unity.2d.common`, `com.unity.modules.audio`, `com.unity.test-framework`, `com.unity.modules.jsonserialize`, `com.unity.modules.uielements`, `com.unity.modules.animation`, `com.unity.settings-manager`, `com.unity.modules.imgui`, `com.unity.2d.tilemap`, `com.unity.modules.imageconversion`, `com.unity.modules.wind`, `com.unity.modules.physics2d`, `com.unity.collab-proxy`, `com.unity.modules.accessibility`, `com.unity.burst`, `com.unity.modules.ai`, `com.unity.modules.tetgen`, `com.unity.modules.umbra`?**
  _High betweenness centrality (0.075) - this node is a cross-community bridge._
- **Why does `InteracaoCapitulo1` connect `InteracaoCapitulo1` to `Capitulo1Director`, `SantaRitaChapter1Builder`, `TipoInteracao`, `Interactable`, `unityengine`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **What connects `Instance`, `Nome`, `LimiteMin` to the rest of the system?**
  _573 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `InteracaoCapitulo1` be split into smaller, more focused modules?**
  _Cohesion score 0.06635802469135803 - nodes in this community are weakly interconnected._
- **Should `Capitulo1Director` be split into smaller, more focused modules?**
  _Cohesion score 0.09333333333333334 - nodes in this community are weakly interconnected._
- **Should `SinucaMinigameUI` be split into smaller, more focused modules?**
  _Cohesion score 0.07550482879719052 - nodes in this community are weakly interconnected._