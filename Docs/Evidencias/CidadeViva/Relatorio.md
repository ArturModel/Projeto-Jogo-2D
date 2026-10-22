# Revisão de cidade viva — iniciada em 21/10/2026, fechamento em 22/10/2026

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
