using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Tipo = InteracaoCapitulo1.TipoInteracao;

internal sealed partial class Posto3D
{
    private void ConstruirSala()
    {
        var madeira = Resources.Load<Texture2D>("Art/Tiles/Madeira");
        var tinta = Superficie("Reboco de loja", new Color(.85f, .79f, .62f), SantaRitaBarLayout.NativeSurface(true).texture, new Vector2(5f, 1f));
        var forro = Superficie("Forro de madeira clara", new Color(.54f, .52f, .44f), madeira, new Vector2(6f, 5f));
        var rejunte = new Color(.26f, .31f, .28f);
        var ceramica = Superficie("Cerâmica comercial gasta", Color.white,
            CriarCeramica(new Color(.51f, .53f, .45f), rejunte), new Vector2(5f, 5f));
        var azulejo = Superficie("Azulejos creme", Color.white,
            CriarCeramica(new Color(.82f, .78f, .64f), new Color(.51f, .49f, .42f)), new Vector2(6f, 1f));
        var petroleo = Superficie("Esmalte azul petróleo", new Color(.20f, .39f, .40f), madeira);
        var ferragem = Superficie("Bronze escurecido da fachada", new Color(.28f, .20f, .13f), madeira);
        var faixaExterna = Superficie("Esmalte vermelho descascado", new Color(.58f, .21f, .12f), madeira);
        var aluminio = Superficie("Alumínio fosco", new Color(.56f, .58f, .57f));
        var escuro = Superficie("Borracha e ferragem", new Color(.16f, .19f, .17f));
        var latao = Superficie("Latão riscado", new Color(.56f, .40f, .20f));
        var madeiraLoja = Superficie("Madeira de balcão", new Color(.63f, .54f, .41f), madeira);
        var branco = Superficie("Chapa creme", new Color(.76f, .73f, .65f));
        var luz = Superficie("Tubos fluorescentes", new Color(.93f, .99f, .88f), unlit: true);

        Caixa("Piso contínuo", V(0, -.065f, 0), V(10.1f, .13f, 9.1f), ceramica);
        Caixa("Forro", V(0, 2.99f, 0), V(10.1f, .11f, 9.1f), forro);
        Caixa("Fundo", V(0, 1.5f, 4.5f), V(10.1f, 3f, .14f), tinta);
        Caixa("Azulejo do fundo", V(0, .52f, 4.405f), V(10f, 1.04f, .06f), azulejo, solida: false);
        Caixa("Faixa do fundo", V(0, 1.07f, 4.36f), V(10f, .065f, .045f), petroleo, solida: false);
        foreach (float lado in new[] { -1f, 1f })
        {
            Caixa("Parede lateral", V(lado * 5f, 1.5f, 0), V(.14f, 3f, 9f), tinta);
            Caixa("Azulejo lateral", V(lado * 4.90f, .52f, 0), V(.065f, 1.04f, 8.94f), azulejo, solida: false);
            Caixa("Faixa lateral", V(lado * 4.86f, 1.07f, 0), V(.045f, .065f, 8.94f), petroleo, solida: false);
            Caixa("Fachada", V(lado * 2.92f, 1.5f, Soleira), V(4.16f, 3f, .16f), tinta);
            Caixa("Faixa da fachada", V(lado * 2.92f, .36f, Soleira - .10f), V(4.14f, .72f, .055f), faixaExterna, solida: false);
            Caixa("Batente", V(lado * .81f, 1.18f, Soleira), V(.10f, 2.35f, .23f), ferragem);
        }
        Caixa("Verga da porta", V(0, 2.68f, Soleira), V(1.65f, .64f, .18f), tinta);
        Caixa("Guarnição superior", V(0, 2.09f, Soleira), V(1.70f, .09f, .23f), ferragem);
        Caixa("Soleira", V(0, -.015f, Soleira), V(1.59f, .04f, .38f), aluminio, solida: false);
        Caixa("Pátio sob a marquise", V(0, -.07f, -6f), V(10f, .13f, 3.05f), ceramica);
        Caixa("Marquise", V(0, 2.91f, -5.54f), V(10.1f, .18f, 2.12f), forro);
        Caixa("Capacho de borracha", V(0, .014f, -3.8f), V(1.43f, .028f, .77f), escuro, solida: false);
        ConstruirPorta(ferragem, madeiraLoja, latao);
        ConstruirChuva();

        var shaderVidro = Resources.Load<Shader>("Art/Sinuca/JanelaMolhada");
        var atlasVidro = Resources.Load<Texture2D>("Art/Sinuca/JanelasLateraisExterior");
        if (shaderVidro != null && atlasVidro != null)
        {
            var vidro = new Material(shaderVidro) { mainTexture = atlasVidro };
            recursos.Add(vidro);
            foreach (float x in new[] { -2.60f, 2.60f })
            {
                Caixa("Caixilho da vitrine", V(x, 1.63f, -4.385f), V(2.35f, 1.38f, .08f), aluminio, solida: false);
                var janela = Quadro("Vitrine molhada", V(x, 1.63f, -4.33f), V(2.20f, 1.22f, 1f), vidro);
                janela.Rotate(0f, 180f, 0f);
                Caixa("Travessa da vitrine", V(x, 1.63f, -4.295f), V(2.24f, .035f, .035f), escuro, solida: false);
                Caixa("Montante da vitrine", V(x, 1.63f, -4.295f), V(.035f, 1.24f, .035f), escuro, solida: false);
            }
        }

        foreach (float z in new[] { -2.25f, .8f, 3.2f })
        {
            Caixa("Carcaça fluorescente", V(0, 2.84f, z), V(1.52f, .12f, .28f), escuro, solida: false);
            foreach (float dz in new[] { -.08f, .08f })
                Caixa("Tubo fluorescente", V(0, 2.765f, z + dz), V(1.43f, .042f, .05f), luz, solida: false);
            Luz("Luz fria do teto", V(0, 2.66f, z), new Color(.82f, .95f, .87f), 4.4f, 6.25f);
        }
        Luz("Luz quente do caixa", V(-1.7f, 2.45f, 2.65f), new Color(1f, .75f, .46f), 2.4f, 4.6f);
        Luz("Luz fria do vidro", V(-3.2f, 2.03f, -3.8f), new Color(.54f, .75f, 1f), 1.4f, 3.5f);
        Luz("Luz da marquise", V(.35f, 2.45f, -5.6f), new Color(.83f, .89f, .72f), 2.7f, 4f);
        var ambiente = new GameObject("Luz rebatida do posto").AddComponent<Light>();
        ambiente.transform.SetParent(cenario, false);
        ambiente.transform.localRotation = Quaternion.Euler(45f, -37f, 0f);
        ambiente.type = LightType.Directional;
        ambiente.cullingMask = RoomMask;
        ambiente.color = new Color(.57f, .67f, .80f);
        ambiente.intensity = .19f;

        Caixa("Balcão", V(-.55f, .51f, 2.25f), V(3.68f, 1.02f, .83f), petroleo);
        Caixa("Tampo de fórmica", V(-.55f, 1.055f, 2.25f), V(3.79f, .075f, .92f), madeiraLoja);
        Caixa("Rodapé do caixa", V(-.55f, .08f, 1.82f), V(3.66f, .16f, .07f), escuro, solida: false);
        for (int i = 0; i < 3; i++)
            Caixa("Moldura da frente do balcão", V(-1.73f + i * 1.17f, .55f, 1.81f), V(.95f, .54f, .035f), madeiraLoja, solida: false);
        Caixa("Registradora", V(-.63f, 1.18f, 2.36f), V(.54f, .17f, .42f), escuro);
        Caixa("Teclado", V(-.63f, 1.28f, 2.29f), V(.39f, .05f, .27f), branco, solida: false);
        Caixa("Caderno de plantão", V(-1.79f, 1.118f, 2.04f), V(.51f, .045f, .32f), branco, tipo: Tipo.LivroRegistroPosto);
        for (int i = 0; i < 5; i++)
            Caixa("Linhas de plantão", V(-1.79f, 1.142f, 1.95f + i * .04f), V(.36f, .003f, .006f), aluminio, solida: false);
        Caixa("Televisor de tubo", V(.80f, 1.31f, 2.24f), V(.61f, .45f, .49f), escuro, tipo: Tipo.TvPortatilPosto);
        var tela = Superficie("TV com recepção fraca", new Color(.35f, .47f, .54f), unlit: true);
        Caixa("Vidro do televisor", V(.73f, 1.34f, 1.992f), V(.43f, .31f, .018f), tela, solida: false);
        for (int i = 0; i < 13; i++)
            Caixa("Entrelaçamento da TV", V(.73f, 1.202f + .021f * i, 1.98f), V(.43f, .004f, .006f), aluminio, solida: false);
        Caixa("Antena", V(.77f, 1.68f, 2.28f), V(.014f, .39f, .014f), aluminio, solida: false).transform.Rotate(0f, 0f, 25f);

        Caixa("Geladeira", V(-3.85f, 1.07f, 3.59f), V(1.13f, 2.14f, 1.14f), branco);
        Arte("Porta com bebidas", "Art/Props/Geladeira", V(-3.85f, 1.12f, 3.011f), 2f);
        Caixa("Puxador da geladeira", V(-3.39f, 1.13f, 2.95f), V(.035f, .31f, .065f), aluminio, solida: false);
        Prateleira(V(3.90f, 0f, 2.85f), petroleo, madeiraLoja, branco, aluminio);
        Prateleira(V(3.90f, 0f, .42f), petroleo, madeiraLoja, branco, aluminio);
        Caixa("Apoio para café", V(-4.13f, .48f, -.36f), V(1.12f, .96f, 1.27f), madeiraLoja);
        Arte("Café em garrafa térmica", "Art/Props/Cafe", V(-4.08f, 1.26f, -.43f), .70f);
        Arte("Chimarrão do plantão", "Art/Props/Chimarrao", V(-3.86f, 1.15f, -.78f), .42f);
        Arte("Engradado", "Cap1/BottlesCrate", V(-4.13f, .28f, -1.38f), .56f);
        Arte("Caixas de estoque", "Cap1/CardboardBoxes_Stack", V(4.22f, .55f, -1.85f), 1.1f);
        Arte("Relógio do plantão", "Cap1/WallClock", V(-.46f, 2.28f, 4.39f), .40f);
        MontarVentilador();
        Letreiro("Placa de serviço", "CONVENIÊNCIA\nCAFÉ  •  QUEROSENE", V(-2.1f, 2.13f, 4.38f), madeiraLoja, 1.42f, .53f);
        Letreiro("Placa de saída", "SAÍDA", V(0, 2.44f, -4.375f), petroleo, .86f, .25f, 180f);
        Letreiro("Placa de café", "CAFÉ", V(-4.89f, 1.87f, -.39f), madeiraLoja, .69f, .28f, -90f);

        Personagem("Frentista", "Art/Characters/Npc_Frentista", V(-1.23f, 0f, 3.42f), Tipo.NpcFrentista);
        Personagem("Motorista abrigado", "Art/Characters/Npc_Motorista", V(-3.72f, 0f, -2.72f), Tipo.NpcMotoristaPosto);
    }

