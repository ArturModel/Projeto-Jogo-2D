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
            sr.sortingOrder=1000-Mathf.RoundToInt(sr.transform.position.y*10);
        }
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>())
            if(sr.name.Contains("Arvore")&&sr.GetComponent<SantaRitaForeground>()==null)sr.gameObject.AddComponent<SantaRitaForeground>();
    }
    static void Replace(string name,string key,float width)
    {
        var g=GameObject.Find(name);if(g==null)return;var sr=g.GetComponent<SpriteRenderer>();var sprite=Resources.Load<Sprite>("Art/Props/"+key);if(sr==null||sprite==null)return;
        var b=g.GetComponent<BoxCollider2D>();Vector2 sz=b!=null?Vector2.Scale(b.size,g.transform.lossyScale):Vector2.zero,off=b!=null?Vector2.Scale(b.offset,g.transform.lossyScale):Vector2.zero;
        sr.sprite=sprite;g.transform.localScale=Vector3.one*width/sprite.bounds.size.x;
        if(b!=null){b.size=sz/g.transform.localScale.x;b.offset=off/g.transform.localScale.x;}
    }
    static void Add(GameObject parent,string name,string key,float x,float y,float width)
    {
        if(parent==null||parent.transform.Find(name)!=null)return;var sprite=Resources.Load<Sprite>("Art/Props/"+key);if(sprite==null)return;
        var g=new GameObject(name);g.transform.SetParent(parent.transform,false);g.transform.position=new Vector3(x,y,0);g.transform.localScale=Vector3.one*width/sprite.bounds.size.x;
        var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;var b=g.AddComponent<BoxCollider2D>();b.size=new Vector2(sprite.bounds.size.x*.8f,.4f/g.transform.localScale.x);b.offset=new Vector2(0,.2f/g.transform.localScale.x);
    }
}
