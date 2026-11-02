# Sinuquinha de quinze bolas — 31/10/2026

Branch atualizacao_sinuca, base 30d811c; trabalho local anterior preservado. [Plano executado](../../ai-plans/2026-10-31_PatrickDotto_sinuquinha-15.md).

## Regra e apresentação atuais

O pedido mais recente substitui a Sinuca Brasileira de sete bolas. Zé oferece lisas/listradas ou identificação por 1–7/9–15. São quinze bolas de objeto, incluindo a preta, mais a branca. Grupos de sete, preta por último, saída com branca posicionada, primeira conversão legal define o grupo e permite continuar. Faltas concedem branca na mão; preta prematura ou com branca encerra a mão para o adversário. Preta na saída é recolocada. Série melhor de três e consequência narrativa preservadas.

Não há regra de par/ímpar (com a 8 reservada os grupos ficariam desiguais). É regra da casa de bola 8, sem alegar implementação integral de WPA. A referência primária para composição do conjunto e grupos é [WPA, seção 8-Ball](https://www.wpapool.com/wp-content/uploads/2026/01/2026.01.02-WPA-Rules.pdf); as simplificações locais estão no kernel e nas regras da UI. No complemento mais recente, o autor pediu novamente a bola marcada nas mãos seguintes. Da segunda mão em diante o X fica visível: encaçapá-la anula a tacada inteira, restaura posições/atividade das bolas e concede branca na mão ao adversário. A preta e a última bola disponível do grupo nunca são bloqueadas.

Raio ampliado de 3,9 para 5,4 unidades; esferas com números, discos claros, lisas e faixas nas 9–15. O quaternion físico gira a textura conforme o deslocamento. Mesmo solver de movimento/rebote, ampliado para dezesseis corpos. IA avalia o grupo válido e evita preta antecipada/primeiro contato ilegal. Guia inclui a direção da bola colorida no contato; não é garantia da trajetória completa após outros choques.

Órbita livre, controles em relação à projeção da câmera, vista do taco levantada para ver a branca e luminária ocultada apenas na superior. Sem restrição nem marcação D do modelo anterior. Tapete sob a mesa removido; capacho preservado e acima do piso. TV/geladeira ampliadas e parede reorganizada para evitar sobreposição da TV com a bandeira.

A arte do bar continua 2D, compartilhada com a perspectiva. Bandeira mantém a versão de mundo e ganha detalhe de exame 256 px com fonte Aseprite. Papéis/cadernos/recibos/calendário/quadro e avisos reconhecidos têm conteúdo tipográfico no exame, com leitura integral rolável; os sprites pequenos não viram texto raster esticado. Fotos/objetos preservam a arte existente, sem alegar reconstrução em alta definição de cada imagem do jogo.

Marcus conserva rosto/roupa/paleta. Apoio, passagem e balanço das pernas foram refinados; menor oscilação do torso, folha equipada alinhada à lente da lanterna e contatos sonoros nos quadros 0/4 do relógio por distância. Fontes Aseprite/GIFs ficam em Arte/Producao/Marcus. Escala revista nesta execução no bar; os demais interiores usam as dimensões existentes do dressing, sem uma reautoria global de C1.

## Verificação

Editor final: 279 verificações + PASS runtime em 01/11/2026 02:14:04 UTC (31/10 no fuso local). Inclui 115 tacadas na mão completa, duas conversões físicas de preta para encerrar série, marcada nas mãos 2/3, teclado, caminhada/apoio e carga explícita do slot 5 apesar de último slot 1. Player QA final: também 279 verificações + PASS runtime em 01/11/2026 02:20:10 UTC (31/10 local), com a mesma mão física de 115 tacadas e fim de série/carga. Build QA Succeeded, zero erros. Build normal Succeeded, zero erros; mesmo hash da build QA. A DLL normal contém SinuquinhaRules e exclui tipos de QA/autoria. Capturas são locais, não aprovação artística.

A suíte nova é explícita (-srSinuquinhaQA), somente Editor/Development, com namespace de save temporário. Exercita regras, entrada no bar, caminhada e apoio sonoro, documentos/bandeira, capacho, escolha do Zé por EventSystem/Input System, identidade de fontes, mouse/projeção nas três vistas, órbita oposta/360, saída física, mão inteira com decisões da IA para os dois lados e fim de série/callback. A fixture final coloca grupos já limpos e preta junto da caçapa para duas conversões físicas; não injeta vitória na campanha. Nenhum teste de sete bolas é usado para aprovar a regra nova.

## Limites

Não equivale a campanha legítima completa, audição humana ou aprovação artística do autor. A legibilidade do conteúdo é garantida pelo leitor tipográfico; retratos/objetos continuam pixel art, e não fotografias reconstruídas. A composição aprovada permanece 2D; personagens da perspectiva usam os mesmos sprites em malhas orientadas à câmera. A melhoria de escala foi localizada no bar, não uma modificação indiscriminada de todos os ambientes.

Capturas e logs extensos continuam ignorados no Git. Somente fontes e evidências textuais finais são versionáveis. Não houve commit/push nem migração de saves. Escola/C3 e geografia não foram reabertas.


## Complemento do autor — teclado, lista e carga na pausa

A/D passam a contornar em 360°; setas esquerda/direita ajustam a mira, com Shift para ajuste lento. Os botões de contorno foram retirados. A vista superior também aceita rotação e conserva projeção/inversa. A linha clara da branca tem contraste contra o pano; uma seta dourada indica a tendência da colorida, com legenda e limite na tabela.

Listas por jogador mostram as bolas já retiradas de seu grupo. ESC → Carregar Jogo usa o seletor existente de cinco slots, desabilita vazios e informa que o progresso em RAM será substituído. Cancelar conserva a pausa; carregar fecha o minigame e usa explicitamente o slot escolhido. Não há nova persistência nem autosave.

A mesa e duas cadeiras minúsculas do recorte foram localizadas na calçada do Bar do Zé: os sprites antigos foram substituídos por móveis plásticos do mesmo conjunto do interior, com larguras 1,35/0,65 unidades e colisores preservados. Dependência compartilhada indispensável: recuperação de referências de áudio/rim e sala após recarga de scripts do Editor, evitando erros de referência nula; nenhuma mudança nas faixas do autor.

Na auditoria, três arquivos diferiam da captura da tarefa anterior, mas suas datas de escrita (20:16/20:19) são anteriores ao snapshot desta execução (20:22). Eles foram preservados, sem restauração sobre o progresso do autor. Uma nova captura SHA256, fora do repositório, foi criada antes da rodada final para verificar os seis arquivos durante a validação atual. Não alegar que coincidem com uma captura mais antiga.


A rodada intermediária detectou botões de slots vazios ainda interagíveis, apesar da ação de carga já rejeitá-los: agora ficam efetivamente desabilitados no modo carregar e disponíveis no modo salvar. Outra rodada detectou referência falsa nula do SpriteRenderer durante a reconstrução do rim após reload; substituído o operador de coalescência por teste Unity == null. A rodada Editor final não registrou erros. As falhas intermediárias não são tratadas como PASS.


## Desempenho observado no Player QA

1920 × 1080, limite 60, vSync 0, 180 quadros: mediana 16,67 ms, p95 16,69 ms, máximo 195,31 ms. O pico isolado impede afirmar ausência total de travadas. Quad de UI: 0,006471 ms por reconstrução, quatro vértices, 200 invocações após aquecimento; não representa todo o custo da sala. A busca de peças da luminária passou a cache, sem percorrer a hierarquia a cada quadro.

## Evidências locais

- Editor/runtime-qa.txt e PlayerQA/runtime-qa.txt: rodada final, sem FAIL/ERROR.
- PlayerQA/bar.png, ref7-Mesa3D.png, mesa-LinhaTaco.png, bola-marcada.png, regra-segunda-mao.png: câmeras reais do executável.
- PlayerQA/cinco-slots.png e menu-carregar.png: seletor e pausa.
- PlayerQA/calcada-escala.png e marcus-andando.png: escala revisada e pose em movimento; não vídeo completo nem audição humana.
- PlayerQA/documento-legivel.png e bandeira-exame.png: leitura/detalhe.
- saves-preservados.txt: seis hashes iguais à referência da rodada final.

Capturas extensas permanecem locais e ignoradas pelo Git. Builds em Build/Sinuquinha15QA e Build/Capitulo2. Materiais/arte continuam sujeitos à avaliação do autor.


## Build final, devolução e cache do Editor

Executável: `Build/Capitulo2/SantaRita.exe`. QA e normal compartilham SHA256 `9e83b0af5bfb8c443722d2288673ae456e181ef4d2befe6e26bfd97cde9c6002`. Smoke de inicialização normal permaneceu respondendo, sem exceções C# nos padrões registrados; foi encerrado pelo harness. Houve avisos de cor WindowsMediaFoundation e recusa de ExclusiveFullscreen com fallback FullscreenWindow/medição de refresh do driver. Não são omitidos nem tratados como prova de compatibilidade com todo monitor/GPU.

Sessão mais recente do autor restaurada em RAM e comparada campo a campo, sem diferenças; Editor devolvido ao bar e sem cena suja. Indicador de progresso não salvo mantido porque a RAM não coincide com nenhum snapshot em disco. Seis arquivos pessoais com hashes idênticos à referência anterior à rodada final. Execução em segundo plano voltou à configuração original. `assembly.txt`, `smoke.txt` e `saves-preservados.txt` contêm as verificações compactas.

A inspeção adicional `Editor/guia-clara.png` coloca apenas branca/colorida alinhadas numa fixture de prática: guia da colorida ativo, seta de três vértices e contraste visível. Serve para conferir o significado das duas linhas; não representa uma mão legítima nem a quantidade normal de bolas. Depois a prática foi fechada e o snapshot do autor restaurado.

Após essa inspeção/devolução, o TMP gravou somente os caches `Assets/Resources/Fonts/VT323-Regular Bitmap.asset` e `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`. Não foram revertidos sobre trabalho anterior. Hash da árvore atual `c78b6f0b3eded126b1bcb907f3bd8cafed7dc512c84dc6eafb50987636d73d49`; código/cenas/sprites permanecem aqueles das duas builds verificadas. Não alegar uma nova build com esse hash de cache.

Refresh/validate estruturais: PASS, 111 fontes C# e quatro consultas reais; diff --check PASS. Esses checks não equivalem a aprovação artística.
