using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal sealed partial class Capitulo2Exterior : MonoBehaviour
{
    private const string Art = "Art/Cap2/Remodelagem/";
    private static readonly HashSet<string> Fachadas = new()
    {
        "Igreja", "Prefeitura", "Rodoviaria", "Fundos_MesmoMercadinho", "OficinaRadio",
        "CasaCostura", "Sociedade", "Abrigo_Cemiterio"
    };
    private readonly List<SpriteRenderer> clima = new();
    private readonly List<UnityEngine.Object> recursos = new();
    private readonly List<Vector2> fontes = new();
    private readonly List<(LensFlareComponentSRP flare, Vector2 bulb)> flaresDosPostes = new();
    private LensFlareDataSRP perfilDosFlares;
    private Material lit, shadow;
    private PlayerController controladorMarcus;
    private SpriteRenderer marcus;
    private Material materialMarcus;
    private Texture texturaMarcus;
    private string pasta;
    private PecaCentroCena[] pecasDaCena;
    private AguaCentro aguaCentro;
    private Camera cameraPrincipal;
    private Material materialDispersao;
    private readonly List<(Vector2 bulb, Vector4 local)> fontesDispersao = new();
    private readonly Vector4[] dispersaoVisiveis = new Vector4[LimiteLuzesCentro];
    private bool dispersaoInicializada;
    private float ultimaDispCamMinX, ultimaDispCamMaxX, ultimaDispCamMinY, ultimaDispCamMaxY;
    private int ultimoClimaAtivo = -1;

    internal void Montar(bool cemiterio, Vector2 min, Vector2 max)
    {
        lit = Resources.Load<Material>("Art/WorldLit");
        if (lit == null) return;
        Material worldPbr = lit;
        if (!cemiterio)
        {
            var shader = Shader.Find(SantaRitaWorldLighting.StockSpriteShader);
            if (shader != null)
            {
                lit = new Material(shader);
                recursos.Add(lit);
            }
        }
        pasta = Art + (cemiterio ? "Cemiterio/" : "Centro/");
        pecasDaCena = cemiterio ? Array.Empty<PecaCentroCena>() : GetComponentsInChildren<PecaCentroCena>(true);
        var sinalizacao = transform.Find("Sinalização fixa do Centro");
        if (!cemiterio && sinalizacao != null)
            foreach (var renderer in sinalizacao.GetComponentsInChildren<SpriteRenderer>(true))
                renderer.sharedMaterial = lit;
        var existentes = Array.FindAll(FindObjectsByType<SpriteRenderer>(), s => s.GetComponent<PecaCentroCena>() == null);
        foreach (var sprite in existentes)
        {
            if (sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            if (!cemiterio && sprite.sharedMaterial == worldPbr) sprite.sharedMaterial = lit;
            if (sprite.name == "Poste") Trocar(sprite, "Art/RuaNorte/Poste", 1.12f);
            if (cemiterio && sprite.name.StartsWith("Arvore", StringComparison.Ordinal))
            {
                Vector2 at = sprite.transform.position;
                if (!cemiterio && NaPraca(at)) continue;
                string[] especies = { "Pitangueira", "Camboata", "Bracatinga", "Araucaria" };
                int variation = Mathf.Abs(Mathf.RoundToInt(at.x * 19 + at.y * 37)) % especies.Length;
                if (Trocar(sprite, "Art/RuaNorte/Vegetacao/" + especies[variation], Mathf.Clamp(sprite.bounds.size.x, 2.7f, 5.3f)) &&
                    cemiterio)
                    sprite.color = Color.Lerp(Color.white, new Color(.68f, .75f, .76f), .36f + variation * .11f);
            }
            if (cemiterio && sprite.name.StartsWith("Sepultura_", StringComparison.Ordinal))
            {
                Vector2 at = sprite.transform.position;
                int variation = Mathf.Abs(Mathf.RoundToInt(at.x + at.y * 2)) % 5;
                Trocar(sprite, pasta + (variation == 1 ? "Cruz" : "Lapide"), variation == 1 ? .68f : 1.17f);
            }
        }

        if (!cemiterio)
        {
            PavimentarCentro(existentes);
            controladorMarcus = FindAnyObjectByType<PlayerController>();
            if (controladorMarcus != null && controladorMarcus.gameObject.scene == gameObject.scene &&
                controladorMarcus.TryGetComponent(out marcus))
                materialMarcus = IsolarTextura(marcus);
            var mapaAgua = Array.Find(FindObjectsByType<UnityEngine.Tilemaps.Tilemap>(), m => m.gameObject.scene == gameObject.scene && m.name == "Água e madeira do Centro");
            aguaCentro = AguaCentro.Instalar(mapaAgua, controladorMarcus != null ? controladorMarcus.transform : null);
            ComporPraca(existentes);
            ComporDetalhesGeradosDaPraca();
            ComporMargensDoCentro();
            IntegrarObjetosDasRuas(existentes);
        }

        foreach (var sprite in existentes)
        {
            if (sprite == null || !sprite.gameObject.activeInHierarchy ||
                sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            if (sprite.name == "Poste")
            {
                var lamp = new Vector2(sprite.bounds.max.x - .18f, sprite.bounds.max.y - .22f);
                fontes.Add(lamp);
                if (!cemiterio)
                {
                    bool praca = NaPraca(sprite.transform.position);
                    LuzPraca("Luz do poste", lamp - Vector2.up * 1.8f, praca ? 4.8f : 4.1f,
                        praca ? .69f : .54f, Ambar);
                    Vector2 pe = sprite.transform.position;
                    PecaPraca("Desgaste junto ao poste", "Art/RuaNorte/Entrada/Modulos/Folhas",
                        pe.x, pe.y, .85f, noPiso: true);
                }
            }
            if (Fachadas.Contains(sprite.name) && (cemiterio || sprite.name != "Igreja") &&
                (cemiterio || transform.Find("Luzes fixas do Centro") == null))
            {
                Vector2 at = sprite.transform.position;
                // A fonte fica acima da soleira; o gatilho e a fachada conservam seus IDs.
                var fixture = new GameObject("Luz de soleira · " + sprite.name, typeof(Light2D));
                fixture.transform.SetParent(transform, false);
                fixture.transform.position = at + new Vector2(0, .45f);
                var light = fixture.GetComponent<Light2D>();
                light.lightType = Light2D.LightType.Point;
                light.color = new Color(1, .72f, .43f);
                light.intensity = .59f;
                light.pointLightInnerRadius = .55f;
                light.pointLightOuterRadius = 4.2f;
                if (!cemiterio) light.shadowsEnabled = false;
            }
        }

        if (cemiterio)
        {
            Prop("Velas junto ao abrigo", "Velas", new Vector2(76.20f, 100.24f));
            Prop("Vaso no acesso ao jazigo", "Vaso", new Vector2(62.18f, 98.94f));
            Prop("Vaso junto à ala leste", "Vaso", new Vector2(74.85f, 89.12f));
            Prop("Mureta da ala oeste", "Pedra", new Vector2(58.6f, 85.46f));
            Prop("Mureta da ala leste", "Pedra", new Vector2(69.65f, 85.46f));
            Prop("Grade sobre o muro oeste", "Grade", new Vector2(59.7f, 84.08f));
            Prop("Grade sobre o muro leste", "Grade", new Vector2(68.4f, 84.08f));
            Submata(new Vector2(50.75f, 88.9f), new Vector2(50.1f, 95.6f), new Vector2(56.8f, 103.1f),
                new Vector2(71.7f, 103.8f), new Vector2(81.15f, 100.1f), new Vector2(81.1f, 91.2f));
        }
        else
        {
            foreach (var at in new[] { new Vector2(66.08f, 20.83f), new Vector2(69.99f, 20.83f) })
            {
                if (GramaNoCentro(at)) continue;
                var balizador = Prop("Balizador da soleira", "Balizador", at);
                if (balizador != null)
                    ConfigurarSpriteVolumetricoPraca(balizador, balizador.bounds.min.y, .85f, 0f, 0f, .25f, modoOclusor: -1);
            }
            foreach (var at in new[] { new Vector2(71.78f, 30.76f), new Vector2(26.65f, 21.33f), new Vector2(28.2f, 40.91f) })
                if (!GramaNoCentro(at)) Prop("Ralo de sarjeta", "Ralo", at, plano: true);
            IntegrarDetalhesDosAcessos();
            CriarFontesDeIluminacaoVivas();
            IntegrarLuzDasEntradasAoPiso();
            IluminarPisoDoCentro();
            if (aguaCentro != null) aguaCentro.SincronizarLuzesDoCentro(luzesVolumetricasPraca, origemDoPiso, materialDoPiso);
        }

        var projection = cemiterio ? Resources.Load<Shader>("Art/Cap2/Remodelagem/PisoMolhadoC2") : null;
        if (projection != null)
        {
            shadow = new Material(projection);
            recursos.Add(shadow);
            foreach (var sprite in existentes)
            {
                if (sprite == null || sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
                if (Fachadas.Contains(sprite.name) || sprite.name.StartsWith("Sepultura_") ||
                    sprite.name.StartsWith("Arvore") || sprite.name == "Poste") Projetar(sprite);
            }
            foreach (var p in fontes) Reflexo(p - new Vector2(.02f, 3.12f));
        }
        Nevoa(min, max, cemiterio);
    }

    private Material IsolarTextura(SpriteRenderer renderer)
    {
        // A fachada e o corpo animado têm bindings independentes do material comum.
        var material = new Material(lit) { mainTexture = renderer.sprite?.texture };
        renderer.sharedMaterial = material;
        recursos.Add(material);
        return material;
    }

    private bool Trocar(SpriteRenderer target, string caminho, float largura)
    {
        var sprite = Resources.Load<Sprite>(caminho);
        if (sprite == null) return false;
        target.sprite = sprite;
        // Escala só o desenho. A árvore e o banco mantêm a pegada original.
        var box = target.GetComponent<BoxCollider2D>();
        Vector3 old = target.transform.localScale;
        var scale = largura / Mathf.Max(.01f, sprite.bounds.size.x);
        target.transform.localScale = new Vector3(scale, scale, old.z);
        if (box != null)
        {
            var ratio = new Vector2(old.x / scale, old.y / scale);
            box.size = Vector2.Scale(box.size, ratio);
            box.offset = Vector2.Scale(box.offset, ratio);
        }
        target.sharedMaterial = lit;
        return true;
    }

    private SpriteRenderer Prop(string nome, string arte, Vector2 at, bool plano = false)
    {
        var existente = BuscarPecaDaCena(nome, at);
        if (existente != null)
        {
            var rendererExistente = existente.GetComponent<SpriteRenderer>();
            if (rendererExistente != null) rendererExistente.sharedMaterial = lit;
            return rendererExistente;
        }
        var sprite = Resources.Load<Sprite>(arte.StartsWith("Art/", StringComparison.Ordinal) ? arte : pasta + arte);
        if (sprite == null) return null;
        var obj = new GameObject(nome, typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
#if UNITY_EDITOR
        if (gameObject.scene.name == "CentroSantaRita")
        {
            var origem = obj.AddComponent<PecaCentroCena>();
            origem.nomeDeOrigem = nome;
            origem.posicaoDeOrigem = at;
        }
#endif
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sharedMaterial = lit;
        renderer.sortingOrder = plano ? -1700 : 1000 - Mathf.RoundToInt(at.y * 10);
        return renderer;
    }

    private PecaCentroCena BuscarPecaDaCena(string nome, Vector2 origem)
    {
        if (pecasDaCena == null) return null;
        foreach (var peca in pecasDaCena)
            if (peca != null && peca.nomeDeOrigem == nome &&
                (peca.posicaoDeOrigem - origem).sqrMagnitude < .0001f)
                return peca;
        return null;
    }

    private void Submata(params Vector2[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            var bush = Prop("Submata de margem", "Art/RuaNorte/Vegetacao/" +
                (i % 2 == 0 ? "MoitaDeBorda" : "SamambaiaDaMata"), positions[i]);
            if (bush != null) bush.color = new Color(.76f, .82f, .80f);
        }
    }

    private void Projetar(SpriteRenderer sprite)
    {
        if (!sprite.enabled || !sprite.gameObject.activeInHierarchy) return;
        Vector2 at = sprite.transform.position;
        Vector2 direction = Vector2.zero;
        float nearest = 10f;
        foreach (var source in fontes)
        {
            Vector2 delta = at - (source - Vector2.up * 2.5f);
            float distance = delta.magnitude;
            if (distance >= nearest) continue;
            nearest = distance;
            direction = delta.normalized;
        }
        if (nearest >= 10f) return;
        var pts = sprite.sprite.vertices;
        var vertices = new Vector3[pts.Length];
        Vector3 scale = sprite.transform.lossyScale;
        float yMin = sprite.sprite.bounds.min.y;
        for (int i = 0; i < pts.Length; i++)
        {
            float altura = (pts[i].y - yMin) * scale.y;
            vertices[i] = new Vector3(pts[i].x * scale.x * (sprite.flipX ? -1 : 1) + altura * direction.x * .54f,
                altura * (direction.y * .48f + .12f), 0);
        }
        var mesh = new Mesh { name = "Silhueta " + sprite.name, vertices = vertices, uv = sprite.sprite.uv };
        mesh.triangles = Array.ConvertAll(sprite.sprite.triangles, n => (int)n);
        recursos.Add(mesh);
        var obj = new GameObject("Sombra · " + sprite.name, typeof(MeshFilter), typeof(MeshRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = shadow;
        renderer.sortingOrder = -1600;
        var props = new MaterialPropertyBlock();
        props.SetTexture("_SilhouetteTex", sprite.sprite.texture);
        props.SetColor("_Tint", new Color(.030f, .043f, .051f, Mathf.Lerp(.26f, .04f, nearest / 10f)));
        renderer.SetPropertyBlock(props);
    }

    private void Reflexo(Vector2 at, float intensidade = .26f)
    {
        var obj = new GameObject("Brilho de piso molhado", typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
        obj.transform.localScale = new Vector3(1.95f, 3.20f, 1);
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = Quad();
        renderer.sortingOrder = -1580;
        renderer.sharedMaterial = shadow;
        var props = new MaterialPropertyBlock();
        props.SetFloat("_Wet", 1);
        props.SetColor("_Tint", new Color(.90f, .60f, .32f, intensidade));
        renderer.SetPropertyBlock(props);
        clima.Add(renderer);
    }

    private Sprite quad;
    private Sprite Quad()
    {
        if (quad != null) return quad;
        var texture = Texture2D.whiteTexture;
        quad = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), texture.width);
        recursos.Add(quad);
        return quad;
    }

    private void Nevoa(Vector2 min, Vector2 max, bool cemiterio)
    {
        var shader = Resources.Load<Shader>("Art/RuaNorte/Nevoeiro");
        if (shader == null) return;
        Vector2 extent = max - min;
        for (int i = 0; i < 2; i++)
        {
            var obj = new GameObject(i == 0 ? "Névoa de fundo" : "Dispersão nos postes", typeof(SpriteRenderer));
            obj.transform.SetParent(transform, false);
            obj.transform.position = (min + max) * .5f;
            obj.transform.localScale = new Vector3(extent.x, extent.y, 1);
            var renderer = obj.GetComponent<SpriteRenderer>();
            renderer.sprite = Quad();
            renderer.sortingOrder = 2030 + i;
            var material = new Material(shader);
            recursos.Add(material);
            material.SetVector("_Ground", new Vector4(min.x, min.y, extent.x, extent.y));
            material.SetFloat("_Entrance", 1);
            material.SetFloat("_Front", i);
            material.SetFloat("_BeamStrength", cemiterio ? .54f : .52f);
            if (!cemiterio)
            {
                material.SetFloat("_BeamLean", 0);
                material.SetFloat("_PracaDaMatriz", 1);
            }
            if (!cemiterio && i == 1 && materialDoPiso != null)
            {
                material.SetTexture("_FireflyTerrain", materialDoPiso.GetTexture("_GroundCells"));
                material.SetVector("_FireflyTerrainInfo",
                    new Vector4(origemDoPiso.x, origemDoPiso.y, LinhaDoAtlas(PisoCentro.Grama), 0));
                material.SetFloat("_UseFireflyTerrain", 1);
            }
            var lamps = new Vector4[LimiteLuzesCentro];
            int count = Mathf.Min(lamps.Length, fontes.Count);
            if (!cemiterio && i == 1)
            {
                fontesDispersao.Clear();
                materialDispersao = material;
                dispersaoInicializada = false;
            }
            for (int j = 0; j < count; j++)
            {
                Vector2 p = fontes[j] - min;
                var packed = new Vector4(p.x, p.y, p.y - 2.5f, 1);
                lamps[j] = packed;
                if (!cemiterio && i == 1) fontesDispersao.Add((fontes[j], packed));
            }
            material.SetVectorArray("_EntranceLamps", lamps);
            material.SetInteger("_EntranceLampCount", count);
            renderer.sharedMaterial = material;
            clima.Add(renderer);
        }
        if (!cemiterio) PrepararFlaresDosPostes();
    }

    private void PrepararFlaresDosPostes()
    {
        if (fontes.Count == 0 || perfilDosFlares != null) return;
        perfilDosFlares = ScriptableObject.CreateInstance<LensFlareDataSRP>();
        perfilDosFlares.elements = new[]
        {
            new LensFlareDataElementSRP
            {
                tint = new Color(1f, .61f, .24f),
                sizeXY = new Vector2(1f, .12f),
                uniformScale = 1.06f,
                localIntensity = .23f,
                edgeOffset = .72f,
                fallOff = .79f
            },
            new LensFlareDataElementSRP
            {
                tint = new Color(1f, .70f, .32f),
                sizeXY = new Vector2(.14f, 1f),
                uniformScale = .44f,
                rotation = 22f,
                localIntensity = .12f,
                edgeOffset = .72f,
                fallOff = .79f
            },
            new LensFlareDataElementSRP
            {
                position = .48f,
                flareType = SRPLensFlareType.Polygon,
                sideCount = 6,
                sdfRoundness = .38f,
                tint = new Color(1f, .64f, .34f),
                uniformScale = .25f,
                localIntensity = .095f,
                edgeOffset = .58f,
                fallOff = .78f
            },
            new LensFlareDataElementSRP
            {
                position = .74f,
                flareType = SRPLensFlareType.Polygon,
                sideCount = 6,
                sdfRoundness = .5f,
                tint = new Color(1f, .56f, .26f),
                uniformScale = .39f,
                localIntensity = .065f,
                edgeOffset = .76f,
                fallOff = .78f
            }
        };
        recursos.Add(perfilDosFlares);

        var grupoFlares = new GameObject("Reflexos na lente dos postes");
        grupoFlares.transform.SetParent(transform, false);
        foreach (var bulb in fontes)
        {
            var emissor = new GameObject("Lens flare");
            emissor.transform.SetParent(grupoFlares.transform, false);
            emissor.transform.position = new Vector3(bulb.x, bulb.y, 0f);
            var flare = emissor.AddComponent<LensFlareComponentSRP>();
            flare.lensFlareData = perfilDosFlares;
            flare.attenuationByLightShape = false;
            flare.scaleByDistanceCurve = AnimationCurve.Constant(0f, 1f, 1f);
            flare.radialScreenAttenuationCurve = AnimationCurve.EaseInOut(.5f, 1f, 1f, 0f);
            flare.intensity = .36f;
            flaresDosPostes.Add((flare, bulb));
        }
    }

    private void AtualizarDispersaoNevoa(bool enabled, bool temCam, float camMinX, float camMaxX, float camMinY, float camMaxY)
    {
        if (!enabled || !temCam || materialDispersao == null || fontesDispersao.Count == 0) return;
        if (dispersaoInicializada &&
            Mathf.Abs(camMinX - ultimaDispCamMinX) < .25f &&
            Mathf.Abs(camMaxX - ultimaDispCamMaxX) < .25f &&
            Mathf.Abs(camMinY - ultimaDispCamMinY) < .25f &&
            Mathf.Abs(camMaxY - ultimaDispCamMaxY) < .25f)
            return;

        ultimaDispCamMinX = camMinX;
        ultimaDispCamMaxX = camMaxX;
        ultimaDispCamMinY = camMinY;
        ultimaDispCamMaxY = camMaxY;
        dispersaoInicializada = true;

        float minX = camMinX - 1f;
        float maxX = camMaxX + 1f;
        float minY = camMinY - 1f;
        float maxY = camMaxY + 1f;
        int visiveis = 0;
        for (int i = 0; i < fontesDispersao.Count; i++)
        {
            var (bulb, local) = fontesDispersao[i];
            // Em Nevoeiro.shader, o cone/halo de cada poste atua somente em [-2.4,+2.4] x [-3.6,+0.6].
            if (bulb.x + 2.4f < minX || bulb.x - 2.4f > maxX ||
                bulb.y + .6f < minY || bulb.y - 3.6f > maxY) continue;
            dispersaoVisiveis[visiveis++] = local;
        }
        materialDispersao.SetVectorArray("_EntranceLamps", dispersaoVisiveis);
        materialDispersao.SetInteger("_EntranceLampCount", visiveis);
    }

    private void AtualizarFlaresDosPostes(bool enabled, bool temCam, float camMinX, float camMaxX, float camMinY, float camMaxY)
    {
        if (flaresDosPostes.Count == 0) return;
        bool temMarcus = enabled && marcus != null;
        Vector2 posMarcus = temMarcus ? (Vector2)marcus.bounds.center : Vector2.zero;
        float minX = camMinX - 3.5f;
        float maxX = camMaxX + 3.5f;
        float minY = camMinY - 3.5f;
        float maxY = camMaxY + 3.5f;
        foreach (var (flare, bulb) in flaresDosPostes)
        {
            if (flare == null) continue;
            if (!enabled)
            {
                if (flare.enabled) flare.enabled = false;
                flare.intensity = 0f;
                continue;
            }
            bool naTela = !temCam || (bulb.x >= minX && bulb.x <= maxX && bulb.y >= minY && bulb.y <= maxY);
            if (flare.enabled != naTela) flare.enabled = naTela;
            if (!naTela) continue;
            float boost = 0f;
            if (temMarcus)
            {
                float lateral = Mathf.Abs(posMarcus.x - bulb.x);
                float abaixo = bulb.y - posMarcus.y;
                if (lateral < 1.85f && abaixo > -.15f && abaixo < 3.10f)
                {
                    float entrada = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-.15f, .45f, abaixo));
                    float saida = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(1.65f, 3.10f, abaixo));
                    float largura = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.35f, 1.85f, lateral));
                    boost = entrada * saida * largura;
                }
            }
            flare.intensity = Mathf.Lerp(.36f, .92f, boost);
        }
    }

    private void LateUpdate()
    {
        if (marcus != null && marcus.sprite != null && marcus.sprite.texture != texturaMarcus)
        {
            texturaMarcus = marcus.sprite.texture;
            if (materialMarcus != null) materialMarcus.mainTexture = texturaMarcus;
        }
        bool enabled = !MenuPausa.ReduzirEfeitosIntensos;
        if (cameraPrincipal == null || !cameraPrincipal.isActiveAndEnabled)
            cameraPrincipal = Camera.main;
        bool temCam = cameraPrincipal != null;
        float camMinX = 0f, camMaxX = 0f, camMinY = 0f, camMaxY = 0f;
        if (temCam)
        {
            float meiaAltura = cameraPrincipal.orthographic ? cameraPrincipal.orthographicSize : 6f;
            float meiaLargura = meiaAltura * Mathf.Max(.5f, cameraPrincipal.aspect);
            Vector3 posCam = cameraPrincipal.transform.position;
            camMinX = posCam.x - meiaLargura;
            camMaxX = posCam.x + meiaLargura;
            camMinY = posCam.y - meiaAltura;
            camMaxY = posCam.y + meiaAltura;
        }
        if (materialDoPiso != null)
        {
            if (ultimoClimaAtivo != (enabled ? 1 : 0))
                materialDoPiso.SetFloat("_WindStrength", enabled ? 1f : 0f);
            if (temCam) AtualizarVisibilidadeCentro(camMinX, camMaxX, camMinY, camMaxY);
            AtualizarLuzesDinamicasDaPraca(enabled);
            SantaRitaRuaNorte.AtualizarReflexoDoMarcus(materialDoPiso, enabled ? marcus : null, origemDoPiso);
        }
        AtualizarIluminacaoViva(enabled);
        AtualizarDispersaoNevoa(enabled, temCam, camMinX, camMaxX, camMinY, camMaxY);
        int estadoClima = enabled ? 1 : 0;
        if (ultimoClimaAtivo != estadoClima)
        {
            foreach (var renderer in clima) renderer.enabled = enabled;
            ultimoClimaAtivo = estadoClima;
        }
        AtualizarFlaresDosPostes(enabled, temCam, camMinX, camMaxX, camMinY, camMaxY);
    }

    private void OnDestroy()
    {
        foreach (var item in recursos) Destroy(item);
    }
}

