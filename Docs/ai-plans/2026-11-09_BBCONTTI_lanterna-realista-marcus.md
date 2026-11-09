# Plano Executado — Redesign Realista da Lanterna e Sprite do Marcus (`MarcusLanterna.png` V20)

- **Autor:** `@BBCONTTI` (`BBCONTTI`)
- **Data/Hora:** 2026-11-09 15:25 (-03:00)
- **Status:** Concluído (Aprovação artística final sujeita à avaliação do autor)
- **Ambiente:** Windows / PowerShell / Unity `6000.6.0f1` (URP 2D) / Python 3 (`Pillow`)

---

## 1. Solicitação do Usuário

1. Dar uma repaginada na lanterna do jogo para deixá-la mais realista, imersiva e coerente com a direção de arte de *Santa Rita — A Noite das 03:17*.
2. Atualizar o sprite atual do Marcus (`MarcusCaminhadaV20.png`, `576×256`, 4 direções × 9 colunas) usando Python (`Pillow`) para segurar a lanterna na mão quando estiver com ela ligada (`GameState.LanternaLigada`), sem distorcer as animações existentes.
3. Manter a lanterna compacta/proporcional à mão do Marcus (sem ficar exagerada) e manter o braço que segura a lanterna fixo/parado durante a caminhada para que a lanterna não fique balançando.

---

## 2. Diagnóstico e Contexto Consultados

- **Memória Persistente e Cofre Obsidian:**
  - `docs/ai-memory/INDEX.md` e `docs/ai-memory/CURRENT_STATE.md`
  - `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`
  - `docs/architecture-vault/generated/PlayerController.md`
  - `Docs/Direcao_Arte_Narrativa_Audio.md`, `Docs/Producao_Artistica_Execucao.md`, `Docs/Guia_Visual_Santa_Rita.md`, `.agents/skills/santa-rita-pixel-art/SKILL.md`
- **Diagnóstico Técnico no Código C# e Arte:**
  1. Em `PlayerController.cs`, `Awake()` possuía a condição `if (!folhaCaminhadaDirecional) PrepararPosesLanterna();`, o que impedia o carregamento de `MarcusLanterna.png` desde que a folha `MarcusCaminhadaV20.png` (`576×256`, 4 linhas × 9 colunas) passou a ser usada. Além disso, `MarcusLanterna.png` ainda estava no formato antigo (`1088×192`, 3 linhas × 17 colunas).
  2. Em `SantaRitaRuaNorte.cs`, `_PlayerLamp` e `_PlayerLampDirection` eram enviados apenas para `LuzVolumetrica.shader`, mas não para `Nevoeiro.shader` (`frontFogMaterial`), e o cálculo de `handheld` em `LuzVolumetrica.shader` usava apenas um `smoothstep` simples sem separação entre foco central halógeno (hotspot), penumbra âmbar e luz de proximidade nos pés.
  3. Nas cenas 2D fora da `SantaRitaRuaNorte` (como o Capítulo 2) e no interior 3D (`Posto3D.CasaDoAvo.cs`), faltava o cone volumétrico visível saindo da lente na mão do Marcus e a temperatura halógena quente de época (2004).

---

## 3. Plano de Ação Executado

- [x] Criar o gerador reprodutível em Python/Pillow (`Tools/Aseprite/gerar-marcus-lanterna.py`) que lê `Arte/Producao/Marcus/MarcusCaminhadaV20.png` (`576×256`), mantém 100% da animação de pernas, cabeça, tronco e braço oposto, fixa o braço que segura a lanterna (acompanhando apenas o bob vertical de `1px` do tronco) e desenha uma lanterna compacta de `3px` na mão do Marcus nas 4 direções.
- [x] Exportar e validar `Arte/Producao/Marcus/MarcusLanterna.png` e `Projeto Jogo 2D/Assets/StreamingAssets/MarcusLanterna.png` (`576×256`, 36 células validadas pelas regras de `PlayerController.ValidarFolhaMarcus`).
- [x] Integrar `gerar-marcus-lanterna.py` ao script `Tools/Aseprite/Export-Marcus.ps1`.
- [x] Atualizar `PlayerController.cs`:
  - Habilitar `PrepararPosesLanterna()` para folhas direcionais de 4 linhas (`576×256`) e usar `quadrosCaminhadaComLanterna` / `repousoComLanterna` quando `DeveUsarLanternaAgora()` estiver ativo.
  - Refinar a `Light2D` (`FeixeLanternaMarcus`) com perfil halógeno realista e adicionar o `SpriteRenderer` filho `ConeVolumetricoLanterna` (feixe volumétrico pixel-art em `32 PPU` com halo na lente, hotspot quente central e penumbra âmbar alinhados exatamente à lente na mão do Marcus, preservando `flashlight.position` para compatibilidade total com `Chapter2R2RuntimeVerification`).
- [x] Atualizar `SantaRitaWorldLighting.cs` para preservar o material próprio de renderizadores com `"Lanterna"` no nome.
- [x] Atualizar `LuzVolumetrica.shader`, `Nevoeiro.shader` e `SantaRitaRuaNorte.cs` para projetar o feixe halógeno em múltiplas camadas (spill periférico, refletor principal, hotspot quente, anéis sutis de refletor e dispersão volumétrica na neblina/garoa da rua).
- [x] Atualizar `Posto3D.CasaDoAvo.cs` para harmonizar a lanterna 3D com a cor halógena quente (`new Color(1f, .89f, .70f)`) e ângulo de foco/penumbra (`16°` / `46°`).
- [x] Atualizar `AudioC2RuntimeVerification.cs` para validar `quadrosCaminhadaComLanterna` e `repousoComLanterna` quando `equipped == true`.

