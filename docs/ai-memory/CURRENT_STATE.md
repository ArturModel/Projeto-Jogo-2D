# Estado atual — Santa Rita

Atualizado em 31/10/2026 sobre `atualizacao_sinuca`, base `30d811c` com trabalho local preservado. Bar/Sinuca R4 implementado: Player QA 200 verificações + PASS, build normal zero erros; smoke normal limitado ao menu pelo aviso do Firewall (sem ação de permissão). Resultados e limites no plano/relatório abaixo.
Este registro substitui os resumos da branch que analisavam apenas C1. O progresso e as decisões recentes do autor prevalecem. Código/cenas e evidências citadas devem ser conferidos antes de novas mudanças.

## Implementado

- C1 preservado: breve abertura, carro marrom próprio, lanterna obtida no carro; mapa obtido na Rodoviária em C2 (posse antiga preservada), posto, mercadinho, bairro/casa de Adílio, Bar do Zé e sinuca 3D em perspectiva (`SinucaTableView` / `SinucaMinigameUI` com vistas diagonal/linha do taco/superior, planta do bar compartilhada, contato e giro na branca, prática opcional e física de 120 Hz).
- C2 implementado até Bocha e consequência imediata: Centro, Igreja, Prefeitura, Rodoviária, depósito ligado ao mercadinho, oficina de rádio, casa da costureira, Sociedade Beira-Rio, Cemitério separado e abrigo. Não é criação de C3.
- Mundo conectado: C1/C2 organizam a produção. Retornos pelo mercadinho e caminho superior do Bar do Zé; requisitos narrativos existentes preservados. Escola adiada por decisão explícita.
- Rotinas e investigação: NPCs ocupados com trabalho e convivência, conhecimento limitado, pistas contextuais e ramos de conversa; praça sem exposição dedicada a Júlia. Revisitas e diário distinguem fonte e interpretação.
- Ambientes recompostos por função. Marcus ancora escala/acabamento. Tiles, materiais, fachadas, flora, decalques e luz local; fontes Aseprite e procedência em `ASSET_SOURCES.csv`. Meta visual permanece sujeita ao autor.
- Sociedade: salão e cancha horizontal na mesma cena, três degraus rasos, projeção de profundidade dos atores (15 px/escala até 90%), paredes/forro com recorte por aproximação, luz própria, bancos nas duas laterais e saibro 19 × 5,5.
- Bocha: apresentação em perspectiva, minimapa, consulta geral, Ponto/Rafa/Volo, força oscilante segurando clique e lançamento ao soltar, linha de mira ajustada ao mouse, rastros, colisões/rebotes de cabeceira inclusive balim; mecânica da água removida.
- Movimento e passos sincronizados no apoio. Lanterna manual F com poses, luz e preferência no snapshot. Músicas do autor preservadas, loops/atenuação por contexto e efeitos ligados a eventos.
- Saídas com soleira/capacho e ação Sair; portas C2 por E sem cooldown global manual, com bloqueio durante fade/leitura e antirretorno apenas em passagens automáticas.
- GameState em RAM; salvar só no orelhão. Carga com escolha explícita de slot, inclusive após derrota em Sinuca/Bocha. Compatibilidade de saves anteriores preservada.

## Evidências históricas, não testes novos desta integração

- [C2 R2](../../Docs/Consolidacao_C2_R2.md): 290 verificações R2 e 63 C1 no executável QA.
- [Cidade viva](../../Docs/Evidencias/CidadeViva/Relatorio.md): recomposição, cemitério e retornos.
- [Bocha](../../Docs/Evidencias/BochaPerspectiva/Relatorio.md): física, controles, apresentação e limites.
- [Saídas/escada](../../Docs/Evidencias/SaidasEscada/Relatorio.md): 167 verificações QA, build comum, profundidade/bancos e preservação de saves.
- [Portas E](../../Docs/Evidencias/PortasE/Relatorio.md): reprodução antes da correção, 62 checks Editor + 62 Player QA, build normal sem erros e cinco saves intactos em 23/10.

## Esta integração

