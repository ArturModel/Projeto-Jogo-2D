using UnityEngine;

internal sealed partial class Posto3D
{
    private Transform sino, helice, chaveDaSala, pendulo, portaFria;
    private Light luzDaSala;
    private SantaRitaInteriorAudio motorVentilador;
    private Collider alvoVentilador, alvoGeladeira, alvoInterruptor;
    private bool sinoTocou, ventiladorLigado = true, geladeiraAberta;
    private float tempoSino = 2f, velocidadeHelice = 1f, aberturaFria, tempoPendulo;

    private void MontarSino(Material latao)
    {
        sino = Pivo("Sininho da entrada", cenario, V(.31f, 2.12f, Soleira + .15f));
        Caixa("Suporte do sino", V(.31f, 2.15f, Soleira + .07f), V(.040f, .054f, .20f), latao, solida: false);
        Cilindro("Copa do sino", V(0, -.043f, 0), V(.086f, .042f, .086f), latao, sino);
        Cilindro("Boca do sino", V(0, -.081f, 0), V(.118f, .014f, .118f), latao, sino);
        Cilindro("Badalo", V(0, -.10f, 0), V(.026f, .020f, .026f), latao, sino);
    }

    private void SoarSino()
    {
        if (sino == null || sinoTocou) return;
        sinoTocou = true;
        tempoSino = 0f;
        SantaRitaInteriorAudio.Tocar("SinoPorta", sino.position, vista.transform, .88f,
            interior == Interior.Posto ? 1.06f : .95f);
    }

    private void MontarVentilador()
    {
        var esmalte = Superficie("Esmalte do ventilador", new Color(.30f, .35f, .30f));
        var metal = Superficie("Grade do ventilador", new Color(.45f, .46f, .39f));
        var suporte = Pivo("Ventilador de parede", cenario, V(2.28f, 2.21f, 4.21f));
        Caixa("Base aparafusada", V(0, -.20f, .08f), V(.16f, .22f, .16f), esmalte, suporte, false);
        helice = Pivo("Hélice", suporte, V(0, 0, -.07f));
        for (int i = 0; i < 3; i++)
        {
            var pa = Pivo("Pá", helice, Vector3.zero);
            pa.localRotation = Quaternion.Euler(0, 0, i * 120f);
            Caixa("Pá de metal", V(0, .117f, 0), V(.075f, .20f, .022f), esmalte, pa, false);
        }
        for (int i = 0; i < 12; i++)
        {
            var raio = Pivo("Grade", suporte, V(0, 0, -.125f));
            raio.localRotation = Quaternion.Euler(0, 0, i * 30f);
            Caixa("Raio da grade", V(0, .116f, 0), V(.009f, .232f, .011f), metal, raio, false);
            Caixa("Aro da grade", V(0, .233f, 0), V(.126f, .015f, .018f), metal, raio, false);
        }
        Cilindro("Cubo do ventilador", V(0, 0, -.15f), V(.09f, .025f, .09f), esmalte, suporte).Rotate(90, 0, 0);
        alvoVentilador = AlvoFisico(suporte, "Alvo do ventilador", V(0, 0, -.13f), V(.53f, .53f, .13f));
        motorVentilador = SantaRitaInteriorAudio.Criar(transform, suporte, vista.transform, "MotorVentilador", 6.5f, .61f);
    }

