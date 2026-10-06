using System.Collections;
using UnityEngine;

// A primeira coisa que acontece depois do prólogo (§8 do documento).
//
// O objetivo aparece discreto e some — nada de HUD fixo. E é aqui que a
// exploração opcional do carro é paga: quem abriu o porta-luvas sabe o
// endereço; quem não abriu vai ter que se virar com a memória do
// protagonista, que é justamente o que não se pode confiar neste jogo.
public class ChegadaEmSantaRita : MonoBehaviour
{
    [SerializeField] private float esperaAntesDaFala = 2.2f;
    [SerializeField] private float esperaAntesDoObjetivo = 1.4f;

    [TextArea]
    [SerializeField] private string comSabendoOEndereco = "Bairro das Casas, 40. É subindo.";

    [TextArea]
    [SerializeField] private string semSaberOEndereco = "Faz tempo. Acho que era pra cima.";

    [TextArea]
    [SerializeField] private string objetivo = "Chegar até a casa do avô.";


    private IEnumerator Start()
    {
        // Entrou pelo menu direto na gameplay, ou já passou daqui: sem
        // narração de chegada.
        if (!GameState.Tem(Flags.PrologoConcluido) || GameState.Tem(Flags.ChegouCasaAvo))
        {
            yield break;
        }

        yield return new WaitForSeconds(esperaAntesDaFala);

        Legenda.Dizer(
            GameState.Tem(Flags.PrologoAbriuPortaLuvas)
                ? comSabendoOEndereco
                : semSaberOEndereco,
            3.5f
        );

        yield return new WaitForSeconds(esperaAntesDoObjetivo);

        Legenda.Dizer(objetivo, 4f);
    }
}
