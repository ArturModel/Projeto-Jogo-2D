# 30/10/2026 — Bar, Sinuca Brasileira e inventário R4

Implementados bar/planta compartilhada, três vistas, regras brasileiras com bola marcada, contato na branca/redes, cinco slots, mapa na Rodoviária e recibo com coleta/entrega. Música expandida/abafada e onze soleiras testadas por E. Player QA final: 200 verificações + PASS. Build normal zero erros, mesmo hash de fontes, QA excluído da DLL; menu observado, cliques limitados por aviso do Firewall não automatizado. Seis arquivos de saves intactos. Graphify/Obsidian atualizados, 103 C#. [Plano executado](../ai-plans/2026-10-30_Patrick-Dotto_bar-sinuca-inventario.md), [evidências e limites](../Evidencias/BarSinucaR4/Relatorio.md).

# Restauração do Comportamento Investigativo Inicial da Casa do Avô — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_restaurar-investigacao-inicial-casa-avo.md`](../ai-plans/2026-10-30_PatrickDotto_restaurar-investigacao-inicial-casa-avo.md)
- **Resumo:** Restaurados `InteracaoCapitulo1.cs` e `Capitulo1Director.cs` exatamente ao estado inicial (`git diff` vazio), mantendo a sutileza investigativa original (`"Descobrir quem escreveu o aviso no verso do cupom."`) sem dicas explícitas entregando o próximo passo. Sessão ativa em Play Mode também restaurada ao objetivo original.

---

# Fluxo Original das 5 Pistas da Casa do Avô (Voltar ao Mercadinho → Bar do Zé) — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_fluxo-5-pistas-voltar-mercadinho-depois-bar.md`](../ai-plans/2026-10-30_PatrickDotto_fluxo-5-pistas-voltar-mercadinho-depois-bar.md)
- **Resumo:** Restaurada em `InteracaoCapitulo1.cs` a progressão original (Casa do Avô → voltar ao Mercadinho com o cupom → Bar do Zé), adicionando aviso explícito em legenda e no objetivo assim que as 5 pistas da Casa do Avô são coletadas (`"Vasculhei as 5 pistas da casa do vô. Preciso voltar ao Mercadinho e mostrar o aviso no cupom para a atendente."`), lembrete direto em `ZeDesconversa()` caso o jogador suba ao Bar antes de falar com a atendente e confirmação clara ao fechar o relato no Mercadinho (`"O homem de capa sentou na mesa do Zé. Hora de subir ao Bar do Zé na Rua Norte."`). Sessão ativa em Play Mode também ajustada em RAM.

---

# Esclarecimento sobre o Fluxo das 5 Pistas da Casa do Avô — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_esclarecimento-5-pistas-casa-avo.md`](../ai-plans/2026-10-30_PatrickDotto_esclarecimento-5-pistas-casa-avo.md)
- **Resumo:** Confirmado ao autor que não faltava examinar nenhum item na Casa do Avô (todas as 5 pistas já haviam sido coletadas) e que, com o ajuste em `InteracaoCapitulo1.cs`, basta examinar as pistas da casa para seguir direto ao confronto com Zé no Bar da Rua Norte sem necessidade de retornar ao Mercadinho.

---

# Reconhecimento das Pistas da Casa do Avô e Progressão Direta ao Bar do Zé — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_reconhecimento-pistas-casa-avo.md`](../ai-plans/2026-10-30_PatrickDotto_reconhecimento-pistas-casa-avo.md)
- **Resumo:** Corrigido o fluxo de conclusão das pistas da Casa do Avô (`InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` e `InteracaoCapitulo1.InteragirZe()` em `Projeto Jogo 2D/Assets/Scripts/InteracaoCapitulo1.cs` e `Capitulo1Director.DispararChegadaCasaDoAvo()` em `Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs`) para que, ao reunir as pistas da casa incluindo o documento/cupom (`Flags.Pista_CasaAvoCompleta` + `Flags.Pista_NotaFiscal2004` / `Flags.CasaAvo_LeuBilhete`), o jogo marque imediatamente `Flags.Pista_RumoBarDoZe`, atualize o objetivo/GPS do mapa para o Bar do Zé, exiba confirmação em legenda na Casa do Avô e permita confrontar Zé na Sinuca sem obrigar um retorno forçado ao Mercadinho caso o jogador já tenha passado por lá na subida. Atualizado também o `GameState` em RAM da sessão ativa em Play Mode no Unity Editor via MCP para destravar imediatamente a partida em andamento.

---

# Restauração Automática da Cena `MainMenu.unity` Após Build e Verificação de Saves Pessoais — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_restaurar-cena-mainmenu-e-saves.md`](../ai-plans/2026-10-30_PatrickDotto_restaurar-cena-mainmenu-e-saves.md)
- **Resumo:** Reaberta a cena `Assets/Scenes/MainMenu.unity` no Unity Editor (que havia permanecido na cena vazia `Untitled` criada por `EditorSceneManager.NewScene` durante `Chapter2BuildVerification.Run()`) e atualizado o bloco `finally` de `Chapter2BuildVerification.Run()` para sempre restaurar `MainMenu.unity` ao concluir builds no Editor. Confirmada a preservação integral dos saves pessoais do autor (`Slot 1` e `Slot 5` na Sociedade Beira-Rio e `Slot 2` na Rua Norte / Bar do Zé).

---

# Implementação e Validação da Sinuca 3D do Bar do Zé (Referência FlyOrDie, Controles Simples, S01–S27) — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_sinuca-3d-flyordie-simples-r1.md`](../ai-plans/2026-10-30_PatrickDotto_sinuca-3d-flyordie-simples-r1.md)
- **Resumo:** Implementada a elevação visual, espacial e física do boss de Sinuca do Bar do Zé (`Prompt_Sinuca_3D_FlyOrDie_Simples_R1`) preservando o ciclo essencial de controles (**mirar, segurar para carregar a força e soltar para tacar**) e as regras canônicas (7 bolas numeradas compartilhadas, 4 pontos/mão, melhor de 3, Bola Marcada `[X]` da 2ª mão em diante e invariante das 7 bolas). Criado `SinucaTableView.cs` (`MaskableGraphic`) com câmeras **Mesa 3D** em perspectiva oblíqua padrão e **Vista Superior** tática (`V` ou botão no HUD) + zoom no scroll, projeção `ScreenToTablePoint`, esferas 3D com números `1..7` e badge `[X]`, taco 3D recuando com a carga e guia de mira discreta até o primeiro contato. Refatorado `SinucaMinigameUI.cs` com física de passo fixo (`120 Hz`), colisão contínua varrida, rebote nos 12 bicos das caçapas, efeito longitudinal opcional na branca (`Normal / Avançar / Recuar`), prática curta opcional antes do duelo, IA de Zé avaliando candidatos no mesmo kernel físico (`~4.2 ms`), áudio por evento físico real e terror psicológico sutil respeitando `MenuPausa.ReduzirEfeitosIntensos`. Validados `S01`–`S27` em EditMode (`Sinuca3DVerification.cs`), regressão C1/C2 (`Chapter2Verification.cs`, `BochaPerspectiveVerification.cs`) e no executável StandaloneWindows64 (`Build/Sinuca3DQA/SantaRita.exe -srSinuca3DQA` com `Sinuca3DRuntimeVerification.cs`), com evidências e capturas salvas em `Docs/Evidencias/Sinuca3D/` e resumo consolidado em `Docs/Sinuca_3D_Execucao.md`.

