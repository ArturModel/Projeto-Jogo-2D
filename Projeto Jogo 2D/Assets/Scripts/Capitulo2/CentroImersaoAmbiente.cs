using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Camada leve de imersao sonora e atmosferica para CentroSantaRita.
/// Aloca seus recursos procedurais uma vez, recalcula zonas apenas quando o jogador se move
/// e respeita integralmente MenuPausa.ReduzirEfeitosIntensos e entradas em interiores.
/// </summary>
public sealed class CentroImersaoAmbiente : MonoBehaviour
{
    private readonly struct PocaCentro
    {
        public readonly float x, y, rx, ry, cos, sin;

        public PocaCentro(float x, float y, float rx, float ry, float tilt)
        {
            this.x = x;
            this.y = y;
            this.rx = rx;
            this.ry = ry;
            cos = Mathf.Cos(tilt);
            sin = Mathf.Sin(tilt);
        }
    }

    private readonly struct PontoUrbano
    {
        public readonly Vector2 pos;
        public readonly float raio;
        public readonly float ganho;

        public PontoUrbano(float x, float y, float raio, float ganho)
        {
            pos = new Vector2(x, y);
            this.raio = raio;
            this.ganho = ganho;
        }
    }

    // Espelho exato das poças de PrepararPocasDoCentro (Capitulo2Exterior.Matriz.cs) para som/respingo sem desenho duplicado.
    private static readonly PocaCentro[] PocasCentro =
    {
        new(32.5f, 24.8f, .36f, .52f, -.12f), new(29.7f, 29.4f, .40f, .76f, .04f), new(33.2f, 34.6f, .52f, .86f, .03f),
        new(29.8f, 42.7f, .38f, .66f, -.12f), new(32.6f, 46.4f, .39f, .62f, .18f), new(32.7f, 50.2f, .58f, 1.02f, .13f),
        new(29.8f, 58.1f, .46f, .84f, .11f), new(32.3f, 63.7f, .53f, 1.02f, -.18f), new(33.2f, 69.1f, .44f, .68f, .04f),
        new(30.0f, 75.6f, .48f, .70f, .05f), new(32.6f, 79.4f, .43f, .65f, -.14f),
        new(41.2f, 38.9f, .98f, .35f, -.12f), new(50.8f, 38.9f, .98f, .35f, .12f), new(46.0f, 36.2f, .86f, .38f, .02f),
        new(46.0f, 61.25f, 1.04f, .65f, .03f), new(45.82f, 59.45f, .83f, .43f, -.07f), new(42.2f, 61.9f, .78f, .48f, -.08f),
        new(49.8f, 61.9f, .78f, .48f, .08f), new(46.0f, 55.2f, .92f, .56f, -.04f), new(46.0f, 45.2f, .88f, .54f, .05f),
        new(36.2f, 50.4f, .56f, .74f, .09f), new(55.8f, 50.4f, .56f, .74f, -.09f),
        new(66.3f, 38.9f, 1.05f, .41f, .06f), new(73.4f, 35.6f, .83f, .44f, -.16f),
        new(62.4f, 26.0f, .44f, .86f, -.08f), new(59.2f, 31.2f, .52f, .74f, .13f), new(58.7f, 46.9f, .48f, .78f, .15f),
        new(61.9f, 42.4f, .33f, .53f, -.12f), new(62.2f, 55.3f, .63f, .85f, -.14f), new(59.1f, 60.9f, .54f, .93f, .21f),
        new(58.6f, 65.7f, .42f, .72f, -.14f), new(62.3f, 71.4f, .50f, .89f, -.05f), new(59.2f, 76.6f, .42f, .68f, .16f),
        new(24.5f, 22.4f, .77f, .32f, .24f), new(13.5f, 16.3f, .58f, .29f, .18f),
        new(24.5f, 47.35f, .60f, .22f, -.12f), new(16.6f, 31.55f, .53f, .20f, .10f), new(25.0f, 26.35f, .42f, .19f, -.18f),
        new(65.2f, 19.4f, .48f, .22f, .12f), new(75.0f, 62.15f, .60f, .22f, -.10f), new(11.8f, 3.75f, .52f, .21f, .15f),
        new(27.5f, 83.65f, .67f, .30f, -.18f), new(30.4f, 81.85f, .45f, .32f, -.36f)
    };

    private static readonly PontoUrbano[] PontosUrbanos =
    {
        new(68.00f, 21.45f, 9.5f, 1.00f), // Rodoviaria (marquise/luminaria forte)
        new(10.00f, 33.05f, 7.5f, .90f),  // Oficina de Radio (transformador / antena)
        new(88.00f, 65.45f, 8.0f, .72f),  // Prefeitura (toldo e postes)
        new(20.20f, 49.30f, 7.0f, .68f),  // Fundos do Mercadinho
        new(21.80f, 27.95f, 6.5f, .62f),  // Casa da Costureira
        new(53.15f, 60.40f, 5.5f, .65f),  // Orelhao Matriz
        new(12.70f,  4.60f, 5.5f, .60f),  // Orelhao Beira-Rio
        new( 4.35f,  4.45f, 7.5f, .75f),  // Sociedade Beira-Rio
        new(39.00f, 45.00f, 6.0f, .45f),  // Postes da praca
        new(53.00f, 45.00f, 6.0f, .45f),
        new(39.00f, 56.00f, 6.0f, .45f),
        new(53.00f, 56.00f, 6.0f, .45f)
    };

