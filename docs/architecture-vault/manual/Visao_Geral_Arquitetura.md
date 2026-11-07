# Arquitetura atual — C1/C2

Atualizado em 26/10/2026 a partir dos fontes atuais. [Inventário completo](Catalogo_Completo_Scripts.md).

| Sistema | Responsabilidades e fontes |
|---|---|
| Sessão/persistência | `GameState`, `CheckpointSystem`, `PayphoneCheckpoint`: RAM, snapshots, slots explícitos, save manual; isolamento de QA. |
| Mundo C1 | `Capitulo1Director.cs`, `InteracaoCapitulo1.cs`, `ChegadaEmSantaRita.cs`, `TransicaoDeZonaTrigger`: zonas e interiores, progressão inicial e retornos do Centro. As classes foram separadas; não procurar tudo no arquivo legado Chegada. |
| Mundo C2 | `Capitulo2Director`, `Capitulo2Interactable`, `Capitulo2Portal`, `Capitulo2Gate`, `SantaRitaGeography`; cenas independentes do Centro/interiores/cemitério e dados em `Resources/Cap2/Investigation.json`. |
| Investigação/UI | `GameMessageUI`, `MapaInterativoUI`, `Interactable`, `InteractionPrompt`, `UIFactory`: diálogo/exame próximo, mapa/zoom/legenda, diário e ações sem spoilers. |
| Movimento/luz | `PlayerController`, `SantaRitaFootsteps`, `SantaRitaAtmosphere`, `SantaRitaActorElevation`: poses, apoio/passo, lanterna F, luz, projeção localizada de desnível. |
| Ambientes | `SantaRitaRoomLayout`, `SantaRitaNpcWork`, `SantaRitaExitThreshold`, `SantaRitaShallowStairs`, `SantaRitaRoomCutaway`: trabalho, circulação, saídas, escada e paredes da Sociedade. |
| Sinuca | `SinucaMinigameUI`: jogo do Bar do Zé e retorno ao seletor explícito de saves após derrota. |
| Bocha | `BochaMinigameUI`, `BochaCourtView`, `BochaShot`, `BochaSimulation`, `BochaRules`, `BochaAI`: apresentação separada da simulação, força por clique/soltura, Ponto/Rafa/Volo, vistas/minimapa, IA, pontuação, colisões/rebotes e consequência. |
| Áudio | `AudioSettingsController`, `AudioAmbiente`, `SantaRitaFootsteps`, `SantaRitaBarAudio`, `Capitulo2Audio` e sistemas legados: música fornecida pelo autor, contexto/atenuação e efeitos por evento. Confirmar responsáveis reais antes de adicionar nova reprodução. |
| Autoria/QA | `Assets/Editor/` e verificadores `#if UNITY_EDITOR || DEVELOPMENT_BUILD`; fontes Aseprite em Arte/ e Tools/. Código de autoria e fixtures não entram no Player comum. |

## Contratos

- Nenhum autosave ao coletar pista, trocar de cena ou vencer minigame. Carga exige escolha de slot; não recarregar o último automaticamente.
- Portal manual aceita E sem janela global de espera; fade em andamento não pode ser sobrescrito. Passagens automáticas mantêm antirretorno e reset de relógio/sessão.
- C1/C2 compartilham estado e geografia; o cemitério é cena própria. Preservar compatibilidade de saves antigos e os dois caminhos de retorno.
- A cancha pertence à mesma cena do salão; o minigame usa apresentação própria coerente com a sala. Dimensão do desenho em mundo não precisa ser dimensão física da simulação.
- Arquitetura, luz e mobiliário orientam exploração; evitar títulos automáticos e a cidade organizada para receber Marcus.

Grafo/cofre são derivados. Extração AST de C# não verifica narrativa, cenas YAML, importação de sprites, luz renderizada ou qualidade artística; consultar fontes e relatórios próprios.

## Contratos Bar/Sinuca R4 (30/10/2026)

