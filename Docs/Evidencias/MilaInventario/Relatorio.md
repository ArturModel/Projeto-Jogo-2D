# Mila, fitas, bolsa e caderno — 04/11/2026

## Resultado e uso

Bolsa em **I** com vinte espaços de apresentação e cinco atalhos existentes. Objetos adquiridos aparecem pelo seu ícone; selecionar dois troca a ordem e selecionar um atalho atribui o objeto sem duplicar posse. Recibo entregue deixa a bolsa; fichas permanecem exclusivamente no mapa. Capacidade atual usa catálogo de nove itens, não armazenamento arbitrário de objetos ainda não implementados.

Caderno em **J** conserva páginas e caligrafia e recebe índice manuscrito por Todas/Bairro/Centro/Pessoas/Fitas/Marcadas. Só exibe notas adquiridas. Marcador de tecido liga-se ao ID da anotação e parte da página, preservado no snapshot; ao reabrir retorna à nota marcada. Sem marcador, usa última folha lida. Filtros são organização editorial, não objetivos de missão.

Mila empresta o tocador depois da conversa e do teste de velocidade de três posições; uma configuração regular conclui o reparo. Não é necessário encontrar a primeira fita antes de visitar a oficina. Quatro fitas atuais: casa do avô junto dos retratos; estojo de testes da oficina; material de som da Igreja; material de manutenção da Sociedade. São anexos aos pontos existentes, sem criar outro prédio/depósito ou reconstruir cenas. Coleta explícita GUARDAR FITA, rótulos e procedência examináveis. Fita final de Amaro continua futura.

Tocador permite selecionar fitas adquiridas, reproduzir/pausar, parar e rebobinar ao início. Troca de cena interrompe; conclusão registra anotação pessoal e origem. Sem aparelho, a fita pode ser examinada, não reproduzida. Exploração/equipamentos e menus concorrentes são bloqueados durante as telas; ESC fecha a tela atual sem abrir pausa por baixo. Música existente é atenuada durante consulta. Não foi criado autosave; organização, empréstimo, coleta e notas permanecem em RAM até o orelhão.

**Vozes ainda não integradas.** O Player contém transcrições temporizadas e suporte a clipes futuros; nenhuma locutora adulta foi apresentada como voz juvenil final. ElevenLabs autenticado fora do repositório; VoiceDesign bloqueado pelo plano, sem compra. Jessica rejeitada e removida da seleção automática. Pesquisa Qwen/Chatterbox/skills, duas novas audições e análise técnica em [produção vocal](../../Vozes_Pesquisa_E_Producao.md). Audições privadas não são assets do jogo. Publicação de vozes e alinhamento por timestamps reais continuam pendentes.

## Validação executada

- Unity 6000.6.0f1: **32 verificações específicas + PASS runtime no Editor** e **32 + PASS no Player QA final**. Cobrem catálogo/arte/transcrição, reparo incorreto/correto, nove objetos, fichas fora da bolsa, ordem/atalhos, F bloqueado, ESC sintético, marcador/índice, pausa/rebobinar, interrupção em troca de cena, anexos físicos em Igreja/C1, save manual/restauração e recibo entregue. Não é campanha integral nem teste de todos os filtros com toda combinação de notas.
- Capturas reais do executável QA em 1280×720 revisadas: bolsa sem sobreposição, índice sobre papel com caligrafia, controles e rótulos do tocador legíveis. Capturas locais ignoradas pelo Git; aprovação artística do autor não presumida.
- Builds **QA e normal Succeeded / zero erros**, sem avisos C# nos logs finais. Mesmo fingerprint C#/cenas/dados/texturas/materiais/metas: `8cd24f8373d2232fe1b567396cc659abb1f08de0354b34cd5e124493ff812193`.
- Assembly normal contém bolsa/caderno/tocador e não contém `MilaRuntimeVerification` nem a ferramenta Editor `MilaProductionVerification`; conferência dos identificadores de metadata, não teste de toda API pública. QA permanece condicionado a Editor/DEBUG e flag explícita.
- Smoke do Player normal iniciou a aplicação sem exceções de scripts, sem interagir com slots ou campanha. O log conserva avisos conhecidos do driver D3D/vsync e primárias de cor do vídeo do menu; não equivale a console global sem avisos. Instância de teste encerrada.
- Sete arquivos pessoais de save: datas anteriores à tarefa; não há hash capturado no início de toda a implementação para comparação retroativa. Hashes capturados imediatamente antes/depois do smoke normal permaneceram iguais. Fixtures usaram `BeginIsolatedTests`, nunca os slots pessoais. Editor inicialmente fechado, sem sessão RAM do autor a restaurar.
- Ferramentas: Aseprite batch gerou três fontes editáveis e PNGs originais Point/uncompressed; `voice_pipeline.py` e `qwen_audition.py` compilam e dry-run funciona; último também gerou WAV pela API oficial. Skill de áudio validada; Audio Lens instalado em revisão fixa e self-test 12/12.

## Problemas encontrados e corrigidos

QA Editor inicialmente recebia teclado sintético na superfície errada; fixture passou a clonar InputSettings para o escopo do teste, aceitar entrada sem foco e restaurar o objeto original. Proteção DontUnloadUnusedAsset evita perder a referência original durante mudança de cena. Rodada final encerrou sem exceção de limpeza.

Dependência compartilhada indispensável: `Chapter2BuildVerification` chamava Exit antes de terminar o finally que restaura configurações de build. Saída agora agendada por delayCall; cenas e ProjectSettings terminaram sem diff. Não se alterou pipeline visual/runtime de C1 para essa correção.

## Limites e entrega

Fontes, dados e código em Changes, nada staged; sem commit/push. Cenários e saves não foram regenerados. O tocador é uma ilustração original simples com controles; carretéis não têm animação física. Teste usa timestamps provisórios de transcrição, não dublagem. Atuação, sotaque, mix final e percurso legítimo completo continuam fora da prova técnica. Escola/C3 não foram iniciados.

Histórico/índices atualizados conforme o [plano](../../ai-plans/2026-11-04_PatrickDotto_mila-inventario-caderno.md). Refresh e validate Graphify PASS, 123 C# cobertos/hashes atuais; quatro consultas, canvas/HTML/relatório válidos. Fingerprint de Assets atual reconferido e igual ao das duas builds. git diff --check PASS; nada staged.
