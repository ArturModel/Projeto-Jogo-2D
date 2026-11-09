using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private const float DesnivelSociedade = .46875f;
    private static readonly Color MadeiraSociedade = new Color(.36f, .28f, .20f);
    private static readonly Color EstruturaSociedade = new Color(.22f, .19f, .15f);
    private static readonly Color RebocoSociedade = new Color(.51f, .50f, .42f);
    private static readonly Color LambrilSociedade = new Color(.24f, .29f, .25f);
    private static readonly Color LuzSociedade = new Color(1f, .79f, .55f);

    private void MontarSociedade()
    {
        var madeira = Resources.Load<Texture2D>("Bocha/MadeiraCancha");
        var saibro = Resources.Load<Texture2D>("Bocha/SaibroCancha");
        PisoSociedade("Assoalho do salão", new Rect(.36f, .42f, 29.28f, 7.68f), 0f, madeira);
        PisoSociedade("Corredor lateral do salão", new Rect(.36f, 8.10f, 2.84f, 9.71f), 0f, madeira);

        float fundo = -DesnivelSociedade;
        PisoSociedade("Margem da cabeceira", new Rect(3.20f, 9.10f, 3.80f, 8.71f), fundo, madeira);
        PisoSociedade("Patamar oeste da escada", new Rect(3.20f, 8.10f, .65f, 1f), fundo, madeira);
        PisoSociedade("Patamar leste da escada", new Rect(5.55f, 8.10f, 1.45f, 1f), fundo, madeira);
        PisoSociedade("Corredor sul da cancha", new Rect(7f, 8.10f, 19f, 1.40f), fundo, madeira);
        PisoSociedade("Corredor norte da cancha", new Rect(7f, 15f, 19f, 2.81f), fundo, madeira);
        PisoSociedade("Corredor da cabeceira oposta", new Rect(26f, 8.10f, 3.64f, 9.71f), fundo, madeira);
        PisoSociedade("Saibro da Sociedade", new Rect(7f, 9.50f, 19f, 5.50f), fundo, saibro,
            new Color(.83f, .76f, .63f));

        // O corte no assoalho do salão e os três espelhos seguem o mesmo volume
        // que SantaRitaShallowStairs usa para projetar os pés.
        for (int i = 0; i < 3; i++)
        {
            float inicio = 6.30f + i * (2.80f / 3);
            float profundidade = 2.80f / 3;
            float altura = fundo * (i + 1) / 3;
            var degrau = Box("Degrau de madeira", new Vector2(4.70f, inicio + profundidade * .5f),
                new Vector3(1.70f, .07f, profundidade), altura - .07f, MadeiraSociedade);
            TexturaSociedade(degrau, madeira, Vector2.one);
            Box("Nariz do degrau", new Vector2(4.70f, inicio + .04f), new Vector3(1.70f, .035f, .08f),
                altura - .035f, EstruturaSociedade);
        }

        ParedesSociedade();
        MobiliaSalaoSociedade();
        MobiliaCanchaSociedade(madeira);
        LuzesSociedade();
    }

    private void PisoSociedade(string nome, Rect area, float altura, Texture2D textura, Color? cor = null)
    {
        Rect faixaEscada = new Rect(3.85f, 6.30f, 1.70f, 2.80f);
        if (altura == 0 && area.Overlaps(faixaEscada))
        {
            // Recortes retangulares deixam a escada visível sem sobrepor o piso.
            PisoSociedade(nome, Rect.MinMaxRect(area.xMin, area.yMin, area.xMax, faixaEscada.yMin), altura, textura, cor);
            PisoSociedade(nome, Rect.MinMaxRect(area.xMin, faixaEscada.yMin, faixaEscada.xMin, area.yMax), altura, textura, cor);
            PisoSociedade(nome, Rect.MinMaxRect(faixaEscada.xMax, faixaEscada.yMin, area.xMax, area.yMax), altura, textura, cor);
            return;
        }

        var piso = Box(nome, area.center, new Vector3(area.width, .11f, area.height),
            altura - .11f, cor ?? new Color(.66f, .60f, .50f));
        TexturaSociedade(piso, textura, new Vector2(area.width, area.height));
    }

    private static void TexturaSociedade(GameObject objeto, Texture2D textura, Vector2 repeticao)
    {
        if (objeto == null || textura == null || !objeto.TryGetComponent<Renderer>(out var renderer)) return;
        var properties = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(properties);
        properties.SetTexture("_BaseMap", textura);
        properties.SetVector("_BaseMap_ST", new Vector4(repeticao.x, repeticao.y, 0, 0));
        renderer.SetPropertyBlock(properties);
    }

    private void ParedesSociedade()
    {
        float fundo = -DesnivelSociedade;
        // Cutaway do lado da câmera. A alvenaria de fundo continua de pé e recebe
        // luz rasante. O perfil frontal baixo mostra simultaneamente os dois pisos.
        ParedeSociedade("Parede norte", new Vector2(15f, 17.90f), new Vector2(29.4f, .24f), fundo, 2.64f);
        ParedeSociedade("Parede oeste do salão", new Vector2(.26f, 9.16f), new Vector2(.22f, 17.59f), 0f, 1.25f);
        ParedeSociedade("Parede leste", new Vector2(29.72f, 9.16f), new Vector2(.22f, 17.59f), fundo, 1.83f);
        ParedeSociedade("Mureta frontal oeste", new Vector2(2.10f, .32f), new Vector2(3.70f, .20f), 0f, .46f);
        ParedeSociedade("Mureta frontal leste", new Vector2(17.92f, .32f), new Vector2(23.42f, .20f), 0f, .46f);
        ParedeSociedade("Divisória oeste da escada", new Vector2(3.50f, 7.80f), new Vector2(.60f, .26f), fundo, 1.26f);
        ParedeSociedade("Divisória baixa da cancha", new Vector2(17.60f, 7.80f), new Vector2(24f, .26f), fundo, 1.18f);
        ParedeSociedade("Contenção da cabeceira", new Vector2(3.20f, 12.55f), new Vector2(.31f, 9.50f), fundo, 1.25f);

        foreach (float x in new[] { 3.58f, 5.82f })
        {
            Box("Mureta ao lado da escada", new Vector2(x, 7.65f), new Vector3(.19f, .67f, 2.70f), fundo, LambrilSociedade);
            Box("Corrimão de madeira", new Vector2(x, 7.65f), new Vector3(.20f, .075f, 2.70f), .20f, MadeiraSociedade);
        }

        Box("Soleira de entrada", new Vector2(5f, .42f), new Vector3(1.78f, .08f, .70f), -.077f, new Color(.36f, .33f, .29f));
        Box("Capacho junto à saída", new Vector2(5f, .96f), new Vector3(1.52f, .018f, .68f), .002f, new Color(.28f, .22f, .15f));
        foreach (float x in new[] { 4.03f, 5.97f })
            Box("Ombreira da entrada", new Vector2(x, .34f), new Vector3(.19f, 1.64f, .27f), 0f, MadeiraSociedade);

        foreach (float x in new[] { 10f, 19f, 26f })
            Box("Montante de pinho da cancha", new Vector2(x, 17.68f), new Vector3(.22f, 2.62f, .23f), fundo, EstruturaSociedade);

        foreach (float x in new[] { 8f, 24f, 28f })
            JanelaSociedade(x);
        QuadroSociedade("Mural da cancha", "Bocha/MuralSociedade", new Vector2(17.7f, 17.74f), 2.18f, 1.17f);
        QuadroSociedade("Placar de giz", "Bocha/PlacarSociedade", new Vector2(6.3f, 17.74f), 1.67f, 1.03f);
    }

    private void ParedeSociedade(string nome, Vector2 p, Vector2 plano, float baseY, float altura)
    {
        Box(nome, p, new Vector3(plano.x, altura, plano.y), baseY, RebocoSociedade);
        bool horizontal = plano.x > plano.y;
        float lambril = Mathf.Min(.72f, altura - .08f);
        var dimensoes = new Vector3(plano.x + (horizontal ? 0 : .035f), lambril,
            plano.y + (horizontal ? .035f : 0));
        Box("Lambril · " + nome, p, dimensoes, baseY, LambrilSociedade);
        dimensoes.y = .045f;
        Box("Friso · " + nome, p, dimensoes, baseY + lambril, MadeiraSociedade);
    }

    private void JanelaSociedade(float x)
    {
        Vector2 p = new Vector2(x, 17.75f);
        Box("Vidro frio da janela", p, new Vector3(1.47f, .90f, .075f), 1.02f, new Color(.18f, .28f, .37f));
        foreach (float dx in new[] { -.75f, 0f, .75f })
            Box("Montante da janela", p + new Vector2(dx, -.055f), new Vector3(.07f, .99f, .10f), .97f, EstruturaSociedade);
        foreach (float y in new[] { .97f, 1.48f, 1.91f })
            Box("Travessa da janela", p + Vector2.down * .055f, new Vector3(1.58f, .075f, .11f), y, MadeiraSociedade);
    }

    private void MobiliaSalaoSociedade()
    {
        SpriteProp("Copa de água quente", "Art/Props/ApoioAguaQuente", new Vector2(2.50f, 4.80f), 3f);
        SpriteProp("Mesa de chimarrão", "Art/Props/Chimarrao", new Vector2(9f, 3.80f), 3.20f);
        SpriteProp("Armário de material", "Art/Props/ArmarioMadeira", new Vector2(24.40f, 5.30f), 2.20f);
        SpriteProp("Caixa de manutenção", "Art/Props/CaixasManutencao", new Vector2(26.10f, 5.15f), 1.03f);
        BancoSociedade(new Vector2(2f, 6.20f), 2.2f, 0, false);
        foreach (float x in new[] { 11.8f, 20.4f })
            BancoSociedade(new Vector2(x, 5.2f), 2.8f, 0, false);

        var mesa = new Vector2(15.80f, 4.94f);
        Box("Mesa de visitas", mesa, new Vector3(2.10f, .10f, .80f), .84f, MadeiraSociedade);
        foreach (float dx in new[] { -.91f, .91f })
            foreach (float dy in new[] { -.28f, .28f })
                Box("Pé da mesa de visitas", mesa + new Vector2(dx, dy), new Vector3(.10f, .84f, .10f), 0f, EstruturaSociedade);
        Vector2 caderno = mesa + new Vector2(.28f, -.11f);
        Box("visitas_sociedade", caderno, new Vector3(.40f, .05f, .31f), .94f, new Color(.38f, .28f, .19f));
        Box("Páginas do caderno", caderno, new Vector3(.365f, .014f, .27f), .99f, new Color(.65f, .59f, .43f));
        Box("Vinco do caderno", caderno, new Vector3(.012f, .009f, .26f), 1.003f, new Color(.30f, .27f, .21f));
        QuadroSociedade("Mural do salão", "Bocha/MuralSociedade", new Vector2(15.8f, 7.60f), 1.83f, .55f);
        QuadroSociedade("Flâmula da Sociedade", "Bocha/FlamulaSociedade", new Vector2(20.40f, 7.60f), .61f, .49f);
    }

    private void MobiliaCanchaSociedade(Texture2D madeira)
    {
        float fundo = -DesnivelSociedade;
        foreach (float y in new[] { 9.38f, 15.12f })
        {
            var guia = Box("Guia da cancha", new Vector2(16.5f, y), new Vector3(19.4f, .30f, .24f), fundo, MadeiraSociedade);
            TexturaSociedade(guia, madeira, new Vector2(13, 1));
            Box("Acabamento da guia", new Vector2(16.5f, y), new Vector3(19.4f, .035f, .26f),
                fundo + .30f, new Color(.48f, .36f, .24f));
        }
        foreach (float x in new[] { 6.86f, 26.14f })
        {
            var cabeceira = Box("Cabeceira de madeira da cancha", new Vector2(x, 12.25f), new Vector3(.28f, .43f, 6f),
                fundo, MadeiraSociedade);
            TexturaSociedade(cabeceira, madeira, new Vector2(1, 4));
        }
        Box("Linha de lançamento", new Vector2(8f, 12.25f), new Vector3(.046f, .006f, 5.42f),
            fundo + .004f, new Color(.63f, .55f, .41f));
        foreach (float x in new[] { 13f, 18f, 23f })
            foreach (float y in new[] { 9.72f, 14.78f })
                Box("Marca gasta do saibro", new Vector2(x, y), new Vector3(.03f, .005f, .30f), fundo + .003f,
                    new Color(.54f, .47f, .35f));

        foreach (float x in new[] { 10f, 14f, 18f, 22f })
        {
            BancoSociedade(new Vector2(x, 8.43f), 2.65f, fundo, true);
            BancoSociedade(new Vector2(x, 16.27f), 2.65f, fundo, false);
        }
        SpriteSociedadeElevado("cordas", "Art/Props/BancadaBochas", new Vector2(4f, 10.1f), 1.60f, fundo);
        SpriteSociedadeElevado("regra_bocha", "Art/Props/SuporteBochas", new Vector2(4f, 15.7f), 1.70f, fundo);
        Box("bocha", new Vector2(6.86f, 11.50f), new Vector3(.30f, .035f, .66f), fundo + .43f, MadeiraSociedade);
    }

    private void BancoSociedade(Vector2 p, float largura, float baseY, bool encostoSul)
    {
        Box("Assento do banco", p, new Vector3(largura, .12f, .56f), baseY + .41f, MadeiraSociedade);
        Box("Encosto do banco", p + Vector2.up * (encostoSul ? -.24f : .24f),
            new Vector3(largura, .47f, .10f), baseY + .49f, MadeiraSociedade);
        foreach (float dx in new[] { -.39f * largura, .39f * largura })
            Box("Pé do banco", p + Vector2.right * dx, new Vector3(.16f, .41f, .44f), baseY, EstruturaSociedade);
    }

    private void QuadroSociedade(string nome, string arte, Vector2 p, float largura, float altura)
    {
        SpriteSociedadeElevado(nome, arte, p, largura, altura);
    }

    private void SpriteSociedadeElevado(string nome, string arte, Vector2 p, float largura, float altura)
    {
        var objeto = SpriteProp(nome, arte, p, largura);
        if (objeto != null)
        {
            Vector3 posicao = objeto.transform.position;
            posicao.y = altura;
            objeto.transform.position = posicao;
        }
    }

    private void LuzesSociedade()
    {
        foreach (float x in new[] { 7f, 15.8f, 23.5f })
            Lamp("Luz baixa do salão", new Vector2(x, 4.7f), 2.34f, LuzSociedade, 1.5f, 6.5f);
        Lamp("Luz da escada", new Vector2(4.70f, 7.50f), 2.13f, LuzSociedade, 1.18f, 4.8f);
        Lamp("Luz da bancada", new Vector2(4.20f, 13.56f), 1.84f, LuzSociedade, 1.2f, 5.8f);
        foreach (float x in new[] { 9.4f, 16.5f, 23.9f })
        {
            var p = new Vector2(x, 12.25f);
            Box("Haste do pendente", p, new Vector3(.04f, .72f, .04f), 2.52f, EstruturaSociedade);
            Box("Carcaça do pendente", p, new Vector3(.82f, .15f, .34f), 2.37f, EstruturaSociedade);
            Box("Difusor do pendente", p, new Vector3(.71f, .039f, .27f), 2.34f, new Color(1f, .80f, .53f));
            Lamp("Pendente da cancha", p, 2.29f, LuzSociedade, 2.35f, 7.3f);
        }
        foreach (float x in new[] { 8f, 24f, 28f })
            Lamp("Claridade fria da janela", new Vector2(x, 17.37f), 1.52f, new Color(.43f, .59f, .76f), .50f, 3.2f);
    }
}

