using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarPrefeitura()
    {
        ConfigurarEntrada(-4.5f, V(0f, EyeHeight, -2.85f), "Foley/footstep_concrete_000");

        // Materiais cívicos (anos 80/90)
        var pisoTex = CriarCeramica(new Color(.64f, .68f, .59f), new Color(.55f, .60f, .51f));
        var pisoMat = Superficie("PisoPrefeitura", Color.white, pisoTex, new Vector2(13f, 14f), .22f);
        var reboco = Superficie("RebocoPrefeitura", new Color(.50f, .49f, .42f), brilho: .08f);
        var lambril = Superficie("LambrilPrefeitura", new Color(.18f, .25f, .19f), brilho: .16f);
        var tetoMat = Superficie("TetoPrefeitura", new Color(.36f, .35f, .30f), brilho: .06f);
        var madeiraTex = CriarTabuas(new Color(.33f, .22f, .14f), new Color(.27f, .18f, .11f));
        var madeira = Superficie("MadeiraPrefeitura", Color.white, madeiraTex, new Vector2(2f, 2f), .18f);
        var madeiraEscura = Superficie("MadeiraEscuraPrefeitura", new Color(.20f, .13f, .08f), brilho: .20f);
        var acoArquivo = Superficie("AcoArquivoPrefeitura", new Color(.30f, .34f, .31f), brilho: .26f);
        var latao = Superficie("LataoPrefeitura", new Color(.74f, .59f, .27f), brilho: .65f);
        var vidroDivisoria = VidroExterior(new Color(.62f, .70f, .68f, .28f));
        var vidroJanela = VidroExterior();
        var papelCreme = Superficie("PapelCremePrefeitura", new Color(.88f, .84f, .74f), brilho: .06f);
        var cupulaVerde = Superficie("CupulaVerdeBanco", new Color(.11f, .34f, .19f), brilho: .45f);
        var globoLuz = Superficie("GloboLuzPrefeitura", new Color(1f, .93f, .76f), unlit: true);

        // Casca da sala: x [-6.5, 6.5], z [-4.5, 9.5], altura 3.6m
        Caixa("PisoPrefeitura", V(0f, -.05f, 2.5f), V(13.2f, .10f, 14.2f), pisoMat);
        Caixa("TetoPrefeitura", V(0f, 3.65f, 2.5f), V(13.2f, .10f, 14.2f), tetoMat);

        // Vigas do teto em caixotão
        foreach (float zv in new[] { -2.2f, .5f, 3.2f, 5.9f, 8.4f })
            Caixa("VigaTransversal", V(0f, 3.49f, zv), V(13.0f, .22f, .24f), madeiraEscura, solida: false);
        foreach (float xv in new[] { -3.8f, 0f, 3.8f })
            Caixa("VigaLongitudinal", V(xv, 3.52f, 2.5f), V(.22f, .16f, 14.0f), madeiraEscura, solida: false);

        // Paredes laterais
        ParedeLateralPrefeitura(-6.5f, 1f, reboco, lambril, madeiraEscura);
        ParedeLateralPrefeitura(6.5f, -1f, reboco, lambril, madeiraEscura);

        // Parede frontal (z = -4.5) com vão da porta central
        Caixa("ParedeFrontalEsq", V(-3.515f, 1.8f, -4.5f), V(5.97f, 3.6f, .24f), reboco);
        Caixa("ParedeFrontalDir", V(3.515f, 1.8f, -4.5f), V(5.97f, 3.6f, .24f), reboco);
        Caixa("ParedeFrontalSup", V(0f, 2.89f, -4.5f), V(1.06f, 1.42f, .24f), reboco);
        Caixa("LambrilFrontalEsq", V(-3.515f, .575f, -4.37f), V(5.94f, 1.15f, .04f), lambril, solida: false);
        Caixa("FrisoFrontalEsq", V(-3.515f, 1.17f, -4.36f), V(5.94f, .05f, .06f), madeiraEscura, solida: false);
        Caixa("LambrilFrontalDir", V(3.515f, .575f, -4.37f), V(5.94f, 1.15f, .04f), lambril, solida: false);
        Caixa("FrisoFrontalDir", V(3.515f, 1.17f, -4.36f), V(5.94f, .05f, .06f), madeiraEscura, solida: false);

        // Porta de entrada + vista da rua chuvosa
        ConstruirPorta(madeira, madeiraEscura, latao, 0f, -4.5f, 1.06f, 2.18f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, -4.53f), 180f, ExteriorInterior3D.Local.PrefeituraFrente);

        // Parede dos fundos (z = 9.5) com duas janelas altas de vidro noturno
        var vaosFundo = new[]
        {
            new Rect(-3.05f, .95f, 1.30f, 2.00f),
            new Rect(1.75f, .95f, 1.30f, 2.00f)
        };
        ExteriorInterior3D.Parede(-6.5f, 6.5f, 3.6f, vaosFundo, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeFundoTrecho", V((a + b) * .5f, (y0 + y1) * .5f, 9.5f), V(w, h, .24f), reboco);
        });
        Caixa("LambrilFundo", V(0f, .47f, 9.37f), V(12.96f, .94f, .04f), lambril, solida: false);
        Caixa("FrisoFundo", V(0f, .96f, 9.36f), V(12.96f, .05f, .06f), madeiraEscura, solida: false);
        foreach (float xj in new[] { -2.4f, 2.4f })
            ConstruirJanelaNoturna(xj, 1.95f, 9.44f, 1.30f, 2.00f, madeiraEscura, vidroJanela);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, 9.62f), 0f, ExteriorInterior3D.Local.PrefeituraFundos);

        // Divisória de atendimento (madeira e vidro) em z = 3.2f com portão central
        ConstruirDivisoriaArquivo(madeira, madeiraEscura, vidroDivisoria, latao);

        // Salão frontal (z = -4.5 a 3.2)
        MontarSalaoFrontalPrefeitura(madeira, madeiraEscura, acoArquivo, latao, vidroDivisoria, papelCreme, globoLuz);

        // Sala do arquivo público nos fundos (z = 3.2 a 9.5)
        MontarArquivoPrefeitura(madeira, madeiraEscura, acoArquivo, latao, papelCreme, cupulaVerde, globoLuz);
    }

    private void ParedeLateralPrefeitura(float x, float dirInterno, Material reboco, Material lambril, Material friso)
    {
        Caixa("ParedeLateral", V(x, 1.8f, 2.5f), V(.24f, 3.6f, 14.2f), reboco);
        Caixa("LambrilLateral", V(x + dirInterno * .13f, .575f, 2.5f), V(.04f, 1.15f, 13.96f), lambril, solida: false);
        Caixa("FrisoLateral", V(x + dirInterno * .14f, 1.17f, 2.5f), V(.06f, .05f, 13.96f), friso, solida: false);
        Caixa("RodapeLateral", V(x + dirInterno * .14f, .07f, 2.5f), V(.05f, .14f, 13.96f), friso, solida: false);
    }

    private void ConstruirJanelaNoturna(float x, float y, float z, float largura, float altura,
        Material moldura, Material vidro)
    {
        MolduraVazada(V(x, y, z), largura, altura, moldura, .07f, .14f);
        Caixa("PeitorilJanela", V(x, y - altura * .5f - .03f, z - .05f), V(largura + .22f, .06f, .24f), moldura, solida: false);
        Caixa("VidroJanela", V(x, y, z), V(largura, altura, .02f), vidro, solida: false);
        Caixa("TravessaVertJanela", V(x, y, z - .01f), V(.04f, altura, .04f), moldura, solida: false);
        Caixa("TravessaHorizJanela", V(x, y + .15f, z - .01f), V(largura, .04f, .04f), moldura, solida: false);
        SpotLuz("LuzLuaJanela", V(x, y + .6f, z + .15f), V(26f, 180f, 0f),
            new Color(.38f, .52f, .80f), .68f, 5.2f, 64f, 22f);
    }

    private void ConstruirDivisoriaArquivo(Material madeira, Material madeiraEscura, Material vidro, Material latao)
    {
        const float z = 3.2f;
        const float meioVao = .65f;

        // Alas esquerda e direita (madeira inferior + vidro superior + bandeira)
        foreach (int sinal in new[] { -1, 1 })
        {
            float cx = sinal * (meioVao + (6.4f - meioVao) * .5f);
            float w = 6.4f - meioVao;
            Caixa("DivisoriaBase", V(cx, .575f, z), V(w, 1.15f, .14f), madeiraEscura);
            Caixa("DivisoriaPeitoril", V(cx, 1.17f, z), V(w, .05f, .18f), madeira);
            Caixa("DivisoriaVidro", V(cx, 1.85f, z), V(w - .10f, 1.30f, .03f), vidro);
            Caixa("DivisoriaVerga", V(cx, 3.05f, z), V(w, 1.10f, .14f), madeiraEscura);

            for (float xm = meioVao; xm <= 6.35f; xm += 1.15f)
                Caixa("MontanteDivisoria", V(sinal * xm, 1.85f, z), V(.08f, 1.36f, .14f), madeiraEscura);
        }

        // Batentes e verga do portão central
        Caixa("BatentePortaoEsq", V(-meioVao - .05f, 1.15f, z), V(.10f, 2.30f, .18f), madeiraEscura);
        Caixa("BatentePortaoDir", V(meioVao + .05f, 1.15f, z), V(.10f, 2.30f, .18f), madeiraEscura);
        Caixa("VergaPortao", V(0f, 2.95f, z), V(1.50f, 1.30f, .16f), madeiraEscura);
        Letreiro("PlacaArquivoPublico", "ARQUIVO PÚBLICO", V(0f, 2.48f, z - .09f), madeiraEscura, 1.38f, .28f);

        // Portão articulado/bloqueável controlado por c2_prefeitura_atendimento
        var portaoGo = new GameObject("PortaoArquivoPublico");
        portaoGo.layer = RoomLayer;
        portaoGo.transform.SetParent(world, false);
        portaoGo.transform.localPosition = V(0f, 0f, z);

        var folhaPortao = Caixa("FolhaPortaoArquivo", V(0f, 1.10f, 0f), V(1.24f, 2.18f, .06f), madeira, portaoGo.transform, solida: true);
        Caixa("VidroPortaoArquivo", V(0f, 1.56f, 0f), V(.92f, .86f, .07f), vidro, portaoGo.transform, solida: false);
        Caixa("TrincoPortaoArquivo", V(.48f, 1.02f, 0f), V(.12f, .05f, .14f), latao, portaoGo.transform, solida: false);

        var door = portaoGo.AddComponent<Capitulo2PublicRecordsDoor3D>();
        door.portaRenderer = folhaPortao.GetComponent<Renderer>();
        door.portaCollider = folhaPortao.GetComponent<Collider>();
        door.Sincronizar();
    }

    private void MontarSalaoFrontalPrefeitura(Material madeira, Material madeiraEscura, Material aco,
        Material latao, Material vidro, Material papel, Material globoLuz)
    {
        // Balcão de atendimento (z = 1.6f, x = -3.6f a -0.5f; corredor livre à direita em x = 0)
        Caixa("BalcaoAtendimento", V(-2.05f, .53f, 1.60f), V(3.10f, 1.06f, .72f), madeira);
        Caixa("TampoBalcao", V(-2.05f, 1.08f, 1.60f), V(3.22f, .05f, .82f), madeiraEscura);
        Caixa("DivisoriaVidroEsq", V(-3.00f, 1.42f, 1.56f), V(1.10f, .64f, .03f), vidro, solida: false);
        Caixa("DivisoriaVidroDir", V(-.85f, 1.42f, 1.56f), V(.65f, .64f, .03f), vidro, solida: false);
        Caixa("PosteDivisoria1", V(-3.55f, 1.42f, 1.56f), V(.05f, .68f, .05f), latao, solida: false);
        Caixa("PosteDivisoria2", V(-2.45f, 1.42f, 1.56f), V(.05f, .68f, .05f), latao, solida: false);
        Caixa("PosteDivisoria3", V(-1.18f, 1.42f, 1.56f), V(.05f, .68f, .05f), latao, solida: false);
        Caixa("PosteDivisoria4", V(-.53f, 1.42f, 1.56f), V(.05f, .68f, .05f), latao, solida: false);
        Caixa("BarraSupDivisoria", V(-2.04f, 1.75f, 1.56f), V(3.08f, .04f, .05f), latao, solida: false);
        Letreiro("PlacaAtendimento", "ATENDIMENTO", V(-1.82f, 1.92f, 1.55f), madeiraEscura, 1.15f, .22f);

        // Itens sobre o balcão: carimbo, almofada, caixa de fichas e Livro de Protocolo
        Caixa("AlmofadaCarimbo", V(-1.28f, 1.12f, 1.46f), V(.14f, .03f, .10f), aco, solida: false);
        Cilindro("BaseCarimbo", V(-1.12f, 1.13f, 1.46f), V(.06f, .02f, .06f), madeiraEscura);
        Cilindro("CaboCarimbo", V(-1.12f, 1.18f, 1.46f), V(.03f, .05f, .03f), madeira);
        Caixa("CaixaFichas", V(-1.68f, 1.16f, 1.50f), V(.26f, .11f, .34f), madeiraEscura, solida: false);
        Caixa("FichasCartoes", V(-1.68f, 1.19f, 1.48f), V(.21f, .08f, .24f), papel, solida: false);

        var capaProtocolo = Superficie("CapaProtocolo", new Color(.22f, .13f, .11f), brilho: .18f);
        var fitaProtocolo = Superficie("FitaProtocolo", new Color(.65f, .16f, .14f), brilho: .12f);
        Caixa("LivroProtocolo", V(-2.25f, 1.14f, 1.46f), V(.44f, .06f, .34f), capaProtocolo, interactionId: "protocolo");
        Caixa("PaginasProtocolo", V(-2.25f, 1.165f, 1.46f), V(.40f, .03f, .30f), papel, solida: false);
        Caixa("FitaProtocolo", V(-2.21f, 1.18f, 1.63f), V(.025f, .01f, .09f), fitaProtocolo, solida: false);

        // Clarice atrás do balcão
        Personagem("Clarice", "Art/Characters/Npc_Clarice", V(-1.4f, 0f, 2.45f), "prefeitura");
        Caixa("MesaApoioClarice", V(-2.85f, .38f, 2.75f), V(1.30f, .76f, .58f), madeira);
        Caixa("PapeisClarice", V(-2.70f, .78f, 2.72f), V(.36f, .04f, .28f), papel, solida: false);

        // Luminária de mesa próxima a Clarice
        Cilindro("BaseLampadaBalcao", V(-.82f, 1.12f, 1.72f), V(.14f, .02f, .14f), latao);
        Cilindro("HasteLampadaBalcao", V(-.82f, 1.28f, 1.72f), V(.03f, .16f, .03f), latao);
        Cilindro("CupulaLampadaBalcao", V(-.88f, 1.44f, 1.68f), V(.22f, .09f, .16f), latao);
        Luz("LuzBalcao", V(-.95f, 1.38f, 1.58f), new Color(1f, .80f, .50f), .95f, 3.8f);

        // Placa de inauguração da praça na parede esquerda
        Caixa("BasePlacaPraca", V(-6.34f, 1.58f, -1.20f), V(.05f, .64f, .92f), madeiraEscura, interactionId: "placa_praca");
        Caixa("ChapaLataoPraca", V(-6.30f, 1.58f, -1.20f), V(.02f, .48f, .76f), latao, solida: false);
        Caixa("LuminariaPlaca", V(-6.24f, 2.04f, -1.20f), V(.16f, .04f, .34f), latao, solida: false);
        SpotLuz("LuzPlacaPraca", V(-6.12f, 2.02f, -1.20f), V(58f, -90f, 0f), new Color(1f, .82f, .52f), .95f, 2.8f, 56f, 18f);

        // Bancos de espera (esquerda e direita)
        ConstruirBancoEspera(V(-5.55f, 0f, -2.75f), 90f, madeira, madeiraEscura);
        ConstruirBancoEspera(V(5.55f, 0f, -2.10f), -90f, madeira, madeiraEscura);
        ConstruirBancoEspera(V(5.55f, 0f, .20f), -90f, madeira, madeiraEscura);

        // Arquivos de aço altos
        ConstruirArquivoAco(V(-5.85f, 0f, 2.25f), aco, latao);
        ConstruirArquivoAco(V(5.85f, 0f, 2.25f), aco, latao);
        ConstruirArquivoAco(V(5.85f, 0f, 1.35f), aco, latao);

        // Mapa municipal emoldurado e retratos cívicos
        Caixa("MolduraMapa", V(6.34f, 1.72f, -.85f), V(.05f, 1.12f, 1.56f), madeiraEscura, solida: false);
        Caixa("FundoMapa", V(6.31f, 1.72f, -.85f), V(.02f, .96f, 1.40f), papel, solida: false);
        Caixa("MolduraRetrato1", V(-4.3f, 1.95f, -4.34f), V(.58f, .74f, .05f), madeiraEscura, solida: false);
        Caixa("TelaRetrato1", V(-4.3f, 1.95f, -4.31f), V(.46f, .62f, .02f), papel, solida: false);
        Caixa("MolduraRetrato2", V(4.3f, 1.95f, -4.34f), V(.58f, .74f, .05f), madeiraEscura, solida: false);
        Caixa("TelaRetrato2", V(4.3f, 1.95f, -4.31f), V(.46f, .62f, .02f), papel, solida: false);

        // Relógio de parede com pêndulo animado na divisória
        ConstruirRelogioMunicipal(V(3.3f, 2.38f, 3.09f), madeiraEscura, latao, papel);

        // Luminárias pendentes do salão frontal (esquerda falhando, direita apagada)
        var corGloboFrente = new Color(1f, .86f, .60f);
        var globoFalhoFrente = BulboInstancia("GloboFalhoPrefeitura", corGloboFrente);
        var luzFrente = ConstruirPendenteCivico(V(-1.7f, 3.05f, -.6f), madeiraEscura, globoFalhoFrente,
            new Color(1f, .84f, .58f), .95f, 5.8f);
        ConstruirPendenteCivico(V(2.3f, 3.05f, -.6f), madeiraEscura,
            Superficie("GloboApagadoPrefeitura", new Color(.20f, .20f, .19f), brilho: .2f),
            new Color(.98f, .89f, .72f), 0f, 0f);

        LuzOscilante(luzFrente, globoFalhoFrente, .95f, corGloboFrente, new Color(.22f, .19f, .14f),
            intensidadeMinFator: .10f, somFaisca: true, ganhoSom: .22f, semente: 1.4f);
        SomLocal("Compressor", V(-5.85f, 1.0f, 2.25f), 6.2f, .20f);
    }

    private void MontarArquivoPrefeitura(Material madeira, Material madeiraEscura, Material aco,
        Material latao, Material papel, Material cupulaVerde, Material globoLuz)
    {
        // Estantes altas de arquivo embaladas com livros e caixas
        ConstruirEstanteArquivo(V(-5.85f, 0f, 4.65f), 90f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(-5.85f, 0f, 6.55f), 90f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(-5.85f, 0f, 8.40f), 90f, madeiraEscura, latao, papel);

        ConstruirEstanteArquivo(V(5.85f, 0f, 4.65f), -90f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(5.85f, 0f, 6.55f), -90f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(5.85f, 0f, 8.40f), -90f, madeiraEscura, latao, papel);

        ConstruirEstanteArquivo(V(-4.55f, 0f, 8.95f), 180f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(0f, 0f, 8.95f), 180f, madeiraEscura, latao, papel);
        ConstruirEstanteArquivo(V(4.55f, 0f, 8.95f), 180f, madeiraEscura, latao, papel);

        // Mesa de leitura central (x = 0, z = 6.4f)
        Caixa("TampoMesaLeitura", V(0f, .76f, 6.40f), V(2.40f, .06f, 1.15f), madeira);
        Caixa("SaiaMesaLeitura", V(0f, .68f, 6.40f), V(2.22f, .10f, .98f), madeiraEscura, solida: false);
        foreach (float px in new[] { -1.06f, 1.06f })
        foreach (float pz in new[] { 5.92f, 6.88f })
            Caixa("PernaMesaLeitura", V(px, .37f, pz), V(.09f, .74f, .09f), madeiraEscura);

        // Cadeiras da mesa de leitura
        ConstruirCadeiraMadeira(V(-.62f, 0f, 5.45f), 0f, madeira, madeiraEscura);
        ConstruirCadeiraMadeira(V(.62f, 0f, 5.45f), 0f, madeira, madeiraEscura);

        // Registro de Manutenção (esquerda da mesa)
        var capaManut = Superficie("CapaManutencao", new Color(.16f, .24f, .20f), brilho: .18f);
        Caixa("LivroManutencao", V(-.62f, .815f, 6.30f), V(.46f, .055f, .34f), capaManut, interactionId: "manutencao");
        Caixa("PaginasManutencao", V(-.62f, .845f, 6.30f), V(.42f, .025f, .30f), papel, solida: false);

        // Extrato Público (direita da mesa)
        var pastaExtrato = Superficie("PastaExtratoPublico", new Color(.64f, .52f, .34f), brilho: .12f);
        Caixa("PastaExtrato", V(.62f, .805f, 6.30f), V(.38f, .03f, .44f), pastaExtrato, interactionId: "extrato_publico");
        Caixa("FolhasExtrato", V(.62f, .825f, 6.30f), V(.32f, .02f, .38f), papel, solida: false);

        // Luminária de banqueiro no centro da mesa iluminando os registros
        Cilindro("BaseBancoMesa", V(0f, .805f, 6.70f), V(.16f, .025f, .16f), latao);
        Cilindro("HasteBancoMesa", V(0f, .99f, 6.72f), V(.03f, .19f, .03f), latao);
        Caixa("CupulaBancoMesa", V(0f, 1.15f, 6.62f), V(.34f, .12f, .18f), cupulaVerde, solida: false);
        var luzMesa = Luz("LuzMesaArquivo", V(0f, 1.07f, 6.52f), new Color(.98f, .80f, .44f), 1.15f, 3.8f);
        SpotLuz("SpotMesaArquivo", V(0f, 1.10f, 6.60f), V(72f, 180f, 0f), new Color(1f, .84f, .50f), 1.45f, 3.0f, 74f, 30f);

        // Pendentes do arquivo (esquerda fraca, direita apagada)
        ConstruirPendenteCivico(V(-2.2f, 3.05f, 6.35f), madeiraEscura, globoLuz, new Color(.90f, .78f, .54f), .68f, 5.2f);
        ConstruirPendenteCivico(V(2.2f, 3.05f, 6.35f), madeiraEscura,
            Superficie("GloboApagadoPrefeitura", new Color(.20f, .20f, .19f), brilho: .2f),
            new Color(.90f, .78f, .54f), 0f, 0f);

        float t = 0f;
        RegistrarAnimacao(dt =>
        {
            t += dt;
            if (luzMesa != null)
                luzMesa.intensity = 1.15f + Mathf.Sin(t * 17f) * .03f;
        });
    }

    private void ConstruirBancoEspera(Vector3 pos, float yaw, Material assento, Material estrutura)
    {
        var p = Pivo("BancoEspera", world, pos);
        p.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Caixa("AssentoBanco", V(0f, .44f, 0f), V(1.65f, .06f, .46f), assento, p);
        Caixa("EncostoBanco", V(0f, .78f, -.19f), V(1.65f, .34f, .05f), assento, p, solida: false);
        foreach (float px in new[] { -.70f, .70f })
        {
            Caixa("PernaBancoFrente", V(px, .22f, .17f), V(.07f, .44f, .07f), estrutura, p, solida: false);
            Caixa("PernaBancoTras", V(px, .46f, -.19f), V(.07f, .92f, .07f), estrutura, p, solida: false);
        }
    }

    private void ConstruirArquivoAco(Vector3 pos, Material aco, Material latao)
    {
        var p = Pivo("ArquivoAco", world, pos);
        Caixa("CorpoArquivo", V(0f, .76f, 0f), V(.54f, 1.52f, .68f), aco, p);
        for (int i = 0; i < 4; i++)
        {
            float y = .24f + i * .35f;
            Caixa("GavetaArquivo", V(0f, y, -.34f), V(.48f, .30f, .03f), aco, p, solida: false);
            Caixa("PuxadorArquivo", V(0f, y, -.365f), V(.14f, .03f, .03f), latao, p, solida: false);
        }
    }

    private void ConstruirEstanteArquivo(Vector3 pos, float yaw, Material madeiraEscura, Material latao, Material papel)
    {
        var p = Pivo("EstanteArquivo", world, pos);
        p.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Caixa("FundoEstante", V(0f, 1.15f, .19f), V(1.56f, 2.30f, .04f), madeiraEscura, p);
        Caixa("LateralEstanteEsq", V(-.76f, 1.15f, 0f), V(.05f, 2.30f, .42f), madeiraEscura, p);
        Caixa("LateralEstanteDir", V(.76f, 1.15f, 0f), V(.05f, 2.30f, .42f), madeiraEscura, p);

        Color[] coresLivros =
        {
            new Color(.34f, .14f, .13f),
            new Color(.15f, .22f, .30f),
            new Color(.19f, .27f, .18f),
            new Color(.36f, .28f, .17f),
            new Color(.25f, .20f, .16f)
        };

        for (int prateleira = 0; prateleira < 5; prateleira++)
        {
            float y = .16f + prateleira * .50f;
            Caixa("Prateleira", V(0f, y, 0f), V(1.50f, .04f, .40f), madeiraEscura, p, solida: false);
            if (prateleira == 4) continue;

            if (prateleira == 2)
            {
                // Caixas de arquivo morto com etiqueta
                for (int c = -1; c <= 1; c++)
                {
                    Caixa("CaixaArquivoMorto", V(c * .44f, y + .16f, -.01f), V(.36f, .28f, .32f),
                        Superficie("PapelaoArquivo", new Color(.54f, .48f, .36f)), p, solida: false);
                    Caixa("EtiquetaCaixa", V(c * .44f, y + .17f, -.175f), V(.16f, .08f, .01f), papel, p, solida: false);
                }
            }
            else
            {
                // Fileira de livros/atas encadernadas
                for (int b = 0; b < 11; b++)
                {
                    float bx = -.63f + b * .125f;
                    float bh = .28f + ((b + prateleira) % 3) * .04f;
                    var cor = coresLivros[(b + prateleira * 2) % coresLivros.Length];
                    Caixa("LivroAta", V(bx, y + .02f + bh * .5f, -.02f), V(.09f, bh, .26f),
                        Superficie("LombadaAta_" + ((b + prateleira * 2) % 5), cor), p, solida: false);
                    Caixa("FileteAta", V(bx, y + bh * .75f, -.152f), V(.06f, .02f, .01f), latao, p, solida: false);
                }
            }
        }
    }

    private void ConstruirCadeiraMadeira(Vector3 pos, float yaw, Material madeira, Material madeiraEscura)
    {
        var p = Pivo("CadeiraLeitura", world, pos);
        p.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Caixa("AssentoCadeira", V(0f, .45f, 0f), V(.44f, .04f, .44f), madeira, p);
        Caixa("EncostoCadeira", V(0f, .82f, -.19f), V(.42f, .32f, .04f), madeira, p, solida: false);
        foreach (float px in new[] { -.18f, .18f })
        {
            Caixa("PernaCadeiraF", V(px, .225f, .18f), V(.045f, .45f, .045f), madeiraEscura, p, solida: false);
            Caixa("PernaCadeiraT", V(px, .46f, -.19f), V(.045f, .92f, .045f), madeiraEscura, p, solida: false);
        }
    }

    private Light ConstruirPendenteCivico(Vector3 pos, Material haste, Material globo, Color cor, float intensidade, float alcance)
    {
        Cilindro("HastePendente", pos + V(0f, .32f, 0f), V(.03f, .32f, .03f), haste);
        Cilindro("CanopyPendente", pos + V(0f, .54f, 0f), V(.14f, .03f, .14f), haste);
        Esfera("GloboPendente", pos, V(.32f, .24f, .32f), globo);
        return intensidade > 0f ? Luz("LuzPendente", pos - V(0f, .12f, 0f), cor, intensidade, alcance) : null;
    }

    private void ConstruirRelogioMunicipal(Vector3 pos, Material madeiraEscura, Material latao, Material mostrador)
    {
        var p = Pivo("RelogioMunicipal", world, pos);
        Caixa("CaixaRelogio", V(0f, 0f, 0f), V(.44f, .76f, .12f), madeiraEscura, p, solida: false);
        var mostradorGo = Cilindro("MostradorRelogio", V(0f, .18f, -.065f), V(.30f, .02f, .30f), mostrador, p);
        mostradorGo.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        var penduloPivo = Pivo("PenduloRelogioPivo", p, V(0f, .02f, -.06f));
        Caixa("HastePendulo", V(0f, -.15f, 0f), V(.02f, .28f, .01f), latao, penduloPivo, solida: false);
        Esfera("DiscoPendulo", V(0f, -.29f, 0f), V(.09f, .09f, .03f), latao, penduloPivo);

        float t = 0f;
        RegistrarAnimacao(dt =>
        {
            t += dt;
            penduloPivo.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 2.4f) * 14f);
        });
        SomLocal("Pendulo", pos, 8.2f, .56f);
    }
}

public sealed class Capitulo2PublicRecordsDoor3D : MonoBehaviour
{
    public Renderer portaRenderer;
    public Collider portaCollider;

    private void OnEnable()
    {
        GameState.OnEstadoAlterado += Sincronizar;
        Sincronizar();
    }

    private void OnDisable()
    {
        GameState.OnEstadoAlterado -= Sincronizar;
    }

    private void Start()
    {
        Sincronizar();
    }

    public void Sincronizar()
    {
        bool aberto = GameState.Tem("c2_prefeitura_atendimento") || Capitulo2Director.ExploracaoNoEditor;
        if (portaRenderer != null) portaRenderer.enabled = !aberto;
        if (portaCollider != null) portaCollider.enabled = !aberto;
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = !aberto;
        foreach (var c in GetComponentsInChildren<Collider>(true))
            c.enabled = !aberto;
    }
}
