# Mãos da sinuca — 06/11/2026

Referência: `Assets/StreamingAssets/MarcusWalk.png`, inspecionada antes da autoria.

PNG RGBA originais selecionados em `Projeto Jogo 2D/Assets/Resources/Art/Sinuca/`.
MaoApoio: ponte esquerda vista por trás, polegar junto ao indicador; taco de referência removido com ImageGen para receber o taco 3D.
MaoTaco: mão direita fechada, dorso e nós dos dedos visíveis; substitui a palma anterior rejeitada pelo autor.

Origem: OpenAI ImageGen, gerado/editado nesta sessão com referência explícita de Marcus. Não é pacote CC0 nem captura de fotografia. Não há aprovação artística do autor presumida. As primeiras tentativas espalmadas foram rejeitadas e não são a arte selecionada.

Fontes achatadas PNG preservadas, sem edição destrutiva posterior. Aseprite não encontrado no caminho configurado nesta máquina; não foi fabricado um fonte com camadas ou animação quadro a quadro inexistente.

Animação por transforms: apoio separado e plantado no plano da mesa/tabela; empunhadura filha de TacoProporcional, com recuo/avanço reais. O plano da mão tem inclinação de perspectiva e pivot no contato. O taco carregado usa a mesma geometria e mão à direita da câmera. Pontos de contato e enquadramento completo ainda precisam de revisão no Unity após as capturas fornecidas pelo autor.


## Revisão posterior — mãos em primeiro plano / NPC parado

Pose do apoio aceita pelo autor, corte pelo tampo rejeitado. Mãos agora orientadas à câmera; taco/mãos em câmera URP Overlay isolada para evitar a oclusão do tampo. Empunhadura aumentada/aproximada e com roll, mostrando mais antebraço. Não houve nova geração/edição de PNG nesta revisão. Caminhada sem mãos, só taco. Movimentação/posicionamento/taco do Zé removidos expressamente; descrição e checks de NPC móvel acima são históricos. Compilação e 22 verificações do rig registrados em compilacao-primeiro-plano.txt / rig-primeiro-plano.txt; inspeção real da pilha URP e encaixe ainda pendente. Plano atualizado em Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-maos-primeiro-plano.md.


## Pegada fechada revisada — 06/11/2026

A última foto de empunhadura/alinhamento/tensão substitui a interpretação de palma aberta. Nova MaoTaco.png mostra dorso de mão fechada e pulso curto, gerada/editada por ImageGen a partir da pose de referência e estilo Marcus. Antiga arte frontal em Versoes/MaoTaco_dorso_frontal_rejeitada.png; tentativas abertas intermediárias não foram integradas. GUID preservado. Alinhamento pelo eixo projetado, dorso à frente do taco e dissolução em pixels na terminação do pulso. 0 erros C# e 22 QA de projeção/assets; HLSL/render/encaixe final ainda não verificados em Play. Fonte e limites: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-fechada.md. Nenhuma aprovação artística do autor presumida.


## Dupla aprovada — revisão posterior de 06/11

Atlas MaosMarcus_aprovadas.png é a última arte explicitamente aprovada pelo autor; apoio corrigido para a pose aberta original, empunhadura mantém versão da dupla. Resources/Art/Sinuca/MaosMarcus.png é o recurso ativo; individuais antigos preservados como histórico. Fonte copiada sem edição de pixels; ImageGen embutido, prompt em Arte/Producao/Sinuca/Prompt_maos_aprovadas.txt. Apoio usa duas profundidades e ponto central no eixo, mão traseira acompanha taco. 55 verificações numéricas PASS e 0 erros C#; render nativo/HLSL/Player pendentes por falta de acesso. Plano: Docs/ai-plans/2026-11-06_BBCONTTI_sinuca-maos-aprovadas.md


## Atlas unificado e apoio -30%

Fonte atual: MaosMarcus_unificadas.png; prompt: Prompt_maos_unificadas.txt; recurso: Resources/Art/Sinuca/MaosMarcusUnificadas.png. Ambas mãos geradas juntas, pele e estilo harmonizados; nova pegada superior espelhada horizontalmente na arte. Apoio .21m (30% abaixo da revisão anterior). 57 QA numéricos PASS, compilação 0 erros. Render/aprovação da nova folha ainda pendentes.
