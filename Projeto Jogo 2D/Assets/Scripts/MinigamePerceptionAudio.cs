using UnityEngine;

internal static class MinigamePerceptionAudio
{
    internal static AudioClip CriarCoracao(string nome)
    {
        const int hz = 22050;
        var amostras = new float[Mathf.RoundToInt(.34f * hz)];
        for (int i = 0; i < amostras.Length; i++)
        {
            float t = i / (float)hz;
            amostras[i] = PulsoCardiaco(t, 64) + .58f * PulsoCardiaco(t - .13f, 53);
        }
        var clip = AudioClip.Create(nome, amostras.Length, 1, hz, false);
        clip.SetData(amostras, 0);
        return clip;
    }

    private static float PulsoCardiaco(float t, float hz)
    {
        if (t < 0 || t > .17f) return 0;
        float envelope = Mathf.SmoothStep(0, 1, t / .013f) *
            (1 - Mathf.SmoothStep(0, 1, (t - .08f) / .09f)) * Mathf.Exp(-t * 22);
        return (.75f * Mathf.Sin(2 * Mathf.PI * hz * t) + .20f * Mathf.Sin(4 * Mathf.PI * hz * t)) * envelope;
    }
}
