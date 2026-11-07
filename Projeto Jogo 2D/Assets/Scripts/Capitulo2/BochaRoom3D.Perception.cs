using System;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class BochaRoom3D
{
    private static readonly int Abertura = Shader.PropertyToID("_BochaAbertura");
    private GameObject figura;
    private Transform interiorBuraco;
    private int idQueda = -1;
    private Vector3 centroQueda;
    private float fundoQueda;

    private void CriarFigura()
    {
        var sprite = Resources.Load<Sprite>("Art/Characters/Npc_Frequentador_Sentado");
        if (sprite == null) return;

        figura = new GameObject("Alucinacao_FiguraSentada") { layer = Layer };
        figura.transform.SetParent(root.transform, false);
        figura.transform.localPosition = new Vector3(-4.20f, .03f, 5.05f);
        figura.transform.localRotation = Quaternion.Euler(0, -56, 0);

        var pontos = sprite.vertices;
        var vertices = new Vector3[pontos.Length];
        float baseY = sprite.bounds.min.y;
        float escala = 1.40f / sprite.bounds.size.y;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = new Vector3(pontos[i].x * escala, (pontos[i].y - baseY) * escala, 0);

        var mesh = new Mesh
        {
            name = "Bocha_FiguraSentada",
            vertices = vertices,
            uv = sprite.uv,
            triangles = Array.ConvertAll(sprite.triangles, n => (int)n)
        };
        mesh.RecalculateNormals();
        owned.Add(mesh);
        figura.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = figura.AddComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        var mat = Mat("figuraSentada", new Color(.81f, .77f, .68f), sprite.texture);
        mat.SetFloat("_AlphaClip", 1);
        mat.SetFloat("_ActorLighting", 1);
        mat.SetFloat("_Cull", 0);
        mat.renderQueue = 2450;
        renderer.sharedMaterial = mat;
    }

    internal void MostrarFigura(bool mostrar)
    {
        if (mostrar && figura == null) CriarFigura();
        if (figura != null) figura.SetActive(mostrar);
    }

    internal bool FiguraEmFoco
    {
        get
        {
            if (figura == null || !figura.activeSelf) return false;
            Vector3 alvo = figura.transform.position + Vector3.up * 1.04f;
            Vector3 delta = alvo - camera.transform.position;
            if (Vector3.Dot(camera.transform.forward, delta.normalized) < .988f) return false;
            var raio = new Ray(camera.transform.position, delta);
            foreach (var obstaculo in obstacles)
                if (obstaculo.IntersectRay(raio, out float distancia) && distancia < delta.magnitude - .08f)
                    return false;
            return true;
        }
    }

    private void CriarInteriorBuraco()
    {
        // The top edge matches the cut in SalaLit. The dark sides hide the room below the floor.
        const int lados = 72;
        var vertices = new Vector3[lados * 2 + 1];
        var triangles = new int[lados * 9];
        for (int i = 0; i < lados; i++)
        {
            float angulo = 2 * Mathf.PI * i / lados;
            float raio = 1 + .10f * Mathf.Sin(7 * angulo) + .065f * Mathf.Cos(13 * angulo);
            var direcao = new Vector3(Mathf.Cos(angulo), 0, Mathf.Sin(angulo));
            vertices[i] = direcao * (raio + .02f) + Vector3.down * .025f;
            vertices[i + lados] = direcao * (raio * .69f) + Vector3.down * 1.10f;
            int proximo = (i + 1) % lados;
            int inicio = i * 9;
            triangles[inicio] = i;
            triangles[inicio + 1] = i + lados;
            triangles[inicio + 2] = proximo;
            triangles[inicio + 3] = proximo;
            triangles[inicio + 4] = i + lados;
            triangles[inicio + 5] = proximo + lados;
            triangles[inicio + 6] = i + lados;
            triangles[inicio + 7] = lados * 2;
            triangles[inicio + 8] = proximo + lados;
        }

        vertices[lados * 2] = Vector3.down * 1.10f;
        var mesh = new Mesh { name = "Bocha_InteriorBuraco", vertices = vertices, triangles = triangles };
        mesh.RecalculateNormals();
        owned.Add(mesh);
        var go = new GameObject("Alucinacao_Buraco") { layer = Layer };
        interiorBuraco = go.transform;
        interiorBuraco.SetParent(root.transform, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>();
        var mat = Mat("interiorBuraco", new Color(.029f, .020f, .026f), null, true);
        mat.SetFloat("_Cull", 0);
        renderer.sharedMaterial = mat;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    internal void AbrirBuraco(BochaBall bola)
    {
        if (interiorBuraco == null) CriarInteriorBuraco();
        idQueda = bola.id;
        centroQueda = bola.Centro;
        fundoQueda = 0;
        float raio = bola.raio * 3.35f;
        var ponto = new Vector3(bola.pos.x, 0, bola.pos.y);
        interiorBuraco.localPosition = ponto;
        interiorBuraco.localScale = new Vector3(raio, 1, raio);
        interiorBuraco.gameObject.SetActive(true);
        AjustarAbertura(new Vector4(ponto.x, ponto.z, raio, 1));
    }

    internal void AnimarQueda(float segundos)
    {
        float t = Mathf.Clamp01((segundos - .10f) / .56f);
        fundoQueda = 1.26f * t * t;
    }

    internal void FecharBuraco()
    {
        if (idQueda < 0) return;
        idQueda = -1;
        interiorBuraco.gameObject.SetActive(false);
        AjustarAbertura(Vector4.zero);
    }

    private void AjustarAbertura(Vector4 abertura)
    {
        foreach (string nome in new[] { "saibro", "assoalho", "linha", "rastro" })
            if (materials.TryGetValue(nome, out var material)) material.SetVector(Abertura, abertura);
    }

    private void AplicarQueda(BochaBall bola, Transform visual, Transform sombra)
    {
        if (bola.id != idQueda) return;
        visual.localPosition = centroQueda + Vector3.down * fundoQueda;
        visual.localRotation = bola.rotacao * Quaternion.Euler(fundoQueda * 250, 0, 0);
        visual.gameObject.SetActive(fundoQueda < 1.18f);
        sombra.gameObject.SetActive(false);
    }
}
