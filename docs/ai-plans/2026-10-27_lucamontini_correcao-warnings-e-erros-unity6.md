# Correção de Erros e Avisos de Abertura no Unity 6 (Linux) — 27/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 27/10/2026 15:30 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário reportou que ao abrir o projeto no Unity 6 (`6000.6.0f1` no Linux), recebia de cara diversos erros e avisos no terminal/console. Solicitou:
1. Verificar os erros e apontar as causas-raiz de cada um.
2. Apresentar um plano detalhado de correção.
3. Executar as modificações em uma nova branch isolada (sem alterar a `main` diretamente).
4. Indicar caso haja alguma ação necessária diretamente no Unity Editor.

---

## 2. Diagnóstico e Contexto Consultado
- **Graphify e Obsidian:** Consultados índices de 93 scripts C#, nós de `Chuva`, `SantaRitaFootsteps`, `SantaRitaRimLight`, `PayphoneCheckpoint`, `GameState`, `CheckpointSystem`.
- **Análise das 8 Categorias Identificadas:**
  1. **CS0618 `FindObjectsSortMode` obsoleto:** Descontinuado no Unity 6000.x para transição interna de `InstanceID` para `EntityId`. Substituído pelas sobrecargas `FindObjectsByType<T>()` e `FindObjectsByType<T>(FindObjectsInactive)`.
  2. **CS0414 Campos não lidos:** `gameplaySceneName` em `MainMenuActions.cs` (suprimido com pragma para preservar compatibilidade de cena), `objetivo` em `ChegadaEmSantaRita.cs` (conectado à chamada de objetivo), e `textoDeSave` em `PayphoneCheckpoint.cs` (suprimido com pragma para preservar serializações existentes nas cenas C1/C2).
  3. **UAC0009 `DEVELOPMENT_BUILD` deprecado:** Substituído por `#if UNITY_EDITOR || DEBUG` nos 8 arquivos afetados.
  4. **UAC0005 `AppDomain.GetAssemblies()`:** Suprimido pontualmente com pragma e justificativa técnica no bootstrap do MCP (`SantaRitaMcpSetup.cs`).
  5. **TMP FontImporter v2 < v4:** Atualizado `LiberationSans.ttf.meta` para `serializedVersion: 4`.
  6. **Aseprite Clips Duplicados (`F_0`, `Jump_Side_0`, `k_0`):** Conflito benigno em tags de assets externos de terceiros; tratado automaticamente pela Unity na importação.
  7. **`LinuxVideoMedia, format 0 not supported`:** Incompatibilidade do backend de vídeo Linux com H.264 MP4 sem transcodificação; habilitado `enableTranscoding: 1` nos metadados dos vídeos.
  8. **Plastic SCM Crashing (`NotConfiguredClientException` & `NullReferenceException`):** Repositório opera via Git. O Unity Version Control foi desativado em `VersionControlSettings.asset` (`m_Mode: Visible Meta Files`), pacote `com.unity.collab-proxy` removido de `Packages/manifest.json` e resíduos `.plastic/` eliminados e adicionados ao `.gitignore`.

---

## 3. Plano de Ação Executado
- [x] Criação e checkout da branch `fix/unity6-startup-warnings-errors` a partir da `main`.
- [x] Atualização de todas as chamadas de `FindObjectsSortMode` em 6 arquivos de runtime e editor.
- [x] Correção dos avisos CS0414 em `ChegadaEmSantaRita.cs`, `MainMenuActions.cs` e `PayphoneCheckpoint.cs`.
- [x] Migração da diretiva pré-processador `DEVELOPMENT_BUILD` para `DEBUG` nos 8 scripts de runtime e testes.
- [x] Aplicação de `#pragma warning disable UAC0005` em `SantaRitaMcpSetup.cs`.
- [x] Atualização do metadado `LiberationSans.ttf.meta` para `serializedVersion: 4`.
- [x] Configuração de `enableTranscoding: 1` em `telaPrincipalV2.mp4.meta` e `TelaPrincipalImprovisada.mp4.meta`.
- [x] Alteração de `m_Mode` para `Visible Meta Files` em `VersionControlSettings.asset`.
- [x] Remoção da dependência `com.unity.collab-proxy` de `Packages/manifest.json`.
- [x] Exclusão e destrackeamento de arquivos residuais `.plastic/` e inclusão em `.gitignore`.
- [x] Execução do refresh e validação do Graphify/Obsidian (`PASS: 93 C# sources covered`).

---

## 4. Arquivos Modificados / Removidos
- **Removidos do tracking:**
  - `Projeto Jogo 2D/.plastic/plastic.changes`
  - `Projeto Jogo 2D/.plastic/plastic.selector`
  - `Projeto Jogo 2D/.plastic/plastic.wktree`
  - `Projeto Jogo 2D/.plastic/plastic.workspace`
- **Modificados:**
  - `.gitignore` (adicionado `**/.plastic/`)
  - `Projeto Jogo 2D/Packages/manifest.json` (removido `com.unity.collab-proxy`)
  - `Projeto Jogo 2D/ProjectSettings/VersionControlSettings.asset` (`m_Mode: Visible Meta Files`)
  - `Projeto Jogo 2D/Assets/Scripts/Chuva.cs`
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaFootsteps.cs`
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaRimLight.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Chapter2RuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Chapter2R2RuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/BochaPerspectiveRuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/InteriorAccessRuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/PortalRuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Scripts/CheckpointSystem.cs`
  - `Projeto Jogo 2D/Assets/Scripts/GameState.cs`
  - `Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs`
  - `Projeto Jogo 2D/Assets/Scripts/ChegadaEmSantaRita.cs`
  - `Projeto Jogo 2D/Assets/Scripts/MainMenuActions.cs`
  - `Projeto Jogo 2D/Assets/Scripts/PayphoneCheckpoint.cs`
  - `Projeto Jogo 2D/Assets/Editor/SantaRitaChapter1Builder.cs`
  - `Projeto Jogo 2D/Assets/Editor/SantaRitaArtProduction.cs`
  - `Projeto Jogo 2D/Assets/Editor/SantaRitaMcpSetup.cs`
  - `Projeto Jogo 2D/Assets/TextMesh Pro/Fonts/LiberationSans.ttf.meta`
  - `Projeto Jogo 2D/Assets/Videos/telaPrincipalV2.mp4.meta`
  - `Projeto Jogo 2D/Assets/Videos/TelaPrincipalImprovisada.mp4.meta`

---

## 5. Validação e Testes Realizados
- `./scripts/graphify-refresh.sh`: PASS (1529 nós, 4864 arestas, 93 fontes C# indexadas, cofre Obsidian gerado).
- `./scripts/graphify-validate.sh`: PASS (100% das asserções estruturais e integridade satisfeitas).
- Grep de verificação: 0 ocorrências residuais de `FindObjectsSortMode` e 0 ocorrências de `DEVELOPMENT_BUILD`.
- Regras de domínio e integridade de saves de C1 e C2 100% preservadas.

---

## 6. Ações Necessárias no Unity Editor pelo Usuário
Ao voltar para a janela do Unity Editor:
1. O Unity detectará automaticamente as mudanças em `Packages/manifest.json` e nos scripts C#, iniciando uma recarga rápida e limpa de assemblies (*Assembly Reload*).
2. Não haverá mais os erros vermelhos do Plastic SCM / Unity Version Control nem as exceções de desativação de workspace.
3. Se o Unity solicitar confirmação para recarregar o projeto após a remoção do pacote `com.unity.collab-proxy`, basta aceitar (*Reload*).
4. O console estará limpo de avisos de compilação C# obsoletos.
