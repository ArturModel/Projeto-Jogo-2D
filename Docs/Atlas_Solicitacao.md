# PROMPT PARA CODEX — RECONSTRUÇÃO COMPLETA DO MAPA DO JOGADOR

## Objetivo principal

Quero substituir o mapa atualmente utilizado pelo jogador em **Santa Rita: A Noite das 03:17** por um sistema cartográfico interativo, bonito, detalhado e profissional, que represente fielmente o mundo realmente construído no Unity.

As três referências principais são:

**1. Stardew Valley — identidade visual**

Quero uma cidade ilustrada em pixel art, com ruas, calçadas, casas, telhados, árvores, jardins, estabelecimentos, rio, pontes e elementos arquitetônicos reconhecíveis.

O mapa deve possuir beleza artística própria, ser agradável de explorar e combinar com os sprites, cores, escala e ambientação de nosso jogo.

**2. Silent Hill 2 — investigação e atmosfera**

Quero um mapa que faça parte da experiência narrativa, permitindo que Marcus reconheça lugares, registre informações e acrescente anotações sobre locais investigados, passagens e acontecimentos relevantes.

As anotações devem transmitir a sensação de que Marcus está tentando compreender a cidade e organizar sua investigação, sem revelar informações que ele ainda não descobriu.

**3. Project Zomboid — fidelidade espacial e zoom**

Quero que o mapa corresponda efetivamente à organização do mundo jogável.

Ao aproximar o zoom, devem aparecer detalhes adicionais de ruas, edifícios, acessos, terrenos, pontos de referência e, quando apropriado, plantas dos interiores já explorados.

Essas três referências devem ser combinadas numa identidade original, adequada à cidade brasileira de 2004 e ao nosso terror psicológico.

**Não copie mapas, sprites, texturas, interfaces ou outros materiais protegidos desses jogos. Utilize-os exclusivamente como referências de design.**

---

## 1. Antes de implementar: compreender o projeto existente

Leia integralmente:

- `AGENTS.md` e as skills relevantes disponíveis no repositório.
- Os documentos de geografia, mapa, arte, arquitetura e narrativa.
- `MapaInterativoUI.cs`.
- `SantaRitaGeography.cs`.
- `Capitulo1Director.cs`.
- `Capitulo2Director.cs`.
- As cenas Unity, seus construtores, portais, entradas, saídas, colliders e dados de posicionamento.
- Os sistemas de GameState, diário, objetivos e salvamento.

Verifique o estado atual do projeto antes de modificar qualquer arquivo.

O sistema atual possui uma imagem-base `MapaSantaRitaArte_HD`, coordenadas UV calibradas, marcadores, zoom, arraste, rotas, abas e informações narrativas.

**Não quero apenas substituir essa imagem por outra de resolução superior. Quero mudar a arquitetura visual do mapa para que ela seja baseada na geografia verdadeira do jogo.**

Preserve os recursos úteis já existentes, mas elimine as dependências da antiga ilustração que causem divergências geográficas.

Não refaça os capítulos, as cenas ou os sistemas de movimentação para adaptá-los ao mapa. O novo mapa deve representar corretamente o mundo existente, e não o contrário.

## 2. A geografia jogável é a fonte da verdade

Este é o requisito mais importante de toda a tarefa.

O novo mapa deve reproduzir as posições relativas, dimensões, caminhos, ruas, edificações e conexões reais das regiões implementadas.

Não basta colocar o ícone de um edifício aproximadamente na localização correta.

Se determinada construção está à direita de uma rua, seu desenho no mapa deve respeitar isso. Se uma porta fica voltada para uma calçada, essa orientação deve ser mantida. Se uma rua faz uma curva, ela não pode aparecer como uma linha reta por conveniência.

Inspecione concretamente os espaços construídos no Unity.

Como o projeto utiliza tanto cenas independentes quanto regiões construídas em tempo de execução, não assuma que todos os objetos estarão disponíveis como GameObjects serializados antes do Play Mode.

Quando necessário, utilize os dados dos construtores, prefabs, tiles, limites de regiões, posições de objetos e portais para reconstruir a planta correta.

**Não presuma que as coordenadas de mundo de cenas diferentes pertencem ao mesmo sistema geográfico.**

