using UnityEngine;

public static class SantaRitaPlaceDressing
{
    public static void ApplyC1()
    {
        var posto   = GameObject.Find("Interior1_Posto");
        var mercado = GameObject.Find("Interior2_Mercadinho");
        var casa    = GameObject.Find("Interior3_CasaDoAvo");
        var bar     = GameObject.Find("Interior4_BarDoZe");

        Replace("Geladeira_Bebidas_Posto","Geladeira",2.4f);
        Replace("Prateleira_Salgadinhos_Posto","Prateleira",2.4f);
        Replace("Prateleira_Viagem_Posto","Prateleira",2.4f);
        Replace("Caixa_Conveni\u00eancia_Posto","Caixa",3.8f);
        Add(posto,"CafeParaViagem","Cafe",203.4f,-55.9f,2.5f);
        Add(posto,"ChimarraoPlantao","Chimarrao",212.6f,-57.1f,1.7f);

        Replace("Secos_Mercado_Noroeste","Prateleira",3.2f);
        Replace("Secos_Mercado_Nordeste","Prateleira",3.2f);
        Replace("Bebidas_Mercado","Geladeira",2.6f);
        Replace("Caixa_Mercadinho","Caixa",3.8f);
        Add(mercado,"EstoqueDaFrente","Prateleira",303.3f,-57f,2.2f);

        Add(casa,"FogaoDaCozinha","Fogao",401.9f,-52.1f,1.5f);
        Add(casa,"SofaDaSala","Sofa",413.9f,-56.4f,2.8f);
        Replace("TV_Sala_Avo","Televisor",1.5f);

        Replace("Balcao_Bar_Sprite","BalcaoBar",4.1f);
        Replace("Mesa_Bar_Esq","Chimarrao",2.3f);
        Replace("Mesa_Sinuca_Central","MesaSinuca",3.9f);
        Add(bar,"GeladeiraDoBar","Geladeira",516f,-49.7f,1.6f);
        Add(bar,"TelevisaoFutebol","Televisor",507.5f,-48.2f,1.5f);

        foreach (var n in new[]{"NPC_Frentista_Interior","NPC_Atendente_Interior","NPC_Ze_Interior","NPC_Frequentador_Interior"})
        {
            var g = GameObject.Find(n);
            if (g != null)
            {
                var r = g.GetComponent<SantaRitaNpcRoutine>() ?? g.AddComponent<SantaRitaNpcRoutine>();
                r.atividade   = n.Contains("Ze") ? "limpar" : n.Contains("Frequentador") ? "chimarrao" : "papeis";
                r.faseInicial = n.Length;
            }
        }

        ApplyExteriorC1();

        var litMat = Resources.Load<Material>("Art/WorldLit");
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>())
        {
            if (sr.sprite != null && sr.transform.position.y < -40)
            {
                switch (sr.sprite.name)
                {
                    case "WoodTable_Worn":  Replace(sr.name,"MesaMadeira",2.5f);  break;
                    case "FoldingChair":    Replace(sr.name,"CadeiraMadeira",.9f); break;
                    case "Wardrobe_Medium": Replace(sr.name,"ArmarioMadeira",2f);  break;
                    case "Cabinet_Medium":  Replace(sr.name,"ArmarioMadeira",1.8f);break;
                    case "Window_Curtain":  Replace(sr.name,"JanelaCortina",1.5f); break;
                    case "Radio":           Replace(sr.name,"RadioAntigo",.9f);    break;
                }
            }
            if (sr.sortingOrder < 0 || sr.name.Contains("Lanterna") || sr.name.Contains("Chuva")
                || sr.name.Contains("Parede") || sr.name == "Player") continue;
            if (litMat != null && sr.sharedMaterial != litMat) sr.sharedMaterial = litMat;
            sr.sortingOrder = 1000 - Mathf.RoundToInt(sr.transform.position.y * 10);
        }
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>())
            if ((sr.name.Contains("Arvore") || sr.name.Contains("Araucaria"))
                && sr.GetComponent<SantaRitaForeground>() == null)
                sr.gameObject.AddComponent<SantaRitaForeground>();
    }

    // ──────────────────────────────────────────────────────────────────────
    // ApplyC2 — veste os interiores de C2 com props autenticos existentes
    // em Art/Props/. Chamado por Capitulo2Director ao entrar em interior.
    // Nenhuma cena e regenerada; nenhum sprite novo criado.
    // ──────────────────────────────────────────────────────────────────────
    public static void ApplyC2()
    {
        ApplyBarDoZeInterior();
        ApplyCasaDoAvoInterior();
        ApplyMercadinhoDeposito();
        ApplyOficinaRadio();
        ApplyCasaCostureira();
        ApplyPrefeitura();
        ApplyRodoviaria();
        ApplyIgreja();
        ApplyAbrigo();

        // Material lit e profundidade por Y — identico ao padrao de C1
        var litMat = Resources.Load<Material>("Art/WorldLit");
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>())
        {
            if (sr == null || sr.sprite == null) continue;
            if (sr.sortingOrder < 0
                || sr.name.Contains("Parede") || sr.name.Contains("Piso")
                || sr.name.Contains("Friso")  || sr.name.Contains("Lambril")
                || sr.name.Contains("Chao")   || sr.name == "Player") continue;
            if (litMat != null && sr.sharedMaterial != litMat) sr.sharedMaterial = litMat;
            sr.sortingOrder = 1000 - Mathf.RoundToInt(sr.transform.position.y * 10);
        }
    }

    // ── Bar do Ze (C1 interior) ───────────────────────────────────────────
    // ApplyC1 ja cuida do balcao, sinuca e radio. Aqui adicionamos mesas e
    // cadeiras para frequentadores, geladeira e televisao (guard anti-duplicata).
    private static void ApplyBarDoZeInterior()
    {
        var bar = GameObject.Find("Casa") ?? GameObject.Find("Interior4_BarDoZe");
        if (bar == null) return;

        Add(bar,"Mesa_Frequentador_Dir","MesaMadeira", 0f,   -3.2f, 2.0f, false);
        Add(bar,"Cadeira_Freq_Dir_1",  "CadeiraMadeira",-0.8f,-2.6f, 0.85f,false);
        Add(bar,"Cadeira_Freq_Dir_2",  "CadeiraMadeira", 0.8f,-2.6f, 0.85f,false);
        Add(bar,"Mesa_Frequentador_Esq","MesaMadeira",-3.5f, -3.0f, 1.8f, false);
        Add(bar,"Cadeira_Freq_Esq",    "CadeiraMadeira",-3.5f,-2.4f, 0.85f,false);
        Add(bar,"ChimarraoBalcao",     "Chimarrao",    1.8f, -1.5f, 1.2f, false);

        if (GameObject.Find("GeladeiraDoBar")  == null)
            Add(bar,"GeladeiraDoBar", "Geladeira", 3.2f,-1.8f,1.6f,true);
        if (GameObject.Find("TelevisaoFutebol") == null)
            Add(bar,"TelevisaoFutebol","Televisor", 2.4f,-0.8f,1.5f,true);
    }

    // ── Casa do Avo Adilio (C1 interior) ─────────────────────────────────
    // ApplyC1 ja substitui Radio, WoodTable, FoldingChair, Wardrobe, Window.
    // Aqui adicionamos fogao, chimarrao e sofa com guard anti-duplicata.
    private static void ApplyCasaDoAvoInterior()
    {
        var casa = GameObject.Find("Casa") ?? GameObject.Find("Interior3_CasaDoAvo");
        if (casa == null) return;

        if (GameObject.Find("FogaoDaCozinha") == null)
            Add(casa,"FogaoDaCozinha","Fogao",   -2.5f,-3.8f,1.5f,true);
        if (GameObject.Find("ChimarraoMesa")  == null)
            Add(casa,"ChimarraoMesa","Chimarrao", 0.6f,-2.8f,1.2f,false);
        if (GameObject.Find("SofaDaSala")     == null)
            Add(casa,"SofaDaSala",   "Sofa",     -1.2f,-2.5f,2.4f,true);
    }

    // ── Mercadinho / Deposito (C2) ────────────────────────────────────────
    // Existem: EstoqueSecos, CargaOrganizada, MesaConferencia, RomaneioSobreMesa
    // Adiciona: prateleira extra, caixa registradora, geladeira, cargas deposito
    private static void ApplyMercadinhoDeposito()
    {
        var raiz = GameObject.Find("EstoqueSecos")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_MercadinhoDeposito");
        if (raiz == null) return;

        Add(raiz,"PrateleiraFundoDeposito","Prateleira",    0f,  4.5f,3.0f,true);
        Add(raiz,"CaixaAtendimento",       "Caixa",         2.8f, 0.2f,2.5f,true);
        Add(raiz,"GeladeiraBebidas",        "Geladeira",     3.8f,-1.0f,1.8f,true);
        Add(raiz,"CargaDeposito_A",        "CargasMercado",-1.5f, 4.8f,2.0f,true);
        Add(raiz,"CargaDeposito_B",        "EstoqueDeposito",-3.0f,4.5f,1.5f,true);
    }

    // ── Oficina de Radio (C2) ─────────────────────────────────────────────
    // Existem: BancadaEletronica, EstanteDePecas, AparelhoRecebido, ArmarioSeco, BancoEspera
    // Adiciona: radio antigo em reparo, cadeira de trabalho, caderno de servicos
    private static void ApplyOficinaRadio()
    {
        var raiz = GameObject.Find("BancadaEletronica")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_OficinaRadio");
        if (raiz == null) return;

        Add(raiz,"RadioEmReparo",   "RadioAntigo",  -1.2f, 0.8f,1.0f,false);
        Add(raiz,"CadeiraOficina",  "CadeiraMadeira",-1.8f,-0.5f,0.9f,false);
        Add(raiz,"CadernoServicos", "Caderno",        0.5f, 1.0f,0.7f,false);
    }

    // ── Casa da Costureira Celina (C2) ────────────────────────────────────
    // Existem: MaquinaCelina, MesaDoCha, SofaDaSala, TecidosGuardados, RetratoSala, TapeteDaSala
    // Adiciona: janela com cortina, cadeira na mesa do cha, armario para tecidos
    private static void ApplyCasaCostureira()
    {
        var raiz = GameObject.Find("MaquinaCelina")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_CasaCostureira");
        if (raiz == null) return;

        Add(raiz,"JanelaCostura",    "JanelaCortina", -2.5f,1.5f,1.5f,false);
        Add(raiz,"CadeiraChaDaCelina","CadeiraMadeira", 2.0f,-0.8f,0.9f,false);
        Add(raiz,"ArmarioDaTecela",  "ArmarioMadeira",  3.8f, 0.5f,2.0f,true);
    }

    // ── Prefeitura (C2) ───────────────────────────────────────────────────
    // Existem: ArquivoDeConsulta, ArquivoExpediente, BalcaoConsulta, BancoEspera x2,
    //          MesaLeitura, ExtratoDeContas, NPC_clarice, divisorias, AnexoManutencao
    // Adiciona: arquivo metalico no fundo, quadro de avisos, cadeira da funcionaria, caderno
    private static void ApplyPrefeitura()
    {
        var raiz = GameObject.Find("BalcaoConsulta")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_PrefeituraSantaRita");
        if (raiz == null) return;

        Add(raiz,"ArquivoMetalFundo",      "ArquivoMetal",    3.2f, 3.0f,2.0f,true);
        Add(raiz,"QuadroAvisosPrefeitura", "QuadroAvisos",   -1.5f, 2.5f,2.2f,false);
        Add(raiz,"CadeiraFuncionaria",     "CadeiraMadeira",   2.5f, 1.2f,0.9f,false);
        Add(raiz,"CadernoProtocolo",       "Caderno",           0.3f, 0.8f,0.7f,false);
    }

    // ── Rodoviaria (C2) ───────────────────────────────────────────────────
    // Existem: Guiche, Horarios, BancoEspera, Bagagem, Motorista_Escala, Passageira_Espera, NPC_dario
    // Adiciona: mesa publica no guiche, quadro de horarios na parede, banco extra, cadeira do motorista
    private static void ApplyRodoviaria()
    {
        var raiz = GameObject.Find("Guiche")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_RodoviariaSantaRita");
        if (raiz == null) return;

        Add(raiz,"MesaGuiche",       "MesaPublica",     0f,  1.5f,2.8f,true);
        Add(raiz,"QuadroHorarios",   "QuadroAvisos",   -2.8f, 2.8f,2.4f,false);
        Add(raiz,"BancoEsperaExtra", "BancoPraca",     -3.5f,-1.2f,2.6f,true);
        Add(raiz,"CadeiraMotorista", "CadeiraMadeira",  1.8f, 3.0f,0.9f,false);
    }

    // ── Igreja Matriz (C2) ────────────────────────────────────────────────
    // Existem: Altar, ApoioDasVelas, BancoDaNave, MesaDaSacristia, MuralComunitario,
    //          DegrauAltar, NPC_bento, janelas, friso, lambril
    // Adiciona: armario de velas na sacristia, quadro paroquial, cadeira sacristia, janela cortina
    private static void ApplyIgreja()
    {
        var raiz = GameObject.Find("Altar")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_IgrejaMatriz");
        if (raiz == null) return;

        Add(raiz,"ArmarioVelasSacristia","ArmarioVelas",    3.5f, 2.0f,2.0f,true);
        Add(raiz,"QuadroComunicados",    "QuadroAvisos",    4.0f, 3.5f,2.0f,false);
        Add(raiz,"CadeiraSacristia",     "CadeiraMadeira",  3.8f, 1.2f,0.9f,false);
        Add(raiz,"JanelaLateralIgreja",  "JanelaCortina",  -4.2f, 2.0f,1.5f,false);
    }

    // ── Abrigo do Cemiterio (C2) ──────────────────────────────────────────
    // Existem: ArmarioVelas, BancoDaRonda, FerramentasGuardadas, MesaDaRonda, CadernoDeManutencao
    // Adiciona: caixas de manutencao extra, apoio de agua quente, caderno de ronda extra
    private static void ApplyAbrigo()
    {
        var raiz = GameObject.Find("MesaDaRonda")?.transform.parent?.gameObject
                ?? GameObject.Find("Capitulo2_AbrigoCemiterio");
        if (raiz == null) return;

        Add(raiz,"CaixasManutencaoExtra","CaixasManutencao",-2.0f, 2.5f,1.6f,true);
        Add(raiz,"ApoioTermoGuarda",     "ApoioAguaQuente",  0.8f, 0.8f,0.9f,false);
        Add(raiz,"CadernoRondaExtra",    "Caderno",          -0.5f, 0.9f,0.7f,false);
    }

    // ── C1 Exterior ───────────────────────────────────────────────────────
    private static void ApplyExteriorC1()
    {
        Replace("Banco_Rodovia",       "BancoPraca",      2.4f);
        Replace("Banco_Venda",         "BancoPraca",      2.4f);
        Replace("Banco_Encruzilhada",  "BancoPraca",      2.4f);
        Replace("Placa_Horarios_Rodovia","QuadroAvisos",  1.9f);
        Replace("Mural_Encruzilhada",  "QuadroAvisos",    2.0f);
        Replace("Caixotes_Barro_Sul",  "CaixasManutencao",1.5f);
        Replace("Caixotes_Venda",      "CargasMercado",   1.8f);

        var z1 = GameObject.Find("Zona1_Rodovia");
        if (z1 != null)
        {
            Add(z1,"Araucaria_Rodovia_1",  "Araucaria",  101.5f,16.5f,4.2f,true);
            Add(z1,"Araucaria_Rodovia_2",  "Araucaria",  126.0f,16.8f,4.0f,true);
            Add(z1,"Arbusto_Rodovia_Sul_1","Shrub",      106.5f, 5.6f,1.3f,false);
            Add(z1,"Arbusto_Rodovia_Sul_2","Shrub",      124.5f, 5.6f,1.3f,false);
            Add(z1,"Samambaia_Rodovia_Sul","Fern",        121.2f, 6.4f,1.0f,false);
            Add(z1,"Capim_Rodovia_Sul",    "GrassClump", 111.5f, 5.8f,0.9f,false);
        }

        var z2 = GameObject.Find("Zona2_Posto");
        if (z2 != null)
        {
            Add(z2,"Araucaria_Posto_Fundo","Araucaria",  220.0f,17.5f,4.2f,true);
            Add(z2,"Samambaia_Venda",      "Fern",       223.5f,13.8f,1.0f,false);
            Add(z2,"Arbusto_Venda",        "Shrub",      233.8f,14.0f,1.3f,false);
            Add(z2,"Grelha_Posto",         "GrelhaDrenagem",213.2f,12.2f,1.0f,false);
            Add(z2,"ApoioAgua_Posto",      "ApoioAguaQuente",209.5f,13.6f,0.9f,true);
        }

        var z3 = GameObject.Find("Zona3_Encruzilhada");
        if (z3 != null)
        {
            Add(z3,"Araucaria_Encruzilhada",  "Araucaria",   328.0f,19.5f,4.0f,true);
            Add(z3,"Cargas_Mercadinho_Calcada","CargasMercado",308.2f,13.4f,1.6f,true);
            Add(z3,"Grelha_Encruzilhada",     "GrelhaDrenagem",313.5f,12.3f,1.0f,false);
            Add(z3,"Flores_Canteiro_Esquina", "Flowers",     333.5f,13.5f,1.2f,false);
            Add(z3,"Samambaia_Esquina",       "Fern",        337.2f,13.8f,1.0f,false);
        }

        var z4 = GameObject.Find("Zona4_Bairro");
        if (z4 != null)
        {
            Add(z4,"Araucaria_Fundo_Avo_1","Araucaria",421.0f,29.0f,4.4f,true);
            Add(z4,"Araucaria_Fundo_Avo_2","Araucaria",434.0f,29.2f,4.2f,true);
            Add(z4,"Samambaia_Avo",        "Fern",      427.5f,24.6f,1.0f,false);
            Add(z4,"Flores_Avo",           "Flowers",   419.5f,24.6f,1.2f,false);
            Add(z4,"Arbusto_Bairro_14",    "Shrub",     407.2f,12.8f,1.3f,false);
            Add(z4,"Capim_Bairro_22",      "GrassClump",433.8f,12.6f,0.9f,false);
        }

        var z5 = GameObject.Find("Zona5_RuaNorteBar");
        if (z5 != null)
        {
            Add(z5,"Araucaria_BarDoZe",  "Araucaria",     521.0f,17.5f,4.2f,true);
            Add(z5,"Grelha_RuaNorte",    "GrelhaDrenagem",529.5f,12.2f,1.0f,false);
            Add(z5,"Caixas_BarDoZe_Ext", "CaixasManutencao",522.8f,13.5f,1.5f,true);
            Add(z5,"Arbusto_RuaNorte",   "Shrub",         507.0f,13.6f,1.3f,false);
        }
    }

    // ── Utilitarios ───────────────────────────────────────────────────────
    static void Replace(string name, string key, float width)
    {
        var g = GameObject.Find(name);
        if (g == null) return;
        var sr = g.GetComponent<SpriteRenderer>();
        var sprite = Resources.Load<Sprite>("Art/Props/" + key)
                  ?? Resources.Load<Sprite>("Art/World/" + key);
        if (sr == null || sprite == null) return;
        var b = g.GetComponent<BoxCollider2D>();
        Vector2 sz  = b != null ? Vector2.Scale(b.size,   g.transform.lossyScale) : Vector2.zero;
        Vector2 off = b != null ? Vector2.Scale(b.offset, g.transform.lossyScale) : Vector2.zero;
        sr.sprite = sprite;
        g.transform.localScale = Vector3.one * width / sprite.bounds.size.x;
        if (b != null)
        {
            b.size   = sz  / g.transform.localScale.x;
            b.offset = off / g.transform.localScale.x;
        }
    }

    static void Add(GameObject parent, string name, string key,
                    float x, float y, float width, bool solid = true)
    {
        if (parent == null || parent.transform.Find(name) != null) return;
        var sprite = Resources.Load<Sprite>("Art/Props/" + key)
                  ?? Resources.Load<Sprite>("Art/World/" + key);
        if (sprite == null) return;
        var g  = new GameObject(name);
        g.transform.SetParent(parent.transform, false);
        g.transform.position   = new Vector3(x, y, 0);
        g.transform.localScale = Vector3.one * width / sprite.bounds.size.x;
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        if (solid)
        {
            var b     = g.AddComponent<BoxCollider2D>();
            float dep = key == "Araucaria" ? 0.9f : 0.4f;
            b.size   = new Vector2(sprite.bounds.size.x * .75f, dep / g.transform.localScale.x);
            b.offset = new Vector2(0f, (dep * .5f) / g.transform.localScale.x);
        }
    }
}
