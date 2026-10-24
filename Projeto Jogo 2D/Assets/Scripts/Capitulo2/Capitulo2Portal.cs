using UnityEngine;

public sealed class Capitulo2Portal : Interactable
{
    public string cenaDestino, entradaDestino, textoPrompt, flagNecessaria;
    [TextArea] public string avisoBloqueio;
    public bool automatico;
    public bool saidaInterior;
    private static float ultimoCorte = float.NegativeInfinity;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession() => ultimoCorte = float.NegativeInfinity;
    public override string Prompt => automatico ? "Seguir pela rua" : saidaInterior ? "Sair" : "Abrir porta";
    public override void Interact()
    {
        if (GameMessageUI.IsShowing || BochaMinigameUI.IsShowing || SceneFader.IsTransitioning) return;
        float agora = Time.unscaledTime;
        // E é uma tentativa deliberada: não descartá-la por causa de outra porta.
        // Apenas passagens por contato precisam evitar o retorno automático.
        // Um relógio reiniciado também não deve manter um bloqueio antigo.
        if (automatico && agora >= ultimoCorte && agora - ultimoCorte < 1.5f) return;
        if (!string.IsNullOrEmpty(flagNecessaria) && !GameState.Tem(flagNecessaria))
        { GameMessageUI.Instance.Show(avisoBloqueio, "MARCUS", "marcus"); return; }
        ultimoCorte = agora;
        SceneTransition.Ir(cenaDestino, entradaDestino);
    }
    private void OnTriggerEnter2D(Collider2D other)
    { if (automatico && other.GetComponent<PlayerController>() != null) Interact(); }
}