    private static readonly Vector2 PosicaoIgreja = new(46f, 63.35f);
    private static readonly Vector2 PosicaoRodoviaria = new(68f, 21.45f);
    private static readonly Vector2 PosicaoPonte = new(45.5f, -4.5f);

    private static CentroImersaoAmbiente instance;
    private static bool cenaRegistrada;

    private AudioSource fonteVento;
    private AudioLowPassFilter filtroVento;
    private AudioSource fonteMata;
    private AudioLowPassFilter filtroMata;
    private AudioSource fonteUrbano;
    private AudioLowPassFilter filtroUrbano;
    private AudioSource fontePontual;
    private AudioLowPassFilter filtroPontual;

    private static AudioClip cacheVento;
    private static AudioClip cacheMata;
    private static AudioClip cacheUrbano;
    private static AudioClip cacheRangeMadeira;
    private static AudioClip cacheSinoDistante;
    private static AudioClip cacheGoteira;

    private AudioClip clipVento;
    private AudioClip clipMata;
    private AudioClip clipUrbano;
    private AudioClip clipRangeMadeira;
    private AudioClip clipSinoDistante;
    private AudioClip clipGoteira;
    private AudioClip clipTrafegoDistante;
    private AudioClip clipTrovao;

    private readonly List<Object> recursos = new();
    private Texture2D texturaParticula;
    private Material materialParticula;
    private ParticleSystem psBrisa;
    private ParticleSystem psVagalumes;

    private PlayerController jogador;
    private Camera cam;
    private Light2D luzGlobal;
    private float intensidadeLuzBase = .17f;
    private Color corLuzBase = new(.56f, .68f, .88f, 1f);
    private bool luzGlobalCapturada;
    private bool relampagoAtivo;

    private Vector2 ultimaPosAvaliada = new(float.NaN, float.NaN);
    private float proximoRecalculo;
    private float pesoVento = .45f;
    private float pesoMata = .15f;
    private float pesoUrbano;
    private float panUrbano;
    private float panMata;
    private float pesoVagalumes;

    private float proximoTrovao;
    private float tempoTrovaoSom = -1f;
    private float tempoRelampago;
    private float forcaRelampago;
    private float proximoSino;
    private float proximoTrafego;
    private float proximaGoteira;
    private float proximoRangePonte;
    private int passosNaPonte;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        if (cenaRegistrada)
        {
            SceneManager.sceneLoaded -= AoCarregarCena;
            cenaRegistrada = false;
        }
        instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Inicializar()
    {
        if (!cenaRegistrada)
        {
            SceneManager.sceneLoaded += AoCarregarCena;
            cenaRegistrada = true;
        }
        InstalarSeCentro(SceneManager.GetActiveScene().name);
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo) => InstalarSeCentro(cena.name);

    private static void InstalarSeCentro(string nomeCena)
    {
        if (nomeCena != "CentroSantaRita")
        {
            if (instance != null) Destroy(instance.gameObject);
            return;
        }
        if (instance != null) return;
        var existente = FindAnyObjectByType<CentroImersaoAmbiente>();
        if (existente != null) { instance = existente; return; }
        var go = new GameObject("CentroImersaoAmbiente");
        instance = go.AddComponent<CentroImersaoAmbiente>();
    }

    /// <summary>Retorna cobertura de poça [0..1] nas 43 poças do piso de CentroSantaRita.</summary>
    public static float CoberturaDePocaCentro(Vector2 pos)
    {
        for (int i = 0; i < PocasCentro.Length; i++)
        {
            ref readonly var p = ref PocasCentro[i];
            float dx = pos.x - p.x;
            float dy = pos.y - p.y;
            if (Mathf.Abs(dx) > p.rx + .22f || Mathf.Abs(dy) > p.ry + .22f) continue;
            float lx = (dx * p.cos + dy * p.sin) / p.rx;
            float ly = (-dx * p.sin + dy * p.cos) / p.ry;
            float q = lx * lx + ly * ly;
            if (q <= 1f) return Mathf.Clamp01(1f - q * .52f);
        }
        return 0f;
    }

