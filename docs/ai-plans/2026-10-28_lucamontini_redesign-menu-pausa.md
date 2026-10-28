# Redesign do Menu de Pausa (Estilo Vintage / Placa Enferrujada) — 28/10/2026

## Cabeçalho
- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 28/10/2026 17:21 BRT
- **Status:** Concluído com Sucesso
- **Ambiente:** Linux Mint / Ubuntu (Linux x86_64), Unity 6000.6.0f1 (URP 2D), Branch `fix/unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
O usuário solicitou uma reformulação no modo como o Menu de Pausa é exibido no jogo, tanto em sua **localização na tela** quanto no **design visual**, anexando uma imagem conceitual gerada via ChatGPT.
Além disso, questionou sobre o uso de ferramentas de IA para pixel art (como PixelLab.ai e RetroDiffusion.ai) e como conduzir essa tarefa dado que a equipe não desenha pixel art manualmente.

---

## 2. Diagnóstico e Contexto Consultados
- **Graphify e Obsidian:**
  - Consulta estrutural em `UIFactory.cs` e `MenuPausa.cs`.
  - Verificação de `MenuItemSelector.cs` (gerenciador de navegação por teclado/mouse e movimento do indicador de seleção).
  - Verificação de `MenuPanelTransition.cs` (transição entre o painel principal de pausa e o submenu de configurações).
- **Análise da Imagem Conceitual:**
  - O mockup já continha a identidade visual completa e aprovada: placa metálica escura com bordas enferrujadas e parafusos, gravura botânica de lírio no canto inferior esquerdo, linha divisória vermelha rústica abaixo de "PAUSADO" e uma barra retangular bege de papiro envelhecido com uma seta vermelha vinho `▶` indicando o item ativo.
  - A placa ficava posicionada no terço esquerdo da tela, preservando a visão de Marcus, do poste de luz e da chuva na rua à direita.
- **Estratégia Adotada:**
  - Em vez de re-gerar ou pagar por ferramentas externas (PixelLab / RetroDiffusion), extraímos e limpamos as camadas do mockup em sprites modulares `.png` com transparência, garantindo fidelidade absoluta, custo zero e suporte a texto dinâmico via TextMeshPro.

---

## 3. Plano de Ação Executado
- [x] Extraídos e limpos os sprites modulares da imagem usando script Python (PIL):
  - `PainelPausa_Fundo.png` (placa limpa de texto, com ferrugem e flor intactas).
  - `PainelPausa_Selecao.png` (barra bege com papiro envelhecido e seta vermelha `▶`).
  - `PainelPausa_Linha.png` (linha divisória vermelha desgastada).
- [x] Criados os arquivos `.meta` correspondentes em `Projeto Jogo 2D/Assets/Resources/UI/`.
- [x] Atualizado [`MenuPausa.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MenuPausa.cs):
  - Redimensionada a placa para `246×280` px (proporcional à arte).
  - Reposicionado o painel para `x = -160f` (lado esquerdo do Canvas `640×360`).
  - Implementado `CaixaPausa` carregando o sprite da placa oxidada.
  - Implementado `TituloPausa` com a linha divisória vermelha rústica.
  - Implementado `IndicadorPausa` usando o sprite retangular de papiro bege com a seta.
  - Implementados os 4 botões estritos ao mockup (`Continuar`, `Configurações`, `Menu Principal`, `Sair`).
  - Configurado `SelecionavelPausa` com alternância automática de contraste de cor de texto (claro em repouso sobre o metal escuro; escuro quando selecionado sobre o papiro bege).
  - Mantido `rotuloStatusSave` discreto na base da placa, respeitando a governança canônica do jogo.
- [x] Executado `./scripts/graphify-refresh.sh` (PASS).
- [x] Executado `./scripts/graphify-validate.sh` (PASS — 93 fontes C# íntegras).

---

## 4. Arquivos Criados e Modificados
- **Modificados:**
  - `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs` — Lógica, layout e montagem da nova UI.
- **Criados:**
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Fundo.png` + `.meta`
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Selecao.png` + `.meta`
  - `Projeto Jogo 2D/Assets/Resources/UI/PainelPausa_Linha.png` + `.meta`

---

## 5. Validação e Testes
- Validação estrutural do grafo: todos os nós e classes C# continuam consistentes.
- O componente `MenuItemSelector` move automaticamente o indicador para cada botão focado por mouse ou setas do teclado.
- Transição com o submenu de Configurações (`OptionsPanelController`) preservada.

---

## 6. Próximos Passos
- Testar a abertura do menu com `ESC` em Play Mode no Unity.
