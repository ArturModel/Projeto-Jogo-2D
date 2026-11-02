# Bar do Zé / Sinuca Brasileira / itens — R4

Estado: implementação validada tecnicamente em 30/10/2026; build normal entregue. Conferência manual da build normal limitada pelo aviso do Firewall, detalhado abaixo. Não representa aprovação artística pelo autor.

## Implementação

- Planta `SantaRitaBarLayout` compartilhada pela exploração e projeção da sinuca: balcão/bebidas no fundo, mesas e cadeiras plásticas por grupos de convivência, TV, rádio, calendário de outubro de 2004 e cartaz de galeto. Atores usam os mesmos sprites, coordenadas e poses; Zé alterna trabalho no balcão/mesa e acompanha o duelo. Fontes Aseprite editáveis e exportação reproduzível em `Tools/Aseprite/bar-2004.lua`.
- Câmeras diagonal com órbita pelo botão central, linha do taco com ajuste fino horizontal e superior. Zoom limitado; projeção inversa testada. Mesa com área útil 2,84 × 1,42 m e bolas proporcionais (~52,7 mm), cores brasileiras sólidas, seis bocas/redes, visual de queda e acomodação amortecida. Trata-se de projeção 3D em Canvas, preservando a câmera URP 2D do mundo.
- Núcleo separado de regras brasileiras: valores 1–7, bola da vez, livre/opcional/castigo, reposições, saída repetida, falta de sete, devolução de ação e término por pontuação. A bola marcada é a exceção narrativa autorizada; nunca impede a bola da vez. Série melhor de três preservada. Não constitui certificação arbitral de todas as situações desportivas.
- Contato na branca por disco arrastável, giro lateral nas tabelas e longitudinal na colisão. Força oscilante por pressão, tacada ao soltar, cancelamento fora do campo/foco, seleção explícita de bola cantada e prática opcional.
- Cinco slots derivados do GameState existente: lanterna, mapa, recibo e duas fichas. Sem armazenamento paralelo. Carro entrega só lanterna; folheto de orientação disponível na rodoviária fornece o mapa C2. Diário continua acessível antes disso. Saves antigos que já possuíam mapa preservam sua posse.
- Recibo exige PEGAR; mercadinho exige ENTREGAR e prossegue ao testemunho. A posse/entrega é exibida na barra. Ações usam botão acima do diálogo e não coletam só ao examinar.
- Música do bairro contínua em Posto, Encruzilhada e Bairro; interiores respectivos reduzem e filtram em 1800 Hz, sem reiniciar o loop. Arquivos do autor preservados. Teste de fonte/filtro não equivale a aprovação auditiva.
- Soleiras encostadas ao acesso e alcance condicionado aos pés no tapete, mantendo E manual.

## Contexto preservado

HEAD 30d811c e branch atualizacao_sinuca. Revisados PRs de interface/inicialização, retratos/pistas, flora e preenchimento de interiores. A grande reformulação inicial de SinucaMinigameUI/SinucaTableView e QA já estava em Changes; o trabalho atual amplia essa base. Nenhum reset geral, commit ou push. Escola continua adiada, C2 termina na Bocha e consequência imediata.

MCP localizado pela janela real em 127.0.0.1:8088, instância Projeto Jogo 2D@a7a0c66d831bf259; a tentativa antiga em 8080 não correspondia ao servidor ativo. Nenhuma cena com trabalho não salvo foi regenerada. Backup e cliente temporário fora do repositório.

## Referências e procedência

