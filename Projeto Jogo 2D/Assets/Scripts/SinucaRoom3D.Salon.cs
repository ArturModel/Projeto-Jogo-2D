using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class SinucaRoom3D
{
    private Transform ceilingFan, campainhaBalcao;
    private float tempoCampainha = 1f;
    private readonly List<Transform> contactActors = new List<Transform>();
    private readonly List<Transform> contactShadows = new List<Transform>();
    private readonly List<Renderer> overheadDetails = new List<Renderer>();
    private readonly Transform[] janelasFachada = new Transform[2];
    private readonly GameObject[] marcasJanela = new GameObject[2];
    private Material vidroJanela;
    private float intensidadeChuvaJanela = -1f;

    // All additions stay above head height or within the existing wall clearance.
    // Entry, seating, service and play retain the shared physical plan.
    private void BuildSalonDepth()
    {
        BuildSalonDecoration();
        BuildSalonPatrons();
        MontarCampainhaDoBalcao();
        RoomFill("Luz baixa da mesa da janela", new Vector3(3.56f, 2.46f, -3.10f),
            new Color(1f, .75f, .49f), .87f, 4.5f);
        var timber=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var stone=Mat("basePilares",new Color(.30f,.29f,.25f));
        foreach(float z in new[]{-6.8f,-1.7f,3.45f})
        {
            foreach(float x in new[]{-5.29f,5.29f})
            {
                Box("PilarEstrutural",new Vector3(x,1.43f,z),new Vector3(.22f,2.86f,.24f),timber);
                Box("SapataPilar",new Vector3(x,.17f,z),new Vector3(.25f,.34f,.28f),stone);
                Box("CapitelPilar",new Vector3(x,2.70f,z),new Vector3(.32f,.13f,.38f),timber);
                var brace=Box("MaoFrancesa",new Vector3(x-Mathf.Sign(x)*.26f,2.68f,z),new Vector3(.10f,.64f,.12f),timber);
                brace.transform.localRotation=Quaternion.Euler(0,0,Mathf.Sign(x)*-45);
                overheadDetails.Add(brace.GetComponent<Renderer>());
            }
            overheadDetails.Add(Box("TravessaSala",new Vector3(0,2.86f,z),new Vector3(10.7f,.20f,.25f),timber).GetComponent<Renderer>());
        }
        foreach(float x in new[]{-3.6f,0f,3.6f})
            overheadDetails.Add(Box("LongarinaForro",new Vector3(x,2.99f,-1.8f),new Vector3(.10f,.08f,11.2f),timber).GetComponent<Renderer>());

        BuildWindowBay(0, "JanelaEsquerdaDaFachada", -JanelaFachadaX);
        BuildWindowBay(1, "JanelaDireitaDaFachada", JanelaFachadaX);
        ExteriorInterior3D.Montar(scenery, camera, Layer, new Vector3(0,0,SoleiraBar), 180f, ExteriorInterior3D.Local.Bar);
        // The service zone reads as built cabinetry rather than a flat back wall.
        foreach(float x in new[]{-3.73f,-.43f})
            Box("MontantePrateleira",new Vector3(x,1.95f,3.46f),new Vector3(.09f,1.58f,.20f),timber);
        Box("CoroamentoBalcao",new Vector3(-2.08f,2.77f,3.40f),new Vector3(3.55f,.16f,.34f),timber);
        Box("SoleiraMadeira",new Vector3(0,.012f,-7.12f),new Vector3(1.15f,.024f,.24f),timber);
        foreach(float x in new[]{-5.34f,5.34f})
            Box("CimalhaLateral",new Vector3(x,2.77f,-1.8f),new Vector3(.14f,.11f,11.2f),timber);

        var fan=new GameObject("VentiladorTeto").transform;fan.SetParent(scenery,false);
        fan.localPosition=new Vector3(0,2.70f,-4.8f);
        Cylinder("HasteVentilador",new Vector3(0,.15f,0),.018f,.30f,timber,fan);
        Cylinder("MotorVentilador",Vector3.zero,.10f,.10f,Mat("ferroPintado",new Color(.055f,.065f,.060f)),fan);
        ceilingFan=new GameObject("Helice").transform;ceilingFan.SetParent(fan,false);
        for(int i=0;i<3;i++)
        {
            var blade=Box("PaVentilador",new Vector3(0,-.04f,.32f),new Vector3(.15f,.018f,.62f),timber,ceilingFan);
            blade.transform.RotateAround(fan.position,Vector3.up,i*120);
        }
        overheadDetails.AddRange(fan.GetComponentsInChildren<Renderer>());

        // Bake contact footprints once. Moving actors only update transforms.
        foreach(var p in SantaRitaBarLayout.Props)
        {
            if(p.kind==SantaRitaBarLayout.Kind.Table)
                ContactPatch("ContatoMesa",Plan(p.position),new Vector2(p.width*.65f,p.width*.55f),.23f);
            else if(p.kind==SantaRitaBarLayout.Kind.Counter)
                ContactPatch("ContatoBalcao",Plan(p.position),new Vector2(3.6f,1.0f),.34f);
            else if(p.id=="GeladeiraDoBar"||p.id=="EngradadosDoBar")
                ContactPatch("ContatoMovel",Plan(p.position),new Vector2(.95f,.80f),.36f);
        }
        foreach(var a in actors)
        {
            if(a.definition.kind!=SantaRitaBarLayout.Kind.Actor)continue;
            contactActors.Add(a.root);
            contactShadows.Add(ContactPatch("Contato_"+a.definition.id,a.root.localPosition,new Vector2(.75f,.48f),.35f));
        }
        ContactPatch("OclusaoMesaSinuca",Vector3.zero,new Vector2(3.45f,1.78f),.24f);
    }

    private void MontarCampainhaDoBalcao()
    {
        var latao = Mat("lataoCampainha", new Color(.57f, .40f, .19f));
        var baseEscura = Mat("baseCampainha", new Color(.17f, .13f, .10f));
        var balcao = scenery.Find("Balcao_Bar_Sprite");
        if (balcao == null) return;
        var ponto = balcao.localPosition + new Vector3(1.03f, 1.066f, -.26f);
        Cylinder("BaseCampainha", ponto, .105f, .032f, baseEscura, scenery);
        Cylinder("CopaCampainha", ponto + Vector3.up * .047f, .081f, .065f, latao, scenery);
        campainhaBalcao = Cylinder("BotaoCampainha", ponto + Vector3.up * .099f, .022f, .045f, latao, scenery).transform;
    }

    internal void ConfigurarSomDoSalao(Transform dono)
    {
        if (ceilingFan != null)
            SantaRitaInteriorAudio.Criar(dono, ceilingFan, camera.transform, "MotorVentilador", 6.5f, .39f);
    }

    internal bool PodeTocarCampainha() => campainhaBalcao != null &&
        EmFoco(root.transform.InverseTransformPoint(campainhaBalcao.position), 2.15f, .982f, out _);

    internal void TocarCampainha()
    {
        if (tempoCampainha < .22f) return;
        tempoCampainha = 0f;
        SantaRitaInteriorAudio.Tocar("CampainhaBalcao", campainhaBalcao.position, camera.transform, .82f);
    }

    private void BuildWindowBay(int index, string name, float x)
    {
        var at = new Vector3(x, JanelaFachadaY, -7.49f);
        var bay = new GameObject(name).transform;
        bay.SetParent(scenery, false);
        bay.localPosition = at;
        janelasFachada[index] = bay;
        var frame=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var molduraExterna = Mat("caixilhoAzulFachada", new Color(.17f,.26f,.28f), tiles[2]);
        float meiaLargura = JanelaFachadaLargura * .5f;
        float meiaAltura = JanelaFachadaAltura * .5f;
        foreach (float lado in new[] { -1f, 1f })
        {
            Box("OmbreiraJanela", new Vector3(lado * (meiaLargura+.025f),0,.035f),
                new Vector3(.10f,1.13f,.17f),frame,bay);
            Box("VergaJanela", new Vector3(0,lado * (meiaAltura+.025f),.035f),
                new Vector3(1.92f,.09f,.17f),frame,bay);
            Box("Caixilho exterior", new Vector3(lado * (meiaLargura+.032f),0,-.083f),
                new Vector3(.12f,1.15f,.065f),molduraExterna,bay);
            Box("Travessa exterior", new Vector3(0,lado * (meiaAltura+.038f),-.083f),
                new Vector3(1.97f,.11f,.065f),molduraExterna,bay);
        }
        Box("PeitorilJanela",new Vector3(0,-meiaAltura-.065f,.045f),new Vector3(1.99f,.075f,.28f),frame,bay);
        foreach (float coluna in new[] { -.46f, 0f, .46f })
            Box("MontanteJanela",new Vector3(coluna,0,.045f),new Vector3(.034f,JanelaFachadaAltura,.072f),frame,bay);
        Box("TravessaJanela",new Vector3(0,.04f,.045f),new Vector3(JanelaFachadaLargura,.037f,.072f),frame,bay);
        RoomFill(name+"_LuzExterior",at+bay.forward*.31f,new Color(.86f,.68f,.48f),.34f,3.3f);
        CombineNet(bay);
        BuildWetWindowPane(index, bay);
        BuildWindowHandprint(index, bay);
    }

    private void BuildWetWindowPane(int index, Transform bay)
    {
        if (vidroJanela == null)
        {
            var shader = Resources.Load<Shader>("Art/Sinuca/JanelaMolhada");
            if (shader == null) return;
            vidroJanela = new Material(shader) { name = "VidroMolhadoDasJanelas" };
            owned.Add(vidroJanela);
        }

        var vidro = Primitive("VidroJanela", PrimitiveType.Quad, new Vector3(0f, 0f, .019f),
            new Vector3(JanelaFachadaLargura, JanelaFachadaAltura, 1f), vidroJanela, bay).GetComponent<MeshRenderer>();
        vidro.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        vidro.shadowCastingMode = ShadowCastingMode.Off;
        vidro.receiveShadows = false;
    }

    private void BuildWindowHandprint(int index, Transform bay)
    {
        var texture = Resources.Load<Texture2D>("Art/Sinuca/MarcaMaoVidro");
        if (texture == null) return;
        if (!materials.TryGetValue("marcaMaoVidro", out var material))
        {
            material = new Material(Resources.Load<Material>("Art/BarRef7/BarPixelCutout")) { name = "marcaMaoVidro" };
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", new Color(.77f, .84f, .87f));
            material.SetFloat("_Cutoff", .40f);
            materials.Add("marcaMaoVidro", material);
            owned.Add(material);
        }

        var marca = Primitive("MarcaDeMaoNoVidro", PrimitiveType.Quad, new Vector3(.27f, .27f, .048f),
            new Vector3(.245f, .36f, 1f), material, bay);
        marca.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        marca.SetActive(false);
        marcasJanela[index] = marca;
    }

    public Vector3 WindowPosition(int index) => janelasFachada[index].position;

    public bool CanInspectWindow(int index)
    {
        if (!Walking) return false;
        var delta = janelasFachada[index].TransformPoint(.27f, .27f, .05f) - camera.transform.position;
        return delta.sqrMagnitude <= 2.55f * 2.55f && Vector3.Dot(delta.normalized, camera.transform.forward) >= .91f;
    }

    public void ShowWindowHandprint(int index, bool visible)
    {
        if (marcasJanela[index] != null) marcasJanela[index].SetActive(visible);
    }

    private Transform ContactPatch(string name,Vector3 at,Vector2 size,float opacity)
    {
        var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=name;g.layer=Layer;
        UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.transform.SetParent(dynamic,false);
        g.transform.localPosition=new Vector3(at.x,.006f,at.z);g.transform.localRotation=Quaternion.Euler(90,0,0);
        g.transform.localScale=new Vector3(size.x,size.y,1);
        string key="contato_"+opacity;
        if(!materials.TryGetValue(key,out var mat))
        {
            mat=new Material(Resources.Load<Shader>("Art/Sinuca/ContatoSala"));mat.SetFloat("_Opacity",opacity);
            materials.Add(key,mat);owned.Add(mat);
        }
        var r=g.GetComponent<MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;
        return g.transform;
    }
    private void TickSalon()
    {
        if (vidroJanela != null)
        {
            // Same stronger storm in the final hand as ChuvaMinigame. The clock lives in the shader.
            float chuva = MenuPausa.ReduzirEfeitosIntensos ? .24f : SinucaMinigameUI.MaoEmCurso >= 3 ? .91f : .66f;
            if (chuva != intensidadeChuvaJanela)
            {
                intensidadeChuvaJanela = chuva;
                vidroJanela.SetFloat("_RainAmount", chuva);
            }
        }
        if (MenuPausa.Aberto || !Application.isFocused) return;
        if (ceilingFan != null) ceilingFan.Rotate(0f, Time.deltaTime * 32f, 0f, Space.Self);
        if (campainhaBalcao != null && tempoCampainha < .22f)
        {
            float antes = Mathf.Sin(Mathf.PI * Mathf.Clamp01(tempoCampainha / .22f));
            tempoCampainha += Time.deltaTime;
            float depois = Mathf.Sin(Mathf.PI * Mathf.Clamp01(tempoCampainha / .22f));
            campainhaBalcao.localPosition += Vector3.down * (.017f * (depois - antes));
        }
        for(int i=0;i<contactActors.Count;i++)
        {
            var p=contactActors[i].localPosition;contactShadows[i].localPosition=new Vector3(p.x,.007f,p.z);
        }
    }
}
