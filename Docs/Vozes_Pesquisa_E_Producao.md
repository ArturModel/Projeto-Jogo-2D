# Vozes de Santa Rita — pesquisa e produção

04/11/2026. Pesquisa nas fontes primárias e documentação via Context7. A necessidade é uma voz ficcional juvenil em português brasileiro com entonação gaúcha discreta, espontaneidade e identidade estável entre fitas. Popularidade de repositório não comprova esse resultado.

**Revisão posterior do autor, no mesmo dia:** as duas candidatas Qwen foram rejeitadas por artificialidade e idade percebida inadequada; também faltaram ambiência e Marcus ao fundo. A recomendação funcional abaixo NÃO é aprovação do elenco ou garantia de qualidade. Encenação e bloqueios reais estão na [revisão do quarto](Evidencias/MilaInventario/Revisao_Cena_Quarto.md).

## Escolha técnica

**Caminho técnico estudado e testado: Qwen3-TTS VoiceDesign 1.7B.** Cria o timbre por descrição em português, em vez de selecionar uma locutora adulta e aumentar o pitch. O fluxo oficial permite criar uma referência e depois reutilizá-la no modelo Base para manter a personagem. A adequação funcional não se confirmou nas audições do autor; não tratar como solução vocal escolhida. Código e cartão dos pesos consultados indicam Apache-2.0.

| Recurso estudado | Contribuição real | Limite para este jogo | Decisão |
|---|---|---|---|
| Qwen3-TTS VoiceDesign + Base | Desenhar uma candidata e reutilizar a referência em falas seguintes; português suportado | Suporte a português não garante sotaque gaúcho nem idade percebida. Modelo precisa de recursos de inferência | Duas audições geradas na demonstração oficial; helper preparado |
| Chatterbox Multilingual V3 / pt-BR | Modelo especializado em português brasileiro e clonagem por referência; cartão pt-BR indica MIT | Não resolve a ausência de referência juvenil adequada por simples descrição. Nano/Turbo são opções inglesas, não substitutos para nossas fitas | Alternativa para comparar depois de obter referência própria/licenciada |
| Skill oficial `openai/skills/speech` | CLI de fala em lote e direção por instruções | A skill trabalha com vozes prontas, declara criação de voz personalizada fora do escopo e exige chave OpenAI separada | Estudada; não instalada como solução da Júlia |
| Skills comunitárias Qwen (`takashi728`, `mdddj`) | Automatizam as APIs locais do modelo | Uma pressupõe NVIDIA e caminhos Unix; a outra parte do Base 0.6B, que exige referência. Nenhuma acrescenta um modelo vocal melhor por ser skill | Não instalar cegamente nem afirmar que corrigem nosso hardware |
| Audio Lens / Audio Forge | Análise local de WAV, silêncio, pico e clipping, com testes conhecidos | Não avalia atuação, sotaque, idade ou inteligibilidade. Não gera a voz | Skill instalada e testada, somente o componente de análise |

Audio Forge também contém síntese baseada em MLX/Fish para Apple Silicon e geração musical. Não foram instaladas essas partes: não atendem diretamente ao computador atual nem ao problema de elenco.

## O que foi executado

- Conferência do computador: Ryzen 7 5700U, cerca de 9,8 GiB de RAM utilizável e Radeon integrada; sem NVIDIA/CUDA detectada. Não instalar vários GB de pesos durante o Unity para depois presumir desempenho aceitável. Inferência CPU não foi medida.
- Demonstração oficial `Qwen/Qwen3-TTS`, modelo VoiceDesign, API pública: duas submissões com a mesma fala e direção de adolescente ficcional gaúcha, sem envio de documentos, mídias ou chave ElevenLabs. Ambas retornaram WAV. Serviço público é de demonstração, com disponibilidade/cotas próprias; não é backend garantido de produção.
- Primeira amostra: 10,62 s, mono PCM16, 24 kHz. Audio Lens: silêncio abaixo de -60 dBFS em 10,89% das amostras, RMS médio -14,71 dBFS, um sample em escala máxima. O detector não classificou clipping (limiar próprio >1%); o pico de 0 dBFS exige margem na masterização. Isto não comprova ausência de distorção audível.
- `Audio Lens` instalado em `~/.codex/skills/audio-lens` via instalador de skills, commit fixado `5f6ba48292892839bd67232c539fed517e9b763d` de `bman654/audio-forge`. CLI/imports revisados; lê WAV/grava imagens solicitadas, sem chamadas de rede na análise. Self-test: **12/12 PASS**. Disponível para descoberta a partir do próximo turno.
- Skill de áudio do projeto ampliada com critérios de elenco e audição. Não foi instalado modelo local Qwen ou Chatterbox; não houve compra ou nova assinatura.
- ElevenLabs permanece autenticado por configuração privada fora do repositório. VoiceDesign da conta respondeu 403/feature_not_available. A Jessica foi rejeitada pelo autor e removida da seleção automática. Não contornar restrições do plano.

Prévia privada: `C:/Users/Usuario/.santarita/voice/previews/julia-qwen-candidata-01.wav`. Segunda audição e metadados são produzidos pelo helper `Tools/Audio/qwen_audition.py`. Nenhuma foi copiada para Resources nem declarada elenco final.

## Direção e critério de escolha

As fitas de 1990 mostram amigos adolescentes; a Bíblia fixa Marcus aos 16, mas não autoriza inventar idade numérica exata para Júlia. Júlia tem iniciativa e curiosidade técnica, sem voz de locutora, caricatura infantil, sedução ou monólogo sobre a tragédia. Lucas brinca e Marcus é mais reservado. Gaúcho aparece na construção das frases e ritmo, sem inserir “bah” em todas as falas. Manter a redação compreensível e sem spoilers futuros.

Comparar candidatas **sem ruído ou filtro** usando a mesma fala: conversa casual, interrupção, riso curto e trecho preocupado. Verificar dicção, idade percebida, sotaque brasileiro/gaúcho, pausas e estabilidade em pelo menos três falas. Depois selecionar uma referência, reutilizar sua identidade, registrar modelo/direção/referência e produzir falas por personagem. Não inventar escuta subjetiva a partir de espectrogramas.

Somente depois: alinhamento real das legendas, montagem de diálogo, distância ao microfone, foley e desgaste de cassete leve. Pitch-shift e chiado não corrigem um elenco errado. As quatro fitas atuais continuam em modo de transcrição; a dublagem e sincronização final ainda não estão concluídas.

## Fontes verificadas

- [Qwen3-TTS oficial e fluxo VoiceDesign → Base](https://github.com/QwenLM/Qwen3-TTS)
- [Pesos VoiceDesign e licença](https://huggingface.co/Qwen/Qwen3-TTS-12Hz-1.7B-VoiceDesign)
- [Demonstração oficial usada](https://huggingface.co/spaces/Qwen/Qwen3-TTS)
- [Chatterbox oficial](https://github.com/resemble-ai/chatterbox) e [modelo pt-BR](https://huggingface.co/ResembleAI/Chatterbox-Multilingual-pt-br)
- [Skill oficial Speech](https://github.com/openai/skills/tree/main/skills/.curated/speech)
- [Skill Qwen comunitária local GPU](https://github.com/takashi728/qwen3-tts-cli/blob/master/SKILL.md) e [skill Base 0.6B](https://github.com/mdddj/qwen3-tts-0.6b/blob/main/SKILL.md)
- [Audio Forge / Audio Lens](https://github.com/bman654/audio-forge/tree/5f6ba48292892839bd67232c539fed517e9b763d/skills/audio-lens)