---

# Resolução de Conflitos Git/VS Code e Avisos/Erros de Inicialização no Console do Unity — 30/10/2026

- **Autor:** @PatrickDotto (Patrick Dotto)
- **Documento do Plano:** [`docs/ai-plans/2026-10-30_PatrickDotto_resolucao-conflitos-git-e-console-unity.md`](../ai-plans/2026-10-30_PatrickDotto_resolucao-conflitos-git-e-console-unity.md)
- **Resumo:** Sincronizada a branch `main` local com `origin/main` (`30d811c`), eliminando os bloqueios de `git pull` causados por resíduos do Plastic SCM (`.plastic/`), quebra de linha em `Kenney-Interface-License.txt` e reimportação de fonte. Diagnosticadas e corrigidas as ocorrências do Console do Unity: (1) em `SantaRitaMcpSetup.ConectarManualmente()`, substituído o `Task.Delay(1500)` fixo por polling real de `IsLocalHttpServerReachable()` e verificação de `IsManagedServerLaunchProcessAlive()`, evitando que `bridge.StartAsync()` tentasse conectar à porta `8088` antes de o servidor Python FastMCP/Uvicorn terminar de instalar dependências e subir; (2) suprimidos com `#pragma warning disable CS0414` os dois campos serializados legados (`esperaAntesDoObjetivo` e `objetivo`) em `ChegadaEmSantaRita.cs`; (3) esclarecidos os avisos únicos de transcodificação de vídeo (`WindowsMediaFoundation`). Validação Graphify/Obsidian com status PASS em 94 scripts C#.

---

# Adição e Harmonização de Sprites nos Cenários (Flora e Props) — 29/10/2026

- **Autor:** @Venturini01 (Ariel Venturini)
- **Documento do Plano:** [`docs/ai-plans/2026-10-29_Venturini01_adicao-sprites-cenarios-flora-props.md`](../ai-plans/2026-10-29_Venturini01_adicao-sprites-cenarios-flora-props.md)
- **Resumo:** Analisado o acervo visual existente em `Assets/Resources/Art/World/` e `Assets/Resources/Art/Props/`. Realizada a substituição de props genéricos provisórios no exterior de C1 (bancos `Bench_Wood` e placas `RoadSign_Blank` substituídos por `BancoPraca.png` e `QuadroAvisos.png`). Adicionados sprites de alta fidelidade regional (Araucárias sulistas `Araucaria.png`, samambaias `Fern.png`, arbustos `Shrub.png`, tufos de capim `GrassClump.png` e canteiros `Flowers.png`) ao longo das 5 zonas de C1 (`EntradaDaCidade`), além de grelhas pluviais de sarjeta e fardos de entrega em `SantaRitaPlaceDressing.cs`. Todos os elementos receberam material lit `WorldLit`, profundidade por Y e suporte a caminhada sob copas com `SantaRitaForeground`. Compilação .NET e validação Graphify concluídas com êxito (`PASS: 94 scripts`).

---

# Leitura de Contexto, Lore e Alinhamento de Arquitetura — 29/10/2026

- **Autor:** @Venturini01 (Ariel Venturini)
- **Documento do Plano:** [`docs/ai-plans/2026-10-29_Venturini01_leitura-contexto-conversa-lore-projeto.md`](../ai-plans/2026-10-29_Venturini01_leitura-contexto-conversa-lore-projeto.md)
- **Resumo:** Concluída a leitura aprofundada dos arquivos do arquivo `Conversa.zip` (histórico de conversas do WhatsApp do grupo UNESC com 1.116 linhas, bíblia consolidada v5.0 de lore com backgrounds, roteiros revisados em PDF e assets compartilhados) e de toda a árvore do projeto Unity 2D (`Projeto Jogo 2D/Assets/`, cenas C1/C2, contratos de persistência e governança). Configurado o ambiente local com `graphifyy==0.9.96` no Python 3.14 do usuário e ajustados os scripts de lifecycle (`graphify-project.py`, `graphify-refresh.ps1` e `graphify-validate.ps1`) para descoberta automática de binários em Windows. Atualizado o catálogo de scripts para 94 fontes C# com a inclusão de `DicaDeTecla.cs`. Validação executada com sucesso (`.\scripts\graphify-validate.ps1` PASS em 94 scripts).

---

# Refinamento Visual e Acessibilidade dos Controles dos Menus — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_refinamento-visual-controles-menus.md`](../ai-plans/2026-10-28_lucamontini_refinamento-visual-controles-menus.md)
- **Resumo:** Executadas as melhorias visuais e de acessibilidade solicitadas. A flor/planta no canto inferior esquerdo foi totalmente removida de `PainelPausa_Fundo.png` por inpainting cirúrgico e microrruído calibrado, unificando a placa de ardósia oxidada sem resquícios. As caixas de seleção (toggles) ganharam moldura bronze com cavidade escura e indicador proporcional âmbar de 8×8 px. As barras (sliders) ganharam trilho visível de 0% a 100% e cursor ergonômico. O seletor de resolução foi transformado em um Stepper Horizontal elegante (`< 1920 x 1080 >`) com suporte a setas e teclas A/D, eliminando a lista suspensa e a barra de rolagem branca. O preview de legendas recebeu moldura estilizada translúcida, extinguindo a tarja preta crua. Validação completa com `.\scripts\graphify-validate.ps1` (PASS em 93 scripts).

---

# Correção de Glifo Unicode do Indicador de Submenus — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_correcao-glifo-indicador-menu.md`](../ai-plans/2026-10-28_lucamontini_correcao-glifo-indicador-menu.md)
- **Resumo:** Diagnosticados e sanados os avisos no console após o teste in-game. O aviso referente ao caractere `\u25B6` (`▶`) vinha do método `Indicador` em `MenuPausa.cs`: a fonte pixel art bitmap `VT323-Regular Bitmap` não contém esse glifo específico no seu atlas, gerando fallback para quadrado `\u25A1`. Foi restaurado o caractere nativo `">"` com `corDestaque` (âmbar vintage), fiel ao protótipo e 100% suportado pela fonte. O aviso de `Color primaries 0` foi esclarecido como metadado benigno de vídeo no Windows Media Foundation, sem relação com os menus. Validação completa com `.\scripts\graphify-validate.ps1` (PASS em 93 scripts).

