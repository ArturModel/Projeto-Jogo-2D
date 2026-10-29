# Fidelidade Visual do Menu de Pausa ao Protótipo Conceitual — 28/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 28/10/2026 21:30 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11 (PowerShell), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário comparou visualmente o estado atual do Menu de Pausa no jogo com a arte conceitual/protótipo aprovado, apontando quatro divergências visuais:
1. A borda direita da placa oxidada estava truncada/cortada reto, sem os rebites e oxidação.
2. Os botões e textos das opções estavam descentralizados na coluna (com `"Sair"` flutuando no meio).
3. O título `"PAUSADO"` estava menor e com fonte pixelada genérica sem a tipografia desgastada de imprensa original.
4. O rótulo de status de salvamento estava descentralizado para a direita e continha a instrução `(salve em um Orelhão)`.
Solicitou um plano detalhado para atingir fidelidade visual absoluta ao protótipo e, após aprovação do plano, sua execução imediata.

---

## 2. Diagnóstico Técnico e Contexto
- **Borda Direita Cortada:** O arquivo `PainelPausa_Fundo.png` original havia sido recortado com `433×495` px em vez das dimensões totais da placa (`456×520` px), excluindo 23 pixels da borda direita e 25 pixels da base.
- **Título PAUSADO:** Renderizado via `TMP_Text` padrão (`fontSize = 28f`) em vez de utilizar o grafismo rústico serifado (*woodblock slab-serif*) com a linha vermelha rústica.
- **Alinhamento dos Botões:** Em `MenuPausa.cs`, `BotaoPausa` utilizava `TextAlignmentOptions.Center` isolado em cada rótulo, quebrando a coluna vertical. No protótipo, todas as 4 opções iniciam rigorosamente no mesmo X (`~37f` dentro do container, após a seta `▶`).
- **Status de Salvamento:** `rotuloStatusSave` possuía offset forçado em `new Vector2(10f, -102f)` e texto com menção ao orelhão.

---

## 3. Plano de Ação Executado
- [x] Extraído o fundo integral da placa oxidada com `456×520` px (`x: [24..480]`, `y: [15..535]`), restaurando a borda direita de 23px, cantos e rebites.
- [x] Aplicado inpainting contínuo de textura de ardósia no miolo de texto, preservando 100% da flor botânica no quadrante inferior esquerdo e transparência dos 4 cantos arredondados.
- [x] Salvo `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` com 0 pixels residuais de texto.
- [x] Extraído `PainelPausa_Titulo.png` com canal alfa contendo o carimbo serifado vintage `"PAUSADO"` e a linha divisória vermelha rústica original. Criado respectivo `.meta` (`TextureImporter`, Sprite 2D).
- [x] Atualizado `MenuPausa.cs`:
  - `TituloPausa`: carrega `PainelPausa_Titulo` e exibe como imagem proporcional `(132f, 42f)` em `y = 85f`, com fallback para `TMP_Text`.
  - `BotaoPausa`: configurado `TextAlignmentOptions.MidlineLeft` com recuo de `14f` no centro e largura `150f` (início a `37f` da margem esquerda do botão), alinhando verticalmente `"Continuar"`, `"Configurações"`, `"Menu Principal"` e `"Sair"`.
  - `rotuloStatusSave`: centralizado em `(0f, -105f)` e simplificado para `"Progresso não salvo"` e `"Jogo salvo"`, removendo `(salve em um Orelhão)`.
- [x] Executado `.\scripts\graphify-refresh.ps1` (PASS).
- [x] Executado `.\scripts\graphify-validate.ps1` (PASS — 93 fontes C# íntegras).

---

## 4. Arquivos Modificados e Criados
- **Criados:**
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Titulo.png`
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Titulo.png.meta`
  - `docs/ai-plans/2026-10-28_lucamontini_fidelidade-visual-menu-pausa.md`
- **Modificados:**
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` (restaurado para 456x520 com borda direita completa)
  - `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs` (título gráfico, alinhamento à esquerda dos botões e status centralizado)
  - `docs/ai-memory/CHANGELOG_AI.md`

---

## 5. Validação e Testes
- **Scan de Pixels do Fundo:** 0 pixels claros residuais na zona de texto; flor botânica 100% íntegra (0 mismatches).
- **Integridade Graphify/Obsidian:** `.\scripts\graphify-validate.ps1` aprovado com 93 classes C# e hashes válidos.
- **Render de Verificação:** Gerado `scratch_verification_render.png` demonstrando casamento 1:1 com o protótipo conceitual.

---

## 6. Próximos Passos
- Entrar no Play Mode no Unity Editor e pressionar `ESC` para verificar a exibição em tempo real na resolução do jogo (`640×360`).