Resolver `.gitignore`/`AGENTS.md`, preservar integralmente Assets/ e ProjectSettings/, atualizar memória/cofre ao C2, regenerar índice estrutural local e validar ferramentas. Resultado desta execução: [integração](../GRAPHIFY-INTEGRACAO-C2.md).

## Limites e próximos passos

Avaliação artística, audição e campanha legítima integral não são comprovadas por fixtures técnicas. Não reabrir C1 inteiro, reconstruir cenas ou migrar saves para atualizar documentação. Escola/C3 continuam fora deste escopo. O registro de integração acima é histórico. Na revisão de 31/10, MCP local em 8088 foi conferido contra o projeto real e usado para autoria/QA sem salvar ou regenerar cenas.

## Revisão Bar/Sinuca R4 — 30/10/2026

[Plano executado](../ai-plans/2026-10-30_Patrick-Dotto_bar-sinuca-inventario.md) e [evidências](../Evidencias/BarSinucaR4/Relatorio.md). `SinucaBrasileiraRules` substitui a meta antiga de quatro bolas: valores, sequência e faltas brasileiras com exceção narrativa da bola marcada. `SantaRitaBarLayout` fornece posições/atores à exploração e projeção do duelo. `SantaRitaInventory` deriva cinco slots de flags existentes; `SantaRitaItemAction` permite coleta/entrega explícitas sem persistência automática. Música compartilhada no Posto/Encruzilhada/Bairro e filtrada nos interiores. Soleiras exigem pés sobre o tapete. Não considerar os relatórios antigos de sinuca 4 pontos como prova dessas regras novas.

## Decisão posterior do autor — bar com identidade 2D (31/10/2026)

A estética Ref7 de NPCs/móveis em blocos e atlas de madeira diferente foi rejeitada. O bar de exploração deve permanecer 2D com os sprites originais de Zé/frequentadores, props e material WorldLit do restante do jogo. `SantaRitaBarLayout.SpriteFor` é a fonte comum das duas apresentações; não restabelecer prioridade de BarRef7/Baked.

A sinuca apresenta a mesma pixel art em 2.5D, com mesa/bolas/redes em profundidade. Piso e parede vêm do interior C1; fontes de sprites/UV/pivôs são compartilhadas e verificadas. Cuia/térmica próprias substituem o antigo sprite que continha uma mesa inteira. Oito complementos Aseprite seguem a escala/paleta nativa; Inter mantém o distintivo histórico.

[Plano desta revisão](../ai-plans/2026-10-31_PatrickDotto_bar-identidade-2d.md) e [evidências](../Evidencias/BarIdentidade2D/Relatorio.md). Grafo e cofre continuam auxiliares; avaliação artística do autor e campanha legítima integral não são presumidas.

Fechamento desta revisão: 325 PASS Editor + 325 PASS Player QA final; builds QA/normal sem erros e mesmo hash; seis arquivos pessoais intactos. Circulação: 934 células e aproximação aos NPCs/examináveis. Sessão RAM restaurada; Editor no bar. Fontes e limites no relatório acima.


## Decisão posterior — sinuquinha de quinze bolas (31/10/2026)

Pedido mais recente substitui a Sinuca Brasileira de sete bolas por quinze objetos + branca. `SinuquinhaRules` é o kernel atual, com lisas/listradas ou identificação 1–7/9–15, branca pronta, grupos e preta final. Regra da casa, não par/ímpar nem regulamento oficial integral. Textos e testes R4 de sete bolas são históricos. Física de movimento/rebote mantida; buffers e IA ampliados. Números, listras e rolamento, guia da colorida, órbita livre, capacho sem tapete sob a mesa, detalhe de bandeira, documentos tipográficos e movimento de Marcus revistos. Plano: [sinuquinha-15](../ai-plans/2026-10-31_PatrickDotto_sinuquinha-15.md). Resultados finais e limites: [relatório](../Evidencias/Sinuquinha15/Relatorio.md).

