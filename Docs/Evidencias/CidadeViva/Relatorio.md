# Revisão de cidade viva — iniciada em 21/10/2026, fechamento em 22/10/2026

> Histórico pré-R2. A consolidação posterior de ambientes, lanterna manual e seleção após derrota está em [Consolidação C2 R2](../../Consolidacao_C2_R2.md), com builds e verificações próprias. O arquivo de build normal foi atualizado; as contagens abaixo continuam correspondendo às rodadas históricas indicadas.

## Implementação

- Cemitério municipal separado em `CemiterioSantaRita`, com acesso pelo Centro, abrigo e caminho de volta. Saves antigos com posição no antigo cemitério são encaminhados à nova cena, conservando coordenadas.
- Centro com fonte, bancos, jardim, lixeiras e quadro de avisos cotidiano. Removida a mesa dedicada a Júlia. O boletim antigo fica no arquivo da Prefeitura; o jazigo tem aparência envelhecida.
- Igreja, Prefeitura, Rodoviária, depósito, oficina, costura, Sociedade/cancha e abrigo revistos por função. Quatro interiores ativos de C1 recebem mobiliário compatível: conveniência, mercadinho, casa do avô e Bar do Zé.
- 24 novos móveis a partir dos atlas, com fonte Aseprite; mesa de sinuca, piso de saibro, pilares e guias contínuas de madeira desenhados em pixels. Mesas de convivência ficam no salão, documentos nos arquivos, estoque nas áreas de serviço e ferramentas nos locais de trabalho. Caminhos rurais diagonais receberam acabamento em pixels para retirar a aparência de escada de lajes.
- Moradores com atividades locais independentes da aproximação do jogador: varrer, manusear papéis, limpar, costurar, cuidar do equipamento e tomar chimarrão. São rotinas locais animadas, não uma simulação completa de horários e trajetos pela cidade.
- Retorno bidirecional pelas conexões do Mercadinho e da Rua Norte/Bar do Zé. Progressão da investigação não fecha caminhos já liberados.
- Rim light discreto ligado às luzes próximas, transparência gradual de copas e respingos no contato dos pés complementam URP 2D, iluminação ambiente/dinâmica, sombras, oclusão de contato, partículas e pós-processamento já existentes. Escopo real em `Docs/Matriz_Tecnicas_Arte.md`.

## Evidências

- Dez cenas passaram na verificação de acessos: `cenas-acessos.txt`.
- 98 contratos de produção e 114 verificações de regras/saves passaram novamente em 22/10, após a correção das camadas. Relatórios copiados para `verificacao-producao.txt` e `regras-save.txt` nesta pasta.
- Dois percursos C2 na build Development passaram, com 128 verificações por percurso, incluindo save antigo do cemitério e ida/volta pelos dois acessos. Resultado final: `PlayerQA/RuntimeFinal/runtime-qa.txt`. Rodaram com fixture e slots isolados.
- A revisão de C1 passou com 35 verificações e repetição aprovada, já com a mesa de sinuca detalhada: `PlayerQA/C1Final/runtime-qa.txt`. Os testes não usam slots pessoais. A fixture final inclui equipamento e visita à casa do avô compatíveis com a fase da investigação.
- A inspeção visual encontrou e corrigiu cadernos escondidos atrás das mesas pela ordenação por Y. O depósito usa caixas de mercadoria e vasilhames, preservando ferramentas na oficina e no abrigo.
- Player final compilado em 22/10 às 00:25 (horário de Brasília), zero erros: `PlayerFinal/build-windows.txt`. A verificação de acessos dessas cenas está em `cenas-acessos.txt`.
- Inicialização final em modo batch: Unity/Mono, PhysX e D3D11 ativos, processo respondendo, sem exceções de script/shader. O teste registrou fallback de tela cheia e avisos do vídeo preexistente; não equivale à campanha ou inspeção visual interativa. Detalhes em `PlayerFinal/startup-final.txt`.
- Código de fixture/QA e ferramenta de composição ausentes da assembly de jogo final. Editor devolvido à MainMenu, sem cena suja e fora de Play. Não houve commit, publicação ou alteração de slots pessoais.

## Material local

Capturas, GIFs e rodadas intermediárias foram retirados do projeto na limpeza solicitada pelo autor. O histórico permanece no arquivo local indicado em `../README.md`; futuras capturas são ignoradas pelo Git. Aqui ficam apenas o resumo e os resultados finais necessários.

O guichê foi aberto visualmente e Dário reposicionado atrás do balcão; cadernos foram colocados sobre as mesas de exame. Mila e Celina foram aproximadas de suas bancadas. Acesso/colisão dessas posições foi revalidado nas dez cenas na build final.

## Reprodução

Com o Editor fora de Play e cenas sem trabalho não salvo, `Santa Rita > Cidade > Compilar Player validado` compila a versão comum em `Build/Capitulo2/SantaRita.exe`, registra os acessos/build nesta pasta e restaura as cenas abertas. O comando fica exclusivamente no Editor; ferramentas de autoria não integram o player.

## Limites

