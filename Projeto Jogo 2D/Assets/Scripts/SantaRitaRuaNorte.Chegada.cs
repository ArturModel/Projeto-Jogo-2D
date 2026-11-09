using System.Collections.Generic;
using UnityEngine;

public sealed partial class SantaRitaRuaNorte
{
    private const string ModulosDaRodovia = Art + "Entrada/Modulos/";
    private const string RelevosDaRodovia = Art + "Entrada/Relevos/";
    private readonly List<Sprite> spritesDaFiacao = new();

    private void ComporChegada(SpriteRenderer carro)
    {
        lighting.SetFloat("_Arrival", 1);
        ComporMargemNorte();
        ComporMargemSul();
        ComporPavimento();
        ConfigurarReflexoDoCarro(carro);
    }

    private void ComporMargemNorte()
    {
        // O muro acompanha o recuo do carro e termina antes da área de embarque.
        var muros = new[]
        {
            new Vector2(102.15f, 14.18f), new Vector2(104.53f, 14.18f),
            new Vector2(107.20f, 15.20f), new Vector2(109.58f, 15.20f), new Vector2(111.96f, 15.20f),
            new Vector2(114.32f, 14.36f), new Vector2(116.70f, 14.36f),
            new Vector2(123.25f, 14.41f), new Vector2(125.63f, 14.41f)
        };
        for (int i = 0; i < muros.Length; i++)
        {
            var muro = ModuloDaRodovia("Muro de arrimo", "Muro", muros[i], flip: i % 2 != 0,
                volume: new Vector3(2.42f, .36f, .92f));
            if (muro == null) continue;
            LimiteDaRodovia(muro.transform, new Vector2(0, .24f), new Vector2(2.42f, .30f));
            if (i == 3 || i == 7)
                FixarDetalheDaRodovia(muro, "Saída de drenagem", "DrenoDeMuro", new Vector2(.62f, .08f));
        }
        var pilares = new[]
        {
            new Vector2(101.02f, 14.12f), new Vector2(105.71f, 14.12f),
            new Vector2(106.03f, 15.13f), new Vector2(113.17f, 15.13f),
            new Vector2(117.79f, 14.29f), new Vector2(122.12f, 14.34f), new Vector2(126.72f, 14.34f)
        };
        for (int i = 0; i < pilares.Length; i++)
        {
            var pilar = ModuloDaRodovia("Arremate do muro", "Pilar", pilares[i], flip: i % 2 == 0,
                volume: new Vector3(.45f, .38f, 1.02f));
            if (pilar != null) LimiteDaRodovia(pilar.transform, new Vector2(0, .21f), new Vector2(.45f, .37f));
        }

        var abrigo = ModuloDaRodovia("Abrigo da parada", "Abrigo", new Vector2(119.58f, 13.98f));
        if (abrigo != null)
        {
            LimiteDaRodovia(abrigo.transform, new Vector2(0, .64f), new Vector2(3.15f, .23f));
            LimiteDaRodovia(abrigo.transform, new Vector2(-1.50f, .15f), new Vector2(.25f, .35f));
            LimiteDaRodovia(abrigo.transform, new Vector2(1.50f, .15f), new Vector2(.25f, .35f));
            Vector2 pe = abrigo.transform.position;
            RegistrarVolumeDaRodovia(pe + new Vector2(0, .61f), new Vector3(3.15f, .25f, 1.44f));
            RegistrarVolumeDaRodovia(pe + new Vector2(-1.48f, .10f), new Vector3(.27f, .33f, 2.29f));
            RegistrarVolumeDaRodovia(pe + new Vector2(1.48f, .10f), new Vector3(.27f, .33f, 2.29f));
            int telhado = RegistrarVolumeDaRodovia(pe, new Vector3(3.44f, 1.01f, 2.48f), 2.08f);
            SetReceiver(abrigo, pe.y, .88f, telhado);
            FixarDetalheDaRodovia(abrigo, "Cano de chuva do abrigo", "CanoDeChuva", new Vector2(-1.52f, .07f));
            FixarDetalheDaRodovia(abrigo, "Caixa elétrica do abrigo", "CaixaEletrica", new Vector2(1.50f, .68f));
        }
        var lixeira = transform.Find("Lixeira_Rodovia").GetComponent<SpriteRenderer>();
        var spriteLixeira = Resources.Load<Sprite>(ModulosDaRodovia + "Lixeira");
        if (spriteLixeira != null)
        {
            lixeira.sprite = spriteLixeira;
            AplicarRelevoDaRodovia(lixeira);
        }

        var balizadores = ModuloDaRodovia("Balizadores do orelhão", "Balizadores", new Vector2(127.72f, 13.97f),
            volume: new Vector3(1.13f, .28f, .67f));
        if (balizadores != null)
        {
            LimiteDaRodovia(balizadores.transform, new Vector2(-.43f, .11f), new Vector2(.34f, .27f));
            LimiteDaRodovia(balizadores.transform, new Vector2(.43f, .11f), new Vector2(.34f, .27f));
        }
        MontarDefensas(new[] { 130.55f, 133.13f }, 14.23f);

        Plant("Bosque da curva", "Bracatinga", 133.38f, 16.80f, 1.07f);
        Plant("Copa junto à parada", "Pitangueira", 121.93f, 15.31f, .95f);
        Plant("Copa atrás do recuo", "Bracatinga", 109.36f, 16.29f, 1.06f);
        EspalharVegetacao("Sebe", new (float x, float y, float escala)[]
        {
            (101.3f, 14.92f, 1.05f), (105.55f, 15.43f, 1.15f), (107.53f, 15.81f, 1.04f),
            (111.65f, 15.65f, 1.06f), (114.75f, 14.99f, 1.10f), (117.33f, 15.0f, .83f),
            (121.83f, 14.94f, .92f), (124.25f, 15.17f, 1.05f), (127.31f, 14.78f, .95f),
            (129.18f, 14.85f, 1.09f), (132.34f, 15.02f, 1.06f), (135.10f, 14.77f, 1.13f)
        });
        EspalharVegetacao("Samambaia", new (float x, float y, float escala)[]
        {
            (100.85f, 13.98f, .81f), (104.11f, 14.12f, .73f), (105.56f, 14.99f, 1.07f),
            (110.57f, 15.08f, .88f), (113.1f, 14.18f, .71f), (115.31f, 14.29f, .72f),
            (117.90f, 14.42f, .89f), (121.41f, 14.35f, .80f), (122.77f, 14.30f, .74f),
            (126.7f, 14.24f, .75f), (129.41f, 14.37f, 1.03f), (134.21f, 14.27f, .85f)
        });
        EspalharVegetacao("HeraDoMuro", new (float x, float y, float escala)[]
        {
            (108.03f, 15.15f, 1f), (117.39f, 14.30f, .86f), (125.05f, 14.35f, 1f)
        });
    }

