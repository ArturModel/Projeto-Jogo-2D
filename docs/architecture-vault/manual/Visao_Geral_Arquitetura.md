# Visão Geral da Arquitetura — Santa Rita (Capítulo 1)

**Atualizado em:** 26/10/2026  
**Base:** Verificado nos 44 scripts C# de `Projeto Jogo 2D/Assets/` (~15.010 linhas) e nos documentos de auditoria em `Docs/`  

> [!NOTE] Documentação Detalhada do Cofre
> Para explorar o inventário completo de arquivos e fluxos, consulte também:
> - [[Catalogo_Completo_Scripts]] — Tabela detalhada de todos os 44 scripts C#, classes, structs e enums.
> - [[Mapa_Cenas_Zonas_E_Fluxo_Cap1]] — Topologia das 5 cenas, 9 zonas/interiores do `Capitulo1Director` e cadeia de flags.
> - [[Guia_Trabalho_Com_IA]] — Fluxo de trabalho obrigatório com Graphify + Obsidian + Agentes de IA.

---

## 1. Subsistemas Centrais

### 1.1 Persistência, Estado em Sessão e Save Manual (Regra Canônica R1)
- [[GameState]]: Gerenciador estático de estado em RAM (`flagsSessao`, `numerosSessao`, `textosSessao`, `diarioSessao`, `objetivoAtualSessao`, `TemProgressoNaoSalvo`). Garante a regra canônica de que **jamais há autosave automático** ao coletar pistas, trocar de cena ou vencer o boss.
- [[Flags]]: Catálogo estático de todas as constantes de progresso do Prólogo e do Capítulo 1 (equipamento do carro, pistas da Casa de Adílio, Mercadinho, duelo no Bar do Zé, Ficha do Clube e Limiar do Centro).
- [[CheckpointSystem]]: Único subsistema que serializa e grava `[[ManualSaveData]]` e `[[GameStateSnapshot]]` em disco (`santa_rita_save_v2.json`, `.bak` e 5 slots `santa_rita_save_v2_slot_0..4.json`).
- [[PayphoneCheckpoint]]: Ponto de interação físico no cenário (Orelhões da Encruzilhada e da Rua Norte) que reproduz o áudio procedural de orelhão brasileiro e abre a seleção de 5 Slots para salvar ou carregar.

### 1.2 Direção de Mundo, Zonas e Investigação (`ChegadaEmSantaRita.cs`)
- [[Capitulo1Director]]: Constrói e gerencia as **5 Zonas Externas** (`Zona1_Rodovia`, `Zona2_Posto`, `Zona3_Encruzilhada`, `Zona4_Bairro`, `Zona5_RuaNorteBar`) e os **4 Interiores** (`Interior_Posto`, `Interior_Mercadinho`, `Interior_CasaDoAvo`, `Interior_BarDoZe`), controlando limites de câmera, iluminação, áudio ambiente e fechamento do Capítulo 1.
- [[InteracaoCapitulo1]]: Implementa as **35 interações contextuais** (`[[TipoInteracao]]`) distribuídas pelas zonas e interiores (diálogos com Frentista, Atendente do Mercadinho, Vizinha Dona Lurdes, Zé, pistas da Casa do Avô e Portão do Centro).
- [[TransicaoDeZonaTrigger]] & [[DoorTrigger]] & [[SceneTransition]] & [[SpawnPoint]]: Infraestrutura de travessia espacial entre cenas e zonas com preservação do ponto de entrada e direção do olhar.
- [[DentroDaCasaDoAvo]] & [[ItemDeCena]]: Direção narrativa e inspeção de pistas em múltiplas etapas no interior da casa de Adílio (nº 40).

### 1.3 Navegação, UI e Investigação
- [[MapaInterativoUI]]: Interface aberta via `M` ou `Tab` com estética de caderno de bolso (4 abas: **Radar / Mapa**, **Diário Investigativo**, **Fichas do Clube** e **Legenda**). Reúne marcador em tempo real de Marcus, rota GPS pontilhada até o objetivo, pontos de interesse clicáveis e entradas categorizadas (`[FATO]`, `[PISTA]`, `[HIPÓTESE]`, `[ITEM]`).
- [[UIFactory]]: Fábrica utilitária para instanciação padronizada de componentes de interface gráfica no padrão de Canvas `640×360` e fonte pixelada `VT323-Regular Bitmap`.
- [[GameMessageUI]]: Painel de diálogos paginados com suporte a retrato de falante (`portrait` 48×48), crachá de nome (`SpeakerBadge`) e modal de inspeção de pistas.
- [[Legenda]] & [[InteractionPrompt]] & [[SceneFader]]: Legendas de pensamento no rodapé com fila de prioridade, aviso contextual `[E]` e transições de tela com efeito de lâmpada falhando (*flicker*) e letreiro de região.

