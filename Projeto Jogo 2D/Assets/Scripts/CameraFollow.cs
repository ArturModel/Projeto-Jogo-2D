using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    [SerializeField] private Transform target;

    [Header("Enquadramento")]
    [SerializeField] private Vector3 offset =
        new Vector3(0f, 1.5f, -10f);

    [Header("Suavização")]
    [SerializeField, Range(0.01f, 0.5f)]
    private float smoothTime = 0.12f;

    [Header("Pixel Art")]
    [SerializeField] private int pixelsPerUnit = 32;

    private Vector3 velocidadeAtual;

    // Posição suavizada "real", sem arredondar. O arredondamento é aplicado
    // só na hora de escrever no transform — se a suavização partisse do valor
    // já arredondado, ela brigaria com o snap e a câmera tremeria.
    private Vector3 posicaoSuave;

    private void Awake()
    {
        posicaoSuave = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 posicaoDesejada = target.position + offset;

        posicaoSuave = Vector3.SmoothDamp(
            posicaoSuave,
            posicaoDesejada,
            ref velocidadeAtual,
            smoothTime
        );

        // O SmoothDamp se aproxima do alvo mas nunca chega. Se a sobra ficar
        // menor que meio pixel, encostamos de uma vez: senão o arredondamento
        // segura a câmera parada por vários frames e solta um salto de 1 pixel
        // no fim — que é a travadinha ao parar de andar.
        float meioPixel = 0.5f / Mathf.Max(1, pixelsPerUnit);

        if (Vector2.Distance(posicaoSuave, posicaoDesejada) < meioPixel)
        {
            posicaoSuave.x = posicaoDesejada.x;
            posicaoSuave.y = posicaoDesejada.y;
            velocidadeAtual = Vector3.zero;
        }

        // A câmera precisa parar exatamente em cima do grid de pixels. Fora
        // dele, a borda de cada sprite cobre um texel pela metade e se mistura
        // com o fundo escuro, criando um contorno preto enquanto há movimento.
        float pixel = 1f / Mathf.Max(1, pixelsPerUnit);

        transform.position = new Vector3(
            Mathf.Round(posicaoSuave.x / pixel) * pixel,
            Mathf.Round(posicaoSuave.y / pixel) * pixel,
            -10f
        );
    }
}