    private void ConstruirPorta(Material moldura, Material madeira, Material latao)
    {
        var vidro = Superficie("Vidro âmbar da porta", new Color(.66f, .54f, .31f, .15f), unlit: true);
        vidro.SetOverrideTag("RenderType", "Transparent");
        vidro.SetFloat("_Surface", 1f);
        vidro.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        vidro.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        vidro.SetInt("_ZWrite", 0);
        vidro.SetInt("_Cull", 0);
        vidro.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        vidro.renderQueue = (int)RenderQueue.Transparent;

        var folhaFixa = new GameObject("Folha fixa de vidro").transform;
        folhaFixa.SetParent(cenario, false);
        folhaFixa.localPosition = V(-.78f, 0f, Soleira);
        MontarFolha(folhaFixa, 1f, moldura, madeira, vidro, latao, out _);

        porta = new GameObject("Dobradiça direita da porta").transform;
        porta.SetParent(cenario, false);
        porta.localPosition = V(.78f, 0f, Soleira);
        saida = MontarFolha(porta, -1f, moldura, madeira, vidro, latao, out macaneta);
        MontarSino(latao);

        Quadro("Bandeira de vidro", V(0, 2.235f, Soleira), V(1.52f, .22f, 1f), vidro);
        Caixa("Travessa da bandeira", V(0, 2.37f, Soleira), V(1.69f, .058f, .13f), moldura, solida: false);
        for (int coluna = -1; coluna <= 1; coluna++)
            Caixa("Divisão da bandeira", V(coluna * .38f, 2.235f, Soleira), V(.035f, .23f, .078f), moldura, solida: false);

        var sprite = Resources.Load<Texture2D>("Art/Characters/MarcusMaoMacaneta");
        if (sprite == null) return;
        mao = Quadro("Mão de Marcus", Vector3.zero, V(.38f, .38f * sprite.height / sprite.width, 1f),
            MaterialRecorte("Mão de Marcus", sprite));
        mao.gameObject.SetActive(false);
    }

