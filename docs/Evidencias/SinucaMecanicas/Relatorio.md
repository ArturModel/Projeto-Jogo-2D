# Sinuca imersiva — resultado técnico 06/11/2026

Implementação e revisão das capturas do autor concluídas localmente; revisão visual final pendente.

| Verificação executada | Resultado | Evidência |
|---|---|---|
| Assembly-CSharp contra referências do projeto Unity | 0 erros, 17 avisos CS0649 de campos serializados/legados | compilacao.txt |
| Kernel real de colisão/rolamento/caçapas, mira, puxada e amostras de síntese de som | 46 PASS | testes-numericos.txt |
| Caminhada, limites/móveis, alcance, passagem nas quatro faces, elevação da ponte na tabela, caminhos de Zé e presença física | 147 PASS | circulacao-ze.txt |
| Projeção numérica do ponto da empunhadura, taco carregado à direita, alpha/importador e parentagem ao taco | 18 PASS | rig-artes.txt |
| Graphify refresh | FAIL: executável/dependência indisponível | Não atualizou índice |
| Graphify validate | FAIL: hashes C# diferem do índice antigo | Não falsificados |
| Play / Unity MCP / captura própria / Player build | Não executados nesta revisão | MCP: no_unity_session |

Total: 211 verificações específicas PASS. São verificações numéricas/estáticas de assets, não validação visual ou input nativo. A compilação .NET usa o projeto e DLLs de Unity; não é uma build Player.

## Comportamento implementado

Entrada caminhando na sala, sem corpo visível. WASD e mouse relativo capturado; Esc alterna cursor/interface e cancela carga. Botão Pausa mantém acesso ao menu global. Clique no volume da branca ao alcance prepara a postura, clique/puxada/soltura seguinte executa; Q levanta. Movimento de mouse acumulado uma vez por frame durante puxada, sem limite de janela. Mesa e móveis impedem atravessar; câmera em pé não segue bolas.

Duas mãos: apoio em ponte, inclinado com pivô no canal; empunhadura dorsal fechada, filha da geometria do taco. Plano de apoio e eixo do taco sobem na tabela; apoio não recua durante puxada. Câmera e posição de mão readequadas para mantê-la enquadrada no recuo. Mesmo taco e empunhadura aparecem à direita ao caminhar.

Zé mantém o sprite original. Rota cardinal em grade pelos mesmos obstáculos; malha persistente separa pernas/torso e fase depende de distância andada. Aguarda chegada e 0,75 s de preparo, move taco 3D e só aplica impulso após iniciar golpe. Espera se Marcus bloqueia a passagem; não é uma navegação que empurra o jogador. Tacada sem posição física usa alternativa de segurança alcançável.

HUD de 26/28 unidades nas bordas, transparência, ações pequenas, opções/ajuda recolhidas e força contextual. Física com impulso de massas iguais e rolamento mais longo; áudio sintético curto, específico e sem duplicação genérica.

## Arte / limites honestos

Capturas antes desta correção foram fornecidas pelo autor e mostram mão espalmada acima do taco e empunhadura ausente. Novas artes foram inspecionadas isoladamente, tomando Marcus como referência. Ponte selecionada foi gerada com taco de referência e editada por ImageGen para removê-lo, preservando a pose. Empunhadura foi refeita como punho fechado visto por cima.

Não foi possível produzir captura equivalente depois das mudanças: Unity MCP sem sessão; tentativa CUA anterior rejeitada para Unity, sem repetição/bypass. A projeção verifica o ponto de ancoragem, não o contorno inteiro, oclusão da mão pela tabela, iluminação/material, o encaixe aparente entre dedos e taco ou a qualidade artística dos passos de Zé. Esses pontos continuam exigindo inspeção em jogo. Não há aprovação artística/auditiva presumida. Os movimentos de Zé reaproveitam o sprite original por deformação leve de malha; não são um personagem novo com animações direcionais desenhadas.

