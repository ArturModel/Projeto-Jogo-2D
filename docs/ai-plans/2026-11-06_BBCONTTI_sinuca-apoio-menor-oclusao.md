# Sinuca — apoio menor e empunhadura cobrindo taco

Autor @BBCONTTI (git config user.name); execução Codex; 2026-11-06T16:31-03:00, America/Sao_Paulo.
Ambiente Windows / SantaRita / Unity 6000.6.0f1. Status: implementado e compilado; render nativo pendente.

## Pedido e diagnóstico

Autor aprovou visual em captura e pediu diminuir ligeiramente o apoio e impedir taco de atravessar empunhadura. Memória INDEX/CURRENT_STATE, arquitetura/catálogo/mapa, código e shader conferidos. Skills pixel-art/QA já lidas nesta conversa. Captura fornecida confirma interseção no dorso inferior: deslocamento uniforme de -.035m não garante oclusão ao longo da haste 3D que se aproxima da câmera.

## Executado

- [x] Apoio .36m para .30m (redução linear 16,7%) nas duas camadas idênticas. Pivô/contato preservados.
- [x] Apenas material de empunhadura usa _ZTest=8 (Always), renderQueue=2470, depois de taco opaco e apoio AlphaTest. Pixels transparentes continuam descartados antes de escrever cor/profundidade: quad não oculta cenário, somente silhueta da mão. Apoio mantém teste de profundidade padrão LEqual=4 e suas duas camadas.
- [x] Sprite, atlas aprovado, pose e geometria 3D preservados. Nenhuma nova geração de imagem, cena ou save editado.
- [x] QA existente atualizado para nova largura e regressão da configuração de profundidade da empunhadura.

## Arquivos

Assets/Scripts/SinucaRoom3D.cs, Assets/Resources/Art/Sinuca/MaoFlutuante.shader, Tools/QA/verify-sinuca-rig.py; evidências rig/compilacao-apoio-menor-oclusao.txt e notas deste plano.

## Validação e limites

56 verificações numéricas/assets PASS. Assembly-CSharp compilado: 0 erros, 17 avisos CS0649 legados/serializados. Isso não compila HLSL nem comprova render. Consulta MCP de console tentada; acesso visual ao Unity foi recusado anteriormente nesta conversa, não repetido. Aprovação da captura anterior não equivale a aprovação deste ajuste ainda não visto.

Graphify query/refresh indisponíveis; validate acusa hashes antigos. Nenhum índice falsificado. Sem commit/push nem alteração de saves/cenas. Pendência: conferir empunhadura/apoio em Play.
