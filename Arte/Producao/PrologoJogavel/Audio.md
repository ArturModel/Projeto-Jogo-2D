# Áudio do percurso rural — 06/11/2026

Gravações reais licenciadas individualmente como CC0 1.0, conferidas nas páginas originais em 06/11/2026. Os arquivos usados são as prévias HQ MP3 públicas, convertidas para PCM estéreo 44.1 kHz; não são downloads dos WAV originais sem compressão.

- `MotorInterior.wav`: “Car Engine interior”, NachtmahrTV, https://freesound.org/people/NachtmahrTV/sounds/556692/ — https://creativecommons.org/publicdomain/zero/1.0/. Fonte pública https://cdn.freesound.org/previews/556/556692_5620304-hq.mp3. Recorte 6–22 s, remoção de DC, passa-altas 35 Hz/passa-baixas 1500 Hz, costura de 1 s e ajuste de ganho. Loop resultante 15 s. Pico 0.62; RMS 0.1008; salto de amostra na costura 0.00130.
- `RodagemTerra.wav`: “Honda CRV 2014 Interior Slow Gravel Road”, loganzsound, https://freesound.org/people/loganzsound/sounds/784175/ — https://creativecommons.org/publicdomain/zero/1.0/. Fonte pública https://cdn.freesound.org/previews/784/784175_16682330-hq.mp3. Recorte 1–11 s, remoção de DC, passa-altas 130 Hz/passa-baixas 3200 Hz, compressor e costura de 1 s. Loop resultante 9 s. Pico 0.62; RMS 0.0428; salto na costura 0.00076.

Reprodução da preparação: `Tools/Audio/prepare-prologo-loops.py` (ffmpeg e numpy). Manter as fontes de entrada fora do player; informar os dois arquivos por argumentos. Nada é sintetizado por este processo. O veículo gravado não estabelece marca/modelo canônico do carro de Marcus.

O mixer existente é o único proprietário das fontes. O motor substitui o loop anterior na mesma camada; terra acrescenta uma camada cuja intensidade acompanha superfície e velocidade. O asfalto diminui ao entrar na terra; ambos os pneus ficam com alvo zero na parada. Pitch do motor recua na desaceleração. Volume de efeitos, pausa e foco continuam comuns a todos. Ajuste final de naturalidade, equilíbrio e repetição depende de audição do autor no Play; medições não equivalem a aprovação subjetiva.
