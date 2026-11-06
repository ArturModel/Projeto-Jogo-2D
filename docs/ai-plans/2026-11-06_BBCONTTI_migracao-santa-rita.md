# Migração para pasta única SantaRita

Autor: agente Codex, sessão do usuário local BBCONTTI (identidade GitHub não consultada).
Data: 06/11/2026, America/Sao_Paulo.
Status: migração concluída. Ambiente: Windows, Unity 6000.6.0f1.

## Solicitação
Criar uma pasta nova a partir da main e migrar melhoria-minigames com o menu dos minigames.

## Execução
- Repositório independente em C:/Temp/SantaRita, origin oficial atualizado por fetch.
- main e melhoria-minigames em 7072744e; origem da branch: C:/Temp/SantaRitaPR6Merge.
- Patch local preservado em C:/Temp/santa-rita-migracao-local.patch; alterações de BochaMinigameUI e MenuAmbiente migradas.
- Menu cria o texto antes de configurar o botão e registra os botões dinâmicos para seleção ao abrir/voltar.
- Objetos LFS copiados do cache existente: 2019 arquivos materializados, zero ponteiros pendentes.
- Raiz Git: C:/Temp/SantaRita. Projeto para abrir no Unity: C:/Temp/SantaRita/Projeto Jogo 2D.
- Pastas anteriores preservadas.

## Contexto e verificações
Memória e notas arquiteturais consultadas na cópia de origem durante o diagnóstico do menu; implementação conferida no C# real. Branch inclui origin/main, Git independente, hash da bocha idêntico à origem e git diff --check sem problemas. Graphify indisponível: refresh falhou pela ausência de graphifyy==0.9.96; não houve instalação de ferramentas. Verificação visual/compilação no Unity pendente; editor aberto anteriormente apontava para outra cópia.

## Arquivos
BochaMinigameUI.cs, MenuAmbiente.cs, este plano e entradas de memória da migração. Sem commit/push nesta execução.

Validador Graphify executado: índice desatualizado em relação aos C# migrados; refresh bloqueado pela ferramenta ausente.
