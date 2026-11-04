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
