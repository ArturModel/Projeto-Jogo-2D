# Sinuquinha de quinze bolas, leitura e movimento

Autor: @PatrickDotto — Patrick Dotto (git config; gh indisponível). Execução: Codex. Data: 31/10/2026, America/Sao_Paulo. Status: concluído tecnicamente; avaliação artística permanece com o autor. Ambiente: Windows 11, Unity 6000.6.0f1, Windows x64/D3D11, branch atualizacao_sinuca, base 30d811c com trabalho local anterior preservado.

## Solicitação e decisão atual

A solicitação mais recente substitui a Sinuca Brasileira de sete bolas por sinuquinha com quinze bolas de objeto e branca, lisas/listradas ou identificação numérica, preta por último e branca pronta na saída. Pede bolas maiores/numeradas/rolando, previsão da colorida, órbita completa, retirada do tapete sob a mesa, capacho de entrada, bandeira detalhada, exame legível, pernas mais naturais e objetos em escala coerente.

A alternativa numérica é 1–7 contra 9–15, equivalente aos dois grupos de sete. Não se implementou par/ímpar: tirando a preta 8 isso formaria grupos desiguais. A escolha é explicada antes da saída. Trata-se de regra da casa de bola 8, não alegação de implementação integral de regulamento oficial.

## Contexto e diagnóstico

Consultados INDEX/CURRENT_STATE, notas manuais de arquitetura/cenas/catálogo, Graphify para SinucaMinigameUI/SinucaBrasileiraRules e fontes reais. Skills pixel art, QA, áudio, narrativa e verificação antes de concluir. O estado 2D aprovado continua soberano; a sala em perspectiva compartilha fontes e planta do bar. A nova decisão torna históricos os testes e textos de sete bolas. Identificadas restrição do semicírculo D, marcação no pano, 8 texturas/esferas apenas e tutorial/falas antigas; a luminária ocultada na vista superior não reaparecia por busca que excluía inativos.

## Plano executado

- [x] Kernel próprio SinuquinhaRules, grupos, faltas, branca na mão e preta final.
- [x] Adaptar UI, rack, IA e buffers para quinze bolas + branca; escolha antes da saída.
- [x] Ampliar raio, criar texturas numeradas/listradas, manter quaternions de rolamento e direção prevista da colorida.
- [x] Câmera sem limite angular, controles projetados na vista e remoção das restrições D.
- [x] Retirar tapete sob a mesa; ordenar capacho; ajustar escala/posições de TV e geladeira sem sobreposição na parede.
- [x] Bandeira de exame 256 px, fonte Aseprite e conteúdo tipográfico para documentos, sem inventar pistas.
- [x] Refinar apoio/passagem das pernas, reduzir oscilação do torso e alinhar lanterna; conservar eventos de apoio 0/4.
- [x] Atualizar falas/quadro do Zé para a regra atual sem alterar flags narrativas ou saves.
- [x] QA final Editor/Player, build normal, preservação de saves e refresh/validate.

## Arquivos

Novos: Assets/Scripts/SinuquinhaRules.cs, Capitulo2/Sinuquinha15Verification.cs e metas; Arte/Producao/BarIdentidade2D/Inter1994Detalhe.aseprite e PNG/meta em Resources/Art/Props.
Modificados nesta execução: SinucaMinigameUI, SinucaTableView, SinucaRoom3D e parciais Actors/Table, SantaRitaBarLayout, GameMessageUI, InteracaoCapitulo1, PlayerController, dispatcher Chapter2RuntimeVerification; scripts Aseprite marcus-walk/marcus-flashlight/bar-identidade-2d e respectivas exportações; documentação/índices/fontes de assets. O kernel SinucaBrasileiraRules permanece como histórico, sem controlar a partida atual.

## Validação e limites

Resultados finais em [relatório](../Evidencias/Sinuquinha15/Relatorio.md). Fixtures técnicas não equivalem a campanha legítima completa nem aprovação artística do autor. Não regenerar cenas, migrar saves, reabrir C1 completo ou criar C3. Fontes e índices auxiliares preservam o trabalho anterior.


## Complemento recebido durante a execução

- [x] A/D contornam; setas miram; órbita também na vista superior. Sem botões de contorno.
- [x] Lista de bolas encaçapadas de cada grupo/jogador.
- [x] Linha clara com contraste, seta dourada da colorida e legenda.
- [x] Reativar exceção da marcada em mãos 2/3: X visível, tacada anulada e formação restaurada, sem bloquear a última bola/preta.
- [x] ESC → Carregar Jogo abre cinco slots; cancelamento preserva pausa e escolha carrega o slot explícito.
- [x] Corrigir mesa/cadeiras da calçada identificadas no novo recorte; reutilizar arte do interior.
- [x] Dependências compartilhadas localizadas: SantaRitaBarAudio, SantaRitaRimLight e SinucaTableView recuperam referências após recarga do Editor. Não altera narrativa nem músicas.
- [x] Repetir Editor/Player/normal após o complemento, renovar relatórios e índices.

Preservação: a captura de saves da tarefa anterior estava desatualizada para três arquivos, com gravações anteriores ao snapshot inicial desta execução. Não foram revertidos. Criada uma referência de hashes atual, fora do repo, antes da rodada final; verificar preservação a partir dela.


## Encerramento e evidência final

- Editor: 279 verificações + PASS runtime, 01/11 02:14:04 UTC (31/10 local).
- Player QA: 279 verificações + PASS runtime, 01/11 02:20:10 UTC. Inclui mão de 115 tacadas e callback de série.
- Builds QA/normal Succeeded, zero erros; hash comum 9e83b0af5bfb8c443722d2288673ae456e181ef4d2befe6e26bfd97cde9c6002.
- DLL normal contém SinuquinhaRules e exclui QA/autoria. Smoke normal iniciou/respondia, sem exceções C#; avisos de vídeo/driver estão no relatório.
- Seis arquivos pessoais iguais à referência anterior à rodada final. Snapshot RAM mais recente restaurado campo a campo, no bar; indicador não salvo mantido, pois o conteúdo difere dos snapshots persistidos. Não houve gravação de orelhão pessoal.
- Graphify refresh e validate: PASS, 111 C#; git diff --check: PASS. Sem commit/push. Settings originais de execução em segundo plano restaurados; cena não suja.
- Uma fixture adicional de prática isolou branca/colorida para conferir a seta: objectGuide=True, três vértices na seta; captura guia-clara.png. É uma demonstração visual, sem campanha.
- Após devolver o Editor, somente os caches TMP VT323-Regular Bitmap.asset e LiberationSans SDF - Fallback.asset foram escritos em Assets. Hash da árvore atual c78b6f0b3eded126b1bcb907f3bd8cafed7dc512c84dc6eafb50987636d73d49; código/cenas/sprites não foram alterados depois das builds. Não confundir com uma nova build executada.

Pendências: avaliação artística/audição humana e campanha legítima completa, não presumidas pela suíte. O pico de frame observado impede prometer ausência absoluta de travadas. Não houve expansão para Escola/C3 nem reautoria geral de C1.
