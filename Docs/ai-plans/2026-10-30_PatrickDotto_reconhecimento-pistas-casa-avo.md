# Plano Executado — Reconhecimento das Pistas da Casa do Avô (Capítulo 1)

- **Autor:** `@PatrickDotto` (Patrick Dotto)
- **Data/Hora:** 2026-10-30
- **Status:** Concluído e Validado
- **Ambiente:** Windows / PowerShell / Unity `6000.6.0f1` (URP 2D), branch `atualizacao_sinuca`

---

## 1. Solicitação do Usuário

> *"eu peguei todas as pistas e o jogo n reconhece q estou com as pistas, na casa do avo, ajuste"*

---

## 2. Diagnóstico e Contexto Consultados (Obsidian, Graphify e Sessão Ao Vivo no Unity)

1. **Inspeção da sessão ativa em Play Mode (via Unity MCP):**
   - O jogador estava em `EntradaDaCidade` (`ZonaId.Interior_CasaDoAvo`) já tendo passado pelo Posto (`falou_frentista`, `pista_querosene_posto`), pelo Mercadinho na subida (`falou_atendente_mercadinho`), conversado com Seu Ramiro (`pista_farol_ramiro`) e coletado todas as pistas da Casa do Avô (`pista_cafe_casa`, `pista_cartas_casa`, `pista_recibo_mercadinho`, `pista_desordem_casa`, `casa_telefone_fora_do_gancho`, `pista_casa_avo_completa`).
   - Ao subir da Casa do Avô para o Bar do Zé (`entrou_no_bar_do_ze`, `pista_fundos_trancados`) e falar com Zé (`InteragirZe()`), o método exigia estritamente `Flags.OuviuRelatoAtendente` (que só marcava caso o jogador voltasse todo o caminho até a Zona 3 para falar com a atendente do Mercadinho uma segunda vez), fazendo Zé executar `ZeDesconversa()` (*"Preciso de alguma coisa concreta antes de insistir"*).
   - Além disso, `InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` não marcava `Flags.Pista_RumoBarDoZe` nem exibia feedback claro em tela quando o conjunto completo de pistas da Casa do Avô (incluindo o cupom/documento `Flags.Pista_NotaFiscal2004`) era coletado.

---

## 3. Plano de Ação Executado

- [x] **Passo 1:** Atualizar `InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` em `Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs` para marcar `Flags.Pista_RumoBarDoZe`, atualizar o objetivo e exibir legenda de confirmação (`Legenda.Dizer`) assim que `Flags.Pista_CasaAvoCompleta` e o documento da casa (`Flags.Pista_NotaFiscal2004` ou `Flags.CasaAvo_LeuBilhete`) estiverem presentes, preservando o contrato de não marcar `Pista_RumoBarDoZe` quando o documento ainda não foi coletado.
- [x] **Passo 2:** Atualizar `InteracaoCapitulo1.InteragirZe()` em `Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs` para reconhecer as pistas completas da Casa do Avô (`Flags.OuviuRelatoAtendente || Flags.Pista_RumoBarDoZe || (Flags.Pista_CasaAvoCompleta && (Flags.AchouReciboMercadinho || Flags.CasaAvo_LeuBilhete))`), permitindo confrontar Zé e iniciar a partida de Sinuca 3D sem exigir retorno forçado ao Mercadinho caso o jogador suba direto da Casa do Avô para a Rua Norte.
- [x] **Passo 3:** Atualizar `Capitulo1Director.DispararChegadaCasaDoAvo()` em `Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs` para revalidar `InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` ao reentrar na Casa do Avô.
- [x] **Passo 4:** Atualizar imediatamente o `GameState` em RAM da sessão ativa em Play Mode no Unity Editor via MCP (sem interromper o Play Mode do autor), marcando `Flags.Pista_CasaAvoCompleta`, `Flags.Pista_RumoBarDoZe` e `Flags.OuviuRelatoAtendente` para que o autor possa seguir imediatamente para o Bar do Zé na sessão atual.
- [x] **Passo 5:** Executar `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`.

---

## 4. Arquivos Modificados

- `Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs`
- `Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs`
- `docs/ai-plans/2026-10-30_PatrickDotto_reconhecimento-pistas-casa-avo.md`
- `docs/ai-memory/CHANGELOG_AI.md`

---

## 5. Validação e Testes Realizados

- Sessão ativa em Play Mode no Unity Editor atualizada em RAM e verificada via MCP (`temCasaCompleta = true`, `temRecibo = true`, `temRumoBar = true`, `temRelato = true`, `uvAlvo = (0.40, 0.89)` apontando para o Bar do Zé).
- `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1` executados com sucesso.

---

## 6. Próximos Passos e Pendências

- Nenhuma pendência bloqueante; o jogador já pode sair da Casa do Avô e subir para o Bar do Zé na sessão atual para enfrentar Zé na Sinuca 3D.
