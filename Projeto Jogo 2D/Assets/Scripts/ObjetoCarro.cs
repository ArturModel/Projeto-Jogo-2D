using UnityEngine;

// Base dos objetos que o jogador pode mexer dentro do carro.
//
// Regra de design do documento: nada de contorno ou brilho de "objeto de
// videogame". O destaque é apenas um leve clarear quando o cursor passa por
// cima — quem observa encontra, quem não observa termina o prólogo igual.
[RequireComponent(typeof(Collider2D))]
public abstract class ObjetoCarro : MonoBehaviour
{
    [Header("Destaque")]
    [SerializeField] private float clareamento = 0.22f;
    [SerializeField] private float velocidadeDestaque = 8f;

    private SpriteRenderer render;
    private Color corBase;
    private float destaque;

    public bool Apontado { get; set; }

    // Alguns objetos deixam de aceitar interação depois de resolvidos.
    public virtual bool Disponivel => true;

    protected virtual void Awake()
    {
        render = GetComponent<SpriteRenderer>();

        if (render != null)
        {
            corBase = render.color;
        }
    }

    protected virtual void Update()
    {
        float alvo = Apontado && Disponivel ? 1f : 0f;
        destaque = Mathf.MoveTowards(destaque, alvo, velocidadeDestaque * Time.deltaTime);

        if (render != null)
        {
            float k = 1f + destaque * clareamento;
            render.color = new Color(corBase.r * k, corBase.g * k, corBase.b * k, corBase.a);
        }
    }

    public abstract void Ativar();
}
