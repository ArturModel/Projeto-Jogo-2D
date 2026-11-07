# Saves de trabalho antes da sinuca e da bocha

Autor @BBCONTTI (BBCONTTI), via Codex  
Data e hora 07/11/2026, 10:57 (São Paulo)  
Status concluído, com validação estática  
Ambiente SantaRita em C:/Temp, Unity 6000.6.0f1

## Solicitação

Preparar os slots 2 e 3 nos pontos da campanha imediatamente anteriores à sinuca e à bocha para trabalhar na transição 2D para 3D.

## Diagnóstico e execução

- [x] Confirmar a instância do Editor pelo caminho registrado no log e consultar a memória, o cofre e a skill de QA. A consulta Graphify falhou por indisponibilidade do executável.
- [x] Confirmar no leitor de saves a precedência dos JSONs por slot e a numeração do menu. Derivar os dois estados de trabalho do slot 1 com os requisitos de progressão verificados no código.
- [x] Preparar o slot 2 em EntradaDaCidade no interior do Bar do Zé, junto ao gatilho da mesa, com o desafio aceito. Preparar o slot 3 em SociedadeBeiraRio, junto à interação da cancha, com o capítulo 2 e as pistas exigidas.
- [x] Conferir a ausência dos dois slots no diretório e nas preferências, instalar os arquivos e verificar o conteúdo relido. Preservar o save geral e o slot 1 byte a byte.

## Arquivos e verificação

Foram instalados `santa_rita_save_v2_slot_1.json` e `santa_rita_save_v2_slot_2.json` no armazenamento persistente local do jogo. O plano e o changelog são as únicas alterações no repositório.

Os arquivos foram validados pelo esquema v2, pela numeração do menu, pelos requisitos dos duelos e pelas coordenadas dos gatilhos. Na Bocha, uma verificação estática da cena encontrou caminho livre desde a chegada e colocou Marcus a cerca de uma unidade do gatilho de raio 1,6. A janela do Editor não foi autorizada para controle. A carga e a transição em tempo de jogo seguem sem verificação.

O refresh de Graphify/Obsidian falhou porque Graphify está indisponível. O validador acusou scripts de mudanças preexistentes ausentes no índice.
