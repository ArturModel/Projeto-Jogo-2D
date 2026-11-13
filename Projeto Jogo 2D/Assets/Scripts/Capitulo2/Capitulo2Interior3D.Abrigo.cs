using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarAbrigo()
    {
        const float altura = 2.95f;
        const float xMin = -4.2f;
        const float xMax = 4.2f;
        const float zMin = -4.5f;
        const float zMax = 4.5f;

        ConfigurarEntrada(zMin, V(0f, EyeHeight, -2.85f), "Foley/footstep_concrete_000");

        // Materiais principais
        var texPiso = CriarCeramica(new Color(.23f, .24f, .25f), new Color(.18f, .19f, .20f), 64, 4);
        var matPiso = Superficie("PisoAbrigo", Color.white, texPiso, new Vector2(5f, 5.4f), brilho: .10f);

        var texTabuaEscura = CriarTabuas(new Color(.19f, .14f, .10f), new Color(.14f, .10f, .07f), 64, 4);
        var matMadeiraEscura = Superficie("MadeiraEscuraAbrigo", Color.white, texTabuaEscura, new Vector2(2f, 2f), brilho: .12f);

        var texTabuaMedia = CriarTabuas(new Color(.31f, .22f, .15f), new Color(.25f, .17f, .12f), 64, 4);
        var matMadeiraMedia = Superficie("MadeiraMediaAbrigo", Color.white, texTabuaMedia, new Vector2(2f, 2f), brilho: .14f);

        var matParede = Superficie("ParedeCalAbrigo", new Color(.52f, .53f, .49f), brilho: .04f);
        var matUmidade = Superficie("UmidadeAbrigo", new Color(.31f, .33f, .29f), brilho: .06f);
        var matRodape = Superficie("RodapePedraAbrigo", new Color(.22f, .22f, .23f), brilho: .08f);
        var matTeto = Superficie("TetoAbrigo", new Color(.15f, .12f, .10f), brilho: .06f);

        var matFerro = Superficie("FerroEnferrujadoAbrigo", new Color(.18f, .16f, .15f), brilho: .38f);
        var matLatao = Superficie("LataoAbrigo", new Color(.58f, .47f, .24f), brilho: .55f);
        var matPratoMetal = Superficie("PratoMetalAbrigo", new Color(.36f, .37f, .39f), brilho: .58f);

        var matCera = Superficie("CeraVelaAbrigo", new Color(.88f, .83f, .69f), brilho: .22f);
        var matCeraVelha = Superficie("CeraVelhaAbrigo", new Color(.78f, .72f, .57f), brilho: .18f);
        var matPavio = Superficie("PavioAbrigo", new Color(.12f, .10f, .09f), brilho: .05f);
        var matCaixaPapelao = Superficie("CaixaVelasPapelao", new Color(.48f, .39f, .27f), brilho: .07f);
        var matCouro = Superficie("CouroCadernoAbrigo", new Color(.22f, .12f, .08f), brilho: .24f);
        var matPapel = Superficie("PapelCadernoAbrigo", new Color(.86f, .81f, .69f), brilho: .05f);
        var matTinta = Superficie("TintaCadernoAbrigo", new Color(.18f, .16f, .15f), brilho: .05f);
        var matChama = Superficie("ChamaVelaAbrigo", new Color(1f, .76f, .30f), unlit: true);
        var matBulboFrio = Superficie("BulboAbrigo", new Color(.95f, .74f, .42f), unlit: true);

        // Piso, teto e caibros escuros
        Caixa("PisoAbrigo", V(0f, -.05f, 0f), V(8.4f, .10f, 9.0f), matPiso);
        Caixa("TetoAbrigo", V(0f, altura + .05f, 0f), V(8.6f, .10f, 9.2f), matTeto);

        Caixa("VigaCumeeira", V(0f, altura - .06f, 0f), V(.18f, .12f, 9.0f), matMadeiraEscura, solida: false);
        float[] zVigas = { -3.6f, -1.8f, 0f, 1.8f, 3.6f };
        foreach (float zv in zVigas)
            Caixa("VigaTeto", V(0f, altura - .09f, zv), V(8.4f, .16f, .14f), matMadeiraEscura, solida: false);

        // Paredes fechadas (frente com vao da porta, fundo com janela gradeada)
        ParedeComJanela(xMin, xMax, zMin, altura, .18f, matParede, new Rect(-.53f, 0f, 1.06f, 2.18f));
        ParedeComJanela(xMin, xMax, zMax, altura, .18f, matParede, new Rect(-.65f, 1.30f, 1.30f, .88f));
        Caixa("ParedeEsquerda", V(xMin - .09f, altura * .5f, 0f), V(.18f, altura, 9.0f), matParede);
        Caixa("ParedeDireita", V(xMax + .09f, altura * .5f, 0f), V(.18f, altura, 9.0f), matParede);

        // Rodapes e manchas de umidade nas paredes caiadas
        Caixa("RodapeEsq", V(xMin + .01f, .09f, 0f), V(.03f, .18f, 8.95f), matRodape, solida: false);
        Caixa("RodapeDir", V(xMax - .01f, .09f, 0f), V(.03f, .18f, 8.95f), matRodape, solida: false);
        Caixa("RodapeFundo", V(0f, .09f, zMax - .01f), V(8.35f, .18f, .03f), matRodape, solida: false);
        Caixa("UmidadeEsq", V(xMin + .005f, .52f, -1.1f), V(.01f, .78f, 2.4f), matUmidade, solida: false);
        Caixa("UmidadeFundo", V(-2.2f, .48f, zMax - .005f), V(2.1f, .70f, .01f), matUmidade, solida: false);
        Caixa("UmidadeDir", V(xMax - .005f, .60f, 2.6f), V(.01f, .88f, 1.9f), matUmidade, solida: false);

        // Janela gradeada na parede dos fundos
        MolduraVazada(V(0f, 1.74f, zMax), 1.30f, .88f, matMadeiraEscura, .06f, .22f);
        Caixa("PeitorilJanela", V(0f, 1.28f, zMax - .04f), V(1.44f, .06f, .26f), matMadeiraEscura, solida: false);
        Caixa("VidroJanelaFundo", V(0f, 1.74f, zMax + .01f), V(1.26f, .84f, .02f),
            VidroExterior(), solida: false);
        for (int i = -2; i <= 2; i++)
            Cilindro("GradeVert", V(i * .24f, 1.74f, zMax - .01f), V(.022f, .44f, .022f), matFerro);
        Caixa("GradeHoriz", V(0f, 1.74f, zMax - .01f), V(1.28f, .024f, .024f), matFerro, solida: false);

        // Porta de entrada, chuva e exterior do jardim/cemiterio
        ConstruirPorta(matMadeiraEscura, matMadeiraMedia, matLatao, 0f, zMin, 1.06f, 2.18f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, zMin - .03f), 180f, ExteriorInterior3D.Local.CemiterioFrente);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, zMax + .03f), 0f, ExteriorInterior3D.Local.CemiterioFundos);

        // Luminaria falha perto da porta com estalos elétricos
        var corBulboPorta = new Color(.95f, .74f, .42f);
        var bulboPortaFalho = BulboInstancia("BulboFalhoAbrigo", corBulboPorta);
        Caixa("SuporteArandelaPorta", V(-1.05f, 2.18f, zMin + .03f), V(.14f, .20f, .06f), matFerro, solida: false);
        Esfera("BulboArandelaPorta", V(-1.05f, 2.16f, zMin + .09f), V(.07f, .09f, .07f), bulboPortaFalho);
        var luzPortaAbrigo = Luz("LuzEntradaAbrigo", V(-1.05f, 2.10f, zMin + .25f), new Color(.96f, .72f, .40f), .52f, 4.4f);
        LuzOscilante(luzPortaAbrigo, bulboPortaFalho, .52f, corBulboPorta, new Color(.14f, .12f, .09f),
            intensidadeMinFator: .06f, somFaisca: true, ganhoSom: .22f, semente: 7.4f);

        ConstruirBancoEntrada(matMadeiraMedia, matMadeiraEscura);

        // Lado esquerdo: Estante "Apoio do cemiterio" e ferramentas de zeladoria
        ConstruirApoioCemiterio(matMadeiraMedia, matMadeiraEscura, matCaixaPapelao, matCera, matCeraVelha, matPavio);
        ConstruirFerramentasZeladoria(matMadeiraMedia, matFerro, matLatao, matRodape);

        // Fundo direito: Mesa da ronda com Caderno de velas e vela acesa tremulante
        ConstruirMesaDaRonda(matMadeiraEscura, matMadeiraMedia, matLatao, matCouro, matPapel, matTinta,
            matPratoMetal, matCera, matPavio, matChama);

        // Detalhes extras de ambiente (vasos de pedra no canto esquerdo, quadro de chaves perto da porta)
        Cilindro("VasoCanto1", V(-3.55f, .22f, 3.85f), V(.26f, .22f, .26f), matRodape, solida: true);
        Cilindro("VasoCanto2", V(-3.18f, .17f, 3.98f), V(.22f, .17f, .22f), matRodape, solida: true);

        Caixa("QuadroChaves", V(1.18f, 1.48f, zMin + .03f), V(.44f, .34f, .04f), matMadeiraEscura, solida: false);
        for (int k = 0; k < 3; k++)
        {
            float xk = 1.04f + k * .14f;
            Caixa("GanchoChave", V(xk, 1.48f, zMin + .06f), V(.02f, .02f, .04f), matLatao, solida: false);
            Caixa("ChavePendurada", V(xk, 1.41f, zMin + .075f), V(.02f, .08f, .01f), matFerro, solida: false);
        }

        // Iluminacao geral sombria + luz fria da janela dos fundos
        Luz("LuzAmbienteAbrigo", V(0f, 2.65f, 0f), new Color(.30f, .38f, .46f), .18f, 7.5f);
        SpotLuz("LuzLuaJanela", V(0f, 2.20f, zMax + .65f), V(32f, 180f, 0f),
            new Color(.42f, .58f, .76f), .95f, 7.2f, 46f, 20f);
    }

    private void ConstruirBancoEntrada(Material madeira, Material madeiraEscura)
    {
        var banco = Pivo("BancoEntrada", world, V(2.15f, 0f, -3.72f));
        Caixa("AssentoBanco", V(0f, .45f, 0f), V(1.38f, .06f, .40f), madeira, banco);
        Caixa("TravessaBanco", V(0f, .22f, 0f), V(1.18f, .05f, .06f), madeiraEscura, banco, solida: false);
        Caixa("PernaBancoEsqF", V(-.56f, .225f, -.14f), V(.07f, .45f, .07f), madeiraEscura, banco, solida: false);
        Caixa("PernaBancoEsqT", V(-.56f, .225f, .14f), V(.07f, .45f, .07f), madeiraEscura, banco, solida: false);
        Caixa("PernaBancoDirF", V(.56f, .225f, -.14f), V(.07f, .45f, .07f), madeiraEscura, banco, solida: false);
        Caixa("PernaBancoDirT", V(.56f, .225f, .14f), V(.07f, .45f, .07f), madeiraEscura, banco, solida: false);
    }

    private void ConstruirApoioCemiterio(Material madeira, Material madeiraEscura,
        Material papelao, Material cera, Material ceraVelha, Material pavio)
    {
        var estante = Pivo("ApoioCemiterio", world, V(-3.68f, 0f, .45f));

        // Estrutura da estante (4 postes, 4 prateleiras)
        float[] px = { -.26f, .26f };
        float[] pz = { -1.18f, 1.18f };
        foreach (float x in px)
        foreach (float z in pz)
            Caixa("PosteEstante", V(x, 1.02f, z), V(.06f, 2.04f, .06f), madeiraEscura, estante);

        float[] yPrat = { .14f, .70f, 1.28f, 1.84f };
        foreach (float y in yPrat)
            Caixa("Prateleira", V(0f, y, 0f), V(.58f, .04f, 2.42f), madeira, estante);

        // Colisor unico de interacao cobrindo toda a estante
        AlvoFisico(estante, "AlvoApoioCemiterio", V(.08f, 1.05f, 0f), V(.75f, 1.95f, 2.50f),
            "apoio_cemiterio", "Examinar apoio do cemiterio");

        // Caixas de velas empilhadas nas prateleiras inferiores
        Caixa("CaixaVelas1", V(.02f, .31f, -.72f), V(.40f, .30f, .52f), papelao, estante, solida: false);
        Caixa("CaixaVelas2", V(.04f, .31f, -.10f), V(.38f, .30f, .48f), papelao, estante, solida: false);
        var caixaTortinha = Pivo("CaixaVelas3Pivo", estante, V(.01f, .57f, -.68f));
        caixaTortinha.localRotation = Quaternion.Euler(0f, 9f, 0f);
        Caixa("CaixaVelas3", V(0f, 0f, 0f), V(.34f, .22f, .44f), papelao, caixaTortinha, solida: false);

        Caixa("CaixaVelas4", V(0f, .86f, .68f), V(.40f, .28f, .54f), papelao, estante, solida: false);
        Caixa("CaixaVelas5", V(.02f, 1.11f, .70f), V(.34f, .22f, .44f), papelao, estante, solida: false);

        // Fileiras de velas de alturas variadas nas prateleiras do meio e superior
        float[] alturasVelas = { .14f, .20f, .17f, .23f, .12f, .19f, .15f, .21f };
        for (int i = 0; i < alturasVelas.Length; i++)
        {
            float z = -.96f + i * .16f;
            float h = alturasVelas[i];
            Material matV = i % 2 == 0 ? cera : ceraVelha;
            Cilindro("VelaPrat2_" + i, V(.10f, .72f + h * .5f, z), V(.055f, h * .5f, .055f), matV, estante);
            Cilindro("PavioPrat2_" + i, V(.10f, .72f + h + .012f, z), V(.007f, .012f, .007f), pavio, estante);
        }

        for (int i = 0; i < 10; i++)
        {
            float z = -.98f + i * .21f;
            float h = .13f + (i % 3) * .045f;
            Material matV = i % 3 == 0 ? ceraVelha : cera;
            Cilindro("VelaPrat3_" + i, V(.08f, 1.30f + h * .5f, z), V(.05f, h * .5f, .05f), matV, estante);
            Cilindro("PavioPrat3_" + i, V(.08f, 1.30f + h + .01f, z), V(.007f, .01f, .007f), pavio, estante);
            if (i % 2 == 0)
            {
                float h2 = h * .82f;
                Cilindro("VelaPrat3Fundo_" + i, V(-.10f, 1.30f + h2 * .5f, z + .05f), V(.048f, h2 * .5f, .048f), ceraVelha, estante);
            }
        }

        // Velas grossas de vigilia na prateleira superior
        for (int i = 0; i < 4; i++)
        {
            float z = -.75f + i * .45f;
            Cilindro("VelaVigilia_" + i, V(.05f, 1.86f + .13f, z), V(.09f, .13f, .09f), cera, estante);
            Cilindro("PavioVigilia_" + i, V(.05f, 1.86f + .27f, z), V(.01f, .015f, .01f), pavio, estante);
        }
    }

    private void ConstruirFerramentasZeladoria(Material madeira, Material ferro, Material latao, Material pedra)
    {
        // Pa encostada na parede esquerda (depois da estante)
        var pa = Pivo("PaZelador", world, V(-3.98f, 0f, 2.25f));
        pa.localRotation = Quaternion.Euler(0f, 0f, -11f);
        Cilindro("CaboPa", V(0f, .82f, 0f), V(.032f, .62f, .032f), madeira, pa);
        Caixa("PunhoPa", V(0f, 1.45f, 0f), V(.04f, .04f, .14f), madeira, pa, solida: false);
        Caixa("LaminaPa", V(0f, .14f, 0f), V(.02f, .28f, .24f), ferro, pa, solida: false);

        // Enxada / rastelo ao lado da pa
        var rastelo = Pivo("RasteloZelador", world, V(-4.02f, 0f, 2.75f));
        rastelo.localRotation = Quaternion.Euler(0f, 0f, -9f);
        Cilindro("CaboRastelo", V(0f, .78f, 0f), V(.028f, .76f, .028f), madeira, rastelo);
        Caixa("BarraRastelo", V(0f, 1.54f, 0f), V(.04f, .04f, .42f), ferro, rastelo, solida: false);

        // Caixa de ferramentas metalica e lampiao no chao antes da estante
        Caixa("CaixaFerramentas", V(-3.62f, .14f, -1.55f), V(.36f, .26f, .54f), ferro);
        Caixa("AlcaCaixaFerramentas", V(-3.62f, .29f, -1.55f), V(.04f, .05f, .24f), latao, solida: false);

        // Lampiao apagado sobre um caixote baixo
        Caixa("CaixoteLampiao", V(-3.64f, .18f, -1.02f), V(.38f, .36f, .38f), madeira);
        var lampiao = Pivo("LampiaoZelador", world, V(-3.60f, .36f, -1.02f));
        Cilindro("BaseLampiao", V(0f, .04f, 0f), V(.11f, .04f, .11f), latao, lampiao);
        Cilindro("VidroLampiao", V(0f, .14f, 0f), V(.08f, .08f, .08f), VidroExterior(new Color(.55f, .62f, .66f, .32f)), lampiao);
        Cilindro("ChapeuLampiao", V(0f, .24f, 0f), V(.11f, .03f, .11f), ferro, lampiao);
    }

    private void ConstruirMesaDaRonda(Material madeiraEscura, Material madeiraMedia, Material latao,
        Material couro, Material papel, Material tinta, Material pratoMetal, Material cera, Material pavio, Material chama)
    {
        var mesa = Pivo("MesaDaRonda", world, V(2.15f, 0f, 2.85f));

        // Tampo e estrutura da mesa
        Caixa("TampoMesa", V(0f, .75f, 0f), V(1.64f, .06f, .88f), madeiraMedia, mesa);
        Caixa("GaveteiroMesa", V(.46f, .48f, 0f), V(.56f, .48f, .78f), madeiraEscura, mesa);
        Caixa("PuxadorGaveta1", V(.46f, .60f, -.40f), V(.14f, .03f, .03f), latao, mesa, solida: false);
        Caixa("PuxadorGaveta2", V(.46f, .38f, -.40f), V(.14f, .03f, .03f), latao, mesa, solida: false);
        Caixa("PernaMesaEsqF", V(-.72f, .36f, -.36f), V(.08f, .72f, .08f), madeiraEscura, mesa);
        Caixa("PernaMesaEsqT", V(-.72f, .36f, .36f), V(.08f, .72f, .08f), madeiraEscura, mesa);

        // Cadeira do vigia levemente afastada para nao obstruir a aproximacao
        var cadeira = Pivo("CadeiraRonda", mesa, V(-.22f, 0f, -.72f));
        cadeira.localRotation = Quaternion.Euler(0f, -18f, 0f);
        Caixa("AssentoCadeira", V(0f, .44f, 0f), V(.44f, .05f, .44f), madeiraMedia, cadeira);
        Caixa("EncostoCadeira", V(0f, .72f, -.19f), V(.44f, .52f, .05f), madeiraEscura, cadeira);
        Caixa("PernaCad1", V(-.18f, .22f, -.18f), V(.05f, .44f, .05f), madeiraEscura, cadeira, solida: false);
        Caixa("PernaCad2", V(.18f, .22f, -.18f), V(.05f, .44f, .05f), madeiraEscura, cadeira, solida: false);
        Caixa("PernaCad3", V(-.18f, .22f, .18f), V(.05f, .44f, .05f), madeiraEscura, cadeira, solida: false);
        Caixa("PernaCad4", V(.18f, .22f, .18f), V(.05f, .44f, .05f), madeiraEscura, cadeira, solida: false);

        // Caderno de velas aberto com capa e lombada de couro
        var caderno = Pivo("CadernoDeVelas", mesa, V(-.18f, .785f, -.04f));
        caderno.localRotation = Quaternion.Euler(0f, 8f, 0f);
        Caixa("CapaCouroCaderno", V(0f, .006f, 0f), V(.42f, .012f, .30f), couro, caderno, solida: false);
        Caixa("LombadaCaderno", V(0f, .016f, 0f), V(.032f, .020f, .305f), couro, caderno, solida: false);

        var pagEsq = Pivo("PaginaEsqPivo", caderno, V(-.10f, .018f, 0f));
        pagEsq.localRotation = Quaternion.Euler(0f, 0f, -3f);
        Caixa("PaginaEsq", V(0f, 0f, 0f), V(.18f, .012f, .27f), papel, pagEsq, solida: false);
        Caixa("LinhasEsq1", V(0f, .007f, .06f), V(.13f, .002f, .014f), tinta, pagEsq, solida: false);
        Caixa("LinhasEsq2", V(-.01f, .007f, .01f), V(.11f, .002f, .014f), tinta, pagEsq, solida: false);
        Caixa("LinhasEsq3", V(0f, .007f, -.04f), V(.12f, .002f, .014f), tinta, pagEsq, solida: false);

        var pagDir = Pivo("PaginaDirPivo", caderno, V(.10f, .018f, 0f));
        pagDir.localRotation = Quaternion.Euler(0f, 0f, 3f);
        Caixa("PaginaDir", V(0f, 0f, 0f), V(.18f, .012f, .27f), papel, pagDir, solida: false);
        Caixa("LinhasDir1", V(0f, .007f, .07f), V(.13f, .002f, .014f), tinta, pagDir, solida: false);
        Caixa("LinhasDir2", V(.01f, .007f, .02f), V(.10f, .002f, .014f), tinta, pagDir, solida: false);
        Caixa("LinhasDir3", V(0f, .007f, -.03f), V(.12f, .002f, .014f), tinta, pagDir, solida: false);

        AlvoFisico(caderno, "AlvoCadernoVelas", V(0f, .05f, 0f), V(.54f, .22f, .44f),
            "caderno_velas", "Ler caderno de velas");

        // Caneca metalica ao lado do caderno
        Cilindro("CanecaRonda", V(-.56f, .83f, .16f), V(.07f, .05f, .07f), pratoMetal, mesa);

        // Pires de metal com vela acesa tremulante projetando sombras na mesa e paredes
        var vela = Pivo("VelaMesaRonda", mesa, V(.36f, .782f, .14f));
        Cilindro("PiresMetal", V(0f, .008f, 0f), V(.14f, .008f, .14f), pratoMetal, vela);
        Cilindro("BordaPires", V(0f, .014f, 0f), V(.155f, .005f, .155f), pratoMetal, vela);
        Cilindro("CorpoVela", V(0f, .085f, 0f), V(.048f, .072f, .048f), cera, vela);
        Cilindro("PavioVela", V(0f, .165f, 0f), V(.006f, .012f, .006f), pavio, vela);

        var chamaGo = Esfera("ChamaVelaRonda", V(0f, .195f, 0f), V(.022f, .046f, .022f), chama, vela);
        var chamaTr = chamaGo.transform;
        var luzVela = Luz("LuzVelaRonda", V(0f, .23f, 0f), new Color(1f, .64f, .24f), 1.35f, 5.8f, vela);
        luzVela.shadows = LightShadows.Soft;
        luzVela.shadowStrength = .78f;

        float tempoVela = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoVela += dt;
            float tremor = Mathf.Sin(tempoVela * 7.3f) * .5f + Mathf.Sin(tempoVela * 13.7f) * .35f + Mathf.Sin(tempoVela * 21.1f) * .15f;
            luzVela.intensity = 1.35f + tremor * .28f;
            luzVela.transform.localPosition = new Vector3(
                Mathf.Sin(tempoVela * 9.5f) * .006f,
                .23f + Mathf.Cos(tempoVela * 11.2f) * .005f,
                Mathf.Cos(tempoVela * 8.1f) * .006f);
            float sx = .022f + tremor * .003f;
            float sy = .046f + tremor * .007f;
            chamaTr.localScale = new Vector3(sx, sy, sx);
            chamaTr.localPosition = new Vector3(Mathf.Sin(tempoVela * 10.4f) * .003f, .195f + tremor * .003f, 0f);
        });
    }
}
