# Fitas — revisão da cena de quarto

04/11/2026. Autor rejeitou as duas audições Qwen: artificiais, idade percebida errada, ausência de espaço e Marcus ao fundo. Rejeição registrada nos metadados privados e na skill/direção de produção. Nenhum áudio dessas audições foi publicado no jogo.

## Mudança concreta

`Tools/Audio/quarto_scene.json` passa a tratar a prova como uma cena de adolescentes num quarto em 1990. Júlia próxima do gravador, Lucas ao lado e Marcus atrás, mais baixo/abafado mas inteligível. Marcus tem 16 anos na Bíblia; a idade exata de Júlia não foi inventada. Conversa breve, respostas com pouco intervalo, sem voz de locutor, discurso explicativo ou risos artificiais forçados. Desgaste de fita não corrige timbre adulto. Essas são direções de elenco, ainda não vozes aprovadas.

Obtidas prévias públicas HQ de gravações CC0, metadados/licenças conferidos nas páginas originais:

- [House Room Tone — aleclubin](https://freesound.org/people/aleclubin/sounds/641306/): ambiente real de interior. Trecho inicial, ajuste de ganho e fades.
- [Wooden Chair — OverlookHotelRecords](https://freesound.org/people/OverlookHotelRecords/sounds/538961/): evento de cadeira recortado e atenuado. Original é cadeira de madeira em piso duro; não afirmar gravação da casa de Adílio.
- [press button(Cassette player) — 220bluebulb](https://freesound.org/people/220bluebulb/sounds/572138/): clique de gravador recortado e atenuado.

Render offline em `Tools/Audio/tape_scene.py`: base real de quarto/foley, envelopes de entrada/saída, tomadas separadas, posições discretas, filtro e reflexões curtas para distância, margem de pico e manifesto de transcrição. Não usa pitch-shift. Falta de uma tomada impede exportar uma cena completa. Somente `--ambience-only` exporta base identificada SEM VOZES.

Arquivo produzido: `C:/Users/Usuario/.santarita/voice/room-pilot/quarto-base-sem-vozes.wav`, 14 segundos, estéreo PCM16/24 kHz. É um stem de produção baixo para acompanhar diálogo, não áudio final para o jogador. Audio Lens: zero samples clipped; pico -22,69 dBFS; RMS médio do downmix -39,02 dBFS; silêncio abaixo de -60 dBFS em 7,13% das amostras. Métricas não comprovam naturalidade por audição.

## Limite encontrado nas vozes

Uma nova tomada dirigida da Júlia foi gerada, ainda sem avaliação do autor. Lucas falhou na tentativa e em uma recuperação; Marcus falhou na primeira tentativa. A demo oficial Qwen retornou `event: error` e `data: null`, sem informar causa. Não atribuir o erro a quota, filtro ou pagamento sem evidência. Sem insistência indefinida, troca de réplica para contornar limites ou cobrança em outro serviço.

Google AI Studio foi aberto na sessão existente para conferir alternativa. A UI do Gemini 3.8 Flash TTS ofereceu Voice Design, mas exibiu exigência de chave de nível pago. Nenhuma chave criada/vinculada, compra ou contrato aceito. Foi apenas verificação de interface; não foi produzido áudio Google. A assinatura exibida na conta não comprovou acesso à API paga. A aba foi intermediária, sem handoff necessário.

**Cena com o elenco completo não concluída:** ainda faltam vozes apropriadas de Lucas/Marcus, timbre da Júlia convincente e avaliação de atuação. Ambiente foi preparado separadamente. Não usar as audições rejeitadas como substituição nem apresentar esta base como a nova fita final.

## Verificações e preservação

Renderer executado com gravações reais no modo ambiente. Ramo completo testado em fixture explicitamente sintética: três cues, ganho limitado, manifesto não aprovado; sinais de teste nunca apresentados como voz. Exportação real com elenco faltante bloqueada como esperado. Fixture temporária criada nesta revisão removida depois do teste. Python compila e skill validada. Sem mudanças nos Assets/cenas/saves/Player, portanto não foi refeita build Unity sem alteração de runtime. Derivados Graphify/Obsidian atualizados e resultado de integridade no plano correspondente. Sem commit/push.
