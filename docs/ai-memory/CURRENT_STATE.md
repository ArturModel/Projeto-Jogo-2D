# Estado atual — Santa Rita

Atualizado em 26/10/2026, integração Graphify/Obsidian sobre `main` local `86b7c02`.
Este registro substitui os resumos da branch que analisavam apenas C1. O progresso e as decisões recentes do autor prevalecem. Código/cenas e evidências citadas devem ser conferidos antes de novas mudanças.

## Implementado

- C1 preservado: breve abertura, carro marrom próprio, mapa/lanterna obtidos no carro, posto, mercadinho, bairro/casa de Adílio, Bar do Zé e sinuca.
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

Avaliação artística, audição e campanha legítima integral não são comprovadas por fixtures técnicas. Não reabrir C1 inteiro, reconstruir cenas ou migrar saves para atualizar documentação. Escola/C3 continuam fora deste escopo. Unity MCP não está disponível nesta sessão; esta integração de documentação não exige mudar cenas nem executar o Editor.