---

# Redesign do Menu de Configurações e Submenus (Áudio, Vídeo, Legendas) — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_redesign-menu-configuracoes.md`](../ai-plans/2026-10-28_lucamontini_redesign-menu-configuracoes.md)
- **Resumo:** Unificada a identidade visual do Menu de Configurações e seus submenus (Áudio, Vídeo, Legendas) à placa metálica oxidada do Menu de Pausa. O painel de opções foi movido do centro (`x = 0f`) para o terço esquerdo da tela (`x = -160f`, tamanho `246×280` px), reutilizando 100% dos assets validados (`PainelPausa_Fundo.png`, `PainelPausa_Selecao.png` e `PainelPausa_Linha.png`). A transição entre pausa e configurações ocorre suavemente no mesmo quadrante. Categorias adotou botões idênticos aos da pausa com papiro e seta vermelha, e os submenus ganharam layout refinado para largura interna de 192px com indicador `▶` e controles em tons bronze/carvão. Removido o método legado `Caixa`. Validação completa com `.\scripts\graphify-validate.ps1` (PASS em 93 scripts).

---

# Remoção da Faixa Residual Atrás de Configurações — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_remocao-retangulo-configuracoes.md`](../ai-plans/2026-10-28_lucamontini_remocao-retangulo-configuracoes.md)
- **Resumo:** Eliminada definitivamente a faixa retangular bege que permanecia atrás de "Configurações" em `PainelPausa_Fundo.png`. O diagnóstico identificou que o script anterior continha uma trava de flor em `y >= 231`, que classificou erroneamente o canto do papiro como flor. Corrigido o limite (a flor autêntica só inicia em `y = 288`), cobrindo 100% da faixa de `y = 183` a `240` com ardósia lisa e inpaint perimetral, reduzindo a contagem de pixels beges na zona de "Configurações" a exatamente 0. O lírio botânico permanece 100% íntegro. Validação completa com `.\scripts\graphify-validate.ps1` (PASS em 93 scripts).

---

