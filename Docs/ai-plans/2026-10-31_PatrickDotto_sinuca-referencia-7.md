> Direção visual substituída pelo pedido posterior do autor: manter o bar 2D e usar a mesma pixel art na sinuca em perspectiva. Ver 2026-10-31_PatrickDotto_bar-identidade-2d.md. Este plano não constitui aprovação artística e a rodada Player Ref7 foi interrompida por crash nativo antes de concluir.

# Sinuca e Bar do Zé — referência 7

- Autor: Patrick Dotto (git config user.name; gh indisponível), execução por Codex.
- Data: 31/10/2026, America/Sao_Paulo.
- Ambiente: Windows, Unity 6000.6.0f1, URP 2D + renderer 3D dedicado, branch atualizacao_sinuca, base 30d811c; alterações anteriores preservadas.
- Status: implementação e validação em andamento; sem aprovação artística do autor.

## Pedido
Recompor o bar em direção à imagem 7, corrigir caçapas externas/redes, proporções, personagens/objetos em 3D e fluidez; simplificar mira, alvo anunciado e efeito; compactar inventário com imagens dos objetos; bandeira do Inter com distintivo vigente em 1994. Preservar as regras brasileiras, bola marcada, exploração e saves.

## Diagnóstico
Índices CURRENT_STATE/INDEX, notas manuais do Obsidian e consulta Graphify confrontados com C# atual. A antiga projeção reconstruía 15.837 vértices de UI e custou 33,848965 ms por reconstrução na medição local (20 invocações; 1536x672). O valor zero de bytes do coletor antigo não demonstra ausência de alocação. Uso de sprites planos ampliados produzia distorção. Importador geral também sobrescrevia pivôs da arte nova.

## Execução
- [x] Sala persistente com geometria 3D, texturas, luz/sombras e câmera URP em RenderTexture; UI usa um quad.
- [x] Mesa de pano 2,84 × 1,42 m, bocas externas, couro e redes volumétricas com reação à queda.
- [x] Mobiliário funcional, rádio, garrafas, TV, calendário, placas, taqueira, engradados e futebol.
- [x] Planta comum para exploração e minigame; sprites exportados dos modelos, pivôs por apoio no chão.
- [x] Mouse absoluto, câmera independente da mira, botões para contornar; três vistas e zoom.
- [x] Efeito somente no disco da branca; alvo com explicação contextual.
- [x] Inventário compacto de cinco slots com imagens, estado derivado do snapshot.
- [x] Fontes Aseprite e procedência dos assets.
- [ ] QA Editor, QA Player e build normal com evidências finais.
- [ ] Verificação final dos saves e índices.

## Dependências compartilhadas indispensáveis
`SantaRitaArtImporter`: excluir somente BarRef7 da padronização geral para permitir atlas brutos e pivôs de projeção. `SantaRitaPlaceDressing`: impedir decoração antiga de substituir objetos do grupo BarCoerente quando presente. `Capitulo1Director`: preaquecer sala durante fade de entrada no bar. Demais salas e cenas aprovadas não foram regeneradas.

## Arquivos principais
`SinucaTableView`, quatro parciais `SinucaRoom3D`, `SinucaMinigameUI`, `SantaRitaBarLayout`, `SantaRitaInventory`, authoring Editor `SinucaRef7Authoring`, QA `SinucaRef7RuntimeVerification` e extensão da regressão R4; renderer/material/texturas em Resources/Art/BarRef7 e Settings. Fontes em Arte/Producao/BarRef7. Lista final no diff; alterações anteriores não são atribuídas a esta etapa.

## Verificação e limites
A primeira execução detectou uma ambiguidade de reflection no coletor de performance, corrigida especificando VertexHelper. Outra tentativa não recebeu clique de coleta com GameView sem foco; o harness Editor passou a rotear seus dispositivos virtuais explicitamente e restaurar a configuração ao encerrar. Não mascarar essas tentativas como PASS. O Player continua usando o comportamento normal de input.

Resultados finais serão registrados em Docs/Evidencias/SinucaRef7/Relatorio.md. A referência é direção artística; não declarar reprodução exata, perfeição ou aprovação visual do autor. Não houve commit/push.
