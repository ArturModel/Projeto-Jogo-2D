using UnityEngine;
using UnityEngine.Rendering;
using Tipo = InteracaoCapitulo1.TipoInteracao;

internal sealed partial class Posto3D
{
    private Collider portaDeposito;
    private Capitulo2Portal acessoDeposito;

    private void ConstruirMercadinho()
    {
        var madeira = Resources.Load<Texture2D>("Art/Tiles/Madeira");
        var parede = Superficie("Reboco gasto da mercearia", new Color(.91f, .87f, .73f),
            SantaRitaBarLayout.NativeSurface(true).texture, new Vector2(6f, 1f));
        var teto = Superficie("Forro antigo", new Color(.75f, .71f, .59f), madeira, new Vector2(7f, 5f));
        var azul = Superficie("Esmalte azul do mercadinho", new Color(.27f, .40f, .56f), madeira);
        var azulClaro = Superficie("Chapa pintada de azul", new Color(.38f, .54f, .66f));
        var escura = Superficie("Ferragens", new Color(.18f, .22f, .24f));
        var pinho = Superficie("Pinho usado", new Color(.86f, .70f, .47f), madeira);
        var bege = Superficie("Papel e fórmica", new Color(.90f, .82f, .66f));
        var latao = Superficie("Latão antigo", new Color(.54f, .38f, .18f));
        var piso = Superficie("Ladrilho gasto da mercearia", Color.white,
            CriarCeramica(new Color(.67f, .56f, .43f), new Color(.37f, .32f, .28f)), new Vector2(7f, 5f));
        var azulejo = Superficie("Azulejo claro", Color.white,
            CriarCeramica(new Color(.87f, .86f, .76f), new Color(.56f, .55f, .46f)), new Vector2(7f, 1f));
        var tubo = Superficie("Fluorescente da loja", new Color(.96f, .96f, .85f), unlit: true);

        Caixa("Piso", V(0, -.065f, .23f), V(11.9f, .13f, 9.52f), piso);
        Caixa("Forro", V(0, 2.99f, .23f), V(11.9f, .11f, 9.52f), teto);
        Caixa("Parede dos fundos", V(0, 1.5f, 4.92f), V(11.9f, 3f, .16f), parede);
        Caixa("Azulejo dos fundos", V(0, .49f, 4.81f), V(11.8f, .98f, .06f), azulejo, solida: false);
        Caixa("Rodameio dos fundos", V(0, 1.01f, 4.76f), V(11.8f, .083f, .06f), azul, solida: false);
        foreach (float lado in new[] { -1f, 1f })
        {
            Caixa("Parede lateral", V(lado * 5.9f, 1.5f, .23f), V(.14f, 3f, 9.52f), parede);
            Caixa("Azulejos da lateral", V(lado * 5.79f, .49f, .23f), V(.06f, .98f, 9.35f), azulejo, solida: false);
            Caixa("Rodameio da lateral", V(lado * 5.74f, 1.01f, .23f), V(.06f, .083f, 9.38f), azul, solida: false);
            Caixa("Fachada de reboco", V(lado * 3.35f, 1.5f, Soleira), V(5.10f, 3f, .15f), parede);
            Caixa("Rodapé externo azul", V(lado * 3.35f, .39f, Soleira - .09f), V(5.09f, .78f, .05f), azul, solida: false);
            Caixa("Batente", V(lado * .81f, 1.18f, Soleira), V(.10f, 2.35f, .23f), azul);
        }
        Caixa("Verga", V(0, 2.68f, Soleira), V(1.66f, .64f, .18f), parede);
        Caixa("Soleira", V(0, -.016f, Soleira), V(1.60f, .042f, .40f), azulClaro, solida: false);
        Caixa("Calçada sob o toldo", V(0, -.075f, -5.66f), V(11.8f, .13f, 2.40f), piso);
        for (int i = 0; i < 14; i++)
        {
            var lona = Caixa("Faixa do toldo", V(-2.21f + i * .34f, 2.75f, -5.13f), V(.34f, .045f, 1.56f),
                i % 2 == 0 ? azulClaro : bege, solida: false);
            lona.transform.Rotate(-10f, 0f, 0f);
        }
        Caixa("Capacho", V(.17f, .015f, -3.72f), V(1.52f, .03f, .78f), escura, solida: false);
        ConstruirPorta(azul, pinho, latao);
        ConstruirChuva();
        ConstruirVitrinesMercadinho(azulClaro, azul);

        foreach (float x in new[] { -2.9f, 2.9f })
        foreach (float z in new[] { -1.8f, 2.5f })
        {
            Caixa("Chapa da luminária", V(x, 2.86f, z), V(1.26f, .09f, .29f), escura, solida: false);
            Caixa("Fluorescente", V(x, 2.795f, z), V(1.17f, .041f, .17f), tubo, solida: false);
            Luz("Luz da mercearia", V(x, 2.66f, z), new Color(1f, .92f, .72f), 4.25f, 6f);
        }
        Luz("Luz de atendimento", V(-.8f, 2.55f, 2.62f), new Color(1f, .76f, .48f), 2.4f, 4.6f);
        Luz("Luz do toldo", V(.28f, 2.52f, -5.46f), new Color(.82f, .88f, .76f), 3.5f, 4.2f);

        Caixa("Balcão de madeira", V(-.50f, .51f, 2.61f), V(3.85f, 1.02f, .82f), azul);
        Caixa("Tampo de fórmica", V(-.50f, 1.055f, 2.61f), V(3.97f, .075f, .91f), pinho);
        for (int i = 0; i < 4; i++)
            Caixa("Almofada do balcão", V(-1.95f + .95f * i, .52f, 2.185f), V(.76f, .54f, .055f), pinho, solida: false);
        Caixa("Rodapé de madeira", V(-.50f, .08f, 2.175f), V(3.85f, .13f, .057f), escura, solida: false);
        Caixa("Caderno de fiados", V(-1.85f, 1.132f, 2.44f), V(.57f, .071f, .38f), bege, tipo: Tipo.CadernoFiadoMercadinho);
        for (int i = 0; i < 6; i++)
            Caixa("Pauta do caderno", V(-1.85f, 1.17f, 2.30f + .047f * i), V(.44f, .004f, .007f), azulClaro, solida: false);
        Arte("Rádio antigo do caixa", "Art/Props/RadioAntigo", V(.77f, 1.32f, 2.37f), .46f);
        Caixa("Registradora antiga", V(.22f, 1.17f, 2.74f), V(.55f, .155f, .39f), escura);
        Caixa("Teclas da registradora", V(.22f, 1.25f, 2.67f), V(.45f, .044f, .20f), bege, solida: false);
        Personagem("Atendente", "Art/Characters/Npc_Atendente", V(-.83f, 0f, 3.78f), Tipo.NpcAtendenteMercadinho);
        Letreiro("Placa da mercearia", "MERCADINHO SANTA RITA", V(-.72f, 2.29f, 4.78f), azul, 3.34f, .51f);
        Arte("Relógio de parede", "Cap1/WallClock", V(-3.52f, 2.28f, 4.77f), .39f);
        Arte("Caixas de entrega", "Art/Props/CargasMercado", V(-3.67f, .62f, 4.10f), 1.25f);

        Prateleira(V(4.65f, 0f, 2.16f), azul, pinho, bege, azulClaro);
        Prateleira(V(4.65f, 0f, -.18f), azul, pinho, bege, azulClaro);
        Caixa("Frente dos secos e suprimentos", V(4.35f, .98f, -.18f), V(.13f, 1.78f, 1.88f), azul,
            tipo: Tipo.PrateleirasMercadinho).GetComponent<MeshRenderer>().enabled = false;
        Caixa("Geladeira de bebidas", V(4.99f, 1.06f, -2.63f), V(.99f, 2.12f, 1.36f), bege);
        var geladeira = Arte("Frente da geladeira", "Art/Props/Geladeira", V(4.475f, 1.06f, -2.63f), 1.96f);
        if (geladeira != null) geladeira.Rotate(0f, 90f, 0f);
        Arte("Engradado de reposição", "Cap1/BeerCrate_Orange", V(4.25f, .28f, -3.69f), .58f);
        ConstruirHortifruti(pinho, azul, bege);
        ConstruirPortaDeposito(azul, azulClaro, latao);
    }

