# Remoção Definitiva dos Resquícios de "Menu Principal" e "Sair" — 28/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 28/10/2026 17:43 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário enviou uma nova captura de tela do jogo demonstrando que, embora o título, a linha e o botão "Continuar" tenham ficado limpos, ainda constavam no fundo os resquícios das palavras "Menu Principal" (letras `"Men"`) e "Sair" (palavra `"Sair"`).
Solicitou a análise da causa e a remoção definitiva.

---

## 2. Diagnóstico Técnico
- **Causa Raiz:** O script de reconstrução anterior havia iniciado a varredura da metade inferior em `x = 165` para não avançar sobre o lírio botânico. No entanto, o scan detalhado revelou que o lírio botânico termina em `x <= 85` e que as palavras originais "Menu Principal" e "Sair" começavam em `x = 131`. A faixa entre `x = 131` e `x = 165` havia ficado como uma lacuna intocada, deixando visíveis as letras iniciais `"Men"` e `"Sai"`.

---

## 3. Plano de Ação Executado
- [x] Ajustado o início da varredura inferior de síntese contínua de textura para `x = 100` (15 pixels além do término da flor), cobrindo integralmente o intervalo de `x = 100` a `395`.
- [x] Preservada integralmente a flor botânica no quadrante `x in [10..85], y in [250..480]` (1.083 pixels intactos).
- [x] Validação automatizada por scan de pixels:
  - Pixels claros de texto residual na zona de "Men" e "Sair" (`[x: 120..165, y: 290..380]`): **reduzidos a exatamente 0**.
  - Pixels claros de texto residual em toda a área de texto (`[x: 95..395, y: 45..390]`): **exatamente 0**.
- [x] Atualizado o sprite em `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png`.
- [x] Executado `./scripts/graphify-refresh.sh` (PASS).
- [x] Executado `./scripts/graphify-validate.sh` (PASS — 93 scripts C# íntegros).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` — Fundo 100% limpo com textura de ardósia e flor preservadas, sem resquícios de "Men" ou "Sair".

---

## 5. Validação e Próximos Passos
- Ao pausar o jogo no Unity Editor (`ESC`), a placa oxidada agora aparece 100% lisa e uniforme, sem qualquer texto residual em nenhuma das opções.
