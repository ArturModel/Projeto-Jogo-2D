# Capítulo 2 — execução e evidências

## Autoridade e baseline

- Pedido de implementação completo: `Prompt_Codex_Capitulo_02_R1.md`, diretriz `Capitulo_02_Diretriz_Exploracao.md` e `Referencias/Mapa_Capitulo_02_Delimitacao_Aprovada.png`.
- O prompt não existia em Docs no início; foi copiado do TXT enviado, sem alterar seu conteúdo. Bíblia R1 copiada em uma única representação Markdown.
- **Decisão explícita do autor nesta execução:** a Escola permanece para o próximo capítulo, fora da linha azul. Isso substitui sua inclusão no TXT. Não mover a Escola para caber neste recorte.
- Git inicial limpo; branch existente `capitulo_2`; commit `aebf3dc` (`ajustes`). Sem commits/pushes automáticos.
- Unity 6000.6.0f1, URP 17.6.0, Input System 1.20.0. Escala aprovada real: 32 PPU, personagem em célula 64×64, câmera 512×288, UI 640×360. Não aplicar os valores históricos 16 PPU/320×180.
- Capítulo 1: cinco áreas externas, quatro interiores em zonas na cena EntradaDaCidade; cenas CasaDoAvo e BarDoZe também existem como suporte. Sinuca autoral melhor de três, sete bolas compartilhadas, quatro pontos/mão, bola marcada a partir da segunda mão.
- Save atual v2, cinco slots, snapshot de flags/números/textos/diário em RAM; só orelhões gravam. Há saves pessoais pré-Zé; leitura apenas, sem sobrescrever. Testes usarão armazenamento isolado.
- Editor aberto observado em MainMenu. Referência visual e execução do trecho final ainda serão registradas no marco de integração; análise de código não conta como playtest.

## Contrato C1 → C2

`venceu_ze_sinuca` → portão da Rua Norte → Marcus procura Adílio e acredita que Lucas morreu → ficha `ficha_clube_ze_01`, mapa/lanterna/pistas mantidos → Zé diz que dois homens passaram com Adílio e menciona registros da estrada de 1998 → paradeiro e autoria continuam desconhecidos → consultar o plantão público da Prefeitura e pessoas que tiveram contato com Adílio.

A fonte do relato herdado é Zé, não uma confirmação onisciente. Preservar a fala aprovada; confrontar seus limites na continuação. Substituir apenas a tela de demonstração no limiar. O bloqueio pré-vitória continua.

## Correspondência geográfica de trabalho

Coordenadas abaixo são unidades de produção, não medidas oficiais da ilustração. Exterior C2 usa a transformação da imagem de referência `x=(pixelX−200)×0,32`, `y=(355−pixelY)×0,32` (referência 676×503). O norte permanece acima.

| Marco | Posição de trabalho aproximada | Relação e conexão |
|---|---|---|
| Retorno Bar do Zé | (28, 80) | Noroeste; retorno à Rua Norte original, spawn junto ao portão |
| Fundos do Mercadinho | (20, 50) | Oeste da praça; depósito do mesmo comércio, ligação de serviço com C1 só após Zé |
| Praça da Matriz | (44, 40) | Núcleo central, ruas integradas e dois acessos de residências existentes |
| Igreja | (45, 67) | Acima da praça, abaixo/sudoeste do Cemitério |
| Prefeitura | (69, 48) | Leste do Centro, entre praça e aproximação do Clube; seguir letreiro da referência mais recente |
| Rodoviária | (68, 25) | Inferior/leste do núcleo; acesso público e telefone |
| Cemitério | (62, 91) | Norte do Centro, a leste do Bar; portão e abrigo local, sem túmulo de Lucas |
| Sociedade Beira-Rio | (4, 6) | Sudoeste, na margem urbana do rio; salão, cancha e orelhão |
| Oficina de rádio / casa de costura | (11, 34)/(22, 29) | Duas fachadas já ilustradas a oeste/sudoeste da praça; usos distintos |
| Rio e ponte | margem sul, ponte a leste da Sociedade | Ponte preservada como referência; outra margem fora da produção |
| Escola/Clube/Operário/Truco/Capela | lado leste/sul da linha | Sem produção de interiores; nenhum capítulo seguinte implementado |

