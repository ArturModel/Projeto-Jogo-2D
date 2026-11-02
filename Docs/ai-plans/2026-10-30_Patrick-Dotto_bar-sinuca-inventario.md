# Bar, Sinuca Brasileira e inventário — plano executado

Autor: Patrick Dotto (git config; gh indisponível, login GitHub não confirmado). Data: 30/10/2026, America/Sao_Paulo. Ambiente: branch atualizacao_sinuca, Unity 6000.6.0f1. Status: implementação e validação técnica concluídas, com limite explícito de conferência manual por Firewall.

## Pedido e decisões

Remodelar primeiro o Bar do Zé e compartilhar sua disposição com a sinuca: mesas/cadeiras plásticas, atendimento e bebidas no fundo, TV/rádio, moradores com rotinas, calendário de 2004 e pôsteres contextualizados. Três vistas: diagonal, linha do taco com posicionamento ao redor e superior 2D; Zé idêntico ao ator do bar, reações, mesa/redes e física/efeitos revisados. Autor escolheu **Sinuca Brasileira oficial + bola marcada** em resposta explícita; não manter pontuação simplificada de 4 bolas.

Inventário visível de cinco slots; lanterna no carro, mapa somente na rodoviária C2, coleta/entrega explícita do recibo. Música do bairro em Posto/Encruzilhada e interiores atenuados/abafados. Tapetes encostados à porta, interação de saída só praticamente em cima.

## Diagnóstico

Equipe: PRs de interface/stepper e inicialização, retratos/cadeia de pistas, flora/props C1 e preenchimento de oito interiores C2, Bar/Casa do Avô. HEAD 30d811c. Há extensa alteração preexistente, incluindo cerca de 2406 linhas modificadas em SinucaMinigameUI, novo SinucaTableView e QA. Preservar sem reset/stash indiscriminado.

Consultados INDEX/CURRENT_STATE, visão de arquitetura, skills de arte/narrativa/áudio/QA, direção e produção. Graphify consultado para SinucaMinigameUI. Renderizador atual é projeção em MaskableGraphic, não ambiente Unity 3D real; há somente duas câmeras, spin longitudinal discreto e regra antiga. Uma planta compartilhada deve evitar duas versões divergentes do bar.

MCP inicialmente procurado incorretamente em 8080. Pela UI confirmou-se endereço real 8088, Session Active e Codex Configured. Instância única Projeto Jogo 2D@a7a0c66d831bf259. Play preservado, sem salvar/regenerar cenas. Cliente HTTP local temporário fica fora do repo.

## Checklist

- [x] Investigar mudanças e preservar trabalho preexistente.
- [x] Confirmar escolha de regras com autor.
- [x] Localizar sessão MCP real pelo Editor.
- [x] Alterar música em código: Posto/Encruzilhada/Bairro compartilham fonte; interiores usam volume da casa e filtro gradual de 1800 Hz.
- [x] Fontes/loops/volume/filtro verificados em seis regiões no Editor; aprovação auditiva não presumida.
- [x] Planta compartilhada do bar, mobiliário/atores/iluminação e arte de paredes.
- [x] Três câmeras e entorno correspondente, ator Zé e reações.
- [x] Física/efeitos de branca, redes e regras oficiais com bola marcada.
- [x] Inventário cinco slots e migração compatível, mapa na rodoviária e recibo.
- [x] Soleiras/alcance de saída em todos os interiores.
- [x] QA automatizado de regras/jogabilidade/câmeras, saves isolados e build normal; smoke normal limitado ao menu pelo pedido de Firewall.

## Alterações desta execução

