# Refinamento Visual e Acessibilidade dos Controles dos Menus

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 2026-10-28 23:35 (Horário de Brasília)
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11, PowerShell, Unity 6000.6.0f1 (URP 2D)
- **Branch:** `unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
Executar as melhorias visuais e de acessibilidade aprovadas:
1. Remover completamente a flor/planta do canto inferior esquerdo de todos os menus, mantendo o fundo comum de ardósia oxidada.
2. Novo design para caixas de marcação (toggles), eliminando o bloco amarelo sólido monolítico.
3. Novo design para barras (sliders), tornando o trilho visível em todo o trajeto e refinando o cursor.
4. Despoluição visual do menu de resolução: adotada a Opção 2 (Seletor Horizontal / Stepper com setas `< 1920 x 1080 >`), eliminando popups gigantes e a barra de rolagem branca.
5. Suavização e acabamento no retângulo do exemplo de legendas, removendo a tarja preta crua.

---

## 2. Diagnóstico e Ações Executadas

### 2.1. Remoção da Flor de `PainelPausa_Fundo.png`
- Inpainting cirúrgico (OpenCV Telea) na região `x: 18..118, y: 270..505` de `PainelPausa_Fundo.png`.
- Preservação rigorosa da ferragem oxidada da margem esquerda e dos rebites inferiores.
- Síntese de microrruído calibrada com o desvio padrão da ardósia vizinha ($\sigma = 2.65$).
- Resultado: Luminosidade média de 13.95 (idêntica aos 14.1 da placa) e 0 pixels residuais da flor.

### 2.2. Caixas de Marcação (Toggles)
- `LinhaToggle` em `MenuPausa.cs`:
  - Moldura de 16×16 px em metal escovado/bronze (`#6B6152`), claramente visível tanto no estado Ligado quanto Desligado.
  - Cavidade interna escavada de 14×14 px em carvão escuro (`#141210`).
  - Marcação de seleção proporcional de 8×8 px em âmbar quente (`#F2B85C`), centralizada com respiro de 3px em relação à borda.
  - Feedback visual de seleção com realce na moldura.

### 2.3. Barras (Sliders)
- `LinhaSlider` em `MenuPausa.cs`:
  - Trilho com altura de 6px e canaleta visível em tom grafite/bronze (`#38332E`), delimitando a amplitude de 0% a 100%.
  - Preenchimento em âmbar quente suave (`#DCA856`).
  - Cursor (handle) ergonômico de 5×10 px em tom osso vintage (`#E0D8C8`), destacando em ouro brilhante (`#F8D070`) quando selecionado.

### 2.4. Seletor de Resolução Horizontal (Stepper `< 1920 x 1080 >`)
- `LinhaStepperResolucao` e `HorizontalStepperNavigation` em `MenuPausa.cs`:
  - Interface na própria linha com moldura bronze e botões retro `<` e `>` para clique com mouse.
  - Suporte completo a navegação por teclado (`A` / `D` ou Setas Esquerda / Direita) e controle.
  - Resolução exibida em linha única sem quebra de texto.
  - Popup vertical e barra de rolagem branca da Unity 100% desativados.
  - Total compatibilidade com `VideoSettingsController` (atualização e salvamento em `PlayerPrefs`).

### 2.5. Card de Exemplo de Legendas
- Moldura de 144×20 px em bronze escovado translúcido (`#594D3F`, alpha 0.6).
- Fundo dinâmico interno translúcido ativado apenas quando a opção "Fundo Escuro" está marcada.
- Texto `"Exemplo de legenda"` centralizado e responsivo ao tamanho configurado, sem aspecto de tarja preta crua.

---

## 3. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png`
- `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs`

---

## 4. Validação e Testes
- **Console Unity (Unity MCP):** 0 erros e 0 warnings no console.
- **`.\scripts\graphify-refresh.ps1`:** Sucesso (código 0, 1544 nós, 4931 arestas).
- **`.\scripts\graphify-validate.ps1`:** **`PASS: 93 C# sources covered; portable paths; current hashes; canvas/HTML/report; four real queries.`**

---

## 5. Próximos Passos
- Verificação in-game pelo usuário abrindo os menus e testando os seletores e a placa limpa.