# Remoção de Costuras Retangulares no Menu de Pausa — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_remocao-costuras-fundo-pausa.md`](../ai-plans/2026-10-28_lucamontini_remocao-costuras-fundo-pausa.md)
- **Resumo:** Eliminadas definitivamente as duas costuras retangulares no fundo metálico do Menu de Pausa (o degrau entre Continuar/Configurações e a linha horizontal entre Menu Principal/Progresso não salvo). O inpainting cirúrgico foi aplicado diretamente na placa autêntica via OpenCV Telea, removendo os glifos de texto e unificando a antiga faixa de seleção com amostra de ardósia ajustada à vizinhança e inpainting perimetral de 3px, reduzindo o gradiente das antigas emendas para nível de ruído natural ($1.9$ a $5.4$). Também foi regenerado `PainelPausa_Selecao.png` por inpainting direto dos 1.400 pixels de "Continuar", extinguindo os artefatos de ladrilhamento de 50px que ficavam visíveis ao lado de "Sair". Validação integral com `.\scripts\graphify-validate.ps1` (PASS nos 93 scripts C#).

---

# Fidelidade Visual do Menu de Pausa ao Protótipo Conceitual — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_fidelidade-visual-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_fidelidade-visual-menu-pausa.md)
- **Resumo:** Restaurada a fidelidade visual absoluta do Menu de Pausa ao protótipo conceitual aprovado. Reextraído o sprite `PainelPausa_Fundo.png` em suas dimensões integrais (`456×520` px), recuperando a borda direita oxidada e rebites (23px anteriormente cortados) com síntese de ardósia (0 resquícios de texto) e flor botânica 100% preservada. Criado `PainelPausa_Titulo.png` com o carimbo vintage serifado e linha vermelha rústica. Em `MenuPausa.cs`, ajustado `TituloPausa` para carregar o grafismo fiel, alinhados os botões à esquerda (`TextAlignmentOptions.MidlineLeft` com recuo de 37f da margem do botão) formando coluna uniforme ao lado da seta `▶` do papiro, e centralizado o status de save em `(0f, -105f)` com texto limpo (`"Progresso não salvo"` e `"Jogo salvo"`). Validação com `.\scripts\graphify-validate.ps1` (PASS em 93 scripts).

---

# Sincronização da Branch Remota e Novo Menu de Pausa — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_sincronizacao-branch-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_sincronizacao-branch-menu-pausa.md)
- **Resumo:** Sincronizados com sucesso os commits de `origin/fix/unity6-startup-warnings-errors` na branch de trabalho local. Integrado o redesign final do Menu de Pausa (`fe8c098`: `PainelPausa_Fundo.png`, `PainelPausa_Linha.png`, `PainelPausa_Selecao.png` e novo `MenuPausa.cs`), além da porta 8088 do MCP e remoção do Plastic SCM. Adicionada compatibilidade completa Windows no `SantaRitaMcpSetup.cs` para localizar `uvx.exe` em `~/.local/bin` e iniciar o servidor headless de forma coordenada.

---

# Diagnóstico: Branch de Trabalho e Redesign do Menu de Pausa — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_diagnostico-branch-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_diagnostico-branch-menu-pausa.md)
- **Resumo:** Investigado o motivo pelo qual o jogo exibia o menu de pausa antigo e o aviso de codec do MP4. Confirmado que o trabalho do novo menu de pausa subiu corretamente ao GitHub no commit `fe8c098`, porém sob o nome de branch `origin/fix/unity6-startup-warnings-errors` (com `fix/`), enquanto a branch local ativa foi criada como `unity6-startup-warnings-errors` (sem `fix/`) a partir da `main`. O aviso do Windows Media Foundation foi esclarecido como fallback benigno de metadados de cor.

---

# Desativação do UVC e Ajuste de Inicialização do Unity MCP — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_desativar-uvc-ajuste-mcp.md`](../ai-plans/2026-10-28_lucamontini_desativar-uvc-ajuste-mcp.md)
- **Resumo:** Desativação definitiva do Unity Version Control (Plastic SCM) com transição para `Visible Meta Files` em `VersionControlSettings.asset` e remoção do pacote `com.unity.collab-proxy` de `manifest.json`. No `SantaRitaMcpSetup.cs`, adicionada localização dinâmica de executáveis `uvx.exe` (`~/.local/bin`, WinGet e `.santarita`), injeção dinâmica de PATH no processo do Editor e inicialização coordenada do servidor HTTP headless antes do WebSocket client para permitir conexão autônoma do Unity MCP com Antigravity/Gemini. Validação completa com `.\scripts\graphify-validate.ps1` (PASS nos 93 scripts).

---

# Esclarecimento de Ambiente: Graphify e Obsidian no Windows — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_esclarecimento-ambiente-graphify-obsidian.md`](../ai-plans/2026-10-28_lucamontini_esclarecimento-ambiente-graphify-obsidian.md)
- **Resumo:** Esclarecido ao autor o status do Graphify e do Obsidian no ambiente Windows. O Graphify CLI foi provisionado via `uv tool` com Python 3.12 isolado em `~/.local/bin`, permitindo validação (`PASS`) e consultas (`graphify query`) locais sem necessidade de instalação manual. Confirmado que o cofre do Obsidian já existe completo em `docs/architecture-vault/`.

---

# Verificação de Acesso e Branch Git — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_verificacao-acesso-branch.md`](../ai-plans/2026-10-28_lucamontini_verificacao-acesso-branch.md)
- **Resumo:** Verificação de acesso à pasta do jogo (`Projeto Jogo 2D`) e da branch git ativa (`unity6-startup-warnings-errors`). Ajustada a robustez de detecção do interpretador Python/uv nos scripts PowerShell (`scripts/graphify-refresh.ps1` e `scripts/graphify-validate.ps1`) para contornar o stub do Windows Store (`exit 9009`). Validação completa executada com sucesso (`.\scripts\graphify-validate.ps1` PASS em todos os 93 scripts C#).

---

# Remoção Definitiva dos Resquícios de "Menu Principal" e "Sair" — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_remocao-resquicios-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_remocao-resquicios-menu-pausa.md)
- **Resumo:** Eliminados de forma definitiva os últimos resquícios das palavras "Menu Principal" e "Sair" que permaneciam visíveis no fundo da placa (`PainelPausa_Fundo.png`). A causa foi identificada: o script anterior havia iniciado a varredura inferior em `x = 165` para poupar o lírio, mas o lírio termina em `x <= 85` e os textos começavam em `x = 131`, deixando uma faixa de 34 pixels intocada. Corrigido o início da varredura para `x = 100`, cobrindo 100% da área de texto (reduzindo os pixels residuais a exatamente 0) e preservando todos os 1.083 pixels do lírio botânico. Validação Graphify e Obsidian com status PASS (93 scripts C# íntegros).

---

# Limpeza Definitiva do Fundo do Menu de Pausa — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_limpeza-fundo-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_limpeza-fundo-menu-pausa.md)
- **Resumo:** Eliminados de forma definitiva todos os resquícios de texto (ghost letters) que estavam impressos nos sprites do fundo da placa (`PainelPausa_Fundo.png`) e na barra de seleção (`PainelPausa_Selecao.png`). Reconstruída a textura densa de ardósia escura com varredura contínua sobrepondo 100% da área de texto (reduzindo pixels residuais de 4.819 para 0), preservando integralmente o desenho botânico do lírio, a moldura oxidada com parafusos e a seta vermelha ▶ sem nenhum resquício da letra "C". Validação Graphify e Obsidian com status PASS (93 scripts C# íntegros).

---

# Redesign do Menu de Pausa (Estilo Vintage / Placa Enferrujada) — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_redesign-menu-pausa.md`](../ai-plans/2026-10-28_lucamontini_redesign-menu-pausa.md)
- **Resumo:** Implementado o novo design e posicionamento do Menu de Pausa em conformidade com o mockup de arte aprovado pela equipe. Extraídas e limpas as camadas do mockup em sprites modulares (`PainelPausa_Fundo.png`, `PainelPausa_Selecao.png` e `PainelPausa_Linha.png`) com transparência e metadados de Sprite 2D. Atualizado `MenuPausa.cs` para ancorar a placa no terço esquerdo da tela (deixando Marcus e a chuva visíveis à direita), renderizar a placa de metal oxidado com a flor, a linha divisória vermelha, 4 botões estritos ao mockup (`Continuar`, `Configurações`, `Menu Principal`, `Sair`) e a barra de seleção em papiro envelhecido com a seta vermelha que desliza para o botão focado, com contraste automático de cor de texto. Validação Graphify e Obsidian com status PASS (93 scripts C# íntegros).

---

# Diagnóstico e Resolução de Conflito de Porta (8080) MCP Unity — 28/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-28_lucamontini_diagnostico-conflito-porta-8080-mcp.md`](../ai-plans/2026-10-28_lucamontini_diagnostico-conflito-porta-8080-mcp.md)
- **Resumo:** Diagnosticada a causa do erro de conexão do MCP no Unity: a porta `8080` estava sendo ocupada pelo backend Java Spring Boot (`BffKickstartApplication`, PID 124682) do desenvolvedor. A sonda TCP do Unity detectava a porta aberta pelo Spring Boot, assumia falsamente que o servidor MCP estava pronto e tentava handshake WebSocket em `/hub/plugin`, recebendo HTTP 404 da aplicação Java. Alterada a porta padrão do MCP em `SantaRitaMcpSetup.cs` para a porta dedicada `8088`, eliminando qualquer colisão com servidores locais de trabalho. Validação Graphify e Obsidian com status PASS.

---

# Correção de Race Condition e Suporte Multiplataforma MCP Unity — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_correcao-race-condition-mcp-linux.md`](../ai-plans/2026-10-27_lucamontini_correcao-race-condition-mcp-linux.md)
- **Resumo:** Refatorado `SantaRitaMcpSetup.cs` para suportar nativamente Linux/macOS e Windows. Eliminada a condição de corrida (*race condition*) que causava o erro vermelho `[WebSocket] Connection failed` e aviso `Failed to start MCP transport: Http` na abertura do Unity. O bootstrap agora apenas ajusta o ambiente (`~/.local/bin/uvx`, separador `:`, remoção de `.exe`) e delega a conexão ordenada ao `HttpAutoStartHandler` nativo do pacote MCP. Validação Graphify e Obsidian com status PASS.

---

# Correção de Erros e Avisos de Abertura no Unity 6 (Linux) — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_correcao-warnings-e-erros-unity6.md`](../ai-plans/2026-10-27_lucamontini_correcao-warnings-e-erros-unity6.md)
- **Resumo:** Criação da branch isolada `fix/unity6-startup-warnings-errors`. Resolvidos avisos de API obsoleta `FindObjectsSortMode` (CS0618) no Unity 6, campos não lidos (CS0414), diretiva `DEVELOPMENT_BUILD` (UAC0009 migrada para `DEBUG`), aviso UAC0005 no MCP setup, metadados da fonte TextMeshPro (v4), transcodificação de vídeo para Linux e eliminação dos erros críticos e NullReferenceExceptions do Plastic SCM / Unity Version Control (configurado `m_Mode: Visible Meta Files`, removido `com.unity.collab-proxy` e pasta residual `.plastic/`). Validação completa Graphify e Obsidian com status PASS.

---

# Resolução de Espelho APT, Instalação do Git LFS e Checkout de Assets — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_instalacao-git-lfs-e-pull-assets.md`](../ai-plans/2026-10-27_lucamontini_instalacao-git-lfs-e-pull-assets.md)
- **Resumo:** Resolução de erro HTTP 403 do espelho APT `mirror.uepg.br`. Baixado pacote oficial diretamente de `archive.ubuntu.com`, extraído e instalado `git-lfs` em `~/.local/bin/git-lfs`. Inicializado LFS (`git lfs install`), baixados 1.438 objetos (167 MB) via `git lfs pull` e realizado checkout de 1.769 objetos (172 MB) via `git lfs checkout`. Texturas e áudios validados como binários reais intactos. Projeto 100% pronto para abertura no Unity 6000.6.0f1. Validação de governança Graphify e Obsidian com status PASS.

---

# Verificação de Branch e Pré-Requisitos para Abertura no Unity — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_verificacao-branch-e-pre-requisitos-unity.md`](../ai-plans/2026-10-27_lucamontini_verificacao-branch-e-pre-requisitos-unity.md)
- **Resumo:** Verificação da branch ativa (`main`) e auditoria de pré-requisitos antes de abrir o Unity. Detectada ausência do pacote `git-lfs` no Linux Mint com assets binários presentes apenas como ponteiros de ~130 bytes; estruturado passo a passo de correção (`sudo apt install git-lfs`, `git lfs install`, `git lfs pull`) para evitar corrupção de importação na biblioteca do Unity 6000.6.0f1. Validação de governança Graphify e Obsidian concluída com PASS.

---

# Auditoria Pós-Merge, Governança Reforçada e Histórico de Planos — 27/10/2026

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Documento do Plano:** [`docs/ai-plans/2026-10-27_lucamontini_merge-main-auditoria-governanca.md`](../ai-plans/2026-10-27_lucamontini_merge-main-auditoria-governanca.md)
- **Resumo:** Pull da `main` realizado após merge de Patrick Dotto. Análise de diferenças confirmou que nenhum código ou asset foi perdido (C1 + C2 intactos com 93 scripts). Corrigida a diluição das regras de governança que havia ocorrido no merge: restaurado checklist obrigatório em `AGENTS.md` e regras de agente (`.agents/rules/`). Implementado sistema obrigatório de registro de planos executados por prompt em `docs/ai-plans/` contendo autor do GitHub e data. Graphify local atualizado para `0.9.96` e validado com sucesso (`./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` PASS).

---

# Integração ao progresso local — 26/10/2026

Resolução do merge Graphify/Obsidian sobre main local `86b7c02`: união de gitignore, preservação das regras de produção, atualização da memória/cofre para C2 até Bocha e consequência, inventário dos 93 C#, índice AST limpo e ferramentas portáveis com validação real e propagação de falhas. Escola adiada, retornos C1, saves/slots, lanterna F, escada/ambientes e correção E preservados. Nenhum arquivo Unity alterado; licença de áudio preexistente mantida fora do merge. [Resultado e testes](../GRAPHIFY-INTEGRACAO-C2.md).

---

> Registro histórico da branch de preparação, baseada em C1 anterior. Alegações antigas de completude/compatibilidade não comprovam o estado atual. Para a integração ao C2, consulte [estado atual](CURRENT_STATE.md) e o relatório de integração.

# Registro de Alterações por Agentes de IA (CHANGELOG_AI)

**Atualizado em:** 26/10/2026
**Por:** Antigravity AI
**Base:** Verificado nos logs de execução e Git history

---

## [2026-10-26] — Alinhamento Integral com Roteiro Universal v2.0 e Prompt Mestre v2 (Multiplataforma Windows + Linux)

### Objetivo da Tarefa
Verificar os documentos de referência do usuário (*Roteiro Universal v2.0* e *Prompt Mestre v2*) e incorporar todos os requisitos faltantes para que os colegas (especialmente em Windows PowerShell e múltiplos agentes como Claude Code, Codex e Cursor) tenham suporte completo.

### Arquivos Criados / Modificados
- **Scripts PowerShell para Windows (`scripts/*.ps1`):**
  - `scripts/graphify-setup.ps1`, `scripts/graphify-refresh.ps1`, `scripts/graphify-validate.ps1`.
- **Governança e Segurança (`AGENTS.md`, `CLAUDE.md`, `.graphifyignore`, `.gitignore`):**
  - Adicionados marcadores `<!-- BEGIN/END memoria-projeto -->` em `AGENTS.md`, diretiva `@AGENTS.md` em `CLAUDE.md`, regras de segurança contra prompt injection, distinção CLI vs. Skill e exclusão de segredos/certificados (`.env*`, `.pem`, `.key`, `.p12`, `.pfx`, `**/.obsidian/workspace*.json`).
- **Documentação (`README.md`, `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`):**
  - Adicionados comandos de instalação via `winget` / PowerShell no Windows, matriz de integração por agente e tabela de solução de problemas.

### Validações Realizadas
1. Execução de `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` (0 erros).
2. Commit e push atualizados na branch `chore/graphify-obsidian-setup`.

---

## [2026-10-26] — Detalhamento Completo dos 44 Scripts no Obsidian/Graphify, Criação do AGENTS.md e Publicação da Branch no GitHub

### Objetivo da Tarefa
Analisar minuciosamente todos os arquivos do projeto do jogo (44 scripts C# / ~15.010 linhas, 5 cenas, 9 zonas/interiores), documentar tudo em detalhes no Graphify e no cofre do Obsidian, criar o arquivo `AGENTS.md` (e `CLAUDE.md`) para obrigar qualquer IA a buscar no Graphify/Obsidian no início de todo prompt e atualizar ambos ao final, atualizar o `README.md` com o passo a passo de instalação/uso para os colegas e publicar a nova branch no GitHub.

### Arquivos Criados / Modificados
- **Governança de Agentes na Raiz:**
  - `AGENTS.md`: Instruções universais obrigatórias de pré-busca (Graphify + Obsidian + `docs/ai-memory/`) no início de cada prompt e atualização/validação ao final.
  - `CLAUDE.md`: Espelho de governança para compatibilidade nativa com Claude Code.
  - `.agents/rules/project-context.md`: Atualizado para exigir leitura das novas notas manuais do Obsidian e execução do ciclo completo em todo prompt.
- **Documentação Detalhada no Cofre Obsidian (`docs/architecture-vault/`):**
  - `docs/architecture-vault/manual/Catalogo_Completo_Scripts.md`: Inventário completo de todos os 44 scripts C#, suas classes, structs, enums e responsabilidades, conectados via wikilinks às notas geradas.
  - `docs/architecture-vault/manual/Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`: Mapa completo das 5 cenas Unity, das 9 zonas/interiores do `Capitulo1Director`, coordenadas UV do radar e cadeia de investigação (`Flags`).
  - `docs/architecture-vault/manual/Guia_Trabalho_Com_IA.md`: Guia prático de comandos e fluxo de trabalho da equipe com IA.
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md` e `manual/README.md` e `docs/architecture-vault/README.md`: Expandidos com todos os 7 subsistemas e instruções multiplataforma.
- **Memória Persistente e Guia do Repositório:**
  - `README.md`: Adicionada seção completa *"Desenvolvimento com IA: Graphify + Obsidian + Memória Persistente"* explicando instalação (`uv tool install graphifyy`, abertura do vault no Obsidian) e fluxo automático com IA.
  - `docs/ai-memory/ARCHITECTURE.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/ai-memory/CHANGELOG_AI.md`: Atualizados com o inventário integral dos 44 scripts.

### Validações Realizadas
1. Execução de `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh` com 0 erros.
2. Nenhum arquivo de código C# ou cena do Unity foi alterado indevidamente pela documentação.
3. Commit e push da branch `chore/graphify-obsidian-setup` para o repositório remoto no GitHub.

---

## [2026-10-26] — Setup Inicial: Antigravity + Graphify + Obsidian

### Objetivo da Tarefa
Estruturar o projeto com o grafo de conhecimento Graphify, cofre Markdown navegável para Obsidian e memória persistente em disco para que futuros agentes de IA operem com contexto preservado e leitura prévia estruturada.

### Arquivos Criados / Modificados
- **Configurações e Governança:**
  - `.graphifyignore`: Filtro de segurança descartando 1.001 imagens e 18 vídeos da indexação em massa.
  - `.agents/rules/graphify.md`: Regra nativa do Antigravity para priorização do grafo.
  - `.agents/workflows/graphify.md`: Definição de workflow `/graphify`.
  - `.agents/rules/project-context.md`: Regra de governança de contexto e preservação do código.
- **Grafo e Índices (`graphify-out/`):**
  - `graphify-out/graph.json`, `graphify-out/GRAPH_REPORT.md`, `graphify-out/graph.html`.
- **Cofre Obsidian (`docs/architecture-vault/`):**
  - `docs/architecture-vault/README.md`, `manual/` e `generated/` (`graph.canvas` + notas atômicas).
- **Memória Persistente (`docs/ai-memory/`):**
  - `INDEX.md`, `PROJECT_CONTEXT.md`, `ARCHITECTURE.md`, `DECISIONS.md`, `CURRENT_STATE.md`, `CHANGELOG_AI.md`.
- **Scripts de Operação (`scripts/`):**
  - `graphify-setup.sh`, `graphify-refresh.sh`, `graphify-validate.sh`.
- **Documentação:**
  - `docs/PLAN-SETUP-GRAPHIFY-OBSIDIAN.md`, `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md`.

## 31/10/2026 — Bar do Zé: identidade 2D preservada

Correção expressa após rejeição da estética Ref7: exploração com sprites e WorldLit originais, sinuca com as mesmas fontes de pixel art/UV em perspectiva. Zé e frequentadores sem corpos em blocos; móveis nativos, banco/placas/foto/taqueira/engradado/tapete/bandeira e cuia com fontes Aseprite. Núcleos de física, regras e saves preservados. [Plano executado](../ai-plans/2026-10-31_PatrickDotto_bar-identidade-2d.md); resultados e limites no [relatório](../Evidencias/BarIdentidade2D/Relatorio.md).


## 31/10/2026 — Sinuquinha de quinze bolas, teclado e carga na pausa

SinuquinhaRules substitui a pontuação de sete bolas por grupos lisas/listradas ou 1–7/9–15 e preta final. Branca pronta, buffers/IA de dezesseis bolas, texturas numeradas/rolamento, guia da colorida; marcada reativada nas mãos 2/3 no complemento do autor, com X e restauração da formação ao cair. A/D contornam, setas miram, lista de encaçapadas e ESC → Carregar Jogo com cinco slots explícitos/cancelamento pausado. Bar sem tapete sob a mesa, capacho visível, escala de TV/geladeira/cadeiras/mesa de calçada; leitura tipográfica, bandeira detalhada e caminhada com apoios 0/4. Referências de rim/áudio/sala recuperadas após reload.

Editor e Player QA: 279 verificações cada + encerramento PASS. Builds QA/normal zero erros e mesmo hash; QA/autoria excluídos da DLL final. Seis saves preservados na rodada final e RAM mais recente restaurada; caches TMP posteriores à build documentados. Graphify/Obsidian atualizados: 111 C#. [Plano executado](../ai-plans/2026-10-31_PatrickDotto_sinuquinha-15.md), [resultados/limites](../Evidencias/Sinuquinha15/Relatorio.md). Sem aprovação artística presumida, commit ou push.


## 01/11/2026 — Recuperação de arquivos ausentes / Safe Mode
Recuperados somente os 640 arquivos ausentes preservados no stash, sem sobrescrever existentes ou consumir o stash. [Plano e validação](../ai-plans/2026-11-01_PatrickDotto_recuperar-safe-mode.md).


## 01/11/2026 — Áudio C2 / LFS
Trilhas Centro/Cemitério/Bocha e recuperação LFS: [plano e limites](../ai-plans/2026-11-01_PatrickDotto_audio-c2-lfs.md).

Fechamento AudioC2: MCP recuperado; Player QA 137 verificações + PASS final; builds finais sem erros e mesmo hash; seis saves intactos, RAM/posição restauradas, índices 112 fontes. Limites e evidências no plano acima.

01/11 — Inventário sem fichas e passada de corpo inteiro: [plano](../ai-plans/2026-11-01_PatrickDotto_itens-passada.md). Validação em andamento.

Fechamento itens-passada: 141 verificações + PASS; builds e hash iguais, índices 112, saves/RAM preservados. Avaliação artística reservada ao autor.

01/11 — Revisão de proporções e moldura: [plano](../ai-plans/2026-11-01_PatrickDotto_proporcoes-inventario.md).

Proporções: encerrado com QA final PASS, builds sem erros/hash igual, refresh/validate 112, saves/RAM intactos.

02/11 — [Bar, rotinas e passada](../ai-plans/2026-11-02_PatrickDotto_bar-rotinas-passada.md): implementação em validação.

Bar-rotinas-passada concluído tecnicamente: QA 159 verificações + PASS, builds sem erros/hash igual, térmica/cuia conferidas nas duas visões; índices 112, saves preservados. Rodadas intermediárias e limitações registradas no plano.

02/11 — [Marcus ciclo limpo e alpha](../ai-plans/2026-11-02_PatrickDotto_marcus-ciclo-limpo.md): em validação.

Marcus ciclo limpo concluído tecnicamente: 33 verificações + PASS, builds sem erros/hash igual, exportação reproduzida em sandbox, índices 112, saves preservados.

02/11 — [Lanterna de costas](../ai-plans/2026-11-02_PatrickDotto_lanterna-costas.md), correção pontual em validação.

Lanterna-costas concluída: captura e QA PASS, builds sem erros/hash igual, índices 112 e saves intactos.

02/11 — [Console limpo](../ai-plans/2026-11-02_PatrickDotto_console-limpo.md): APIs obsoletas e campo não utilizado removidos.


02/11 — [Atlas cartográfico](../ai-plans/2026-11-02_PatrickDotto_atlas-cartografico.md): levantamento real de 19 regiões/38 acessos, camadas/plantas, marcador e investigação por estados existentes, sem GPS ou ilustração antiga na UI. QA Editor/Player final PASS; builds normais/QA sem erros e mesmo hash; 6 arquivos pessoais intactos. Fontes/limites em [relatório](../Evidencias/Atlas/Relatorio.md) e [arquitetura](../Atlas_Arquitetura.md). Não inferir aprovação artística ou campanha integral.


03/11 — [Caderno e paisagem](../ai-plans/2026-11-03_PatrickDotto_caderno-paisagem.md): scroll dos painéis isolado, fichas só adquiridas/ordenadas sem cupom, título Mapa da Cidade e paisagem decorativa, livro independente com escrita manuscrita e reflexões por conhecimento adquirido. Avisos sem repetição/carga falsa e sem autosave. Validação final em consolidação; [relatório](../Evidencias/Caderno/Relatorio.md).


Fechamento caderno/paisagem: testes específicos (30) e Atlas (237) PASS em Editor e Player; builds finais sem erros e hash igual, 120 C# indexados. Sessão RAM/posição/zona/indicador restaurados, seis arquivos pessoais intactos, cenas/configurações sem diff. Corpus final com 64 textos/variantes; original do diário preservado. Alterações não staged, sem commit/push. Limites e evidências no relatório Caderno.

TMP gravou cache VT323 após restauração; único asset posterior às builds. Hash do Assets atual difere do fingerprint comum das duas builds por esse cache, sem mudanças posteriores em código/cenas/corpus/arte. Detalhe no relatório.


04/11 — Priorização documental de exploração: Mila/tocador, Lia/rodoviária e orelhões primeiro; dependências locais separadas da reforma geral. PDF reorganizado e lista em Docs/Ideias_Exploracao_Ordem_Producao.md. Plano: ../ai-plans/2026-11-04_PatrickDotto_prioridades-exploracao.md. Implementação ainda pendente.

04/11 — Proposta Mila/fitas para discussão antes de implementar: Docs/Proposta_Mila_Fitas.md; plano 2026-11-04_PatrickDotto_proposta-mila-fitas.md. Produção vocal não iniciada; nenhuma ferramenta TTS conectada.


04/11 — Mila/fitas, bolsa e caderno: empréstimo/reparo contextual, quatro fitas físicas/transcrições, bolsa de vinte espaços com cinco atalhos, índice manuscrito e marcador por ID. 32 checks + PASS Editor e Player, builds finais/hash igual. Dublagem final pendente: ElevenLabs autenticado privado, candidata Jessica rejeitada; pesquisa e duas audições Qwen oficiais, Audio Lens instalado 12/12. [Plano executado](../ai-plans/2026-11-04_PatrickDotto_mila-inventario-caderno.md), [relatório](../Evidencias/MilaInventario/Relatorio.md), [pesquisa vocal](../Vozes_Pesquisa_E_Producao.md). Sem autosave/reconstrução/commit/push.


04/11 — Revisão das audições pelo autor: duas candidatas Qwen também REJEITADAS (artificialidade/idade), faltaram quarto/Marcus ao fundo. Direção agora exige prova de cena completa com vozes em posições distintas. Base real de quarto/foley CC0 preparada em prévia privada, renderer validado; elenco completo NÃO produzido: Lucas/Marcus retornaram erro na demo, Voice Design do AI Studio exige chave paga. Sem compras, Assets ou saves alterados. [Plano e bloqueios](../ai-plans/2026-11-04_PatrickDotto_fitas-cena-quarto.md); [fontes/evidências](../Evidencias/MilaInventario/Revisao_Cena_Quarto.md).

04/11 — Pedido posterior de amostra única: cena com três tomadas ElevenLabs v3 e ambiente/foley renderizada em prévia privada; elenco ainda não aprovado. Plano: ../ai-plans/2026-11-04_PatrickDotto_amostra-fita-quarto.md. Sem publicar no jogo.

04/11 — Amostra R2: novas tomadas Júlia/Lucas, Marcus chamado pelo nome na porta; REC/cadeira/quarto aumentados e tratamento mono de cassete com chiado. Prévia privada, sem aprovação/publicação. Plano ../ai-plans/2026-11-04_PatrickDotto_amostra-fita-r2.md.

04/11 — Autor forneceu roteiro R3 e rejeitou timbre adulto da R2. Oito falas registradas literalmente em Tools/Audio/quarto_roteiro_r3.json; teste juvenil Qwen falhou. Sem nova amostra adequada nem publicação. Plano ../ai-plans/2026-11-04_PatrickDotto_roteiro-fita-r3.md.

06/11/2026 — Migração para C:/Temp/SantaRita com Git independente, main atualizada, branch melhoria-minigames e alterações locais da bocha/menu. Plano: ../ai-plans/2026-11-06_BBCONTTI_migracao-santa-rita.md. LFS completo; Unity ainda não validado nesta cópia.


06/11/2026 — Sinuca em primeira pessoa: WASD, mouse capturado/Esc livre, aproximação à branca, mãos e taco 3D/carregado, Zé circulando e preparando a tacada; HUD fino e física/puxada/som revistos. Compilação e QA numérico registrados, sem aprovação visual/auditiva ou Player; Graphify indisponível. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-imersiva.md. Direção de caminhada substitui o default de câmera arcade. Saves e cenas preservados.


06/11/2026 — Direção mais recente da sinuca: Zé voltou a ficar parado e sem taco, jogada apenas simulada. Mãos são billboards; ocultas ao caminhar (fica só taco). Apoio mantém arte aceita; overlay URP de primeiro plano evita corte pelo tampo e empunhadura foi aproximada/rotacionada para expor antebraço. Validação visual da overlay ainda pendente; fonte atual prevalece sobre plano anterior de NPC móvel. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-primeiro-plano.md.


06/11/2026 — Empunhadura refeita após nova referência: pegada fechada vista pelo dorso/de cima, pulso curto; versão de palma/dedos por baixo rejeitada e não publicada. Pivô e rotação seguem eixo projetado do taco; dorso fica à frente do eixo e pulso termina com dissolução em pixels. Apoio mantém PNG/pose. 0 erros C#, 22 QA de rig; shader/render e aprovação artística in-game pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-fechada.md.


06/11/2026 — Autor aprovou a última dupla ImageGen após correção só do apoio (ponte aberta próxima à original). Atlas MaosMarcus.png integrado, contato do apoio no eixo sem offset lateral e máscaras complementares de dorso/polegar em profundidades distintas; empunhadura segue taco. 0 erros C#, 55 verificações numéricas/assets PASS. Play/HLSL/render pendentes: MCP no_unity_session e CUA sem acesso. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-maos-aprovadas.md.


06/11/2026 — Apoio da sinuca reduzido .36 → .30m (16,7%) a pedido do autor; empunhadura desenhada após taco com ZTest Always apenas na própria silhueta recortada, evitando haste atravessar dorso. Apoio mantém profundidade normal e contato. 0 erros C#, 56 checks numéricos/assets PASS; render nativo pendente. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-apoio-menor-oclusao.md


06/11/2026 — Empunhadura do taco espelhada para mão esquerda, pulso -12° em torno do mesmo contato; apoio preservado. Atlas não regenerado. 56 checks numéricos/assets PASS, 0 erros C#/17 avisos CS0649; render/Play pendente. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-pegada-esquerda.md


06/11/2026 — Autor pediu mão esquerda original vista de cima, polegar quase oculto. Novo MaoEsquerdaTaco.png ImageGen integrado, sem mirrorX/roll corretivo -12°; apoio preservado no atlas. 57 checks numéricos/assets PASS. Compilação geral bloqueada por CS0246 SinucaZeComments ausente em mudanças concorrentes de SinucaMinigameUI.cs; Play/render pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-esquerda-desenhada.md


06/11/2026 — Sinuca: trilha .31 → .23, impactos taco/bolas destacados, Zé reage a conversões/faltas/marcada e proximidade da vitória. Legenda inferior central fonte 16, escrita progressiva, leitura/fade; murmúrios sintéticos originais em fonte dedicada, pausa/foco/efeitos respeitados. 27 testes de fala + 46 de mecânicas PASS; Assembly-CSharp 0 erros/17 avisos CS0649. CS0246 transitório SinucaZeComments de edição concorrente resolvido. Render/audição/Player pendentes (MCP sem sessão). Refresh Graphify falha por CLI ausente; validate com hashes antigos. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-falas-audio.md.


06/11/2026 — Dupla reunida em MaosMarcusUnificadas.png ImageGen para mesma pele/estilo; nova pegada superior espelhada na arte, apoio mantido. Apoio reduzido mais 30% (.30 → .21m) sem mudar contato. 57 checks numéricos/assets PASS, compilação geral voltou a passar 0 erros/17 CS0649. Play/render pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-atlas-unificado-apoio-30.md


06/11/2026 — Áudio menu/sinuca: AudioSettingsController sincroniza ambos os painéis ao abrir/alterar, aplica sliders a todos os mixers e preserva zero/valores baixos (migração de reset removida). Master só no AudioListener; defaults ausentes música .35/efeitos 1; música de mixer .5 com duck .35 na sinuca, trilha direta da sinuca .10, impactos reforçados. 22 testes isolados + 8 checks nativos Unity PASS; compilação 0 erros/17 avisos CS0649, preferências da fixture restauradas. Graphify indisponível/hashes antigos; audição/Player pendentes. Plano: ../ai-plans/2026-11-06_BBCONTTI_audio-global-sinuca.md.


06/11/2026 — Áudio dividido em Geral/Música/Fala/Chuva/Taco e bolas/Outros efeitos nos dois menus. Novas preferências independentes audio_fala/audio_chuva/audio_impactos; chuva fora do SFX, voz de Zé/fitas e impactos com ganhos próprios. Passo discreto de madeira na sala da sinuca (.065, cada .62 m de movimento real), silencioso parado/colidindo. 29 testes de configurações + 18 checks Unity + 27 de fala + 75 de movimento PASS; 0 erros/16 CS0649. Preferências da fixture restauradas, sem cenas/saves alterados. Audição/layout visual/Player pendentes; Graphify indisponível. Plano: ../ai-plans/2026-11-06_BBCONTTI_categorias-audio-passos.md.

06/11/2026 — Sinuca: caçapas recortadas em todos os sólidos, iluminação focal sombria, sprites e mãos iluminados; cilindros/redes otimizados. 624 amostras de passagem sem obstrução, 117 checks numéricos PASS, shaders/console sem erros. Autor aprovou apresentação; Player/FPS pendentes, Graphify ausente. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-luz-cacapas.md.

06/11/2026 — Checkpoint 7cb8779e enviado à branch melhoria-minigames antes da reforma. SinucaRoom3D.Furnishings adiciona móveis sólidos e arquitetura/luzes secundárias; apoio de objetos em tampos físicos. QA Editor/limites em ../ai-plans/2026-11-06_BBCONTTI_sala-sinuca-reformulada.md. Graphify indisponível.

06/11/2026 — Remoção dos assentos vazios e dos obstáculos invisíveis correspondentes na sinuca 3D, conforme pedido do autor. Plano ../ai-plans/2026-11-06_BBCONTTI_sinuca-sem-cadeiras-vazias.md.

06/11/2026 — Sinuca: bola falsa visual separada do solver, exame por E, batidas e inspeção sob a mesa; legenda compartilhada Marcus/Zé com murmúrio Marcus mais grave (pitch .77). Impacto usa Foley/door (Kenney CC0). Bloco de água corrigido; poça rejeitada pelo autor e removida em favor de três gotas pequenas. Console sem erros C#; aprovação visual final/audição/campanha/Player pendentes. Graphify indisponível e índice desatualizado. Plano: ../ai-plans/2026-11-06_BBCONTTI_sinuca-percepcoes.md.
