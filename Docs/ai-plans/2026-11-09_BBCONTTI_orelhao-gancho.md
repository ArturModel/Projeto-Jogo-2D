# Orelhão interativo no jogo

- Autor local @BBCONTTI com implementação por Codex
- Data 09/11/2026, horário de São Paulo
- Ambiente Santa Rita, branch `melhoria-minigames`, Unity 6000.6.0f1
- Status Implementado e compilado

O pedido foi preservar o zoom na cena ao vivo e usar o aparelho para pegar o monofone, discar um registro e recolocá-lo para gravar.

Foram consultados a memória, as notas de arquitetura do Obsidian e o índice Graphify. O código de `SantaRitaInspecao` e `PayphoneCheckpoint` confirmou o enquadramento existente e o ponto único de persistência manual.

- [x] `Assets/Scripts/PayphoneJogoUI.cs` apresenta o gabinete com monofone móvel, berço, visor verde e teclado. A seleção de registro fica disponível quando o fone sai do gancho.
- [x] `Assets/Scripts/PayphoneCheckpoint.cs` conecta E ao gancho, mantém Esc como cancelamento e preserva a carga explícita por # ou Enter. O retorno do fone confirma o save selecionado.
- [x] A síntese original separa retirada, tom de linha, DTMF por tecla e reposição. A retirada dura 0,85 s e alcança −9,7 dBFS. A reposição dura 0,20 s e alcança −10,4 dBFS.
- [x] Compilação com o response file do Unity e Roslyn concluída sem erros. O compilador apontou um aviso preexistente em BochaMinigameUI.

A sessão do Editor já estava em Play Mode. Validação visual, audição e regressão interativa permanecem pendentes. Consultei o guia de verificação em Play Mode da Unity CLI.


Graphify atualizou o grafo e o cofre Obsidian. A validação passou para 181 fontes C#, com caminhos portáveis e hashes atualizados.
