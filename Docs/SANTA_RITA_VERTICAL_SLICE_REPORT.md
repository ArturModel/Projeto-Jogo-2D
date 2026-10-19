# SANTA RITA — A NOITE DAS 03:17
## Relatório de Implementação e Validação do Vertical Slice (Capítulo 1 — Até a Consequência do Boss 1: Zé / Sinuca)

### 1. Escopo Canônico Aplicado (Revisão R1)
Todas as divergências identificadas na auditoria inicial foram corrigidas no código e nas cenas para refletir integralmente o documento **`SANTA RITA | PROMPT 02 | REVISAO R1`**:

| Ponto Canônico (Revisão R1) | Estado Anterior no Projeto | Correção Aplicada |
|---|---|---|
| **Linha do Tempo** | Misturava menções vagas sem fixar 2004 no jogo | **1990** (Júlia), **1998** (acidente às 03:17 e abandono às 03:19), **2004** (retorno de Marcus em seu próprio carro). |
| **Gatilho do Retorno de Marcus** | `"Três dias"` sem atender no prólogo | Corrigido em [`PrologoDirector.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PrologoDirector.cs), [`PrologoVideo.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PrologoVideo.cs) e `Prologo.unity`: Adílio parou de atender **no mesmo dia** (hoje cedo), e o café morno na casa confirma que a rotina foi interrompida horas antes. |
| **Save Manual Exclusivo em Orelhão** | `GameState.Marcar()` salvava em `PlayerPrefs` automaticamente a cada flag | [`GameState.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/GameState.cs) agora opera **100% em RAM na sessão** (`flagsSessao`, `diarioSessao`, `objetivoAtualSessao`, `TemProgressoNaoSalvo`). Apenas [`CheckpointSystem.Save`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/CheckpointSystem.cs) (acionado nos Orelhões via [`PayphoneCheckpoint.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/PayphoneCheckpoint.cs)) grava o snapshot persistente em disco com backup e recuperação contra corrupção. |
| **Proibição de Spoiler sobre Lucas** | Orelhão dizia `"Ele saiu do clube vivo"` e bilhete dizia `"Ninguém o levou"` | Removidas todas as frases que antecipavam o plot twist final ou contradiziam o rapto/coação de Adílio. |
| **Mapa Interativo (`M` / `Tab`)** | Inexistente | Criado [`MapaInterativoUI.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/MapaInterativoUI.cs) com estética de mapa de bolso/caderno gasto, inspirado na imagem conceitual de Santa Rita, *GTA San Andreas* (marcador do jogador em tempo real, waypoint pulsante de objetivo e legenda) e *Zelda: Ocarina of Time* (entrada ao sul, chefes nos cantos e Clube do Boss Final no topo norte), integrado ao **Diário Investigativo** (`[FATO]`, `[PISTA]`, `[HIPÓTESE]`) e aba de **Fichas do Clube**. |
| **Boss 1 — Zé e Sinuca (Melhor de 3)** | Inexistente | Criada a cena [`BarDoZe.unity`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scenes/Cidade/BarDoZe.unity) e o minigame completo [`SinucaMinigameUI.cs`](file:///c:/Users/Usuario/Projeto-Jogo-2D/Projeto%20Jogo%202D/Assets/Scripts/SinucaMinigameUI.cs) com a regra psicológica *"A Bola Marcada / Essa Fica na Mesa"*, entrega da **Ficha do Clube #01 (`ficha_clube_ze_01`)**, revelação de que Adílio não saiu voluntariamente e desbloqueio do **Limiar do Centro**. |

---

### 2. Fluxo Jogável Completo do Capítulo 1

1. **`MainMenu` → `Prologo`**:
   - Marcus dirige seu próprio carro sob a tempestade de 2004 rumo a Santa Rita porque Adílio parou de atender o telefone hoje cedo.
   - Interações opcionais no interior do carro: **Celular** (`4 chamadas não atendidas`), **Rádio** (boletim da chuva na vicinal de Santa Rita) e **Porta-luvas** (endereço *Bairro das Casas, nº 40* e foto antiga).
2. **`EntradaDaCidade` (Chegada, Posto, Encruzilhada e Bairro das Casas)**:
   - **Carro de Marcus** estacionado na entrada da cidade.
   - **Posto da Entrada & Frentista**: orienta Marcus sobre o *Bairro das Casas, nº 40*, o Mercadinho e o funcionamento dos Orelhões na tempestade.
   - **Orelhão da Encruzilhada** (`orelhao_encruzilhada`): realiza o **Save Manual** e contextualiza a queda do tronco telefônico externo.
   - **Casa do Avô (Exterior → `CasaDoAvo.unity`)**: porta destrancada no nº 40.
3. **`CasaDoAvo` (Investigação da Rotina Interrompida)**:
   - **Café passado hoje** (`Pista_CafeAindaQuente`): bule e xícara ainda mornos.
   - **Cartas de baralho na mesa** (`Pista_CartasMesa`): paciência interrompida na metade.
   - **Nota Fiscal de 2004** (`Pista_NotaFiscal2004`): cupom de hoje do Mercadinho com anotação no verso sobre a Rua Norte / Bar do Zé e o Clube.
   - **Desordem localizada perto da porta** (`Pista_DesordemPorta`): banco tombado e marcas de barro junto à saída.
   - **Telefone fora do gancho** e **Bilhete na cômoda**.
   - Ao reunir as pistas da casa, o Diário registra a hipótese de que Adílio não saiu voluntariamente e aponta para a **Atendente do Mercadinho** e o **Bar do Zé**.
4. **Encruzilhada (Mercadinho) → Rua Norte (Exterior do Bar do Zé)**:
   - **Atendente do Mercadinho**: confirma que Adílio comprou café hoje cedo e subiu para o lado do Bar do Zé acompanhado de perto por homens vindos do Centro.
   - **Orelhão do Bar do Zé** (`orelhao_bar_do_ze`): permite salvar manualmente imediatamente antes do confronto com o Boss 1.
5. **`BarDoZe` (Confronto com Zé — Minigame de Sinuca Melhor de 3)**:
   - Diálogo pré-duelo com retrato e crachá de nome (`ZÉ` e `MARCUS`).
   - **Minigame de Sinuca (`SinucaMinigameUI`)**:
     - Série **Melhor de 3 mãos** (`2-0` ou `2-1`).
     - Branca + **7 bolas numeradas compartilhadas (1..7)**; **4 pontos** vencem a mão.
     - Invariante estrita de contabilidade: `pontosMarcus + pontosZe + bolasNaMesa == 7`.
     - A partir da 2ª mão entra a regra psicológica **"A Bola Marcada / Essa Fica na Mesa" (`[X]`)**: encaçapar a bola marcada é falta, anula os pontos da tacada, repõe a bola marcada de forma determinística sem sobreposição e entrega bola na mão ao oponente. Quando restam `< 2` bolas na mesa, a marcação é desativada para nunca travar a mão.
   - **Consequência Imediata da Vitória**:
     - Zé entrega a **Ficha do Clube #01 (`ficha_clube_ze_01`)**, visível na aba *Fichas / Itens* do menu `M`.
     - Zé confirma que Adílio não saiu de casa por vontade própria e foi levado pelo portão norte rumo ao Centro porque voltou a investigar a madrugada das 03:17 de 1998.
     - O **Portão do Limiar do Centro** na Rua Norte é destrancado na sessão, encerrando o Vertical Slice do Capítulo 1 ao ser atravessado.

---

### 3. Controles do Jogo

- **`WASD` / Setas**: Movimentação de Marcus (e ajuste fino da bola branca na mão).
- **`E`**: Interagir com portas, pistas, NPCs e Orelhões / Avançar caixas de diálogo.
- **`M` ou `Tab`**: Abrir / fechar o **Mapa Interativo de Santa Rita, Diário Investigativo e Fichas do Clube**.
- **`ESC`**: Fechar o Mapa Interativo ou abrir o **Menu de Pausa** (com aviso de progresso não salvo e opção *Efeitos Suaves*).
- **Na Sinuca (`SinucaMinigameUI`)**:
  - **Mouse** ou **`A`/`D` (Setas)**: Mirar o taco.
  - **Segurar Botão Esquerdo do Mouse** ou **`Espaço`**: Carregar força da tacada (soltar para disparar).
  - **Botão Direito do Mouse** ou **`C`**: Cancelar carregamento da tacada antes de soltar.
