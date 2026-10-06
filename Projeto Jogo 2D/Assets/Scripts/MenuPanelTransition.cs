using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPanelTransition : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup optionsMenu;
    [SerializeField] private CanvasGroup focusOverlay;

    [Header("Posições")]
    [SerializeField] private RectTransform mainMenuRect;
    [SerializeField] private RectTransform optionsMenuRect;

    [Header("Seleção")]
    [SerializeField] private GameObject primeiroBotaoPrincipal;
    [SerializeField] private GameObject primeiroBotaoOpcoes;

    [Header("Animação")]
    [SerializeField] private float duracao = 0.25f;
    [SerializeField] private float deslocamento = 18f;
    [SerializeField] private float escurecimento = 0.55f;

    private Vector2 posicaoMainMenu;
    private Vector2 posicaoOptionsMenu;
    private bool emTransicao;

    private void Awake()
    {
        posicaoMainMenu = mainMenuRect.anchoredPosition;
        posicaoOptionsMenu = optionsMenuRect.anchoredPosition;

        ConfigurarGrupo(mainMenu, 1f, true);

        ConfigurarGrupo(optionsMenu, 0f, false);
        optionsMenu.gameObject.SetActive(false);

        focusOverlay.alpha = 0f;
        focusOverlay.interactable = false;
        focusOverlay.blocksRaycasts = false;
    }

    // Para interfaces montadas por código (menu de pausa). O objeto precisa
    // estar inativo até aqui, porque o Awake já usa estas referências.
    public void Vincular(CanvasGroup principal, CanvasGroup opcoes, CanvasGroup overlay,
                         RectTransform principalRect, RectTransform opcoesRect,
                         GameObject primeiroPrincipal, GameObject primeiroOpcoes)
    {
        mainMenu = principal;
        optionsMenu = opcoes;
        focusOverlay = overlay;
        mainMenuRect = principalRect;
        optionsMenuRect = opcoesRect;
        primeiroBotaoPrincipal = primeiroPrincipal;
        primeiroBotaoOpcoes = primeiroOpcoes;
    }

    public void AbrirOpcoes()
    {
        if (!emTransicao)
            StartCoroutine(TrocarPainel(true));
    }

    public void FecharOpcoes()
    {
        if (!emTransicao)
            StartCoroutine(TrocarPainel(false));
    }

    private IEnumerator TrocarPainel(bool abrirOpcoes)
    {
        emTransicao = true;

        CanvasGroup origem =
            abrirOpcoes ? mainMenu : optionsMenu;

        CanvasGroup destino =
            abrirOpcoes ? optionsMenu : mainMenu;

        RectTransform origemRect =
            abrirOpcoes ? mainMenuRect : optionsMenuRect;

        RectTransform destinoRect =
            abrirOpcoes ? optionsMenuRect : mainMenuRect;

        Vector2 origemInicial =
            abrirOpcoes ? posicaoMainMenu : posicaoOptionsMenu;

        Vector2 destinoFinal =
            abrirOpcoes ? posicaoOptionsMenu : posicaoMainMenu;

        float direcao = abrirOpcoes ? 1f : -1f;

        origem.interactable = false;
        origem.blocksRaycasts = false;

        destino.gameObject.SetActive(true);
        destino.alpha = 0f;
        destino.interactable = false;
        destino.blocksRaycasts = false;

        destinoRect.anchoredPosition =
            destinoFinal + Vector2.right * deslocamento * direcao;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        float overlayInicial = focusOverlay.alpha;
        float overlayFinal = abrirOpcoes ? escurecimento : 0f;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(tempo / duracao);
            float suave = t * t * (3f - 2f * t);

            origem.alpha = 1f - suave;
            destino.alpha = suave;

            origemRect.anchoredPosition =
                Vector2.Lerp(
                    origemInicial,
                    origemInicial - Vector2.right *
                    deslocamento * direcao,
                    suave
                );

            destinoRect.anchoredPosition =
                Vector2.Lerp(
                    destinoFinal + Vector2.right *
                    deslocamento * direcao,
                    destinoFinal,
                    suave
                );

            focusOverlay.alpha =
                Mathf.Lerp(
                    overlayInicial,
                    overlayFinal,
                    suave
                );

            yield return null;
        }

        origem.alpha = 0f;
        origem.gameObject.SetActive(false);
        origemRect.anchoredPosition = origemInicial;

        destino.alpha = 1f;
        destino.interactable = true;
        destino.blocksRaycasts = true;
        destinoRect.anchoredPosition = destinoFinal;

        GameObject primeiroSelecionado =
            abrirOpcoes
                ? primeiroBotaoOpcoes
                : primeiroBotaoPrincipal;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(
                primeiroSelecionado
            );

        emTransicao = false;
    }

    private void ConfigurarGrupo(
        CanvasGroup grupo,
        float alpha,
        bool interativo)
    {
        grupo.alpha = alpha;
        grupo.interactable = interativo;
        grupo.blocksRaycasts = interativo;
    }
}