### 1.4 Gameplay, Câmera, Clima e Iluminação 2D
- [[PlayerController]]: Movimentação top-down cadenciada de Marcus em 4 direções com lanterna pixel-perfect na mão (`LanternaNaMao` + feixe `Light2D`).
- [[CameraFollow]]: Câmera ortográfica a `32 PPU` com interpolação suave (`SmoothDamp`), alinhamento ao grid de pixels e confinamento estrito por bordas (`AplicarClampBordas`) para nunca vazar o vazio fora do cenário.
- [[Chuva]]: Simulação de tempestade desenhada pixel a pixel via `NativeArray<Color32>` em duas camadas (gotas no ar e respingos/poças no chão) reagindo à luz dos postes e da lanterna, com áudio abafado em interiores.
- [[CamadaDeLuz]] & [[AudioAmbiente]] & [[TileAleatorio]]: Controle de grupos de luz 2D por estado (`"presente"`, `"memoria"`), mixer de camadas de ambiência e tiles com variação determinística por hash de coordenada.

### 1.5 Prólogo Cinematográfico (Carro em 2004)
- [[PrologoDirector]] & [[PrologoVideo]]: Sequência de chegada no carro próprio de Marcus em 2004 sob a tempestade após Adílio parar de atender no mesmo dia.
- [[EstradaScroller]] & [[OlharPrimeiraPessoa]] & [[CamadaParallax]] & [[LimpadorParabrisa]] & [[ChuvaRolando]]: Motor pseudo-3D de estrada com curvatura, câmera 2D panorâmica com respiração do veículo, paralaxe em camadas e limpadores de para-brisa.
- [[PonteiroCarro]] & [[ObjetoCarro]] & [[CelularCarro]] & [[RadioCarro]] & [[PortaLuvasCarro]]: Interações apontar-e-clicar no interior do carro (4 chamadas perdidas no celular, boletim da chuva no rádio e mapa/foto no porta-luvas).

### 1.6 Confronto do Boss 1 — Zé e Minigame de Sinuca
- [[SinucaMinigameUI]]: Implementação completa do duelo em série **Melhor de 3 mãos** contra o Zé (`BarDoZe.unity` / `Interior_BarDoZe`).
  - Conjunto: Branca + 7 bolas numeradas compartilhadas (`1..7`); **4 pontos** vencem a mão.
  - Regra autoral canônica (a partir da 2ª mão): **"A Bola Marcada / Essa Fica na Mesa"** (`[X]`). Encaçapar a bola marcada é falta, anula os pontos da tacada, repõe a bola marcada de forma livre de colisão e concede bola na mão ao oponente (desativada automaticamente quando restam `< 2` bolas).
  - Invariante de integridade: `pontosMarcus + pontosZe + bolasNaMesa == 7`.
  - Recompensa de vitória (`2-0` ou `2-1`): Concessão da **Ficha do Clube #01 (`ficha_clube_ze_01`)**, relato de que Adílio não saiu por vontade própria e desbloqueio do **Limiar do Centro**.

### 1.7 Menus, Pausa, Configurações e Automação de Editor
- [[MainMenuActions]] & [[MenuPausa]] & [[MenuPanelTransition]] & [[OptionsPanelController]] & [[MenuItemSelector]]: Fluxo de Menu Principal e Pausa in-game (`ESC`) com proteção contra perda de progresso não salvo.
- [[AudioSettingsController]] & [[VideoSettingsController]] & [[SubtitleSettingsController]]: Persistência independente de preferências de áudio (`AudioMixer`), vídeo e legendas em `PlayerPrefs`.
- [[SantaRitaChapter1Builder]]: Automação de Editor (`Assets/Editor/`) que gera sprites de props/pistas, constrói/atualiza as cenas do Capítulo 1, valida os contratos R1 em batchmode e empacota o build Windows x64.

---

## 2. Relações Verificadas no Grafo

- [[GameState]] `<--->` [[CheckpointSystem]] via `ExportarSnapshot()` / `ImportarSnapshot()`.
- [[PayphoneCheckpoint]] `--->` [[CheckpointSystem]] via `SaveSlot()` / `TryLoadSlot()`.
- [[Capitulo1Director]] `--->` [[InteracaoCapitulo1]], [[TransicaoDeZonaTrigger]], [[PayphoneCheckpoint]], [[CameraFollow]] e [[SceneFader]].
- [[MapaInterativoUI]] `--->` [[GameState]] (leitura de `Diario`, `ObjetivoAtual` e `Flags`) e [[Capitulo1Director]] (leitura de `ZonaAtual` e coordenadas UV).
- [[SinucaMinigameUI]] `--->` [[GameState]] ao marcar `Flags.VenceuZeSinuca`, `Flags.FichaZeObtida` e `Flags.CentroLiberado`.
