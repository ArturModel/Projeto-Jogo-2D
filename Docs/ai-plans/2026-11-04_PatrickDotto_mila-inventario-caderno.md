# Mila, inventário e caderno
Autor: @PatrickDotto / Patrick Dotto (nome confirmado por git config; handle conforme histórico local, gh indisponível para confirmação online). Data/hora: 04/11/2026, fechamento após 15h25 BRT. Ambiente: Windows / Unity 6000.6.0f1, Editor inicialmente fechado.

Status: implementação funcional e QA concluídos; pesquisa e duas audições Qwen produzidas; dublagem definitiva e aprovação de elenco pendentes. Sem compra, commit ou push.

## Solicitação e contexto

Implementar Mila/fitas conforme PDF e proposta; bolsa maior mantendo cinco atalhos e linguagem visual; índice imersivo do caderno e marcador. Depois configurar voz por IA e pesquisar skills/projetos abertos porque a amostra adulta Jessica foi rejeitada. Não prometer naturalidade sem audição. Fontes canônicas, mudanças recentes e saves do autor prevalecem.

Consultados índices persistentes, três notas manuais Obsidian, Graphify direcionado a inventário/caderno/director, Bíblia, proposta Mila, PDF e direção/produção artística. Assinaturas e chamadas verificadas nos fontes reais. Skills narrativa/áudio/pixel-art/QA, Context7, skill-installer/creator e verificação antes de concluir aplicadas. Nenhuma delegação.

## Plano executado

- [x] Bolsa e atalhos sobre flags/textos RAM compatíveis com snapshot v2; fichas apenas mapa.
- [x] Índice manuscrito por conhecimento adquirido e marcador de tecido por ID/parte da nota.
- [x] Mila com reparo/ajuste de velocidade e empréstimo; quatro fitas físicas contextualizadas; aparelho com transcrições e controles.
- [x] Originais Aseprite e arte Point/uncompressed, sem reconstruir cenas.
- [x] Integração ElevenLabs privada e autoria fora do Player; candidata rejeitada retirada; nenhuma compra.
- [x] Pesquisa primária Qwen/Chatterbox/skills e hardware; helper Qwen testado com geração real; duas audições privadas.
- [x] Instalar somente Audio Lens de Audio Forge em revisão fixa, self-test 12/12; ampliar a skill do projeto para vozes.
- [x] QA real Editor e executável, build normal, conferência visual das capturas QA, smoke inicial normal e preservação de slots.
- [ ] Aprovar timbre/atuação/sotaque, gerar vozes definitivas, montar e alinhar fitas aos timestamps reais. Não consta como implementado.

## Arquivos

Novos: `SantaRitaTapeDeck.cs`, `Capitulo2/MilaRuntimeVerification.cs`, `Editor/MilaProductionVerification.cs` e metas; `Resources/Tapes` (catálogo, três PNGs/metas); três fontes em Arte/Producao/Caderno; `Tools/Aseprite/fitas.lua`; `Tools/Audio/{voice_pipeline.py,qwen_audition.py,README.md}`; pesquisa vocal, relatório/evidências e este plano.

Alterados: SantaRitaInventory, MarcusNotebookUI, PlayerController, MenuPausa, MapaInterativoUI, Capitulo2Director, Capitulo2Audio, Investigation.json, Entries.json e ASSET_SOURCES.csv. Dependência compartilhada de autoria: Chapter2BuildVerification agenda saída depois do finally de restauração. Skill santa-rita-audio e índices/notas manuais atualizados. Metadados de cenas e ProjectSettings terminaram sem diff. Nenhuma remoção de trabalho anterior. Propostas/PDF de priorização preexistentes nesta entrada preservados.

Configuração ElevenLabs, amostras, logs volumosos e revisão das skills externas ficam fora do repositório; sem chave ou saves no índice. Audio Lens instalado no perfil do Codex, disponível a partir do próximo turno. Não instalado Qwen local: sem GPU NVIDIA e RAM limitada; demonstração pública não é serviço garantido de produção.

## Validação

32 checks específicos + PASS no Editor e 32 + PASS no Player final. Builds QA/normal Succeeded, zero erros/avisos C# e fingerprint idêntico `8cd24f8373d2232fe1b567396cc659abb1f08de0354b34cd5e124493ff812193`. Capturas reais bolsa/índice/tocador revisadas. Assembly normal sem runner QA/ferramentas Editor. Smoke normal limitado à inicialização, com avisos conhecidos D3D/vídeo; não comprova campanha ou console global sem avisos. Sete arquivos pessoais anteriores à tarefa; hashes iguais antes/depois do smoke, sem baseline retroativo inventado. Testes de persistência em armazenamento isolado.

Skill validada; helpers Python compilam/dry-run e Qwen retorna WAV; Audio Lens 12/12. Primeira tentativa do validador da skill precisou YAML no ambiente isolado e UTF-8 no Windows; execução final PASS. Falhas QA anteriores de foco/restauração de InputSettings foram corrigidas antes dos resultados finais. Detalhes e limites no [relatório](../Evidencias/MilaInventario/Relatorio.md).

Graphify refresh e validate executados: PASS, 123 C# cobertos, caminhos portáveis e hashes atuais; quatro consultas reais e canvas/HTML/relatório válidos. Fingerprint atual de Assets reconferido e idêntico ao das duas builds. git diff --check PASS; nada staged. Derivados Graphify/Obsidian atualizados (muitas notas geradas mudam por relações/linhas, não correspondem a alterações de gameplay).

## Pendências

Dublagem e sincronização final; avaliação artística/sonora do autor e percurso de campanha sem fixtures. Próximo passo vocal é comparar as audições secas e selecionar uma identidade consistente, não produzir lotes com uma voz já rejeitada. Escola/C3 permanecem adiadas.
