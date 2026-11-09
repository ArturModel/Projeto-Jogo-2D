using UnityEngine;
using UnityEngine.Rendering;
using Tipo = InteracaoCapitulo1.TipoInteracao;

internal sealed partial class Posto3D
{
    private const float PortaDaCasaEntreaberta = -14f;
    private Light lanternaCasa;
    private int variantePasso;
    private float distanciaPasso;
    private static readonly Vector3 ChegadaCasa = new Vector3(.02f, EyeHeight, -2.95f);

    private void ConstruirCasaDoAvo()
    {
        var madeira = Resources.Load<Texture2D>("Art/Tiles/Madeira");
        var assoalho = Superficie("Tábuas antigas de pinho", new Color(.77f, .63f, .47f),
            SantaRitaBarLayout.NativeSurface(false).texture, new Vector2(8, 7));
        var teto = Superficie("Forro de pinho", new Color(.61f, .55f, .44f), madeira, new Vector2(7, 4));
        var papel = Superficie("Papel de parede desbotado", Color.white, PapelDaCasa(), new Vector2(8, 1));
        var lambril = Superficie("Lambril de madeira", new Color(.49f, .37f, .26f), madeira, new Vector2(6, 1));
        var nogueira = Superficie("Nogueira escura", new Color(.42f, .26f, .17f), madeira);
        var marrom = Superficie("Madeira encerada", new Color(.65f, .47f, .32f), madeira);
        var pintura = Superficie("Tinta verde antiga", new Color(.29f, .36f, .26f), madeira);
        var osso = Superficie("Esmalte marfim", new Color(.82f, .77f, .62f));
        var ferro = Superficie("Ferro", new Color(.17f, .17f, .17f));
        var latao = Superficie("Latão da maçaneta", new Color(.56f, .40f, .23f));
        var tecido = Superficie("Tecido vinho", new Color(.48f, .23f, .19f), PapelDaCasa());
        var vermelho = Superficie("Vermelho gasto do tapete", new Color(.50f, .28f, .23f), madeira);
        var creme = Superficie("Fibra clara da borda", new Color(.70f, .54f, .38f));
        var luz = Superficie("Cúpula de vidro opalino", new Color(.93f, .72f, .49f), unlit: true);
        var cozinha = Superficie("Ladrilho da cozinha", Color.white,
            CriarCeramica(new Color(.62f, .59f, .49f), new Color(.34f, .32f, .30f)), new Vector2(3, 2));

        Caixa("Assoalho", V(0, -.075f, .23f), V(11.8f, .15f, 9.49f), assoalho);
        Caixa("Piso da cozinha", V(-3.94f, .006f, 2.74f), V(3.75f, .012f, 4.22f), cozinha, solida: false);
        Caixa("Forro", V(0, 2.98f, .23f), V(11.8f, .11f, 9.49f), teto);
        ParedeComJanela(-5.9f, 5.9f, 4.91f, .14f, papel,
            new Rect(-4.855f, 1.165f, 1.47f, 1.09f), new Rect(2.905f, 1.165f, 1.47f, 1.09f));
        Caixa("Lambril dos fundos", V(0, .47f, 4.81f), V(11.68f, .94f, .07f), lambril, solida: false);
        Caixa("Rodameio", V(0, .96f, 4.76f), V(11.68f, .07f, .07f), nogueira, solida: false);
        foreach (float lado in new[] { -1f, 1f })
        {
            Caixa("Parede lateral", V(lado * 5.87f, 1.50f, .23f), V(.13f, 3f, 9.49f), papel);
            Caixa("Lambril lateral", V(lado * 5.78f, .47f, .23f), V(.06f, .94f, 9.35f), lambril, solida: false);
            Caixa("Rodameio lateral", V(lado * 5.73f, .96f, .23f), V(.065f, .07f, 9.35f), nogueira, solida: false);
            float janelaX = lado < 0f ? -3.25f : 3.16f;
            ParedeComJanela(lado * 3.26f - 2.63f, lado * 3.26f + 2.63f, Soleira, .14f, papel,
                new Rect(janelaX - .735f, 1.165f, 1.47f, 1.09f));
            Caixa("Tábua de beiral", V(lado * 3.26f, 2.69f, Soleira - .26f), V(5.26f, .28f, .47f), nogueira, solida: false);
            Caixa("Batente da casa", V(lado * .62f, 1.12f, Soleira), V(.10f, 2.24f, .22f), pintura);
        }
        Caixa("Verga da porta", V(0, 2.59f, Soleira), V(1.26f, .82f, .15f), papel);
        Caixa("Varanda molhada", V(0, -.125f, -5.32f), V(11.8f, .18f, 1.84f), PisoMolhado(assoalho));
        Caixa("Soleira de madeira", V(0, -.012f, Soleira), V(1.22f, .049f, .43f), nogueira, solida: false);
        ConstruirPortaDaCasa(pintura, nogueira, latao);
        ConstruirChuva();

        foreach (float z in new[] { -2.68f, 2.77f })
        {
            Caixa("Viga", V(0, 2.84f, z), V(11.61f, .21f, .20f), lambril, solida: false);
            Caixa("Haste da luminária", V(-.50f, 2.65f, z), V(.028f, .20f, .028f), ferro, solida: false);
            Caixa("Luminária doméstica", V(-.50f, 2.53f, z), V(.38f, .085f, .38f), luz, solida: false);
            var lampada = Luz("Luz quente da casa", V(-.50f, 2.44f, z), new Color(1f, .76f, .48f), z < 0f ? 2.65f : 1.90f, 4.35f);
            if (z < 0f) luzDaSala = lampada;
        }
        Luz("Luz da cozinha", V(-4.00f, 2.40f, 3.38f), new Color(1f, .82f, .60f), 1.9f, 3.85f);
        Luz("Luz do quarto", V(4.22f, 2.43f, 2.47f), new Color(1f, .73f, .46f), 1.45f, 3.55f);
        Luz("Lâmpada da varanda", V(.25f, 2.38f, -5.35f), new Color(1f, .77f, .51f), 3.0f, 4.2f);
        lanternaCasa = new GameObject("Lanterna de Marcus 3D").AddComponent<Light>();
        lanternaCasa.transform.SetParent(vista.transform, false);
        lanternaCasa.transform.localPosition = V(.14f, -.12f, .12f);
        lanternaCasa.type = LightType.Spot;
        lanternaCasa.spotAngle = 42f;
        lanternaCasa.innerSpotAngle = 16f;
        lanternaCasa.cullingMask = RoomMask;
        lanternaCasa.color = new Color(.93f, .96f, 1f);
        lanternaCasa.range = 7.2f;
        lanternaCasa.intensity = 3.5f;
        lanternaCasa.shadows = LightShadows.Soft;
        lanternaCasa.enabled = false;

        MontarJanelaCasa(-4.12f, 4.74f, nogueira, tecido);
        MontarJanelaCasa(3.64f, 4.74f, nogueira, tecido);
        MontarJanelaCasa(-3.25f, -4.34f, nogueira, tecido, 180f);
        MontarJanelaCasa(3.16f, -4.34f, nogueira, tecido, 180f);
        MontarRelogioDaCasa(nogueira, latao);
        MontarInterruptorDaCasa(osso, ferro);
        Caixa("Moldura do retrato de família", V(.54f, 1.69f, 4.71f), V(.62f, .46f, .12f), nogueira,
            tipo: Tipo.RetratoFamiliaCasaAvo);
        Arte("Retrato de Natal de 1997", "Cap1/FramedPhoto", V(.54f, 1.69f, 4.64f), .43f);

        ConstruirCozinhaCasa(marrom, nogueira, osso, ferro);
        ConstruirSalaCasa(marrom, nogueira, tecido, vermelho, creme, osso, ferro);
        ConstruirQuartoCasa(nogueira, marrom, tecido, osso);
        ConstruirDesordemCasa(nogueira, tecido, vermelho, ferro);
    }

