using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Tipo = InteracaoCapitulo1.TipoInteracao;

internal sealed partial class Posto3D : MonoBehaviour
{
    internal const float DuracaoEntrada = 3.05f;
    internal const float SaidaAntesDaNevoa = .96f;
    private const int Layer = 30;
    private const int RoomMask = 1 << Layer;
    private const float Soleira = -4.5f;
    private const float EyeHeight = 1.61f;
    private const float VelocidadeCaminhada = 2.45f;
    private static readonly Vector3 Chegada = new Vector3(.31f, EyeHeight, -2.94f);
    private static Posto3D atual;
    internal static bool Aberto => atual != null;
    internal static Vector2 FocoDaPorta
    {
        get
        {
            if (atual == null) return Vector2.one * .5f;
            Vector3 ponto = atual.vista.WorldToViewportPoint(atual.macaneta.position);
            return ponto.z > 0f ? new Vector2(ponto.x, ponto.y) : Vector2.one * .5f;
        }
    }

    private readonly List<Object> recursos = new List<Object>();
    private readonly Dictionary<Collider, Tipo> alvos = new Dictionary<Collider, Tipo>();
    private readonly List<Transform> personagens = new List<Transform>();
    private Camera vista, rua;
    private int mascaraRua, frameCaptura;
    private Chuva chuva;
    private Canvas canvas;
    private RenderTexture target;
    private TextMeshProUGUI dica;
    private InteracaoCapitulo1 interacao;
    private Transform cenario, porta, macaneta, mao;
    private MeshRenderer chuva3D;
    private Material materialChuva;
    private Collider saida;
    private PlayerController jogador;
    private bool jogadorAtivo, ruaOcluida, tocouFecho, tocouPorta, cursorVisivel;
    private CursorLockMode cursorAnterior;
    private float yaw, pitch;
    private Vector2 focoEntrada;
    private Vector2 velocidade;
    private Vector3 inicioDaSaida;
    internal enum Interior { Posto, Mercadinho, CasaDoAvo }
    private bool tocouSaida;
    private Interior interior;
    private bool Casa => interior == Interior.CasaDoAvo;
    private Vector3 PontoDeChegada => Casa ? ChegadaCasa : Chegada;
    private float AnguloDaPorta(float abertura) => Casa
        ? Mathf.Lerp(PortaDaCasaEntreaberta, -103f, abertura) : 100f * abertura;

    internal static void Abrir(bool animar, Vector2 foco = default,
        Interior interior = Interior.Posto, bool vindoDoDeposito = false)
    {
        if (Aberto) return;
        var posto = new GameObject("Exploracao" + interior + "3D").AddComponent<Posto3D>();
        posto.interior = interior;
        atual = posto;
        try
        {
            posto.Inicializar(animar, foco, vindoDoDeposito);
        }
        catch
        {
            Fechar();
            throw;
        }
    }

