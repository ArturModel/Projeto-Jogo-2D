# CONTINUAÇÃO DO PROJETO — CAPÍTULO 2 COMPLETO ATÉ A BOCHA
## Prompt de execução para Codex | Revisão C2-R1

Você continuará o MESMO projeto Unity que acabamos de desenvolver e aprimorar. O capítulo 1 foi testado pelo autor e aprovado. A solicitação agora é implementar o capítulo 2 inteiro, da consequência da vitória contra Zé até a vitória/derrota contra Tonão na Cancha da Bocha, com encerramento coerente e preparação limitada da próxima etapa.

Esta é uma AUTORIZAÇÃO DE IMPLEMENTAÇÃO, não somente uma solicitação de análise. Faça uma inspeção inicial e um plano executável, depois desenvolva, integre, execute, revise e corrija. Não encerre a tarefa depois de apresentar um plano, criar pastas, escrever diálogos ou gerar scripts isolados.

Você já possui contexto e documentos, mas sua memória da conversa não substitui os arquivos atuais. Verifique a versão realmente implementada antes de continuar.

---

## 1. CONTRATO: CONTINUAR SEM RECOMEÇAR, COMPLETAR SEM INFLAR

O capítulo 2 precisa ser substancialmente mais rico que o primeiro: mais área útil, interiores funcionais, ações de investigação, personagens com respostas contextualizadas, pistas, acontecimentos e um segundo minigame completo. A qualidade aprovada do capítulo 1 é o piso, não um teto nem algo descartável.

Não interpretar “mais conteúdo” como:
- Mais linhas de código, YAML, diálogos repetidos ou arquivos gerados.
- Muitas fachadas inacessíveis apresentadas como interiores concluídos.
- Caminhadas longas sem acontecimentos.
- O mesmo cômodo recolorido várias vezes.
- Uma lista de missões desconectadas.
- Um minigame funcional, mas sem identidade, apresentado como boss finalizado.

O autor informou aproximadamente 60 mil linhas de diff e três ou quatro cenas novas no capítulo 1. Isso é contexto, NÃO uma métrica de produtividade. Não tentar ultrapassar 100 mil linhas, duplicar sistemas ou reserializar cenas para produzir volume. Um diff grande deve resultar de conteúdo necessário, nunca ser objetivo.

“Completo” significa: escopo contratado construído, integrado, jogável, revisado e validado por critérios observáveis. Não significa prometer ausência absoluta de bugs ou dizer “100%” sem evidências.

Não reduzir o escopo silenciosamente para terminar mais rápido. Se houver impedimento real, descreva o que ele bloqueia, avance nas partes independentes e registre o trabalho restante. Não maquie pendências como polimento opcional.

---

## 2. AUTORIDADE, CONTEXTO E FRONTEIRA NOVA

Leia:
1. Instruções aplicáveis do repositório, incluindo AGENTS.md e regras dos diretórios.
2. Decisões explícitas posteriores do autor, documentação atual e aprovações registradas.
3. Santa_Rita_Biblia_Consolidada_R1.md ou sua representação equivalente, se ainda for a consolidação vigente. Não ler três formatos idênticos como fontes distintas.
4. Relatórios, plano, testes, cenas, assets e saves de teste do capítulo 1.
5. Mapa aprovado e correções posteriores de posições, fachadas e conexões.
6. Código relevante ao caminho que será estendido e seus sistemas compartilhados.

V2 e V5 são históricos complementares, não licença para restaurar contradições.

Este prompt SUBSTITUI somente o limite antigo “parar após a Sinuca / não construir Centro e Bocha”. Agora Centro, Cemitério, Prefeitura e Bocha entram no escopo descrito abaixo. Permanecem válidos o cânone, o save manual, as posições do mapa e as demais restrições compatíveis.

Uma proposta antiga não se torna decisão do autor apenas porque está num documento. Diferencie:
- Confirmado pelo autor.
- Implementado e aprovado no capítulo 1.
- Base narrativa compatível.
- Nova proposta de design deste prompt.
- Pendência real.

A implementação existente mostra o estado do jogo, não prova sozinha uma mudança de cânone. Se houver conflito sério entre aprovação registrada, implementação e história, apresente evidência e pergunte somente o necessário. Não reabra decisões já resolvidas.

O nome comercial do jogo e o nome da cidade estavam sendo discutidos. Não renomeie o projeto para Vila Prestes, Passo dos Prestes, Alvorada ou qualquer candidato sem confirmação no material atual. Preserve os nomes vigentes e IDs estáveis; alterações de título não pertencem a esta tarefa.

---

## 3. RECONHECIMENTO DO CAPÍTULO 1 REAL

Antes de modificar sistemas compartilhados:
- Registre estado do Git e alterações existentes. Não descarte trabalho local.
- Confira ProjectVersion.txt, pacotes, pipeline, assemblies e ferramentas disponíveis.
- Descubra a cena de inicialização e o fluxo real de execução.
- Leia e, com ferramentas disponíveis, execute o trecho final do capítulo 1.
- Identifique os diálogos REAIS da vitória, ficha entregue, pistas, objetivo ativo, gate do Centro e mensagem/tela que encerrava o recorte.
- Confira como funcionam os saves já produzidos: IDs, versão, serialização, defaults e local de gravação.
- Inspecione o minigame de Sinuca aprovado: controles, câmera, feedback, ritmo, UI, derrota, placar e integração.
- Identifique direções visuais/sonoras já aprovadas no projeto. Não substituir por uma proposta antiga.

Produza um contrato curto de entrada do capítulo 2:
estado necessário -> ponto de entrada -> conhecimento atual de Marcus -> inventário relevante -> informação obtida de Zé -> pergunta ainda aberta -> primeira ação da continuação.

Não suponha que a mecânica “bola marcada”, a fala “eu vi levarem ele” ou cada proposta do prompt anterior foram implementadas. Leia o que existe e use a continuação efetiva.

Remova/substitua SOMENTE a barreira de demonstração que encerrava o capítulo 1 após a vitória. Não remova a condição que impede acesso ao Centro antes de vencer Zé.

Obtenha uma referência de qualidade: capturas do jogo, cenas relevantes, configuração de câmera, tempos/consumo medidos quando possível, funcionamento de menus e sequência de save/load. Não invente um benchmark.

Se o Editor não puder ser executado, registre a limitação. Não afirme que viu ou jogou uma cena apenas por analisar seu YAML.

---

## 4. CÂNONE QUE NÃO PODE SER ALTERADO

