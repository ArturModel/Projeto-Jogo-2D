# Arte do orelhão no clima da exploração

Autor do repositório @BBCONTTI. Execução em 09/11/2026 às 00:00 BRT. Unity 6000.6.0f1, projeto C:/Temp/SantaRita. Arte integrada.

## Pedido e diagnóstico

Redesenhar o close-up do orelhão com base no print da exploração. Foram consultados o sprite MarcusWalk, o Guia Visual, a direção de arte, as notas do Obsidian e o layout de PayphoneJogoUI. A imagem anterior tinha recorte âmbar forte, reflexos dourados difusos e sombreado mais suave que a câmera do jogo. O CLI Graphify estava indisponível no shell.

## Execução

- [x] Recriar o close-up com grupos de pixels visíveis, luz fraca de poste, asfalto carvão, vegetação em sombra e orelhão ocre gasto.
- [x] Preservar manga comprida, pose junto ao telefone, mão livre oculta e espaço para instruções à esquerda.
- [x] Substituir Assets/Resources/UI/OrelhaoJogo/Rua.png e preservar o GUID. Chuva e névoa vêm da camada animada existente.
- [x] Conferir proporção, conteúdo, enquadramento do aparelho e limites dos controles no Canvas. Nenhuma cena ou script alterado.

## Prompt e procedência

OpenAI ImageGen integrado. “Redesenhar o close-up em 16:9 com a pixel art da exploração e MarcusWalk como referência. Grupos de pixels com leitura de 512×288, sombras carvão e oliva, asfalto e meio-fio anônimos e luz pálida localizada. Orelhão ocre escurecido e oxidado. Preservar o enquadramento, a placa metálica vazia, a manga comprida e o braço livre oculto. Sem chuva pintada, texto ou interface.”

## Validação

Comparação visual com a arte anterior, o print da exploração e o sprite original. PNG RGB 1671×941. A placa comporta as teclas existentes na referência 640×360. A verificação da composição animada no Play fica separada da conferência da arte.

O Unity reimportou Rua.png em Play Mode, com 1671 × 941 px, filtro Point e uma única camada de mip. O modal não estava aberto durante a conferência. A sessão de Play foi preservada. Graphify refresh e validate não concluíram porque o CLI local está ausente.
