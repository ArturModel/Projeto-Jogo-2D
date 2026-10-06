using System.Collections;
using UnityEngine;

// O fim do primeiro objetivo — e o começo do mistério de verdade.
//
// O jogador entra procurando o avô e encontra a casa vazia. Nada aqui prova
// sequestro e nada prova que ele saiu por vontade própria; é o bilhete, se o
// jogador achar, que começa a desmontar a primeira suposição. Essa é a regra
// da V2: cada resposta reinterpreta uma pista anterior em vez de acrescentar
// informação.
public class DentroDaCasaDoAvo : MonoBehaviour
{
    [SerializeField] private float espera = 1.8f;

    [TextArea]
    [SerializeField] private string aoEntrar = "A porta estava destrancada.";

    [TextArea]
    [SerializeField] private string vazio = "Vô? ...";

    [TextArea]
    [SerializeField] private string objetivo = "Procurar algum sinal do que aconteceu.";


    private IEnumerator Start()
    {
        bool primeiraVez = !GameState.Tem(Flags.ChegouCasaAvo);
        GameState.Marcar(Flags.ChegouCasaAvo);

        if (!primeiraVez)
        {
            yield break;
        }

        yield return new WaitForSeconds(espera);
        Legenda.Dizer(aoEntrar, 3f);

        yield return new WaitForSeconds(3.4f);
        Legenda.Dizer(vazio, 2.5f);

        yield return new WaitForSeconds(3f);
        Legenda.Dizer(objetivo, 4f);
    }
}
