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
