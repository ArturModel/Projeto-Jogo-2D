# Paleta do sprite no orelhão interativo

- Autor local @BBCONTTI com implementação por Codex
- Data 09/11/2026, horário de São Paulo
- Ambiente Santa Rita, branch `melhoria-minigames`, Unity 6000.6.0f1

A captura do autor mostrou que o gabinete estava quase preto e sem a cor da cúpula. A leitura do Obsidian, do Graphify e de `UIFactory` confirmou que a textura escura compartilhada era multiplicada pelas cores das peças da interface.

- [x] Em `Assets/Scripts/PayphoneJogoUI.cs`, aplicar o laranja queimado do sprite à carcaça. A amostra da captura tem tons recorrentes RGB (129, 66, 30) e (146, 81, 38).
- [x] Separar casco, aço, monofone e visor com relevos e valores distintos. Texturas de pixel art são geradas em memória, com filtro Point.
- [x] Limitar a alteração à apresentação do orelhão no mundo.

O Editor estava em uma sessão de jogo já aberta. A revisão visual da nova paleta em Play Mode permanece pendente.

Compilação Roslyn dos scripts atuais concluída sem erros. Os avisos são de arquivos de Bocha e Capítulo 2.
