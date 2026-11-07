# Sinuca, eventos, console e janelas

Autor @BBCONTTI. Execução Codex.
Data 07/11/2026, America/Sao_Paulo.
Ambiente Windows, Unity 6000.6.0f1, branch melhoria-minigames.
Status testado pelo autor e autorizado para commit e push de todas as alterações locais.

## Pedido

Substituir água no cano e eco da festa por apagão e Zé assombrado. Adicionar console por aspas com comandos e botões, batidas na janela com investigação opcional, vistas laterais coerentes com o exterior e vidro com profundidade e chuva animada. O autor solicitou incluir também todas as outras alterações locais no commit.

## Contexto e execução

Consultados AGENTS.md, memória, notas manuais, Bíblia R1, Lore_Atual, direção de produção e skills locais de narrativa, arte, áudio e QA. Fonte de implementação conferida em SinucaMinigameUI, SinucaRoom3D, agendador, bocha e sistema de chuva. Instância Unity confirmada em C:/Temp/SantaRita/Projeto Jogo 2D.

- [x] Cinco eventos na bolsa aleatória, com exclusão de apresentação e comandos manuais.
- [x] Apagão com troca das posições de todas as bolas ativas e falas de Marcus e Zé.
- [x] Zé assombrado fora do quadro, reação ao vê-lo, impacto, piscada, coração e tremor até concluir a próxima tacada.
- [x] Console com event_apagao, event_ze, event_batidas, event_bola_fantasma e event_janela, comandos equivalentes aos botões.
- [x] Batidas no vidro, pista espacial, E próximo da janela e marca de mão temporária. Ignorar permite continuidade do sorteio.
- [x] Atlas único de duas janelas laterais com fundo e primeiro plano, textura sem chuva pintada, reflexos úmidos e paralaxe por ângulo.
- [x] Shader com chuva exterior e gotas no vidro. Um quad por janela, duas leituras de atlas e animação na GPU. Intensidade reduzida com a preferência de efeitos.
- [x] Procedência de arte e áudio registrada em ASSET_SOURCES.csv.
- [x] Integração das alterações locais de bocha, sala, decoração, transições e áudio de chuva conforme autorização explícita do autor.

## Arquivos

SinucaMinigameUI.cs e parciais Perception, Sustos, Console e Janela; SinucaPerceptionSchedule; SinucaRoom3D e parciais Opponent, Salon e Perception; MinigamePerceptionAudio compartilhado com bocha; sprites de Zé e marca no vidro; atlas JanelasLateraisExterior e shader JanelaMolhada; teste do agendador, lore e procedência.

Também incluídos os arquivos locais de AudioSettingsController, Chuva, ChuvaMinigame e áudio, SantaRitaBarAudio, SceneFader, componentes de bocha e diretor, decoração, ferramenta Aseprite e capturas BochaSalaEntrada.

## Validação

- Autor confirmou teste no jogo e solicitou commit e push com todas as mudanças locais.
- Compilação dos scripts com referências reais Unity passou com zero erros e três avisos preexistentes do harness e bocha.
- Agendador passou 4.323.887 verificações com 1.000 sementes e 25.000 eventos, incluindo comandos manuais.
- Física da sinuca passou 136 verificações no harness numérico.
- Unity confirmou shader suportado, sem mensagens de compilação, atlas RGBA 384x248 com Point e um nível de mip.
- Graphify refresh falhou porque graphify não está instalado. Validate acusou índice antigo com scripts ausentes. Índice automático permanece pendente.

## Encerramento

Commit e push autorizados na branch atual. Nenhum passo adicional de implementação solicitado.
