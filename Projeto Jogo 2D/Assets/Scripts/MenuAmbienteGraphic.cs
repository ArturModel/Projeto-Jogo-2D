using UnityEngine;
using UnityEngine.UI;

// Chuva em perspectiva e luz pintada em geometria UI; sem vídeo, blur,
// textura recriada por frame, RNG global ou alocação por gota.
public sealed class MenuAmbienteGraphic : MaskableGraphic
{
    public enum Camada { Ambiente, Leitura }
    public Camada Tipo;
    public float Tempo, LuzClube, LuzTelefone, Saida, Brilho = 1;
    public Vector2 Foco;
    Vector2 resposta;
    float tempoResposta = -100;
    Rect area;

    public void Responder(Vector2 ponto, float tempo)
    { resposta = ponto; tempoResposta = tempo; }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); area = rectTransform.rect;
        if (Tipo == Camada.Leitura) { Leitura(vh); return; }
        // A placa de base tem textura rica; o azul da noite fica sobre a imagem.
        Caixa(vh, 0, 0, 1, 1, new Color(.005f, .018f, .035f, .25f));
        float vacilo = .5f + .5f * Mathf.Sin(Tempo * .57f);
        // Escurecer o interior, conservando os caixilhos e a textura da pintura.
        float sombra = Mathf.Lerp(.46f + vacilo * .14f, .10f, LuzClube);
        Caixa(vh, .446f, .32f, .489f, .421f, new Color(.015f, .019f, .023f, sombra));
        Caixa(vh, .504f, .325f, .548f, .427f, new Color(.015f, .019f, .023f, sombra));
        Caixa(vh, .565f, .331f, .611f, .435f, new Color(.015f, .019f, .023f, sombra));
        Caixa(vh, .77f, .18f, .855f, .48f, new Color(.015f, .019f, .023f, sombra * .65f));
        Halo(vh, new Vector2(.535f, .745f), new Vector2(.065f, .12f), new Color(1, .64f, .26f, .10f + LuzClube * .03f));
        Halo(vh, new Vector2(.885f, .4f), new Vector2(.09f, .24f), new Color(.48f, .7f, .61f, LuzTelefone * .07f));
        Reflexos(vh);
        Chuva(vh);
        Ondas(vh);
        float decorrido = Tempo - tempoResposta;
        if (decorrido >= 0 && decorrido < 1.1f)
        {
            if (resposta.y < .3f)
                Anel(vh, resposta, new Vector2(.004f + decorrido * .022f, .001f + decorrido * .006f), new Color(.72f, .83f, .85f, (1 - decorrido / 1.1f) * .6f));
            else
                Halo(vh, new Vector2(.9f, .4f), new Vector2(.075f, .16f), new Color(.7f, .91f, .8f, (1 - decorrido / 1.1f) * .1f));
        }
    }

    void Leitura(VertexHelper vh)
    {
        // Um degradê lateral mantém a rua legível e não apaga o clube.
        for (int i = 0; i < 40; i++)
        {
            float x = i / 40f;
            float a = .66f * Mathf.Pow(Mathf.Clamp01(1 - x / .61f), 1.4f);
            a += LuzClube * .10f;
            Caixa(vh, x, 0, x + .025f, 1, new Color(.006f, .012f, .022f, a));
        }
        Caixa(vh, 0, 0, 1, 1, new Color(0, 0, 0, Saida * .9f));
    }

    void Chuva(VertexHelper vh)
    {
        for (int i = 0; i < 148; i++)
        {
            float z = Hash(i * 3 + 1);
            float x = Mathf.Repeat(Hash(i * 7 + 9) - Tempo * (.009f + z * .012f), 1.05f) - .025f;
            float y = Mathf.Repeat(Hash(i * 11 + 5) - Tempo * (.39f + z * .5f), 1.12f) - .06f;
            float pertoLuz = Mathf.Exp(-Mathf.Pow((x - .54f) / .12f, 2)) * .2f;
            float a = (.065f + z * .12f + pertoLuz) * Mathf.Clamp(Brilho, .2f, 1.5f);
            var c = new Color(.64f, .75f, .8f, a);
            float comprimento = .008f + z * .02f;
            Linha(vh, new Vector2(x, y), new Vector2(x + comprimento * .12f, y + comprimento), .45f + z * .5f, c);
        }
    }

    void Reflexos(VertexHelper vh)
    {
        for (int i = 0; i < 56; i++)
        {
            float y = .012f + Hash(i * 13 + 1) * .23f;
            float centro = i % 3 == 0 ? .267f : i % 3 == 1 ? .54f : .81f;
            float x = centro + (Hash(i * 5 + 2) - .5f) * (.02f + (.26f - y) * .20f);
            float a = (.022f + .04f * Mathf.Sin(Tempo * 2.2f + i * .9f)) * Brilho;
            a = Mathf.Max(0, a) * (1 + LuzClube * .6f);
            float largura = .003f + Hash(i * 3 + 7) * .021f;
            Caixa(vh, x - largura, y, x + largura, y + .001f, new Color(.98f, .71f, .32f, a));
        }
    }

    void Ondas(VertexHelper vh)
    {
        for (int i = 0; i < 30; i++)
        {
            float fase = Mathf.Repeat(Tempo * (.5f + Hash(i + 3) * .35f) + Hash(i * 3), 1);
            float x = Hash(i * 23 + 2);
            float y = .018f + Hash(i * 17 + 4) * .23f;
            // Abaixo da linha do passeio: nenhuma onda em fachada ou céu.
            float limite = .29f - .20f * x;
            if (y > limite) continue;
            float escala = .25f + (1 - y / .3f) * .75f;
            float largura = (.002f + fase * .011f) * escala;
            Anel(vh, new Vector2(x, y), new Vector2(largura, largura * .25f), new Color(.5f, .63f, .7f, (1 - fase) * .22f * Brilho));
        }
    }

    void Halo(VertexHelper vh, Vector2 p, Vector2 raio, Color cor)
    {
        int n = vh.currentVertCount;
        vh.AddVert(Pos(p), cor, Vector2.zero);
        cor.a = 0;
        const int passos = 32;
        for (int i = 0; i <= passos; i++)
        {
            float ang = i * Mathf.PI * 2 / passos;
            vh.AddVert(Pos(p + Vector2.Scale(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), raio)), cor, Vector2.zero);
            if (i > 0) vh.AddTriangle(n, n + i, n + i + 1);
        }
    }

    void Anel(VertexHelper vh, Vector2 p, Vector2 raio, Color c)
    {
        const int passos = 16;
        Vector2 anterior = p + new Vector2(raio.x, 0);
        for (int i = 1; i <= passos; i++)
        {
            float ang = i * Mathf.PI * 2 / passos;
            Vector2 proximo = p + Vector2.Scale(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), raio);
            Linha(vh, anterior, proximo, .6f, c); anterior = proximo;
        }
    }

    Vector3 Pos(Vector2 p) { return new Vector3(area.xMin + p.x * area.width, area.yMin + p.y * area.height); }

    void Linha(VertexHelper vh, Vector2 a, Vector2 b, float largura, Color c)
    {
        Vector3 inicio = Pos(a), fim = Pos(b);
        Vector3 normal = Vector3.Cross((fim - inicio).normalized, Vector3.forward) * largura * .5f;
        Quad(vh, inicio - normal, inicio + normal, fim + normal, fim - normal, c);
    }

    void Caixa(VertexHelper vh, float x0, float y0, float x1, float y1, Color c)
    { Quad(vh, Pos(new Vector2(x0, y0)), Pos(new Vector2(x0, y1)), Pos(new Vector2(x1, y1)), Pos(new Vector2(x1, y0)), c); }

    static void Quad(VertexHelper vh, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color cor)
    {
        int n = vh.currentVertCount;
        vh.AddVert(a, cor, Vector2.zero); vh.AddVert(b, cor, Vector2.zero);
        vh.AddVert(c, cor, Vector2.zero); vh.AddVert(d, cor, Vector2.zero);
        vh.AddTriangle(n, n + 1, n + 2); vh.AddTriangle(n, n + 2, n + 3);
    }

    static float Hash(int n) { return Mathf.Repeat(Mathf.Sin(n * 127.1f + 31.7f) * 43758.5453f, 1); }
}
