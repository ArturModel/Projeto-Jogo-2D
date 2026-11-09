using UnityEngine;

internal sealed partial class Capitulo2Interior3D
{
    private static readonly Color CivicWood = new(.27f, .155f, .094f);
    private static readonly Color CivicDarkWood = new(.135f, .081f, .056f);
    private static readonly Color CivicBrass = new(.47f, .34f, .17f);
    private static readonly Color CivicPaper = new(.76f, .69f, .53f);
    private static readonly Color CivicNightGlass = new(.112f, .177f, .213f);

    private void MontarIgreja()
    {
        var plaster = new Color(.57f, .48f, .38f);
        var stone = new Color(.43f, .40f, .35f);
        CivicEnvelope(18, 20, 9, 1, 4.45f, plaster, CivicDarkWood);
        CivicTiles(18, 20, "PisoCreme", stone);
        CivicBox("Passadeira da nave", 9, 10.05f, 2.10f, .025f, 15.30f, .015f, new Color(.29f, .086f, .071f));
        foreach (float edge in new[] { 8.05f, 9.95f })
            CivicBox("Friso bordado", edge, 10.05f, .034f, .010f, 15.18f, .038f, CivicBrass);

        foreach (float x in new[] { 4.5f, 13.5f })
        foreach (float z in new[] { 6f, 9.5f, 13f })
            CivicBench(x, z + .2f, 3.02f, .42f, CivicWood, true);

        CivicBox("Degrau do presbitério", 9, 17.92f, 10.84f, .12f, 1.90f, .012f, stone);
        CivicBox("Nosing do degrau", 9, 17.00f, 10.88f, .055f, .09f, .08f, new Color(.54f, .47f, .35f));
        CivicBox("Soco do altar", 9, 18.80f, 3.62f, .17f, .40f, .13f, CivicDarkWood);
        CivicBox("Frente do altar", 9, 18.80f, 3.34f, .70f, .37f, .30f, CivicWood);
        CivicBox("Linho do altar", 9, 18.80f, 3.69f, .07f, .48f, 1.0f, new Color(.79f, .72f, .57f));
        CivicBox("Orla do linho", 9, 18.54f, 3.57f, .23f, .025f, .84f, new Color(.74f, .67f, .50f));
        CivicBox("Haste da cruz", 9, 19.72f, .15f, 1.72f, .11f, 2.10f, CivicDarkWood);
        CivicBox("Braço da cruz", 9, 19.71f, .95f, .15f, .12f, 3.16f, CivicDarkWood);

        CivicBox("Apoio de pedra para as velas", 2.70f, 17.81f, 1.45f, .75f, .40f, .02f, new Color(.31f, .265f, .225f));
        CivicBox("Bandeja de ferro", 2.70f, 17.82f, 1.55f, .06f, .45f, .77f, new Color(.15f, .125f, .105f));
        CivicBox("velas_igreja", 2.70f, 17.80f, .12f, .25f, .11f, .84f, CivicPaper);
        for (int i = 0; i < 9; i++)
        {
            float x = 2.13f + i % 5 * .245f;
            float z = 17.70f + i / 5 * .23f;
            float h = .10f + (i * 7 % 5) * .048f;
            CivicBox("Vela de cera", x, z, .095f, h, .09f, .84f, CivicPaper);
            if (i % 3 == 1)
                CivicBox("Chama das velas", x, z, .025f, .048f, .025f, .84f + h, new Color(1f, .64f, .22f));
        }
        Lamp("Clarão das velas", new Vector2(2.71f, 17.68f), 1.17f, new Color(1f, .62f, .29f), 1.55f, 3.0f);
        CivicTable("Mesa da sacristia", 14.9f, 17.50f, 1.58f, .41f, .86f, CivicWood);
        CivicLedger("Livro da sacristia", 14.64f, 17.43f, .88f, .42f, .25f);
        CivicBoard("Mural comunitário", 14.8f, 4.20f, 1.49f, .97f, .86f, CivicDarkWood);
        CivicBox("foto_igreja", 14.89f, 4.115f, .52f, .40f, .018f, 1.20f, new Color(.66f, .58f, .42f));
        CivicBox("Fotografia comunitária", 14.89f, 4.098f, .43f, .29f, .017f, 1.26f, new Color(.33f, .31f, .26f));
        CivicBox("Passe-partout da fotografia", 14.89f, 4.085f, .37f, .022f, .018f, 1.27f, CivicPaper);
        CivicWindow(2.3f, 19.80f, 1.60f, 2.42f, 1.36f, CivicDarkWood, new Color(.19f, .28f, .36f));
        CivicWindow(15.7f, 19.80f, 1.60f, 2.42f, 1.36f, CivicDarkWood, new Color(.23f, .23f, .33f));

        foreach (float z in new[] { 5.15f, 10.15f, 15.35f })
            CivicFixture("Pendente da nave", 9, z, 3.32f, CivicBrass, new Color(1f, .75f, .49f), 2.6f, 8f);
        CivicFixture("Luz do altar", 9, 18.3f, 3.65f, CivicBrass, new Color(1f, .75f, .49f), 2.45f, 7f);
    }

