# Registros integrados ao orelhão — entrega e publicação

Autor: @ismaelweinertb — Ismael Weinert. Data: 06/11/2026, America/Sao_Paulo.
Ambiente: Windows / Unity 6000.6.0f1 / feature/prologo-jogavel.
Status: funcionamento validado pelo autor; commit e push solicitados.

## Solicitação e decisões
Substituir o modal de slots por uma vista frontal do orelhão do menu, com aproximação rápida, seleção no aparelho e animação de discagem/gravação. Correção posterior: incorporar fone e cabo à mesma imagem e carregar o save diretamente dessa vista, sem recuar para o menu antes de escurecer. O autor declarou “deu tudo certo, commit e push”.

## Contexto consultado
Memória INDEX/CURRENT_STATE, cofre manual de arquitetura/catálogo/fluxo, Graphify PayphoneCheckpoint, skills locais de pixel art/áudio/QA e ImageGen; código real de PayphoneCheckpoint, CheckpointSystem, SceneFader, MainMenuActions, MenuAmbiente e UIFactory. Imagem original do menu e screenshots do autor orientaram a composição. O destino público origin e as permissões da conta ismaelweinertb foram verificados na publicação anterior; o usuário autorizou expressamente publicação pública nessa branch e agora pediu o novo commit/push.

## Executado
- [x] Produzir a imagem frontal detalhada do aparelho com visor e teclado alinhados à UI.
- [x] Integrar fone e cabo à imagem única, removendo os elementos separados do player conforme correção do autor.
- [x] Usar teclas 1–5 para registros e visor para local/data/estado; manter slots vazios indisponíveis na carga.
- [x] Aproximação de 0,22 s, confirmação visual de discagem/gravação e sons do sistema existente; DTMF curto por tecla confirmada.
- [x] Preservar gravação manual pelo orelhão e seleção explícita de slot; permitir consulta sem saves no menu.
- [x] Manter o aparelho enquadrado durante a carga e usar fade direto de 0,28 s até a cena salva.
- [x] Preservar a assinatura anterior de CarregarSlot e acrescentar overload para transição direta.
- [x] Preservar aviso de progresso não salvo e volume geral/efeitos, inclusive zero.
- [x] Registrar validação funcional expressa do autor e sua autorização de publicação.
- [x] Atualizar Graphify/cofre e validar os hashes estruturais antes do commit.

## Arquivos
Novo PayphoneRegistroUI.cs e meta; nova imagem Resources/MenuAmbiente/OrelhaoFrente.png e meta; alterações em PayphoneCheckpoint.cs, CheckpointSystem.cs, SceneFader.cs, MainMenuActions.cs, MenuAmbiente.cs e MenuAmbienteRuntimeQA.cs. Procedência em ASSET_SOURCES.csv e Arte/Producao/MenuAmbiente/README.md. Documentação e índices atualizados. Estudo de monofone separado foi descartado e mantido localmente fora de Assets; não faz parte do player.

## Verificação e limites
Código compilou no Unity; a última inspeção do console apresentou logs da suíte de QA, sem erro de compilação. A suíte foi ampliada para salvar/carregar com armazenamento isolado e verificar enquadramento/fade. Tentativas em Play não completaram a navegação inicial: 11 verificações, falhas de foco/seleção e NullReference ao procurar submenu não aberto; não há PASS automatizado integral alegado. O estado foi conferido ao encerrar: QA não estava rodando e armazenamento isolado estava desativado. Nenhuma build nova é alegada. O autor inspecionou o resultado e confirmou o funcionamento, autorizando o commit e push. Essa validação humana não é confundida com o resultado da suíte.

## Publicação
Commit de apresentação do menu anterior: 879a90c4, já enviado ao origin. Este pedido publica a integração dos registros e as correções aprovadas, sem force push. Caches TMP, alterações preexistentes de importação do prólogo, configurações alheias e capturas temporárias ficam fora do commit. Uma repetição integral da QA automatizada permanece uma verificação técnica futura, sem impedir o encerramento autorizado pelo autor.
