# Sinuca 3D do Bar do Zé — Execução e Verificação (R1)

## 1. Visão Geral e Diretriz Principal

Esta entrega eleva a apresentação visual, a resposta física e a ambientação do primeiro boss de **Santa Rita — A Noite das 03:17** (Sinuca contra Zé no `Interior_BarDoZe`, cena `EntradaDaCidade`) ao patamar de qualidade alcançado pela Bocha do Capítulo 2, utilizando o **FlyOrDie Pool** como referência espacial de mesa em perspectiva 3D, volume das esferas, leitura das tabelas/caçapas e taco 3D visível, **sem aumentar a complexidade operacional dos controles**.

- **Ciclo básico inalterado:** **mirar → segurar para carregar a força → soltar para tacar**.
- **Simplicidade garantida:** todas as adições (alternância de câmera 3D/Superior, zoom limitado, guia curta até o primeiro contato, efeito longitudinal opcional na branca e modo de prática curta) são **opcionais**. A série inteira contra Zé continua 100% vencível utilizando apenas a tacada **Normal**.

---

## 2. Controles e Proteções de Entrada (`S02`–`S07`)

| Ação | Comando Principal | Alternativa | Comportamento |
| :--- | :--- | :--- | :--- |
| **Mirar** | Mover o **Mouse** sobre a mesa | Teclas `A` / `D` ou `←` / `→` (`Shift` = ajuste fino) | Aponta a direção da tacada no plano 3D da mesa; mira trava ao iniciar a carga. |
| **Carregar força** | Segurar **Botão Esquerdo do Mouse** | Segurar **Espaço** | A barra de força oscila continuamente (`8%` a `100%`) mesmo com o mouse parado. |
| **Executar tacada** | Soltar **Botão Esquerdo do Mouse** | Soltar **Espaço** | Somente a liberação do **mesmo dispositivo** que iniciou a carga dispara a tacada. |
| **Cancelar carga** | **Botão Direito do Mouse** | Tecla `C` ou `Backspace` | Cancela a carga imediatamente sem consumir turno nem mover bolas. |
| **Alternar vista** | Tecla `V` | Botão `VISTA: MESA 3D (V)` | Alterna entre **Mesa 3D** (perspectiva oblíqua) e **Vista Superior** (tática), preservando mira e cancelando carga ativa com segurança. |
| **Zoom opcional** | **Scroll do Mouse** | — | Ajusta zoom limitado (`0.84x` a `1.45x`) sem cortar caçapas ou HUD. |
| **Bola na Mão** | Mover **Mouse** ou `WASD` | Confirmar com **Clique Esq.** ou `E` / `Enter` | Posiciona a branca em local livre; o clique de confirmação não inicia carga no mesmo frame (`S06`). |
| **Pausa / Regras** | Tecla `ESC` | Botão `REGRAS` | Suspende o minigame e cancela qualquer carga pendente sem disparar tacada. |

**Proteções ativas:**
- Soltar o botão do mouse fora do viewport da mesa cancela a carga sem tacar (`S05`).
- Clicar em botões da HUD (`VISTA`, `PRÁTICA LIVRE`, `REGRAS`, `EFEITO`) não transmite clique à mesa.
- Perda de foco da janela (`OnApplicationFocus(false)`), troca de resolução, abertura de modal ou `MenuPausa` cancelam qualquer carga ativa (`S04`).
- Enquanto `SinucaMinigameUI.IsShowing` está verdadeiro, o mundo 2D subjacente (`PlayerController`, lanterna em `F`, portas em `E` e reinteração com NPCs) fica bloqueado.

---

## 3. Regras Canônicas Preservadas (`S13`–`S16`, `S19`)

1. **Bolas e pontuação:** Branca + 7 bolas numeradas compartilhadas (`1` a `7`). Cada bola numerada encaçapada em tacada legal vale **1 ponto** para o autor da tacada. **4 pontos** vencem a mão.
2. **Série Melhor de 3:** Vencer **2 mãos** encerra a série imediatamente (inclusive em `2–0`, sem forçar 3ª mão). Marcus quebra nas mãos 1 e 3; Zé quebra na mão 2.
3. **"Essa Fica na Mesa" (Bola Marcada `[X]`):**
   - Mão 1: sinuca franca (`numeroBolaMarcada = -1`).
   - Mãos 2 e 3: Zé marca uma bola ativa por turno com giz `[X]` (`(contadorTurnosMao + maoAtual) % ativas.Count`). Tocar nela ou usá-la em tabela/carambola é permitido; encaçapá-la é **falta** para qualquer jogador.
   - Se restar apenas **1 bola numerada** na mesa (placar `3×3`), a marcação é desativada automaticamente para permitir encerrar a mão (`S14`).
