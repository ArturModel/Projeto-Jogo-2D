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

---

## Como trabalhamos

**Uma cena por pessoa de cada vez.** Cena é o arquivo que mais dá conflito.
Avise no grupo antes de abrir `EntradaDaCidade`, `CasaDoAvo`, `MainMenu` ou
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

## Pastas

```
Projeto Jogo 2D/        projeto Unity
  Assets/Art/           arte importada
  Assets/Scenes/        cenas
  Assets/Scripts/       código
  Assets/Tiles/         tiles e Tile Palettes
Arte/                   arquivos de trabalho (Aseprite, guias, paletas)
```
