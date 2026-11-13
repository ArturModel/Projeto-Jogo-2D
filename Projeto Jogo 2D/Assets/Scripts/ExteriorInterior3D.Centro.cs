using UnityEngine;

internal sealed partial class ExteriorInterior3D
{
    private static readonly string[] CopasCentro = { "Camboata", "Bracatinga", "Pitangueira", "Araucaria" };

    private bool ConstruirCentro(int layer, Local local)
    {
        switch (local)
        {
            case Local.IgrejaPraca: MontarIgrejaPraca(layer); return true;
            case Local.PrefeituraFrente: MontarPrefeituraFrente(layer); return true;
            case Local.PrefeituraFundos: MontarMatagalCercado(layer, 101, comMuro: true, zMuro: 4.5f); return true;
            case Local.RodoviariaFrente: MontarRuaAsfaltada(layer, 102, larguraCalcada: 2.6f, larguraPista: 7.0f, comBanco: true); return true;
            case Local.RodoviariaPlataforma: MontarRodoviariaPlataforma(layer); return true;
            case Local.DepositoPatio: MontarDepositoPatio(layer); return true;
            case Local.OficinaRua: MontarRuaDeChaoMatagal(layer, 104, comPosteRede: true, comCorreio: false); return true;
            case Local.CostureiraRua: MontarRuaDeChaoMatagal(layer, 105, comPosteRede: false, comCorreio: true); return true;
            case Local.SociedadeBeiraRio: MontarSociedadeBeiraRio(layer); return true;
            case Local.CemiterioFrente: MontarCemiterioFrente(layer); return true;
            case Local.CemiterioFundos: MontarCemiterioFundos(layer); return true;
            default: return false;
        }
    }