4. **Faltas e penalidade:**
   - Condições de falta: (a) branca encaçapada, (b) nenhum contato da branca com bola numerada, (c) nenhuma bola tocar tabela ou cair em caçapa **após** o primeiro contato válido, ou (d) encaçapar a Bola Marcada `[X]`.
   - Na falta: apenas os pontos daquela tacada são invalidados (pontos anteriores na mão são preservados), todas as bolas numeradas caídas naquela tacada são repostas deterministicamente sem sobreposição (`EncontrarPosicaoLivreDeterministica`), e o adversário recebe **Bola na Mão** (`S15`, `S16`).
5. **Invariante das 7 bolas (`S13`):**
   - Após cada resolução de tacada em repouso: `pontosMarcus + pontosZe + bolasNumeradasAtivas == 7`.

---

## 4. Novidades Acessíveis e Simulação Física (`S09`–`S12`, `S17`–`S20`)

- **Simulação em passo fixo (`FixedStep = 1f / 120f`):**
  - Acumulador independente de framerate com sub-stepping interno, colisão contínua varrida (*swept collision*) bola-bola e rebote nos 12 bicos geométricos das 6 caçapas (`bicosCacapas`), eliminando *tunneling* em tacadas rápidas e garantindo determinismo entre `30`, `60`, `120` e `144 FPS` (`S09`, `S10`, `S11`, `S12`).
- **Guia de mira discreta:**
  - Calcula no mesmo modelo geométrico da mesa (`CalcularGuiaDeMira`) o segmento até o primeiro contato (círculo fantasma de raio combinado `2 * BallRadius` ou tabela) e uma indicação curta da direção inicial da bola atingida, sem prever cadeias inteiras de rebotes nem usar magnetismo oculto.
- **Efeito longitudinal opcional na branca (`Normal / Avançar / Recuar`):**
  - Controle recolhível na HUD (ou teclas `1`, `2`, `3` durante a mira) que volta automaticamente para `Normal` a cada novo turno.
  - `Avançar` (topspin) e `Recuar` (backspin) integram força longitudinal real na física pós-impacto (`spinLongitudinal`), acompanhados de uma frase simples na HUD explicando a tendência da branca (`S17`, `S18`).
- **Modo de Prática Curta opcional:**
  - Acessível no modal de abertura (`PRÁTICA CURTA`) ou no botão superior (`PRÁTICA LIVRE`), com 3 bolas de treino e reposição automática ao limpar a mesa, sem conceder ficha, flags narrativas, diálogo de vitória ou escrita em disco (`S20`).
- **Ambientação do Bar do Zé, Áudio por Evento e Terror Psicológico (`S23`–`S25`):**
  - Mesa 3D em mogno sobre o interior real de `Interior_BarDoZe`, com iluminação quente sobre o feltro verde gasto, 6 caçapas com aros de latão, diamantes de mira, giz azul na borda, esferas 3D com rotação `Quaternion` e taco 3D com virola e ponteira de giz.
  - Sons físicos sintetizados e integrados ao `SantaRitaSound` / `SantaRitaBarAudio` para tacada, colisão bola-bola, tabela, queda na caçapa e fricção de giz, respeitando pausa e `PlayerPrefs("audio_efeitos")`.
  - Nas mãos 2 e 3, o murmúrio do bar recua (`SantaRitaBarAudio`) e há leve oscilação de luz apenas entre turnos (respeitando `MenuPausa.ReduzirEfeitosIntensos`, `S24`).

---

## 5. Arquivos Criados e Modificados

| Arquivo | Ação | Descrição |
| :--- | :--- | :--- |
| `Projeto Jogo 2D/Assets/Scripts/SinucaTableView.cs` | Criado | Renderizador procedural 3D (`MaskableGraphic`) da mesa de sinuca com câmeras `Mesa3D` e `VistaSuperior`, raycast `ScreenToTablePoint`, esferas 3D, taco 3D, guia de mira e overlays `1..7` / `[X]`. |
| `Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs` | Modificado | Integração com `SinucaTableView`, física de passo fixo (`120 Hz`), bicos de caçapa, efeito longitudinal opcional, modo de prática curta, IA de Zé no mesmo kernel físico e áudio por evento. |
| `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Sinuca3DRuntimeVerification.cs` | Criado | Suíte automatizada de verificação em PlayMode e Standalone Player (`-srSinuca3DQA`) cobrindo `S01` a `S27` e capturando screenshots. |
| `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Chapter2RuntimeVerification.cs` | Modificado | Registro da flag de linha de comando `-srSinuca3DQA` e modo `sinuca3DForce`. |
| `Projeto Jogo 2D/Assets/Editor/Sinuca3DVerification.cs` | Criado | Suíte EditMode de validação estática e física determinística (`S01`–`S27`). |

---

## 6. Matriz de Aceite (`S01`–`S27`) e Evidências

