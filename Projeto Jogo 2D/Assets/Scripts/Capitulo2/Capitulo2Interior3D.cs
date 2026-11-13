using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[DefaultExecutionOrder(50)]
internal sealed partial class Capitulo2Interior3D : MonoBehaviour
{
    internal const float DuracaoEntrada = 3.05f;
    internal const float DuracaoEntradaIntegrada = 0.22f;
    internal const float DuracaoSaidaIntegrada = 0.98f;
    internal const float SaidaAntesDaNevoa = .96f;
    private const int RoomLayer = 27;
    private const int ExteriorLayer = 26;
    private const int RoomMask = 1 << RoomLayer;
    private const int ExteriorMask = 1 << ExteriorLayer;
    private const float EyeHeight = 1.61f;
    private const float VelocidadeCaminhada = 2.55f;

    private struct AlvoInteracao
    {
        public string Id;
        public string Prompt;
        public Capitulo2Interactable Fonte;
        public Action AoAcionar;
    }

    private struct AlvoPortal
    {
        public Capitulo2Portal Portal;
        public string Prompt;
    }

    private readonly List<UnityEngine.Object> owned = new();
    private readonly Dictionary<string, Material> surfaces = new();
    private readonly Dictionary<Texture2D, Material> spriteMaterials = new();
    private readonly Dictionary<Sprite, Mesh> spriteMeshes = new();
    private readonly Dictionary<Collider, AlvoInteracao> alvos = new();
    private readonly Dictionary<string, Bounds> boundsPorId = new();
    private readonly Dictionary<Collider, AlvoPortal> portais = new();
    private readonly List<Transform> personagens = new();
    private readonly List<Action<float>> animacoes = new();
    private readonly Dictionary<string, Capitulo2Interactable> interactablesPorId = new();

    private Transform world, porta, macaneta, mao;
    private Camera vista, baseCamera;
    private RenderTexture target;
    private Canvas canvas;
    private TextMeshProUGUI dica;
    private Light lanterna;
    private MeshRenderer chuva3D;
    private Material materialChuva;
    private Collider saida;
    private Capitulo2Portal portalPrincipal;
    private PlayerController player;
    private Chuva chuva;
    private CursorLockMode cursorAnterior;
    private bool cursorVisivel, ruaOcluida, ambienteAlterado, tocouFecho, tocouPorta, tocouSaida;
    private int previousMask, frameCaptura;
    private CameraClearFlags previousClear;
    private Color previousBackground;
    private AmbientMode previousAmbientMode;
    private Color previousAmbientLight, previousAmbientSky, previousAmbientEquator, previousAmbientGround;
    private float previousAmbientIntensity;
    private float yaw, pitch, soleiraZ = -4.5f, passoAcumulado;
    private Vector3 pontoChegada = new Vector3(0f, EyeHeight, -2.85f);
    private Vector3 inicioSaida;
    private Quaternion inicioRotSaida = Quaternion.identity;
    private Vector3 pendenteSociedade = new Vector3(0f, 2.75f, 4.5f);
    private static Vector2 focoEntradaPendente = new Vector2(.5f, .5f);
    private Vector2 focoEntrada = new Vector2(.5f, .5f);
    private Vector2 velocidade;
    private string somPasso = "Foley/footstep_wood_000";
    private int indicePasso;
    private bool animandoEntrada, modoPreview;

    internal static Capitulo2Interior3D Atual { get; private set; }
    internal static bool EntradaIntegradaPendente { get; set; }
    internal RenderTexture TargetTexture => target;
    internal Camera Camera3D => vista;
    internal float SoleiraZ => soleiraZ;

    internal static void PrepararFocoEntrada(Vector2 foco) => focoEntradaPendente = foco;

    internal static bool EhCenaInterior3D(string scene) => scene switch
    {
        "IgrejaMatriz" => true,
        "PrefeituraSantaRita" => true,
        "RodoviariaSantaRita" => true,
        "MercadinhoDeposito" => true,
        "OficinaRadio" => true,
        "CasaCostureira" => true,
        "AbrigoCemiterio" => true,
        "SociedadeBeiraRio" => true,
        _ => false
    };

