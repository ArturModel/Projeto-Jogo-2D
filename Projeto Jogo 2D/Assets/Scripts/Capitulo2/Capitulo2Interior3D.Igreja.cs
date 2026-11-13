using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarIgreja()
    {
        ConfigurarEntrada(-4.5f, V(0f, EyeHeight, -2.85f), "Foley/footstep_concrete_000");

        // Materiais principais (calibrados para espaço Linear)
        var parede = Superficie("RebocoIgreja", new Color(.44f, .40f, .35f), brilho: .12f);
        var pedra = Superficie("PedraIgreja", new Color(.42f, .40f, .37f), brilho: .18f);
        var pedraClara = Superficie("PedraClaraIgreja", new Color(.56f, .53f, .48f), brilho: .22f);
        var madeiraEscura = Superficie("MadeiraEscuraIgreja", new Color(.35f, .23f, .15f),
            CriarTabuas(new Color(.38f, .25f, .16f), new Color(.29f, .18f, .11f)),
            new Vector2(2f, 2f), .28f);
        var madeiraBanco = Superficie("MadeiraBancoIgreja", new Color(.44f, .29f, .18f),
            CriarTabuas(new Color(.48f, .32f, .20f), new Color(.38f, .24f, .15f)),
            new Vector2(3f, 1f), .36f);
        var madeiraPorta = Superficie("MadeiraPortaIgreja", new Color(.38f, .24f, .15f),
            CriarTabuas(new Color(.42f, .27f, .17f), new Color(.32f, .20f, .12f)),
            new Vector2(1.5f, 2.5f), .30f);
        var pisoMat = Superficie("CeramicaIgreja", Color.white,
            CriarCeramica(new Color(.64f, .60f, .53f), new Color(.44f, .41f, .36f), 64, 4),
            new Vector2(13f, 18f), .28f);
        var tapete = Superficie("TapeteIgreja", new Color(.52f, .12f, .14f), brilho: .06f);
        var ouroTapete = Superficie("BordaTapeteIgreja", new Color(.74f, .58f, .24f), brilho: .35f);
        var latao = Superficie("LataoIgreja", new Color(.84f, .66f, .28f), brilho: .74f, metalico: .85f);
        var ferro = Superficie("FerroIgreja", new Color(.22f, .22f, .25f), brilho: .42f, metalico: .72f);
        var linho = Superficie("LinhoAltar", new Color(.88f, .86f, .80f), brilho: .06f);
        var cera = Superficie("CeraVela", new Color(.90f, .85f, .72f), brilho: .18f);
        var chamaMat = Superficie("ChamaVela", new Color(1f, .76f, .30f), unlit: true, hdr: 3.6f);
        var bulboMat = Superficie("BulboLustre", new Color(1f, .86f, .56f), unlit: true, hdr: 3.4f);
        var difusorLustreMat = Superficie("DifusorLustre", new Color(1f, .78f, .44f), unlit: true, hdr: 2.1f);
        var papel = Superficie("PapelMural", new Color(.84f, .80f, .70f), brilho: .04f);

        // ====================================================================
        // 1. PISO, PASSADEIRA E PAREDES ENCLAUSURADAS (Altura 9.6m -> Cume 11.6m)
        // ====================================================================
        Caixa("PisoNave", V(0f, -.05f, 4.5f), V(13.4f, .10f, 18.4f), pisoMat);

        // Passadeira carmesim com frisos dourados do corredor central até o presbitério
        Caixa("PassadeiraCentral", V(0f, .006f, 2.85f), V(1.88f, .012f, 13.5f), tapete, solida: false);
        Caixa("FrisoPassadeiraEsq", V(-.91f, .008f, 2.85f), V(.08f, .013f, 13.5f), ouroTapete, solida: false);
        Caixa("FrisoPassadeiraDir", V(.91f, .008f, 2.85f), V(.08f, .013f, 13.5f), ouroTapete, solida: false);

        // Paredes laterais (x = ±6.5f, altura 9.6m)
        Caixa("ParedeEsq", V(-6.58f, 4.80f, 4.5f), V(.18f, 9.60f, 18.2f), parede);
        Caixa("ParedeDir", V(6.58f, 4.80f, 4.5f), V(.18f, 9.60f, 18.2f), parede);
        // Lambri de madeira escura nas laterais
        Caixa("LambriEsq", V(-6.45f, .80f, 4.5f), V(.08f, 1.60f, 18.0f), madeiraEscura, solida: false);
        Caixa("LambriDir", V(6.45f, .80f, 4.5f), V(.08f, 1.60f, 18.0f), madeiraEscura, solida: false);

        // Parede do fundo (presbitério, z = 13.5f)
        Caixa("ParedeFundo", V(0f, 4.80f, 13.58f), V(13.4f, 9.60f, .18f), parede);
        Caixa("LambriFundo", V(0f, .90f, 13.45f), V(13.0f, 1.80f, .08f), madeiraEscura, solida: false);

        // Parede da entrada (z = -4.5f) com vão da porta central (largura 1.24m, altura 2.48m)
        ParedeComJanela(-6.5f, 6.5f, -4.56f, 9.60f, .18f, parede, new Rect(-.62f, 0f, 1.24f, 2.48f));
        ConstruirPorta(madeiraPorta, madeiraEscura, latao, 0f, -4.5f, 1.24f, 2.48f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, -4.53f), 180f, ExteriorInterior3D.Local.IgrejaPraca);

        // Empenas (fechamento triangular acima de 9.6m até o cume em 11.6m)
        for (int i = 0; i < 6; i++)
        {
            float t = (i + .5f) / 6f;
            float w = Mathf.Lerp(12.8f, 2.0f, t);
            float y = 9.60f + t * 2.05f;
            Caixa("EmpenaEntrada", V(0f, y, -4.56f), V(w, .38f, .18f), parede, solida: false);
            Caixa("EmpenaFundo", V(0f, y, 13.58f), V(w, .38f, .18f), parede, solida: false);
        }

        // ====================================================================
        // 2. TETO ABOBADADO EM MADEIRA, TESOURAS E PILASTRAS VERTICAIS
        // ====================================================================
        float anguloTelhado = Mathf.Atan2(2.20f, 6.50f) * Mathf.Rad2Deg;
        var aguaEsq = Caixa("TetoEsq", V(-3.30f, 10.50f, 4.5f), V(6.95f, .16f, 18.4f), madeiraEscura, solida: false);
        aguaEsq.transform.localRotation = Quaternion.Euler(0f, 0f, anguloTelhado);
        var aguaDir = Caixa("TetoDir", V(3.30f, 10.50f, 4.5f), V(6.95f, .16f, 18.4f), madeiraEscura, solida: false);
        aguaDir.transform.localRotation = Quaternion.Euler(0f, 0f, -anguloTelhado);
        Caixa("Cumeeira", V(0f, 11.45f, 4.5f), V(.32f, .34f, 18.2f), madeiraEscura, solida: false);

        float[] zPilares = { -2.0f, 1.5f, 5.0f, 8.5f, 12.0f };
        foreach (float zp in zPilares)
        {
            // Pilastras altas nas duas paredes laterais
            foreach (int lado in new[] { -1, 1 })
            {
                float xp = lado * 6.26f;
                Caixa("BasePilastra", V(xp, .30f, zp), V(.52f, .60f, .52f), pedra);
                Caixa("FustePilastra", V(xp, 4.75f, zp), V(.38f, 8.30f, .38f), pedraClara);
                Caixa("CapitelPilastra", V(xp, 9.05f, zp), V(.54f, .36f, .54f), pedra);
                Caixa("MisulaTesoura", V(lado * 5.95f, 8.55f, zp), V(.44f, .55f, .24f), madeiraEscura, solida: false);
            }

            // Tesoura de madeira atravessando a nave bem alto (y = 8.95m até 11.45m)
            Caixa("LinhaTesoura", V(0f, 8.95f, zp), V(12.6f, .28f, .24f), madeiraEscura, solida: false);
            Caixa("PenduralTesoura", V(0f, 10.22f, zp), V(.24f, 2.35f, .22f), madeiraEscura, solida: false);

            var escoraEsq = Caixa("EscoraEsq", V(-1.65f, 9.85f, zp), V(3.45f, .18f, .18f), madeiraEscura, solida: false);
            escoraEsq.transform.localRotation = Quaternion.Euler(0f, 0f, 27f);
            var escoraDir = Caixa("EscoraDir", V(1.65f, 9.85f, zp), V(3.45f, .18f, .18f), madeiraEscura, solida: false);
            escoraDir.transform.localRotation = Quaternion.Euler(0f, 0f, -27f);
        }

        // ====================================================================
        // 3. CORO ALTO E ÓRGÃO DE TUBOS SOBRE A ENTRADA
        // ====================================================================
        Caixa("PisoCoroAlto", V(0f, 3.80f, -3.15f), V(12.8f, .22f, 2.70f), madeiraEscura, solida: false);
        Caixa("VigaCoroAlto", V(0f, 3.58f, -1.85f), V(12.8f, .30f, .26f), madeiraEscura, solida: false);

        // Colunas de sustentação do coro (fora da linha da porta e dos corredores)
        foreach (float xc in new[] { -2.40f, 2.40f })
        {
            Caixa("BaseColunaCoro", V(xc, .22f, -1.90f), V(.44f, .44f, .44f), pedra);
            Cilindro("ColunaCoro", V(xc, 2.00f, -1.90f), V(.30f, 1.65f, .30f), madeiraEscura, solida: true);
            Caixa("CapitelCoro", V(xc, 3.52f, -1.90f), V(.46f, .28f, .46f), madeiraEscura, solida: false);
        }

        // Balaustrada do coro alto
        Caixa("CorrimaoCoro", V(0f, 4.76f, -1.85f), V(12.6f, .09f, .14f), madeiraBanco, solida: false);
        Caixa("RodapeCoro", V(0f, 3.96f, -1.85f), V(12.6f, .12f, .12f), madeiraEscura, solida: false);
        for (float xb = -5.8f; xb <= 5.81f; xb += .48f)
            Caixa("BalaustreCoro", V(xb, 4.36f, -1.85f), V(.07f, .72f, .07f), madeiraBanco, solida: false);

        // Órgão de tubos monumental no coro alto (y = 3.9m até 9.5m)
        Caixa("CaixaOrgaoBase", V(0f, 4.95f, -3.75f), V(3.80f, 2.10f, .88f), madeiraEscura, solida: false);
        Caixa("MolduraOrgaoSup", V(0f, 6.08f, -3.75f), V(4.05f, .20f, .98f), madeiraBanco, solida: false);
        Caixa("TecladoOrgao", V(0f, 4.55f, -3.22f), V(1.30f, .24f, .32f), madeiraBanco, solida: false);

        for (int i = -7; i <= 7; i++)
        {
            float dist = Mathf.Abs(i);
            float alturaTubo = 3.45f - dist * .22f + (i % 2 == 0 ? .18f : 0f);
            float raioTubo = .13f - dist * .005f;
            float baseY = 6.18f;
            Cilindro("TuboOrgao", V(i * .23f, baseY + alturaTubo * .5f, -3.55f),
                V(raioTubo, alturaTubo * .5f, raioTubo), latao, solida: false);
        }
        SpotLuz("SpotOrgao", V(0f, 7.65f, -1.55f), V(40f, 180f, 0f),
            new Color(1f, .74f, .42f), 16.0f, 9.8f, 74f, 26f, shadows: LightShadows.Soft, shadowTier: 0);

        // ====================================================================
        // 4. VITRAIS ALTOS NAS LATERAIS + FEIXES DE LUZ COLORIDA
        // ====================================================================
        var coresVitral = new[]
        {
            new Color(.18f, .38f, .86f), // Cobalto
            new Color(.90f, .58f, .16f), // Âmbar
            new Color(.76f, .16f, .22f), // Rubi
            new Color(.16f, .64f, .38f)  // Esmeralda
        };
        float[] zVitrais = { -.25f, 3.25f, 6.75f, 10.25f };

        for (int idx = 0; idx < zVitrais.Length; idx++)
        {
            float zv = zVitrais[idx];
            Color corPrincipal = coresVitral[idx % coresVitral.Length];
            Color corSecundaria = coresVitral[(idx + 1) % coresVitral.Length];
            var vidA = Superficie("VitralA_" + idx, corPrincipal * .64f, unlit: true, hdr: 1.42f);
            var vidB = Superficie("VitralB_" + idx, corSecundaria * .64f, unlit: true, hdr: 1.42f);

            foreach (int lado in new[] { -1, 1 })
            {
                float xParede = lado * 6.45f;
                float xVidro = lado * 6.42f;

                // Moldura externa e arco escalonado
                Caixa("VitralMolduraEsq", V(xParede, 4.55f, zv - .64f), V(.12f, 3.90f, .12f), pedra, solida: false);
                Caixa("VitralMolduraDir", V(xParede, 4.55f, zv + .64f), V(.12f, 3.90f, .12f), pedra, solida: false);
                Caixa("VitralPeitoril", V(xParede, 2.55f, zv), V(.16f, .16f, 1.46f), pedra, solida: false);
                Caixa("VitralArco1", V(xParede, 6.56f, zv), V(.12f, .24f, 1.16f), pedra, solida: false);
                Caixa("VitralArco2", V(xParede, 6.78f, zv), V(.12f, .22f, .72f), pedra, solida: false);

                // Painéis coloridos do vitral (y = 2.65m até 6.55m)
                Caixa("VitralPainelInf", V(xVidro, 3.60f, zv), V(.04f, 1.85f, 1.14f), vidA, solida: false);
                Caixa("VitralPainelSup", V(xVidro, 5.55f, zv), V(.04f, 1.85f, 1.14f), vidB, solida: false);
                Caixa("VitralRosacea", V(xVidro, 6.55f, zv), V(.04f, .42f, .58f), vidA, solida: false);

                // Travessas de chumbo
                Caixa("VitralTravessaV", V(xVidro - lado * .02f, 4.55f, zv), V(.03f, 3.80f, .06f), ferro, solida: false);
                Caixa("VitralTravessaH1", V(xVidro - lado * .02f, 3.60f, zv), V(.03f, .06f, 1.16f), ferro, solida: false);
                Caixa("VitralTravessaH2", V(xVidro - lado * .02f, 4.58f, zv), V(.03f, .06f, 1.16f), ferro, solida: false);
                Caixa("VitralTravessaH3", V(xVidro - lado * .02f, 5.56f, zv), V(.03f, .06f, 1.16f), ferro, solida: false);

                // Feixe de luz colorida projetado sobre os bancos e o piso (luar incide pelo lado +X)
                float yawSpot = lado < 0 ? 90f : -90f;
                bool lancaSombraVitral = lado > 0;
                SpotLuz("SpotVitral_" + idx + "_" + lado,
                    V(lado * 6.00f, 5.05f, zv),
                    V(36f, yawSpot, 0f),
                    Color.Lerp(corPrincipal, Color.white, .22f),
                    lancaSombraVitral ? 15.0f : 4.2f,
                    lancaSombraVitral ? 13.5f : 9.5f,
                    50f, 18f,
                    shadows: lancaSombraVitral ? LightShadows.Soft : LightShadows.None, shadowTier: 0);
            }
        }

        // ====================================================================
        // 5. FILEIRAS DE BANCOS DE MADEIRA (COM GENUFLEXÓRIOS E HINÁRIOS)
        // ====================================================================
        float[] zBancos = { -.20f, 1.55f, 3.30f, 5.05f, 6.80f };
        var capaHinario = Superficie("CapaHinario", new Color(.16f, .18f, .28f), brilho: .10f);

        foreach (float zb in zBancos)
        {
            foreach (int lado in new[] { -1, 1 })
            {
                float xb = lado * 2.85f;
                var banco = Pivo("BancoIgreja", world, V(xb, 0f, zb));

                // Assento e encosto (colisor sólido no bloco principal do banco)
                Caixa("AssentoBanco", V(0f, .44f, 0f), V(2.58f, .07f, .46f), madeiraBanco, banco, solida: true);
                var encosto = Caixa("EncostoBanco", V(0f, .80f, -.20f), V(2.58f, .54f, .07f), madeiraBanco, banco, solida: false);
                encosto.transform.localRotation = Quaternion.Euler(-8f, 0f, 0f);

                // Laterais / braços do banco
                Caixa("LateralEsqBanco", V(-1.26f, .48f, -.03f), V(.08f, .96f, .54f), madeiraEscura, banco, solida: false);
                Caixa("LateralDirBanco", V(1.26f, .48f, -.03f), V(.08f, .96f, .54f), madeiraEscura, banco, solida: false);

                // Genuflexório (apoio de joelhos atrás do banco) e porta-hinários
                Caixa("Genuflexorio", V(0f, .14f, -.36f), V(2.36f, .07f, .18f), tapete, banco, solida: false);
                Caixa("SuporteHinario", V(0f, .64f, -.28f), V(2.34f, .05f, .10f), madeiraEscura, banco, solida: false);
                Caixa("HinarioA", V(-.55f, .69f, -.28f), V(.18f, .04f, .13f), capaHinario, banco, solida: false);
                Caixa("HinarioB", V(.62f, .69f, -.28f), V(.18f, .04f, .13f), capaHinario, banco, solida: false);
            }
        }

        // ====================================================================
        // 6. LUSTRES PENDENTES DE LATÃO NA NAVE CENTRAL
        // ====================================================================
        foreach (float zl in new[] { .80f, 4.50f, 8.20f })
        {
            var corHalo = new Color(1f, .74f, .36f);

            var lustre = Pivo("LustreNave", world, V(0f, 5.25f, zl));
            Cilindro("CorrenteLustre", V(0f, 3.10f, 0f), V(.03f, 3.10f, .03f), ferro, lustre, solida: false);
            Cilindro("HasteLustre", V(0f, .40f, 0f), V(.06f, .45f, .06f), latao, lustre, solida: false);
            Cilindro("CoroaLustre", V(0f, .02f, 0f), V(1.18f, .06f, 1.18f), latao, lustre, solida: false);
            Esfera("DifusorLustre", V(0f, -.03f, 0f), V(.40f, .14f, .40f), difusorLustreMat, lustre);
            HaloLuz("HaloDifusorLustre", V(0f, -.10f, 0f), corHalo, 1.75f, .38f, lustre, falloff: 3.8f, core: .30f);

            for (int c = 0; c < 6; c++)
            {
                float ang = c * Mathf.PI * 2f / 6f;
                float cx = Mathf.Cos(ang) * .54f;
                float cz = Mathf.Sin(ang) * .54f;
                Cilindro("BracoLustre", V(cx * .5f, .06f, cz * .5f), V(.025f, .04f, .025f), latao, lustre, solida: false);
                Cilindro("VelaLustre", V(cx, .15f, cz), V(.04f, .11f, .04f), cera, lustre, solida: false);
                Esfera("BulboLustre", V(cx, .29f, cz), V(.09f, .12f, .09f), bulboMat, lustre);
                HaloLuz("HaloBulboLustre_" + c, V(cx, .30f, cz), corHalo, .58f, .46f, lustre, falloff: 5.6f, core: .52f);
            }

            SpotLuz("SpotLustre", V(0f, -.18f, 0f), V(90f, 0f, 0f),
                new Color(1f, .76f, .44f), 22.0f, 11.0f, 122f, 48f, lustre,
                shadows: LightShadows.Soft, shadowTier: 0);
            Luz("LuzLustre", V(0f, .24f, 0f), new Color(1f, .74f, .40f), 6.2f, 6.8f, lustre);
        }

        // ====================================================================
        // 7. PRESBITÉRIO, ALTAR E CRUZ MONUMENTAL
        // ====================================================================
        // Degraus baixos não-sólidos para não travar a cápsula fixa em EyeHeight
        Caixa("DegrauPresbiterio1", V(0f, .06f, 9.80f), V(8.4f, .12f, .55f), pedraClara, solida: false);
        Caixa("DegrauPresbiterio2", V(0f, .12f, 11.80f), V(9.2f, .24f, 3.50f), pedra, solida: false);
        Caixa("PassadeiraPresbiterio", V(0f, .245f, 10.90f), V(1.88f, .012f, 2.20f), tapete, solida: false);

        // Altar-mor
        var altar = Pivo("AltarMor", world, V(0f, .24f, 11.85f));
        Caixa("MesaAltar", V(0f, .48f, 0f), V(2.45f, .96f, 1.02f), madeiraEscura, altar, solida: true);
        Caixa("TampoAltar", V(0f, .99f, 0f), V(2.60f, .08f, 1.14f), madeiraBanco, altar, solida: false);
        Caixa("ToalhaAltar", V(0f, 1.035f, 0f), V(2.58f, .02f, 1.12f), linho, altar, solida: false);
        Caixa("CaimentoToalhaFrente", V(0f, .82f, -.565f), V(2.48f, .42f, .02f), linho, altar, solida: false);
        Caixa("FrontalAltar", V(0f, .46f, -.52f), V(.92f, .64f, .03f), tapete, altar, solida: false);

        // Sacrário dourado e castiçais no altar
        Caixa("Sacrario", V(0f, 1.28f, .24f), V(.44f, .48f, .34f), latao, altar, solida: false);
        Caixa("PortaSacrario", V(0f, 1.28f, .06f), V(.26f, .32f, .03f), ouroTapete, altar, solida: false);
        Caixa("BibliaAltar", V(-.48f, 1.07f, -.18f), V(.34f, .06f, .25f), linho, altar, solida: false);

        foreach (float xCast in new[] { -.96f, -.66f, .66f, .96f })
        {
            Cilindro("CasticalAltar", V(xCast, 1.16f, -.05f), V(.06f, .13f, .06f), latao, altar, solida: false);
            Cilindro("VelaAltar", V(xCast, 1.38f, -.05f), V(.035f, .11f, .035f), cera, altar, solida: false);
            Esfera("ChamaAltar", V(xCast, 1.52f, -.05f), V(.05f, .09f, .05f), chamaMat, altar);
            HaloLuz("HaloChamaAltar", V(xCast, 1.53f, -.05f), new Color(1f, .68f, .24f), .38f, .46f, altar, falloff: 6.2f, core: .50f);
        }
        var luzAltar = Luz("LuzAltar", V(0f, 1.85f, -.25f), new Color(1f, .76f, .42f), 4.6f, 6.5f, altar);
        SpotLuz("SpotAltar", V(0f, 3.65f, -.95f), V(62f, 0f, 0f),
            new Color(1f, .80f, .50f), 16.0f, 8.8f, 78f, 30f, altar,
            shadows: LightShadows.Soft, shadowTier: 0);

        // Cruz Monumental atrás do altar (y = 2.2m até 7.8m)
        Caixa("CruzHasteVertical", V(0f, 5.00f, 13.28f), V(.34f, 5.60f, .22f), madeiraEscura, solida: false);
        Caixa("CruzBracoHorizontal", V(0f, 6.20f, 13.28f), V(3.20f, .34f, .22f), madeiraEscura, solida: false);
        Caixa("CruzFileteDouradoV", V(0f, 5.00f, 13.16f), V(.10f, 5.30f, .03f), latao, solida: false);
        Caixa("CruzFileteDouradoH", V(0f, 6.20f, 13.16f), V(2.96f, .10f, .03f), latao, solida: false);
        SpotLuz("SpotCruz", V(0f, .65f, 12.62f), V(-74f, 0f, 0f),
            new Color(1f, .78f, .46f), 19.0f, 12.0f, 52f, 18f,
            shadows: LightShadows.Soft, shadowTier: 0);

        // ====================================================================
        // 8. VELÁRIO VOTIVO ("velas_igreja") COM CHAMA OSCILANTE
        // ====================================================================
        var velario = Pivo("VelarioIgreja", world, V(-3.35f, 0f, 8.85f));
        Caixa("BaseVelario", V(0f, .06f, 0f), V(1.38f, .12f, .64f), ferro, velario, solida: true,
            interactionId: "velas_igreja");
        Caixa("ColunaVelarioEsq", V(-.56f, .54f, 0f), V(.07f, .96f, .07f), ferro, velario, solida: false);
        Caixa("ColunaVelarioDir", V(.56f, .54f, 0f), V(.07f, .96f, .07f), ferro, velario, solida: false);
        Caixa("BandejaVelario", V(0f, 1.02f, 0f), V(1.34f, .08f, .58f), ferro, velario, solida: true,
            interactionId: "velas_igreja");
        Caixa("DegrauVelarioSup", V(0f, 1.14f, .12f), V(1.24f, .14f, .26f), ferro, velario, solida: false);

        var haloVelarioMat = HaloMaterial("HaloVelarioMat", new Color(1f, .60f, .22f), .28f, 6.2f, .44f);
        var chamasVelario = new Transform[14];
        for (int i = 0; i < 14; i++)
        {
            int fila = i / 7;
            int col = i % 7;
            float vx = -.48f + col * .16f;
            float vz = fila == 0 ? -.12f : .12f;
            float baseY = fila == 0 ? 1.06f : 1.21f;
            float alt = .14f + ((i * 37) % 10) * .014f;

            Cilindro("VelaVotiva_" + i, V(vx, baseY + alt * .5f, vz), V(.032f, alt * .5f, .032f), cera, velario, solida: false);
            var chama = Esfera("ChamaVotiva_" + i, V(vx, baseY + alt + .035f, vz), V(.04f, .075f, .04f), chamaMat, velario);
            HaloLuz("HaloVelaVotiva_" + i, V(vx, baseY + alt + .04f, vz), new Color(1f, .60f, .22f), .26f, .28f,
                velario, falloff: 6.2f, core: .44f, matOverride: haloVelarioMat);
            chamasVelario[i] = chama.transform;
        }
        HaloLuz("HaloVelarioGeral", V(0f, 1.38f, 0f), new Color(1f, .56f, .20f), 1.55f, .15f, velario, falloff: 3.6f, core: .20f);

        var luzVelario = Luz("LuzVelario", V(0f, 1.52f, -.05f), new Color(1f, .62f, .24f), 5.2f, 6.4f, velario);
        AlvoFisico(velario, "AlvoVelario", V(0f, .85f, 0f), V(1.48f, 1.55f, .82f), "velas_igreja");

        float tempoVelas = 0f;
        RegistrarAnimacao(dt =>
        {
            tempoVelas += dt;
            float osc = Mathf.Sin(tempoVelas * 7.3f) * .14f + Mathf.Sin(tempoVelas * 13.7f) * .09f;
            luzVelario.intensity = 5.2f + osc * 3.2f;
            luzAltar.intensity = 4.6f + osc * 1.1f;
            haloVelarioMat.SetFloat("_Intensity", .28f + osc * .18f);
            for (int i = 0; i < chamasVelario.Length; i++)
            {
                if (chamasVelario[i] == null) continue;
                float s = 1f + Mathf.Sin(tempoVelas * 9.5f + i * 1.3f) * .18f;
                chamasVelario[i].localScale = new Vector3(.04f, .075f * s, .04f);
            }
        });

        // ====================================================================
        // 9. MURAL DA COMUNIDADE E FOTO ("foto_igreja")
        // ====================================================================
        var mural = Pivo("MuralIgreja", world, V(-6.38f, 1.62f, -1.55f));
        mural.localRotation = Quaternion.Euler(0f, -90f, 0f);
        Caixa("MolduraMural", V(0f, 0f, 0f), V(1.56f, 1.08f, .07f), madeiraEscura, mural, solida: false);
        Caixa("FundoMural", V(0f, 0f, -.03f), V(1.42f, .94f, .03f), tapete, mural, solida: false);
        Caixa("AvisoMural1", V(-.38f, .16f, -.05f), V(.36f, .44f, .015f), papel, mural, solida: false);
        Caixa("AvisoMural2", V(-.36f, -.22f, -.05f), V(.40f, .24f, .015f), papel, mural, solida: false);
        Caixa("MolduraFotoComunidade", V(.28f, .04f, -.05f), V(.54f, .42f, .03f), madeiraBanco, mural, solida: false);
        Caixa("FotoComunidade", V(.28f, .04f, -.068f), V(.44f, .32f, .01f), pedraClara, mural, solida: false);
        AlvoFisico(mural, "AlvoFotoIgreja", V(0f, 0f, -.15f), V(1.65f, 1.25f, .45f), "foto_igreja");
        Caixa("BaseArandelaMural", V(0f, .64f, -.03f), V(.26f, .05f, .06f), latao, mural, solida: false);
        Esfera("BulboArandelaMural", V(0f, .68f, -.08f), V(.08f, .06f, .08f), bulboMat, mural);
        HaloLuz("HaloMural", V(0f, .68f, -.10f), new Color(1f, .76f, .42f), .45f, .34f, mural, falloff: 5.6f, core: .45f);
        Luz("LuzMural", V(0f, .72f, -.45f), new Color(1f, .78f, .48f), 3.2f, 4.4f, mural);

        // ====================================================================
        // 10. PADRE BENTO E ILUMINAÇÃO DA ENTRADA / CORO
        // ====================================================================
        Personagem("Padre Bento", "Art/Characters/Npc_Padre", V(1.9f, 0f, 9.2f), "padre");
        Luz("LuzPadreBento", V(1.6f, 2.35f, 8.6f), new Color(1f, .80f, .52f), 2.8f, 4.8f);
        Cilindro("BaseLanternaEntrada", V(0f, 3.67f, -3.40f), V(.26f, .04f, .26f), ferro, solida: false);
        Esfera("GloboLanternaEntrada", V(0f, 3.57f, -3.40f), V(.22f, .16f, .22f), difusorLustreMat);
        HaloLuz("HaloLanternaEntrada", V(0f, 3.55f, -3.40f), new Color(1f, .76f, .42f), .85f, .40f, falloff: 4.8f, core: .44f);
        SpotLuz("SpotEntradaIgreja", V(0f, 3.44f, -3.40f), V(82f, 0f, 0f),
            new Color(1f, .76f, .46f), 11.0f, 7.2f, 126f, 48f,
            shadows: LightShadows.Soft, shadowTier: 0);
    }
}
