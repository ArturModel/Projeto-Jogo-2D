using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed partial class SantaRitaRuaNorte
{
    private static readonly (float x, float y, float rx, float ry, float tilt, float blue, float shine)[] PocasDoBairro =
    {
        (19.35f, 3.9f, .44f, .89f, -.18f, .45f, .95f),
        (20.78f, 6.5f, .29f, .66f, .06f, .63f, .81f),
        (10.85f, 9.3f, 1.05f, .33f, .05f, .52f, .89f),
        (16.1f, 9.8f, .64f, .29f, -.18f, .66f, .74f),
        (24.4f, 9.05f, .91f, .35f, .12f, .39f, .90f),
        (25.62f, 11.26f, .28f, .56f, .52f, .49f, .92f),
        (31.85f, 9.95f, .87f, .28f, -.12f, .62f, .85f),
        (19.95f, 13.18f, .39f, .71f, -.16f, .53f, .86f),
        (17.82f, 17.35f, .46f, .87f, .07f, .74f, .81f),
        (12.75f, 21.55f, .92f, .38f, -.14f, .54f, .82f),
        (22.85f, 21.73f, 1.18f, .37f, -.07f, .52f, .98f),
        (28.83f, 20.95f, .69f, .24f, .15f, .54f, .84f),
        (34.5f, 24.87f, .55f, 1.12f, .09f, .39f, .88f),
        (36.26f, 29.07f, .43f, .75f, -.16f, .54f, .86f),
        (34.83f, 31.91f, .28f, .71f, .03f, .56f, .88f),
    };

    private static void SelectBairroGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        flipX = false;
        flipY = false;
        bool patioSul = y >= 11 && y <= 12 && x >= 6 && x <= 33;
        bool patioNorte = y == 23 && x >= 6 && x <= 41;
        if (patioSul || patioNorte)
        {
            kind = (x == 12 || x == 26) && y != 12 ? Ground.Drain : Ground.Walk;
            return;
        }
        if (y == 13 && x >= 6 && x <= 33 || y == 24 && x >= 6 && x <= 41)
        {
            kind = Ground.Verge;
            return;
        }
        kind = Ground.Grass;
    }

    private void DressBairro()
    {
        casterCount = 0;
        DressProp("Predio_CasaDoAvo_40", "Bairro/Avo", new Vector3(6.35f, 2.0f, 3.0f), heightFactor: .86f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_14", "Bairro/Lurdes", new Vector3(5.20f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_22", "Bairro/Casa22", new Vector3(5.25f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_32", "Bairro/Casa32", new Vector3(5.25f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_48", "Bairro/Casa48", new Vector3(5.15f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Van_Garagem_Casa14", "Bairro/Van", new Vector3(3.3f, .95f, 1.74f), sombra: Sombra.Veiculo);
        DressProp("Caixotes_Casa22", "Caixotes", new Vector3(.96f, .64f, .89f), sombra: Sombra.Contato);
        DressProp("CaixaCorreio_40_Sprite", null, new Vector3(.56f, .42f, 1.13f), sombra: Sombra.Contato);
        DressProp("Seta_Subida_BarZe", null, new Vector3(.34f, .26f, 1.36f), sombra: Sombra.Contato);
        DressProp("NPC_SeuRamiro_Bairro", null, new Vector3(.42f, .35f, 1.49f), sombra: Sombra.Contato);
        foreach (string fence in new[] { "Cerca_Casa14", "Cerca_Casa22", "Cerca_Casa32", "Cerca_Avo_Esq", "Cerca_Avo_Dir" })
            DressProp(fence, null, new Vector3(3.25f, .39f, .92f), sombra: Sombra.Contato);
        for (int i = 1; i <= 5; i++)
            DressProp("Poste_Z4_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));

        for (int i = 0; i < 6; i++)
            DressTree("Arvore_Sul_Z4_" + i, i % 2 == 0 ? "Pitangueira" : "Araucaria", .86f + i * .026f);
        for (int i = 0; i < 4; i++)
            DressTree("Arvore_Meio_Z4_" + i, i % 2 == 0 ? "Bracatinga" : "Arvore", .94f + i * .034f);
        for (int i = 0; i < 7; i++)
            DressTree("Arvore_Norte_Z4_" + i, i % 2 == 0 ? "Araucaria" : "Arvore", .91f + i * .021f);
        PlantUnderstory("Moita_Curva_Sul", new Vector2(416.7f, 5.6f), .81f, false);
        PlantUnderstory("Moita_Subida_Central", new Vector2(421.0f, 17.8f), .65f, true);
        PlantUnderstory("Moita_Cerca_Avo", new Vector2(419.8f, 24.25f), .6f, false);
        PlantUnderstory("Moita_Subida_Bar", new Vector2(432.6f, 28.6f), .85f, true);
        PlantUnderstory("Moita_Fundos", new Vector2(427.7f, 27.45f), .80f, false);

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z4_1", .66f, -.42f, 3.18f, 8.5f),
            Lamp("Poste_Z4_3", .66f, -.42f, 3.18f, 8.8f),
            Lamp("Poste_Z4_4", .66f, -.42f, 3.18f, 8.7f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.95f, 1.50f, .94f, 1),
            new Vector4(1.99f, 1.53f, .97f, 1),
            new Vector4(1.94f, 1.43f, .89f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z4_2", .66f, -.42f, 3.18f, 8.15f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z4_5", .66f, -.42f, 3.18f, 7.95f));
        lighting.SetFloat("_DoorReflection", 0);
        UpdateCasters(false);
    }
}
