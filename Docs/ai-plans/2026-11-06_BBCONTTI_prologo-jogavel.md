# Prólogo jogável — cabine, estrada, chuva e chegada

Autor: BBCONTTI (identidade Git local; execução Codex). Data: 06/11/2026, America/Sao_Paulo. Ambiente: Windows, Unity 6000.6.0f1, branch `feature/prologo-jogavel`. Status: concluído; verificação final pelo Play/MCP, imagem ausente e console sem erros.

## Solicitação
Transformar a introdução em vídeo em uma viagem interativa e tranquila. Arte detalhada em camadas, volante com deslocamento pequeno na faixa, olhar e objetos do carro. Revisão posterior: integrar estrada/chuva à paisagem e tornar a parada coerente com o exterior seguinte. Usuário prefere testar pelo Play do Editor; builds extras não devem acompanhar cada ajuste.

## Diagnóstico
Memória persistente, visão arquitetural, catálogo, fluxo C1/C2, Bíblia R1, direção de arte e skills locais de pixel art/narrativa/áudio/QA consultados. Graphify consultado após disponibilização local da versão 0.9.96 em C:/Temp. Código real confirmou que `PrologoVideo` estava ativo e o protótipo em camadas estava inativo. Vídeo de 10 segundos inspecionado em quadros. A chegada real é `EntradaDaCidade`, spawn `saida_carro`, no recuo da Zona1_Rodovia; não é a igreja nem a casa de Adílio.

## Execução
- [x] Criar branch a partir da main sincronizada.
- [x] Gerar cabine, volante/mãos, paisagem e objetos como imagens independentes.
- [x] Implementar direção sem colisão/derrota, olhar limitado, rádio, limpadores, faróis, telefone e porta-luvas opcionais.
- [x] Integrar à cena preservando GUID da cena, vídeo de referência e fluxo de chegada.
- [x] Corrigir CanvasRenderer ausente e fornecer BoxCollider2D concreto aos objetos antes de AddComponent.
- [x] Revisar asfalto com textura ancorada ao movimento, bordas graduais e pintura desgastada.
- [x] Separar chuva exterior de gotas aderidas e limpeza local pelas palhetas.
- [x] Aproximação pela própria estrada, entrada no recuo sem imagem de mobiliário, contravolante, desaceleração, desligamento e pausa antes de sair.
- [x] Registrar fontes raster/procedência e atualizar índices estruturais.

## Arquivos
`PrologoJogavel.cs`, `PrologoEstradaGraphic.cs`, `PrologoRuntimeVerification.cs`, `Editor/PrologoJogavelVerification.cs`, recursos `Resources/PrologoJogavel/*`, cena `Prologo.unity`, `AudioAmbiente.cs`, manifesto de assets, fontes e documentação. Código de build permanece explícito e não roda ao abrir o projeto.

## Validação e limites
Compilação direta Roslyn passou antes da primeira execução; Play/MCP confirmou a correção dos componentes e console sem exceções na rodada posterior. Primeira build Development realizou 25 checks funcionais PASS; cobre direção, olhar, objetos, pausa, foco, chegada e salto deliberado. Não usar capturas pretas do player batch como prova visual. Capturas do Editor são as evidências visuais reais, com reposicionamento do relógio declarado nas fixtures. Build inicial completa teve logs de duplicação de luz global nas cenas de campanha; resultado Succeeded não equivale a zero mensagens de erro.

Refresh Graphify: PASS, 116 fontes C# cobertas. Verificação final pelo Play: fixture aos 56 s confirmou velocidade zero, deslocamento lateral para o recuo e ausência da imagem removida; console sem erros. Resultado e limites no relatório de evidências. Sem gravação em slots pessoais; estado de campanha alterado apenas em RAM. Não afirmar campanha integral nem audição subjetiva por estes testes.

## Pendências
Revisão visual pelo autor no Play do Editor. A rodada inicial de 25 checks é evidência histórica funcional, não teste novo desta remoção de imagem. Novas iterações devem priorizar Play, conforme preferência expressa nesta conversa.

Decisão posterior do autor (06/11): remover do prólogo a imagem de placa/banco/orelhão, que parecia uma plataforma na estrada. Somente a estrada e um recuo lateral discreto permanecem visíveis. O carro encosta, desacelera e para antes da transição; os marcos são vistos apenas na próxima cena. A imagem descartada foi arquivada fora de Assets, em Arte/Producao/PrologoJogavel/Descartados. Testes visuais devem priorizar Play, sem nova build por este ajuste.
