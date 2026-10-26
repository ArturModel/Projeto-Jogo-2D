# Catálogo Completo de Scripts C# — Santa Rita (Capítulo 1)

**Atualizado em:** 26/10/2026  
**Escopo:** Todos os 44 scripts C# do projeto (`43` em `Assets/Scripts/` + `1` em `Assets/Editor/`, totalizando ~15.010 linhas de código).

> [!TIP] Navegação no Obsidian
> Clique em qualquer `[[NomeDaClasse]]` abaixo para abrir a nota atômica gerada pelo Graphify em `generated/`, ver seus métodos, campos e conexões no grafo!

---

## 1. Persistência, Estado em RAM e Save Manual (Regra Canônica R1)

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `GameState.cs` | 414 | [[GameState]], [[Flags]], [[GameStateSnapshot]], [[EntradaDiario]], [[ParNumero]], [[ParTexto]] | Gerencia 100% em RAM o estado da sessão (`flagsSessao`, `numerosSessao`, `textosSessao`, `diarioSessao`, `objetivoAtualSessao`, `TemProgressoNaoSalvo`). Sincroniza entradas automáticas do diário ao marcar flags canônicas e exporta/importa `GameStateSnapshot` apenas quando solicitado pelo `CheckpointSystem`. |
| `CheckpointSystem.cs` | 328 | [[CheckpointSystem]], [[ManualSaveData]] | Único sistema autorizado a gravar progresso em disco (`santa_rita_save_v2.json`, `.bak` e 5 slots `santa_rita_save_v2_slot_0..4.json`). Salva cena, coordenadas `(X, Y)`, ID do orelhão, descrição amigável do local, data/hora e o `GameStateSnapshot` completo. |
| `PayphoneCheckpoint.cs` | 528 | [[PayphoneCheckpoint]], `PayphoneSlotInputWatcher` | Ponto físico de salvamento manual nos Orelhões (`Interactable`). Gera e reproduz áudio procedural autêntico de orelhão brasileiro (gancho, tom 425 Hz, leitura de ficha/cartão, discagem DTMF e chiado de chuva) e exibe o modal interativo de 5 Slots para Salvar ou Carregar. |

---

## 2. Direção de Mundo, Zonas e Investigação do Capítulo 1

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `ChegadaEmSantaRita.cs` | 3.550 | [[ChegadaEmSantaRita]], [[Capitulo1Director]], [[InteracaoCapitulo1]], [[TransicaoDeZonaTrigger]], [[ZonaId]], [[InfoZona]], [[TipoInteracao]], [[TilePiso]], [[MovelLoja]] | Coração do Capítulo 1. `Capitulo1Director` constrói e gerencia as **5 Zonas Externas** (`Zona1_Rodovia`, `Zona2_Posto`, `Zona3_Encruzilhada`, `Zona4_Bairro`, `Zona5_RuaNorteBar`) e os **4 Interiores** (`Interior_Posto`, `Interior_Mercadinho`, `Interior_CasaDoAvo`, `Interior_BarDoZe`). `InteracaoCapitulo1` implementa todas as 35 interações narrativas, diálogos com NPCs (Frentista, Atendente, Dona Lurdes, Zé), coleta de pistas e fechamento do Capítulo 1 no Limiar do Centro. |
| `DentroDaCasaDoAvo.cs` | 54 | [[DentroDaCasaDoAvo]] | Diretor de entrada na Casa de Adílio (nº 40). Marca `Flags.ChegouCasaAvo`, registra o fato inicial no Diário Investigativo e dispara as legendas de recepção (*"A porta estava destrancada"*, *"Vô? ..."*). |
| `Interactable.cs` | 62 | [[Interactable]] | Classe base abstrata para todos os objetos interativos via tecla `E`. Mantém um registro estático (`Registry`) para busca espacial rápida (`FindNearest`) pelo `PlayerController` sem exigir varreduras caras de física. |
| `ItemDeCena.cs` | 83 | [[ItemDeCena]] | Componente para objetos examináveis em múltiplas etapas de texto. Marca flags na primeira leitura (`flagAoExaminar`) e na conclusão (`flagAoTerminar`), integrando pistas da Casa do Avô ao Diário. |
| `DoorTrigger.cs` | 65 | [[DoorTrigger]] | Gatilho de travessia de portas entre cenas (ou interiores), com suporte a exigência de flag (`flagNecessaria`), mensagem de porta trancada e destino via `SceneTransition`. |
| `SpawnPoint.cs` | 54 | [[SpawnPoint]] | Ponto nomeado no cenário (`id`, `direcaoInicial`) onde Marcus é posicionado ao carregar uma cena ou atravessar uma porta. |
| `SceneTransition.cs` | 48 | [[SceneTransition]] | Mantém o `entradaPendente` entre trocas de cena, consumido pelo `PlayerController` para posicionar Marcus no `SpawnPoint` correspondente. |

