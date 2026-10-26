# Santa Rita — A Noite das 03:17

Jogo 2D de terror psicológico, feito em **Unity 6000.6.0f1** com URP 2D.

O projeto do Unity fica em `Projeto Jogo 2D/`. A pasta `Arte/` guarda os
arquivos de trabalho (Aseprite, guias, paletas) que **não** entram no jogo.

---

## Antes de clonar

**1. Instale a mesma versão do Unity: `6000.6.0f1`.**
Versão diferente converte os arquivos e gera conflito em todo mundo. Instale
pelo Unity Hub, em *Installs → Install Editor → Archive*.

**2. Instale o Git LFS** (uma vez por máquina):

```bash
git lfs install
```

Sem isso, as imagens e os áudios chegam como arquivos de texto de 130 bytes e
o projeto abre todo quebrado.

## Clonando

```bash
git clone <url-do-repositorio>
```

A primeira abertura no Unity demora: ele reconstrói a pasta `Library`, que não
vem no repositório (tem quase 7 GB).

## Configure o merge de cenas (uma vez por máquina)

Cena e prefab são arquivos YAML gigantes. Sem a ferramenta do Unity, qualquer
conflito vira trabalho perdido:

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver '"C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Tools/UnityYAMLMerge.exe" merge -p %O %B %A %A'
git config merge.unityyamlmerge.recursive binary
```

(No macOS o caminho é `/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/Tools/UnityYAMLMerge`.)
(No Linux o caminho é `~/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Tools/UnityYAMLMerge`.)

---

## Como trabalhamos

**Uma cena por pessoa de cada vez.** Cena é o arquivo que mais dá conflito.
Avise no grupo antes de abrir `EntradaDaCidade`, `CasaDoAvo`, `BarDoZe`, `MainMenu` ou
`Prologo`. Se precisar mexer em duas coisas ao mesmo tempo, transforme o
pedaço em **prefab** e edite o prefab.

**O `.meta` vai junto, sempre.** Cada arquivo dentro de `Assets/` tem um
`.meta` ao lado. Ele guarda o identificador que liga o arquivo às cenas. Se
você versionar só o `.png` e esquecer o `.png.meta`, quem puxar perde todas as
referências daquele sprite.

**Branch por tarefa, junta por pull request.**

```bash
git checkout -b arte/casa-do-avo
# ... trabalha ...
git add .
git commit -m "Casa do avô: sprite e colisores"
git push -u origin arte/casa-do-avo
```

**Nunca commite a `Library`, a `Temp` nem os `.csproj`.** Já estão no
`.gitignore`; se aparecerem no `git status`, tem algo errado.

---

## Regras de arte (resumo)

| regra | valor |
|---|---|
| escala | 32 px por unidade |
| 1 unidade | 1,15 m |
| personagem | 48 px de altura (1,5 u), quadro de 64×64, pé na linha 56 |
| cenário | desenhado em **70%** da medida real (`unidades = metros × 0,7 ÷ 1,15`) |
| câmera | 512×288 de referência, Pixel Perfect com Crop Frame |
| iluminação | a arte é pintada com **luz normal**; a noite vem da camada de luz do jogo |

Paletas em `Arte/Chao/Chao_paleta.gpl` e `Arte/Heroi/Heroi_paleta.gpl`.

---

## Desenvolvimento com IA: Graphify + Obsidian + Memória Persistente

Este repositório já vem configurado com um grafo de conhecimento (**Graphify**), um cofre navegável (**Obsidian**) e regras automáticas de agentes (**`AGENTS.md`**, **`CLAUDE.md`** e **`.agents/rules/`**) para que qualquer IA compreenda toda a arquitetura antes de alterar o código e mantenha a documentação sincronizada ao final de cada prompt.

> **Importante:** O pacote oficial no PyPI chama-se **`graphifyy`** (com dois `y`), enquanto o comando no terminal é `graphify`. O grafo e o cofre funcionam como índice estrutural rápido, mas **o código-fonte C# é sempre a fonte da verdade**.

### 1. Como instalar o Graphify e o Obsidian na sua máquina

#### Passo A — Instalar o `uv` e o Graphify CLI (Python 3.10+)

**No Windows (PowerShell):**
```powershell
# 1. Instale o uv (gerenciador de ferramentas Python):
winget install --id astral-sh.uv -e
# (Feche e reabra o PowerShell após instalar o uv)

# 2. Instale o Graphify oficial (pacote graphifyy com dois 'y'):
uv tool install graphifyy
uv tool update-shell

# 3. Verifique a instalação e o ambiente do projeto:
graphify --version
.\scripts\graphify-setup.ps1
```

**No Linux / macOS / WSL2 (Bash):**
```bash
# 1. Instale o uv (se ainda não tiver):
curl -LsSf https://astral.sh/uv/install.sh | sh
export PATH="$HOME/.local/bin:$PATH"

