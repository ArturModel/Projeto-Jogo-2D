# Mapa de Cenas, Zonas e Fluxo Narrativo — Santa Rita (Capítulo 1)

**Atualizado em:** 26/10/2026  
**Escopo:** Cenas Unity (`Assets/Scenes/`), Zonas de `[[Capitulo1Director]]`, Cadeia de Pistas e Flags Canônicas (`[[Flags]]`).

---

## 1. Cenas do Jogo (`Assets/Scenes/`)

| Cena | Caminho | Controladores Principais | Papel no Fluxo do Jogo |
| :--- | :--- | :--- | :--- |
| **MainMenu** | `Assets/Scenes/MainMenu.unity` | [[MainMenuActions]], [[MenuPanelTransition]], [[OptionsPanelController]], [[AudioSettingsController]], [[VideoSettingsController]], [[SubtitleSettingsController]] | Tela título com vídeo/atmosfera de fundo, início de Novo Jogo (`Prologo`), carregamento de Save Manual por Slot de Orelhão (`Continuar`) e opções. |
| **Prologo** | `Assets/Scenes/Prologo.unity` | [[PrologoDirector]], [[PrologoVideo]], [[EstradaScroller]], [[OlharPrimeiraPessoa]], [[PonteiroCarro]], [[CelularCarro]], [[RadioCarro]], [[PortaLuvasCarro]], [[LimpadorParabrisa]] | Viagem de Marcus em seu próprio carro sob a tempestade de 2004 rumo a Santa Rita após Adílio parar de atender no mesmo dia. |
| **EntradaDaCidade** | `Assets/Scenes/Cidade/EntradaDaCidade.unity` | [[ChegadaEmSantaRita]], [[Capitulo1Director]], [[PlayerController]], [[CameraFollow]], [[Chuva]], [[MapaInterativoUI]], [[PayphoneCheckpoint]] | Mundo multi-zonas de Santa Rita contendo as 5 regiões externas e os acessos aos 4 interiores exploráveis do Capítulo 1. |
| **CasaDoAvo** | `Assets/Scenes/Cidade/CasaDoAvo.unity` | [[DentroDaCasaDoAvo]], [[Capitulo1Director]], [[InteracaoCapitulo1]], [[ItemDeCena]] | Interior da casa de Adílio (Bairro das Casas, nº 40), onde Marcus investiga a rotina interrompida no mesmo dia. |
| **BarDoZe** | `Assets/Scenes/Cidade/BarDoZe.unity` | [[Capitulo1Director]], [[InteracaoCapitulo1]], [[SinucaMinigameUI]] | Interior do Bar do Zé na Rua Norte, palco do interrogatório de Zé e do duelo de Sinuca em série Melhor de 3. |

---

## 2. As 9 Zonas e Interiores do `Capitulo1Director`

O script [[Capitulo1Director]] (em `ChegadaEmSantaRita.cs`) organiza o espaço urbano de Santa Rita em **5 Zonas Externas** e **4 Interiores**, conectados por [[TransicaoDeZonaTrigger]] com cortes de fade e letreiros topográficos via [[SceneFader]]:

```
[Zona1_Rodovia] (Carro de Marcus: Lanterna + Mapa)
       │
       ▼ (Leste)
[Zona2_Posto] ◄───► [Interior_Posto] (Frentista + Livro de Plantão + TV)
       │
       ▼ (Leste)
[Zona3_Encruzilhada] ◄───► [Interior_Mercadinho] (Atendente + Caderno de Fiado)
  ├── [Orelhão da Encruzilhada] (Save Manual #1)
  │
  ▼ (Norte)
[Zona4_Bairro] (Vizinha Dona Lurdes + Morador + Galpão)
  ├──► [Interior_CasaDoAvo] (Café morno + Cartas + Nota Fiscal 2004 + Desordem + Telefone)
  │
  ▼ (Subida Norte — exige visitar Casa do Avô primeiro)
[Zona5_RuaNorteBar]
  ├── [Orelhão do Bar do Zé] (Save Manual #2)
  ├──► [Interior_BarDoZe] (NPC Zé + Mesa de Sinuca → [[SinucaMinigameUI]])
  └──► [Limiar do Centro] (Portão trancado até vencer Zé; atravessá-lo encerra o Cap. 1)
```

