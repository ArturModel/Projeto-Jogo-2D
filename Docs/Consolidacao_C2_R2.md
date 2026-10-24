# Consolidação C2 R2 — execução

## Base e proteção

22/10/2026; branch `capitulo_2`; base `fbd2d420150adb82978cee649e632ef31b100e61`, com 40 alterações locais anteriores preservadas. Backup externo: `C:\Users\Usuario\.santarita\backups\C2_R2_20261022_155741`. Suítes usam namespace temporário próprio. Para a inspeção do Player comum, uma cópia intacta de C1 e fixtures de apresentação ficaram em um perfil externo isolado; os arquivos originais foram preservados, restaurados e comparados por SHA256 (cinco arquivos idênticos). Preferências originais restauradas; nenhum redirecionamento permanece.

Escopo: C2, lanterna compartilhada e seletor de carga após derrota nos dois jogos. Escola/C3 e reautoria geral de C1 ficam fora. Não executar builder legado. Floricultura: nenhuma ocorrência encontrada em cenas, scripts, dados ou documentos da versão local; não criar comércio para preencher a ausência.

## Marcos

- [x] A: integração, conversas e consequências, fontes, efeitos, seleção explícita de save.
- [x] B: Sociedade como piloto; planta, montagem, circulação e rotina.
- [x] C: preferência manual F, poses e luz, persistência apenas no orelhão.
- [x] D: função e escala nos outros ambientes, sem decorar todos da mesma forma.
- [x] E: câmera real, regressões, AI, Player normal e registro das limitações. Campanha integral, avaliação artística e audição não são certificadas por estes testes.

## Plantas de trabalho (unidades do mundo; Marcus ~1,5 de altura)

Portas e posições geográficas externas preservadas. Câmera de gameplay existente, sem girar, encolher Marcus ou afastar zoom para disfarçar escala.

| Ambiente | Função, planta e circulação | Trabalho / informação / luz |
|---|---|---|
| Sociedade, 30 × 19 | Entrada sul x5; convivência x2–13/y2–7; apoio junto à parede oeste; cancha x6–28/y9–15; espectadores em y7 e y16; parede norte começa em y17. Corredor oeste dá acesso ao suporte e bancada sem atravessar lançamento. | Tonão: suporte → bancada → cabeceira → descanso. Nair: copa e secagem da bancada. Caderno na mesa pequena; regras junto ao suporte; lâmpadas sobre cancha e convivência. |
| Igreja, 18 × 22 | Entrada central, nave com corredor x7–11 e dois conjuntos de bancos; altar norte, apoio lateral. | Bento arruma apoio do altar, acessível pelo corredor lateral. Mural perto da entrada; velas em nicho. |
| Prefeitura, 20 × 17 | Recepção ao sul; espera lateral; passagem central até consulta pública autorizada ao norte. | Clarice junto de atendimento; protocolo sobre balcão; manutenção/extrato em mesa de consulta. Luminárias de trabalho. |
| Rodoviária, 22 × 15 | Entrada central; espera oeste; guichê leste com passagem do funcionário; horários visíveis da espera. | Dário confere bilhetes; passageiros esperam junto a bancos; telefone narrativo junto ao guichê, distinto do save. |
| Depósito, 16 × 14 | Entrada sul; porta de serviço norte para loja C1; estoque nas laterais, eixo central livre. | Conferência em mesa; romaneio em prancheta; campainha junto à porta da loja. Sem segundo atendente/fachada. |
| Oficina, 16 × 13 | Espera sul, bancada norte, peças em estante oeste, equipamento em teste separado. | Mila trabalha em rádio e ferramenta; desenho no armário seco; luz de bancada. |
| Costura, 16 × 14 | Espera sul/leste; máquina ao norte/oeste, tecidos na parede; corredor central livre. | Celina à frente da máquina, tecido apoiado na mesa; encomenda na bancada. Luz doméstica e de tarefa. |
| Abrigo, 13 × 11 | Entrada sul; guarda de material oeste/norte; mesa de manutenção leste; passagem central. | Caderno em mesa alcançável; ferramentas guardadas. Luz do apoio não ilumina todo o cemitério. |
| Cemitério | Manter limites, portão sul e jazigo canônico. Caminhos entre conjuntos de sepulturas, manutenção perto do abrigo. | Alcina varre percurso concreto. Portão/abrigo iluminados, margens legíveis com ganho da lanterna. |
| Centro | Manter ruas/lotes/river/mapa. Corrigir ligações de passeio/porta, bancos orientados aos caminhos e manutenção de drenagem. | Chico junto da grelha; praça pública cotidiana. Pontos práticos iluminados e intervalos com contraste moderado. |

