using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Capitulo2Portal : Interactable
{
    public string cenaDestino, entradaDestino, textoPrompt, flagNecessaria;
    [TextArea] public string avisoBloqueio;
    public bool automatico;
    public bool saidaInterior;
    private static float ultimoCorte = float.NegativeInfinity;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession() => ultimoCorte = float.NegativeInfinity;
    public override bool CanInteractAt(Vector2 p) => base.CanInteractAt(p) && (!saidaInterior || SantaRitaExitThreshold.OnMat(p,transform.position));
    public override string Prompt => automatico ? "Seguir pela rua" : saidaInterior ? "Sair" : "Abrir porta";
    public override void Interact()
    {
        if (GameMessageUI.IsShowing || BochaMinigameUI.IsShowing || SceneFader.IsTransitioning) return;
        float agora = Time.unscaledTime;
        // E é uma tentativa deliberada: não descartá-la por causa de outra porta.
        // Apenas passagens por contato precisam evitar o retorno automático.
        // Um relógio reiniciado também não deve manter um bloqueio antigo.
        if (automatico && agora >= ultimoCorte && agora - ultimoCorte < 1.5f) return;
        if (!Capitulo2Director.ExploracaoNoEditor &&
            !string.IsNullOrEmpty(flagNecessaria) && !GameState.Tem(flagNecessaria))
        { GameMessageUI.Instance.Show(avisoBloqueio, "MARCUS", "marcus"); return; }
        ultimoCorte = agora;
        if (saidaInterior && Capitulo2Interior3D.Atual != null)
        {
            Capitulo2Interior3D.Atual.DispararSaidaPeloPortal(this);
            return;
        }
        if (!saidaInterior && Capitulo2Interior3D.EhCenaInterior3D(cenaDestino))
        {
            if (SceneManager.GetActiveScene().name == "CentroSantaRita" &&
                Capitulo2CentroPortalAnim.SuportaCena(cenaDestino))
            {
                Capitulo2CentroPortalAnim.Entrar(this, cenaDestino, entradaDestino);
                return;
            }
            Vector2 foco = Vector2.one * .5f;
            var cam = Camera.main;
            if (cam != null)
            {
                Vector3 vp = cam.WorldToViewportPoint(transform.position + Vector3.up * .45f);
                if (vp.z > 0f)
                    foco = new Vector2(Mathf.Clamp(vp.x, .18f, .82f), Mathf.Clamp(vp.y, .18f, .82f));
            }
            Capitulo2Interior3D.PrepararFocoEntrada(foco);
            SceneFader.Instance.TransicionarPortaPosto(
                foco,
                () => SceneTransition.PrepararEntrada(entradaDestino),
                () => Capitulo2Interior3D.FocoDaPorta,
                Capitulo2Interior3D.AnimarEntrada,
                Capitulo2Interior3D.DuracaoEntrada,
                cenaDestino: cenaDestino);
            return;
        }
        SceneTransition.Ir(cenaDestino, entradaDestino);
    }
    private void OnTriggerEnter2D(Collider2D other)
    { if (automatico && other.GetComponent<PlayerController>() != null) Interact(); }
}