- A cidade fica no Rio Grande do Sul.
- Em 1990, Marcus tem 16 anos e ocorre a tragédia de Júlia no Clube Recreativo.
- Em 1998, Marcus tem 24 anos: Tonão fere Lucas no clube; Marcus o leva de carro; ocorre o acidente às 03:17 e o abandono às 03:19. Lucas SOBREVIVE.
- Em 2004, Marcus tem 30 anos e retorna de carro próprio para procurar Adilio.
- Adilio deixou de responder no mesmo dia. A casa apresenta rotina recente e sinais discretos de resistência, sem uma chacina ou destruição generalizada.
- Marcus acredita que Lucas morreu. A CONFIRMAÇÃO de Lucas vivo e de sua identidade atual fica para o encontro final no Clube.
- Marcus não tem amnésia literal. Lembra de Júlia, de Lucas e dos fatos gerais, mas evita conexões que o responsabilizam.
- A cidade é real e habitada. Distorções fazem parte da percepção/memória e do sobrenatural ambíguo.
- Lucas não controla magicamente ruas, edifícios, clima ou aparições.
- Zé é dono do Bar do Zé e primeiro boss. Tonão é o segundo boss, na Sociedade Beira-Rio/Cancha da Bocha. Amaro é o terceiro, no Galpão do Truco.
- O Clube é o local do quarto e último confronto. Não criar um “castelo” ou outro prédio final no lugar dele.
- O final envolve pedir socorro ou não. Sua encenação ainda não entra nesta entrega.
- Não fixar por conveniência quem sequestrou Adilio, a operação do desaparecimento, uma identidade falsa de Lucas ou o destino final do avô, caso essas questões continuem pendentes na documentação mais recente.

Tonão deve preservar seu background:
homem dos reparos, reputação de “dar um jeito”, confiança de Júlia, conhecimento dos improvisos, insistência em resgate próprio e dificuldade de admitir limites. Seu eixo não é simplesmente ser forte ou mau: ele insiste porque reconhecer a falha ameaça quem acredita ser.

A revelação central desta etapa é:
Júlia permaneceu consciente, houve oportunidades de pedir ajuda e Tonão insistiu em soluções insuficientes enquanto o tempo passava.

Se parte disso já estiver explicitada no capítulo 1 aprovado, não finja que Marcus esqueceu. Aprofunde responsabilidades, evidências e consequência emocional sem contradizer o estado real.

Não confirmar agora:
a sobrevivência de Lucas, sua identidade atual, toda a operação contra Adilio ou a verdade completa da ligação de Marcus em 1990. Esta última pertence ao desenvolvimento associado a Amaro, salvo alteração explícita posterior.

Não usar a tabela antiga da V5 para transformar Tonão em assassino de Lucas ou adiantar a revelação do sobrevivente.

---

## 5. ESCOPO CONCRETO DO CAPÍTULO 2

ENTRADA:
jogador conclui a Sinuca na sessão atual, ou carrega um save manual legítimo posterior à vitória, e atravessa a conexão liberada.

CORPO DA ENTREGA:
A. Continuação da investigação iniciada no capítulo 1.
B. Centro completo como região de exploração, com praça, circulação, fachadas e interiores contratados.
C. Igreja e seus espaços necessários; Escola e Rodoviária conforme o mapa.
D. Prefeitura em seu setor correto, entre Centro e Clube, com os ambientes necessários à investigação.
E. Fundos do mesmo Mercadinho já existente, sem duplicar frente, loja ou NPC.
F. Cemitério completo no setor norte, com exploração e função narrativa própria.
G. Acessos à Sociedade Beira-Rio, exterior, interior, cancha e espaços de apoio necessários.
H. Tonão, tutorial, Bocha jogável, camada de horror, vitória, derrota e recompensa.
I. Reações pós-boss, atualização da investigação e gancho para a etapa seguinte.
J. Regressão do capítulo 1 e compatibilidade dos saves.

FORA DA ENTREGA:
boss do Truco, capítulo 3, confronto final, interiores finais do Clube, revelação de Lucas vivo, resgate final de Adilio e finais. Bairro Operário, Galpão, Capela e demais regiões futuras não recebem capítulos completos só para preencher a vista.

Pode preparar um destino, flag ou referência do próximo capítulo. Não preencher o próximo mapa por iniciativa própria. Fachadas/limites visíveis exigidos pela continuidade visual não autorizam seus interiores.

Não entregar só “Centro e uma mesa de Bocha”. O Cemitério, a Igreja, os demais interiores centrais e as conexões fazem parte do trabalho, não de uma lista para depois.

---

## 6. FIDELIDADE AO MAPA: CONTRATO ESPACIAL

Use a referência visual APROVADA, não a última imagem por ordem de arquivo. Houve mapas mais extensos descartados e artes que repetiram posições erradas.

Posições obrigatórias:
- Entrada à esquerda; Posto/Avenida e Encruzilhada já se conectam ao capítulo 1.
- Bairro de Adilio na região ocidental, com seu acesso existente.
- Bar do Zé ao norte/noroeste.
- Centro na região central.
- Igreja no setor superior do núcleo central, relacionada à Praça da Matriz.
- Cemitério ao norte do Centro, a leste da área do Bar, conforme a referência aprovada; não transferi-lo para o rio nem juntá-lo à Capela.
- Cancha da Bocha a sudoeste do Centro, JUNTO AO RIO, na margem urbana mostrada no mapa. Não movê-la para o Centro, para leste ou para a outra margem por conveniência.
- Prefeitura ENTRE Centro e Clube. Escola ocupa o local trocado com a Prefeitura na arte antiga, integrando o planejamento central.
- Clube centro-direita, acessado através do Centro e de sua aproximação pela Prefeitura.
- Rodoviária na porção inferior/leste do núcleo central, conforme a referência vigente.
- Galpão do Truco sudeste; Capela meio inferior. Ambos são referências futuras, não destinos deste capítulo.
- Estrada da Usina removida. Não ressuscitar trilhas/setores anteriormente retirados.

Mercadinho:
é UM estabelecimento. Fachada e atendimento são da Encruzilhada; o Centro mostra/acessa a parte de trás. Preserve identidade, orientação, estoque e ligações. Um acesso traseiro não cria outra loja nem permite contornar o bloqueio pré-Sinuca.

Ruas estão nas cenas. Não criar “Centro -> cena de rua vazia -> Cemitério” quando a ligação pode ocorrer diretamente nas bordas dos setores. Espaço de transição com função faz parte do próprio setor, não uma cena de preenchimento.

Antes da decoração, monte um registro espacial com:
região -> posição relativa -> vizinhos -> edifícios -> acesso público -> porta de serviço -> destinos/spawns -> bloqueios -> referência visual.

Registre coordenadas de trabalho em tiles/unidades e limites das cenas a partir do projeto, sem inventar medidas oficiais para a ilustração. Preserve posições, ordem, orientação e conexões. A imagem conceitual não é uma especificação métrica de cada tile.

“Exatamente conforme o mapa” não significa colar a imagem no chão. Construa áreas completas no estilo de gameplay, com Tilemaps/sprites e colisão. Ajustes locais de largura para circulação são aceitáveis; mover um marco ou alterar uma conexão exige decisão.