Crie uma correspondência explícita e verificável entre:

- Coordenadas locais de cada cena ou região.
- Posição dessa região no atlas da cidade.
- Orientação, escala e limites.
- Portais de entrada e saída.
- Localização dos edifícios e pontos importantes.
- Posição de Marcus e dos objetivos.

Ajustes visuais na representação são permitidos, mas não podem falsificar a topologia, os acessos ou a localização dos lugares.

Se encontrar inconsistências entre a ilustração antiga e os cenários reais, use os cenários aprovados e as decisões geográficas mais recentes como referência.

## 3. Preservar a organização canônica da cidade

A organização aprovada continua obrigatória.

Em especial:

- O Bar do Zé permanece no setor da Rua Norte.
- O Bairro e a casa de Adilio conservam suas conexões reais.
- O Mercadinho é um único estabelecimento, com sua fachada na Encruzilhada e acesso aos fundos pela região do Centro.
- O Centro concentra praça, Igreja e conexões urbanas.
- O Cemitério permanece ao norte.
- A Sociedade Beira-Rio e sua cancha de Bocha permanecem a sudoeste, junto ao rio.
- A Prefeitura respeita a posição corrigida em relação ao Centro e ao Clube.
- A Escola pertence à região aprovada para conteúdo futuro, não deve substituir a Prefeitura.
- A Capela permanece na região inferior central do mapa.
- O Clube Recreativo, o Bairro Operário e o Galpão do Truco conservam as posições e conexões estabelecidas na documentação mais recente.

**Não antecipar a construção do capítulo 3.**

As regiões futuras podem aparecer na representação geral conforme o conhecimento disponível ao jogador, mas não devem ganhar interiores inventados, detalhes inexistentes ou informações narrativas antecipadas.

O nome da cidade deve ser configurável, sem provocar uma renomeação geral do projeto nesta tarefa.

## 4. Criar um atlas original de pixel art

A interface final precisa parecer um mapa feito especificamente para o nosso jogo, não um painel de debug, radar de GTA ou desenho genérico de cidade.

Quero que o jogador reconheça visualmente:

- Formato das ruas e calçadas.
- Casas e diferenças entre bairros.
- Telhados e edificações públicas.
- Igreja e praça central.
- Cemitério e sua organização.
- Rio, margens e pontes.
- Sociedade Beira-Rio.
- Posto de gasolina.
- Mercadinho.
- Bar do Zé.
- Árvores, cercas, jardins, terrenos e pequenos caminhos.

Utilize a linguagem de pixel art já estabelecida no jogo, mantendo perspectiva top-down ou três quartos, mas sem misturar escalas e perspectivas incompatíveis.

A estética precisa lembrar uma pequena cidade gaúcha em 2004: materiais envelhecidos, ruas úmidas, terrenos modestos, vegetação regional, casas comuns, comércios tradicionais e diferenças naturais de ocupação.

A atmosfera deve ser levemente melancólica e misteriosa, sem deixar o mapa completamente escuro.

**Não quero um mapa alegre e saturado demais como Stardew Valley, mas também não quero um mapa monocromático e ilegível.**

Use uma paleta autoral de cores moderadas, boa hierarquia visual e detalhes que fiquem agradáveis mesmo em telas grandes.

Produza assets com acabamento consistente. Evite texturas borradas, deformações, repetição excessiva de objetos e construções genéricas.

Prefira reutilizar e adaptar a arte aprovada quando ela for adequada. Crie novas artes somente quando forem realmente necessárias.

## 5. O mapa deve ter zoom com níveis de detalhe verdadeiros

O zoom não pode simplesmente ampliar uma imagem.

Quero um sistema em que a quantidade de informação apresentada seja progressivamente maior conforme o jogador aproxima a visualização.

### Nível 1 — Cidade inteira

Apresentar uma visão geral bonita e organizada.

Mostrar os principais caminhos, bairros, rio, grandes construções e regiões já conhecidas.

Os nomes devem ser legíveis, mas sem excessos.

O objetivo desse nível é permitir que o jogador compreenda a estrutura da cidade e se oriente.

### Nível 2 — Região ou bairro

Ao aproximar, apresentar mais detalhes:

