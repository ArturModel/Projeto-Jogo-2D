using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Um grupo de luzes que acende e apaga junto.
//
// É o que permite um lugar mudar sem virar outra cena. O documento (§5.3) pede
// que o Clube volte a ser a noite da tragédia, que ruas esvaziem, que sons
// antigos retornem — e nada disso funciona se a luz estiver achatada dentro da
// arte. Aqui a luz de cada estado do lugar é uma camada nomeada: "presente"
// apaga, "memoria" acende, e o cenário é o mesmo.
//
// Cada luz guarda a intensidade com que foi autorada; a camada só multiplica
// esse valor. Assim dá para regular uma luz individual no Inspector sem
// quebrar o fade do grupo.
public class CamadaDeLuz : MonoBehaviour
{
    private static readonly List<CamadaDeLuz> Registro = new List<CamadaDeLuz>();

    [Tooltip("Nome usado para acender/apagar de fora. Ex: \"presente\", \"memoria\".")]
    [SerializeField] private string nome = "presente";

    [SerializeField] private bool acesaNoInicio = true;

    [Tooltip("Segundos de transição quando ninguém especifica.")]
    [SerializeField] private float tempoPadrao = 1.2f;

    private Light2D[] luzes;
    private float[] intensidadesAutorais;
    private float fator = 1f;
    private float fatorAlvo = 1f;
    private float velocidade;

    public string Nome => nome;

    private void Awake()
    {
        luzes = GetComponentsInChildren<Light2D>(true);
        intensidadesAutorais = new float[luzes.Length];

        for (int i = 0; i < luzes.Length; i++)
        {
            intensidadesAutorais[i] = luzes[i].intensity;
        }

        fator = fatorAlvo = acesaNoInicio ? 1f : 0f;
        Aplicar();
    }

    private void OnEnable()
    {
        Registro.Add(this);
    }

    private void OnDisable()
    {
        Registro.Remove(this);
    }

    private void Update()
    {
        if (Mathf.Approximately(fator, fatorAlvo))
        {
            return;
        }

        fator = Mathf.MoveTowards(fator, fatorAlvo, velocidade * Time.deltaTime);
        Aplicar();
    }

    private void Aplicar()
    {
        for (int i = 0; i < luzes.Length; i++)
        {
            if (luzes[i] != null)
            {
                luzes[i].intensity = intensidadesAutorais[i] * fator;
            }
        }
    }

    public void Definir(float alvo, float tempo = -1f)
    {
        alvo = Mathf.Clamp01(alvo);
        tempo = tempo < 0f ? tempoPadrao : tempo;

        fatorAlvo = alvo;
        velocidade = tempo <= 0.01f
            ? 99f
            : Mathf.Max(0.01f, Mathf.Abs(alvo - fator) / tempo);

        if (tempo <= 0.01f)
        {
            fator = alvo;
            Aplicar();
        }
    }

    // --- atalhos estáticos -------------------------------------------

    private static CamadaDeLuz Achar(string nome)
    {
        foreach (CamadaDeLuz c in Registro)
        {
            if (c != null && c.nome == nome)
            {
                return c;
            }
        }

        return null;
    }

    public static void Acender(string nome, float tempo = -1f)
    {
        Achar(nome)?.Definir(1f, tempo);
    }

    public static void Apagar(string nome, float tempo = -1f)
    {
        Achar(nome)?.Definir(0f, tempo);
    }

    // A troca de estado do lugar: uma camada some enquanto a outra nasce.
    public static void Trocar(string apagar, string acender, float tempo = -1f)
    {
        Apagar(apagar, tempo);
        Acender(acender, tempo);
    }
}
