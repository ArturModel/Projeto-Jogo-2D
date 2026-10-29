# Plano Executado — Leitura de Contexto: Conversas, Lore e Arquitetura do Projeto

- **Autor (GitHub):** @Venturini01 (Ariel Venturini)
- **Data e Hora:** 2026-10-29 19:43 (BRT)
- **Status:** Concluído com Sucesso
- **Ambiente/Plataforma:** Windows (PowerShell), Unity 6000.6.0f1 (URP 2D), Python 3.14, Graphify 0.9.96, Git

---

## 1. Solicitação do Usuário

> "Boa noite, leia p arquivo zip Conversas na pasta compartilhada, também leia os arquivos da pasta do projeto jogo 2d que abri para entender"

---

## 2. Diagnóstico e Contexto Consultado

1. **Localização e Extração do Arquivo de Conversas:**
   - O arquivo `Conversa.zip` foi localizado em `C:\Users\Ariel Venturini\Downloads\Conversa.zip`.
   - Extraído para diretório de scratch com preservação do repositório:
     - `Conversa do WhatsApp com goty unesc 2026.txt` (1.116 linhas de histórico do grupo do projeto acadêmico UNESC).
     - `Santa_Rita_Lore_Consolidada_v5_Backgrounds.docx` (Bíblia consolidada v5.0 com backgrounds, motivações e cronologia).
     - `Roteiro_Lore_Santa_Rita_0317-1.pdf` e `Santa_Rita_0317_Lore_Revisada.pdf` (Roteiros canônicos, temática e terror psicológico).
     - Mídias, áudios, diagramas e stickers de referência trocados pela equipe.

2. **Mapeamento da Equipe e Papéis:**
   - **Torres / Artur (@ArturModel):** Roteiro base, programação core, diálogos do Cap 1, autoramento de cenas e caixas de diálogo.
   - **Patrick:** Áudio/trilha sonora (Suno IA), prototipação de mecânicas (bocha perspectiva, sinuca), divisão e cobrança de metas.
   - **Luca Montini (@lucamontini):** Coordenação/PO, governança de arquitetura (Graphify + Obsidian), redesign do menu de pausa e configurações (remoção da flor, placa oxidada, submenus, steppers).
   - **Ismael:** Mecânicas, suporte com testes, humor e alinhamento do prólogo.
   - **Ariel Venturini (@Venturini01 - Usuário Atual):** Design visual, interface (UI), direção de arte/pixel art, objetos/mobiliário e apresentação do projeto.

3. **Arquitetura e Estado Atual do Jogo (Santa Rita — A Noite das 03:17):**
   - **Universo e Tema:** "Uma cidade não repete uma noite. As pessoas repetem a escolha." Duas tragédias: Júlia (1990) sob o Clube Recreativo e Lucas (1998) no acidente às 03:17. Marcus (protagonista) retorna em 2004 em carro próprio marrom.
   - **Regras Inegociáveis:** Save 100% em RAM com persistência exclusiva no Orelhão (`PayphoneCheckpoint.cs`), proibição de spoiler de Lucas no Cap. 1, PPU = 32, câmera 512×288, canvas 640×360.
   - **Capítulo 1:** Preservado (Entrada da cidade, posto, mercadinho, casa do avô Adílio, Bar do Zé com sinuca/ficha 1).
   - **Capítulo 2:** Implementado até a Bocha na Sociedade Beira-Rio (cancha horizontal, escada rasa de 3 degraus, física de perspectiva sem água/ficha 2). Cenas independentes para Centro, Igreja, Prefeitura, Rodoviária, Oficina de Rádio, Casa da Costureira e Cemitério separado com abrigo.
   - **Capítulo 3 / Futuro:** Escola expressamente adiada; truco planejado para momento posterior.

4. **Ambiente Técnico e Graphify no Windows:**
   - Detectada adição recente de `Projeto Jogo 2D/Assets/Scripts/DicaDeTecla.cs` (elevando o projeto de 93 para 94 scripts C#).
   - Instalado `graphifyy==0.9.96` no Python 3.14 local (`AppData\Local\Python\pythoncore-3.14-64\Scripts`).
   - Ajustados os scripts `scripts/graphify-project.py`, `scripts/graphify-refresh.ps1` e `scripts/graphify-validate.ps1` para resolver dinamicamente o executável `graphify.exe` no Windows sem dependência manual de PATH.

---

## 3. Ações Executadas

1. [x] **Leitura Integral dos Documentos de Lore e Histórico:**
   - Leitura completa das 1.116 linhas de conversa do WhatsApp com compreensão da dinâmica de equipe, prazos para UNESC e decisões de escopo.
   - Leitura dos documentos canônicos de narrativa (`Santa_Rita_Lore_Consolidada_v5_Backgrounds.docx`, `Roteiro_Lore_Santa_Rita_0317-1.pdf`, `Santa_Rita_0317_Lore_Revisada.pdf`).
2. [x] **Inspeção dos Arquivos do Projeto Jogo 2D:**
   - Consulta ao cofre Obsidian (`docs/architecture-vault/manual/`) e aos scripts C# de controle (`PlayerController`, `GameState`, `Capitulo1Director`, `Capitulo2Director`, `MenuPausa`, `UIFactory`, etc.).
   - Atualização do inventário em `docs/architecture-vault/manual/Catalogo_Completo_Scripts.md` para incluir `DicaDeTecla.cs` (totalizando 94 scripts).
3. [x] **Sincronização e Validação do Graphify:**
   - Executado refresh do grafo estrutural (1.580 nós, 5.084 arestas, 76 comunidades indexadas).
   - Executado `.\scripts\graphify-validate.ps1` com sucesso (`PASS: 94 C# sources covered`).

---

## 4. Arquivos Modificados / Criados

- `scripts/graphify-project.py` (modificado: adicionado fallback para `sys.prefix/Scripts`)
- `scripts/graphify-refresh.ps1` (modificado: adicionado fallback para pasta Scripts do Python no PATH)
- `scripts/graphify-validate.ps1` (modificado: adicionado fallback para pasta Scripts do Python no PATH)
- `docs/architecture-vault/manual/Catalogo_Completo_Scripts.md` (modificado: atualizado para 94 scripts incluindo `DicaDeTecla.cs`)
- `docs/ai-plans/2026-10-29_Venturini01_leitura-contexto-conversa-lore-projeto.md` (criado: registro deste ciclo)
- `docs/ai-memory/CHANGELOG_AI.md` (atualizado)

---

## 5. Validação e Testes Realizados

- `.\scripts\graphify-validate.ps1`: PASS (94 scripts C# mapeados, hashes íntegros, grafo, canvas e cofre Obsidian atualizados).
- Teste de integridade de consultas Graphify AST para `GameState`, `Capitulo2Portal`, `BochaSimulation`, `SantaRitaShallowStairs`.
