# Sinuca — referência 7 / execução

Autor: Patrick Dotto (identidade git), 31/10/2026, America/Sao_Paulo. Branch atualizacao_sinuca, base 30d811c com alterações locais preservadas. Estado: implementação.

Pedido: refazer ambiente conforme imagem 7, caçapas externas/redes conforme foto 3, mesa/bolas/NPCs/móveis coerentes, travadas, mira simples absoluta, efeitos só no disco da branca, bola cantada didática e inventário compacto com ícones. Bandeira do Inter com escudo de 1994.

Consulta: INDEX/CURRENT_STATE, Obsidian arquitetura/catálogo/fluxo, Graphify query SinucaTableView (300 tokens), skills pixel-art/QA/diagnosing-bugs/imagegen, direção e matriz técnica. Código C# confirmado.

Diagnóstico reproduzível: invocar OnPopulateMesh(VertexHelper) real 20 vezes na vista da partida em andamento: média 33,849 ms/reconstrução, 15837 vértices, resolução 1536×672. Arquivo local baseline.json. Esse custo sozinho ultrapassa 16,7 ms/quadro. Snapshot de RAM e versões anteriores fora do repo. Editor estava em Play, cena sem dirty; Play encerrado antes de editar.

Hipóteses ordenadas: (1) reconstrução/ordenação CPU da geometria estática a cada frame; (2) billboards ampliados e perspectiva affine deformam personagens/objetos; (3) câmera segue a própria mira e cria realimentação/inversão no movimento relativo. Substituição por malhas persistentes, câmera real/RenderTexture e ray-plane de mira absoluta.

Plano: [ ] URP 3D dedicado preservando Renderer2D do mundo; [ ] ambiente com planta comum e materiais da referência; [ ] personagens com volume/escala/poses, futebol e bandeira histórica; [ ] mesa, caçapas externas, redes e esferas; [ ] HUD/controles/disco/nominação; [ ] ícones cinco slots; [ ] QA visual e de desempenho + regressão + builds; [ ] índices/documentação/evidências.

Fontes: imagem 7 do autor como referência de direção; dois atlas originais gerados por ImageGen (texturas/placas e itens). Escudo consultado no histórico oficial do Inter, fase 1993–2006 com quatro estrelas: https://m.internacional.com.br/clube/distintivos/copa-do-brasil . Não presumir licença CC0 para marca; atribuir origem em ASSET_SOURCES.csv.