## Inspeção antes

Capturas em `Docs/Evidencias/C2R2/Antes`, feitas pela build Development anterior, com fixture isolada e reposicionamento declarado. Servem à comparação visual; não representam percurso legítimo da campanha. A inspeção de código confirmou cancha vertical 24 × 40, acessórios genéricos dos NPCs, lanterna automática externa e reação pós-vitória ocultando opcionais. A avaliação das imagens e o percurso observado serão registrados após a captura.

## Dependências compartilhadas indispensáveis

- Player/GameState/SceneFader: estado manual de equipamento, bloqueio de input/apresentação e pose; sem nova política de C1.
- Payphone/Sinuca: reutilizar o seletor de cinco registros na derrota; cancelar volta ao resultado. Não reabrir regras da Sinuca.
- Migração espacial dos interiores reduzidos: adaptar somente posição carregada de save antigo, em memória, preservando arquivos e flags.
- SceneFader: a sincronização antiga removia o Cemitério da lista em cada Play; a correção mantém a cena e preserva entradas adicionais. Código de sincronização restrito ao Editor.
- Névoa compartilhada: respeitar também `Capitulo2Director.interior`, evitando névoa externa dentro das novas salas. C1 mantém seu próprio critério.

## Revisões após observar a câmera

- Sociedade passou de 24 × 40 para 30 × 19: cancha 22 × 6 no eixo horizontal, apoio e convivência ao sul/oeste. O metro adicional ao norte preserva passagem de espectadores em vez de apertá-los contra a guia.
- Antes: oficina/costura com muito chão sobrando e papel/pano genérico afastado do móvel; Rodoviária com guichê isolado; depósito com estoque de desenho muito simplificado. Depois: bancadas em escala, poses com braço/ferramenta, estações ligadas a superfícies e caixas próprias de C2 com dobras/fita/impressão.
- A primeira captura da Rodoviária revelou passageiros antigos em raízes externas ao diretor, duplicados pela nova composição. Remoção restrita a essas duas raízes conhecidas, sem apagar conteúdo desconhecido.
- No piloto, a referência da luz do ator estava nula: possuir a lanterna não criava feixe em C2. A fonte agora pertence ao equipamento de Marcus e é criada uma vez em cada ator.
- Primeiro ensaio R2 no Editor: 261 checks passaram (conversas/callbacks, F/UI, cenas e apoios). O ensaio parou por procurar o botão da Sinuca pelo nome exibido; esse botão tem identificador genérico. Corrigido o harness para acionar o botão real. Esse ensaio não é resultado final de aprovação.

## Evidências e resultado

### Mudanças funcionais

- Derrota na Sinuca e na Bocha abre o mesmo seletor explícito dos cinco slots. Cancelar retorna ao resultado; escolher carrega exatamente aquele registro. Sem save, há retorno ao menu, sem checkpoint inventado. Não há mais recarga automática do registro mais recente nesses resultados.
- O despachante real de conversas prioriza apresentação e opcionais disponíveis; reação pós-Bocha ocorre uma vez e não esconde conteúdo restante. Consequências/diário só ocorrem após fechar a conversa. Primeiro telefonema tardio conserva a apresentação e integra o recado.
- Protocolo exige a autorização de Clarice; Alcina/caderno de manutenção não viram testemunhas de um trajeto de Adílio que não presenciaram. Rádio LOCAL tem retorno de teste audível contextual. Percepção do jazigo usa a referência serializada atual. Resposta abafada da Bocha tem clip próprio e atraso após colisão real, sem trocar trilhas Suno.
- Poses de empunhadura completas em Aseprite para Marcus, com braço/mão de sua paleta, oito poses de passada e repouso. Mesma `MarcusGait`/apoios 0 e 4; origem da luz acompanha a lente. Corrigida a inversão herdada ao virar da direita para cima/baixo. F altera RAM, não salva automaticamente, e é suprimido durante UI, transição e minigames sem apagar a escolha.
- Posições de checkpoints antigos são adaptadas à nova planta em RAM, por área livre conectada à saída. O save antigo da Sociedade não nasce dentro da cancha fechada. Cenas/metas/GUIDs e destinos geográficos preservados.