    private void MontarIgrejaPraca(int layer)
    {
        var calcada = Material("CalcadaMatriz", "Art/Tiles/Calcada", new Color(.58f, .56f, .50f), new Vector2(22f, 3f), .58f);
        var pedra = Material("PedraMatriz", "Art/Tiles/Pedra", new Color(.52f, .49f, .43f), new Vector2(16f, 16f), .68f);
        var grama = Material("GramaPraca", "Art/Tiles/Grama", new Color(.32f, .41f, .27f), new Vector2(8f, 8f));
        var asfalto = Material("AsfaltoPraca", "Art/Tiles/Asfalto", new Color(.46f, .49f, .51f), new Vector2(24f, 4f), .92f);

        // Adro da Igreja (z = 0..4.5) e Praça da Matriz (z = 4.5..26.5)
        Piso(layer, "Adro da Matriz", 0f, 4.5f, -.02f, calcada);
        Piso(layer, "Piso da Praca", 4.5f, 26.5f, -.05f, pedra);
        Piso(layer, "Avenida da Praca", 26.5f, 33.0f, -.15f, asfalto);
        Piso(layer, "Margem sul da Praca", 33.0f, 42.0f, -.06f, grama);

        // Guias da avenida além da praça
        foreach (float z in new[] { 26.5f, 33.0f })
            Objeto(layer, PrimitiveType.Cube, "Meio-fio Praca", new Vector3(0f, -.095f, z), new Vector3(40f, .14f, .18f), pedra);

        // Degraus suaves na soleira e vasos/votivos do adro
        Objeto(layer, PrimitiveType.Cube, "DegrauAdro", new Vector3(0f, -.03f, 2.2f), new Vector3(4.6f, .06f, .55f), pedra);
        Billboard(layer, "VotivosMatriz", "Art/Cap2/Remodelagem/Matriz/Votivos", new Vector3(-2.32f, -.02f, .65f), .74f, new Color(.98f, .86f, .68f));
        foreach (float x in new[] { -3.95f, 3.95f })
            Billboard(layer, "VasoAdro", "Art/Cap2/Remodelagem/Matriz/VasoFolhagem", new Vector3(x, -.02f, .95f), 1.32f, new Color(.88f, .92f, .82f));

        // Muretas laterais do adro
        foreach (float sinal in new[] { -1f, 1f })
        {
            Billboard(layer, "MuretaAdroA", "Art/Cap2/Remodelagem/Matriz/MuretaPedra", new Vector3(sinal * 6.0f, -.02f, .85f), 1.15f, new Color(.85f, .84f, .80f));
            Billboard(layer, "MuretaAdroB", "Art/Cap2/Remodelagem/Matriz/MuretaPedra", new Vector3(sinal * 9.0f, -.02f, .85f), 1.15f, new Color(.85f, .84f, .80f));
        }

        // Orelhão (leste/esquerda ao olhar para fora) e Quadro de avisos (oeste/direita)
        Billboard(layer, "OrelhaoMatriz", "Art/RuaNorte/Orelhao", new Vector3(-7.15f, -.02f, 3.65f), 2.05f, new Color(.95f, .92f, .86f));
        Billboard(layer, "BalizaOrelhao", "Art/RuaNorte/Entrada/Modulos/Balizadores", new Vector3(-7.75f, -.02f, 3.55f), .68f, new Color(.88f, .88f, .86f));
        var luzOrelhao = Luz(layer, "LuzOrelhaoMatriz", new Vector3(-7.15f, 2.1f, 3.5f), 1.35f, 5.2f);
        luzOrelhao.color = new Color(.96f, .82f, .58f);

        Billboard(layer, "QuadroPraca", "Art/Props/QuadroAvisos", new Vector3(7.15f, -.02f, 3.65f), 1.55f, new Color(.92f, .88f, .78f));
        var luzQuadro = Luz(layer, "LuzQuadroMatriz", new Vector3(7.15f, 1.95f, 3.5f), 1.1f, 4.6f);
        luzQuadro.color = new Color(1f, .78f, .52f);

        // Quatro canteiros simétricos da praça com árvores, arbustos, flores e bancos
        foreach (float sx in new[] { -1f, 1f })
        foreach (float zC in new[] { 9.8f, 19.6f })
        {
            float cx = sx * 7.2f;
            Objeto(layer, PrimitiveType.Cube, "CanteiroBorda", new Vector3(cx, -.02f, zC), new Vector3(5.6f, .10f, 5.8f), pedra);
            Objeto(layer, PrimitiveType.Cube, "CanteiroGrama", new Vector3(cx, .035f, zC), new Vector3(5.2f, .02f, 5.4f), grama);
            Billboard(layer, "BancoPraca", "Art/Cap2/Remodelagem/Centro/Banco", new Vector3(sx * 3.85f, -.04f, zC - 1.1f), .86f, new Color(.86f, .83f, .76f));
            Billboard(layer, "ArvorePraca", "Art/RuaNorte/Vegetacao/" + (zC < 12f ? "Camboata" : "Araucaria"),
                new Vector3(cx, .04f, zC + .2f), zC < 12f ? 4.8f : 6.1f, new Color(.82f, .90f, .76f));
            Billboard(layer, "ArbustoPraca", "Art/Cap2/Remodelagem/PracaDecoracao/" + (sx < 0 ? "ArbustoA" : "ArbustoB"),
                new Vector3(cx - sx * 1.4f, .04f, zC - 1.5f), .95f, new Color(.82f, .88f, .74f));
            Billboard(layer, "FloresPraca", "Art/Cap2/Remodelagem/PracaDecoracao/" + (zC < 12f ? "FloresAmarelas" : "FloresLilas"),
                new Vector3(cx - sx * 1.1f, .04f, zC + 1.4f), .68f, new Color(.92f, .92f, .88f));
        }

        // Balizadores ao longo do eixo central de pedra
        foreach (float z in new[] { 5.8f, 9.6f, 16.8f, 20.6f })
        {
            Billboard(layer, "BalizadorEsq", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", new Vector3(-3.18f, -.04f, z), .72f, new Color(.85f, .85f, .85f));
            Billboard(layer, "BalizadorDir", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", new Vector3(3.18f, -.04f, z), .72f, new Color(.85f, .85f, .85f));
        }

        // Fonte Pública / Memorial no centro da Praça (z = 14.2)
        Objeto(layer, PrimitiveType.Cube, "BaseFontePraca", new Vector3(0f, .06f, 14.2f), new Vector3(3.2f, .22f, 3.2f), pedra);
        Objeto(layer, PrimitiveType.Cube, "AguaFontePraca", new Vector3(0f, .18f, 14.2f), new Vector3(2.6f, .04f, 2.6f),
            Material("AguaFonte", "Art/Tiles/Agua", new Color(.34f, .48f, .58f), new Vector2(2f, 2f), .90f));
        Billboard(layer, "FontePublicaPraca", "Art/Props/Fonte", new Vector3(0f, .18f, 14.2f), 2.15f, new Color(.92f, .94f, .96f));
        var luzFonte = Luz(layer, "LuzAguaFonte", new Vector3(0f, .95f, 14.2f), 1.45f, 5.2f);
        luzFonte.color = new Color(.56f, .74f, .92f);

        // Quatro postes altos da praça
        var p1 = new Vector3(-10.45f, 3.82f, 6.4f);
        var p2 = new Vector3(10.45f, 3.82f, 6.4f);
        Poste(layer, p1, 10.5f);
        Poste(layer, p2, 10.5f);
        Poste(layer, new Vector3(-10.45f, 3.82f, 19.4f), 9.5f);
        Poste(layer, new Vector3(10.45f, 3.82f, 19.4f), 9.5f);

        // Mata ao fundo além da avenida
        var rng = new System.Random(202);
        for (int i = -5; i <= 5; i++)
            Billboard(layer, "ArvoreFundoPraca", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(i * 3.6f + (float)rng.NextDouble() * 1.2f, -.05f, 35.5f + (float)rng.NextDouble() * 3.5f),
                4.4f + (float)rng.NextDouble() * 2.2f, new Color(.78f, .86f, .72f));

        var luar = Luz(layer, "LuarPraca", new Vector3(0f, 6.5f, 15f), 2.2f, 26f);
        luar.color = new Color(.54f, .64f, .76f);
        var adroLuz = Luz(layer, "LuzEntradaMatriz", new Vector3(0f, 2.5f, 1.4f), 1.6f, 6.8f);
        adroLuz.color = new Color(1f, .78f, .52f);

        MontarNevoa(layer, p1, p2, false);
        MontarChuva(layer, 22f);
    }

    private void MontarPrefeituraFrente(int layer)
    {
        MontarRuaAsfaltada(layer, 103, larguraCalcada: 2.8f, larguraPista: 6.4f, comBanco: false);
        Billboard(layer, "BalizaPrefeituraEsq", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", new Vector3(-2.1f, -.03f, 2.2f), .70f, new Color(.85f, .85f, .85f));
        Billboard(layer, "BalizaPrefeituraDir", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", new Vector3(2.1f, -.03f, 2.2f), .70f, new Color(.85f, .85f, .85f));
        Billboard(layer, "MuretaOpostaPrefeitura", "Art/Cap2/Remodelagem/Matriz/MuretaPedra", new Vector3(0f, -.04f, 11.2f), 1.1f, new Color(.80f, .80f, .76f));
    }

    private void MontarRuaAsfaltada(int layer, int seed, float larguraCalcada, float larguraPista, bool comBanco)
    {
        float fimPista = larguraCalcada + larguraPista;
        var calcada = Material("CalcadaRua", "Art/Tiles/Calcada", new Color(.57f, .55f, .48f), new Vector2(22f, 2f), .60f);
        var asfalto = Material("AsfaltoRua", "Art/Tiles/Asfalto", new Color(.48f, .51f, .53f), new Vector2(24f, 5f), .94f);
        var grama = Material("GramaRua", "Art/Tiles/Grama", new Color(.34f, .42f, .28f), new Vector2(22f, 13f));
        var pedra = Material("GuiaRua", "Art/Tiles/Pedra", new Color(.46f, .44f, .36f), new Vector2(30f, 1f), .40f);
        var tinta = Material("FaixaRua", null, new Color(.54f, .45f, .22f), Vector2.one);

        Piso(layer, "CalcadaFrente", 0f, larguraCalcada, -.025f, calcada);
        Piso(layer, "AsfaltoFrente", larguraCalcada, fimPista, -.16f, asfalto);
        Piso(layer, "CalcadaOposta", fimPista, fimPista + 1.6f, -.035f, calcada);
        Piso(layer, "MataOposta", fimPista + 1.6f, 29f, -.06f, grama);

        foreach (float z in new[] { larguraCalcada, fimPista })
            Objeto(layer, PrimitiveType.Cube, "Meio-fio", new Vector3(0f, -.098f, z), new Vector3(40f, .15f, .18f), pedra);

        for (int i = -4; i <= 4; i++)
            Objeto(layer, PrimitiveType.Cube, "FaixaEixo", new Vector3(i * 4.2f, -.154f, (larguraCalcada + fimPista) * .5f),
                new Vector3(2.35f, .012f, .09f), tinta);

        if (comBanco)
        {
            Billboard(layer, "BancoCalcada", "Art/Cap2/Remodelagem/Centro/Banco", new Vector3(-2.8f, -.025f, 1.35f), .88f, new Color(.86f, .84f, .78f));
            Billboard(layer, "LixeiraCalcada", "Art/RuaNorte/Entrada/Modulos/Lixeira", new Vector3(2.9f, -.025f, 1.55f), .76f, new Color(.82f, .84f, .82f));
        }

        var rng = new System.Random(seed);
        for (int fileira = 0; fileira < 2; fileira++)
        for (int i = -5; i <= 5; i++)
        {
            float x = i * 3.55f + (float)rng.NextDouble() * 1.8f - .9f;
            float z = fimPista + 2.5f + fileira * 4.3f + (float)rng.NextDouble() * 2.0f;
            Billboard(layer, "ArvoreRua", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(x, -.04f, z), 3.8f + (float)rng.NextDouble() * 2.6f, new Color(.82f, .90f, .74f));
            if (fileira == 0)
                Billboard(layer, "MoitaRua", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x + .5f, -.04f, z - 1.55f), .85f + (float)rng.NextDouble() * .55f, new Color(.80f, .87f, .68f));
        }

        var a = new Vector3(-4.2f, 3.82f, fimPista + .55f);
        var b = new Vector3(5.2f, 3.82f, fimPista + .75f);
        Poste(layer, a, 10.2f);
        Poste(layer, b, 9.0f);
        var luar = Luz(layer, "LuarRua", new Vector3(0f, 5.4f, fimPista + 1f), 2.0f, 19f);
        luar.color = new Color(.54f, .64f, .75f);
        var reflexo = Luz(layer, "LuzMarquise", new Vector3(.3f, 2.35f, larguraCalcada - .4f), 1.5f, 6.5f);
        reflexo.color = new Color(1f, .79f, .58f);

        MontarNevoa(layer, a, b, false);
        MontarChuva(layer, fimPista + 5f);
    }

    private void MontarRodoviariaPlataforma(int layer)
    {
        var asfalto = Material("AsfaltoPlatExt", "Art/Tiles/Asfalto", new Color(.44f, .47f, .49f), new Vector2(20f, 4f), .92f);
        var grama = Material("GramaPlatExt", "Art/Tiles/Grama", new Color(.30f, .38f, .25f), new Vector2(18f, 10f));
        Piso(layer, "PistaOnibus", 0f, 7.5f, -.10f, asfalto);
        Piso(layer, "MataAlemPlataforma", 7.5f, 24f, -.06f, grama);

        for (float x = -7.5f; x <= 7.5f; x += 2.5f)
            Billboard(layer, "GradePlataforma", "Art/Cap2/Remodelagem/Cemiterio/Grade", new Vector3(x, -.04f, 7.8f), 1.15f, new Color(.72f, .76f, .78f));

        var rng = new System.Random(107);
        for (int i = -5; i <= 5; i++)
        {
            float x = i * 3.2f + (float)rng.NextDouble() * 1.4f;
            Billboard(layer, "ArvorePlat", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(x, -.05f, 9.4f + (float)rng.NextDouble() * 4.5f), 4.0f + (float)rng.NextDouble() * 2.2f, new Color(.78f, .86f, .74f));
        }

        var poste = new Vector3(4.8f, 3.82f, 6.8f);
        Poste(layer, poste, 8.8f);
        var luar = Luz(layer, "LuarPlat", new Vector3(0f, 5.2f, 8f), 1.75f, 18f);
        luar.color = new Color(.50f, .62f, .78f);

        MontarNevoa(layer, poste, new Vector3(-5.2f, 3.82f, 7.2f), true);
        MontarChuva(layer, 14f);
    }

    private void MontarDepositoPatio(int layer)
    {
        var calcada = Material("PatioDeposito", "Art/Tiles/Calcada", new Color(.54f, .52f, .46f), new Vector2(16f, 4f), .68f);
        var terra = Material("AcessoTerraDeposito", "Art/Tiles/Terra", new Color(.56f, .45f, .33f), new Vector2(16f, 4f), .85f);
        var grama = Material("GramaDeposito", "Art/Tiles/Grama", new Color(.33f, .41f, .27f), new Vector2(20f, 12f));

        Piso(layer, "PatioServico", 0f, 4.2f, -.03f, calcada);
        Piso(layer, "AcessoBarroDeposito", 4.2f, 8.2f, -.11f, terra);
        Piso(layer, "MatagalDeposito", 8.2f, 26f, -.06f, grama);

        Billboard(layer, "CaixotesPatio", "Art/RuaNorte/Caixotes", new Vector3(-3.1f, -.03f, 1.6f), .92f, new Color(.92f, .86f, .76f));
        Billboard(layer, "LixeiraPatio", "Art/RuaNorte/Entrada/Modulos/Lixeira", new Vector3(3.2f, -.03f, 1.45f), .82f, new Color(.84f, .86f, .84f));

        var rng = new System.Random(108);
        for (int fileira = 0; fileira < 2; fileira++)
        for (int i = -5; i <= 5; i++)
        {
            float x = i * 3.2f + (float)rng.NextDouble() * 1.6f - .8f;
            float z = 9.2f + fileira * 4.0f + (float)rng.NextDouble() * 1.8f;
            Billboard(layer, "ArvoreDeposito", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(x, -.05f, z), 3.7f + (float)rng.NextDouble() * 2.4f, new Color(.82f, .89f, .74f));
            if (fileira == 0)
                Billboard(layer, "MoitaDeposito", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x + .4f, -.05f, z - 1.3f), .95f + (float)rng.NextDouble() * .5f, new Color(.80f, .87f, .68f));
        }

        var poste = new Vector3(3.4f, 3.82f, 4.1f);
        Poste(layer, poste, 9.2f);
        var luzPorta = Luz(layer, "LuzPatioDeposito", new Vector3(0f, 2.4f, 1.2f), 1.45f, 6.0f);
        luzPorta.color = new Color(1f, .76f, .48f);

        MontarNevoa(layer, poste, new Vector3(-4.5f, 3.82f, 5.2f), true);
        MontarChuva(layer, 15f);
    }

    private void MontarRuaDeChaoMatagal(int layer, int seed, bool comPosteRede, bool comCorreio)
    {
        var terra = Material("RuaDeChao", "Art/Tiles/Terra", new Color(.58f, .46f, .33f), new Vector2(22f, 5f), .88f);
        var grama = Material("MatagalGrama", "Art/Tiles/Grama", new Color(.31f, .40f, .25f), new Vector2(22f, 14f));

        Piso(layer, "TerreiroCasa", 0f, 2.0f, -.04f, grama);
        Piso(layer, "RuaDeChao", 2.0f, 6.2f, -.12f, terra);
        Piso(layer, "MatagalAlemRua", 6.2f, 28f, -.05f, grama);

        if (comPosteRede)
            Billboard(layer, "PosteRedeOficina", "Art/RuaNorte/Entrada/Modulos/PosteRede", new Vector3(-2.9f, -.04f, 1.75f), 2.4f, new Color(.84f, .84f, .80f));
        if (comCorreio)
            Billboard(layer, "CorreioCostureira", "Art/RuaNorte/Bairro/Detalhes/CorreioDeMadeira", new Vector3(2.3f, -.04f, 1.35f), .85f, new Color(.92f, .86f, .74f));

        var rng = new System.Random(seed);
        for (int fileira = 0; fileira < 3; fileira++)
        for (int i = -6; i <= 6; i++)
        {
            float x = i * 2.85f + (float)rng.NextDouble() * 1.6f - .8f;
            float z = 6.9f + fileira * 3.6f + (float)rng.NextDouble() * 1.8f;
            if (fileira == 0)
            {
                Billboard(layer, "MoitaBorda", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x, -.04f, z - .45f), .95f + (float)rng.NextDouble() * .65f, new Color(.80f, .88f, .68f));
                if (i % 2 == 0)
                    Billboard(layer, "Samambaia", "Art/RuaNorte/Vegetacao/SamambaiaDaMata",
                        new Vector3(x + .9f, -.04f, z - .2f), .75f, new Color(.78f, .86f, .68f));
            }
            if (fileira >= 1 || i % 2 != 0)
                Billboard(layer, "ArvoreMata", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                    new Vector3(x, -.05f, z + .8f), 3.8f + (float)rng.NextDouble() * 2.6f, new Color(.80f, .89f, .73f));
        }

        var poste = new Vector3(4.3f, 3.82f, 5.8f);
        Poste(layer, poste, 7.6f);
        var luzPorta = Luz(layer, "LuzSoleiraCasa", new Vector3(0f, 2.2f, .9f), 1.1f, 5.2f);
        luzPorta.color = new Color(1f, .74f, .46f);
        var luar = Luz(layer, "LuarMatagal", new Vector3(0f, 5.5f, 10f), 1.5f, 18f);
        luar.color = new Color(.50f, .62f, .74f);

        MontarNevoa(layer, poste, new Vector3(-4.5f, 3.82f, 6.2f), true);
        MontarChuva(layer, 16f);
    }

    private void MontarSociedadeBeiraRio(int layer)
    {
        var terra = Material("RuaChaoBeiraRio", "Art/Tiles/Terra", new Color(.57f, .45f, .32f), new Vector2(22f, 5f), .88f);
        var grama = Material("MargemRio", "Art/Tiles/Grama", new Color(.30f, .39f, .25f), new Vector2(22f, 6f));
        var agua = Material("RioSantaRita", "Art/Tiles/Agua", new Color(.22f, .33f, .40f), new Vector2(18f, 12f), .96f);

        Piso(layer, "TerreiroSociedade", 0f, 2.2f, -.04f, terra);
        Piso(layer, "RuaDeChaoSociedade", 2.2f, 6.4f, -.11f, terra);
        Piso(layer, "MargemBeiraRio", 6.4f, 9.6f, -.06f, grama);
        Piso(layer, "RioDeSantaRita", 9.6f, 30.0f, -.24f, agua);

        Billboard(layer, "BancoSociedade", "Art/Cap2/Remodelagem/Centro/Banco", new Vector3(-3.1f, -.04f, 1.25f), .92f, new Color(.88f, .84f, .76f));
        Billboard(layer, "CorreioSociedade", "Art/RuaNorte/Bairro/Detalhes/CorreioDeMadeira", new Vector3(3.2f, -.04f, 1.15f), .85f, new Color(.92f, .86f, .76f));

        var rng = new System.Random(106);
        for (int i = -6; i <= 6; i++)
        {
            float x = i * 2.9f + (float)rng.NextDouble() * 1.4f - .7f;
            if (Mathf.Abs(x) > 1.8f)
                Billboard(layer, "MoitaBeiraRio", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x, -.05f, 7.5f + (float)rng.NextDouble() * 1.4f), .90f + (float)rng.NextDouble() * .55f, new Color(.78f, .86f, .68f));
            Billboard(layer, "ArvoreOutraMargem", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(x, -.12f, 26.5f + (float)rng.NextDouble() * 2.5f), 4.2f + (float)rng.NextDouble() * 2.0f, new Color(.55f, .66f, .62f));
        }

        var posteA = new Vector3(4.5f, 3.82f, 2.1f);
        var posteB = new Vector3(-5.8f, 3.82f, 6.6f);
        Poste(layer, posteA, 8.8f);
        Poste(layer, posteB, 7.8f);
        var reflexoRio = Luz(layer, "ReflexoRio", new Vector3(-2.0f, 1.4f, 13.5f), 1.35f, 12f);
        reflexoRio.color = new Color(.46f, .62f, .76f);

        MontarNevoa(layer, posteA, posteB, true);
        MontarChuva(layer, 18f);
    }