    private void ComporMargemSul()
    {
        // A proteção é interrompida nos acessos da margem, inclusive perto da mala.
        int defensas = MontarDefensas(new[] { 102.64f, 105.22f, 111.78f, 114.36f, 117.08f, 127.73f, 130.31f, 132.89f }, 5.44f);
        if (defensas > 0)
            for (int x = 104; x <= 130; x += 6)
                transform.Find("CercaSul_" + x).gameObject.SetActive(false);

        var postes = new[]
        {
            new Vector2(102.1f, 5.02f), new Vector2(111.28f, 5.25f),
            new Vector2(121.11f, 5.02f), new Vector2(130.46f, 5.20f), new Vector2(135.02f, 5.18f)
        };
        SpriteRenderer anterior = null;
        foreach (var pe in postes)
        {
            var poste = ModuloDaRodovia("Poste da rede", "PosteRede", pe, tinta: new Color(.72f, .75f, .79f),
                volume: new Vector3(.19f, .20f, 2.83f));
            if (poste == null) break;
            LimiteDaRodovia(poste.transform, new Vector2(0, .11f), new Vector2(.23f, .26f));
            if (anterior != null) CriarVaoDeFiacao(anterior, poste);
            anterior = poste;
        }

        EspalharVegetacao("Samambaia", new (float x, float y, float escala)[]
        {
            (101.34f, 5.39f, .91f), (104.01f, 5.33f, .74f), (107.25f, 5.13f, 1.18f),
            (109.61f, 5.40f, .93f), (113.22f, 5.37f, .76f), (116.06f, 5.41f, .75f),
            (118.63f, 5.39f, .97f), (120.33f, 5.16f, 1.11f), (124.61f, 5.33f, 1.15f),
            (127.19f, 5.38f, .74f), (130.79f, 5.39f, .78f), (134.38f, 5.29f, 1.02f)
        });
        EspalharVegetacao("Sebe", new (float x, float y, float escala)[]
        {
            (103.06f, 4.85f, 1.1f), (108.10f, 4.79f, .91f), (115.01f, 4.88f, 1.12f),
            (119.20f, 4.66f, .96f), (125.70f, 4.81f, 1.15f), (133.41f, 4.84f, 1.16f)
        });
        Plant("Copa sobre a vala oeste", "Pitangueira", 101.4f, 3.23f, 1.03f);
        Plant("Copa sobre a vala leste", "Bracatinga", 134.78f, 3.07f, 1.08f);
    }

