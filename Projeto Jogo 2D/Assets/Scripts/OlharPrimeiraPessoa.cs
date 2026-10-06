using UnityEngine;
using UnityEngine.InputSystem;

// A "primeira pessoa" do prólogo.
//
// Não existe câmera 3D nem carro modelado: a arte do interior é maior que a
// tela e a câmera passeia dentro dela, com limite. Para o jogador o efeito é
// olhar em volta; para o sistema é uma câmera 2D deslizando dentro de um
// desenho. O documento (§3.2) pede exatamente isso — visão limitada, sem
// rotações absurdas e sem mostrar partes do carro que não foram produzidas.
public class OlharPrimeiraPessoa : MonoBehaviour
{
    [Header("Alcance do olhar (unidades de mundo)")]
    // A arte do interior tem 896x504 e a viewport 640x360, a 32 PPU. Sobram
    // 128px de cada lado na horizontal e 72px na vertical; fico abaixo disso
    // para a borda do desenho nunca aparecer.
    [SerializeField] private float alcanceX = 3.4f;
    [SerializeField] private float alcanceY = 1.8f;

    [Header("Resposta")]
    [SerializeField] private float suavidade = 6f;

    [Tooltip("Quanto da tela precisa ser percorrido para chegar ao limite.")]
    [Range(0.2f, 1f)]
    [SerializeField] private float zonaUtil = 0.8f;

    [Header("Respiração")]
    [Tooltip("Balanço leve do carro, para a cena nunca ficar estática.")]
    [SerializeField] private float balanco = 0.035f;

    [SerializeField] private float velocidadeBalanco = 1.7f;

    // Lido pelas camadas de parallax.
    public static Vector2 Deslocamento { get; private set; }

    private Vector3 origem;
    private Vector2 alvo;
    private Vector2 atual;

    private void Awake()
    {
        origem = transform.position;
    }

    private void Update()
    {
        alvo = LerEntrada();
        atual = Vector2.Lerp(atual, alvo, 1f - Mathf.Exp(-suavidade * Time.deltaTime));

        Deslocamento = atual;

        float t = Time.time * velocidadeBalanco;
        Vector2 tremor = new Vector2(
            Mathf.Sin(t * 1.3f) * balanco * 0.6f,
            Mathf.Sin(t) * balanco
        );

        transform.position = origem + (Vector3)(atual + tremor);
    }

    private Vector2 LerEntrada()
    {
        Vector2 v = Vector2.zero;

        // Mouse: posição na tela, normalizada e com zona morta no centro.
        if (Mouse.current != null)
        {
            Vector2 p = Mouse.current.position.ReadValue();
            Vector2 n = new Vector2(
                p.x / Mathf.Max(1, Screen.width) * 2f - 1f,
                p.y / Mathf.Max(1, Screen.height) * 2f - 1f
            );

            v = n / Mathf.Max(0.2f, zonaUtil);
        }

        // Analógico direito tem prioridade quando está sendo usado.
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();

            if (stick.sqrMagnitude > 0.04f)
            {
                v = stick;
            }
        }

        v.x = Mathf.Clamp(v.x, -1f, 1f);
        v.y = Mathf.Clamp(v.y, -1f, 1f);

        return new Vector2(v.x * alcanceX, v.y * alcanceY);
    }
}
