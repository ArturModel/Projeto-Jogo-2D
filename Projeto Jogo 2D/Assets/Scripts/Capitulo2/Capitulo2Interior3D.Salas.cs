using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarDeposito()
    {
        Color ferro = new(.24f, .30f, .28f);
        Color pinho = new(.45f, .31f, .19f);
        CascaDaSala(16, 14, new(.40f, .43f, .40f), new(.54f, .52f, .43f), ferro, false, true);

        // O corredor contorna a mesa de conferência e continua até a mesma loja.
        foreach (float z in new[] { 3.5f, 7f, 10.5f })
        {
            Estrado(2.5f, z, pinho);
            SpriteProp("Cargas secas", "Art/Props/EstoqueDeposito", new(2.5f, z + .03f), 2.3f)
                .transform.position += Vector3.up * .15f;
        }
        Estante("Estoque de secos", 13.3f, 9.6f, 2.62f, ferro, pinho);
        for (int i = 0; i < 5; i++)
            CaixaDePapel("Caixa de papelão", 12.4f + i * .45f, 9.58f, i % 2 == 0 ? .67f : 1.34f, .38f, .37f, .42f);
        Estrado(13.4f, 4.1f, pinho);
        SpriteProp("Vasilhames retornáveis", "Art/RuaNorte/Bar/Detalhes/EngradadosRetornaveis", new(13.4f, 4.1f), 1.74f)
            .transform.position += Vector3.up * .15f;
        BancadaDaSala("Conferência de carga", 8, 8.4f, 2.56f, 1.02f, pinho, ferro);
        PapelDaSala("romaneio", 7.83f, 8.3f, .89f, .36f, .27f, new(.67f, .59f, .40f));
        Box("Lápis do conferente", new(8.18f, 8.25f), new(.045f, .03f, .28f), .90f, new(.55f, .36f, .16f));
        Box("Travessa da porta de serviço", new(8, 12.47f), new(1.93f, .17f, .30f), 2.32f, pinho);
        foreach (float x in new[] { 7.10f, 8.90f })
            Box("Batente da porta de serviço", new(x, 12.47f), new(.14f, 2.32f, .30f), 0, pinho);
        Box("Folha fechada da loja", new(8, 12.60f), new(1.67f, 2.31f, .12f), .04f, new(.22f, .28f, .25f));
        Box("Maçaneta da loja", new(8.61f, 12.52f), new(.11f, .07f, .10f), 1.02f, new(.65f, .52f, .29f));
        Box("Soleira de serviço", new(8, 11.96f), new(1.81f, .035f, .52f), .003f, new(.41f, .36f, .28f));

        LuzDaSala("Luz de recebimento", new(8, 4.0f), new(.86f, .90f, .82f), 2.9f, 7.8f);
        LuzDaSala("Luz da conferência", new(8, 8.15f), new(1f, .80f, .55f), 4.0f, 7.0f);
        LuzDaSala("Luz da porta de serviço", new(8.0f, 11.5f), new(1f, .74f, .48f), 2.0f, 4.2f);
    }

    private void MontarOficina()
    {
        Color azul = new(.24f, .37f, .40f);
        Color freijo = new(.41f, .28f, .18f);
        CascaDaSala(16, 13, new(.43f, .33f, .24f), new(.53f, .51f, .42f), azul, true);
        BancadaDaSala("Bancada de Mila", 9.5f, 7.9f, 3.50f, 1.02f, freijo, azul);
        Box("Tapete isolante da bancada", new(9.52f, 7.86f), new(1.43f, .018f, .72f), .884f, new(.18f, .25f, .23f));
        RadioDaOficina("radio_test", 9.5f, 7.91f, .90f, .88f);
        RadioDaOficina("Segundo rádio aguardando reparo", 10.68f, 8.11f, .89f, .65f);
        Box("Ferro de solda", new(8.47f, 7.67f), new(.40f, .046f, .062f), .895f, new(.21f, .18f, .16f));
        Box("Ponta do ferro", new(8.69f, 7.67f), new(.12f, .025f, .023f), .90f, new(.46f, .48f, .45f));
        Box("Suporte do ferro", new(8.55f, 7.67f), new(.31f, .04f, .17f), .884f, azul);
        Box("Braço da luminária", new(8.37f, 8.22f), new(.043f, .52f, .043f), .88f, azul);
        Box("Cúpula metálica", new(8.53f, 8.18f), new(.34f, .11f, .24f), 1.38f, azul);
        Lamp("Luz de reparo", new(8.53f, 8.10f), 1.36f, new(1f, .77f, .48f), 1.7f, 2.5f);

        CaixaDePapel("Entrega para orçamento", 12.6f, 3.1f, 0, 1.49f, .74f, .45f);
        SpriteProp("Televisor aguardando orçamento", "Art/Props/Televisor", new(12.6f, 3.16f), 1.42f)
            .transform.position += Vector3.up * .45f;
        SpriteProp("Banco da espera", "Art/Props/BancoCancha", new(3.4f, 3.3f), 2.4f);
        Estante("Peças catalogadas", 3, 8.8f, 1.94f, azul, freijo);
        for (int i = 0; i < 4; i++)
            Box("Gaveta de componentes", new(2.30f + .47f * i, 8.69f), new(.42f, .28f, .41f), .67f, azul * (1 + .05f * i));
        ArmarioDaSala("Arquivo de esquemas", 13.5f, 8.6f, 1.88f, .76f, 1.77f, freijo);
        PapelDaSala("desenho_radio", 13.5f, 8.40f, 1.78f, .46f, .38f, new(.72f, .67f, .48f));

        LuzDaSala("Pendente da oficina", new(9.1f, 7.6f), new(1f, .84f, .66f), 3.55f, 6.3f);
        LuzDaSala("Pendente da espera", new(5.0f, 3.7f), new(.83f, .88f, 1f), 2.4f, 6.9f);
    }

    private void MontarCostureira()
    {
        Color verde = new(.30f, .38f, .31f);
        Color cedro = new(.39f, .25f, .17f);
        CascaDaSala(16, 14, new(.46f, .34f, .25f), new(.64f, .53f, .41f), verde, true);
        TapeteDaSala(10.80f, 4.50f, 4.85f, 2.58f, new(.43f, .23f, .20f), new(.70f, .52f, .36f));
        SpriteProp("Sofá da sala", "Art/Props/Sofa", new(12.8f, 3.7f), 2.92f);
        SpriteProp("Mesa do chá", "Art/Props/Chimarrao", new(11.9f, 8.7f), 2.25f);
        ArmarioDaSala("Armário de tecido", 3, 10.1f, 1.85f, .74f, 1.94f, cedro);
        BancadaDaSala("Mesa de corte", 7.8f, 8.9f, 3.44f, 1.05f, cedro, new(.27f, .24f, .20f));
        Box("Tecido em corte", new(8.40f, 8.92f), new(1.39f, .028f, .72f), .885f, new(.54f, .31f, .28f));
        Box("Dobra do tecido", new(8.77f, 9.16f), new(.86f, .047f, .13f), .91f, new(.59f, .36f, .31f));
        PapelDaSala("encomenda_faixa", 7.68f, 8.68f, .895f, .39f, .28f, new(.77f, .68f, .49f));
        Box("Base da máquina", new(6.79f, 9.06f), new(.80f, .07f, .35f), .89f, cedro);
        Color esmalte = new(.14f, .16f, .15f);
        Box("Coluna da máquina", new(7.01f, 9.06f), new(.17f, .36f, .16f), .96f, esmalte);
        Box("Braço da máquina", new(6.80f, 9.06f), new(.54f, .15f, .14f), 1.22f, esmalte);
        Box("Cabeçote", new(6.56f, 9.06f), new(.14f, .21f, .15f), 1.13f, esmalte);
        Box("Agulha", new(6.53f, 9.05f), new(.015f, .17f, .015f), .98f, new(.56f, .57f, .51f));
        Box("Rolo de linha", new(7.01f, 9.06f), new(.07f, .09f, .07f), 1.38f, new(.75f, .64f, .44f));
        Box("Fita métrica", new(8.63f, 8.63f), new(.95f, .017f, .044f), .914f, new(.79f, .65f, .34f));
        CaixaDePapel("Caixa de retalhos junto ao armário", 3.08f, 9.8f, 0, .72f, .57f, .32f);
        QuadroDaSala("Retrato na sala", 8, 12.61f, 1.55f, .84f, .63f, cedro, new(.55f, .42f, .35f));

        LuzDaSala("Pendente da mesa de corte", new(7.55f, 8.45f), new(1f, .82f, .60f), 3.8f, 6.5f);
        LuzDaSala("Pendente da sala", new(11.7f, 4.9f), new(1f, .69f, .43f), 2.9f, 6.5f);
    }

    private void MontarAbrigo()
    {
        Color cal = new(.59f, .54f, .44f);
        Color peroba = new(.39f, .28f, .19f);
        CascaDaSala(13, 11, new(.39f, .33f, .26f), cal, new(.33f, .33f, .27f), true);
        Estante("apoio_cemiterio", 2.3f, 6.6f, 1.98f, peroba, peroba);
        for (int i = 0; i < 6; i++)
            CaixaDePapel("Caixa fechada de velas", 1.54f + .29f * i, 6.6f, .15f, .26f, .35f, .24f);
        for (int i = 0; i < 7; i++)
            VelaDaRonda(1.60f + .23f * i, 6.53f, 1.34f, .13f + (i % 3) * .045f, false);

        BancadaDaSala("Mesa da ronda", 9.2f, 6.7f, 2.20f, .91f, peroba, new(.27f, .22f, .17f));
        Box("Capa do caderno", new(9.14f, 6.65f), new(.52f, .036f, .34f), .886f, new(.25f, .28f, .24f));
        PapelDaSala("caderno_velas", 9.14f, 6.65f, .925f, .49f, .30f, new(.77f, .68f, .50f));
        Box("Lombo do caderno", new(9.14f, 6.65f), new(.02f, .013f, .30f), .953f, new(.49f, .40f, .28f));
        Box("Pires metálico", new(9.89f, 6.84f), new(.35f, .03f, .29f), .886f, new(.31f, .31f, .28f));
        VelaDaRonda(9.89f, 6.84f, .916f, .19f, true);
        SpriteProp("Banco da ronda", "Art/Props/BancoCancha", new(9.5f, 2.8f), 2.4f);
        SpriteProp("Caixa de ferramentas", "Art/Props/CaixasManutencao", new(2.3f, 3.1f), 1.4f);

        LuzDaSala("Pendente da ronda", new(8.6f, 6.3f), new(1f, .77f, .52f), 2.8f, 6.0f);
        LuzDaSala("Pendente da soleira", new(6.5f, 2.45f), new(.89f, .81f, .64f), 1.4f, 5.4f);
    }

    private void CascaDaSala(float largura, float fundo, Color piso, Color reboco, Color lambril, bool tabuas, bool portaServico = false)
    {
        const float parede = 2.65f;
        float norte = fundo - 1.02f;
        float centro = largura * .5f;
        var pisoBase = Box("Piso de apoio", new(centro, norte * .5f), new(largura, .12f, norte), -.121f, piso * .72f);
        var materialPiso = new MaterialPropertyBlock();
        var renderer = pisoBase.GetComponent<Renderer>();
        var tile = Resources.Load<Texture2D>(tabuas ? "Art/Tiles/Madeira" : "Art/Tiles/PisoCinza");
        if (tile != null && renderer != null)
        {
            renderer.GetPropertyBlock(materialPiso);
            var repeticao = Instantiate(tile);
            repeticao.wrapMode = TextureWrapMode.Repeat;
            repeticao.filterMode = FilterMode.Point;
            owned.Add(repeticao);
            materialPiso.SetTexture("_BaseMap", repeticao);
            materialPiso.SetVector("_BaseMap_ST", new(largura, norte, 0, 0));
            renderer.SetPropertyBlock(materialPiso);
        }
        else
        {
            float passo = tabuas ? .51f : 1.12f;
            for (int i = 0; i < Mathf.CeilToInt(largura / passo); i++)
            {
                float x = i * passo;
                float comprimento = Mathf.Min(passo - .021f, largura - x);
                if (comprimento <= 0) break;
                Box(tabuas ? "Tábua do piso" : "Faixa de ladrilho", new(x + comprimento * .5f, norte * .5f),
                    new(comprimento, .015f, norte), -.015f, piso * (.91f + .026f * (i * 7 % 6)));
            }
        }

        foreach (float x in new[] { .22f, largura - .22f })
        {
            Box("Parede lateral cortada", new(x, norte * .5f), new(.44f, .64f, norte), 0, reboco * .76f);
            Box("Arremate da parede lateral", new(x, norte * .5f), new(.47f, .038f, norte), .64f, lambril);
        }

        float abertura = centro - 1;
        Box("Parede sul esquerda", new(abertura * .5f, .30f), new(abertura, .40f, .40f), 0, reboco * .78f);
        Box("Parede sul direita", new(largura - abertura * .5f, .30f), new(abertura, .40f, .40f), 0, reboco * .78f);
        Box("Soleira de entrada", new(centro, .53f), new(1.93f, .032f, .48f), .003f, lambril * .78f);
        TapeteDaSala(centro, 1.09f, 1.62f, .73f, lambril * .58f, lambril * .90f);

        float faixa = portaServico ? (largura - 1.95f) * .5f : largura;
        Box("Alvenaria norte", new(faixa * .5f, norte), new(faixa, parede, .33f), 0, reboco);
        Box("Lambril norte", new(faixa * .5f, norte - .19f), new(faixa, .76f, .07f), 0, lambril);
        Box("Friso norte", new(faixa * .5f, norte - .22f), new(faixa, .057f, .07f), .75f, lambril * 1.24f);
        if (portaServico)
        {
            Box("Alvenaria norte direita", new(largura - faixa * .5f, norte), new(faixa, parede, .33f), 0, reboco);
            Box("Lambril norte direito", new(largura - faixa * .5f, norte - .19f), new(faixa, .76f, .07f), 0, lambril);
            Box("Friso norte direito", new(largura - faixa * .5f, norte - .22f), new(faixa, .057f, .07f), .75f, lambril * 1.24f);
            Box("Verga da porta de serviço", new(centro, norte), new(1.95f, .25f, .33f), 2.40f, reboco);
        }

        foreach (float x in new[] { 2.35f, largura - 2.35f })
        {
            QuadroDaSala("Janela de madeira", x, norte - .19f, 1.10f, 1.66f, 1.12f, lambril * .59f, new(.19f, .28f, .31f));
            Box("Travessa do caixilho", new(x, norte - .27f), new(1.52f, .036f, .028f), 1.65f, lambril * 1.14f);
            Box("Montante do caixilho", new(x, norte - .27f), new(.045f, .96f, .028f), 1.18f, lambril * 1.14f);
            Box("Peitoril", new(x, norte - .27f), new(1.79f, .065f, .23f), 1.07f, reboco * .82f);
        }
    }

    private void LuzDaSala(string nome, Vector2 position, Color color, float intensity, float range)
    {
        Box("Cabo da " + nome, position, new(.036f, .47f, .036f), 2.70f, new(.20f, .20f, .18f));
        Box("Cúpula da " + nome, position, new(.50f, .10f, .37f), 2.64f, new(.37f, .36f, .29f));
        Box("Difusor da " + nome, position, new(.40f, .041f, .28f), 2.60f, color);
        Lamp(nome, position, 2.57f, color, intensity, range);
    }

    private void BancadaDaSala(string nome, float x, float z, float largura, float profundidade, Color tampo, Color pernas)
    {
        Box(nome, new(x, z), new(largura, .09f, profundidade), .795f, tampo);
        foreach (float lado in new[] { -1f, 1f })
        foreach (float frente in new[] { -1f, 1f })
            Box("Pé de bancada", new(x + lado * (largura * .5f - .13f), z + frente * (profundidade * .5f - .11f)),
                new(.095f, .795f, .095f), 0, pernas);
        Box("Travessa de bancada", new(x, z + profundidade * .36f), new(largura - .15f, .08f, .09f), .22f, pernas);
    }

    private void Estante(string nome, float x, float z, float largura, Color estrutura, Color prateleira)
    {
        Box(nome, new(x, z + .30f), new(largura, 1.91f, .075f), .07f, estrutura);
        foreach (float lado in new[] { -1f, 1f })
            Box("Lateral de estante", new(x + lado * (largura * .5f - .035f), z), new(.07f, 1.97f, .66f), 0, estrutura);
        foreach (float y in new[] { .095f, .61f, 1.28f, 1.91f })
            Box("Prateleira", new(x, z), new(largura, .055f, .66f), y, prateleira);
    }

    private void Estrado(float x, float z, Color madeira)
    {
        foreach (float lado in new[] { -.67f, 0, .67f })
            Box("Sapata do estrado", new(x + lado, z), new(.17f, .10f, .88f), 0, madeira * .74f);
        for (int i = -2; i <= 2; i++)
            Box("Ripa do estrado", new(x, z + i * .17f), new(2.15f, .047f, .145f), .10f, madeira);
    }

    private void ArmarioDaSala(string nome, float x, float z, float largura, float profundidade, float altura, Color cor)
    {
        Box(nome, new(x, z), new(largura, altura, profundidade), 0, cor * .80f);
        float frente = z - profundidade * .5f;
        foreach (float lado in new[] { -1f, 1f })
        {
            Box("Folha do armário", new(x + lado * largura * .25f, frente), new(largura * .48f, altura - .15f, .041f), .07f, cor);
            Box("Puxador do armário", new(x + lado * .078f, frente - .045f), new(.032f, .17f, .060f), altura * .53f, new(.55f, .48f, .32f));
        }
    }

    private void CaixaDePapel(string nome, float x, float z, float y, float largura, float profundidade, float altura)
    {
        Box(nome, new(x, z), new(largura, altura, profundidade), y, new(.48f, .36f, .23f));
        Box("Fita da caixa", new(x, z), new(largura * .13f, .008f, profundidade + .008f), y + altura, new(.60f, .51f, .35f));
        Box("Etiqueta da caixa", new(x + largura * .21f, z - profundidade * .50f),
            new(largura * .27f, altura * .20f, .009f), y + altura * .57f, new(.68f, .62f, .46f));
    }

    private void PapelDaSala(string nome, float x, float z, float y, float largura, float profundidade, Color cor)
    {
        Box(nome, new(x, z), new(largura, .023f, profundidade), y, cor);
        for (int i = 0; i < 4; i++)
            Box("Linhas do papel", new(x - largura * .045f, z - profundidade * .23f + i * profundidade * .14f),
                new(largura * (i == 2 ? .43f : .65f), .004f, .010f), y + .023f, cor * .59f);
    }

    private void RadioDaOficina(string nome, float x, float z, float y, float largura)
    {
        Box(nome, new(x, z), new(largura, .29f, .31f), y, new(.25f, .23f, .19f));
        Box("Mostrador do rádio", new(x - largura * .13f, z - .159f), new(largura * .56f, .076f, .012f), y + .16f, new(.44f, .48f, .36f));
        for (int i = 0; i < 4; i++)
            Box("Ranhura do alto-falante", new(x - largura * .13f, z - .163f), new(largura * .57f, .011f, .012f), y + .052f + .025f * i, new(.14f, .15f, .14f));
        Box("Seletor de faixa", new(x + largura * .31f, z - .17f), new(.10f, .10f, .045f), y + .085f, new(.52f, .49f, .38f));
    }

    private void VelaDaRonda(float x, float z, float y, float altura, bool acesa)
    {
        Box("Vela branca", new(x, z), new(.064f, altura, .065f), y, new(.75f, .68f, .50f));
        Box("Pavio", new(x, z), new(.012f, .025f, .012f), y + altura, new(.22f, .20f, .16f));
        if (!acesa) return;
        Box("Chama", new(x, z), new(.037f, .077f, .034f), y + altura + .017f, new(1f, .70f, .27f));
        Lamp("Reflexo da vela no caderno", new(x, z), y + altura + .077f, new(1f, .60f, .30f), .76f, 2.6f);
    }

    private void QuadroDaSala(string nome, float x, float z, float y, float largura, float altura, Color moldura, Color miolo)
    {
        Box(nome, new(x, z), new(largura, altura, .079f), y, moldura);
        Box("Miolo da moldura", new(x, z - .046f), new(largura - .13f, altura - .12f, .024f), y + .06f, miolo);
    }

    private void TapeteDaSala(float x, float z, float largura, float profundidade, Color centro, Color margem)
    {
        Box("Barra do tapete", new(x, z), new(largura, .009f, profundidade), .004f, margem);
        Box("Trama do tapete", new(x, z), new(largura - .16f, .008f, profundidade - .16f), .014f, centro);
        foreach (float lado in new[] { -1f, 1f })
            Box("Franja do tapete", new(x, z + lado * (profundidade * .5f + .035f)), new(largura - .13f, .008f, .068f), .004f, centro * 1.22f);
    }
}