Compare capturas do layout com a referência após o blockout e após a montagem final. Produza uma tabela “marco esperado -> posição entregue -> evidência”. Não declare fidelidade por intuição.

Se só houver mapas conflitantes ou faltar a referência essencial, peça o arquivo certo. Continue trabalho independente, mas não improvise a geografia.

---

## 7. INTERIORES COMPLETOS E COERENTES COM AS FACHADAS

Não contar cada quarto como um “novo lugar” para inflar escopo. O importante é função, identidade e execução.

Entregue os edifícios previstos como unidades coerentes:

IGREJA:
salão/nave legível, áreas de circulação, elementos religiosos cotidianos e espaço de conversa/exame. Sacristia ou apoio apenas com função. O padre não vira sacerdote demoníaco. O horror vem da presença que não se tornou ação.

ESCOLA:
entrada e ambientes acessíveis compatíveis com o prédio, com memória de uma comunidade real. Não abrir um labirinto enorme nem colocar crianças e funcionários ativos de madrugada sem explicação. Relacione conteúdo a registros públicos, lembranças ou vida cotidiana, sem inventar dados pessoais centrais.

RODOVIÁRIA:
espera, bilheteria/guichê e apoio proporcionais ao edifício. Sua função pode explorar partidas, permanência e informações de circulação; um horário/registro só prova aquilo que efetivamente registra. Não fabricar bilhete de Adilio como certeza de fuga voluntária.

PREFEITURA:
recepção e espaço de consulta/arquivo pertinente. Acesso a documentos deve ser plausível, sobretudo à noite. Não presumir que Marcus pode consultar qualquer arquivo reservado. Cargo político e testemunho precisam respeitar as datas.

MERCADINHO — FUNDOS:
pátio, serviço ou depósito compatíveis com o capítulo 1. Não criar atalho que cause loop de transição ou duplique interiores. O local precisa acrescentar informação ou uma conexão útil.

OUTRAS FACHADAS HABITÁVEIS:
se o mapa aprovado tiver residências/lojas sem função, selecione pelo menos duas existentes para usos distintos de exploração, desde que não precise alterar o mapa. Caso não haja fachadas compatíveis, registre a lacuna; não acrescente um bairro para cumprir quota.
Não fazer ambas servirem para buscar uma chave genérica.

CEMITÉRIO:
área externa completa, muro/portão, caminhos, túmulos e pontos de observação compatíveis. Pequena edificação existente no mapa pode ser acessível com função; não confundir com a Capela do setor inferior.
Júlia tem relação com memória e luto; não inventar data de nascimento. Não criar túmulo verdadeiro de Lucas para sustentar uma morte falsa.

SOCIEDADE BEIRA-RIO:
chegada, exterior junto à água, salão de convívio e cancha. Apoio/depósito/objetos de manutenção só quando dão contexto ou interação.
Não colocar a cisterna histórica debaixo da cancha: ela fica sob o Clube Recreativo. A cancha pode evocá-la perceptivamente, sem transferir fisicamente a tragédia.

Para CADA interior:
- Entrada e saída correspondentes; retorno pelo lado correto da porta.
- Mobiliário na escala do jogador, circulação testada, oclusão/sorting corretos.
- Iluminação e som próprios, chuva exterior coerente.
- Ao menos uma função diferenciada: investigação, conversa, pequena resolução ambiental, reinterpretação ou construção de personagem.
- Conteúdo final do recorte, não “texto provisório” em todas as interações.
- Estados posteriores e comportamento ao revisitar/carregar.
- Explicação contextual de acesso naquele horário. Nem tudo precisa estar ocupado, nem tudo precisa estar abandonado.

Não usar a diversidade de interiores para abandonar a unidade visual do jogo.

---

## 8. CADEIA NARRATIVA: OBJETIVO CLARO, EXPLORAÇÃO LIVRE

Marcus continua procurando Adilio. A investigação de 1990 só cresce porque ajuda a entender as pessoas, relações e contradições encontradas nessa busca. Ele não abandona uma suspeita urgente para colecionar fotografias sem motivo.

A progressão principal deve ser linear em causalidade, mas permitir exploração local e pistas em ordem diferente. Cada objetivo precisa responder:
“O que Marcus descobriu, por que isso importa AGORA, aonde pode ir e o que espera conseguir lá?”

Para cada sequência, registre:
ID estável; pré-condições; pergunta; ação jogável; pessoa/objeto; informação obtida; fonte real; próxima hipótese; objetivo atualizado; limites de spoiler; persistência; respostas de revisita.

Estrutura funcional nova, NÃO relato de cenas já canônicas:
1. Usar o gancho REAL de Zé e a ficha/item efetivo para justificar entrada no Centro.
2. Obter orientação concreta sobre os antigos jogadores, a Sociedade Beira-Rio e quem pode esclarecer a informação recebida.
3. Encontrar no Centro uma versão pública/coletiva da história; preservar a homenagem e o cotidiano.
4. Contrastar essa versão em interiores relevantes, com testemunhos ou materiais que tragam diferenças de contexto, não simples repetição.
5. Usar o Cemitério para aprofundar a memória de Júlia e obter uma contradição/reinterpretação pertinente. Não fazer o local existir só para um susto.
6. Reconectar o que foi descoberto à pergunta atual sobre Adilio e à razão para procurar Tonão.
7. Chegar à Cancha, estabelecer relação, explorar, confrontar a evasiva e entrar na partida.
8. Após o boss, obter avanço concreto sobre a busca atual E sobre o resgate de Júlia, sem encerrar o mistério.
9. Devolver controle, permitir save manual e apresentar objetivo/gancho da etapa seguinte sem implementá-la.

Essas funções podem mudar de ordem local para respeitar o que já foi entregue. Não transformar o exemplo numa rota arbitrária de obrigar todas as portas uma vez.

É permitido visitar Tonão antes de todas as pistas: faça a conversa responder ao conhecimento atual. Não depender de uma barreira invisível sem explicação. O confronto completo exige condições de investigação legíveis, não um contador secreto de objetos.

Não obrigue uma informação opcional obscura para vencer ou avançar. Uma evidência essencial precisa ter origem acessível, recuperação por revisita ou outro caminho causal, sem duplicar narradores oniscientes.

A mesma resposta “vá falar com fulano” não pode sustentar o capítulo inteiro. Alterne examinar, comparar, observar, dialogar e resolver situações físicas simples.

---

## 9. LIBERDADE CRIATIVA E LIMITES DOS FATOS NOVOS

Este prompt autoriza criar conteúdo local compatível: falas, descrições, microeventos, puzzles de ambiente, nomes de apoio não definidos e detalhes cotidianos necessários ao capítulo.

Essa liberdade NÃO autoriza fechar silenciosamente questões centrais ainda pendentes:
responsáveis pelo desaparecimento, cativeiro, identidade de Lucas, testemunhas-chave que escondem sua sobrevivência, profissão de Helena, causa do perigo final ou origem sobrenatural.