- `SantaRitaBarLayout`: definição física comum de mobiliário e atores do Interior4_BarDoZe; posições vivas e poses alimentam `SinucaTableView`. Não duplicar a planta do minigame. `SantaRitaBarHost` suspende trabalho do Zé durante o duelo e retoma depois.
- `SinucaBrasileiraRules`: núcleo independente de Unity, pontuação por valor/bola da vez, sequência, reposição, falta sete, recusa e término. `SinucaMinigameUI` traduz contato físico/conversões ao núcleo e conserva série melhor de três e callbacks narrativos. Bola marcada é uma regra local explícita. Não aplicar antigos testes de meta quatro.
- `SinucaSpinSelector`: contato contínuo na branca; `SinucaTableView` possui vistas diagonal, linha do taco e superior, projeção/inversa e redes. Sala 3D persistente em RenderTexture URP: renderer 3D no índice 1, Renderer2D no índice 0 preservado. A UI exibe um quad e não redesenha malhas do cenário a cada frame. A câmera da exploração tem sua máscara suspensa enquanto a sinuca opaca está aberta e a recupera ao fechar.
- `SantaRitaInventory`: cinco slots derivados de flags de GameState. Mapa mantém a chave legada para compatibilidade, mas aquisição nova ocorre só na Rodoviária. Diário não exige mapa. Recibo exige coleta/entrega explícitas por `SantaRitaItemAction`. Nenhum autosave.
- `Interactable.CanInteractAt` permite especializar proximidade; saídas C1/C2 consultam `SantaRitaExitThreshold.OnMat`. Mesma seleção produz prompt e E.

## Ref7 — apresentação do bar (31/10/2026)

`SinucaRoom3D` e seus parciais Scenery/Table/Actors constroem a geometria uma vez. A simulação continua 2D/120 Hz, com o mesmo núcleo de regras. As bolas/efeitos/redes recebem transformações dessa simulação; a reação da rede é uma animação visual amortecida, não um solver de tecido.

`SinucaTableView : RawImage` oferece projeção e raycast inversos, mouse absoluto e câmera independente da mira. A origem da sala fica próxima de zero para evitar erro de precisão no plano de contato; layer 30 e ativação da sala isolam a exploração. Vistas do taco têm paredes laterais; paredes intervenientes são recortadas ao consultar por fora.

`SantaRitaBarLayout` fornece âncoras, planta compacta e limites caminháveis. Paredes e mobiliário da exploração são derivados dos modelos; uma posição antiga fora dos limites é ajustada somente em RAM na chegada ao bar, sem regravar o save. `SinucaRef7Authoring` é ferramenta Editor explícita para materiais/importação/projeção de sprites; BarRef7 é exceção localizada no importador genérico por possuir atlas brutos e pivôs próprios.

Artigos de inventário usam imagens, cinco slots compactos e posse derivada do snapshot. A seleção de alvo explica o primeiro toque e desabilita opções ilegais. Não há botão/painel de presets de efeito: o controle é o disco de contato da branca. Nenhuma destas alterações cria persistência paralela.

## Correção de identidade 2D (31/10/2026, decisão posterior do autor)

A abordagem Ref7 de exportar personagens/móveis em blocos para a exploração foi rejeitada. `SantaRitaBarLayout.SpriteFor` agora é a fonte dos sprites nativos para ambas as apresentações; `NativeSurface` recupera os pixels do piso/parede do interior original. A exploração recebe `Art/WorldLit` como o restante do jogo. Não reaplicar o atlas AmbienteAtlas ou os PNGs Baked para mobiliar a exploração.

A sinuca usa sprites em malhas 2.5D com os UV/pivôs originais (`Sprite.vertices`, `uv`, `triangles`), além da mesa/bolas/redes com profundidade. `PixelSources` permite verificar a identidade de cada fonte. A pose de beber troca malha e textura originais juntas. Props sobre balcão/mesas possuem `elevation/supportOffset`, relacionando o apoio físico à representação 2D; placas permanecem presas à parede. Os núcleos de física/regras/saves continuam separados da apresentação.

`SinucaRef7Authoring.Prepare` só prepara renderer/materiais, e `SinucaPixelPreviewSession` captura uma fixture isolada. A exportação inversa de modelos para substituir sprites 2D foi removida; materiais/arquivos anteriores ficam preservados como histórico. Fontes novas em Arte/Producao/BarIdentidade2D e Lua reproduzível; não inserir ferramentas no Player.


## Sinuquinha de quinze bolas — decisão mais recente (31/10/2026)

