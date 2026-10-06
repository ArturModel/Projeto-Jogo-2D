using UnityEngine;

// Os limpadores.
//
// O documento (§9) chama isso de "ritmo da cena": é o metrônomo do prólogo, e
// pede explicitamente que NÃO fique sincronizado com as falas. Por isso o
// período tem uma variação pequena a cada passada — dois limpadores de carro
// velho nunca batem no mesmo tempo duas vezes seguidas.
public class LimpadorParabrisa : MonoBehaviour
{
    [Header("Palhetas")]
    [SerializeField] private Transform[] palhetas;

    [Header("Movimento")]
    [SerializeField] private float anguloParado = -62f;
    [SerializeField] private float anguloAberto = 26f;

    [Tooltip("Duração de uma passada (ida ou volta).")]
    [SerializeField] private float duracaoPassada = 0.42f;

    [Tooltip("Pausa entre um ciclo completo e o próximo.")]
    [SerializeField] private float pausa = 0.55f;

    [Tooltip("Variação aleatória da pausa, para não virar metrônomo.")]
    [SerializeField] private float irregularidade = 0.12f;

    [Header("Som")]
    [SerializeField] private string camadaAudio = "limpador";

    private float tempo;
    private float pausaAtual;
    private bool somTocado;

    public bool Ativo { get; set; } = true;

    private void Awake()
    {
        pausaAtual = pausa;
    }

    private void Update()
    {
        float ciclo = duracaoPassada * 2f + pausaAtual;

        if (!Ativo)
        {
            AplicarAngulo(anguloParado);
            return;
        }

        tempo += Time.deltaTime;

        if (tempo < duracaoPassada * 2f && !somTocado)
        {
            somTocado = true;

            if (AudioAmbiente.Instance != null)
            {
                AudioAmbiente.Instance.Disparar(camadaAudio, 0.7f);
            }
        }

        if (tempo >= ciclo)
        {
            tempo -= ciclo;
            somTocado = false;
            pausaAtual = pausa + Random.Range(-irregularidade, irregularidade);
        }

        AplicarAngulo(AnguloEm(tempo));
    }

    private float AnguloEm(float t)
    {
        if (t < duracaoPassada)
        {
            return Mathf.Lerp(anguloParado, anguloAberto, Suavizar(t / duracaoPassada));
        }

        if (t < duracaoPassada * 2f)
        {
            float f = (t - duracaoPassada) / duracaoPassada;
            return Mathf.Lerp(anguloAberto, anguloParado, Suavizar(f));
        }

        return anguloParado;
    }

    // Acelera no meio e freia nas pontas, como um braço mecânico de verdade.
    private static float Suavizar(float t)
    {
        return t * t * (3f - 2f * t);
    }

    private void AplicarAngulo(float angulo)
    {
        foreach (Transform p in palhetas)
        {
            if (p != null)
            {
                p.localRotation = Quaternion.Euler(0f, 0f, angulo);
            }
        }
    }
}
