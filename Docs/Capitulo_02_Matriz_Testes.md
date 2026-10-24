# Capítulo 2 — matriz de verificação

Bocha revisada em 23/10/2026: [relatório QA/PO](Evidencias/BochaPerspectiva/Relatorio.md). 28 regressões de física/carga, 117 verificações gerais e 70 verificações no Player Development com a sala atual; build comum do mesmo conteúdo. Resultados e limites desta revisão prevalecem sobre as contagens históricas abaixo para a Bocha.

Esta matriz separa automação, inspeção de código/imagens e playtest humano. `Parcial` não equivale a teste integral aprovado. Consolidação atual em [C2 R2](Consolidacao_C2_R2.md), com resultados em `Evidencias/C2R2`. `Evidencias/CidadeViva` conserva a revisão anterior e o relatório atualizado de build normal.

## R2 — resultados atuais

| Área | Evidência e alcance |
|---|---|
| Integração narrativa | Despachante/callbacks reais, primeiras visitas antes/depois da Bocha, opcionais e telefonema tardio; protocolo autorizado e fontes corrigidas. Passou em QA. |
| Ambientes | Dez cenas C2, portas/spawns/alcance e câmera real; oito interiores com plantas próprias. Sociedade: cancha horizontal 22 × 6, salão 30 × 19. Composição vista em câmera de jogo; aprovação artística não presumida. |
| Lanterna F | 290 checks R2 incluem aquisição, pressão/segurar, bloqueios modais, foco, legado, persistência manual, direções e viradas. Pares na build normal e GIFs no Editor; limites de input físico descritos no índice. |
| Escolha após derrota | Sinuca e Bocha: cinco registros, cancelamento preserva resultado, carga do slot escolhido em vez do mais recente; caso sem save. Passou no executável QA. |
| Saves e C1 | Migração de checkpoint antigo da cancha para área conectada à saída; 63 verificações C1 passaram na build QA. Player comum carregou cópia intacta de save antigo junto ao Bar. Cinco arquivos pessoais idênticos por SHA256 após restauração. |
| Build/IA | QA e normal sem erros; assembly comum sem autoria/harness. Decisão síncrona Bocha: 4,33/18,06/50,32 ms nos três cenários, nesta máquina. |
| T01 e audição | Parcial: Novo Jogo/prólogo/chegada tentados no Player comum; teclas da automação não recebidas. Sem campanha integral legítima aprovada, mixagem humana ou avaliação de diversão/medo. |

