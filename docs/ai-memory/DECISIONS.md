# Registro de Decisões de Arquitetura e Design (ADR)

**Atualizado em:** 26/10/2026  
**Por:** Antigravity AI  
**Base:** Verificado nos contratos de auditoria R1 e histórico do projeto  

---

### D01 — Save Manual Exclusivo via Orelhão (18/10/2026) [Verificado]
- **Contexto:** `GameState.cs` executava `PlayerPrefs.Save()` a cada alteração de flag, criando um autosave invisível incompatível com a tensão de um jogo de terror e investigação.
- **Decisão:** Remover qualquer persistência automática em `GameState`. O progresso em sessão permanece estritamente em RAM. O salvamento em disco é exclusivo dos Orelhões através de `CheckpointSystem.Save()`.
- **Consequência:** Implementada flag `TemProgressoNaoSalvo` e diálogo de confirmação ao sair pelo menu de pausa.

### D02 — Fechamento da Cronologia e Gatilho do Prólogo (18/10/2026) [Verificado]
- **Contexto:** Havia divergências entre scripts antigos que citavam *"três dias sem notícias"* de Adílio, contradizendo a rotina de resistência repentina.
- **Decisão:** Fixar a linha canônica: 1990 (Júlia), 1998 (03:17 no Clube), 2004 (retorno de Marcus em carro próprio). Adílio deixou de atender **no mesmo dia**, comprovado pelo café ainda morno na casa.
- **Consequência:** Atualizados `PrologoDirector.cs`, `PrologoVideo.cs` e a cena `CasaDoAvo.unity`.

### D03 — Proibição Estrita de Menções à Sobrevivência de Lucas (18/10/2026) [Verificado]
- **Contexto:** Linhas antigas em orelhões diziam *"Ele saiu do clube vivo"*, estragando o clímax da narrativa.
- **Decisão:** Proibir em diálogos, logs, nomes de flags ou retratos qualquer menção de que Lucas sobreviveu ao acidente de 1998 durante o Capítulo 1.
- **Consequência:** Textos saneados em todos os scripts e prefabs.

### D04 — Minigame de Sinuca Autoral: "A Bola Marcada" (18/10/2026) [Verificado]
- **Contexto:** O confronto com o Zé precisava de mecânica própria para integrar tensão psicológica e diálogo sem recorrer a combate tradicional.
- **Decisão:** Desenvolver minigame de sinuca 2D em série Melhor de 3. Introdução da regra *"A Bola Marcada / Essa Fica na Mesa"* na 2ª mão, com desativação preventiva quando restam `< 2` bolas.
- **Consequência:** Implementado `SinucaMinigameUI.cs` e cena `BarDoZe.unity`.

### D05 — Mapa Interativo (`M`) e Diário em Caderno Único (18/10/2026) [Verificado]
- **Contexto:** O jogador precisava consultar a geografia de Santa Rita e as pistas encontradas sem fragmentação de menus.
- **Decisão:** Unificar Mapa de Santa Rita, Diário Investigativo (`[FATO]`, `[PISTA]`, `[HIPÓTESE]`) e Inventário de Fichas do Clube em um único painel de caderno gasto acessível por `M` ou `Tab`.
- **Consequência:** Criado `MapaInterativoUI.cs`.

### D06 — Isolamento de Mídias e Governança do Graphify (26/10/2026) [Verificado]
- **Contexto:** O projeto possui 1.001 imagens e 18 vídeos que causariam estouro de cota e lentidão se indexados por visão computacional indiscriminada.
- **Decisão:** Criar `.graphifyignore` descartando assets de imagem, áudio e vídeo da indexação estrutural, mantendo o foco em código C# e documentação de design.
- **Consequência:** Indexação ultraveloz (1.584 nós e 3.025 arestas) com custo de tokens reduzido e cofre Obsidian perfeitamente navegável.
