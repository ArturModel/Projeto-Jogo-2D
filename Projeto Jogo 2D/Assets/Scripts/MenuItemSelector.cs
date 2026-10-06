using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemSelector : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler
{
    [Header("Indicador")]
    [SerializeField] private RectTransform indicador;

    [Header("Som")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somSelecao;

    private static GameObject ultimoSelecionado;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarSelecao()
    {
        ultimoSelecionado = null;
    }

    // Para interfaces montadas por código (menu de pausa).
    public void Vincular(RectTransform indicador, AudioSource fonte, AudioClip som)
    {
        this.indicador = indicador;
        audioSource = fonte;
        somSelecao = som;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current == null)
            return;

        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        else
        {
            MoverIndicador();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MoverIndicador();

        bool realmenteMudou =
            ultimoSelecionado != null &&
            ultimoSelecionado != gameObject;

        if (realmenteMudou &&
            audioSource != null &&
            somSelecao != null)
        {
            audioSource.PlayOneShot(somSelecao);
        }

        ultimoSelecionado = gameObject;
    }

    private void MoverIndicador()
    {
        if (indicador == null)
            return;

        Vector3 novaPosicao = indicador.position;
        novaPosicao.y = transform.position.y;
        indicador.position = novaPosicao;
    }
}