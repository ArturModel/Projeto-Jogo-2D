# Sinuca — dupla de mãos aprovada e apoio corrigido

Autor: @BBCONTTI (git config user.name), execução Codex. Data/hora: 2026-11-06T16:23-03:00, America/Sao_Paulo.
Ambiente: Windows, C:/Temp/SantaRita, Unity 6000.6.0f1.
Status: imagem aprovada explicitamente pelo autor; integrada em código. Render nativo/Play pendente.

## Solicitação e decisão final

Refazer coerência/anatomia das mãos, contato central com taco 3D e oclusão. Após rejeitar a primeira ponte circular, autor pediu voltar à dupla da primeira folha e corrigir só apoio. Última folha foi aprovada com “ficou perfeito mete bala”. Aprovação se refere à imagem apresentada, não ao resultado ainda não visto em jogo.

## Contexto consultado

AGENTS, skills santa-rita-pixel-art / santa-rita-qa / imagegen; direção de arte, critérios de produção, matriz de técnicas, memória INDEX/CURRENT_STATE, visão geral, catálogo e mapa de cenas. MarcusWalk, duas artes antigas e referência do autor inspecionados. Graphify query indisponível; código real conferido. Instância MCP identificada, mas consultas retornaram no_unity_session; CUA recusou acesso ao Unity. Não houve bypass, cenas/saves/ram não manipulados.

## Executado

- [x] Arte criada/editada com ferramenta ImageGen embutida. Última saída exec-b8b58dc3-abed-4468-a600-5ba87fd903ea.png copiada sem alterar pixels.
- [x] Atlas RGBA 1774×887, duas células quadradas, cores/estilo compartilhados. Fonte preservada em Arte/Producao/Sinuca/MaosMarcus_aprovadas.png; prompt em Prompt_maos_aprovadas.txt.
- [x] Resources/Art/Sinuca/MaosMarcus.png + meta próprio, Point, sem mipmaps/compressão/resize NPOT. Artes antigas e GUIDs preservados.
- [x] HandSprite recorta células apenas por UV/Sprite.Create, sem processamento destrutivo dos pixels. Apoio largura .36m, contato UV (.569,.662) dentro da célula; empunhadura .18m, pivô (.5,.5). Origem do apoio coincide com o eixo projetado central do taco, removendo deslocamento lateral antigo de -.012m.
- [x] Ponte ancorada a -.25m da branca com altura .047m + elevação de tabela; não acompanha puxada. Rotação calculada pela projeção do eixo.
- [x] Mesma imagem do apoio em duas máscaras complementares no shader: dorso atrás (+.025m), polegar à frente (-.025m). Linha diagonal de separação acompanha junção do polegar. Compartilham tamanho/pivô; escala ao longo do raio da câmera mantém projeção/seam coincidentes. Geometria 3D do taco permanece.
- [x] Empunhadura acompanha taco em -.72m, deslocada -.035m em profundidade para ocultá-lo pelo dorso; escala compensada mantém contato projetado. Mãos continuam ocultas ao caminhar.
- [x] QA e documentos atualizados. Sem commit/push, cena regenerada/salva ou saves alterados.

## Arquivos

Assets/Scripts/SinucaRoom3D.cs; Assets/Resources/Art/Sinuca/MaoFlutuante.shader; novo MaosMarcus.png + meta; Tools/QA/verify-sinuca-rig.py; Arte/Producao/Sinuca/MaosMarcus_aprovadas.png e prompt; ASSET_SOURCES.csv; memória, arquitetura, plano e evidências.

## Validação

Assembly-CSharp.csproj compilado via dotnet build --no-restore: 0 erros, 17 avisos CS0649 de campos serializados/legados. Não é build Player nem compilação HLSL.

55 verificações numéricas/assets PASS em rig-maos-aprovadas.txt: atlas/importação/alpha; silhueta da empunhadura 100% enquadrada em 4:3, 16:9 e 21:9, força 0/.5/1, zoom padrão; identidade projetiva de camadas; contato estacionário sobre eixo móvel com/sem elevação da tabela; máscaras complementares e parentagem. Testes não comprovam anatomia, transições, zoom nem oclusão visual nativa.

read_console tentou consultar erros mas retornou no_unity_session. Captura/Play/HLSL/Player não verificados: conector sem sessão e CUA negou acesso. Não foi solicitado outro acesso nem simulada captura.

Graphify refresh falhou por executável indisponível; validate falhou porque hashes preexistentes estão antigos. Índices não falsificados.

## Pendências

Conferir em Play, principalmente contato do polegar, separação diagonal das camadas, zoom e puxada perto das tabelas. Arte isolada aprovada pelo autor; integração visual ainda pendente. Não alegar aprovação da renderização.
