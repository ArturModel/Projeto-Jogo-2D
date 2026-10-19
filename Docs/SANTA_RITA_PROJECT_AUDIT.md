# SANTA RITA — A NOITE DAS 03:17
## Relatório de Auditoria e Contrato de Implementação (Etapas 1 e 2 — Revisão R1)

Data: 18/10/2026
Escopo aprovado: **Capítulo 1 (Vertical Slice até a consequência imediata do 1º Boss — Zé / Sinuca)**
Percurso jogável: `MainMenu` → `Prologo` (Carro próprio, 2004) → `EntradaDaCidade` (Entrada, Posto/Frentista, Encruzilhada/Mercadinho, Bairro das Casas, Exterior da Casa do Avô, Rua Norte / Exterior do Bar do Zé, Orelhões, Bloqueio do Centro) → `CasaDoAvo` (Investigação doméstica) → `BarDoZe` (Confronto com Zé + Sinuca Melhor de 3 — "A Bola Marcada / Essa Fica na Mesa" + Ficha do Clube + Liberação do limiar do Centro).

---

## A. Estado Inicial Auditado
1. **Engine e Pipeline:** Unity 6 (`6000.6.0f1` / Editor instalado `6000.6.6f1`), URP 2D (`17.6.0`), Input System (`1.20.0`), 2D Pixel Perfect (`5.1.0`), TextMeshPro.
2. **Escala Visual Real do Projeto:** PPU = 32, resolução de referência `512×288` (`PixelPerfectCamera`) e `640×360` nos Canvas (`UIFactory`), tiles de `32×32`, sprites de personagem em células `64×64` (48px úteis). Preservado integralmente para manter compatibilidade com todos os assets artesanais da equipe.
3. **Problemas Críticos Encontrados na Auditoria Inicial:**
   - **Violação de Save Manual:** `GameState.cs` gravava em `PlayerPrefs` com `PlayerPrefs.Save()` a cada flag/número/texto alterado (autosave invisível permanente), enquanto `CheckpointSystem.cs` salvava apenas cena e posição.
   - **Divergências de Cânone (Herança V2 descartada):**
     - `PrologoDirector.cs` e `PrologoVideo.cs` afirmavam que Adilio estava há *"três dias"* sem atender (R1 confirma: Adilio parou de responder **no mesmo dia** em 2004).
     - `PayphoneCheckpoint.cs` continha a frase *"Ele saiu do clube vivo"*, antecipando/distorcendo a revelação final de Lucas.
     - `CasaDoAvo.unity` e `DentroDaCasaDoAvo.cs` usavam um bilhete de encontro voluntário (*"Rua do Clube, 21h. Ele foi. Ninguém o levou."*, flag `sabe_que_avo_saiu_sozinho`), incompatível com a R1 (rotina interrompida no mesmo dia, café passado, cartas na mesa, desordem localizada sutil e recibo do Mercadinho).
   - **Sistemas Parciais/Ausentes:**
     - `CameraFollow.cs` sem contenção pelas bordas do mapa (`Clamp`), dead zone ou look-ahead.
     - `GameMessageUI.cs` sem suporte a retrato (`portrait`) e nome do falante.
     - Ausência de Diário de Investigação (objetivo atual, fatos observados, pistas, itens/fichas).
     - Ausência do **Mapa Interativo (`Tecla M`)** solicitado pelo autor (inspirado no mapa conceitual de Santa Rita, *GTA San Andreas* e *Zelda: Ocarina of Time*).
     - Ausência de Posto/Frentista, Atendente do Mercadinho, Bar do Zé (fachada e interior), NPC Zé, bloqueio físico/narrativo do Centro e minigame de Sinuca (Melhor de 3).

---

## B. Decisões Confirmadas pelo Autor vs. Direções Propostas vs. Pendências

### 1. CONFIRMADO PELO AUTOR (Cânone R1 Fechado)
- **Cronologia:** Santa Rita (RS). **1990**: Tragédia de Júlia na cisterna (Marcus aos 16 anos). **1998**: Confronto no Clube, acidente de carro às **03:17** e partida/abandono por Marcus às **03:19** (Marcus aos 24 anos; Lucas sobrevive, mas Marcus acredita que ele morreu). **2004**: Retorno de Marcus aos 30 anos em **carro próprio** procurando o avô **Adilio**, que deixou de responder **no mesmo dia**.
- **Casa de Adilio:** Cartas na mesa, café passado recentemente e desordem localizada compatível com resistência (tapete dobrado, cadeira/sofá deslocado), sem sangue ou destruição. Nota/recibo aponta para o Mercadinho.
- **Segredo de Lucas:** Lucas vive em 2004, mas isso **jamais** pode ser revelado no Capítulo 1 (nem em diálogos, retratos, diário, nomes de flags visíveis ou conquistas).
- **Bar do Zé e Sinuca:** Zé é proprietário do Bar do Zé e o Velho da Sinuca. Controla o ritmo da conversa exigindo uma série **Melhor de 3** (vence quem ganhar 2 mãos: `2-0` ou `2-1`). Mistura partida real e pressão psicológica/memória.
- **Recompensa e Fechamento do Capítulo 1:** Vitória concede uma única vez a **Ficha do Clube (Origem: Zé)** e o relato limitado de Zé apoiando que Adilio não saiu por vontade própria, além de liberar o acesso ao limiar do Centro na sessão (onde o recorte se encerra após permitir salvar manualmente no orelhão).
- **Save Manual Exclusivo:** Nenhum autosave ao trocar de cena, coletar pista, entrar no bar ou vencer/perder mão ou série. Apenas o **Orelhão** persiste o estado completo. Configurações de áudio/vídeo persistem separadamente.
- **Mapa Interativo (`Tecla M`):** Abre o mapa dinâmico de Santa Rita (estilo *GTA SA / Zelda OoT*) com posição de Marcus em tempo real, orelhões, objetivo ativo, cantos dos 4 chefes e Clube ao norte (lado oposto à chegada).