    private void ConstruirPortaDaCasa(Material pintura, Material madeira, Material latao)
    {
        porta = new GameObject("Dobradiça esquerda da casa").transform;
        porta.SetParent(cenario, false);
        porta.localPosition = V(-.565f, 0, Soleira);
        saida = Caixa("Folha única da casa nº 40", V(.565f, 1.08f, 0), V(1.13f, 2.16f, .085f), pintura, porta)
            .GetComponent<Collider>();
        foreach (float lado in new[] { -1f, 1f })
        {
            foreach (float y in new[] { .31f, .80f, 1.29f, 1.78f })
            {
                Caixa("Moldura do almofadado", V(.565f, y, lado * .052f), V(.88f, .43f, .026f),
                    madeira, porta, solida: false);
                Caixa("Almofada de madeira", V(.565f, y, lado * .067f), V(.79f, .35f, .021f),
                    pintura, porta, solida: false);
            }
            Caixa("Chapa da fechadura", V(.99f, 1.03f, lado * .074f), V(.062f, .17f, .030f),
                latao, porta, solida: false);
            var puxador = Caixa("Maçaneta da casa", V(.99f, 1.035f, lado * .102f), V(.105f, .047f, .065f),
                latao, porta, solida: false).transform;
            if (lado < 0) macaneta = puxador;
        }
        porta.localRotation = Quaternion.Euler(0f, PortaDaCasaEntreaberta, 0f);

        var textura = Resources.Load<Texture2D>("Art/Characters/MarcusMaoMacaneta");
        if (textura == null) return;
        mao = Quadro("Mão de Marcus", Vector3.zero, V(.38f, .38f * textura.height / textura.width, 1f),
            MaterialRecorte("Mão de Marcus", textura));
        mao.gameObject.SetActive(false);
    }

