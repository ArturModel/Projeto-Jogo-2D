# Capítulo 2 — procedência e integração de arte/áudio

## Produção desta execução

As novas fachadas, móveis, oito personagens e oito retratos foram gerados para Santa Rita com a ferramenta ImageGen da OpenAI. Não são downloads de packs nem assets CC0. O inventário `ASSET_SOURCES.csv` identifica essa procedência separadamente dos recursos anteriores do repositório.

Direção usada nos pedidos: pixel art vista de cima/3 quartos, fachada voltada ao sul, cidade pequena do interior gaúcho em 2004, materiais modestos e gastos, cores contidas, chuva/noite, iluminação quente localizada. Evitar logotipos, interfaces modernas, elementos sobrenaturais explícitos e texto gerado nos letreiros. Os letreiros legíveis são objetos de texto do Unity.

| Lote original | Conteúdo final | Operações de integração |
|---|---|---|
| Atlas de seis fachadas | Igreja, Prefeitura, Rodoviária, Sociedade Beira-Rio, oficina de rádio, casa de costura | Remoção de fundo pelo próprio ImageGen; corte de atlas, escala inteira/nearest para o projeto, pivô na base |
| Atlas de dezesseis móveis | Bancos, altar, mesa pública, arquivos, guichê, bancada eletrônica, máquina de costura, sepultura, portão, velas, memorial, suporte/placar de Bocha, manutenção, mesa comunitária | Recorte técnico das células; ajuste à escala 32 PPU |
| Atlas de moradores e retratos | Tonão, Bento, Clarice, Alcina, Dario, Mila, Celina, Nair | Personagem em célula 64×64 com 48 pixels úteis; retratos 64×64; Point, sem mipmaps |
| Texturas do editor de autoria | Grama, terra, calçada, asfalto, pedra, água, madeira, pisos e areia | Desenho procedural original 32×32; linhas de eixo somente na faixa central das ruas |
| Atlas de documentos | Protocolo, manutenção, homenagem, registro de visitas, desenho de Júlia e caderno de velas | Seis recortes próprios para exame ampliado; a transcrição legível e rolável é texto da interface, não texto inventado na imagem |

### Reutilização de C1

`Assets/Art/Capitulo2/Legado` contém cópias integrais de 27 PNGs existentes. Cada cópia foi comparada ao original por SHA256. A origem e a atribuição de C1 são preservadas no inventário; essas imagens não são novas gerações nem recebem uma licença CC0 nova.

Esses PNGs tinham importação com vários recortes. Carregar o primeiro sprite retornava somente um fragmento da árvore ou do móvel. C2 usa uma importação independente `Single`, sem compressão, filtro Point e sem mipmaps: 16 PPU nativos para esses objetos, 32 para o NPC. A escala aprovada de Marcus, das novas artes e do projeto continua em 32 PPU.

Separadamente, quinze terrenos de `Resources/Cap1` receberam `isReadable: 1`: o montador aprovado de C1 precisa ler seus pixels também no player compilado. Os PNGs, recortes, PPU e demais parâmetros visuais desses terrenos foram preservados. Essa correção resolve exceções que não apareciam quando o Editor carregava os arquivos diretamente do disco.

Arquivos de trabalho da geração foram mantidos no diretório de imagens da sessão; os recortes usados pelo jogo ficam em `Assets/Art/Capitulo2` e `Assets/Resources/UI/Portraits`. O projeto não depende dos arquivos de trabalho para compilar ou executar.

Os materiais aprovados de C1 continuam com suas origens anteriores no inventário. Nenhuma atribuição ou licença existente foi substituída. Não se atribui autoria humana exclusiva nem licença aberta às imagens geradas.

## Áudio

`Capitulo2Audio` sintetiza três texturas originais de 16 segundos: Centro (motivo discreto), Cemitério (respiração/harmônicos e sino irregular) e Sociedade (sopro e madeira). Loops com entrada/saída suave e troca gradual de região; a chuva continua no componente aprovado `Chuva`. Não são novas faixas musicais licenciadas de terceiros.

Impacto de Bocha: síntese curta disparada por contato físico real; nunca faz parte do loop da Sociedade. Sino percebido da Igreja: evento único após a conversa, com legenda e nota no diário. As configurações globais de música, efeitos e volume geral são respeitadas. No diálogo/Bocha a ambiência baixa para 67% de seu volume configurado.

Os dois WAV fornecidos anteriormente para Bairro/Casa do Avô e Bar do Zé permanecem no fluxo aprovado de C1. Não se estende a música do Bar para a Sociedade.

## Ocorrências e acessibilidade

| Gatilho | Duração | Alteração sensorial | Persistência / gameplay |
|---|---|---|---|
| Encerrar primeiro testemunho de Bento | 3 s de sino / 5,5 s de legenda | Sino sintetizado baixo sem movimento correspondente do cordão | Flag única, diário como percepção sem prova; sem dano ou mudança de pista |
| Ler memorial depois de ouvir Bento | 2,4 s / 5,5 s de legenda | Pulso lento e discreto no tom do memorial, percepção das velas no vidro | Flag única; geometria e conteúdo não mudam |
| Colisão real em mão posterior à primeira | Legenda na própria UI | Resposta abafada descrita pelo impacto | Uma vez por mão; física permanece visível e mensurável |

`Reduzir efeitos intensos` desativa os efeitos sensoriais de mundo; legenda e diário conservam a informação. Nenhum clarão, mudança oculta de atrito, bola invisível ou susto de volume alto é necessário para jogar. Avaliação de tensão/diversão e mixagem por audição humana deve ser registrada como playtest, separada dos testes técnicos.
