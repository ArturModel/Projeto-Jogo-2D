# Configurações de áudio compartilhadas entre menu e sinuca

Autor: @BBCONTTI (git config user.name), execução Codex. Data: 06/11/2026, America/Sao_Paulo.
Ambiente: Windows / C:/Temp/SantaRita / Unity 6000.6.0f1 / melhoria-minigames.
Status: implementado; compilação, harness isolado e fixture nativa Unity PASS. Audição subjetiva e Player independente não executados.

## Pedido

Música ainda excessivamente alta, efeitos baixos, configurações aparentando voltar ao máximo entre menu e sinuca e slider da sinuca sem efeito audível.

## Contexto e diagnóstico

Consultados AGENTS, memória/cofre (índice, estado, visão geral, catálogo e cenas), direção de áudio e skills santa-rita-audio/qa. Graphify query não disponível; confirmação em AudioSettingsController, MenuPausa, MainMenu.unity, gameAudio.mixer, MenuAmbiente, Capitulo1Director e fontes de áudio reais. Nenhuma alteração baseada somente nos índices.

Instância Projeto Jogo 2D@e163f99d040b4865 confirmada pelo recurso project/info como C:/Temp/SantaRita/Projeto Jogo 2D. Editor em Play/MainMenu. Inspeção nativa identificou música salva em 0 e slider do MainMenuManager em .15633437; efeito salvo .187581256. O painel só lia preferências em Start: reabrir ou trocar outro painel deixava a representação desatualizada.

AlterarVolumeMusica/Efeitos aplicava somente o mixer de uma instância. O menu de pausa construído por código aceita mixer nulo, e o treino aberto sobre MainMenu continua usando a música roteada pelo mixer do menu. Essa combinação não era coberta pelo volume .23 da trilha do bar. Na inspeção específica o painel PausaUI tinha mixer vinculado; não se presume que estava nulo naquela reprodução. A fixture nativa confirmou a correção também no caso sem mixer local.

Havia ainda migração audio_init_v2 que elevava preferências menores que .15 para 1 e aplicação duplicada de master (AudioListener × mixer), fazendo o mesmo ajuste soar diferente entre menu e fontes diretas.

## Plano executado

- [x] Defaults somente para chaves ausentes: geral 1, música .35, efeitos 1. Removida migração que elevava volumes baixos/zero. Valores existentes preservados.
- [x] Vinculação/sincronização imediata e em OnEnable, atualização por evento entre painéis ativos e recarga ao reabrir. Listeners próprios removidos antes de religar; zero permitido também nos sliders legados.
- [x] Cada alteração atualiza todos os mixers carregados e as preferências, com persistência imediata. Geral aplicado somente em AudioListener; master do mixer em ganho unitário. Não limpar AudioListener.pause ao ajustar volume.
- [x] Menus com música no mixer usam ganho de base .5 e .35 adicional enquanto a sinuca estiver aberta; Update detecta abrir/fechar inclusive no treino MainMenu. O ganho não modifica o valor escolhido/exibido, nem reinicia a faixa. Atualização inicial de mixer adiada ao primeiro Update do painel.
- [x] Fonte direta do bar durante sinuca: .23 → .10, mantendo fade existente. Taco .23–.54 → .28–.66 e bolas .56 → .70. Outras sínteses/contagem de vozes preservadas.
- [x] Testes executados com preferências isoladas e fixture nativa com restauração em finally. Sem cenas salvas/regeneradas, saves de campanha acessados, commit ou push.

## Arquivos

- Projeto Jogo 2D/Assets/Scripts/AudioSettingsController.cs
- Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs
- Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs
- Tools/QA/verify-audio-settings.py
- Docs/Evidencias/SinucaMecanicas/audio-global-qa.txt, audio-global-unity.json, compilacao-audio-global.txt, graphify-audio-global-{refresh,validate}.txt
- Plano e memória/cofre de arquitetura.

Alterações anteriores de arte, mãos, física e UI no workspace preservadas. Não foram criados novos arquivos de áudio; somente ganhos de sínteses/trilhas já registradas foram alterados.

## Validação

**22 PASS** no C# completo do controlador com doubles isolados: defaults, preservação de zero/valores baixos, carregamento menu/pausa inativa, sliders reais do harness, mixer global mesmo sem referência local, sincronização nos dois sentidos, mute música/efeitos, reabertura, ausência de callbacks duplicados, ganho de treino/retorno, preferências não alteradas pelo ducking, geral único, pausa preservada, limites e persistência/troca de cena.

**8 PASS nativos em Play/MainMenu**, por execute_code na instância confirmada: fixture com Slider/AudioSettingsController reais e mixer local nulo carrega preferência; ajuste de música muda o gameAudio real; menu ativo sincroniza; zero persiste e leva mixer a -80 dB; geral .4 chega ao AudioListener e mixer master permanece 0 dB. Preferências e estado AudioListener.pause restaurados em finally, objeto temporário destruído. A fixture não abriu a sinuca nem altera a campanha; o caminho contextual do treino foi coberto pelo harness. Resultado bruto em audio-global-unity.json.

Assembly-CSharp final: **0 erros / 17 avisos CS0649 preexistentes**. Console Unity consultado sem erros após recarga. git diff --check dos três scripts passou.

Graphify refresh executado: falha CLI ausente. Validate executado: falha hashes desatualizados. Logs preservados, índices não falsificados. Nenhuma alegação de audição humana ou build Player nesta revisão.

## Pendências

Conferir equilíbrio percebido pelo autor e reproduzir na build final. Restaurar disponibilidade do Graphify para regenerar índices. Não é necessário mudar saves ou progressão.
