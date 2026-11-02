# Resolução de Conflitos Git/VS Code e Avisos/Erros de Inicialização no Console do Unity — 30/10/2026

## Cabeçalho
- **Autor:** @PatrickDotto (Patrick Dotto)
- **Data/Hora:** 30/10/2026 13:46 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11 / PowerShell / Unity 6000.6.0f1 (URP 2D) / Graphify 0.9.96 / Git LFS 3.7.1

---

## 1. Solicitação do Usuário
1. Resolver os conflitos no VS Code que impediam o `git pull` na branch `main` e os problemas relacionados no Unity.
2. Explicar e resolver os avisos e erros exibidos no Console do Unity após a sincronização (`Unexpected timestamp values detected`, `Color primaries 0 is unknown or unsupported by WindowsMediaFoundation`, `MCP-FOR-UNITY: [WebSocket] Connection failed`, `MCP-FOR-UNITY: Failed to start MCP transport: Http` e avisos `CS0414`).

---

## 2. Diagnóstico e Contexto Consultados
- **Obsidian e Memória:**
  - `docs/ai-memory/INDEX.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/ai-memory/CHANGELOG_AI.md`.
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`.
- **Graphify CLI:**
  - `graphify query "SantaRitaMcpSetup video" --budget 300`
  - `graphify explain "SantaRitaMcpSetup"`
- **Diagnóstico Técnico:**
  1. **Bloqueio de `git pull` no VS Code:** A branch local `main` estava 12 commits atrás de `origin/main` (`434b320` -> `30d811c`) com arquivos sujos automaticamente pelo antigo pacote Plastic SCM (`.plastic/plastic.changes`, `.plastic/plastic.wktree`), cache de fonte (`VT323-Regular Bitmap.asset`) e quebra de linha `CRLF` em `Kenney-Interface-License.txt`. Sincronizada a branch `main` com `origin/main` (`30d811c`) e objetos LFS.
  2. **Erro e Warning do `MCP-FOR-UNITY` (`[WebSocket] Connection failed` / `Failed to start MCP transport: Http`):** Em `SantaRitaMcpSetup.ConectarManualmente()`, após invocar `StartLocalHttpServer(true)`, o código aguardava apenas um `Task.Delay(1500)` fixo antes de chamar `bridge.StartAsync()`. No Windows, a inicialização do servidor Python FastMCP/Uvicorn via `uvx` na porta `8088` levou ~26 segundos na primeira execução (instalando 72 pacotes) e 3–8 segundos em partidas frias, fazendo o WebSocket tentar conectar antes de a porta `8088` estar ouvindo.
  3. **Warnings de Compilação `CS0414` em `ChegadaEmSantaRita.cs`:** Os campos serializados legados `esperaAntesDoObjetivo` e `objetivo` (mantidos para compatibilidade de YAML da cena `EntradaDaCidade`) deixaram de ser lidos diretamente no `Start()` após o commit `2be1e35`, gerando 2 avisos `CS0414` por passe de compilação.
  4. **Mensagens de Vídeo (`Unexpected timestamp values detected`, `First video frame not zero` e `Color primaries 0 is unknown or unsupported by WindowsMediaFoundation`):** Emitidas uma única vez pelo `VideoClipImporter` do Windows quando os arquivos `.mp4.meta` receberam `enableTranscoding: 1` (adicionado em `baae3e4` para compatibilidade com Linux). O `WindowsMediaFoundation` transcodificou os vídeos para o cache local (`Library/Artifacts`), ajustando os timestamps e aplicando fallback padrão BT.709 de cor sem impacto funcional.

---

## 3. Plano de Ação Executado
- [x] Sincronizar a branch `main` local com `origin/main` (`30d811c`), limpando artefatos `.plastic/` e puxando objetos Git LFS.
- [x] Atualizar `SantaRitaMcpSetup.ConectarManualmente()` para verificar `IsManagedServerLaunchProcessAlive()` (evitando duplo disparo simultâneo com `HttpAutoStartHandler`) e fazer polling de `IsLocalHttpServerReachable()` a cada 500ms até que o servidor local na porta `8088` esteja efetivamente escutando antes de chamar `bridge.StartAsync()`.
- [x] Adicionar `#pragma warning disable CS0414` nos campos serializados legados `esperaAntesDoObjetivo` e `objetivo` em `ChegadaEmSantaRita.cs` (seguindo o padrão de `PayphoneCheckpoint.cs` e `MainMenuActions.cs`).
- [x] Executar `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`.

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs`: Polling de prontidão HTTP (`IsLocalHttpServerReachable` + `IsManagedServerLaunchProcessAlive`) antes de iniciar a ponte WebSocket.
- `Projeto Jogo 2D/Assets/Scripts/ChegadaEmSantaRita.cs`: Supressão pontual de `CS0414` em campos serializados legados.
- `docs/ai-plans/2026-10-30_PatrickDotto_resolucao-conflitos-git-e-console-unity.md`: Registro deste plano.
- `docs/ai-memory/CHANGELOG_AI.md`: Registro no histórico de intervenções.

---

## 5. Validação e Testes Realizados
- `.\scripts\graphify-refresh.ps1`: Executado com sucesso (1591 nós, 5120 arestas, 79 comunidades, 1670 notas no cofre Obsidian).
- `.\scripts\graphify-validate.ps1`: `PASS: 94 C# sources covered; portable paths; current hashes; canvas/HTML/report; four real queries.`
- Verificação de rede: Servidor MCP ativo e escutando em `127.0.0.1:8088`.

---

## 6. Próximos Passos
- Ao focar a janela do Unity Editor, o Unity recompilará `ChegadaEmSantaRita.cs` e `SantaRitaMcpSetup.cs`, limpará os avisos `CS0414` e conectará automaticamente a ponte `MCP-FOR-UNITY` ao servidor já ativo na porta `8088`.