# 2. Instale o Graphify oficial (pacote graphifyy com dois 'y'):
uv tool install graphifyy
uv tool update-shell

# 3. Verifique a instalação e o ambiente do projeto:
graphify --version
./scripts/graphify-setup.sh
```

#### Passo B — Instalar e abrir o Cofre no Obsidian
1. Baixe e instale o [Obsidian](https://obsidian.md/download) (Windows, macOS ou Linux).
2. Abra o Obsidian e clique em **"Abrir pasta como cofre"** (*Open folder as vault*).
3. Selecione a pasta **`docs/architecture-vault`** dentro deste repositório:
   ```text
   Projeto-Jogo-2D/docs/architecture-vault
   ```
4. **Como explorar no Obsidian:**
   - **`manual/`**: Notas detalhadas da equipe e da arquitetura (nunca são apagadas pelo Graphify):
     - `Visao_Geral_Arquitetura.md` — Mapa dos 7 subsistemas do jogo.
     - `Catalogo_Completo_Scripts.md` — Descrição completa de todos os **44 scripts C#** (~15.010 linhas).
     - `Mapa_Cenas_Zonas_E_Fluxo_Cap1.md` — As 5 cenas, as 9 zonas/interiores do `Capitulo1Director` e a cadeia de flags.
     - `Guia_Trabalho_Com_IA.md` — Guia rápido de comandos e uso com IA.
   - **`generated/`**: Mais de **1.700 notas atômicas** geradas automaticamente para cada classe, método e enum do jogo, além do mapa visual interativo **`generated/graph.canvas`**.
   - Pressione **`Ctrl + G`** (ou `Cmd + G`) no Obsidian para abrir o **Graph View** e navegar visualmente pelas conexões do código!

### 2. Como usar no dia a dia com IA (Antigravity, Claude Code, Codex, Cursor, Copilot, Windsurf)

Graças aos arquivos **`AGENTS.md`**, **`CLAUDE.md`** (que importa `@AGENTS.md`) e **`.agents/rules/project-context.md`** na raiz do projeto, o agente de IA já recebe instruções automáticas para seguir este ciclo em **todo novo prompt**:

1. **No início de cada prompt (Busca Obrigatória):**
   - A IA lê o estado atual em `docs/ai-memory/INDEX.md` e `docs/ai-memory/CURRENT_STATE.md`.
   - A IA consulta as notas do Obsidian em `docs/architecture-vault/manual/`.
   - A IA executa buscas no grafo (`graphify query "<pergunta>"`, `graphify path "<A>" "<B>" --undirected`, `graphify explain "<Classe>"`) antes de abrir ou editar arquivos `.cs`.
2. **No final de cada execução (Atualização Obrigatória):**
   - Após modificar qualquer script C# ou arquitetura, a IA roda:
     - **Linux / macOS:** `./scripts/graphify-refresh.sh` e `./scripts/graphify-validate.sh`
     - **Windows (PowerShell):** `.\scripts\graphify-refresh.ps1` e `.\scripts\graphify-validate.ps1`
   - E atualiza tanto o cofre do Obsidian (`docs/architecture-vault/manual/` e `generated/`) quanto a memória persistente (`docs/ai-memory/CURRENT_STATE.md` e `CHANGELOG_AI.md`).

> **Dica para outros agentes:** Se você usa Claude Code, Codex ou Cursor e quiser instalar os hooks/skills locais da sua ferramenta, basta rodar `graphify claude install`, `graphify codex install` ou `graphify cursor install` na raiz do projeto (consulte `docs/GRAPHIFY-OBSIDIAN-OPERACAO.md` para detalhes).

---

## Pastas

```
Projeto Jogo 2D/              projeto Unity
  Assets/Art/                 arte importada
  Assets/Scenes/              cenas (MainMenu, Prologo, EntradaDaCidade, CasaDoAvo, BarDoZe)
  Assets/Scripts/             43 scripts C# de gameplay, UI, áudio e persistência
  Assets/Editor/              automação de build e validação (SantaRitaChapter1Builder.cs)
  Assets/Tiles/               tiles e Tile Palettes
Arte/                         arquivos de trabalho (Aseprite, guias, paletas)
Docs/                         relatórios de auditoria canônica R1 e Vertical Slice
docs/architecture-vault/      cofre do Obsidian (manual/ + generated/ + graph.canvas)
docs/ai-memory/               memória persistente de contexto para agentes de IA
graphify-out/                 grafo de conhecimento estrutural (graph.json, GRAPH_REPORT.md, graph.html)
scripts/                      scripts multiplataforma (.sh e .ps1) de setup, refresh e validação
AGENTS.md / CLAUDE.md         regras automáticas de governança para agentes de IA
```
