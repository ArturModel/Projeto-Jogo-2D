# Estado Atual do Projeto — Santa Rita

**Atualizado em:** 26/10/2026  
**Por:** Antigravity AI  
**Base:** Verificado nos 44 scripts C#, relatórios de auditoria, Git status e índice do Graphify  

---

## 1. Estado Confirmado do Código e Cenas [Verificado]
- **Vertical Slice do Capítulo 1:** Totalmente implementado nas 5 cenas principais:
  - `MainMenu.unity`: Menu principal com seleção de itens, carregamento dos 5 slots de save do Orelhão (`PayphoneCheckpoint.AbrirModalCarregarDoMenu()`) e opções de áudio/vídeo/legendas.
  - `Prologo.unity`: Viagem de carro em 2004 (`PrologoDirector` / `PrologoVideo`), rádio, celular, porta-luvas e chegada sob chuva forte.
  - `EntradaDaCidade.unity`: Mundo multi-zonas gerenciado por `Capitulo1Director` (5 Zonas Externas + 4 Interiores: Posto, Mercadinho, Casa do Avô nº 40 e Bar do Zé), Orelhões de save manual e Mapa Interativo (`MapaInterativoUI`).
  - `CasaDoAvo.unity`: Investigação da rotina interrompida no mesmo dia (café morno, cartas, recibo de 2004 do Mercadinho, desordem perto da porta e telefone fora do gancho).
  - `BarDoZe.unity`: Confronto com o Zé, minigame de sinuca em série Melhor de 3 (`SinucaMinigameUI` com a regra *"A Bola Marcada"*), entrega da Ficha do Clube #01 e liberação do Limiar do Centro.
- **Grafo, Cofre Obsidian e Governança Multiplataforma de IA (Alinhado ao Roteiro Universal v2.0 e Prompt Mestre v2):**
  - Grafo completo em `graphify-out/` (`graph.json`, `GRAPH_REPORT.md`, `graph.html`).
  - Cofre Obsidian em `docs/architecture-vault/` com `generated/` (1.700+ notas atômicas e `graph.canvas`) e `manual/` (`Visao_Geral_Arquitetura.md`, `Catalogo_Completo_Scripts.md`, `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md`, `Guia_Trabalho_Com_IA.md`).
  - Governança multi-agente configurada na raiz via `AGENTS.md` (com marcadores `<!-- BEGIN/END memoria-projeto -->`), `CLAUDE.md` (com `@AGENTS.md`) e `.agents/rules/project-context.md`.
  - Scripts idempotentes multiplataforma em `scripts/`: `.sh` (Linux/macOS/WSL) e `.ps1` (Windows PowerShell).
  - `README.md` e `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md` atualizados com instruções completas para Windows e Linux.

---

## 2. Trabalho em Andamento [Verificado]
- Branch dedicada `chore/graphify-obsidian-setup` sincronizada e publicada no GitHub (`origin/chore/graphify-obsidian-setup`).

---

## 3. Bloqueios Técnicos [Verificado]
- **Nenhum bloqueio ativo.** A compilação, o grafo estrutural, o cofre do Obsidian e os scripts de validação estão 100% operacionais.

---

## 4. Pendências Canônicas e Futuras (Capítulo 2+) [Pendência]
- **C02 [Pendência]:** Mandante e destino final do rapto de Adílio (propositalmente mantido em mistério no Capítulo 1).
- **C03 [Pendência]:** Sustentação dramática de como o frentista e Helena guardam o segredo de Lucas vivo.
- **C04 [Pendência]:** Modelagem e montagem do bairro do Centro (que se abre no início do Capítulo 2).
- **C05 [Pendência]:** Ajuste fino de sensibilidade do taco na Sinuca após sessões de playtest com jogadores humanos.
