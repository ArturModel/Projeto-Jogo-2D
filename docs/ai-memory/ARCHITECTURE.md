# Arquitetura do Software — Santa Rita (Projeto Jogo 2D)

**Atualizado em:** 26/10/2026  
**Por:** Antigravity AI  
**Base:** Verificado nos 44 scripts C# (`Projeto Jogo 2D/Assets/Scripts/` e `Assets/Editor/`, ~15.010 linhas) e nos contratos R1 (`Docs/`)  

---

## 1. Subsistemas de Software e Responsabilidades

### 1.1 Persistência, Estado em RAM e Save Manual (Regra Canônica R1)
- **[`GameState.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/GameState.cs) (414 linhas) [Verificado]:**
  - Mantém o estado da sessão 100% em RAM: `flagsSessao` (`HashSet<string>`), `numerosSessao`, `textosSessao`, `diarioSessao` (`List<EntradaDiario>`) e `objetivoAtualSessao`.
  - Define a classe estática `Flags` com todas as constantes canônicas do Prólogo e do Capítulo 1.
  - Sincroniza automaticamente entradas do Diário Investigativo ao marcar flags de pistas/progresso (`SincronizarDiarioAutomaticoPorFlag`).
  - Controla a propriedade `TemProgressoNaoSalvo` que dispara avisos caso o jogador tente sair sem salvar no Orelhão.
- **[`CheckpointSystem.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CheckpointSystem.cs) (328 linhas) [Verificado]:**
  - Único sistema autorizado a gravar progresso em disco (`santa_rita_save_v2.json`, fallback `.bak` e 5 slots `santa_rita_save_v2_slot_0..4.json`).
  - Serializa `ManualSaveData` contendo cena, posição `(X, Y)`, `payphoneId`, descrição amigável do local, carimbo de data/hora e o `GameStateSnapshot` completo.
- **[`PayphoneCheckpoint.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PayphoneCheckpoint.cs) (528 linhas) [Verificado]:**
  - Ponto de contato físico (Orelhões da Encruzilhada e do Bar do Zé). Gera áudio procedural autêntico de orelhão brasileiro (gancho, tom de linha 425 Hz, leitura de cartão/ficha, discagem DTMF e chiado de chuva) e abre o modal de seleção dos **5 Slots de Save/Load**.

### 1.2 Direção de Mundo, Zonas e Investigação do Capítulo 1
- **[`ChegadaEmSantaRita.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/ChegadaEmSantaRita.cs) (3.550 linhas) [Verificado]:**
  - `Capitulo1Director`: Constrói e gerencia as **5 Zonas Externas** (`Zona1_Rodovia`, `Zona2_Posto`, `Zona3_Encruzilhada`, `Zona4_Bairro`, `Zona5_RuaNorteBar`) e os **4 Interiores** (`Interior_Posto`, `Interior_Mercadinho`, `Interior_CasaDoAvo`, `Interior_BarDoZe`), controlando limites de câmera, iluminação, áudio e painel de fechamento do Capítulo 1.
  - `InteracaoCapitulo1`: Implementa as **35 interações narrativas** (`TipoInteracao`) com NPCs (Frentista, Atendente do Mercadinho, Vizinha Dona Lurdes, Morador, Zé), objetos e pistas.
  - `TransicaoDeZonaTrigger`: Gatilhos de corte de câmera com fade-to-black entre zonas e interiores.
- **[`DentroDaCasaDoAvo.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/DentroDaCasaDoAvo.cs), [`Interactable.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/Interactable.cs), [`ItemDeCena.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/ItemDeCena.cs), [`DoorTrigger.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/DoorTrigger.cs), [`SpawnPoint.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SpawnPoint.cs), [`SceneTransition.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SceneTransition.cs) [Verificado]:**
  - Infraestrutura modular de interação via tecla `E` (registro estático `Interactable.Registry`), inspeção progressiva de pistas e travessia entre cenas preservando ponto de entrada e direção.

### 1.3 Interface, Investigação e Navegação
- **[`MapaInterativoUI.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MapaInterativoUI.cs) (1.159 linhas) [Verificado]:**
  - Interface integrada aberta por `M` ou `Tab` com 4 abas:
    1. **Radar / Mapa de Santa Rita** (estilo *Zelda OoT* + *GTA SA*) com posição de Marcus em tempo real, zoom/pan, rota GPS pontilhada até o objetivo e seleção de pontos de interesse.
    2. **Diário Investigativo** categorizado em `[FATO]`, `[PISTA]` e `[HIPÓTESE]`.
    3. **Fichas do Clube / Itens Chave** (Lanterna, Mapa, Recibo 2004, Ficha do Clube #01).
    4. **Legenda Completa** dos pontos da cidade.
- **[`GameMessageUI.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/GameMessageUI.cs) (642 linhas), [`UIFactory.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/UIFactory.cs) (92 linhas), [`Legenda.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/Legenda.cs) (193 linhas), [`InteractionPrompt.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/InteractionPrompt.cs) (119 linhas), [`SceneFader.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SceneFader.cs) (484 linhas) [Verificado]:**
  - Caixas de diálogo paginadas com crachá de falante e retratos 48×48 (`marcus`, `frentista`, `atendente`, `ze`, `adilio`), fábrica de Canvas `640×360` com fonte `VT323-Regular Bitmap`, fila de legendas por prioridade e fader com efeito de lâmpada falhando (*flicker*) e letreiro de região.

