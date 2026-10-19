using UnityEngine;

// Qualquer coisa que o jogador possa examinar: um telefone fora do gancho, uma
// xícara fria, uma foto na parede.
//
// É o tijolo do sistema de pistas. Cada item guarda uma sequência de falas —
// olhar duas vezes pode revelar mais do que olhar uma — e opcionalmente marca
// uma flag, que é como uma pista "encontrada" muda um diálogo lá na frente.
public class ItemDeCena : Interactable
{
    [Header("Interação")]
    [SerializeField] private string rotulo = "Examinar";

    [TextArea]
    [SerializeField] private string[] falas = { "Nada de mais." };

    [Header("Progresso")]
    [Tooltip("Marcada na primeira vez que o jogador examina. Deixe vazio se não precisa.")]
    [SerializeField] private string flagAoExaminar = "";

    [Tooltip("Marcada só quando o jogador chega na última fala.")]
    [SerializeField] private string flagAoTerminar = "";

    private int etapa;

    public override string Prompt => rotulo;

    public override void Interact()
    {
        if (falas == null || falas.Length == 0)
        {
            return;
        }

        if (etapa == 0 && !string.IsNullOrEmpty(flagAoExaminar))
        {
            GameState.Marcar(flagAoExaminar);
            RegistrarPistaLegada(flagAoExaminar);
        }

        GameMessageUI.Instance.Show(falas[Mathf.Min(etapa, falas.Length - 1)]);

        if (etapa < falas.Length - 1)
        {
            etapa++;

            if (etapa == falas.Length - 1 && !string.IsNullOrEmpty(flagAoTerminar))
            {
                GameState.Marcar(flagAoTerminar);
                RegistrarPistaLegada(flagAoTerminar);
            }
        }
        else if (!string.IsNullOrEmpty(flagAoTerminar))
        {
            GameState.Marcar(flagAoTerminar);
            RegistrarPistaLegada(flagAoTerminar);
        }
    }

    private static void RegistrarPistaLegada(string flag)
    {
        if (flag == Flags.CasaAvo_LeuBilhete)
        {
            GameState.RegistrarNotaDiario(
                "bilhete_casa_adilio",
                "Bilhete na cômoda de Adílio",
                "PISTA",
                "Um recado deixado na cômoda aponta que Adílio subiu para resolver um assunto urgente perto do Bar do Zé."
            );
        }
        else if (flag == Flags.CasaAvo_OlhouTelefone)
        {
            GameState.RegistrarNotaDiario(
                "telefone_casa_adilio",
                "Telefone de Adílio fora do gancho",
                "PISTA",
                "O aparelho fixo da sala ficou mal encaixado no gancho hoje cedo, explicando por que minhas ligações da estrada não completavam."
            );
        }

        InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo();
    }
}
