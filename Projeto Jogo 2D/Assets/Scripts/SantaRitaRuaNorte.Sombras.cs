using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class SantaRitaRuaNorte
{
    private RenderTexture sombrasDosPostes;
    private RenderTexture sombraDaLanterna;
    private Material materialSombraLanterna;
    private Mesh malhaSombraLanterna;
    private MaterialPropertyBlock propsSombraLanterna;
    private SpriteRenderer[] renderersDaZona;
    private Vector4[] luzesDeRecorte;

    private void ProjetarSombrasDaRua()
    {
        if (sombrasDosPostes != null && sombrasDosPostes.IsCreated()) return;
        var shader = Resources.Load<Shader>(Art + "Entrada/ProjetarRecorte");
        if (shader == null || !shader.isSupported)
        {
            luzesDeRecorte = null;
            lighting.SetFloat("_ArrivalShadowEnabled", 0);
            return;
        }

        const int ppu = 32;
        if (sombrasDosPostes == null)
        {
            sombrasDosPostes = new RenderTexture(Mathf.RoundToInt(size.x * ppu),
                Mathf.RoundToInt(size.y * ppu), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
            {
                name = "Sombras projetadas de " + name,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
        }
        if (!sombrasDosPostes.Create())
        {
            lighting.SetFloat("_ArrivalShadowEnabled", 0);
            return;
        }

        var material = new Material(shader);
        var quad = CriarMalhaDeProjecao();
        var props = new MaterialPropertyBlock();
        props.SetVector("_ShadowArea", new Vector4(size.x, size.y, 0, 0));
        renderersDaZona = GetComponentsInChildren<SpriteRenderer>();
        using (var pass = new CommandBuffer { name = "Projetar silhuetas sobre o piso" })
        {
            pass.SetRenderTarget(sombrasDosPostes);
            pass.SetViewport(new Rect(0, 0, sombrasDosPostes.width, sombrasDosPostes.height));
            pass.ClearRenderTarget(false, true, Color.clear);
            for (int i = 0; i < luzesDeRecorte.Length; i++)
            {
                var lamp = luzesDeRecorte[i];
                var channel = Vector4.zero;
                channel[i] = 1;
                props.SetVector("_ShadowLamp", lamp);
                props.SetVector("_ShadowChannel", channel);
                DesenharSilhuetasDaLuz(pass, quad, material, props, renderersDaZona, lamp);
            }
            Graphics.ExecuteCommandBuffer(pass);
        }
        Destroy(quad);
        Destroy(material);
        lighting.SetTexture("_ArrivalShadows", sombrasDosPostes);
        lighting.SetVector("_ArrivalShadowSize", size);
        lighting.SetFloat("_ArrivalShadowEnabled", 1);
    }

    private void ProjetarSombraDaLanterna(Vector4 lamp)
    {
        var shader = Resources.Load<Shader>(Art + "Entrada/ProjetarRecorte");
        if (shader == null || !shader.isSupported)
        {
            lighting.SetFloat("_PlayerShadowEnabled", 0);
            return;
        }

        const int ppu = 32;
        if (sombraDaLanterna == null)
        {
            sombraDaLanterna = new RenderTexture(Mathf.RoundToInt(size.x * ppu),
                Mathf.RoundToInt(size.y * ppu), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
            {
                name = "Sombra da lanterna em " + name,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
        }
        if (!sombraDaLanterna.IsCreated() && !sombraDaLanterna.Create())
        {
            lighting.SetFloat("_PlayerShadowEnabled", 0);
            return;
        }

        if (materialSombraLanterna == null) materialSombraLanterna = new Material(shader);
        if (malhaSombraLanterna == null) malhaSombraLanterna = CriarMalhaDeProjecao();
        propsSombraLanterna ??= new MaterialPropertyBlock();
        renderersDaZona ??= GetComponentsInChildren<SpriteRenderer>();

        // Usa altura efetiva compatível com o projetor de silhuetas para projetar sombras nítidas como os postes
        Vector4 shadowLamp = new Vector4(lamp.x, lamp.y, Mathf.Max(2.45f, lamp.z), lamp.w);
        propsSombraLanterna.SetVector("_ShadowArea", new Vector4(size.x, size.y, 0, 0));
        propsSombraLanterna.SetVector("_ShadowLamp", shadowLamp);
        propsSombraLanterna.SetVector("_ShadowChannel", new Vector4(1, 0, 0, 0));

        using (var pass = new CommandBuffer { name = "Projetar silhuetas da lanterna" })
        {
            pass.SetRenderTarget(sombraDaLanterna);
            pass.SetViewport(new Rect(0, 0, sombraDaLanterna.width, sombraDaLanterna.height));
            pass.ClearRenderTarget(false, true, Color.clear);
            DesenharSilhuetasDaLuz(pass, malhaSombraLanterna, materialSombraLanterna, propsSombraLanterna, renderersDaZona, shadowLamp);
            Graphics.ExecuteCommandBuffer(pass);
        }
        lighting.SetTexture("_PlayerShadows", sombraDaLanterna);
        lighting.SetVector("_ArrivalShadowSize", size);
        lighting.SetFloat("_PlayerShadowEnabled", 1);
    }

    private void DesativarSombraDaLanterna()
    {
        if (lighting != null) lighting.SetFloat("_PlayerShadowEnabled", 0);
    }

    private void LiberarRecursosSombraLanterna()
    {
        if (sombraDaLanterna != null)
        {
            sombraDaLanterna.Release();
            Destroy(sombraDaLanterna);
            sombraDaLanterna = null;
        }
        if (materialSombraLanterna != null)
        {
            Destroy(materialSombraLanterna);
            materialSombraLanterna = null;
        }
        if (malhaSombraLanterna != null)
        {
            Destroy(malhaSombraLanterna);
            malhaSombraLanterna = null;
        }
    }

    private void DesenharSilhuetasDaLuz(CommandBuffer pass, Mesh quad, Material material,
        MaterialPropertyBlock props, SpriteRenderer[] renderers, Vector4 lamp)
    {
        foreach (var renderer in renderers)
        {
            if (renderer == null || renderer == player) continue;
            var sprite = renderer.sprite;
            if (!renderer.enabled || renderer.sharedMaterial != lighting || sprite == null || sprite.packed) continue;
            renderer.GetPropertyBlock(receiverProperties);
            var receiver = receiverProperties.GetVector("_Receiver");
            if (receiver.y <= 0 || receiver.w > 0) continue;

            // Os carros já projetam a carroceria e a cabine como volumes separados.
            int id = (int)receiver.z - 1;
            if (id >= 0 && casterMin[id].w == (float)Sombra.Veiculo) continue;
            float depth = id >= 0 ? Mathf.Clamp((casterMax[id].y - casterMin[id].y) * .4f, .08f, .42f) : .17f;
            float density = receiverProperties.GetVector("_Wind").x > 0 ? .66f : 1f;
            DesenharSilhuetaDaLuz(pass, quad, material, props, renderer, origin, lamp, receiver.x,
                receiver.y, depth, density);
        }
    }

    internal static void DesenharSilhuetaDaLuz(CommandBuffer pass, Mesh quad, Material material,
        MaterialPropertyBlock props, SpriteRenderer renderer, Vector2 origin, Vector4 lamp,
        float baseY, float inclinacao, float depth, float density)
    {
        var bounds = renderer.bounds;
        float foot = baseY - origin.y;
        float left = bounds.min.x - origin.x;
        float right = bounds.max.x - origin.x;
        float nearestX = Mathf.Clamp(lamp.x, left, right);
        if (new Vector2(nearestX - lamp.x, foot - lamp.y).sqrMagnitude > lamp.w * lamp.w) return;

        var sprite = renderer.sprite;
        var texture = sprite.texture;
        var rect = sprite.rect;
        bool flip = renderer.flipX ^ (renderer.transform.lossyScale.x < 0);
        float height = Mathf.Max(.01f, (bounds.max.y - baseY) * inclinacao);
        props.SetTexture("_ShadowSprite", texture);
        props.SetVector("_ShadowUV", new Vector4((flip ? rect.xMax : rect.xMin) / texture.width,
            rect.yMin / texture.height, (flip ? -rect.width : rect.width) / texture.width, rect.height / texture.height));
        props.SetVector("_ShadowFoot", new Vector4(left, right, foot, depth));
        props.SetVector("_ShadowHeight", new Vector4(height, density, 0, 0));
        pass.DrawMesh(quad, Matrix4x4.identity, material, 0, 0, props);
    }

    internal static Mesh CriarMalhaDeProjecao()
    {
        // A subdivisão vertical mantém a projeção estável perto da altura da lâmpada.
        const int rows = 12;
        var points = new Vector3[(rows + 1) * 2];
        var triangles = new int[rows * 6];
        for (int row = 0; row <= rows; row++)
        {
            float y = row / (float)rows;
            int first = row * 2;
            points[first] = new Vector3(0, y, 0);
            points[first + 1] = new Vector3(1, y, 0);
            if (row == rows) break;
            int index = row * 6;
            triangles[index] = first;
            triangles[index + 1] = first + 2;
            triangles[index + 2] = first + 1;
            triangles[index + 3] = first + 1;
            triangles[index + 4] = first + 2;
            triangles[index + 5] = first + 3;
        }
        return new Mesh { name = "Plano para projeção de sombras", vertices = points, triangles = triangles };
    }
}
