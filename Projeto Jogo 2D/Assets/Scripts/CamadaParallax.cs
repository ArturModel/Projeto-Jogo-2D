using UnityEngine;

// Profundidade falsa para as camadas do prólogo.
//
// Quando o jogador olha para o lado, o painel do carro tem que deslizar muito e
// a estrada quase nada — é isso que faz o para-brisa parecer uma janela e não
// um adesivo colado no painel. A camada acompanha a câmera na proporção da
// distância: acompanhar 100% equivale a estar infinitamente longe (parada na
// tela), 0% equivale a estar colado no rosto do jogador.
[ExecuteAlways]
public class CamadaParallax : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("0 = colado no jogador (desliza tudo). 1 = no infinito (não desliza).")]
    [SerializeField] private float distancia = 0.5f;

    private Vector3 origem;

    private void Awake()
    {
        origem = transform.position;
    }

    private void OnEnable()
    {
        origem = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = origem + (Vector3)(OlharPrimeiraPessoa.Deslocamento * distancia);
    }
}