    private void Inicializar(bool animar, Vector2 foco, bool vindoDoDeposito)
    {
        focoEntrada = foco;
        cursorAnterior = Cursor.lockState;
        cursorVisivel = Cursor.visible;
        cenario = new GameObject(interior + " — Interior 3D").transform;
        cenario.SetParent(transform, false);
        cenario.position = new Vector3(0f, 0f, 180f);
        vista = new GameObject("Camera" + interior + "3D").AddComponent<Camera>();
        vista.transform.SetParent(cenario, false);
        vista.cullingMask = RoomMask;
        vista.clearFlags = CameraClearFlags.SolidColor;
        vista.backgroundColor = new Color(.024f, .030f, .033f);
        vista.nearClipPlane = .025f;
        vista.farClipPlane = 35f;
        vista.depth = -10f;
        vista.allowHDR = false;
        var pipeline = vista.gameObject.AddComponent<UniversalAdditionalCameraData>();
        pipeline.SetRenderer(1);
        pipeline.volumeLayerMask = 0;
        pipeline.renderPostProcessing = false;

        rua = Camera.main;
        switch (interior)
        {
            case Interior.Mercadinho: ConstruirMercadinho(); break;
            case Interior.CasaDoAvo: ConstruirCasaDoAvo(); break;
            default: ConstruirSala(); break;
        }
        var exterior = Casa ? ExteriorInterior3D.Local.Casa : interior == Interior.Mercadinho
            ? ExteriorInterior3D.Local.Mercadinho : ExteriorInterior3D.Local.Posto;
        ExteriorInterior3D.Montar(cenario, vista, Layer, V(0,0,Soleira-.03f), 180f, exterior);
        if (Casa) ExteriorInterior3D.Montar(cenario, vista, Layer, V(0,0,4.98f), 0f, ExteriorInterior3D.Local.Jardim);
        canvas = UIFactory.CreateCanvas(transform, interior + "Canvas", 6400);
        var imagem = new GameObject("Vista" + interior, typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
        imagem.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(imagem.rectTransform);
        imagem.raycastTarget = false;
        target = new RenderTexture(960, Mathf.Max(1, Mathf.RoundToInt(960f * Screen.height / Mathf.Max(1, Screen.width))), 24, RenderTextureFormat.ARGB32)
        {
            name = interior + "_Offscreen",
            antiAliasing = 2,
            filterMode = FilterMode.Bilinear
        };
        recursos.Add(target);
        target.Create();
        vista.targetTexture = target;
        imagem.texture = target;
        dica = UIFactory.CreateLabel(canvas.transform, "Interação no interior", 13f);
        dica.raycastTarget = false;
        dica.alignment = TextAlignmentOptions.Center;
        dica.rectTransform.anchorMin = dica.rectTransform.anchorMax = new Vector2(.5f, .5f);
        dica.rectTransform.anchoredPosition = new Vector2(0f, -28f);
        dica.rectTransform.sizeDelta = new Vector2(460f, 42f);
        interacao = gameObject.AddComponent<InteracaoCapitulo1>();
        interacao.enabled = false;

        jogador = FindAnyObjectByType<PlayerController>();
        chuva = FindAnyObjectByType<Chuva>();
        if (rua != null)
        {
            mascaraRua = rua.cullingMask;
            rua.cullingMask = 0;
        }
        if (jogador != null)
        {
            jogadorAtivo = jogador.enabled;
            jogador.enabled = false;
        }
        ruaOcluida = true;
        InteractionPrompt.Instance.SetTarget(null);
        if (chuva != null) chuva.DefinirModoInterior(true, animar ? 0f : 1f);

        yaw = vindoDoDeposito ? 180f : 0f;
        pitch = 3f;
        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);
        vista.transform.localPosition = vindoDoDeposito ? new Vector3(3.67f, EyeHeight, 3.62f) : PontoDeChegada;
        vista.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        if (animar) AtualizarEntrada(0f);
        else
        {
            if (chuva3D != null) chuva3D.enabled = false;
            OrientarPersonagens();
        }
        Physics.SyncTransforms();
    }

    internal static void AnimarEntrada(float tempo)
    {
        if (atual != null) atual.AtualizarEntrada(tempo);
    }

