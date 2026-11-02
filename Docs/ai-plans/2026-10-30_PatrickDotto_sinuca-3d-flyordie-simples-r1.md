# Plano de Execução: Sinuca 3D do Bar do Zé (Referência FlyOrDie, Controles Simples e Integração C1/C2)

- **Autor:** `@PatrickDotto` (Patrick Dotto)
- **Data/Hora:** 2026-10-30 15:18 (BRT / UTC-3)
- **Status:** Concluído (M1–M6 e S01–S27 validados em EditMode e StandaloneWindows64 Player)
- **Ambiente:** Windows 10/11 · PowerShell · Unity `6000.6.0f1` (URP 2D) · Branch `atualizacao_sinuca` (`30d811c9dc27006507e821807b395c696c4e6f69`)

---

## 1. Solicitação do Usuário
Implementar integralmente o prompt `Prompt_Sinuca_3D_FlyOrDie_Simples_R1`: elevar a apresentação visual, espacial e física do boss de Sinuca do Bar do Zé (Capítulo 1) ao patamar da Bocha em perspectiva (Capítulo 2), tendo o FlyOrDie Pool como referência visual/espacial, **sem aumentar a complexidade dos controles**.
- Preservar o ciclo essencial: **mirar, segurar para carregar a força e soltar para tacar**.
- Novidades acessíveis e opcionais:
  1. Duas vistas principais (**Mesa 3D** em perspectiva oblíqua padrão e **Vista Superior** tática) alternadas por `V` ou botão no HUD + zoom limitado no scroll.
  2. **Guia de mira discreta** até o primeiro contato (bola ou tabela/bico) + indicação curta da direção inicial da bola atingida, usando a geometria real da simulação.
  3. **Efeito longitudinal opcional na branca** (`Normal / Avançar / Recuar`) em controle recolhível simples com frase explicativa de tendência pós-contato. **A partida completa deve continuar vencível usando apenas `Normal`.**
  4. **Prática curta opcional** antes do duelo (sem conceder ficha, flags narrativas, pontos de série ou saves).
  5. **Identidade do Bar do Zé e terror psicológico contextual** centrado na restrição/controle de Zé (*"Essa fica na mesa"* / Bola Marcada `[X]`), com áudio por evento físico real e respeito a `MenuPausa.ReduzirEfeitosIntensos` e `audio_efeitos`.
- Cumprir todos os marcos `M1`–`M6` e a matriz de testes `S01`–`S27`, incluindo validação em executável Player e preservação de saves pessoais (`IsolatedSaveScope`).

---