Todos os testes foram executados com `CheckpointSystem.BeginIsolatedTests()` (`IsolatedSaveScope`), preservando intactos os saves pessoais do autor.

| Código | Critério | Status EditMode | Status Player (`.exe`) | Evidência |
| :--- | :--- | :--- | :--- | :--- |
| **S01** | Abertura do duelo no Bar do Zé pela conversa real | PASS | PASS | `sinuca-00-bar-do-ze.png`, `sinuca-01-modal-inicial.png` |
| **S02** | Ciclo básico mirar → segurar → soltar = 1 tacada | PASS | PASS | `runtime-qa.txt` |
| **S03** | Carga avança com mouse parado e mira trava na carga | PASS | PASS | `sinuca-04-carregando-taco-3d.png`, `runtime-qa.txt` |
| **S04** | Cancelar carga (Dir./C, vista, foco, pausa, regras) | PASS | PASS | `sinuca-05-carga-cancelada.png`, `runtime-qa.txt` |
| **S05** | Soltar fora do viewport da mesa não dispara tacada | PASS | PASS | `runtime-qa.txt` |
| **S06** | Confirmar Bola na Mão não inicia carga no mesmo frame | PASS | PASS | `runtime-qa.txt` |
| **S07** | Vistas Mesa 3D (padrão) e Vista Superior (`V`) + zoom | PASS | PASS | `sinuca-02-pratica-3d.png`, `sinuca-03-vista-superior.png` |
| **S08** | Números `1..7` e Bola Marcada `[X]` legíveis em 720p/1080p | PASS | PASS | `sinuca-07-bola-marcada-3d.png`, `sinuca-08-bola-marcada-superior.png` |
| **S09** | Simulação em passo fixo consistente em 30, 60, 120 e 144 FPS | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S10** | Colisão rápida em raspada sem *tunneling* | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S11** | Bola no bico da caçapa rebate na quina sem sumir | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S12** | Bola na boca da caçapa entra uma única vez (+1 ponto) | PASS | PASS | `verificacao-editmode-s01-s27.txt` |
| **S13** | Invariante das 7 bolas numeradas (`ptsM + ptsZ + ativas == 7`) | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `sr-feedback-audit.txt` |
| **S14** | Bola Marcada ativa nas mãos 2–3 e desativa com 1 bola | PASS | PASS | `sinuca-07-bola-marcada-3d.png`, `verificacao-editmode-s01-s27.txt` |
| **S15** | Faltas preservam pontos anteriores e dão Bola na Mão | PASS | PASS | `verificacao-editmode-s01-s27.txt` |
| **S16** | Reposição determinística sem sobreposição | PASS | PASS | `verificacao-editmode-s01-s27.txt` |
| **S17** | Efeito longitudinal opcional (`Recuar < Normal < Avançar`) | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S18** | Partida completa vencida usando apenas tacadas `Normal` | PASS | PASS | `runtime-qa.txt` (vitória em 16 tacadas `Normal`) |
| **S19** | Série melhor de 3 encerra em `2–0` ou `2–1` e permite revanche | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S20** | Prática curta não altera flags, ficha ou saves | PASS | PASS | `sinuca-02-pratica-3d.png`, `runtime-qa.txt` |
| **S21** | Vitória paga Ficha #01 e libera passagem ao Centro 1 única vez | PASS | PASS | `sinuca-09-pos-vitoria-ze.png`, `runtime-qa.txt` |
| **S22** | Save permanece 100% manual no orelhão (`HasCheckpoint == false`) | PASS | PASS | `runtime-qa.txt` |
| **S23** | Áudio por evento físico respeita pausa e volume | PASS | PASS | `runtime-qa.txt`, `sr-feedback-audit.txt` |
| **S24** | Terror psicológico não move mira/bolas e respeita `ReduzirEfeitosIntensos` | PASS | PASS | `runtime-qa.txt` |
| **S25** | IA de Zé decide em `~4.2 ms` (12 candidatos) sem travar frame | PASS | PASS | `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt` |
| **S26** | Enquadramento íntegro em `1920×1080` e `1280×720` | PASS | PASS | `sinuca-res-1920x1080.png`, `sinuca-res-1280x720.png` |
| **S27** | Executável StandaloneWindows64 testado (`SantaRita.exe -srSinuca3DQA`) | PASS | PASS | `Build/Sinuca3DQA/SantaRita.exe`, `Docs/Evidencias/Sinuca3D/runtime-qa.txt` |

> **Nota de Honestidade Epistêmica (`AGENTS.md` / `santa-rita-qa`):** A validação automatizada em EditMode e no executável StandaloneWindows64 comprova os contratos técnicos `S01`–`S27`, mas não substitui a avaliação artística e o playtest humano completo (`T01`) pelo autor.
