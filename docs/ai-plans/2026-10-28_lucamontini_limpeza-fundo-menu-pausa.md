# Limpeza Definitiva do Fundo do Menu de Pausa — 28/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 28/10/2026 17:35 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário enviou uma captura de tela do jogo demonstrando que o Menu de Pausa apresentava resquícios das letras originais do mockup ("PAUSADO", "Continuar", "Configurações", "Menu Principal", "Sair" e uma letra "c" no botão de seleção) impressas no arquivo do fundo, sobrepondo-se ao menu real interativo.
Solicitou a limpeza completa do fundo mantendo a textura rústica e as flores botânicas.

---

## 2. Diagnóstico Técnico
- **Causa Raiz 1 (Fundo da Placa):** O script anterior de inpainting utilizou uma grade de amostragem com espaçamento largo (`range(..., 100)` com blocos de `50–80 px`), deixando lacunas vazias que mantiveram mais de 4.800 pixels claros das palavras originais gravados na imagem.
- **Causa Raiz 2 (Botão de Seleção):** A limpeza do papiro começou em `x = 40`, deixando os pixels de `x = 25` a `40` (a curva da letra "C" de "Continuar") intocados logo ao lado da seta vermelha.

---

## 3. Plano de Ação Executado
- [x] Executado algoritmo de síntese e reconstrução contínua e densa (sobreposição a cada 25–35 px) utilizando amostras puras da ardósia escura livre.
- [x] Preservada integralmente a flor (lírio botânico) no quadrante `x < 169, y >= 250` (1.070 pixels preservados).
- [x] Preservadas as bordas metálicas oxidadas, parafusos e transparência dos cantos.
- [x] Corrigido o ponto de início da limpeza do papiro no botão de seleção para `x = 25` (imediatamente no término da seta vermelha em `x = 24`).
- [x] Verificação automatizada por scan de pixels:
  - Pixels claros de texto residual no fundo: reduzidos de **4.819 para 0**.
  - Pixels escuros de texto residual na barra de seleção: reduzidos de **39 para 0**.
- [x] Salvas as novas imagens em `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` e `PainelPausa_Selecao.png`.
- [x] Executado `./scripts/graphify-refresh.sh` (PASS).
- [x] Executado `./scripts/graphify-validate.sh` (PASS — 93 scripts C# íntegros).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` — Fundo 100% limpo com textura de ardósia e flor botânica preservadas.
- `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Selecao.png` — Barra de papiro limpa com seta vermelha intacta e sem resto da letra "c".

---

## 5. Validação e Próximos Passos
- Ao abrir o Unity Editor e pausar o jogo (`ESC`), o fundo metálico de ardósia agora aparecerá completamente liso e limpo, sem nenhuma letra fantasma competindo com o texto interativo do TextMeshPro.
