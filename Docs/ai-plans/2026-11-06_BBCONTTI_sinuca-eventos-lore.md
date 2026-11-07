# Sinuca — lore e eventos aleatórios

Autor: @BBCONTTI (Git `user.name`: BBCONTTI); execução: Codex.
Data: 06/11/2026, America/Sao_Paulo. Status: implementação e testes técnicos concluídos; avaliação de ritmo/audição pendente.
Ambiente: branch `melhoria-minigames`, C:/Temp/SantaRita, Unity 6000.6.0f1, Windows, .NET 10.0.402.

## Solicitação e diagnóstico

Conferir os DOCX V5 Backgrounds e Bíblia R1, atualizar a referência Markdown do projeto, melhorar os dois eventos da sinuca para não sobrepor nem se concentrar no começo e adicionar ocorrências compatíveis com a lore.

R1 já está representada em Docs/Santa_Rita_Biblia_Consolidada_R1.md. A V5 contém decisões superadas (Lucas morto, cidade vazia e fichas como saída); nenhuma foi reintroduzida. Os prompts embutidos nos documentos foram tratados como conteúdo de referência, não ordens de execução. A sinuquinha de quinze bolas e os avanços posteriores do projeto prevalecem sobre propostas mecânicas históricas.

Consultados AGENTS.md, memória INDEX/CURRENT_STATE, notas manuais de visão geral/catálogo/cenas, direção artística/narrativa/áudio, critérios vivos e skills locais de narrativa, áudio e QA. Confirmada a instância Unity Projeto Jogo 2D@e163f99d040b4865 e raiz real. Graphify query indisponível: comando não instalado; código C# real confirmou os gatilhos.

Antes: batidas após 2,2 s de turno elegível a partir do segundo turno; falsa bola no primeiro turno elegível a partir do terceiro. Flags únicas por série e ausência de exclusão compartilhada explicavam concentração e sobreposição.

## Executado

- [x] Referência Docs/Lore_Atual.md: cronologia, backgrounds, fatos versus percepções, limites de revelação, catálogo e ideias futuras explicitamente separadas.
- [x] SinucaPerceptionSchedule independente de Unity/regras/saves: relógio de gameplay, 40–85 s iniciais, 55–115 s entre eventos, limites de avanço de turnos, bolsa embaralhada sem repetição consecutiva, um Active por vez.
- [x] Integração com a UI: sorteio em turno de Marcus, de pé, fora de carga/painéis/falas; duração limitada; fim de mão não reinicia sorteio da série; fila narrativa preservada no encerramento natural.
- [x] Sob a mesa opcional expira após 24 s; inspeção pode terminar por E/Q/Esc ou 12 s. Bola falsa expira após 16 s, exame ou troca de turno. Permanecem percepções fora do solver.
- [x] Água no cano e eco da festa: eventos de 7 s, síntese original já existente, legendas sem spoiler; sem download, nova dublagem ou música.
- [x] Pausa/foco/painéis/cursor livre suspendem apresentação; treinos/fechamento limpam estados. Sem mudança de cena, progressão, saves ou regras físicas.

## Arquivos

Modificados: Assets/Scripts/SinucaMinigameUI.cs e SinucaMinigameUI_Perception.cs.
Criados: Assets/Scripts/SinucaPerceptionSchedule.cs (+ meta Unity), Docs/Lore_Atual.md, Tools/QA/SinucaPerceptionSchedule/{Program.cs,SinucaPerceptionSchedule.csproj,README.md}, este plano e atualizações aditivas de memória/visão geral.
As alterações preexistentes do autor em atores, bolas, sons e outros arquivos foram preservadas. Nenhum commit/push.

## Validação e evidências

- Teste .NET com o C# aplicado: PASS, 4.131.611 verificações, 1.000 sementes, 24.000 eventos; 46 segundos iniciais distintos; os quatro tipos podem abrir. Cobre exclusão, espera, turnos, estados inseguros, fala ocupada, reset, repetição e distribuição em séries longas. Rodar: `dotnet run --project Tools/QA/SinucaPerceptionSchedule/SinucaPerceptionSchedule.csproj`.
- Unity: primeiro pedido de compilação só de scripts não importou o arquivo novo e retornou CS0246. Refresh completo de Assets resolveu; console posterior sem erros/avisos, tipo carregado em Assembly-CSharp. Teste no assembly real: 10.000 verificações de exclusão e cooldown, quatro aberturas possíveis.
- Regressão existente `python Tools/QA/verify-sinuca-mechanics.py`: 53 PASS. Aviso CS0414 somente no harness recortado, não no assembly Unity. Cobre física/mira/puxada/colisões/encaçapamento/repouso/áudio sintetizado.
- Graphify refresh: FALHOU porque graphify não está instalado. Validate: FALHOU por índice desatualizado, incluindo parciais preexistentes e o novo agendador. Não instalar dependências ou afirmar índice atualizado silenciosamente.
- Não produzida uma build Player nova; não realizada campanha completa, input manual de cada evento ou audição/artística. O Editor foi mantido no projeto e nenhum estado do duelo em execução foi usado como fixture nem alterado por teste. Teste Unity instanciou apenas agendadores independentes.

## Pendências

Ouvir/testar o ritmo em uma série real 2–0 e 2–1; ajustar os intervalos a partir dessa avaliação. Eventos não são garantidos em séries rápidas e nenhuma revelação essencial depende do sorteio. Reinstalar a ferramenta local Graphify conforme a operação do projeto e então regenerar/validar seus índices. Arte/falas de telefone/reflexo/homenagem são ideias documentadas, não integrações concluídas.
