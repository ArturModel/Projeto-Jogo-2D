# Menu integrado — implementação e publicação

Autor: @ismaelweinertb — Ismael Weinert. Data: 06/11/2026, America/Sao_Paulo.
Ambiente: Windows / Unity 6000.6.0f1 / feature/prologo-jogavel.
Status: funcionamento validado pelo autor; encerramento e commit/push autorizados.

## Pedido e contexto
Melhorar o vídeo de fundo do menu ao nível do prólogo, com pixel art detalhada e interações integradas aos botões. Inspecionar o vídeo original para preservar a base. Pedido posterior: commit e push; o autor afirmou que viu tudo funcional e autorizou concluir.

Consultados AGENTS.md, skills locais de pixel art e QA, memória, cofre manual, Graphify MainMenuActions/Menu, código real, MainMenu.unity e vídeo telaPrincipalV2.mp4. Referências visuais e fonte raster preservadas em Arte/Producao/MenuAmbiente. Instância Unity e projeto confirmados antes da inspeção e dos testes.

## Executado
- [x] Examinar sequência do vídeo e preservar composição da rua/clube/orelhão.
- [x] Produzir duas imagens detalhadas com procedência registrada.
- [x] Separar orelhão, chuva, luzes, reflexos e leitura do menu.
- [x] Integrar seleção por mouse/teclado, configurações e atalho no orelhão.
- [x] Preservar callbacks, música, seleção explícita de slots e saves manuais.
- [x] Acrescentar transição de confirmação e inicialização exclusiva da MainMenu.
- [x] Executar teste em Play com eventos InputSystem e armazenamento isolado; conservar relatório real, inclusive falhas.
- [x] Registrar validação funcional expressa do autor e autorização de conclusão.
- [x] Conferir console sem erros/avisos na inspeção de encerramento.
- [x] Atualizar índices estruturais e validar integridade antes do commit.
- [ ] Repetir a suíte automatizada integral e testar player final: não executado antes do encerramento solicitado.

## Arquivos
MainMenuActions.cs, MainMenu.unity (somente enabled/playOnAwake do vídeo), MenuAmbiente.cs, MenuAmbienteGraphic.cs, MenuAmbienteRuntimeQA.cs, Editor/MenuAmbienteAuthoring.cs, respectivos meta, Resources/MenuAmbiente, fontes/referências de arte, ASSET_SOURCES.csv, documentação e índices estruturais.

## Validação e limites
Primeira suíte: 21 verificações PASS e 6 FAIL de 27. Navegação, submenus, brilho, orelhão, modal e preservação do save de teste passaram. A suíte esperava chegada ao prólogo antes do fade existente e apagamento dos slots, divergindo do código real. O roteiro foi corrigido para aguardar a cena e preservar slots; sua sequência posterior também dependia dessas expectativas. Não houve nova execução integral após essa correção. Nenhuma build nova concluída ou aprovação automática integral é alegada. Controle nativo do computador não foi autorizado; input de QA foi injetado no InputSystem dentro do Unity.

O autor declarou: “pelo que já vi está tudo funcional acho que podemos concluir já”. Isso constitui a validação funcional relatada pelo autor, sem substituir o histórico de QA. Caches dinâmicos TMP, metadados preexistentes do prólogo e alterações alheias ao menu foram preservados fora do commit. O push usa a identidade local ismaelweinertb na branch existente; sem merge ou force push.
