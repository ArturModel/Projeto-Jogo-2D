# Contexto do projeto

Santa Rita — A Noite das 03:17: exploração/investigação e terror psicológico 2D, interior do Rio Grande do Sul em 2004. Marcus retorna no próprio carro após Adílio parar de atender no mesmo dia. Preservar cânone R1, suspense e limites de conhecimento de cada morador. Não antecipar revelações de Lucas.

C1 e C2 até Bocha/consequência estão implementados; capítulos são organização da produção de um mundo conectado. Escola permanece adiada. Ver [estado atual](CURRENT_STATE.md).

Stack confirmada em `ProjectSettings/ProjectVersion.txt` e `Packages/manifest.json`: Unity 6000.6.0f1, URP 17.6.0, Input System 1.20.0, Aseprite importer 6.0.0, Tilemap/Animation. Usar a versão do projeto; não presumir compatibilidade de outra versão. Mundo de referência 32 PPU, câmera 512×288 e UI 640×360 com exceções legadas documentadas. Marcus e referência ambiental orientam a arte.

`Projeto Jogo 2D/` contém Assets, Packages e ProjectSettings. `Arte/` e `Tools/Aseprite/` guardam autoria fora do Player. `Docs/` guarda produção/evidências; `docs/ai-memory`, `docs/architecture-vault` e `graphify-out` são índices compartilhados. Preservar os dois conjuntos de caminhos na indexação Git; Windows não distingue Docs/docs no disco.

Builds e QA usam os verificadores existentes em Assets/Editor e harnesses condicionados a Editor/Development. Saves pessoais não são fixtures. Resultados, datas e limites estão nos relatórios; não afirmar validação nova a partir deles. Não regenerar cenas para atualizar o grafo.