Antes de escrever uma pista nova, diferencie:
A. Fato confirmado/herdado.
B. Fala de personagem, possivelmente parcial.
C. Inferência de Marcus.
D. Encenação/proposta nova.
E. Distorção perceptiva.

Uma foto de Tonão com ferramentas pode apoiar sua ligação à manutenção; não prova que sequestrou Adilio. Um relato sobre uma voz em 1990 não identifica um mandante em 2004.

Se a pista indispensável depender de PENDÊNCIA CENTRAL, formule uma decisão curta com alternativas e impacto, e pergunte. Não pare toda a tarefa por pendências de cenas futuras. Não deixe uma cadeia falsa apenas para evitar perguntar.

A origem do conhecimento de Zé/Tonão precisa existir. Não afirme que Tonão sabe onde está Adilio sem explicar como. Não transforme toda a cidade num grupo de sequestradores.

Investigar não elimina telefonar: preserve ou desenvolva a resposta já aprovada do capítulo 1 às tentativas de pedir ajuda. Não inventar espera obrigatória para desaparecidos, ausência universal de celulares ou telecomunicações funcionando somente para salvar.

---

## 10. DENSIDADE DE CONTEÚDO, NÃO ENCHIMENTO

Meça o que o capítulo 1 efetivamente entregou e planeje a ampliação com um inventário comparável de áreas, edifícios acessíveis, situações, pistas e estados de NPCs. Registre diferenças reais, não apenas número de arquivos.

Como envelope inicial DE PRODUÇÃO, não como fatos da lore nem quota cega, planeje:
- Cerca de 6–8 objetivos principais causalmente conectados.
- Cerca de 10–14 sequências jogáveis, incluindo confronto e consequência.
- Pelo menos duas pequenas linhas opcionais com conclusão ou recompensa interpretativa.
- Um conjunto de pistas de naturezas distintas, obrigatórias e opcionais, sem fragmentar um documento em dez itens para inflar a contagem.
- Retornos com mudança de significado em pelo menos dois lugares conhecidos.
- Diversos interiores com identidade e uso, não um grande salão com todas as funções.
- Mais de uma ocorrência de horror com preparação e consequência, além da Bocha.

Ajuste essas quantidades ao mapa e ao ritmo observado, registrando por quê. Não use números para fabricar falas inúteis. Não reduzir a entrega a uma “versão mínima” sem avisar.

Cada linha opcional deve aprofundar pessoa/lugar/tema, não virar desvio longo da busca urgente. Não adicionar produção de itens, economia, combate, missões diárias ou múltiplos finais para criar volume.

Júlia também precisa aparecer como pessoa: curiosidade, interesse em aparelhos, humor ou planos. Não entregar apenas relatórios sobre sua morte.

Não prometer horas de gameplay por estimativa de distância. Registre tempos de exploração e releitura em testes, e diferencie ritmo rápido, investigativo e repetição por derrota.

---

## 11. DIÁLOGOS, PISTAS E DIÁRIO

Todo personagem principal deve ter voz, interesse imediato, limite do que sabe e motivo plausível para dizer ou omitir.

TONÃO:
frases concretas, defesa do que fez e insistência em ter tentado. Não monólogo de vilão confessando tudo ao perder.

PADRE:
compaixão e desconforto, sem autoridade sobrenatural que explique o jogo.

PREFEITURA:
versão institucional, cuidado com o relato público e documentos com contexto. Não declarar todas as autoridades corruptas.

MORADORES:
variar desconhecimento, hábito, lembrança fragmentada e ajuda genuína. Nem todos sabem o nome de Marcus sem razão.

Inclua respostas para:
primeira visita; visita precoce; pista específica apresentada; contradição encontrada; conversa interrompida; revisita; pós-Bocha.

Não é necessário multiplicar árvores gigantescas. Escolhas simples podem alterar tom e acesso contextual sem criar cânones incompatíveis.

Texto em português natural, com acentuação, sem caricatura de sotaque gaúcho. Identidade regional vem do ambiente e relações, não de bordões em toda frase.

Retratos seguem o sistema aprovado do capítulo 1. Nome, expressão e rosto não podem entregar Lucas atual nem acusar antecipadamente alguém sem base.

Diário deve permitir:
ver objetivo atual, reler pista integral, localizar origem e distinguir evidência de interpretação. Não registrar uma conclusão antes da cena que a sustenta.

Para cada revelação estrutural, planeje ao menos dois apoios de natureza diferente. Não apresentar duas cópias do mesmo boato como confirmação independente.

Datas: 1990/Júlia; 1998/Lucas; 2004/presente. Dia/mês não aprovados não viram canônicos automaticamente.

Fotografias/fitas precisam de origem e acesso coerentes. Não duplicar o gravador de Júlia nem distribuir gravações impossíveis por todos os cômodos. Se um fragmento for ouvido como memória, catalogue-o assim e não dê ao jogador uma prova física inexistente.

---

## 12. HORROR DO CAPÍTULO: AMEAÇA SEM PERDER LEGIBILIDADE

Não espalhar efeitos aleatórios por toda interação. Construa uma progressão:
normalidade reconhecível -> detalhe incompatível -> confirmação sensorial inquietante -> perda de confiança na leitura do lugar -> confronto.

CENTRO:
cotidiano e investigação; pessoas ainda vivem ali. Sua aparência é a referência estável contra a qual alterações posteriores serão sentidas.

IGREJA:
o peso de consolar sem agir. Um som ou frase reinterpretada deve se conectar à testemunha, não a um demônio genérico.

CEMITÉRIO:
opressão, memória e silêncio. Preserve sua identidade sonora própria e os espaços legíveis. Não usar escuridão absoluta, música estourada ou túmulo falso de Lucas como atalho.

BOCHA:
ameaça ligada à repetição de tentativas, força, controle e consequências. A sala vai deixando de parecer apenas um lugar de lazer.

Toda ocorrência deve registrar gatilho, duração, impacto de gameplay, o que altera de fato, o que Marcus percebe, persistência e alternativa acessível.

Propostas de encenação possíveis:
- Um som familiar chega de um local onde não deveria estar.
- Uma placa/objeto ganha uma leitura diferente depois de outra pista.
- Um espaço reconhecido apresenta por instantes características de 1990.
- A resposta ao impacto da bocha parece vir debaixo do salão.

Escolha poucas ocorrências fortes. Não use todas mecanicamente. Não mudar a gravação física de uma fita porque faltou uma resposta no roteiro.

Sem revelações sonoras que obriguem volume alto. Sem flashes obrigatórios, jumpscares repetidos, perseguidor novo ou dano escondido pelo cenário.

Não prometer que um efeito “causa medo real” sem playtest. Verifique se a tensão funciona; escreva honestamente o que foi observado.

---

## 13. BOCHA: AUTENTICIDADE, TÁTICA E IDENTIDADE