    private void MontarCemiterioFrente(int layer)
    {
        var terra = Material("CaminhoCemiterio", "Art/Tiles/Terra", new Color(.48f, .40f, .31f), new Vector2(16f, 4f), .82f);
        var grama = Material("GramaCemiterio", "Art/Tiles/Grama", new Color(.26f, .34f, .24f), new Vector2(20f, 14f));
        var pedra = Material("PedraTumulo", "Art/Tiles/Pedra", new Color(.44f, .44f, .42f), new Vector2(4f, 2f), .28f);

        Piso(layer, "CaminhoAbrigo", 0f, 2.6f, -.04f, terra);
        Piso(layer, "CampoCemiterio", 2.6f, 26f, -.06f, grama);

        Billboard(layer, "VelasAbrigoExt", "Art/Cap2/Remodelagem/Cemiterio/Velas", new Vector3(-1.85f, -.04f, .75f), .58f, new Color(1f, .88f, .68f));
        Billboard(layer, "VasoAbrigoExt", "Art/Cap2/Remodelagem/Cemiterio/Vaso", new Vector3(2.05f, -.04f, .80f), .68f, new Color(.84f, .84f, .80f));

        // Sepulturas em profundidade
        for (int row = 0; row < 3; row++)
        for (int col = -2; col <= 2; col++)
        {
            if (col == 0 && row == 0) continue;
            float x = col * 2.9f + (row % 2 == 0 ? .35f : -.35f);
            float z = 4.6f + row * 3.8f;
            Objeto(layer, PrimitiveType.Cube, "BaseSepultura", new Vector3(x, .04f, z), new Vector3(1.1f, .16f, 1.8f), pedra);
            string tex = (row + col) % 2 == 0 ? "Art/Cap2/Remodelagem/Cemiterio/Lapide" : "Art/Cap2/Remodelagem/Cemiterio/Cruz";
            Billboard(layer, "MarcoSepultura", tex, new Vector3(x, .12f, z + .65f), tex.EndsWith("Cruz") ? .92f : 1.05f, new Color(.78f, .82f, .84f));
        }

        // Cerca e árvores sombrias
        for (float x = -10f; x <= 10f; x += 2.5f)
            Billboard(layer, "GradeCemiterio", "Art/Cap2/Remodelagem/Cemiterio/Grade", new Vector3(x, -.05f, 16.0f), 1.25f, new Color(.70f, .74f, .76f));

        var rng = new System.Random(109);
        for (int i = -4; i <= 4; i++)
            Billboard(layer, "ArvoreCemiterio", "Art/RuaNorte/Vegetacao/" + (i % 2 == 0 ? "Araucaria" : "Camboata"),
                new Vector3(i * 4.1f + (float)rng.NextDouble(), -.06f, 17.8f + (float)rng.NextDouble() * 3.2f),
                4.8f + (float)rng.NextDouble() * 2.0f, new Color(.68f, .76f, .76f));

        var luar = Luz(layer, "LuarCemiterio", new Vector3(0f, 5.2f, 9f), 1.85f, 22f);
        luar.color = new Color(.48f, .62f, .80f);
        var velaLuz = Luz(layer, "LuzVelasAbrigo", new Vector3(-1.85f, .65f, .75f), .85f, 3.6f);
        velaLuz.color = new Color(1f, .72f, .38f);

        MontarNevoa(layer, new Vector3(-4f, 3.8f, 8f), new Vector3(4f, 3.8f, 8f), true);
        MontarChuva(layer, 17f);
    }