## 2. Diagnóstico e Contexto Consultados (M1 — Baseline e Contrato)
- **Memória e Cofre Obsidian:**
  - `docs/ai-memory/INDEX.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/ai-memory/CHANGELOG_AI.md`
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`
- **Skills e Diretrizes:**
  - `.agents/skills/santa-rita-pixel-art/SKILL.md`, `santa-rita-narrativa/SKILL.md`, `santa-rita-audio/SKILL.md`, `santa-rita-qa/SKILL.md`
  - `Docs/Direcao_Arte_Narrativa_Audio.md`, `Docs/Producao_Artistica_Execucao.md`, `Docs/Bocha_Perspectiva_Execucao.md`
- **Inspeção de Código Real:**
  - `SinucaMinigameUI.cs`: regras autorais atuais (branca + bolas 1..7, 4 pontos/mão, melhor de 3, Bola Marcada `[X]` da 2ª mão em diante quando restam >= 2 bolas numeradas, faltas resolvidas após repouso completo, reposição sem sobreposição e invariante `pontosMarcus + pontosZe + bolasEmJogo == 7`).
  - `InteracaoCapitulo1.cs`: fluxo de entrada (`InteragirZe`, `InteragirMesaSinuca`, `InteragirQuadroRegras`), pagamento único de recompensa (`AoVencerZeNaSinuca`: `ZeDerrotadoNaSinuca`, `RecebeuFichaClubeZe`, `LimiarCentroDesbloqueado`) e abertura do portão do Centro.
  - `BochaCourtView.cs`, `BochaSimulation.cs`, `BochaMinigameUI.cs`, `BochaAI.cs`: arquitetura de renderização 3D via `MaskableGraphic` no Canvas `640×360` sem interferir na câmera 2D URP do mundo.
  - `Chapter2Verification.cs` (`AuditOldFeedback`) e `Chapter2RuntimeVerification.cs` (`SinucaRegression`, `SinucaPauseRegression`): contratos de reflexão obrigatórios em `SinucaMinigameUI`.

---

## 3. Marcos de Implementação (M1–M6)
- [x] **M1 — Baseline e contrato:** Observar Sinuca e Bocha atuais, registrar controles, regras, contratos de reflexão e estado do repositório.
- [x] **M2 — Partida normal estável:** Kernel físico de passo fixo (`1/120 s`), colisão contínua/varrida esfera-esfera e esfera-tabela/bico de caçapa, caçapas com boca/garganta física, rotação 3D das bolas (`Quaternion`), efeito longitudinal (`Normal / Avançar / Recuar`) e invariantes das 7 bolas preservadas.
- [x] **M3 — Mesa e input 3D:** `SinucaTableView : MaskableGraphic` com perspectiva 3D oblíqua padrão (**Mesa 3D**) e **Vista Superior** (`V` + botão HUD + zoom no scroll), projeção `ScreenToTablePoint` por plano horizontal da mesa, esferas 3D sombreadas com números legíveis e marca `[X]`, taco 3D recuando com a carga, guia de mira discreta até o primeiro contato e proteções completas de input.
- [x] **M4 — Novidades opcionais:** Modo de **Prática curta** antes do duelo (sem recompensa/flags/save) e controle recolhível de efeito na branca (`Normal / Avançar / Recuar`), mantendo a série 100% vencível apenas com `Normal`.
- [x] **M5 — Bar e terror:** Identidade visual do Bar do Zé ao redor da mesa 3D, som separado por evento físico real (`SantaRitaSound` + síntese de impacto/giz/caçapa/tabela) e encenação psicológica contextual ligada à Bola Marcada e `ReduzirEfeitosIntensos`.
- [x] **M6 — Regressão e Player:** Verificação automatizada `S01`–`S27` + suítes C1/C2 existentes, build e execução do Player (`Build/Sinuca3DQA/SantaRita.exe -srSinuca3DQA`), captura de evidências comparáveis (`Docs/Evidencias/Sinuca3D/`) e fechamento Graphify/Obsidian (`Docs/Sinuca_3D_Execucao.md`).

---

## 4. Arquivos Criados, Modificados ou Removidos
- **Criados:**
  - `Projeto Jogo 2D/Assets/Scripts/SinucaTableView.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Sinuca3DRuntimeVerification.cs`
  - `Projeto Jogo 2D/Assets/Editor/Sinuca3DVerification.cs`
  - `Docs/Sinuca_3D_Execucao.md`
  - `Docs/Evidencias/Sinuca3D/` (relatórios `verificacao-editmode-s01-s27.txt`, `runtime-qa.txt`, `build-qa.txt` e capturas `sinuca-00` a `sinuca-09`, `sinuca-res-1920x1080.png`, `sinuca-res-1280x720.png`)
- **Modificados:**
  - `Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/Chapter2RuntimeVerification.cs`
  - `docs/ai-memory/CHANGELOG_AI.md`
  - `docs/ai-memory/CURRENT_STATE.md`
  - `docs/architecture-vault/manual/Catalogo_Completo_Scripts.md`
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`

---

## 5. Validação e Testes Realizados
1. **EditMode (`Sinuca3DVerification.Run`, `Chapter2Verification.Run`, `BochaPerspectiveVerification.Run`):**
   - `PASS 15 verificações de Sinuca 3D (S01..S27 EditMode)` (`Docs/Evidencias/Sinuca3D/verificacao-editmode-s01-s27.txt`).
   - `PASS 11/11` em `Chapter2Verification.AuditOldFeedback` (`Library/sr-feedback-audit.txt`) e `PASS 48 verificações` de regras/física/saves do Capítulo 2.
   - `PASS 28 regressões físicas/força` na Bocha em perspectiva (`BochaPerspectiveVerification.Run`).
2. **StandaloneWindows64 Player (`Build/Sinuca3DQA/SantaRita.exe -srSinuca3DQA`):**
   - `ExitCode: 0` — `PASS runtime 2026-10-30T18:16:46.8120191Z` (`Docs/Evidencias/Sinuca3D/runtime-qa.txt`).
   - Série completa contra Zé vencida em 16 tacadas usando apenas o modo `Normal`, pagando Ficha #01 e liberando o portão do Centro uma única vez sem autosave.
3. **Graphify Refresh & Validate:**
   - Executados `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`.

---

## 6. Próximos Passos e Pendências
- Avaliação artística e playtest humano completo (`T01`) pelo autor no Editor/Player.
