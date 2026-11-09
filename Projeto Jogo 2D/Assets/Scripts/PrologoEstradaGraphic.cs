using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Geometria limitada e determinística, sem criar objetos/partículas a cada quadro.
// Cenário raster ancorado no mundo, terreno em perspectiva e chuva no vidro.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class PrologoEstradaGraphic : MaskableGraphic
{
    public PrologoJogavel Viagem;
    public bool Chuva;
    private VertexHelper mesh;
    private float deslocamento;
    private const string Art = "Art/RuaNorte/";
    private const float InicioEntrada = 428f;
    private enum Arte { SemTextura, Solo, Arvore, Araucaria, Bracatinga, Pitangueira, Arbusto, Poste, Entrada, Orelhao, Nevoa }

    private struct Marco
    {
        public Sprite Sprite;
        public Arte Arte;
        public float Lateral, Distancia, Altura;
        public bool Invertido;
    }

    private readonly List<Marco> marcos = new List<Marco>();
    private readonly List<Marco> marcosRurais = new List<Marco>(96);
    private readonly Sprite[] vegetacao = new Sprite[5];
    private Texture2D terreno, pisoEntrada;
    private Sprite poste;
    private Material materialAsfalto;
    private readonly float[] ultimaLimpeza = new float[230];
    public override Texture mainTexture => terreno != null ? terreno : Texture2D.whiteTexture;
    private static Color Cor(float r,float g,float b,float a=1) => new Color(r,g,b,a);

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < ultimaLimpeza.Length; i++) ultimaLimpeza[i] = -20;
        if (Chuva) return;

        terreno = Resources.Load<Texture2D>(Art + "Terreno");
        var shader = Resources.Load<Shader>("PrologoJogavel/AsfaltoMolhado");
        if (terreno == null || shader == null)
        {
            Debug.LogError("Piso ou shader do prólogo ausente.");
            return;
        }

        materialAsfalto = new Material(shader);
        pisoEntrada = SantaRitaRuaNorte.CreateEntranceGroundLookup(terreno);
        materialAsfalto.SetTexture("_EntranceCells", pisoEntrada);
        materialAsfalto.SetVector("_EntranceMap", new Vector4(
            Capitulo1Director.CarroEntrada.x - 100f, PrologoJogavel.DistanciaParada,
            PrologoJogavel.EscalaLongitudinalEntrada, PrologoJogavel.EscalaLateralEntrada));
        var postes = Capitulo1Director.PostesEntrada;
        materialAsfalto.SetVector("_EntranceLamps", new Vector4(
            PrologoJogavel.DistanciaEntrada(postes[0].x), PrologoJogavel.DistanciaEntrada(postes[1].x),
            PrologoJogavel.DistanciaEntrada(postes[2].x), PrologoJogavel.LateralEntrada(postes[0].y)));
        materialAsfalto.SetFloat("_EntranceStart", InicioEntrada);

        string[] especies = { "Arvore", "Araucaria", "Bracatinga", "Pitangueira", "Arbusto" };
        for (int i = 0; i < especies.Length; i++)
            vegetacao[i] = CarregarSprite(especies[i], (Arte)((int)Arte.Arvore + i));
        poste = CarregarSprite("Poste", Arte.Poste);
        var orelhao = CarregarSprite("Orelhao", Arte.Orelhao);
        var placa = SantaRitaRuaNorte.ObjetoDaEntrada("PlacaSantaRita");
        if (placa != null) materialAsfalto.SetTexture("_Entrada", placa.texture);
        MarcoEntrada(placa, Arte.Entrada, Capitulo1Director.PlacaEntrada, 2.45f);
        MarcoEntrada(SantaRitaRuaNorte.ObjetoDaEntrada("BancoRodovia"), Arte.Entrada, Capitulo1Director.BancoEntrada, .86f);
        MarcoEntrada(SantaRitaRuaNorte.ObjetoDaEntrada("QuadroParada"), Arte.Entrada, Capitulo1Director.QuadroEntrada, 1.82f);
        MarcoEntrada(orelhao, Arte.Orelhao, Capitulo1Director.OrelhaoEntrada, 1.4f);
        foreach (var posicao in postes) MarcoEntrada(poste, Arte.Poste, posicao, 3.15f);

        // Norte à esquerda no sentido leste. As copas abrem espaço para a placa e a parada.
        float[] norte = { 103f, 115.2f, 118f, 123.5f, 128.2f, 132f };
        float[] sul = { 103.5f, 109f, 114.5f, 120f, 127f, 132.5f };
        for (int i = 0; i < norte.Length; i++)
        {
            int especie = i % 3 == 1 ? 1 : 0;
            MarcoEntrada(vegetacao[especie], (Arte)((int)Arte.Arvore + especie),
                new Vector2(norte[i], 15.2f + (i % 2) * .8f), 3.9f + i * .1f);
            especie = i % 2 == 0 ? 3 : 2;
            MarcoEntrada(vegetacao[especie], (Arte)((int)Arte.Arvore + especie),
                new Vector2(sul[i], 2.2f + (i % 2) * .9f), 3.5f + i * .1f);
        }
        MarcoEntrada(vegetacao[4], Arte.Arbusto, new Vector2(104.4f, 15.2f), .75f);
        MarcoEntrada(vegetacao[4], Arte.Arbusto, new Vector2(115.4f, 14.9f), .64f);
        MarcoEntrada(vegetacao[4], Arte.Arbusto, new Vector2(106.5f, 5.35f), .82f);
        MarcoEntrada(vegetacao[4], Arte.Arbusto, new Vector2(124.5f, 5.15f), .72f);
        marcos.Sort((a, b) => b.Distancia.CompareTo(a.Distancia));

        material = materialAsfalto;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        SetMaterialDirty();
    }

    private Sprite CarregarSprite(string nome, Arte arte)
    {
        var sprite = Resources.Load<Sprite>(Art + nome);
        if (sprite != null) materialAsfalto.SetTexture("_" + arte, sprite.texture);
        return sprite;
    }

    private void MarcoEntrada(Sprite sprite, Arte arte, Vector2 posicao, float altura)
    {
        marcos.Add(new Marco
        {
            Sprite = sprite, Arte = arte, Altura = altura * 1.5f,
            Lateral = PrologoJogavel.LateralEntrada(posicao.y),
            Distancia = PrologoJogavel.DistanciaEntrada(posicao.x)
        });
    }

    protected override void OnDestroy()
    {
        if (materialAsfalto != null) Destroy(materialAsfalto);
        if (pisoEntrada != null) Destroy(pisoEntrada);
        base.OnDestroy();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); if(Viagem==null)return;
        mesh=vh; deslocamento=Viagem.Distancia;
        if(Chuva) Vidro(); else Estrada();
    }

    private Vector2 Projetar(float x,float z)
    {
        float curva=PrologoJogavel.CentroVia(deslocamento+z)-PrologoJogavel.CentroVia(deslocamento)-Viagem.Rumo*z;
        return new Vector2(480+(x-Viagem.PosicaoLateral+curva)*1050/z,405-1600/z);
    }
    private void Estrada()
    {
        if (materialAsfalto == null) return;
        materialAsfalto.SetVector("_View", new Vector4(
            Viagem.PosicaoLateral, deslocamento, Viagem.FarolAlto ? 1 : 0,
            MenuPausa.ReduzirEfeitosIntensos ? 0 : Viagem.Tempo));
        const float margem = 62f;
        // UV/Z e 1/Z mantêm as texturas presas ao solo, inclusive nas curvas.
        for (int i = 99; i >= 0; i--)
        {
            float z0 = 7f + i * 2f, z1 = z0 + 2f;
            QuadTextura(Projetar(-margem,z0), Projetar(margem,z0), Projetar(margem,z1), Projetar(-margem,z1),
                Color.white, new Vector2(-margem, z0 + deslocamento), new Vector2(margem, z0 + deslocamento),
                new Vector2(margem, z1 + deslocamento), new Vector2(-margem, z1 + deslocamento),
                Arte.Solo, 1f / z0, 1f / z1);
        }

        // As sombras ficam entre o terreno e os sprites, em ambos os trechos da viagem.
        PrepararCenarioRural();
        DesenharSombras(marcos);
        DesenharSombras(marcosRurais);
        DesenharMarcos(marcos);
        DesenharMarcos(marcosRurais);
        // A camada baixa atravessa as copas e a pista, antes da chuva no vidro.
        QuadTextura(new Vector2(-44, 236), new Vector2(1004, 236), new Vector2(1004, 558), new Vector2(-44, 558),
            Color.white, Vector2.zero, Vector2.right, Vector2.one, Vector2.up, Arte.Nevoa);
    }

    private void PrepararCenarioRural()
    {
        marcosRurais.Clear();
        int primeiro = Mathf.CeilToInt((deslocamento + 9f) / 9f);
        int ultimo = Mathf.FloorToInt(Mathf.Min(InicioEntrada - 1f, deslocamento + 195f) / 9f);
        for (int i = ultimo; i >= primeiro; i--)
        {
            float mundo = i * 9f;
            for (int lado = -1; lado <= 1; lado += 2)
            {
                int semente = i * 47 + lado * 11;
                int especie = Mathf.Min(3, (int)(Ruido(semente + 14) * 4));
                float afastamento = 6.5f + Ruido(semente) * 7f;
                if (i % 5 != 2)
                    MarcoRural(vegetacao[especie], (Arte)((int)Arte.Arvore + especie), lado * afastamento, mundo,
                        4.1f + Ruido(semente + 4) * 3.6f);
                MarcoRural(vegetacao[4], Arte.Arbusto, lado * (4.9f + Ruido(semente + 9) * 2.4f), mundo,
                    .75f + Ruido(semente + 12) * .95f);
            }
            if (i % 5 == 0)
                MarcoRural(poste, Arte.Poste, 5.65f, mundo, 4.4f, true);
        }
    }

    private void MarcoRural(Sprite sprite, Arte arte, float lateral, float mundo, float altura, bool invertido = false)
    {
        marcosRurais.Add(new Marco
        {
            Sprite = sprite, Arte = arte, Lateral = lateral, Distancia = mundo, Altura = altura, Invertido = invertido
        });
    }

    private void DesenharMarcos(List<Marco> grupo)
    {
        foreach (var marco in grupo)
        {
            float z = marco.Distancia - deslocamento;
            if (marco.Sprite != null && z >= 9f && z < 198f)
                DesenharMarco(marco.Sprite, marco.Arte, marco.Lateral, z, marco.Altura, marco.Invertido);
        }
    }

    private void DesenharSombras(List<Marco> grupo)
    {
        foreach (var marco in grupo)
        {
            float z = marco.Distancia - deslocamento;
            if (marco.Sprite == null || z < 8f || z > 85f) continue;
            float x = marco.Lateral;
            float largura = marco.Altura * marco.Sprite.rect.width / marco.Sprite.rect.height;

            // O contato prende o pé ao piso. A segunda sombra usa o recorte do objeto.
            float contato = Mathf.Clamp01(1f - z / 85f);
            for (int anel = 3; anel > 0; anel--)
                SombraContato(x, z, largura * (.10f + anel * .055f), .30f + anel * .23f, contato * .12f);

            Vector2 fonte = new Vector2(Viagem.PosicaoLateral + Mathf.Sign(x - Viagem.PosicaoLateral) * .57f, 0);
            float alturaLuz = 1.15f;
            float forca = marco.Arte == Arte.Poste ? 0 : .29f * Mathf.Clamp01((59f - z) / 41f) *
                Mathf.Clamp01(1f - Mathf.Abs(x - fonte.x) / (3.2f + z * .11f));
            if (marco.Distancia >= InicioEntrada)
            {
                foreach (var posicao in Capitulo1Director.PostesEntrada)
                    UsarLuzMaisForte(x, z, PrologoJogavel.LateralEntrada(posicao.y) + 1.1f,
                        PrologoJogavel.DistanciaEntrada(posicao.x) - deslocamento, 4.7f,
                        ref fonte, ref alturaLuz, ref forca);
            }
            else
            {
                float posteMaisProximo = Mathf.Round(marco.Distancia / 45f) * 45f;
                UsarLuzMaisForte(x, z, 4.8f, posteMaisProximo - deslocamento, 4.4f,
                    ref fonte, ref alturaLuz, ref forca);
            }
            if (forca < .035f) continue;

            Vector2 direcao = new Vector2(x - fonte.x, z - fonte.y);
            // A altura determina o alcance, com limite para objetos que passam acima da lâmpada.
            float alcance = marco.Altura / Mathf.Max(alturaLuz - marco.Altura, alturaLuz * .47f);
            Vector2 ponta = Vector2.ClampMagnitude(direcao * alcance, alturaLuz < 2f ? 11f : 18f);
            float pivo = marco.Sprite.pivot.x / marco.Sprite.rect.width;
            if (marco.Invertido) pivo = 1 - pivo;
            float esquerda = x - largura * pivo, direita = esquerda + largura;
            float zFim = Mathf.Max(7f, z + ponta.y);
            SpriteUV(marco.Sprite, marco.Invertido, out var uv0, out var uv1);
            QuadTextura(Projetar(esquerda, z), Projetar(direita, z),
                Projetar(direita + ponta.x, zFim), Projetar(esquerda + ponta.x, zFim),
                Cor(.012f, .017f, .025f, forca * Mathf.Clamp01(1 - z / 112f)),
                uv0, new Vector2(uv1.x, uv0.y), uv1, new Vector2(uv0.x, uv1.y), marco.Arte,
                1f / z, 1f / zFim, sombra: true);
        }
    }

    private static void UsarLuzMaisForte(float x, float z, float luzX, float luzZ, float altura,
        ref Vector2 fonte, ref float alturaLuz, ref float forca)
    {
        float lateral = x - luzX, longitudinal = (z - luzZ) * .55f;
        float distancia = Mathf.Sqrt(lateral * lateral + longitudinal * longitudinal);
        float intensidade = .64f * Mathf.Clamp01(1 - distancia / 10f);
        if (distancia < .8f || intensidade <= forca) return;
        fonte = new Vector2(luzX, luzZ);
        alturaLuz = altura;
        forca = intensidade;
    }

    private void SombraContato(float x, float z, float rx, float rz, float alfa)
    {
        var meio = Projetar(x, z);
        var cor = Cor(.012f, .017f, .025f, alfa);
        for (int i = 0; i < 10; i++)
        {
            float a = i * Mathf.PI * .2f, b = (i + 1) * Mathf.PI * .2f;
            Quad(meio, Projetar(x + Mathf.Cos(a) * rx, z + Mathf.Sin(a) * rz),
                Projetar(x + Mathf.Cos(b) * rx, z + Mathf.Sin(b) * rz), meio, cor);
        }
    }

    private void DesenharMarco(Sprite sprite, Arte arte, float x, float z, float altura, bool inverter)
    {
        Vector2 pe = Projetar(x, z);
        float h = altura * 1050f / z;
        Rect recorte = sprite.rect;
        float w = h * recorte.width / recorte.height;
        float pivoX = sprite.pivot.x / recorte.width;
        if (inverter) pivoX = 1 - pivoX;
        float esquerda = pe.x - w * pivoX;
        float baseY = pe.y - h * sprite.pivot.y / recorte.height;
        float visibilidade = Mathf.Exp(-Mathf.Max(0, z - 14f) * .022f);
        float alfa = visibilidade * (1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(100, 180, z)));
        float farol = Mathf.Clamp01((92f - z) / 80f) *
            Mathf.Clamp01(1f - Mathf.Abs(x - Viagem.PosicaoLateral) / 12f) *
            (Viagem.FarolAlto ? 1f : .64f);
        float posteProximo = 0;
        foreach (var posicao in Capitulo1Director.PostesEntrada)
        {
            float distancia = Mathf.Abs(deslocamento + z - PrologoJogavel.DistanciaEntrada(posicao.x));
            posteProximo = Mathf.Max(posteProximo, Mathf.Clamp01(1f - distancia / 25f) *
                Mathf.Clamp01(1f - Mathf.Abs(x - PrologoJogavel.LateralEntrada(posicao.y)) / 8f));
        }
        var c = Cor(.29f + farol * .21f + posteProximo * .54f,
            .37f + farol * .19f + posteProximo * .39f,
            .48f + farol * .14f + posteProximo * .22f, alfa);
        if (arte == Arte.Poste)
        {
            Vector2 lampada = new Vector2(esquerda + w * (inverter ? .22f : .78f), baseY + h * .925f);
            for (int i = 6; i > 0; i--)
                Disco(lampada, h * i * .039f, Cor(.92f, .58f, .27f, Mathf.Sqrt(alfa) * .034f), 12);
        }

        SpriteUV(sprite, inverter, out var uv0, out var uv1);
        QuadTextura(new Vector2(esquerda, baseY), new Vector2(esquerda + w, baseY),
            new Vector2(esquerda + w, baseY + h), new Vector2(esquerda, baseY + h), c,
            uv0, new Vector2(uv1.x, uv0.y), uv1, new Vector2(uv0.x, uv1.y), arte);
    }

    private static void SpriteUV(Sprite sprite, bool inverter, out Vector2 min, out Vector2 max)
    {
        var recorte = sprite.rect;
        var tex = sprite.texture;
        // Meio texel de margem para impedir que o recorte leia a peça vizinha do atlas.
        min = new Vector2((recorte.xMin + .5f) / tex.width, (recorte.yMin + .5f) / tex.height);
        max = new Vector2((recorte.xMax - .5f) / tex.width, (recorte.yMax - .5f) / tex.height);
        if (inverter) (min.x, max.x) = (max.x, min.x);
    }

    private void Vidro()
    {
        float fase=Viagem.FaseLimpador%1;
        bool varrendo=Viagem.Limpador>0 && Viagem.Tempo<PrologoJogavel.Parado && fase<.52f;
        float varredura=0;
        if(varrendo) varredura=fase<.26f?Mathf.SmoothStep(0,1,fase/.26f):1-Mathf.SmoothStep(0,1,(fase-.26f)/.26f);
        float angulo=Mathf.Lerp(12,100,varredura)*Mathf.Deg2Rad;
        bool reduzir = MenuPausa.ReduzirEfeitosIntensos;
        Quad(new Vector2(-30,245),new Vector2(990,245),new Vector2(990,550),new Vector2(-30,550),
            Cor(.06f,.12f,.18f,.045f+Viagem.Molhado*.045f));
        // Cortinas de chuva com escalas distintas, iluminadas onde cruzam o feixe.
        CortinaChuva(reduzir ? 112 : 210, 11, 190f, 13f, .49f, reduzir ? .19f : .29f);
        CortinaChuva(reduzir ? 40 : 86, 43, 345f, 29f, .76f, reduzir ? .20f : .38f);
        // Cada gota memoriza a passada da palheta sobre sua região do vidro.
        int gotas = reduzir ? 145 : ultimaLimpeza.Length;
        for(int i=0;i<gotas;i++)
        {
            float x=45+Ruido(i*37+1)*870;
            float y=273+Mathf.Repeat(Ruido(i*51+6)*225-Viagem.Tempo*(.7f+Ruido(i)*3),225);
            float borda=Mathf.Abs(x-480)/435;
            Vector2 p=new Vector2(x,y);
            if(varrendo)
                for(int l=0;l<2;l++)
                {
                    Vector2 d=p-new Vector2(190+l*390,268);
                    if(d.magnitude<258 && Mathf.Abs(Mathf.Atan2(d.y,d.x)-angulo)<.14f)ultimaLimpeza[i]=Viagem.Tempo;
                }
            float a=(borda>.72f?.50f:.34f)*Mathf.Clamp01((Viagem.Tempo-ultimaLimpeza[i])/3.8f);
            float altura=2.3f+Ruido(i*17)*5.2f;
            Linha(p,p+Vector2.down*altura,1.5f,Cor(.055f,.11f,.17f,a));
            Linha(p+new Vector2(-.6f,.4f),p+new Vector2(-.6f,-altura*.5f),.65f,Cor(.74f,.81f,.85f,a));
            if(i%3==0) Linha(p+Vector2.down*altura,p+new Vector2(-1.1f,-altura-7f-Ruido(i*89)*8f),
                .66f,Cor(.49f,.62f,.72f,a*.62f));
        }
        for(int i=0;i<2;i++)
        {
            Vector2 pe=new Vector2(190+i*390,268);
            Vector2 ponta=pe+new Vector2(Mathf.Cos(angulo),Mathf.Sin(angulo))*255;
            Linha(pe+new Vector2(0,-2),ponta+new Vector2(0,-2),6,Cor(.015f,.018f,.023f,.8f));
            Linha(pe,ponta,3.6f,Cor(.035f,.042f,.047f));
            Linha(pe+Vector2.up,ponta+Vector2.up, .85f,Cor(.27f,.29f,.28f,.7f));
        }
    }
    private void CortinaChuva(int quantidade, int semente, float velocidade, float comprimento, float largura, float opacidade)
    {
        float vento = .24f + Viagem.Direcao * .035f;
        for (int i = 0; i < quantidade; i++)
        {
            int id = i * 97 + semente;
            float fase = Ruido(id * 13) * 295f - Viagem.Tempo * velocidade * (.7f + Ruido(id) * .6f);
            float y = Mathf.Repeat(fase, 295f);
            float x = -25f + Mathf.Repeat(Ruido(id * 3) * 1010f + fase * vento, 1010f);
            float h = comprimento * (.50f + Ruido(id * 7) * .85f);
            float farol = Mathf.Clamp01(1f - Mathf.Abs(x - 480f) / 430f) * Mathf.Clamp01((245f - y) / 175f);
            float alfa = opacidade * (.36f + Ruido(id * 19) * .47f + farol * .25f);
            Linha(new Vector2(x, 252f + y), new Vector2(x - h * vento, 252f + y - h),
                largura, Cor(.61f,.72f,.82f,alfa));
        }
    }

    private void Linha(Vector2 a,Vector2 b,float largura,Color cor)
    {
        Vector2 d=(b-a).normalized; Vector2 n=new Vector2(-d.y,d.x)*largura*.5f;
        Quad(a-n,b-n,b+n,a+n,cor);
    }
    private void Disco(Vector2 centro,float raio,Color cor,int passos)
    {
        for(int i=0;i<passos;i++)
        {
            float a=i*Mathf.PI*2/passos,b=(i+1)*Mathf.PI*2/passos;
            Quad(centro,centro+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*raio,
                centro+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*raio,centro,cor);
        }
    }
    private void Quad(Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color cor)
        => QuadTextura(a,b,c,d,cor,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);
    private void QuadTextura(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color cor,
        Vector2 u0, Vector2 u1, Vector2 u2, Vector2 u3, Arte arte = Arte.SemTextura, float q0 = 1, float q1 = 1, bool sombra = false)
    {
        int n = mesh.currentVertCount;
        int camada = sombra ? -(int)arte : (int)arte;
        Vertice(a, cor, u0, camada, q0); Vertice(b, cor, u1, camada, q0);
        Vertice(c, cor, u2, camada, q1); Vertice(d, cor, u3, camada, q1);
        mesh.AddTriangle(n,n+1,n+2); mesh.AddTriangle(n,n+2,n+3);
    }

    private void Vertice(Vector2 pos, Color cor, Vector2 uv, int camada, float q)
    {
        var v = UIVertex.simpleVert;
        v.position = pos - new Vector2(480, 270);
        v.color = cor;
        v.uv0 = new Vector4(uv.x * q, uv.y * q, q, 0);
        v.uv1 = new Vector2(camada, 0);
        mesh.AddVert(v);
    }
    private static float Ruido(int i)
    {
        unchecked { uint x=(uint)(i*374761393+668265263); x=(x^(x>>13))*1274126177; return (x&65535)/65535f; }
    }
}
