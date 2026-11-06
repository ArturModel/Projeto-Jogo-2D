# Sinuca — empunhadura esquerda e ângulo de pulso

Autor @BBCONTTI (git config user.name), execução Codex, 2026-11-06T16:34-03:00, America/Sao_Paulo.
Ambiente Windows / SantaRita / Unity 6000.6.0f1. Status implementado; conferência visual pendente.

Pedido: autor identificou duas mãos direitas e pediu que a mão do taco fosse esquerda, no mesmo ponto, com ângulo mais condizente com pegada.

Contexto: memória INDEX/CURRENT_STATE e arquitetura recente conferidos; implementação e QA reais lidos. Demais direções/skills/catálogo/mapa já consultados nesta conversa; Graphify indisponível neste ambiente.

Execução:
- [x] Somente empunhadura espelhada no X no transform (sem regenerar ou editar pixels do atlas aprovado).
- [x] Pulso girado -12 graus ao redor do mesmo pivô de contato; mantém alinhamento relativo ao eixo projetado do taco.
- [x] PlaceHandDepth recebe mirrorX opcional; escala é recalculada a cada frame, sem alternância de sinal ou crescimento acumulado.
- [x] Apoio, tamanho, shaders de oclusão e movimento da puxada preservados.
- [x] Verificação de enquadramento existente atualizada para silhueta espelhada e rotacionada.

Arquivos: Assets/Scripts/SinucaRoom3D.cs; Tools/QA/verify-sinuca-rig.py; evidências rig-pegada-esquerda.txt e compilacao-pegada-esquerda.txt; memória/plano.

Validação: 56 checks numéricos/assets PASS; Assembly-CSharp compilado com 0 erros/17 avisos CS0649 legados. Consulta de console MCP tentada, histórico de no_unity_session e acesso CUA negado; render/Play não comprovados. Enquadramento numérico não comprova anatomia ou naturalidade. Arte, cenas, saves e configurações não modificados; sem commit/push.

Graphify refresh/validate tentados; dependência indisponível e hashes antigos. Pendência: conferir ângulo da mão esquerda em Play. Não atribuída aprovação artística deste ajuste ao autor.
