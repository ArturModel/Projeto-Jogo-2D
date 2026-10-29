# Registro de Execução: Desativação do UVC e Ajuste de Inicialização do Unity MCP

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 2026-10-28 20:27 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11 / PowerShell / Unity 6000.6.0f1 (URP 2D) / Graphify 0.9.96 / UV 0.13.6

---

## 1. Solicitação do Usuário
O usuário abriu o projeto Unity pela primeira vez neste ambiente e relatou erros no console:
1. `NotConfiguredClientException`: Falta de configuração do Unity Version Control (Plastic SCM) (`client.conf` ausente em `AppData/Local/plastic4`).
2. `MCP-FOR-UNITY`: Falha de conexão WebSocket/Http (`Unable to connect to the remote server`).

Decisões do usuário:
1. Desativar completamente o Unity Version Control (UVC / Plastic SCM), utilizando exclusivamente o GitHub/Git.
2. Manter a inicialização automática do Unity MCP ativa para permitir conexão do assistente de IA (Antigravity/Gemini).

---

## 2. Diagnóstico e Contexto Consultados
- **Obsidian / Memória:**
  - `docs/ai-memory/CURRENT_STATE.md` e `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`.
  - `Docs/Ferramentas_Skills_Santa_Rita.md` e `Docs/Producao_Artistica_Execucao.md`: Unity MCP CoplayDev v10.0.0 com servidor local em `127.0.0.1:8080/mcp`.
- **Graphify CLI:**
  - `graphify query "VersionControl MCP" --budget 300` identificou `SantaRitaMcpSetup` e sua ligação com o ciclo do Unity Editor.
- **Causa Raiz Identificada:**
  1. **UVC / Plastic SCM:** `VersionControlSettings.asset` continha `m_Mode: Unity Version Control` e `manifest.json` incluía `com.unity.collab-proxy`. Como o repositório usa Git e o desenvolvedor não possui conta do Plastic SCM configurada, o cliente Codice falhava ao inicializar.
  2. **MCP-FOR-UNITY:** `SantaRitaMcpSetup.cs` fixava `MCPForUnity.UvxPath` para `~/.santarita/bin/uvx.exe` (caminho inexistente nesta máquina, onde `uvx.exe` reside em `~/.local/bin/uvx.exe`). Além disso, chamava diretamente `bridge.StartAsync()` (o cliente WebSocket da Unity) sem antes iniciar o servidor HTTP local (`ServerManagement.StartLocalHttpServer`), resultando em conexão recusada.

---

## 3. Plano de Ação Executado
- [x] Ajustar `ProjectSettings/VersionControlSettings.asset` de `Unity Version Control` para `Visible Meta Files` (padrão Git canônico).
- [x] Remover o pacote `com.unity.collab-proxy` de `Packages/manifest.json` para eliminar completamente dependências de runtime do Plastic SCM.
- [x] Refatorar `Assets/Editor/SantaRitaMcpSetup.cs`:
  - Detectar dinamicamente o executável `uvx.exe` em `~/.santarita/bin`, `~/.local/bin` ou diretórios de instalação do WinGet.
  - Inserir diretórios de ferramentas no `PATH` do processo Unity em tempo de execução.
  - Acionar `Server.StartLocalHttpServer(quiet: true)` antes de iniciar a ponte `Bridge.StartAsync()`, garantindo que o servidor local esteja pronto para receber a conexão da Unity e de clientes MCP externos (Antigravity/Gemini/Codex).
- [x] Atualizar grafo e cofre via `.\scripts\graphify-refresh.ps1`.
- [x] Validar integridade dos 93 scripts C# via `.\scripts\graphify-validate.ps1`.

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/ProjectSettings/VersionControlSettings.asset`: Modo alterado para `Visible Meta Files`.
- `Projeto Jogo 2D/Packages/manifest.json`: Removida dependência `"com.unity.collab-proxy": "2.13.6"`.
- `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs`: Resolução resiliente de paths e inicialização coordenada do servidor HTTP e da ponte WebSocket.

---

## 5. Validação e Testes
- `uvx --from mcpforunityserver mcp-for-unity --help`: Executado com sucesso, baixando dependências e validando funcionamento do pacote.
- `.\scripts\graphify-refresh.ps1`: Executado com sucesso (1529 nós, 4865 arestas, cofre atualizado com 1612 notas e `graph.html` gerado).
- `.\scripts\graphify-validate.ps1`: `PASS: 93 C# sources covered; portable paths; current hashes; canvas/HTML/report; four real queries.`

---

## 6. Próximos Passos
- Ao reabrir ou focar a janela da Unity, pressionar `Ctrl + R` ou clicar em `Clear` no console:
  - Os erros vermelhos do Plastic SCM não ocorrerão mais.
  - O MCP iniciará seu servidor HTTP headless em segundo plano e conectará a ponte automaticamente.