Substitui a regra R4 de sete bolas descrita acima, que permanece histórica. `SinuquinhaRules` controla a partida atual: quinze objetos numerados e branca; dois grupos de sete, preta 8 por último; alternativa de identificação lisas/listradas ou 1–7/9–15. Zé oferece a escolha antes da saída e a branca já está posicionada. Não é par/ímpar nem declaração de regras WPA completas. `SinucaBrasileiraRules` e testes antigos de pontuação por valor ficam como histórico, sem autoridade sobre a partida nova.

`SinucaMinigameUI`/`SinucaTableView`/`SinucaRoom3D` usam buffers de dezesseis bolas e raio 5,4 unidades. Texturas esféricas numeradas/listradas giram pelo quaternion físico; a previsão inclui direção da bola de objeto. Órbita livre, ajuste de mira projetado na câmera e branca na mão em toda a superfície válida. A marcação/restrição do semicírculo D foi removida. A série melhor de três e callback narrativo permanecem.

`GameMessageUI` exibe documentos com texto tipográfico sobre papel e leitura integral rolável, em vez de ampliar glifos de um sprite genérico. Bandeira tem detalhe de exame separado de 256 px. `SantaRitaBarLayout` conserva fontes 2D comuns às vistas, capacho acima do piso, sem tapete sob a mesa; escala/posições de TV e geladeira revistas. `PlayerController` mantém apoio sonoro nos quadros 0/4 e lente alinhada à nova oscilação dos sprites.

Suíte atual: `Sinuquinha15Verification`, dispatcher `-srSinuquinhaQA`, somente Editor/Development. Evidências em Docs/Evidencias/Sinuquinha15; não usar suítes antigas de sete bolas como comprovação da regra nova.

Complemento posterior nesta execução: a exceção da marcada retorna nas mãos 2/3. SinuquinhaRules.Result.revertShot permite restaurar snapshot físico transitório (posições/atividade/rotações), sem tocar saves. Marcador é um X de giz visível; última bola/preta não bloqueiam. Contorno A/D, mira nas setas, direção da colorida com seta dourada, listas de encaçapadas e rotação superior. MenuPausa.AbrirCarregar reutiliza PayphoneCheckpoint.AbrirSelecaoCarregar e seus cinco slots, preservando pausa no cancelamento e encerrando sinuca antes da carga.

Capitulo2Audio passa a tocar os OGGs do autor por cena: Centro padrão C2, Cemitério/Abrigo e Sociedade exclusivos. Mantém fontes entre interiores e aplica filtro/volume sem reiniciar a faixa da mesma região. Inventário não habilita Image sem sprite e recupera ícones após importação tardia. Recursos LFS precisam ser materializados; extensão PNG não comprova conteúdo raster.

Inventário: fichas pertencem exclusivamente à aba do mapa; flags persistidas preservadas. Marcus usa ciclo autoral de corpo inteiro exportado pelo Aseprite, sem reconstrução procedural das pernas; relógio de distância/apoio continua em MarcusGait.

Acabamento posterior: inventário usa moldura nativa fina sem fatiar painel da pausa; exportador Marcus preserva poses e encorpa o jeans antes da exportação de lanterna.

Bar: SantaRitaBarLayout possui catálogo de poses e fase compartilhado com SinucaRoom3D. Cadeiras ocupadas integram o sprite sentado; CuiaMesa/TermicaMesa alternam visibilidade conforme manuseio. Marcus mantém contatos lógicos 0/4 com 16 poses visuais pelo QuadroDetalhado; frames sem deslocamento entre ticks conservam fase.

Marcus: exportador reconstrói duas pernas por pose sem deformações cumulativas. PlayerController valida dimensões/conteúdo/alpha das folhas antes de recortar; falha equipada mantém corpo normal, sem retângulo. Export-Marcus.ps1 aguarda ambos os processos e publica somente folhas completas.


## Atlas cartográfico — 02/11/2026

`SantaRitaAtlas`/`SantaRitaAtlasView` substituem a imagem/UV históricos na interface do jogador. `SantaRitaAtlasAuthoring` levanta sprites, Tilemaps, regiões e portas em fixture de Play para Resources/Atlas; ferramentas ficam no Editor. `Capitulo2Director.PosicaoNoMapa` delega à transformação única do atlas. Diário/fichas e aquisição permanecem em MapaInterativoUI; visitas/origem de entrada são flags/textos RAM compatíveis com snapshot v2 e save manual. Costuras entre cenas não são ruas nem rotas. [Contrato completo](../../Atlas_Arquitetura.md).