A Bocha deve continuar reconhecível:
balim, aproximação, colocação, força, colisão, retirada de bolas e disputa pela proximidade. Não é sinuca sem caçapas, boliche, QTE de uma barra nem jogo de apertar um botão no tempo certo.

Reutilize sistemas adequados de UI, input, sessão e áudio da Sinuca. Não copie sua condição de vitória, sua “bola proibida” ou propriedades físicas sem adaptar à cancha.

Antes de efeitos complexos, construa uma fatia de qualidade:
arremesso controlável -> rolagem -> colisão -> repouso -> medida -> turno -> resultado -> próximo estado.
Ela precisa ser interessante sem horror e continuar legível com horror.

Leia regras/documentação primária confiável da modalidade pertinente se necessário. Não copie de memória uma suposta regra oficial. Qualquer simplificação abaixo é VARIANTE AUTORAL DE GAMEPLAY, não regulamento de campeonato.

O formato melhor de três está confirmado para Sinuca; não é automaticamente uma regra da Bocha. Primeiro procure uma decisão posterior do autor. Na ausência dela, use a proposta de protótipo abaixo, identificada como decisão local reversível deste capítulo.

---

## 14. BASE PROPOSTA PARA A BOCHA — ESPECIFICAÇÃO FECHÁVEL

PROPOSTA NOVA, subordinada a decisões de Bocha já aprovadas no repositório:
duelo individual Marcus x Tonão; quatro bochas por lado e um balim; partida até seis pontos. A meta inicial é uma disputa compacta que permita viradas, não uma obrigação de duração. Balanceie em playtest sem esconder mudanças.

Defina termos na UI/documentação:
- Lançamento: uso de uma bocha.
- Mão: ciclo com balim colocado e até oito lançamentos.
- Partida: sequência de mãos até alguém atingir a pontuação-alvo.

Antes de integrar a narrativa, resolva TODAS as regras:

A. Balim começa em posição válida de um conjunto de disposições testadas, mostrado antes da primeira jogada. Não reposicionar secretamente para favorecer a IA. Permitir deslocá-lo por colisões.
B. Alternar quem inicia entre mãos válidas; primeira mão pode ser de Marcus para ensinar, com vantagem avaliada.
C. Depois do primeiro lançamento, joga quem está mais distante do balim, enquanto tiver bochas, até superar o adversário. Se um lado esgotou suas bochas, o outro lança as restantes.
D. Igualdade aproximada de melhor distância usa uma regra explícita: na proposta, joga quem não fez o último lançamento, havendo estoque. Não sortear silenciosamente.
E. Cada lado tem quatro lançamentos por mão; uma bocha inválida consome o lançamento. Não recarregar o estoque porque o arremesso ficou ruim.
F. Apurar pontos somente ao encerrar a mão, após todas as bolas pararem. Conta-se uma unidade para cada bocha válida do lado vencedor estritamente mais próxima do que a melhor adversária.
G. Medida pela geometria física entre superfícies, não pelo tamanho desenhado do sprite. Use tolerância explícita de empate. Se as melhores empatam dentro da tolerância, a mão não dá pontos.
H. Sem bochas adversárias válidas, contar as próprias válidas; nenhuma bocha válida dos dois lados significa zero. A soma obtida numa mão nunca excede quatro nessa variante.
I. Resolver saída da cancha, lançamento inválido, contato de borda e balim fora com política documentada. Proposta para balim fora: anular a mão e repeti-la com o mesmo iniciador/disposição, sem pontos ou punição inventada. Testar para evitar que esse caso se torne estratégia abusiva.
J. Declarar vitória apenas depois de apurar a mão e alcançar seis ou mais pontos. Não encerrar por uma vantagem provisória com bochas ainda por lançar.
K. Nenhum empate/anulação reinicia o jogo inteiro nem cria save. Nenhum limite de tempo ou de tentativas oculto.
L. Oferecer resumo visual da contagem antes da próxima mão e permitir rever as regras.

Teste a duração. Se a combinação produzir partidas excessivamente longas, ajuste pontuação, apresentação ou número de bolas de forma consistente e documentada, não com acertos forçados da IA.

Use rótulos claros para distinguir nova proposta de regra confirmada do autor. Uma regra de outro esporte não é importada só por reutilizar o controlador do primeiro boss.

---

## 15. CONTROLES E FÍSICA DA BOCHA

Priorize:
- Direção por mouse, força legível e cancelamento antes da confirmação.
- Faixa/posição de lançamento clara.
- Antevisão suficiente para aprender sem prometer uma trajetória falsa.
- Feedback de rolagem, contato, distância e posição.
- Poucos comandos, com descoberta gradual de estratégia.

As decisões principais devem incluir aproximar, proteger posição, retirar uma bocha adversária e mover o balim. Se houver modos de arremesso distintos, cada um precisa de efeito, comando, custo/limite e explicação reais.

Não chamar um impulso plano de arremesso aéreo. A primeira versão pode oferecer rolagem suave e lançamento mais forte de remoção, sem implementar trajetória vertical complexa se isso não agregar valor.

A física da cancha não deve parecer uma mesa de sinuca:
calibre atrito, desaceleração, restituição, massa relativa, escala e resposta do balim. Não simule realismo adicionando jitter.

Input e lançamento bloqueados enquanto houver movimento significativo ou animação de resolução. Evento de UI não pode consumir bocha. Câmera não se move no meio da mira.

Use o sistema físico escolhido no projeto e configurações isoladas do novo boss. Não mude Physics2D global ou os materiais da Sinuca sem análise e regressão.

Trate:
tunneling, bolas sobrepostas, repouso falso, colisão simultânea, objetos fora dos limites, reposicionamento de anulação e resultados próximos da tolerância.

Sistemas separados:
simulação; regras da mão; pontuação da partida; IA; apresentação; efeitos perceptivos; recompensa/progressão.

Uma simulação auxiliar para a IA não pode mexer na cena real nem tocar efeitos. Não criar uma segunda física divergente que “prevê” resultados impossíveis.

---

## 16. MECÂNICA PSICOLÓGICA PROPOSTA — A CANCHA GUARDA MARCAS

PROPOSTA NOVA DE DESIGN. Se já existir uma mecânica de Bocha aprovada, preserve sua intenção e compare; não a substitua silenciosamente.

A assinatura proposta são RASTROS DA ÁGUA:
entre mãos, faixas do piso passam a apresentar marcas de umidade que alteram de maneira estável a rolagem. Tonão insiste que a cancha está igual. Marcus precisa ler o que vê, recalibrar força e posição, em vez de repetir a mesma tentativa mais forte.

