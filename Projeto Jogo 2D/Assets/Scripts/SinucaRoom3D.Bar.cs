using UnityEngine;
using Tipo = InteracaoCapitulo1.TipoInteracao;

public sealed partial class SinucaRoom3D
{
    private const float SoleiraBar = -7.40f;
    private const float JanelaFachadaX = 3.48f;
    private const float JanelaFachadaY = 1.71f;
    private const float JanelaFachadaLargura = 1.76f;
    private const float JanelaFachadaAltura = .96f;
    internal const float DuracaoEntradaBar = 3.10f;
    internal const float SaidaAntesDaNevoa = 1.27f;
    private static readonly Vector3 ChegadaBar = new Vector3(.10f, 1.62f, -5.72f);
    private static readonly Vector2 OlharChegada = new Vector2(4f, 3f);
    private Transform portaBar, macanetaExterior, maoMarcusPorta;
    private Vector3 pontoDaMacanetaFechada, inicioDaSaidaDoBar;
    private Quaternion olharDaSaidaDoBar;
    private Transform macanetaInterior;
    private MeshRenderer chuvaDaEntrada;
    private Material materialChuvaDaEntrada;
    private Vector2 focoDaRua;
    private bool abrindoBar, tocouFecho, tocouPorta;
    internal Vector3 PosicaoExploracao => standingPosition;
    internal Vector2 OlharExploracao => new Vector2(standingYaw, standingPitch);

    private void ConstruirParedeDaEntrada(string nome, float y, float altura, float largura, float z, float profundidade, Material material)
    {
        const float metadeVao = .54f;
        float baseParede = y - altura * .5f;
        float topoParede = y + altura * .5f;
        float baseJanela = Mathf.Max(baseParede, JanelaFachadaY - JanelaFachadaAltura * .5f);
        float topoJanela = Mathf.Min(topoParede, JanelaFachadaY + JanelaFachadaAltura * .5f);

        void Trecho(float x0, float x1, float y0, float y1)
        {
            if (x1 <= x0 || y1 <= y0) return;
            Box(nome, new Vector3((x0 + x1) * .5f, (y0 + y1) * .5f, z),
                new Vector3(x1 - x0, y1 - y0, profundidade), material);
        }

        foreach (float sinal in new[] { -1f, 1f })
        {
            float a = sinal < 0f ? -largura * .5f : metadeVao;
            float b = sinal < 0f ? -metadeVao : largura * .5f;
            if (baseJanela >= topoJanela)
            {
                Trecho(a, b, baseParede, topoParede);
                continue;
            }

            float esquerda = sinal * JanelaFachadaX - JanelaFachadaLargura * .5f;
            float direita = esquerda + JanelaFachadaLargura;
            Trecho(a, esquerda, baseParede, topoParede);
            Trecho(direita, b, baseParede, topoParede);
            Trecho(esquerda, direita, baseParede, baseJanela);
            Trecho(esquerda, direita, topoJanela, topoParede);
        }
        float topo = y + altura / 2f;
        if (topo > 2.14f)
            Box(nome + "Acima", new Vector3(0f, (topo + 2.14f) / 2f, z),
                new Vector3(metadeVao * 2f, topo - 2.14f, profundidade), material);
    }