    /// <summary>Chamado ao pisar na ponte de madeira do Beira-Rio para reforçar a imersão tátil.</summary>
    public static void NotificarPassoMadeiraCentro(Vector2 pos)
    {
        if (instance == null || pos.y > 0.5f || pos.y < -10f || pos.x < 42f || pos.x > 49f) return;
        instance.RegistrarPassoNaPonte(pos);
    }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        GarantirConfigurado();
        AgendarProximosEventos(inicial: true);
    }

    private void OnEnable()
    {
        if (instance == null) instance = this;
        GarantirConfigurado();
        if (SceneFader.EmTransicaoAudioPortal)
        {
            if (fonteVento != null) fonteVento.volume = 0f;
            if (fonteMata != null) fonteMata.volume = 0f;
            if (fonteUrbano != null) fonteUrbano.volume = 0f;
            if (fontePontual != null) fontePontual.volume = 0f;
        }
    }

    private void GarantirConfigurado()
    {
        if (clipVento == null || clipMata == null || clipUrbano == null ||
            clipRangeMadeira == null || clipSinoDistante == null || clipGoteira == null)
        {
            PrepararAudios();
        }
        if (psBrisa == null || psVagalumes == null || texturaParticula == null || materialParticula == null)
            PrepararParticulas();
        CapturarLuzGlobal();
    }

    private void PrepararAudios()
    {
        if (cacheVento == null) { cacheVento = CriarClipVentoFolhagem(); cacheVento.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        if (cacheMata == null) { cacheMata = CriarClipMataBeiraRio(); cacheMata.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        if (cacheUrbano == null) { cacheUrbano = CriarClipChuvaCoberturaUrbana(); cacheUrbano.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        if (cacheRangeMadeira == null) { cacheRangeMadeira = CriarClipRangeMadeira(); cacheRangeMadeira.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        if (cacheSinoDistante == null) { cacheSinoDistante = CriarClipSinoDistante(); cacheSinoDistante.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        if (cacheGoteira == null) { cacheGoteira = CriarClipGoteiraTelhado(); cacheGoteira.hideFlags = HideFlags.DontUnloadUnusedAsset; }
        clipVento = cacheVento;
        clipMata = cacheMata;
        clipUrbano = cacheUrbano;
        clipRangeMadeira = cacheRangeMadeira;
        clipSinoDistante = cacheSinoDistante;
        clipGoteira = cacheGoteira;
        if (clipTrafegoDistante == null) clipTrafegoDistante = Resources.Load<AudioClip>("Audio/Prologo/RodagemTerra");
        if (clipTrovao == null) clipTrovao = Resources.Load<AudioClip>("Audio/TrovaoEntrada");

        fonteVento = GarantirFonte("Centro_Vento", fonteVento, clipVento, loop: true, cutoff: 1850f, out filtroVento);
        fonteMata = GarantirFonte("Centro_MataRio", fonteMata, clipMata, loop: true, cutoff: 6800f, out filtroMata);
        fonteUrbano = GarantirFonte("Centro_UrbanoCobertura", fonteUrbano, clipUrbano, loop: true, cutoff: 3200f, out filtroUrbano);
        fontePontual = GarantirFonte("Centro_EventosPontuais", fontePontual, null, loop: false, cutoff: 6500f, out filtroPontual);
    }

    private AudioSource GarantirFonte(string nome, AudioSource atual, AudioClip clip, bool loop, float cutoff, out AudioLowPassFilter filtro)
    {
        if (atual == null)
        {
            var existente = transform.Find(nome);
            if (existente != null) atual = existente.GetComponent<AudioSource>();
        }
        if (atual == null)
        {
            var go = new GameObject(nome);
            go.transform.SetParent(transform, false);
            atual = go.AddComponent<AudioSource>();
            atual.playOnAwake = false;
            atual.spatialBlend = 0f;
            atual.volume = 0f;
            filtro = go.AddComponent<AudioLowPassFilter>();
        }
        else
        {
            filtro = atual.GetComponent<AudioLowPassFilter>();
            if (filtro == null) filtro = atual.gameObject.AddComponent<AudioLowPassFilter>();
        }
        atual.clip = clip;
        atual.loop = loop;
        filtro.cutoffFrequency = cutoff;
        return atual;
    }

    private void PrepararParticulas()
    {
        texturaParticula = CriarTexturaSuave16();
        recursos.Add(texturaParticula);
        var shader = Shader.Find("Sprites/Default");
        if (shader == null) return;
        materialParticula = new Material(shader) { mainTexture = texturaParticula };
        recursos.Add(materialParticula);

        psBrisa = CriarSistemaParticulas("Centro_BrisaLeve", maxParticulas: 28, tamanhoMin: .08f, tamanhoMax: .15f,
            tempoVida: 3.4f, corA: new Color(.78f, .88f, 1f, .28f), corB: new Color(.64f, .78f, .94f, .18f),
            velX: 1.55f, velY: -.65f, shapeBox: new Vector3(18f, 10f, 0f), sortOrder: 18);

        psVagalumes = CriarSistemaParticulas("Centro_Vagalumes", maxParticulas: 5, tamanhoMin: .08f, tamanhoMax: .13f,
            tempoVida: 4.2f, corA: new Color(.96f, .98f, .54f, .40f), corB: new Color(.70f, .94f, .46f, .28f),
            velX: .22f, velY: .18f, shapeBox: new Vector3(13f, 8f, 0f), sortOrder: 19);
    }

    private ParticleSystem CriarSistemaParticulas(string nome, int maxParticulas, float tamanhoMin, float tamanhoMax,
        float tempoVida, Color corA, Color corB, float velX, float velY, Vector3 shapeBox, int sortOrder)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(transform, false);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.maxParticles = maxParticulas;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(tempoVida * .75f, tempoVida * 1.15f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(.15f, .45f);
        main.startSize = new ParticleSystem.MinMaxCurve(tamanhoMin, tamanhoMax);
        main.startColor = new ParticleSystem.MinMaxGradient(corA, corB);

        var emission = ps.emission;
        emission.rateOverTime = 0f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = shapeBox;

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.x = new ParticleSystem.MinMaxCurve(velX * .6f, velX * 1.3f);
        vel.y = new ParticleSystem.MinMaxCurve(velY - .22f, velY + .22f);
        vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, .24f), new GradientAlphaKey(.9f, .72f), new GradientAlphaKey(0f, 1f) });
        col.color = grad;

        var rend = ps.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = materialParticula;
        rend.sortingLayerName = "Default";
        rend.sortingOrder = sortOrder;
        return ps;
    }

    private void CapturarLuzGlobal()
    {
        if (luzGlobalCapturada) return;
        foreach (var l in FindObjectsByType<Light2D>(FindObjectsInactive.Exclude))
        {
            if (l.lightType == Light2D.LightType.Global)
            {
                luzGlobal = l;
                intensidadeLuzBase = l.intensity;
                corLuzBase = l.color;
                luzGlobalCapturada = true;
                break;
            }
        }
    }

    private void AgendarProximosEventos(bool inicial)
    {
        float agora = Time.time;
        proximoTrovao = agora + (inicial ? Random.Range(6f, 12f) : Random.Range(24f, 42f));
        proximoSino = agora + (inicial ? Random.Range(14f, 24f) : Random.Range(46f, 74f));
        proximoTrafego = agora + (inicial ? Random.Range(8f, 16f) : Random.Range(28f, 48f));
        proximaGoteira = agora + Random.Range(2.2f, 4.5f);
    }

    private void Update()
    {
        if (jogador == null) jogador = FindAnyObjectByType<PlayerController>();
        if (cam == null) cam = Camera.main;
        if (jogador == null) return;

        bool interior = Capitulo2Director.Instance != null && Capitulo2Director.Instance.interior;
        bool pausado = MenuPausa.Aberto;
        bool emDialogo = GameMessageUI.IsShowing || SantaRitaFalasEntrada.Interagindo ||
                         PayphoneCheckpoint.IsSaveUIOpen || SantaRitaTapeDeck.IsOpen ||
                         BochaMinigameUI.IsShowing;

        Vector2 pos = jogador.transform.position;
        if (Time.time >= proximoRecalculo || (pos - ultimaPosAvaliada).sqrMagnitude > .01f)
        {
            proximoRecalculo = Time.time + .22f;
            ultimaPosAvaliada = pos;
            RecalcularZonas(pos);
        }

        AtualizarCamadasContinuas(interior, pausado, emDialogo);
        AtualizarEventosAmbientais(pos, interior, pausado, emDialogo);
        AtualizarParticulas(interior, pausado);
        AtualizarRelampago(interior, pausado);
    }

    private void RecalcularZonas(Vector2 pos)
    {
        // Centro urbanizado (praca e avenidas): x 28..64, y 20..74
        bool naAreaAberta = pos.x < 24f || pos.x > 68f || pos.y < 14f || pos.y > 74f;
        float ventoBase = naAreaAberta ? .62f : .38f;
        if (pos.y < 10f || pos.y > 76f) ventoBase = .76f;
        pesoVento = ventoBase;

        // Mata e beira-rio: rios e floresta sul (y < 18), oeste arborizado (x < 25), limite norte do cemiterio (y > 73)
        float mRio = Mathf.InverseLerp(18f, 4f, pos.y);
        float mOeste = Mathf.InverseLerp(26f, 10f, pos.x);
        float mNorte = pos.x > 40f ? Mathf.InverseLerp(73f, 83f, pos.y) : Mathf.InverseLerp(76f, 84f, pos.y);
        pesoMata = Mathf.Clamp01(Mathf.Max(mRio, Mathf.Max(mOeste * .92f, mNorte * .85f)));
        panMata = pos.y < 14f ? Mathf.Clamp((PosicaoPonte.x - pos.x) * .03f, -.35f, .35f)
            : pos.x < 26f ? -.42f : 0f;

        // Zumbido eletrico + goteira em marquises e postes principais
        float melhorUrbano = 0f;
        float pan = 0f;
        for (int i = 0; i < PontosUrbanos.Length; i++)
        {
            ref readonly var pt = ref PontosUrbanos[i];
            float dist = Vector2.Distance(pos, pt.pos);
            if (dist >= pt.raio) continue;
            float w = (1f - Mathf.SmoothStep(0f, 1f, dist / pt.raio)) * pt.ganho;
            if (w > melhorUrbano)
            {
                melhorUrbano = w;
                pan = Mathf.Clamp((pt.pos.x - pos.x) / pt.raio * .65f, -.6f, .6f);
            }
        }
        pesoUrbano = melhorUrbano;
        panUrbano = pan;

        // Vagalumes apenas perto de grama, beira-rio, mata oeste e canteiros da praca
        float naPracaGrama = Vector2.Distance(pos, new Vector2(46f, 50.45f)) < 11f ? .55f : 0f;
        pesoVagalumes = Mathf.Clamp01(Mathf.Max(pesoMata, naPracaGrama));
    }

    private void AtualizarCamadasContinuas(bool interior, bool pausado, bool emDialogo)
    {
        float efeitos = AudioSettingsController.VolumeEfeitos;
        float chuvaAmb = AudioSettingsController.VolumeChuvaAmbiente;
        float dt = Time.unscaledDeltaTime;

        float sairCena = SceneFader.AtenuacaoAudioSaidaCena;
        float fatorSaida = 1f - sairCena;
        float atenuacao = (interior ? .18f : (pausado ? .28f : (emDialogo ? .55f : 1f))) * fatorSaida;
        float velFade = sairCena > .001f ? .85f : (SceneFader.EmTransicaoAudioPortal ? .45f : .18f);
        float velCorte = sairCena > .001f ? 9000f : 3200f;
        float respVento = .85f + .15f * Mathf.Sin(Time.time * .31f);

        // ChuvaAmbiente e VolumeEfeitos ja embutem seus ganhos de calibracao do AudioSettingsController: nao multiplicar entre si.
        AtualizarFonteLoop(fonteVento, filtroVento,
            pesoVento * .95f * respVento * chuvaAmb * atenuacao,
            Mathf.Lerp(interior ? 420f : 1850f, 520f, sairCena),
            pan: Mathf.Sin(Time.time * .19f) * .18f, dt, velFade, velCorte);

        AtualizarFonteLoop(fonteMata, filtroMata,
            pesoMata * 1.15f * efeitos * atenuacao * (interior ? .35f : 1f),
            Mathf.Lerp(interior ? 650f : 6800f, 520f, sairCena),
            pan: panMata, dt, velFade, velCorte);

        AtualizarFonteLoop(fonteUrbano, filtroUrbano,
            pesoUrbano * .95f * chuvaAmb * atenuacao * (interior ? .25f : 1f),
            Mathf.Lerp(interior ? 580f : 3200f, 520f, sairCena),
            pan: panUrbano, dt, velFade, velCorte);
    }

    private static void AtualizarFonteLoop(AudioSource src, AudioLowPassFilter filtro, float alvo, float corteAlvo,
        float pan, float dt, float velFade = .18f, float velCorte = 3200f)
    {
        if (src == null || src.clip == null) return;
        if (alvo > .0002f && !src.isPlaying) src.Play();
        src.volume = Mathf.MoveTowards(src.volume, alvo, dt * velFade);
        src.panStereo = Mathf.MoveTowards(src.panStereo, pan, dt * .8f);
        if (filtro != null) filtro.cutoffFrequency = Mathf.MoveTowards(filtro.cutoffFrequency, corteAlvo, dt * velCorte);
        if (alvo <= .0002f && src.volume <= .0002f && src.isPlaying) src.Stop();
    }

    private void AtualizarEventosAmbientais(Vector2 pos, bool interior, bool pausado, bool emDialogo)
    {
        if (pausado) return;
        float sairCena = SceneFader.AtenuacaoAudioSaidaCena;
        if (fontePontual != null)
        {
            fontePontual.volume = 1f - sairCena;
            if (sairCena > .001f && filtroPontual != null)
                filtroPontual.cutoffFrequency = Mathf.MoveTowards(filtroPontual.cutoffFrequency, 520f, Time.unscaledDeltaTime * 9000f);
        }
        if (SceneFader.IsTransitioning || sairCena > .001f) return;

        float agora = Time.time;
        float efeitos = AudioSettingsController.VolumeEfeitos;
        float chuvaAmb = AudioSettingsController.VolumeChuvaAmbiente;

        // 1) Relampago distante + trovao sincronizado
        if (!interior && !emDialogo && agora >= proximoTrovao)
        {
            proximoTrovao = agora + Random.Range(28f, 48f);
            tempoRelampago = .52f;
            forcaRelampago = Random.Range(.65f, 1f);
            tempoTrovaoSom = agora + Random.Range(.45f, 1.05f);
        }

        if (tempoTrovaoSom > 0f && agora >= tempoTrovaoSom)
        {
            tempoTrovaoSom = -1f;
            if (!interior && !emDialogo && clipTrovao != null && fontePontual != null && chuvaAmb > 0f)
            {
                filtroPontual.cutoffFrequency = Random.Range(1400f, 3400f);
                fontePontual.panStereo = Random.Range(-.35f, .35f);
                fontePontual.pitch = Random.Range(.84f, .96f);
                float volTrovao = Random.Range(.65f, 1.0f) * chuvaAmb;
                fontePontual.PlayOneShot(clipTrovao, volTrovao);
                Capitulo2Audio.NotificarEventoSonoro(.52f);
            }
        }

        // 2) Sino distante da Matriz (suave, nunca durante dialogos)
        if (!interior && !emDialogo && agora >= proximoSino && clipSinoDistante != null && fontePontual != null && efeitos > 0f)
        {
            proximoSino = agora + Random.Range(52f, 84f);
            float distIgreja = Vector2.Distance(pos, PosicaoIgreja);
            float proxSino = 1f - Mathf.SmoothStep(0f, 1f, distIgreja / 58f);
            if (proxSino > .12f)
            {
                filtroPontual.cutoffFrequency = Mathf.Lerp(1200f, 4200f, proxSino);
                fontePontual.panStereo = Mathf.Clamp((PosicaoIgreja.x - pos.x) * .025f, -.6f, .6f);
                fontePontual.pitch = 1f;
                fontePontual.PlayOneShot(clipSinoDistante, (.45f + .55f * proxSino) * efeitos);
                Capitulo2Audio.NotificarEventoSonoro(.22f);
            }
        }

        // 3) Trafego/onibus distante no asfalto molhado perto da Rodoviaria e saida leste
        if (!interior && !emDialogo && agora >= proximoTrafego && clipTrafegoDistante != null && fontePontual != null && chuvaAmb > 0f)
        {
            proximoTrafego = agora + Random.Range(32f, 54f);
            float distRodo = Vector2.Distance(pos, PosicaoRodoviaria);
            if (distRodo < 36f)
            {
                float prox = 1f - Mathf.SmoothStep(0f, 1f, distRodo / 36f);
                filtroPontual.cutoffFrequency = 750f;
                fontePontual.panStereo = Mathf.Clamp((PosicaoRodoviaria.x - pos.x) * .025f, -.55f, .55f);
                fontePontual.pitch = Random.Range(.82f, .94f);
                fontePontual.PlayOneShot(clipTrafegoDistante, (.28f + .36f * prox) * chuvaAmb);
            }
        }

        // 4) Goteiras esporadicas perto de marquises e fachadas
        if (!interior && agora >= proximaGoteira && pesoUrbano > .35f && clipGoteira != null && fontePontual != null && chuvaAmb > 0f)
        {
            proximaGoteira = agora + Random.Range(2.4f, 6.2f);
            filtroPontual.cutoffFrequency = 5200f;
            fontePontual.panStereo = Mathf.Clamp(panUrbano + Random.Range(-.2f, .2f), -.7f, .7f);
            fontePontual.pitch = Random.Range(.85f, 1.25f);
            fontePontual.PlayOneShot(clipGoteira, (.36f + .44f * pesoUrbano) * chuvaAmb);
        }
    }

    private void RegistrarPassoNaPonte(Vector2 pos)
    {
        passosNaPonte++;
        if (passosNaPonte % 3 != 0 || Time.time < proximoRangePonte) return;
        if (clipRangeMadeira == null || fontePontual == null || AudioSettingsController.VolumeEfeitos <= 0f) return;
        proximoRangePonte = Time.time + .95f;
        filtroPontual.cutoffFrequency = 3200f;
        fontePontual.panStereo = Mathf.Clamp((pos.x - PosicaoPonte.x) * .25f, -.3f, .3f);
        fontePontual.pitch = Random.Range(.90f, 1.08f);
        fontePontual.PlayOneShot(clipRangeMadeira, .62f * AudioSettingsController.VolumeEfeitos);
    }

    private void AtualizarParticulas(bool interior, bool pausado)
    {
        bool ativo = !interior && !pausado && !MenuPausa.ReduzirEfeitosIntensos && cam != null;
        if (psBrisa != null)
        {
            if (ativo)
            {
                var p = cam.transform.position;
                psBrisa.transform.position = new Vector3(p.x, p.y, 0f);
                var em = psBrisa.emission;
                // VolumeChuvaAmbiente ja vem multiplicado por 0.10 de audio; usar taxa visual direta.
                em.rateOverTime = 5.5f;
                if (!psBrisa.isPlaying) psBrisa.Play();
            }
            else if (psBrisa.isPlaying)
            {
                psBrisa.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        if (psVagalumes != null)
        {
            if (ativo && pesoVagalumes > .14f)
            {
                var p = cam.transform.position;
                psVagalumes.transform.position = new Vector3(p.x, p.y, 0f);
                var em = psVagalumes.emission;
                em.rateOverTime = 1.2f * pesoVagalumes;
                if (!psVagalumes.isPlaying) psVagalumes.Play();
            }
            else if (psVagalumes.isPlaying)
            {
                psVagalumes.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    private void AtualizarRelampago(bool interior, bool pausado)
    {
        if (luzGlobal == null) return;
        if (tempoRelampago <= 0f || interior || pausado || MenuPausa.ReduzirEfeitosIntensos)
        {
            if (relampagoAtivo && luzGlobalCapturada)
            {
                luzGlobal.intensity = intensidadeLuzBase;
                luzGlobal.color = corLuzBase;
                relampagoAtivo = false;
            }
            tempoRelampago = 0f;
            return;
        }

        if (!relampagoAtivo)
        {
            intensidadeLuzBase = luzGlobal.intensity;
            corLuzBase = luzGlobal.color;
            luzGlobalCapturada = true;
            relampagoAtivo = true;
        }

        tempoRelampago -= Time.unscaledDeltaTime;
        float t = 1f - Mathf.Clamp01(tempoRelampago / .52f);
        // Pulso duplo sutil (0..0.34 e 0.46..0.84) sem ofuscar a tela
        float pulso1 = Mathf.Sin(Mathf.Clamp01(t / .34f) * Mathf.PI);
        float pulso2 = Mathf.Sin(Mathf.Clamp01((t - .46f) / .38f) * Mathf.PI) * .65f;
        float flash = Mathf.Max(0f, Mathf.Max(pulso1, pulso2)) * forcaRelampago;

        luzGlobal.intensity = intensidadeLuzBase + flash * .16f;
        luzGlobal.color = Color.Lerp(corLuzBase, new Color(.78f, .88f, 1f, 1f), flash * .78f);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        if (luzGlobal != null && luzGlobalCapturada)
        {
            luzGlobal.intensity = intensidadeLuzBase;
            luzGlobal.color = corLuzBase;
        }
        foreach (var r in recursos)
            if (r != null) Destroy(r);
        recursos.Clear();
    }

    // --- Geradores de audio procedural leves e sem emendas ---

    private static AudioClip CriarClipVentoFolhagem()
    {
        const int hz = 22050, seg = 12;
        int n = hz * seg;
        var dados = new float[n];
        var rng = new System.Random(4109);
        float lp1 = 0f, lp2 = 0f, prev = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.Min(1f, Mathf.Min(t, seg - t) / .75f);
            float white = (float)rng.NextDouble() * 2f - 1f;
            lp1 = Mathf.Lerp(lp1, white, .035f);
            lp2 = Mathf.Lerp(lp2, lp1, .08f);
            float rajada = .48f + .32f * Mathf.Sin(2f * Mathf.PI * t / 6f) + .20f * Mathf.Sin(2f * Mathf.PI * t / 4f + 1.3f);
            float folha = (white - prev) * .08f * Mathf.Pow(rajada, 2f);
            prev = white;
            dados[i] = Mathf.Clamp((lp2 * 1.45f * rajada + folha * 1.8f) * env, -.65f, .65f);
        }
        var clip = AudioClip.Create("Centro_VentoFolhagem", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static AudioClip CriarClipMataBeiraRio()
    {
        const int hz = 22050, seg = 12;
        int n = hz * seg;
        var dados = new float[n];
        var rng = new System.Random(7723);
        float lp1 = 0f, lp2 = 0f, prev = 0f;

        // Momentos irregulares de canto de grilo e sapo ao longo dos 12s para soar organico (sem loop mecanico)
        float[] griloProximo = { .45f, 2.35f, 4.80f, 6.75f, 9.20f, 10.95f };
        float[] griloDistante = { 1.35f, 3.60f, 5.95f, 8.15f, 10.10f };
        float[] sapoMargem = { 1.80f, 5.40f, 9.65f };

        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.Min(1f, Mathf.Min(t, seg - t) / .70f);
            float w = (float)rng.NextDouble() * 2f - 1f;
            lp1 = Mathf.Lerp(lp1, w, .04f);
            lp2 = Mathf.Lerp(lp2, lp1, .12f);
            float brisaMargem = .55f + .45f * Mathf.Sin(2f * Mathf.PI * t / 4f + .5f);
            float folhas = (lp1 - prev) * 1.30f * brisaMargem;
            prev = lp1;

            // Grilo noturno suave (~2950 Hz com trinado natural em rajadas curtas de 3 pulsos)
            float cantoGrilo = 0f;
            for (int g = 0; g < griloProximo.Length; g++)
            {
                float d = t - griloProximo[g];
                if (d < 0f || d > .34f) continue;
                for (int p = 0; p < 3; p++)
                {
                    float u = d - p * .085f;
                    if (u > 0f && u < .058f)
                    {
                        float e = Mathf.Sin(Mathf.PI * u / .058f);
                        float trinado = .55f + .45f * Mathf.Sin(2f * Mathf.PI * 36f * u);
                        cantoGrilo += (Mathf.Sin(2f * Mathf.PI * 2960f * u) * .65f +
                                       Mathf.Sin(2f * Mathf.PI * 3140f * u) * .35f) * e * trinado * .18f;
                    }
                }
            }

            // Segundo grilo mais distante (~3480 Hz, mais suave e agudo)
            for (int g = 0; g < griloDistante.Length; g++)
            {
                float d = t - griloDistante[g];
                if (d < 0f || d > .26f) continue;
                for (int p = 0; p < 2; p++)
                {
                    float u = d - p * .095f;
                    if (u > 0f && u < .052f)
                    {
                        float e = Mathf.Sin(Mathf.PI * u / .052f);
                        cantoGrilo += Mathf.Sin(2f * Mathf.PI * 3480f * u) * e * .095f;
                    }
                }
            }

            // Sapos suaves na margem do rio (dois coaxos graves por chamada)
            float sapo = 0f;
            for (int s = 0; s < sapoMargem.Length; s++)
            {
                float d = t - sapoMargem[s];
                if (d < 0f || d > .55f) continue;
                for (int p = 0; p < 2; p++)
                {
                    float u = d - p * .21f;
                    if (u > 0f && u < .16f)
                    {
                        float e = Mathf.Sin(Mathf.PI * u / .16f);
                        sapo += (Mathf.Sin(2f * Mathf.PI * 132f * u) * .7f +
                                 Mathf.Sin(2f * Mathf.PI * 264f * u) * .3f) * e * e * .22f;
                    }
                }
            }

            dados[i] = Mathf.Clamp((lp2 * .85f + folhas * .46f + cantoGrilo + sapo) * env, -.65f, .65f);
        }
        var clip = AudioClip.Create("Centro_MataBeiraRio", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static AudioClip CriarClipChuvaCoberturaUrbana()
    {
        const int hz = 22050, seg = 10;
        int n = hz * seg;
        var dados = new float[n];
        var rng = new System.Random(2083);
        float lp = 0f, prev = 0f, envGota = 0f;
        int amostrasGota = 0, durGota = 1;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.Min(1f, Mathf.Min(t, seg - t) / .65f);
            float w = (float)rng.NextDouble() * 2f - 1f;
            lp = Mathf.Lerp(lp, w, .085f);
            float chiadoTelha = (lp - prev) * .9f;
            prev = lp;

            // Pequenos pingos esparsos de agua em beiral/marquise com envelope suave (zero senoides/zumbido 60Hz)
            if (amostrasGota <= 0 && rng.NextDouble() < .00065)
            {
                durGota = 160 + rng.Next(260);
                amostrasGota = durGota;
            }
            envGota = 0f;
            if (amostrasGota > 0)
            {
                float u = 1f - (amostrasGota / (float)durGota);
                envGota = Mathf.Sin(Mathf.PI * u);
                envGota *= envGota;
                amostrasGota--;
            }
            float chuvaBeiral = lp * .72f + chiadoTelha * (.60f + envGota * .95f);
            dados[i] = Mathf.Clamp(chuvaBeiral * env, -.65f, .65f);
        }
        var clip = AudioClip.Create("Centro_UrbanoCobertura", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static AudioClip CriarClipRangeMadeira()
    {
        const int hz = 22050;
        int n = (int)(hz * .38f);
        var dados = new float[n];
        var rng = new System.Random(619);
        float fase = 0f, lp = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .05f, t)) * Mathf.Exp(-t * 7.5f);
            float freq = Mathf.Lerp(74f, 54f, t / .38f);
            fase += 2f * Mathf.PI * freq / hz;
            lp = Mathf.Lerp(lp, (float)rng.NextDouble() * 2f - 1f, .09f);
            float madeira = Mathf.Sin(fase) * .28f + Mathf.Sin(fase * 2f) * .12f + lp * .22f;
            dados[i] = Mathf.Clamp(madeira * env, -.5f, .5f);
        }
        var clip = AudioClip.Create("Centro_RangeMadeira", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static AudioClip CriarClipSinoDistante()
    {
        const int hz = 22050;
        int n = (int)(hz * 3.8f);
        var dados = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float atk = Mathf.SmoothStep(0f, 1f, t / .045f);
            float corpo = Mathf.Sin(2f * Mathf.PI * 293.66f * t) * Mathf.Exp(-t * .95f) * .36f +
                          Mathf.Sin(2f * Mathf.PI * 349.23f * t) * Mathf.Exp(-t * 1.15f) * .20f +
                          Mathf.Sin(2f * Mathf.PI * 587.33f * t) * Mathf.Exp(-t * 1.65f) * .15f +
                          Mathf.Sin(2f * Mathf.PI * 146.83f * t) * Mathf.Exp(-t * .75f) * .24f;
            dados[i] = Mathf.Clamp(corpo * atk, -.8f, .8f);
        }
        var clip = AudioClip.Create("Centro_SinoDistante", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static AudioClip CriarClipGoteiraTelhado()
    {
        // Pingo sutil baseado em ruido filtrado sem chirp digital (sem senoide em rampa)
        const int hz = 22050;
        int n = (int)(hz * .07f);
        var dados = new float[n];
        var rng = new System.Random(3301);
        float lp = 0f, prev = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)hz;
            float env = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .006f, t)) * Mathf.Exp(-t * 48f);
            float w = (float)rng.NextDouble() * 2f - 1f;
            lp = Mathf.Lerp(lp, w, .22f);
            float splash = lp - prev * .65f;
            prev = lp;
            dados[i] = Mathf.Clamp(splash * env * 1.15f, -.55f, .55f);
        }
        var clip = AudioClip.Create("Centro_Goteira", n, 1, hz, false);
        clip.SetData(dados, 0);
        return clip;
    }

    private static Texture2D CriarTexturaSuave16()
    {
        const int s = 16;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        var px = new Color[s * s];
        float c = (s - 1) * .5f;
        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float d = Mathf.Clamp01(Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c);
                float a = Mathf.Pow(1f - d, 1.8f);
                px[y * s + x] = new Color(1f, 1f, 1f, a);
            }
        tex.SetPixels(px);
        tex.Apply(false, true);
        return tex;
    }
}