- Ruas secundárias.
- Calçadas e caminhos.
- Casas individuais.
- Estabelecimentos identificáveis.
- Muros, cercas e jardins importantes.
- Portões e acessos principais.
- Orelhões conhecidos.
- Marcos narrativos e de exploração.

As construções devem corresponder aos cenários reais, não a representações arbitrárias.

### Nível 3 — Detalhamento local

Ao aproximar ainda mais, o jogador deve conseguir reconhecer:

- O formato das construções.
- Entradas e saídas conhecidas.
- Caminhos estreitos.
- Pátios e áreas externas.
- Elementos relevantes do cenário.
- Pontos investigados.
- Passagens descobertas.
- Objetos importantes quando fizer sentido representá-los cartograficamente.

**A aproximação deve revelar detalhes reais, e não inventar informações que não existem no jogo.**

Implemente níveis de detalhe progressivos, boa nitidez, rótulos dinâmicos e transição confortável entre escalas.

O zoom deve permanecer funcional e nítido até a aproximação máxima definida para a interface.

## 6. Interiores: representação contextual

Quero que o mapa consiga representar interiores importantes quando isso acrescentar valor ao jogador.

Por exemplo, após Marcus explorar a casa de Adilio, o jogador poderia selecionar a construção e visualizar uma planta simplificada dos ambientes conhecidos.

O mesmo vale, quando adequado, para:

- Bar do Zé.
- Igreja.
- Prefeitura.
- Rodoviária.
- Sociedade Beira-Rio.
- Outros interiores realmente construídos e explorados.

As plantas devem corresponder à organização real: paredes, salas, portas, balcões, mesas e passagens importantes.

Entretanto, **não obrigue o sistema a desenhar cada móvel de todos os interiores**. Escolha o nível de detalhe adequado à navegação.

Não mostre salas secretas, passagens não descobertas ou estruturas inexistentes.

Interiores pequenos podem usar apenas um destaque da entrada e o nome do estabelecimento.

Ao sair de um interior, o mapa deve continuar localizando Marcus corretamente na cidade.

## 7. Sistema de descoberta e anotações inspirado em Silent Hill

Quero que o mapa evolua junto com a investigação.

Implemente uma camada de anotações do jogador, integrada aos estados narrativos existentes.

Exemplos:

**Uma rua bloqueada:** Marcus encontra uma obra ou portão fechado e passa a ter essa informação registrada na região correspondente.

**Uma informação contraditória:** depois de investigar um local, uma pequena anotação pode aparecer próxima ao edifício ou em seu painel contextual.

**Um lugar importante:** depois de descobrir que determinado estabelecimento está ligado à busca por Adilio, o local pode receber uma marcação discreta.

**Um caminho descoberto:** o atlas atualiza os acessos conhecidos.

A linguagem visual pode utilizar discretamente lápis, tinta, círculos, riscos, símbolos e pequenas observações manuscritas.

Mas as anotações devem ser coerentes com o conhecimento real de Marcus.

Não transformar suspeitas em fatos confirmados.

Não inventar pistas ou antecipar revelações.

Não revelar Lucas vivo, sua localização, segredos do Clube ou informações de capítulos futuros.

Reutilize os estados narrativos existentes em `GameState` e nos sistemas de investigação.

Se forem necessários novos estados de descoberta, eles precisam ser compatíveis com snapshots e saves antigos.

**Não crie um sistema de fog of war que esconda artificialmente todas as construções.** A cidade pode ter um mapa de orientação geral; o que deve depender da exploração são os detalhes conhecidos, as anotações e os acessos descobertos.

## 8. A posição de Marcus precisa ser precisa

O indicador do jogador deve corresponder à localização verdadeira dentro da região.

Não quero que Marcus apareça apenas no centro aproximado do bairro enquanto caminha por diferentes ruas.

Quando estiver numa área externa, o marcador deve acompanhar corretamente sua posição no atlas.

Ao atravessar uma transição, não pode saltar arbitrariamente para uma rua que não corresponde ao destino.

Quando Marcus entrar num interior:

- No mapa da cidade, mostre sua localização associada corretamente ao edifício.
- Numa planta interna disponível, apresente a posição correspondente ao ambiente.
- Preserve a orientação e o contexto do local.

Teste diferentes pontos das regiões e todas as conexões principais.

