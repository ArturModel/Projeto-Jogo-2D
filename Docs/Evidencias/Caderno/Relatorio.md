# Caderno e acabamento do mapa — 03/11/2026

## Resultado funcional

- Scroll de Fichas/Legenda e painel contextual não altera o zoom. Caderno tem tela própria; roda/setas/botões viram folhas sem operar o mapa oculto.
- Mapa da Cidade substitui o título Atlas. Entorno com morros baixos, campos, árvores e arroio; reaproveita a arte atual e não muda as 19 regiões/38 acessos.
- Fichas apenas adquiridas, ordem 1/2/3, sem pendentes ou cupom do mercadinho.
- Caderno manuscrito com papel e capa originais Aseprite e fonte Kalam OFL. 64 textos/variantes editoriais + abertura, desbloqueados pelos registros reais; reflexões pessoais, sem novos fatos ou objetivos inventados. Versões condicionais só usam conhecimento adquirido. Registros antigos sem versão autoral são preservados como transcrição identificada.
- Aviso Caderno atualizado após alterações reais; não repete registro idêntico nem carga de save. Dados originais do diário preservados. Marcador de folha no snapshot v2. Nenhum autosave.
- Pequenos rabiscos aparecem com as anotações já descobertas ao aproximar a carta.

## Execução e testes

Skills pixel-art, narrativa, QA e verification-before-completion aplicadas. Fonte licenciada de Google Fonts; licença em StreamingAssets/Licenses/Kalam-OFL.txt. Autoria explícita pelo menu Santa Rita → Mapa e caderno, fora de Play. Não altera cenas ou fontes aprovadas. Documentação Unity 6000.6 consultada via Context7 para conferir o Game View.

Editor: 30 verificações específicas e encerramento PASS; regressão Atlas 237 verificações e encerramento PASS. Player final específico: 30 verificações + PASS; regressão cartográfica: 237 verificações + PASS (antes do último complemento editorial do prólogo, sem alteração no código do atlas). Builds QA/normal em consolidação final; a nota adicional é conteúdo, não nova mecânica.

As primeiras rodadas do Editor encontraram interferência de dispositivos sintéticos remanescentes e janela Game View estreita. Fixture passou a criar dispositivos desde o início e removê-los também ao falhar; vista ampliada para a revisão. Não houve correção por suprimir verificação. As capturas do Player têm 1280×720.

## Preservação e limites

Sessão original estava em Play na EntradaDaCidade. RAM, posição, zona e indicador de progresso preservados em backup externo antes de interromper; restauração final concluída. RAM restaurada idêntica ao JSON original; posição/zona e indicador não salvo restaurados, com mapa aberto. Seis arquivos pessoais de save conferidos por SHA256 sem mudança durante a execução. Os testes usam armazenamento temporário próprio e não gravam slots pessoais.

Capturas e relatórios brutos em EditorQA, AtlasEditor e PlayerQA são evidências locais ignoradas pelo Git, conforme a política anterior de limpeza. Este relatório e plano ficam no repositório. Avaliação artística pelo autor, ensaio subjetivo da escrita e campanha legítima completa continuam distintos do QA técnico.


## Conferência manual adicional

No Player normal sem flags de QA, menu → escolha explícita do slot 1 C2 → mundo → clique no item mapa → CADERNO → Próxima. Carta e livro exibidos corretamente; virada observada. A nota antiga do prólogo apareceu na transcrição de compatibilidade; ganhou texto editorial próprio antes do fechamento. Conferência de IDs literais dos registros ativos não encontrou outro sem versão autoral, além da cronologia que tem projeção especial. Catálogo final: 64 textos/variantes, sem repetição da mesma condição. Não presumir teste de teclado físico: o helper Windows detectou trocas de foco e teclas curtas não produziram encerramento; o processo de teste identificado foi fechado sem salvar. Mouse e Input System do QA são evidências distintas.


## Builds finais

QA e normal Succeeded, zero erros. Mesmo SHA256 de fontes/cenas/dados/texturas/materiais/metas: `59052d449a474eb18124d41d4808935fb762d903b66ace9fd3f0f34e99896d6b`. Player normal em `Build/Capitulo2/SantaRita.exe`. A configuração de produção para não rodar sem foco foi preservada antes das builds finais; o QA liga execução em background somente no processo da fixture. Ferramentas de autoria e runner QA ficam fora do normal (metadados do assembly conferidos). Caderno e fonte estão incluídos.

Graphify refresh/validate: PASS, 120 C#. Índice estrutural não comprova arte/campanha. `git diff --check`: PASS após restauração. Cenas e ProjectSettings sem diff, nada staged. Nenhum commit, push ou publicação.


Estado final: Editor em Play na sessão original, não numa fixture. Seis hashes pessoais reconferidos sem alteração. Nenhuma partida de teste ficou aberta no Player. Avaliação artística pelo autor continua aberta; não há pendência de implementação deste pedido.


### Cache da fonte após restaurar a sessão

Depois das builds, o TextMeshPro escreveu seu cache de glifos em `Assets/Resources/Fonts/VT323-Regular Bitmap.asset` durante a restauração do Editor. É o único asset com escrita posterior às builds finais. Não se descartou a fonte/alterações anteriores do autor para forçar igualdade. O hash completo atual do Assets passou a `cd7dae25bcaede14c0f37f23d171f3c22ace746458908f25faafa91f848afafe`; as duas builds foram produzidas do mesmo estado `59052d44…`. Não houve alteração posterior em C#, cenas, corpus do caderno ou arte. A fonte manuscrita é estática e não sofreu essa escrita. Esta diferença de cache está registrada; não representa uma nova implementação validada por uma build que não a contém.
