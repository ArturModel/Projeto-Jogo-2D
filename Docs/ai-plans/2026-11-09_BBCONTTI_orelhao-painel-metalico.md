# Redesign diegético do orelhão — painel metálico e telecomunicações anos 2000

- Autor: @BBCONTTI — BBCONTTI
- Data: 09/11/2026, America/Sao_Paulo
- Ambiente: Windows / Unity 6000.6.0f1 / branch melhoria-minigames
- Status: implementado e compilado com sucesso (zero erros)

## Solicitação do usuário

O usuário enviou uma captura da interface anterior do orelhão in-game e solicitou:
> "vamos refazer essa UI do orelhão pra não ficar tão no padrão da unity, ficar de um modo bem criativo"
Posteriormente aprovou a **Opção 1**: Painel frontal metálico diegético de orelhão brasileiro clássico (aço escovado inoxidável, visor LCD verde-fósforo, cartão telefônico indutivo e teclas metálicas).

## Diagnóstico e contexto consultados

- Memória persistente (`INDEX.md`, `CURRENT_STATE.md`, `PROJECT_CONTEXT.md`).
- Cofre Obsidian (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`).
- Histórico dos planos anteriores do dia (`2026-11-09_BBCONTTI_inspecoes-orelhao-em-cena.md`).
- Regras de domínio canônicas: Save manual exclusivo no orelhão em RAM (`GameState.cs`), persistência somente via `CheckpointSystem.SaveSlot()` / `Save()` em 5 slots; Marcus como âncora; PPU 32 e Canvas 640x360.
- `PayphoneCheckpoint.cs`, `PayphoneJogoUI.cs`, `PayphoneRegistroUI.cs`, `CameraFollow.cs`, `UIFactory.cs` e `SantaRitaInventory.cs`.

## Plano de ação executado

- [x] Desenvolver uma interface diegética completa de telecomunicações públicas brasileiras (2004) em `PayphoneJogoUI.cs`.
- [x] Criar geradores procedurais de sprites pixel-perfect (Point filter, 9-slice) diretamente em C# sem dependências externas:
  - `ObterSpriteGabinete()`: Aço escovado inoxidável com bordas chanfradas e vinco metálico.
  - `ObterSpriteParafuso()`: Parafusos metálicos de fenda cruzada nos 4 cantos do gabinete.
  - `ObterSpriteVisorLCD()`: Moldura plástica rebaixada e tela verde-oliva com scanlines horizontais de LCD.
  - `ObterSpriteTecla()`: Teclas 3D em relevo prensado de metal escovado com chanfros de luz e sombra.
  - `ObterSpriteCartao()`: Cartão telefônico indutivo clássico de 20 créditos espetado na ranhura.
  - `ObterSpriteLED()`: Mini indicadores de LED para leitura de cartão e status.
- [x] Redesenhar o Visor LCD verde-fósforo com 3 linhas de telemetria autêntica:
  - Linha 1: Status da linha e modo ativo (`● LINHA ATIVA [SINAL 425 Hz]`).
  - Linha 2: Nome do registro selecionado em destaque caps (`REG 01 > ENCRUZILHADA CENTRAL`).
  - Linha 3: Data, horário da gravação e status de ocupação do slot.
- [x] Implementar matriz telefônica clássica 3x2:
  - Linha superior: Teclas `[ 1 ]`, `[ 2 ]`, `[ 3 ]`
  - Linha inferior: Teclas `[ 4 ]`, `[ 5 ]`, `[ #  DISCAR ]`
  - Cada tecla com indicador luminoso de slot ocupado (`● SALVO` / `○ LIVRE`).
  - Suporte a clique duplo ou retocar no mesmo número para confirmação rápida.
- [x] Chaves seletoras industriais táteis para alternar entre `[● GRAVAR]` e `[● CARREGAR]`.
- [x] Placa metálica de instruções dinâmicas no rodapé com avisos contextuais de perda de progresso e atalhos.
- [x] Preservar integralmente os contratos e atalhos de teclado: `1` a `5`, `#`, `Enter`, `NumpadEnter`, `Esc`, `E`.

## Arquivos modificados

- `Projeto Jogo 2D/Assets/Scripts/PayphoneJogoUI.cs`: Reescrita completa da apresentação visual e procedural da interface do orelhão.

## Validação e testes realizados

1. **Compilação C#**: Executado `dotnet build "Projeto Jogo 2D/Assembly-CSharp.csproj"` com resultado `Exit code: 0`, zero erros e nenhum novo aviso introduzido.
2. **Correção de Hierarquia de Canvas**:
   - Identificado que o container de fundo (`background`) estava sendo instanciado com pai nulo antes da atribuição de `stage`, deixando os componentes fora da hierarquia do `PayphoneSaveCanvas`.
   - Corrigida a instanciação para `UIFactory.CreateBox(transform, "ControlesDoOrelhao", null)` como filho direto de `s_modalGo`, garantindo renderização imediata na árvore do Canvas.
   - Ajustada a curva de opacidade para iniciar em 20% da aproximação da câmera (`InverseLerp(.20f, .85f, opening)`).
3. **Ciclo Graphify / Validate**:
   - `scripts/graphify-refresh.ps1`: acusou ferramenta CLI graphify ausente do ambiente local uv/python (conforme já registrado nos planos de hoje).
   - `scripts/graphify-project.py validate`: executado, mantendo a lista de scripts recentes aguardando indexação da ferramenta.
3. **Mockup e proporções**: Validado o layout de 236x252 no canvas com enquadramento relativo na câmera e escala adaptativa.

## Próximos passos e pendências

- Validação em Play Mode na cena com Marcus no orelhão da Encruzilhada Central para inspeção visual do autor.
