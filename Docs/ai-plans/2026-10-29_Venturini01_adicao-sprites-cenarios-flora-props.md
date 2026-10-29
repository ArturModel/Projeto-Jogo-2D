# Plano Executado — Adição e Harmonização de Sprites nos Cenários (Flora e Props)

- **Autor (GitHub):** @Venturini01 (Ariel Venturini)
- **Data e Hora:** 2026-10-29 20:00 (BRT)
- **Status:** Concluído com Sucesso
- **Ambiente/Plataforma:** Windows (PowerShell), Unity 6000.6.0f1 (URP 2D), .NET 10, Graphify 0.9.96, Git

---

## 1. Solicitação do Usuário

> "preciso adicionar sprites nos cenários, olhe o que ja tem e adicione se necessario, seguindo o padrao ja estabelecido, depois me mostre oque fez"

---

## 2. Diagnóstico do Cenário Atual

1. **Assets Disponíveis em Repositório:**
   - Em `Assets/Resources/Art/World/`, existiam sprites de flora sulista de alta fidelidade pixel art (`PPU = 32`, paleta terrosa, sombras calibradas): `Araucaria.png` (símbolo autêntico da serra catarinense/gaúcha), `Fern.png` (samambaias), `GrassClump.png` (tufos de capim), `Shrub.png` (arbustos), `Flowers.png` e `PicketFence.png`.
   - Em `Assets/Resources/Art/Props/`, existiam móveis e elementos urbanos autênticos: `BancoPraca.png`, `QuadroAvisos.png`, `CaixasManutencao.png`, `CargasMercado.png`, `GrelhaDrenagem.png`, `ApoioAguaQuente.png`, etc.
2. **Lacunas Identificadas no Capítulo 1:**
   - No exterior de C1 (`EntradaDaCidade`), ainda eram utilizados bancos e murais provisórios legados (`Bench_Wood`, `RoadSign_Blank`, caixotes genéricos).
   - A vegetação de C1 usava exclusivamente `Oak_Tree` sem a flora regional de araucárias, samambaias e arbustos que enriquecem a atmosfera rural/serrana de Santa Rita.
   - O método `CarregarSprite` em `Capitulo1Director.cs` não possuía fallback direto para `Art/Props/` e `Art/World/`.
   - O `SantaRitaPlaceDressing.ApplyC1()` atuava apenas nos interiores (`y < -40`).
3. **Correção no Projeto .NET:**
   - `DicaDeTecla.cs` adicionado ao `Assembly-CSharp.csproj`, restaurando compilação limpa (`0 erros`).

---

## 3. Ações Executadas

1. [x] **Expansão do Carregador de Sprites em Runtime:**
   - Em `Capitulo1Director.cs`, adicionados fallbacks diretos para `Art/Props/` e `Art/World/` no método `CarregarSprite`, permitindo que qualquer prop oficial seja instanciado por nome.
2. [x] **Substituição de Props Genéricos por Versões Autênticas:**
   - `Banco_Rodovia`, `Banco_Venda`, `Banco_Encruzilhada` -> Substituídos pelo autêntico `BancoPraca.png` (2.4f largura).
   - `Placa_Horarios_Rodovia` e `Mural_Encruzilhada` -> Substituídos por `QuadroAvisos.png` com avisos pregados em madeira.
   - `Caixotes_Barro_Sul` -> `CaixasManutencao.png`.
   - `Caixotes_Venda` -> `CargasMercado.png` (fardos comerciais).
3. [x] **Adição de Vegetação Sulista (Flora) e Props de Cenário nas 5 Zonas de C1:**
   - **Zona 1 (Rodovia):**
     - Araucárias no horizonte (`Araucaria_Rodovia_1` e `Araucaria_Rodovia_2`) com colisor de tronco e ordenação por profundidade.
     - Arbustos, samambaias e tufos de capim no acostamento de barro e junto à cerca sul (`Arbusto_Rodovia_Sul_1`, `Arbusto_Rodovia_Sul_2`, `Samambaia_Rodovia_Sul`, `Capim_Rodovia_Sul`).
   - **Zona 2 (Posto de Gasolina & Venda Antiga):**
     - Araucária no fundo do pátio (`Araucaria_Posto_Fundo`).
     - Samambaia e arbusto úmido nos cantos da Venda abandonada (`Samambaia_Venda`, `Arbusto_Venda`).
     - Grelha de drenagem de chuva no piso do posto (`Grelha_Posto`) e ponto de água quente (`ApoioAgua_Posto`).
   - **Zona 3 (Encruzilhada & Mercadinho):**
     - Araucária na subida do barro (`Araucaria_Encruzilhada`).
     - Cargas e caixarias na calçada do mercadinho (`Cargas_Mercadinho_Calcada`).
     - Grelha pluvial no meio-fio da esquina (`Grelha_Encruzilhada`).
     - Canteiro de flores e samambaia na casa da esquina (`Flores_Canteiro_Esquina`, `Samambaia_Esquina`).
   - **Zona 4 (Bairro Residencial / Casa de Adílio Nº 40):**
     - Duas araucárias imponentes na mata atrás da Casa do Avô (`Araucaria_Fundo_Avo_1`, `Araucaria_Fundo_Avo_2`).
     - Samambaia e flores nos canteiros da casa 40 (`Samambaia_Avo`, `Flores_Avo`).
     - Arbusto e capim nos quintais das casas 14 e 22 (`Arbusto_Bairro_14`, `Capim_Bairro_22`).
   - **Zona 5 (Rua Norte & Bar do Zé):**
     - Araucária na mata de fundo (`Araucaria_BarDoZe`).
     - Grelha pluvial na sarjeta do asfalto antes do portão do Centro (`Grelha_RuaNorte`).
     - Caixas de manutenção e arbusto lateral (`Caixas_BarDoZe_Ext`, `Arbusto_RuaNorte`).
4. [x] **URP 2D Lit Material e Sorting Layers:**
   - Todos os sprites recebem automaticamente o material `Art/WorldLit` para responder à luz ambiente, luminárias âmbar e à lanterna em `F`.
   - Sorting order calculado por Y (`1000 - y*10`), garantindo que Marcus passe na frente de troncos/bancos e atrás de copas (com `SantaRitaForeground`).
5. [x] **Governança e Validação:**
   - Compilação via `dotnet build Assembly-CSharp.csproj`: êxito com 0 erros.
   - Sincronização do grafo AST via `scripts\graphify-refresh.ps1`.
   - Validação via `scripts\graphify-validate.ps1`: PASS (94 scripts C# cobertos).

---

## 4. Arquivos Modificados

- `Projeto Jogo 2D/Assets/Scripts/SantaRitaPlaceDressing.cs` (adicionado `ApplyExteriorC1` e suporte a `Art/World` e colisões configuráveis)
- `Projeto Jogo 2D/Assets/Scripts/Capitulo1Director.cs` (adicionado fallback para `Art/Props` e `Art/World` em `CarregarSprite`)
- `Projeto Jogo 2D/Assembly-CSharp.csproj` (adicionado `DicaDeTecla.cs` para compilação .NET limpa)
- `docs/ai-plans/2026-10-29_Venturini01_adicao-sprites-cenarios-flora-props.md` (criado: registro deste plano)
- `docs/ai-memory/CHANGELOG_AI.md` (atualizado)
