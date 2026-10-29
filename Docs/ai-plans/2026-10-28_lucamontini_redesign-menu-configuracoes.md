# Redesign do Menu de Configurações e Submenus (Áudio, Vídeo, Legendas)

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 2026-10-28 22:32 (Horário de Brasília)
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11, PowerShell, Unity 6000.6.0f1 (URP 2D)
- **Branch:** `unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
Aplicar o mesmo estilo, design e identidade visual da placa de ferro oxidada (com lírio gravado, tipografia vintage, linha divisória vermelha e papiro de seleção) ao menu de configurações e seus submenus (Áudio, Vídeo, Legendas). Reposicionar todos os painéis para o terço esquerdo da tela (`x = -160f`), removendo a antiga caixa de diálogo centralizada (`x = 0f`), mantendo a transição suave e evitando qualquer retrabalho através do reuso integral dos assets já validados.

---

## 2. Diagnóstico e Contexto
- **Arquivo Alvo:** `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs`.
- **Assets Reutilizados (100% de reaproveitamento, zero retrabalho):**
  - `Assets/Resources/UI/PainelPausa_Fundo.png` (`456×520` px, textura autêntica de ferro oxidado com lírio e rebites).
  - `Assets/Resources/UI/PainelPausa_Selecao.png` (`310×47` px, papiro com seta vermelha rústica `▶`).
  - `Assets/Resources/UI/PainelPausa_Linha.png` (linha divisória rústica carmim).
- **Posicionamento e Proporção:**
  - `painelOpcoes` migrado para `CaixaPausa` com tamanho `246×280` px ancorado em `new Vector2(-160f, 0f)`.
  - Transição via `MenuPanelTransition.cs` ocorre no mesmo quadrante esquerdo (`x = -160f`) com deslocamento suave de 18px.
  - O cabeçalho dinâmico `OptionsPanelController` atualiza `CONFIGURAÇÕES`, `ÁUDIO`, `VÍDEO` e `LEGENDAS` mantendo a linha vermelha abaixo.
  - Subpainel `Categorias` usa `IndicadorPausa` e `BotaoPausa` com papiro e seta vermelha.
  - Subpainéis `Áudio`, `Vídeo` e `Legendas` ajustados para caber perfeitamente na largura interna de 192px com indicador `▶` e controles em tons vintage.

---

## 3. Ações Executadas
1. **Atualização de Constantes em `MenuPausa.cs`:**
   - `LarguraOpcoes` ajustada de `370f` para `246f`.
2. **Construção de `PainelOpcoes`:**
   - Substituição da chamada `Caixa(...)` por `CaixaPausa(tela, "PainelOpcoes", new Vector2(LarguraPausa, AlturaPausa), out RectTransform opcoesRect)`.
   - `Titulo(opcoesRect, "CONFIGURAÇÕES", 92f, LarguraPausa)` com linha vermelha rústica `PainelPausa_Linha` na base.
3. **Categorias de Configurações:**
   - Migrado para `IndicadorPausa(catRect)` e `BotaoPausa`:
     - `Áudio` (`y = 44f`)
     - `Vídeo` (`y = 12f`)
     - `Legendas` (`y = -20f`)
     - `Voltar` (`y = -52f`)
4. **Subpainel de Áudio:**
   - `Volume Geral` (`y = 44f`), `Música` (`y = 12f`), `Efeitos` (`y = -20f`), `Voltar` (`y = -58f`).
   - Sliders com largura interna compacta (`52f × 8f`), trilhos em cinza carvão e preenchimento bronze/âmbar.
5. **Subpainel de Vídeo:**
   - `Tela Cheia` (`y = 48f`), `Resolução` (`y = 24f`), `VSync` (`y = 0f`), `Brilho` (`y = -24f`), `Efeitos Suaves` (`y = -48f`), `Voltar` (`y = -74f`).
6. **Subpainel de Legendas:**
   - `Legendas Ativas` (`y = 48f`), `Tamanho` (`y = 22f`), `Fundo Escuro` (`y = -4f`), Preview (`160×24` px em `y = -36f`), `Voltar` (`y = -74f`).
7. **Remoção de Código Obsoleto:**
   - Remoção do método legado `Caixa` (caixa de diálogo genérica com flores nos cantos e linha vermelha lateral).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs`

---

## 5. Validação e Testes
- **Compilação Unity (Unity MCP `read_console`):** 0 erros e 0 warnings no console Unity.
- **`.\scripts\graphify-refresh.ps1`:** Executado com sucesso (código 0, AST de 121 arquivos processada, 1534 nós, 4894 arestas).
- **`.\scripts\graphify-validate.ps1`:** PASS (93 fontes C# cobertos, caminhos portáveis, hashes íntegros).

---

## 6. Próximos Passos
- Teste in-game pelo usuário abrindo ESC -> Configurações -> Áudio / Vídeo / Legendas.
- Ajuste de sensibilidade ou novos submenus caso desejado.
