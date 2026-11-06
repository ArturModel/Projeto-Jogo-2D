---
name: santa-rita-audio
description: Produzir e integrar efeitos, foley, ambiências e vozes ficcionais de Santa Rita preservando as músicas fornecidas pelo autor.
---

Leia a seção Áudio de `Docs/Direcao_Arte_Narrativa_Audio.md` e os sistemas existentes antes de criar fontes. Identifique o evento real e seu proprietário: passo, colisão, porta, coleta, página, diálogo, tacada ou lançamento. Não duplicar efeitos já implementados em Chuva, Sinuca, orelhão ou Bocha.

Passos de Marcus usam exclusivamente `PlayerController.PeApoiou`, disparado depois de aplicar o quadro de contato 0/4 de `MarcusGait`. Nunca iniciar um segundo relógio de passos. A cadência acompanha a distância real; teleporte, parede e repouso não disparam som. Preservar a ligação ao alterar animações.

Use gravações/bibliotecas com licença comprovada para materiais; registre URL, autor, licença e alterações por arquivo. Síntese original deve ser identificada como síntese e revisada por audição. Suno fica para as trilhas escolhidas pelo autor.

Verifique volumes geral/efeitos, pausa, limite de vozes, transição de região, ducking e continuidade de loops. Passos dependem de distância realmente percorrida e superfície, sem tocar quando Marcus está contra uma parede. Preserve silêncio como elemento de terror. Meça clipping e duração; registre separadamente a audição humana pendente.

Para vozes e fitas, consulte `Docs/Vozes_Pesquisa_E_Producao.md`: seleção de modelo, elenco, direção juvenil gaúcha e limites de hardware. Qwen VoiceDesign cria uma candidata por descrição; Base reutiliza a referência selecionada. CustomVoice não equivale a VoiceDesign. Ferramentas de autoria em `Tools/Audio` usam prévias privadas fora de Assets; credenciais nunca entram no Player. A candidata Jessica foi rejeitada pelo autor: não reutilizar automaticamente. Testar timbre sem efeitos de cassete antes de produzir o elenco inteiro; pitch e filtros não substituem atuação. Análises Audio Lens verificam defeitos técnicos, não naturalidade, idade percebida ou sotaque. Registrar modelo, texto, direção, origem/licença e avaliação separadamente; não declarar voz aprovada sem audição do autor.

Correção do autor em 04/11: as duas audições Qwen também foram rejeitadas por artificialidade/idade inadequada. A prova de fita precisa incluir conversa e espaço: Júlia perto do gravador, Lucas ao lado e Marcus audível ao fundo, com ambiência real do quarto e manuseio discreto. Marcus tem 16 anos em 1990; não usar sua voz adulta de 2004 nem inventar idade canônica exata para Júlia. Separar aprovação do timbre seco e avaliação da cena completa; uma não comprova a outra. Use `Tools/Audio/quarto_scene.json` e o renderer como estudo de encenação. Falta de uma voz bloqueia exportação de cena completa; base de ambiente sem atores deve ter esse estado explícito. Chiado, reverberação e pitch não tornam uma voz inadequada em elenco aprovado.
