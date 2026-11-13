using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal sealed partial class Capitulo2Interior3D
{
    private static Vector3 V(float x, float y, float z) => new Vector3(x, y, z);

    // Compensação de luz rebatida para os interiores gerados em tempo de execução.
    // Cada ambiente conserva sua temperatura sem depender de luzes diretas para ser legível.
    private void AjustarLuzIndireta(string scene)
    {
        Color indireta = scene switch
        {
            "IgrejaMatriz" => new Color(.22f, .19f, .16f),
            "PrefeituraSantaRita" => new Color(.18f, .18f, .17f),
            "RodoviariaSantaRita" => new Color(.16f, .18f, .20f),
            "MercadinhoDeposito" => new Color(.19f, .17f, .15f),
            "OficinaRadio" => new Color(.18f, .17f, .16f),
            "CasaCostureira" => new Color(.20f, .17f, .15f),
            "AbrigoCemiterio" => new Color(.15f, .17f, .19f),
            "SociedadeBeiraRio" => new Color(.19f, .17f, .15f),
            _ => new Color(.18f, .18f, .18f)
        };
        foreach (var material in surfaces.Values)
            if (material != null && material.shader != null && material.shader.name == "SantaRita/Sinuca/SalaLit")
                material.SetColor("_AmbientColor", indireta);
        foreach (var material in spriteMaterials.Values)
            if (material != null && material.shader != null && material.shader.name == "SantaRita/Sinuca/SalaLit")
                material.SetColor("_AmbientColor", indireta);
        RenderSettings.ambientLight = indireta;
    }

    private void ConfigurarEntrada(float soleira, Vector3 chegada, string passoSom = "Foley/footstep_wood_000")
    {
        soleiraZ = soleira;
        pontoChegada = chegada;
        somPasso = passoSom;
    }

    private void RegistrarAnimacao(Action<float> animacao)
    {
        if (animacao != null) animacoes.Add(animacao);
    }

    private Transform Pivo(string nome, Transform pai, Vector3 pos)
    {
        var t = new GameObject(nome).transform;
        t.gameObject.layer = RoomLayer;
        t.SetParent(pai != null ? pai : world, false);
        t.localPosition = pos;
        return t;
    }

    private Material Superficie(string nome, Color cor, Texture2D textura = null,
        Vector2? escala = null, float brilho = .14f, bool unlit = false, bool transparente = false,
        float metalico = 0f, float hdr = 1f)
    {
        string chave = $"{nome}_{cor}_{(textura != null ? textura.GetEntityId().ToString() : "0")}_{escala}_{brilho}_{unlit}_{transparente}_{metalico}_{hdr}";
        if (surfaces.TryGetValue(chave, out var cached)) return cached;

        var salaLit = !unlit && !transparente ? Resources.Load<Shader>("Art/Sinuca/SalaLit") : null;
        var shader = salaLit != null && salaLit.isSupported
            ? salaLit
            : Shader.Find(unlit ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit");
        var mat = new Material(shader) { name = nome, color = cor };
        if (shader == salaLit)
            mat.SetColor("_BaseColor", cor);
        else if (unlit && !transparente && hdr > 1.01f)
            mat.SetColor("_BaseColor", new Color(cor.r * hdr, cor.g * hdr, cor.b * hdr, cor.a));
        if (textura != null)
        {
            mat.SetTexture("_BaseMap", textura);
            if (escala.HasValue)
            {
                mat.SetTextureScale("_BaseMap", escala.Value);
                mat.SetVector("_BaseMap_ST", new Vector4(escala.Value.x, escala.Value.y, 0f, 0f));
            }
        }
        if (shader == salaLit)
        {
            mat.SetFloat("_RoomSurface", .72f);
            mat.SetFloat("_ActorLighting", 0f);
            mat.SetFloat("_CeilingFade", 3.2f);
            mat.SetFloat("_Smoothness", Mathf.Clamp(brilho, .02f, .88f));
            mat.SetFloat("_Metallic", Mathf.Clamp01(metalico));
            mat.SetColor("_AmbientColor", new Color(.010f, .012f, .016f, 1f));
            mat.SetColor("_HazeColor", new Color(.003f, .004f, .006f, 1f));
            mat.SetFloat("_HazeStrength", .10f);
            mat.SetFloat("_FilmicTonemap", 1f);
        }
        else if (!unlit)
        {
            mat.SetFloat("_Smoothness", brilho);
            mat.SetFloat("_Metallic", Mathf.Clamp01(metalico));
        }
        if (transparente)
        {
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_Cull", 0f);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_SrcBlendAlpha", (int)BlendMode.Zero);
            mat.SetInt("_DstBlendAlpha", (int)BlendMode.One);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }
        surfaces[chave] = mat;
        owned.Add(mat);
        return mat;
    }

    private Material VidroExterior(Color? tint = null)
    {
        if (tint == null)
        {
            if (surfaces.TryGetValue("VidroExterior", out var cached)) return cached;
            var shader = Resources.Load<Shader>("Art/Sinuca/JanelaMolhada");
            if (shader != null)
            {
                var mat = new Material(shader) { name = "VidroExterior" };
                surfaces["VidroExterior"] = mat;
                owned.Add(mat);
                return mat;
            }
        }
        return Superficie(tint.HasValue ? $"Vidro_{tint.Value}" : "VidroExterior",
            tint ?? new Color(.05f, .08f, .11f, .18f), brilho: .75f, unlit: true, transparente: true);
    }

    private Texture2D CriarCeramica(Color a, Color b, int size = 64, int tiles = 4)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Ceramica_" + a + "_" + b,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat
        };
        int cell = size / tiles;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            bool rejunte = x % cell == 0 || y % cell == 0;
            bool alt = ((x / cell) + (y / cell)) % 2 == 0;
            Color c = rejunte ? Color.Lerp(a, b, .65f) * .58f : (alt ? a : b);
            tex.SetPixel(x, y, c);
        }
        tex.Apply();
        owned.Add(tex);
        return tex;
    }

    private Texture2D CriarTabuas(Color a, Color b, int size = 64, int planks = 4)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Tabuas_" + a,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat
        };
        int w = size / planks;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            int idx = x / w;
            bool junta = x % w == 0 || (y + idx * 19) % size == 0;
            float grao = Mathf.Sin((y + idx * 7) * .35f) * .04f;
            Color baseC = idx % 2 == 0 ? a : b;
            Color c = junta ? baseC * .52f : new Color(
                Mathf.Clamp01(baseC.r + grao),
                Mathf.Clamp01(baseC.g + grao),
                Mathf.Clamp01(baseC.b + grao));
            tex.SetPixel(x, y, c);
        }
        tex.Apply();
        owned.Add(tex);
        return tex;
    }

    private GameObject Caixa(string nome, Vector3 pos, Vector3 escala, Material mat,
        Transform pai = null, bool solida = true, string interactionId = null, string customPrompt = null)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nome;
        go.layer = RoomLayer;
        go.transform.SetParent(pai != null ? pai : world, false);
        go.transform.localPosition = pos;
        go.transform.localScale = escala;
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        if (mat != null && mat.shader != null && mat.shader.name.Contains("Unlit"))
            mr.shadowCastingMode = ShadowCastingMode.Off;
        var col = go.GetComponent<Collider>();
        if (!solida && string.IsNullOrEmpty(interactionId))
            Destroy(col);
        else if (!string.IsNullOrEmpty(interactionId))
        {
            if (!solida) col.isTrigger = true;
            RegistrarAlvo(col, interactionId, customPrompt);
        }
        return go;
    }

    private GameObject Cilindro(string nome, Vector3 pos, Vector3 escala, Material mat,
        Transform pai = null, bool solida = false, string interactionId = null, string customPrompt = null)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = nome;
        go.layer = RoomLayer;
        go.transform.SetParent(pai != null ? pai : world, false);
        go.transform.localPosition = pos;
        go.transform.localScale = escala;
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        if (mat != null && mat.shader != null && mat.shader.name.Contains("Unlit"))
            mr.shadowCastingMode = ShadowCastingMode.Off;
        var col = go.GetComponent<Collider>();
        if (!solida && string.IsNullOrEmpty(interactionId))
            Destroy(col);
        else if (!string.IsNullOrEmpty(interactionId))
        {
            if (!solida) col.isTrigger = true;
            RegistrarAlvo(col, interactionId, customPrompt);
        }
        return go;
    }

    private GameObject Esfera(string nome, Vector3 pos, Vector3 escala, Material mat, Transform pai = null)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = nome;
        go.layer = RoomLayer;
        go.transform.SetParent(pai != null ? pai : world, false);
        go.transform.localPosition = pos;
        go.transform.localScale = escala;
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        if (mat != null && mat.shader != null && mat.shader.name.Contains("Unlit"))
            mr.shadowCastingMode = ShadowCastingMode.Off;
        Destroy(go.GetComponent<Collider>());
        return go;
    }

    private Collider AlvoFisico(Transform pai, string nome, Vector3 pos, Vector3 tamanho,
        string interactionId = null, string customPrompt = null, Action aoAcionar = null)
    {
        var go = new GameObject(nome);
        go.layer = RoomLayer;
        go.transform.SetParent(pai != null ? pai : world, false);
        go.transform.localPosition = pos;
        var col = go.AddComponent<BoxCollider>();
        col.size = tamanho;
        col.isTrigger = true;
        if (!string.IsNullOrEmpty(interactionId) || aoAcionar != null)
            RegistrarAlvo(col, interactionId, customPrompt, aoAcionar);
        return col;
    }

    private void RegistrarAlvo(Collider col, string interactionId, string customPrompt = null, Action aoAcionar = null)
    {
        if (col == null) return;
        Capitulo2Interactable fonte = null;
        if (!string.IsNullOrEmpty(interactionId))
        {
            interactablesPorId.TryGetValue(interactionId, out fonte);
            if (!boundsPorId.ContainsKey(interactionId))
                boundsPorId[interactionId] = col.bounds;
        }
        alvos[col] = new AlvoInteracao
        {
            Id = interactionId,
            Prompt = customPrompt,
            Fonte = fonte,
            AoAcionar = aoAcionar
        };
    }

    private void RegistrarPortalSecundario(Collider col, string cenaDestino, string prompt)
    {
        if (col == null) return;
        foreach (var p in FindObjectsByType<Capitulo2Portal>(FindObjectsSortMode.None))
        {
            if (p.gameObject.scene != gameObject.scene) continue;
            if (p.cenaDestino == cenaDestino)
            {
                portais[col] = new AlvoPortal { Portal = p, Prompt = prompt };
                return;
            }
        }
    }

    private Light Luz(string nome, Vector3 pos, Color cor, float intensidade, float alcance, Transform pai = null)
    {
        var l = new GameObject(nome, typeof(Light)).GetComponent<Light>();
        l.gameObject.layer = RoomLayer;
        l.transform.SetParent(pai != null ? pai : world, false);
        l.transform.localPosition = pos;
        l.type = LightType.Point;
        l.color = cor;
        l.intensity = intensidade;
        l.range = alcance;
        l.shadows = LightShadows.None;
        l.cullingMask = RoomMask;
        return l;
    }

    private Light SpotLuz(string nome, Vector3 pos, Vector3 euler, Color cor, float intensidade, float alcance,
        float angulo = 55f, float inner = 25f, Transform pai = null,
        LightShadows shadows = LightShadows.None, int shadowTier = 0, float shadowBias = .02f, float shadowNormalBias = .03f)
    {
        var l = new GameObject(nome, typeof(Light)).GetComponent<Light>();
        l.gameObject.layer = RoomLayer;
        l.transform.SetParent(pai != null ? pai : world, false);
        l.transform.localPosition = pos;
        l.transform.localRotation = Quaternion.Euler(euler);
        l.type = LightType.Spot;
        l.color = cor;
        l.intensity = intensidade;
        l.range = alcance;
        l.spotAngle = angulo;
        l.innerSpotAngle = inner;
        l.shadows = shadows;
        l.cullingMask = RoomMask;
        if (shadows != LightShadows.None)
        {
            l.shadowStrength = .88f;
            l.shadowBias = shadowBias;
            l.shadowNormalBias = shadowNormalBias;
            l.shadowNearPlane = .12f;
            var data = l.gameObject.AddComponent<UniversalAdditionalLightData>();
            data.additionalLightsShadowResolutionTier = shadowTier;
        }
        return l;
    }

    private Transform Arte(string nome, string resource, Vector3 pos, float altura,
        float yaw = 0f, string interactionId = null, string customPrompt = null, Transform pai = null)
    {
        var sprite = Resources.Load<Sprite>(resource);
        if (sprite == null && !resource.StartsWith("Art/"))
            sprite = Resources.Load<Sprite>("Art/Props/" + resource) ?? Resources.Load<Sprite>("Art/World/" + resource);
        if (sprite == null) return null;

        var obj = new GameObject(nome, typeof(MeshFilter), typeof(MeshRenderer));
        obj.layer = RoomLayer;
        obj.transform.SetParent(pai != null ? pai : world, false);
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        float scale = altura / Mathf.Max(.05f, sprite.bounds.size.y);
        obj.transform.localScale = Vector3.one * scale;
        obj.GetComponent<MeshFilter>().sharedMesh = MeshFor(sprite);
        obj.GetComponent<MeshRenderer>().sharedMaterial = SpriteMaterial(sprite.texture);

        if (!string.IsNullOrEmpty(interactionId))
        {
            var col = obj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(
                Mathf.Max(.35f, sprite.bounds.size.x),
                Mathf.Max(.35f, sprite.bounds.size.y),
                .35f);
            RegistrarAlvo(col, interactionId, customPrompt);
        }
        return obj.transform;
    }

    private Transform Personagem(string nome, string resource, Vector3 pos, string interactionId,
        float altura = 1.68f, string customPrompt = null)
    {
        var sprite = Resources.Load<Sprite>(resource);
        if (sprite == null)
        {
            // Fallback: find matching sprite in scene by interactionId
            if (!string.IsNullOrEmpty(interactionId) && interactablesPorId.TryGetValue(interactionId, out var inter))
            {
                var sr = inter.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sprite = sr.sprite;
            }
        }

        Transform tr;
        if (sprite != null)
        {
            var obj = new GameObject(nome, typeof(MeshFilter), typeof(MeshRenderer));
            obj.layer = RoomLayer;
            obj.transform.SetParent(world, false);
            float scale = altura / Mathf.Max(.05f, sprite.bounds.size.y);
            obj.transform.localPosition = new Vector3(pos.x, pos.y - sprite.bounds.min.y * scale, pos.z);
            obj.transform.localScale = Vector3.one * scale;
            obj.GetComponent<MeshFilter>().sharedMesh = MeshFor(sprite);
            obj.GetComponent<MeshRenderer>().sharedMaterial = SpriteMaterial(sprite.texture);
            tr = obj.transform;
        }
        else
        {
            var fallback = Caixa(nome, pos + Vector3.up * .84f, new Vector3(.46f, 1.68f, .32f),
                Superficie("NpcFallback", new Color(.25f, .24f, .26f)), solida: false);
            tr = fallback.transform;
        }

        var col = tr.gameObject.AddComponent<CapsuleCollider>();
        col.isTrigger = false;
        col.center = new Vector3(0f, .5f, 0f);
        col.radius = .28f;
        col.height = 1.65f;
        if (!string.IsNullOrEmpty(interactionId))
            RegistrarAlvo(col, interactionId, customPrompt);

        personagens.Add(tr);
        return tr;
    }

    private void Letreiro(string nome, string texto, Vector3 pos, Material fundo,
        float largura, float altura, float yaw = 0f, Color? corTexto = null)
    {
        var placa = Caixa(nome, pos, new Vector3(largura, altura, .04f), fundo, solida: false);
        placa.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        var tmpGo = new GameObject(nome + "_Texto", typeof(TextMeshPro));
        tmpGo.layer = RoomLayer;
        tmpGo.transform.SetParent(placa.transform, false);
        tmpGo.transform.localPosition = new Vector3(0f, 0f, -.55f);
        tmpGo.transform.localScale = new Vector3(1f / Mathf.Max(.01f, largura), 1f / Mathf.Max(.01f, altura), 1f);
        var tmp = tmpGo.GetComponent<TextMeshPro>();
        tmp.text = texto;
        tmp.fontSize = 2.2f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = corTexto ?? new Color(.94f, .89f, .76f);
        tmp.rectTransform.sizeDelta = new Vector2(largura * .92f, altura * .88f);
    }

    private void MolduraVazada(Vector3 centro, float largura, float altura, Material mat, float espessura = .08f, float prof = .12f)
    {
        Caixa("MolduraSup", centro + Vector3.up * (altura * .5f + espessura * .5f), new Vector3(largura + espessura * 2f, espessura, prof), mat, solida: false);
        Caixa("MolduraInf", centro - Vector3.up * (altura * .5f + espessura * .5f), new Vector3(largura + espessura * 2f, espessura, prof), mat, solida: false);
        Caixa("MolduraEsq", centro - Vector3.right * (largura * .5f + espessura * .5f), new Vector3(espessura, altura, prof), mat, solida: false);
        Caixa("MolduraDir", centro + Vector3.right * (largura * .5f + espessura * .5f), new Vector3(espessura, altura, prof), mat, solida: false);
    }

    private void ParedeComJanela(float xMin, float xMax, float z, float altura, float espessura, Material mat, Rect vao)
    {
        ExteriorInterior3D.Parede(xMin, xMax, altura, new[] { vao }, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeTrecho", new Vector3((a + b) * .5f, (y0 + y1) * .5f, z), new Vector3(w, h, espessura), mat);
        });
    }

    private void ConstruirPorta(Material folhaMat, Material marcoMat, Material metalMat,
        float xCentro = 0f, float zSoleira = -4.5f, float largura = 1.06f, float altura = 2.18f)
    {
        var vidro = VidroExterior();

        Caixa("SoleiraPorta", new Vector3(xCentro, .015f, zSoleira), new Vector3(largura + .26f, .03f, .34f), marcoMat, solida: false);
        Caixa("BatenteEsq", new Vector3(xCentro - largura * .5f - .05f, altura * .5f, zSoleira), new Vector3(.10f, altura, .20f), marcoMat);
        Caixa("BatenteDir", new Vector3(xCentro + largura * .5f + .05f, altura * .5f, zSoleira), new Vector3(.10f, altura, .20f), marcoMat);
        Caixa("VergaPorta", new Vector3(xCentro, altura + .06f, zSoleira), new Vector3(largura + .20f, .12f, .20f), marcoMat);

        porta = Pivo("PortaEntradaPivo", world, new Vector3(xCentro - largura * .5f, 0f, zSoleira));
        var limite = new GameObject("ColisorPortaEntrada").AddComponent<BoxCollider>();
        limite.gameObject.layer = RoomLayer;
        limite.transform.SetParent(porta, false);
        limite.center = new Vector3(largura * .5f, altura * .5f, 0f);
        limite.size = new Vector3(largura, altura, .075f);
        saida = limite;

        float montante = .09f;
        float centroX = largura * .5f;
        float alturaMeio = .88f;
        float alturaVidro = altura - alturaMeio - .10f;
        float larguraVao = largura - montante * 2f;

        // Montantes laterais e travessas (inferior, meio, superior)
        Caixa("MontantePortaEsq", new Vector3(montante * .5f, altura * .5f, 0f), new Vector3(montante, altura, .07f), marcoMat, porta, solida: false);
        Caixa("MontantePortaDir", new Vector3(largura - montante * .5f, altura * .5f, 0f), new Vector3(montante, altura, .07f), marcoMat, porta, solida: false);
        Caixa("TravessaPortaInf", new Vector3(centroX, .07f, 0f), new Vector3(larguraVao, .14f, .07f), marcoMat, porta, solida: false);
        Caixa("TravessaPortaMeio", new Vector3(centroX, alturaMeio, 0f), new Vector3(larguraVao, .11f, .07f), marcoMat, porta, solida: false);
        Caixa("TravessaPortaSup", new Vector3(centroX, altura - .05f, 0f), new Vector3(larguraVao, .10f, .07f), marcoMat, porta, solida: false);

        // Almofada inferior com friso
        float hAlmofada = alturaMeio - .195f;
        Caixa("AlmofadaPortaInf", new Vector3(centroX, .14f + hAlmofada * .5f, 0f), new Vector3(larguraVao, hAlmofada, .042f), folhaMat, porta, solida: false);
        Caixa("FrisoPortaInf", new Vector3(centroX, .14f + hAlmofada * .5f, 0f), new Vector3(larguraVao * .84f, hAlmofada * .82f, .056f), marcoMat, porta, solida: false);

        // Vidraça superior com quadriculado para enxergar o interior iluminado antes da porta abrir
        float yVidro = alturaMeio + .055f + alturaVidro * .5f;
        var vidraca = GameObject.CreatePrimitive(PrimitiveType.Quad);
        vidraca.name = "VidroPortaEntrada";
        vidraca.layer = RoomLayer;
        vidraca.transform.SetParent(porta, false);
        vidraca.transform.localPosition = new Vector3(centroX, yVidro, 0f);
        vidraca.transform.localScale = new Vector3(larguraVao, alturaVidro, 1f);
        Destroy(vidraca.GetComponent<Collider>());
        vidraca.GetComponent<MeshRenderer>().sharedMaterial = vidro;

        Caixa("DivisaoVidroVert", new Vector3(centroX, yVidro, 0f), new Vector3(.028f, alturaVidro, .035f), marcoMat, porta, solida: false);
        Caixa("DivisaoVidroHoriz", new Vector3(centroX, yVidro, 0f), new Vector3(larguraVao, .028f, .035f), marcoMat, porta, solida: false);

        // Espelho e maçaneta de latão/metal
        Caixa("EspelhoMacaneta", new Vector3(largura - .08f, 1.00f, 0f), new Vector3(.048f, .19f, .085f), metalMat, porta, solida: false);
        var mac = Caixa("MacanetaPorta", new Vector3(largura - .12f, 1.02f, 0f), new Vector3(.13f, .032f, .16f), metalMat, porta, solida: false);
        macaneta = mac.transform;
    }

    private void ConstruirMao()
    {
        var tex = Resources.Load<Texture2D>("Art/Characters/MarcusMaoMacaneta");
        if (tex != null)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            obj.name = "MaoMarcus3D";
            obj.layer = RoomLayer;
            obj.transform.SetParent(world, false);
            obj.transform.localScale = new Vector3(.38f, .38f * tex.height / Mathf.Max(1, tex.width), 1f);
            Destroy(obj.GetComponent<Collider>());
            obj.GetComponent<MeshRenderer>().sharedMaterial = SpriteMaterial(tex);
            mao = obj.transform;
            mao.gameObject.SetActive(false);
            return;
        }

        var pele = Superficie("PeleMarcus", new Color(.74f, .57f, .46f));
        var manga = Superficie("MangaMarcus", new Color(.19f, .21f, .24f));
        mao = Pivo("MaoMarcus3D", world, Vector3.zero);
        Caixa("Antebraco", new Vector3(0f, 0f, -.16f), new Vector3(.085f, .075f, .26f), manga, mao, solida: false);
        Caixa("Palma", new Vector3(0f, 0f, 0f), new Vector3(.08f, .04f, .10f), pele, mao, solida: false);
        Caixa("Dedos", new Vector3(0f, -.01f, .07f), new Vector3(.075f, .035f, .06f), pele, mao, solida: false);
        mao.gameObject.SetActive(false);
    }

    private void ConstruirChuva(float z = -5.2f)
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/ChuvaEntradaBar");
        if (shader == null || !shader.isSupported) return;

        materialChuva = new Material(shader);
        materialChuva.SetVector("_Luz", new Vector4(.35f, 2.25f, soleiraZ - .95f, 0f));
        owned.Add(materialChuva);

        const int quantidade = 430;
        var vertices = new Vector3[quantidade * 4];
        var cantos = new Vector2[vertices.Length];
        var parametros = new List<Vector4>(vertices.Length);
        var indices = new int[quantidade * 6];
        var random = new System.Random(317);
        float Sortear(float min, float max) => Mathf.Lerp(min, max, (float)random.NextDouble());

        for (int i = 0; i < quantidade; i++)
        {
            var origem = V(Sortear(-2.40f, 2.40f), Sortear(0f, 1f), soleiraZ - Sortear(.35f, 2.05f));
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

        var malha = new Mesh { name = "ChuvaEntradaC2" };
        malha.vertices = vertices;
        malha.uv = cantos;
        malha.SetUVs(1, parametros);
        malha.triangles = indices;
        malha.bounds = new Bounds(V(0f, 1.9f, soleiraZ - 1.18f), V(5f, 3.9f, 2.4f));
        owned.Add(malha);

        var objeto = new GameObject("ChuvaEntrada3D", typeof(MeshFilter), typeof(MeshRenderer));
        objeto.layer = RoomLayer;
        objeto.transform.SetParent(world, false);
        objeto.GetComponent<MeshFilter>().sharedMesh = malha;
        chuva3D = objeto.GetComponent<MeshRenderer>();
        chuva3D.sharedMaterial = materialChuva;
        chuva3D.shadowCastingMode = ShadowCastingMode.Off;
        chuva3D.enabled = false;
    }

    private Mesh MeshFor(Sprite sprite)
    {
        if (spriteMeshes.TryGetValue(sprite, out var mesh)) return mesh;
        var points = sprite.vertices;
        var vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) vertices[i] = new Vector3(points[i].x, points[i].y, 0f);
        mesh = new Mesh { name = sprite.name, vertices = vertices, uv = sprite.uv };
        mesh.triangles = Array.ConvertAll(sprite.triangles, x => (int)x);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        spriteMeshes[sprite] = mesh;
        owned.Add(mesh);
        return mesh;
    }

    private Material SpriteMaterial(Texture2D texture)
    {
        if (spriteMaterials.TryGetValue(texture, out var material)) return material;
        var salaLit = Resources.Load<Shader>("Art/Sinuca/SalaLit");
        if (salaLit != null && salaLit.isSupported)
        {
            material = new Material(salaLit);
            material.SetColor("_BaseColor", Color.white);
            material.SetTexture("_BaseMap", texture);
            material.SetFloat("_RoomSurface", 0f);
            material.SetFloat("_AlphaClip", 1f);
            material.SetFloat("_Cutoff", .55f);
            material.SetFloat("_Cull", 0f);
            material.SetFloat("_ActorLighting", 1f);
            material.SetFloat("_Smoothness", .02f);
            material.SetColor("_AmbientColor", new Color(.014f, .016f, .020f, 1f));
            material.SetColor("_HazeColor", new Color(.003f, .004f, .006f, 1f));
            material.SetFloat("_HazeStrength", .10f);
            material.SetFloat("_FilmicTonemap", 1f);
        }
        else
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetTexture("_BaseMap", texture);
            material.SetFloat("_AlphaClip", 1f);
            material.SetFloat("_Cull", 0f);
            material.SetFloat("_Cutoff", .55f);
            material.SetFloat("_Smoothness", .05f);
            material.EnableKeyword("_ALPHATEST_ON");
        }
        spriteMaterials[texture] = material;
        owned.Add(material);
        return material;
    }

    private Material BulboInstancia(string nome, Color cor, float hdr = 3.2f)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
        {
            name = nome,
            color = cor
        };
        if (hdr > 1.01f)
            mat.SetColor("_BaseColor", new Color(cor.r * hdr, cor.g * hdr, cor.b * hdr, cor.a));
        owned.Add(mat);
        return mat;
    }

    private Material HaloMaterial(string nome, Color cor, float intensidade = .55f, float falloff = 5.5f, float core = .45f)
    {
        var sh = Resources.Load<Shader>("Art/Sinuca/HaloLuz");
        var mat = new Material(sh != null && sh.isSupported ? sh : Shader.Find("Universal Render Pipeline/Unlit"))
        {
            name = nome
        };
        mat.SetColor("_Color", cor);
        mat.SetFloat("_Intensity", intensidade);
        mat.SetFloat("_Falloff", falloff);
        mat.SetFloat("_Core", core);
        owned.Add(mat);
        return mat;
    }

    private GameObject HaloLuz(string nome, Vector3 pos, Color cor, float tamanho, float intensidade = .55f,
        Transform pai = null, float falloff = 5.5f, float core = .45f, Material matOverride = null)
    {
        var sh = Resources.Load<Shader>("Art/Sinuca/HaloLuz");
        if (sh == null || !sh.isSupported) return null;

        var mat = matOverride;
        if (mat == null)
        {
            string chave = $"Halo_{cor}_{intensidade}_{falloff}_{core}";
            if (!surfaces.TryGetValue(chave, out mat))
            {
                mat = HaloMaterial(nome + "_Mat", cor, intensidade, falloff, core);
                surfaces[chave] = mat;
            }
        }

        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = nome;
        go.layer = RoomLayer;
        go.transform.SetParent(pai != null ? pai : world, false);
        go.transform.localPosition = pos;
        go.transform.localScale = new Vector3(tamanho, tamanho, 1f);
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.lightProbeUsage = LightProbeUsage.Off;
        Destroy(go.GetComponent<Collider>());
        return go;
    }

    private SantaRitaInteriorAudio SomLocal(string efeito, Vector3 posLocal, float alcance, float ganho)
    {
        if (vista == null) return null;
        var origem = Pivo("Som_" + efeito, world, posLocal);
        return SantaRitaInteriorAudio.Criar(world, origem, vista.transform, efeito, alcance, ganho);
    }

    private void LuzOscilante(Light luz, Material emissor, float intensidadeBase, Color corAcesa, Color corApagada,
        float intensidadeMinFator = .08f, bool somFaisca = true, float ganhoSom = .22f, float semente = 0f,
        Light luzSecundaria = null, float intensidadeSecundaria = 0f,
        float emissaoHdr = 3.2f, Material haloEmissor = null, float haloIntensidadeBase = .48f)
    {
        if (luz == null) return;
        float t = semente * 1.73f;
        float proximoSurto = 2.4f + Mathf.Abs(Mathf.Sin(semente * 3.1f + .7f)) * 3.6f;
        float fimSurto = 0f;
        float ultimoSom = -1f;
        bool estavaApagada = false;

        RegistrarAnimacao(dt =>
        {
            t += dt;
            if (t >= proximoSurto)
            {
                fimSurto = t + .28f + Mathf.Abs(Mathf.Sin(t * 2.7f + semente)) * .62f;
                proximoSurto = fimSurto + 2.8f + Mathf.Abs(Mathf.Cos(t * 1.9f + semente)) * 5.2f;
            }

            float zumbido = .94f + Mathf.Sin(t * 31f + semente) * .035f + Mathf.Sin(t * 7.3f + semente) * .025f;
            float fator = zumbido;
            bool apagada = false;

            if (t < fimSurto)
            {
                float onda = Mathf.Sin(t * 43f + semente * 4f) * Mathf.Sin(t * 19f + semente * 2f);
                if (onda > .08f)
                {
                    fator = intensidadeMinFator;
                    apagada = true;
                }
                else
                {
                    fator = Mathf.Lerp(intensidadeMinFator, 1.08f, Mathf.Abs(onda));
                }
            }

            luz.intensity = intensidadeBase * fator;
            if (luzSecundaria != null)
                luzSecundaria.intensity = intensidadeSecundaria * fator;
            float norm = Mathf.InverseLerp(intensidadeMinFator, 1f, fator);
            if (emissor != null)
            {
                var hdrAcesa = new Color(corAcesa.r * emissaoHdr, corAcesa.g * emissaoHdr, corAcesa.b * emissaoHdr, corAcesa.a);
                emissor.SetColor("_BaseColor", Color.Lerp(corApagada, hdrAcesa, norm));
            }
            if (haloEmissor != null)
                haloEmissor.SetFloat("_Intensity", haloIntensidadeBase * Mathf.Lerp(.04f, 1f, norm));

            if (somFaisca && !modoPreview && apagada != estavaApagada && vista != null && t - ultimoSom > .19f)
            {
                ultimoSom = t;
                SantaRitaInteriorAudio.Tocar("Interruptor", luz.transform.position, vista.transform,
                    ganhoSom, UnityEngine.Random.Range(.84f, 1.18f));
            }
            estavaApagada = apagada;
        });
    }
}