| Teste | Evidência / limite de cobertura |
|---|---|
| T01 | **Pendente:** novo jogo atravessando C1 e C2 sem QA/debug. Não foi realizado por pessoa nesta execução. |
| T02 | Save pré-Zé em armazenamento isolado mantém bloqueio do Centro; porta de serviço também não permite contorno. |
| T03 | **Parcial:** formato v2 e snapshot pós-Zé em fixture preservam ficha/pistas e atravessam a continuação; não havia save legítimo pós-Zé do autor disponível. |
| T04 | **Parcial:** resolver real da Sinuca e limiar aprovado foram executados; a campanha completa com vitória legítima na mesma sessão permanece ligada a T01. |
| T05 | Limiar real termina seu diálogo e abre Centro; a tela antiga de demonstração não é mais chamada. |
| T06 | Portas de ida/volta das dez cenas executadas; serviço retorna à posição da loja original. Spawns e acessos também conferidos no Editor. |
| T07 | **Parcial:** câmeras observadas em capturas de todas as cenas; navegação amostrada por colisão. Caminhar manualmente por todos os corredores/bordas ainda é playtest. |
| T08 | Depósito entra e retorna ao mesmo Mercadinho de C1; sem segunda frente ou segunda loja. |
| T09 | Planta comparada à referência: Cemitério norte, Sociedade sudoeste na margem urbana; tabela de coordenadas no plano. |
| T10 | Gate pré-Zé testado; destinos futuros sem portas utilizáveis; perímetro com cerca/vegetação, ponte em manutenção. |
| T11 | Tonão visitado cedo não abre duelo; Cemitério antes da Prefeitura tem leitura válida. |
| T12 | Comparação do memorial reconcilia fontes fora de ordem; objetivo crítico se completa antes das linhas opcionais. |
| T13 | Repetição pós-Bocha não duplica ficha nem diário. Flags e notas possuem identificadores estáveis. |
| T14 | **Parcial:** snapshot/recarga e posição foram exercitados, inclusive recarga real pós-vitória; não foi carregado cada estágio intermediário da investigação. |
| T15 | Requisitos de Tonão satisfeitos antes de rádio/costura; opcionais não condicionam o boss. |
| T16 | Revisão dos textos novos: fontes e limites identificados, datas alinhadas e nenhuma identidade atual/sobrevivência de Lucas revelada. |
| T17 | Oito interiores produzidos, percorridos por portas e capturados com suas funções; Escola excluída pela decisão expressa do autor. |
| T18 | Regras de estoque, repouso, turnos e lançamentos executadas; cancelamento por foco e pausa exercitado. Uso humano contínuo do mouse/teclado ainda ligado a T01. |
| T19 | Simulação executa aproximação e colisões/remoção, inclusive deslocamento e saída do balim; IA usa o mesmo kernel. |
| T20 | Medição em repouso, tolerância/empate, um e múltiplos pontos: verificações puras no Unity. |
| T21 | Estoque esgotado, lado sem válidas e ambos sem válidas: verificações puras no Unity. |
| T22 | Cabeceiras fechadas com rebote de bochas e balim; invalidação defensiva, anulação, repetição e proteção contra dupla contagem: verificações de regras/física. |
| T23 | Vencedor somente após resolução; partidas completas reais de vitória e derrota executadas pela UI de QA. |
| T24 | Piso seco/úmido comparado e clone da IA preserva faixa; faixa fixa durante a mão e anunciada na UI. |
| T25 | **Parcial:** pausa de mira/carga e perda de foco testadas; não foi exercitada uma pausa humana em cada fase/efeito possível. |
| T26 | IA venceu estratégia fraca e perdeu para estratégia forte nas regras; vitória e derrota também exercitadas na UI. Sem terceira fase forçada. |
| T27 | Redimensionamento real pausa sem consumir estoque; callback de perda de foco cancela carga sem consumir estoque. Não substitui avaliação humana em monitores diferentes. |
| T28 | Carregar snapshot anterior descarta vitória sem orelhão; nenhum save automático de boss. |
| T29 | Orelhão real grava segunda ficha; recarga pelo fluxo público restaura consequência e posição. |
| T30 | Derrota real na UI oferece último orelhão; escopo isolado vazio oferece só menu, sem criar checkpoint. |
| T31 | Arquivo inválido, versão futura e cena desconhecida recusados; leitura preserva arquivo e outro slot válido. |
| T32 | Snapshot v2 conserva estado C1 e não cria vitória C2; incompatibilidade futura não migra silenciosamente. |
| T33 | Armazenamento e preferências próprios; namespace temporário, estado anterior em RAM restaurado. |
| T34 | **Parcial:** Sinuca física/resolver/invariantes, cenas antigas, revisitas e inventário foram conferidos. Controles completos e audição das músicas de C1 ainda pedem playtest humano. |
| T35 | Compilação e referência de cenas verificadas; falhas nativas/CPU de C1 encontradas, corrigidas e retestadas. Percurso final sem exceção gerenciada. |
| T36 | Uma chuva no retorno; ambiências com fade e impactos só por contato na implementação. **Mixagem e continuidade por audição humana pendentes.** |
| T37 | Capturas de texto/retratos, transcrições roláveis, mapa/legenda e resoluções diferentes inspecionadas. |
| T38 | Build Development e build de jogador geradas sem erros; percurso completo em Development passou. Startup do jogador registrado separadamente. |
| T39 | Comparação controlada em `Final/Desempenho`: C1 e C2 com média de 16,68 ms, 1280×720, vSync=0, limite 60 FPS, 90 frames/cena. Não é perfil completo nem garantia de FPS em outro PC. |
| T40 | Revisão visual identificou e corrigiu recortes de sprites, sobreposição de calçada/asfalto, leitura cortada, banners técnicos, textura periódica e espaços vazios. **Ritmo e diversão continuam pendentes de pessoa.** |

## Problemas encontrados e regressões permanentes

1. `level2 / Position out of bounds`: scripts secundários em um único arquivo eram serializados localmente na cena. Componentes separados em arquivos com nome correspondente; referências persistentes. Verificação impede novo MonoScript local antes do build.
2. `Texture ... is not readable`: C1 compõe pisos lendo pixels; quinze importadores dos terrenos habilitam CPU. A verificação deriva do conjunto usado pelo montador, sem alterar PNGs ou escala.
3. E repetido reiniciava fechamento do exame: estado de fechamento impede reentrada; textos longos agora têm rolagem.
4. Móveis/árvores carregavam primeiro fragmento do atlas: cópias integrais e importação própria de C2, comparadas por hash.
5. Estampas de passeio apagavam a rua: rasterização por célula, preservação do asfalto nos encontros com terra e seis amostras de regressão dos eixos.

6. A build de jogador removia recursos de pós-processamento: desativada a remoção no registro ativo de `URPShaderStrippingSetting`; o registro legado já estava desativado, mas não era a configuração efetiva. Reteste da abertura registrado à parte.