Cada porta será registrada com cena/spawn/direção no inventário gerado da autoria. Capturas de planta e jogo devem ser comparadas com essa tabela antes do fechamento.

## Cadeia narrativa e funções

| ID | Pergunta/ação | Fonte e limite | Persistência/revisita |
|---|---|---|---|
| C2-01 | O que Adílio buscava nos registros? Consultar recepção pública da Prefeitura | Pedido público do próprio Adílio; prova consulta anterior, não localização atual | Protocolo no diário; funcionária diferencia pedido de paradeiro |
| C2-02 | Que versão se repetiu em Santa Rita? Examinar homenagem na praça | Texto público seletivo, não verdade objetiva | Releitura após testemunho/Bocha |
| C2-03 | Júlia respondia? Conversar com padre e observar nave/apoio | Testemunha presencial parcial; não explica sobrenatural | Depoimento, revisita e ocorrência perceptiva única |
| C2-04 | Como se tentou resgatar? Comparar cópia pública de manutenção e memória do Cemitério | Material de manutenção + luto; não identifica sequestradores | Comparação explícita e recuperável |
| C2-05 | Por que Tonão agora? Seguir a consulta de Adílio à Sociedade | Zeladora e registro de visita anterior ao desaparecimento | Conversa precoce válida, requisitos legíveis |
| C2-06 | Ler o piso e confrontar a insistência | Prática opcional e duelo de Bocha | Quatro bochas/lado, seis pontos, sem checkpoint |
| C2-07 | O que Tonão admite e o que sabe hoje? | Relato próprio + cópia de recado entregue a Adílio; plano não prova deslocamento posterior | Ficha #02 única, diário, retornos e objetivo limitado a Amaro |
| C2-08 | Relatar ausência / salvar / retomar controle | Telefonia funcional da Rodoviária e orelhões manuais | Nenhuma gravação automática |

Opcionais com conclusão: oficina (Júlia como pessoa e aparelhos, sem novas fitas); costura (homenagem e escolha das palavras). Retornos de C1: Zé e atendente reconhecem novas perguntas sem onisciência. Rodoviária distingue ausência de registro de prova de não viagem.

## Bocha — decisão local reversível

Variante autoral proposta no prompt, sem chamar de regulamento oficial: Marcus × Tonão, quatro bochas cada, balim móvel, partida até seis. Alterna iniciador em mãos válidas. Joga o lado mais distante com estoque; empate de melhor distância passa ao lado que não lançou por último. Pontos somente após oito lançamentos e repouso: bochas estritamente mais próximas que a melhor rival, tolerância explícita de 0,01 unidade entre superfícies. Empate das melhores: zero; sem rival válido: próprias válidas; ambas sem válidas: zero. Por pedido explícito do autor em 21/10, laterais têm guias de baixa restituição e ambas as cabeceiras são fechadas com madeira: bochas e balim rebatem, perdem velocidade e continuam válidos. A invalidação defensiva de dados/lançamento incorreto permanece; chegar ao fundo não anula uma mão. Sem limite secreto de mãos.

Rolagem/remoção compartilham física. Piso normal na primeira mão; faixa de umidade anunciada e fixada entre mãos, com atrito maior igualmente para todos. Simulação determinística por passos fixos, separada de regras/IA/UI. A IA usa a mesma simulação auxiliar, sem alterar a mesa real ou ler input futuro.

## Limites editoriais

- Júlia consciente e demora no socorro ganham testemunho + material de manutenção + confirmação de Tonão. Não transferir cisterna para a Sociedade.
- Nenhuma confirmação de Lucas vivo, nome/rosto atual, cativeiro ou responsáveis pelo desaparecimento. Marcus lembra dos amigos e evita conexões.
- Dados locais novos são propostas de encenação: nomes de apoio, plantão noturno de manutenção/consulta pública, visita de Adílio à Sociedade, comparação de homenagem e registros. Não fixam cargo político histórico ou profissão de Helena.
- Horror: ocorrência única na igreja, contradição perceptiva no Cemitério, marcas estáveis da cancha. Acessibilidade mantém a informação física.