Complemento mais recente: marcada reativada nas mãos 2/3 com X e anulação/restituição da tacada; não bloquear preta ou última bola do grupo. A/D contornam, setas miram, lista de encaçapadas e ESC → Carregar Jogo (cinco slots explícitos). Mesa/cadeiras da calçada usam arte/escala do interior. Referências de render/áudio/rim recuperadas após recarga do Editor.

Fechamento desta execução: 279 verificações Editor + 279 Player QA e encerramentos PASS; builds sem erros e mesmo hash, QA/autoria fora do normal, seis saves preservados na rodada final e RAM restaurada. Refresh/validate: 111 C#. Dois caches TMP escritos após a build, documentados no relatório; não houve mudança posterior em código/cenas/sprites. Avaliação artística/audição/campanha legítima continuam separadas.

01/11/2026: arquivos novos ausentes recuperados do stash sem sobrescrever existentes. Unity recompilou e saiu do Safe Mode para MainMenu; stash e saves preservados.


## 01/11/2026 — Áudio C2 / LFS
Trilhas C2 fornecidas integradas por cena e inventário protegido contra ícone nulo; 122 binários LFS materializados. MCP reconectado à instância correta. Player QA: 137 verificações + encerramento PASS; builds QA/normal sem erros e hash de Assets idêntico. Onze visitas validam seleção, volume/filtro e loop real no fim do arquivo; cinco ícones e sprites do bar carregados. Ensaio completo ocorreu no executável, sem aprovação artística/audição subjetiva.

01/11 — Fichas somente no mapa; cinco slots agrupados com moldura da pausa. Ciclo Marcus passa a poses completas sem pernas procedurais; lanterna regenerada. Player QA 141 verificações + PASS, builds sem erros, RAM/saves preservados. Plano itens-passada.

01/11 — Proporções: jeans encorpado em caminhada/repouso, lanterna regenerada; barra com contorno fino nativo sem textura fatiada. QA final 141 verificações + PASS, primeira falha de repouso sob build paralela registrada no plano proporcoes-inventario. Saves/RAM preservados.

02/11 — Bar: Rua Norte exterior recebe trilha do bairro; poses sentadas com cadeira integrada e ações em oito quadros; mate/térmica sobre a mesa e manuseio sincronizado entre salão e sinuca. Dressing legado não sobrescreve BarCoerente; props apoiados seguem plano da mesa na perspectiva. Marcus usa 16 poses por direção, fase preservada entre ticks e mesmos contatos 0/4. QA final Windowed: 159 verificações + PASS, builds/hash iguais; limites no plano bar-rotinas-passada.

02/11 — Ciclo limpo de Marcus substitui transformações acumuladas do ciclo anterior: duas pernas articuladas completas por pose; lanterna regenerada e alpha validado nas 102 células, loader rejeita dados inválidos. Export-Marcus.ps1 reproduz exportação em sequência. Player QA 33 verificações + PASS, 4 direções/repouso com/sem equipamento, builds/hash iguais; plano marcus-ciclo-limpo.

02/11 — Ajuste pontual lanterna de costas: corpo oculta o objeto, feixe e estado permanecem ativos; outras direções preservadas. QA 33 + PASS e builds sem erros/hash igual, plano lanterna-costas.

02/11 — Console: chamadas FindObjectsSortMode.None substituídas por overload atual, campo legado sem leitura removido com referências QA. Compilação real C# sem avisos/erros; índices 112.


02/11 — [Atlas cartográfico](../ai-plans/2026-11-02_PatrickDotto_atlas-cartografico.md): levantamento real de 19 regiões/38 acessos, camadas/plantas, marcador e investigação por estados existentes, sem GPS ou ilustração antiga na UI. QA Editor/Player final PASS; builds normais/QA sem erros e mesmo hash; 6 arquivos pessoais intactos. Fontes/limites em [relatório](../Evidencias/Atlas/Relatorio.md) e [arquitetura](../Atlas_Arquitetura.md). Não inferir aprovação artística ou campanha integral.


