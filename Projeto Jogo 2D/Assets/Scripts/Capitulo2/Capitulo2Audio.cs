using UnityEngine;
using UnityEngine.SceneManagement;

// Texturas sonoras originais sintetizadas. Não substituem faixas aprovadas de C1.
// Chuva permanece no sistema Chuva; impactos são disparados apenas pela física.
public sealed class Capitulo2Audio : MonoBehaviour
{
    private static Capitulo2Audio instance;
    private readonly AudioSource[] fontes = new AudioSource[3];
    private AudioSource fonteHinoMatriz;
    private PlayerController jogadorCentro;
    private int regiao;
    private static AudioClip hinoMatriz;
    private void OnEnable()
    {
        instance=this;
        foreach(var old in GetComponents<AudioSource>())old.Stop();
        for(int i=0;i<fontes.Length;i++){var child=transform.Find("TrilhaC2_"+i);if(child!=null)fontes[i]=child.GetComponent<AudioSource>();}
        var canto = transform.Find("HinoMatriz_SinteseOriginal");
        if(canto!=null)fonteHinoMatriz=canto.GetComponent<AudioSource>();
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
        source.clip=clip;source.volume=AudioSettingsController.VolumeEfeitos*.65f;source.Play();Destroy(go,2.2f);Destroy(clip,2.3f);
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
        float proximidadeIgreja = ProximidadeDaMatriz(scene);
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
            if(scene=="CentroSantaRita"&&i==0&&proximidadeIgreja>0f)
                alvo*=Mathf.Lerp(1f,.62f,proximidadeIgreja);
            if(interior)alvo*=.65f;
            var filter=f.GetComponent<AudioLowPassFilter>();
            if(filter!=null)filter.cutoffFrequency=Mathf.MoveTowards(filter.cutoffFrequency,interior?1800f:22000f,Time.unscaledDeltaTime*16000f);
            if (alvo > 0 && BochaMinigameUI.IsShowing)
                alvo = AudioSettingsController.TrilhaDiretaMinigame;
            else if (GameMessageUI.IsShowing || SantaRitaTapeDeck.IsOpen)
                alvo *= .67f;
            alvo *= AudioSettingsController.VolumeMusica;
            if (alvo > 0 && !f.isPlaying) f.Play();
            f.volume = Mathf.MoveTowards(f.volume, alvo, Time.unscaledDeltaTime * .24f);
            if (alvo == 0 && f.volume == 0 && f.isPlaying) f.Stop();
        }
        AtualizarHinoDaMatriz(scene,ativo,interior,proximidadeIgreja);
    }

    private float ProximidadeDaMatriz(string scene)
    {
        if(scene=="IgrejaMatriz")return 1f;
        if(scene!="CentroSantaRita")return 0f;
        if(jogadorCentro==null)jogadorCentro=FindAnyObjectByType<PlayerController>();
        if(jogadorCentro==null)return 0f;
        float distancia=Vector2.Distance(jogadorCentro.transform.position,new Vector2(46f,63.35f));
        return Mathf.SmoothStep(0f,1f,Mathf.InverseLerp(18f,7f,distancia));
    }

    private void AtualizarHinoDaMatriz(string scene,bool ativo,bool interior,float proximidade)
    {
        bool localMatriz=scene=="CentroSantaRita"||scene=="IgrejaMatriz";
        if(fonteHinoMatriz==null&&(ativo&&localMatriz))
        {
            var go=new GameObject("HinoMatriz_SinteseOriginal");
            go.transform.SetParent(transform,false);
            go.transform.position=new Vector3(46f,65.2f,0f);
            fonteHinoMatriz=go.AddComponent<AudioSource>();
            fonteHinoMatriz.clip=CriarHinoMatrizSintetizado();
            fonteHinoMatriz.loop=true;
            fonteHinoMatriz.playOnAwake=false;
            fonteHinoMatriz.spatialBlend=.35f;
            fonteHinoMatriz.rolloffMode=AudioRolloffMode.Logarithmic;
            fonteHinoMatriz.minDistance=4f;
            fonteHinoMatriz.maxDistance=20f;
            fonteHinoMatriz.priority=64;
            fonteHinoMatriz.volume=0f;
            fonteHinoMatriz.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency=4600f;
        }
        if(fonteHinoMatriz==null)return;

        float alvo=ativo&&localMatriz?proximidade*.50f:0f;
        if(interior)alvo*=.82f;
        if(GameMessageUI.IsShowing||SantaRitaTapeDeck.IsOpen)alvo*=.67f;
        // O hino é uma ambiência diegética da igreja; o controle de música pode
        // ficar zerado sem silenciar os sons do mundo.
        alvo*=AudioSettingsController.VolumeEfeitos;
        if(alvo>0f&&!fonteHinoMatriz.isPlaying)fonteHinoMatriz.Play();
        fonteHinoMatriz.volume=Mathf.MoveTowards(fonteHinoMatriz.volume,alvo,Time.unscaledDeltaTime*.22f);
        var filtro=fonteHinoMatriz.GetComponent<AudioLowPassFilter>();
        if(filtro!=null)
        {
            float corte=localMatriz?Mathf.Lerp(1150f,4600f,proximidade):1150f;
            if(interior)corte=3200f;
            filtro.cutoffFrequency=Mathf.MoveTowards(filtro.cutoffFrequency,corte,Time.unscaledDeltaTime*9000f);
        }
        if(alvo==0f&&fonteHinoMatriz.volume==0f&&fonteHinoMatriz.isPlaying)fonteHinoMatriz.Stop();
    }

    // Motivo original de órgão sintetizado para a Matriz; audição e aprovação do autor pendentes.
    private static AudioClip CriarHinoMatrizSintetizado()
    {
        if(hinoMatriz!=null)return hinoMatriz;
        const int hz=22050,segundos=16;
        int[] melodia={60,64,67,72,71,69,67,64};
        int[][] acordes={
            new[]{48,55,60,64},
            new[]{41,48,53,57},
            new[]{43,50,55,59},
            new[]{48,55,60,64}
        };
        var dados=new float[hz*segundos];
        for(int i=0;i<dados.Length;i++)
        {
            float t=i/(float)hz;
            int compasso=Mathf.Min(3,(int)(t/4f));
            int nota=Mathf.Min(7,(int)(t/2f));
            float tempoNota=t%2f;
            float ataque=Mathf.SmoothStep(0f,1f,tempoNota/.10f);
            float saida=1f-Mathf.SmoothStep(1.78f,2f,tempoNota);
            float borda=Mathf.Min(t%4f,4f-t%4f);
            float entradaLoop=Mathf.SmoothStep(0f,1f,t/.12f);
            float saidaLoop=1f-Mathf.SmoothStep(segundos-.12f,segundos,t);
            float soma=0f;
            foreach(int midi in acordes[compasso])
                soma+=OndaOrgao(FrequenciaMidi(midi),t)*.025f*Mathf.SmoothStep(0f,1f,borda/.28f);
            soma+=OndaOrgao(FrequenciaMidi(melodia[nota]),t)*.06f*ataque*saida;
            dados[i]=Mathf.Clamp(soma*entradaLoop*saidaLoop,-.42f,.42f);
        }
        hinoMatriz=AudioClip.Create("C2_HinoMatriz_SinteseOriginal_16s",dados.Length,1,hz,false);
        hinoMatriz.SetData(dados,0);
        return hinoMatriz;
    }

    private static float FrequenciaMidi(int nota)=>440f*Mathf.Pow(2f,(nota-69)/12f);

    private static float OndaOrgao(float frequencia,float t)
    {
        float fase=2f*Mathf.PI*frequencia*t;
        return (Mathf.Sin(fase)+.16f*Mathf.Sin(fase*2f)+.04f*Mathf.Sin(fase*3f)+.01f*Mathf.Sin(fase*4f))/1.21f;
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