A transformação entre cena e atlas deve funcionar em ida e volta, sem depender de ajustes visuais improvisados para cada ponto.

## 9. Substituir as rotas irreais

O mapa atual desenha uma linha pontilhada direta entre Marcus e o objetivo.

Não quero manter esse comportamento se ele atravessar edifícios, rios, muros ou regiões inacessíveis.

Prefiro um sistema de orientação discreto, coerente com a atmosfera de 2004 e com a investigação de Marcus.

Se houver uma rota exibida, ela deve seguir caminhos realmente conectados e respeitar acessos bloqueados.

Se ainda não existir um modelo confiável de caminhos, apresente a localização ou direção aproximada do objetivo conhecido, sem desenhar uma rota falsa.

Não quero uma interface de GPS moderno.

O mapa deve parecer uma ferramenta de consulta e investigação, mesmo contendo facilidades de navegação próprias de um videogame.

## 10. Interface completa e profissional

Redesenhe a apresentação do mapa dentro da interface do jogo.

Quero uma tela em que o mapa ocupe a maior parte do espaço disponível.

Evite o aspecto de um pequeno JPG enquadrado dentro de um painel com excesso de botões e textos.

Requisitos:

- Abertura pela tecla M, respeitando a regra atual de aquisição do mapa.
- Fechamento consistente com M ou Esc.
- Zoom pelo scroll do mouse e por controles alternativos.
- Arrastar com o mouse.
- Navegação pelo teclado.
- Centralizar em Marcus.
- Selecionar lugares.
- Consultar nomes e descrições.
- Destaque discreto do objetivo conhecido.
- Legenda de símbolos.
- Boa leitura em 16:9.
- Escala adequada para diferentes resoluções.
- Textos em português brasileiro.
- Nenhuma interferência nos controles da exploração.

Preserve o acesso ao diário e às fichas, que já existem no sistema atual.

Pode reorganizar as abas e painéis para melhorar a experiência, mas não elimine funcionalidades que já funcionam.

Uma possibilidade é manter o mapa como área principal e abrir informações adicionais num painel lateral discreto, somente quando necessário.

A interface deve ter caráter próprio: molduras, cores, botões, tipografia e transições condizentes com o jogo.

Evite excesso de efeitos, ícones pulsantes, cores neon e animações que atrapalhem a consulta.

## 11. Rótulos, ícones e hierarquia visual

Não grave todos os nomes diretamente na textura do mapa.

Utilize rótulos independentes, capazes de mudar de tamanho, posição ou visibilidade conforme o zoom.

Em visão geral, mostre os grandes pontos de referência.

Em visão intermediária, mostre os estabelecimentos.

Em visão detalhada, apresente rótulos locais e marcações investigativas.

Impeça a sobreposição exagerada de textos e ícones.

A posição de Marcus deve ser identificável mesmo em áreas cheias de construções.

As cores e símbolos precisam ter significados consistentes.

Não utilize vinte tipos de marcador sem necessidade.

O mapa deve permanecer bonito mesmo com todos os ícones opcionais ocultos.

## 12. Arquitetura técnica: evitar outro mapa artificial

Não quero que a implementação termine sendo uma imagem grande acompanhada de dezenas de coordenadas UV arbitrárias.

Crie uma estrutura de dados de geografia que possa ser mantida e ampliada nos próximos capítulos.

Essa estrutura deve representar as regiões, seus limites, transformações, edifícios, portais, marcos, caminhos e vínculos narrativos.

A implementação pode combinar:

- Camadas de Tilemap ou sprites cartográficos próprios.
- Elementos gráficos estáticos produzidos a partir das cenas.
- Um atlas georreferenciado.
- Rótulos e marcadores dinâmicos.
- Plantas interiores opcionais.
- Ferramentas de geração e validação no Editor.

Escolha a solução mais adequada ao projeto real, não necessariamente a mais complexa.

**A prioridade é uma representação visual excelente, fiel e sustentável.**

Se uma parte do atlas for gerada a partir dos objetos ou tiles do Unity, faça isso preferencialmente na autoria ou no Editor, com dados armazenados de maneira reproduzível.

Não reconstrua a cidade inteira nem gere milhares de texturas a cada abertura do mapa.

