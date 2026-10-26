# Cofre Arquitetural do Projeto Santa Rita — A Noite das 03:17

Este diretório é um **cofre Markdown nativo do Obsidian**, projetado para visualização gráfica, exploração de código e manutenção de notas de arquitetura e design de jogo em conjunto com agentes de IA.

---

## Como Abrir no Obsidian (Windows, macOS ou Linux)

1. Instale e abra o [Obsidian](https://obsidian.md/) (no Linux via Flatpak: `flatpak run md.obsidian.Obsidian` ou AppImage/Snap).
2. Na tela inicial do Obsidian, clique em **"Abrir pasta como cofre"** (*Open folder as vault*).
3. Selecione a pasta `docs/architecture-vault` dentro do repositório clonado:
   ```text
   <caminho-do-repositorio>/docs/architecture-vault
   ```
4. O Obsidian carregará instantaneamente todas as notas manuais, as 1.600+ notas atômicas de código e o Canvas interativo.

---

## Estrutura do Cofre (Duas Camadas)

O cofre é organizado em **duas camadas estritas** para garantir que regenerações automáticas do Graphify nunca destruam o trabalho manual da equipe:

```text
docs/architecture-vault/
├── README.md                              # Este guia de abertura
├── manual/                                # NOTAS HUMANAS E DE ARQUITETURA (NUNCA SOBRESCRITAS PELO GRAPHIFY)
│   ├── README.md                          # Índice das notas manuais
│   ├── Visao_Geral_Arquitetura.md         # Mapa completo dos 7 subsistemas do jogo
│   ├── Catalogo_Completo_Scripts.md       # Inventário detalhado de todos os 44 scripts C# (~15.010 linhas)
│   ├── Mapa_Cenas_Zonas_E_Fluxo_Cap1.md   # Topologia das 5 cenas, 9 zonas/interiores e cadeia de flags
│   └── Guia_Trabalho_Com_IA.md            # Guia prático do fluxo Graphify + Obsidian + IA
└── generated/                             # NOTAS ATÔMICAS GERADAS PELO GRAPHIFY (SOBRESCREVÍVEIS)
    ├── graph.canvas                       # Canvas visual interativo de todas as entidades
    ├── Capitulo1Director.md
    ├── GameState.md
    ├── CheckpointSystem.md
    ├── SinucaMinigameUI.md
    └── ... (1.650+ notas atômicas de classes, métodos, structs, enums e conceitos)
```

### Regras de Ouro do Cofre:
- **`generated/`**: Contém nós atômicos de código C# e conceitos mapeados pelo Graphify. É atualizado automaticamente por `./scripts/graphify-refresh.sh` (`graphify export obsidian --dir docs/architecture-vault/generated`). **Não edite arquivos manualmente dentro de `generated/`**.
- **`manual/`**: Contém notas de arquitetura, catálogos de scripts, mapas de cenas/zonas e decisões de design. **Esta pasta nunca é apagada pelo Graphify** e deve ser consultada no início de cada tarefa e atualizada ao final de mudanças arquiteturais.
- **Wikilinks Conectados:** Qualquer nota em `manual/` pode (e deve) apontar para notas em `generated/` usando wikilinks, como `[[GameState]]`, `[[Capitulo1Director]]` ou `[[SinucaMinigameUI]]`.

---

## Recursos Visuais no Obsidian

- **Visualizador Canvas:** Abra `generated/graph.canvas` dentro do Obsidian para navegar por um quadro infinito de cartões conectados visualmente por arestas.
- **Visualização em Grafo (Graph View):** Pressione `Ctrl + G` (ou `Cmd + G` no macOS) no Obsidian para visualizar a constelação completa de classes, métodos e comunidades do projeto coloridas por tags (`#community/...`).
