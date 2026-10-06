using System.Collections;
using UnityEngine;

// O celular: o objeto mais importante da abertura (§5.1).
//
// Ele apresenta a ausência do avô sem que ninguém explique nada, e planta o
// tema que atravessa o jogo inteiro — alguém tentando falar com alguém por
// telefone e não conseguindo. A ligação que não completa aqui é a primeira
// de uma série: o orelhão da noite de Júlia, o telefone do final.
public class CelularCarro : ObjetoCarro
{
    [Header("Áudio")]
    [SerializeField] private string camadaChamada = "telefone";

    [Header("Tempos")]
    [SerializeField] private float duracaoChamada = 4.5f;

    private int etapa;
    private bool ocupado;

    public override bool Disponivel => !ocupado;

    public override void Ativar()
    {
        if (ocupado)
        {
            return;
        }

        GameState.Marcar(Flags.PrologoOlhouCelular);

        switch (etapa)
        {
            case 0:
                etapa = 1;
                Legenda.Dizer("Quatro chamadas. Nenhuma atendida.",
                              3f, Legenda.PrioridadeOpcional);
                break;

            case 1:
                etapa = 2;
                StartCoroutine(Ligar());
                break;

            default:
                Legenda.Dizer("Não adianta insistir daqui.",
                              2.5f, Legenda.PrioridadeOpcional);
                break;
        }
    }

    private IEnumerator Ligar()
    {
        ocupado = true;
        GameState.Marcar(Flags.PrologoTentouLigarAvo);

        Legenda.Dizer("\"Vô, atende...\"", 2.5f, Legenda.PrioridadeOpcional);

        if (AudioAmbiente.Instance != null)
        {
            AudioAmbiente.Instance.Definir(camadaChamada, 0.55f, 0.2f);
        }

        yield return new WaitForSeconds(duracaoChamada);

        if (AudioAmbiente.Instance != null)
        {
            AudioAmbiente.Instance.Definir(camadaChamada, 0f, 0.3f);
        }

        // A queda por falta de sinal é de propósito: nada confirma que ele não
        // pode atender, só que ninguém atendeu.
        Legenda.Dizer("Caiu. Sinal fraco por aqui.", 3f, Legenda.PrioridadeOpcional);

        ocupado = false;
    }
}