    private void MontarPrefeitura()
    {
        var plaster = new Color(.62f, .59f, .46f);
        var olive = new Color(.25f, .31f, .264f);
        var darkOlive = new Color(.17f, .22f, .181f);
        CivicEnvelope(20, 15, 10, 1, 3.46f, plaster, olive);
        CivicTiles(20, 15, "PisoVerde", new Color(.40f, .44f, .37f));
        foreach (float x in new[] { 3.5f, 16.5f })
            CivicBench(x, 3.8f, 2.16f, .42f, CivicWood, false);
        CivicDesk("Balcão de consulta", 10, 6.80f, 2.28f, .50f, olive);
        CivicLedger("protocolo", 9.5f, 6.75f, 1.04f, .37f, .27f);
        CivicBox("Carimbo do protocolo", 9.13f, 6.83f, .08f, .10f, .08f, 1.04f, CivicDarkWood);
        CivicBox("Bandeja de fichas", 10.95f, 6.82f, .29f, .07f, .22f, 1.04f, CivicDarkWood);
        CivicBox("Fichas da bandeja", 10.95f, 6.82f, .22f, .025f, .15f, 1.109f, CivicPaper);
        CivicCabinet(3.1f, 8.30f, 1.25f, .40f, olive);
        CivicBoard("placa_praca", 3.1f, 8.04f, .95f, .53f, .73f, CivicWood);
        CivicBox("Placa de inauguração", 3.1f, 7.985f, .79f, .38f, .022f, .80f, CivicBrass);

        foreach (float x in new[] { 3.8f, 16.2f })
        {
            CivicBox("Divisória de consulta pública", x, 10, 7.6f, .94f, .44f, 0, olive);
            CivicBox("Arremate da divisória", x, 10, 7.60f, .075f, .48f, .93f, CivicWood);
            for (int i = -2; i <= 2; i++)
                CivicBox("Painel moldurado da divisória", x + i * 1.48f, 9.766f, 1.27f, .66f, .041f, .14f, darkOlive);
        }
        var gate = CivicBox("Folhas de acesso ao arquivo", 10, 10, 4.78f, .91f, .32f, 0, CivicWood);
        var binding = gate.AddComponent<Capitulo2PublicRecordsDoor3D>();
        binding.Sincronizar();

        CivicCabinet(3f, 13.51f, 1.30f, .42f, olive);
        CivicCabinet(17f, 13.51f, 1.30f, .42f, olive);
        CivicTable("Mesa de leitura pública", 10, 13.19f, 2.63f, .41f, .87f, CivicWood);
        CivicLedger("manutencao", 9.1f, 13.10f, .89f, .51f, .26f);
        CivicLedger("extrato_publico", 10.9f, 13.10f, .89f, .45f, .26f);
        CivicWindow(2.3f, 14.83f, 1.48f, 1.45f, 1.38f, darkOlive, CivicNightGlass);
        CivicWindow(17.7f, 14.83f, 1.48f, 1.45f, 1.38f, darkOlive, CivicNightGlass);

        CivicFixture("Luz da espera", 5.9f, 4.15f, 2.95f, olive, new Color(.98f, .83f, .62f), 2.25f, 7.5f);
        CivicFixture("Luz de protocolo", 10, 6.98f, 2.91f, olive, new Color(1f, .83f, .61f), 2.7f, 7.2f);
        CivicFixture("Luz da espera leste", 15.5f, 4.15f, 2.95f, olive, new Color(.98f, .83f, .62f), 2.25f, 7.5f);
        CivicFixture("Luz da consulta", 10, 12.90f, 2.91f, olive, new Color(.98f, .87f, .71f), 2.8f, 8.0f);
    }