03/11 — Caderno pessoal independente substitui o painel de diário: J ou CADERNO abre livro com caligrafia, folhas, marcador e reflexões desbloqueadas pelos registros adquiridos. Originais narrativos/snapshot v2 preservados; OnDiarioAtualizado/OnSnapshotRestaurado distinguem descoberta de carga e evitam avisos falsos. Scroll dos painéis não opera mapa; fichas só possuídas e em ordem, sem cupom. Mapa da Cidade recebe entorno decorativo sem mudar as cartas ou acessos. Verificação final: [plano](../ai-plans/2026-11-03_PatrickDotto_caderno-paisagem.md), [relatório](../Evidencias/Caderno/Relatorio.md).


Fechamento caderno/paisagem: testes específicos (30) e Atlas (237) PASS em Editor e Player; builds finais sem erros e hash igual, 120 C# indexados. Sessão RAM/posição/zona/indicador restaurados, seis arquivos pessoais intactos, cenas/configurações sem diff. Corpus final com 64 textos/variantes; original do diário preservado. Alterações não staged, sem commit/push. Limites e evidências no relatório Caderno.

TMP gravou cache VT323 após restauração; único asset posterior às builds. Hash do Assets atual difere do fingerprint comum das duas builds por esse cache, sem mudanças posteriores em código/cenas/corpus/arte. Detalhe no relatório.


## Mila, bolsa e caderno — 04/11/2026

[Plano](../ai-plans/2026-11-04_PatrickDotto_mila-inventario-caderno.md) e [relatório](../Evidencias/MilaInventario/Relatorio.md): bolsa I com vinte espaços de apresentação/catálogo atual de nove itens e cinco atalhos reorganizáveis, fichas só no mapa; índice manuscrito do caderno e marcador por ID/parte, preservado no snapshot manual. Mila empresta tocador após ajuste; quatro fitas contextualizadas com coleta explícita e transcrição temporizada. Conteúdo de Amaro futuro não antecipado. Sem autosave ou reconstrução de cenas.

32 verificações específicas + PASS em Editor e Player final; builds QA/normal zero erros, sem avisos C#, fingerprint igual. Cenas/ProjectSettings sem diff. QA fora do assembly normal; smoke normal só inicialização, avisos conhecidos de vídeo/driver permanecem. Sete saves pessoais anteriores à tarefa; hashes iguais no intervalo do smoke, sem alegar baseline de início.

Dublagem NÃO concluída: ElevenLabs privado autenticado, criação de voz bloqueada pelo plano; Jessica rejeitada por soar adulta/artificial e retirada da seleção automática. Duas audições ficcionais Qwen VoiceDesign geradas na demo oficial, ainda sem aprovação. Skill Audio Lens instalada/testada 12/12; skill santa-rita-audio ampliada. [Pesquisa, fontes, hardware e próximos passos](../Vozes_Pesquisa_E_Producao.md). Ferramentas/segredos fora do Player; nenhum modelo pesado instalado, compra, commit ou push.


04/11 — Revisão das audições pelo autor: duas candidatas Qwen também REJEITADAS (artificialidade/idade), faltaram quarto/Marcus ao fundo. Direção agora exige prova de cena completa com vozes em posições distintas. Base real de quarto/foley CC0 preparada em prévia privada, renderer validado; elenco completo NÃO produzido: Lucas/Marcus retornaram erro na demo, Voice Design do AI Studio exige chave paga. Sem compras, Assets ou saves alterados. [Plano e bloqueios](../ai-plans/2026-11-04_PatrickDotto_fitas-cena-quarto.md); [fontes/evidências](../Evidencias/MilaInventario/Revisao_Cena_Quarto.md).

04/11 — Pedido posterior de amostra única: cena com três tomadas ElevenLabs v3 e ambiente/foley renderizada em prévia privada; elenco ainda não aprovado. Plano: ../ai-plans/2026-11-04_PatrickDotto_amostra-fita-quarto.md. Sem publicar no jogo.

04/11 — Amostra R2: novas tomadas Júlia/Lucas, Marcus chamado pelo nome na porta; REC/cadeira/quarto aumentados e tratamento mono de cassete com chiado. Prévia privada, sem aprovação/publicação. Plano ../ai-plans/2026-11-04_PatrickDotto_amostra-fita-r2.md.

