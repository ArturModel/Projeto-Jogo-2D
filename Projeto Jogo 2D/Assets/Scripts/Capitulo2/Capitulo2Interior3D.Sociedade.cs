using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private void MontarSociedade()
    {
        const float xMin = -7.5f;
        const float xMax = 15.5f;
        const float zMin = -4.5f;
        const float zMax = 12.5f;
        const float altura = 4.2f;
        const float centroX = (xMin + xMax) * .5f;
        const float centroZ = (zMin + zMax) * .5f;
        const float largura = xMax - xMin;
        const float profundidade = zMax - zMin;

        ConfigurarEntrada(zMin, V(0f, EyeHeight, -2.85f), "Foley/footstep_wood_000");
        pendenteSociedade = V(0f, 3.25f, 1.2f);

        // Materiais principais
        var texMadeira = Resources.Load<Texture2D>("Bocha/MadeiraCancha")
            ?? CriarTabuas(new Color(.36f, .25f, .16f), new Color(.29f, .20f, .13f));
        var texSaibro = Resources.Load<Texture2D>("Bocha/SaibroCancha");

        var pisoMadeira = Superficie("SociedadePisoMadeira", new Color(.78f, .70f, .58f), texMadeira, new Vector2(10f, 8f), .18f);
        var pisoSaibro = Superficie("SociedadeSaibro", new Color(.82f, .64f, .46f), texSaibro, new Vector2(8f, 3f), .06f);
        var madeira = Superficie("SociedadeMadeira", new Color(.35f, .24f, .15f), texMadeira, new Vector2(2f, 2f), .22f);
        var madeiraClara = Superficie("SociedadeMadeiraClara", new Color(.48f, .35f, .23f), texMadeira, new Vector2(2f, 1f), .24f);
        var madeiraEscura = Superficie("SociedadeMadeiraEscura", new Color(.20f, .15f, .11f), brilho: .16f);
        var reboco = Superficie("SociedadeReboco", new Color(.40f, .38f, .31f), brilho: .06f);
        var lambril = Superficie("SociedadeLambril", new Color(.18f, .22f, .19f), brilho: .12f);
        var tetoMat = Superficie("SociedadeTeto", new Color(.16f, .13f, .10f), brilho: .08f);
        var metal = Superficie("SociedadeMetal", new Color(.22f, .23f, .25f), brilho: .55f);
        var latao = Superficie("SociedadeLatao", new Color(.68f, .54f, .28f), brilho: .65f);
        var vidro = VidroExterior();
        var papel = Superficie("SociedadePapel", new Color(.84f, .79f, .67f), brilho: .05f);
        var capaLivro = Superficie("SociedadeCapaLivro", new Color(.34f, .20f, .14f), brilho: .15f);
        var cordaMat = Superficie("SociedadeCorda", new Color(.66f, .56f, .38f), brilho: .05f);
        var gizBranco = Superficie("SociedadeGiz", new Color(.84f, .81f, .73f), brilho: .04f);

        // Piso completo (assoalho ao redor da cancha + cancha de saibro rebaixada levemente sob as tábuas)
        // Cancha: x = -2.0 a 14.0, z = 3.5 a 9.5
        Caixa("PisoSalaoSul", V(centroX, -.05f, -.5f), V(largura, .10f, 8.0f), pisoMadeira);
        Caixa("PisoCorredorOeste", V(-4.75f, -.05f, 6.5f), V(5.5f, .10f, 6.0f), pisoMadeira);
        Caixa("PisoCorredorLeste", V(14.75f, -.05f, 6.5f), V(1.5f, .10f, 6.0f), pisoMadeira);
        Caixa("PisoCorredorNorte", V(centroX, -.05f, 11.0f), V(largura, .10f, 3.0f), pisoMadeira);
        Caixa("PisoCanchaSaibro", V(6.0f, -.06f, 6.5f), V(16.0f, .10f, 6.0f), pisoSaibro);

        // Teto e tesouras de madeira
        Caixa("TetoSociedade", V(centroX, altura + .08f, centroZ), V(largura + .4f, .16f, profundidade + .4f), tetoMat);
        for (float x = -5.5f; x <= 13.5f; x += 3.8f)
        {
            Caixa("VigaTeto", V(x, altura - .12f, centroZ), V(.22f, .24f, profundidade), madeiraEscura, solida: false);
            Caixa("PilastraParedeNorte", V(x, altura * .5f, zMax - .08f), V(.22f, altura, .16f), madeiraEscura);
        }

        // Paredes externas com colisão sólida
        // Parede frontal (z = -4.5) com porta em x = 0 e 3 janelas para a rua chuvosa
        var vaosFrente = new[]
        {
            new Rect(-4.4f, 1.05f, 1.8f, 1.35f),
            new Rect(-.53f, 0f, 1.06f, 2.18f),
            new Rect(3.2f, 1.05f, 1.8f, 1.35f),
            new Rect(8.8f, 1.05f, 1.8f, 1.35f)
        };
        ExteriorInterior3D.Parede(xMin, xMax, altura, vaosFrente, (a, b, y0, y1) =>
        {
            float w = b - a;
            float h = y1 - y0;
            if (w <= .01f || h <= .01f) return;
            Caixa("ParedeFrente", V((a + b) * .5f, (y0 + y1) * .5f, zMin), V(w, h, .22f), reboco);
        });

        // Janelas frontais com vidro exterior e esquadrias
        foreach (float jx in new[] { -3.5f, 4.1f, 9.7f })
            ConstruirJanelaSociedade(V(jx, 1.725f, zMin), 1.8f, 1.35f, madeira, vidro);

        // Lambril frontal (abaixo das janelas e entre a porta)
        Caixa("LambrilFrenteEsq", V(-4.015f, .50f, zMin + .12f), V(6.97f, 1.00f, .04f), lambril, solida: false);
        Caixa("FrisoFrenteEsq", V(-4.015f, 1.02f, zMin + .13f), V(6.97f, .05f, .06f), madeira, solida: false);
        Caixa("LambrilFrenteDir", V(8.015f, .50f, zMin + .12f), V(14.97f, 1.00f, .04f), lambril, solida: false);
        Caixa("FrisoFrenteDir", V(8.015f, 1.02f, zMin + .13f), V(14.97f, .05f, .06f), madeira, solida: false);

        // Parede norte (fundo, z = 12.5), oeste (x = -7.5) e leste (x = 15.5)
        ParedeCompleta("ParedeNorte", V(centroX, altura * .5f, zMax), V(largura, altura, .24f), reboco, lambril, madeira, true);
        ParedeCompleta("ParedeOeste", V(xMin, altura * .5f, centroZ), V(.24f, altura, profundidade), reboco, lambril, madeira, false);
        ParedeCompleta("ParedeLeste", V(xMax, altura * .5f, centroZ), V(.24f, altura, profundidade), reboco, lambril, madeira, false);

        // Porta de entrada, chuva e exterior
        ConstruirPorta(madeira, madeiraEscura, latao, 0f, zMin, 1.06f, 2.18f);
        Caixa("CapachoEntrada", V(0f, .01f, zMin + .55f), V(1.35f, .02f, .65f), madeiraEscura, solida: false);
        ConstruirChuva(zMin - .7f);
        ExteriorInterior3D.Montar(world, vista, ExteriorLayer, V(0f, 0f, zMin - .03f), 180f, ExteriorInterior3D.Local.SociedadeBeiraRio);

        // --- SALÃO SOCIAL (z = -4.5 a 2.5) ---
        ConstruirBalcaoNair(madeira, madeiraClara, madeiraEscura, latao, metal, vidro);
        ConstruirMesaVisitas(madeira, madeiraEscura, papel, capaLivro, latao);
        ConstruirMesasSalao(madeira, madeiraClara, madeiraEscura, vidro);

        // Nair atrás/junto ao balcão
        Personagem("Nair", "Art/Characters/Npc_Nair", V(-4.2f, 0f, 1.2f), "nair");

        // Pendente principal da Sociedade (usado por BochaMinigameUI / PendenteNoEcra)
        ConstruirPendente("PendenteSociedadePrincipal", pendenteSociedade, 4.2f, metal, latao, .88f, 6.8f, balanco: true);

        // --- CANCHA DE BOCHA (x = -2.0 a 14.0, z = 3.5 a 9.5) ---
        ConstruirCanchaBocha(madeira, madeiraClara, madeiraEscura, gizBranco);

        // Quadros e placares na parede norte
        QuadroParede("PlacarSociedade", "Bocha/PlacarSociedade", V(3.0f, 2.35f, zMax - .18f), 2.1f, 1.25f, madeiraEscura);
        QuadroParede("MuralSociedade", "Bocha/MuralSociedade", V(8.5f, 2.35f, zMax - .18f), 2.5f, 1.35f, madeiraEscura);
        QuadroParede("FlamulaSociedade", "Bocha/FlamulaSociedade", V(-3.8f, 2.45f, zMax - .18f), .85f, .70f, madeiraEscura);

        // Quadro de Regras da Bocha (próximo à entrada da cancha)
        ConstruirQuadroRegras(V(-2.75f, 0f, 3.15f), madeira, madeiraEscura, papel);

        // Suporte de Bochas + alvo "bocha" na cabeceira oeste da cancha
        ConstruirSuporteBochas(V(-2.15f, 0f, 6.5f), madeira, madeiraEscura);

        // Cordas enroladas e balde de manutenção no canto fundo-esquerdo da cancha
        ConstruirCordasManutencao(V(-5.8f, 0f, 10.9f), cordaMat, madeiraEscura, metal);

        // Tonão junto à cancha de bocha
        Personagem("Tonão", "Art/Characters/Npc_Tonao", V(2.6f, 0f, 6.5f), "tonao");

        // Luminárias pendentes baixas sobre a cancha de saibro (leste falhando com estalos)
        ConstruirPendente("PendenteCancha_1.5", V(1.5f, 3.05f, 6.5f), altura, metal, latao, .72f, 6.2f, balanco: true);
        ConstruirPendente("PendenteCancha_6", V(6.0f, 3.05f, 6.5f), altura, metal, latao, 1.05f, 7.2f, balanco: true);
        ConstruirPendente("PendenteCancha_10.5", V(10.5f, 3.05f, 6.5f), altura, metal, latao, .85f, 6.5f, balanco: true, oscilar: true);

        // Luzes de preenchimento contidas para manter cantos escuros e tensão
        Luz("LuzEntradaSociedade", V(0f, 2.6f, -2.6f), new Color(1f, .74f, .46f), .45f, 5.2f);
        Luz("LuzBalcaoNair", V(-4.6f, 2.7f, 1.1f), new Color(1f, .76f, .48f), .75f, 5.4f);
        Luz("LuzSalaoLeste", V(8.2f, 2.9f, -.2f), new Color(1f, .74f, .48f), .38f, 6.0f);
        Luz("LuzCantoCordas", V(-5.4f, 2.6f, 10.5f), new Color(.92f, .70f, .44f), .48f, 4.8f);

        foreach (float jx in new[] { -3.5f, 4.1f, 9.7f })
            Luz("BrilhoRuaJanela", V(jx, 1.75f, zMin + .45f), new Color(.34f, .48f, .68f), .28f, 3.6f);

        SomLocal("Compressor", V(-5.6f, .9f, 1.2f), 7.5f, .32f);
        SomLocal("MotorVentilador", V(6.0f, 4.0f, 6.5f), 11.5f, .24f);
    }

    private void ParedeCompleta(string nome, Vector3 pos, Vector3 escala, Material reboco, Material lambril, Material friso, bool eixoX)
    {
        Caixa(nome, pos, escala, reboco);
        float yLambril = .52f;
        Vector3 escLambril = eixoX
            ? V(escala.x, 1.04f, .04f)
            : V(.04f, 1.04f, escala.z);
        Vector3 offset = eixoX
            ? V(0f, yLambril - pos.y, pos.z > 0f ? -.13f : .13f)
            : V(pos.x > 0f ? -.13f : .13f, yLambril - pos.y, 0f);
        Caixa(nome + "_Lambril", pos + offset, escLambril, lambril, solida: false);

        Vector3 escFriso = eixoX
            ? V(escala.x, .05f, .06f)
            : V(.06f, .05f, escala.z);
        Caixa(nome + "_Friso", pos + offset + V(0f, .54f, 0f), escFriso, friso, solida: false);
    }

    private void ConstruirJanelaSociedade(Vector3 centro, float larg, float alt, Material marco, Material vidro)
    {
        Caixa("VidroJanela", centro, V(larg, alt, .03f), vidro, solida: false);
        MolduraVazada(centro, larg, alt, marco, .065f, .18f);
        Caixa("DivisoriaJanelaV", centro, V(.05f, alt, .08f), marco, solida: false);
        Caixa("DivisoriaJanelaH", centro, V(larg, .05f, .08f), marco, solida: false);
        Caixa("PeitorilJanela", centro - V(0f, alt * .5f + .04f, -.04f), V(larg + .18f, .06f, .28f), marco, solida: false);
    }

    private void ConstruirBalcaoNair(Material madeira, Material madeiraClara, Material madeiraEscura,
        Material latao, Material metal, Material vidro)
    {
        // Balcão de atendimento na lateral oeste do salão (x = -5.85, z = 1.1)
        const float bx = -5.85f;
        const float bz = 1.1f;
        Caixa("BalcaoCorpo", V(bx, .50f, bz), V(.72f, 1.00f, 3.60f), madeira);
        Caixa("BalcaoTampo", V(bx + .05f, 1.03f, bz), V(.86f, .07f, 3.74f), madeiraClara);
        Caixa("BalcaoRodape", V(bx + .37f, .08f, bz), V(.04f, .16f, 3.60f), madeiraEscura, solida: false);
        Caixa("ApoioPeBalcao", V(bx + .48f, .16f, bz), V(.05f, .05f, 3.45f), latao, solida: false);

        // Estante de fundo contra a parede oeste
        Caixa("EstanteFundoBalcao", V(-7.15f, 1.55f, bz), V(.36f, 2.20f, 3.30f), madeiraEscura);
        for (float sy = .95f; sy <= 2.35f; sy += .55f)
            Caixa("PrateleiraBalcao", V(-7.05f, sy, bz), V(.38f, .04f, 3.20f), madeiraClara, solida: false);

        // Garrafas e copos no balcão e na estante
        var garrafaVerde = Superficie("GarrafaVerde", new Color(.14f, .34f, .18f), brilho: .75f);
        var garrafaAmbar = Superficie("GarrafaAmbar", new Color(.44f, .24f, .09f), brilho: .75f);
        for (int i = 0; i < 6; i++)
        {
            float gz = bz - 1.15f + i * .46f;
            var gMat = i % 2 == 0 ? garrafaVerde : garrafaAmbar;
            Cilindro("GarrafaEstante", V(-7.02f, 1.68f, gz), V(.07f, .16f, .07f), gMat);
            Cilindro("CopoBalcao", V(bx + .14f, 1.12f, gz - .1f), V(.06f, .06f, .06f), vidro);
        }

        // Chaleira / térmica de água quente para chimarrão
        Cilindro("TermicaChimarrão", V(bx, 1.24f, bz + 1.15f), V(.16f, .18f, .16f), metal);
        Cilindro("TampaTermica", V(bx, 1.44f, bz + 1.15f), V(.11f, .04f, .11f), latao);
        Arte("ApoioAguaQuente", "Art/Props/ApoioAguaQuente", V(-6.8f, .85f, bz - 1.2f), 1.55f, 90f);
    }

    private void ConstruirMesaVisitas(Material madeira, Material madeiraEscura, Material papel, Material capaLivro, Material latao)
    {
        // Mesa de recepção e Livro de Visitas próximo à entrada (x = 2.5f, z = -0.55f)
        var pos = V(2.5f, 0f, -.55f);
        Caixa("MesaVisitasTampo", pos + V(0f, .80f, 0f), V(1.65f, .07f, .82f), madeira);
        foreach (float dx in new[] { -.70f, .70f })
            foreach (float dz in new[] { -.30f, .30f })
                Caixa("PeMesaVisitas", pos + V(dx, .39f, dz), V(.08f, .78f, .08f), madeiraEscura);

        // Livro de visitas aberto com interação "visitas_sociedade"
        var livroPos = pos + V(-.12f, .855f, -.04f);
        Caixa("LivroVisitasCapa", livroPos, V(.44f, .035f, .32f), capaLivro,
            solida: false, interactionId: "visitas_sociedade", customPrompt: "[E] Ler livro de visitas");
        Caixa("LivroVisitasFolhaEsq", livroPos + V(-.10f, .022f, 0f), V(.19f, .014f, .28f), papel, solida: false);
        Caixa("LivroVisitasFolhaDir", livroPos + V(.10f, .022f, 0f), V(.19f, .014f, .28f), papel, solida: false);
        Cilindro("TinteiroVisitas", pos + V(.26f, .865f, .10f), V(.06f, .035f, .06f), latao);

        // Luminária de mesa quente iluminando o livro
        var lumPos = pos + V(.55f, .84f, .16f);
        Cilindro("BaseAbajurVisitas", lumPos + V(0f, .02f, 0f), V(.14f, .02f, .14f), latao);
        Cilindro("HasteAbajurVisitas", lumPos + V(0f, .18f, 0f), V(.03f, .16f, .03f), latao);
        Cilindro("CupulaAbajurVisitas", lumPos + V(0f, .36f, 0f), V(.26f, .12f, .26f),
            Superficie("CupulaVisitas", new Color(.88f, .72f, .46f), brilho: .1f));
        Luz("LuzMesaVisitas", lumPos + V(0f, .30f, 0f), new Color(1f, .76f, .44f), .62f, 3.2f);
    }

    private void ConstruirMesasSalao(Material madeira, Material madeiraClara, Material madeiraEscura, Material vidro)
    {
        Vector3[] mesas =
        {
            V(6.2f, 0f, -1.3f),
            V(10.6f, 0f, -1.3f),
            V(8.4f, 0f, 1.1f)
        };

        foreach (var m in mesas)
        {
            Caixa("MesaSalaoTampo", m + V(0f, .76f, 0f), V(1.30f, .06f, .95f), madeiraClara);
            foreach (float dx in new[] { -.52f, .52f })
                foreach (float dz in new[] { -.36f, .36f })
                    Caixa("PeMesaSalao", m + V(dx, .37f, dz), V(.07f, .74f, .07f), madeiraEscura);

            // Cadeiras ao redor da mesa
            ConstruirCadeira(m + V(0f, 0f, -.72f), 0f, madeira, madeiraEscura);
            ConstruirCadeira(m + V(0f, 0f, .72f), 180f, madeira, madeiraEscura);
            ConstruirCadeira(m + V(-.88f, 0f, 0f), 90f, madeira, madeiraEscura);

            // Copos sobre a mesa
            Cilindro("CopoMesa1", m + V(-.22f, .84f, .12f), V(.06f, .06f, .06f), vidro);
            Cilindro("CopoMesa2", m + V(.18f, .84f, -.14f), V(.06f, .06f, .06f), vidro);
        }
    }

    private void ConstruirCadeira(Vector3 pos, float yaw, Material madeira, Material madeiraEscura)
    {
        var pivo = Pivo("Cadeira", world, pos);
        pivo.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Caixa("AssentoCadeira", V(0f, .45f, 0f), V(.44f, .05f, .42f), madeira, pivo);
        Caixa("EncostoCadeira", V(0f, .72f, -.18f), V(.42f, .46f, .05f), madeira, pivo, solida: false);
        foreach (float dx in new[] { -.17f, .17f })
            foreach (float dz in new[] { -.16f, .16f })
                Caixa("PeCadeira", V(dx, .22f, dz), V(.05f, .44f, .05f), madeiraEscura, pivo, solida: false);
    }

    private void ConstruirCanchaBocha(Material madeira, Material madeiraClara, Material madeiraEscura, Material gizBranco)
    {
        // Limites da cancha: x = -2.0 a 14.0 (centro 6.0, larg 16.0), z = 3.5 a 9.5 (centro 6.5, prof 6.0)
        // Tábuas laterais (guias / kickboards)
        foreach (float z in new[] { 3.42f, 9.58f })
        {
            Caixa("TabuaLateralCancha", V(6.0f, .18f, z), V(16.28f, .36f, .16f), madeira);
            Caixa("CorrimaoLateralCancha", V(6.0f, .38f, z), V(16.32f, .05f, .20f), madeiraClara, solida: false);
        }
        // Cabeceiras oeste e leste
        foreach (float x in new[] { -2.08f, 14.08f })
        {
            Caixa("CabeceiraCancha", V(x, .22f, 6.5f), V(.16f, .44f, 6.16f), madeira);
            Caixa("TopoCabeceiraCancha", V(x, .46f, 6.5f), V(.20f, .05f, 6.20f), madeiraClara, solida: false);
        }

        // Linhas de lançamento e marca central no saibro
        Caixa("LinhaLancamentoOeste", V(-.4f, -.005f, 6.5f), V(.05f, .01f, 5.6f), gizBranco, solida: false);
        Caixa("LinhaLancamentoLeste", V(12.4f, -.005f, 6.5f), V(.05f, .01f, 5.6f), gizBranco, solida: false);
        Caixa("LinhaCentroCancha", V(6.0f, -.005f, 6.5f), V(.04f, .01f, 5.6f), gizBranco, solida: false);

        // Bancos de madeira para espectadores ao longo dos dois lados da cancha
        foreach (float bx in new[] { 1.2f, 5.8f, 10.4f })
        {
            ConstruirBancoEspectador(V(bx, 0f, 2.70f), 2.8f, encostoSul: true, madeira, madeiraEscura);
            ConstruirBancoEspectador(V(bx, 0f, 10.35f), 2.8f, encostoSul: false, madeira, madeiraEscura);
        }
    }

    private void ConstruirBancoEspectador(Vector3 pos, float larg, bool encostoSul, Material madeira, Material madeiraEscura)
    {
        Caixa("AssentoBanco", pos + V(0f, .44f, 0f), V(larg, .08f, .50f), madeira);
        Caixa("EncostoBanco", pos + V(0f, .72f, encostoSul ? -.21f : .21f), V(larg, .42f, .07f), madeira);
        foreach (float dx in new[] { -larg * .38f, larg * .38f })
            Caixa("PeBanco", pos + V(dx, .21f, 0f), V(.12f, .42f, .42f), madeiraEscura);
    }

    private void QuadroParede(string nome, string resource, Vector3 pos, float larg, float alt, Material moldura)
    {
        var tex = Resources.Load<Texture2D>(resource);
        if (tex != null)
        {
            var mat = Superficie(nome + "_Mat", Color.white, tex, Vector2.one, .08f);
            Caixa(nome, pos, V(larg, alt, .03f), mat, solida: false);
        }
        else
        {
            Arte(nome, resource, pos, alt, 180f);
        }
        MolduraVazada(pos, larg, alt, moldura, .06f, .08f);
    }

    private void ConstruirQuadroRegras(Vector3 pos, Material madeira, Material madeiraEscura, Material papel)
    {
        var pivo = Pivo("QuadroRegrasBocha", world, pos);
        pivo.localRotation = Quaternion.Euler(0f, -24f, 0f);
        Caixa("PosteRegrasEsq", V(-.45f, .85f, 0f), V(.09f, 1.70f, .09f), madeiraEscura, pivo);
        Caixa("PosteRegrasDir", V(.45f, .85f, 0f), V(.09f, 1.70f, .09f), madeiraEscura, pivo);
        Caixa("QuadroRegrasFundo", V(0f, 1.42f, 0f), V(1.02f, .82f, .06f), madeira, pivo,
            solida: true, interactionId: "regra_bocha", customPrompt: "[E] Ler regras da bocha");
        Caixa("QuadroRegrasFolha", V(0f, 1.42f, -.036f), V(.86f, .66f, .012f), papel, pivo, solida: false);
        Letreiro("TextoRegrasBocha", "REGRAS DA BOCHA\n4 bochas por lado\nVence quem fizer 6 pontos",
            pos + V(0f, 1.42f, -.045f), papel, .82f, .60f, -24f, new Color(.16f, .13f, .10f));
    }

    private void ConstruirSuporteBochas(Vector3 pos, Material madeira, Material madeiraEscura)
    {
        // Suporte de madeira na cabeceira oeste com as 8 bochas + balim e alvo "bocha"
        Caixa("SuporteBochasBase", pos + V(0f, .42f, 0f), V(.42f, .84f, 1.45f), madeiraEscura,
            solida: true, interactionId: "bocha", customPrompt: "[E] Jogar bocha");
        Caixa("SuporteBochasCalha", pos + V(0f, .88f, 0f), V(.46f, .08f, 1.50f), madeira,
            solida: false, interactionId: "bocha", customPrompt: "[E] Jogar bocha");

        var marcusMat = Superficie("BochaMarcus", new Color(.16f, .44f, .48f), brilho: .52f);
        var tonaoMat = Superficie("BochaTonao", new Color(.68f, .34f, .16f), brilho: .52f);
        var balimMat = Superficie("BalimCancha", new Color(.88f, .82f, .64f), brilho: .48f);

        for (int i = 0; i < 4; i++)
        {
            Esfera("BochaMarcusRack", pos + V(-.08f, .98f, -.52f + i * .32f), Vector3.one * .14f, marcusMat);
            Esfera("BochaTonaoRack", pos + V(.08f, .98f, -.52f + i * .32f), Vector3.one * .14f, tonaoMat);
        }

        // Algumas bochas + balim já no saibro perto da cabeceira para chamar o olhar
        Esfera("BalimSaibro", V(1.65f, .035f, 6.35f), Vector3.one * .07f, balimMat);
        Esfera("BochaSaibro1", V(1.25f, .065f, 6.10f), Vector3.one * .14f, marcusMat);
        Esfera("BochaSaibro2", V(1.90f, .065f, 6.72f), Vector3.one * .14f, tonaoMat);

        // Alvo amplo na beira da cancha para facilitar iniciar a bocha olhando para as bolas ou cabeceira
        AlvoFisico(world, "AlvoCanchaBocha", pos + V(.25f, .95f, 0f), V(1.1f, 1.3f, 2.2f),
            "bocha", "[E] Jogar bocha");
    }

    private void ConstruirCordasManutencao(Vector3 pos, Material cordaMat, Material madeiraEscura, Material metal)
    {
        // Rolos de corda de sisal + balde de manutenção no canto fundo-esquerdo
        for (int r = 0; r < 3; r++)
        {
            var roloPos = pos + V(r * .38f - .32f, .09f, (r % 2) * .22f);
            Cilindro("RoloCordaBase_" + r, roloPos, V(.34f, .09f, .34f), cordaMat,
                solida: true, interactionId: "cordas", customPrompt: "[E] Examinar cordas");
            Cilindro("RoloCordaTopo_" + r, roloPos + V(0f, .14f, 0f), V(.28f, .07f, .28f), cordaMat, solida: false);
        }
        Cilindro("BaldeManutencao", pos + V(.65f, .18f, -.18f), V(.28f, .18f, .28f), metal, solida: true);
        Caixa("RodoSaibroCabo", pos + V(.78f, .68f, .25f), V(.05f, 1.35f, .05f), madeiraEscura, solida: false);
        Caixa("RodoSaibroBase", pos + V(.78f, .05f, .25f), V(.65f, .08f, .10f), madeiraEscura, solida: false);
        AlvoFisico(world, "AlvoCordasSociedade", pos + V(0f, .45f, 0f), V(1.4f, 1.0f, 1.2f),
            "cordas", "[E] Examinar cordas");
    }

    private void ConstruirPendente(string nome, Vector3 pos, float tetoY, Material metal, Material latao,
        float intensidade, float alcance, bool balanco, bool oscilar = false)
    {
        float fioAlt = Mathf.Max(.2f, tetoY - pos.y);
        var pivo = Pivo(nome + "_Pivo", world, V(pos.x, tetoY, pos.z));
        Cilindro(nome + "_Fio", V(0f, -fioAlt * .5f, 0f), V(.025f, fioAlt * .5f, .025f), metal, pivo);
        Cilindro(nome + "_Soquete", V(0f, -fioAlt + .08f, 0f), V(.09f, .07f, .09f), latao, pivo);
        Cilindro(nome + "_Cupula", V(0f, -fioAlt - .03f, 0f), V(.54f, .08f, .54f), metal, pivo);
        var corLamp = new Color(1f, .84f, .54f);
        var lampMat = oscilar
            ? BulboInstancia(nome + "_LampadaFalha", corLamp)
            : Superficie("LampadaAcesaSociedade", corLamp, brilho: .2f, unlit: true);
        Esfera(nome + "_Lampada", V(0f, -fioAlt - .09f, 0f), Vector3.one * .14f, lampMat, pivo);

        var luz = Luz(nome + "_Luz", V(0f, -fioAlt - .22f, 0f), new Color(1f, .78f, .48f), intensidade, alcance, pivo);
        var spot = SpotLuz(nome + "_Spot", V(0f, -fioAlt - .14f, 0f), V(90f, 0f, 0f), new Color(1f, .80f, .50f),
            intensidade * 1.05f, alcance + 1.0f, 76f, 30f, pivo);

        if (oscilar)
        {
            LuzOscilante(luz, lampMat, intensidade, corLamp, new Color(.18f, .14f, .10f),
                intensidadeMinFator: .08f, somFaisca: true, ganhoSom: .22f, semente: pos.x * .47f + 8.3f);
            RegistrarAnimacao(_ =>
            {
                if (spot != null && luz != null) spot.intensity = luz.intensity * 1.05f;
            });
        }

        if (balanco)
        {
            float fase = pos.x * .7f + pos.z * .4f;
            float baseInt = luz.intensity;
            float tempo = 0f;
            RegistrarAnimacao(dt =>
            {
                tempo += dt;
                pivo.localRotation = Quaternion.Euler(Mathf.Sin(tempo * 1.1f + fase) * .9f, 0f, Mathf.Cos(tempo * .85f + fase) * 1.1f);
                if (!oscilar)
                    luz.intensity = baseInt + Mathf.Sin(tempo * 6.5f + fase) * .04f;
            });
        }
    }
}
