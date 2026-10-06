using System.Collections;
using UnityEngine;

// O rádio: ambientação e informação opcional (§5.2).
//
// A transmissão nunca entrega a trama. Fala de chuva, estrada e aviso da
// comunidade — e cita Santa Rita antes da placa aparecer, para o nome chegar
// ao jogador pelo ambiente e não por um letreiro. Nada sobre Júlia, Lucas ou a
// tragédia: no prólogo isso ainda não existe para o jogador.
public class RadioCarro : ObjetoCarro
{
    [Header("Áudio")]
    [SerializeField] private string camadaEstatica = "radio";
    [SerializeField] private float volumeLigado = 0.32f;

    [Header("Transmissão")]
    [TextArea]
    [SerializeField]
    private string[] transmissao =
    {
        "\"...volume de chuva acima da média para a época...\"",
        "\"...pista molhada na vicinal, atenção redobrada...\"",
        "\"...a comunidade de Santa Rita informa que a celebração...\"",
        "\"...segue mantida, apesar do tempo.\"",
    };

    [SerializeField] private float intervaloEntreFalas = 4.5f;

    private bool ligado;
    private Coroutine locucao;

    public override void Ativar()
    {
        ligado = !ligado;

        if (AudioAmbiente.Instance != null)
        {
            AudioAmbiente.Instance.Definir(camadaEstatica, ligado ? volumeLigado : 0f, 0.4f);
        }

        if (ligado)
        {
            GameState.Marcar(Flags.PrologoOuviuRadio);
            locucao = StartCoroutine(Transmitir());
            return;
        }

        if (locucao != null)
        {
            StopCoroutine(locucao);
            locucao = null;
        }
    }

    private IEnumerator Transmitir()
    {
        // Um respiro antes: o rádio demora a "pegar" a estação.
        yield return new WaitForSeconds(1.2f);

        foreach (string linha in transmissao)
        {
            if (!ligado)
            {
                yield break;
            }

            Legenda.Dizer(linha, intervaloEntreFalas - 1f, Legenda.PrioridadeOpcional);
            yield return new WaitForSeconds(intervaloEntreFalas);
        }

        locucao = null;
    }
}