    private void ConstruirVitrinesMercadinho(Material caixilho, Material acabamento)
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/JanelaMolhada");
        var atlas = Resources.Load<Texture2D>("Art/Sinuca/JanelasLateraisExterior");
        if (shader == null || atlas == null) return;
        var vidro = new Material(shader) { mainTexture = atlas };
        recursos.Add(vidro);
        foreach (float x in new[] { -3.0f, 3.0f })
        {
            Caixa("Caixilho da vitrine", V(x, 1.65f, -4.385f), V(2.72f, 1.46f, .08f), caixilho, solida: false);
            var janela = Quadro("Vidro molhado da vitrine", V(x, 1.65f, -4.33f), V(2.58f, 1.31f, 1f), vidro);
            janela.Rotate(0f, 180f, 0f);
            Caixa("Travessa da vitrine", V(x, 1.65f, -4.29f), V(2.64f, .046f, .045f), acabamento, solida: false);
            Caixa("Montante da vitrine", V(x, 1.65f, -4.29f), V(.046f, 1.34f, .045f), acabamento, solida: false);
        }
    }

    private void ConstruirHortifruti(Material pinho, Material azul, Material bege)
    {
        var fruta = new[]
        {
            Superficie("Laranja", new Color(.92f, .55f, .16f)),
            Superficie("Maçã", new Color(.69f, .19f, .13f)),
            Superficie("Limão", new Color(.42f, .61f, .22f)),
            Superficie("Batata", new Color(.64f, .48f, .29f))
        };
        Caixa("Ilha de hortifruti", V(-3.92f, .47f, -.34f), V(1.71f, .94f, 2.75f), azul);
        Caixa("Borda da ilha", V(-3.92f, .97f, -.34f), V(1.80f, .075f, 2.84f), pinho);
        for (int tipo = 0; tipo < fruta.Length; tipo++)
        {
            float x = -4.34f + tipo % 2 * .83f;
            float z = -.99f + tipo / 2 * 1.30f;
            Caixa("Caixote de hortifruti", V(x, 1.017f, z), V(.74f, .064f, 1.18f), pinho, solida: false);
            for (int i = 0; i < 14; i++)
            {
                var pos = V(x - .20f + (i % 3) * .19f + (i % 2) * .02f, 1.11f + (i % 3) * .014f,
                    z - .43f + (i / 3) * .20f);
                var item = Geometria(PrimitiveType.Sphere, "Fruta da banca", pos, V(.16f, .15f, .16f), fruta[tipo], cenario);
                item.GetComponent<Collider>().enabled = false;
                item.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            }
        }
        Letreiro("Placa do hortifruti", "HORTIFRÚTI", V(-3.85f, 1.51f, .93f), azul, 1.03f, .29f, 70f);
        Caixa("Etiqueta dos preços", V(-3.34f, .97f, .88f), V(.24f, .015f, .19f), bege, solida: false);
    }

    private void ConstruirPortaDeposito(Material azul, Material caixilho, Material latao)
    {
        const float x = 3.67f;
        portaDeposito = Caixa("Porta de serviço / depósito", V(x, 1.08f, 4.76f), V(1.09f, 2.16f, .14f), azul)
            .GetComponent<Collider>();
        foreach (float lado in new[] { -1f, 1f })
            Caixa("Batente do depósito", V(x + lado * .56f, 1.10f, 4.66f), V(.09f, 2.21f, .13f), caixilho, solida: false);
        Caixa("Puxador do depósito", V(x + .38f, 1.07f, 4.64f), V(.045f, .19f, .072f), latao, solida: false);
        Letreiro("Placa do depósito", "DEPÓSITO", V(x, 2.39f, 4.70f), azul, 1.10f, .31f);
        acessoDeposito = portaDeposito.gameObject.AddComponent<Capitulo2Portal>();
        acessoDeposito.enabled = false;
        acessoDeposito.cenaDestino = "MercadinhoDeposito";
        acessoDeposito.entradaDestino = "loja";
        acessoDeposito.flagNecessaria = Flags.ZeDerrotadoNaSinuca;
        acessoDeposito.avisoBloqueio = "A porta de serviço está fechada pelo lado do Centro. Preciso resolver a passagem da Rua Norte com Zé.";
    }
}
