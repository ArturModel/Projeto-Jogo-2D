# Bocha — iluminação e ambientação
Autor: @BBCONTTI (git user.name: BBCONTTI), execução Codex
Data/hora: 07/11/2026 00:29 America/Sao_Paulo
Status: implementado e verificado em fixture no Editor; avaliação artística do autor e Player pendentes.
Ambiente: Windows, Unity 6000.6.0f1, Projeto Jogo 2D@e163f99d040b4865.

## Solicitação
Clarear os arredores da bocha sem perder o clima sombrio nem o protagonismo da iluminação da cancha; acrescentar decoração funcional e arquitetura para diminuir a aparência de caixa vazia.

## Diagnóstico e contexto
AGENTS.md, memória INDEX/CURRENT_STATE, visão geral/catálogo/topologia Obsidian, direção de arte, critérios vivos e skills santa-rita-pixel-art/qa consultados. Código real: BochaCourtView e BochaRoom3D; shaders SalaLit e ContatoSala existentes. A sala é criada em runtime, não é uma cena dedicada. Reboco dependia de NativeSurface do bar, cuja ausência gera fallback de madeira. Graphify query indisponível (comando ausente); fonte atual usada como autoridade.

## Executado
- [x] Captura antes na câmera de caminhada.
- [x] Seis arandelas com bulbos, braços, chapéus e eletrodutos; dois pendentes secundários. Luz principal mantida em 8.5, secundárias menores, apenas luz principal com sombra.
- [x] Reboco independente do bar, com variação arquitetural do shader existente; madeira e pedra reutilizadas do projeto.
- [x] Pilastras, bases/capitéis, mãos francesas, cornijas, sanca de entrada e alcova de equipamentos.
- [x] Mural de avisos, quadros de flâmulas, relógio, armário ventilado/caixa, aparador/térmica/caneca e vassoura/escova de saibro.
- [x] Conferência em quatro ângulos na câmera da sala, mural reposicionado para não atravessar pilastra.
- [x] 273 amostras livres nos corredores e faixa de lançamento; 17 luzes, 222 renderers, 0 shaders inválidos.
- [x] Console final sem erros, compilação concluída; fixture descartada, Editor devolvido ao MainMenu sem salvar cena.
- [x] Refresh/validate executados com falha externa documentada abaixo.

## Arquivos
Modificado o arquivo já preexistente e não rastreado Assets/Scripts/Capitulo2/BochaRoom3D.cs. Alterações anteriores em BochaCourtView/BochaMinigameUI/MenuPausa e arte da mão preservadas. Nenhuma cena salva/regenerada, nenhuma gravação de slot pessoal. Criados este plano, relatório e capturas em Docs/Evidencias/BochaAmbientacao; memória e nota arquitetural atualizadas.

## Validação e limites
Fixture explícita: instância isolada de BochaRoom3D em Play, Camera.Render em RenderTexture 1280×720. Não equivale a input físico, campanha completa ou build Player. Não houve alteração no kernel, regras, saves ou progressão. Compilação passou após corrigir substituição textual literal durante a edição. As quatro capturas finais estão em Depois_0 a Depois_3.png; qa.txt contém resultados executados.
Graphify refresh: FAIL, graphify indisponível (solicita graphifyy 0.9.96). Validate: FAIL, índice anterior não inclui BochaRoom3D e nove arquivos recentes da sinuca. Não foi alegada atualização do grafo.

## Pendências
Avaliação artística do autor e execução no Player; restaurar Graphify em tarefa própria. Sem commit/push.


Complemento de encerramento: ao sair do Play foi detectada MissingReferenceException preexistente em SetActive durante OnDisable, quando a raiz já havia sido destruída. SetActive agora verifica root != null. Recompilação e chamada com raiz ausente passaram; nenhuma alteração visual adicional.
