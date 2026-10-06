using UnityEngine;

// O porta-luvas: a interação mais exploratória (§5.3).
//
// Justifica o protagonista saber chegar na casa e entrega um pedaço de contexto
// pessoal sem abrir a lore central. A foto é o item que ganha outro peso
// depois: o avô que ele veio procurar aparece nela ao lado de alguém que o
// jogador ainda não sabe quem é.
public class PortaLuvasCarro : ObjetoCarro
{
    [Header("Conteúdo revelado")]
    [SerializeField] private SpriteRenderer conteudo;

    [Header("Falas")]
    [TextArea]
    [SerializeField]
    private string[] achados =
    {
        "Um mapa dobrado e uma foto. Bairro das Casas, 40 — a letra é dele.",
        "Ele, a mãe e eu. Não lembro de ninguém tirando essa foto.",
    };

    private int etapa;

    public override void Ativar()
    {
        if (etapa == 0)
        {
            GameState.Marcar(Flags.PrologoAbriuPortaLuvas);

            if (conteudo != null)
            {
                conteudo.enabled = true;
            }

            Legenda.Dizer(achados[0], 4f, Legenda.PrioridadeOpcional);
            etapa = 1;
            return;
        }

        if (etapa == 1)
        {
            GameState.Marcar(Flags.PrologoViuFotoAvo);
            Legenda.Dizer(achados[1], 4f, Legenda.PrioridadeOpcional);
            etapa = 2;
            return;
        }

        Legenda.Dizer("Depois eu olho isso direito.", 2.5f, Legenda.PrioridadeOpcional);
    }
}