A comparação visual é feita em capturas da câmera real e corrigida durante a produção. Não equivale à aprovação artística pelo autor nem a afirmar igualdade com a imagem de referência. Os testes automatizados usam fixtures e saves isolados; não certificam uma campanha humana completa nem audição da mixagem.

## Revisão dos retornos antigos — 22/10/2026

Baseline `fbd2d420150adb82978cee649e632ef31b100e61`, branch `capitulo_2`, alterações locais sem commit. O inventário dos retornos e o que já estava corrigido está em `../../Capitulo_02_Matriz_Testes.md`.

- Montador C1 aposentado como preparação destrutiva: validação/compilação não reescrevem cenas, Bar ou sprites. Comando de cenas C2 ausentes também preserva importadores aprovados; reautoria exige a opção explícita de regeneração.
- Recibo, apresentação e testemunho separados; diário/objetivo/orelhão respeitam o conhecimento transmitido. Casa sem bilhete não inventa seu conteúdo. Café/cartas/desordem/telefone formulam suspeitas; retirado o identificador técnico da ficha das falas.
- Nota atualizada sinaliza progresso e notifica a UI. Fallback do primeiro slot só aceita legado sem índice; save moderno de outro slot não aparece duplicado. Escopo QA restaura memória/spawn e limpa preferências próprias de forma persistente.
- Sinuca: tabela só vale após primeiro contato; penalidade coerente nas falas; Esc pausa, suspender cancela carga, regras não restauram estado obsoleto da IA e rotinas terminam ao fechar/recomeçar. Resolver real cobre falta + bola comum, quarta bola + branca, 2–0, 2–1 e derrota.
- Camadas reais de áudio por mão: murmúrio abstrato, água ressonante e chuva abafada, com controle de efeitos/pausa e sem alterar as faixas Suno. Síntese original identificada no manifesto; audição humana não certificada.
- Geografia centralizada. Capela no meio inferior; Prefeitura no antigo lugar da Escola, com porta, retorno e acesso; Escola permanece futura. Rótulos históricos são corrigidos por texto sobreposto sem modificar a imagem do mapa. Novas raízes de autoria em C1 não são mais ocultadas genericamente.
- Falha encontrada no executável: cemitério ausente da lista persistida de build. Cena registrada e destinos das portas verificados antes da compilação; cemitério/abrigo/retorno passaram após correção.

### Verificação desta revisão

- Cinco falhas reproduzidas antes da correção pelo comando `Auditar retornos antigos (isolado)`; onze grupos passaram depois.
- 125 verificações de regras/saves/narrativa/áudio e 98 contratos de produção passaram no Unity atual.
- Executável Development: 58 verificações em C1, incluindo conversas reais antes/depois da coleta, revisitas, Esc por Input System, bolas congeladas na pausa, regras nas duas esperas da IA, eventos de áudio e retorno ao Centro.
- Executável Development: 151 verificações em C2, incluindo investigação, Bocha, vitória/derrota, consequência, gravação/recarga, retorno pelos dois acessos, cemitério e marcador de Marcus na interface do mapa durante as visitas.
- Esses percursos usam fixtures, reposicionamento e entradas controladas conforme os próprios relatórios. Não são um novo jogo legítimo completo sem QA/debug. T01, avaliação artística/diversão e audição permanecem pendentes de playtest humano.

Relatórios de build registram commit base, hash SHA256 dos C#/cenas, ambiente, alvo, resultado e caminho. Capturas e logs intermediários continuam ignorados pelo Git.

### Rastreabilidade dos executáveis

| Execução | Build e origem |
|---|---|
| C1: 58 checks, encerrado às 18:12:22 UTC | Development de 18:09:57 UTC; SHA256 C#/cenas `a2568e4fa8a5d6f2ee8fcfe4d6265dfad579fb6f7467b9138de0e5f8ec789114`. |
| C2: 151 checks, encerrado às 18:19:56 UTC | Development de 18:14:56 UTC; SHA256 `817a2d6d5e664c8a19c4d59b5712c2e3715748c58b1703c46a93ff19a97b59ce`. |
| Limpeza de saves após encerrar processo: passou às 18:25:13 UTC | Development de 18:24:06 UTC; SHA256 `8fc27ac6c5cfca17b2572ec508434265c1d00dbda6db96902093e317010824b1`. |
| Player comum atualizado | 18:26:29 UTC / 15:26 Brasília; mesmo SHA256 da linha anterior; 222.281.416 bytes; zero erros. `Build/Capitulo2/SantaRita.exe`. |

Após os percursos completos, as diferenças finais de código foram a persistência explícita da limpeza do namespace de QA, sua prova curta e a exclusão do simulador de testes da Sinuca no Player comum. Não houve mudança posterior das regras, diálogos, mapa ou física da campanha. O relatório de inicialização inclui inspeção Cecil da assembly final: ferramentas/fixture/simulador ausentes, áudio e geografia presentes.

Seis chaves residuais, pertencentes ao namespace exclusivo de QA `c88f68d6ee354f52a3a0eea527095e72`, foram preservadas em backup local e removidas. Após a execução curta corrigida, o Registro do player e do Editor apresentou **zero chaves SRQA**. Os cinco arquivos pessoais de save mantiveram os hashes anteriores. A limpeza não apagou slots pessoais.
