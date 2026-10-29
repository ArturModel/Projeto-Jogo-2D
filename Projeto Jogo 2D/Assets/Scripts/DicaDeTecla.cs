using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Aviso de controle no estilo dos jogos de terror: a tecla e duas palavras,
// pequeno e embaixo da tela. Aparece devagar, fica até o jogador usar a tecla
// (ou o tempo acabar) e some. Avisos pedidos em sequência entram na fila.
public class DicaDeTecla : MonoBehaviour
{
    private struct Pedido
    {
        public string tecla;
        public string texto;
        public Func<bool> feito;
        public float tempoMaximo;
    }

    private static DicaDeTecla instance;

    private static DicaDeTecla Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DicaDeTecla");
                instance = go.AddComponent<DicaDeTecla>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    private const float DuracaoFadeEntrada = 0.8f;
    private const float DuracaoFadeSaida = 0.6f;
    // Depois de usada a tecla, o aviso ainda fica um instante: some com calma.
    private const float EsperaAposUso = 0.5f;

    private readonly Queue<Pedido> fila = new Queue<Pedido>();
    private CanvasGroup group;
    private Image icone;
    private TextMeshProUGUI rotulo;
    private RectTransform linhaRect;
    private bool rodando;

    // tecla: nome do sprite em Resources/UI sem o prefixo "Tecla" (ex: "F").
    public static void Mostrar(string tecla, string texto, Func<bool> feito, float tempoMaximo = 12f)
    {
        Instance.fila.Enqueue(new Pedido { tecla = tecla, texto = texto, feito = feito, tempoMaximo = tempoMaximo });
        if (!Instance.rodando) Instance.StartCoroutine(Instance.Rodar());
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Build();
    }

    private void Build()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "DicaCanvas", 8950);
        group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;

        // Linha centralizada embaixo: [tecla] texto
        GameObject linha = new GameObject("Linha", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        linha.transform.SetParent(canvas.transform, false);
        linhaRect = linha.GetComponent<RectTransform>();
        linhaRect.anchorMin = linhaRect.anchorMax = new Vector2(0.5f, 0f);
        linhaRect.pivot = new Vector2(0.5f, 0f);
        linhaRect.anchoredPosition = new Vector2(0f, 28f);

        HorizontalLayoutGroup layout = linha.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = linha.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject iconeGo = new GameObject("Tecla", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        iconeGo.transform.SetParent(linha.transform, false);
        icone = iconeGo.GetComponent<Image>();
        icone.raycastTarget = false;
        LayoutElement le = iconeGo.GetComponent<LayoutElement>();
        le.preferredWidth = le.preferredHeight = 17f;

        rotulo = UIFactory.CreateLabel(linha.transform, "Texto", 16f);
        rotulo.color = new Color(0.90f, 0.88f, 0.82f, 1f);
        rotulo.alignment = TextAlignmentOptions.MidlineLeft;
        rotulo.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private IEnumerator Rodar()
    {
        rodando = true;

        while (fila.Count > 0)
        {
            Pedido p = fila.Dequeue();
            icone.sprite = UIFactory.LoadSprite("Tecla" + p.tecla);
            rotulo.text = p.texto;
            LayoutRebuilder.ForceRebuildLayoutImmediate(linhaRect);

            float tempo = 0f;
            float visivel = 0f;
            float aposUso = -1f;

            while (true)
            {
                // Some enquanto há diálogo, mapa ou pausa por cima, sem perder a vez.
                bool encoberto = GameMessageUI.IsShowing || MapaInterativoUI.IsShowing || MenuPausa.Aberto;
                bool usada = p.feito != null && p.feito();
                if (usada && aposUso < 0f) aposUso = 0f;

                if (!encoberto) tempo += Time.unscaledDeltaTime;
                if (aposUso >= 0f) aposUso += Time.unscaledDeltaTime;

                bool acabou = aposUso >= EsperaAposUso || tempo >= p.tempoMaximo;
                float alvo = encoberto || acabou ? 0f : 1f;
                float velocidade = alvo > visivel ? 1f / DuracaoFadeEntrada : 1f / DuracaoFadeSaida;
                visivel = Mathf.MoveTowards(visivel, alvo, velocidade * Time.unscaledDeltaTime);
                group.alpha = visivel;

                if (acabou && visivel <= 0f) break;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.4f);
        }

        rodando = false;
    }
}
