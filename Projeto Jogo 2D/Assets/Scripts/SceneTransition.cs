using UnityEngine;

// Troca de cena preservando "por onde eu entrei".
//
// O SceneFader já sabe apagar a tela e carregar; o que faltava era o destino
// dentro da cena nova. Sem isso toda porta jogaria o jogador na posição
// gravada na cena, e voltar por uma porta te devolveria ao lugar errado.
public static class SceneTransition
{
    private static string entradaPendente;
    private static Vector2? posicaoPendente;
    public static void IrParaPosicao(string cena, Vector2 posicao)
    {
        entradaPendente = null; posicaoPendente = posicao;
        SceneFader.Instance.LoadScene(cena);
    }

    // Chamado pela porta. O nome tem que bater com o Id de um SpawnPoint na
    // cena de destino.
    public static void Ir(string cena, string entrada)
    {
        if (!string.IsNullOrEmpty(entrada) && entrada != "c1_portao_norte" && entrada != "saida_bar_ze")
            SantaRitaSound.Play("door", .2f);
        entradaPendente = entrada;
        posicaoPendente = null;
        SceneFader.Instance.LoadScene(cena);
    }

    // Chamado pelo PlayerController no Start da cena nova. Devolve false se a
    // entrada não existe — aí o jogador fica onde a cena o colocou, em vez de
    // cair em (0,0) sem aviso.
    public static bool ConsumirEntrada(out Vector2 posicao, out int direcao)
    {
        posicao = Vector2.zero;
        direcao = -1;
        if (posicaoPendente.HasValue)
        { posicao = posicaoPendente.Value; posicaoPendente = null; return true; }

        if (string.IsNullOrEmpty(entradaPendente))
        {
            return false;
        }

        string procurada = entradaPendente;
        entradaPendente = null;

        SpawnPoint ponto = SpawnPoint.Procurar(procurada);

        if (ponto == null)
        {
            Debug.LogWarning($"SpawnPoint \"{procurada}\" não existe nesta cena.");
            return false;
        }

        posicao = ponto.transform.position;
        direcao = ponto.DirecaoInicial;
        return true;
    }
}
