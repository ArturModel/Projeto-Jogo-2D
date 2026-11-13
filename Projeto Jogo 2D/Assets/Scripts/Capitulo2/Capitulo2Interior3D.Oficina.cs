using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarOficina()
    {
        const float xMin = -5.5f, xMax = 5.5f;
        const float zMin = -4.5f, zMax = 6.5f;
        const float altura = 3.1f;

        ConfigurarEntrada(zMin, V(0f, EyeHeight, -2.85f), "Foley/footstep_wood_000");

        // Materiais principais da oficina
        var texTabuas = CriarTabuas(new Color(.33f, .23f, .15f), new Color(.26f, .18f, .12f));
        var pisoMat = Superficie("Oficina_PisoTabuas", Color.white, texTabuas, new Vector2(6f, 6f), brilho: .16f);
        var rebocoMat = Superficie("Oficina_Reboco", new Color(.44f, .40f, .33f), brilho: .06f);
        var lambrilMat = Superficie("Oficina_LambrilAzul", new Color(.14f, .24f, .28f), brilho: .14f);
        var madeiraEscura = Superficie("Oficina_MadeiraEscura", new Color(.22f, .15f, .10f), brilho: .18f);
        var madeiraBancada = Superficie("Oficina_MadeiraBancada", new Color(.38f, .26f, .16f), brilho: .20f);
        var madeiraPorta = Superficie("Oficina_MadeiraPorta", new Color(.30f, .20f, .13f), brilho: .18f);
        var tetoMat = Superficie("Oficina_Teto", new Color(.24f, .19f, .15f), brilho: .06f);
        var metalEscuro = Superficie("Oficina_MetalEscuro", new Color(.18f, .19f, .21f), brilho: .42f);
        var metalClaro = Superficie("Oficina_MetalClaro", new Color(.56f, .58f, .60f), brilho: .62f);
        var latao = Superficie("Oficina_Latao", new Color(.74f, .58f, .26f), brilho: .65f);
        var cobre = Superficie("Oficina_Cobre", new Color(.76f, .40f, .20f), brilho: .58f);
        var borrachaMat = Superficie("Oficina_Borracha", new Color(.12f, .14f, .15f), brilho: .08f);
        var esmalteVerde = Superficie("Oficina_EsmalteVerde", new Color(.16f, .32f, .24f), brilho: .45f);
        var papelEsquema = Superficie("Oficina_PapelEsquema", new Color(.84f, .86f, .80f), brilho: .05f);
        var azulPlanta = Superficie("Oficina_AzulPlanta", new Color(.15f, .28f, .46f), brilho: .08f);
        var tracoClaro = Superficie("Oficina_TracoClaro", new Color(.85f, .91f, .96f), unlit: true);
        var vidro = VidroExterior();

        // Piso e teto
        Caixa("PisoOficina", V(0f, -.05f, 1f), V(xMax - xMin, .10f, zMax - zMin), pisoMat);
        Caixa("TetoOficina", V(0f, altura + .05f, 1f), V(xMax - xMin, .10f, zMax - zMin), tetoMat);

        // Caibros de madeira no teto
        for (float z = -3.2f; z <= 5.6f; z += 2.2f)
            Caixa("CaibroTeto", V(0f, altura - .08f, z), V(xMax - xMin - .1f, .16f, .14f), madeiraEscura, solida: false);
        for (float x = -3.6f; x <= 3.6f; x += 3.6f)
            Caixa("VigaLongitudinal", V(x, altura - .04f, 1f), V(.12f, .08f, zMax - zMin - .1f), madeiraEscura, solida: false);

        // Paredes principais (fundo, esquerda, direita) com lambril azul-petróleo
        Caixa("ParedeFundo", V(0f, altura * .5f, zMax + .06f), V(xMax - xMin + .24f, altura, .12f), rebocoMat);
        Caixa("LambrilFundo", V(0f, .52f, zMax - .01f), V(xMax - xMin, 1.04f, .03f), lambrilMat, solida: false);
        Caixa("RodaMeioFundo", V(0f, 1.06f, zMax - .02f), V(xMax - xMin, .05f, .04f), madeiraEscura, solida: false);

        Caixa("ParedeEsquerda", V(xMin - .06f, altura * .5f, 1f), V(.12f, altura, zMax - zMin + .24f), rebocoMat);
        Caixa("LambrilEsquerda", V(xMin + .01f, .52f, 1f), V(.03f, 1.04f, zMax - zMin), lambrilMat, solida: false);
        Caixa("RodaMeioEsquerda", V(xMin + .02f, 1.06f, 1f), V(.04f, .05f, zMax - zMin), madeiraEscura, solida: false);

        Caixa("ParedeDireita", V(xMax + .06f, altura * .5f, 1f), V(.12f, altura, zMax - zMin + .24f), rebocoMat);
        Caixa("LambrilDireita", V(xMax - .01f, .52f, 1f), V(.03f, 1.04f, zMax - zMin), lambrilMat, solida: false);
        Caixa("RodaMeioDireita", V(xMax - .02f, 1.06f, 1f), V(.04f, .05f, zMax - zMin), madeiraEscura, solida: false);

        // Parede frontal (z = -4.5f) com vão da janela e porta para a rua chuvosa (em ordem crescente de X)
        var vaoJanela = new Rect(-3.65f, .92f, 1.90f, 1.28f);
        var vaoPorta = new Rect(-.53f, 0f, 1.06f, 2.18f);
        ExteriorInterior3D.Parede(xMin, xMax, altura, new[] { vaoJanela, vaoPorta }, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeFrontalTrecho", V((a + b) * .5f, (y0 + y1) * .5f, zMin - .06f), V(w, h, .12f), rebocoMat);
        });

        // Lambril frontal nos trechos sólidos (sem cruzar o vão da janela)
        Caixa("LambrilFrontalEsq", V(-4.575f, .52f, zMin + .01f), V(1.85f, 1.04f, .03f), lambrilMat, solida: false);
        Caixa("LambrilFrontalJanela", V(-2.70f, .46f, zMin + .01f), V(1.90f, .92f, .03f), lambrilMat, solida: false);
        Caixa("LambrilFrontalMeio", V(-1.14f, .52f, zMin + .01f), V(1.22f, 1.04f, .03f), lambrilMat, solida: false);
        Caixa("LambrilFrontalDir", V(3.015f, .52f, zMin + .01f), V(4.97f, 1.04f, .03f), lambrilMat, solida: false);
        Caixa("RodaMeioFrontalEsqExt", V(-4.575f, 1.06f, zMin + .02f), V(1.85f, .05f, .04f), madeiraEscura, solida: false);
        Caixa("RodaMeioFrontalMeio", V(-1.14f, 1.06f, zMin + .02f), V(1.22f, .05f, .04f), madeiraEscura, solida: false);
        Caixa("RodaMeioFrontalDir", V(3.015f, 1.06f, zMin + .02f), V(4.97f, .05f, .04f), madeiraEscura, solida: false);

        // Janela frontal esquerda com vista para a rua
        var centroJanela = V(-2.70f, 1.56f, zMin - .04f);
        MolduraVazada(centroJanela, 1.90f, 1.28f, madeiraEscura, .07f, .14f);
        Caixa("PeitorilJanela", V(-2.70f, .91f, zMin + .03f), V(2.04f, .05f, .22f), madeiraBancada, solida: false);
        Caixa("TravessaJanelaV", centroJanela, V(.05f, 1.28f, .06f), madeiraEscura, solida: false);
        Caixa("TravessaJanelaH", centroJanela, V(1.90f, .04f, .06f), madeiraEscura, solida: false);
        Caixa("VidroJanelaFrontal", centroJanela, V(1.86f, 1.24f, .02f), vidro, solida: false);

        // Porta de entrada, chuva e exterior visível
        ConstruirPorta(madeiraPorta, madeiraEscura, latao, 0f, zMin, 1.06f, 2.18f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, zMin - .03f), 180f, ExteriorInterior3D.Local.OficinaRua);

        // Capacho na entrada
        Caixa("CapachoEntrada", V(0f, .01f, -3.95f), V(1.05f, .02f, .62f),
            Superficie("Oficina_Capacho", new Color(.29f, .22f, .15f), brilho: .04f), solida: false);

        // ============================================================
        // BANCADA PRINCIPAL DA MILA (x = 1.4f, z = 3.2f)
        // ============================================================
        var bancada = Pivo("BancadaMila", world, V(1.4f, 0f, 3.2f));
        Caixa("TampoBancada", V(0f, .84f, 0f), V(2.45f, .06f, .98f), madeiraBancada, bancada);
        Caixa("AbaTraseiraBancada", V(0f, .94f, .46f), V(2.45f, .14f, .05f), madeiraEscura, bancada, solida: false);
        Caixa("PrateleiraInferiorBancada", V(0f, .24f, 0f), V(2.26f, .04f, .82f), madeiraEscura, bancada, solida: false);
        foreach (float px in new[] { -1.12f, 1.12f })
        foreach (float pz in new[] { -.40f, .40f })
            Caixa("PeBancada", V(px, .42f, pz), V(.09f, .84f, .09f), madeiraEscura, bancada, solida: false);

        // Tapete antiestático de borracha sobre a bancada
        Caixa("TapeteAntiestatico", V(-.18f, .874f, -.04f), V(1.18f, .008f, .62f), borrachaMat, bancada, solida: false);

        // Rádio de teste ("radio_test") sobre a bancada
        MontarRadioDeTeste(bancada, V(-.32f, .88f, -.06f), madeiraPorta, latao, metalEscuro);

        // Ferro de solda e suporte
        var solda = Pivo("EstacaoSolda", bancada, V(.36f, .88f, -.12f));
        Caixa("BaseSolda", V(0f, .03f, 0f), V(.16f, .06f, .14f), metalEscuro, solda, solida: false);
        Caixa("EsponjaSolda", V(.02f, .063f, .02f), V(.07f, .012f, .06f),
            Superficie("Oficina_Esponja", new Color(.65f, .54f, .24f), brilho: .05f), solda, solida: false);
        var caboSolda = Cilindro("FerroSolda", V(-.05f, .09f, -.02f), V(.026f, .11f, .026f),
            Superficie("Oficina_CaboAzul", new Color(.14f, .34f, .52f), brilho: .25f), solda);
        caboSolda.transform.localRotation = Quaternion.Euler(48f, 15f, -20f);
        var pontaSolda = Cilindro("PontaSolda", V(-.02f, .14f, .04f), V(.010f, .06f, .010f), metalClaro, solda);
        pontaSolda.transform.localRotation = Quaternion.Euler(48f, 15f, -20f);

        // Carretéis de fio de cobre
        for (int i = 0; i < 3; i++)
        {
            var xC = .58f + i * .16f;
            var zC = .18f - (i % 2) * .12f;
            Cilindro("CarretelBase_" + i, V(xC, .885f, zC), V(.10f, .008f, .10f), madeiraEscura, bancada);
            Cilindro("CarretelFio_" + i, V(xC, .93f, zC), V(.078f, .04f, .078f), cobre, bancada);
            Cilindro("CarretelTopo_" + i, V(xC, .975f, zC), V(.10f, .008f, .10f), madeiraEscura, bancada);
        }

        // Multímetro analógico
        var multi = Pivo("Multimetro", bancada, V(.26f, .88f, .16f));
        multi.localRotation = Quaternion.Euler(0f, -18f, 0f);
        Caixa("CorpoMultimetro", V(0f, .022f, 0f), V(.14f, .044f, .20f),
            Superficie("Oficina_AmareloMulti", new Color(.78f, .58f, .16f), brilho: .25f), multi, solida: false);
        Caixa("VisorMultimetro", V(0f, .045f, .045f), V(.10f, .004f, .07f), papelEsquema, multi, solida: false);
        Cilindro("SeletorMultimetro", V(0f, .048f, -.035f), V(.05f, .008f, .05f), metalEscuro, multi);

        // Luminária articulada metálica da bancada + SpotLuz focada
        var lum = Pivo("LuminariaArticulada", bancada, V(.82f, .87f, .24f));
        Cilindro("BaseLuminaria", V(0f, .018f, 0f), V(.18f, .018f, .18f), metalEscuro, lum);
        var bracoInf = Caixa("BracoInfLum", V(-.06f, .21f, -.04f), V(.024f, .38f, .024f), metalClaro, lum, solida: false);
        bracoInf.transform.localRotation = Quaternion.Euler(-16f, 0f, 18f);
        var bracoSup = Caixa("BracoSupLum", V(-.24f, .44f, -.14f), V(.022f, .36f, .022f), metalClaro, lum, solida: false);
        bracoSup.transform.localRotation = Quaternion.Euler(34f, 0f, 48f);
        var cupula = Cilindro("CupulaLuminaria", V(-.42f, .48f, -.22f), V(.18f, .08f, .18f), esmalteVerde, lum);
        cupula.transform.localRotation = Quaternion.Euler(18f, 0f, 22f);
        var lampadaGlow = Superficie("Oficina_LampadaBancada", new Color(1f, .92f, .72f), unlit: true);
        Esfera("BulboLuminaria", V(-.42f, .44f, -.22f), V(.06f, .06f, .06f), lampadaGlow, lum);
        SpotLuz("SpotBancadaMila", V(-.42f, .42f, -.22f), V(74f, -138f, 0f),
            new Color(1f, .84f, .54f), 2.15f, 3.8f, 54f, 22f, lum);

        // Caixas de válvulas e componentes sob a bancada
        Caixa("CaixaValvulas1", V(-.55f, .36f, .04f), V(.44f, .22f, .36f),
            Superficie("Oficina_Papelao", new Color(.48f, .38f, .25f), brilho: .04f), bancada, solida: false);
        Caixa("CaixaValvulas2", V(.38f, .38f, -.02f), V(.50f, .26f, .42f), metalEscuro, bancada, solida: false);

        // Mila ao lado/atrás da bancada
        Personagem("Mila", "Art/Characters/Npc_Mila", V(2.8f, 0f, 4.1f), "mila");

        // ============================================================
        // ARQUIVO DE ESQUEMAS ("desenho_radio") em x = -2.6f, z = 5.4f
        // ============================================================
        var arquivo = Pivo("ArquivoEsquemas", world, V(-2.6f, 0f, 5.4f));
        Caixa("CorpoArquivo", V(0f, .58f, 0f), V(1.50f, 1.16f, .64f), madeiraEscura, arquivo);
        for (int g = 0; g < 4; g++)
        {
            float gy = .20f + g * .25f;
            Caixa("GavetaArquivo_" + g, V(0f, gy, -.325f), V(1.36f, .20f, .03f), madeiraBancada, arquivo, solida: false);
            Caixa("PuxadorGaveta_" + g, V(0f, gy, -.345f), V(.18f, .025f, .025f), latao, arquivo, solida: false);
            Caixa("EtiquetaGaveta_" + g, V(0f, gy + .045f, -.342f), V(.16f, .04f, .006f), papelEsquema, arquivo, solida: false);
        }

        // Tampo inclinado e esquema do rádio aberto e bem visível
        var prancha = Pivo("PranchaEsquema", arquivo, V(0f, 1.24f, -.04f));
        prancha.localRotation = Quaternion.Euler(-22f, 0f, 0f);
        Caixa("BasePrancha", V(0f, 0f, 0f), V(1.42f, .04f, .76f), madeiraBancada, prancha, solida: false);
        Caixa("FolhaPlantaRadio", V(0f, .026f, -.02f), V(1.04f, .008f, .62f), azulPlanta, prancha, solida: false);
        // Desenho técnico simplificado na planta
        Caixa("DiagramaBorda", V(0f, .032f, -.02f), V(.96f, .004f, .54f), tracoClaro, prancha, solida: false);
        Caixa("DiagramaFundo", V(0f, .034f, -.02f), V(.92f, .004f, .50f), azulPlanta, prancha, solida: false);
        Caixa("DiagramaBobina", V(-.22f, .037f, .06f), V(.24f, .004f, .14f), tracoClaro, prancha, solida: false);
        Caixa("DiagramaValvula", V(.18f, .037f, .02f), V(.18f, .004f, .22f), tracoClaro, prancha, solida: false);
        Caixa("DiagramaLinhaH", V(0f, .037f, -.12f), V(.68f, .004f, .016f), tracoClaro, prancha, solida: false);
        foreach (float px in new[] { -.46f, .46f })
        foreach (float pz in new[] { -.26f, .22f })
            Cilindro("TachinhaPlanta", V(px, .036f, pz), V(.022f, .008f, .022f), latao, prancha);

        AlvoFisico(prancha, "AlvoDesenhoRadio", V(0f, .08f, 0f), V(1.22f, .38f, .80f),
            "desenho_radio", "Examinar esquema do rádio");

        // ============================================================
        // PAREDE ESQUERDA: ESTANTE DE PEÇAS E TELEVISOR CRT
        // ============================================================
        var estante = Pivo("EstantePecas", world, V(-4.95f, 0f, 1.3f));
        Caixa("FundoEstante", V(-.36f, 1.05f, 0f), V(.04f, 2.10f, 2.10f), madeiraEscura, estante);
        for (int p = 0; p < 5; p++)
            Caixa("Prateleira_" + p, V(0f, .14f + p * .48f, 0f), V(.72f, .04f, 2.08f), madeiraBancada, estante,
                solida: p == 0);
        foreach (float pz in new[] { -1.02f, 1.02f })
            Caixa("LateralEstante", V(0f, 1.05f, pz), V(.72f, 2.10f, .04f), madeiraEscura, estante, solida: false);

        // Gaveteiros organizadores de componentes na estante
        var corGavetaA = Superficie("Oficina_GavetaVerde", new Color(.22f, .34f, .28f), brilho: .2f);
        var corGavetaB = Superficie("Oficina_GavetaOcre", new Color(.56f, .42f, .20f), brilho: .2f);
        for (int col = 0; col < 4; col++)
        for (int row = 0; row < 3; row++)
        {
            float gz = -.76f + col * .50f;
            float gy = .74f + row * .12f;
            Caixa("GavetaComp", V(.08f, gy, gz), V(.46f, .09f, .42f), (col + row) % 2 == 0 ? corGavetaA : corGavetaB, estante, solida: false);
            Caixa("EtiquetaComp", V(.315f, gy, gz), V(.01f, .04f, .14f), papelEsquema, estante, solida: false);
        }
        // Bobinas e caixas nas prateleiras superiores
        for (int i = 0; i < 4; i++)
            Cilindro("RoloFioEstante_" + i, V(.06f, 1.68f, -.66f + i * .44f), V(.22f, .10f, .22f),
                i % 2 == 0 ? cobre : metalEscuro, estante);

        // Carrinho com Televisor CRT para conserto (x = -4.75f, z = 3.9f)
        var carrinho = Pivo("CarrinhoTV", world, V(-4.75f, 0f, 3.9f));
        Caixa("TampoCarrinho", V(0f, .64f, 0f), V(.86f, .05f, .96f), metalEscuro, carrinho);
        Caixa("BaseCarrinho", V(0f, .14f, 0f), V(.82f, .04f, .92f), metalEscuro, carrinho, solida: false);
        foreach (float cx in new[] { -.38f, .38f })
        foreach (float cz in new[] { -.42f, .42f })
            Caixa("HasteCarrinho", V(cx, .39f, cz), V(.04f, .50f, .04f), metalClaro, carrinho, solida: false);

        var tvArt = Arte("TelevisorCRT_Arte", "Art/Props/Televisor", V(0f, 1.06f, 0f), .78f, yaw: -90f, pai: carrinho);
        if (tvArt == null)
        {
            // Fallback 3D detalhado caso sprite não seja carregado
            Caixa("GabineteTV", V(0f, .96f, 0f), V(.54f, .58f, .70f), madeiraPorta, carrinho, solida: false);
            Caixa("TelaTV", V(.275f, .98f, -.08f), V(.02f, .42f, .44f),
                Superficie("Oficina_TelaCRT", new Color(.14f, .18f, .17f), brilho: .72f), carrinho, solida: false);
            Cilindro("DialTV_1", V(.28f, 1.06f, .22f), V(.06f, .02f, .06f), latao, carrinho);
            Cilindro("DialTV_2", V(.28f, .94f, .22f), V(.06f, .02f, .06f), latao, carrinho);
        }

        // Banco de espera perto da entrada (x = 2.65f, z = -3.65f)
        var banco = Pivo("BancoEspera", world, V(2.65f, 0f, -3.65f));
        Caixa("AssentoBanco", V(0f, .45f, 0f), V(1.65f, .06f, .48f), madeiraBancada, banco);
        Caixa("EncostoBanco", V(0f, .78f, -.20f), V(1.65f, .28f, .05f), madeiraBancada, banco, solida: false);
        foreach (float bx in new[] { -.72f, .72f })
        {
            Caixa("PeBancoF", V(bx, .22f, .18f), V(.07f, .44f, .07f), madeiraEscura, banco, solida: false);
            Caixa("PeBancoT", V(bx, .44f, -.18f), V(.07f, .88f, .07f), madeiraEscura, banco, solida: false);
        }
        // Pilha de revistas técnicas no banco
        Caixa("Revista1", V(.45f, .49f, .02f), V(.24f, .016f, .32f), papelEsquema, banco, solida: false);
        Caixa("Revista2", V(.47f, .506f, -.01f), V(.24f, .016f, .32f), azulPlanta, banco, solida: false);

        // Relógio de parede na parede de fundo com pêndulo animado
        var relogio = Pivo("RelogioParede", world, V(.45f, 2.25f, zMax - .06f));
        Caixa("CaixaRelogio", V(0f, 0f, 0f), V(.44f, .58f, .12f), madeiraEscura, relogio, solida: false);
        Cilindro("MostradorRelogio", V(0f, .12f, -.065f), V(.30f, .01f, .30f), papelEsquema, relogio)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        var pendulo = Pivo("PenduloRelogio", relogio, V(0f, -.02f, -.05f));
        Caixa("HastePendulo", V(0f, -.10f, 0f), V(.016f, .20f, .01f), latao, pendulo, solida: false);
        Cilindro("DiscoPendulo", V(0f, -.21f, 0f), V(.07f, .012f, .07f), latao, pendulo)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        float tempoPendulo = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoPendulo += dt;
            pendulo.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(tempoPendulo * 2.4f) * 11f);
        });
        SomLocal("Pendulo", V(.45f, 2.25f, zMax - .06f), 7.8f, .54f);
        SomLocal("MotorVentilador", V(1.4f, .90f, 3.2f), 5.6f, .24f);

        // ============================================================
        // ILUMINAÇÃO PENDENTE E AMBIENTE TENSO
        // ============================================================
        CriarPendenteOficina(V(-1.8f, altura, 1.2f), esmalteVerde, metalEscuro, .88f, 5.8f, oscilar: true, semente: 2.1f);
        CriarPendenteOficina(V(1.6f, altura, 3.0f), esmalteVerde, metalEscuro, .78f, 5.5f, oscilar: false, semente: 0f);
        Luz("LuzEntradaOficina", V(0f, 2.55f, -3.1f), new Color(1f, .76f, .48f), .45f, 4.5f);
        Luz("LuzEsquemas", V(-2.6f, 2.35f, 4.7f), new Color(.72f, .80f, .94f), .75f, 4.4f);
        Luz("LuzJanelaChuva", V(-2.7f, 1.65f, -3.8f), new Color(.44f, .58f, .76f), .50f, 3.8f);
    }

    private void MontarRadioDeTeste(Transform pai, Vector3 pos, Material madeira, Material latao, Material metal)
    {
        var radio = Pivo("RadioDeTeste", pai, pos);
        Caixa("GabineteRadio", V(0f, .16f, 0f), V(.54f, .32f, .28f), madeira, radio, solida: false);
        Caixa("MolduraRadio", V(0f, .16f, -.142f), V(.50f, .28f, .015f), latao, radio, solida: false);

        // Grade do alto-falante à esquerda
        var tecidoGrelha = Superficie("Oficina_GrelhaRadio", new Color(.20f, .17f, .13f), brilho: .05f);
        Caixa("FundoGrelhaRadio", V(-.11f, .16f, -.148f), V(.22f, .22f, .01f), tecidoGrelha, radio, solida: false);
        for (int i = -2; i <= 2; i++)
            Caixa("RipinhaGrelha_" + i, V(-.11f + i * .042f, .16f, -.154f), V(.012f, .22f, .008f), latao, radio, solida: false);

        // Mostrador iluminado à direita com pulso sutil
        var dialMat = BulboInstancia("Oficina_DialRadio", new Color(1f, .76f, .34f));
        Caixa("DialRadio", V(.13f, .20f, -.148f), V(.18f, .09f, .01f), dialMat, radio, solida: false);
        Caixa("PonteiroDial", V(.11f, .20f, -.155f), V(.01f, .085f, .006f),
            Superficie("Oficina_Ponteiro", new Color(.85f, .20f, .14f), unlit: true), radio, solida: false);

        // Botões giratórios de sintonia e volume
        for (int b = 0; b < 3; b++)
        {
            var botao = Cilindro("BotaoRadio_" + b, V(.06f + b * .07f, .09f, -.155f), V(.038f, .018f, .038f), metal, radio);
            botao.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        // Antena telescópica traseira
        var antena = Cilindro("AntenaRadio", V(.20f, .44f, .10f), V(.010f, .26f, .010f),
            Superficie("Oficina_Antena", new Color(.68f, .70f, .72f), brilho: .7f), radio);
        antena.transform.localRotation = Quaternion.Euler(0f, 0f, -12f);

        var luzDial = Luz("LuzDialRadio", V(.13f, .20f, -.22f), new Color(1f, .70f, .26f), .68f, 1.45f, radio);
        float tempoRadio = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoRadio += dt;
            float pulso = .78f + Mathf.Sin(tempoRadio * 3.1f) * .16f + Mathf.Sin(tempoRadio * 7.3f) * .06f;
            luzDial.intensity = .68f * pulso;
            dialMat.color = new Color(1f * pulso, .76f * pulso, .34f * pulso);
        });

        AlvoFisico(radio, "AlvoRadioTeste", V(0f, .18f, -.04f), V(.64f, .44f, .42f),
            "radio_test", "Examinar rádio de teste");
    }

    private void CriarPendenteOficina(Vector3 tetoPos, Material cupulaMat, Material caboMat, float intensidade, float alcance,
        bool oscilar = false, float semente = 0f)
    {
        var pendente = Pivo("PendenteOficina", world, tetoPos);
        Cilindro("FioPendente", V(0f, -.32f, 0f), V(.016f, .32f, .016f), caboMat, pendente);
        Cilindro("SoquetePendente", V(0f, -.62f, 0f), V(.06f, .05f, .06f), caboMat, pendente);
        Cilindro("CupulaPendente", V(0f, -.70f, 0f), V(.38f, .09f, .38f), cupulaMat, pendente);
        var corAcesa = new Color(1f, .86f, .60f);
        var bulboMat = oscilar
            ? BulboInstancia("Oficina_BulboFalho", corAcesa)
            : Superficie("Oficina_BulboQuente", corAcesa, unlit: true);
        Esfera("BulboPendente", V(0f, -.76f, 0f), V(.09f, .09f, .09f), bulboMat, pendente);
        var luz = Luz("LuzPendente", V(0f, -.88f, 0f), new Color(1f, .80f, .52f), intensidade, alcance, pendente);
        if (oscilar)
            LuzOscilante(luz, bulboMat, intensidade, corAcesa, new Color(.22f, .18f, .14f),
                intensidadeMinFator: .08f, somFaisca: true, ganhoSom: .22f, semente: semente);
    }
}