Regras: [publicação CBBS/Ases](https://www.asessinuca.com.br/regra-sinuca), [regulamento](https://www.asessinuca.com.br/regulamento-do-bilhar). Inspiração de controle de câmera/giro: [Virtual Pool](https://vponline.celeris.com/support/quickstart). Não foram copiados recursos visuais desses jogos. Novos props são autoria Aseprite e poses derivam dos personagens existentes; ASSET_SOURCES.csv conserva essa distinção.

## Evidência

`Editor/runtime-qa.txt`, `PlayerQA/runtime-qa.txt`, relatórios de build/assembly e preservação em `PlayerNormal/`: resultados finais registrados nos arquivos citados. Capturas locais ficam fora de Changes por .gitignore. Fixture usa aproximações/estados em RAM e input real de EventSystem/Input System; não comprova uma campanha completa jogada legitimamente.

### Resultados confirmados até aqui

- Editor: 200 verificações + encerramento PASS, sem FAIL/ERROR no relatório final. Uma mão física/IA completa em 45 tacadas (36×34), fixture física de série 2×0/callback, contato por clique, cancelamento, coleta/entrega e 11 saídas por E (quatro C1, sete C2).
- Build Development QA: Succeeded, zero erros, 297.889.945 bytes; SHA256 agregado `a984dd1c00639304fc0363029de7f0e5b0c9ce42e35d3b58a96d150e92be6307`. `Chapter2RuntimeVerification` presente na assembly de QA, confirmado por leitura de metadata Mono.Cecil.
- Graphify refresh e validate: PASS, 103 fontes C#, hashes atuais/caminhos portáveis; 1814 nós, 5980 arestas, 94 comunidades e 1908 notas. Índice estrutural, sem inferir aprovação de arte/áudio.

As dimensões de mesas comerciais variam entre fabricantes; a implementação escolheu campo de 2,84×1,42 m, não uma alegação de que todo bar possui esse tamanho. A escala é compartilhada entre exploração e projeção. Referência comercial de acabamento/redes: [mesa tradicional com redinhas](https://fabricadasinuca.com.br/products/mesa-de-sinuca-fs-com-rede-grande); não foram baixadas suas imagens.

- Player QA final: 200 verificações e encerramento PASS em 30/10/2026 22:56:41 UTC, nenhuma linha FAIL/ERROR. Mesma mão 45 tacadas/36×34; cliques, disco de contato, perda de foco, série, itens, música, snapshot e onze saídas. Capturas do Player conferidas: fundo opaco, sem duplicação da cena de exploração, controles separados, forro/paredes na vista de taco.
- A sequência de frames de QA alterna as câmeras diretamente para medir a projeção; o texto do botão de vista nessas capturas não é usado como indicador do modo do teste. No jogo a troca ocorre pelo botão/V.

## Limites e reprodução

Fluxo visual recomposto: `EntradaDaCidade` / `Interior4_BarDoZe`, que é o bar da campanha conectada atual. As cenas avulsas antigas abertas diretamente no Editor não foram reautoradas nesta etapa. Saves pessoais foram comparados por hash; compatibilidade de equipamento/entrega foi exercitada por snapshot isolado, sem afirmar homologação de todos os saves históricos.

A mão completa usa o seletor de IA e o integrador/resolvedor reais em passos acelerados, dirigidos pela fixture; os cliques são exercitados separadamente com Input System/EventSystem. A série usa fixture física declarada da última bola junto à caçapa. Isso não equivale a uma campanha inteira jogada por uma pessoa nem a uma avaliação auditiva/artística do autor. As redes usam acomodação analítica amortecida ligada à queda; não são uma simulação de tecido.

Fontes de arte reproduzíveis: `aseprite -b --script-param root=<raiz-do-repo> --script Tools/Aseprite/bar-2004.lua`. Fontes editáveis fora do Player em Arte/Producao/Bar2004; PNG/meta no Resources; autoria/derivação no CSV.

## Entrega normal e preservação

- `Build/Capitulo2/SantaRita.exe`: build normal Succeeded, zero erros, 226.327.149 bytes. SHA256 de fontes igual ao Player QA: `a984dd1c00639304fc0363029de7f0e5b0c9ce42e35d3b58a96d150e92be6307`. Relatório `PlayerNormal/build-windows.txt`.
- Metadata da DLL normal: seis componentes novos/renderer presentes; Chapter2RuntimeVerification, SimularTesteDeInvariantes e BeginIsolatedTests ausentes. `PlayerNormal/assembly.txt`. Ferramentas Lua/fontes Aseprite e Editor ficam fora do conteúdo jogável. A dependência de helpers MCPForUnity.Runtime do pacote permanece; não é a bridge/servidor Editor e não foi atribuída como causa do aviso de rede.
- Executável normal abriu o menu, observado na captura nativa; Windows exibiu pedido de acesso de rede pelo Firewall sobre a janela. Nenhuma ação nesse pedido foi automatizada. A instância de teste foi encerrada. A seleção de slots por cliques nessa build normal **não foi homologada**; `PlayerNormal/smoke.txt` distingue isso dos testes do Player QA. A regra de computador usada proíbe atuar em pedidos de segurança/privacidade, sem exigir nova consulta ao autor ausente.
- Seis arquivos originais (slots 0,1,2,4, principal e backup) com SHA256 idêntico ao baseline externo. Sem resíduos SRQA em PlayerPrefs e sem diretório de teste desta execução. Temporários antigos preservados. `PlayerNormal/saves-preservados.txt`.
- Editor devolvido à MainMenu, fora de Play, cena sem alterações não salvas. MCP respondeu após a compilação.

Verificação visual final do forro e das falas de convivência ocorre na build QA final; a suíte do Editor foi executada antes desses últimos ajustes de renderização/texto. Nenhuma aprovação artística ou auditiva do autor foi presumida.
