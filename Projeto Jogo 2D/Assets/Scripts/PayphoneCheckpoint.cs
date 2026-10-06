using UnityEngine;
using UnityEngine.SceneManagement;

// O orelhão: ponto de save e, ao mesmo tempo, a voz que sabe o que o
// protagonista ainda não admite. Cada uso avança uma linha da lista —
// a voz "sabe mais" a cada vez que você atende.
public class PayphoneCheckpoint : Interactable
{
    [Header("Orelhão")]
    [TextArea]
    [SerializeField]
    private string[] falas =
    {
        "\"Alô?\"",
        "\"Você ainda está aí?\"",
        "\"Você disse que ligou.\"",
        "\"Ele saiu do clube vivo.\"",
        "\"Dessa vez, fala.\"",
        "\"Por que você não ligou?\"",
    };

    [SerializeField] private string textoDeSave = "O telefone registra sua passagem.";

    public override string Prompt => "Atender";

    public override void Interact()
    {
        CheckpointSystem.Save(
            SceneManager.GetActiveScene().name,
            transform.position
        );

        GameState.DefinirNumero(
            Flags.OrelhaoUsos,
            GameState.Numero(Flags.OrelhaoUsos) + 1
        );

        GameMessageUI.Instance.Show($"{ProximaFala()}\n<size=70%>{textoDeSave}</size>");
    }

    private string ProximaFala()
    {
        if (falas == null || falas.Length == 0)
        {
            return string.Empty;
        }

        int index = Mathf.Clamp(
            GameState.Numero(Flags.OrelhaoVozIndice),
            0,
            falas.Length - 1
        );

        string fala = falas[index];

        // A última fala fica: depois dela, não há mais nada a dizer.
        if (index < falas.Length - 1)
        {
            GameState.DefinirNumero(Flags.OrelhaoVozIndice, index + 1);
        }

        return fala;
    }
}