### Build e revisão

Unity 6000.6.0f1, Windows x64/D3D11. Sem commit nesta etapa. SHA256 conjunto de C#/cenas/dados/texturas/materiais/metas: `8ac8e9f20fba115336649495a7976196ef3e4d99c23c5ba970c81bf757a460c0`.

| Artefato | Resultado |
|---|---|
| QA Development `Build/ProducaoQA/SantaRita.exe` | Zero erros; 295.465.837 bytes; 22/10 às 20:23:45 UTC. |
| Player comum `Build/Capitulo2/SantaRita.exe` | Zero erros; 224.015.098 bytes; 22/10 às 20:28:19 UTC. Mesmo hash de fontes da QA. |
| Assembly do Player comum | Sem `Chapter2RuntimeVerification`, autoria R2 ou builder C1. Rotinas e migração de planta presentes. |
| Regras/saves e produção | 125 verificações e 98 contratos passaram no Editor. |
| Cenas/acessos | Dez cenas C2 passaram; destinos/IDs de spawn, aproximação a interações e ligação do jazigo conferidos. |
| Runtime R2 no executável QA | 290 verificações + encerramento PASS às 20:29:01 UTC. Conversas em ordens diferentes, fontes, todos os interiores, portais/mapa, poses/viradas de F, bloqueios de UI, dois seletores, caso sem save e migração antiga. |
| Regressão C1 na mesma build QA | 63 verificações + encerramento PASS às 20:52:05 UTC: investigação, Esc/pausa da Sinuca, espera da IA, revisitas e retornos C1/C2. Primeira tentativa em segundo plano falhou no Esc; repetição em primeiro plano passou sem alterar produto ou harness. |
| IA Bocha | `Escolher` síncrono: 4,33 ms (balim), 18,06 ms (quatro bochas + balim), 50,32 ms (sete + balim), nesta máquina. Não é medição de FPS da praça. |
| Player comum pela interface | Novo Jogo/prólogo/chegada; menu/carregamento por clique; Sociedade, Cemitério, praça, rua e Abrigo com pares apagada/acesa; cópia intacta do save C1 de Rua Norte carregada pelo slot 2. |

### Índice local das evidências

Raiz: `Docs/Evidencias/C2R2/` (arquivos pesados ignorados pelo Git; estes caminhos são locais e não prometem acesso a outros leitores).

- `Antes/`: dez capturas da build anterior, com fixture declarada; `Piloto/` e `Editor/`: revisão intermediária. `PlayerQA_Final/`: capturas finais de todos os ambientes, cabeceira, entrada/convivência, quatro direções, comparações de F e seletores de derrota; `runtime-qa.txt`, `build-qa.txt`, `cenas-acessos.txt`. Regressão final C1 em `C1RegressaoFinal/runtime-qa.txt`. Os seis relatórios finais R2 selecionados ficam versionáveis; capturas, progresso e tentativas intermediárias continuam ignorados.
- `PlayerNormal/`: pares `sociedade`, `cemiterio`, `centro-praca`, `centro-transicao`, `abrigo`, com sufixos `-desligada.png`/`-ligada.png`; mesmo ponto, câmera, resolução e brilho configurado em cada par. Clima/partículas continuam animados. `c1-save-antigo-rua-norte.png`, `seletor-cinco-slots.png`, `assembly.txt`, `saves-preservados.txt`.
- `Tonao-rotina.gif`, `Nair-rotina.gif`: atividades e deslocamentos na câmera de gameplay. `Marcus-cima.gif`, `Marcus-baixo.gif`, `Marcus-esquerda.gif`, `Marcus-direita.gif`, `Marcus-detalhe.gif`: passada, parada, F apagando e reacendendo a empunhadura. Cada direção tem 36 quadros reais (30 acesa/6 apagada); as alternâncias passam por eventos de F no Input System. Originais e tempos de captura em `MovimentoEditor/captura.csv`.
- Captura no Editor: 1024 × 576, quadro real de gameplay, reposicionamento declarado por segmento; exportação normal 512 × 288 e detalhe 384 × 320 com ampliação inteira. GIFs sem trilha de áudio e com durações obtidas do CSV; não são montagem de sprites para simular gameplay.
- Player comum: captura lógica de janela 1536 × 864, render configurado em 1920 × 1080, sem indicador Development. Pares usam preferência carregada em fixtures distintas; isso prova apresentação e carga do estado, não o recebimento de F físico nessa build.
- Build final detalhada em `Docs/Evidencias/CidadeViva/PlayerFinal/build-windows.txt`; contratos/regras em `Docs/Evidencias/Producao/`. Os relatórios históricos de CidadeViva anteriores a R2 continuam históricos.

