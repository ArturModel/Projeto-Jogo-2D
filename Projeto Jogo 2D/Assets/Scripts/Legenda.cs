using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Falas roteirizadas podem interromper pensamentos opcionais no prólogo.
public class Legenda : MonoBehaviour
{
    public const int PrioridadeOpcional = 0;
    public const int PrioridadeRoteiro = 10;
    public const int PrioridadeCritica = 20;

    private class Fala
    {
        public string Texto;
        public float Duracao;
        public int Prioridade;
    }

    private static Legenda instancia;

    public static Legenda Instance
    {
        get
        {
            if (instancia == null)
            {
                GameObject go = new GameObject("Legenda");
                instancia = go.AddComponent<Legenda>();
            }

            return instancia;
        }
    }

    // Resolução de referência da UI. O prólogo roda em 640x360, então a legenda
    // precisa acompanhar a densidade de pixel dele para não ficar o dobro do
    // tamanho do resto da cena.
    public static Vector2 ResolucaoUI = new Vector2(640f, 360f);

    private readonly List<Fala> fila = new List<Fala>();
    private MinigameDialogue legenda;
    private Fala atual;
    private Coroutine rotina;

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        Construir();
    }

    private void Construir()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "LegendaCanvas", 8700);
        CanvasScaler escala = canvas.GetComponent<CanvasScaler>();
        escala.referenceResolution = ResolucaoUI;

        legenda = new MinigameDialogue(canvas.transform, "LegendaDoPrologo");
    }

    public static void Dizer(string frase, float duracao = 3f,
                             int prioridade = PrioridadeRoteiro)
    {
        Instance.Enfileirar(frase, duracao, prioridade);
    }

    // Descarta o que está na fila. Usado quando a cena vira de página (o carro
    // estaciona, por exemplo) e falas antigas perderam o sentido.
    public static void Limpar()
    {
        if (instancia == null)
        {
            return;
        }

        instancia.fila.Clear();
        if (instancia.rotina != null) instancia.StopCoroutine(instancia.rotina);
        instancia.rotina = null;
        instancia.atual = null;
        instancia.legenda.Limpar();
    }

    private void Enfileirar(string frase, float duracao, int prioridade)
    {
        Fala nova = new Fala { Texto = frase, Duracao = duracao, Prioridade = prioridade };

        // Fala mais importante corta a que está no ar; as outras esperam a vez.
        if (atual != null && prioridade > atual.Prioridade)
        {
            fila.Insert(0, nova);

            if (rotina != null)
            {
                StopCoroutine(rotina);
            }

            atual = null;
            rotina = StartCoroutine(Rodar());
            return;
        }

        int i = fila.Count;

        while (i > 0 && fila[i - 1].Prioridade < prioridade)
        {
            i--;
        }

        fila.Insert(i, nova);

        if (rotina == null)
        {
            rotina = StartCoroutine(Rodar());
        }
    }

    private IEnumerator Rodar()
    {
        while (fila.Count > 0)
        {
            atual = fila[0];
            fila.RemoveAt(0);

            legenda.Dizer("Marcus", atual.Texto, MinigameDialogue.MarcusPitch, atual.Duracao);
            while (legenda.Ocupada)
            {
                if (Time.timeScale == 0f || !Application.isFocused)
                    legenda.Suspender();
                else
                    legenda.Atualizar(Time.deltaTime);
                yield return null;
            }

            atual = null;
            yield return new WaitForSeconds(.2f);
        }

        rotina = null;
    }
}