## Mapa da cidade e caderno — 03/11/2026

`MapaInterativoUI` mantém mapa/fichas/legenda e abre o `MarcusNotebookUI` como tela independente (J ou botão CADERNO). `IsShowing` inclui o livro para bloquear a exploração. Roda sobre painéis não altera zoom; a carta mantém zoom e pan anteriores. Fichas exibem apenas posse em ordem numérica, sem cupom.

`MarcusNotebookContent` projeta registros já adquiridos em primeira pessoa a partir de `Resources/Notebook/Entries.json`, sem mudar os documentos, flags ou fatos originais. Variantes condicionais dependem de conhecimento real. Registros legados sem versão própria são transcrições identificadas, sem novos fatos. `GameState.OnDiarioAtualizado(id)` notifica alteração material; `OnSnapshotRestaurado` reconcilia cache sem aviso de descoberta. `caderno.folha` é apenas marcador RAM/snapshot v2; salvar continua exclusivo do orelhão. Kalam tem licença OFL distribuída; TMP estático evita geração de fonte em jogo.

Entorno `Resources/Atlas/Paisagem.png` e rabiscos por descoberta não modificam a geometria, cenas ou acessos. A autoria de fonte/paisagem roda explicitamente fora de Play; não há reconstrução ao abrir o Editor. QA: `-srNotebookQA` em Development, além da regressão `-srAtlasQA`. Relatório: [caderno](../../Evidencias/Caderno/Relatorio.md).


## Pertences e fitas — 04/11/2026

SantaRitaInventory deriva posse das flags existentes/item.<id>; bolsa.ordem e bolsa.atalho.<n> são textos em RAM/snapshot v2. Cinco atalhos não são capacidade máxima: bolsa possui vinte células de apresentação para o catálogo atual de nove objetos. Fichas continuam no mapa, não itens. Contrato HasItem(int) preserva índices históricos.

SantaRitaTapeDeck gerencia modal, reparo de velocidade de Mila, seleção adquirida, controles e transcrição de Resources/Tapes/Catalog.json; clipes de voz são opcionais e ainda ausentes. SantaRitaTapePickup/Placement anexam fitas a objetos já existentes depois da carga, sem salvar/regenerar cenas. Conclusão anota fato pessoal, troca de cena interrompe. PlayerController/Mapa/Menu bloqueiam comandos concorrentes e ClosedFrame evita ESC abrir pausa por baixo. Capitulo2Audio atenua música durante consulta. Tudo permanece RAM até o orelhão.

MarcusNotebookUI mantém índice manuscrito e filtros de registros adquiridos. caderno.marcador.id/.parte resolve o marcador por nota mesmo com páginas adicionadas; caderno.folha é fallback. APIs de notas originais não substituídas.

MilaProductionVerification fica no Editor; MilaRuntimeVerification apenas Editor/DEBUG com flags explícitas e saves temporários. Chapter2BuildVerification agenda Exit para permitir finally restaurar configurações. Autoria vocal em Tools/Audio e segredos fora do repo/Player. [Relatório e limites](../../Evidencias/MilaInventario/Relatorio.md).


## Sinuca em primeira pessoa — 06/11/2026

SinucaMinigameUI preserva SinuquinhaRules e kernel 1/120. Caminhando3D é a entrada; LinhaTaco é a postura após aproximar/clicar na branca. SinucaRoom3D mantém posição de pé independente da bola, clearance da sala/mesa/props, caminho do Zé e rig 3D. SinucaTableView continua RawImage com render target ajustado ao aspect. Mão de empunhadura é filha do taco; apoio separado com altura da tabela. Carregado e tacada usam o mesmo taco 3D.

Cursor Locked usa deltas acumulados para força e relativos para olhar/mirar; Esc passa à UI sem disparar nem mover; MenuPausa deixa Esc da sinuca para ela enquanto não estiver aberto, e ClosedFrame impede consumir o fechamento duas vezes. HUD fornece Pausa. Fechar/desativar restaura cursor anterior; foco perdido cancela e libera. Nenhuma persistência nova. Zé planeja rota nos obstáculos e aguarda chegada/preparo antes do impulso, com apresentação da arte original por malha de passos.