### Reprodução

Com Editor parado e cenas salvas: `Santa Rita/Arte/Verificar produção e regressão (isolado)`, `Santa Rita/Capítulo 2/Verificar cenas e acessos`, `Santa Rita/Arte/Build QA de produção` e `Santa Rita/Cidade/Compilar Player validado`. O autoring R2 é explícito, nunca callback automático. Não executar reconstrução antiga de C1.

QA: definir `SR_EVIDENCE_DIR` para uma pasta local e executar a build Development com `-srC2R2QA`; C1 usa `-srC1LoadSmoke`, mantendo a janela em primeiro plano. O Player comum não contém esses atalhos. Gravação: `Tools/QA/Record-R2-Motion.cs` via Unity MCP em Play no menu, seguida de `Tools/Aseprite/r2-evidence-gif.lua`. Fontes de produção editáveis em `Arte/Producao/C2R2/` e `Arte/Producao/Marcus/`; procedência em `ASSET_SOURCES.csv`.

### Limites e backlog separado

- Tentativa legítima no Player comum: Novo Jogo, prólogo e chegada sem flags/teleporte. O teclado enviado por `sky.press_key` não produziu navegação/pausa/F (inclusive no menu), embora cliques funcionassem. Não foi possível concluir o percurso até o segundo boss com essa automação; isso não estabelece defeito do teclado físico e não pode ser apresentado como T01 aprovado. Input System e callbacks foram exercitados nas suítes QA.
- Carga de uma cópia intacta de C1 confirma leitura/cena/posição antigas, sem fabricar flags. Não comprova todas as combinações de saves nem vitória humana na campanha.
- Há avisos de timestamp/cor dos vídeos antigos, e o redirecionamento temporário do perfil produziu avisos `CreateDirectory` na telemetria Unity. Perfil original já restaurado; nenhum erro C# foi observado. Overflow do diálogo introdutório de C1 foi observado na regressão e fica no backlog de UI legado, separado de R2.
- Audição da mixagem, naturalidade de foley, medo, diversão e aprovação artística do autor continuam pendentes. Logs de amplitude/duração, imagens e testes não certificam essas avaliações. Não foi criado C3, Escola ou floricultura.
- Revisão final de Changes: capturas/GIFs/logs ignorados, fontes e relatórios finais selecionados mantidos. `git diff --check` de código/docs passou; a checagem integral aponta somente 301 campos YAML vazios que o serializador Unity grava com espaço depois de `:` nas cenas. Sem remover esse formato para produzir um hash de build diferente sem mudança funcional. Hash de fontes reconferido após o último Play: idêntico ao dos dois executáveis.

Editor devolvido a MainMenu, sem Play e sem cena suja. Cinco hashes pessoais reconferidos após todas as suítes; zero chaves `SRQA_` tanto no Player quanto no Editor.

Não considerar montagem, captura isolada ou fixture como aprovação artística, audição ou campanha legítima. Não foram feitos commit, push ou publicação.