Não remover a distinção entre estes resultados e T01 ao apresentar a entrega.

## Auditoria dos retornos antigos — 22/10/2026

Comparação com o anexo do autor. Baseline `fbd2d42` (`capitulo_2`); correções locais sem commit.

| Retornos | Resultado da revisão |
|---|---|
| Auto-setup, marcador de Library, Bar e arte aprovada | Callback já desligado; método destrutivo ainda existia. Removidos auto-setup e apagamento/cópia do Bar. O comando C1 só verifica/compila: não gera sprites, altera importadores ou reconstrói cenas. |
| Isolamento, ausência de save, resíduos e Clear | Isolamento já implementado; `Clear` continua cancelando somente spawn. Escopo restaura também spawn/indicador de progresso anteriores; limpar RAM em QA não toca preferências legadas pessoais. |
| Recibo, flags, diário e orelhão | Corrigidos. Visitar a casa não apresenta documento. Apresentação e relato têm flags específicas; testemunho só é registrado ao concluir a conversa real. Conversa genérica não antecipa diário, fala contextual ou objetivo. Revisita idempotente. |
| Café, cartas, desordem e telefone | Observações/hipóteses proporcionais. Três objetos sem o bilhete não inventam sua leitura nem revelam o Bar. A pergunta a Zé consulta o conhecimento do bilhete. |
| ID técnico e penalidade da marcada | Identificador retirado da fala; Zé/frequentador explicam anulação dos pontos da tacada, reposição e branca na mão, sem ponto para o adversário. |
| Ordem da tabela | Contato anterior à primeira bola numerada não satisfaz a regra. Física/resolver reais usados na regressão. |
| Esc e regras nas esperas da IA | Pausa global recebe Esc sobre a Sinuca. Regras não substituem o estado da mesa; esperas conferem o painel antes de continuar. Fechar/recomeçar cancela rotinas anteriores; suspender cancela carga. |
| Áudio por etapa | Texturas originais sintetizadas: murmúrio sem palavras na primeira mão, corte do murmúrio/água ressonante na segunda, chuva abafada na terceira. Suno preservado. Duração, pico e bordas testados; audição humana pendente. |
| Slot 1 e atualização de nota | Corrigidos. Só JSON legado sem `slotIndex` aparece no primeiro slot; moderno de outro slot não duplica. Nota alterada marca progresso e notifica UI. |
| Capela, troca Prefeitura/Escola e geografia | `SantaRitaGeography` centraliza marcos/projeção/objetivos/cenas. Capela no meio inferior; Prefeitura no antigo local Escola, com acesso/retorno. Escola futura. Arquivo de imagem preservado; rótulos históricos corrigidos por texto de UI. |
| Cenas C2 no mapa, marcadores e novas raízes | Reconhecimento C2 já existia; centralizado. Removida desativação de raízes desconhecidas em C1; só objetos explícitos do protótipo são ocultados. Acessos e posições serializadas conferidos na suíte de cenas. |
| Continuação, escala e mundo conectado | Já presentes: Centro jogável, ficha/pistas mantidas, dois retornos a C1 e cemitério separado. Unity 6000.6.0f1/32 PPU preservados. |
| Teste real da tacada e regressão C1 | Teste manual de reposição substituído pelo resolver da partida: marcada + comum, quarta bola + branca, séries 2–0/2–1 e derrota. Entradas controladas não equivalem a vitória humana. Runtime verifica investigação fora de ordem, revisita e pausa. |
| Build atual e campanha legítima | Relatórios atuais distinguem compilação, runtime e inspeção. Falha histórica não comprova falha atual. T01 continua separado: campanha integral sem fixture/debug, arte/diversão e mixagem não são certificados por automação. |

O primeiro executável desta revisão revelou um problema adicional real: `CemiterioSantaRita` existia no projeto, mas faltava na lista persistida de cenas da build. Registrada a cena e adicionada validação dos destinos de todas as portas antes de compilar. A ferramenta de build restaura e salva APIs gráficas, lista de cenas e assets pré-carregados temporários. Criar cenas C2 ausentes também deixou de reimportar incondicionalmente a arte aprovada.

`Chapter2Verification.AuditOldFeedback()` reproduziu cinco falhas antes das correções no Unity: diário, testemunho antecipado, slot duplicado, tabela fora de ordem e IA tacando com regras abertas. Após correção, os onze grupos dessa regressão passaram. Suíte atual: 125 verificações, além de 98 contratos de produção. Saída intermediária em `Library/sr-feedback-audit.txt` (local/ignorada); resultados finais em `Evidencias/CidadeViva/Relatorio.md`.