Suítes históricas Sinuquinha15/Sinuca3D com órbita A/D e carga oscilante descrevem comportamento anterior, não foram usadas como aprovação da direção FPS. Os harnesses atuais ficam em Tools/QA, fora do Player.

Nenhum save pessoal aberto/gravado; nenhuma cena salva ou regenerada. Alterações preexistentes preservadas. Arte em PNG RGBA com Point/sem mipmap/sem compressão; fontes achatadas e procedência documentadas em Arte/Producao/Sinuca e ASSET_SOURCES.csv. Aseprite ausente no caminho local configurado; nenhum fonte em camadas fictício foi anunciado.

Plano: ../../ai-plans/2026-11-06_BBCONTTI_sinuca-imersiva.md.


## Revisão posterior — mãos em primeiro plano / NPC parado

Pose do apoio aceita pelo autor, corte pelo tampo rejeitado. Mãos agora orientadas à câmera; taco/mãos em câmera URP Overlay isolada para evitar a oclusão do tampo. Empunhadura aumentada/aproximada e com roll, mostrando mais antebraço. Não houve nova geração/edição de PNG nesta revisão. Caminhada sem mãos, só taco. Movimentação/posicionamento/taco do Zé removidos expressamente; descrição e checks de NPC móvel acima são históricos. Compilação e 22 verificações do rig registrados em compilacao-primeiro-plano.txt / rig-primeiro-plano.txt; inspeção real da pilha URP e encaixe ainda pendente. Plano atualizado em Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-maos-primeiro-plano.md.


## Pegada fechada revisada — 06/11/2026

A última foto de empunhadura/alinhamento/tensão substitui a interpretação de palma aberta. Nova MaoTaco.png mostra dorso de mão fechada e pulso curto, gerada/editada por ImageGen a partir da pose de referência e estilo Marcus. Antiga arte frontal em Versoes/MaoTaco_dorso_frontal_rejeitada.png; tentativas abertas intermediárias não foram integradas. GUID preservado. Alinhamento pelo eixo projetado, dorso à frente do taco e dissolução em pixels na terminação do pulso. 0 erros C# e 22 QA de projeção/assets; HLSL/render/encaixe final ainda não verificados em Play. Fonte e limites: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-fechada.md. Nenhuma aprovação artística do autor presumida.


## Dupla aprovada — revisão posterior de 06/11

Atlas MaosMarcus_aprovadas.png é a última arte explicitamente aprovada pelo autor; apoio corrigido para a pose aberta original, empunhadura mantém versão da dupla. Resources/Art/Sinuca/MaosMarcus.png é o recurso ativo; individuais antigos preservados como histórico. Fonte copiada sem edição de pixels; ImageGen embutido, prompt em Arte/Producao/Sinuca/Prompt_maos_aprovadas.txt. Apoio usa duas profundidades e ponto central no eixo, mão traseira acompanha taco. 55 verificações numéricas PASS e 0 erros C#; render nativo/HLSL/Player pendentes por falta de acesso. Plano: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-maos-aprovadas.md


## Apoio menor / empunhadura sobre taco

Apoio reduzido 16,7%; empunhadura usa fila posterior ao taco e ZTest Always, só nos pixels opacos. 56 verificações PASS; compilação C# 0 erros/17 CS0649. Render/HLSL não conferidos. Evidências: rig-apoio-menor-oclusao.txt e compilacao-apoio-menor-oclusao.txt. Plano: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-apoio-menor-oclusao.md


## Mão esquerda redesenhada de cima

Recurso novo MaoEsquerdaTaco.png substitui espelhamento. Apoio preservado. 57 verificações numéricas/assets PASS; compilação geral bloqueada por SinucaZeComments ausente (SinucaMinigameUI.cs:102), sem stub/remoção de código concorrente. Render/Play ainda pendente. Arte gerada ainda não aprovada pelo autor. Plano: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-esquerda-desenhada.md
