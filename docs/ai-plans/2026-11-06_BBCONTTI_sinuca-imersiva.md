# Sinuca: mecânica, HUD e primeira pessoa

Autor: @BBCONTTI (git config user.name; identidade GitHub não consultada). Execução: Codex.
Data/hora: 2026-11-06 15:04 UTC-03, America/Sao_Paulo.
Ambiente: Windows, C:/Temp/SantaRita, Unity 6000.6.0f1, branch melhoria-minigames.
Status: implementação local; validação numérica/compilação; avaliação visual e Player pendentes.

## Solicitação e direção mais recente

Dar peso às bolas, melhorar colisão/rolamento/som e microajuste; substituir força oscilante por clicar/puxar/soltar; organizar e afinar UI. Pedido posterior substitui a entrada diretamente na visão do taco por caminhar em primeira pessoa pela sala com WASD, aproximar da branca e entrar em posição de tacada. Duas mãos separadas; empunhadura vinculada ao taco 3D. Revisão por capturas: apoio espalmado/por cima do taco rejeitado; empunhadura escondida/palma incorreta; exigir cursor FPS, Esc livre, taco carregado à direita e Zé caminhando/preparando/tacando.

## Diagnóstico / fontes

AGENTS, memória INDEX/CURRENT_STATE, catálogo/cofre/cenas, direção de arte/produção, skills locais pixel-art/audio/QA e skill ImageGen. Código real SinucaMinigameUI, SinucaTableView, SinucaRoom3D e parciais, SantaRitaBarLayout, MenuPausa. MarcusWalk.png e capturas do autor inspecionadas. Graphify query indisponível no PATH; nenhuma relação inferida foi tratada como implementação.

## Executado

- [x] Impulso de massas iguais corrigido, atrito/velocidade mínima/rebote e potência revistos; preview da IA no mesmo kernel.
- [x] Puxada espacial com zona morta; força volta ao devolver mouse; lateral ignorada; clique sem puxada cancela. Espaço sobe até limite, sem oscilação.
- [x] Microajustes de mira contínuos, Shift e setas; direção travada na puxada; cancelamento por pausa/foco/resolução/UI.
- [x] Sons curtos secos específicos, pool limitado e debounce; retirada da camada genérica duplicada. Síntese original, sem afirmar gravação real ou aprovação auditiva.
- [x] HUD topo 26 e rodapé 28 unidades, transparência, ancoragem às bordas; opções/ajuda recolhidas; força contextual; bloqueio de cliques sob painéis.
- [x] Caminhada com posição independente da branca, limites/obstáculos compartilhados, aproximação suave, alcance e clique no volume visível da branca; levantar após golpe/Q.
- [x] Cursor Locked e mouse relativo no jogo; Esc libera/retoma; pausa explícita no HUD; restauração ao fechar/desativar/perder foco. Frame de captura não dispara tacada. Força acumula deltas uma vez por frame.
- [x] Apoio inclinado, pivot de ponte, taco acima da ponte e elevação sobre tabela; mão dorsal filha do taco. Câmera/recuo/carregado ajustados com checagem de projeção. Mesma geometria 3D carregada à direita durante caminhada.
- [x] Zé mantém arte original; planejamento em grade pelos obstáculos, circulação contínua, passos por distância com pernas separadas na malha, apoio/preparo/avanço do taco antes de aplicar impulso. Espera se Marcus obstrui passagem. Sem teleporte para executar tacada.
- [x] Proteção SetActive para sala destruída na saída de Play, observada em log histórico do Editor.
- [x] Procedência das artes, relatório, plano, memória e nota arquitetural.

## Arquivos

Modificados: Assets/Scripts/SinucaMinigameUI.cs, SinucaTableView.cs, SinucaRoom3D.cs, SinucaRoom3D.Actors.cs, SinucaRoom3D.Table.cs, MenuPausa.cs; ASSET_SOURCES.csv; notas de memória e arquitetura.
Criados: Assets/Resources/Art/Sinuca/ (duas texturas + metas), Arte/Producao/Sinuca/README.md, Tools/QA/verify-sinuca-mechanics.py, verify-sinuca-walking.py, verify-sinuca-rig.py, este plano e evidências.
Alterações preexistentes de bocha/menu/metas/URP/migração preservadas. Sem salvar/regenerar cenas, alterar slots ou gravar progresso.

## Verificação e limites

Compilação final: 0 erros / 17 avisos CS0649. Kernel 46 PASS, circulação/rotas/apoio 147 PASS, projeção/assets 18 PASS: 211 verificações. Resultados finais no relatório ../Evidencias/SinucaMecanicas/Relatorio.md. Harnesses executam trechos reais de C# sob .NET e Unity managed math, com apresentação/estado nativo isolados. Não equivalem a Play, input nativo, renderização, audição, campanha ou build Player.

Unity MCP retornou no_unity_session. CUA negou acesso à Unity em tentativa anterior; não houve repetição/bypass. Não foi possível captar novo enquadramento in-game; as capturas desta revisão foram fornecidas pelo autor. As artes e poses finais precisam ser vistas no jogo; não declarar correção visual aprovada.

Graphify refresh: FAIL (graphifyy==0.9.96 ausente). Validate: FAIL (hashes C# diferentes do índice anterior). Índices não falsificados ou sobrescritos manualmente.

Pendências: validar perspectiva/oclusão de toda a silhueta das mãos junto às quatro tabelas e força máxima; mouse real, foco, pausa, janelas e resoluções; apresentação completa dos passos/apoio do Zé e eventual refinamento artístico de poses. Suítes históricas de órbita A/D/força oscilante têm expectativas anteriores, não foram usadas como evidência desta direção FPS. Sem commit, push ou build Player.
