using UnityEngine;

// Texturas abstratas sintetizadas no projeto: murmúrio sem palavras, água
// ressonante e chuva abafada. As faixas Suno do autor não são modificadas.
public sealed class SantaRitaBarAudio : MonoBehaviour
{
    private static SantaRitaBarAudio instance;
    private readonly AudioSource[] layers = new AudioSource[3];
    private bool inside, paused;
    private int hand;

    public static void DefinirContexto(bool noBar, int mao)
    {
        if (instance == null)
        {
            if (!noBar) return;
            var go = new GameObject("AmbienciaNarrativaBar");
            DontDestroyOnLoad(go); instance = go.AddComponent<SantaRitaBarAudio>();
            for (int i = 0; i < instance.layers.Length; i++)
            {
                var source = go.AddComponent<AudioSource>();
                source.clip = CriarTextura(i); source.loop = true;
                source.playOnAwake = false; source.spatialBlend = 0; source.volume = 0;
                instance.layers[i] = source;
            }
        }
        instance.inside = noBar; instance.hand = mao;
    }

    public static float VolumeDaCamada(int layer, int mao)
    {
        if (layer == 0) return mao < 2 ? .17f : 0;
        if (layer == 1) return mao >= 2 ? .23f : 0;
        return mao >= 3 ? .13f : 0;
    }

    private void Update()
    {
        bool active = inside && Capitulo1Director.Instance != null;
        bool pause = MenuPausa.Aberto || Time.timeScale == 0;
        for (int i = 0; i < layers.Length; i++)
        {
            var source = layers[i];
            if (pause != paused) { if (pause) source.Pause(); else source.UnPause(); }
            float target = active ? VolumeDaCamada(i, hand) * Mathf.Clamp01(PlayerPrefs.GetFloat("audio_efeitos", 1)) : 0;
            if (!pause && target > 0 && !source.isPlaying) source.Play();
            source.volume = Mathf.MoveTowards(source.volume, target, Time.unscaledDeltaTime * .25f);
            if (!pause && target == 0 && source.volume == 0 && source.isPlaying) source.Stop();
        }
        paused = pause;
    }

    public static AudioClip CriarTextura(int type)
    {
        const int rate = 22050, seconds = 16;
        var data = new float[rate * seconds];
        var rng = new System.Random(317 + type); float low = 0, mid = 0;
        for (int i = 0; i < data.Length; i++)
        {
            float t = i / (float)rate;
            float noise = (float)rng.NextDouble() * 2 - 1;
            low = Mathf.Lerp(low, noise, .018f); mid = Mathf.Lerp(mid, noise, .18f);
            float sound;
            if (type == 0)
            {
                // Modulação de três faixas, sem fala ou conteúdo narrativo audível.
                sound = 0;
                for (int v = 0; v < 3; v++)
                {
                    float pulse = Mathf.Max(0, Mathf.Sin(t * (2.5f + v * .7f) + v * 2.1f));
                    sound += (Mathf.Sin(t * Mathf.PI * 2 * (117 + v * 41)) * low * 2 + mid * .16f) * pulse;
                }
            }
            else if (type == 1)
            {
                float ripple = .5f + .5f * Mathf.Sin(t * Mathf.PI * .75f);
                sound = low * (1.5f + ripple) + Mathf.Sin(t * Mathf.PI * 2 * 73) * ripple * .12f;
                float drip = t % 2.7f;
                sound += Mathf.Sin(2 * Mathf.PI * (420 * drip - 55 * drip * drip)) * Mathf.Exp(-drip * 13) * .18f;
            }
            else sound = low * 1.6f + mid * .24f;
            float edge = Mathf.Min(1, Mathf.Min(t, seconds - t) / .08f);
            data[i] = Mathf.Clamp(sound * .65f * edge, -.85f, .85f);
        }
        var clip = AudioClip.Create("SR_SinteseBar_" + type, data.Length, 1, rate, false);
        clip.SetData(data, 0); return clip;
    }

    private void OnDestroy()
    {
        foreach (var source in layers) if (source != null && source.clip != null) Destroy(source.clip);
        if (instance == this) instance = null;
    }
}
