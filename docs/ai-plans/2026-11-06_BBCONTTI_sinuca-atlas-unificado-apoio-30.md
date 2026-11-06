# Sinuca — atlas único, pegada espelhada e apoio -30%

Autor @BBCONTTI (git config user.name), execução Codex, 2026-11-06T16:47-03:00, America/Sao_Paulo.
Ambiente Windows / SantaRita / Unity 6000.6.0f1. Status integrado e compilado; render nativo pendente.

Pedido: espelhar nova empunhadura, produzir ambas mãos juntas para igualdade de pele/estilo; depois reduzir apoio em 30% no próprio jogo. Contexto recente de memória/arquitetura e código conferido; skills ImageGen/pixel-art/QA já lidas na conversa. Índices são auxiliares; Graphify segue indisponível.

Executado:
- [x] Atlas único ImageGen usando apoio aprovado e nova pegada superior como referências. Empunhadura espelhada NA IMAGEM: nós à direita e pulso à esquerda/baixo, polegar quase oculto. Paleta/contorno/agrupamentos de pixels harmonizados na mesma geração.
- [x] Saída exec-310cb919-ae16-4d03-99be-371ecdfcd1a5.png copiada sem edição de pixels para Resources/Art/Sinuca/MaosMarcusUnificadas.png + meta novo (Point, sem mipmaps/compressão/resize NPOT). Fonte em Arte/Producao/Sinuca/MaosMarcus_unificadas.png e prompt em Prompt_maos_unificadas.txt.
- [x] HandSprite volta a carregar ambas células de um único recurso; apoio à esquerda, pegada à direita. Nenhum mirrorX adicional em runtime.
- [x] Apoio .30m → .21m (30% menor na escala linear) nas DUAS camadas, pivô/contato/posição mantidos.
- [x] Pegada largura da célula .16m, pivô (.55,.50), preservando escala útil aproximada da arte anterior que tinha mais margem e outra proporção. UV de dissolução atualizada para pulso à esquerda. Oclusão sobre taco preservada.
- [x] QA/documentação/procedência atualizados. Cenas, saves e arte histórica preservados; sem commit/push.

Arquivos: Assets/Scripts/SinucaRoom3D.cs; novo MaosMarcusUnificadas.png + meta; Tools/QA/verify-sinuca-rig.py; fonte/prompt em Arte/Producao/Sinuca; ASSET_SOURCES.csv; evidências/memória/plano/arquitetura.

Validação: 57 checks numéricos/assets PASS (rig-maos-unificadas-30.txt); compilação geral Assembly-CSharp passou 0 erros/17 avisos CS0649 (compilacao-maos-unificadas-30.txt). Referência SinucaZeComments que bloqueava rodada anterior já não bloqueou esta compilação; nenhum código desse sistema alterado aqui.

Nova consulta de console MCP tentada; Play/render/HLSL/Player continuam não verificados nesta execução. Arte nova ainda sem aprovação do autor; aprovação de folhas anteriores não se transfere automaticamente. Graphify refresh indisponível, validate hashes antigos; não falsificados. Pendência: revisão do novo atlas e escala em Play.
