using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarRodoviaria()
    {
        ConfigurarEntrada(-4.5f, V(0f, EyeHeight, -2.85f), "Foley/footstep_concrete_000");

        var pisoTex = CriarCeramica(new Color(.52f, .49f, .44f), new Color(.44f, .41f, .36f), 64, 4);
        var azulejoTex = CriarCeramica(new Color(.15f, .34f, .40f), new Color(.12f, .29f, .35f), 64, 4);
        var madeiraTex = CriarTabuas(new Color(.35f, .24f, .15f), new Color(.28f, .19f, .12f), 64, 4);

        var pisoMat = Superficie("RodoPiso", Color.white, pisoTex, new Vector2(15f, 12f), .18f);
        var azulejoMat = Superficie("RodoAzulejo", Color.white, azulejoTex, new Vector2(10f, 2f), .32f);
        var rebocoMat = Superficie("RodoReboco", new Color(.52f, .49f, .42f), brilho: .06f);
        var frisoMat = Superficie("RodoFriso", new Color(.10f, .19f, .24f), brilho: .24f);
        var tetoMat = Superficie("RodoTeto", new Color(.14f, .15f, .16f), brilho: .05f);
        var acoMat = Superficie("RodoAco", new Color(.15f, .18f, .20f), brilho: .34f);
        var cromoMat = Superficie("RodoCromo", new Color(.58f, .60f, .62f), brilho: .58f);
        var madeiraMat = Superficie("RodoMadeira", Color.white, madeiraTex, Vector2.one, .16f);
        var balcaoAzulMat = Superficie("RodoBalcaoAzul", new Color(.14f, .29f, .42f), brilho: .22f);
        var vinilAzulMat = Superficie("RodoVinilAzul", new Color(.13f, .27f, .44f), brilho: .28f);
        var lataoMat = Superficie("RodoLatao", new Color(.68f, .54f, .24f), brilho: .50f);
        var vidroMat = VidroExterior();

        ConstruirEstruturaRodoviaria(pisoMat, azulejoMat, rebocoMat, frisoMat, tetoMat, acoMat, madeiraMat, vidroMat);

        ConstruirPorta(balcaoAzulMat, acoMat, cromoMat, 0f, -4.5f, 1.16f, 2.22f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, -4.53f), 180f, ExteriorInterior3D.Local.RodoviariaFrente);

        ConstruirAreaEsperaRodoviaria(acoMat, cromoMat, vinilAzulMat, madeiraMat, lataoMat);
        ConstruirOrelhaoRodoviaria(balcaoAzulMat, acoMat, cromoMat);
        ConstruirQuadroHorarios(madeiraMat, acoMat, rebocoMat);
        ConstruirGuicheRodoviaria(balcaoAzulMat, madeiraMat, acoMat, cromoMat, lataoMat, vidroMat);
        ConstruirIluminacaoRodoviaria(acoMat, cromoMat, lataoMat);
    }

    private void ConstruirEstruturaRodoviaria(Material pisoMat, Material azulejoMat, Material rebocoMat,
        Material frisoMat, Material tetoMat, Material acoMat, Material madeiraMat, Material vidroMat)
    {
        // Piso principal (x: -7.5..7.5, z: -4.5..7.5)
        Caixa("PisoRodoviaria", V(0f, -.05f, 1.5f), V(15f, .10f, 12f), pisoMat);
        // Teto
        Caixa("TetoRodoviaria", V(0f, 3.55f, 1.5f), V(15f, .10f, 12f), tetoMat);

        // Rodapé e paredes laterais (esquerda x=-7.5, direita x=7.5)
        foreach (float x in new[] { -7.58f, 7.58f })
        {
            Caixa("ParedeAzulejoLat", V(x, .675f, 1.5f), V(.16f, 1.35f, 12f), azulejoMat);
            Caixa("ParedeFrisoLat", V(x, 1.37f, 1.5f), V(.18f, .05f, 12f), frisoMat, solida: false);
            Caixa("ParedeRebocoLat", V(x, 2.425f, 1.5f), V(.16f, 2.15f, 12f), rebocoMat);
        }

        // Parede frontal (z = -4.5) com janela esquerda, vão da porta central e janela direita
        var vaosFrente = new[]
        {
            new Rect(-4.6f, 1.05f, 2.2f, 1.35f),
            new Rect(-.58f, 0f, 1.16f, 2.22f),
            new Rect(2.4f, 1.05f, 2.2f, 1.35f)
        };
        ParedeBandaRodoviaria(-7.5f, 7.5f, -4.5f, 0f, 1.35f, .18f, azulejoMat, vaosFrente);
        ParedeBandaRodoviaria(-7.5f, 7.5f, -4.5f, 1.35f, 3.5f, .18f, rebocoMat, vaosFrente);
        foreach (float xJan in new[] { -3.5f, 3.5f })
        {
            MolduraVazada(V(xJan, 1.725f, -4.5f), 2.2f, 1.35f, acoMat, .06f, .20f);
            Caixa("VidroJanelaFrente", V(xJan, 1.725f, -4.5f), V(2.16f, 1.31f, .03f), vidroMat, solida: false);
        }

        // Parede dos fundos (z = 7.5) com portas/janelas de vidro para a plataforma à esquerda
        var vaosFundo = new[]
        {
            new Rect(-3.55f, 0f, 2.70f, 2.35f)
        };
        ParedeBandaRodoviaria(-7.5f, 7.5f, 7.5f, 0f, 1.35f, .18f, azulejoMat, vaosFundo);
        ParedeBandaRodoviaria(-7.5f, 7.5f, 7.5f, 1.35f, 3.5f, .18f, rebocoMat, vaosFundo);

        // Portas duplas de vidro + bandeira da saída para a plataforma
        Caixa("MarcoPlatEsq", V(-3.55f, 1.175f, 7.5f), V(.08f, 2.35f, .20f), acoMat);
        Caixa("MarcoPlatDir", V(-.85f, 1.175f, 7.5f), V(.08f, 2.35f, .20f), acoMat);
        Caixa("MarcoPlatSup", V(-2.2f, 2.38f, 7.5f), V(2.78f, .08f, .20f), acoMat);
        Caixa("MontantePlatCentro", V(-2.2f, 1.175f, 7.5f), V(.07f, 2.35f, .14f), acoMat);
        Caixa("VidroPlatEsq", V(-2.875f, 1.175f, 7.5f), V(1.26f, 2.28f, .03f), vidroMat);
        Caixa("VidroPlatDir", V(-1.525f, 1.175f, 7.5f), V(1.26f, 2.28f, .03f), vidroMat);
        Caixa("BarraEmpurrarEsq", V(-2.875f, 1.02f, 7.45f), V(.95f, .05f, .06f), acoMat, solida: false);
        Caixa("BarraEmpurrarDir", V(-1.525f, 1.02f, 7.45f), V(.95f, .05f, .06f), acoMat, solida: false);

        // Exterior da plataforma chuvosa visto pelos vidros dos fundos
        var asfaltoPlat = Superficie("PlatAsfalto", new Color(.06f, .07f, .08f), brilho: .42f);
        var faixaAmarela = Superficie("PlatFaixa", new Color(.68f, .54f, .14f), brilho: .15f);
        Caixa("PlataformaExterna", V(-2.2f, -.08f, 9.8f), V(7.5f, .10f, 4.6f), asfaltoPlat, solida: false);
        Caixa("FaixaSegurancaPlat", V(-2.2f, -.025f, 9.1f), V(6.5f, .01f, .16f), faixaAmarela, solida: false);
        Caixa("PilarPlataforma", V(-3.8f, 1.6f, 9.3f), V(.26f, 3.2f, .26f), acoMat, solida: false);
        Caixa("CoberturaPlataforma", V(-2.2f, 3.15f, 9.6f), V(7.2f, .12f, 3.8f), acoMat, solida: false);
        Luz("LuzPlataformaFria", V(-2.2f, 2.75f, 9.2f), new Color(.44f, .60f, .82f), .80f, 6.8f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, 12.1f), 0f, ExteriorInterior3D.Local.RodoviariaPlataforma);

        // Treliças metálicas e terças de madeira aparentes no teto
        foreach (float zTr in new[] { -2.5f, 0f, 2.5f, 5.0f })
        {
            Caixa("BanzoInfTrelica", V(0f, 3.08f, zTr), V(14.8f, .08f, .09f), acoMat, solida: false);
            Caixa("BanzoSupTrelica", V(0f, 3.44f, zTr), V(14.8f, .08f, .09f), acoMat, solida: false);
            for (float xM = -6.5f; xM <= 6.5f; xM += 2.6f)
                Caixa("MontanteTrelica", V(xM, 3.26f, zTr), V(.06f, .34f, .07f), acoMat, solida: false);
        }
        foreach (float xTerca in new[] { -5.2f, -2.6f, 0f, 2.6f, 5.2f })
            Caixa("TercaMadeiraTeto", V(xTerca, 3.47f, 1.5f), V(.09f, .07f, 11.8f), madeiraMat, solida: false);

        // Tapete de entrada e letreiros internos
        Caixa("CapachoEntrada", V(0f, .008f, -3.95f), V(1.36f, .016f, .68f),
            Superficie("RodoCapacho", new Color(.19f, .16f, .14f), brilho: .04f), solida: false);
        Letreiro("PlacaPlataforma", "EMBARQUE · PLATAFORMAS", V(-2.2f, 2.64f, 7.36f),
            Superficie("PlacaVerde", new Color(.10f, .24f, .18f)), 2.2f, .34f, 180f);
    }

    private void ParedeBandaRodoviaria(float xMin, float xMax, float z, float yMin, float yMax,
        float espessura, Material mat, Rect[] vaos)
    {
        float alturaBanda = yMax - yMin;
        var recorte = new Rect[vaos.Length];
        for (int i = 0; i < vaos.Length; i++)
        {
            float vy0 = Mathf.Clamp(vaos[i].yMin - yMin, 0f, alturaBanda);
            float vy1 = Mathf.Clamp(vaos[i].yMax - yMin, 0f, alturaBanda);
            recorte[i] = new Rect(vaos[i].xMin, vy0, vaos[i].width, Mathf.Max(0f, vy1 - vy0));
        }
        ExteriorInterior3D.Parede(xMin, xMax, alturaBanda, recorte, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeTrechoRodo", V((a + b) * .5f, yMin + (y0 + y1) * .5f, z), V(w, h, espessura), mat);
        });
    }

    private void ConstruirAreaEsperaRodoviaria(Material acoMat, Material cromoMat, Material vinilAzulMat,
        Material madeiraMat, Material lataoMat)
    {
        // Duas fileiras de bancos de espera metálicos com estofado azul
        ConstruirBancoTerminal(V(-4.35f, 0f, .15f), acoMat, cromoMat, vinilAzulMat);
        ConstruirBancoTerminal(V(-4.35f, 0f, 2.35f), acoMat, cromoMat, vinilAzulMat);

        // Passageira na espera
        Personagem("Passageira", "Art/Characters/Npc_Passageira", V(-3.8f, 0f, 1.2f), "rodo_passageira");

        // Bagagem ao lado do banco (malas de viagem com alças e fechos de latão)
        var malasPivo = Pivo("BagagemEspera", world, V(-2.45f, 0f, 1.2f));
        var couroMat = Superficie("MalaCouro", new Color(.34f, .20f, .12f), brilho: .22f);
        var lonaMat = Superficie("MalaLona", new Color(.25f, .30f, .22f), brilho: .10f);
        var cintaMat = Superficie("MalaCinta", new Color(.16f, .10f, .07f), brilho: .18f);

        // Mala grande deitada
        Caixa("MalaGrande", V(0f, .17f, 0f), V(.62f, .34f, .40f), couroMat, malasPivo);
        Caixa("CintaMala1", V(-.17f, .17f, 0f), V(.04f, .35f, .41f), cintaMat, malasPivo, solida: false);
        Caixa("CintaMala2", V(.17f, .17f, 0f), V(.04f, .35f, .41f), cintaMat, malasPivo, solida: false);
        Caixa("FechoMala1", V(-.17f, .24f, -.21f), V(.05f, .05f, .02f), lataoMat, malasPivo, solida: false);
        Caixa("FechoMala2", V(.17f, .24f, -.21f), V(.05f, .05f, .02f), lataoMat, malasPivo, solida: false);
        Caixa("AlcaMalaGrande", V(0f, .36f, -.06f), V(.18f, .04f, .05f), cintaMat, malasPivo, solida: false);

        // Mala média apoiada em cima
        var malaSup = Pivo("MalaSuperior", malasPivo, V(.05f, .34f, .02f));
        malaSup.localRotation = Quaternion.Euler(0f, 14f, 0f);
        Caixa("MalaLona", V(0f, .12f, 0f), V(.46f, .24f, .32f), lonaMat, malaSup, solida: false);
        Caixa("CintaLona1", V(-.12f, .12f, 0f), V(.035f, .25f, .33f), cintaMat, malaSup, solida: false);
        Caixa("CintaLona2", V(.12f, .12f, 0f), V(.035f, .25f, .33f), cintaMat, malaSup, solida: false);
        Caixa("AlcaMalaLona", V(0f, .26f, 0f), V(.14f, .035f, .04f), cintaMat, malaSup, solida: false);

        AlvoFisico(malasPivo, "AlvoBagagem", V(0f, .32f, 0f), V(.78f, .70f, .56f), "bagagem", "Examinar bagagem");

        // Lixeira metálica próxima aos bancos
        Cilindro("LixeiraRodoviaria", V(-6.45f, .32f, 1.25f), V(.36f, .32f, .36f), acoMat, solida: true);
        Cilindro("AroLixeira", V(-6.45f, .64f, 1.25f), V(.38f, .02f, .38f), cromoMat, solida: false);

        // Motorista de escala próximo às portas da plataforma + banqueta com garrafa térmica
        Personagem("Motorista", "Art/Characters/Npc_Motorista", V(-1.2f, 0f, 5.6f), "rodo_motorista");
        Caixa("BanquetaMotorista", V(-.45f, .24f, 6.25f), V(.42f, .48f, .42f), madeiraMat);
        Cilindro("TermicaCorpo", V(-.45f, .62f, 6.25f), V(.11f, .14f, .11f),
            Superficie("TermicaVermelha", new Color(.56f, .14f, .12f), brilho: .42f), solida: false);
        Cilindro("TermicaTampa", V(-.45f, .78f, 6.25f), V(.09f, .035f, .09f), cromoMat, solida: false);
    }

    private void ConstruirBancoTerminal(Vector3 centro, Material acoMat, Material cromoMat, Material vinilAzulMat)
    {
        var banco = Pivo("BancoTerminal", world, centro);
        // Longarina e pés metálicos
        Caixa("LongarinaBanco", V(0f, .38f, 0f), V(2.05f, .06f, .07f), acoMat, banco, solida: false);
        foreach (float xPe in new[] { -.86f, .86f })
        {
            Caixa("PeBanco", V(xPe, .20f, 0f), V(.06f, .40f, .44f), acoMat, banco);
            Caixa("SapataBanco", V(xPe, .015f, 0f), V(.08f, .03f, .50f), cromoMat, banco, solida: false);
        }
        // 3 assentos em vinil azul com apoios de braço cromados
        for (int i = -1; i <= 1; i++)
        {
            float x = i * .66f;
            Caixa("AssentoBanco", V(x, .44f, .04f), V(.58f, .06f, .44f), vinilAzulMat, banco);
            Caixa("EncostoBanco", V(x, .72f, -.17f), V(.58f, .42f, .06f), vinilAzulMat, banco);
        }
        foreach (float xBr in new[] { -1.01f, -.33f, .33f, 1.01f })
            Caixa("BracoBanco", V(xBr, .58f, .02f), V(.04f, .04f, .42f), cromoMat, banco, solida: false);
    }

    private void ConstruirOrelhaoRodoviaria(Material azulMat, Material acoMat, Material cromoMat)
    {
        var pivo = Pivo("OrelhaoRodoviaria", world, V(-7.38f, 0f, -1.2f));
        // Cúpula acústica azul montada na parede esquerda
        Caixa("FundoCabineFone", V(.04f, 1.48f, 0f), V(.06f, .92f, .68f), acoMat, pivo);
        Caixa("CupulaTopoFone", V(.28f, 1.92f, 0f), V(.52f, .08f, .72f), azulMat, pivo, solida: false);
        Caixa("CupulaEsqFone", V(.26f, 1.56f, -.34f), V(.48f, .68f, .05f), azulMat, pivo, solida: false);
        Caixa("CupulaDirFone", V(.26f, 1.56f, .34f), V(.48f, .68f, .05f), azulMat, pivo, solida: false);

        // Aparelho telefônico metálico, teclado, fone e ranhura de fichas
        Caixa("CaixaTelefone", V(.12f, 1.44f, 0f), V(.14f, .36f, .24f), cromoMat, pivo);
        Caixa("TecladoTelefone", V(.20f, 1.44f, .03f), V(.02f, .12f, .10f), acoMat, pivo, solida: false);
        Caixa("RanhuraFicha", V(.20f, 1.56f, -.05f), V(.02f, .04f, .05f), acoMat, pivo, solida: false);
        Caixa("GanchoMonofone", V(.18f, 1.48f, -.09f), V(.05f, .06f, .05f), acoMat, pivo, solida: false);
        Caixa("Monofone", V(.21f, 1.46f, -.09f), V(.05f, .22f, .05f), acoMat, pivo, solida: false);

        AlvoFisico(pivo, "AlvoTelefoneRodo", V(.26f, 1.46f, 0f), V(.56f, .86f, .72f),
            "telefone_rodo", "Usar telefone público");
    }

    private void ConstruirQuadroHorarios(Material madeiraMat, Material acoMat, Material rebocoMat)
    {
        var pivo = Pivo("QuadroHorarios", world, V(1.15f, 0f, 7.36f));
        var painelMat = Superficie("QuadroFundo", new Color(.10f, .13f, .15f), brilho: .12f);

        // Quadro grande na parede dos fundos (z = 7.36)
        MolduraVazada(V(1.15f, 2.08f, 7.35f), 2.10f, 1.24f, madeiraMat, .07f, .08f);
        Caixa("FundoQuadroHorarios", V(0f, 2.08f, 0f), V(2.08f, 1.22f, .04f), painelMat, pivo);
        Letreiro("TituloHorarios", "HORÁRIOS · LINHAS INTERMUNICIPAIS", V(1.15f, 2.54f, 7.31f),
            Superficie("FaixaTituloHorarios", new Color(.14f, .28f, .38f)), 1.98f, .22f, 180f);

        // Tiras de rotas pintadas
        Color[] coresLinha =
        {
            new Color(.76f, .72f, .60f),
            new Color(.68f, .76f, .70f),
            new Color(.78f, .68f, .52f),
            new Color(.66f, .72f, .80f)
        };
        for (int i = 0; i < 4; i++)
        {
            float y = 2.24f - i * .19f;
            Caixa("TiraHorario_" + i, V(0f, y, -.025f), V(1.86f, .12f, .015f),
                Superficie("TiraHor" + i, coresLinha[i]), pivo, solida: false);
        }

        // Porta-folhetos de madeira abaixo do quadro
        Caixa("EstanteFolhetosBase", V(0f, .74f, -.12f), V(1.42f, .72f, .22f), madeiraMat, pivo);
        Caixa("PrateleiraFolhetos1", V(0f, .88f, -.22f), V(1.32f, .04f, .10f), acoMat, pivo, solida: false);
        Caixa("PrateleiraFolhetos2", V(0f, .58f, -.22f), V(1.32f, .04f, .10f), acoMat, pivo, solida: false);
        Color[] coresFolheto = { new Color(.74f, .28f, .22f), new Color(.22f, .46f, .62f), new Color(.78f, .64f, .22f), rebocoMat.color };
        for (int i = 0; i < 4; i++)
        {
            float xf = -.45f + i * .30f;
            var fMat = Superficie("Folheto_" + i, coresFolheto[i]);
            Caixa("FolhetoSup_" + i, V(xf, 1.00f, -.22f), V(.18f, .22f, .02f), fMat, pivo, solida: false);
            Caixa("FolhetoInf_" + i, V(xf, .70f, -.22f), V(.18f, .22f, .02f), fMat, pivo, solida: false);
        }

        AlvoFisico(pivo, "AlvoHorarios", V(0f, 1.52f, -.18f), V(2.15f, 2.20f, .60f),
            "horarios", "Consultar quadro de horários e folhetos");
    }

    private void ConstruirGuicheRodoviaria(Material azulMat, Material madeiraMat, Material acoMat,
        Material cromoMat, Material lataoMat, Material vidroMat)
    {
        // Balcão do guichê em (4.2, 3.6) com cabine fechada até a parede dos fundos
        Caixa("BalcaoGuicheBase", V(4.2f, .525f, 3.6f), V(3.30f, 1.05f, .56f), azulMat);
        Caixa("TampoGuiche", V(4.2f, 1.075f, 3.6f), V(3.42f, .05f, .66f), madeiraMat);

        // Divisórias laterais da cabine do guichê (lado esquerdo x=2.62, lado direito x=5.78 com passagem atrás)
        Caixa("ParedeGuicheEsq", V(2.62f, 1.25f, 5.55f), V(.14f, 2.50f, 3.90f), azulMat);
        Caixa("ParedeGuicheDir", V(5.78f, 1.25f, 4.95f), V(.14f, 2.50f, 2.70f), azulMat);
        Caixa("TesteiraGuiche", V(4.2f, 2.34f, 3.6f), V(3.30f, .38f, .16f), azulMat);
        Letreiro("PlacaGuiche", "GUICHÊ · PASSAGENS", V(4.2f, 2.34f, 3.50f),
            Superficie("FundoPlacaGuiche", new Color(.10f, .16f, .22f)), 1.95f, .26f, 0f);

        // Vidro e grades metálicas do guichê (não sólidos para permitir raio de interação até o Dário)
        Caixa("VidroGuiche", V(4.2f, 1.66f, 3.6f), V(3.04f, 1.08f, .02f), vidroMat, solida: false);
        Caixa("TravessaGuiche", V(4.2f, 2.14f, 3.6f), V(3.08f, .04f, .05f), acoMat, solida: false);
        for (float xBarra = 2.95f; xBarra <= 5.45f; xBarra += .31f)
            Caixa("GradeGuiche", V(xBarra, 1.62f, 3.56f), V(.022f, 1.04f, .022f), cromoMat, solida: false);

        // Bandeja passa-dinheiro, rolo de passagens e luminária de mesa no balcão
        Caixa("BandejaGuiche", V(4.2f, 1.11f, 3.45f), V(.38f, .02f, .24f), cromoMat, solida: false);
        Cilindro("RoloPassagens", V(3.45f, 1.20f, 3.72f), V(.14f, .07f, .14f),
            Superficie("PapelBilhete", new Color(.84f, .80f, .68f)), solida: false);
        Caixa("SuporteBilhete", V(3.45f, 1.13f, 3.72f), V(.16f, .06f, .18f), acoMat, solida: false);

        // Luminária de mesa do guichê
        var lampPivo = Pivo("LuminariaGuiche", world, V(4.95f, 1.10f, 3.76f));
        Cilindro("BaseLampadaGuiche", V(0f, .02f, 0f), V(.15f, .02f, .15f), lataoMat, lampPivo, solida: false);
        Cilindro("HasteLampadaGuiche", V(0f, .18f, 0f), V(.025f, .16f, .025f), lataoMat, lampPivo, solida: false);
        Caixa("CupulaLampadaGuiche", V(-.04f, .34f, -.04f), V(.22f, .09f, .16f),
            Superficie("CupulaVerdeGuiche", new Color(.14f, .36f, .20f), brilho: .40f), lampPivo, solida: false);
        Luz("LuzMesaGuiche", V(-.04f, .26f, -.04f), new Color(1f, .80f, .48f), 1.05f, 3.4f, lampPivo);

        // Móvel de arquivo ao fundo do guichê
        Caixa("ArquivoGuiche", V(5.15f, .68f, 6.85f), V(.65f, 1.36f, .52f), acoMat);

        // Dário atrás do balcão
        Personagem("Dário", "Art/Characters/Npc_Dario", V(4.2f, 0f, 4.7f), "dario");
    }

    private void ConstruirIluminacaoRodoviaria(Material acoMat, Material cromoMat, Material lataoMat)
    {
        var bulboMat = Superficie("RodoBulboQuente", new Color(.96f, .82f, .56f), unlit: true);
        var bulboQueimado = Superficie("RodoBulboQueimado", new Color(.14f, .15f, .16f), brilho: .2f);

        // Pendentes industriais: metade apagada ou fraca para isolar zonas de sombra
        Vector3[] pendentes =
        {
            V(0f, 2.86f, -1.6f),
            V(0f, 2.86f, 2.2f),
            V(4.2f, 2.86f, 1.6f),
            V(-2.2f, 2.86f, 4.8f)
        };
        float[] intensidadesPend = { .72f, 0f, .82f, .28f };
        float[] alcancesPend = { 5.2f, 0f, 5.4f, 4.2f };
        for (int i = 0; i < pendentes.Length; i++)
        {
            var p = pendentes[i];
            bool acesa = intensidadesPend[i] > 0f;
            Cilindro("HastePendente_" + i, V(p.x, 3.22f, p.z), V(.03f, .34f, .03f), acoMat, solida: false);
            Cilindro("CupulaPendente_" + i, V(p.x, p.y + .06f, p.z), V(.44f, .06f, .44f), acoMat, solida: false);
            Esfera("BulboPendente_" + i, V(p.x, p.y - .02f, p.z), V(.11f, .11f, .11f),
                acesa ? (i == 3 ? BulboInstancia("RodoBulboFraco", new Color(.48f, .38f, .24f)) : bulboMat) : bulboQueimado);
            if (acesa)
                Luz("LuzPendente_" + i, V(p.x, p.y - .18f, p.z), new Color(1f, .79f, .52f), intensidadesPend[i], alcancesPend[i]);
        }

        // Tubo fluorescente falhando com estalos elétricos e zumbido de reator
        var fluorPivo = Pivo("LuminariaFluorescente", world, V(-4.2f, 3.22f, 1.25f));
        Caixa("CalhaFluorescente", V(0f, .05f, 0f), V(1.38f, .05f, .16f), cromoMat, fluorPivo, solida: false);
        var corTuboAcesa = new Color(.78f, .88f, .96f);
        var tuboMat = BulboInstancia("TuboFluorescenteRodo", corTuboAcesa);
        Cilindro("TuboFluorescente", V(0f, -.01f, 0f), V(.05f, .62f, .05f), tuboMat, fluorPivo, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        var luzFluor = Luz("LuzFluorescente", V(0f, -.16f, 0f), new Color(.74f, .86f, .96f), .95f, 6.2f, fluorPivo);
        LuzOscilante(luzFluor, tuboMat, .95f, corTuboAcesa, new Color(.16f, .19f, .22f),
            intensidadeMinFator: .06f, somFaisca: true, ganhoSom: .26f, semente: 4.3f);
        SomLocal("Compressor", fluorPivo.localPosition, 6.8f, .28f);

        // Arandela sobre o orelhão
        Caixa("BaseArandelaFone", V(-7.42f, 2.34f, -1.2f), V(.06f, .18f, .14f), lataoMat, solida: false);
        Esfera("GloboArandelaFone", V(-7.32f, 2.34f, -1.2f), V(.14f, .14f, .14f), bulboMat);
        Luz("LuzOrelhao", V(-7.05f, 2.24f, -1.2f), new Color(1f, .82f, .54f), .68f, 3.9f);

        // Luz ambiente fria entrando pelas portas de vidro da plataforma
        Luz("LuzFriaSaidaPlataforma", V(-2.2f, 1.9f, 6.7f), new Color(.44f, .60f, .82f), .58f, 5.2f);
    }
}