    private Texture2D PapelDaCasa()
    {
        const int tamanho = 64;
        var textura = new Texture2D(tamanho, tamanho, TextureFormat.RGBA32, true)
        { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
        var cores = new Color32[tamanho * tamanho];
        for (int y = 0; y < tamanho; y++)
        for (int x = 0; x < tamanho; x++)
        {
            int faixa = x % 16;
            int grao = (x * 19 + y * 37 + x * y * 3) % 10;
            int mancha = (int)(10 * Mathf.Sin(x * .10f) * Mathf.Sin(y * .12f));
            int borda = faixa == 0 || faixa == 15 ? -18 : faixa <= 3 ? -10 : 0;
            cores[x + y * tamanho] = new Color32((byte)(195 + grao + mancha + borda),
                (byte)(174 + grao + mancha + borda), (byte)(137 + grao + mancha + borda), 255);
        }
        textura.SetPixels32(cores);
        textura.Apply();
        recursos.Add(textura);
        return textura;
    }

    private void MontarJanelaCasa(float x, float z, Material madeira, Material cortina, float yaw = 0)
    {
        var janela = new GameObject("Janela da casa").transform;
        janela.SetParent(cenario, false);
        janela.localPosition = V(x, 1.71f, z);
        janela.localRotation = Quaternion.Euler(0, yaw, 0);
        MolduraVazada(Vector3.zero, 1.47f, 1.09f, madeira, janela);
        var quadro = Quadro("Vidro da janela da casa", Vector3.zero, V(1.47f, 1.09f, 1f), VidroExterior());
        quadro.SetParent(janela, false);
        quadro.localPosition = V(0, 0, -.06f);
        Caixa("Travessa", V(0, 0, -.085f), V(1.50f, .045f, .042f), madeira, janela, false);
        Caixa("Montante", V(0, 0, -.085f), V(.045f, 1.13f, .042f), madeira, janela, false);
        Caixa("Peitoril", V(0, -.64f, -.056f), V(1.75f, .077f, .29f), madeira, janela, false);
        Caixa("Varão da cortina", V(0, .70f, -.16f), V(1.82f, .041f, .041f), madeira, janela, false);
        foreach (float lado in new[] { -1f, 1f })
        for (int i = 0; i < 4; i++)
        {
            float altura = 1.15f + .028f * i;
            Caixa("Dobra da cortina", V(lado * (.53f + .075f * i), .62f - altura * .5f, -.16f - .024f * (i % 2)),
                V(.10f, altura, .075f), cortina, janela, false);
        }
    }

    private void ConstruirCozinhaCasa(Material marrom, Material madeira, Material osso, Material ferro)
    {
        Caixa("Armário da pia", V(-3.96f, .45f, 4.19f), V(3.14f, .90f, .71f), marrom);
        Caixa("Tampo da pia", V(-3.96f, .94f, 4.16f), V(3.22f, .09f, .83f), osso);
        Caixa("Cuba antiga", V(-4.25f, 1f, 4.12f), V(.70f, .028f, .48f), ferro, solida: false);
        Caixa("Torneira", V(-4.25f, 1.09f, 4.39f), V(.06f, .18f, .065f), ferro, solida: false);
        Caixa("Bico da torneira", V(-4.25f, 1.17f, 4.31f), V(.06f, .036f, .17f), ferro, solida: false);
        for (int i = 0; i < 3; i++)
        {
            float x = -4.95f + i * .98f;
            Caixa("Porta do armário", V(x, .47f, 3.82f), V(.88f, .58f, .046f), madeira, solida: false);
            Caixa("Puxador do armário", V(x + .26f, .62f, 3.78f), V(.035f, .12f, .04f), ferro, solida: false);
        }
        Caixa("Bule e xícara", V(-2.81f, 1.10f, 4.05f), V(.65f, .26f, .43f), osso,
            tipo: Tipo.PistaCafeCasaAvo).GetComponent<MeshRenderer>().enabled = false;
        CilindroCasa("Bule esmaltado", V(-2.91f, 1.16f, 4.08f), V(.28f, .16f, .28f), osso);
        CilindroCasa("Tampa do bule", V(-2.91f, 1.34f, 4.08f), V(.28f, .035f, .28f), ferro);
        Caixa("Bico do bule", V(-3.07f, 1.22f, 4.08f), V(.19f, .055f, .064f), osso, solida: false).transform.Rotate(0, 0, -24f);
        CilindroCasa("Xícara", V(-2.58f, 1.06f, 3.99f), V(.14f, .062f, .14f), osso);
        CilindroCasa("Café pela metade", V(-2.58f, 1.124f, 3.99f), V(.113f, .003f, .113f), madeira);
        Caixa("Fogão", V(-5.36f, .48f, 2.73f), V(.75f, .96f, .96f), osso);
        var fogao = Arte("Porta e queimadores do fogão", "Art/Props/Fogao", V(-4.96f, .50f, 2.73f), .92f);
        if (fogao != null) fogao.Rotate(0f, -90f, 0f);
        Caixa("Prateleira da cozinha", V(-4.41f, 1.74f, 4.64f), V(2.48f, .065f, .29f), madeira, solida: false);
        foreach (float x in new[] { -5.25f, -5.00f, -4.70f })
            CilindroCasa("Lata de mantimentos", V(x, 1.89f, 4.59f), V(.18f, .12f, .18f), osso);
    }

    private void ConstruirSalaCasa(Material marrom, Material madeira, Material tecido, Material vermelho,
        Material creme, Material osso, Material ferro)
    {
        Caixa("Tapete da sala", V(-1.32f, .011f, -0.23f), V(4.51f, .022f, 3.55f), vermelho, solida: false);
        foreach (float lado in new[] { -1f, 1f })
        {
            Caixa("Franja do tapete", V(-1.32f, .024f, -.23f + lado * 1.70f), V(4.50f, .011f, .075f), creme, solida: false);
            Caixa("Borda do tapete", V(-1.32f + lado * 2.18f, .024f, -.23f), V(.071f, .011f, 3.38f), creme, solida: false);
        }
        Caixa("Mesa de paciência", V(-1.37f, .72f, 0.32f), V(1.90f, .11f, 1.16f), marrom);
        foreach (float x in new[] { -2.15f, -.59f })
        foreach (float z in new[] { -.13f, .78f })
            Caixa("Pé da mesa", V(x, .34f, z), V(.10f, .68f, .10f), madeira);
        Caixa("Cartas de paciência", V(-1.37f, .815f, 0.28f), V(1.29f, .088f, .64f), osso,
            tipo: Tipo.PistaCartasCasaAvo).GetComponent<MeshRenderer>().enabled = false;
        for (int carta = 0; carta < 7; carta++)
        {
            float x = -1.97f + carta * .20f;
            float z = .21f + (carta % 3) * .08f;
            Caixa("Carta de baralho", V(x, .784f, z), V(.115f, .012f, .18f), osso, solida: false)
                .transform.Rotate(0f, (carta % 4 - 1) * 9f, 0f);
            Caixa("Naipe da carta", V(x, .792f, z), V(.040f, .004f, .050f), carta % 2 == 0 ? ferro : vermelho, solida: false);
        }
        Caixa("Ás fora do monte", V(-0.79f, .784f, -0.06f), V(.113f, .012f, .179f), osso, solida: false)
            .transform.Rotate(0f, 25f, 0f);
        Caixa("Marca do ás", V(-0.79f, .792f, -0.06f), V(.043f, .004f, .050f), vermelho, solida: false);

        Caixa("Estrutura da poltrona", V(-3.16f, .29f, .94f), V(1.15f, .58f, .93f), madeira);
        Caixa("Almofada da poltrona", V(-3.16f, .57f, .91f), V(.91f, .22f, .78f), tecido, solida: false);
        Caixa("Encosto alto", V(-3.16f, .94f, 1.30f), V(1.10f, .77f, .24f), tecido, solida: false);
        foreach (float x in new[] { -3.65f, -2.67f })
            Caixa("Braço da poltrona", V(x, .66f, .92f), V(.18f, .24f, .82f), tecido, solida: false);
        Caixa("Rack da TV", V(-4.87f, .34f, -.29f), V(.72f, .68f, 1.22f), marrom);
        Caixa("TV de tubo desligada", V(-4.91f, .99f, -.30f), V(.58f, .62f, .94f), ferro);
        var tv = Arte("Frente da televisão", "Art/Props/Televisor", V(-4.60f, .99f, -.30f), .58f);
        if (tv != null) tv.Rotate(0f, -90f, 0f);
        Caixa("Antena da televisão", V(-4.86f, 1.37f, -.34f), V(.021f, .31f, .021f), ferro, solida: false).transform.Rotate(28f, 0f, 0f);
    }

    private void ConstruirQuartoCasa(Material madeira, Material marrom, Material tecido, Material osso)
    {
        Caixa("Estante que divide o quarto", V(3.56f, .62f, 2.81f), V(.43f, 1.24f, 1.45f), madeira);
        for (int i = 0; i < 3; i++)
            Caixa("Prateleira da divisória", V(3.31f, .33f + i * .31f, 2.81f), V(.09f, .055f, 1.38f), marrom, solida: false);
        Caixa("Cama de solteiro", V(4.65f, .32f, 3.38f), V(1.58f, .62f, 2.66f), madeira);
        Caixa("Cabeceira", V(4.65f, .67f, 4.68f), V(1.68f, 1.33f, .14f), madeira);
        Caixa("Colchão", V(4.65f, .69f, 3.36f), V(1.48f, .17f, 2.54f), osso, solida: false);
        Caixa("Colcha dobrada", V(4.65f, .79f, 3.03f), V(1.48f, .075f, 1.91f), tecido, solida: false);
        Caixa("Travesseiro", V(4.66f, .82f, 4.28f), V(.88f, .14f, .46f), osso, solida: false);

        Caixa("Cômoda com o cupom", V(4.60f, .48f, .57f), V(1.74f, .96f, .68f), madeira);
        Caixa("Tampo da cômoda", V(4.60f, .985f, .57f), V(1.81f, .065f, .74f), marrom);
        for (int i = 0; i < 3; i++)
        {
            float y = .23f + i * .235f;
            Caixa("Gaveta", V(4.60f, y, .21f), V(1.51f, .17f, .035f), marrom, solida: false);
            Caixa("Puxador da gaveta", V(4.60f, y, .175f), V(.14f, .031f, .043f), osso, solida: false);
        }
        Arte("Rádio antigo", "Art/Props/RadioAntigo", V(5.09f, 1.22f, .52f), .41f);
        Caixa("Cupom amassado do Mercadinho", V(4.06f, 1.055f, .40f), V(.36f, .076f, .26f), osso,
            tipo: Tipo.PistaNotaFiscal2004).transform.Rotate(0f, -18f, 0f);
    }

    private void ConstruirDesordemCasa(Material madeira, Material tecido, Material barro, Material ferro)
    {
        Caixa("Mesa lateral do telefone", V(5.06f, .40f, -3.2f), V(.97f, .80f, .75f), madeira);
        Caixa("Telefone com o fone fora do gancho", V(5.06f, .86f, -3.11f), V(.71f, .17f, .53f), ferro,
            tipo: Tipo.TelefoneCasaAvo);
        Arte("Disco e fone pendurado", "Cap1/TelefoneForaDoGancho", V(5.06f, .64f, -3.56f), .72f);
        Caixa("Cabide do casaco", V(-1.69f, 1.84f, -4.35f), V(.76f, .08f, .17f), madeira, solida: false);
        var casaco = Caixa("Casaco de chuva no prego", V(-1.69f, 1.37f, -4.25f), V(.42f, .80f, .075f), tecido, solida: false);
        casaco.transform.Rotate(0f, 0f, -9f);
        Caixa("Manga do casaco", V(-1.94f, 1.45f, -4.24f), V(.17f, .62f, .078f), tecido, solida: false)
            .transform.Rotate(0f, 0f, -18f);

        var banco = new GameObject("Banco de madeira tombado").transform;
        banco.SetParent(cenario, false);
        banco.localPosition = V(-2.75f, .26f, -3.37f);
        banco.localRotation = Quaternion.Euler(69f, 18f, 13f);
        Caixa("Assento do banco caído", Vector3.zero, V(.63f, .095f, .37f), madeira, banco);
        foreach (float x in new[] { -.21f, .21f })
        foreach (float z in new[] { -.12f, .12f })
            Caixa("Pé do banco tombado", V(x, -.20f, z), V(.059f, .37f, .059f), madeira, banco, false);
        Caixa("Marcas de botas perto do banco", V(-.99f, .055f, -3.15f), V(.85f, .10f, 1.62f), barro,
            tipo: Tipo.PistaDesordemPorta).GetComponent<MeshRenderer>().enabled = false;
        for (int passo = 0; passo < 9; passo++)
        {
            float x = -.43f - passo * .083f + (passo % 2) * .22f;
            float z = -4.22f + passo * .24f;
            Caixa("Calcanhar de barro", V(x, .014f, z), V(.13f, .009f, .125f), barro, solida: false)
                .transform.Rotate(0f, passo % 2 == 0 ? -14f : 12f, 0f);
            Caixa("Sola de bota", V(x, .013f, z + .16f), V(.18f, .009f, .17f), barro, solida: false);
            Caixa("Rastro de bota voltando", V(x - .54f, .013f, z + .08f), V(.17f, .009f, .21f), barro, solida: false);
        }
    }

    private void CilindroCasa(string nome, Vector3 pos, Vector3 tamanho, Material material)
    {
        var objeto = Geometria(PrimitiveType.Cylinder, nome, pos, tamanho, material, cenario);
        objeto.GetComponent<Collider>().enabled = false;
    }

    private void PassosNoInterior(float distancia, float posicaoZ, float posicaoX)
    {
        distanciaPasso += distancia;
        if (distanciaPasso < .58f) return;
        distanciaPasso -= .58f;
        variantePasso = (variantePasso + 1) % 3;
        bool madeira = Casa && !(posicaoX < -2.10f && posicaoZ > .64f);
        string piso = madeira ? "wood" : "concrete";
        SantaRitaSound.Play("footstep_" + piso + "_00" + variantePasso, .14f, 1f, .09f);
    }
}
