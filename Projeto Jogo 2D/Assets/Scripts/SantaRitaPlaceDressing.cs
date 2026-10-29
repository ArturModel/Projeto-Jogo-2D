using UnityEngine;

public static class SantaRitaPlaceDressing
{
    public static void ApplyC1()
    {
        var posto=GameObject.Find("Interior1_Posto");var mercado=GameObject.Find("Interior2_Mercadinho");
        var casa=GameObject.Find("Interior3_CasaDoAvo");var bar=GameObject.Find("Interior4_BarDoZe");
        Replace("Geladeira_Bebidas_Posto","Geladeira",2.4f);Replace("Prateleira_Salgadinhos_Posto","Prateleira",2.4f);Replace("Prateleira_Viagem_Posto","Prateleira",2.4f);
        Replace("Caixa_Conveniência_Posto","Caixa",3.8f);
        Add(posto,"CafeParaViagem","Cafe",203.4f,-55.9f,2.5f);Add(posto,"ChimarraoPlantao","Chimarrao",212.6f,-57.1f,1.7f);
        Replace("Secos_Mercado_Noroeste","Prateleira",3.2f);Replace("Secos_Mercado_Nordeste","Prateleira",3.2f);Replace("Bebidas_Mercado","Geladeira",2.6f);Replace("Caixa_Mercadinho","Caixa",3.8f);
        Add(mercado,"EstoqueDaFrente","Prateleira",303.3f,-57,2.2f);
        Add(casa,"FogaoDaCozinha","Fogao",401.9f,-52.1f,1.5f);Add(casa,"SofaDaSala","Sofa",413.9f,-56.4f,2.8f);Replace("TV_Sala_Avo","Televisor",1.5f);
        Replace("Balcao_Bar_Sprite","BalcaoBar",4.1f);Replace("Mesa_Bar_Esq","Chimarrao",2.3f);
        Replace("Mesa_Sinuca_Central","MesaSinuca",3.9f);
        Add(bar,"GeladeiraDoBar","Geladeira",516,-49.7f,1.6f);Add(bar,"TelevisaoFutebol","Televisor",507.5f,-48.2f,1.5f);
        foreach(var n in new[]{"NPC_Frentista_Interior","NPC_Atendente_Interior","NPC_Ze_Interior","NPC_Frequentador_Interior"})
        {var g=GameObject.Find(n);if(g!=null){var r=g.GetComponent<SantaRitaNpcRoutine>()??g.AddComponent<SantaRitaNpcRoutine>();r.atividade=n.Contains("Ze")?"limpar":n.Contains("Frequentador")?"chimarrao":"papeis";r.faseInicial=n.Length;}}

        ApplyExteriorC1();

        var litMat = Resources.Load<Material>("Art/WorldLit");
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>())
        {
            if(sr.sprite!=null&&sr.transform.position.y< -40)
            {
                switch(sr.sprite.name)
                {
                    case "WoodTable_Worn": Replace(sr.name,"MesaMadeira",2.5f);break;
                    case "FoldingChair": Replace(sr.name,"CadeiraMadeira",.9f);break;
                    case "Wardrobe_Medium": Replace(sr.name,"ArmarioMadeira",2);break;
                    case "Cabinet_Medium": Replace(sr.name,"ArmarioMadeira",1.8f);break;
                    case "Window_Curtain": Replace(sr.name,"JanelaCortina",1.5f);break;
                    case "Radio": Replace(sr.name,"RadioAntigo",.9f);break;
                }
            }
            if(sr.sortingOrder<0||sr.name.Contains("Lanterna")||sr.name.Contains("Chuva")||sr.name.Contains("Parede")||sr.name=="Player")continue;
            if(litMat!=null&&sr.sharedMaterial!=litMat)sr.sharedMaterial=litMat;
            sr.sortingOrder=1000-Mathf.RoundToInt(sr.transform.position.y*10);
        }
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>())
            if((sr.name.Contains("Arvore")||sr.name.Contains("Araucaria"))&&sr.GetComponent<SantaRitaForeground>()==null)sr.gameObject.AddComponent<SantaRitaForeground>();
    }

    private static void ApplyExteriorC1()
    {
        // 1. Substituir mobiliário e placas genéricas pelos assets autênticos aprovados
        Replace("Banco_Rodovia","BancoPraca",2.4f);
        Replace("Banco_Venda","BancoPraca",2.4f);
        Replace("Banco_Encruzilhada","BancoPraca",2.4f);
        Replace("Placa_Horarios_Rodovia","QuadroAvisos",1.9f);
        Replace("Mural_Encruzilhada","QuadroAvisos",2.0f);
        Replace("Caixotes_Barro_Sul","CaixasManutencao",1.5f);
        Replace("Caixotes_Venda","CargasMercado",1.8f);

        // 2. Zona 1: Rodovia (Araucárias sulistas no horizonte + vegetação rasteira)
        var z1 = GameObject.Find("Zona1_Rodovia");
        if(z1!=null)
        {
            Add(z1,"Araucaria_Rodovia_1","Araucaria",101.5f,16.5f,4.2f,true);
            Add(z1,"Araucaria_Rodovia_2","Araucaria",126.0f,16.8f,4.0f,true);
            Add(z1,"Arbusto_Rodovia_Sul_1","Shrub",106.5f,5.6f,1.3f,false);
            Add(z1,"Arbusto_Rodovia_Sul_2","Shrub",124.5f,5.6f,1.3f,false);
            Add(z1,"Samambaia_Rodovia_Sul","Fern",121.2f,6.4f,1.0f,false);
            Add(z1,"Capim_Rodovia_Sul","GrassClump",111.5f,5.8f,0.9f,false);
        }

        // 3. Zona 2: Posto & Antiga Venda
        var z2 = GameObject.Find("Zona2_Posto");
        if(z2!=null)
        {
            Add(z2,"Araucaria_Posto_Fundo","Araucaria",220.0f,17.5f,4.2f,true);
            Add(z2,"Samambaia_Venda","Fern",223.5f,13.8f,1.0f,false);
            Add(z2,"Arbusto_Venda","Shrub",233.8f,14.0f,1.3f,false);
            Add(z2,"Grelha_Posto","GrelhaDrenagem",213.2f,12.2f,1.0f,false);
            Add(z2,"ApoioAgua_Posto","ApoioAguaQuente",209.5f,13.6f,0.9f,true);
        }

        // 4. Zona 3: Encruzilhada & Mercadinho
        var z3 = GameObject.Find("Zona3_Encruzilhada");
        if(z3!=null)
        {
            Add(z3,"Araucaria_Encruzilhada","Araucaria",328.0f,19.5f,4.0f,true);
            Add(z3,"Cargas_Mercadinho_Calcada","CargasMercado",308.2f,13.4f,1.6f,true);
            Add(z3,"Grelha_Encruzilhada","GrelhaDrenagem",313.5f,12.3f,1.0f,false);
            Add(z3,"Flores_Canteiro_Esquina","Flowers",333.5f,13.5f,1.2f,false);
            Add(z3,"Samambaia_Esquina","Fern",337.2f,13.8f,1.0f,false);
        }

        // 5. Zona 4: Bairro Residencial (Casa do Avô Adílio)
        var z4 = GameObject.Find("Zona4_Bairro");
        if(z4!=null)
        {
            Add(z4,"Araucaria_Fundo_Avo_1","Araucaria",421.0f,29.0f,4.4f,true);
            Add(z4,"Araucaria_Fundo_Avo_2","Araucaria",434.0f,29.2f,4.2f,true);
            Add(z4,"Samambaia_Avo","Fern",427.5f,24.6f,1.0f,false);
            Add(z4,"Flores_Avo","Flowers",419.5f,24.6f,1.2f,false);
            Add(z4,"Arbusto_Bairro_14","Shrub",407.2f,12.8f,1.3f,false);
            Add(z4,"Capim_Bairro_22","GrassClump",433.8f,12.6f,0.9f,false);
        }

        // 6. Zona 5: Rua Norte & Bar do Zé
        var z5 = GameObject.Find("Zona5_RuaNorteBar");
        if(z5!=null)
        {
            Add(z5,"Araucaria_BarDoZe","Araucaria",521.0f,17.5f,4.2f,true);
            Add(z5,"Grelha_RuaNorte","GrelhaDrenagem",529.5f,12.2f,1.0f,false);
            Add(z5,"Caixas_BarDoZe_Ext","CaixasManutencao",522.8f,13.5f,1.5f,true);
            Add(z5,"Arbusto_RuaNorte","Shrub",507.0f,13.6f,1.3f,false);
        }
    }

    static void Replace(string name,string key,float width)
    {
        var g=GameObject.Find(name);if(g==null)return;var sr=g.GetComponent<SpriteRenderer>();
        var sprite=Resources.Load<Sprite>("Art/Props/"+key)??Resources.Load<Sprite>("Art/World/"+key);
        if(sr==null||sprite==null)return;
        var b=g.GetComponent<BoxCollider2D>();Vector2 sz=b!=null?Vector2.Scale(b.size,g.transform.lossyScale):Vector2.zero,off=b!=null?Vector2.Scale(b.offset,g.transform.lossyScale):Vector2.zero;
        sr.sprite=sprite;g.transform.localScale=Vector3.one*width/sprite.bounds.size.x;
        if(b!=null){b.size=sz/g.transform.localScale.x;b.offset=off/g.transform.localScale.x;}
    }

    static void Add(GameObject parent,string name,string key,float x,float y,float width,bool solid=true)
    {
        if(parent==null||parent.transform.Find(name)!=null)return;
        var sprite=Resources.Load<Sprite>("Art/Props/"+key)??Resources.Load<Sprite>("Art/World/"+key);
        if(sprite==null)return;
        var g=new GameObject(name);g.transform.SetParent(parent.transform,false);g.transform.position=new Vector3(x,y,0);g.transform.localScale=Vector3.one*width/sprite.bounds.size.x;
        var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;
        if(solid)
        {
            var b=g.AddComponent<BoxCollider2D>();
            float depth = key=="Araucaria" ? 0.9f : 0.4f;
            b.size=new Vector2(sprite.bounds.size.x*.75f, depth/g.transform.localScale.x);
            b.offset=new Vector2(0, (depth*.5f)/g.transform.localScale.x);
        }
    }
}