Plano e limites: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-imersiva.md. Índices gerados desatualizados por ausência de Graphify.


### Revisão posterior: mãos e Zé — 06/11/2026

Substitui a descrição anterior do NPC móvel: Zé fica na posição original, sem taco, sem malha de passos, sem caminho e sem preparo físico. IA calcula e aplica impulso após espera. Mãos de Marcus mantêm contato em 3D mas orientam a face à câmera; o roll da empunhadura é apenas apresentação. Layer 29 contém exclusivamente taco/mãos em câmera URP Overlay com depth limpo e lente/projeção sincronizada; sala continua layer 30 no mesmo target. Assim, geometria da mesa não recorta os sprites. Caminhada exibe só taco. Nenhuma cena/persistência alterada. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-primeiro-plano.md. Renderização final ainda não conferida em Play.


Sinuca, pegada revisada 06/11/2026: GripScreenRoll calcula rotação do sprite frontal pelo eixo do taco em pixels; AlignGripToShaft mantém contato em -0.72 m do taco e desloca o dorso apenas em profundidade para ocultar o eixo sob a mão. Shader Resources/Art/Sinuca/MaoFlutuante usa dissolução fixa em UV apenas na terminação do pulso; não altera controle/física. Revisão nativa da renderização ainda pendente. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-fechada.md.


Sinuca 06/11 — apresentação atual das mãos: HandSprite usa duas células do atlas MaosMarcus, substituindo os recursos individuais antigos em runtime. AlignBridgeToShaft alinha ponto de apoio ao eixo e reparte a mesma arte em dorso/polegar por máscaras complementares de MaoFlutuante. PlaceHandDepth compensa escala ao mudar profundidade ao longo do raio de câmera. Controles/física/saves permanecem. Arte isolada aprovada; integração render ainda pendente. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-aprovadas.md


Sinuca, ajuste posterior 06/11: apenas empunhadura configura _ZTest Always e fila 2470 em MaoFlutuante, depois do taco/apoio. Alpha clip limita sobreposição à silhueta; apoio .30m segue LEqual e máscaras de profundidade. Imagem aprovada permanece. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-apoio-menor-oclusao.md


Sinuca 06/11: PlaceHandDepth aceita mirrorX para empunhadura esquerda; AlignGripToShaft aplica roll do eixo -12° no mesmo pivô. Apoio não espelhado. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-esquerda.md


Sinuca 06/11 — mão esquerda redesenhada: HandSprite carrega MaoEsquerdaTaco inteira para empunhadura e célula esquerda de MaosMarcus para apoio. mirrorX/roll -12° removidos; ângulo anatômico vem da arte, referência ao eixo projetado mantida. Novo pivô (.45,.50), largura .22m. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-esquerda-desenhada.md


06/11/2026 — SinucaMinigameUI contém SinucaZeComments, seletor transitório por resultado adjudicado (conversões/faltas/marcada e marcos de poucas bolas/preta por jogador). Legenda inferior independente com TMP maxVisibleCharacters, espera/fade e áudio abstrato dedicado, suspensa por foco/pausa/painéis; sem flags/saves. Treino e encerramento não mantêm falas. Capitulo1Director mantém fade/loop com volume de sinuca .23. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-falas-audio.md. Render/audição ainda pendentes.


Sinuca 06/11 — ambas mãos agora usam células de MaosMarcusUnificadas, pele/estilo gerados juntos; pegada superior já espelhada na arte. Apoio .21m nas duas camadas (redução adicional 30%); contato preservado. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-atlas-unificado-apoio-30.md


06/11/2026 — AudioSettingsController mantém PlayerPrefs como fonte única menu/pausa, sincroniza sliders por evento e OnEnable, vincula sem depender de Start, altera todos os mixers carregados e aplica master uma única vez no AudioListener. Defaults somente para chaves ausentes; música .35, efeitos 1. Ganhos de mixer .5 e contexto sinuca .35 não alteram preferências. Capitulo1Director preserva fade da trilha direta com sinuca .10. Fixture nativa e harness PASS; plano ../../ai-plans/2026-11-06_BBCONTTI_audio-global-sinuca.md.