Evite alocações grandes de memória durante o uso, uso excessivo de RenderTextures, atualizações desnecessárias por frame e dependência de caminhos absolutos da máquina de desenvolvimento.

Preserve a nitidez da pixel art e escolha filtragem e mipmaps conforme os diferentes tipos de camada, sem produzir borrões ao aproximar.

As ferramentas de geração devem ser explícitas, seguras e não destrutivas.

**Não execute builders antigos capazes de recriar cenas aprovadas.**

O sistema deve funcionar numa cópia limpa do repositório e no executável Windows, não apenas no Unity Editor.

## 13. Não comprometer o conteúdo existente

Esta tarefa é a reconstrução do mapa utilizado pelo jogador.

Não modificar desnecessariamente:

- História e diálogos.
- Sinuca.
- Bocha.
- Progressão dos bosses.
- Animações e movimentação de Marcus.
- Geografia jogável já aprovada.
- Iluminação dos cenários.
- Trilhas e efeitos sonoros.
- Cenas dos capítulos 1 e 2.
- Salvamento manual por orelhões.

Se uma alteração compartilhada for indispensável para integrar o atlas, faça-a de maneira restrita e documentada.

**Não implementar salvamento automático.**

As descobertas podem atualizar o mapa em memória durante a sessão; sua persistência deve continuar obedecendo ao salvamento manual do jogo.

Os saves existentes devem continuar compatíveis.

## 14. Qualidade gráfica e acabamento

Não considere o trabalho finalizado apenas porque existe zoom funcional.

Quero que a cidade seja agradável de observar e permita reconhecer suas regiões.

O resultado deve transmitir:

- Identidade visual.
- Organização espacial.
- Coerência arquitetônica.
- Proporções plausíveis.
- Materiais e vegetação naturais.
- Boa leitura dos caminhos.
- Detalhamento progressivo.
- Identidade brasileira e gaúcha.
- Atmosfera de mistério.
- Continuidade visual com as cenas.

Se os edifícios parecerem blocos genéricos ou se os bairros forem preenchidos com casas repetidas sem critério, o resultado não atende ao pedido.

Não basta gerar novos sprites e espalhá-los por coordenadas aproximadas.

Cada elemento deve estar fundamentado na geografia real ou numa decisão artística explicitamente documentada.

## 15. Testes obrigatórios

Execute verificações reais no Unity e, quando o ambiente permitir, no Player Windows.

### Geografia

1. Comparar todas as regiões já jogáveis com o atlas.
2. Conferir posição e orientação dos edifícios principais.
3. Testar as transições entre regiões dos capítulos 1 e 2.
4. Validar que caminhos e pontes do mapa existem nas cenas.
5. Conferir a localização do Cemitério, Prefeitura e Sociedade Beira-Rio.
6. Verificar que frente e fundos do Mercadinho representam o mesmo estabelecimento.
7. Confirmar que locais futuros não receberam detalhes inventados.

### Zoom e navegação

8. Abrir o mapa na visão geral.
9. Aproximar progressivamente até o maior nível de detalhe.
10. Confirmar que novas informações visuais aparecem com a aproximação.
11. Verificar nitidez, textos e símbolos.
12. Arrastar até os limites, sem revelar áreas vazias.
13. Centralizar em Marcus após navegar para outra região.
14. Trocar entre abas sem perder o estado do mapa.
15. Testar controles de mouse e teclado.
16. Verificar o comportamento em diferentes resoluções e proporções suportadas.

### Investigação

17. Abrir o mapa antes de descobrir determinado local.
18. Descobrir uma informação e confirmar sua nova anotação.
19. Conferir que suspeitas não são apresentadas como fatos.
20. Verificar o funcionamento de lugares ainda não visitados.
21. Testar o mapa dentro de interiores.
22. Retornar a regiões conhecidas depois de avançar na história.
23. Confirmar que não existem spoilers de capítulos futuros.
24. Confirmar que objetivos não apontam para posições falsas.

### Salvamento e regressão

25. Carregar um save existente do capítulo 1.
26. Carregar um save existente do capítulo 2.
27. Atualizar o mapa durante a exploração sem produzir autosave.
28. Salvar manualmente num orelhão e restaurar as descobertas.
29. Alterar a exploração sem salvar e conferir o estado após recarregar.
30. Abrir e fechar o mapa sem afetar movimento, pausa, Sinuca ou Bocha.
31. Verificar erros de console e integridade dos assets.
32. Testar a build final e registrar desempenho e consumo de memória do mapa.