    private Collider MontarFolha(Transform folha, float sinal, Material moldura, Material madeira, Material vidro,
        Material latao, out Transform puxador)
    {
        var limite = new GameObject("Limite da folha").AddComponent<BoxCollider>();
        limite.gameObject.layer = Layer;
        limite.transform.SetParent(folha, false);
        limite.center = V(sinal * .385f, 1.045f, 0);
        limite.size = V(.77f, 2.09f, .075f);

        foreach (float x in new[] { .026f, .744f })
            Caixa("Montante de bronze", V(sinal * x, 1.045f, 0), V(.052f, 2.09f, .075f), moldura, folha, false);
        foreach (float y in new[] { .05f, .73f, 2.055f })
            Caixa("Travessa da folha", V(sinal * .385f, y, 0), V(.77f, .069f, .075f), moldura, folha, false);
        Caixa("Almofada de madeira", V(sinal * .385f, .385f, 0), V(.66f, .60f, .042f), madeira, folha, false);
        Caixa("Friso da almofada", V(sinal * .385f, .385f, -.03f), V(.57f, .51f, .021f), moldura, folha, false);

        var quad = Geometria(PrimitiveType.Quad, "Painel de vidro", V(sinal * .385f, 1.39f, 0),
            V(.666f, 1.26f, 1f), vidro, folha);
        quad.GetComponent<Collider>().enabled = false;
        quad.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        foreach (float x in new[] { .27f, .50f })
            Caixa("Pinázio vertical", V(sinal * x, 1.39f, 0), V(.025f, 1.29f, .034f), moldura, folha, false);
        foreach (float y in new[] { 1.08f, 1.40f, 1.72f })
            Caixa("Pinázio horizontal", V(sinal * .385f, y, 0), V(.70f, .024f, .034f), moldura, folha, false);
        puxador = Caixa("Maçaneta exterior", V(sinal * .691f, 1.03f, -.070f), V(.035f, .17f, .05f), latao, folha, false).transform;
        Caixa("Maçaneta interior", V(sinal * .691f, 1.03f, .070f), V(.035f, .17f, .05f), latao, folha, false);
        return limite;
    }
    private void Prateleira(Vector3 pos, Material metal, Material madeira, Material claro, Material cinza)
    {
        var vermelha = Superficie("Embalagem vermelha", new Color(.66f, .22f, .13f));
        var verde = Superficie("Embalagem verde", new Color(.32f, .55f, .31f));
        var laranja = Superficie("Embalagem laranja", new Color(.82f, .51f, .20f));
        Material[] produtos = { vermelha, claro, verde, laranja };
        Caixa("Fundo de gôndola", pos + V(.31f, .96f, 0), V(.15f, 1.91f, 1.96f), metal);
        for (int andar = 0; andar < 4; andar++)
        {
            float y = .22f + andar * .43f;
            Caixa("Prateleira", pos + V(0, y, 0), V(.67f, .072f, 1.95f), madeira);
            Caixa("Trilho dos preços", pos + V(-.35f, y, 0), V(.04f, .073f, 1.95f), cinza, solida: false);
            for (int produto = 0; produto < 8; produto++)
            {
                if ((produto + andar * 2) % 9 == 0) continue;
                float z = -.80f + produto * .223f;
                float h = .13f + (produto * 7 + andar * 3) % 4 * .048f;
                Caixa("Embalagem", pos + V(-.11f, y + .036f + h * .5f, z), V(.24f, h, .16f),
                    produtos[(produto + andar) % produtos.Length], solida: false);
                Caixa("Rótulo de embalagem", pos + V(-.236f, y + .038f + h * .59f, z), V(.013f, h * .39f, .13f), claro, solida: false);
                Caixa("Etiqueta de preço", pos + V(-.376f, y, z), V(.01f, .044f, .087f), claro, solida: false);
            }
        }
    }

