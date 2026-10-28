# Diagnóstico e Resolução de Conflito de Porta (8080) no MCP Unity — 28/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 28/10/2026 16:22 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário relatou que ao abrir o Unity hoje, voltou a ocorrer um erro do MCP no terminal:
- `MCP-FOR-UNITY: Server ready on http://127.0.0.1:8080`
- `MCP-FOR-UNITY: [WebSocket] Connection failed. Check that the server URL is correct, the server is running, and your API key (if required) is valid. (Detail: Unable to connect to the remote server)`
- `MCP-FOR-UNITY: Failed to start MCP transport: Http`
Questionou o motivo pelo qual esse erro voltou a acontecer.

---

## 2. Diagnóstico Técnico Exaustivo
- **Investigação de Rede no Host:**
  - Foi executado `lsof -i :8080` e detectou-se o processo Java `PID 124682` escutando na porta `8080`:
    `/usr/lib/jvm/java-21-openjdk-amd64/bin/java ... br.com.betha.bffkickstart.BffKickstartApplication`
    (Aplicação backend Spring Boot em `/home/lucamontini/Documentos/portal-servicos-desafio/...`).
- **A Dinâmica da Falha:**
  1. O Unity MCP estava configurado para usar `http://127.0.0.1:8080`.
  2. Ao inicializar, o método `IsLocalHttpServerReachable()` testa a abertura de conexão TCP na porta 8080 com timeout de 50ms.
  3. Como o Spring Boot estava rodando e ouvindo na porta 8080, a conexão TCP foi aceita imediatamente.
  4. O Unity MCP assumiu que seu próprio servidor Python já estava pronto e logou: `Server ready on http://127.0.0.1:8080`.
  5. Em seguida, o `WebSocketTransportClient` do Unity tentou conectar em `ws://127.0.0.1:8080/hub/plugin`.
  6. Quem atendeu a requisição foi o Spring Boot (e não o servidor MCP). Como a rota `/hub/plugin` não existe na aplicação Java, o Spring Boot respondeu com `HTTP 404 Not Found`.
  7. O cliente MCP do Unity considerou a resposta 404 uma falha de conexão e cuspiu o erro vermelho de WebSocket.
  8. Além disso, como a porta 8080 já pertencia ao Java, o servidor Python do MCP sequer pôde ser iniciado nela.

---

## 3. Ação Executada
- [x] Atualizado `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs` para utilizar a porta dedicada **`8088`** (`http://127.0.0.1:8088`), que é completamente livre de colisões com microsserviços e aplicações Spring Boot/Tomcat.
- [x] Executado `./scripts/graphify-refresh.sh` (PASS).
- [x] Executado `./scripts/graphify-validate.sh` (PASS — 93 scripts C# indexados).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs` — Migração da porta padrão do MCP de 8080 para 8088.

---

## 5. Validação e Próximos Passos
- Ao recarregar no Unity, a porta 8088 será utilizada, permitindo que a aplicação Spring Boot do desenvolvedor continue rodando normalmente na porta 8080 sem colidir com o Unity MCP.