## Marcos vivos

- [x] M0a — fontes lidas, baseline e decisão da Escola registrados.
- [x] M0b — cenas compiladas de C1, portas/revisitas e contrato de saves exercitados com dados isolados; T01 legítimo continua separado.
- [x] M1 — Bocha base: 75 verificações executadas no Unity; física, regras, saves isolados e partidas completas com vitória/derrota da IA.
- [x] M2 — nove cenas prontas, oito interiores, portas e comparação da planta com a referência; acesso de serviço da loja original exercitado.
- [x] M3 — caminho crítico atravessado em build, Bocha real e segunda ficha/recado únicos.
- [x] M4 — rádio, costura, comparação fora de ordem, revisitas de C1 e diário executados/revisados.
- [x] M5 — IA vence/perde na simulação; vitória e derrota na UI real, tutorial e efeitos integrados. Avaliação humana de tensão/diversão pendente.
- [x] M6 — arte própria, importação, colisão, leitura rolável e procedência conferidas. Audição humana da mixagem pendente.
- [ ] M7 — regressão, saves, build Windows e percurso.

## Evidências e pendências atuais

Leitura concluída: prompt C2, diretriz, Bíblia R1, relatórios C1, estado/save/gate/transição/UI e versões. Memória consultada sem registro específico útil. Nenhum dado pessoal gravado. Os relatórios antigos são históricos e não certificam as verificações deste capítulo.

### Execuções atuais

- `regras-save.txt`: PASS 74 (2026-10-21 03:41 UTC). Inclui duas partidas completas de Bocha pelo kernel real: Tonão vence uma estratégia fraca e perde para busca tática mais precisa, sem alterar bolas/placar depois do lançamento.
- `cenas-acessos.txt`: nove cenas inspecionadas, referências novas sem missing scripts/sprites; interações e spawns alcançáveis pela malha de colisão amostrada a cada 0,5 unidade, raio 0,24. Acesso à sala pública da Prefeitura conferido no estado liberado pelo atendimento. Não substitui avaliação humana de circulação.
- Primeira build de teste bem-sucedida: Unity 6000.6.0f1, Windows x64, DX11, Development; 267.523.542 bytes, zero erros, 2026-10-21 03:55:59 UTC (`build-qa.txt`).
- `runtime-qa-primeira-validacao.txt`: 75 linhas, percurso automatizado em build desde fixture C1 pós-Zé até confronto e consequência; passou. Usa portas reais, interações, IA e orelhão, com reposicionamento de QA entre pontos e aceleração explícita da partida. **Não certifica T01 nem um save legítimo pós-Zé obtido por playtest.**
- Evidência de falha encontrada e corrigida: E repetido reiniciava o fade de fechamento da leitura; adicionado estado `fechando`. O percurso posterior conseguiu fechar as leituras e devolver controle.
- Revisão visual encontrou corte em transcrições longas, nomes técnicos nos banners, padrão repetitivo indevido na textura procedural e áreas abertas vazias. Correções em execução: rolagem de leitura, arte própria para documentos, títulos legíveis, textura sem padrão algébrico periódico, jardins da praça, gramado/caminhos no Cemitério, passadeira da Igreja, mobiliário lateral e rodapés.
- Build inicial interrompida por requisição do compilador bloqueada; reprodução isolada compilou. Uma tentativa seguinte registrou falta de memória na compilação de shader. Repetição com dois workers, compilação de shaders limitada e DX11 terminou com zero erros. A análise URP abria cenas de luz global sobre uma cena já aberta; o comando agora abre uma cena vazia antes do build. O protótipo legado `GameplayScene`, sem uso no fluxo aprovado, fica fora do executável da campanha, conservado no repositório.

Sequência reproduzível: autoria explícita C2 → verificações de regras/saves → verificação de referências C1/cenas C2 → build Development → carga/portas C1 → percurso C2 → comparação de desempenho → build de jogador. Os comandos de QA exigem argumento explícito e não entram na build de jogador. Não declarar diversão ou mixagem aprovadas por automação.

