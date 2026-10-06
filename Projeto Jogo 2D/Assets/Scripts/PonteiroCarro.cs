using UnityEngine;
using UnityEngine.InputSystem;

// Quem descobre o que está sob o cursor e entrega o clique ao objeto certo.
//
// O documento pede raycast a partir do cursor (§10). Em 2D isso é um
// OverlapPoint: mais barato e sem depender de câmera de física.
public class PonteiroCarro : MonoBehaviour
{
    [SerializeField] private Camera camera2D;
    [SerializeField] private LayerMask camadas = ~0;

    private ObjetoCarro apontado;

    // O diretor desliga o ponteiro nos trechos em que o jogador não deve
    // conseguir mexer em nada (a chegada, o estacionamento).
    public bool Ativo { get; set; } = true;

    private void Awake()
    {
        if (camera2D == null)
        {
            camera2D = Camera.main;
        }
    }

    private void Update()
    {
        ObjetoCarro achado = Ativo ? Procurar() : null;

        if (achado != apontado)
        {
            if (apontado != null)
            {
                apontado.Apontado = false;
            }

            apontado = achado;

            if (apontado != null)
            {
                apontado.Apontado = true;
            }
        }

        if (apontado == null || Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && apontado.Disponivel)
        {
            apontado.Ativar();
        }
    }

    private ObjetoCarro Procurar()
    {
        if (Mouse.current == null || camera2D == null)
        {
            return null;
        }

        Vector2 tela = Mouse.current.position.ReadValue();
        Vector2 mundo = camera2D.ScreenToWorldPoint(tela);

        Collider2D bateu = Physics2D.OverlapPoint(mundo, camadas);

        return bateu != null ? bateu.GetComponent<ObjetoCarro>() : null;
    }
}
