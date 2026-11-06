# Prólogo rural — execução de 06/11/2026

Solicitação: integrar o fundo ao deslocamento da estrada; chegar mais cedo à terra com duas curvas e percurso menor; separar solo de vegetação e pista; melhorar motor e rodagem de terra.

Implementação: horizonte distante sem casas/postes/cercas embutidos. Atlas 4x2 com árvores/arbustos/casas projetados por posição no mundo, mesma trajetória da pista. Distância governa UVs e elementos; parar o carro congela o cenário. Duas curvas suaves contínuas em espaço, horizonte acompanha rumo. Terra aos 140–166 m (aproximadamente 12–14 s); desaceleração 34–44 s; manobra ao recuo 36–44 s; transição após 51 s. Recuo de terra livre de arbustos, sem a imagem rejeitada de chegada. Próxima cena/saida_carro preservados.

Três materiais de solo: asfalto, terra compactada e solo com folhas/capim. Acostamento exposto com bordas de ruído suave e faixas de alpha liga pistas ao solo. Materiais de superfície usam Repeat/Bilinear/mipmaps; atlas/cabine/horizonte preservam Point. Correção observada: os dois novos materiais inicialmente ficaram Clamp após importação anterior à recompilação do importador; reimportados e conferidos Repeat.

Motor: gravação real com filtragem para perspectiva interior. Rodagem de terra: gravação real independente, troca gradual com o asfalto e ganho/pitch dependentes da velocidade. Mixer único preserva preferências/foco/pausa. Fontes/licenças e métricas em Arte/Producao/PrologoJogavel/Audio.md; reprodução por Tools/Audio/prepare-prologo-loops.py.

Validação em Unity MCP, instância Projeto Jogo 2D@995a38c4a4c14731, pasta confirmada. Sem build por preferência expressa do autor. Oito imagens presentes; shader sem erro. Fixture explícita no Play reposiciona tempo/distância e inicializa áudio para inspeção, não é campanha integral: seis malhas válidas com menos de 18 mil vértices; solo e terra presentes em canais distintos; posições finitas; quatro fases de áudio e pausa/retomada verificadas. EditorChecks.txt preserva saída final. Erros de reflexão/inicialização da fixture nas tentativas anteriores foram corrigidos na execução; não eram exceções do loop normal do jogo.

Captura visual de terra inspecionada no Play: vegetação em perspectiva, solo distinto e transição visíveis. Capturas mantidas fora de Assets. Uma captura final de parada saiu preta por captura/fade assíncrono e não comprova qualidade visual; não usar como evidência artística. Nenhuma aprovação visual ou audição subjetiva do autor é presumida. Saves pessoais não foram usados, criados ou carregados. Novos testes completos de controles/skip não foram repetidos; fluxo de interação preservado.

Fechamento: chegada em EntradaDaCidade com PrologoConcluido em RAM observada no Play (Transicao.txt). O primeiro observador expirou aos 30 s após reinício da viagem e falhou por tempo curto; a observação posterior confirmou chegada. Console final sem entradas de erro; Play encerrado. Refresh e validate PASS: 116 C#.