### Retomada de 21/10 — regressão de carga

- O percurso posterior alcançou a consequência, salvou/recuperou a segunda ficha e abriu diário/legenda, mas o player fechou ao retornar à Entrada da Cidade: arquivo compilado `level2`, `Position out of bounds`. Esse percurso não recebeu PASS final. A evidência parcial está em `runtime-progresso.txt`; o PASS anterior de `runtime-qa.txt` não cobre esse retorno.
- Hipótese de serialização da cena foi investigada em uma cópia QA: abrir/salvar C1 pelo Unity alterou apenas viewport da câmera. Não foi integrado esse diff ao capítulo aprovado.
- Em execução: build em novo diretório com `BuildOptions.CleanBuildCache`, seguida de teste explícito de carga C1 antes do percurso completo. Nenhum arquivo pessoal de save foi usado.
- Novas verificações de regressão: seis pontos dos eixos asfaltados após todos os caminhos de terra; save com nome de cena desconhecido; cancelamento da carga da Bocha ao redimensionar janela. As primeiras duas já foram executadas no Editor; o redimensionamento depende da próxima execução do player.
- A revisão visual identificou árvores/móveis cortados pela escolha do primeiro sub-sprite do legado. As 27 cópias integrais têm importadores próprios e hash igual ao original. A revisão das ruas encontrou calçadas sobrescrevendo o asfalto ao longo do mesmo segmento; a autoria agora rasteriza cada célula uma vez e preserva a via quando um acesso de terra a encontra.


### Correção comprovada de C1 e revisão do acabamento

- Regravar as cenas e limpar o cache não corrigiram a falha nativa. A causa foi isolada nas referências locais de `MonoScript` para classes secundárias de `ChegadaEmSantaRita.cs`. `Capitulo1Director`, `InteracaoCapitulo1` e `TransicaoDeZonaTrigger` agora possuem arquivos próprios. As cenas apontam para GUIDs persistentes. A comparação normalizada das três cenas confirmou que nenhum dado de posição, objeto ou câmera mudou nessa reparação.
- O teste seguinte entrou na cena e revelou a leitura CPU dos terrenos no player. Quinze metadados de `Resources/Cap1`, exatamente o conjunto retornado por `NomeArquivoTile`, habilitam `isReadable`; os PNGs e demais parâmetros visuais continuam iguais. A build agora verifica esses contratos antes de compilar.
- `runtime-c1-carga.txt` confirma a carga do player. O percurso posterior de C2 passou até salvar, recarregar pelo fluxo público, recuperar a posição e voltar à Rua Norte com as duas fichas e uma chuva. As portas de serviço e os diálogos de revisita de atendente/Zé também passaram.
- A planta foi comparada novamente à referência: marcos norte/centro/sudoeste mantidos, linha de produção a oeste da Escola e sem outra margem. Os terrenos de mata a oeste e perto do rio receberam distribuição irregular; a cabeceira da ponte tem caminho de terra. Não foram criados bairros ou deslocados edifícios para preencher a vista.
- A captura tentou sobrescrever um PNG anterior e falhou; a automação passou a usar pasta de evidências nova por execução. A falha de captura foi distinguida de uma exceção do jogo. O diretório `Final` na entrega reúne a execução consolidada; as anteriores ficam como histórico.
- Execução consolidada passou em 21/10 às 11:35 UTC: derrota com save e sem save, foco/resolução, serviço/limiar C1, vitória e recompensa C2, recarga/posição e retorno. Comparação controlada também passou (16,68 ms em ambas as amostras, 1280×720, limite 60 FPS). M7 permanece parcial enquanto T01/avaliações humanas da matriz não forem realizados.
- Build de jogador gerada sem o tipo/argumentos do harness de QA. A primeira abertura identificou remoção de recursos de pós-processamento pelo registro ativo URP (o registro legado estava em 0). Corrigido um campo e regenerada a build: 207.676.240 bytes, zero erros, 11:41 UTC. Reteste do startup passou: menu observado por captura nativa, sem exceções ou shaders de pós-processamento removidos; componente/argumento do harness ausentes da assembly de jogador. Arquivo final e resultado ficam em `Final`.
