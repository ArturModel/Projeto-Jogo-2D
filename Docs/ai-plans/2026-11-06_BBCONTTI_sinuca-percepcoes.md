# Sinuca: bola falsa e sons sob a mesa

Autor: @BBCONTTI (BBCONTTI), implementação com Codex.
Data: 06/11/2026, America/Sao_Paulo.
Status: implementado; compilação no Editor confirmada; avaliação final de gotas e audição pelo autor pendente.
Ambiente: Windows, Unity 6000.6.0f1, branch melhoria-minigames.

## Solicitação
Adicionar uma bola falsa disfarçada e uma sequência de ouvir/olhar sob a mesa. Marcus fala na mesma legenda do Zé com efeito mais grave. Feedback posterior: substituir batida sintetizada por impacto de madeira, corrigir bloco preto e remover poça grande, deixando apenas gotinhas. Autor solicitou commit e push das alterações da sessão.

## Contexto
Consultadas Bíblia narrativa, direção de arte/narrativa/áudio, memória persistente, notas manuais do cofre e skills locais de narrativa, áudio e QA. Código atual é a fonte da implementação. A lembrança remete à cisterna e à festa que abafava o pedido de ajuda, sem revelar Lucas.

## Executado
- [x] Bola percebida com geometria/material/número de uma bola real, sem corpo no solver; exame por E e falsa previsão de contato.
- [x] Batidas em turno estável do jogador, aproximação e E para olhar sob o tampo; E/Q/Esc para sair.
- [x] Sequência transitória de madeira molhada/marcas e gotas, sem mudança no save ou nas regras da partida.
- [x] Legenda compartilhada com identificação Marcus/Zé, fila narrativa e murmúrio de Marcus com pitch 0.77.
- [x] Batida usa Audio/Foley/door, efeito Kenney CC0 já presente, em vez da batida sintetizada.
- [x] Corrigido erro de escala que convertia a água em bloco. Poça rejeitada foi removida: três gotas pequenas e escalonadas substituem superfície e ondas.
- [x] Preservadas alterações preexistentes da sala, sem regenerar/salvar cenas, modificar saves ou publicar revelações.

## Arquivos
SinucaMinigameUI.cs e SinucaRoom3D.cs recebem integração; novos parciais SinucaMinigameUI_Perception.cs e SinucaRoom3D_Perception.cs, com seus .meta. Memória e nota arquitetural atualizadas. Alterações preexistentes da sala e respectivas evidências seguem no commit solicitado.

## Validação e limites
Unity confirmou o método novo carregado; console sem erros C# após as últimas alterações. Captura isolada reproduziu o bloco preto e comprovou a correção de escala. A apresentação da poça foi rejeitada e substituída por gotas no código; não afirmar aprovação visual dessa última versão. Batida original substituída por asset de impacto de madeira existente (0.2656s). Não houve campanha integral ou build Player nova. git diff --check passou.
Graphify refresh falhou: ferramenta indisponível. Graphify validate falhou por novos fontes ausentes do índice; índice permanece desatualizado. Não instalar dependências nem inventar validação para concluir o envio solicitado.

## Pendências
Avaliar aparência das gotas, naturalidade da batida e jogar a sequência completa; atualizar Graphify quando disponível. Commit/push autorizados pelo autor em 06/11/2026.