    internal static Vector2 FocoDaPorta
    {
        get
        {
            if (Atual == null || Atual.vista == null || Atual.macaneta == null)
                return Vector2.one * .5f;
            Vector3 ponto = Atual.vista.WorldToViewportPoint(Atual.macaneta.position);
            return ponto.z > 0f ? new Vector2(ponto.x, ponto.y) : Vector2.one * .5f;
        }
    }

    internal void Montar(string scene, PlayerController jogador)
    {
        if (!EhCenaInterior3D(scene) || jogador == null || Camera.main == null) return;
        player = jogador;
        baseCamera = Camera.main;
        previousMask = baseCamera.cullingMask;
        previousClear = baseCamera.clearFlags;
        previousBackground = baseCamera.backgroundColor;
        cursorAnterior = Cursor.lockState;
        cursorVisivel = Cursor.visible;
        focoEntrada = focoEntradaPendente;
        focoEntradaPendente = new Vector2(.5f, .5f);

        MapearCena();
        MontarNucleo3D(scene, baseCamera, incluirMao: true, larguraRt: 960);

        canvas = UIFactory.CreateCanvas(transform, scene + "3DCanvas", 6400);
        var imagem = new GameObject("Vista" + scene, typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
        imagem.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(imagem.rectTransform);
        imagem.raycastTarget = false;
        imagem.texture = target;

        dica = UIFactory.CreateLabel(canvas.transform, "InteracaoInteriorC2", 13f);
        dica.raycastTarget = false;
        dica.alignment = TextAlignmentOptions.Center;
        dica.rectTransform.anchorMin = dica.rectTransform.anchorMax = new Vector2(.5f, .5f);
        dica.rectTransform.anchoredPosition = new Vector2(0f, -28f);
        dica.rectTransform.sizeDelta = new Vector2(500f, 44f);
        dica.text = "";

        chuva = FindAnyObjectByType<Chuva>();
        baseCamera.cullingMask = 0;
        baseCamera.clearFlags = CameraClearFlags.SolidColor;
        baseCamera.backgroundColor = Color.black;
        player.enabled = false;
        ruaOcluida = true;
        InteractionPrompt.Instance?.SetTarget(null);
        if (chuva != null) chuva.DefinirModoInterior(true, 1f);

        yaw = 0f;
        pitch = 2f;
        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);
        vista.transform.localPosition = pontoChegada;
        vista.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        if (EntradaIntegradaPendente)
        {
            EntradaIntegradaPendente = false;
            animandoEntrada = true;
            AtualizarEntradaIntegrada(0f);
        }
        OrientarPersonagens();
        Physics.SyncTransforms();

        Atual = this;
    }

    internal bool MontarPreview(string scene, Camera baseCam)
    {
        if (!EhCenaInterior3D(scene) || baseCam == null) return false;
        modoPreview = true;
        baseCamera = baseCam;
        MontarNucleo3D(scene, baseCam, incluirMao: false, larguraRt: 512);
        if (chuva3D != null) chuva3D.enabled = false;
        if (porta != null) porta.localRotation = Quaternion.Euler(0f, 100f, 0f);
        AtualizarPreview(0f);
        OrientarPersonagens();
        return true;
    }

    internal void PrepararCapturaBridgeSaida()
    {
        if (vista == null) return;
        if (chuva3D != null) chuva3D.enabled = false;
        if (mao != null) mao.gameObject.SetActive(false);
        vista.Render();
    }

