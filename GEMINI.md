# Memória do Projeto — Santa Rita

## Unity MCP (`unityMCP`)
- O projeto utiliza o pacote **MCP for Unity** configurado por `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs` na porta **`http://127.0.0.1:8088/mcp`**.
- No Antigravity, o servidor `unityMCP` está configurado em `mcp_config.json` (`"url": "http://127.0.0.1:8088/mcp"`).
- Quando o Unity Editor estiver aberto, use `call_mcp_tool` no servidor `unityMCP` (`refresh_unity`, `execute_menu_item`, `read_console`, `manage_scene`, `manage_camera`, etc.) para compilar, executar menus de autoria/QA e inspecionar a cena ao vivo.
- Menu no Unity para reconectar manualmente caso necessário: **Santa Rita → Ferramentas → Conectar Unity MCP local**.

## Centro de Santa Rita (`CentroSantaRita.unity`)
- A remodelagem do setor oeste e do acesso à **Sociedade** é gerenciada pelo script idempotente [CentroOesteRemodelagem.cs](Projeto%20Jogo%202D/Assets/Editor/CentroOesteRemodelagem.cs) (menu **Santa Rita → Centro → Remodelar Oeste e Acesso Sociedade**).
- **Regra visual de vias**: evitar caminhos diagonais no tilemap `Pavimentos do Centro` (que geram serrilhado/escadinha); usar traçados ortogonais em **rua de chão (`Terra`)** alinhados à grade conectando à estrada principal.
- **Vegetação e iluminação**: manter o matagal (`Matagal das ruas/Bosques e matagal oeste`) próximo às casas sem obstruir portas/soleiras, e iluminação com postes (`Remodelagem Oeste · Rua e Detalhes`) e verificação automática de caminhabilidade até as portas.