    private void MontarGeladeiraMercadinho(Material esmalte, Material metal)
    {
        const float x = 4.99f, z = -2.63f;
        Caixa("Fundo da geladeira", V(x + .43f, 1.07f, z), V(.12f, 2.14f, 1.36f), esmalte);
        foreach (float lado in new[] { -1f, 1f })
        {
            Caixa("Lateral da geladeira", V(x, 1.07f, z + lado * .625f), V(.89f, 2.14f, .11f), esmalte);
            Caixa("Travessa da geladeira", V(x, 1.07f + lado * .995f, z), V(.89f, .15f, 1.25f), esmalte);
        }
        var volume = Caixa("Volume da geladeira", V(x + .04f, 1.07f, z), V(.83f, 2.14f, 1.22f), esmalte);
        volume.GetComponent<MeshRenderer>().enabled = false;
        var bebidas = new[]
        {
            Superficie("Vidro verde das garrafas", new Color(.21f, .42f, .26f)),
            Superficie("Vidro âmbar das garrafas", new Color(.49f, .26f, .11f)),
            Superficie("Rótulo claro", new Color(.74f, .72f, .58f))
        };
        for (int nivel = 0; nivel < 3; nivel++)
        {
            float y = .41f + .53f * nivel;
            Caixa("Prateleira fria", V(x, y, z), V(.78f, .027f, 1.20f), metal, solida: false);
            for (int i = 0; i < 6; i++)
            {
                float zi = z - .45f + .18f * i;
                var corpo = V(x - .19f, y + .12f, zi);
                var vidro = bebidas[(i + nivel) % 2];
                Cilindro("Garrafa", corpo, V(.092f, .107f, .092f), vidro, cenario);
                Cilindro("Gargalo", corpo + V(0, .125f, 0), V(.037f, .035f, .037f), vidro, cenario);
                Cilindro("Rótulo", corpo, V(.095f, .045f, .095f), bebidas[2], cenario);
            }
        }
        portaFria = Pivo("Dobradiça da geladeira", cenario, V(x - .49f, 0f, z - .64f));
        Caixa("Folha da geladeira", V(0, 1.07f, .64f), V(.075f, 2.10f, 1.28f), esmalte, portaFria, false);
        var arte = Arte("Painel da geladeira", "Art/Props/Geladeira", V(x - .532f, 1.07f, z), 1.96f);
        if (arte != null)
        {
            arte.rotation = Quaternion.Euler(0, 90f, 0);
            arte.SetParent(portaFria, true);
        }
        Caixa("Puxador da geladeira", V(-.07f, 1.15f, 1.15f), V(.095f, .30f, .05f), metal, portaFria, false);
        alvoGeladeira = AlvoFisico(portaFria, "Alvo da geladeira", V(0, 1.07f, .64f), V(.17f, 2.10f, 1.27f));
        var compressor = Pivo("Compressor da geladeira", cenario, V(x + .25f, .25f, z));
        SantaRitaInteriorAudio.Criar(transform, compressor, vista.transform, "Compressor", 5.4f, .70f);
    }

    private void MontarRelogioDaCasa(Material madeira, Material metal)
    {
        var relogio = Pivo("Relógio de pêndulo", cenario, V(-1.65f, 2.13f, 4.71f));
        var papel = Superficie("Mostrador marfim", new Color(.77f, .71f, .53f));
        Caixa("Caixa do relógio", Vector3.zero, V(.37f, .68f, .11f), madeira, relogio, false);
        var mostrador = Cilindro("Mostrador", V(0, .143f, -.073f), V(.30f, .016f, .30f), papel, relogio);
        mostrador.Rotate(90, 0, 0);
        for (int i = 0; i < 12; i++)
        {
            var marca = Pivo("Marca de hora", relogio, V(0, .143f, -.094f));
            marca.Rotate(0, 0, i * 30f);
            Caixa("Traço da hora", V(0, .124f, 0), V(.009f, .024f, .008f), metal, marca, false);
        }
        Caixa("Ponteiro de hora", V(-.031f, .161f, -.103f), V(.082f, .012f, .010f), metal, relogio, false).transform.Rotate(0, 0, -29f);
        Caixa("Ponteiro de minuto", V(.029f, .189f, -.107f), V(.013f, .11f, .010f), metal, relogio, false).transform.Rotate(0, 0, -31f);
        pendulo = Pivo("Pêndulo", relogio, V(0, -.012f, -.088f));
        Caixa("Haste do pêndulo", V(0, -.127f, 0), V(.014f, .25f, .017f), metal, pendulo, false);
        Cilindro("Peso do pêndulo", V(0, -.237f, 0), V(.114f, .017f, .114f), metal, pendulo).Rotate(90, 0, 0);
        SantaRitaInteriorAudio.Criar(transform, relogio, vista.transform, "Pendulo", 8.6f, .87f);
    }