04/11 — Autor forneceu roteiro R3 e rejeitou timbre adulto da R2. Oito falas registradas literalmente em Tools/Audio/quarto_roteiro_r3.json; teste juvenil Qwen falhou. Sem nova amostra adequada nem publicação. Plano ../ai-plans/2026-11-04_PatrickDotto_roteiro-fita-r3.md.

06/11/2026 — Pasta consolidada desta sessão: C:/Temp/SantaRita (raiz Git), projeto Unity em C:/Temp/SantaRita/Projeto Jogo 2D, branch melhoria-minigames. Main atualizada e mudanças locais da bocha/menu migradas. Plano: ../ai-plans/2026-11-06_BBCONTTI_migracao-santa-rita.md. Pastas antigas preservadas; abrir esta nova cópia no Editor.


06/11/2026 — Sinuca em primeira pessoa: WASD, mouse capturado/Esc livre, aproximação à branca, mãos e taco 3D/carregado, Zé circulando e preparando a tacada; HUD fino e física/puxada/som revistos. Compilação e QA numérico registrados, sem aprovação visual/auditiva ou Player; Graphify indisponível. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-imersiva.md. Direção de caminhada substitui o default de câmera arcade. Saves e cenas preservados.


06/11/2026 — Direção mais recente da sinuca: Zé voltou a ficar parado e sem taco, jogada apenas simulada. Mãos são billboards; ocultas ao caminhar (fica só taco). Apoio mantém arte aceita; overlay URP de primeiro plano evita corte pelo tampo e empunhadura foi aproximada/rotacionada para expor antebraço. Validação visual da overlay ainda pendente; fonte atual prevalece sobre plano anterior de NPC móvel. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-primeiro-plano.md.


06/11/2026 — Empunhadura refeita após nova referência: pegada fechada vista pelo dorso/de cima, pulso curto; versão de palma/dedos por baixo rejeitada e não publicada. Pivô e rotação seguem eixo projetado do taco; dorso fica à frente do eixo e pulso termina com dissolução em pixels. Apoio mantém PNG/pose. 0 erros C#, 22 QA de rig; shader/render e aprovação artística in-game pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-fechada.md.


06/11/2026 — Autor aprovou a última dupla ImageGen após correção só do apoio (ponte aberta próxima à original). Atlas MaosMarcus.png integrado, contato do apoio no eixo sem offset lateral e máscaras complementares de dorso/polegar em profundidades distintas; empunhadura segue taco. 0 erros C#, 55 verificações numéricas/assets PASS. Play/HLSL/render pendentes: MCP no_unity_session e CUA sem acesso. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-aprovadas.md.


06/11/2026 — Apoio da sinuca reduzido .36 → .30m (16,7%) a pedido do autor; empunhadura desenhada após taco com ZTest Always apenas na própria silhueta recortada, evitando haste atravessar dorso. Apoio mantém profundidade normal e contato. 0 erros C#, 56 checks numéricos/assets PASS; render nativo pendente. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-apoio-menor-oclusao.md


06/11/2026 — Empunhadura do taco espelhada para mão esquerda, pulso -12° em torno do mesmo contato; apoio preservado. Atlas não regenerado. 56 checks numéricos/assets PASS, 0 erros C#/17 avisos CS0649; render/Play pendente. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-esquerda.md


06/11/2026 — Autor pediu mão esquerda original vista de cima, polegar quase oculto. Novo MaoEsquerdaTaco.png ImageGen integrado, sem mirrorX/roll corretivo -12°; apoio preservado no atlas. 57 checks numéricos/assets PASS. Compilação geral bloqueada por CS0246 SinucaZeComments ausente em mudanças concorrentes de SinucaMinigameUI.cs; Play/render pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-esquerda-desenhada.md


