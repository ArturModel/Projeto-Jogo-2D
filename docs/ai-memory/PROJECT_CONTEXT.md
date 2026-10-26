# Contexto do Projeto — Santa Rita: A Noite das 03:17

**Atualizado em:** 26/10/2026  
**Por:** Antigravity AI  
**Base:** Verificado nos arquivos do repositório (`README.md`, `Docs/`, scripts Unity)  

---

## 1. Visão Geral do Jogo
- **Gênero:** Terror psicológico / Investigação narrativa em 2D com visão top-down e transições de tela.
- **Narrativa (Revisão R1):** Marcus retorna a Santa Rita em 2004 em seu próprio carro para procurar o avô Adílio, que parou de atender no mesmo dia. No passado: tragédia de Júlia em 1990 e acidente/confronto às 03:17 de 1998 no Clube.
- **Escopo Atual:** Capítulo 1 (Vertical Slice) — Da chegada na tempestade até a vitória na sinuca contra o Boss 1 (Zé) no Bar do Zé e a liberação do limiar do Centro.

---

## 2. Stack Técnica
- **Engine:** Unity `6000.6.0f1` (Editor compatível `6000.6.6f1`).
- **Render Pipeline:** Universal Render Pipeline (URP 2D `17.6.0`).
- **Input:** New Input System (`1.20.0`).
- **Pixel Art:** 2D Pixel Perfect (`5.1.0`), PPU = 32.
- **Resolução de Referência:** `512×288` (Câmera Pixel Perfect) e `640×360` (Canvas UI).
- **Texto:** TextMeshPro.
- **VCS & LFS:** Git com Git LFS obrigatório para mídias/áudios.

---

## 3. Estrutura de Diretórios
- `Projeto Jogo 2D/`: Raiz do projeto Unity.
  - `Assets/Scripts/`: Lógica central do jogo (43 scripts C#).
  - `Assets/Editor/`: Utilitários de automação e build (`SantaRitaChapter1Builder.cs`).
  - `Assets/Scenes/`: Cenas do jogo (`MainMenu`, `Prologo`, `Cidade/EntradaDaCidade`, `Cidade/CasaDoAvo`, `Cidade/BarDoZe`).
  - Pastas pesadas ignoradas pelo Git: `Library/`, `Temp/`, `Logs/`, `UserSettings/`.
- `Arte/`: Arquivos originais de design (Aseprite, paletas) que não entram no build do Unity.
- `Docs/`: Relatórios de auditoria canônica e validação do Vertical Slice.
- `docs/architecture-vault/`: Cofre Markdown para o Obsidian.
- `docs/ai-memory/`: Memória persistente para agentes Antigravity.
- `.agents/`: Regras e workflows configurados pelo Graphify para agentes.
- `scripts/`: Scripts idempotentes de operação e atualização do grafo.

---

## 4. Convenções e Regras de Desenvolvimento
1. **Regra de Save Manual:** Nenhum dado é persistido em disco sem interação física do jogador com um Orelhão ([`PayphoneCheckpoint.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PayphoneCheckpoint.cs)). Autosaves em trocas de cena são proibidos.
2. **Estado em RAM:** Durante a sessão, o progresso investigativo e flags vivem exclusivamente na memória ([`GameState.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/GameState.cs)).
3. **Escala e UI:** Não alterar PPU 32 nem as dimensões do Canvas sem coordenação artística. Toda UI programática deve passar por [`UIFactory.cs`](file:///home/lucamontini/Documentos/trabalho-segunda/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/UIFactory.cs).
4. **Segredo de Lucas:** Proibição estrita de qualquer menção, pista ou flag que revele que Lucas está vivo no Capítulo 1.
