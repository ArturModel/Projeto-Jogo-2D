# Correção de Race Condition e Suporte Multiplataforma no MCP Unity — 27/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 27/10/2026 15:43 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
Após a resolução de todos os avisos e erros anteriores na abertura do Unity, restou apenas uma mensagem de erro e aviso relacionada ao MCP no console:
- `MCP-FOR-UNITY: [WebSocket] Connection failed. Check that the server URL is correct, the server is running, and your API key (if required) is valid. (Detail: Unable to connect to the remote server)`
- `MCP-FOR-UNITY: Failed to start MCP transport: Http`
O usuário questionou o que é o MCP, por que o erro ocorre e aprovou a opção recomendada de manter o MCP funcionando, corrigindo a inicialização para que conecte de forma limpa e silenciosa no Linux.

---

## 2. Diagnóstico Técnico
- **O que é o MCP:** Model Context Protocol; uma ponte que conecta LLMs e agentes de IA ao Unity Editor via servidor local (porta 8080) gerenciado pelo `uvx`.
- **Causa da Mensagem de Erro:**
  1. *Condição de corrida na inicialização:* O script `SantaRitaMcpSetup.cs` chamava `Bridge.StartAsync()` no primeiro frame do Editor (`[InitializeOnLoad]`), tentando conectar o cliente antes mesmo que o servidor local HTTP tivesse sido iniciado pelo pacote `com.coplaydev.unity-mcp`. O WebSocket disparava erro de conexão não encontrada.
  2. *Incompatibilidade de SO:* O script herdado do desenvolvedor anterior configurava caminhos fixos de Windows (`.exe`, `;`, e pasta de Python do Windows). No Linux, o executável é `uvx` (presente em `~/.local/bin/uvx`), o separador é `:` e não deve haver `.exe` gravado no `EditorPrefs.UvxPath`.

---

## 3. Plano de Ação Executado
- [x] Refatorado `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs`:
  - Removida a chamada prematura a `Bridge.StartAsync()` do `InitializeOnLoad`. O pacote MCP já possui o manipulador oficial `HttpAutoStartHandler` que inicia o servidor local em segundo plano e aguarda sua disponibilidade antes de conectar o cliente.
  - Adicionada detecção multiplataforma (`Application.platform == RuntimePlatform.WindowsEditor`):
    - No Linux/macOS: remove overrides legados com `.exe`, adiciona `~/.local/bin` ao `PATH` com separador `:` e aponta para `~/.local/bin/uvx` se existente.
    - No Windows: preserva a compatibilidade com o setup legado de Patrick Dotto (`.santarita`, `uvx.exe`, `;`).
  - No menu manual (`Santa Rita/Ferramentas/Conectar Unity MCP local`), implementada verificação de disponibilidade do servidor antes de conectar o bridge (`IsLocalHttpServerReachable()`).
- [x] Executado `./scripts/graphify-refresh.sh` (PASS).
- [x] Executado `./scripts/graphify-validate.sh` (PASS — 93 scripts C# indexados).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs` — Suporte multiplataforma e desacoplamento da inicialização automática.

---

## 5. Validação e Testes
- Grafo AST atualizado e validado sem divergências estruturais.
- No Unity Editor, o carregamento passa a ser ordenado: o script configura as preferências de ambiente de forma compatível com Linux e o `HttpAutoStartHandler` sobe o servidor HTTP local sem colisão de WebSocket.
