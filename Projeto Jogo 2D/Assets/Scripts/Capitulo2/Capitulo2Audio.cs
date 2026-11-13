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

    public const float VolumeBaseExterior = .25f;
    public const float FatorInterior = .34f;
    public const float CorteInteriorHz = 900f;
    public static float VolumeInteriorEsperado => VolumeBaseExterior * FatorInterior;

    private float duckingTemporario;
    private float fatorZonaAtual = 1f;
    private float corteZonaAtual = 22000f;
    private float proximoRecalculoZona;

    public static void NotificarEventoSonoro(float intensidade)
    {
        if (instance == null) return;
        instance.duckingTemporario = Mathf.Max(instance.duckingTemporario, Mathf.Clamp01(intensidade));
    }

    private void AtualizarZonaDinamica(string scene, bool interior)
    {
        if (interior || scene != "CentroSantaRita")
        {
            fatorZonaAtual = Mathf.MoveTowards(fatorZonaAtual, 1f, Time.unscaledDeltaTime * .6f);
            corteZonaAtual = Mathf.MoveTowards(corteZonaAtual, 22000f, Time.unscaledDeltaTime * 9000f);
            return;
        }
        if (Time.unscaledTime >= proximoRecalculoZona)
        {
            proximoRecalculoZona = Time.unscaledTime + .2f;
            if (jogadorCentro == null) jogadorCentro = FindAnyObjectByType<PlayerController>();
        }
        if (jogadorCentro == null) return;
        Vector2 p = jogadorCentro.transform.position;
        float alvoFator = 1f;
        float alvoCorte = 22000f;
        if (p.y < 14f) // Beira-Rio / ponte de madeira / Sociedade: destaca o rio e a brisa noturna
        {
            float tRio = Mathf.InverseLerp(14f, 2f, p.y);
            alvoFator = Mathf.Lerp(1f, .78f, tRio);
            alvoCorte = Mathf.Lerp(22000f, 13000f, tRio);
        }
        else if (p.x > 40f && p.y > 74f) // Aproximacao norte rumo ao cemiterio
        {
            float tCem = Mathf.InverseLerp(74f, 83f, p.y);
            alvoFator = Mathf.Min(alvoFator, Mathf.Lerp(1f, .74f, tCem));
            alvoCorte = Mathf.Min(alvoCorte, Mathf.Lerp(22000f, 9200f, tCem));
        }
        else if (p.x < 22f) // Setor oeste / mata e rua de chao
        {
            float tOeste = Mathf.InverseLerp(22f, 8f, p.x);
            alvoFator = Mathf.Min(alvoFator, Mathf.Lerp(1f, .83f, tOeste));
            alvoCorte = Mathf.Min(alvoCorte, Mathf.Lerp(22000f, 14500f, tOeste));
        }
        fatorZonaAtual = Mathf.MoveTowards(fatorZonaAtual, alvoFator, Time.unscaledDeltaTime * .35f);
        corteZonaAtual = Mathf.MoveTowards(corteZonaAtual, alvoCorte, Time.unscaledDeltaTime * 5000f);
    }

    private void Update()
    {
        string scene=SceneManager.GetActiveScene().name;
        bool ativo = Capitulo2Director.CenaDoCapitulo2(scene);
        regiao=scene=="CemiterioSantaRita"||scene=="AbrigoCemiterio"?1:scene=="SociedadeBeiraRio"?2:0;
        bool interior=Capitulo2Director.Instance!=null&&Capitulo2Director.Instance.interior;
        float proximidadeIgreja = ProximidadeDaMatriz(scene);
        duckingTemporario = Mathf.MoveTowards(duckingTemporario, 0f, Time.unscaledDeltaTime * .28f);
        AtualizarZonaDinamica(scene, interior);
        bool dialogoOuLeitura = GameMessageUI.IsShowing || SantaRitaFalasEntrada.Interagindo ||
            PayphoneCheckpoint.IsSaveUIOpen || SantaRitaTapeDeck.IsOpen;
        bool menuOuMapa = MapaInterativoUI.IsShowing || MenuPausa.Aberto;
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
            bool portalSuave = SceneFader.EmTransicaoAudioPortal;
            bool portalParaInterior = SceneFader.TransicaoAudioParaInterior;
            float portalP = SceneFader.ProgressoAudioPortal;
            float fatorExt = (!interior && ativo && i == regiao) ? fatorZonaAtual : 1f;
            float corteExt = corteZonaAtual;
            float alvo = ativo && i == regiao ? VolumeBaseExterior : 0;
            float alvoCorte = interior ? CorteInteriorHz : corteExt;
            if(scene=="CentroSantaRita"&&i==0&&proximidadeIgreja>0f)
            {
                float mixIgreja = portalSuave && portalParaInterior ? proximidadeIgreja * (1f - portalP) : proximidadeIgreja;
                fatorExt*=Mathf.Lerp(1f,.54f,mixIgreja);
                if(!interior)corteExt=Mathf.Min(corteExt,Mathf.Lerp(22000f,11500f,mixIgreja));
                alvoCorte=interior?CorteInteriorHz:corteExt;
            }
            if(!interior&&ativo&&i==regiao)
            {
                alvo*=fatorExt;
                alvo*=1f-.015f*(1f-Mathf.Cos(Time.unscaledTime*.35f)); // leve respiro organico na trilha
                if(duckingTemporario>0.001f)
                {
                    alvo*=Mathf.Lerp(1f,.72f,duckingTemporario);
                    alvoCorte=Mathf.Min(alvoCorte,Mathf.Lerp(22000f,4200f,duckingTemporario));
                }
            }
            else if(scene=="CentroSantaRita"&&i==0&&proximidadeIgreja>0f)
            {
                alvo*=Mathf.Lerp(1f,.54f,proximidadeIgreja);
            }
            if(portalSuave&&ativo&&i==regiao)
            {
                if(portalParaInterior)
                {
                    alvo=VolumeBaseExterior*Mathf.Lerp(fatorExt,FatorInterior,portalP);
                    alvoCorte=Mathf.Lerp(corteExt,CorteInteriorHz,portalP);
                }
                else
                {
                    alvo=VolumeBaseExterior*Mathf.Lerp(FatorInterior,fatorExt,portalP);
                    alvoCorte=Mathf.Lerp(CorteInteriorHz,corteExt,portalP);
                }
            }
            else if(interior)alvo*=FatorInterior;
            if(dialogoOuLeitura){alvo*=.54f;if(!interior)alvoCorte=Mathf.Min(alvoCorte,3600f);}
            else if(menuOuMapa){alvo*=.72f;if(!interior)alvoCorte=Mathf.Min(alvoCorte,5200f);}
            if(SceneFader.IsTransitioning&&!portalSuave)alvo*=.62f;
            var filter=f.GetComponent<AudioLowPassFilter>();
            if(filter!=null)
            {
                float taxa=portalSuave?12f:(interior?9f:5.5f);
                filter.cutoffFrequency=Mathf.Lerp(filter.cutoffFrequency,alvoCorte,1f-Mathf.Exp(-Time.unscaledDeltaTime*taxa));
                if(Mathf.Abs(filter.cutoffFrequency-alvoCorte)<2f)filter.cutoffFrequency=alvoCorte;
            }
            if (alvo > 0 && BochaMinigameUI.IsShowing)
                alvo = AudioSettingsController.TrilhaDiretaMinigame;
            alvo *= AudioSettingsController.VolumeMusica;
            if (alvo > 0 && !f.isPlaying) f.Play();
            float velVolume = alvo < f.volume ? .42f : (portalSuave ? .36f : .22f);
            f.volume = Mathf.MoveTowards(f.volume, alvo, Time.unscaledDeltaTime * velVolume);
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

        bool dentroDaIgreja = scene == "IgrejaMatriz";
        fonteHinoMatriz.spatialBlend = dentroDaIgreja ? 0f : .35f;
        float alvo=ativo&&localMatriz?proximidade*(dentroDaIgreja?.95f:.78f):0f;
        if(interior&&!dentroDaIgreja)alvo*=.82f;
        if(GameMessageUI.IsShowing||SantaRitaFalasEntrada.Interagindo||SantaRitaTapeDeck.IsOpen)alvo*=.62f;
        // O hino é uma ambiência diegética da igreja; o controle de música pode
        // ficar zerado sem silenciar os sons do mundo.
        alvo*=AudioSettingsController.VolumeEfeitos;
        if(alvo>0f&&!fonteHinoMatriz.isPlaying)fonteHinoMatriz.Play();
        fonteHinoMatriz.volume=Mathf.MoveTowards(fonteHinoMatriz.volume,alvo,Time.unscaledDeltaTime*.22f);
        var filtro=fonteHinoMatriz.GetComponent<AudioLowPassFilter>();
        if(filtro!=null)
        {
            float corte=dentroDaIgreja?6800f:(localMatriz?Mathf.Lerp(1150f,4600f,proximidade):1150f);
            if(interior&&!dentroDaIgreja)corte=3200f;
            filtro.cutoffFrequency=Mathf.MoveTowards(filtro.cutoffFrequency,corte,Time.unscaledDeltaTime*9000f);
        }
        if(alvo==0f&&fonteHinoMatriz.volume==0f&&fonteHinoMatriz.isPlaying)fonteHinoMatriz.Stop();
    }

    // Cama sacra e cantabile de orgao de tubos em Do menor (Cm9 - Abmaj7 - Fm9 - Gsus4/G7),
    // em harmonia com Centro_loop e sem ecos discretos ou modulacao FM acumulativa.
    private static AudioClip CriarHinoMatrizSintetizado()
    {
        if (hinoMatriz != null) return hinoMatriz;
        const int hz = 22050, segundos = 24;
        int total = hz * segundos;
        var dados = new float[total];

        int[][] acordes =
        {
            new[] { 48, 55, 62, 63 }, // Cm9
            new[] { 44, 51, 58, 60 }, // Abmaj7
            new[] { 41, 48, 55, 56 }, // Fm9
            new[] { 43, 50, 55, 60 }, // Gsus4
            new[] { 43, 50, 53, 59 }  // G7 (prepara Cm9)
        };
        // Frases lentas e meditativas (inicio, duracao, midi): 24s totais em Do menor
        var notasMelodia = new (float ini, float dur, int midi)[]
        {
            (0.2f, 3.3f, 67),  // G4
            (3.2f, 3.1f, 65),  // F4
            (6.0f, 3.4f, 63),  // Eb4
            (9.1f, 3.2f, 62),  // D4
            (12.0f, 3.4f, 60), // C4
            (15.1f, 3.2f, 63), // Eb4
            (18.0f, 5.8f, 62)  // D4 sustentado sobre Gsus4->G7
        };

        for (int i = 0; i < total; i++)
        {
            float t = i / (float)hz;
            float soma = 0f;

            // 5 blocos harmonicos com ataque e soltura longos (crossfade natural sem cliques nem staccato)
            soma += ContribuirAcordeLegato(acordes[0], t, 0f, 6f, fade: .85f);
            soma += ContribuirAcordeLegato(acordes[1], t, 6f, 12f, fade: .85f);
            soma += ContribuirAcordeLegato(acordes[2], t, 12f, 18f, fade: .85f);
            soma += ContribuirAcordeLegato(acordes[3], t, 18f, 21f, fade: .75f);
            soma += ContribuirAcordeLegato(acordes[4], t, 21f, 24f, fade: .85f);

            for (int n = 0; n < notasMelodia.Length; n++)
            {
                var nota = notasMelodia[n];
                if (t < nota.ini || t > nota.ini + nota.dur) continue;
                float u = t - nota.ini;
                float env = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .42f, u)) *
                            (1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(nota.dur - .65f, nota.dur, u)));
                float f = FrequenciaMidi(nota.midi);
                soma += OndaOrgaoCamara(f, u) * .18f * env;
            }

            // Respiro suave de fole (8 ciclos exatos em 24s) e pedal grave sutil em Do2
            float fole = .93f + .07f * Mathf.Sin(2f * Mathf.PI * t / 3f);
            float pedalDo = OndaOrgaoCamara(FrequenciaMidi(36), t) * .085f;
            float entradaLoop = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .45f, t));
            float saidaLoop = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(segundos - .45f, segundos, t));
            dados[i] = Mathf.Clamp((soma * fole + pedalDo) * entradaLoop * saidaLoop, -.75f, .75f);
        }

        hinoMatriz = AudioClip.Create("C2_HinoMatriz_DoMenor_24s", total, 1, hz, false);
        hinoMatriz.SetData(dados, 0);
        return hinoMatriz;
    }

    private static float ContribuirAcordeLegato(int[] notas, float t, float inicio, float fim, float fade)
    {
        float mar = fade * .6f;
        if (t < inicio - mar || t > fim + mar) return 0f;
        float ent = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inicio - mar, inicio + fade, t));
        float sai = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(fim - fade, fim + mar, t));
        float env = ent * sai;
        if (env <= .0001f) return 0f;
        float s = 0f;
        float localT = t - (inicio - mar);
        for (int k = 0; k < notas.Length; k++)
            s += OndaOrgaoCamara(FrequenciaMidi(notas[k]), localT);
        return (s / notas.Length) * .34f * env;
    }

    private static float FrequenciaMidi(int nota)=>440f*Mathf.Pow(2f,(nota-69)/12f);

    private static float OndaOrgaoCamara(float frequencia, float t)
    {
        float fase = 2f * Mathf.PI * frequencia * t;
        // Timbre quente e limpo de tubos flautados (sem batimentos rapidos nem modulacao FM acumulativa).
        return (Mathf.Sin(fase) * .78f
              + Mathf.Sin(fase * 2f) * .16f
              + Mathf.Sin(fase * 3f) * .045f
              + Mathf.Sin(fase * 4f) * .012f) / .997f;
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