    private void ComporPavimento()
    {
        ModuloDaRodovia("Limitador no recuo do carro", "LimitadorDaVaga", new Vector2(110.93f, 13.93f),
            volume: new Vector3(.68f, .24f, .24f), tinta: new Color(.84f, .84f, .79f));
        ModuloDaRodovia("Papel junto ao banco", "PapelMolhado", new Vector2(120.61f, 13.03f),
            plano: true, tinta: new Color(.78f, .77f, .72f, .72f));

        foreach (float x in new[] { 103.2f, 115.72f, 121.04f, 129.41f, 134.19f })
            ModuloDaRodovia("Boca de lobo junto ao meio-fio", "Grelha", new Vector2(x, 12.55f), plano: true);
        foreach (float x in new[] { 105.93f, 113.06f, 119.32f, 127.46f, 133.27f })
            ModuloDaRodovia("Dreno do passeio sul", "Grelha", new Vector2(x, 7.20f), .86f, plano: true);

        EspalharDecalques("Terra", new (float x, float y, float escala)[]
        {
            (101.96f, 13.99f, 1.0f), (105.74f, 14.6f, 1.05f), (113.10f, 14.43f, 1.1f),
            (117.20f, 14.4f, .95f), (122.02f, 14.04f, .88f), (128.61f, 14.26f, 1.05f),
            (134.19f, 14.02f, 1.02f), (103.1f, 5.58f, .98f), (108.62f, 5.60f, 1.16f),
            (115.46f, 5.59f, 1.03f), (120.73f, 5.45f, .91f), (125.75f, 5.56f, 1.02f),
            (132.28f, 5.62f, 1.10f)
        }, .68f);
        EspalharDecalques("Folhas", new (float x, float y, float escala)[]
        {
            (103.32f, 12.32f, .61f), (115.55f, 12.30f, .66f),
            (129.57f, 12.30f, .61f), (121.25f, 7.50f, .62f)
        }, .49f);
        EspalharDecalques("Pedras", new (float x, float y, float escala)[]
        {
            (105.61f, 14.55f, .68f), (113.71f, 14.09f, .61f), (122.1f, 14.01f, .70f),
            (128.58f, 14.15f, .81f), (134.47f, 13.93f, .66f), (108.21f, 5.84f, .73f),
            (120.65f, 5.68f, .62f), (126.42f, 5.84f, .79f)
        }, .92f);
        EspalharVegetacao("Capim", new (float x, float y, float escala)[]
        {
            (102.7f, 14.02f, .54f), (105.81f, 14.41f, .68f), (108.84f, 15.10f, .50f),
            (112.43f, 14.96f, .56f), (116.18f, 14.10f, .57f), (122.06f, 14.10f, .63f),
            (128.55f, 14.16f, .70f), (131.53f, 14.13f, .50f), (134.81f, 14.09f, .71f),
            (104.33f, 5.67f, .65f), (107.02f, 5.72f, .71f), (112.77f, 5.61f, .58f),
            (118.12f, 5.63f, .68f), (123.71f, 5.64f, .58f), (128.99f, 5.72f, .64f),
            (133.22f, 5.65f, .63f)
        });
    }

