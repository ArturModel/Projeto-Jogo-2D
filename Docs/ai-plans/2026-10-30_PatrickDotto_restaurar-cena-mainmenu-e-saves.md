# Plano de Execução: Restauração da Cena Inicial (`MainMenu.unity`) no Editor e Verificação dos Saves Pessoais

- **Autor:** `@PatrickDotto` (Patrick Dotto)
- **Data/Hora:** 2026-10-30 15:37 (BRT / UTC-3)
- **Status:** Concluído
- **Ambiente:** Windows 10/11 · PowerShell · Unity `6000.6.0f1` (URP 2D) · Branch `atualizacao_sinuca`

---

## 1. Solicitação do Usuário
O usuário solicitou ajustar o Unity Editor para poder dar Play diretamente a partir do Menu Inicial (`MainMenu.unity`) e verificar seus saves pessoais, pois o Editor estava na cena vazia `Untitled` (`"Display 1 No cameras rendering"`).

---

## 2. Diagnóstico e Contexto Consultados
- **Memória e Cofre Obsidian:** `docs/ai-memory/INDEX.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`, `docs/architecture-vault/manual/Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`.
- **Código-Fonte C# Real:**
  - [`Chapter2BuildVerification.cs`](../../Projeto%20Jogo%202D/Assets/Editor/Chapter2BuildVerification.cs): na linha 173, `EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single)` abria uma cena vazia `Untitled` antes de empacotar o executável para evitar avisos aditivos do URP 2D, mas o bloco `finally` não reabria `Assets/Scenes/MainMenu.unity` ao terminar.
  - [`CheckpointSystem.cs`](../../Projeto%20Jogo%202D/Assets/Scripts/CheckpointSystem.cs): verificado que todos os saves pessoais do usuário em `Application.persistentDataPath` foram preservados intactos pelo `IsolatedSaveScope`.

---

## 3. Plano de Ação Executado
- [x] Parar o Play Mode na cena vazia `Untitled` e abrir imediatamente `Assets/Scenes/MainMenu.unity` no Unity Editor via MCP.
- [x] Verificar a integridade dos slots de save pessoais do usuário em `C:/Users/Usuario/AppData/LocalLow/DefaultCompany/Projeto Jogo 2D` (`Slot 1`: Sociedade Beira-Rio, `Slot 2`: Rua Norte / Bar do Zé, `Slot 5`: Sociedade Beira-Rio).
- [x] Atualizar o bloco `finally` de [`Chapter2BuildVerification.Run()`](../../Projeto%20Jogo%202D/Assets/Editor/Chapter2BuildVerification.cs) para reabrir automaticamente `Assets/Scenes/MainMenu.unity` após builds fora de batch mode.
- [x] Executar `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`.

---

## 4. Arquivos Criados, Modificados ou Removidos
- **Criados:**
  - `docs/ai-plans/2026-10-30_PatrickDotto_restaurar-cena-mainmenu-e-saves.md`
- **Modificados:**
  - `Projeto Jogo 2D/Assets/Editor/Chapter2BuildVerification.cs`
  - `docs/ai-memory/CHANGELOG_AI.md`

---

## 5. Validação e Testes Realizados
- Confirmado no Unity Editor que `Assets/Scenes/MainMenu.unity` está aberta e ativa, com `CheckpointSystem.HasCheckpoint == true` e todos os 3 slots salvos pelo autor intactos.
- Executados `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1` com `PASS`.

---

## 6. Próximos Passos e Pendências
- Pronto para o autor pressionar Play diretamente no Unity Editor a partir do `MainMenu`.
