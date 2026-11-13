using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

internal sealed partial class Capitulo2Interior3D
{
    private struct CascaSombra
    {
        public Bounds Box;
        public float Opacidade;
    }

    private void PreAssarSombras(string scene)
    {
        if (world == null) return;
        Physics.SyncTransforms();

        // 1. Descobre limites do piso, pé-direito e obstáculos estáticos (sombras direcionais vêm dos shadow maps URP)
        bool temPiso = false;
        Bounds limitesPiso = default;
        float tetoY = 3.2f;
        var cascata = new List<CascaSombra>(128);
        var mesasTopos = new List<Bounds>(24);
        var matsSombra = new List<Material>();

        foreach (var r in world.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (r == null) continue;
            string n = r.name;
            if (IsDescendenteExterior(r.transform)) continue;

            var b = r.bounds;
            b.center -= world.position;

            if (r.sharedMaterial != null && r.sharedMaterial.shader != null &&
                r.sharedMaterial.shader.name == "SantaRita/Sinuca/SalaLit" &&
                !matsSombra.Contains(r.sharedMaterial))
                matsSombra.Add(r.sharedMaterial);

            if (n.StartsWith("Piso") || n.StartsWith("Passadeira") || n == "PisoPresbiterio")
            {
                if (b.center.y < .22f)
                {
                    if (!temPiso) { limitesPiso = b; temPiso = true; }
                    else limitesPiso.Encapsulate(b);
                }
                continue;
            }

            if (n.StartsWith("Teto") || n.StartsWith("Forro") || n.StartsWith("Cumeeira") ||
                n.StartsWith("Viga") || n.StartsWith("Caibro") || n.StartsWith("Trelica") ||
                n.StartsWith("Terca") || n.StartsWith("Empena") || n.StartsWith("Escora"))
            {
                if (b.min.y > tetoY) tetoY = b.min.y;
                continue;
            }

            if (n.Contains("Vidro") || n.Contains("Bulbo") || n.Contains("Chama") ||
                n.Contains("Lampada") || n.Contains("Dial") || n.Contains("Vitral") ||
                n.StartsWith("Capacho") || n.StartsWith("Tapete") || n.StartsWith("Friso") ||
                n.StartsWith("Rodape") || n.StartsWith("Umidade") || n.StartsWith("TuboFluorescente"))
                continue;

            if (b.max.y > tetoY && n.StartsWith("Parede")) tetoY = b.max.y;

            if (b.size.y > .45f && b.size.y < 1.15f && b.min.y < .25f &&
                b.size.x > .35f && b.size.z > .35f && !n.StartsWith("Parede"))
                mesasTopos.Add(b);

            if (b.size.y < .08f || b.min.y > 2.4f) continue;
            float area = b.size.x * b.size.z;
            if (area < .012f && b.size.y < .5f) continue;

            float op = n.StartsWith("Parede") ? .82f : (b.min.y < .38f ? .76f : .56f);
            cascata.Add(new CascaSombra { Box = b, Opacidade = op });
        }

        // Atualiza altura do sombreamento de teto no SalaLit para combinar com o pé-direito da sala
        float alturaFade = scene == "IgrejaMatriz" ? 9.6f : Mathf.Clamp(tetoY, 2.95f, 10.5f);
        for (int i = 0; i < matsSombra.Count; i++)
            if (matsSombra[i].HasProperty("_CeilingFade"))
                matsSombra[i].SetFloat("_CeilingFade", alturaFade);

        // NPCs também projetam sombra suave no mapa pré-assado e sob os pés
        foreach (var p in personagens)
        {
            if (p == null) continue;
            var lp = world.InverseTransformPoint(p.position);
            cascata.Add(new CascaSombra
            {
                Box = new Bounds(new Vector3(lp.x, .82f, lp.z), new Vector3(.42f, 1.64f, .28f)),
                Opacidade = .70f
            });
            CriarContatoRadial("SombraNPC_" + p.name, new Vector3(lp.x, .005f, lp.z), new Vector2(.82f, .52f), .45f);
        }

        if (!temPiso) return;

        float x0 = limitesPiso.min.x, x1 = limitesPiso.max.x;
        float z0 = limitesPiso.min.z, z1 = limitesPiso.max.z;
        float larg = Mathf.Max(1f, x1 - x0);
        float prof = Mathf.Max(1f, z1 - z0);

        const int res = 96;
        var mapa = new float[res * res];

        // 2. Passo A: Oclusão de cantos junto às paredes perimetrais
        for (int iz = 0; iz < res; iz++)
        {
            float wz = z0 + (iz + .5f) / res * prof;
            float dBordaZ = Mathf.Min(wz - z0, z1 - wz);
            float aoZ = dBordaZ < .75f ? Mathf.Pow(1f - dBordaZ / .75f, 1.75f) * .48f : 0f;

            for (int ix = 0; ix < res; ix++)
            {
                float wx = x0 + (ix + .5f) / res * larg;
                float dBordaX = Mathf.Min(wx - x0, x1 - wx);
                float aoX = dBordaX < .75f ? Mathf.Pow(1f - dBordaX / .75f, 1.75f) * .48f : 0f;
                mapa[iz * res + ix] = Mathf.Clamp01(aoX + aoZ - aoX * aoZ * .45f);
            }
        }

        // 3. Passo B: Oclusão de contato pré-assada de cada móvel, banco, pilar e parede
        const float margemAO = .38f;
        for (int c = 0; c < cascata.Count; c++)
        {
            var b = cascata[c].Box;
            if (b.min.y > .45f) continue;

            int uMin = Mathf.Max(0, Mathf.FloorToInt((b.min.x - margemAO - x0) / larg * res));
            int uMax = Mathf.Min(res - 1, Mathf.CeilToInt((b.max.x + margemAO - x0) / larg * res));
            int vMin = Mathf.Max(0, Mathf.FloorToInt((b.min.z - margemAO - z0) / prof * res));
            int vMax = Mathf.Min(res - 1, Mathf.CeilToInt((b.max.z + margemAO - z0) / prof * res));
            float opBase = cascata[c].Opacidade;

            for (int iz = vMin; iz <= vMax; iz++)
            {
                float wz = z0 + (iz + .5f) / res * prof;
                float dz = wz < b.min.z ? b.min.z - wz : (wz > b.max.z ? wz - b.max.z : 0f);
                for (int ix = uMin; ix <= uMax; ix++)
                {
                    float wx = x0 + (ix + .5f) / res * larg;
                    float dx = wx < b.min.x ? b.min.x - wx : (wx > b.max.x ? wx - b.max.x : 0f);
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);
                    if (dist >= margemAO) continue;
                    float k = 1f - dist / margemAO;
                    float ao = k * k * opBase;
                    int idx = iz * res + ix;
                    if (ao > mapa[idx]) mapa[idx] = ao;
                }
            }
        }

