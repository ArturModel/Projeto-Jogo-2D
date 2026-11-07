# Sinuca — colisões de bolas gravadas

Autor: @BBCONTTI (BBCONTTI). Data: 06/11/2026, America/Sao_Paulo.
Status: implementação e QA técnico concluídos; audição no jogo pendente.
Ambiente: Windows, C:/Temp/SantaRita, Unity 6000.6.0f1.

## Solicitação e diagnóstico

Autor pediu som das bolas mais próximo da sinuca real, menos arcade. O evento físico chamava uma síntese de ruído com senoides 1931/3277 Hz e pitch variável pela força. Memória, cofre manual, direção de áudio e skills santa-rita-audio/qa consultados. Graphify indisponível; código atual confirmado diretamente.

## Execução

- [x] Selecionar gravação de bolas reais com licença CC0 e registrar procedência.
- [x] Recortar três colisões em WAV mono PCM16/44,1 kHz, 180 ms, com remoção de DC, passa-altas leve e fades; sem tons sintetizados adicionados.
- [x] Substituir a síntese bola-bola por alternância das três gravações, pitch 1. Força continua controlando volume (.70 × raiz da intensidade).
- [x] Carregar recursos no Awake, preservar pausa/foco, categoria VolumeImpactos e pool de seis fontes; limite compartilhado de 18 ms entre colisões.
- [x] Atualizar harness numérico para verificar apenas os quatro eventos que continuam sintetizados.
- [x] Compilar, verificar os WAVs e executar regressão numérica.

## Procedência e reprodução

Fonte: [Pool balls.wav, bsumusictech](https://freesound.org/people/bsumusictech/sounds/62331/), [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).
Insumo: prévia pública HQ MP3 do próprio Freesound, https://cdn.freesound.org/previews/62/62331_791247-hq.mp3 (não o WAV original, que exige login). SHA256 `5d624bd6d82f2585875750d9eeabcb3e1b147b5b6b8d8fb47df485872312cbdc`.

`python Tools/Audio/prepare-sinuca-impacts.py CAMINHO_DA_PREVIA.mp3` (ffmpeg e numpy). Inícios dos recortes no insumo: 8.140522 s, 14.851814 s, 16.990794 s, respectivamente Colisao_01/02/03.wav. Alterações descritas acima; gravação sem voz. Não inclui download em runtime.

## Arquivos

- Assets/Scripts/SinucaMinigameUI.cs: carregamento e despacho dos impactos gravados, remoção do sintetizador bola-bola.
- Assets/Resources/Audio/Sinuca/: três WAVs e metas PCM/descompressão em memória/preload; meta da pasta.
- Tools/Audio/prepare-sinuca-impacts.py: preparo reproduzível e medição do PCM final.
- Tools/QA/verify-sinuca-mechanics.py: remove teste do sintetizador eliminado; WAVs validados no preparo.
- Este plano, memória e nota manual. Evidências locais em Docs/Evidencias/SinucaMecanicas/bolas-reais-qa.json, bolas-reais-mecanicas.txt, compilacao-bolas-reais.txt e graphify-bolas-*.txt.

## Validação e limites

Assembly-CSharp compilou com 0 erros/16 avisos CS0649 existentes, após restore local sem feed remoto. Harness: 45 PASS (antes 46, removido um teste do sintetizador eliminado). Três WAVs distintos: 180 ms, pico 0.71997, extremos zero, sem saturação no PCM individual, DC absoluto < .00013. Isso não comprova ausência de saturação no mix completo ou aprovação de timbre.

Unity MCP retornou zero instâncias; importação/reprodução nativa e Player não exercitados. Audição humana pendente. Graphify refresh falhou por CLI ausente; validate falhou por fontes C# novas ainda não indexadas. Não editar índices manualmente para simular refresh.

Cenas, preferências e saves preservados; _Recovery/0 (4).unity e script concorrente SinucaRoom3D.Opponent.cs não foram alterados. Sem commit/push. Próximo passo: ouvir colisões leves/fortes e abertura no Editor, inclusive com slider de impactos em zero.
