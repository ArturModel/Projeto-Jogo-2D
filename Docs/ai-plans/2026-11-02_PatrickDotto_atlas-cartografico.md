# Atlas cartográfico — execução

Autor: @PatrickDotto (Patrick Dotto). Data: 02/11/2026. Ambiente: Windows, Unity 6000.6.0f1. Status: implementação e validação técnica concluídas; avaliação artística pelo autor.

## Solicitação
Reconstrução integral do mapa do jogador baseada no mundo construído, com zoom por camadas, marcadores precisos, descobertas, interiores e diário/fichas preservados. Especificação integral em [Atlas_Solicitacao](../Atlas_Solicitacao.md).

## Diagnóstico e decisões
Consultados índices/memória do repositório, notas manuais Obsidian, query Graphify MapaInterativoUI/SantaRitaGeography, produção/skills, geografia e fontes reais. O mapa anterior dependia de JPG/PNG histórico, UVs aproximadas, descrições antecipadas e GPS em linha reta. C1 tem zonas separadas na mesma cena; C2 não deve herdar a transformação daquela imagem.

## Plano executado
- [x] Confirmar MainMenu, sem Play/cena suja antes da fixture; hashes dos saves fora do repositório.
- [x] Modelo de regiões com limites locais e no atlas; transformação inversível, portas/spawns extraídos.
- [x] Autoria explícita a partir das cenas em Play, sem builders/salvar cenas.
- [x] Camadas por escala a partir da arte real, sem download de arte comercial.
- [x] Interface de atlas, zoom, navegação, plantas, visitas RAM/flags e anotações derivadas de pistas.
- [x] Concluir levantamento, revisar arte e costuras.
- [x] Executar regressão Unity/Player, capturas e build normal.
- [x] Refresh/validate Graphify; encerramento com limites explícitos.

## Arquivos
SantaRitaAtlas.cs (domínio), SantaRitaAtlasView.cs (camadas/UI), MapaInterativoUI.cs (integração), SantaRitaAtlasAuthoring.cs (Editor), AtlasRuntimeVerification.cs e seleção no runner existente. Resources/Atlas recebe levantamento reprodutível e imagens derivadas dos assets atuais. Não mudar movimento, minigames, áudio ou cenas aprovadas.

## Evidência e pendências
Docs/Evidencias/Atlas. Não há aprovação artística automática. A primeira tentativa de levantamento identificou campos privados não serializados em TransicaoDeZonaTrigger; leitura corrigida para reflexão explícita. Nenhum save usado para teste.


## Resultado final e encerramento
237 verificações + PASS no Editor e no executável em cada rodada final; foco na Prefeitura com nota contextual, Igreja/zoom 12x, Sociedade/margem, Cemitério, C1 e plantas. Builds QA/normal Succeeded com zero erros e mesmo fingerprint (ver relatório). Cinco snapshots pessoais C1/C2 importados em isolamento e seis arquivos preservados por SHA256. Nenhum .unity ou configuração do projeto alterado. QA/autoria ausentes do Player normal.

Refresh/validate PASS (116 C#), diff --check PASS, alterações não staged na branch codex/atlas-cartografico. gh indisponível no PATH; autoria usa identificação Git local Patrick Dotto, normalizada como PatrickDotto no nome do plano.

Arquivos de produção: quatro novos scripts (modelo, view, autoria e QA), três scripts existentes integrados (MapaInterativoUI, Capitulo2Director e seletor do runner), Survey.json, 57 PNGs derivados e respectivas metas; ASSET_SOURCES atualizado. Docs, índice Graphify/Obsidian e evidências atualizados. Nenhum commit/push/merge/publicação.

Não há pendência de implementação registrada nesta etapa. Aprovação estética/campanha legítima completa e ensaio subjetivo em outras resoluções são avaliações distintas; não foram presumidos. Limites geográficos das cartas, compatibilidade legada e método de reprodução em Docs/Atlas_Arquitetura.md e Docs/Evidencias/Atlas/Relatorio.md.

Smoke adicional no Player normal: seleção do save C2 e atlas por mouse, centralização/arraste/Fichas em 1536x864; seis saves novamente íntegros. Limite de teclas injetadas/troca de foco documentado no relatório. Skill computer-use aplicada.