Regras de justiça:
- Primeiro demonstrar o piso normal.
- Introduzir a mudança ANTES do lançamento, nunca depois que o jogador decidiu.
- A faixa tem textura/contorno identificáveis além de cor.
- Mostrar qualitativamente o efeito: a bola perde velocidade ali. Não exigir adivinhar coeficientes invisíveis.
- Manter o estado físico da faixa fixo durante toda a mão.
- Aplicar a mesma regra à IA, às bochas e ao balim, com parâmetros explicados.
- Garantir disposições com arremessos legais e possibilidades táticas.
- Não fazer uma marca gerar buracos, teleportes, mortes ou perda automática.
- Reduzir efeitos visuais não pode retirar informação necessária para jogar.
- Não introduzir uma “mancha certa” obrigatória escondida nem uma barra de sanidade que sabota a força.
- A camada perceptiva pode evocar a água de 1990; isso não muda a localização real da cisterna para a Sociedade.

O terror deve estar na relação entre a regra e Tonão:
“Está igual.”
“Foi pouca força.”
“Agora vai.”
“Só mais uma.”

Essas falas são propostas de voz, não a obrigação de repetir quatro frases a cada jogada. Use com parcimônia e reação contextual.

A habilidade do jogador continua sendo ler a cancha e escolher uma boa jogada. Não condenar moralmente uma jogada forte legítima nem obrigar uma ação para depois acusá-lo por obedecer.

A primeira mão introduz desconforto; a segunda já contém a identidade psicológica completa. Nesta proposta, ninguém pode chegar a seis pontos em uma única mão de quatro bolas, mas teste a regra no código.
Se o formato mudar, mantenha as revelações essenciais independentes de prolongar a partida.

Efeitos possíveis de escalada:
conversa social cessa; uma rolagem parece continuar depois do repouso; um impacto recebe resposta abafada embaixo do piso; reflexos e marcas evocam outro salão. Sons ilusórios não substituem os sinais reais de regra.

Não criar uma sequência de sustos entre todos os lançamentos. Silêncio, antecipação e causalidade precisam de espaço.

---

## 17. IA, DIFICULDADE E EXPERIÊNCIA DE CONFRONTO

Tonão deve parecer competente e obstinado, não perfeito nem aleatório:
- Considera aproximação, remoção, proteção e deslocamento do balim.
- Usa estoque, posições e piso público da mesma forma que Marcus.
- Possui erro calibrável, não teletransporte ou resultado escolhido após a colisão.
- Pode ter preferência tática por controle/força, sem ser burro de propósito.
- Não lê inputs futuros ou muda parâmetros em segredo para impedir vitória.
- Tem tempo de decisão curto e apresentação compreensível.
- Não precisa testar milhares de lançamentos por frame para parecer inteligente.

Pistas pré-boss podem ensinar hábitos ou funcionamento da cancha. Não tornam a IA derrotável somente para quem encontrou tudo.

Permita prática/tutorial curto antes de iniciar o confronto valendo, conforme o padrão aprovado. Prática não é save, checkpoint ou retomada automática de uma derrota.

Ao perder uma mão, a partida continua conforme as regras. Ao perder a PARTIDA inteira, siga a política de derrota e carregamento manual, não ofereça um checkpoint secreto por conveniência.

Verifique justiça, sensação dos controles e ritmo por execução. Ausência de erro de compilação não demonstra que acertar uma bola é agradável.

---

## 18. RESULTADO DA BOCHA E CONTINUIDADE DA HISTÓRIA

VITÓRIA:
- Resolver o resultado físico e a partida antes de disparar narrativa.
- Entregar a segunda ficha conforme o sistema aprovado, uma única vez.
- Acrescentar informação concreta à busca de Adilio.
- Confirmar/aprofundar que Júlia respondia e que o resgate próprio foi repetido enquanto se adiava ajuda.
- Tonão pode tentar se defender. Não precisa tornar-se confiável em tudo por perder.
- Preparar uma pergunta ou contato ligado à etapa de Amaro, sem entregar sua sequência.
- Atualizar diário, estados de NPC, portas/gates pertinentes e inventário.
- Devolver controle e garantir acesso ao orelhão.
- Não salvar automaticamente.

A ficha deve ter identidade própria e reutilizar o sistema de itens. Seu vínculo com pessoas/documentos importa; não abrir geografia por magia nem consumir a ficha ao salvar.

O ganho do capítulo não pode ser somente “agora você pode ir ao próximo boss”. O jogador precisa compreender algo novo e saber por que continuar procurando.

Qual fato do presente Tonão entrega depende do gancho e das fontes do projeto. Não inventar que ele participou do sequestro, viu um capanga ou conhece um cativeiro sem resolver a origem do relato.

DERROTA:
encenação compatível com o tom, sem decretar morte literal de Marcus ou revelar poderes de Tonão. Oferecer carregar o último save MANUAL válido ou menu; sem save, não fabricar progresso de recuperação.

PÓS-VITÓRIA:
revisitas reconhecem o resultado e não repetem prêmio/cutscene. O fim do recorte permite salvar e depois mostrar um encerramento discreto. Não cortar automaticamente para créditos descartando a vitória não salva.

---

## 19. SAVE, MIGRAÇÃO E ESTADO ENTRE CAPÍTULOS

REGRA INEGOCIÁVEL:
progresso exclusivamente manual em orelhões. Nada de autosave ao trocar cena, coletar pista, vencer, sair, começar o capítulo ou encerrar uma mão.

Configurações podem persistir separadamente; jamais usar PlayerPrefs ou outro arquivo para conservar progresso que não foi salvo.

Mantenha orelhões acessíveis no Centro e perto da Bocha, com acesso posterior à vitória e indicação clara. Não exigir fichas de boss para salvar. A distância até um aparelho deve evitar repetição desnecessária sem criar save automático.

Compatibilidade:
- Preserve IDs e campos existentes; acrescente estado do capítulo 2 com valores padrão.
- Saves anteriores à vitória em Zé continuam antes dela.
- Saves pós-Zé entram na continuação com itens, pistas, nome do orelhão e objetivo coerentes.
- Não depender de campos C2 que saves antigos não possuem.
- Migração ocorre sem inventar eventos concluídos; não regravar progresso automaticamente na leitura.
- Escrita somente após ação manual, com backup técnico do mesmo save quando apropriado.
- Testar com cópias/fixtures; nunca sobrescrever o save pessoal do autor durante QA.
- Diferenciar versão incompatível, arquivo inválido e ausência de save.
- Os saves não precisam serializar bolas em movimento, já que o jogador não salva dentro da disputa.
- Restaurar estado de mundo sem duplicar managers, NPCs, fichas ou efeitos.

Sessão sem salvar:
ganhar a Bocha não concede posse persistente da ficha. Carregar o último save anterior restaura o estado anterior integralmente.
Sessão com save posterior:
preserva ficha, pistas, vitória e transições liberadas.

Fim do capítulo 1 antigo:
sua mensagem de encerramento não pode bloquear a continuação após a migração, nem aparecer de novo para quem já terminou Zé.

---

## 20. QUALIDADE VISUAL, ÁUDIO E INTERFACE

