using UnityEngine;

// Local, irregular intensity dips on secondary fixtures. The light stays on.
internal sealed class LuzInstavel : MonoBehaviour
{
    private Light fonte;
    private float intensidade, profundidade, fase, tempo;

    internal static void Instalar(Light fonte, float profundidade, float fase)
    {
        if (!Application.isPlaying) return;
        var efeito = fonte.gameObject.AddComponent<LuzInstavel>();
        efeito.fonte = fonte;
        efeito.intensidade = fonte.intensity;
        efeito.profundidade = profundidade;
        efeito.fase = fase;
    }

    private void Update()
    {
        if (MenuPausa.ReduzirEfeitosIntensos)
        {
            fonte.intensity = intensidade;
            return;
        }

        if (MenuPausa.Aberto || !Application.isFocused) return;
        tempo += Time.deltaTime;
        float envelope = Mathf.SmoothStep(0f, 1f,
            Mathf.InverseLerp(.51f, .74f, Mathf.PerlinNoise(fase, tempo * .55f)));
        float oscilacao = Mathf.PerlinNoise(fase + 19f, tempo * 5f);
        fonte.intensity = intensidade * (1f - profundidade * envelope * oscilacao);
    }
}
