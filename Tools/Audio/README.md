# Vozes de Santa Rita — ElevenLabs

Chave em `%USERPROFILE%/.codex/private/santa-rita-voice.json` ou variável ELEVENLABS_API_KEY. Nunca copiar a chave, configuração privada ou dados de conta ao repositório/Player. O catálogo das fitas é fonte dos textos.

`python Tools/Audio/voice_pipeline.py --sample` valida configuração sem chamar rede/consumir créditos.
`--list-voices` consulta apenas a lista da conta. `--sample --generate` gera prévia local da Júlia; `--tape fita01 --generate` produz diálogo somente quando todos os papéis possuem voice_id escolhido.

As prévias ficam em `%USERPROFILE%/.santarita/voice/previews`, não em Assets. Ouvir e aprovar antes de integrar. Duração e alinhamento acompanham as respostas; gravação completa exige seleção consistente dos papéis e sincronização das legendas com os timestamps reais antes da publicação.

Autenticação e geração de primeira prévia confirmadas em 04/11/2026. Criação personalizada de voz respondeu feature_not_available/403 (exige plano pago); nenhuma assinatura comprada. A candidata padrão Jessica foi rejeitada pelo autor por soar adulta/artificial e removida da seleção automática privada. HeyGen foi encontrado como alternativa de conexão, não instalado nem conectado. Não é requisito para a integração REST já configurada.

Documentação oficial consultada via Context7: https://elevenlabs.io/docs/api-reference/text-to-speech/convert-with-timestamps e https://elevenlabs.io/docs/api-reference/text-to-dialogue/convert-with-timestamps . Código de produção e credenciais ficam fora do Player.

## Audições Qwen e medição

`python Tools/Audio/qwen_audition.py` mostra texto/direção sem rede. `--generate` envia somente essa audição ficcional à demonstração pública oficial Qwen; `--resume` recupera o último trabalho sem nova submissão. Sem chave ElevenLabs, compra, upload de arquivos do projeto ou instalação de modelos. O serviço público pode exigir login ou limitar GPU; não é backend de produção nem garantia de disponibilidade. Resultados/metadata ficam no mesmo diretório privado de prévias, sem publicação em Assets. Uma candidata de 10,62 s foi gerada por esse caminho.

Audio Lens instalado em `%USERPROFILE%/.codex/skills/audio-lens`, revisão `5f6ba48292892839bd67232c539fed517e9b763d` do repositório `bman654/audio-forge`. Executar `node <pasta>/bin/audio-lens.js self-test --json` e `analyze <wav> --clips --silence-ratio --peak --rms`. Self-test local: 12/12. Não mede sotaque, atuação ou idade percebida. Comparação e critérios: `Docs/Vozes_Pesquisa_E_Producao.md`.

## Cena completa no quarto — revisão do autor

As duas audições Qwen anteriores também foram rejeitadas. A prova precisa de interação e espaço, não apenas TTS isolado. `quarto_scene.json` mantém elenco, texto, posição, volumes e fontes CC0. `tape_scene.py --dry-run` informa o que falta. Para renderizar: `uv run --no-project --python 3.12 --with numpy --with soundfile python Tools/Audio/tape_scene.py`. Entrada padrão privada: `%USERPROFILE%/.santarita/voice/room-pilot`. Exige tomadas reais dos três papéis: julia.wav/lucas.wav/marcus.wav, nunca substitui ausência por voz rejeitada ou tom sintético.

`--ambience-only` produz explicitamente `quarto-base-sem-vozes.wav`, não uma cena concluída. Base de 14 s renderizada com gravação real de interior, cadeira e botão de cassete, fontes/CC0 e modificações no JSON. Para este estudo usaram-se prévias públicas HQ licenciadas, não os masters originais. Mix de atores usa atenuação/filtro/reflexões curtas para distância, sem pitch-shift. Esses parâmetros não comprovam naturalidade.

Nesta revisão, Qwen gerou uma nova tomada da Júlia, mas falhou em Lucas (tentativa e uma recuperação) e Marcus (uma tentativa); resultado SSE `event: error / data: null`, causa não informada. Não presumir quota ou censura. Sem insistência indefinida ou contorno por réplicas. AI Studio autenticado foi conferido pela UI; Voice Design exige chave de nível pago. Nenhuma chave criada, conta/assinatura contratada ou voz publicada. O bloqueio atual é elenco vocal adequado e acesso efetivo à geração; a ambiência está preparada separadamente.