---

## 4. Arquivos Criados ou Modificados

- **Criados:**
  - `Tools/Aseprite/gerar-marcus-lanterna.py`
  - `docs/ai-plans/2026-11-09_BBCONTTI_lanterna-realista-marcus.md`
- **Modificados:**
  - `Arte/Producao/Marcus/MarcusLanterna.png`
  - `Projeto Jogo 2D/Assets/StreamingAssets/MarcusLanterna.png`
  - `Tools/Aseprite/Export-Marcus.ps1`
  - `Projeto Jogo 2D/Assets/Scripts/PlayerController.cs`
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaWorldLighting.cs`
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaRuaNorte.cs`
  - `Projeto Jogo 2D/Assets/Resources/Art/RuaNorte/LuzVolumetrica.shader`
  - `Projeto Jogo 2D/Assets/Resources/Art/RuaNorte/Nevoeiro.shader`
  - `Projeto Jogo 2D/Assets/Scripts/Posto3D.CasaDoAvo.cs`
  - `Projeto Jogo 2D/Assets/Scripts/Capitulo2/AudioC2RuntimeVerification.cs`
  - `docs/ai-memory/CHANGELOG_AI.md`
  - `docs/ai-memory/CURRENT_STATE.md`

---

## 5. Validação e Testes Realizados

- **Validação de Contrato de Sprite Sheet (`MarcusLanterna.png`):**
  - Verificadas todas as 36 células (`4 linhas × 9 colunas` de `64×64` px = `576×256` px): zero pixels com `alpha > 32` nas bordas de 1px (`x = 0, 63`, `y = 0, 63`) e contagem de pixels opacos dentro da faixa `[180, 1400]` exigida por `PlayerController.ValidarFolhaMarcus`.
- **Inspeção Visual dos 36 Quadros:**
  - Confirmado que a lanterna tem tamanho compacto (`3px` de projeção frontal à mão), o braço que segura a lanterna permanece fixo/estável durante todo o ciclo de caminhada e a animação das pernas, tronco, cabeça e braço livre permanece intacta sem distorções.
- **Graphify Refresh / Validate:**
  - Executados `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`; registrado `FAIL: graphify unavailable` no ambiente Windows local sem a CLI `graphifyy` instalada, mantendo a rastreabilidade documental no cofre e na memória persistente.

---

## 6. Próximos Passos e Pendências

- Avaliação visual e artística pelo autor no Unity Editor / Play Mode das poses com lanterna e da intensidade do cone volumétrico nas cenas de C1 e C2.


---

## 7. Correção Adicional — `MissingComponentException` em `ConeVolumetricoLanterna` e Hot-Reload em Play Mode

- **Causa:** O uso do operador de coalescência nula do C# (`t.GetComponent<SpriteRenderer>() ?? t.gameObject.AddComponent<SpriteRenderer>()`) em objetos recém-criados do Unity (`UnityEngine.Object`) não dispara `AddComponent<SpriteRenderer>()` porque `GetComponent<SpriteRenderer>()` retorna um wrapper C# não-nulo de referência com ponteiro nativo zerado ("fake null"), causando `MissingComponentException` ao acessar `coneVolumetrico.sprite` em `Awake()`, além de perda de referências privadas (`rb`, `corpo`, `quadrosCaminhada`) caso o script seja recompilado com o Play Mode aberto.
- **Correção em `PlayerController.cs`:** Substituído `??` por verificação explícita `if (sr == null) sr = t.gameObject.AddComponent<SpriteRenderer>();` (que invoca a sobrecarga `UnityEngine.Object.op_Equality`) e adicionado `GarantirInicializacao()` em `Awake()`, `Update()`, `LateUpdate()` e `FixedUpdate()` para restaurar `rb`, `corpo` e as folhas de sprites automaticamente mesmo após recompilação em Play Mode.


---

## 8. Refinamento — Luz Mais Sutil, Mais Fraca, Mais Branca e Sombra Projetada Real

- **Solicitação do usuário:** A lanterna deve ser mais sutil, mais fraca, mais branca do que amarela, e atuar exclusivamente como uma fonte de luz real (como os postes) que causa sombra projetada nos objetos do cenário.
- **Ações executadas:**
  - Removido o overlay `ConeVolumetricoLanterna` e o cone de partículas em `Nevoeiro.shader` (que desenhavam um cone amarelo sobreposto sem respeitar oclusão).
  - Em `SantaRitaRuaNorte.Sombras.cs` (`ProjetarSombraDaLanterna`), a lanterna (`_PlayerLamp`) agora projeta dinamicamente as silhuetas reais dos sprites dos objetos (`_PlayerShadows` via `ProjetarRecorte.shader`, exatamente como os postes fazem em `_ArrivalShadows`), além dos volumes 3D de oclusão em `VisibilidadeDaLuz` (`LuzVolumetrica.shader`).
  - Ajustada a cor e a intensidade em `LuzVolumetrica.shader` (`half3(1.05, 1.10, 1.18)` a `half3(1.36, 1.42, 1.50)` com atenuação suave), em `PlayerController.cs` (`Light2D` com `color = new Color(.93f, .96f, 1f)`, `intensity = 1.05f`, `shadowIntensity = .88f`) e em `Posto3D.CasaDoAvo.cs` (`intensity = 3.5f`, `color = new Color(.93f, .96f, 1f)`).
  - Atualizada a lente da lanterna em `Tools/Aseprite/gerar-marcus-lanterna.py` (`MarcusLanterna.png`) para tom branco suave (`(232, 238, 246)`).
