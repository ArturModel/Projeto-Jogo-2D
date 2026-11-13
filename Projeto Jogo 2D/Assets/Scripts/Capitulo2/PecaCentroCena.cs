using UnityEngine;

// Identifica uma peça da praça durante a transição do gerador para a cena.
// A posição de origem é uma chave estável, independente da posição editável.
internal sealed class PecaCentroCena : MonoBehaviour
{
    public string nomeDeOrigem;
    public Vector2 posicaoDeOrigem;
}
