using UnityEngine;

public sealed class Capitulo2Interactable : Interactable
{
    public string interactionId;
    public Sprite arteExame;
    public override string Prompt
    {
        get
        {
            var data = Capitulo2Director.Dados(interactionId);
            if (data == null) return "Examinar";
            if (data.kind == "talk") return "Conversar";
            if (data.kind == "phone") return "Usar telefone";
            if (data.kind == "bocha") return "Observar a cancha";
            return "Examinar";
        }
    }
    public override void Interact() { if (Capitulo2Director.Instance != null) Capitulo2Director.Instance.Interagir(interactionId, arteExame); }
}
