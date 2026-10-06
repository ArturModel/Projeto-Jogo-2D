using UnityEngine;

// Porta, portão, entrada de beco: leva para outra cena e diz por onde chegar.
//
// Usa a mesma linguagem do resto do jogo — chega perto, aparece o "E". Portas
// automáticas existem (marque `automatica`), mas o padrão é exigir a tecla:
// num jogo de terror, atravessar uma porta tem que ser uma decisão, não um
// acidente de caminhada.
public class DoorTrigger : Interactable
{
    [Header("Destino")]
    [SerializeField] private string cenaDestino = "GameplayScene";

    [Tooltip("Id do SpawnPoint na cena de destino.")]
    [SerializeField] private string entrada = "padrao";

    [Header("Comportamento")]
    [SerializeField] private string textoDoPrompt = "Entrar";

    [Tooltip("Dispara ao encostar, sem apertar E.")]
    [SerializeField] private bool automatica;

    [Header("Condição (opcional)")]
    [Tooltip("Se preenchido, a porta só funciona com esta flag marcada.")]
    [SerializeField] private string flagNecessaria = "";

    [TextArea]
    [SerializeField] private string mensagemSeTrancada = "Está trancada.";

    private bool jaDisparou;

    public override string Prompt => textoDoPrompt;

    public override void Interact()
    {
        Atravessar();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!automatica || jaDisparou || other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        Atravessar();
    }

    private void Atravessar()
    {
        if (jaDisparou)
        {
            return;
        }

        if (!string.IsNullOrEmpty(flagNecessaria) && !GameState.Tem(flagNecessaria))
        {
            GameMessageUI.Instance.Show(mensagemSeTrancada);
            return;
        }

        jaDisparou = true;
        SceneTransition.Ir(cenaDestino, entrada);
    }
}
