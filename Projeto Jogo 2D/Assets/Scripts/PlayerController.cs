using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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
    private Sprite[] repousoComLanterna;
    private bool focoLanternaPronto;
    public bool LanternaVisivel => DeveUsarLanternaAgora();
    public int DirecaoAtual => facingDirection;
    private readonly MarcusGait passada = new MarcusGait();
    private bool folhaCaminhadaDirecional;
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
        if (!folhaCaminhadaDirecional) PrepararPosesLanterna();
        PrepararFeixeLanterna();
    }

    private void PrepararFeixeLanterna()
    {
        // Cenas C2 antigas copiavam o ator sem a luz externa do rig C1.
        // O equipamento pertence ao ator em todas as cenas, com uma única fonte.
        if(flashlight==null)
        {var go=new GameObject("FeixeLanternaMarcus");go.transform.SetParent(transform,false);flashlight=go.transform;}
        var light=flashlight.GetComponent<Light2D>()??flashlight.gameObject.AddComponent<Light2D>();
        light.lightType=Light2D.LightType.Point;light.pointLightInnerRadius=1.1f;light.pointLightOuterRadius=7.2f;
        light.pointLightInnerAngle=28;light.pointLightOuterAngle=66;light.intensity=1.6f;
        light.color=new Color(1,.94f,.78f);light.falloffIntensity=.4f;light.shadowIntensity=.85f;
        flashlight.gameObject.SetActive(false);
    }

    private static bool DeveUsarLanternaAgora()
    {
        return GameState.LanternaLigada && Application.isFocused && !BloqueiaEquipamento();
    }

    private static bool BloqueiaEquipamento() => MenuPausa.Aberto || MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen ||
        GameMessageUI.IsShowing || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing ||
        PayphoneCheckpoint.IsSaveUIOpen || SceneFader.IsTransitioning || Time.timeScale == 0 ||
        Capitulo1Director.PainelFechamentoAberto;

    private void OnApplicationFocus(bool foco) { focoLanternaPronto = false; }

    private void PrepararPosesLanterna()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "MarcusLanterna.png");
        if (!File.Exists(path)) { Debug.LogError("Poses de Marcus com lanterna ausentes."); return; }
        var sheet = new Texture2D(2,2,TextureFormat.RGBA32,false);
        ImageConversion.LoadImage(sheet, File.ReadAllBytes(path)); sheet.filterMode=FilterMode.Point; sheet.wrapMode=TextureWrapMode.Clamp;
        int frames=sheet.width/64-1;
        if(!ValidarFolhaMarcus(sheet,frames)){Debug.LogError("Folha da lanterna invalida; preservando corpo sem retangulo opaco.");return;}
        quadrosCaminhadaComLanterna = new Sprite[3,frames]; repousoComLanterna = new Sprite[3];
        for(int row=0;row<3;row++) for(int frame=0;frame<=frames;frame++)
        {
            var pose=Sprite.Create(sheet,new Rect(frame*64,sheet.height-(row+1)*64,64,64),new Vector2(.5f,.125f),32,0,SpriteMeshType.FullRect);
            if(frame<frames)quadrosCaminhadaComLanterna[row,frame]=pose;else repousoComLanterna[row]=pose;
        }
    }

    private static bool ValidarFolhaMarcus(Texture2D sheet,int frames,int linhas=3,bool repousoSeparado=true)
    {
        int colunas = frames + (repousoSeparado ? 1 : 0);
        if (sheet.width != colunas * 64 || sheet.height != linhas * 64) return false;
        if (repousoSeparado && frames != 8 && frames != 16) return false;
        var pixels=sheet.GetPixels32();
        for(int row=0;row<linhas;row++)for(int f=0;f<colunas;f++)
        {
            int count=0;
            for(int y=0;y<64;y++)for(int x=0;x<64;x++)
            {
                byte alpha=pixels[(row*64+y)*sheet.width+f*64+x].a;
                if(alpha>32){if(x==0||x==63||y==0||y==63)return false;count++;}
            }
            if(count<180||count>1400)return false;
        }
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
        folhaCaminhadaDirecional = folha.width == 192 && folha.height == 256;
        int linhas = folhaCaminhadaDirecional ? 4 : 3;
        totalQuadros = folha.width >= 1088 ? 16 : folha.width >= 512 ? 8 : 6;
        if (folhaCaminhadaDirecional) totalQuadros = 3;
        if ((folhaCaminhadaDirecional || folha.width >= 576) &&
            !ValidarFolhaMarcus(folha, totalQuadros, linhas, !folhaCaminhadaDirecional))
        {
            folhaCaminhadaDirecional = false;
            Debug.LogError("Folha de caminhada inválida.");
            Destroy(folha);
            return;
        }
        quadrosCaminhada = new Sprite[linhas, totalQuadros];
        for (int direcao = 0; direcao < linhas; direcao++)
            for (int passo = 0; passo < totalQuadros; passo++)
                quadrosCaminhada[direcao, passo] = Sprite.Create(folha,
                    new Rect(passo * 64, folha.height - (direcao + 1) * 64, 64, 64),
                    new Vector2(0.5f, 0.125f), 32f, 0, SpriteMeshType.FullRect);
        quadrosRepouso = new Sprite[linhas];
        for (int direcao = 0; direcao < linhas; direcao++)
            quadrosRepouso[direcao] = folha.width >= 576 ? Sprite.Create(folha,
                new Rect(totalQuadros*64, folha.height - (direcao + 1) * 64, 64, 64),
                new Vector2(.5f, .125f), 32f, 0, SpriteMeshType.FullRect) :
                quadrosCaminhada[direcao, folhaCaminhadaDirecional ? 1 : 0];

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

        // Ordem importa: atravessar uma porta é mais recente que o último
        // checkpoint, então a entrada da porta ganha.
        if (SceneTransition.ConsumirEntrada(out Vector2 entrada, out int direcao))
        {
            var layout=FindAnyObjectByType<SantaRitaRoomLayout>();
            if(layout!=null)GameState.Marcar(layout.versionFlag);
            TeleportarPara(entrada, direcao);
            return;
        }

        // Vindo de "Continuar": nasce no último orelhão registrado.
        if (CheckpointSystem.ConsumePendingSpawn(out Vector2 spawn))
        {
            var layout = FindAnyObjectByType<SantaRitaRoomLayout>();
            if(layout!=null)spawn=layout.AdaptSavedPosition(spawn);
            TeleportarPara(spawn, -1);
        }
        var planta=FindAnyObjectByType<SantaRitaRoomLayout>();
        if(planta!=null)GameState.Marcar(planta.versionFlag);
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
            SetFacingScale(facingDirection == DirRight && !folhaCaminhadaDirecional ? -1f : 1f);
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
        // Só um pressionamento novo, após foco e liberação da tecla. UI não muda a preferência.
        var teclado = Keyboard.current;
        if(teclado != null && !teclado.fKey.isPressed) focoLanternaPronto = Application.isFocused;
        if(teclado != null && focoLanternaPronto && Application.isFocused && !BloqueiaEquipamento() && teclado.fKey.wasPressedThisFrame)
        { GameState.AlternarLanterna(); focoLanternaPronto = false; }
        // Com a pausa, mapa, orelhão ou minigame de sinuca abertos, o jogador não anda nem interage por baixo.
        if (Keyboard.current == null || MenuPausa.Aberto || MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen ||
            Capitulo1Director.PainelFechamentoAberto)
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

        // A folha de quatro direções tem um perfil direito próprio.
        SetFacingScale(facingDirection == DirRight && !folhaCaminhadaDirecional ? -1f : 1f);

    }

    private void LateUpdate()
    {
        AtualizarPassada();
        if (corpo != null)
            corpo.sortingOrder = 1000 - Mathf.RoundToInt(transform.position.y * 10);
        // Supressão temporária não altera a preferência guardada no GameState.
        bool usarLanterna = DeveUsarLanternaAgora();
        if (flashlight != null && flashlight.gameObject.activeSelf != usarLanterna)
            flashlight.gameObject.SetActive(usarLanterna);
    }

    private void AtualizarPassada()
    {
        Vector2 agora = transform.position; // Posição interpolada, inclusive entre ticks de física.
        float distancia = Vector2.Distance(agora, ultimaPosicaoAnimacao);
        ultimaPosicaoAnimacao = agora;
        // A pausa conserva a pose; diálogo e parada voltam ao repouso sem som.
        if (Time.timeScale == 0 || MenuPausa.Aberto || MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen ||
            SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen) return;
        bool andando = !GameMessageUI.IsShowing && (distancia >= .0001f || currentVelocity.sqrMagnitude > .0001f);
        if(!andando)passada.Reset();
        int apoios = 0;
        passada.Avancar(GameMessageUI.IsShowing ? 0 : distancia, () => apoios++);
        QuadroCaminhada = passada.Quadro;
        int quadro = totalQuadros == 16 ? passada.QuadroDetalhado : totalQuadros == 8 ? QuadroCaminhada : QuadroCaminhada * totalQuadros / 8;
        if (quadrosCaminhada != null)
        {
            int linha = folhaCaminhadaDirecional ? facingDirection : Mathf.Min(facingDirection, DirLeft);
            bool equipada = linha != DirUp && DeveUsarLanternaAgora() && quadrosCaminhadaComLanterna != null;
            if (folhaCaminhadaDirecional)
            {
                // Dois contatos e uma passagem, todos recortes da mesma folha.
                quadro = QuadroCaminhada switch { 0 or 1 => 0, 4 or 5 => 2, _ => 1 };
            }
            corpo.sprite = !andando ?
                (equipada?repousoComLanterna[linha]:quadrosRepouso[linha]) :
                (equipada?quadrosCaminhadaComLanterna[linha,quadrosCaminhadaComLanterna.GetLength(1)==16?passada.QuadroDetalhado:QuadroCaminhada]:quadrosCaminhada[linha,quadro]);
        }
        // Pose já aplicada neste frame: som e imagem compartilham o contato.
        for (int i = 0; i < apoios; i++) PeApoiou?.Invoke();
        AtualizarFeixeLanterna(totalQuadros==16?passada.QuadroDetalhado:QuadroCaminhada, andando);
    }

    private void AtualizarFeixeLanterna(int quadro, bool andando)
    {
        bool visivel = DeveUsarLanternaAgora();
        if (flashlight == null) return;
        flashlight.gameObject.SetActive(visivel);
        if (!visivel) return;
        // As coordenadas são as lentes desenhadas na folha Aseprite (64 px, pivô nos pés).
        int bob = andando && !folhaCaminhadaDirecional
            ? -Mathf.RoundToInt(Mathf.Pow(Mathf.Sin(quadro * Mathf.PI / (totalQuadros == 16 ? 8f : 4f)), 2)) : 0;
        Vector2 lente = facingDirection == DirDown ? new Vector2(40,46+bob) :
            facingDirection == DirUp ? new Vector2(40,28+bob) : new Vector2(16,36+bob);
        Vector2 offset = new Vector2((lente.x-32)/32f,(56-lente.y)/32f);
        if (facingDirection == DirRight) offset.x = -offset.x;
        flashlight.position = SnapToPixel((Vector2)transform.position + offset);
        Vector2 direcao = FlashlightDirections[facingDirection];
        if(flashlight.parent==transform)flashlight.localScale=new Vector3(1/transform.localScale.x,1/transform.localScale.y,1);
        flashlight.rotation=Quaternion.Euler(0,0,Mathf.Atan2(direcao.y,direcao.x)*Mathf.Rad2Deg+flashlightAngleOffset);
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
        if (GameMessageUI.IsShowing || MenuPausa.Aberto || MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen ||
            SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen ||
            Capitulo1Director.PainelFechamentoAberto)
        { currentVelocity = Vector2.zero; return; }
        // A velocidade nunca muda de uma vez: ganha e perde embalo aos poucos,
        // o que dá o peso de "cada passo é uma decisão".
        Vector2 targetVelocity = moveInput * (moveSpeed * SantaRitaShallowStairs.SpeedAt(rb.position));

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