    private int MontarDefensas(float[] pontos, float y)
    {
        int count = 0;
        foreach (float x in pontos)
        {
            var defensa = ModuloDaRodovia("Defensa da vala", "Defensa", new Vector2(x, y),
                volume: new Vector3(2.51f, .29f, .66f), alturaDoVolume: .26f);
            if (defensa == null) continue;
            LimiteDaRodovia(defensa.transform, new Vector2(0, .23f), new Vector2(2.54f, .28f));
            count++;
        }
        return count;
    }

    private void EspalharVegetacao(string arte, (float x, float y, float escala)[] pontos)
    {
        for (int i = 0; i < pontos.Length; i++)
        {
            var (x, y, escala) = pontos[i];
            var tinta = Color.Lerp(new Color(.68f, .76f, .77f), new Color(.88f, .91f, .87f), (i % 4) / 3f);
            var planta = ModuloDaRodovia(arte, arte, new Vector2(x, y), escala, i % 2 == 0, tinta: tinta);
            if (planta == null) continue;
            planta.GetPropertyBlock(receiverProperties);
            receiverProperties.SetVector("_Wind", new Vector4(.036f * escala, planta.bounds.size.y, x * .69f, 0));
            planta.SetPropertyBlock(receiverProperties);
        }
    }

    private void EspalharDecalques(string arte, (float x, float y, float escala)[] pontos, float opacidade)
    {
        for (int i = 0; i < pontos.Length; i++)
        {
            var (x, y, escala) = pontos[i];
            ModuloDaRodovia(arte, arte, new Vector2(x, y), escala, i % 2 == 0,
                plano: true, tinta: new Color(1, 1, 1, opacidade));
        }
    }

