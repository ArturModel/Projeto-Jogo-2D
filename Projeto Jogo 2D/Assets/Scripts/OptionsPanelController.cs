using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OptionsPanelController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private CanvasGroup categorias;
    [SerializeField] private CanvasGroup audioPanel;
    [SerializeField] private CanvasGroup videoPanel;
    [SerializeField] private CanvasGroup legendasPanel;

    [Header("Título")]
    [SerializeField] private TMP_Text titulo;

    [Header("Primeiras seleções")]
    [SerializeField] private GameObject primeiroCategorias;
    [SerializeField] private GameObject primeiroAudio;
    [SerializeField] private GameObject primeiroVideo;
    [SerializeField] private GameObject primeiroLegendas;

    [Header("Transição")]
    [SerializeField] private float duracao = 0.18f;
    [SerializeField] private float deslocamento = 18f;

    [Header("Menu principal")]
    [SerializeField] private MenuPanelTransition transicaoMenu;

    private CanvasGroup painelAtual;
    private bool emTransicao;

    private void Awake()
    {
        MostrarImediatamente(categorias);

        EsconderImediatamente(audioPanel);
        EsconderImediatamente(videoPanel);
        EsconderImediatamente(legendasPanel);

        painelAtual = categorias;

        if (titulo != null)
        {
            titulo.text = "CONFIGURAÇÕES";
        }
    }

    // Para interfaces montadas por código (menu de pausa). O objeto precisa
    // estar inativo até aqui, porque o Awake já usa estas referências.
    public void Vincular(CanvasGroup categorias, CanvasGroup audio, CanvasGroup video,
                         CanvasGroup legendas, TMP_Text titulo,
                         GameObject primeiroCategorias, GameObject primeiroAudio,
                         GameObject primeiroVideo, GameObject primeiroLegendas,
                         MenuPanelTransition transicao)
    {
        this.categorias = categorias;
        audioPanel = audio;
        videoPanel = video;
        legendasPanel = legendas;
        this.titulo = titulo;
        this.primeiroCategorias = primeiroCategorias;
        this.primeiroAudio = primeiroAudio;
        this.primeiroVideo = primeiroVideo;
        this.primeiroLegendas = primeiroLegendas;
        transicaoMenu = transicao;
    }

    public void AbrirAudio()
    {
        AbrirPainel(
            audioPanel,
            "ÁUDIO",
            primeiroAudio,
            false
        );
    }

    public void AbrirVideo()
    {
        AbrirPainel(
            videoPanel,
            "VÍDEO",
            primeiroVideo,
            false
        );
    }

    public void AbrirLegendas()
    {
        AbrirPainel(
            legendasPanel,
            "LEGENDAS",
            primeiroLegendas,
            false
        );
    }

    public void MostrarCategorias()
    {
        AbrirPainel(
            categorias,
            "CONFIGURAÇÕES",
            primeiroCategorias,
            true
        );
    }

    private void AbrirPainel(
        CanvasGroup destino,
        string novoTitulo,
        GameObject primeiraSelecao,
        bool voltando)
    {
        if (emTransicao)
            return;

        if (destino == null || destino == painelAtual)
            return;

        StartCoroutine(
            TrocarPainel(
                destino,
                novoTitulo,
                primeiraSelecao,
                voltando
            )
        );
    }

    private IEnumerator TrocarPainel(
        CanvasGroup destino,
        string novoTitulo,
        GameObject primeiraSelecao,
        bool voltando)
    {
        emTransicao = true;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        CanvasGroup origem = painelAtual;

        RectTransform origemRect =
            origem.GetComponent<RectTransform>();

        RectTransform destinoRect =
            destino.GetComponent<RectTransform>();

        float direcao = voltando ? -1f : 1f;

        origem.interactable = false;
        origem.blocksRaycasts = false;

        destino.gameObject.SetActive(true);
        destino.alpha = 0f;
        destino.interactable = false;
        destino.blocksRaycasts = false;

        origemRect.anchoredPosition = Vector2.zero;

        destinoRect.anchoredPosition =
            Vector2.right * deslocamento * direcao;

        if (titulo != null)
        {
            titulo.text = novoTitulo;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso =
                Mathf.Clamp01(tempo / duracao);

            float suave =
                progresso *
                progresso *
                (3f - 2f * progresso);

            origem.alpha = 1f - suave;
            destino.alpha = suave;

            origemRect.anchoredPosition =
                Vector2.left *
                deslocamento *
                direcao *
                suave;

            destinoRect.anchoredPosition =
                Vector2.Lerp(
                    Vector2.right *
                    deslocamento *
                    direcao,
                    Vector2.zero,
                    suave
                );

            yield return null;
        }

        origem.alpha = 0f;
        origem.interactable = false;
        origem.blocksRaycasts = false;
        origemRect.anchoredPosition = Vector2.zero;
        origem.gameObject.SetActive(false);

        destino.alpha = 1f;
        destino.interactable = true;
        destino.blocksRaycasts = true;
        destinoRect.anchoredPosition = Vector2.zero;

        painelAtual = destino;

        if (EventSystem.current != null &&
            primeiraSelecao != null)
        {
            EventSystem.current.SetSelectedGameObject(
                primeiraSelecao
            );
        }

        emTransicao = false;
    }

    private void MostrarImediatamente(CanvasGroup grupo)
    {
        if (grupo == null)
            return;

        grupo.gameObject.SetActive(true);
        grupo.alpha = 1f;
        grupo.interactable = true;
        grupo.blocksRaycasts = true;

        RectTransform rect =
            grupo.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
        }
    }

    private void EsconderImediatamente(CanvasGroup grupo)
    {
        if (grupo == null)
            return;

        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;
        grupo.gameObject.SetActive(false);

        RectTransform rect =
            grupo.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (emTransicao)
            return;

        if (painelAtual == categorias)
        {
            transicaoMenu.FecharOpcoes();
        }
        else
        {
            MostrarCategorias();
        }
    }
}