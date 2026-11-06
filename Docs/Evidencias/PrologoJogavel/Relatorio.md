# Prólogo jogável — evidências

06/11/2026. Branch `feature/prologo-jogavel`. Fontes e implementação no [plano](../../ai-plans/2026-11-06_BBCONTTI_prologo-jogavel.md).

A primeira cabine foi avaliada positivamente pelo usuário. A revisão solicitada em seguida foca estrada, chuva e estacionamento.

- Referência: vídeo original inspecionado em quadros; mantém-se no projeto.
- Primeira rodada Development: 25 verificações funcionais PASS. Sem saves pessoais; input por dispositivos virtuais. Capturas batch ficaram pretas e não servem como evidência visual.
- Editor/MCP: instância `Projeto Jogo 2D@995a38c4a4c14731`, caminho correto confirmado; CanvasRenderer e colliders corrigidos; console limpo na rodada posterior.
- Capturas em Editor: câmera real, com fixture de tempo explicitada; não são campanha jogada.
- Revisão da chegada: recuo pavimentado foi conferido em `ConstruirZona1_Rodovia`; a imagem com placa/banco/orelhão foi retirada a pedido do autor. Carro para no recuo pavimentado antes da troca para `EntradaDaCidade/saida_carro`.
- Graphify/Obsidian: refresh PASS, 116 C# cobertos.

Verificação final pelo Play/MCP: imagem de chegada ausente, estacionamento completo, velocidade 0 e posição lateral 5,2; console sem erros. Fixture reposicionou o relógio para 56 s. Nenhuma nova build após a preferência por Play e a remoção da imagem. Audição, campanha integral e aprovação artística final não são presumidas.

Decisão posterior do autor (06/11): remover do prólogo a imagem de placa/banco/orelhão, que parecia uma plataforma na estrada. Somente a estrada e um recuo lateral discreto permanecem visíveis. O carro encosta, desacelera e para antes da transição; os marcos são vistos apenas na próxima cena. A imagem descartada foi arquivada fora de Assets, em Arte/Producao/PrologoJogavel/Descartados. Testes visuais devem priorizar Play, sem nova build por este ajuste.
