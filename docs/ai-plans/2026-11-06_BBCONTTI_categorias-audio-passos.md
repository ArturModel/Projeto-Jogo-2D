# Categorias de áudio independentes e passo discreto na sinuca

Autor @BBCONTTI (git config user.name); execução Codex. Data 06/11/2026, America/Sao_Paulo.
Ambiente Windows / C:/Temp/SantaRita / Unity 6000.6.0f1 / melhoria-minigames.
Status implementado; compilação, harnesses e testes de categorias no Unity PASS. Audição subjetiva e Player independente não executados.

## Pedidos

Separar volumes de fala, música, chuva e batidas de taco/bolas, pois reduzir o controle compartilhado deixava quase tudo mudo. Complemento recebido durante a implementação: passo básico e bem leve ao caminhar na sala da sinuca, para experimentar.

## Contexto e diagnóstico

AGENTS, memória/cofre (índice, estado, visão geral, catálogo e cenas), direção/critérios de áudio e skills locais santa-rita-audio/qa consultados. Graphify query indisponível; fontes reais confirmadas antes de editar. Instância MCP confirmada pelo project/info como C:/Temp/SantaRita/Projeto Jogo 2D.

SinucaMinigameUI usava audio_efeitos tanto para voz abstrata de Zé como para impactos. SantaRitaBarAudio usava a mesma preferência para água/chuva. Chuva e a fonte MenuChuva passavam pelo mixer SFX. O jogador 2D já possui passos por PeApoiou; a câmera caminhável da sala 3D tinha movimento próprio sem evento sonoro, portanto não se adicionou outro relógio ao PlayerController.

## Executado

- [x] AudioSettingsController recebe preferências e sliders separados audio_fala, audio_chuva e audio_impactos. Defaults novos 1/.55/1, independentes do valor legado de efeitos; escolhas existentes preservadas. Volume geral continua aplicado uma vez no AudioListener.
- [x] Menu principal cria as três linhas adicionais reutilizando os controles/rotulagem existentes em runtime, sem salvar/regenerar MainMenu.unity. Organiza seis linhas: Geral, Música, Fala, Chuva, Taco e bolas, Outros efeitos. Pausa monta as mesmas seis linhas. Vinculação/sincronização e persistência continuam compartilhadas.
- [x] Fala controla murmúrios de Zé e áudio das fitas. Impactos controla eventos de sinuca (taco/bola/tabela/caçapa/giz). Outros efeitos mantém passos/interface/foley e murmúrio ambiente do bar. Nenhuma categoria nova é multiplicada por Outros efeitos.
- [x] Chuva controla fonte MenuChuva (desvinculada do SFX), as duas fontes de Chuva da gameplay, camada chuva do prólogo e água/chuva abstratas do bar. Seus fades/intensidade/filtros existentes preservados; fontes de chuva usam AudioListener diretamente para não multiplicar Outros efeitos.
- [x] Passo de madeira existente Kenney CC0 em SinucaRoom3D.MovePlayer: acumula distância realmente percorrida após colisão, emite a cada .62 m, alterna três clipes, ganho .065 (contra .18 da exploração). Para ao parar/olhar/colidir e não emite sem foco/na pausa. Usa SantaRitaSound e Outros efeitos; sem novo áudio binário ou duplicação dos passos 2D.
- [x] Testes e evidências executados. Não alterados saves, flags, progressão, cenas no disco ou callbacks narrativos. Alterações preexistentes de mãos/arte preservadas. Sem commit/push.

## Arquivos

Scripts: AudioSettingsController.cs, MenuPausa.cs, SinucaMinigameUI.cs, SinucaRoom3D.cs, Chuva.cs, AudioAmbiente.cs, SantaRitaBarAudio.cs, SantaRitaTapeDeck.cs.

Harnesses: Tools/QA/verify-audio-settings.py, verify-sinuca-dialogue.py e verify-sinuca-walking.py. Logs e resultado MCP em Docs/Evidencias/SinucaMecanicas/categorias-qa.txt, categorias-fala-qa.txt, categorias-unity.json, passos-qa.txt e compilacao-categorias-passos.txt. Fontes de passos já constam em ASSET_SOURCES.csv com licença CC0; sem novo arquivo artístico.

## Validação

- **29 PASS** no controlador C# isolado: defaults independentes, mute de cada categoria sem mudar as demais, sincronização entre menu/pausa, reabertura, preferências e ganhos da revisão anterior. Somente criação visual nativa de UI/descoberta da fonte de chuva são excluídas deste harness.
- **18 PASS nativos Unity**: seis referências de slider presentes em cada menu; efeito geral zero mantém novas categorias; fonte MenuChuva real fora de SFX e volume correto; zero de chuva silencia essa fonte sem mudar impactos; fala/impactos independentes; música escolhida preservada. Preferências restauradas em finally. Resultado bruto em categorias-unity.json.
- **27 PASS** da fala/legenda/murmúrio depois da troca de categoria, com doubles de áudio/UI.
- **75 PASS** do movimento real extraído: um passo por deslocamento, silêncio ao olhar/colidir/sem foco, regressão de circulação/alcance. A primeira execução falhou por campos novos ausentes no harness, corrigido e reexecutado. Unity confirmou carga dos três clipes footstep_wood_000/001/002 (~.25 s).
- Assembly-CSharp: **0 erros / 16 avisos CS0649**. Console Unity consultado sem erros após recarga de categorias. git diff --check dos scripts passou.

Tentativas de capturar os painéis foram afetadas pela sessão em uso/reloads: captura audio-categorias-menu.png mostra a sinuca, não o painel, e não comprova layout. Não afirmar revisão visual concluída a partir dela. Fixtures de painel não salvaram cenas; a sessão já estava novamente na sinuca durante as consultas seguintes. Audição da mix e composição visual dos seis controles ainda dependem de conferência em jogo; não houve aprovação artística presumida.

Refresh e validate Graphify executados: CLI ausente e hashes desatualizados, respectivamente. Logs graphify-categorias-{refresh,validate}.txt; índices não falsificados.

## Pendências

Ouvir o passo experimental em jogo e ajustar por preferência do autor. Conferir layout de seis controles no menu/pausa e Player final. Restaurar Graphify e regenerar índices.