---

## 3. Confronto do Boss 1 — Zé e Minigame de Sinuca

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `SinucaMinigameUI.cs` | 1.412 | [[SinucaMinigameUI]], [[EstadoPartida]], `BolaMesa` | Implementação completa do duelo de sinuca top-down em série **Melhor de 3 mãos** contra Zé. Simula física 2D customizada (colisões elásticas, tabelas, atrito, caçapas), mira com linha guia, barra de força, IA tática do Zé e a regra psicológica autoral **"A Bola Marcada / Essa Fica na Mesa" (`[X]`)** a partir da 2ª mão. Garante o invariante `pontosMarcus + pontosZe + bolasNaMesa == 7`. Na vitória (`2-0` ou `2-1`), entrega a Ficha do Clube #01 (`ficha_clube_ze_01`) e libera o Limiar do Centro. |

---

## 4. Interface do Usuário (UI), Mapa Interativo e Diálogos

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `MapaInterativoUI.cs` | 1.159 | [[MapaInterativoUI]], `PontoMapa` | Painel integrado aberto por `M` ou `Tab` (4 abas: **Radar / Mapa de Santa Rita**, **Diário Investigativo**, **Fichas do Clube / Itens** e **Legenda**). Inspirado em *Zelda: OoT* e *GTA: SA*, exibe posição de Marcus em tempo real, zoom/pan, rota GPS pontilhada até o objetivo ativo, Orelhões e pontos de interesse clicáveis. |
| `GameMessageUI.cs` | 642 | [[GameMessageUI]], [[DialogoLinha]] | Sistema de caixas de diálogo paginadas com suporte a efeito de digitação, crachá de nome do falante (`SpeakerBadge`), retratos 48×48 gerados/carregados sob demanda (`marcus`, `frentista`, `atendente`, `ze`, `adilio`) e painel modal de inspeção visual de pistas. |
| `UIFactory.cs` | 92 | [[UIFactory]] | Fábrica estática para criação padronizada de Canvas (`640×360`), caixas 9-slice (`DialogueBox`, `PromptBox`) e rótulos TextMeshPro com a fonte pixelada `VT323-Regular Bitmap`. |
| `Legenda.cs` | 193 | [[Legenda]], `Fala` | Sistema de legendas de rodapé para pensamentos internos de Marcus e vozes de ambiente, com fila ordenada por prioridade (`PrioridadeOpcional`, `PrioridadeRoteiro`, `PrioridadeCritica`) e integração com `SubtitleSettingsController`. |
| `InteractionPrompt.cs` | 119 | [[InteractionPrompt]] | Indicador flutuante discreto (`[E] Ação`) que aparece suavemente quando Marcus se aproxima de um `Interactable` válido. |
| `SceneFader.cs` | 484 | [[SceneFader]] | Gerencia transições entre cenas e zonas com efeito de lâmpada falhando (*flicker*), fade-to-black, sincronização segura de cenas do Build Profile e letreiro cinematográfico de chegada em cada região/interior (`AreaBannerCanvas`). |

---

