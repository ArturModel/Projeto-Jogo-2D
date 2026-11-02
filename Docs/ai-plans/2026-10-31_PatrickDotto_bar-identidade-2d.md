# Bar do Zé — identidade 2D compartilhada com a sinuca

- Autor: Patrick Dotto (git config user.name; gh indisponível), execução Codex.
- Data: 31/10/2026, America/Sao_Paulo.
- Ambiente: Windows, Unity 6000.6.0f1/URP, branch atualizacao_sinuca, base 30d811c, alterações anteriores preservadas.
- Status: concluído tecnicamente; verificação visual/funcional executada, sem aprovação artística presumida do autor.

## Solicitação
O autor rejeitou a discrepância visual do bar Ref7: manter exploração 2D, sprites/texturas e qualidade do restante do jogo. Ao jogar sinuca, mudar a perspectiva preservando essa arte. Esta decisão substitui a projeção dos modelos em blocos para sprites de exploração.

## Contexto e diagnóstico
INDEX/CURRENT_STATE, três notas manuais de arquitetura, Graphify (SantaRitaBarLayout/SinucaRoom3D), skills pixel-art e QA e direção de produção consultados. Fontes reais confirmaram prioridade de BarRef7/Baked em SpriteFor, corpo 3D em blocos e outro atlas de madeira. Marcus, BalcaoBar, MesaPlasticaBar e Madeira foram inspecionados visualmente.

## Execução
- [x] Sprites originais de Zé e frequentadores, incluindo poses originais sentadas/bebendo.
- [x] Móveis nativos, mesas/cadeiras plásticas, balcão detalhado, rádio, copos e chimarrão; escala e ocupação revistas.
- [x] Piso e parede extraídos da própria arte do interior C1; material WorldLit como os demais interiores.
- [x] Sinuca 2.5D: malhas com geometria/UV/pivôs dos mesmos sprites, mesa/bolas/redes com profundidade 3D. Sem outra anatomia dos NPCs.
- [x] Oito complementos Aseprite na escala/paleta nativa: banco, placas/foto, taqueira, engradado, tapete, bandeira histórica e cuia/térmica.
- [x] Variação de pose atualiza malha e textura; câmera consulta o entorno sem alargar sprites.
- [x] Desativar a antiga exportação de modelos 3D para substituir a arte 2D. Fontes anteriores preservadas, sem uso na exploração.
- [x] Preservar regras, mira absoluta, contato na branca, física, inventário e saves.
- [x] QA Editor: 325 PASS; Player QA final: 325 PASS; duas builds sem erros e mesmo hash.
- [x] Refresh/validate: 109 fontes; seis arquivos pessoais preservados por hash; snapshot em RAM restaurado sem diferenças serializadas.

## Arquivos
SantaRitaBarLayout, SantaRitaWorldLighting, parciais SinucaRoom3D, authoring explícito SinucaRef7Authoring, importador apenas para pivô central do tapete, QA de paridade e teclado virtual dedicado; oito PNG/metas e fontes Aseprite, Tools/Aseprite/bar-identidade-2d.lua, ASSET_SOURCES.csv, documentação. Nenhuma cena foi regenerada ou salva.

## Validação
O teste compara referências reais de Sprite e textura entre exploração e perspectiva, material lit, pose original do Zé, três câmeras/projeção, mouse, efeito, carga/soltura, física/regras, aquisição/entrega/mapa, portas e ausência de autosave. O primeiro Editor passou arte/sinuca, mas falhou ao injetar E no teclado físico sem foco. O harness passou a ter teclado virtual dedicado, sem mudar as regras de interação da campanha.

A build QA anterior Ref7 terminou em crash nativo no UnityPlayer; sua rodada não concluiu e não é prova de qualidade da versão nova. O lote estático de runtime foi substituído por combinação explícita das malhas das redes, mantendo elementos móveis. Não atribuir a causa do crash sem prova; validar o Player novo.

## Limites
As figuras são capturas reais de fixtures isoladas, não uma campanha legítima completa. O alvo é identidade visual uniforme, não aprovar arte pelo autor. Resultado final em Docs/Evidencias/BarIdentidade2D/Relatorio.md. Sem commit/push.

## Fechamento

[Relatório final](../Evidencias/BarIdentidade2D/Relatorio.md), fontes Aseprite e capturas reais conferidas. Circulação adicional: 934 células ligadas à soleira com o collider real, todos os NPCs e examináveis acessíveis por aproximação. Primeira tentativa com círculo inadequado foi corrigida no diagnóstico, sem mudança de física. Editor deixado em Play no bar, com snapshot inicial restaurado, sem gravar cenas ou saves. ProjectSettings temporários de teste foram restaurados. Limites: fixtures não são campanha integral; há pico isolado de tempo de quadro; build normal conferida por compilação/metadata/hash, com avaliação visual da sala realizada no Editor e Player QA.

Após restaurar o Editor, TMP atualizou o cache da fonte já modificada anteriormente. A diferença de hash foi rastreada somente a esse asset e está explicitada no relatório; sem descartar o trabalho anterior.