        // 4. Suavização separável em 2 passes para oclusão de contato suave e gravação na Texture2D RGBA32
        var buf = new float[res * res];
        for (int p = 0; p < 2; p++)
        {
            for (int iz = 0; iz < res; iz++)
            for (int ix = 0; ix < res; ix++)
            {
                float c = mapa[iz * res + ix] * 2f;
                float lVal = mapa[iz * res + Mathf.Max(0, ix - 1)];
                float rVal = mapa[iz * res + Mathf.Min(res - 1, ix + 1)];
                buf[iz * res + ix] = (lVal + c + rVal) * .25f;
            }
            for (int iz = 0; iz < res; iz++)
            for (int ix = 0; ix < res; ix++)
            {
                float c = buf[iz * res + ix] * 2f;
                float dVal = buf[Mathf.Max(0, iz - 1) * res + ix];
                float uVal = buf[Mathf.Min(res - 1, iz + 1) * res + ix];
                mapa[iz * res + ix] = (dVal + c + uVal) * .25f;
            }
        }

        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false)
        {
            name = "MapaSombraChao_" + scene,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        var pixels = new Color32[res * res];
        for (int i = 0; i < pixels.Length; i++)
        {
            byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(mapa[i] * 240f), 0, 240);
            pixels[i] = new Color32(16, 16, 16, a);
        }
        tex.SetPixels32(pixels);
        tex.Apply(false, true);
        owned.Add(tex);

        var sh = Resources.Load<Shader>("Art/Sinuca/SombraPreAssada");
        if (sh != null)
        {
            var matMapa = new Material(sh) { name = "MatSombraAssada_" + scene };
            matMapa.SetTexture("_BaseMap", tex);
            matMapa.SetFloat("_Strength", .30f);
            matMapa.SetFloat("_Radial", 0f);
            owned.Add(matMapa);

            CriarQuadSombra("SombraPreAssadaChao", new Vector3((x0 + x1) * .5f, .0035f, (z0 + z1) * .5f),
                new Vector2(larg, prof), matMapa);

            if (scene == "IgrejaMatriz")
            {
                const float px0 = -4.6f, px1 = 4.6f, pz0 = 10.05f, pz1 = 13.55f;
                var matPresb = new Material(matMapa) { name = "MatSombraAssada_Presbiterio" };
                matPresb.SetTextureOffset("_BaseMap", new Vector2((px0 - x0) / larg, (pz0 - z0) / prof));
                matPresb.SetTextureScale("_BaseMap", new Vector2((px1 - px0) / larg, (pz1 - pz0) / prof));
                owned.Add(matPresb);
                CriarQuadSombra("SombraPreAssadaPresbiterio", new Vector3((px0 + px1) * .5f, .242f, (pz0 + pz1) * .5f),
                    new Vector2(px1 - px0, pz1 - pz0), matPresb);
            }
        }

        // 6. Contatos leves em objetos sobre mesas/bancadas/altares + projeções de janelas/vitrais
        PreAssarContatosSuperficies(mesasTopos);
        ProjetarLuzesJanelas(scene);
    }

    private void PreAssarContatosSuperficies(List<Bounds> superficies)
    {
        if (superficies.Count == 0) return;
        foreach (var r in world.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (r == null || IsDescendenteExterior(r.transform)) continue;
            string n = r.name;
            if (n.Contains("Folha") || n.Contains("Linha") || n.Contains("Tampo") ||
                n.Contains("Toalha") || n.Contains("Etiqueta") || n.Contains("Chama") ||
                n.Contains("Bulbo") || n.Contains("Tapete") || n.Contains("Ponteiro"))
                continue;

            var b = r.bounds;
            b.center -= world.position;
            if (b.min.y < .38f || b.min.y > 1.35f) continue;
            if (b.size.x > .85f || b.size.z > .85f || b.size.x < .04f || b.size.z < .04f) continue;

            for (int s = 0; s < superficies.Count; s++)
            {
                var sup = superficies[s];
                if (Mathf.Abs(b.min.y - sup.max.y) > .08f) continue;
                if (b.center.x < sup.min.x || b.center.x > sup.max.x ||
                    b.center.z < sup.min.z || b.center.z > sup.max.z) continue;

                CriarContatoRadial("SombraProp_" + n,
                    new Vector3(b.center.x, sup.max.y + .0025f, b.center.z),
                    new Vector2(b.size.x * 1.45f, b.size.z * 1.45f), .46f);
                break;
            }
        }
    }

    private void ProjetarLuzesJanelas(string scene)
    {
        switch (scene)
        {
            case "AbrigoCemiterio":
                CriarProjecaoJanela("ProjGradeCemiterio", new Vector3(0f, .005f, 2.75f), 0f, 2.15f, 3.3f,
                    new Color(.36f, .52f, .80f, .24f), bars: 5f, crossH: true, crossV: false);
                break;
            case "MercadinhoDeposito":
                foreach (float xj in new[] { -2.5f, 2.5f })
                    CriarProjecaoJanela("ProjGradeDeposito_" + xj, new Vector3(xj, .005f, -2.95f), 180f, 2.1f, 2.7f,
                        new Color(.34f, .48f, .72f, .18f), bars: 5f, crossH: false, crossV: false);
                break;
            case "PrefeituraSantaRita":
                foreach (float xj in new[] { -2.4f, 2.4f })
                    CriarProjecaoJanela("ProjJanelaPref_" + xj, new Vector3(xj, .005f, 7.85f), 0f, 1.85f, 2.9f,
                        new Color(.36f, .52f, .80f, .18f), crossH: true, crossV: true);
                break;
            case "OficinaRadio":
                CriarProjecaoJanela("ProjJanelaOficina", new Vector3(-2.70f, .005f, -2.85f), 180f, 2.2f, 2.7f,
                    new Color(.36f, .50f, .76f, .17f), crossH: true, crossV: true);
                break;
            case "CasaCostureira":
                CriarProjecaoJanela("ProjJanelaCostureira", new Vector3(-2.90f, .005f, -2.95f), 180f, 1.9f, 2.5f,
                    new Color(.36f, .50f, .78f, .16f), crossH: true, crossV: true);
                break;
            case "RodoviariaSantaRita":
                foreach (float xj in new[] { -3.5f, 3.5f })
                    CriarProjecaoJanela("ProjJanelaRodo_" + xj, new Vector3(xj, .005f, -2.85f), 180f, 2.3f, 2.7f,
                        new Color(.36f, .52f, .78f, .17f), crossH: true, crossV: true);
                CriarProjecaoJanela("ProjPlatRodo", new Vector3(-2.2f, .005f, 5.85f), 0f, 2.9f, 2.8f,
                    new Color(.34f, .52f, .78f, .16f), crossH: false, crossV: true);
                break;
            case "SociedadeBeiraRio":
                foreach (float xj in new[] { -3.5f, 4.1f, 9.7f })
                    CriarProjecaoJanela("ProjJanelaSoc_" + xj, new Vector3(xj, .005f, -2.95f), 180f, 2.0f, 2.6f,
                        new Color(.34f, .48f, .74f, .15f), crossH: true, crossV: true);
                break;
        }
    }

    private void CriarProjecaoJanela(string nome, Vector3 pos, float yawGraus, float larg, float comp, Color cor,
        float bars = 0f, bool crossH = true, bool crossV = true)
    {
        var sh = Resources.Load<Shader>("Art/Sinuca/ProjecaoLuzSala");
        if (sh == null) return;
        var mat = new Material(sh) { name = "Mat_" + nome };
        mat.SetColor("_BaseColor", cor);
        mat.SetFloat("_Bars", bars);
        mat.SetFloat("_CrossH", crossH ? 1f : 0f);
        mat.SetFloat("_CrossV", crossV ? 1f : 0f);
        owned.Add(mat);

        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = nome;
        quad.layer = RoomLayer;
        Destroy(quad.GetComponent<Collider>());
        quad.transform.SetParent(world, false);
        quad.transform.localPosition = pos;
        quad.transform.localRotation = Quaternion.Euler(90f, yawGraus, 0f);
        quad.transform.localScale = new Vector3(larg, comp, 1f);
        var mr = quad.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
    }

    private void CriarContatoRadial(string nome, Vector3 pos, Vector2 tamanho, float opacidade)
    {
        var sh = Resources.Load<Shader>("Art/Sinuca/SombraPreAssada");
        if (sh == null) return;
        string chave = "ContatoRad_" + opacidade.ToString("F2");
        if (!surfaces.TryGetValue(chave, out var mat))
        {
            mat = new Material(sh) { name = chave };
            mat.SetFloat("_Strength", opacidade);
            mat.SetFloat("_Radial", 1f);
            surfaces[chave] = mat;
            owned.Add(mat);
        }
        CriarQuadSombra(nome, pos, tamanho, mat);
    }

    private void CriarQuadSombra(string nome, Vector3 pos, Vector2 tamanho, Material mat)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = nome;
        quad.layer = RoomLayer;
        Destroy(quad.GetComponent<Collider>());
        quad.transform.SetParent(world, false);
        quad.transform.localPosition = pos;
        quad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        quad.transform.localScale = new Vector3(tamanho.x, tamanho.y, 1f);
        var mr = quad.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
    }

    private bool IsDescendenteExterior(Transform t)
    {
        while (t != null && t != world)
        {
            if (t.name.StartsWith("Exterior ")) return true;
            t = t.parent;
        }
        return false;
    }
}
