# Ida e volta entre a calçada e a conveniência do posto

Autor @BBCONTTI (Git local), com Codex. 08/11/2026. Unity 6000.6.0f1, EntradaDaCidade.

O autor pediu uma aproximação pelo Marcus 2D e uma saída animada pela porta. O mapa de zonas do Obsidian e o código indicam que a rua e a conveniência convivem na mesma cena. O posto já usava uma câmera 3D em RenderTexture e deixava a câmera da rua ativa com máscara zerada. A consulta ao Graphify ficou indisponível.

- CameraFollow aproxima a projeção da cabeça de Marcus antes do corte e devolve o enquadramento normal na calçada.
- SceneFader e Capitulo1Director coordenam a troca de câmera. PlayerController suspende o controle na rua enquanto a animação do posto está em andamento.
- Posto3D anima mão, maçaneta e porta. A câmera ortográfica projeta a rua em uma textura ao vivo, aplicada a um plano na câmera de perspectiva. Marcus é desenhado separadamente em um plano de sprite. O movimento desses planos liga a vista da rua à da porta na ida e na volta.
- Uma câmera de composição mantém o Display 1 ativo enquanto as outras duas renderizam em texturas, evitando o aviso do Game View.
- A opção de reduzir efeitos mantém a troca por fade. O caminho padrão mantém a imagem visível durante a passagem.

A fixture em Play Mode posicionou Marcus junto à porta e acionou os métodos normais de transição, sem usar slots. Registrou zoom de 3,32 vezes, giro do plano de aproximadamente 73 graus, abertura de 100 graus da porta, sprite separado e câmera ativa no Display 1 durante ambas as passagens. A câmera ortográfica e o controle voltaram ao estado normal. As capturas do Game View foram inspecionadas visualmente. O vídeo foi montado a partir de quadros, sem áudio. Sem validação em build standalone. [Captura em movimento](../Evidencias/PostoReforma/transicao-posto.mp4).

O refresh do Graphify falhou porque o executável está indisponível. A validação do Graphify encontrou fontes ausentes no índice.