    private void MontarCemiterioFundos(int layer)
    {
        MontarMatagalCercado(layer, 110, comMuro: false, zMuro: 5.5f);
        var pedra = Material("PedraCemFundos", "Art/Tiles/Pedra", new Color(.42f, .42f, .40f), new Vector2(3f, 2f), .25f);
        Objeto(layer, PrimitiveType.Cube, "SepulturaFundo", new Vector3(-1.6f, .04f, 3.8f), new Vector3(1.0f, .16f, 1.6f), pedra);
        Billboard(layer, "CruzFundo", "Art/Cap2/Remodelagem/Cemiterio/Cruz", new Vector3(-1.6f, .12f, 4.4f), .95f, new Color(.74f, .80f, .82f));
        Billboard(layer, "LapideFundo", "Art/Cap2/Remodelagem/Cemiterio/Lapide", new Vector3(1.9f, -.05f, 4.2f), 1.05f, new Color(.74f, .80f, .82f));
    }

    private void MontarMatagalCercado(int layer, int seed, bool comMuro, float zMuro)
    {
        var grama = Material("GramaFundos", "Art/Tiles/Grama", new Color(.28f, .37f, .24f), new Vector2(20f, 14f));
        Piso(layer, "CampoFundos", 0f, 24f, -.05f, grama);

        for (float x = -9f; x <= 9f; x += 2.6f)
            Billboard(layer, comMuro ? "MuretaFundos" : "GradeFundos",
                comMuro ? "Art/Cap2/Remodelagem/Matriz/MuretaPedra" : "Art/Cap2/Remodelagem/Cemiterio/Grade",
                new Vector3(x, -.05f, zMuro), 1.15f, new Color(.74f, .76f, .76f));

        var rng = new System.Random(seed);
        for (int fileira = 0; fileira < 2; fileira++)
        for (int i = -5; i <= 5; i++)
        {
            float x = i * 3.1f + (float)rng.NextDouble() * 1.4f - .7f;
            float z = zMuro + 1.2f + fileira * 3.8f + (float)rng.NextDouble() * 1.6f;
            if (fileira == 0)
                Billboard(layer, "MoitaFundos", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x, -.05f, z - .6f), .90f + (float)rng.NextDouble() * .55f, new Color(.76f, .84f, .68f));
            Billboard(layer, "ArvoreFundos", "Art/RuaNorte/Vegetacao/" + CopasCentro[rng.Next(CopasCentro.Length)],
                new Vector3(x, -.05f, z + .8f), 4.2f + (float)rng.NextDouble() * 2.2f, new Color(.72f, .82f, .72f));
        }

        var luar = Luz(layer, "LuarFundos", new Vector3(0f, 4.8f, 6.5f), 1.8f, 18f);
        luar.color = new Color(.48f, .62f, .84f);

        MontarNevoa(layer, new Vector3(-4f, 3.8f, 6f), new Vector3(4f, 3.8f, 6f), true);
        MontarChuva(layer, 15f);
    }
}
