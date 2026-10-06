using System.Collections.Generic;
using UnityEngine;

// Base de tudo que o jogador pode interagir apertando E.
// Mantém um registro estático para o PlayerController achar o mais próximo
// sem precisar de colliders ou varredura na cena inteira.
public abstract class Interactable : MonoBehaviour
{
    private static readonly List<Interactable> Registry = new List<Interactable>();

    [Header("Interação")]
    [SerializeField] private float interactionRadius = 1.6f;

    public float InteractionRadius => interactionRadius;

    protected virtual void OnEnable()
    {
        Registry.Add(this);
    }

    protected virtual void OnDisable()
    {
        Registry.Remove(this);
    }

    public abstract void Interact();

    // Dica curta mostrada quando o jogador está perto (ex: "Atender").
    public virtual string Prompt => "Interagir";

    public static Interactable FindNearest(Vector2 position)
    {
        Interactable nearest = null;
        float bestDistance = float.MaxValue;

        foreach (Interactable candidate in Registry)
        {
            if (candidate == null)
            {
                continue;
            }

            float distance = Vector2.Distance(position, candidate.transform.position);

            if (distance > candidate.interactionRadius || distance >= bestDistance)
            {
                continue;
            }

            bestDistance = distance;
            nearest = candidate;
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.4f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
