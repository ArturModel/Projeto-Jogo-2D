---
name: santa-rita-audio
description: Produzir e integrar efeitos, foley e ambiências de Santa Rita preservando as músicas fornecidas pelo autor.
---

Leia a seção Áudio de `Docs/Direcao_Arte_Narrativa_Audio.md` e os sistemas existentes antes de criar fontes. Identifique o evento real e seu proprietário: passo, colisão, porta, coleta, página, diálogo, tacada ou lançamento. Não duplicar efeitos já implementados em Chuva, Sinuca, orelhão ou Bocha.

Passos de Marcus usam exclusivamente `PlayerController.PeApoiou`, disparado depois de aplicar o quadro de contato 0/4 de `MarcusGait`. Nunca iniciar um segundo relógio de passos. A cadência acompanha a distância real; teleporte, parede e repouso não disparam som. Preservar a ligação ao alterar animações.

Use gravações/bibliotecas com licença comprovada para materiais; registre URL, autor, licença e alterações por arquivo. Síntese original deve ser identificada como síntese e revisada por audição. Suno fica para as trilhas escolhidas pelo autor.

Verifique volumes geral/efeitos, pausa, limite de vozes, transição de região, ducking e continuidade de loops. Passos dependem de distância realmente percorrida e superfície, sem tocar quando Marcus está contra uma parede. Preserve silêncio como elemento de terror. Meça clipping e duração; registre separadamente a audição humana pendente.