06/11/2026 — Sinuca: trilha .31 → .23, impactos taco/bolas destacados, Zé reage a conversões/faltas/marcada e proximidade da vitória. Legenda inferior central fonte 16, escrita progressiva, leitura/fade; murmúrios sintéticos originais em fonte dedicada, pausa/foco/efeitos respeitados. 27 testes de fala + 46 de mecânicas PASS; Assembly-CSharp 0 erros/17 avisos CS0649. CS0246 transitório SinucaZeComments de edição concorrente resolvido. Render/audição/Player pendentes (MCP sem sessão). Refresh Graphify falha por CLI ausente; validate com hashes antigos. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-falas-audio.md.


06/11/2026 — Dupla reunida em MaosMarcusUnificadas.png ImageGen para mesma pele/estilo; nova pegada superior espelhada na arte, apoio mantido. Apoio reduzido mais 30% (.30 → .21m) sem mudar contato. 57 checks numéricos/assets PASS, compilação geral voltou a passar 0 erros/17 CS0649. Play/render pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-atlas-unificado-apoio-30.md


06/11/2026 — Áudio menu/sinuca: AudioSettingsController sincroniza ambos os painéis ao abrir/alterar, aplica sliders a todos os mixers e preserva zero/valores baixos (migração de reset removida). Master só no AudioListener; defaults ausentes música .35/efeitos 1; música de mixer .5 com duck .35 na sinuca, trilha direta da sinuca .10, impactos reforçados. 22 testes isolados + 8 checks nativos Unity PASS; compilação 0 erros/17 avisos CS0649, preferências da fixture restauradas. Graphify indisponível/hashes antigos; audição/Player pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_audio-global-sinuca.md.


06/11/2026 — Áudio dividido em Geral/Música/Fala/Chuva/Taco e bolas/Outros efeitos nos dois menus. Novas preferências independentes audio_fala/audio_chuva/audio_impactos; chuva fora do SFX, voz de Zé/fitas e impactos com ganhos próprios. Passo discreto de madeira na sala da sinuca (.065, cada .62 m de movimento real), silencioso parado/colidindo. 29 testes de configurações + 18 checks Unity + 27 de fala + 75 de movimento PASS; 0 erros/16 CS0649. Preferências da fixture restauradas, sem cenas/saves alterados. Audição/layout visual/Player pendentes; Graphify indisponível. Plano: ../ai-plans/2026-11-06_BBCONTTI_categorias-audio-passos.md.

06/11/2026 — Sinuca: versão de luz/caçapas/mãos aprovada pelo autor. Shader SalaLit local evita ambiente do mundo 2D; uma sombra de pendente, pano 1790 vértices, redes 3744 triângulos. Próxima revisão autorizada: reformular salão com luzes fracas secundárias, depois de commit/push. Plano ../ai-plans/2026-11-06_BBCONTTI_sinuca-luz-cacapas.md.

06/11/2026 — Após checkpoint 7cb8779e e push autorizado, salão 3D reformulado: móveis volumétricos nos anchors existentes, reboco/lambril/forro, três arandelas fracas e luz de balcão/TV. Capturas Editor seis vistas, 624 amostras de caçapa livres, 75 caminhada + 57 rig PASS, sem erros/shaders. Nova revisão local; avaliação do autor e Player/FPS pendentes. Plano ../ai-plans/2026-11-06_BBCONTTI_sala-sinuca-reformulada.md.

06/11/2026 — Sinuca: retirados assentos 3D vazios (cadeiras e banquetas); apenas cadeiras integradas aos quatro NPCs sentados permanecem. Walkable não bloqueia assentos removidos. Unity sem erros; inspeção nativa 0 anchors de cadeira e 4 NPCs preservados. Plano ../ai-plans/2026-11-06_BBCONTTI_sinuca-sem-cadeiras-vazias.md.

06/11/2026 — Sinuca: bola falsa visual separada do solver, exame por E, batidas e inspeção sob a mesa; legenda compartilhada Marcus/Zé com murmúrio Marcus mais grave (pitch .77). Impacto usa Foley/door (Kenney CC0). Bloco de água corrigido; poça rejeitada pelo autor e removida em favor de três gotas pequenas. Console sem erros C#; aprovação visual final/audição/campanha/Player pendentes. Graphify indisponível e índice desatualizado. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-percepcoes.md.