    internal void PrepararCapturaBackdropInterior()
    {
        if (vista == null) return;
        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);
        vista.transform.localPosition = new Vector3(pontoChegada.x, EyeHeight, soleiraZ - .22f);
        vista.transform.localRotation = Quaternion.Euler(2f, 0f, 0f);
        vista.fieldOfView = 56f;
        vista.ResetProjectionMatrix();
        if (porta != null) porta.localRotation = Quaternion.Euler(0f, 100f, 0f);
        if (chuva3D != null) chuva3D.enabled = false;
        if (mao != null) mao.gameObject.SetActive(false);
        OrientarPersonagens();
        vista.Render();
    }

    internal void AtualizarPreview(float avanco)
    {
        if (vista == null) return;
        float t = Mathf.Clamp01(avanco);
        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);
        vista.transform.localPosition = new Vector3(
            pontoChegada.x,
            Mathf.Lerp(EyeHeight - .04f, pontoChegada.y, t),
            Mathf.Lerp(soleiraZ - .65f, pontoChegada.z, t));
        vista.transform.localRotation = Quaternion.Euler(Mathf.Lerp(3.2f, 2f, t), 0f, 0f);
        vista.fieldOfView = Mathf.Lerp(42f, 62f, t);
        vista.ResetProjectionMatrix();
        if (porta != null) porta.localRotation = Quaternion.Euler(0f, 100f, 0f);
        if (chuva3D != null) chuva3D.enabled = false;
        OrientarPersonagens();
    }

    private void MontarNucleo3D(string scene, Camera camRef, bool incluirMao, int larguraRt)
    {
        if (!ambienteAlterado)
        {
            previousAmbientMode = RenderSettings.ambientMode;
            previousAmbientLight = RenderSettings.ambientLight;
            previousAmbientSky = RenderSettings.ambientSkyColor;
            previousAmbientEquator = RenderSettings.ambientEquatorColor;
            previousAmbientGround = RenderSettings.ambientGroundColor;
            previousAmbientIntensity = RenderSettings.ambientIntensity;
            ambienteAlterado = true;
        }
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.032f, .036f, .048f);
        RenderSettings.ambientIntensity = 1f;

        world = new GameObject(scene + (modoPreview ? " — Preview 3D" : " — Interior 3D")).transform;
        world.SetParent(transform, false);
        world.position = new Vector3(0f, 0f, 180f);

        var cameraObject = new GameObject("Camera" + scene + "3D", typeof(Camera));
        cameraObject.transform.SetParent(world, false);
        vista = cameraObject.GetComponent<Camera>();
        vista.cullingMask = modoPreview ? RoomMask : (RoomMask | ExteriorMask);
        vista.clearFlags = CameraClearFlags.SolidColor;
        vista.backgroundColor = new Color(.006f, .008f, .011f);
        vista.nearClipPlane = .025f;
        vista.farClipPlane = 65f;
        vista.fieldOfView = 62f;
        vista.depth = camRef != null ? camRef.depth + 1f : 1f;
        vista.allowHDR = true;
        var urp = cameraObject.AddComponent<UniversalAdditionalCameraData>();
        urp.SetRenderer(1);
        urp.volumeLayerMask = RoomMask;
        urp.renderPostProcessing = !modoPreview;
        urp.renderShadows = !modoPreview;

        var volumeGo = new GameObject("VolumeBloomInterior3D", typeof(Volume));
        volumeGo.layer = RoomLayer;
        volumeGo.transform.SetParent(world, false);
        var volume = volumeGo.GetComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 50f;
        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = scene + "_BloomProfile";
        var bloom = profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(1.05f);
        bloom.intensity.Override(.72f);
        bloom.scatter.Override(.68f);
        bloom.clamp.Override(6.0f);
        bloom.highQualityFiltering.Override(false);
        volume.sharedProfile = profile;
        owned.Add(profile);

        lanterna = new GameObject("LanternaMarcus3D", typeof(Light)).GetComponent<Light>();
        lanterna.gameObject.layer = RoomLayer;
        lanterna.transform.SetParent(vista.transform, false);
        lanterna.transform.localPosition = new Vector3(.12f, -.10f, .05f);
        lanterna.type = LightType.Spot;
        lanterna.color = new Color(1f, .90f, .72f);
        lanterna.intensity = 4.6f;
        lanterna.range = 16.0f;
        lanterna.spotAngle = 46f;
        lanterna.innerSpotAngle = 18f;
        lanterna.shadows = modoPreview ? LightShadows.None : LightShadows.Soft;
        lanterna.shadowStrength = .86f;
        lanterna.shadowBias = .02f;
        lanterna.shadowNormalBias = .03f;
        lanterna.shadowNearPlane = .12f;
        var lanternaUrp = lanterna.gameObject.AddComponent<UniversalAdditionalLightData>();
        lanternaUrp.additionalLightsShadowResolutionTier = 1;
        lanterna.cullingMask = RoomMask;
        lanterna.enabled = GameState.LanternaLigada;

        switch (scene)
        {
            case "IgrejaMatriz": MontarIgreja(); break;
            case "PrefeituraSantaRita": MontarPrefeitura(); break;
            case "RodoviariaSantaRita": MontarRodoviaria(); break;
            case "MercadinhoDeposito": MontarDeposito(); break;
            case "OficinaRadio": MontarOficina(); break;
            case "CasaCostureira": MontarCostureira(); break;
            case "AbrigoCemiterio": MontarAbrigo(); break;
            case "SociedadeBeiraRio": MontarSociedade(); break;
        }

        AjustarLuzIndireta(scene);

        if (incluirMao) ConstruirMao();
        foreach (var r in world.GetComponentsInChildren<Renderer>(true))
            if (r.name == "Volume de neblina") Destroy(r.gameObject);
        if (!modoPreview) PreAssarSombras(scene);

        target = new RenderTexture(
            larguraRt,
            Mathf.Max(1, Mathf.RoundToInt(larguraRt * Screen.height / (float)Mathf.Max(1, Screen.width))),
            24,
            RenderTextureFormat.ARGB32)
        {
            name = scene + (modoPreview ? "_Preview3D" : "_Offscreen3D"),
            antiAliasing = 2,
            filterMode = FilterMode.Bilinear
        };
        owned.Add(target);
        target.Create();
        vista.targetTexture = target;
    }

    private void MapearCena()
    {
        foreach (var item in FindObjectsByType<Capitulo2Interactable>(FindObjectsSortMode.None))
        {
            if (item.gameObject.scene != gameObject.scene) continue;
            if (!string.IsNullOrEmpty(item.interactionId) && !interactablesPorId.ContainsKey(item.interactionId))
                interactablesPorId.Add(item.interactionId, item);
        }
        foreach (var portal in FindObjectsByType<Capitulo2Portal>(FindObjectsSortMode.None))
        {
            if (portal.gameObject.scene != gameObject.scene) continue;
            if (portal.saidaInterior && portalPrincipal == null)
                portalPrincipal = portal;
            else if (portalPrincipal == null)
                portalPrincipal = portal;
        }
    }

    internal static void AnimarEntrada(float tempo)
    {
        if (Atual != null) Atual.AtualizarEntrada(tempo);
    }

    private static float Curva(float tempo, float inicio, float fim) =>
        Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inicio, fim, tempo));

    private void AtualizarEntrada(float tempo)
    {
        if (porta == null || macaneta == null) return;
        animandoEntrada = tempo < DuracaoEntrada;
        float abrir = Curva(tempo, 1.06f, 1.86f);
        float fechar = Curva(tempo, 2.65f, DuracaoEntrada);
        porta.localRotation = Quaternion.Euler(0f, 100f * abrir * (1f - fechar), 0f);

        if (!tocouFecho && tempo >= 1.06f)
        {
            tocouFecho = true;
            SantaRitaSound.Play("door", .20f);
        }
        if (!tocouPorta && tempo >= 2.95f)
        {
            tocouPorta = true;
            SantaRitaSound.Play("door", .10f, .88f);
        }

        var fora = new Vector3(pontoChegada.x, 1.45f, soleiraZ - 1.39f);
        float cruzar = Curva(tempo, 1.46f, 2.62f);
        var posicao = Vector3.Lerp(fora, pontoChegada, cruzar);
        posicao.y += Posto3D.MovimentoDaSoleira(cruzar);
        vista.transform.localPosition = posicao;
        var deFora = Quaternion.LookRotation(world.InverseTransformPoint(macaneta.position) - fora);
        vista.transform.localRotation = Quaternion.Slerp(deFora, Quaternion.Euler(pitch, yaw, 0f),
            Curva(tempo, 1.42f, 2.58f));
        vista.fieldOfView = Mathf.Lerp(55f, 62f, cruzar);

        vista.ResetProjectionMatrix();
        float soltarFoco = Curva(tempo, .76f, 1.43f);
        if (soltarFoco < 1f)
        {
            Vector3 ponto = vista.WorldToViewportPoint(macaneta.position);
            Vector2 deslocamento = (focoEntrada - (Vector2)ponto) * (2f * (1f - soltarFoco));
            var projecao = vista.projectionMatrix;
            projecao.m02 -= deslocamento.x;
            projecao.m12 -= deslocamento.y;
            vista.projectionMatrix = projecao;
        }

        AnimarMao(tempo, .78f, 1.06f, 1.50f, 1.84f);
        if (chuva3D != null) chuva3D.enabled = tempo < DuracaoEntrada;
        if (materialChuva != null) materialChuva.SetFloat("_Tempo", Time.unscaledTime);
        if (chuva != null) chuva.DefinirOclusaoPortaBar(fechar);
        OrientarPersonagens();
    }

    internal static void PrepararSaida()
    {
        if (Atual == null) return;
        Atual.inicioSaida = Atual.vista.transform.localPosition;
        Atual.inicioRotSaida = Atual.vista.transform.localRotation;
        Atual.tocouSaida = false;
    }

    internal static void AnimarEntradaIntegrada(float tempo)
    {
        if (Atual != null) Atual.AtualizarEntradaIntegrada(tempo);
    }

    internal static void AnimarSaidaIntegrada(float tempo)
    {
        if (Atual != null) Atual.AtualizarSaidaIntegrada(tempo);
    }

    private void AtualizarEntradaIntegrada(float tempo)
    {
        if (vista == null) return;
        animandoEntrada = tempo < DuracaoEntradaIntegrada;
        float fechar = Curva(tempo, 0f, DuracaoEntradaIntegrada);
        if (porta != null) porta.localRotation = Quaternion.Euler(0f, 100f * (1f - fechar), 0f);
        if (!tocouPorta && tempo >= DuracaoEntradaIntegrada * .75f)
        {
            tocouPorta = true;
            SantaRitaSound.Play("door", .12f, .88f);
        }

        vista.transform.localPosition = pontoChegada;
        vista.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        vista.fieldOfView = 62f;
        vista.ResetProjectionMatrix();
        if (chuva3D != null) chuva3D.enabled = tempo < DuracaoEntradaIntegrada * .85f;
        if (materialChuva != null) materialChuva.SetFloat("_Tempo", Time.unscaledTime);
        float oclusao = SceneFader.EmTransicaoAudioPortal && SceneFader.TransicaoAudioParaInterior
            ? Mathf.Lerp(.42f, 1f, fechar)
            : fechar;
        if (chuva != null) chuva.DefinirOclusaoPortaBar(oclusao);
        OrientarPersonagens();
    }

    private void AtualizarSaidaIntegrada(float tempo)
    {
        if (vista == null) return;
        float abertura = Curva(tempo, .12f, .68f);
        if (porta != null) porta.localRotation = Quaternion.Euler(0f, 100f * abertura, 0f);
        if (!tocouSaida && tempo >= .12f)
        {
            tocouSaida = true;
            SantaRitaSound.Play("door", .18f);
        }

        float olhar = Curva(tempo, .04f, .58f);
        float passo = Curva(tempo, .40f, DuracaoSaidaIntegrada);
        var soleiraLimite = new Vector3(pontoChegada.x, EyeHeight, soleiraZ + .14f);
        var pos = Vector3.Lerp(inicioSaida, soleiraLimite, passo);
        pos.y += Posto3D.MovimentoDaSoleira(passo) * .35f;
        vista.transform.localPosition = pos;

        Vector3 alvoOlhar = new Vector3(pontoChegada.x, 1.32f, soleiraZ - 1.65f);
        Vector3 dirOlhar = (alvoOlhar - pos).sqrMagnitude > .001f ? (alvoOlhar - pos) : Vector3.back;
        Quaternion rotSaida = Quaternion.LookRotation(dirOlhar, Vector3.up);
        vista.transform.localRotation = Quaternion.Slerp(inicioRotSaida, rotSaida, olhar);
        vista.fieldOfView = Mathf.Lerp(62f, 50f, passo);
        vista.ResetProjectionMatrix();

        AnimarMao(tempo, .02f, .22f, .56f, .80f);

        if (chuva3D != null) chuva3D.enabled = abertura > 0f && tempo < DuracaoSaidaIntegrada * .84f;
        if (materialChuva != null) materialChuva.SetFloat("_Tempo", Time.unscaledTime);
        if (chuva != null) chuva.DefinirOclusaoPortaBar(1f - abertura);
        OrientarPersonagens();
    }

    internal static void AnimarSaida(float tempo)
    {
        if (Atual != null) Atual.AtualizarSaida(tempo);
    }

    private void AtualizarSaida(float tempo)
    {
        if (porta == null) return;
        float abertura = Curva(tempo, .47f, 1.39f);
        porta.localRotation = Quaternion.Euler(0f, 100f * abertura, 0f);
        if (!tocouSaida && tempo >= .47f)
        {
            tocouSaida = true;
            SantaRitaSound.Play("door", .18f);
        }

        float passo = Curva(tempo, .83f, 1.62f);
        var soleira = new Vector3(pontoChegada.x, EyeHeight, soleiraZ + .23f);
        vista.transform.localPosition = Vector3.Lerp(inicioSaida, soleira, passo);
        AnimarMao(tempo, .08f, .47f, .97f, 1.29f);

        if (chuva3D != null) chuva3D.enabled = abertura > 0f;
        if (materialChuva != null) materialChuva.SetFloat("_Tempo", Time.unscaledTime);
        if (chuva != null) chuva.DefinirOclusaoPortaBar(1f - abertura);
        OrientarPersonagens();
    }

    private void AnimarMao(float tempo, float inicio, float pegar, float soltar, float fim)
    {
        if (mao == null || macaneta == null) return;
        bool visivel = tempo > inicio && tempo < fim;
        mao.gameObject.SetActive(visivel);
        if (!visivel) return;
        float alcance = Curva(tempo, inicio, pegar) * (1f - Curva(tempo, soltar, fim));
        var pulso = vista.ViewportToWorldPoint(new Vector3(.89f, -.25f, .57f));
        mao.position = Vector3.Lerp(pulso, macaneta.position - vista.transform.forward * .055f, alcance);
        mao.rotation = vista.transform.rotation * Quaternion.Euler(0f, 0f, -20f);
    }

    private void Update()
    {
        if (vista == null) return;
        if (modoPreview)
        {
            if (!MenuPausa.Aberto && Application.isFocused && Time.timeScale > 0f)
            {
                float dtPreview = Time.deltaTime;
                for (int i = 0; i < animacoes.Count; i++)
                    animacoes[i]?.Invoke(dtPreview);
            }
            return;
        }
        if (player == null) return;
        bool minigame = BochaMinigameUI.IsShowing;
        vista.enabled = !minigame;
        canvas.enabled = !minigame;

        if (!MenuPausa.Aberto && Application.isFocused && Time.timeScale > 0f)
        {
            float dtAnim = Time.deltaTime;
            for (int i = 0; i < animacoes.Count; i++)
                animacoes[i]?.Invoke(dtAnim);
        }

        if (minigame)
        {
            velocidade = Vector2.zero;
            return;
        }

        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);

        if (SceneFader.IsTransitioning || animandoEntrada || MenuPausa.Aberto || MapaInterativoUI.IsShowing ||
            SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen || PayphoneCheckpoint.IsSaveUIOpen ||
            !Application.isFocused || Time.timeScale == 0f)
        {
            velocidade = Vector2.zero;
            if (!SceneFader.IsTransitioning) Capturar(false);
            dica.text = "";
            return;
        }

        var teclado = Keyboard.current;
        var mouse = Mouse.current;
        if (GameMessageUI.IsShowing)
        {
            velocidade = Vector2.zero;
            Capturar(false);
            dica.text = "";
            if (teclado != null && (teclado.eKey.wasPressedThisFrame || teclado.spaceKey.wasPressedThisFrame))
                GameMessageUI.Instance.Avancar();
            return;
        }

        if (SantaRitaFalasEntrada.Interagindo)
        {
            velocidade = Vector2.zero;
            Capturar(false);
            dica.text = "";
            if ((teclado != null && (teclado.eKey.wasPressedThisFrame || teclado.spaceKey.wasPressedThisFrame)) ||
                (mouse != null && mouse.leftButton.wasPressedThisFrame))
                SantaRitaFalasEntrada.Avancar();
            return;
        }

        Capturar(true);
        if (teclado == null)
        {
            velocidade = Vector2.zero;
            return;
        }

        if (teclado.fKey.wasPressedThisFrame)
        {
            GameState.AlternarLanterna();
            if (lanterna != null) lanterna.enabled = GameState.LanternaLigada;
            SantaRitaSound.Play("Interiores/Interruptor", .26f, UnityEngine.Random.Range(.95f, 1.06f));
        }
        else if (lanterna != null && lanterna.enabled != GameState.LanternaLigada)
        {
            lanterna.enabled = GameState.LanternaLigada;
        }

        var olhar = mouse != null && frameCaptura != Time.frameCount ? mouse.delta.ReadValue() : Vector2.zero;
        yaw += olhar.x * .12f;
        pitch = Mathf.Clamp(pitch - olhar.y * .12f, -78f, 78f);
        vista.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector2 entrada = PlayerController.LerDirecao(teclado);
        float maximo = VelocidadeCaminhada;
        if (PlayerController.ShiftPressionado(teclado)) maximo *= PlayerController.MultiplicadorCorrida;
        float dt = Mathf.Min(Time.deltaTime, .05f);
        Vector3 alvoMov = Quaternion.Euler(0f, yaw, 0f) * new Vector3(entrada.x, 0f, entrada.y) * maximo;
        velocidade = PlayerController.AjustarVelocidade(velocidade, new Vector2(alvoMov.x, alvoMov.z), dt);
        Vector3 antes = vista.transform.localPosition;
        Mover(new Vector3(velocidade.x, 0f, velocidade.y) * dt);
        float dist = Vector3.Distance(antes, vista.transform.localPosition);
        if (dist > .0001f)
        {
            passoAcumulado += dist;
            if (passoAcumulado >= .68f)
            {
                passoAcumulado = 0f;
                indicePasso = (indicePasso + 1) % 3;
                string prefixo = somPasso.Substring(0, somPasso.Length - 1);
                SantaRitaSound.Play(prefixo + indicePasso, .075f, UnityEngine.Random.Range(.93f, 1.07f));
            }
        }
        OrientarPersonagens();

        Collider foco = null;
        if (Physics.Raycast(vista.transform.position, vista.transform.forward, out var hit, 2.75f, RoomMask, QueryTriggerInteraction.Collide))
            foco = hit.collider;

        bool podeSair = foco != null && foco == saida && portalPrincipal != null;
        AlvoPortal portalAlvo = default;
        bool temPortal = foco != null && portais.TryGetValue(foco, out portalAlvo);
        AlvoInteracao alvo = default;
        bool temAlvo = foco != null && alvos.TryGetValue(foco, out alvo);

        if (podeSair)
            dica.text = "·\n[E] " + portalPrincipal.Prompt;
        else if (temPortal)
            dica.text = "·\n[E] " + portalAlvo.Prompt;
        else if (temAlvo)
            dica.text = "·\n[E] " + ResolverPrompt(alvo);
        else
            dica.text = "·";

        bool acionar = teclado.eKey.wasPressedThisFrame ||
                       (mouse != null && mouse.leftButton.wasPressedThisFrame && frameCaptura != Time.frameCount);
        if (!acionar) return;

        if (podeSair)
        {
            SairPeloPortal(portalPrincipal);
        }
        else if (temPortal)
        {
            Capturar(false);
            portalAlvo.Portal.Interact();
        }
        else if (temAlvo)
        {
            if (alvo.AoAcionar != null)
            {
                alvo.AoAcionar();
                return;
            }
            if (alvo.Fonte != null)
            {
                Capturar(false);
                alvo.Fonte.Interact();
            }
            else if (!string.IsNullOrEmpty(alvo.Id) && Capitulo2Director.Instance != null)
            {
                Capturar(false);
                Capitulo2Director.Instance.Interagir(alvo.Id);
            }
        }
    }

    private string ResolverPrompt(AlvoInteracao alvo)
    {
        if (!string.IsNullOrEmpty(alvo.Prompt)) return alvo.Prompt;
        if (alvo.Fonte != null) return alvo.Fonte.Prompt;
        var dados = !string.IsNullOrEmpty(alvo.Id) ? Capitulo2Director.Dados(alvo.Id) : null;
        return dados != null && !string.IsNullOrEmpty(dados.prompt) ? dados.prompt : "Examinar";
    }

    internal void DispararSaidaPeloPortal(Capitulo2Portal portal) => SairPeloPortal(portal);

    private void SairPeloPortal(Capitulo2Portal portal)
    {
        if (portal == null || SceneFader.IsTransitioning) return;
        Capturar(false);
        PrepararSaida();
        if (portal.cenaDestino == "CentroSantaRita" && Capitulo2CentroPortalAnim.SuportaCena(gameObject.scene.name))
        {
            Capitulo2CentroPortalAnim.Sair(this, portal);
            return;
        }
        SceneFader.Instance.TransicionarPortaPosto(
            FocoDaPorta,
            () => SceneTransition.PrepararEntrada(portal.entradaDestino),
            () => Vector2.one * .5f,
            animarAntes: AnimarSaida,
            antesDaNevoa: SaidaAntesDaNevoa,
            cenaDestino: portal.cenaDestino);
    }

    private void Mover(Vector3 delta)
    {
        int passos = Mathf.Max(1, Mathf.CeilToInt(delta.magnitude / .04f));
        Vector3 passo = delta / passos;
        for (int i = 0; i < passos; i++)
            MoverPasso(passo);
    }

    private void MoverPasso(Vector3 delta)
    {
        Vector3 pe = vista.transform.position - Vector3.up * EyeHeight;
        Vector3 baixo = pe + Vector3.up * .26f;
        Vector3 alto = pe + Vector3.up * 1.42f;
        const float raio = .23f;

        var lateral = new Vector3(delta.x, 0f, 0f);
        if (lateral.x != 0f && !Physics.CheckCapsule(baixo + lateral, alto + lateral, raio, RoomMask, QueryTriggerInteraction.Ignore))
        {
            vista.transform.position += lateral;
            baixo += lateral;
            alto += lateral;
        }
        else if (lateral.x != 0f)
        {
            velocidade.x = 0f;
        }

        var longitudinal = new Vector3(0f, 0f, delta.z);
        if (longitudinal.z != 0f && !Physics.CheckCapsule(baixo + longitudinal, alto + longitudinal, raio, RoomMask, QueryTriggerInteraction.Ignore))
            vista.transform.position += longitudinal;
        else if (longitudinal.z != 0f)
            velocidade.y = 0f;
    }

    private void OrientarPersonagens()
    {
        if (vista == null) return;
        foreach (var p in personagens)
        {
            if (p == null) continue;
            var delta = p.position - vista.transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > .001f) p.rotation = Quaternion.LookRotation(delta);
        }
    }

    private void Capturar(bool capturar)
    {
        var modo = capturar ? CursorLockMode.Locked : CursorLockMode.None;
        if (Cursor.lockState != modo)
        {
            Cursor.lockState = modo;
            if (capturar) frameCaptura = Time.frameCount;
        }
        Cursor.visible = !capturar;
    }

    internal static Vector3 PendenteNoEcra(Camera fallback)
    {
        if (Atual != null && Atual.vista != null)
            return Atual.vista.WorldToScreenPoint(Atual.world.TransformPoint(Atual.pendenteSociedade));
        return fallback != null ? fallback.WorldToScreenPoint(SantaRitaBochaAmbiente.PendenteDaEntrada) : Vector3.zero;
    }

    internal static bool TryFocus(Interactable targetInteractable, out Camera camera, out Bounds bounds)
    {
        camera = null;
        bounds = default;
        if (Atual == null || Atual.vista == null || targetInteractable == null) return false;
        string id = targetInteractable is Capitulo2Interactable cue ? cue.interactionId : targetInteractable.name;
        if (!string.IsNullOrEmpty(id) && Atual.boundsPorId.TryGetValue(id, out var b))
        {
            bounds = b;
            camera = Atual.vista;
            return true;
        }
        bounds = new Bounds(Atual.vista.transform.position + Atual.vista.transform.forward * 1.6f, new Vector3(.6f, .6f, .6f));
        camera = Atual.vista;
        return true;
    }

    private void LiberarRua()
    {
        if (ambienteAlterado)
        {
            ambienteAlterado = false;
            RenderSettings.ambientMode = previousAmbientMode;
            RenderSettings.ambientLight = previousAmbientLight;
            RenderSettings.ambientSkyColor = previousAmbientSky;
            RenderSettings.ambientEquatorColor = previousAmbientEquator;
            RenderSettings.ambientGroundColor = previousAmbientGround;
            RenderSettings.ambientIntensity = previousAmbientIntensity;
        }
        if (!ruaOcluida) return;
        ruaOcluida = false;
        if (baseCamera != null)
        {
            baseCamera.cullingMask = previousMask;
            baseCamera.clearFlags = previousClear;
            baseCamera.backgroundColor = previousBackground;
        }
        if (player != null) player.enabled = true;
        if (chuva != null) chuva.DefinirModoInterior(false);
        Cursor.lockState = cursorAnterior;
        Cursor.visible = cursorVisivel;
    }

    private void OnDestroy()
    {
        LiberarRua();
        if (Atual == this) Atual = null;
        if (vista != null) vista.targetTexture = null;
        if (target != null) target.Release();
        foreach (var item in owned)
            if (item != null) Destroy(item);
    }
}