    private void ConstruirChuva()
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/ChuvaEntradaBar");
        if (shader == null || !shader.isSupported) return;
        materialChuva = new Material(shader);
        recursos.Add(materialChuva);
        const int quantidade = 430;
        var vertices = new Vector3[quantidade * 4];
        var cantos = new Vector2[vertices.Length];
        var parametros = new List<Vector4>(vertices.Length);
        var indices = new int[quantidade * 6];
        var random = new System.Random(317);
        float Sortear(float min, float max) => Mathf.Lerp(min, max, (float)random.NextDouble());
        for (int i = 0; i < quantidade; i++)
        {
            var origem = V(Sortear(-2.20f, 2.43f), Sortear(0f, 1f), Soleira - Sortear(.35f, 2.05f));
            var gota = new Vector4(Sortear(1.60f, 2.65f), Sortear(.095f, .26f), Sortear(.003f, .006f), Sortear(.30f, .65f));
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
        var malha = new Mesh { name = "Chuva da marquise do posto" };
        malha.vertices = vertices;
        malha.uv = cantos;
        malha.SetUVs(1, parametros);
        malha.triangles = indices;
        malha.bounds = new Bounds(V(0f, 1.9f, Soleira - 1.18f), V(5f, 3.9f, 2.4f));
        recursos.Add(malha);

        var objeto = new GameObject("Chuva da avenida", typeof(MeshFilter), typeof(MeshRenderer));
        objeto.layer = Layer;
        objeto.transform.SetParent(cenario, false);
        objeto.GetComponent<MeshFilter>().sharedMesh = malha;
        chuva3D = objeto.GetComponent<MeshRenderer>();
        chuva3D.sharedMaterial = materialChuva;
        chuva3D.shadowCastingMode = ShadowCastingMode.Off;
        chuva3D.receiveShadows = false;
    }

