using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Pensamentos do protagonista em legenda, no rodapé.
//
// A GameMessageUI é uma caixa: serve para "alguém está falando com você".
// Aqui é o contrário — é a cabeça do personagem, e não deve interromper o que
// o jogador está fazendo. Por isso legenda simples, sem caixa e sem esperar
// tecla.
//
// A fila com prioridade existe porque o prólogo mistura falas roteirizadas
// (que têm hora marcada) com falas de interação opcional. Sem prioridade, mexer
// no rádio na hora errada engoliria a fala que apresenta o sumiço do avô.
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
    private TextMeshProUGUI texto;
    private CanvasGroup grupo;
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
        Canvas canvas = UIFactory.CreateCanvas(transform, "LegendaCanvas", 500);
        CanvasScaler escala = canvas.GetComponent<CanvasScaler>();
        escala.referenceResolution = ResolucaoUI;

        grupo = canvas.gameObject.AddComponent<CanvasGroup>();
        grupo.alpha = 0f;
        grupo.blocksRaycasts = false;
        grupo.interactable = false;

        texto = UIFactory.CreateLabel(canvas.transform, "Texto", 18f);
        texto.alignment = TextAlignmentOptions.Bottom;
        texto.textWrappingMode = TextWrappingModes.Normal;
        texto.color = new Color(0.88f, 0.86f, 0.80f, 1f);

        // sombra dura, para a legenda ler contra o para-brisa claro ou o asfalto
        texto.fontMaterial.EnableKeyword("UNDERLAY_ON");

        RectTransform r = texto.rectTransform;
        r.anchorMin = new Vector2(0.5f, 0f);
        r.anchorMax = new Vector2(0.5f, 0f);
        r.pivot = new Vector2(0.5f, 0f);
        r.anchoredPosition = new Vector2(0f, 26f);
        r.sizeDelta = new Vector2(ResolucaoUI.x * 0.72f, 70f);
    }

    public static void Dizer(string frase, float duracao = 3f,
                             int prioridade = PrioridadeRoteiro)
    {
        // Mesma configuração do painel de legendas do menu (e da pausa).
        if (!SubtitleSettingsController.LegendasAtivas)
        {
            return;
        }

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

            texto.text = atual.Texto;

            // Lido a cada fala: mudar o tamanho na pausa vale já na próxima.
            texto.fontSize = 18f * SubtitleSettingsController.MultiplicadorTamanho;

            yield return Aparecer(1f, 0.25f);
            yield return new WaitForSeconds(atual.Duracao);
            yield return Aparecer(0f, 0.4f);

            atual = null;

            // respiro entre falas: legenda emendada vira parede de texto
            yield return new WaitForSeconds(0.2f);
        }

        rotina = null;
    }

    private IEnumerator Aparecer(float alvo, float tempo)
    {
        float inicio = grupo.alpha;
        float t = 0f;

        while (t < tempo)
        {
            t += Time.deltaTime;
            grupo.alpha = Mathf.Lerp(inicio, alvo, t / tempo);
            yield return null;
        }

        grupo.alpha = alvo;
    }
}