    private void MontarRodoviaria()
    {
        var plaster = new Color(.55f, .56f, .50f);
        var stationBlue = new Color(.20f, .31f, .34f);
        var stationCream = new Color(.62f, .54f, .39f);
        var steel = new Color(.19f, .205f, .203f);
        CivicEnvelope(22, 13, 11, 1, 3.23f, plaster, stationBlue);
        CivicTiles(22, 13, "PisoCinza", new Color(.39f, .40f, .39f));
        CivicBox("Faixa no acesso à plataforma", 11, 2.25f, 2.1f, .021f, .20f, .013f, stationCream);
        foreach (float z in new[] { 4.2f, 8.2f })
            CivicBench(5, z, 2.61f, .43f, stationBlue, false);

        CivicDesk("Guichê de passagens", 16f, 9.50f, 2.48f, .42f, stationBlue);
        CivicBox("Perfil superior do guichê", 16, 9.60f, 2.59f, .10f, .12f, 2.12f, steel);
        foreach (float x in new[] { 14.79f, 17.21f })
            CivicBox("Montante do guichê", x, 9.58f, .10f, 1.15f, .12f, 1.05f, steel);
        CivicBox("Apoio de folhetos", 10.70f, 11.54f, 1.83f, .16f, .42f, .90f, stationBlue);
        CivicBoard("horarios", 10.70f, 11.61f, 1.73f, 1.08f, 1.04f, steel);
        CivicBox("Cabeçalho de linhas", 10.70f, 11.538f, 1.49f, .21f, .021f, 1.78f, stationBlue);
        for (int i = 0; i < 4; i++)
        {
            CivicBox("Tabela de partidas", 10.56f, 11.532f, 1.16f, .025f, .018f, 1.23f + i * .133f, CivicPaper);
            CivicBox("Coluna de horários", 11.27f, 11.530f, .14f, .025f, .018f, 1.23f + i * .133f, stationCream);
        }
        CivicLedger("Folhetos de ruas e linhas", 10.68f, 11.44f, 1.062f, .47f, .17f);
        CivicBox("Armário do telefone", 19.2f, 9.85f, .61f, .91f, .45f, .015f, stationBlue);
        CivicBox("Tampo do telefone", 19.2f, 9.84f, .70f, .07f, .50f, .91f, steel);
        var phone = SpriteProp("telefone_rodo", "Cap1/TelefoneForaDoGancho", new Vector2(19.2f, 9.74f), .46f);
        if (phone != null) phone.transform.position += Vector3.up * .98f;
        CivicBox("bagagem", 3.30f, 4.49f, .79f, .52f, .37f, .015f, new Color(.36f, .22f, .13f));
        foreach (float x in new[] { 3.10f, 3.50f })
        {
            CivicBox("Correia da mala", x, 4.298f, .065f, .46f, .023f, .048f, CivicDarkWood);
            CivicBox("Correia superior da mala", x, 4.49f, .065f, .013f, .37f, .533f, CivicDarkWood);
        }
        CivicBox("Alça da mala", 3.30f, 4.47f, .18f, .055f, .045f, .55f, CivicDarkWood);
        CivicWindow(2.3f, 12.83f, 1.80f, 1.18f, 1.33f, stationBlue, CivicNightGlass);
        CivicWindow(19.7f, 12.83f, 1.80f, 1.18f, 1.33f, stationBlue, CivicNightGlass);
        CivicFixture("Fluorescente da espera", 5.65f, 5.65f, 2.77f, steel, new Color(.81f, .92f, .89f), 2.4f, 8.8f);
        CivicFixture("Fluorescente dos horários", 10.70f, 10.5f, 2.76f, steel, new Color(.86f, .92f, .81f), 2.6f, 7.0f);
        CivicFixture("Fluorescente do guichê", 17.1f, 9.38f, 2.78f, steel, new Color(.87f, .94f, .84f), 2.45f, 8.0f);
    }

