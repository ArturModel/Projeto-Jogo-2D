using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class BochaRoom3D
{
    private const float LinhaTonao = .40f;
    private const float PassoTonao = .17f;
    private static readonly Vector2 DescansoTonao = new Vector2(2.05f, LinhaTonao);
    private static readonly int AtlasTonaoST = Shader.PropertyToID("_BaseMap_ST");

    private Transform corpoTonao, sombraTonao;
    private MeshRenderer spriteTonao;
    private MaterialPropertyBlock quadroTonao;
    private Vector2 posTonao, destinoTonao, rumoTonao, miraTonao;
    private float passadaTonao, gestoTonao, solturaTonao;
    private int vistaTonao = 1, frameTonao, frameExibidoTonao = -1;
    private bool preparandoTonao, espelhadoTonao;

    private void ConstruirTonao(Material sombra)
    {
        var atlas = Resources.Load<Texture2D>("Bocha/TonaoBocha");
        if (atlas == null) return;

        corpoTonao = new GameObject("Tonão · Adversário").transform;
        corpoTonao.gameObject.layer = Layer;
        corpoTonao.SetParent(root.transform, false);
        var material = Mat("Tonao", Color.white, atlas);
        material.SetFloat("_AlphaClip", 1);
        material.SetFloat("_ActorLighting", 1);
        material.SetFloat("_Cull", 0);
        material.SetFloat("_Smoothness", 0);

        const float alturaQuadro = 2.20f;
        var plano = Primitive("Tonão · Sprite", PrimitiveType.Quad, Vector3.zero, Vector3.one * alturaQuadro, material);
        plano.transform.SetParent(corpoTonao, false);
        // O atlas exportado coloca as solas a sete oitavos da altura da célula.
        plano.transform.localPosition = Vector3.up * (alturaQuadro * .375f);
        spriteTonao = plano.GetComponent<MeshRenderer>();
        spriteTonao.shadowCastingMode = ShadowCastingMode.Off;
        quadroTonao = new MaterialPropertyBlock();

        sombraTonao = Primitive("Tonão · Contato", PrimitiveType.Quad, Vector3.zero,
            new Vector3(.92f, .59f, 1), sombra).transform;
        sombraTonao.localRotation = Quaternion.Euler(90, 0, 0);
        sombraTonao.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        ReiniciarTonao();
    }

    internal void ReiniciarTonao()
    {
        posTonao = destinoTonao = DescansoTonao;
        rumoTonao = Vector2.up;
        preparandoTonao = false;
        passadaTonao = gestoTonao = 0;
        frameTonao = 0;
        AtualizarSpriteTonao();
    }

    internal void PrepararTonao(BochaShot shot)
    {
        // A faixa livre entre a cabeceira e a linha atende às três posições.
        destinoTonao = new Vector2(shot.x, LinhaTonao);
        miraTonao = shot.velocidade.normalized;
        solturaTonao = shot.tipo == BochaThrowType.Ponto ? .66f : shot.tipo == BochaThrowType.Rafa ? .56f : .76f;
        preparandoTonao = true;
        gestoTonao = 0;
    }

    // A mudança para a primeira pose de mão vazia dispara um único lançamento.
    internal bool AvancarTonao(float delta)
    {
        float dt = Mathf.Min(delta, .08f);
        if (dt <= 0) return false;
        if (!preparandoTonao)
        {
            if (gestoTonao > 0)
            {
                gestoTonao += dt;
                if (gestoTonao < solturaTonao + .34f) return false;
                gestoTonao = 0;
                destinoTonao = DescansoTonao;
                frameTonao = 0;
            }
            if (posTonao == destinoTonao) return false;
        }

        if (posTonao != destinoTonao)
        {
            var anterior = posTonao;
            posTonao = Vector2.MoveTowards(anterior, destinoTonao, (preparandoTonao ? 2.10f : 1.65f) * dt);
            var deslocamento = posTonao - anterior;
            rumoTonao = deslocamento.normalized;
            passadaTonao += deslocamento.magnitude;
            int passo = Mathf.FloorToInt(passadaTonao / PassoTonao) % 4;
            frameTonao = passo == 3 ? 2 : passo + 1;
            if (posTonao != destinoTonao) return false;
            posTonao = destinoTonao;
            if (!preparandoTonao)
            {
                frameTonao = 0;
                rumoTonao = Vector2.up;
                return false;
            }
        }

        rumoTonao = miraTonao;
        gestoTonao += dt;
        if (gestoTonao < solturaTonao)
        {
            frameTonao = gestoTonao < solturaTonao * .36f ? 4 :
                gestoTonao < solturaTonao * .76f ? 5 : 6;
            return false;
        }

        frameTonao = 7;
        preparandoTonao = false;
        return true;
    }

    private void AtualizarSpriteTonao()
    {
        if (corpoTonao == null) return;
        var pes = new Vector3(posTonao.x, 0, posTonao.y);
        corpoTonao.localPosition = pes;
        sombraTonao.localPosition = pes + Vector3.up * .006f;
        var paraCamera = camera.transform.localPosition - pes;
        var horizontal = new Vector2(paraCamera.x, paraCamera.z);
        bool visivel = camera.orthographic || horizontal.sqrMagnitude > .32f * .32f;
        spriteTonao.enabled = visivel;
        if (!visivel) return;

        if (horizontal.sqrMagnitude < .001f) horizontal = -rumoTonao;
        horizontal.Normalize();
        float frente = Vector2.Dot(rumoTonao, horizontal);
        vistaTonao = frente > (vistaTonao == 0 ? .60f : .72f) ? 0 :
            frente < (vistaTonao == 1 ? -.60f : -.72f) ? 1 : 2;
        var direita = new Vector2(-horizontal.y, horizontal.x);
        bool perfilEsquerdo = vistaTonao == 2 && Vector2.Dot(rumoTonao, direita) < 0;
        bool espelhar = frameTonao < 4 && (perfilEsquerdo || vistaTonao != 2 && frameTonao == 3);
        var rotacao = Quaternion.LookRotation(new Vector3(-horizontal.x, 0, -horizontal.y));
        corpoTonao.localRotation = camera.orthographic ? rotacao * Quaternion.Euler(68, 0, 0) : rotacao;

        int linha = frameTonao < 4 ? vistaTonao : perfilEsquerdo ? 6 : vistaTonao + 3;
        int quadro = linha * 4 + frameTonao % 4;
        if (quadro == frameExibidoTonao && espelhar == espelhadoTonao) return;
        frameExibidoTonao = quadro;
        espelhadoTonao = espelhar;
        quadroTonao.SetVector(AtlasTonaoST, new Vector4(espelhar ? -.25f : .25f, 1f / 7,
            (quadro % 4 + (espelhar ? 1 : 0)) * .25f, (6 - linha) / 7f));
        spriteTonao.SetPropertyBlock(quadroTonao);
    }
}