### 2. DIREÇÕES PROPOSTAS E APROVADAS PARA O CAPÍTULO 1
- **Estacionamento na Entrada (C04 local):** Marcus estaciona o carro na entrada/posto porque o trecho urbano após a chuva forte está com lama/bloqueio parcial de via e ele prefere seguir a pé até o posto e a rua do avô.
- **Uso do Orelhão como Telefone:** Ao usar o orelhão, além de registrar o checkpoint manual, reflete-se de forma coerente por que Marcus não aciona socorro externo imediato (linha local instável na tempestade / ausência de prova material de crime antes de verificar os locais da cidade).
- **Cadeia de Pistas (C05 protótipo):**
  1. Posto (Frentista): confirma que Adilio mora subindo para o Bairro das Casas e esteve ativo mais cedo no dia.
  2. Casa de Adilio: exame da rotina interrompida (café, cartas, desordem localizada) + **Recibo do Mercadinho** emitido no mesmo dia (2004).
  3. Mercadinho (Atendente): reconhece o recibo de Adilio e relata que um desconhecido perguntou por Adilio pouco antes, tendo sido visto antes conversando com Zé no bar da rua norte.
  4. Bar do Zé: Zé condiciona a conversa à série Melhor de 3 na sinuca.
- **Regra do Protótipo de Sinuca ("A Bola Marcada / Essa Fica na Mesa"):**
  - Branca + 7 bolas numeradas compartilhadas (1 a 7). Cada bola legalmente encaçapada vale 1 ponto; **4 pontos vencem a mão**.
  - A partir da Mão 2, em turnos sinalizados, Zé marca uma bola ("Essa fica na mesa") com número e símbolo visíveis. Ela pode ser tocada ou usada em tabela/carambola, mas encaçapá-la é falta (zera os pontos da tacada, repõe as bolas caídas na tacada em posição livre determinística, dá bola na mão ao oponente e passa o turno). Regra desativada quando restam menos de 2 bolas.

### 3. PENDÊNCIAS REAIS REGISTRADAS (C02–C05 — Não Fixadas Silenciosamente)
- **C02:** Mecanismo exato e mandante do desaparecimento de Adilio (mantido apenas como indício de saída não voluntária).
- **C03:** Como o segredo da sobrevivência de Lucas foi mantido entre frentista/Adilio/Helena (nenhum NPC revela Lucas vivo).
- **C04:** Encenação definitiva do Centro completo (neste recorte, o Centro fica inacessível antes de vencer Zé e, após a vitória, atravessar o limiar exibe o encerramento do Capítulo 1).
- **C05:** Calibragem final pós-playtest humano da modalidade autoral de Sinuca.

---

## C. Checklist de Marcos (Etapa 2)
- [x] **M0:** Contrato, auditoria e registro de fontes (`SANTA_RITA_PROJECT_AUDIT.md`, `ASSET_SOURCES.csv`).
- [ ] **M1:** Core de estado em RAM vs. Save manual seguro no Orelhão (`GameState`, `CheckpointSystem`, `PayphoneCheckpoint`), `CameraFollow` com clamp de bordas, `GameMessageUI` com retratos e nome do falante, `MenuPausa` no estilo papel envelhecido e confirmação de saída.
- [ ] **M2 & M3:** Mapa Interativo (`M`) + Diário de Investigação (`Tab`/`J` ou aba integrada), correção canônica do Prólogo (2004, mesmo dia, carro próprio), Casa do Avô (desordem localizada, café, cartas, recibo 2004), Posto (Frentista), Mercadinho (Atendente), bloqueio do Centro e Exterior/Interior do Bar do Zé (`BarDoZe.unity`).
- [ ] **M4:** Minigame de Sinuca (Melhor de 3, física top-down, tutorial na Mão 1, "Bola Marcada" na Mão 2/3, IA justa do Zé, vitória 2-0/2-1 entregando Ficha do Clube + pista, derrota 0-2 oferecendo recarregar último Orelhão ou Menu).
- [ ] **M5 & M6:** Polimento de áudio/chuva, validação automatizada em Unity Editor batchmode, geração de build Windows x64 e relatório `SANTA_RITA_VERTICAL_SLICE_REPORT.md`.