Preserve:
2D pixel art top-down 3/4 ortográfico, proporção 16:9, referência 320×180, tiles 16×16, PPU 16, personagens aproximadamente 24×32 e movimento cardinal, quando forem as especificações atuais aprovadas.

A cena pode ocupar muitas telas. Não reduzir o Centro inteiro ao tamanho do viewport nem usar perspectiva 3D/isométrica.

Câmera:
follow externo com bordas; interiores proporcionais; enquadramento fixo e legível na Bocha; transições curtas; sem rotação durante gameplay. Pixel Perfect deve funcionar sem tremor entre câmera, sprites e física.

Preserve a identidade de Marcus e a unidade dos retratos. Não misturar objetos de resoluções, contornos e paletas incompatíveis sem adaptação.

Arte de acabamento inclui:
sorting, pivôs, sombras/iluminação, portas, posições de interação, animações, passagens, chuva e contraste de leitura. Não escurecer objetos para esconder placeholder ou erro de composição.

ÁUDIO:
priorize os arquivos efetivamente aprovados. A direção posterior do Bar do Zé passou a usar vanerão frio, distante e discreto; não restaurar o samba/pagode da R1 por ser a tabela antiga. Não alterar uma faixa aprovada para “uniformizar” tudo.

Centro: tema de referência do lugar, investigativo e memorável, sem festa.
Cemitério: harmônio/graves respirados, metal/sinos irregulares e opressão, sem copiar o violão ou piano das demais regiões.
Bocha: sopro grave, madeira, padrão rolar -> silêncio -> impacto; intensificação contextual sem virar trailer épico.

Essas assinaturas são direção, não afirmação de assets presentes. Não fingir compor/gravar com ferramentas inexistentes.

Chuva e sons de ações são camadas independentes. Impactos de bochas acompanham colisões reais; sons perceptivos têm distinção suficiente para não falsear regra. Não duplicar chuva a cada cena.

Mixagem:
diálogos/interface legíveis; música não cobre bola/turno; limite sons repetitivos; transições/loops inspecionados; volumes persistentes; pausa correta. Não transformar volume alto em única estratégia de terror.

UI:
reutilizar diário, prompts de interação, menus, retratos e foco do capítulo 1. Pausa sem Salvar. Texto com acentos, sem clipping, legível na escala real; não resolver excesso de conteúdo diminuindo a fonte indefinidamente.

Acessibilidade:
feedback além da cor e áudio, intensidade reduzível de flashes/tremor e controles previsíveis. Não obrigar reflexo durante susto para completar investigação.

---

## 21. ARQUITETURA, ASSETS E ALTERAÇÕES SEGURAS

A versão de Unity solicitada originalmente foi 6.3 LTS, mas houve uso posterior de 6.6. NÃO escolha uma pela conversa: verifique ProjectVersion.txt e o ambiente real. Não atualizar/downgradear engine, Input System, URP ou pacotes por hábito.

Respeite serviços e dados existentes. Estenda interações, diálogo, progressão, áudio e save sem criar outro gerenciador concorrente para cada cena.

Prefira conteúdo editável pelo projeto a grandes scripts que constroem tudo em runtime. Se ferramentas de autoria forem necessárias, use-as de forma idempotente e limitada; a abertura do Editor não pode reescrever cenas artesanais aprovadas.

Preserve .meta/GUIDs. Evite reformatar/reimportar assets não relacionados. Não fazer commits/pushes/publicação sem autorização separada. Não criar branches destrutivas ou descartar mudanças da equipe.

Pesquisa externa está autorizada para documentação e assets faltantes, dentro das permissões reais. Priorize material existente. Verifique licença por item, uso comercial, modificação, atribuição, redistribuição incorporada e eventuais restrições de IA. Sem conclusão jurídica automática.

Registre origem/licença/alterações em inventário existente ou ASSET_SOURCES.csv; preserve créditos. Não copiar assets de Zelda, Pokémon ou outros jogos, transmissões esportivas ou gravações protegidas sem permissão.

Compras, publicação, envio de código privado ou instalação de integrações com novas permissões exigem confirmação. Não executar scripts de assets baixados sem revisão.

Se houver subagentes autorizados, divida tarefas independentes com propriedade de arquivos. Um responsável integra narrativa/estado. Não permita dois agentes reserializando a mesma cena ou alterando o mesmo save sem coordenação. Não instalar ferramentas só para paralelizar.

---

## 22. PLANO VIVO, MARCOS E CONTINUIDADE DE EXECUÇÃO

Use o local de documentação já adotado. Se não houver equivalente, mantenha um documento principal: Docs/Capitulo_02_Execucao.md.

Ele deve reunir, sem burocracia duplicada:
contrato de entrada; mapa/portas; sequência de objetivos; inventário de conteúdo; limites de revelação; regras da Bocha; decisões locais; dependências; marcos; evidências; pendências e próximo passo.

Não tratar esses nomes como funcionalidades automáticas do Codex. São arquivos de projeto para manter consistência em trabalho longo.

Marcos:
M0 — Inspecionar estado real, registrar baseline e contrato C1 -> C2; resolver só impedimentos centrais.
M1 — Prototipar Bocha base e seu principal risco; provar lançamento, medida, turnos e resultado.
M2 — Blockout fiel de Centro, interiores, Prefeitura, Cemitério e Beira-Rio, com transições reais.
M3 — Caminho crítico completo, do gancho existente ao confronto e consequência, usando os sistemas do jogo.
M4 — Conteúdo secundário, estados/revisitas, diálogos/retratos, puzzles e pistas.
M5 — Bocha psicológica, IA, feedback, terror contextual e recompensa integrados.
M6 — Acabamento audiovisual, interfaces, navegação, licenças e desempenho.
M7 — Regressão C1+C2, saves anteriores, testes da Bocha e build Windows quando disponível.

Faça revisão por marco: executar -> observar -> corrigir -> registrar. Blockout é etapa de trabalho, não entrega final disfarçada.

Não precisa pedir “continuo?” após cada marco autorizado. Pergunte apenas quando houver decisão central, permissão ou dado indispensável não resolvível.

Se houver limite de sessão/contexto, registre o estado real e o próximo passo reproduzível. Na continuação, leia esse registro e o repositório antes de trabalhar. Não encurte o capítulo para caber na última resposta e não prometa execução em segundo plano.

---

## 23. VALIDAÇÃO DE NARRATIVA, GEOGRAFIA E JOGABILIDADE

Faça revisões separadas, mesmo se realizadas pelo mesmo agente:

NARRATIVA:
cada objetivo tem causa; cada testemunha sabe o que diz; datas corretas; urgência de Adilio preservada; passado não vira passeio gratuito; nenhuma revelação de Lucas vivo; fontes falíveis não se tornam fatos absolutos.

MAPA:
igreja/rodoviária/prefeitura/escola/cemitério/bocha nas posições aprovadas; portas/spawns coerentes; ruas integradas; sem frente duplicada do mercado; sem Estrada da Usina ou Capela no lugar errado.

