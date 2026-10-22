# Direção de produção — Santa Rita

Atualização autorizada pelo autor em 21/10/2026. Esta direção prevalece sobre letreiros automáticos e conveniências narrativas da implementação anterior. O mapa e os fatos da Bíblia continuam sendo as autoridades de geografia e cânone.

## Arte

Correção expressa do autor nesta revisão: mapa orienta geografia e conexões; composição de ruas, praça e mobiliário pode ser redesenhada para uso plausível. Centro é uma praça cívica/comercial em funcionamento. Remover exposição dedicada a Júlia da praça; memória e pistas ficam em fontes discretas e contextualizadas. Cemitério é cena externa independente. C1/C2 são organização de produção, com retorno pelos dois caminhos existentes.

A imagem de referência fornecida pelo autor em 21/10 (`Docs/Referencias/Direcao_Visual_Aprovada.jpg`) define a meta ambiental: vegetação em camadas, matéria e desgaste legíveis, bordas orgânicas, arquitetura rural e luzes quentes localizadas. Marcus (`Assets/StreamingAssets/MarcusWalk.png`) ancora a escala dos atores; harmonizar seu acabamento com essa direção junto dos NPCs. Comparar cada personagem no tamanho nativo e na câmera do jogo. Não confundir resolução do arquivo com densidade real de desenho.

- Referência de mundo: 32 pixels/unidade; preservar exceções documentadas do legado sem reescalonar tudo indiscriminadamente. Personagens novos em células de 64, pés alinhados, corpo útil próximo de 48 pixels.
- Não gerar lotes independentes sem uma folha de referências comum. Personagens, móveis e fachadas devem compartilhar perspectiva, paleta e tamanho dos detalhes.
- Materiais: asfalto contínuo com agregado discreto; fissuras, remendos e poças em camada de detalhes; meio-fio separa passeio e pista; terra e grama possuem bordas orgânicas. Marcação acompanha a via e termina nos cruzamentos. Repetição de módulos deve ser pouco perceptível na câmera normal.
- Interiores organizados por função: circulação, atendimento, trabalho, estoque e uso cotidiano. Desgaste conta a história do lugar, sem lixo aleatório. Mercadinho e conveniência têm mobiliário, estoque e composição diferentes.
- URP 2D: sprites que recebem luz precisam de material lit; a presença de Light2D sozinha não comprova iluminação. Luz ambiente mantém leitura; lâmpadas e lanterna modelam volumes. Sombras de contato, oclusão, primeiro plano, chuva e névoa têm hierarquia e custo limitado. Pós-processamento não deve borrar pixels nem esconder pistas.
- Preservar fontes Aseprite, tags, paleta e exportação reproduzível. Point, sem mipmaps e sem compressão destrutiva para pixel art. Registrar origem/licença por asset em ASSET_SOURCES.csv.

## Exploração e narrativa

- Não anunciar automaticamente o nome do local por aproximação ou mudança de cena. Não usar títulos flutuantes. Placas físicas são possíveis quando o estabelecimento realmente as teria; informação é descoberta por observação e exame.
- Prompt de proximidade informa a ação possível, sem revelar conteúdo, identidade desconhecida ou solução: conversar, examinar, abrir porta.
- Cada personagem tem ocupação, necessidade imediata e limite de conhecimento. A rodoviária atende passageiros e horários; a Prefeitura trabalha com registros; o jardineiro cuida dos ralos. O mundo continua acontecendo sem Marcus.
- Só reconhecer Marcus se houver vínculo estabelecido ou apresentação anterior. Antes disso, usar papel/contexto em vez de nome na interface. Não contar a desconhecidos detalhes que só o jogador sabe.
- Evitar uma cadeia de moradores que espontaneamente distribuem a solução. Marcus apresenta uma pergunta apoiada no que descobriu; a resposta abre hipótese, não fornece certeza sem fonte. Revisitas mudam apenas por informação realmente transmitida.
- Separar no diário documento, testemunho, interpretação e percepção. Café morno e desordem sugerem interrupção; não provam sozinhos sequestro. Não transformar dedução em fato.
- Terror psicológico cresce do cotidiano, antecipação e contradição. Não depender de ruído alto, sustos repetidos ou NPCs falando como narradores do enredo. Não antecipar revelações de Lucas.
- Preservar opções de visita fora de ordem e progressão aprovada. Mudanças em requisitos exigem revisão do grafo e dos saves. Escola continua adiada.

## Áudio

- Músicas fornecidas (menu, bairro/casa do avô, Bar do Zé) são fontes Suno do autor; preservar arquivos, atribuição e escopo.
- Efeitos têm eventos concretos: passos por deslocamento real, superfície, porta ao atravessar, folha ao examinar, item ao adquirir, bolas ao colidir. Não disparar efeito de coleta só porque uma janela abriu.
- Preferir gravação/foley ou biblioteca licenciada para sons materiais; síntese é adequada a UI, protótipo e efeitos abstratos, mas deve ser identificada como tal. Auditar licença por arquivo, não presumir que todo um site é CC0.
- Evitar múltiplos sistemas tocando o mesmo evento. Respeitar volume geral/efeitos, pausa, limites de vozes e repetição. Música reduz suavemente durante pista/partida e retorna sem reiniciar o loop.
- Verificação técnica: duração, pico, clipping, loop e escopo. Audição humana é necessária para aprovar naturalidade e mixagem; não atribuir essa aprovação a um teste automatizado.

## Evidências

Comparar cenas antes/depois na mesma câmera, iluminação e escala. Registrar vídeos para animação e capturas para arte. Exercitar cena no Editor e no executável. QA usa saves isolados; fixture não equivale a campanha jogada. Não chamar a arte de aprovada pelo autor sem avaliação dele.