    private static float Curva(float tempo, float inicio, float fim) =>
        Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inicio, fim, tempo));

    private void AtualizarEntrada(float tempo)
    {
        float abrir = Curva(tempo, 1.06f, 1.86f);
        float fechar = Curva(tempo, 2.65f, DuracaoEntrada);
        porta.localRotation = Quaternion.Euler(0f, AnguloDaPorta(abrir * (1f - fechar)), 0f);
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

        if (tempo >= 1.43f) SoarSino();

        var fora = new Vector3(Casa ? .06f : .36f, 1.45f, Soleira - 1.39f);
        float cruzar = Curva(tempo, 1.46f, 2.62f);
        var posicao = Vector3.Lerp(fora, PontoDeChegada, cruzar);
        posicao.y += MovimentoDaSoleira(cruzar);
        vista.transform.localPosition = posicao;
        var deFora = Quaternion.LookRotation(cenario.InverseTransformPoint(macaneta.position) - fora);
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
        if (chuva != null) chuva.DefinirOclusaoPortaBar(Casa ? .82f * fechar : fechar);
        OrientarPersonagens();
    }

    internal static float MovimentoDaSoleira(float passo)
    {
        if (MenuPausa.ReduzirEfeitosIntensos) return 0f;
        return Mathf.Sin(passo * Mathf.PI * 4f) * Mathf.Sin(passo * Mathf.PI) * .026f;
    }

    internal static void PrepararSaida()
    {
        if (atual == null) return;
        atual.inicioDaSaida = atual.vista.transform.localPosition;
        atual.tocouSaida = false;
        atual.sinoTocou = false;
    }

    internal static void AnimarSaida(float tempo)
    {
        if (atual == null) return;
        atual.AtualizarSaida(tempo);
    }

    private void AtualizarSaida(float tempo)
    {
        float abertura = Curva(tempo, .47f, 1.39f);
        porta.localRotation = Quaternion.Euler(0f, AnguloDaPorta(abertura), 0f);
        if (!tocouSaida && tempo >= .47f)
        {
            tocouSaida = true;
            SantaRitaSound.Play("door", .18f);
        }

        if (tempo >= .79f) SoarSino();

        float passo = Curva(tempo, .83f, 1.62f);
        var soleira = new Vector3(Casa ? .06f : .38f, EyeHeight, Soleira + .23f);
        vista.transform.localPosition = Vector3.Lerp(inicioDaSaida, soleira, passo);
        AnimarMao(tempo, .08f, .47f, .97f, 1.29f);

        if (chuva3D != null) chuva3D.enabled = abertura > 0f;
        if (materialChuva != null) materialChuva.SetFloat("_Tempo", Time.unscaledTime);
        if (chuva != null) chuva.DefinirOclusaoPortaBar(1f - abertura);
        OrientarPersonagens();
    }

    private void AnimarMao(float tempo, float inicio, float pegar, float soltar, float fim)
    {
        if (mao == null) return;
        bool visivel = tempo > inicio && tempo < fim;
        mao.gameObject.SetActive(visivel);
        if (!visivel) return;
        float alcance = Curva(tempo, inicio, pegar) * (1f - Curva(tempo, soltar, fim));
        var pulso = vista.ViewportToWorldPoint(new Vector3(.89f, -.25f, .57f));
        mao.position = Vector3.Lerp(pulso, macaneta.position - vista.transform.forward * .055f, alcance);
        mao.rotation = vista.transform.rotation * Quaternion.Euler(0f, 0f, -20f);
    }

    internal static void Fechar()
    {
        if (atual == null) return;
        var posto = atual;
        atual = null;
        posto.LiberarRua();
        posto.gameObject.SetActive(false);
        Destroy(posto.gameObject);
    }

    private void LiberarRua()
    {
        if (!ruaOcluida) return;
        ruaOcluida = false;
        if (rua != null) rua.cullingMask = mascaraRua;
        if (jogador != null) jogador.enabled = jogadorAtivo;
        if (chuva != null) chuva.DefinirModoInterior(false);
        Cursor.lockState = cursorAnterior;
        Cursor.visible = cursorVisivel;
    }

    private void OnDestroy()
    {
        if (atual == this)
        {
            atual = null;
            LiberarRua();
        }
        if (vista != null) vista.targetTexture = null;
        if (target != null) target.Release();
        foreach (var recurso in recursos) Destroy(recurso);
    }

    private void Update()
    {
        if (!MenuPausa.Aberto && Application.isFocused && Time.timeScale > 0f)
            AnimarDetalhes(Time.deltaTime);
        vista.aspect = Screen.width / (float)Mathf.Max(1, Screen.height);
        if (SceneFader.IsTransitioning || MenuPausa.Aberto || MapaInterativoUI.IsShowing ||
            SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen || PayphoneCheckpoint.IsSaveUIOpen ||
            !Application.isFocused || Time.timeScale == 0f)
        {
            velocidade = Vector2.zero;
            Capturar(false);
            dica.text = "";
            return;
        }

        var teclado = Keyboard.current;
        if (GameMessageUI.IsShowing)
        {
            velocidade = Vector2.zero;
            Capturar(false);
            dica.text = "";
            if (teclado != null && teclado.eKey.wasPressedThisFrame) GameMessageUI.Instance.Avancar();
            return;
        }

        if (SantaRitaFalasEntrada.Interagindo)
        {
            velocidade = Vector2.zero;
            Capturar(false);
            dica.text = "";
            if (teclado != null && teclado.eKey.wasPressedThisFrame) SantaRitaFalasEntrada.Avancar();
            return;
        }

        Capturar(true);
        if (teclado == null)
        {
            velocidade = Vector2.zero;
            return;
        }
        if (lanternaCasa != null)
        {
            if (teclado.fKey.wasPressedThisFrame) GameState.AlternarLanterna();
            lanternaCasa.enabled = GameState.LanternaLigada;
        }
        var mouse = Mouse.current;
        var olhar = mouse != null && frameCaptura != Time.frameCount ? mouse.delta.ReadValue() : Vector2.zero;
        yaw += olhar.x * .12f;
        pitch = Mathf.Clamp(pitch - olhar.y * .12f, -68f, 68f);
        vista.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector2 entrada = PlayerController.LerDirecao(teclado);
        float maximo = VelocidadeCaminhada;
        if (PlayerController.ShiftPressionado(teclado)) maximo *= PlayerController.MultiplicadorCorrida;
        float dt = Mathf.Min(Time.deltaTime, .05f);
        Vector3 alvo = Quaternion.Euler(0f, yaw, 0f) * new Vector3(entrada.x, 0f, entrada.y) * maximo;
        velocidade = PlayerController.AjustarVelocidade(velocidade, new Vector2(alvo.x, alvo.z), dt);
        Vector3 antesDoPasso = vista.transform.localPosition;
        Mover(new Vector3(velocidade.x, 0f, velocidade.y) * dt);
        var depoisDoPasso = vista.transform.localPosition;
        PassosNoInterior(Vector3.Distance(antesDoPasso, depoisDoPasso), depoisDoPasso.z, depoisDoPasso.x);
        OrientarPersonagens();

        Collider foco = null;
        if (Physics.Raycast(vista.transform.position, vista.transform.forward, out var hit, 2.55f, RoomMask, QueryTriggerInteraction.Collide))
            foco = hit.collider;
        bool podeSair = foco != null && foco == saida;
        string promptFisico = PromptFisico(foco);
        bool podeAbrirDeposito = foco != null && foco == portaDeposito;
        bool temAlvo = foco != null && alvos.TryGetValue(foco, out _);
        if (temAlvo) interacao.Configurar(alvos[foco]);
        dica.text = podeSair ? interior switch
        {
            Interior.CasaDoAvo => "·\n[E] Sair para o bairro",
            Interior.Mercadinho => "·\n[E] Sair para a encruzilhada",
            _ => "·\n[E] Sair para a avenida"
        } :
            podeAbrirDeposito ? "·\n[E] Abrir a porta de serviço" :
            promptFisico != null ? "·\n[E] " + promptFisico :
            temAlvo ? "·\n[E] " + interacao.Prompt : "·";
        if (!teclado.eKey.wasPressedThisFrame &&
            (mouse == null || !mouse.leftButton.wasPressedThisFrame || frameCaptura == Time.frameCount)) return;
        if (podeSair)
        {
            Capturar(false);
            switch (interior)
            {
                case Interior.CasaDoAvo: Capitulo1Director.Instance.SairDaCasaDoAvo3D(); break;
                case Interior.Mercadinho: Capitulo1Director.Instance.SairDoMercadinho3D(); break;
                default: Capitulo1Director.Instance.SairDoPosto3D(); break;
            }
        }
        else if (podeAbrirDeposito)
        {
            Capturar(false);
            acessoDeposito.Interact();
        }
        else if (promptFisico != null)
        {
            AcionarObjeto(foco);
        }
        else if (temAlvo)
        {
            Capturar(false);
            interacao.DefinirFocoInterior(vista, foco.bounds);
            interacao.Interact();
        }
    }

    private void Mover(Vector3 delta)
    {
        // Limita cada teste de cápsula para conservar a colisão em corrida.
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
            velocidade.x = 0f;

        var longitudinal = new Vector3(0f, 0f, delta.z);
        if (longitudinal.z != 0f && !Physics.CheckCapsule(baixo + longitudinal, alto + longitudinal, raio, RoomMask, QueryTriggerInteraction.Ignore))
            vista.transform.position += longitudinal;
        else if (longitudinal.z != 0f)
            velocidade.y = 0f;
    }

    private void OrientarPersonagens()
    {
        foreach (var personagem in personagens)
        {
            var delta = personagem.position - vista.transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > .001f) personagem.rotation = Quaternion.LookRotation(delta);
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
}