### Coordenadas e Mapeamento UV no Radar (`[[MapaInterativoUI]]`)
| `ZonaId` | Título no Banner | Tipo | Limites de Mundo `(min → max)` | UV no Mapa (`uvMin → uvMax`) |
| :--- | :--- | :---: | :--- | :--- |
| `Zona1_Rodovia` | RODOVIA DE ENTRADA — SANTA RITA | Exterior | `(100, 0) → (136, 20)` | `(0.09, 0.14) → (0.16, 0.21)` |
| `Zona2_Posto` | AVENIDA SUL — POSTO & VENDA ANTIGA | Exterior | `(200, 0) → (238, 22)` | `(0.16, 0.19) → (0.24, 0.25)` |
| `Interior_Posto` | CONVENIÊNCIA DO POSTO | Interior | `(200, -60) → (216, -48)` | `(0.17, 0.20) → (0.19, 0.22)` |
| `Zona3_Encruzilhada` | ENCRUZILHADA DO MERCADINHO | Exterior | `(300, 0) → (340, 26)` | `(0.24, 0.22) → (0.32, 0.29)` |
| `Interior_Mercadinho` | MERCADINHO SANTA RITA | Interior | `(300, -60) → (318, -47)` | `(0.25, 0.23) → (0.27, 0.25)` |
| `Zona4_Bairro` | BAIRRO DAS CASAS | Exterior | `(400, 0) → (444, 34)` | `(0.17, 0.29) → (0.29, 0.43)` |
| `Interior_CasaDoAvo` | CASA DO AVÔ ADÍLIO — Nº 40 | Interior | `(400, -60) → (418, -46)` | `(0.23, 0.37) → (0.25, 0.39)` |
| `Zona5_RuaNorteBar` | RUA NORTE — EXTERIOR DO BAR DO ZÉ | Exterior | `(500, 0) → (542, 28)` | `(0.29, 0.43) → (0.39, 0.56)` |
| `Interior_BarDoZe` | BAR DO ZÉ — SINUCA & BEBIDAS | Interior | `(500, -60) → (520, -46)` | `(0.32, 0.47) → (0.35, 0.50)` |

---

## 3. Cadeia de Investigação e Flags Canônicas (`[[Flags]]`)

A progressão narrativa do Capítulo 1 é governada pelas flags em `[[GameState]]`, que atualizam automaticamente o objetivo ativo e adicionam notas categorizadas (`[FATO]`, `[PISTA]`, `[HIPÓTESE]`, `[ITEM]`) no caderno de `[[MapaInterativoUI]]`:

1. **Chegada e Equipamento (`Zona1_Rodovia`):**
   - Examinar o carro de Marcus (`TipoInteracao.CarroMarcus`) marca `Flags.MapaDoCarroObtido` e `Flags.LanternaDoCarroObtida`.
   - Libera a travessia para `Zona2_Posto` e ativa a lanterna (`LanternaNaMao`) em `[[PlayerController]]`.
2. **Posto da Entrada (`Zona2_Posto` / `Interior_Posto`):**
   - Conversar com o Frentista (`TipoInteracao.NpcFrentista`) marca `Flags.FalouFrentista`, confirmando que Adílio esteve ativo hoje cedo e apontando a subida para o Bairro das Casas (nº 40).
3. **Investigação Doméstica (`Interior_CasaDoAvo` / `CasaDoAvo.unity`):**
   - Entrar na casa marca `Flags.ChegouCasaAvo`.
   - **4 Pistas Principais da Rotina Interrompida:**
     1. `Flags.ExaminouCafeCasa` (`Pista_CafeAindaQuente`): Café passado hoje ainda morno no bule.
     2. `Flags.ExaminouCartasCasa` (`Pista_CartasMesa`): Jogo de paciência interrompido na metade.
     3. `Flags.AchouReciboMercadinho` (`Pista_NotaFiscal2004`): Cupom fiscal emitido hoje (2004) no Mercadinho com anotação sobre o Bar do Zé / Rua Norte.
     4. `Flags.ExaminouDesordemCasa` (`Pista_DesordemPorta`): Banco caído e marcas de barro perto da saída.
   - Ao reunir as pistas, `InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo()` marca `Flags.Pista_CasaAvoCompleta` e direciona Marcus ao Mercadinho.
4. **Mercadinho da Encruzilhada (`Interior_Mercadinho`):**
   - Conversar com a Atendente (`TipoInteracao.NpcAtendenteMercadinho`) após ter o recibo marca `Flags.FalouAtendenteMercadinho` e `Flags.Pista_RumoBarDoZe`, revelando que um estranho vindo do Centro perguntou por Adílio após falar com Zé.
5. **Bar do Zé e Duelo de Sinuca (`Interior_BarDoZe` / `BarDoZe.unity`):**
   - Falar com Zé (`TipoInteracao.NpcZeBar`) marca `Flags.ConheceuZeBar` e abre o minigame `[[SinucaMinigameUI]]`.
   - Vencer a série Melhor de 3 (`2-0` ou `2-1`) marca:
     - `Flags.VenceuZeSinuca`
     - `Flags.FichaZeObtida` (`ficha_clube_ze_01` — 1ª Ficha do Clube de Santa Rita)
     - `Flags.Pista_AdilioNaoSaiuVoluntariamente`
     - `Flags.CentroLiberado` (`LimiarCentroDesbloqueado`)
6. **Fechamento do Capítulo 1 (`Zona5_RuaNorteBar`):**
   - Com `Flags.CentroLiberado`, interagir com o portão norte (`TipoInteracao.LimiarCentro`) marca `Flags.Capitulo1Concluido` e abre o painel de encerramento do Vertical Slice (`Capitulo1Director.AbrirPainelFechamento()`), permitindo salvar ou retornar ao menu.
