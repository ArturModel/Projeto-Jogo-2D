using System.Collections.Generic;
using UnityEngine;

// Um lugar nomeado onde o jogador pode nascer ao entrar na cena.
// A porta da rua manda "porta_frente"; quem procura esse nome é este registro.
public class SpawnPoint : MonoBehaviour
{
    private static readonly List<SpawnPoint> Registro = new List<SpawnPoint>();

    [Tooltip("Nome usado pela porta de origem. Ex: \"porta_frente\", \"saida_carro\".")]
    [SerializeField] private string id = "padrao";

    [Tooltip("Para onde o jogador olha ao nascer aqui. 0=Baixo 1=Cima 2=Esquerda 3=Direita")]
    [SerializeField] private int direcaoInicial = 0;

    public string Id => id;
    public int DirecaoInicial => direcaoInicial;

    public void Configurar(string novoId, int novaDirecao = 0)
    {
        id = novoId;
        direcaoInicial = novaDirecao;
    }

    private void OnEnable()
    {
        Registro.Add(this);
    }

    private void OnDisable()
    {
        Registro.Remove(this);
    }

    public static SpawnPoint Procurar(string id)
    {
        foreach (SpawnPoint ponto in Registro)
        {
            if (ponto != null && ponto.id == id)
            {
                return ponto;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.4f, 0.9f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.6f);
    }
}