    private static Vector3 V(float x, float y, float z) => new Vector3(x, y, z);

    private Texture2D CriarCeramica(Color cor, Color rejunte)
    {
        const int tamanho = 128, bloco = 32;
        var textura = new Texture2D(tamanho, tamanho, TextureFormat.RGBA32, true)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat
        };
        var pixels = new Color[tamanho * tamanho];
        for (int y = 0; y < tamanho; y++)
        for (int x = 0; x < tamanho; x++)
        {
            float desbotado = ((x * 31 + y * 57 + x * y * 13) % 23) * .005f;
            float alternar = ((x / bloco + y / bloco) & 1) == 1 ? .12f : 0f;
            pixels[x + y * tamanho] = x % bloco == 0 || y % bloco == 0 ? rejunte
                : cor * (1f - desbotado - alternar - (x % bloco == 1 || y % bloco == 1 ? .10f : 0f));
        }
        textura.SetPixels(pixels);
        textura.Apply();
        recursos.Add(textura);
        return textura;
    }

    private Material Superficie(string nome, Color cor, Texture textura = null, Vector2 repeticao = default, bool unlit = false)
    {
        var material = new Material(Resources.Load<Material>("Art/BarRef7/" + (unlit ? "BarUnlit" : "BarLit"))) { name = nome };
        if (!unlit) material.shader = Resources.Load<Shader>("Art/Sinuca/SalaLit");
        material.SetColor("_BaseColor", cor);
        if (textura != null)
        {
            material.SetTexture("_BaseMap", textura);
            material.SetTextureScale("_BaseMap", repeticao == default ? Vector2.one : repeticao);
            if (!unlit) material.SetFloat("_RoomSurface", .65f);
        }
        recursos.Add(material);
        return material;
    }

    private Material MaterialRecorte(string nome, Texture textura)
    {
        var material = Superficie(nome, Color.white, textura);
        material.SetFloat("_RoomSurface", 0f);
        material.SetFloat("_AlphaClip", 1f);
        material.SetFloat("_Cull", 0f);
        material.SetFloat("_ActorLighting", 1f);
        material.SetFloat("_Smoothness", .02f);
        return material;
    }

    private GameObject Caixa(string nome, Vector3 pos, Vector3 tamanho, Material material,
        Transform pai = null, bool solida = true, Tipo? tipo = null)
    {
        var objeto = Geometria(PrimitiveType.Cube, nome, pos, tamanho, material, pai ?? cenario);
        var collider = objeto.GetComponent<Collider>();
        collider.enabled = solida;
        if (tipo.HasValue) alvos.Add(collider, tipo.Value);
        return objeto;
    }

    private Transform Quadro(string nome, Vector3 pos, Vector3 tamanho, Material material)
    {
        var objeto = Geometria(PrimitiveType.Quad, nome, pos, tamanho, material, cenario);
        objeto.GetComponent<Collider>().enabled = false;
        objeto.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        return objeto.transform;
    }

    private GameObject Geometria(PrimitiveType tipo, string nome, Vector3 pos, Vector3 tamanho, Material material, Transform pai)
    {
        var objeto = GameObject.CreatePrimitive(tipo);
        objeto.name = nome;
        objeto.layer = Layer;
        objeto.transform.SetParent(pai, false);
        objeto.transform.localPosition = pos;
        objeto.transform.localScale = tamanho;
        objeto.GetComponent<MeshRenderer>().sharedMaterial = material;
        return objeto;
    }

    private Transform Arte(string nome, string caminho, Vector3 pos, float altura)
    {
        var sprite = Resources.Load<Sprite>(caminho);
        var textura = sprite != null ? sprite.texture : Resources.Load<Texture2D>(caminho);
        return textura == null ? null : Quadro(nome, pos, V(altura * textura.width / textura.height, altura, 1f), MaterialRecorte(nome, textura));
    }

    private void Personagem(string nome, string caminho, Vector3 pe, Tipo tipo)
    {
        var sprite = Resources.Load<Sprite>(caminho);
        if (sprite != null)
        {
            var textura = sprite.texture;
            RectInt limites = RecorteDaFigura(textura);
            const float altura = 1.79f;
            var material = MaterialRecorte(nome, textura);
            material.SetTextureOffset("_BaseMap", new Vector2(limites.x / (float)textura.width, limites.y / (float)textura.height));
            material.SetTextureScale("_BaseMap", new Vector2(limites.width / (float)textura.width, limites.height / (float)textura.height));
            var figura = Quadro(nome, pe + V(0f, altura * .5f, 0f),
                V(altura * limites.width / limites.height, altura, 1f), material);
            personagens.Add(figura);
        }
        var alcance = new GameObject("Interação: " + nome).AddComponent<BoxCollider>();
        alcance.gameObject.layer = Layer;
        alcance.transform.SetParent(cenario, false);
        alcance.transform.localPosition = pe + V(0, .91f, 0);
        alcance.size = V(.61f, 1.82f, .32f);
        alvos.Add(alcance, tipo);
    }

    private static RectInt RecorteDaFigura(Texture2D textura)
    {
        if (!textura.isReadable) return new RectInt(0, 0, textura.width, textura.height);
        var pixels = textura.GetPixels32();
        int esquerda = textura.width, fundo = textura.height, direita = -1, topo = -1;
        for (int y = 0; y < textura.height; y++)
        for (int x = 0; x < textura.width; x++)
        {
            if (pixels[y * textura.width + x].a < 128) continue;
            esquerda = Mathf.Min(esquerda, x);
            fundo = Mathf.Min(fundo, y);
            direita = Mathf.Max(direita, x);
            topo = Mathf.Max(topo, y);
        }
        return direita >= esquerda
            ? new RectInt(esquerda, fundo, direita - esquerda + 1, topo - fundo + 1)
            : new RectInt(0, 0, textura.width, textura.height);
    }
    private Light Luz(string nome, Vector3 pos, Color cor, float intensidade, float alcance)
    {
        var luz = new GameObject(nome).AddComponent<Light>();
        luz.transform.SetParent(cenario, false);
        luz.transform.localPosition = pos;
        luz.type = LightType.Point;
        luz.color = cor;
        luz.intensity = intensidade;
        luz.range = alcance;
        luz.cullingMask = RoomMask;
        return luz;
    }

    private void Letreiro(string nome, string mensagem, Vector3 pos, Material material, float largura, float altura, float yaw = 0f)
    {
        var placa = Caixa(nome, pos, V(largura, altura, .043f), material, solida: false);
        placa.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        var texto = new GameObject("Letras").AddComponent<TextMeshPro>();
        texto.gameObject.layer = Layer;
        texto.transform.SetParent(cenario, false);
        texto.transform.localPosition = pos + placa.transform.rotation * V(0, 0, -.029f);
        texto.transform.localRotation = placa.transform.localRotation;
        texto.rectTransform.sizeDelta = new Vector2(largura, altura);
        texto.text = mensagem;
        texto.fontSize = 1.15f;
        texto.fontStyle = FontStyles.Bold;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(.97f, .87f, .65f);
    }
}
