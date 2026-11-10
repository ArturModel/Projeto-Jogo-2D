using UnityEngine;

internal sealed partial class Capitulo2Exterior
{
    private void ComporDetalhesGeradosDaPraca()
    {
        const string detalhes = Art + "PracaDecoracao/";

        // Pequenas manchas de flor sobre os quatro canteiros preservam o eixo livre da praça.
        (string sprite, float x, float y, float largura)[] flores =
        {
            ("FloresCreme", 40.05f, 45.74f, .56f),
            ("FloresAmarelas", 51.95f, 45.74f, .56f),
            ("FloresLilas", 40.05f, 56.74f, .56f),
            ("FloresCreme", 51.95f, 56.74f, .56f)
        };

        foreach (var (sprite, x, y, largura) in flores)
        {
            var flor = PecaPraca("Flores ornamentais da praça", detalhes + sprite, x, y, largura);
            if (flor != null)
                ConfigurarSpriteVolumetricoPraca(flor, y - .18f, .76f, .42f, 0f, 0f, modoOclusor: -1);
        }

        // Folhas pequenas junto às bordas de pedra, longe do caminho central e das entradas.
        (string sprite, float x, float y, float largura, bool inverter)[] folhas =
        {
            ("FolhicoA", 35.18f, 43.35f, .48f, false),
            ("FolhicoB", 56.82f, 43.35f, .40f, true),
            ("FolhicoB", 35.18f, 59.82f, .39f, false),
            ("FolhicoA", 56.82f, 59.82f, .45f, true),
            ("FolhicoA", 34.72f, 51.68f, .36f, true),
            ("FolhicoB", 57.28f, 51.68f, .33f, false)
        };

        foreach (var (sprite, x, y, largura, inverter) in folhas)
        {
            var folhico = PecaPraca("Folhas levadas pela chuva", detalhes + sprite, x, y, largura, noPiso: true);
            if (folhico != null)
                folhico.flipX = inverter;
        }
    }
}