    private void MontarInterruptorDaCasa(Material osso, Material ferro)
    {
        var placa = Pivo("Interruptor da sala", cenario, V(1.16f, 1.30f, Soleira + .11f));
        Caixa("Placa de baquelite", Vector3.zero, V(.137f, .21f, .038f), osso, placa, false);
        chaveDaSala = Caixa("Tecla da sala", V(0, 0, .034f), V(.053f, .094f, .027f), ferro, placa, false).transform;
        chaveDaSala.localRotation = Quaternion.Euler(16f, 0f, 0f);
        alvoInterruptor = AlvoFisico(placa, "Alvo do interruptor", Vector3.zero, V(.27f, .29f, .12f));
    }

    private string PromptFisico(Collider alvo)
    {
        if (alvo == null) return null;
        if (alvo == alvoVentilador) return ventiladorLigado ? "Desligar ventilador" : "Ligar ventilador";
        if (alvo == alvoGeladeira) return geladeiraAberta ? "Fechar geladeira" : "Abrir geladeira";
        if (alvo == alvoInterruptor) return luzDaSala.enabled ? "Apagar luz da sala" : "Acender luz da sala";
        return null;
    }

    private void AcionarObjeto(Collider alvo)
    {
        if (alvo == alvoVentilador)
        {
            ventiladorLigado = !ventiladorLigado;
            motorVentilador.Ligado = ventiladorLigado;
            SantaRitaInteriorAudio.Tocar("Interruptor", alvo.transform.position, vista.transform, .65f);
        }
        else if (alvo == alvoGeladeira)
        {
            geladeiraAberta = !geladeiraAberta;
            SantaRitaInteriorAudio.Tocar("VedacaoGeladeira", alvo.transform.position, vista.transform, .8f,
                geladeiraAberta ? 1f : .89f);
        }
        else if (alvo == alvoInterruptor)
        {
            luzDaSala.enabled = !luzDaSala.enabled;
            chaveDaSala.localRotation = Quaternion.Euler(luzDaSala.enabled ? 16f : -16f, 0f, 0f);
            SantaRitaInteriorAudio.Tocar("Interruptor", alvo.transform.position, vista.transform, .73f);
        }
    }

    private void AnimarDetalhes(float dt)
    {
        if (sino != null)
        {
            tempoSino += dt;
            float balanco = 20f * Mathf.Exp(-tempoSino * 3.8f) * Mathf.Sin(tempoSino * 25f);
            sino.localRotation = Quaternion.Euler(0f, 0f, balanco);
        }
        if (helice != null)
        {
            velocidadeHelice = Mathf.MoveTowards(velocidadeHelice, ventiladorLigado ? 1f : 0f, dt * .86f);
            helice.Rotate(0f, 0f, -238f * velocidadeHelice * dt);
        }
        if (portaFria != null)
        {
            aberturaFria = Mathf.MoveTowards(aberturaFria, geladeiraAberta ? 1f : 0f, dt * 2.4f);
            portaFria.localRotation = Quaternion.Euler(0, -78f * Mathf.SmoothStep(0f, 1f, aberturaFria), 0);
        }
        if (pendulo != null)
        {
            tempoPendulo += dt;
            pendulo.localRotation = Quaternion.Euler(0, 0, 12f * Mathf.Sin(tempoPendulo * Mathf.PI));
        }
    }

    private Transform Pivo(string nome, Transform pai, Vector3 posicao)
    {
        var pivo = new GameObject(nome).transform;
        pivo.SetParent(pai, false);
        pivo.localPosition = posicao;
        return pivo;
    }

    private Transform Cilindro(string nome, Vector3 posicao, Vector3 tamanho, Material material, Transform pai)
    {
        var objeto = Geometria(PrimitiveType.Cylinder, nome, posicao, tamanho, material, pai);
        objeto.GetComponent<Collider>().enabled = false;
        return objeto.transform;
    }

    private Collider AlvoFisico(Transform pai, string nome, Vector3 posicao, Vector3 tamanho)
    {
        var alvo = Pivo(nome, pai, posicao).gameObject;
        alvo.layer = Layer;
        var collider = alvo.AddComponent<BoxCollider>();
        collider.size = tamanho;
        collider.isTrigger = true;
        return collider;
    }
}
