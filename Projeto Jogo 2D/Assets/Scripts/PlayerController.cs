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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        // Ordem importa: atravessar uma porta é mais recente que o último
        // checkpoint, então a entrada da porta ganha.
        if (SceneTransition.ConsumirEntrada(out Vector2 entrada, out int direcao))
        {
            Teleportar(entrada);

            if (direcao >= DirDown && direcao <= DirRight)
            {
                facingDirection = direcao;
            }

            return;
        }

        // Vindo de "Continuar": nasce no último orelhão registrado.
        if (CheckpointSystem.ConsumePendingSpawn(out Vector2 spawn))
        {
            Teleportar(spawn);
        }
    }

    private void Teleportar(Vector2 destino)
    {
        rb.position = destino;
        transform.position = destino;
        currentVelocity = Vector2.zero;
    }

    private void Update()
    {
        // Com a pausa aberta o tempo está parado, mas o Update continua rodando:
        // sem isto o E ainda abriria diálogo por baixo do menu.
        if (Keyboard.current == null || MenuPausa.Aberto)
        {
            moveInput = Vector2.zero;
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
        if (isInteracting)
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

        UpdateAnimator(animator, isMoving, isInteracting);
        UpdateAnimator(outfitAnimator, isMoving, isInteracting);

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
        if (anim == null)
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