JOGABILIDADE:
ações legíveis; exploração fora de ordem tratada; sem softlock; minigame permite escolhas táticas e vitória legítima; terror não sabota controles.

ARTE:
cenas não são placeholders; escala/sorting/colisão consistentes; ambientes reconhecíveis; comparações visuais documentadas.

Somente depois fazer revisão de conjunto: as decisões de cada área precisam combinar. Uma cena isolada bonita não compensa quebrar a transição ou a motivação.

---

## 24. MATRIZ MÍNIMA DE TESTES

Adapte testes à arquitetura e execute no ambiente real. Não apagar/desativar testes antigos para obter sucesso.

CONTINUIDADE:
T01 Novo jogo atravessa C1 e C2 sem debug.
T02 Save anterior à Sinuca preserva bloqueio do Centro.
T03 Save antigo pós-Zé entra no Centro sem perder ficha/pistas.
T04 Vitória em Zé na sessão atual segue diretamente à continuação.
T05 Encerramento antigo de demonstração não bloqueia o fluxo.

GEOGRAFIA:
T06 Todas as conexões e portas de ida/volta usam spawn/orientação corretos.
T07 Câmeras não expõem vazios e não travam em corredores.
T08 Mercado permanece um estabelecimento coerente.
T09 Cemitério norte e Bocha sudoeste junto ao rio conferidos com referência.
T10 Nenhum atalho permite pular a condição pré-Zé ou entrar no capítulo 3.

CONTEÚDO:
T11 Visitar Tonão/Cemitério/interiores antes da ordem principal tem resposta válida.
T12 Pistas coletadas em ordem alternativa ainda produzem os objetivos corretos.
T13 Repetir diálogo/interação não duplica item/evento.
T14 Sair e carregar durante etapas de investigação restaura estado.
T15 Caminho crítico é concluível sem linhas opcionais.
T16 Conclusões relevantes possuem apoios e nenhum texto/retrato/interface entrega Lucas atual.
T17 Cada interior contratado contém sua função e acabamento, não somente porta/fachada.

BOCHA:
T18 Lançamento, cancelamento, estoque, repouso e troca de turno.
T19 Aproximação, proteção, remoção e deslocamento do balim são possíveis.
T20 Medida após repouso, empate de distância, ponto único e múltiplos pontos.
T21 Lado sem bolas, lado sem bochas válidas, ambas sem válidas.
T22 Saídas, balim fora, anulação e repetição não travam nem inventam placar.
T23 Vitória/derrota somente após resolução final da mão.
T24 Piso especial estável durante a mão e igualmente aplicado aos atores.
T25 Pausa durante mira, rolagem, medição, diálogo e efeito não consome lançamento nem duplica resultado.
T26 IA vence e perde de verdade; nenhum debug obrigatório ou terceira fase forçada.
T27 Alterar resolução/foco da janela não altera força, direção ou estado indevidamente.

SAVE:
T28 Vitória sem save não persiste por caminho oculto.
T29 Save posterior à vitória preserva segunda ficha, pistas e gates.
T30 Derrota oferece último save manual; sem save não inventa checkpoint.
T31 Save inválido/incompatível tem tratamento claro e não destrói arquivo válido.
T32 Migração de C1 mantém estado e não cria progresso extra.
T33 Testes usam dados isolados, não o save do autor.

REGRESSÃO E ACABAMENTO:
T34 Sinuca, controles, áudio, retratos, inventário e menus de C1 continuam funcionando.
T35 Sem missing scripts/referências, erros de compilação ou exceções do percurso.
T36 Sem chuva duplicada, impactos falsos constantes ou cortes de música entre cenas.
T37 Texto/retratos legíveis na resolução de referência e escalas testadas.
T38 Build Windows do recorte inicia e conclui o fluxo quando as ferramentas permitirem.
T39 Desempenho comparado à referência com ambiente registrado; sem números inventados.
T40 Revisão visual e playtest identificam problemas de orientação, repetição e ritmo; itens encontrados são corrigidos e retestados.

Teste regras puras em EditMode quando apropriado; integração em PlayMode; percurso e apresentação no Editor/build. Preserve a separação entre:
teste automatizado passou; execução foi observada; revisão visual realizada; avaliação humana ainda pendente.

Uma função de debug para QA é aceitável em contexto de desenvolvimento, mas não substitui percurso legítimo e não deve vazar para a build de jogador.

---

## 25. PORTÃO DE ENTREGA E RELATÓRIO FINAL

Não declarar concluído sem verificar:
[ ] C1 aprovado preservado e regressão executada.
[ ] Continuidade real a partir da vitória/save de Zé.
[ ] Centro e interiores contratados finalizados.
[ ] Prefeitura, Igreja, Escola, Rodoviária, Cemitério e Bocha nas posições aprovadas.
[ ] Investigação encadeada com motivo para cada deslocamento.
[ ] Conteúdo substancialmente ampliado, sem enchimento.
[ ] Cemitério com função e identidade própria.
[ ] Tonão e Bocha reconhecíveis, táticos e psicologicamente inquietantes.
[ ] Tutorial, IA, regras, pontuação, empates, vitória e derrota completos.
[ ] Ficha/pista pós-boss e fechamento do capítulo coerentes.
[ ] Save exclusivamente manual e compatibilidade com C1.
[ ] Retratos, áudio, chuva, UI e acessibilidade básicos integrados.
[ ] Sem spoiler indevido de Lucas e sem resolver pendência central silenciosamente.
[ ] Licenças registradas e nada restrito publicado indevidamente.
[ ] Testes/build/evidências reportados com limitações reais.

Relatório final em português:
1. O que o jogador pode fazer agora, do começo ao encerramento de C2.
2. Como iniciar, qual cena/build usar e quais controles existem.
3. Áreas/interiores entregues e relação com o mapa.
4. Sequência narrativa implementada e quais propostas viraram decisões locais.
5. Regras efetivas da Bocha e integração do terror.
6. Compatibilidade de saves e regressões verificadas.
7. Testes executados, resultados, logs/capturas e ambiente.
8. Assets acrescentados, origem, licença e atribuições.
9. Arquivos/sistemas alterados e justificativa de intervenções em C1.
10. Pendências reais, partes não executadas e instruções manuais indispensáveis.

Não confundir “arquivo criado” com “cena validada”. Não dizer que gerou uma build inexistente, ouviu áudio sem reprodução ou que o jogo é divertido sem avaliação.

Quando o ambiente impedir testes visuais, Editor ou build, deixe isso explícito. Faça o trabalho verificável que estiver ao alcance sem declarar o restante concluído.

OBJETIVO FINAL:
um segundo capítulo maior e mais rico, integrado ao primeiro, fiel ao mapa, com investigação que faça sentido e um confronto de Bocha próprio — completo até sua consequência, sem avançar para o resto da campanha.
