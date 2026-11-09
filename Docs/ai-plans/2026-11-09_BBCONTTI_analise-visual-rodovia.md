# Análise Visual e Sugestões de Melhoria — Rodovia de Entrada (Zona 1)

Autor do registro: @BBCONTTI · 09/11/2026 20:30 -03:00 · Windows / PowerShell · Unity 6000.6.0f1 · URP 2D  
Status: Análise concluída (nenhum arquivo de código ou cena modificado).

## 1. Solicitação do Usuário
Analisar os dois prints anexados da primeira cena explorável (`Zona1_Rodovia` em `EntradaDaCidade`, trecho da parada de ônibus, orelhão e poste) e sugerir melhorias visuais estruturadas para o jogo com base nesse exemplo.

## 2. Diagnóstico e Contexto Consultados (Obsidian, Graphify e Código C#)
- **Memória Persistente e Obsidian:** Lidos `docs/ai-memory/INDEX.md`, `docs/ai-memory/CURRENT_STATE.md`, `docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md` e `Docs/Guia_Visual_Santa_Rita.md`.
- **Skill de Arte:** Consultada `.agents/skills/santa-rita-pixel-art/SKILL.md`, `Docs/Direcao_Arte_Narrativa_Audio.md` e `Docs/Producao_Artistica_Execucao.md`.
- **Código-Fonte C# e Shaders Confirmados:**
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaRuaNorte.cs` (`BuildTilemap`, `BuildWaterMask`, `SelectEntranceGround`, `DressEntrance`)
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaRuaNorte.Chegada.cs` (`ComporMargemNorte`, `ComporMargemSul`, `ComporPavimento`, `CriarVaoDeFiacao`)
  - `Projeto Jogo 2D/Assets/Scripts/SantaRitaRuaNorte.Vegetacao.cs` (`ComporBosqueDaRua`, `FecharMatasDaAvenida`)
  - `Projeto Jogo 2D/Assets/Resources/Art/RuaNorte/LuzVolumetrica.shader` e `Nevoeiro.shader`

## 3. Pontos Diagnosticados nos Prints e Sugestões Propostas
1. **Costura de espelhamento no muro de pedra (`flip: i % 2 != 0` em `ComporMargemNorte`):** A alternância de `flipX` nos módulos `Muro` gera simetria em borboleta visível entre a parada e o orelhão. Sugerido quebrar a emenda com `Pilar`, `HeraDoMuro` ou segunda variante sem `flipX` adjacente.
2. **Recorte da poça grande sob o poste direito (`BuildWaterMask`):** A poça em `(29.65, 12.02)` forma uma mancha leitosa com degrau duro contra o asfalto carvão. Sugerido suavizar a transição de rugosidade periférica em `LuzVolumetrica.shader` e reduzir a opacidade de `waterColor` na borda.
3. **Geometria do cone volumétrico do poste (`Nevoeiro.shader`):** O cone em `_Front` abre de forma muito triangular e uniforme (`width = .17 + max(down, 0) * .87`). Sugerido curvar a queda lateral com atenuação exponencial e concentrar a névoa junto ao bocal.
4. **Silhueta do fundo da mata norte (`SantaRitaRuaNorte.Vegetacao.cs`):** Lacunas escuras entre os troncos superiores deixam vazar o fundo liso. Sugerido preencher com silhuetas de submata/copas escuras em camada de fundo.
5. **Leitura de Marcus na faixa central do asfalto:** O contorno escuro das pernas se funde ao asfalto carvão longe dos postes. Sugerido sutil rim light frio superior/lateral da chuva ou leve ganho no reflexo úmido imediato do asfalto.
6. **Microtransições de ancoragem na calçada:** As bases do orelhão, balizadores e poste tocam os ladrilhos de forma limpa demais (`SelectEntranceGround` / `ComporPavimento`). Sugerido adicionar pequenos decalques de umidade/musgo nas bases e desalinhar levemente os trechos de fiação/defensa sul.

## 4. Plano de Ação Executado
- [x] Consulta obrigatória à memória, cofre Obsidian e regras de direção de arte.
- [x] Inspeção dos dois prints enviados pelo autor e comparação com `rodovia-padrao.png` e `rodovia-corrigida-game.png`.
- [x] Verificação exata dos cálculos de iluminação, máscara de água e montagem modular nos scripts e shaders reais.
- [x] Tentativa de execução de `graphify-refresh.ps1` e `graphify-validate.ps1` (binário `graphify` indisponível no ambiente local, conforme registrado nos planos anteriores).
- [x] Registro deste plano em `docs/ai-plans/` e atualização de `docs/ai-memory/CHANGELOG_AI.md`.

## 5. Arquivos Criados ou Modificados
- Criado: `docs/ai-plans/2026-11-09_BBCONTTI_analise-visual-rodovia.md`
- Modificado: `docs/ai-memory/CHANGELOG_AI.md`

## 6. Validação e Testes
- Nenhuma alteração em código C#, shaders, cenas ou saves pessoais.
- `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1` executados: retornaram exit 1 pela ausência local do pacote `graphifyy==0.9.96`.

## 7. Próximos Passos
- Aguardar escolha do autor sobre quais ajustes visuais deseja aplicar primeiro na `Zona1_Rodovia` (`SantaRitaRuaNorte.Chegada.cs`, `LuzVolumetrica.shader` e `Nevoeiro.shader`).
