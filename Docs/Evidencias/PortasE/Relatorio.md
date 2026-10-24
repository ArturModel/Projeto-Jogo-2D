# Porta mostra E, mas não abre — 23/10/2026

## Diagnóstico e reprodução

Na sessão do autor: CentroSantaRita, portal `Porta_Porta_SociedadeBeiraRio`, destino/entrada válidos, sem bloqueio de progresso nem modal. A chamada direta em frame do jogo abriu a cena. A leitura isolada do relógio pelo MCP no update Editor não foi usada como prova de falha no loop do jogo.

Reprodução no caminho real: teste posiciona Marcus na entrada, semeia o horário da transição anterior em 0,1 s atrás e envia E pelo Input System, passando pelo `PlayerController.Update`. O prompt encontra a porta correta, mas a versão anterior descarta a tentativa por `Time.unscaledTime - ultimoCorte < 1.5`. O teste executado antes da correção falhou em **E abre SociedadeBeiraRio mesmo logo após outra transição**. Captura de diagnóstico fora do repo também confirmou E recebido no update Dynamic e transição recusada.

Os testes da entrega anterior conferiam principalmente saídas após espera e não reproduziam essa entrada com transição recente. O novo teste cobre ida e volta, sem depender dessa espera.

## Correção

- Portas manuais respondem ao novo pressionamento de E sem espera global entre portas.
- `SceneFader.IsTransitioning` impede que um segundo portal substitua o destino/entrada enquanto o fade acontece.
- Passagens automáticas mantêm a proteção de 1,5 s contra retorno involuntário.
- Estado estático reinicia em `SubsystemRegistration`; relógio recuado não deixa passagem automática presa em um prazo de sessão anterior.
- Progressão, bloqueio durante leitura, destinos, cenas, visuais e saves conservados.

## Plano de verificação

- [x] Reproduzir falha antes da correção pelo E do controlador.
- [x] Verificar sete entradas do Centro e suas saídas no Editor.
- [x] Conferir passagem automática, nova sessão, relógio recuado e proteção durante fade/leitura.
- [x] Executar regressão no Player QA.
- [x] Atualizar Player normal.
- [x] Comparar saves pessoais e restaurar partida em memória.

Fixture declarada: progressão C1 em RAM e reposicionamento de aproximação; o evento E segue o caminho real do jogo. Testes possuem namespace de saves isolado. Nenhum slot pessoal é carregado ou gravado pelo teste.

Editor: [62 checks passaram](Editor/runtime-qa.txt). Não foram modificados bindings, política de foco ou configuração do Input System. A documentação oficial consultada distingue updates Editor/Dynamic e roteamento do teclado à Game View; a prova de falha foi executada no loop Dynamic do jogo.

Player QA: [62 checks passaram](PlayerQA/runtime-qa.txt), [build Windows x64/D3D11 com zero erros](PlayerQA/build-qa.txt). Reproduz o mesmo caso que falhou antes da correção e verifica sete entradas do Centro, saídas, primeira passagem automática, relógio recuado, fade, leitura e ausência de autosave. Este é um teste técnico com fixture, não uma campanha integral.

Player normal: [build concluída com zero erros](PlayerNormal/build-windows.txt). SHA256 de fontes/cenas/dados/texturas/materiais/metas igual ao do Player QA: `869ba61457cb46d4de0f4dbdad28edde2cdf979c220758f72d05bdbe2f1ae9b1`.

Preservação: os cinco arquivos pessoais mantiveram os hashes originais. O snapshot em memória foi restaurado e comparado integralmente antes de retomar a interação, com Marcus em `(3.915522, 4.705001)` no Centro. Depois, E enviado em frames reais pelo Input System abriu SociedadeBeiraRio; fade concluído, Marcus em `(5, 2.4)`, sem modal. Partida devolvida em Play dentro da Sociedade, sem gravar checkpoint. [Registro](preservacao.txt).
