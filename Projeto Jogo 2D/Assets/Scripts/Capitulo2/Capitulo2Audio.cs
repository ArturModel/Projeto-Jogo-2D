using UnityEngine;
using UnityEngine.SceneManagement;

// Texturas sonoras originais sintetizadas. Não substituem faixas aprovadas de C1.
// Chuva permanece no sistema Chuva; impactos são disparados apenas pela física.
public sealed class Capitulo2Audio : MonoBehaviour
{
    private static Capitulo2Audio instance;
    private readonly AudioSource[] fontes = new AudioSource[3];
    private int regiao;
    public static void Definir(int assinatura)
    {
        if (instance == null)
        {
            var go = new GameObject("AudioCapitulo2"); DontDestroyOnLoad(go);
            instance = go.AddComponent<Capitulo2Audio>();
        }
        instance.regiao = Mathf.Clamp(assinatura, 0, 2);
    }

    private void Update()
    {
        bool ativo = Capitulo2Director.CenaDoCapitulo2(SceneManager.GetActiveScene().name);
        for (int i = 0; i < fontes.Length; i++)
        {
            if (ativo && i == regiao && fontes[i] == null)
            {
                fontes[i] = gameObject.AddComponent<AudioSource>();
                fontes[i].clip = CriarAmbiencia(i); fontes[i].loop = true;
                fontes[i].playOnAwake = false; fontes[i].spatialBlend = 0; fontes[i].volume = 0;
            }
            var f = fontes[i]; if (f == null) continue;
            float alvo = ativo && i == regiao ? (i == 1 ? .22f : .25f) : 0;
            alvo *= PlayerPrefs.GetFloat("audio_musica", 1);
            if (GameMessageUI.IsShowing || BochaMinigameUI.IsShowing) alvo *= .67f;
            if (alvo > 0 && !f.isPlaying) f.Play();
            f.volume = Mathf.MoveTowards(f.volume, alvo, Time.unscaledDeltaTime * .24f);
            if (alvo == 0 && f.volume == 0 && f.isPlaying) f.Stop();
        }
    }

    private static AudioClip CriarAmbiencia(int tipo)
    {
        const int hz = 22050, segundos = 16; var amostras = new float[hz * segundos];
        var rng = new System.Random(8317 + tipo); float noise = 0;
        float[] notas = { 110f, 146.832f, 164.814f, 130.813f };
        float[] toques = { 1.4f, 6.1f, 11.8f };
        for (int i = 0; i < amostras.Length; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.Min(1, Mathf.Min(t, segundos - t) / .65f);
            noise = Mathf.Lerp(noise, (float)rng.NextDouble() * 2 - 1, .025f);
            float som;
            if (tipo == 0)
            {
                float dt = t % 4f, f = notas[(int)(t / 4f) % 4];
                som = (Mathf.Sin(2 * Mathf.PI * f * t) + .18f * Mathf.Sin(2 * Mathf.PI * f * 2 * t)) * Mathf.Exp(-dt * 1.4f) * .25f;
                som += Mathf.Sin(2 * Mathf.PI * 55 * t) * .08f + noise * .025f;
            }
            else if (tipo == 1)
            {
                float respirar = .5f + .5f * Mathf.Sin(t * Mathf.PI / 4);
                som = (Mathf.Sin(2 * Mathf.PI * 55 * t) * .19f + Mathf.Sin(2 * Mathf.PI * 82.5f * t) * .07f) * respirar;
                foreach (float toque in toques)
                { float d = t - toque; if (d >= 0) som += (Mathf.Sin(2 * Mathf.PI * 293.66f * d) + .3f * Mathf.Sin(2 * Mathf.PI * 1104 * d)) * Mathf.Exp(-d * 2.2f) * .08f; }
                som += noise * .04f;
            }
            else
            {
                float sopro = Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * Mathf.PI / 4)), 2);
                som = (Mathf.Sin(2 * Mathf.PI * 65.406f * t) * .18f + noise * .12f) * sopro;
                som += Mathf.Sin(2 * Mathf.PI * 98 * t) * .025f;
            }
            amostras[i] = som * env;
        }
        var clip = AudioClip.Create("C2_TexturaOriginal_" + tipo, amostras.Length, 1, hz, false); clip.SetData(amostras, 0); return clip;
    }

    public static AudioClip CriarImpactoMadeira()
    {
        const int hz = 22050; var samples = new float[(int)(hz * .22f)]; var rng = new System.Random(1317);
        for (int i = 0; i < samples.Length; i++)
        { float t = i / (float)hz; samples[i] = ((float)rng.NextDouble() * .55f - .275f + Mathf.Sin(2 * Mathf.PI * 180 * t) * .65f) * Mathf.Exp(-t * 36); }
        var clip = AudioClip.Create("C2_ImpactoMadeiraOriginal", samples.Length, 1, hz, false); clip.SetData(samples, 0); return clip;
    }
}
