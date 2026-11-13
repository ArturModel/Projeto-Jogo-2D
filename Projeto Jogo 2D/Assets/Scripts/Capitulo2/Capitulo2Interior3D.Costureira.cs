using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarCostureira()
    {
        const float xMin = -5.5f, xMax = 5.5f;
        const float zMin = -4.5f, zMax = 7.0f;
        const float altura = 3.1f;

        ConfigurarEntrada(zMin, V(0f, EyeHeight, -2.85f), "Foley/footstep_wood_000");

        var texTabuas = CriarTabuas(new Color(.44f, .25f, .14f), new Color(.36f, .19f, .10f), 64, 4);
        var pisoCedro = Superficie("PisoCedroCostureira", Color.white, texTabuas, new Vector2(5.5f, 5.75f), brilho: .38f);
        var madeiraEscura = Superficie("CedroEscuroCostureira", new Color(.26f, .14f, .08f), brilho: .28f);
        var madeiraMedia = Superficie("MadeiraMediaCostureira", new Color(.42f, .24f, .13f), brilho: .24f);
        var paredeOcre = Superficie("ParedeOcreCostureira", new Color(.50f, .40f, .27f), brilho: .08f);
        var paredeSalvia = Superficie("ParedeSalviaCostureira", new Color(.34f, .41f, .32f), brilho: .08f);
        var forro = Superficie("ForroCostureira", new Color(.36f, .31f, .25f), brilho: .06f);
        var metalEscuro = Superficie("FerroFundidoCostureira", new Color(.12f, .12f, .14f), brilho: .52f);
        var latao = Superficie("LataoCostureira", new Color(.76f, .60f, .28f), brilho: .72f);
        var bordô = Superficie("TecidoBordoCostureira", new Color(.48f, .11f, .16f), brilho: .10f);

        CostureiraEstrutura(xMin, xMax, zMin, zMax, altura, pisoCedro, forro, paredeOcre, paredeSalvia, madeiraEscura, madeiraMedia, bordô);

        ConstruirPorta(madeiraMedia, madeiraEscura, latao, 0f, zMin, 1.06f, 2.18f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, -4.53f), 180f, ExteriorInterior3D.Local.CostureiraRua);

        CostureiraEstar(madeiraEscura, madeiraMedia, paredeSalvia, bordô, latao);
        CostureiraMesaCorte(madeiraEscura, madeiraMedia, metalEscuro, latao, bordô);
        CostureiraArmarioRetrato(madeiraEscura, madeiraMedia, paredeSalvia, bordô, latao);

        Personagem("Celina", "Art/Characters/Npc_Celina", V(-2.2f, 0f, 3.8f), "celina");
    }

    private void CostureiraEstrutura(float xMin, float xMax, float zMin, float zMax, float altura,
        Material pisoCedro, Material forro, Material paredeOcre, Material paredeSalvia,
        Material madeiraEscura, Material madeiraMedia, Material tecidoCortina)
    {
        float largura = xMax - xMin;
        float profundidade = zMax - zMin;
        float centroZ = (zMin + zMax) * .5f;

        // Piso de tábuas de cedro polido e forro com vigas
        Caixa("PisoCostureira", V(0f, -.05f, centroZ), V(largura + .4f, .10f, profundidade + .4f), pisoCedro);
        Caixa("TetoCostureira", V(0f, altura + .05f, centroZ), V(largura + .4f, .10f, profundidade + .4f), forro);

        for (float z = -3.0f; z <= 5.5f; z += 2.8f)
            Caixa("VigaTetoCostureira", V(0f, altura - .06f, z), V(largura, .12f, .16f), madeiraEscura, solida: false);

        // Paredes fechadas (fundo, esquerda, direita)
        Caixa("ParedeFundoCostureira", V(0f, altura * .5f, zMax + .10f), V(largura + .4f, altura, .20f), paredeOcre);
        Caixa("ParedeEsqCostureira", V(xMin - .10f, altura * .5f, centroZ), V(.20f, altura, profundidade + .4f), paredeOcre);
        Caixa("ParedeDirCostureira", V(xMax + .10f, altura * .5f, centroZ), V(.20f, altura, profundidade + .4f), paredeOcre);

        // Parede frontal com vão da janela cortinada (esquerda) e vão da porta de entrada (centro)
        var vaoJanela = Rect.MinMaxRect(-3.7f, .95f, -2.1f, 2.25f);
        var vaoPorta = Rect.MinMaxRect(-.53f, 0f, .53f, 2.18f);
        ExteriorInterior3D.Parede(xMin, xMax, altura, new[] { vaoJanela, vaoPorta }, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeFrenteTrecho", V((a + b) * .5f, (y0 + y1) * .5f, zMin), V(w, h, .20f), paredeOcre);
        });

        // Lambril sálvia e rodapés de cedro
        Caixa("LambrilFundo", V(0f, .46f, zMax - .015f), V(largura, .92f, .03f), paredeSalvia, solida: false);
        Caixa("LambrilEsq", V(xMin + .015f, .46f, centroZ), V(.03f, .92f, profundidade), paredeSalvia, solida: false);
        Caixa("LambrilDir", V(xMax - .015f, .46f, centroZ), V(.03f, .92f, profundidade), paredeSalvia, solida: false);

        Caixa("RodapeFundo", V(0f, .07f, zMax - .025f), V(largura, .14f, .04f), madeiraEscura, solida: false);
        Caixa("FrisoFundo", V(0f, .93f, zMax - .025f), V(largura, .05f, .04f), madeiraEscura, solida: false);
        Caixa("RodapeEsq", V(xMin + .025f, .07f, centroZ), V(.04f, .14f, profundidade), madeiraEscura, solida: false);
        Caixa("FrisoEsq", V(xMin + .025f, .93f, centroZ), V(.04f, .05f, profundidade), madeiraEscura, solida: false);
        Caixa("RodapeDir", V(xMax - .025f, .07f, centroZ), V(.04f, .14f, profundidade), madeiraEscura, solida: false);
        Caixa("FrisoDir", V(xMax - .025f, .93f, centroZ), V(.04f, .05f, profundidade), madeiraEscura, solida: false);

        // Janela com cortinas mostrando a noite chuvosa lá fora
        float jx = (vaoJanela.xMin + vaoJanela.xMax) * .5f;
        float jy = (vaoJanela.yMin + vaoJanela.yMax) * .5f;
        float jw = vaoJanela.width;
        float jh = vaoJanela.height;

        MolduraVazada(V(jx, jy, zMin), jw, jh, madeiraEscura, .06f, .22f);
        Caixa("PeitorilJanela", V(jx, vaoJanela.yMin - .02f, zMin + .06f), V(jw + .22f, .05f, .28f), madeiraMedia, solida: false);
        Caixa("TravessaVertJanela", V(jx, jy, zMin), V(.04f, jh, .06f), madeiraEscura, solida: false);
        Caixa("TravessaHorizJanela", V(jx, jy + .08f, zMin), V(jw, .04f, .06f), madeiraEscura, solida: false);
        Caixa("VidroJanelaCostureira", V(jx, jy, zMin), V(jw - .02f, jh - .02f, .02f), VidroExterior(), solida: false);

        // Varão e cortinas laterais
        var pivoCortina = Pivo("CortinaJanelaCostureira", world, V(jx, vaoJanela.yMax + .14f, zMin + .14f));
        var varao = Cilindro("VaraoCortina", Vector3.zero, V(.03f, 1.05f, .03f), madeiraEscura, pivoCortina);
        varao.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        var folhaCortinaEsq = Caixa("CortinaEsq", V(-.86f, -.72f, 0f), V(.36f, 1.48f, .06f), tecidoCortina, pivoCortina, solida: false);
        var folhaCortinaDir = Caixa("CortinaDir", V(.86f, -.72f, 0f), V(.36f, 1.48f, .06f), tecidoCortina, pivoCortina, solida: false);
        Caixa("BandoCortina", V(0f, -.08f, .01f), V(2.04f, .16f, .07f), tecidoCortina, pivoCortina, solida: false);

        Luz("LuzLuaJanelaCostureira", V(jx, jy + .1f, zMin + .35f), new Color(.40f, .54f, .78f), .52f, 3.8f);

        float tempoBrisa = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoBrisa += dt;
            float brisa = Mathf.Sin(tempoBrisa * 1.15f) * 1.4f;
            folhaCortinaEsq.transform.localRotation = Quaternion.Euler(brisa, 0f, 0f);
            folhaCortinaDir.transform.localRotation = Quaternion.Euler(-brisa * .8f, 0f, 0f);
        });
    }

    private void CostureiraEstar(Material madeiraEscura, Material madeiraMedia, Material salvia, Material bordô, Material latao)
    {
        // Tapete tecido aconchegante na área de estar (direita-frente)
        var tapeteBase = Superficie("TapeteTerracota", new Color(.56f, .29f, .20f), brilho: .05f);
        var tapeteBorda = Superficie("TapeteCreme", new Color(.78f, .70f, .55f), brilho: .05f);
        Caixa("TapeteEstarBase", V(3.35f, .008f, -1.1f), V(2.9f, .016f, 3.1f), tapeteBase, solida: false);
        Caixa("TapeteEstarBorda", V(3.35f, .010f, -1.1f), V(2.45f, .016f, 2.65f), tapeteBorda, solida: false);
        Caixa("TapeteEstarCentro", V(3.35f, .012f, -1.1f), V(2.05f, .016f, 2.25f), salvia, solida: false);

        // Sofá de tecido encostado à direita voltado para o centro (-X)
        var sofaTecido = Superficie("SofaTecidoCostureira", new Color(.54f, .47f, .36f), brilho: .06f);
        var pivoSofa = Pivo("SofaCostureira", world, V(4.45f, 0f, -1.1f));
        Caixa("SofaBase", V(0f, .22f, 0f), V(.86f, .22f, 2.05f), sofaTecido, pivoSofa);
        Caixa("SofaAssentoEsq", V(-.04f, .38f, -.48f), V(.74f, .14f, .92f), sofaTecido, pivoSofa, solida: false);
        Caixa("SofaAssentoDir", V(-.04f, .38f, .48f), V(.74f, .14f, .92f), sofaTecido, pivoSofa, solida: false);
        Caixa("SofaEncosto", V(.34f, .64f, 0f), V(.18f, .64f, 2.05f), sofaTecido, pivoSofa);
        Caixa("SofaBracoSul", V(0f, .46f, -.95f), V(.84f, .34f, .16f), sofaTecido, pivoSofa);
        Caixa("SofaBracoNorte", V(0f, .46f, .95f), V(.84f, .34f, .16f), sofaTecido, pivoSofa);
        Caixa("SofaAlmofada1", V(.16f, .56f, -.65f), V(.14f, .34f, .36f), bordô, pivoSofa, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, 10f, 16f);
        Caixa("SofaAlmofada2", V(.16f, .56f, .68f), V(.14f, .34f, .36f), salvia, pivoSofa, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, -8f, 14f);

        foreach (float lx in new[] { -.34f, .34f })
        foreach (float lz in new[] { -.92f, .92f })
            Cilindro("SofaPe", V(lx, .06f, lz), V(.06f, .06f, .06f), madeiraEscura, pivoSofa);

        // Mesinha de chá de madeira com chimarrão e chaleira
        var pivoMesa = Pivo("MesaChaCostureira", world, V(3.05f, 0f, -1.1f));
        Caixa("MesaChaTampo", V(0f, .46f, 0f), V(.68f, .04f, .96f), madeiraMedia, pivoMesa);
        Caixa("MesaChaSaia", V(0f, .40f, 0f), V(.60f, .07f, .86f), madeiraEscura, pivoMesa, solida: false);
        foreach (float px in new[] { -.28f, .28f })
        foreach (float pz in new[] { -.40f, .40f })
            Caixa("MesaChaPe", V(px, .20f, pz), V(.05f, .40f, .05f), madeiraEscura, pivoMesa);

        Caixa("ToalhinhaMesaCha", V(0f, .484f, 0f), V(.36f, .006f, .48f), tapeteBorda, pivoMesa, solida: false);
        Arte("ChimarraoMesaCha", "Art/Props/Chimarrao", V(-.06f, .49f, .08f), .28f, -32f, pai: pivoMesa);

        // Chaleira esmaltada 3D ao lado do chimarrão
        var esmalte = Superficie("ChaleiraEsmalte", new Color(.22f, .32f, .34f), brilho: .48f);
        Cilindro("ChaleiraCorpo", V(.12f, .55f, -.18f), V(.16f, .07f, .16f), esmalte, pivoMesa);
        Cilindro("ChaleiraTampa", V(.12f, .63f, -.18f), V(.10f, .015f, .10f), latao, pivoMesa);
        Esfera("ChaleiraPuxador", V(.12f, .655f, -.18f), V(.03f, .03f, .03f), madeiraEscura, pivoMesa);

        // Luminária pendente âmbar baixa e oscilando com a tempestade na área de estar
        var cupulaAmbar = Superficie("CupulaAmbar", new Color(.80f, .54f, .24f), brilho: .15f);
        var corBulboEstar = new Color(1f, .84f, .54f);
        var bulboQuente = BulboInstancia("BulboPendenteEstar", corBulboEstar);
        var pivoPend = Pivo("PendenteEstar", world, V(3.25f, 3.1f, -1.1f));
        Cilindro("PendenteEstarFio", V(0f, -.48f, 0f), V(.016f, .48f, .016f), madeiraEscura, pivoPend);
        Cilindro("PendenteEstarSoquete", V(0f, -.94f, 0f), V(.06f, .05f, .06f), latao, pivoPend);
        Cilindro("PendenteEstarCupula", V(0f, -1.04f, 0f), V(.34f, .11f, .34f), cupulaAmbar, pivoPend);
        Esfera("PendenteEstarBulbo", V(0f, -1.08f, 0f), V(.08f, .08f, .08f), bulboQuente, pivoPend);

        var luzEstar = Luz("LuzPendenteEstar", V(3.25f, 1.94f, -1.1f), new Color(1f, .72f, .38f), .92f, 5.2f);
        LuzOscilante(luzEstar, bulboQuente, .92f, corBulboEstar, new Color(.20f, .14f, .09f),
            intensidadeMinFator: .18f, somFaisca: true, ganhoSom: .16f, semente: 6.2f);
    }

    private void CostureiraMesaCorte(Material madeiraEscura, Material madeiraMedia,
        Material ferroFundido, Material latao, Material tecidoBordo)
    {
        // Mesa de corte no centro-esquerda (x = -0.8f, z = 3.0f)
        var pivoMesa = Pivo("MesaCorteCostureira", world, V(-.8f, 0f, 3.0f));
        Caixa("MesaCorteTampo", V(0f, .80f, 0f), V(2.25f, .05f, 1.08f), madeiraMedia, pivoMesa);
        Caixa("MesaCorteSaia", V(0f, .72f, 0f), V(2.10f, .10f, .96f), madeiraEscura, pivoMesa, solida: false);
        Caixa("MesaCortePrateleira", V(0f, .22f, 0f), V(1.96f, .035f, .84f), madeiraMedia, pivoMesa);

        foreach (float px in new[] { -.98f, .98f })
        foreach (float pz in new[] { -.44f, .44f })
            Caixa("MesaCortePe", V(px, .40f, pz), V(.07f, .78f, .07f), madeiraEscura, pivoMesa);

        // Rolos de tecido guardados na prateleira inferior
        var linho = Superficie("LinhoNatural", new Color(.84f, .79f, .68f), brilho: .06f);
        var azulMarinho = Superficie("TecidoMarinho", new Color(.14f, .20f, .34f), brilho: .08f);
        var rolo1 = Cilindro("RoloPrateleira1", V(-.45f, .29f, 0f), V(.11f, .36f, .11f), linho, pivoMesa);
        rolo1.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        var rolo2 = Cilindro("RoloPrateleira2", V(-.18f, .29f, 0f), V(.11f, .36f, .11f), azulMarinho, pivoMesa);
        rolo2.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        // Máquina de costura vintage de ferro fundido (lado esquerdo da mesa)
        var pivoMaq = Pivo("MaquinaCosturaVintage", pivoMesa, V(-.64f, .825f, .12f));
        Caixa("MaqBase", V(0f, .02f, 0f), V(.44f, .04f, .22f), ferroFundido, pivoMaq, solida: false);
        Caixa("MaqFrisoBase", V(0f, .042f, -.095f), V(.40f, .006f, .012f), latao, pivoMaq, solida: false);
        Caixa("MaqColuna", V(.15f, .14f, 0f), V(.09f, .22f, .10f), ferroFundido, pivoMaq, solida: false);
        Caixa("MaqBraco", V(-.01f, .22f, 0f), V(.34f, .08f, .09f), ferroFundido, pivoMaq, solida: false);
        Caixa("MaqCabecote", V(-.16f, .17f, 0f), V(.07f, .12f, .08f), ferroFundido, pivoMaq, solida: false);
        Cilindro("MaqAgulha", V(-.16f, .085f, 0f), V(.008f, .045f, .008f), latao, pivoMaq);

        var volante = Cilindro("MaqVolante", V(.21f, .21f, 0f), V(.13f, .018f, .13f), ferroFundido, pivoMaq);
        volante.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        var aroVolante = Cilindro("MaqAroVolante", V(.215f, .21f, 0f), V(.10f, .014f, .10f), latao, pivoMaq);
        aroVolante.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        // Carretel de linha no topo do braço
        Cilindro("MaqPinoCarretel", V(.08f, .28f, 0f), V(.008f, .03f, .008f), latao, pivoMaq);
        Cilindro("MaqCarretel", V(.08f, .285f, 0f), V(.032f, .022f, .032f), tecidoBordo, pivoMaq);

        // Tecido bordô estendido e caindo pela borda da mesa
        Caixa("TecidoBordoMesa", V(.24f, .832f, -.02f), V(.86f, .014f, .64f), tecidoBordo, pivoMesa, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, -6f, 0f);
        Caixa("TecidoBordoCaido", V(.30f, .66f, -.545f), V(.68f, .34f, .016f), tecidoBordo, pivoMesa, solida: false)
            .transform.localRotation = Quaternion.Euler(-4f, -3f, 0f);

        // Fita métrica e tesoura de alfaiate sobre a mesa
        var fitaAmarela = Superficie("FitaMetrica", new Color(.92f, .80f, .34f), brilho: .12f);
        var aco = Superficie("AcoTesoura", new Color(.72f, .75f, .78f), brilho: .78f);
        Caixa("FitaMetrica1", V(-.18f, .832f, -.26f), V(.46f, .005f, .025f), fitaAmarela, pivoMesa, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, 22f, 0f);
        Caixa("FitaMetrica2", V(-.04f, .832f, -.36f), V(.26f, .005f, .025f), fitaAmarela, pivoMesa, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, -34f, 0f);

        var pivoTesoura = Pivo("TesouraCostureira", pivoMesa, V(.72f, .832f, -.18f));
        pivoTesoura.localRotation = Quaternion.Euler(0f, 28f, 0f);
        Caixa("TesouraLamina1", V(-.05f, 0f, .006f), V(.16f, .005f, .016f), aco, pivoTesoura, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, 8f, 0f);
        Caixa("TesouraLamina2", V(-.05f, .002f, -.006f), V(.16f, .005f, .016f), aco, pivoTesoura, solida: false)
            .transform.localRotation = Quaternion.Euler(0f, -8f, 0f);
        Caixa("TesouraCabo1", V(.06f, 0f, .018f), V(.05f, .007f, .032f), ferroFundido, pivoTesoura, solida: false);
        Caixa("TesouraCabo2", V(.06f, 0f, -.018f), V(.05f, .007f, .032f), ferroFundido, pivoTesoura, solida: false);

        // Encomenda da faixa (interativo obrigatório)
        var papel = Superficie("PapelEncomenda", new Color(.93f, .88f, .76f), brilho: .06f);
        var tinta = Superficie("TintaEncomenda", new Color(.22f, .18f, .15f), brilho: .05f);
        var pivoEncomenda = Pivo("EncomendaFaixa", pivoMesa, V(.46f, .838f, -.28f));
        pivoEncomenda.localRotation = Quaternion.Euler(0f, -14f, 0f);
        Caixa("EncomendaFolha", Vector3.zero, V(.26f, .008f, .34f), papel, pivoEncomenda, solida: false,
            interactionId: "encomenda_faixa", customPrompt: "Ler encomenda da faixa");
        Caixa("EncomendaLinha1", V(0f, .006f, .07f), V(.18f, .002f, .015f), tinta, pivoEncomenda, solida: false);
        Caixa("EncomendaLinha2", V(-.01f, .006f, .02f), V(.16f, .002f, .012f), tinta, pivoEncomenda, solida: false);
        Caixa("EncomendaSelo", V(.06f, .007f, -.09f), V(.04f, .004f, .04f), tecidoBordo, pivoEncomenda, solida: false);

        // Luminária pendente sobre a mesa de corte
        var cupulaAtelier = Superficie("CupulaAtelier", new Color(.25f, .36f, .27f), brilho: .35f);
        var bulboAtelier = Superficie("BulboAtelier", new Color(1f, .92f, .72f), unlit: true);
        var pivoPend = Pivo("PendenteMesaCorte", world, V(-.8f, 3.1f, 3.0f));
        Cilindro("PendenteCorteFio", V(0f, -.45f, 0f), V(.016f, .45f, .016f), madeiraEscura, pivoPend);
        Cilindro("PendenteCorteCupula", V(0f, -.94f, 0f), V(.36f, .09f, .36f), cupulaAtelier, pivoPend);
        Esfera("PendenteCorteBulbo", V(0f, -.98f, 0f), V(.08f, .08f, .08f), bulboAtelier, pivoPend);

        var luzMesa = Luz("LuzMesaCorte", V(-.8f, 2.02f, 3.0f), new Color(1f, .80f, .50f), .95f, 5.2f);
        SpotLuz("SpotMesaCorte", V(-.8f, 2.04f, 3.0f), V(90f, 0f, 0f), new Color(1f, .84f, .56f), 1.45f, 3.6f, 58f, 24f);
        float tempoMesa = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoMesa += dt;
            luzMesa.intensity = .95f + Mathf.Sin(tempoMesa * 1.9f + 1.1f) * .04f;
        });
        SomLocal("MotorVentilador", V(-.8f, .90f, 3.0f), 5.2f, .14f);
    }

    private void CostureiraArmarioRetrato(Material madeiraEscura, Material madeiraMedia,
        Material salvia, Material bordô, Material latao)
    {
        // Armário de tecidos na parede esquerda/fundo (x = -4.85f, z = 4.9f)
        var pivoArm = Pivo("ArmarioTecidos", world, V(-4.85f, 0f, 4.9f));
        Caixa("ArmarioFundo", V(-.40f, 1.12f, 0f), V(.05f, 2.24f, 1.68f), madeiraEscura, pivoArm);
        Caixa("ArmarioLatNorte", V(0f, 1.12f, .82f), V(.82f, 2.24f, .05f), madeiraEscura, pivoArm);
        Caixa("ArmarioLatSul", V(0f, 1.12f, -.82f), V(.82f, 2.24f, .05f), madeiraEscura, pivoArm);
        Caixa("ArmarioTopo", V(0f, 2.22f, 0f), V(.86f, .06f, 1.72f), madeiraEscura, pivoArm);
        Caixa("ArmarioBase", V(0f, .08f, 0f), V(.84f, .16f, 1.68f), madeiraEscura, pivoArm);

        float[] prateleirasY = { .56f, 1.08f, 1.60f };
        foreach (float py in prateleirasY)
            Caixa("ArmarioPrateleira", V(0f, py, 0f), V(.76f, .035f, 1.60f), madeiraMedia, pivoArm, solida: false);

        // Peças de tecido dobradas em cores variadas
        var mostarda = Superficie("TecidoMostarda", new Color(.78f, .58f, .18f), brilho: .08f);
        var creme = Superficie("TecidoCreme", new Color(.87f, .82f, .70f), brilho: .06f);
        var marinho = Superficie("TecidoAzulMarinho", new Color(.15f, .22f, .38f), brilho: .08f);
        var rosaAntigo = Superficie("TecidoRosaAntigo", new Color(.68f, .38f, .40f), brilho: .08f);

        Material[] coresTecido = { bordô, salvia, mostarda, creme, marinho, rosaAntigo };
        for (int p = 0; p < prateleirasY.Length; p++)
        {
            float baseY = prateleirasY[p] + .02f;
            for (int c = 0; c < 4; c++)
            {
                float zOff = -.54f + c * .36f;
                var mat = coresTecido[(p * 4 + c) % coresTecido.Length];
                Caixa("PecaTecidoA", V(.04f, baseY + .05f, zOff), V(.44f, .09f, .28f), mat, pivoArm, solida: false);
                var mat2 = coresTecido[(p * 4 + c + 2) % coresTecido.Length];
                Caixa("PecaTecidoB", V(.02f, baseY + .13f, zOff), V(.40f, .07f, .25f), mat2, pivoArm, solida: false);
            }
        }

        // Cesto de retalhos no chão perto do armário e da mesa
        var vime = Superficie("CestoVime", new Color(.56f, .40f, .23f), brilho: .06f);
        var pivoCesto = Pivo("CestoRetalhos", world, V(-3.8f, 0f, 3.6f));
        Cilindro("CestoCorpo", V(0f, .18f, 0f), V(.42f, .18f, .42f), vime, pivoCesto, solida: true);
        Caixa("Retalho1", V(-.05f, .36f, .03f), V(.18f, .05f, .14f), bordô, pivoCesto, solida: false)
            .transform.localRotation = Quaternion.Euler(14f, 25f, -10f);
        Caixa("Retalho2", V(.06f, .37f, -.04f), V(.16f, .05f, .15f), mostarda, pivoCesto, solida: false)
            .transform.localRotation = Quaternion.Euler(-12f, -18f, 16f);
        Caixa("Retalho3", V(0f, .39f, .04f), V(.14f, .04f, .12f), salvia, pivoCesto, solida: false)
            .transform.localRotation = Quaternion.Euler(8f, 45f, 12f);

        // Retrato de família emoldurado na parede de fundo
        var fundoRetrato = Superficie("FundoRetratoSepia", new Color(.64f, .54f, .40f), brilho: .08f);
        var silhuetaEscura = Superficie("SilhuetaRetrato", new Color(.22f, .17f, .14f), brilho: .06f);
        var pivoRetrato = Pivo("RetratoFamilia", world, V(.4f, 1.78f, 6.94f));
        Caixa("RetratoTela", Vector3.zero, V(.72f, .56f, .02f), fundoRetrato, pivoRetrato, solida: false);
        MolduraVazada(V(.4f, 1.78f, 6.92f), .72f, .56f, madeiraEscura, .055f, .06f);
        Caixa("RetratoFigura1", V(-.14f, -.03f, -.015f), V(.14f, .28f, .01f), silhuetaEscura, pivoRetrato, solida: false);
        Esfera("RetratoCabeca1", V(-.14f, .14f, -.015f), V(.09f, .10f, .01f), silhuetaEscura, pivoRetrato);
        Caixa("RetratoFigura2", V(.08f, -.01f, -.015f), V(.16f, .32f, .01f), silhuetaEscura, pivoRetrato, solida: false);
        Esfera("RetratoCabeca2", V(.08f, .18f, -.015f), V(.09f, .10f, .01f), silhuetaEscura, pivoRetrato);

        // Arandelas quentes na parede de fundo iluminando o retrato e o atelier
        var vidroArandela = Superficie("VidroArandela", new Color(1f, .82f, .52f), unlit: true);
        foreach (float ax in new[] { -1.2f, 2.0f })
        {
            Caixa("ArandelaBase", V(ax, 1.92f, 6.93f), V(.08f, .16f, .04f), latao, solida: false);
            Esfera("ArandelaGlobo", V(ax, 1.96f, 6.84f), V(.11f, .13f, .11f), vidroArandela);
            Luz("LuzArandelaFundo", V(ax, 1.96f, 6.65f), new Color(1f, .72f, .40f), .56f, 4.2f);
        }
    }
}
