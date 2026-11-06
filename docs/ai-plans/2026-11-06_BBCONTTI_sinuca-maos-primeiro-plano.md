# Sinuca: mãos em primeiro plano e Zé estático

Autor: @BBCONTTI (git config user.name); execução Codex.
Data/hora: 2026-11-06 15:49 UTC-03, America/Sao_Paulo. Windows / C:/Temp/SantaRita / Unity 6000.6.0f1.
Status: implementado e compilado; revisão visual in-game ainda pendente.

## Solicitação

Nas capturas do autor, mãos quase de perfil viraram linhas; pediu orientar como billboards dos NPCs e esconder mãos ao caminhar. Retirou expressamente toda circulação/animação/taco 3D do Zé, mantendo tacada simulada. Nas novas capturas aprovou a pose do apoio, mas apontou corte reto pelo tampo e pediu empunhadura melhor encaixada e mais antebraço.

## Contexto e diagnóstico

Lidos memória INDEX/CURRENT_STATE, notas manuais de arquitetura/catálogo/cenas; código real dos três scripts da sinuca e parcial Actors. Direção/skills pixel-art e QA já consultadas na sessão. Capturas fornecidas pelo autor e textura atual inspecionadas. Graphify query indisponível.

Rotação fixa de 112/115 graus deixava quads vistos de perfil. Após orientar para câmera, a profundidade opaca da mesa escondia punho/antebraço. Alterar só pivô/ângulo não resolve a oclusão por inteiro. Empunhadura também ficava perto demais da borda inferior durante recuo.

## Executado

- [x] Mãos orientadas à câmera; empunhadura segue posição/recuo do taco sem herdar sua inclinação. Pequeno roll de -15 graus, largura .37 m e contato -.64 m para enquadrar antebraço.
- [x] Apoio mantém a arte/pose aprovada pelo autor, pivô e ancoragem ao contato.
- [x] Câmera URP Overlay CameraTacoEMaos, layer 29 somente para taco/mãos, após base da sala layer 30. Profundidade limpa pela câmera overlay (default confirmado no fonte local UniversalAdditionalCameraData m_ClearDepth=true, propriedade pública somente leitura). Impede corte pelo tampo preservando profundidade entre os três elementos.
- [x] Pose/projeção/lente/aspect da câmera de primeiro plano sincronizadas à câmera da sala; saída continua no mesmo RenderTexture pela pilha URP. Luzes da sala incluem layer 29.
- [x] Caminhando: só taco; duas mãos ocultas.
- [x] Removidos campos e métodos de caminho, passos, pose, tempo de preparo e taco do Zé; Actors voltou ao código original do repositório. Coroutine escolhe e executa jogada com breve espera, sem posição física ou ator móvel.
- [x] Verificações antigas de percurso do Zé/carregado com mão removidas; QA de projeção agora lê contato/largura/roll e pixels opacos reais para medir enquadramento do braço.
- [x] Plano, memória, arquitetura, procedência e evidências atualizados. Sem modificar PNGs, GUIDs, cenas ou saves.

## Arquivos

SinucaRoom3D.cs; SinucaMinigameUI.cs; SinucaRoom3D.Actors.cs restaurado ao comportamento original; Tools/QA/verify-sinuca-walking.py; Tools/QA/verify-sinuca-rig.py; notas/relatórios/Arte/Producao/Sinuca/README.md.

## Validação e limites

Compilação Assembly-CSharp contra referências locais Unity: resultado em ../Evidencias/SinucaMecanicas/compilacao-primeiro-plano.txt. QA de rig: 22 PASS em rig-primeiro-plano.txt. Projeção sem oclusão de cena mostra 100% da amostra opaca em repouso/meia força e 81,5% em força máxima; parte final do antebraço sai da borda inferior como modelo FPS. Circulação de Marcus sem lógica de NPC: 71 PASS em circulacao-sem-npc.txt, executados ao remover o NPC. São verificações numéricas/estáticas, não renderização da câmera overlay.

Falhas de desenvolvimento corrigidas: atribuir clearDepth falhou por propriedade somente leitura (usado default documentado no fonte do pacote); typo de sintaxe no harness corrigido antes da execução final. Sem esconder falhas ou tratar compilação como resultado visual.

Graphify refresh FAIL por ferramenta ausente; validate FAIL por hashes C# desatualizados. Sem edição manual dos índices.

Unity MCP sem sessão na revisão anterior; CUA negado anteriormente para Unity, sem repetição/bypass. Sem Play/Player/captura própria nesta etapa. Necessário conferir pilha URP real, profundidade entre mão/taco, perspectiva do braço, tabelas e força máxima na câmera do jogo. Não declarar aprovação visual por conta da compilação/QA; capturas antes desta correção são do autor.

Sem commit, push, build Player, alterações de saves ou regeneração de cenas.