    private GameObject CivicBox(string name, float x, float z, float w, float h, float d, float y, Color color)
        => Box(name, new Vector2(x, z), new Vector3(w, h, d), y, color);

    private void CivicEnvelope(float width, float depth, float entrance, float halfDoor, float height, Color wall, Color baseboard)
    {
        CivicBox("Parede de fundo", width * .5f, depth, width, height, .33f, 0, wall);
        CivicBox("Lambril de fundo", width * .5f, depth - .17f, width, .83f, .09f, 0, baseboard);
        CivicBox("Friso horizontal", width * .5f, depth - .21f, width, .055f, .09f, .82f, baseboard * 1.23f);
        foreach (float x in new[] { .20f, width - .20f })
        {
            CivicBox("Parede lateral em corte", x, depth * .5f, .38f, .73f, depth, 0, wall);
            CivicBox("Rodapé lateral", x, depth * .5f, .41f, .09f, depth, 0, baseboard);
            CivicBox("Arremate da seção lateral", x, depth * .5f, .39f, .045f, depth, .73f, baseboard);
        }
        float left = entrance - halfDoor;
        float right = width - entrance - halfDoor;
        CivicBox("Seção de fachada oeste", left * .5f, .30f, left, .38f, .39f, 0, baseboard);
        CivicBox("Seção de fachada leste", width - right * .5f, .30f, right, .38f, .39f, 0, baseboard);
        CivicBox("Pedra da soleira", entrance, .41f, halfDoor * 2, .055f, .67f, 0, wall);
        foreach (float offset in new[] { -halfDoor, halfDoor })
            CivicBox("Ombreira da saída", entrance + offset, .42f, .16f, .63f, .36f, .018f, baseboard);
    }

    private void CivicTiles(float width, float depth, string tile, Color fallback)
    {
        var slab = CivicBox("Base do piso", width * .5f, depth * .5f, width, .15f, depth, -.154f, fallback);
        var tileTexture = Resources.Load<Texture2D>("Art/Tiles/" + tile);
        if (tileTexture == null) return;

        // O original fica em Clamp para os tiles 2D. Só a cópia deste piso usa repetição.
        var texture = Instantiate(tileTexture);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point;
        var renderer = slab.GetComponent<MeshRenderer>();
        var material = new Material(renderer.sharedMaterial) { color = new Color(.80f, .80f, .80f) };
        material.SetTexture("_BaseMap", texture);
        material.SetTextureScale("_BaseMap", new Vector2(width, depth));
        renderer.sharedMaterial = material;
        owned.Add(texture);
        owned.Add(material);
    }

    private void CivicBench(float x, float z, float width, float depth, Color paint, bool wood)
    {
        Color hardware = wood ? CivicDarkWood : new Color(.155f, .166f, .157f);
        foreach (float dx in new[] { -.41f, .41f })
        {
            CivicBox("Pé lateral do banco", x + dx * width, z, .13f, .46f, depth, 0, hardware);
            CivicBox("Montante do encosto", x + dx * width, z + depth * .39f, .095f, .42f, .09f, .44f, hardware);
        }
        for (int i = -1; i <= 1; i++)
            CivicBox("Ripa do assento", x, z + i * depth * .33f, width, .065f, depth * .30f, .435f, paint * (1 + i * .06f));
        for (int i = 0; i < 2; i++)
            CivicBox("Ripa do encosto", x, z + depth * .47f, width, .105f, .059f, .62f + i * .17f, paint);
    }

    private void CivicDesk(string name, float x, float z, float w, float d, Color color)
    {
        CivicBox(name, x, z, w, .90f, d, .075f, color);
        CivicBox("Tampo do atendimento", x, z, w + .12f, .065f, d + .13f, .975f, CivicWood);
        CivicBox("Soco do balcão", x, z, w - .09f, .09f, d - .03f, 0, CivicDarkWood);
        foreach (float offset in new[] { -.31f, 0f, .31f })
            CivicBox("Painel frontal do atendimento", x + w * offset, z - d * .5f - .014f, w * .26f, .62f, .022f, .20f, color * .83f);
    }

