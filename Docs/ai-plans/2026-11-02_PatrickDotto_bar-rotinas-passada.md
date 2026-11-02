# Bar: trilha, poses sentadas e passada
Autor: @PatrickDotto / Patrick Dotto; execução Codex. Data: 02/11/2026. Ambiente: Unity 6000.6.0f1 Windows. Status: concluído tecnicamente; sessão restaurada.

Pedido: música do bairro na cena exterior do Bar do Zé; joelhos e continuidade da caminhada com sons sincronizados; frequentadores realmente sentados, orientações variadas, cerveja e serviço/consumo de mate, iguais na sinuca.

Contexto: memória/estado/Obsidian e query Graphify MarcusGait/SantaRitaNpcWork/SantaRitaBarLayout; skills pixel-art, áudio e QA; fontes atuais e exportadores Aseprite. Descobertas: fase zerava em frames sem deslocamento entre ticks; frequentadores só alternavam duas poses e a perspectiva usava relógio separado; cadeira desenhada à frente do corpo.

- [x] Rua Norte exterior usa a trilha do bairro; música própria do interior do bar preservada.
- [x] Conservar fase em frames interpolados sem deslocamento e repousar somente quando movimento realmente cessa; flexão do pé de balanço nos quadros de passagem.
- [x] Quatro sequências com oito poses, cadeira integrada atrás do corpo e pés sob joelhos. Duas poses em três quartos; cerveja pegar/erguer/beber/baixar; mate com térmica, inclinação/serviço e cuia.
- [x] Catálogo de poses e seleção temporal compartilhados entre exploração e SinucaRoom3D; posições do salão iguais e cadeira ocupada não duplicada.
- [x] Player final QA: 159 verificações e encerramento PASS. Salão/sinuca conferidos em capturas reais; térmica inteira e mate visíveis; rotinas, retorno, música e passos verificados. Build normal/QA sem erros com hash idêntico c3beeb375326e5726eaee46521fd088f8f7d283267c3394f79f248f8f786aafe. QA/autoria fora do normal.
- [x] Refresh/validate PASS, 112 fontes; diff check exit 0; seis saves inalterados. RAM/posição mais recentes preservadas em Latest4 para restauração.

Arquivos: Capitulo1Director, MarcusGait, PlayerController, SantaRitaBarLayout, parciais SinucaRoom3D, AudioC2RuntimeVerification, scripts Aseprite, atlas PNG e fontes Aseprite/GIF. Sem nova classe ou cena. Origem: sprites existentes do projeto e acabamento local; nenhum asset externo. Aprovação artística pelo autor não presumida.

Complemento do autor: térmica e cuia fisicamente sobre a mesa. TermicaBar e CuiaMesa compõem o plano compartilhado; ambos ocultam a versão apoiada durante o manuseio. Ordem do mate: pegar térmica → servir → devolver → pegar cuia → beber → devolver; mesma função de fase em 2D/3D.

Correções após capturas/retorno adicional: tampo ocultava os props por ordenação; props apoiados passam à camada acima da superfície. Cuia refeita com porongo, erva em relevo e bomba, inclusive na mão. Ciclo ampliado a 16 quadros sem blur, lanterna equivalente, eventos lógicos de contato 0/4 preservados. Exportações PNG conferidas em 1088x192 após cópia: execução GUI do Aseprite deve aguardar término antes da cópia.

QA intermediário: uma rodada PASS antecede refinamentos. Outra falhou na distância percorrida medida em janela fixa de tempo real. Harness agora usa teclado virtual próprio, deadline de movimento real e espera explícita da desaceleração, mantendo asserções de movimento, contato e ausência de passos em repouso; não tratar essas rodadas como prova final.

Causa adicional confirmada: SantaRitaPlaceDressing.ApplyC1 reescrevia sortingOrder dos props do BarCoerente e atribuía rotina legada ao frequentador. Esses atores/props agora ficam sob responsabilidade do layout do bar. Asserção de runtime exige cuia/térmica acima do tampo. Patch também aplicado aos objetos já criados no Play para resposta imediata ao autor.

Limites das rodadas intermediárias: janela oculta produziu capturas pretas e input insuficiente; uma rodada visível teve crash nativo do Player ao preparar a sinuca (log local preservado). QA passa a aplicar modo Windowed após inicialização das configurações e pré-aquecer poses da perspectiva, sem alterar preferências de vídeo do autor. Não chamar essas rodadas de aprovação final. Documentação Unity 6000.6 via Context7 consultada para Screen.SetResolution e RunInBackground.

Rodada Windowed: PASS, inclusive música, oito poses de cada frequentador, props acima do tampo, movimento de 16 poses, contatos e repouso. Captura em perspectiva revelou o plano vertical da mesa ainda ocluindo parte da térmica: objetos apoiados passam a filhos do plano da mesa com pequeno viés de profundidade; posição deriva do mesmo layout. Refinamento final em validação.

Evidências finais: Docs/Evidencias/BarRotinas/PlayerQA/runtime-qa.txt, build-qa.txt, PlayerNormal/build-windows.txt e capturas locais bar-e-inventario/mesa-detalhe/bar-na-sinuca. Ensaio completo Windowed no Player Development; não equivale a campanha legítima, audição humana ou aprovação artística pelo autor. Nenhum commit/push.

Sessão final restaurada no interior do bar em (511.4757,-57.8630), RAM idêntica ao último snapshot do autor e indicador de progresso não salvo restituído. Na instância restaurada também confirmado que a térmica está acima do tampo.
