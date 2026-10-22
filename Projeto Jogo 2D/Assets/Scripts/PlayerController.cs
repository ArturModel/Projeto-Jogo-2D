using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    // Passo cadenciado, deliberado: sem corrida, com peso ao iniciar e parar.
    [SerializeField] private float moveSpeed = 2.4f;
    [SerializeField] private float acceleration = 14f;
    [SerializeField] private float deceleration = 20f;

    [Header("Animação opcional")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator outfitAnimator;

    // 0 = Baixo, 1 = Cima, 2 = Esquerda, 3 = Direita
    private const int DirDown = 0;
    private const int DirUp = 1;
    private const int DirLeft = 2;
    private const int DirRight = 3;

    [Header("Interação")]
    [SerializeField] private float interactDuration = 0.6f;

    [Header("Lanterna")]
    [SerializeField] private Transform flashlight;
    [SerializeField] private float flashlightDistance = 0.3f;

    // Altura da mão a partir dos pés (o pivô do sprite agora está nos pés).
    [SerializeField] private float flashlightHandHeight = 1f;

    // Mesmo grid dos sprites. A luz precisa andar de pixel em pixel junto com
    // eles: se ela desliza suave enquanto os sprites saltam, o degradê escorrega
    // em relação às bordas e aparece um contorno tremendo durante o movimento.
    [SerializeField] private int pixelsPerUnit = 32;

    // Ajuste único: quantos graus somar ao ângulo "natural" do vetor de
    // direção (Mathf.Atan2) para compensar a orientação de 0° da luz.
    // Achando esse valor certo para UMA direção, as outras três direções
    // acompanham automaticamente — não são 4 números soltos e independentes.
    [SerializeField] private float flashlightAngleOffset = -90f;

    private static readonly Vector2[] FlashlightDirections =
    {
        Vector2.down, Vector2.up, Vector2.left, Vector2.right,
    };

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private int facingDirection = DirDown;
    private float interactTimer;
    private string estadoAnimacao;
    private SpriteRenderer corpo;
    private Sprite[,] quadrosCaminhada;
    private Sprite[] quadrosRepouso;
    private Sprite[,] quadrosCaminhadaComLanterna;
    private SpriteRenderer lanternaMaoRenderer;
    private Sprite spriteLanternaHorizontal;
    private Sprite spriteLanternaBaixo;
    private Sprite spriteLanternaCima;
    private readonly MarcusGait passada = new MarcusGait();
    private Vector2 ultimaPosicaoAnimacao;
    private int totalQuadros = 6;
    public int QuadroCaminhada { get; private set; }
    public event System.Action PeApoiou;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        ultimaPosicaoAnimacao = rb.position;
        corpo = GetComponent<SpriteRenderer>();
        if (corpo != null)
        {
            // Garante que Marcus sempre fique ACIMA de móveis, balcões, carros e casas (que usam sortingOrder <= 4).
            corpo.sortingOrder = 10;
        }
        PrepararCaminhadaMarcus();
        CriarLanternaNaMao();
    }

    private void CriarLanternaNaMao()
    {
        // Cria sprites em pixel art da lanterna preta/prateada com lente amarela acesa
        spriteLanternaHorizontal = GerarSpriteLanterna(0);
        spriteLanternaBaixo = GerarSpriteLanterna(1);
        spriteLanternaCima = GerarSpriteLanterna(2);

        GameObject goLanternaMao = new GameObject("LanternaNaMao");
        goLanternaMao.transform.SetParent(transform, false);
        lanternaMaoRenderer = goLanternaMao.AddComponent<SpriteRenderer>();
        lanternaMaoRenderer.sprite = spriteLanternaBaixo;
        lanternaMaoRenderer.sortingOrder = 11;
        goLanternaMao.SetActive(false);
    }

    private static Sprite GerarSpriteLanterna(int orientacao)
    {
        // 0 = horizontal (apontando para a esquerda no espaço local, pois DirRight espelha localScale.x = -1)
        // 1 = baixo
        // 2 = cima
        int w = 12, h = 12;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        Color[] clear = new Color[w * h];
        tex.SetPixels(clear);

        Color32 maoPele = new Color32(214, 162, 126, 255);
        Color32 corpoMetal = new Color32(42, 46, 52, 255);
        Color32 anelPrata = new Color32(150, 158, 168, 255);
        Color32 lenteLuz = new Color32(255, 244, 175, 255);
        Color32 borda = new Color32(18, 20, 24, 255);

        if (orientacao == 0)
        {
            // Mão segurando cabo (direita -> lente na esquerda, x=2..8, y=4..7)
            for (int x = 3; x <= 8; x++)
            {
                tex.SetPixel(x, 4, borda);
                tex.SetPixel(x, 5, corpoMetal);
                tex.SetPixel(x, 6, corpoMetal);
                tex.SetPixel(x, 7, borda);
            }
            // Mão de Marcus segurando o corpo da lanterna
            tex.SetPixel(6, 5, maoPele);
            tex.SetPixel(7, 5, maoPele);
            tex.SetPixel(6, 6, maoPele);
            // Anel prateado e lente frontal na esquerda (x=2, x=1)
            tex.SetPixel(3, 5, anelPrata);
            tex.SetPixel(3, 6, anelPrata);
            tex.SetPixel(2, 4, anelPrata);
            tex.SetPixel(2, 5, lenteLuz);
            tex.SetPixel(2, 6, lenteLuz);
            tex.SetPixel(2, 7, anelPrata);
            tex.SetPixel(1, 5, lenteLuz);
            tex.SetPixel(1, 6, lenteLuz);
        }
        else if (orientacao == 1)
        {
            // Apontando para baixo (y=8 cabo/mão -> y=2 lente)
            for (int y = 3; y <= 8; y++)
            {
                tex.SetPixel(4, y, borda);
                tex.SetPixel(5, y, corpoMetal);
                tex.SetPixel(6, y, corpoMetal);
                tex.SetPixel(7, y, borda);
            }
            tex.SetPixel(5, 6, maoPele);
            tex.SetPixel(6, 6, maoPele);
            tex.SetPixel(5, 7, maoPele);
            tex.SetPixel(5, 3, anelPrata);
            tex.SetPixel(6, 3, anelPrata);
            tex.SetPixel(4, 2, anelPrata);
            tex.SetPixel(5, 2, lenteLuz);
            tex.SetPixel(6, 2, lenteLuz);
            tex.SetPixel(7, 2, anelPrata);
            tex.SetPixel(5, 1, lenteLuz);
            tex.SetPixel(6, 1, lenteLuz);
        }
        else
        {
            // Apontando para cima (y=3 cabo/mão -> y=9 lente)
            for (int y = 3; y <= 8; y++)
            {
                tex.SetPixel(4, y, borda);
                tex.SetPixel(5, y, corpoMetal);
                tex.SetPixel(6, y, corpoMetal);
                tex.SetPixel(7, y, borda);
            }
            tex.SetPixel(5, 4, maoPele);
            tex.SetPixel(6, 4, maoPele);
            tex.SetPixel(5, 8, anelPrata);
            tex.SetPixel(6, 8, anelPrata);
            tex.SetPixel(5, 9, lenteLuz);
            tex.SetPixel(6, 9, lenteLuz);
            tex.SetPixel(5, 10, lenteLuz);
            tex.SetPixel(6, 10, lenteLuz);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
    }

    private static bool DeveUsarLanternaAgora()
    {
        if (!GameState.Tem(Flags.LanternaDoCarroObtida))
            return false;

        // Regra solicitada: Marcus só fica com a lanterna na rua; em interiores fica SEM lanterna.
        if (Capitulo1Director.EstaEmInterior || (Capitulo2Director.Instance != null && Capitulo2Director.Instance.interior))
            return false;

        string cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (cena == "CasaDoAvo" || cena == "BarDoZe")
            return false;

        return true;
    }

    private void PrepararCaminhadaMarcus()
    {
        string arquivo = Path.Combine(Application.streamingAssetsPath, "MarcusWalkFluido.png");
        if (!File.Exists(arquivo)) arquivo = Path.Combine(Application.streamingAssetsPath, "MarcusWalk.png");
        if (corpo == null || !File.Exists(arquivo)) return;
        Texture2D folha = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(folha, File.ReadAllBytes(arquivo))) return;
        folha.filterMode = FilterMode.Point;
        folha.wrapMode = TextureWrapMode.Clamp;
        totalQuadros = folha.width >= 512 ? 8 : 6;
        quadrosCaminhada = new Sprite[3, totalQuadros];
        for (int direcao = 0; direcao < 3; direcao++)
            for (int passo = 0; passo < totalQuadros; passo++)
                quadrosCaminhada[direcao, passo] = Sprite.Create(folha,
                    new Rect(passo * 64, folha.height - (direcao + 1) * 64, 64, 64),
                    new Vector2(0.5f, 0.125f), 32f, 0, SpriteMeshType.FullRect);
        quadrosRepouso = new Sprite[3];
        for (int direcao = 0; direcao < 3; direcao++)
            quadrosRepouso[direcao] = folha.width >= 576 ? Sprite.Create(folha,
                new Rect(512, folha.height - (direcao + 1) * 64, 64, 64),
                new Vector2(.5f, .125f), 32f, 0, SpriteMeshType.FullRect) : quadrosCaminhada[direcao, 0];

        // A arte anterior trazia seis quadros no Animator, mas quase todos
        // eram cópias. O sprite novo já inclui as roupas em cada pose.
        if (animator != null) animator.enabled = false;
        if (outfitAnimator != null)
        {
            outfitAnimator.enabled = false;
            SpriteRenderer roupa = outfitAnimator.GetComponent<SpriteRenderer>();
            if (roupa != null) roupa.enabled = false;
        }
        corpo.sprite = quadrosCaminhada[0, 0];
    }

    private void Start()
    {
        // Garante que o controlador do Mapa Interativo (tecla M / Tab) esteja ativo na cena.
        MapaInterativoUI.EnsureInstance();
        bool usarLanterna = DeveUsarLanternaAgora();
        if (flashlight != null) flashlight.gameObject.SetActive(usarLanterna);
        if (lanternaMaoRenderer != null) lanternaMaoRenderer.gameObject.SetActive(usarLanterna);

        // Ordem importa: atravessar uma porta é mais recente que o último
        // checkpoint, então a entrada da porta ganha.
        if (SceneTransition.ConsumirEntrada(out Vector2 entrada, out int direcao))
        {
            TeleportarPara(entrada, direcao);
            return;
        }

        // Vindo de "Continuar": nasce no último orelhão registrado.
        if (CheckpointSystem.ConsumePendingSpawn(out Vector2 spawn))
        {
            TeleportarPara(spawn, -1);
        }
    }

    public void TeleportarPara(Vector2 destino, int direcao = -1)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = destino;
            rb.linearVelocity = Vector2.zero;
        }
        transform.position = destino;
        currentVelocity = Vector2.zero;
        moveInput = Vector2.zero;
        ultimaPosicaoAnimacao = destino;
        passada.Reset();
        estadoAnimacao = null;

        if (direcao >= DirDown && direcao <= DirRight)
        {
            facingDirection = direcao;
            if (facingDirection == DirRight) SetFacingScale(-1f);
            else if (facingDirection == DirLeft) SetFacingScale(1f);
        }

        CameraFollow camFollow = FindAnyObjectByType<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.SnapImediatoParaAlvo();
        }
    }

    private void Teleportar(Vector2 destino)
    {
        TeleportarPara(destino, -1);
    }

    private void Update()
    {
        // Com a pausa, mapa, orelhão ou minigame de sinuca abertos, o jogador não anda nem interage por baixo.
        if (Keyboard.current == null || MenuPausa.Aberto || MapaInterativoUI.IsShowing || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen)
        {
            moveInput = Vector2.zero;
            if (InteractionPrompt.Instance != null)
            {
                InteractionPrompt.Instance.SetTarget(null);
            }
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        moveInput =
            new Vector2(horizontal, vertical).normalized;

        Interactable alvo = Interactable.FindNearest(transform.position);
        InteractionPrompt.Instance.SetTarget(alvo);

        if (interactTimer > 0f)
        {
            interactTimer -= Time.deltaTime;
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Com a caixa de diálogo aberta, o E serve para avançar a leitura,
            // não para interagir de novo com o mesmo objeto.
            if (GameMessageUI.IsShowing)
            {
                GameMessageUI.Instance.Avancar();
            }
            else
            {
                interactTimer = interactDuration;

                if (alvo != null)
                {
                    alvo.Interact();
                }
            }
        }

        bool isInteracting = interactTimer > 0f;
        if (isInteracting || GameMessageUI.IsShowing)
        {
            moveInput = Vector2.zero;
        }

        bool isMoving = !isInteracting && moveInput.sqrMagnitude > 0f;

        if (isMoving)
        {
            facingDirection =
                Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y)
                    ? (moveInput.x > 0f ? DirRight : DirLeft)
                    : (moveInput.y > 0f ? DirUp : DirDown);
        }

        if (corpo != null && Capitulo2Director.Instance == null && corpo.sortingOrder < 10)
        {
            corpo.sortingOrder = 10;
        }

        if (quadrosCaminhada == null)
        {
            UpdateAnimator(animator, isMoving, isInteracting);
            UpdateAnimator(outfitAnimator, isMoving, isInteracting);
        }
        string novoEstado = isInteracting ? "Interact" :
            (isMoving ? "Walk" : "Idle") +
            (facingDirection == DirUp ? "Up" : facingDirection == DirDown ? "Down" :
                facingDirection == DirLeft ? "Left" : "Right");
        if (estadoAnimacao != novoEstado)
        {
            if (animator != null && animator.isActiveAndEnabled && animator.runtimeAnimatorController != null)
                animator.CrossFadeInFixedTime(novoEstado, 0.06f);
            if (outfitAnimator != null && outfitAnimator.isActiveAndEnabled && outfitAnimator.runtimeAnimatorController != null)
                outfitAnimator.CrossFadeInFixedTime(novoEstado, 0.06f);
            estadoAnimacao = novoEstado;
        }

        // A arte "esquerda" é espelhada para representar "direita",
        // já que o pack só traz o perfil voltado para um lado.
        // Usamos a escala do transform (em vez de SpriteRenderer.flipX)
        // para evitar qualquer conflito com o Animator.
        if (facingDirection == DirRight)
        {
            SetFacingScale(-1f);
        }
        else if (facingDirection == DirLeft)
        {
            SetFacingScale(1f);
        }

    }

    private void LateUpdate()
    {
        AtualizarPassada();
        if (corpo != null)
            corpo.sortingOrder = 1000 - Mathf.RoundToInt(transform.position.y * 10);
        if (lanternaMaoRenderer != null && corpo != null)
            lanternaMaoRenderer.sortingOrder = corpo.sortingOrder + (facingDirection == DirUp ? -1 : 1);
        // Garante que ao entrar/sair de um interior ou fechar diálogo a lanterna ligue/desligue imediatamente.
        bool usarLanterna = DeveUsarLanternaAgora();
        if (flashlight != null && flashlight.gameObject.activeSelf != usarLanterna)
            flashlight.gameObject.SetActive(usarLanterna);
        if (lanternaMaoRenderer != null && lanternaMaoRenderer.gameObject.activeSelf != usarLanterna)
            lanternaMaoRenderer.gameObject.SetActive(usarLanterna);
    }

    private void AtualizarPassada()
    {
        Vector2 agora = transform.position; // Posição interpolada, inclusive entre ticks de física.
        float distancia = Vector2.Distance(agora, ultimaPosicaoAnimacao);
        ultimaPosicaoAnimacao = agora;
        // A pausa conserva a pose; diálogo e parada voltam ao repouso sem som.
        if (Time.timeScale == 0 || MenuPausa.Aberto || MapaInterativoUI.IsShowing ||
            SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen) return;
        int apoios = 0;
        passada.Avancar(GameMessageUI.IsShowing ? 0 : distancia, () => apoios++);
        QuadroCaminhada = passada.Quadro;
        int quadro = totalQuadros == 8 ? QuadroCaminhada : QuadroCaminhada * totalQuadros / 8;
        if (quadrosCaminhada != null)
        {
            int linha = facingDirection == DirUp ? 1 :
                (facingDirection == DirLeft || facingDirection == DirRight ? 2 : 0);
            corpo.sprite = distancia < .0001f || GameMessageUI.IsShowing ? quadrosRepouso[linha] : quadrosCaminhada[linha, quadro];
        }
        // Pose já aplicada neste frame: som e imagem compartilham o contato.
        for (int i = 0; i < apoios; i++) PeApoiou?.Invoke();
        AtualizarLanternaERenderizadorMao(QuadroCaminhada, distancia > .0001f);
    }

    private void AtualizarLanternaERenderizadorMao(int passoAtual, bool isMoving)
    {
        bool usarLanterna = DeveUsarLanternaAgora();

        if (flashlight != null && flashlight.gameObject.activeSelf != usarLanterna)
            flashlight.gameObject.SetActive(usarLanterna);
        if (lanternaMaoRenderer != null && lanternaMaoRenderer.gameObject.activeSelf != usarLanterna)
            lanternaMaoRenderer.gameObject.SetActive(usarLanterna);

        if (!usarLanterna)
            return;

        Vector2 direction = FlashlightDirections[facingDirection];

        // Balanço sutil de 1 pixel na mão acompanhando a passada de caminhada
        float pixel = 1f / Mathf.Max(1, pixelsPerUnit);
        float bobY = (isMoving && (passoAtual == 3 || passoAtual == 7)) ? pixel : 0f;
        float swingX = 0f;
        if (isMoving && (facingDirection == DirLeft || facingDirection == DirRight))
        {
            swingX = passoAtual < 4 ? pixel : -pixel;
        }

        Vector2 offsetMaoMundo;
        switch (facingDirection)
        {
            case DirLeft:
                offsetMaoMundo = new Vector2(-0.19f - swingX, 0.44f + bobY);
                break;
            case DirRight:
                offsetMaoMundo = new Vector2(0.19f + swingX, 0.44f + bobY);
                break;
            case DirUp:
                offsetMaoMundo = new Vector2(0.14f, 0.48f + bobY);
                break;
            default: // DirDown
                offsetMaoMundo = new Vector2(-0.14f, 0.41f + bobY);
                break;
        }

        Vector2 posMaoMundo = SnapToPixel((Vector2)transform.position + offsetMaoMundo);

        if (lanternaMaoRenderer != null)
        {
            if (facingDirection == DirLeft || facingDirection == DirRight)
                lanternaMaoRenderer.sprite = spriteLanternaHorizontal;
            else if (facingDirection == DirUp)
                lanternaMaoRenderer.sprite = spriteLanternaCima;
            else
                lanternaMaoRenderer.sprite = spriteLanternaBaixo;

            // Como transform.localScale.x fica -1 quando DirRight, mantemos localScale = 1 para que
            // spriteLanternaHorizontal espelhe automaticamente para a direita quando DirRight!
            // Já para DirDown / DirUp compensamos o sinal de localScale.x.
            float parentSign = transform.localScale.x < 0f ? -1f : 1f;
            if (facingDirection == DirLeft || facingDirection == DirRight)
                lanternaMaoRenderer.transform.localScale = Vector3.one;
            else
                lanternaMaoRenderer.transform.localScale = new Vector3(parentSign, 1f, 1f);

            lanternaMaoRenderer.transform.position = posMaoMundo;
            lanternaMaoRenderer.sortingOrder = (facingDirection == DirUp) ? 9 : 11;
        }

        if (flashlight != null)
        {
            // A luz nasce exatamente na lente frontal da lanterna na mão de Marcus
            Vector2 pontaLanterna = SnapToPixel(posMaoMundo + direction * 0.14f);
            flashlight.position = pontaLanterna;

            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg +
                flashlightAngleOffset;
            flashlight.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private Vector2 SnapToPixel(Vector2 posicao)
    {
        float pixel = 1f / Mathf.Max(1, pixelsPerUnit);

        return new Vector2(
            Mathf.Round(posicao.x / pixel) * pixel,
            Mathf.Round(posicao.y / pixel) * pixel
        );
    }

    private void UpdateAnimator(Animator anim, bool isMoving, bool isInteracting)
    {
        if (anim == null || !anim.isActiveAndEnabled || anim.runtimeAnimatorController == null)
        {
            return;
        }

        anim.SetBool("IsMoving", isMoving);
        anim.SetBool("FacingDown", facingDirection == DirDown);
        anim.SetBool("FacingUp", facingDirection == DirUp);
        anim.SetBool("FacingLeft", facingDirection == DirLeft);
        anim.SetBool("FacingRight", facingDirection == DirRight);
        anim.SetBool("IsInteracting", isInteracting);
    }

    private void SetFacingScale(float sign)
    {
        Vector3 scale = transform.localScale;
        scale.x = sign * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (GameMessageUI.IsShowing || MenuPausa.Aberto || MapaInterativoUI.IsShowing ||
            SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen)
        { currentVelocity = Vector2.zero; return; }
        // A velocidade nunca muda de uma vez: ganha e perde embalo aos poucos,
        // o que dá o peso de "cada passo é uma decisão".
        Vector2 targetVelocity = moveInput * moveSpeed;

        float rate = targetVelocity.sqrMagnitude > currentVelocity.sqrMagnitude
            ? acceleration
            : deceleration;

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}