## 5. Controle do Jogador, Câmera, Clima e Iluminação 2D

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `PlayerController.cs` | 553 | [[PlayerController]] | Controle top-down de Marcus em 4 direções com passo cadenciado, animação procedural/sprite-sheet, geração e posicionamento pixel-perfect da lanterna na mão (`LanternaNaMao` + feixe `Light2D`) e acionamento de `Interactable` com `E`. |
| `CameraFollow.cs` | 290 | [[CameraFollow]] | Perseguição suave (`SmoothDamp`) com alinhamento estrito à grade de 32 PPU e contenção automática ou manual pelas bordas da região/interior (`AplicarClampBordas`), impedindo que a câmera mostre áreas vazias fora do cenário. |
| `Chuva.cs` | 817 | [[Chuva]], [[Poca]], `Gota`, `Respingo`, `Camada` | Simulação visual e sonora de tempestade desenhada pixel a pixel via `NativeArray<Color32>` em duas camadas (gotas caindo no ar e respingos/ondinhas nas poças do chão), reagindo dinamicamente à luz dos postes e da lanterna de Marcus, com filtro passa-baixa automático em interiores (`abafado`). |
| `CamadaDeLuz.cs` | 130 | [[CamadaDeLuz]] | Agrupador de luzes URP 2D (`Light2D`) que permite acender, apagar ou transicionar estados atmosféricos inteiros do cenário (`"presente"`, `"memoria"`) multiplicando as intensidades autorais. |
| `AudioAmbiente.cs` | 107 | [[AudioAmbiente]], `Camada` | Mixer de camadas sonoras contínuas (`chuva`, `motor`, `pneus`, `radio`, `telefone`) com transições suaves de volume e pitch sem cortes secos. |
| `TileAleatorio.cs` | 61 | [[TileAleatorio]] | `TileBase` customizado que escolhe variantes de sprite e espelhamento determinístico por hash espacial `(x, y, semente)`, evitando repetição visual em pisos e terrenos. |

---

## 6. Prólogo Cinematográfico (Carro em 2004)

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `PrologoDirector.cs` | 360 | [[PrologoDirector]], [[FalaMarcada]] | Régua de tempo do prólogo interativo dentro do carro de Marcus em 2004. Coordena som no escuro, revelação da imagem, placas na rodovia, curva de entrada em Santa Rita, desaceleração, estacionamento e transição para `EntradaDaCidade`. |
| `PrologoVideo.cs` | 189 | [[PrologoVideo]], `Fala` | Alternativa de reprodução do prólogo via `VideoPlayer` (`StreamingAssets/Prologo_teste.mp4`) sincronizada com legendas roteirizadas e encerramento para `EntradaDaCidade`. |
| `EstradaScroller.cs` | 190 | [[EstradaScroller]], [[Fileira]] | Motor pseudo-3D de perspectiva que move árvores, postes, placas e casas do horizonte em direção ao para-brisa com suporte a curvatura lateral e fade de neblina. |
| `OlharPrimeiraPessoa.cs` | 93 | [[OlharPrimeiraPessoa]] | Desloca suavemente a câmera 2D dentro da arte expandida do interior do veículo conforme o movimento do mouse/analógico, somando a oscilação natural do carro. |
| `CamadaParallax.cs` | 33 | [[CamadaParallax]] | Desloca elementos do interior/exterior do carro proporcionalmente ao `OlharPrimeiraPessoa.Deslocamento` para criar paralaxe realista entre painel, para-brisa e estrada. |
| `LimpadorParabrisa.cs` | 105 | [[LimpadorParabrisa]] | Anima as palhetas do para-brisa com suavização mecânica e leve irregularidade temporal para funcionar como metrônomo orgânico da tempestade. |
| `ChuvaRolando.cs` | 39 | [[ChuvaRolando]] | Move ciclicamente a textura de chuva inclinada atrás do vidro do para-brisa durante o prólogo. |
| `PonteiroCarro.cs` | 71 | [[PonteiroCarro]] | Detecta via `Physics2D.OverlapPoint` qual `ObjetoCarro` está sob o cursor no painel do veículo e dispara a interação ao clicar. |
| `ObjetoCarro.cs` | 47 | [[ObjetoCarro]] | Classe base abstrata dos itens clicáveis dentro do carro, aplicando um leve clareamento visual (`clareamento`) quando apontados pelo cursor. |
| `CelularCarro.cs` | 77 | [[CelularCarro]] | Objeto clicável no carro: mostra as 4 chamadas não atendidas e permite tentar ligar novamente para Adílio até a linha cair na serra. |
| `RadioCarro.cs` | 73 | [[RadioCarro]] | Objeto clicável no carro: liga/desliga o rádio AM/FM com chiado e boletim meteorológico sobre a tempestade em Santa Rita. |
| `PortaLuvasCarro.cs` | 63 | [[PortaLuvasCarro]] | Objeto clicável no carro: revela o mapa dobrado com o endereço de Adílio (*Bairro das Casas, nº 40*) e a fotografia antiga de família. |