06/11/2026 — Sinuca: síntese bola-bola substituída por três recortes de gravação real CC0 (bsumusictech/Freesound 62331), pitch natural fixo e volume pelo impulso, com categoria de impactos preservada. WAVs PCM/180 ms/pico .72 validados; 45 checks numéricos PASS e compilação 0 erros/16 CS0649. Unity MCP sem instâncias; audição/importação nativa/Player pendentes. Graphify refresh indisponível e validate desatualizado. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-bolas-reais.md.

06/11 — Sinuca: Zé usa atlas novo mais detalhado (16 desenhos), três poses de caminhada por direção e quatro poses de inclinação/tacada; IA contorna mesa/móveis antes do contato. Não usa deformação da arte anterior. Compilação C# e harnesses locais passaram; visual no Unity/Player e atualização Graphify pendentes. [Plano](../ai-plans/2026-11-06_BBCONTTI_ze-sinuca-sprites-movimento.md).


06/11/2026 — Zé na sinuca: 24 quadros; caminhada/tacada frente/lado/costas por posição relativa da câmera, inclusive parado, com histerese. Dezesseis quadros anteriores preservados. Falas liberadas enquanto investigação das batidas está apenas pendente; prioridade narrativa preservada. Unity 33 checks e 12 capturas; 32 falas, 1580 planner, 10 timing PASS; compilação geral 0 erros. Graphify indisponível/índice antigo; Player/aprovação artística pendentes. [Plano](../ai-plans/2026-11-06_BBCONTTI_ze-direcional-falas.md).


06/11/2026 — [Sinuca: eventos e lore](../ai-plans/2026-11-06_BBCONTTI_sinuca-eventos-lore.md): R1 conferida contra DOCX R1/V5; Docs/Lore_Atual.md registra fatos, compatibilidade e percepções. SinucaPerceptionSchedule centraliza quatro eventos exclusivos com intervalos variáveis, turnos e repetição distribuída; batidas/bola falsa corrigidas, água no cano/eco da festa adicionados. Teste .NET 1.000 sementes/24.000 eventos PASS, assembly Unity 10.000 checks PASS e regressão física 53 PASS; sem build Player/audição/campanha. Refresh/validate Graphify falharam por ferramenta ausente/índice antigo. Cenas/saves e alterações preexistentes preservados.


06/11/2026 — [Sinuca: efeito e apoio](../ai-plans/2026-11-06_BBCONTTI_sinuca-efeito-apoio.md): direito + arraste, foco suave, UI bola/ponto/taco/ângulos e contato contínuo. Apoio intersecta haste real e permanece estável na puxada. Unity sem erros; física 136 PASS e 28 verificações projetadas do apoio PASS. Fixtures técnicas; Player/input físico/aprovação visual pendentes. Graphify ausente/índice antigo.


07/11/2026 — [Bocha: ambientação](../ai-plans/2026-11-07_BBCONTTI_bocha-ambientacao.md): seis arandelas e dois pendentes fracos, reboco independente do bar, pilastras/reforços/sanca/alcova e decoração funcional. Editor: quatro vistas, 273 amostras livres, 0 shaders inválidos e console sem erros. Fonte runtime BochaRoom3D; cenas/saves preservados. Player e avaliação artística pendentes. Refresh Graphify indisponível; validate aponta índice antigo.

07/11/2026 — [Tonão na bocha](../ai-plans/2026-11-07_BBCONTTI_tonao-bocha.md). Tonão caminha até a posição escolhida pela IA, lança com a mão direita e reage com legenda e murmúrio. Atlas iluminado em frente, costas e ambos os perfis. Fixture Unity passou 27 combinações e registrou 16 capturas. Graphify ausente e índice desatualizado. Partida completa, audição e Player pendentes.


07/11/2026 — [Bocha com alucinações](../ai-plans/2026-11-07_BBCONTTI_bocha-percepcoes.md). O duelo usa a mesma agenda sem repetição imediata da sinuca. Marcus reage à figura no banco quando foca a câmera e à queda aparente de sua bocha. Tonão segue o jogo. O tremor respeita a opção de reduzir efeitos intensos. A queda preserva o estado real e os resultados da jogada. 37 verificações no Unity, capturas da câmera do jogo e regressão da agenda concluídas.