06/11/2026 — AudioSettingsController expõe VolumeFala/VolumeChuva/VolumeImpactos com preferências próprias e seis sliders sincronizados menu/pausa. Chuva/MenuChuva bypassam SFX; AudioAmbiente separa camada chuva; SantaRitaBarAudio separa água/chuva do murmúrio; SinucaMinigameUI e TapeDeck leem fala/impactos. SinucaRoom3D.MovePlayer acumula distância pós-colisão para passo experimental de madeira via SantaRitaSound/Outros efeitos, sem afetar PeApoiou da exploração. Plano ../../ai-plans/2026-11-06_BBCONTTI_categorias-audio-passos.md.

Sinuca 06/11: PocketSlab recorta as seis passagens em todos os sólidos da mesa; SalaLit usa luzes URP com ambiente local. MaoFlutuante recebe luz sem alterar as máscaras de contato. Cylinder compartilha malhas de 6/16 lados. Sombras adicionais URP habilitadas para o único spot da sinuca. Sem alteração de cena ou regras.

Sinuca sala R2 (06/11): SinucaRoom3D.Furnishings constrói volumes no plano SantaRitaBarLayout apenas na sala em primeira pessoa; exploração 2D mantém fontes próprias. BuildSolidProp substitui móveis planos, Character conserva atores/decoração, props apoiados usam altura real .79m. BuildRoomArchitecture inclui forro recortado por câmera e luzes fracas. Nova revisão local posterior ao checkpoint 7cb8779e.

Sinuca 06/11, ajuste posterior: apresentação 3D omite todos os props Kind.Chair e sua colisão de caminhada; assentos ocupados já fazem parte dos sprites de NPCs. SantaRitaBarLayout e exploração 2D não mudam.

## Percepções da sinuca — 06/11/2026
SinucaMinigameUI_Perception separa estados transitórios de investigação/falsa bola das regras SinuquinhaRules. Renderização falsa reutiliza aparência real, mas não integra a lista física; guia percebida pode apontar para ela. E examina/abaixa, E/Q/Esc encerra inspeção; pausa/foco suspendem atualização. SinucaRoom3D_Perception controla câmera baixa, forro inferior e três pequenas gotas; não há poça. Fila narrativa compartilha legenda e sílabas, com identificação de locutor e pitch Marcus .77/Zé 1. Impacto sob mesa usa Foley/door CC0; sem autosave/flags novas. Verificação final visual/auditiva e campanha pendentes. Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-percepcoes.md.


06/11/2026 — Colisões bola-bola: SinucaMinigameUI.GarantirColisoesGravadas pré-carrega três Resources/Audio/Sinuca/Colisao_*.wav no Awake. TocarSomColisaoBola alterna gravações CC0 com pitch fixo, ganho por impulso e gate compartilhado 18 ms; TocarClipImpacto aplica VolumeImpactos e pool/pausa/foco existentes. Tacada/tabela/caçapa/giz mantêm síntese. Procedência e QA: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-bolas-reais.md.

## Zé na sinuca — 06/11/2026
`SinucaRoom3D.Opponent` apresenta o adversário com sprites novos `Art/Characters/Npc_Ze_Sinuca` (16 células, 128 px, PPU 64 em runtime), caminho com colisão geométrica e taco independente do view-model de Marcus. A coroutine de `SinucaMinigameUI` avança caminho/pose somente com apresentação ativa e dispara a física no contato a 82% da preparação. Layout/regras/saves permanecem separados; NPCs restantes usam a apresentação existente. Evidência local e limites no plano ze-sinuca-sprites-movimento.


06/11 — SinucaRoom3D.Opponent mantém heading de mundo; UpdateOpponentView é chamado a cada UpdateCamera e seleciona 24 células direcionais sem reiniciar a pose. Comentários de Zé só cedem às batidas em curso e à inspeção/fila narrativa ativa, sem depender da conclusão de inspeção opcional. Sem APIs públicas novas nem alterações de saves/cenas.


