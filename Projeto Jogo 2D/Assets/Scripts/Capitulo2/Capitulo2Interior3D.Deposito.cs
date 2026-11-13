using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarDeposito()
    {
        ConfigurarEntrada(-4.5f, V(0f, EyeHeight, -2.85f), "Foley/footstep_concrete_000");

        var madeira = Resources.Load<Texture2D>("Art/Tiles/Madeira");
        var pedra = Resources.Load<Texture2D>("Art/Tiles/Pedra");

        var piso = Superficie("ConcretoDeposito", Color.white,
            CriarCeramica(new Color(.34f, .34f, .33f), new Color(.27f, .27f, .26f), 64, 2), new Vector2(8f, 9f), .18f);
        var parede = Superficie("RebocoDeposito", new Color(.44f, .42f, .37f), pedra, new Vector2(6f, 2f), .08f);
        var tijolo = Superficie("TijoloExposto", Color.white,
            CriarCeramica(new Color(.46f, .27f, .21f), new Color(.38f, .22f, .17f), 64, 4), new Vector2(4f, 3f), .07f);
        var concretoEscuro = Superficie("ConcretoRodape", new Color(.26f, .27f, .26f), pedra, new Vector2(5f, 1f), .12f);
        var teto = Superficie("ForroDeposito", new Color(.23f, .22f, .20f), madeira, new Vector2(6f, 6f), .06f);
        var aco = Superficie("AcoEstante", new Color(.24f, .27f, .29f), brilho: .36f);
        var acoEscuro = Superficie("AcoEscuro", new Color(.14f, .16f, .18f), brilho: .42f);
        var pinho = Superficie("PinhoEstrado", new Color(.56f, .42f, .27f),
            CriarTabuas(new Color(.56f, .42f, .27f), new Color(.47f, .35f, .22f)), new Vector2(2f, 2f), .12f);
        var madeiraEscura = Superficie("MadeiraMesa", new Color(.36f, .25f, .16f), madeira, new Vector2(2f, 2f), .16f);
        var papelao = Superficie("PapelaoCaixa", new Color(.62f, .49f, .33f), brilho: .05f);
        var papelaoEscuro = Superficie("PapelaoEscuro", new Color(.49f, .38f, .25f), brilho: .05f);
        var saca = Superficie("SacaEstofa", new Color(.70f, .64f, .50f), brilho: .04f);
        var azulMercadinho = Superficie("EsmalteAzulMercadinho", new Color(.27f, .40f, .56f), brilho: .32f);
        var azulMarco = Superficie("MarcoAzulMercadinho", new Color(.20f, .30f, .43f), brilho: .26f);
        var latao = Superficie("LataoDeposito", new Color(.58f, .43f, .20f), brilho: .62f);
        var esmalteVerde = Superficie("EsmalteLuminaria", new Color(.16f, .25f, .19f), brilho: .45f);
        var bulboQuente = Superficie("BulboDeposito", new Color(1f, .86f, .62f), unlit: true);
        var papel = Superficie("PapelRomaneio", new Color(.92f, .89f, .81f), brilho: .05f);
        var vermelho = Superficie("DetalheVermelho", new Color(.58f, .20f, .16f), brilho: .18f);
        var vidroGarrafa = Superficie("VidroGarrafa", new Color(.18f, .33f, .20f), brilho: .72f);

        // --- Envelope arquitetônico (x: -5.5..5.5, z: -4.5..7.5, altura: 3.3m) ---
        Caixa("PisoDeposito", V(0f, -.06f, 1.5f), V(11.1f, .12f, 12.1f), piso);
        Caixa("TetoDeposito", V(0f, 3.36f, 1.5f), V(11.1f, .12f, 12.1f), teto);

        // Paredes laterais e dos fundos
        Caixa("ParedeOeste", V(-5.58f, 1.65f, 1.5f), V(.16f, 3.3f, 12.2f), parede);
        Caixa("ParedeLeste", V(5.58f, 1.65f, 1.5f), V(.16f, 3.3f, 12.2f), parede);
        Caixa("ParedeFundo", V(0f, 1.65f, 7.58f), V(11.2f, 3.3f, .16f), parede);

        // Barra inferior de concreto e trechos de tijolo aparente
        Caixa("RodapeConcretoOeste", V(-5.48f, .52f, 1.5f), V(.04f, 1.04f, 12f), concretoEscuro, solida: false);
        Caixa("RodapeConcretoLeste", V(5.48f, .52f, 1.5f), V(.04f, 1.04f, 12f), concretoEscuro, solida: false);
        Caixa("RodapeConcretoFundo", V(0f, .52f, 7.48f), V(11f, 1.04f, .04f), concretoEscuro, solida: false);
        Caixa("TijoloExpostoOeste", V(-5.49f, 1.85f, .8f), V(.03f, 1.55f, 3.4f), tijolo, solida: false);
        Caixa("TijoloExpostoLeste", V(5.49f, 1.95f, 4.4f), V(.03f, 1.35f, 2.8f), tijolo, solida: false);
        Caixa("TijoloExpostoFundo", V(-3.1f, 1.90f, 7.49f), V(2.9f, 1.45f, .03f), tijolo, solida: false);

        // Pilares e vigas de aço/concreto no teto
        foreach (float z in new[] { -1.5f, 1.5f, 4.5f })
        {
            Caixa("VigaTeto", V(0f, 3.18f, z), V(11f, .24f, .22f), acoEscuro, solida: false);
            Caixa("PilarOeste", V(-5.42f, 1.65f, z), V(.18f, 3.3f, .22f), concretoEscuro);
            Caixa("PilarLeste", V(5.42f, 1.65f, z), V(.18f, 3.3f, .22f), concretoEscuro);
        }

        // Parede frontal (z = -4.5) com porta de entrada (x: -0.53..0.53) e duas janelas gradeadas
        ParedeComJanela(-5.5f, -.53f, -4.5f, 3.3f, .18f, parede, new Rect(-3.4f, 1.35f, 1.8f, 1.05f));
        ParedeComJanela(.53f, 5.5f, -4.5f, 3.3f, .18f, parede, new Rect(1.6f, 1.35f, 1.8f, 1.05f));
        Caixa("VergaEntrada", V(0f, 2.74f, -4.5f), V(1.06f, 1.12f, .18f), parede);
        Caixa("RodapeFrontalOeste", V(-3.02f, .52f, -4.40f), V(4.94f, 1.04f, .04f), concretoEscuro, solida: false);
        Caixa("RodapeFrontalLeste", V(3.02f, .52f, -4.40f), V(4.94f, 1.04f, .04f), concretoEscuro, solida: false);

        var vidro = VidroExterior();
        foreach (float xJan in new[] { -2.5f, 2.5f })
        {
            MolduraVazada(V(xJan, 1.875f, -4.44f), 1.8f, 1.05f, acoEscuro);
            var painel = Caixa("VidroJanelaDeposito", V(xJan, 1.875f, -4.46f), V(1.78f, 1.03f, .02f), vidro, solida: false);
            painel.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            for (int i = -2; i <= 2; i++)
                Caixa("GradeJanela", V(xJan + i * .34f, 1.875f, -4.41f), V(.025f, 1.05f, .025f), acoEscuro, solida: false);
        }

        // Porta principal de entrada (Centro Santa Rita) e exterior chuvoso
        ConstruirPorta(aco, acoEscuro, latao, 0f, -4.5f, 1.06f, 2.18f);
        ConstruirChuva(-5.2f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, -4.53f), 180f, ExteriorInterior3D.Local.DepositoPatio);

        // --- Porta de serviço dos fundos -> EntradaDaCidade (Mercadinho) ---
        ConstruirPortaMercadinhoFundos(azulMercadinho, azulMarco, latao, acoEscuro, bulboQuente);

        // --- Estantes industriais de aço ---
        MontarEstanteDeposito(V(-4.85f, 0f, -.8f), 0f, 3.0f, aco, pinho, papelao, papelaoEscuro, saca);
        MontarEstanteDeposito(V(-4.85f, 0f, 3.0f), 0f, 3.0f, aco, pinho, papelao, papelaoEscuro, saca);
        MontarEstanteDeposito(V(-2.25f, 0f, 5.65f), 90f, 2.5f, aco, pinho, papelao, papelaoEscuro, saca);
        MontarEstanteDeposito(V(4.85f, 0f, -.8f), 180f, 3.0f, aco, pinho, papelao, papelaoEscuro, saca);
        MontarEstanteDeposito(V(4.85f, 0f, 3.0f), 180f, 3.0f, aco, pinho, papelao, papelaoEscuro, saca);

        // --- Estrados de madeira, sacarias e engradados retornáveis ---
        MontarEstradoComCarga(V(-2.35f, 0f, .2f), pinho, papelao, papelaoEscuro, saca, true);
        MontarEstradoComCarga(V(-2.35f, 0f, 2.7f), pinho, papelao, papelaoEscuro, saca, false);
        MontarEngradadosRetornaveis(V(3.35f, 0f, -2.5f), pinho, vermelho, vidroGarrafa);
        Arte("EngradadosRetornaveis", "Art/RuaNorte/Bar/Detalhes/EngradadosRetornaveis", V(-3.55f, .45f, -3.55f), .90f, 15f);
        Arte("CargasMercado", "Art/Props/CargasMercado", V(-3.3f, .62f, 5.95f), 1.24f, -12f);

        // --- Mesa de conferência (centro-direita) com Romaneio ---
        MontarMesaConferencia(madeiraEscura, acoEscuro, papel, latao, bulboQuente, vermelho);

        // --- Atendente dos fundos ---
        Personagem("Atendente", "Art/Characters/Npc_Atendente", V(2.4f, 0f, 4.2f), "atendente_fundos");

        // --- Luminárias industriais pendentes e iluminação ---
        MontarIluminacaoDeposito(esmalteVerde, acoEscuro, bulboQuente);
    }

    private void ConstruirPortaMercadinhoFundos(Material azul, Material marco, Material latao, Material acoEscuro, Material bulbo)
    {
        const float z = 7.40f;
        Caixa("SoleiraMercadinho", V(0f, .015f, z - .06f), V(1.34f, .03f, .28f), acoEscuro, solida: false);

        var folha = Caixa("PortaServicoMercadinho", V(0f, 1.09f, z), V(1.10f, 2.18f, .12f), azul);
        foreach (float lado in new[] { -1f, 1f })
            Caixa("BatenteServico", V(lado * .60f, 1.11f, z - .03f), V(.10f, 2.22f, .16f), marco);
        Caixa("VergaServico", V(0f, 2.24f, z - .03f), V(1.30f, .10f, .16f), marco, solida: false);
        Caixa("ReforcoChapaInferior", V(0f, .32f, z - .065f), V(.98f, .52f, .02f), marco, solida: false);
        Caixa("BarraAntipanico", V(0f, 1.02f, z - .08f), V(.74f, .05f, .05f), acoEscuro, solida: false);
        Caixa("PuxadorServico", V(.40f, 1.05f, z - .08f), V(.045f, .20f, .07f), latao, solida: false);
        Letreiro("PlacaMercadinho", "MERCADINHO", V(0f, 2.48f, z - .04f), azul, 1.24f, .30f);

        // Arandela de anteparo sobre a porta dos fundos
        Caixa("ArandelaFundoBase", V(0f, 2.80f, z - .05f), V(.26f, .12f, .12f), acoEscuro, solida: false);
        Esfera("ArandelaFundoBulbo", V(0f, 2.75f, z - .09f), V(.10f, .08f, .08f), bulbo);
        Luz("LuzPortaMercadinho", V(0f, 2.66f, z - .35f), new Color(1f, .76f, .48f), .68f, 3.8f);

        var col = folha.GetComponent<Collider>();
        RegistrarPortalSecundario(col, "EntradaDaCidade", "Voltar ao mercadinho");
    }

    private void MontarEstanteDeposito(Vector3 pos, float yaw, float comp, Material aco, Material pinho,
        Material papelao, Material papelaoEscuro, Material saca)
    {
        var pivo = Pivo("EstanteAco", world, pos);
        pivo.localRotation = Quaternion.Euler(0f, yaw, 0f);
        float prof = .78f;
        float alt = 2.38f;
        float[] niveis = { .18f, .84f, 1.50f, 2.16f };

        foreach (float x in new[] { -prof * .5f + .03f, prof * .5f - .03f })
        foreach (float z in new[] { -comp * .5f + .03f, comp * .5f - .03f })
            Caixa("MontanteEstante", V(x, alt * .5f, z), V(.05f, alt, .05f), aco, pivo);

        for (int n = 0; n < niveis.Length; n++)
        {
            float y = niveis[n];
            Caixa("PrateleiraEstante", V(0f, y, 0f), V(prof, .04f, comp), pinho, pivo, solida: n < 2);
            Caixa("TravessaEstante", V(prof * .5f, y - .01f, 0f), V(.03f, .05f, comp), aco, pivo, solida: false);

            int slots = Mathf.Max(3, Mathf.RoundToInt(comp / .72f));
            for (int s = 0; s < slots; s++)
            {
                float zs = -comp * .5f + .38f + s * ((comp - .76f) / Mathf.Max(1, slots - 1));
                if ((s + n) % 3 == 0)
                {
                    Caixa("SacaPrateleira", V(0f, y + .13f, zs), V(.46f, .22f, .44f), saca, pivo, solida: false);
                }
                else
                {
                    float h = .28f + ((s + n) % 2) * .12f;
                    Caixa("CaixaPrateleira", V(-.04f, y + h * .5f + .02f, zs), V(.48f, h, .48f),
                        (s + n) % 2 == 0 ? papelao : papelaoEscuro, pivo, solida: false);
                }
            }
        }
    }

    private void MontarEstradoComCarga(Vector3 pos, Material pinho, Material papelao, Material papelaoEscuro,
        Material saca, bool comSacas)
    {
        var pivo = Pivo("EstradoCarga", world, pos);
        Caixa("EstradoBase", V(0f, .06f, 0f), V(1.22f, .12f, 1.22f), pinho, pivo);
        for (int i = -1; i <= 1; i++)
            Caixa("RipasEstrado", V(i * .42f, .125f, 0f), V(.16f, .02f, 1.20f), pinho, pivo, solida: false);

        if (comSacas)
        {
            for (int camada = 0; camada < 3; camada++)
            for (int i = 0; i < 2; i++)
            {
                var s = Caixa("SacaSecos", V(-.24f + i * .48f, .22f + camada * .17f, (camada % 2) * .08f - .04f),
                    V(.44f, .17f, .78f), saca, pivo);
                s.transform.localRotation = Quaternion.Euler(0f, (camada % 2 == 0 ? 4f : -5f), 0f);
            }
            Caixa("CaixaTopoEstrado", V(.18f, .78f, .16f), V(.52f, .34f, .48f), papelao, pivo, solida: false);
        }
        else
        {
            Caixa("CaixaGrandeA", V(-.22f, .37f, -.18f), V(.62f, .48f, .58f), papelao, pivo);
            Caixa("CaixaGrandeB", V(.28f, .34f, .20f), V(.54f, .42f, .56f), papelaoEscuro, pivo);
            Caixa("CaixaGrandeC", V(-.18f, .79f, -.12f), V(.48f, .36f, .46f), papelaoEscuro, pivo);
        }
    }

    private void MontarEngradadosRetornaveis(Vector3 pos, Material pinho, Material vermelho, Material vidroGarrafa)
    {
        var pivo = Pivo("PilhaEngradados", world, pos);
        for (int pilha = 0; pilha < 3; pilha++)
        {
            float y = .16f + pilha * .31f;
            var corCaixa = pilha % 2 == 0 ? vermelho : pinho;
            Caixa("Engradado", V(0f, y, 0f), V(.52f, .28f, .38f), corCaixa, pivo);
            if (pilha == 2)
            {
                for (int gx = -1; gx <= 1; gx++)
                for (int gz = -1; gz <= 1; gz++)
                    Cilindro("GarrafaRetornavel", V(gx * .13f, y + .20f, gz * .09f), V(.055f, .11f, .055f),
                        vidroGarrafa, pivo);
            }
        }
    }

    private void MontarMesaConferencia(Material madeira, Material acoEscuro, Material papel, Material latao,
        Material bulbo, Material vermelho)
    {
        var pivo = Pivo("MesaConferencia", world, V(1.95f, 0f, 1.55f));
        pivo.localRotation = Quaternion.Euler(0f, -18f, 0f);

        Caixa("TampoMesaConferencia", V(0f, .76f, 0f), V(1.48f, .06f, .84f), madeira, pivo);
        foreach (float x in new[] { -.66f, .66f })
        foreach (float z in new[] { -.34f, .34f })
            Caixa("PernaMesaConferencia", V(x, .37f, z), V(.07f, .74f, .07f), acoEscuro, pivo);
        Caixa("TravessaMesaConferencia", V(0f, .24f, 0f), V(1.28f, .05f, .62f), acoEscuro, pivo, solida: false);

        // Prancheta / Romaneio (interativo)
        var prancheta = Caixa("PranchetaRomaneio", V(-.18f, .805f, -.12f), V(.32f, .025f, .42f), acoEscuro,
            pivo, solida: false, interactionId: "romaneio", customPrompt: "Ler romaneio de entregas");
        prancheta.transform.localRotation = Quaternion.Euler(0f, 12f, 0f);
        Caixa("FolhaRomaneio", V(0f, .65f, 0f), V(.86f, .30f, .86f), papel, prancheta.transform, solida: false);
        Caixa("PrendedorRomaneio", V(0f, .95f, .40f), V(.40f, .45f, .12f), latao, prancheta.transform, solida: false);
        Cilindro("LapisConferencia", V(.08f, .80f, -.14f), V(.014f, .11f, .014f), vermelho, pivo)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 34f);

        // Balança de conferência
        Caixa("BalancaBase", V(.42f, .84f, .06f), V(.32f, .10f, .30f), acoEscuro, pivo);
        Caixa("BalancaPrato", V(.42f, .92f, .06f), V(.26f, .03f, .26f), latao, pivo, solida: false);
        Caixa("BalancaColuna", V(.42f, 1.05f, .19f), V(.06f, .34f, .06f), acoEscuro, pivo, solida: false);
        Caixa("BalancaVisor", V(.42f, 1.24f, .18f), V(.18f, .14f, .07f), papel, pivo, solida: false);

        // Luminária articulada de mesa
        Cilindro("LuminariaMesaBase", V(-.50f, .80f, .20f), V(.16f, .025f, .16f), acoEscuro, pivo);
        Cilindro("LuminariaMesaHaste", V(-.50f, 1.00f, .20f), V(.025f, .20f, .025f), latao, pivo);
        var cupula = Cilindro("LuminariaMesaCupula", V(-.42f, 1.15f, .12f), V(.18f, .08f, .18f), acoEscuro, pivo);
        cupula.transform.localRotation = Quaternion.Euler(26f, 0f, -24f);
        Esfera("LuminariaMesaBulbo", V(-.40f, 1.12f, .10f), V(.06f, .06f, .06f), bulbo, pivo);
        SpotLuz("LuzMesaConferencia", V(-.40f, 1.10f, .10f), V(68f, -140f, 0f),
            new Color(1f, .80f, .48f), 1.45f, 3.1f, 56f, 22f, pivo);
    }

    private void MontarIluminacaoDeposito(Material esmalteVerde, Material acoEscuro, Material bulbo)
    {
        float[] zLuzes = { -1.4f, 2.2f, 5.4f };
        var bulboApagado = Superficie("BulboApagadoDeposito", new Color(.16f, .16f, .16f), brilho: .2f);
        var corBulbo = new Color(1f, .84f, .58f);

        for (int i = 0; i < zLuzes.Length; i++)
        {
            float z = zLuzes[i];
            float x = i == 1 ? .25f : -.15f;
            Cilindro("CanoLuminaria", V(x, 2.98f, z), V(.025f, .32f, .025f), acoEscuro);
            Cilindro("CupulaIndustrial", V(x, 2.66f, z), V(.44f, .09f, .44f), esmalteVerde);

            if (i == 1)
            {
                // Lâmpada central queimada cria corredor escuro entre as estantes
                Esfera("BulboIndustrial", V(x, 2.60f, z), V(.11f, .11f, .11f), bulboApagado);
                continue;
            }

            if (i == 2)
            {
                var bulboFalho = BulboInstancia("BulboFalhoDeposito", corBulbo);
                Esfera("BulboIndustrial", V(x, 2.60f, z), V(.11f, .11f, .11f), bulboFalho);
                var ptFalho = Luz("LuzTetoDeposito_" + i, V(x, 2.48f, z), new Color(1f, .80f, .52f), .95f, 5.8f);
                var spotFalho = SpotLuz("ConeTetoDeposito_" + i, V(x, 2.55f, z), V(90f, 0f, 0f),
                    new Color(1f, .82f, .56f), .85f, 4.8f, 70f, 26f);
                LuzOscilante(ptFalho, bulboFalho, .95f, corBulbo, new Color(.16f, .15f, .13f),
                    intensidadeMinFator: .06f, somFaisca: true, ganhoSom: .24f, semente: 3.7f);
                RegistrarAnimacao(_ =>
                {
                    if (spotFalho != null && ptFalho != null)
                        spotFalho.intensity = ptFalho.intensity * .88f;
                });
                continue;
            }

            Esfera("BulboIndustrial", V(x, 2.60f, z), V(.11f, .11f, .11f), bulbo);
            Luz("LuzTetoDeposito_" + i, V(x, 2.48f, z), new Color(1f, .78f, .50f), .62f, 5.0f);
            SpotLuz("ConeTetoDeposito_" + i, V(x, 2.55f, z), V(90f, 0f, 0f),
                new Color(1f, .80f, .54f), .58f, 4.2f, 72f, 28f);
        }

        SomLocal("Compressor", V(-4.2f, .6f, 6.2f), 8.2f, .42f);
    }
}
