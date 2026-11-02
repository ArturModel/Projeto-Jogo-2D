# Bar do Zé — identidade 2D compartilhada

Revisão de 31/10/2026, branch atualizacao_sinuca, base 30d811c, com alterações locais anteriores preservadas. Substitui a direção Ref7 rejeitada pelo autor. Não é aprovação artística presumida.

## Resultado

A exploração voltou aos sprites originais de Zé/frequentadores, balcão, mesas e cadeiras, com piso/parede recuperados da arte C1 e material WorldLit como os demais interiores. A sinuca consome as mesmas fontes de sprites, geometria e UV em uma apresentação 2.5D; mesa, bolas e redes conservam profundidade 3D. A direção agora parte da arte 2D do jogo e segue para a perspectiva, sem exportar corpos/móveis em blocos para substituir a exploração.

Oito complementos possuem fonte Aseprite reproduzível: banco, placa/foto, taqueira, engradado, tapete, bandeira histórica e cuia/térmica. O sprite antigo de chimarrão incluía uma mesa e foi substituído por uma cuia própria para não haver uma mesa em miniatura sobre a outra. A transcrição do cartaz acompanha novamente a arte de galeto. Rotinas e poses preexistentes foram mantidas; na perspectiva a troca de pose atualiza malha e textura juntas.

## Verificação

| Evidência | Resultado |
|---|---|
| Editor, regressão integrada | 325 verificações PASS, incluindo identidade de fontes, materiais, controles e saídas. |
| Player QA final | 325 verificações PASS + encerramento runtime em 31/10/2026 21:14:54 UTC. |
| Circulação no Editor final | 934 células conectadas; aproximação a cinco NPCs, sete elementos examináveis e à mesa. BFS com o BoxCollider2D real (0,50 × 0,28; offset 0,14), passo 0,25. Não equivale a percurso humano. |
| Builds QA e normal | Succeeded, zero erros, Windows x64/D3D11. |
| Integridade da build normal | Sem tipos de autoria/QA; mesmo hash de fontes/cenas/dados/texturas/materiais/metas da build QA. |
| Saves pessoais | Seis arquivos com SHA256 idêntico ao início desta tarefa. |
| Sessão do Editor | Snapshot original restaurado em RAM e comparado campo a campo, sem diferenças serializadas; devolvido a Interior_BarDoZe, cena sem alterações não salvas. |
| Graphify/Obsidian | Refresh e validate: 109 fontes C#, hashes/caminhos íntegros; índice estrutural, não aprovação de arte. |

Hash das duas builds finais: `f8a49adb2fd08208adc6d42d20c9ff0da6acee1fe2c41b853d37161eb1b392fc`.

A regressão exercita três câmeras e mira inversa, mouse absoluto sem realimentação da câmera, contato na branca, carga/soltura/cancelamento, uma mão física de 45 tacadas (36 × 34), término de série/callback, aquisição/entrega/mapa, snapshots, música e saídas sem autosave. São fixtures isoladas e reposicionamentos declarados, não uma campanha legítima completa. Os refinamentos posteriores de cuia/transcrição foram compilados e cobertos pela rodada final do Player; o Editor final também foi inspecionado.

O primeiro ensaio Editor de saídas falhou pela injeção no teclado físico sem foco; o harness passou a usar teclado virtual dedicado. A primeira consulta adicional de circulação usou um círculo inadequado no rodapé da soleira; a repetição utiliza exatamente tamanho/offset do collider do jogador. Nenhum destes problemas exigiu mudar as regras das portas. A rodada Player Ref7 anterior terminou em crash nativo e não é tratada como PASS; a rodada atual terminou normalmente.

## Desempenho medido

Player QA final, 1920 × 1080, target 60, vSync 0, 180 quadros: mediana 16,67 ms; p95 16,69 ms; máximo 118,18 ms. Houve um pico isolado: não declarar 60 fps sem exceções nem extrapolar a outras GPUs. Reconstrução do quad da UI: 0,002853 ms em média, quatro vértices, 200 invocações reais após aquecimento; a sala não é refeita em malha de UI por quadro. Esta medição do quad não representa todo o custo de renderização.

## Artefatos locais

- `Editor/bar-recomposto.png`, `Editor/ref7-Mesa3D.png`, `Editor/ref7-LinhaTaco.png`, `Editor/ref7-VistaSuperior.png`.
- `PlayerQA/bar-recomposto.png` e `PlayerQA/ref7-*.png`: capturas reais do executável, com marca Development.
- `editor-final.png`: câmera real do Editor com o estado da sessão restaurado.
- `Editor/runtime-qa.txt`, `PlayerQA/runtime-qa.txt`, relatórios de build, `PlayerNormal/assembly.txt`, `circulacao-editor.txt` e `saves-preservados.txt`.
- Build normal: `Build/Capitulo2/SantaRita.exe`.

Capturas/logs extensos continuam ignorados no Git. Somente fontes e evidências textuais finais são versionáveis; nenhum commit/push foi realizado. Os modelos/projeções anteriores ficam preservados como histórico, sem prioridade na exploração. Escolas/C3, geografia, núcleos de regras/física e sistema de save não foram reabertos.

## Cache de fonte após devolver o Editor

Depois das builds, o TMP atualizou o cache da fonte `Assets/Resources/Fonts/VT323-Regular Bitmap.asset` ao restaurar a sessão. Foi o único arquivo em Assets escrito após a build final; o arquivo já possuía trabalho anterior e não foi revertido. Hash da árvore após essa atualização de cache: `c21c7186ad38e6837a2e517da51cf985048eff89e68410634ba874167ecb5c47`. O código, cenas e sprites não foram alterados depois das duas builds de hash `f8a49a...`; não confundir o hash do cache atual com uma nova build executada.
