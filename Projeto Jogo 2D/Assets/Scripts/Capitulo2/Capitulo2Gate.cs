using UnityEngine;

// Porta visível de acesso à consulta pública. O atendimento autoriza o acesso,
// não uma chave genérica; o estado vem do mesmo snapshot de investigação.
public sealed class Capitulo2Gate : MonoBehaviour
{
    public string flag;
    private void OnEnable() { GameState.OnEstadoAlterado += Atualizar; Atualizar(); }
    private void OnDisable() { GameState.OnEstadoAlterado -= Atualizar; }
#if UNITY_EDITOR
    // No primeiro carregamento, OnEnable precede a detecção da cena inicial.
    private void Start() { if (Capitulo2Director.ExploracaoNoEditor) Atualizar(); }
#endif
    private void Atualizar()
    {
        bool fechado = !Capitulo2Director.ExploracaoNoEditor && !GameState.Tem(flag);
        foreach (var c in GetComponents<Collider2D>()) c.enabled = fechado;
        foreach (var s in GetComponentsInChildren<SpriteRenderer>()) s.enabled = fechado;
    }
}
