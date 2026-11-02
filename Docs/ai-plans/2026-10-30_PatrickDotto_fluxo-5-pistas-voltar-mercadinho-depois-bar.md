# Plano Executado — Fluxo Original das 5 Pistas da Casa do Avô (Voltar ao Mercadinho → Bar do Zé)

- **Autor:** `@PatrickDotto` (Patrick Dotto)
- **Data/Hora:** 2026-10-30 16:22
- **Status:** Concluído e Validado
- **Ambiente:** Windows / PowerShell / Unity `6000.6.0f1` (URP 2D), branch `atualizacao_sinuca`

---

## 1. Solicitação do Usuário

> *"ta pode deixar como estava antes, porém após eu pegar as 5 pistas deve dizer q preciso voltar no mercadinho, ai depois eu vou p bar é isso?"*

---

## 2. Diagnóstico e Contexto Consultados (Obsidian, Graphify e Código-Fonte)

- O autor confirmou que deseja manter a ordem narrativa canônica:
  1. Examinar as **5 pistas** na Casa do Avô (`pista_cafe_casa`, `pista_cartas_casa`, `pista_recibo_mercadinho`, `pista_desordem_casa` e `casa_telefone_fora_do_gancho`).
  2. Ao concluir as 5 pistas na Casa do Avô, o jogo deve **avisar claramente** (em legenda na tela e no objetivo/mapa) que é preciso **voltar ao Mercadinho** para mostrar o aviso no verso do cupom à atendente.
  3. Ao conversar com a atendente no Mercadinho (`ConfirmarTestemunhoAtendente()`), ela revela que viu o homem de capa na mesa dos fundos do Bar do Zé (`Flags.OuviuRelatoAtendente` e `Flags.Pista_RumoBarDoZe`), e o jogo orienta o jogador a subir para o **Bar do Zé**.
  4. Caso o jogador suba ao Bar do Zé antes de falar com a atendente sobre o cupom (`ZeDesconversa()`), o pensamento de Marcus também lembra explicitamente que é preciso voltar ao Mercadinho e mostrar o cupom para a atendente antes de confrontar Zé.

---

## 3. Plano de Ação Executado

- [x] **Passo 1:** Restaurar em `InteracaoCapitulo1.InteragirZe()` a exigência original de `Flags.OuviuRelatoAtendente` antes de iniciar o confronto da Sinuca com Zé.
- [x] **Passo 2:** Atualizar `InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` para que, ao reunir as 5 pistas na Casa do Avô (incluindo o cupom `Flags.Pista_NotaFiscal2004`), o objetivo seja definido como `"Voltar ao Mercadinho e mostrar o cupom com o aviso para a atendente."` e a legenda em tela diga `"Vasculhei as 5 pistas da casa do vô. Preciso voltar ao Mercadinho e mostrar o aviso no cupom para a atendente."`.
- [x] **Passo 3:** Atualizar `InteracaoCapitulo1.ConfirmarTestemunhoAtendente()` para que, após ouvir o relato da atendente no Mercadinho, o objetivo passe a `"Ir ao Bar do Zé na Rua Norte e confrontar Zé sobre o homem de capa."` com legenda confirmando a ida ao Bar do Zé.
- [x] **Passo 4:** Atualizar `InteracaoCapitulo1.ZeDesconversa()` para que, se o jogador já tiver o cupom (`Flags.AchouReciboMercadinho`), Marcus pense explicitamente: `"O aviso no cupom veio do Mercadinho. Preciso voltar lá e mostrar o cupom para a atendente antes de confrontar o Zé."`.
- [x] **Passo 5:** Ajustar o `GameState` em RAM da sessão ativa no Unity Editor via MCP para refletir exatamente esse passo (apontando o GPS para o Mercadinho primeiro, para falar com a atendente e depois subir ao Bar do Zé).
- [x] **Passo 6:** Executar `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`.

---

## 4. Arquivos Criados ou Modificados

- `Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs` (modificado)
- `docs/ai-plans/2026-10-30_PatrickDotto_fluxo-5-pistas-voltar-mercadinho-depois-bar.md` (criado)
- `docs/ai-memory/CHANGELOG_AI.md` (atualizado)

---

## 5. Validação e Testes Realizados

- Estado em RAM no Play Mode ativo verificado (`temCasaCompleta = true`, `temRecibo = true`, `temRelato = false`, `uvAlvo = (0.38, 0.60)` apontando para o Mercadinho).
- `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1` executados com status `PASS: 97 C# sources covered`.

---

## 6. Próximos Passos e Pendências

- Nenhuma pendência.
