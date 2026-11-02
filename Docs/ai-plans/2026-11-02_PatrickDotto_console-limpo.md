# Avisos de compilação do Console
Autor: @PatrickDotto / Patrick Dotto; Codex. 02/11/2026, Unity 6000.6.0f1 Windows.
Pedido: remover avisos/erros e explicar check-in, sem efetuar commit/check-in.
Contexto: memória/Obsidian, Graphify SantaRitaItemAction, código real e documentação Unity 6000.6 via Context7.
- [x] Trocar chamadas obsoletas FindObjectsByType(FindObjectsSortMode.None) pelo overload sem ordenação.
- [x] Remover campo atribuído mas nunca consultado bolaMarcadaCaiuNestaTacada e referências de escrita/reflexão no QA histórico; regras atuais usam SinuquinhaRules.
- [x] Compilação C# via Roslyn e rsp/referências reais do Unity: exit 0, saída vazia (zero erros/avisos). Console consultado também sem entradas warning/error. Refresh/validate PASS, 112 fontes; diff check exit 0.
Arquivos: SantaRitaItemAction, BarSinucaR4RuntimeVerification, SinucaMinigameUI, Chapter2RuntimeVerification. Não alterar regras/saves nem ocultar warnings por pragma.

Repositório na branch main, sem workspace .plastic na raiz ou projeto: orientar commit/push pelo Git, não configurar outro serviço. Nenhum commit ou push efetuado. Alterações preexistentes preservadas.