## Perfil de áudio e legendas dos minigames

O ganho da sinuca confirmado pelo autor é a referência dos sliders a 100%. Música 0, chuva .10 e outros efeitos .078125. A chuva subiu cerca de 3,6 dB após nova avaliação do autor. Geral, fala e taco/bolas mantêm ganho unitário antes da mixagem das fontes. Bocha e sinuca compartilham a apresentação de fala, e a Sociedade usa a mesma chuva abafada do bar. [Plano e QA](../ai-plans/2026-11-07_BBCONTTI_mix-global-minigames.md).

07/11/2026 — Bar do Zé em exploração usa piso de 12,5 unidades, corredores laterais mais largos, luz quente localizada e sombras em móveis e personagens. A entrada do duelo dura cerca de 3,7 s com íris texturizada e descida de câmera do pendente para o feltro. O autor testou e aprovou. [Plano](../ai-plans/2026-11-07_BBCONTTI_sinuca-sala-entrada.md).

## Sinuca e integração local — 07/11/2026

Apagão, Zé assombrado e batidas na janela integram os dois eventos anteriores. Console por aspas oferece comandos e botões no Editor e builds de desenvolvimento. Vistas laterais usam atlas com dois planos e shader de chuva/gotas. Autor confirmou teste no jogo e autorizou commit e push de todo o trabalho local. [Plano](../ai-plans/2026-11-07_BBCONTTI_sinuca-eventos-console-janelas.md). Graphify segue indisponível.

08/11/2026 — A Rua Norte de EntradaDaCidade recebeu Tilemap modular com 52 peças de 64 px, fachada de porta única, sprites em escala com Marcus, luz com sombras por volumes XYZ e duas camadas de névoa em torno do bar. Quatro espécies de árvores recebem vento na copa e mantêm colisão nos troncos. [Plano executado](../ai-plans/2026-11-08_BBCONTTI_rua-norte-bar-ze.md).


08/11/2026 — A porta da Rua Norte entra direto na exploração 3D do Bar do Zé. A máscara escura com névoa preserva os efeitos da rua até cobrir o quadro. Marcus abre pela maçaneta direita, e Zé permanece junto à mesa. A conversa no salão inicia a sinuca e o retorno do duelo preserva o lugar do jogador. [Plano e QA](../ai-plans/2026-11-08_BBCONTTI_bar-ze-entrada-3d.md).

08/11/2026 — A Rua Norte ganhou uma camada adicional de altura e oclusão local para as 52 peças do terreno, validada em Play Mode com os presets originais. [Plano e evidências](../ai-plans/2026-11-08_BBCONTTI_relevo-adicional-rua-norte.md).


08/11/2026 — Rua Norte: continuidade de grama, calçada e asfalto com mistura de cor e relevo nas juntas, e altura de trincas/poças limitada ao detalhe. Conferido em Play Mode. [Plano e evidências](../ai-plans/2026-11-08_BBCONTTI_continuidade-piso-rua-norte.md).


08/11/2026 — O exterior do Bar do Zé é a referência de redesign de Santa Rita. [Guia visual](../../Docs/Guia_Visual_Santa_Rita.md) reúne escala, materiais, iluminação, efeitos, continuidade e QA. [Plano](../../Docs/ai-plans/2026-11-08_BBCONTTI_guia-visual-santa-rita.md).


## Rodovia de Entrada — 08/11/2026

A zona inicial de EntradaDaCidade usa a família visual da Rua Norte. Cinco sprites específicos ocupam a mesma folha. A fala de Marcus usa a legenda da sinuca com zoom suave de 2,5% ou 6%, conforme o gatilho. A mala aparece no passeio sul, o orelhão mantém seu checkpoint e a lanterna vem do carro. [Plano e QA](../ai-plans/2026-11-08_BBCONTTI_reforma-entrada-cidade.md).
