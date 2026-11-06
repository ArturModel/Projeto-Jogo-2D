# Sinuca — mão esquerda original em vista superior

Autor @BBCONTTI (git config user.name); execução Codex; 2026-11-06T16:43-03:00, America/Sao_Paulo.
Ambiente Windows / SantaRita / Unity 6000.6.0f1. Status: nova arte integrada; validação visual pendente, compilação geral bloqueada por referência externa.

## Pedido e diagnóstico

Autor esclareceu que simples espelhamento não satisfaz: empunhadura deve ser desenhada como mão esquerda e vista mais de cima para baixo, no ângulo do esboço; polegar fica quase oculto pela mão fechada. Apoio deve permanecer. Esboço/atlas e implementação atual conferidos; memória INDEX/CURRENT_STATE e notas de arquitetura/catálogo/mapa consultados. Skills ImageGen/pixel-art/QA já lidas nesta conversa. Código é fonte real; Graphify indisponível.

## Execução

- [x] Nova arte original ImageGen usando atlas somente para estilo e esboço para pose. Dorso visto de cima, nós à esquerda, pulso curto à direita/baixo e polegar recolhido no lado distante. Sem espelhar a mão anterior.
- [x] Segunda edição de limpeza de alpha; saída exec-5b662fe1-aeb8-4536-b75d-e9f4efe53ead.png copiada sem edição de pixels. Fonte 1536×1024 RGBA, alpha 0–254. Amostras da região de halo visível no preview têm alpha zero; shader faz alpha clip 0.5. Não afirmar alpha interno 255 inexistente.
- [x] Recurso dedicado Resources/Art/Sinuca/MaoEsquerdaTaco.png, meta próprio Point/sem compressão/mipmaps/resize NPOT. Fonte em Arte/Producao/Sinuca/MaoEsquerdaTaco_vista_superior.png e prompt em Prompt_esquerda_desenhada.txt.
- [x] HandSprite carrega nova textura inteira só para empunhadura; apoio mantém a mesma célula do atlas e tamanho .30m.
- [x] Removidos mirrorX e rotação corretiva -12°. Desenho já traz orientação. Continua orientado ao eixo projetado do taco, largura .22m da textura inteira, pivô (.45,.50), contato físico -.72m inalterado.
- [x] Ajustadas UVs da dissolução do pulso para textura nova; empunhadura mantém material na frente do taco com recorte na silhueta.
- [x] Verificação existente usa nova textura/projeção, sem simular espelhamento; documentação/procedência registradas.

## Arquivos

Assets/Scripts/SinucaRoom3D.cs; novo Assets/Resources/Art/Sinuca/MaoEsquerdaTaco.png + meta; Tools/QA/verify-sinuca-rig.py; fonte/prompt em Arte/Producao/Sinuca; ASSET_SOURCES.csv; evidências rig/compilacao-esquerda-desenhada.txt; memória/arquitetura/plano.

## Validação e bloqueios

57 verificações numéricas/assets PASS; silhueta nova 100% enquadrada nas amostras de aspecto/força em zoom padrão. Não comprova anatomia, render ou perspectiva em jogo. PNG gerado inspecionado; aprovação artística do autor ainda não recebida para esta versão.

Compilação geral falhou CS0246: SinucaMinigameUI.cs(102,22) referencia SinucaZeComments, tipo/arquivo ausente ao consultar Assets/Scripts. Referência apareceu em mudanças concorrentes ao trabalho da mão; não é um tipo introduzido por esta alteração. Compilações anteriores passavam. Não inventado stub nem removido código alheio para falsificar validação.

Consulta read_console MCP tentada. Histórico no_unity_session e CUA recusado; nenhuma tentativa de contornar acesso. Render/Play/HLSL/Player não verificados.

Graphify refresh falha por dependência indisponível; validate por hashes antigos. Índices não falsificados. Sem saves/cenas editados, commit ou push. Pendências: resolver referência SinucaZeComments no trabalho correspondente e conferir nova arte em Play com autor.