    private void ConstruirPortaDoBar(Material guarnicao)
    {
        foreach (float x in new[] { -.58f, .58f })
            Box("BatentePorta", new Vector3(x, 1.08f, SoleiraBar), new Vector3(.10f, 2.16f, .18f), guarnicao);
        Box("VergaPorta", new Vector3(0f, 2.16f, SoleiraBar), new Vector3(1.27f, .10f, .18f), guarnicao);
        portaBar = new GameObject("PortaBar_DobradicaEsquerda").transform;
        portaBar.SetParent(scenery, false);
        portaBar.localPosition = new Vector3(-.52f, 0f, SoleiraBar);
        var pintura = Mat("pinturaAzulDaPorta", new Color(.22f, .33f, .36f), tiles[2]);
        Box("FolhaDaPorta", new Vector3(.52f, 1.04f, 0f), new Vector3(1.04f, 2.08f, .085f), pintura, portaBar);
        var latao = Mat("lataoEnvelhecido", new Color(.48f, .34f, .14f));
        for (int lado = -1; lado <= 1; lado += 2)
        {
            float z = lado * .05f;
            foreach (float y in new[] { .31f, .75f, 1.19f, 1.63f })
            {
                Box("MolduraDaAlmofada", new Vector3(.52f, y, z), new Vector3(.81f, .38f, .025f), guarnicao, portaBar);
                Box("AlmofadaDaPorta", new Vector3(.52f, y, z + lado * .016f), new Vector3(.72f, .30f, .025f), lado < 0 ? pintura : Wood, portaBar);
            }
            Box("EspelhoDaMacaneta", new Vector3(.91f, 1.07f, z), new Vector3(.075f, .19f, .024f), latao, portaBar);
            var macaneta = Sphere(lado < 0 ? "MacanetaExteriorDireita" : "MacanetaInterior",
                new Vector3(.91f, 1.09f, lado * .090f), .080f, latao, portaBar).transform;
            if (lado < 0)
            {
                macanetaExterior = macaneta;
                pontoDaMacanetaFechada = root.transform.InverseTransformPoint(macaneta.position);
            }
            else macanetaInterior = macaneta;
        }
        foreach (float y in new[] { .25f, 1.81f })
            Cylinder("Dobradica", new Vector3(-.53f, y, SoleiraBar), .018f, .13f, latao, scenery);
        RoomFill("LuzDaMarquise", new Vector3(.35f, 2.35f, -8.18f), new Color(1f, .78f, .53f), 2.25f, 3.0f);
        ConstruirChuvaDaEntrada();

        var textura = Resources.Load<Texture2D>("Art/Characters/MarcusMaoMacaneta");
        if (textura == null) return;
        var sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height),
            new Vector2(7f / 48f, 18f / 32f), 119f, 0, SpriteMeshType.FullRect);
        sprite.name = "MarcusMaoMacaneta";
        owned.Add(sprite);
        maoMarcusPorta = PixelObject("Marcus_SpritePegadaDaPorta", sprite, Vector3.zero, 1f, false);
        maoMarcusPorta.gameObject.SetActive(false);
    }

    private void ConstruirChuvaDaEntrada()
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/ChuvaEntradaBar");
        if (shader == null || !shader.isSupported) return;
        materialChuvaDaEntrada = new Material(shader) { name = "Gotas da Rua Norte 3D" };
        owned.Add(materialChuvaDaEntrada);

        const int quantidade = 520;
        var vertices = new Vector3[quantidade * 4];
        var cantos = new Vector2[vertices.Length];
        var parametros = new System.Collections.Generic.List<Vector4>(vertices.Length);
        var indices = new int[quantidade * 6];
        var aleatorio = new System.Random(317);
        float Sortear(float min, float max) => Mathf.Lerp(min, max, (float)aleatorio.NextDouble());

        for (int i = 0; i < quantidade; i++)
        {
            // A faixa termina antes da fachada. Paredes e porta ocluem a chuva em profundidade.
            var origem = new Vector3(Sortear(-2.25f, 2.45f), Sortear(0f, 1f), SoleiraBar - Sortear(.20f, 1.90f));
            var gota = new Vector4(Sortear(1.60f, 2.65f), Sortear(.095f, .26f), Sortear(.0028f, .006f), Sortear(.28f, .62f));
            int v = i * 4;
            for (int j = 0; j < 4; j++)
            {
                vertices[v + j] = origem;
                cantos[v + j] = new Vector2(j % 2 == 0 ? -1f : 1f, j / 2);
                parametros.Add(gota);
            }
            int t = i * 6;
            indices[t] = v;
            indices[t + 1] = v + 2;
            indices[t + 2] = v + 1;
            indices[t + 3] = v + 1;
            indices[t + 4] = v + 2;
            indices[t + 5] = v + 3;
        }

        var malha = new Mesh { name = "Volume de chuva da calçada" };
        malha.vertices = vertices;
        malha.uv = cantos;
        malha.SetUVs(1, parametros);
        malha.triangles = indices;
        malha.bounds = new Bounds(new Vector3(0f, 1.9f, SoleiraBar - 1.05f), new Vector3(6f, 3.9f, 1.8f));
        owned.Add(malha);

        var objeto = new GameObject("ChuvaRuaNorte_Entrada3D", typeof(MeshFilter), typeof(MeshRenderer));
        objeto.transform.SetParent(scenery, false);
        objeto.GetComponent<MeshFilter>().sharedMesh = malha;
        chuvaDaEntrada = objeto.GetComponent<MeshRenderer>();
        chuvaDaEntrada.sharedMaterial = materialChuvaDaEntrada;
        chuvaDaEntrada.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        chuvaDaEntrada.receiveShadows = false;
        chuvaDaEntrada.enabled = false;
    }

    internal void IniciarExploracaoDoBar(bool animar, Vector2 foco)
    {
        abrindoBar = animar;
        if (chuvaDaEntrada != null) chuvaDaEntrada.enabled = animar;
        focoDaRua = foco;
        tocouFecho = tocouPorta = false;
        portaBar.localRotation = Quaternion.identity;
        CancelOpponentShot();
        VoltarAExplorar(ChegadaBar, OlharChegada);
        if (animar) AnimarEntradaDoBar(0f);
    }

    internal void VoltarAExplorar(Vector3 posicao, Vector2 olhar)
    {
        if (opponent != null)
        {
            opponent.root.localPosition = Plan(opponent.definition.position);
            opponentHeading = Vector2.down;
        }
        standingPosition = posicao;
        standingYaw = olhar.x;
        standingPitch = olhar.y;
        distanciaPasso = 0f;
        entryLook = 0f;
        mode = SinucaCameraView.Caminhando3D;
        cameraInitialized = false;
        foreach (var bola in balls) bola.gameObject.SetActive(false);
        foreach (var marca in markedSigns) if (marca != null) marca.gameObject.SetActive(false);
        foreach (var item in new[] { cue, bridgeAnchor, gripHand, guide.transform, guideBorder.transform, objectGuide.transform, objectArrow.transform, ghost })
            if (item != null) item.gameObject.SetActive(false);
        AtualizarExploracaoDoBar();
    }

    private static float CurvaDaEntrada(float tempo, float inicio, float fim) =>
        Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inicio, fim, tempo));

    internal float AnimarEntradaDoBar(float segundos)
    {
        if (!abrindoBar) return 1f;
        if (materialChuvaDaEntrada != null) materialChuvaDaEntrada.SetFloat("_Tempo", Time.unscaledTime);
        float abrir = CurvaDaEntrada(segundos, .82f, 1.76f);
        float fechar = CurvaDaEntrada(segundos, 2.60f, DuracaoEntradaBar);
        portaBar.localRotation = Quaternion.Euler(0f, -98f * abrir * (1f - fechar), 0f);
        if (!tocouFecho && segundos >= .80f)
        {
            tocouFecho = true;
            SantaRitaSound.Play("door", .20f);
        }
        if (!tocouPorta && segundos >= 3.02f)
        {
            tocouPorta = true;
            SantaRitaSound.Play("door", .10f, .88f);
        }

        float cruzar = CurvaDaEntrada(segundos, 1.10f, 2.76f);
        var fora = new Vector3(.11f, 1.44f, -8.82f);
        var posicao = Vector3.Lerp(fora, ChegadaBar, cruzar);
        posicao.y += Posto3D.MovimentoDaSoleira(cruzar);
        var deFora = Quaternion.LookRotation(pontoDaMacanetaFechada - fora);
        var deDentro = Quaternion.Euler(OlharChegada.y, OlharChegada.x, 0f);
        camera.orthographic = false;
        camera.fieldOfView = Mathf.Lerp(55f, 62f, cruzar);
        camera.transform.localPosition = posicao;
        camera.transform.localRotation = deFora;
        var raioNoFoco = camera.ViewportPointToRay(focoDaRua).direction;
        var alinharComRua = Quaternion.FromToRotation(raioNoFoco, camera.transform.forward) * deFora;
        var deForaCentrado = Quaternion.Slerp(alinharComRua, deFora, CurvaDaEntrada(segundos, .34f, .94f));
        camera.transform.localRotation = Quaternion.Slerp(deForaCentrado, deDentro, CurvaDaEntrada(segundos, 1.20f, 2.68f));

        AnimarMaoDeMarcus(segundos, macanetaExterior, .42f, .80f, 1.38f, 1.73f);
        SyncFirstPersonCamera();
        OrientPixelArt();
        UpdateOpponentView();
        TickSalon();
        if (segundos >= DuracaoEntradaBar)
        {
            abrindoBar = false;
            if (chuvaDaEntrada != null) chuvaDaEntrada.enabled = false;
            AtualizarExploracaoDoBar();
        }
        return fechar;
    }

    internal Vector2 FocoDaPortaDoBar
    {
        get
        {
            Vector3 ponto = camera.WorldToViewportPoint(macanetaInterior.position);
            return ponto.z > 0f ? new Vector2(ponto.x, ponto.y) : Vector2.one * .5f;
        }
    }

    internal void PrepararSaidaDoBar()
    {
        inicioDaSaidaDoBar = camera.transform.localPosition;
        olharDaSaidaDoBar = camera.transform.localRotation;
        tocouPorta = false;
    }

    internal float AnimarSaidaDoBar(float tempo)
    {
        const float pegar = .44f;
        float abertura = CurvaDaEntrada(tempo, pegar, 1.15f);
        portaBar.localRotation = Quaternion.Euler(0f, -98f * abertura, 0f);
        if (!tocouPorta && tempo >= pegar)
        {
            tocouPorta = true;
            SantaRitaSound.Play("door", .18f);
        }
        // Keep the moving handle in frame while Marcus pushes the leaf clear of his path.
        float passo = CurvaDaEntrada(tempo, .96f, 1.56f);
        var soleira = new Vector3(.23f, 1.62f, SoleiraBar - .16f);
        var recuo = inicioDaSaidaDoBar;
        recuo.z = Mathf.Max(recuo.z, SoleiraBar + .86f);
        var posicao = Vector3.Lerp(Vector3.Lerp(inicioDaSaidaDoBar, recuo, CurvaDaEntrada(tempo, 0f, pegar)), soleira, passo);
        posicao.y += Posto3D.MovimentoDaSoleira(passo);
        camera.transform.localPosition = posicao;
        var alvo = macanetaInterior.position - camera.transform.position;
        var olhandoMao = Quaternion.Slerp(olharDaSaidaDoBar, Quaternion.LookRotation(alvo),
            CurvaDaEntrada(tempo, 0f, pegar));
        camera.transform.localRotation = Quaternion.Slerp(olhandoMao, Quaternion.Euler(5f, 180f, 0f),
            CurvaDaEntrada(tempo, .76f, 1.48f));

        AnimarMaoDeMarcus(tempo, macanetaInterior, .06f, pegar, .86f, 1.23f);
        if (chuvaDaEntrada != null) chuvaDaEntrada.enabled = abertura > 0f;
        if (materialChuvaDaEntrada != null) materialChuvaDaEntrada.SetFloat("_Tempo", Time.unscaledTime);
        SyncFirstPersonCamera();
        OrientPixelArt();
        return abertura;
    }

    private void AnimarMaoDeMarcus(float segundos, Transform macaneta, float inicio, float pegar, float soltar, float fim)
    {
        if (maoMarcusPorta == null) return;
        bool visivel = segundos > inicio && segundos < fim;
        maoMarcusPorta.gameObject.SetActive(visivel);
        if (!visivel) return;

        float pegada = CurvaDaEntrada(segundos, inicio, pegar) * (1f - CurvaDaEntrada(segundos, soltar, fim));
        var pulso = camera.ViewportToWorldPoint(new Vector3(.82f, -.16f, .57f));
        var contato = Vector3.Lerp(pulso, macaneta.position, pegada);
        var pontoNaTela = camera.WorldToScreenPoint(contato);
        var pulsoNaTela = camera.WorldToScreenPoint(pulso);
        var eixo = pulsoNaTela - pontoNaTela;
        float angulo = Mathf.Atan2(eixo.y, eixo.x) * Mathf.Rad2Deg;
        maoMarcusPorta.rotation = camera.transform.rotation * Quaternion.AngleAxis(angulo, Vector3.forward);
        PlaceHandDepth(maoMarcusPorta, contato, -.055f);
    }

    internal void AtualizarExploracaoDoBar()
    {
        TickSalon();
        UpdateCamera();
        SyncFirstPersonCamera();
        AtualizarAtoresDoSalao();
    }

    private bool EmFoco(Vector3 ponto, float alcance, float alinhamento, out float precisao)
    {
        var origem = camera.transform.localPosition;
        var delta = ponto - origem;
        precisao = Vector3.Dot(camera.transform.forward, delta.normalized);
        return delta.sqrMagnitude <= alcance * alcance && precisao >= alinhamento;
    }

    internal bool PodeSairDoBar() => EmFoco(new Vector3(0f, 1.30f, SoleiraBar), 2f, .80f, out _);

    internal Tipo? InteracaoEmFoco(out Bounds enquadramento)
    {
        Tipo? melhor = null;
        Bounds foco = default;
        float alinhamento = .90f;
        void Considerar(Tipo tipo, Bounds bounds, float alcance)
        {
            if (!EmFoco(root.transform.InverseTransformPoint(bounds.center), alcance, alinhamento, out float precisao)) return;
            alinhamento = precisao;
            melhor = tipo;
            foco = bounds;
        }

        if (opponent != null)
            Considerar(Tipo.NpcZeBar, new Bounds(opponent.root.position + Vector3.up * 1.35f, Vector3.one * .4f), 2.35f);
        if (freguesDoBar != null)
            Considerar(Tipo.NpcFrequentadorBar, new Bounds(freguesDoBar.position + Vector3.up * 1.06f, Vector3.one * .4f), 2.1f);
        if (fotoDoClube != null)
            Considerar(Tipo.BalcaoBarDoZe, fotoDoClube.bounds, 3.3f);
        if (regrasDaCasa != null)
            Considerar(Tipo.QuadroRegrasSinuca, regrasDaCasa.bounds, 2.15f);
        enquadramento = foco;
        return melhor;
    }

    private Transform freguesDoBar;
}