## Agenda das percepções — 06/11/2026
`SinucaPerceptionSchedule` é um núcleo C# sem Unity, física, flags ou persistência: controla Active/Age/Remaining, bolsa embaralhada e gates de tempo/turnos. `SinucaMinigameUI_Perception` faz a apresentação dos eventos UnderTable/FalseBall/WaterInPipes/PartyEcho; só a UI decide quando é seguro começar. Fim de mão cancela apresentação, preservando a agenda da série; reset apenas em treino/nova série/fechamento. Fila de falas bloqueia novo sorteio; áudio/painéis/foco/pausa respeitam suspensão existente. Referência e limites em Docs/Lore_Atual.md; testes reproduzíveis em Tools/QA/SinucaPerceptionSchedule. Graphify não foi regenerado: binário local ausente.


## Sinuca: efeito e apoio — 06/11/2026
SinucaMinigameUI.Spin centraliza ajuste contextual por direito + arraste e reutiliza contatoTaco da física. SinucaRoom3D.SetCueContact/SetSpinFocus controlam apresentação. AlignBridgeToShaft intersecta haste com plano de apoio, preservando o canal palma/polegar na mudança de efeito e estabilidade na puxada. Não adiciona persistência, cena ou regra de progressão. [Plano](../../ai-plans/2026-11-06_BBCONTTI_sinuca-efeito-apoio.md).


## Bocha — ambientação 07/11/2026
BochaCourtView apresenta a sala persistente de BochaRoom3D em RenderTexture via renderer 3D da sinuca. BuildAtmosphere adiciona arquitetura e decoração funcional; arandelas/pendentes secundários mantêm a principal da cancha dominante. Reboco independe de NativeSurface do bar. Obstáculos novos usam CanStand; sem alteração de cenas/kernel/persistência. Fixture Editor: 273 pontos livres e quatro vistas. Player e aprovação artística pendentes. Graphify ausente/índice antigo.

## Tonão na bocha

`BochaAI` escolhe o lançamento na simulação existente. `BochaRoom3D` move Tonão pela faixa livre e devolve o sinal de soltura quando troca para o quadro da mão direita aberta. `BochaMinigameUI` usa esse sinal para lançar no kernel. Legendas de percepção de Marcus têm prioridade sobre as falas de Tonão.


## Percepções da bocha — 07/11/2026

`MinigamePerceptionSchedule<T>` fornece a bolsa, os intervalos e os turnos de elegibilidade. `SinucaPerceptionSchedule` conserva os quatro eventos e seu contrato. `BochaMinigameUI.Perception` define quando iniciar as duas ocorrências, controla fala e efeitos e retém o passo do solver durante a animação de queda. `BochaRoom3D.Perception` controla figura, teste de olhar e visualização da bocha que cai. A abertura de `SalaLit` é configurada só nos materiais do piso e das marcas da bocha. [Plano e verificações](../../ai-plans/2026-11-07_BBCONTTI_bocha-percepcoes.md).

## Mixagem compartilhada dos minigames

AudioSettingsController separa o ganho de referência do valor relativo persistido no perfil `audio_perfil_sinuca_`. Menu e pausa usam o mesmo perfil, aplicado aos mixers e às fontes diretas. MinigameDialogue usa SubtitleSettingsController e mantém a apresentação da fala e o áudio de sílabas em sinuca e bocha. As filas e a prioridade dos locutores ficam em cada jogo. [Plano](../../ai-plans/2026-11-07_BBCONTTI_mix-global-minigames.md).

07/11/2026 — SantaRitaBarLayout mantém a planta de referência para SinucaRoom3D e define uma projeção 2D mais larga. SantaRitaBarLighting monta as luzes e sombras na exploração. SceneFader coordena a íris texturizada e SinucaRoom3D aplica um olhar de entrada sobre a câmera inicial antes da escolha de grupos. [Plano](../../ai-plans/2026-11-07_BBCONTTI_sinuca-sala-entrada.md).

## Sinuca, console e janelas — 07/11/2026

SinucaPerceptionSchedule controla cinco eventos e StartForced permite comandos manuais com o mesmo fluxo de apresentação. SinucaMinigameUI.Console oferece campo e botões. Sustos controla apagão, redistribuição, piscada e medo. Janela controla batidas e inspeção opcional por E. MinigamePerceptionAudio compartilha a síntese de coração com bocha. SinucaRoom3D.Salon monta um quad de vidro por lateral, material compartilhado e linha do atlas por MaterialPropertyBlock. JanelaMolhada calcula paralaxe de fundo/primeiro plano, chuva e gotas na GPU, com preferência de redução de efeitos.
