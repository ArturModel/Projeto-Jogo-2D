using UnityEngine;
using UnityEngine.UI;

// Geometria limitada e determinística, sem criar objetos/partículas a cada quadro.
// Cenário e cabine são arte raster; só a perspectiva, reflexos e chuva são animados.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class PrologoEstradaGraphic : MaskableGraphic
{
    public PrologoJogavel Viagem;
    public bool Chuva;
    private VertexHelper mesh;
    private float deslocamento;
    private Texture2D asfalto;
    private Material materialAsfalto;
    private readonly float[] ultimaLimpeza = new float[150];
    public override Texture mainTexture => asfalto != null ? asfalto : Texture2D.whiteTexture;
    private static Color Cor(float r,float g,float b,float a=1) => new Color(r,g,b,a);

    protected override void Start()
    {
        base.Start();
        for(int i=0;i<ultimaLimpeza.Length;i++)ultimaLimpeza[i]=-20;
        if(Chuva)return;
        asfalto=Resources.Load<Texture2D>("PrologoJogavel/Asfalto");
        var shader=Resources.Load<Shader>("PrologoJogavel/AsfaltoMolhado");
        if(shader!=null) { materialAsfalto=new Material(shader); material=materialAsfalto; }
        else Debug.LogError("Shader do asfalto do prólogo ausente.");
        SetMaterialDirty();
    }
    protected override void OnDestroy()
    {
        if(materialAsfalto!=null)Destroy(materialAsfalto);
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
        float curva=Viagem.Curva*z*z*.0009f;
        return new Vector2(480+(x-Viagem.PosicaoLateral+curva)*1050/z,405-1600/z);
    }
    private void Estrada()
    {
        // Faixas longitudinais em profundidade: o grão acompanha o asfalto.
        for(int i=95;i>=0;i--)
        {
            float z0=9f+i*1.8f,z1=z0+1.85f;
            float luz=Mathf.Clamp01((195-z0)/160f)*(Viagem.FarolAlto?1.25f:.86f);
            float neblina=Mathf.InverseLerp(200,100,z0);
            float mundo=z0+deslocamento;
            var tom=Cor(.039f+luz*.048f,.052f+luz*.056f,.073f+luz*.064f,neblina);
            // Transição fina e irregular entre a vegetação pintada e o acostamento.
            for(int e=4;e>=0;e--)
            {
                float w=3.8f+e*.13f+Ruido(Mathf.FloorToInt(mundo)*7)*.05f;
                Chao(-w,w,z0,z1,Cor(.055f,.064f,.06f,(.14f+(.12f*(4-e)))*neblina),true);
            }
            Chao(-3.8f,3.8f,z0,z1,tom,true);
            float tinta=(.36f+Ruido(Mathf.FloorToInt(mundo*2))* .25f)*neblina;
            Chao(-3.8f,-3.75f,z0,z1,Cor(.45f,.44f,.37f,tinta));
            bool recuo=mundo>PrologoJogavel.DistanciaParada-55 && mundo<PrologoJogavel.DistanciaParada+100;
            if(!recuo) Chao(3.75f,3.8f,z0,z1,Cor(.45f,.44f,.37f,tinta));
            Chao(-.075f,-.022f,z0,z1,Cor(.65f,.46f,.18f,tinta));
            Chao(.045f,.1f,z0,z1,Cor(.65f,.46f,.18f,tinta));
            if(recuo)
            {
                // Recuo junto à calçada da Rodovia; a pista continua à esquerda.
                Chao(3.8f,7.6f,z0,z1,Cor(.09f+luz*.05f,.1f+luz*.05f,.105f+luz*.055f,neblina),true);
                Chao(7.6f,7.78f,z0,z1,Cor(.31f,.32f,.31f,neblina));
                Chao(7.78f,10.8f,z0,z1,Cor(.14f,.16f,.16f,neblina),true);
                Chao(7.78f,10.8f,z0,z0+.025f,Cor(.045f,.052f,.055f,neblina*.8f));
            }
            // Alongamento descontínuo das luzes na água, sem efeito de espelho perfeito.
            float fase=(z0+deslocamento)%17f;
            if(fase<8f)
            {
                float a=(.055f+Ruido(Mathf.FloorToInt(mundo)*3)*.18f)*luz*neblina;
                for(int r=0;r<3;r++)
                {
                    float centro=-2.55f+Mathf.Sin(mundo*2.3f+r)*.2f+r*.11f;
                    Chao(centro-.025f,centro+.025f,z0,z0+.13f+Ruido(i+r)*.35f,Cor(.88f,.58f,.28f,a));
                }
                Chao(1.85f,2.04f,z0,z0+.3f,Cor(.52f,.58f,.67f,.065f*luz*neblina));
            }
            for(int j=0;j<24;j++)
            {
                float x=-3.5f+Ruido(i*71+j*19)*7;
                Chao(x,x+.015f+Ruido(i+j)*.025f,z0,z0+.09f,Cor(.58f,.64f,.69f,.04f*luz*neblina));
            }
        }
        // Postes, fios, cercas e reflexos passam continuamente nos dois lados.
        for(int i=4;i>=0;i--)
        {
            float z=12+(i*44f-deslocamento%44f+220)%220;
            for(int lado=-1;lado<=1;lado+=2)
            {
                Vector2 pe=Projetar(lado*5.2f,z);
                float k=1050/z;
                Vector2 topo=pe+Vector2.up*k*4.2f;
                if(lado<0)
                {
                Linha(pe+new Vector2(2,0),topo+new Vector2(2,0),Mathf.Max(1,k*.11f),Cor(.04f,.045f,.043f));
                Linha(pe,topo,Mathf.Max(1,k*.08f),Cor(.16f,.14f,.11f));
                Linha(topo,topo+new Vector2(-lado*k*.6f,-k*.1f),Mathf.Max(1,k*.045f),Cor(.12f,.13f,.13f));
                Vector2 lamp=topo+new Vector2(-lado*k*.6f,-k*.14f);
                // Halos em anéis translúcidos: luz localizada sem cobrir a arte.
                for(int h=5;h>=1;h--) Disco(lamp,k*.13f*h,Cor(1,.65f,.29f,.012f*(6-h)),14);
                Disco(lamp,k*.08f,Cor(1,.76f,.4f,.9f),10);
                Vector2 seguinte=Projetar(lado*5.2f,z+44)+Vector2.up*(1050/(z+44))*4.2f;
                Vector2 anterior=topo;
                for(int f=1;f<=7;f++)
                {
                    float t=f/7f; var p=Vector2.Lerp(topo,seguinte,t)-Vector2.up*Mathf.Sin(t*Mathf.PI)*k*.25f;
                    Linha(anterior,p,.6f,Cor(.025f,.03f,.04f,.8f)); anterior=p;
                }
                }
                for(int f=0;f<3;f++)
                {
                    float zz=z+f*6; var cerca=Projetar(lado*4.55f,zz); float kk=1050/zz;
                    if(lado>0 && zz+deslocamento>PrologoJogavel.DistanciaParada-60)continue;
                    Linha(cerca,cerca+Vector2.up*kk*.9f,Mathf.Max(.7f,kk*.055f),Cor(.19f,.18f,.13f));
                    var proxima=Projetar(lado*4.55f,zz+6);
                    Linha(cerca+Vector2.up*kk*.65f,proxima+Vector2.up*(1050/(zz+6))*.65f,.65f,Cor(.16f,.18f,.16f));
                }
            }
        }
    }
    private void Vidro()
    {
        float fase=Viagem.FaseLimpador%1;
        bool varrendo=Viagem.Limpador>0 && Viagem.Tempo<PrologoJogavel.Parado && fase<.52f;
        float varredura=0;
        if(varrendo) varredura=fase<.26f?Mathf.SmoothStep(0,1,fase/.26f):1-Mathf.SmoothStep(0,1,(fase-.26f)/.26f);
        float angulo=Mathf.Lerp(12,100,varredura)*Mathf.Deg2Rad;
        Quad(new Vector2(20,265),new Vector2(940,265),new Vector2(940,515),new Vector2(20,515),Cor(.12f,.19f,.27f,.035f+Viagem.Molhado*.025f));
        // Chuva exterior: direção/velocidade distintas das gotas aderidas ao vidro.
        for(int i=0;i<80;i++)
        {
            float x=35+Ruido(i*43)*890;
            float y=272+Mathf.Repeat(Ruido(i*29)*240-Viagem.Tempo*(85+Ruido(i)*65),240);
            float vento=3+Viagem.Direcao*2;
            Linha(new Vector2(x,y),new Vector2(x-vento,y-9-Ruido(i*7)*10),.55f,Cor(.61f,.7f,.78f,.1f+Ruido(i*11)*.09f));
        }
        // Cada gota memoriza a passada da palheta sobre sua região do vidro.
        for(int i=0;i<150;i++)
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
            float a=(borda>.72f?.34f:.22f)*Mathf.Clamp01((Viagem.Tempo-ultimaLimpeza[i])/6f);
            float altura=1.4f+Ruido(i*17)*3.1f;
            Linha(p,p+Vector2.down*altura,1.5f,Cor(.055f,.11f,.17f,a));
            Linha(p+new Vector2(-.6f,.4f),p+new Vector2(-.6f,-altura*.5f),.65f,Cor(.74f,.81f,.85f,a));
            if(i%4==0) Linha(p+Vector2.down*altura,p+Vector2.down*(altura+5),.5f,Cor(.52f,.65f,.73f,a*.5f));
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
    private void Chao(float a,float b,float z0,float z1,Color cor,bool textura=false)
    {
        if(!textura) { Quad(Projetar(a,z0),Projetar(b,z0),Projetar(b,z1),Projetar(a,z1),cor); return; }
        QuadTextura(Projetar(a,z0),Projetar(b,z0),Projetar(b,z1),Projetar(a,z1),cor,
            new Vector2(a*.12f,(z0+deslocamento)*.05f),new Vector2(b*.12f,(z0+deslocamento)*.05f),
            new Vector2(b*.12f,(z1+deslocamento)*.05f),new Vector2(a*.12f,(z1+deslocamento)*.05f));
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
        => QuadTextura(a,b,c,d,cor,new Vector2(-1000,-1000),new Vector2(-1000,-1000),new Vector2(-1000,-1000),new Vector2(-1000,-1000));
    private void QuadTextura(Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color cor,Vector2 u0,Vector2 u1,Vector2 u2,Vector2 u3)
    {
        int n=mesh.currentVertCount;
        Vector2 origem=new Vector2(480,270);
        mesh.AddVert(a-origem,cor,u0); mesh.AddVert(b-origem,cor,u1);
        mesh.AddVert(c-origem,cor,u2); mesh.AddVert(d-origem,cor,u3);
        mesh.AddTriangle(n,n+1,n+2); mesh.AddTriangle(n,n+2,n+3);
    }
    private static float Ruido(int i)
    {
        unchecked { uint x=(uint)(i*374761393+668265263); x=(x^(x>>13))*1274126177; return (x&65535)/65535f; }
    }
}