    private SpriteRenderer ModuloDaRodovia(string nome, string arte, Vector2 pe, float escala = 1,
        bool flip = false, bool plano = false, Color? tinta = null,
        Vector3 volume = default, float alturaDoVolume = 0, string pasta = ModulosDaRodovia)
    {
        var sprite = Resources.Load<Sprite>(pasta + arte);
        if (sprite == null) return null;
        var obj = new GameObject(nome, typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = pe;
        obj.transform.localScale = new Vector3(escala, escala, 1);
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.flipX = flip;
        renderer.color = tinta ?? Color.white;
        renderer.sharedMaterial = lighting;
        renderer.sortingOrder = plano ? -19 : 1000 - Mathf.RoundToInt(pe.y * 10);

        int caster = volume.z > 0 ? RegistrarVolumeDaRodovia(pe, volume, alturaDoVolume) : -1;
        SetReceiver(renderer, pe.y, plano ? -1f : .88f, caster);
        AplicarRelevoDaRodovia(renderer);
        return renderer;
    }

    private int RegistrarVolumeDaRodovia(Vector2 pe, Vector3 volume, float altura = 0)
    {
        if (casterCount >= CasterLimit - 1) return -1;
        int id = casterCount++;
        var local = pe - origin;
        casterMin[id] = new Vector4(local.x - volume.x * .5f, local.y + .035f, altura, (float)Sombra.ProjetadaSuave);
        casterMax[id] = new Vector4(local.x + volume.x * .5f, local.y + volume.y, volume.z, 0);
        return id;
    }

    private void FixarDetalheDaRodovia(SpriteRenderer apoio, string nome, string arte, Vector2 deslocamento,
        string pasta = ModulosDaRodovia)
    {
        Vector2 posicao = (Vector2)apoio.transform.position + deslocamento;
        var detalhe = ModuloDaRodovia(nome, arte, posicao, tinta: new Color(.84f, .85f, .85f), pasta: pasta);
        if (detalhe == null) return;

        detalhe.sortingOrder = apoio.sortingOrder + 1;
        apoio.GetPropertyBlock(receiverProperties);
        var receiver = receiverProperties.GetVector("_Receiver");
        // O detalhe acompanha a altura do apoio e compartilha sua sombra no chão.
        receiver.w = 1;
        detalhe.GetPropertyBlock(receiverProperties);
        receiverProperties.SetVector("_Receiver", receiver);
        detalhe.SetPropertyBlock(receiverProperties);
    }

    private void AplicarRelevoDaRodovia(SpriteRenderer renderer)
    {
        if (renderer.sprite == null || renderer.sprite.packed) return;
        string nome = renderer.sprite.texture.name;
        string pasta = posto ? ObjetosDoPosto : encruzilhada ? ObjetosDoMercadinho : bairro ? DetalhesDoBairro : DetalhesDoBar;
        var mapa = pasta == null ? null : Resources.Load<Texture2D>(pasta + "Relevos/" + nome);
        if (mapa == null) mapa = Resources.Load<Texture2D>(RelevosDaRodovia + nome);
        if (mapa == null) return;
        renderer.GetPropertyBlock(receiverProperties);
        receiverProperties.SetTexture("_PropSurface", mapa);
        receiverProperties.SetFloat("_PropSurfaceEnabled", 1);
        renderer.SetPropertyBlock(receiverProperties);
    }

    private static void LimiteDaRodovia(Transform pai, Vector2 offset, Vector2 size)
    {
        var box = pai.gameObject.AddComponent<BoxCollider2D>();
        box.offset = offset;
        box.size = size;
    }

    private void CriarVaoDeFiacao(SpriteRenderer esquerda, SpriteRenderer direita)
    {
        const int ppu = 32;
        var a = esquerda.transform.position;
        var b = direita.transform.position;
        float firstY = esquerda.bounds.max.y - .31f;
        float lastY = direita.bounds.max.y - .31f;
        float minY = Mathf.Min(firstY, lastY) - .65f;
        int height = Mathf.CeilToInt((Mathf.Max(firstY, lastY) - minY) * ppu) + 1;
        int width = Mathf.RoundToInt((b.x - a.x) * ppu) + 1;
        var pixels = new Color32[width * height];
        for (int x = 0; x < width; x++)
        {
            float u = x / (float)(width - 1);
            float flecha = 4 * u * (1 - u);
            int top = Mathf.RoundToInt((Mathf.Lerp(firstY, lastY, u) - minY - .32f * flecha) * ppu);
            int bottom = top - Mathf.RoundToInt(3 + 4 * flecha);
            pixels[top * width + x] = new Color32(16, 17, 21, 244);
            pixels[bottom * width + x] = new Color32(16, 18, 23, 211);
        }
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            name = "Dois cabos entre postes",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels32(pixels);
        texture.Apply(false, true);
        var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero, ppu, 0, SpriteMeshType.FullRect);
        spritesDaFiacao.Add(sprite);
        var vao = new GameObject("Vão de fiação", typeof(SpriteRenderer));
        vao.transform.SetParent(transform, false);
        vao.transform.position = new Vector3(a.x, minY, 0);
        var renderer = vao.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = Mathf.Min(esquerda.sortingOrder, direita.sortingOrder) - 1;
    }

    private void ConfigurarReflexoDoCarro(SpriteRenderer carro)
    {
        if (carro == null || carro.sprite == null || carro.sprite.packed) return;
        var frame = carro.sprite;
        var uv = frame.rect;
        var bounds = carro.bounds;
        var atlas = frame.texture;
        lighting.SetTexture("_ArrivalCar", atlas);
        lighting.SetVector("_ArrivalCarUV", new Vector4(
            (carro.flipX ? uv.xMax : uv.xMin) / atlas.width, uv.yMin / atlas.height,
            (carro.flipX ? -uv.width : uv.width) / atlas.width, uv.height / atlas.height));
        lighting.SetVector("_ArrivalCarBounds", new Vector4(bounds.min.x - origin.x, bounds.min.y - origin.y,
            bounds.size.x, bounds.size.y));
        lighting.SetFloat("_ArrivalCarFeet", carro.transform.position.y - origin.y);
    }
}