### 1.4 Confronto e Minigame do Boss 1 (Zé / Sinuca)
- **[`SinucaMinigameUI.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SinucaMinigameUI.cs) (1.412 linhas) [Verificado]:**
  - Gerencia a série **Melhor de 3 mãos** com física 2D simulada em visão superior.
  - Conjunto de bolas: 1 branca + 7 numeradas compartilhadas (`1..7`). Meta de vitória por mão: **4 pontos**.
  - **Invariante Canônico:** `pontosMarcus + pontosZe + bolasNaMesa == 7`.
  - **Mecânica Psicológica ("A Bola Marcada / Essa Fica na Mesa"):** A partir da 2ª mão, uma bola sorteada recebe marcador `[X]`. Encaçapá-la acarreta falta imediata, anula os pontos da tacada, repõe a bola marcada em posição livre determinística e concede bola na mão ao oponente. Desativada automaticamente quando restam `< 2` bolas na mesa para prevenir deadlock.

### 1.5 Controle, Câmera, Clima e Prólogo
- **[`PlayerController.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PlayerController.cs) (553 linhas) & [`CameraFollow.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CameraFollow.cs) (290 linhas) [Verificado]:**
  - Movimento 2D cadenciado com New Input System, lanterna pixel-perfect na mão (`LanternaNaMao`) e câmera suavizada alinhada ao grid de `32 PPU` com contenção estrita pelas bordas (`AplicarClampBordas`).
- **[`Chuva.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/Chuva.cs) (817 linhas), [`CamadaDeLuz.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CamadaDeLuz.cs), [`AudioAmbiente.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/AudioAmbiente.cs), [`TileAleatorio.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/TileAleatorio.cs) [Verificado]:**
  - Chuva procedural em duas texturas (`NativeArray<Color32>`) reagindo à luz 2D, camadas de iluminação por estado e mixer de ambiência.
- **Prólogo ([`PrologoDirector.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PrologoDirector.cs), [`PrologoVideo.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PrologoVideo.cs), [`EstradaScroller.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/EstradaScroller.cs), [`OlharPrimeiraPessoa.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/OlharPrimeiraPessoa.cs), [`CelularCarro.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CelularCarro.cs), [`RadioCarro.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/RadioCarro.cs), [`PortaLuvasCarro.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PortaLuvasCarro.cs), [`PonteiroCarro.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PonteiroCarro.cs), [`ObjetoCarro.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/ObjetoCarro.cs), [`LimpadorParabrisa.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/LimpadorParabrisa.cs), [`ChuvaRolando.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/ChuvaRolando.cs), [`CamadaParallax.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CamadaParallax.cs)) [Verificado]:**
  - Sequência completa de direção noturna em primeira pessoa rumo a Santa Rita em 2004.

### 1.6 Menus, Pausa, Configurações e Builder de Editor
- **Menus ([`MainMenuActions.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MainMenuActions.cs), [`MenuPausa.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MenuPausa.cs), [`MenuPanelTransition.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MenuPanelTransition.cs), [`OptionsPanelController.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/OptionsPanelController.cs), [`MenuItemSelector.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MenuItemSelector.cs), [`AudioSettingsController.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/AudioSettingsController.cs), [`VideoSettingsController.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/VideoSettingsController.cs), [`SubtitleSettingsController.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SubtitleSettingsController.cs)) [Verificado].**
- **Editor ([`SantaRitaChapter1Builder.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Editor/SantaRitaChapter1Builder.cs), 818 linhas) [Verificado]:** Montagem automatizada de cenas, geração de sprites de pistas/props e testes de validação em batchmode.

---

## 2. Diagrama de Fluxo de Dados e Interações

```
[PayphoneCheckpoint] ───(SaveSlot 0..4)───> [CheckpointSystem] ───(JSON Atômico)───> [Disco / santa_rita_save_v2*.json]
         │                                          ▲
         ▼                                          │ (ExportarSnapshot / ImportarSnapshot)
[GameMessageUI] <─────(Lê Diário/Flags)───── [GameState (RAM)] <────(Marca Pistas/Flags)──── [InteracaoCapitulo1]
         ▲                                          ▲                                                 ▲
         │                                          │ (VenceuZeSinuca + FichaZeObtida)                │ (Constrói 9 Zonas)
 [MapaInterativoUI] (Tecla M)              [SinucaMinigameUI] (Boss Zé)                     [Capitulo1Director]
```