    private void CivicTable(string name, float x, float z, float width, float depth, float top, Color color)
    {
        CivicBox(name, x, z, width, .076f, depth, top - .076f, color);
        foreach (float dx in new[] { -.43f, .43f })
        foreach (float dz in new[] { -.36f, .36f })
            CivicBox("Perna da mesa", x + dx * width, z + dz * depth, .084f, top - .066f, .073f, 0, CivicDarkWood);
        CivicBox("Saia da mesa", x, z - depth * .38f, width * .93f, .15f, .054f, top - .22f, color * .89f);
    }

    private void CivicLedger(string id, float x, float z, float y, float width, float depth)
    {
        CivicBox(id, x, z, width, .023f, depth, y, CivicPaper);
        for (int i = 0; i < 4; i++)
        {
            float inset = i == 3 ? .65f : .79f;
            CivicBox("Linha de tinta no papel", x, z + (i - 1.5f) * depth * .17f,
                width * inset, .002f, .013f, y + .024f, new Color(.35f, .31f, .24f));
        }
    }

    private void CivicCabinet(float x, float z, float width, float depth, Color color)
    {
        CivicBox("Arquivo municipal", x, z, width, 1.38f, depth, .075f, color);
        CivicBox("Soco do arquivo", x, z, width * .95f, .075f, depth * .93f, 0, CivicDarkWood);
        for (int i = 0; i < 4; i++)
        {
            float y = .15f + i * .312f;
            CivicBox("Gaveta do arquivo", x, z - depth * .5f, width * .90f, .27f, .04f, y, color * .81f);
            CivicBox("Puxador de gaveta", x, z - depth * .5f - .036f, .20f, .038f, .048f, y + .11f, CivicBrass);
            CivicBox("Etiqueta de gaveta", x, z - depth * .5f - .024f, .18f, .054f, .017f, y + .167f, CivicPaper);
        }
    }

    private void CivicBoard(string name, float x, float z, float width, float height, float y, Color frame)
    {
        CivicBox(name, x, z, width, height, .10f, y, frame);
        CivicBox("Fundo do quadro", x, z - .051f, width - .11f, height - .11f, .017f, y + .055f, new Color(.22f, .195f, .161f));
        foreach (float side in new[] { -.43f, .43f })
            CivicBox("Pé do quadro", x + width * side, z + .025f, .10f, y, .16f, 0, frame);
    }

    private void CivicWindow(float x, float z, float width, float height, float y, Color frame, Color glass)
    {
        CivicBox("Aro da janela", x, z, width + .13f, height + .13f, .13f, y - .065f, frame);
        CivicBox("Vidraça noturna", x, z - .075f, width - .09f, height - .10f, .029f, y + .045f, glass);
        foreach (float dx in new[] { -.25f, .25f })
            CivicBox("Montante da janela", x + dx * width, z - .104f, .055f, height, .040f, y, frame);
        foreach (float dy in new[] { .34f, .69f })
            CivicBox("Travessa da janela", x, z - .106f, width, .055f, .047f, y + dy * height, frame);
        CivicBox("Peitoril da janela", x, z - .081f, width + .26f, .083f, .28f, y - .077f, frame);
    }

    private void CivicFixture(string name, float x, float z, float height, Color housing, Color glow, float intensity, float range)
    {
        CivicBox(name, x, z, .78f, .084f, .31f, height, housing);
        CivicBox("Difusor da luminária", x, z, .68f, .055f, .22f, height - .053f, glow);
        Lamp(name, new Vector2(x, z), height - .16f, glow, intensity, range);
    }
}

// A porta original mantém colisão e flag. Aqui muda apenas a folha renderizada.
public sealed class Capitulo2PublicRecordsDoor3D : MonoBehaviour
{
    private void OnEnable() => GameState.OnEstadoAlterado += Sincronizar;
    private void OnDisable() => GameState.OnEstadoAlterado -= Sincronizar;

    internal void Sincronizar()
    {
        if (TryGetComponent<Renderer>(out var mesh))
            mesh.enabled = !GameState.Tem("c2_prefeitura_atendimento");
    }
}

