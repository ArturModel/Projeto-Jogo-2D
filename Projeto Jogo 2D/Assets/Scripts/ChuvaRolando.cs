using UnityEngine;

// Chuva caindo, em camadas.
//
// Fica ATRÁS do interior do carro de propósito: como o desenho do interior só
// é transparente no para-brisa, a chuva aparece exatamente onde deveria — do
// lado de fora do vidro — sem precisar de máscara.
public class ChuvaRolando : MonoBehaviour
{
    [SerializeField] private float velocidade = 9f;

    [Tooltip("Deslocamento lateral: chuva de estrada nunca cai reta.")]
    [SerializeField] private float inclinacao = -2.2f;

    [Tooltip("Distância até repetir. Use a altura do sprite em unidades.")]
    [SerializeField] private float ciclo = 8f;

    private Vector3 origem;
    private float andado;

    private void Awake()
    {
        origem = transform.localPosition;
    }

    private void Update()
    {
        andado += velocidade * Time.deltaTime;

        if (andado >= ciclo)
        {
            andado -= ciclo;
        }

        float f = andado / Mathf.Max(0.01f, ciclo);

        transform.localPosition = origem + new Vector3(f * inclinacao, -andado, 0f);
    }
}
