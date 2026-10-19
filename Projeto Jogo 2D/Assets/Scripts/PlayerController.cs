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
    private float tempoPasso;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        corpo = GetComponent<SpriteRenderer>();
        PrepararCaminhadaMarcus();
    }

    private void PrepararCaminhadaMarcus()
    {
        string arquivo = Path.Combine(Application.streamingAssetsPath, "MarcusWalk.png");
        if (corpo == null || !File.Exists(arquivo)) return;
        Texture2D folha = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(folha, File.ReadAllBytes(arquivo))) return;
        folha.filterMode = FilterMode.Point;
        folha.wrapMode = TextureWrapMode.Clamp;
        quadrosCaminhada = new Sprite[3, 6];
        for (int direcao = 0; direcao < 3; direcao++)
            for (int passo = 0; passo < 6; passo++)
                quadrosCaminhada[direcao, passo] = Sprite.Create(folha,
                    new Rect(passo * 64, folha.height - (direcao + 1) * 64, 64, 64),
                    new Vector2(0.5f, 0.125f), 32f, 0, SpriteMeshType.FullRect);

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
        if (flashlight != null) flashlight.gameObject.SetActive(GameState.Tem(Flags.LanternaDoCarroObtida));

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
        // Com a pausa, mapa ou minigame de sinuca abertos, o jogador não anda nem interage por baixo.
        if (Keyboard.current == null || MenuPausa.Aberto || MapaInterativoUI.IsShowing || SinucaMinigameUI.IsShowing)
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

        if (quadrosCaminhada != null)
        {
            if (isMoving) tempoPasso += Time.deltaTime * 8f;
            else tempoPasso = 0f;
            int linha = facingDirection == DirUp ? 1 :
                (facingDirection == DirLeft || facingDirection == DirRight ? 2 : 0);
            corpo.sprite = quadrosCaminhada[linha, isMoving ? (int)tempoPasso % 6 : 0];
        }
        else
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
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.CrossFadeInFixedTime(novoEstado, 0.06f);
            if (outfitAnimator != null && outfitAnimator.runtimeAnimatorController != null)
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

        if (flashlight != null)
        {
            if (flashlight.gameObject.activeSelf != GameState.Tem(Flags.LanternaDoCarroObtida))
                flashlight.gameObject.SetActive(GameState.Tem(Flags.LanternaDoCarroObtida));
            Vector2 direction = FlashlightDirections[facingDirection];

            // A origem é a mão do personagem, não o pivô do sprite. A partir
            // dela o deslocamento é sempre o mesmo nas quatro direções.
            Vector2 mao =
                (Vector2)transform.position + Vector2.up * flashlightHandHeight;

            flashlight.position =
                SnapToPixel(mao + direction * flashlightDistance);

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
        if (anim == null || anim.runtimeAnimatorController == null)
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