Novos: SantaRitaBarLayout.cs (inclui patronos/host/props), SinucaBrasileiraRules.cs, SinucaSpinSelector.cs, SantaRitaInventory.cs, SantaRitaItemAction.cs e BarSinucaR4RuntimeVerification.cs, fontes Aseprite/PNG e exportador Lua. Modificados: Capitulo1Director/Capitulo2Director, InteracaoCapitulo1, ChegadaEmSantaRita, TransicaoDeZonaTrigger, Interactable, Capitulo2Portal, SantaRitaExitThreshold e MapaInterativoUI. SinucaMinigameUI e SinucaTableView foram ampliados sobre a versão preexistente da equipe; Chapter2RuntimeVerification recebeu a entrada da suíte R4. Procedência em ASSET_SOURCES.csv, saídas textuais selecionadas em .gitignore. Nenhuma cena regravada para gerar a decoração, que é composta em runtime.

A validação encontrou uma repetição infinita da saída pela IA (180 tacadas, 0×0). Correção: escolher posição legal no D com linha livre à bola da vez e saída de defesa com potência proporcional à distância, em vez de tentar converter a saída brasileira. Nova mão física/IA encerrou em 45 tacadas, 36×34. A suíte também corrigiu sua expectativa de retorno do depósito: destino real é EntradaDaCidade, preservado. Consultas MCP durante input foram retiradas da execução e GameView recebe foco explícito.

## Referências de pesquisa

Virtual Pool 4: https://vponline.celeris.com/support/quickstart (câmera ao redor da branca/controle de spin). Regras publicadas por Ases Sinuca: https://www.asessinuca.com.br/regra-sinuca e /regulamento-do-bilhar. Distinguir mesa oficial 2,84 × 1,42 m de mesas menores de bar; não chamar variante popular de regra oficial. Nenhum recurso visual externo baixado/licenciado ainda.

## Validação e pendências

Editor: aquisição/entrega/mapa, snapshot, ausência de autosave, seis regiões musicais, planta compartilhada, três projeções, clique/carga/soltar, disco de contato/presets e cancelamento por foco exercitados. Uma mão completa dirigida pela IA usa física/resolução reais sem injetar pontos. Série de duas vitórias usa fixture separada, com sete perto da caçapa e conversão física/callback. Portas C1/C2 em regressão de E. Build QA, build normal, preservação de saves e índices finais ainda em fechamento. Não equivale a campanha completa legítima ou aprovação artística/auditiva.

## Índices e build QA

Refresh e validate executados com PASS: 103 C#, 1814 nós/5979 arestas, 1908 notas; notas manuais de arquitetura, catálogo e fluxo atualizadas. Build Development QA Succeeded, zero erros; execução externa da suíte iniciada. Editor final: 199 verificações mais encerramento PASS.

## Fechamento — 30/10/2026, 20h (America/Sao_Paulo)

- [x] Player QA final: 200 verificações + encerramento PASS, nenhum FAIL/ERROR; mão física/IA 45 tacadas/36×34, série física/callback, itens, música e 11 saídas. Capturas conferidas após fundo opaco/forro/legendas.
- [x] Build normal Succeeded, zero erros, 226.327.149 bytes; mesmo hash de fontes `a984dd1c00639304fc0363029de7f0e5b0c9ce42e35d3b58a96d150e92be6307`. Hooks de QA excluídos conforme metadata.
- [x] Seis arquivos de saves originais preservados por hash, sem resíduo SRQA desta execução.
- [x] Graphify refresh/validate final PASS, 103 C#, 1814 nós/5980 arestas/1908 notas; documentação e índice atualizados.
- [x] Editor em MainMenu, Play=false, dirty=false.

Limite: build normal abriu menu, mas o aviso do Firewall do Windows cobriu o jogo. A skill computer-use veda agir nesses pedidos; instância de teste encerrada, nenhuma escolha de permissão automatizada. Não homologar cliques de seleção de saves nessa build normal com base nesse smoke. A suíte detalhada passou no executável Development com os mesmos fontes de produção. Campanha humana completa, audição e aprovação artística não foram declaradas. Fluxo recomposto é o bar conectado de EntradaDaCidade; cenas avulsas legadas abertas diretamente no Editor permanecem preservadas. Sem commit/push.

[Relatório final, escopo e reprodução](../Evidencias/BarSinucaR4/Relatorio.md).
