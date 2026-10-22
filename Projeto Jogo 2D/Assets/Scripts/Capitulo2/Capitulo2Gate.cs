using UnityEngine;

// Porta visível de acesso à consulta pública. O atendimento autoriza o acesso,
// não uma chave genérica; o estado vem do mesmo snapshot de investigação.
public sealed class Capitulo2Gate : MonoBehaviour
{
    public string flag;
    private void OnEnable() { GameState.OnEstadoAlterado += Atualizar; Atualizar(); }
    private void OnDisable() { GameState.OnEstadoAlterado -= Atualizar; }
    private void Atualizar()
    {
        bool fechado = !GameState.Tem(flag);
        foreach (var c in GetComponents<Collider2D>()) c.enabled = fechado;
        foreach (var s in GetComponentsInChildren<SpriteRenderer>()) s.enabled = fechado;
    }
}
