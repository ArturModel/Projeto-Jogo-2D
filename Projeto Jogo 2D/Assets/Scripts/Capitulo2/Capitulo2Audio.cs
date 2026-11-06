using UnityEngine;
using UnityEngine.SceneManagement;

// Texturas sonoras originais sintetizadas. Não substituem faixas aprovadas de C1.
// Chuva permanece no sistema Chuva; impactos são disparados apenas pela física.
public sealed class Capitulo2Audio : MonoBehaviour
{
    private static Capitulo2Audio instance;
    private readonly AudioSource[] fontes = new AudioSource[3];
    private int regiao;
    private void OnEnable()
    {
        instance=this;
        foreach(var old in GetComponents<AudioSource>())old.Stop();
        for(int i=0;i<fontes.Length;i++){var child=transform.Find("TrilhaC2_"+i);if(child!=null)fontes[i]=child.GetComponent<AudioSource>();}
    }
    private static AudioClip resposta;
    private static GameObject radioFeedback;
    public static AudioClip RespostaAbafada()
    {
        if(resposta!=null)return resposta;
        const int hz=22050;var data=new float[hz*2];var rng=new System.Random(1917);float filtro=0;
        for(int i=0;i<data.Length;i++)
        {
            float t=i/(float)hz;filtro=Mathf.Lerp(filtro,(float)rng.NextDouble()*2-1,.018f);
            float env=Mathf.Pow(Mathf.Sin(Mathf.PI*t/2),2)*Mathf.Exp(-t*.9f);
            data[i]=(filtro*.6f+Mathf.Sin(t*2*Mathf.PI*83)*.06f)*env;
        }
        resposta=AudioClip.Create("C2_RespostaAbafada_SoproOriginal",data.Length,1,hz,false);resposta.SetData(data,0);return resposta;
    }

    public static void TocarRadio(int faixa)
    {
        if(radioFeedback!=null)
        {var old=radioFeedback.GetComponent<AudioSource>();old.Stop();Destroy(old.clip);Destroy(radioFeedback);}
        var go=new GameObject("Radio_Retorno_"+faixa);var source=go.AddComponent<AudioSource>();source.playOnAwake=false;
        radioFeedback=go;
        const int hz=22050;var data=new float[hz*2];var rng=new System.Random(83+faixa);float filtro=0;
        for(int i=0;i<data.Length;i++)
        {
            float t=i/(float)hz;filtro=Mathf.Lerp(filtro,(float)rng.NextDouble()*2-1,faixa==1?.6f:.08f);
            float env=Mathf.Clamp01(t*30)*Mathf.Clamp01((2-t)*8);
            float som=faixa==1?filtro*.23f:faixa==0?filtro*.06f+Mathf.Sin(t*2*Mathf.PI*440)*.035f:
                ((t>.2f&&t<.45f)||(t>.7f&&t<.95f)?Mathf.Sin(t*2*Mathf.PI*660)*.15f:0);
            data[i]=som*env;
        }
        var clip=AudioClip.Create("Radio_"+new[]{"FM_sinal_distante","AM_chiado","LOCAL_dois_tons"}[faixa],data.Length,1,hz,false);clip.SetData(data,0);
        source.clip=clip;source.volume=PlayerPrefs.GetFloat("audio_efeitos",1)*.65f;source.Play();Destroy(go,2.2f);Destroy(clip,2.3f);
    }
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
        string scene=SceneManager.GetActiveScene().name;
        bool ativo = Capitulo2Director.CenaDoCapitulo2(scene);
        regiao=scene=="CemiterioSantaRita"||scene=="AbrigoCemiterio"?1:scene=="SociedadeBeiraRio"?2:0;
        bool interior=Capitulo2Director.Instance!=null&&Capitulo2Director.Instance.interior;
        for (int i = 0; i < fontes.Length; i++)
        {
            if (ativo && i == regiao && fontes[i] == null)
            {
                var musicObject=new GameObject("TrilhaC2_"+i);musicObject.transform.SetParent(transform,false);fontes[i] = musicObject.AddComponent<AudioSource>();
                fontes[i].clip = Resources.Load<AudioClip>("Audio/"+new[]{"Centro","Cemiterio","Bocha"}[i]+"_loop"); fontes[i].loop = true;
                fontes[i].gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency=22000f;
                fontes[i].playOnAwake = false; fontes[i].spatialBlend = 0; fontes[i].volume = 0;
            }
            var f = fontes[i]; if (f == null) continue;
            if(f.clip==null){f.clip=Resources.Load<AudioClip>("Audio/"+new[]{"Centro","Cemiterio","Bocha"}[i]+"_loop");if(f.clip==null)continue;}
            float alvo = ativo && i == regiao ? .25f : 0;
            if(interior)alvo*=.65f;
            var filter=f.GetComponent<AudioLowPassFilter>();
            if(filter!=null)filter.cutoffFrequency=Mathf.MoveTowards(filter.cutoffFrequency,interior?1800f:22000f,Time.unscaledDeltaTime*16000f);
            alvo *= PlayerPrefs.GetFloat("audio_musica", 1);
            if (GameMessageUI.IsShowing || BochaMinigameUI.IsShowing || SantaRitaTapeDeck.IsOpen) alvo *= .67f;
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