---

## 7. Menus, Pausa e Configurações Persistentes

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `MainMenuActions.cs` | 47 | [[MainMenuActions]] | Ações dos botões do Menu Principal: `NovoJogo()` (limpa estado e abre `Prologo`), `Continuar()` (abre modal de 5 slots do Orelhão via `PayphoneCheckpoint.AbrirModalCarregarDoMenu()`) e `Sair()`. |
| `MenuPausa.cs` | 553 | [[MenuPausa]] | Menu de pausa in-game (`ESC`). Constrói programaticamente a caixa de pausa e os subpainéis de opções, exibe o status atual do save manual e exige confirmação explícita caso o jogador tente voltar ao menu ou sair com `GameState.TemProgressoNaoSalvo`. |
| `MenuPanelTransition.cs` | 180 | [[MenuPanelTransition]] | Animação suave de deslize e fade (`CanvasGroup`) na alternância entre o painel principal do menu/pausa e o painel de configurações. |
| `OptionsPanelController.cs` | 290 | [[OptionsPanelController]] | Gerencia a navegação entre as categorias de configurações (`ÁUDIO`, `VÍDEO`, `LEGENDAS`) tanto no `MainMenu` quanto no `MenuPausa`. |
| `MenuItemSelector.cs` | 74 | [[MenuItemSelector]] | Controla o cursor indicador `>` e o efeito sonoro ao passar o mouse ou navegar por teclado/gamepad nos botões de menu. |
| `AudioSettingsController.cs` | 149 | [[AudioSettingsController]] | Controla e persiste em `PlayerPrefs` os volumes Geral, Música e Efeitos (`AudioMixer` em decibéis), garantindo inicialização auditiva segura em todas as cenas. |
| `VideoSettingsController.cs` | 191 | [[VideoSettingsController]] | Controla e persiste resolução de tela, modo Tela Cheia / Janela, VSync e brilho. |
| `SubtitleSettingsController.cs` | 129 | [[SubtitleSettingsController]] | Controla e persiste ativação de legendas, escala de tamanho (`MultiplicadorTamanho`) e fundo escuro com preview em tempo real. |

---

## 8. Automação de Editor, Montagem de Cenas e Testes Batchmode

| Script | Linhas | Classes / Tipos Principais | Responsabilidade Detalhada |
| :--- | :---: | :--- | :--- |
| `SantaRitaChapter1Builder.cs` | 818 | [[SantaRitaChapter1Builder]] | Script de Editor (`Assets/Editor/`) com `[InitializeOnLoad]` e menu `Santa Rita/Build & Validate Capítulo 1 Vertical Slice`. Gera/configura sprites de pistas e props, atualiza automaticamente as cenas `Prologo.unity`, `EntradaDaCidade.unity`, `CasaDoAvo.unity` e `BarDoZe.unity`, sincroniza o `EditorBuildSettings`, executa a suíte de validação automatizada R1 e gera o executável Windows x64. |
