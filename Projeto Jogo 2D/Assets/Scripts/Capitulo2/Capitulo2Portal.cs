using UnityEngine;

public sealed class Capitulo2Portal : Interactable
{
    public string cenaDestino, entradaDestino, textoPrompt, flagNecessaria;
    [TextArea] public string avisoBloqueio;
    public bool automatico;
    private static float ultimoCorte;
    public override string Prompt => automatico ? "Seguir pela rua" : "Abrir porta";
    public override void Interact()
    {
        if (Time.unscaledTime - ultimoCorte < 1.5f || GameMessageUI.IsShowing || BochaMinigameUI.IsShowing) return;
        if (!string.IsNullOrEmpty(flagNecessaria) && !GameState.Tem(flagNecessaria))
        { GameMessageUI.Instance.Show(avisoBloqueio, "MARCUS", "marcus"); return; }
        ultimoCorte = Time.unscaledTime;
        SceneTransition.Ir(cenaDestino, entradaDestino);
    }
    private void OnTriggerEnter2D(Collider2D other)
    { if (automatico && other.GetComponent<PlayerController>() != null) Interact(); }
}