## 16. Evidências visuais obrigatórias

Produza capturas reais do Unity ou do executável, não mockups apresentados como implementação.

Quero pelo menos:

- Mapa completo da cidade.
- Zoom intermediário no Centro.
- Zoom detalhado na Igreja e entorno.
- Região do Cemitério.
- Sociedade Beira-Rio e margem do rio.
- Bairro e casa de Adilio.
- Bar do Zé.
- Mapa com Marcus posicionado.
- Mapa com anotações de investigação.
- Comparação entre representação cartográfica e cena jogável correspondente.
- Um interior explorado, caso seja implementada planta interna.

Inclua uma tabela ou relatório curto comprovando como cada região foi correspondida ao mundo real do jogo.

Onde houver discrepâncias, corrija antes da entrega.

A avaliação estética final será feita pelo autor; não classifique automaticamente as capturas como artisticamente aprovadas.

## 17. Critérios de conclusão

O trabalho somente estará completo quando:

1. O mapa antigo tiver sido substituído na experiência normal do jogador.
2. A apresentação possuir identidade visual própria, consistente com Santa Rita.
3. Os níveis de zoom realmente revelarem detalhes adicionais.
4. As regiões implementadas corresponderem à geografia real.
5. Marcus aparecer na posição correta.
6. Os lugares conhecidos e os objetivos forem representados coerentemente.
7. As anotações investigativas funcionarem e respeitarem a história.
8. Os interiores contemplados apresentarem plantas corretas.
9. O mapa permanecer fácil, confortável e agradável de utilizar.
10. Os saves e o restante do jogo continuarem funcionando.
11. A implementação funcionar fora do Editor, na build.
12. A entrega possuir capturas de gameplay, testes e documentação da arquitetura.

Não use quantidade de arquivos, número de linhas de diff ou quantidade de verificações automatizadas como substituto de qualidade.

## 18. Fluxo de execução

Primeiro, investigue o projeto e registre os problemas específicos do sistema de mapa atual.

Depois, defina a estrutura geográfica e produza uma primeira região-piloto, preferencialmente o Centro, já com apresentação final, zoom e marcadores.

Valide a correspondência dessa região com a cena real.

A partir dessa solução, expanda para todas as regiões jogáveis dos capítulos 1 e 2, mantendo o mesmo padrão de acabamento.

Em seguida, integre anotações, interiores, objetivos e salvamento.

Por fim, faça testes de ponta a ponta, corrija os problemas identificados e apresente as evidências.

Este pedido autoriza a implementação completa, não apenas a elaboração de um planejamento.

Respeite as alterações locais existentes, trabalhe de modo seguro sobre a branch apropriada e não faça push, merge ou publicação sem autorização.

Se alguma ferramenta não estiver disponível, registre objetivamente a limitação e prossiga com as partes que puder concluir, sem alegar testes não realizados.

---

## RESULTADO QUE EU ESPERO

Quero abrir o mapa e sentir que estou olhando para a mesma cidade que explorei com Marcus.

Ao afastar a visão, quero reconhecer a cidade inteira, com seus bairros, rio, estrada e principais construções.

Ao aproximar o Centro, quero enxergar a Igreja, a praça, a Prefeitura, a Rodoviária, as ruas e os acessos exatamente onde estão nas cenas.

Ao aproximar mais, quero reconhecer os prédios, seus caminhos, portões, terrenos e detalhes.

Quando Marcus descobrir algo importante, quero poder voltar ao mapa e encontrar aquela informação representada de maneira natural, como parte da investigação.

**Quero o charme visual de Stardew Valley, a atmosfera investigativa de Silent Hill 2 e a precisão de exploração de Project Zomboid, tudo adaptado à identidade original do nosso jogo.**

O mapa deve deixar de ser uma ilustração aproximada da cidade e se tornar uma das principais ferramentas de exploração e investigação do jogador.

**A prioridade absoluta é qualidade, fidelidade ao mundo, legibilidade, beleza visual e integração real com o projeto existente.**