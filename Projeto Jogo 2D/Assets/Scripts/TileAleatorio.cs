using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tile que sorteia uma variante a partir da posição no mapa.
///
/// O sorteio é feito por hash das coordenadas, não por Random: o mesmo
/// quadradinho recebe sempre a mesma arte, então nada pisca ao recarregar a
/// cena nem ao rolar a câmera. Espelhar na horizontal e na vertical multiplica
/// a variação por quatro sem custar arte nova.
/// </summary>
[CreateAssetMenu(fileName = "TileAleatorio", menuName = "Santa Rita/Tile aleatório")]
public class TileAleatorio : TileBase
{
    [Header("Arte")]
    [SerializeField] private Sprite[] variantes;

    [Header("Variação")]
    [SerializeField] private bool espelharHorizontal = true;
    [SerializeField] private bool espelharVertical = true;

    [Tooltip("Muda o sorteio sem trocar a arte. Útil quando duas áreas " +
             "vizinhas ficam parecidas demais.")]
    [SerializeField] private int semente;

    [Header("Física")]
    [SerializeField] private Tile.ColliderType tipoDeColisor = Tile.ColliderType.None;

    public override void GetTileData(Vector3Int posicao, ITilemap mapa, ref TileData dados)
    {
        if (variantes == null || variantes.Length == 0)
        {
            return;
        }

        uint h = Embaralhar(posicao);

        dados.sprite = variantes[(int)(h % (uint)variantes.Length)];
        dados.color = Color.white;
        dados.colliderType = tipoDeColisor;

        float ex = espelharHorizontal && ((h >> 9) & 1u) == 1u ? -1f : 1f;
        float ey = espelharVertical && ((h >> 10) & 1u) == 1u ? -1f : 1f;
        dados.transform = Matrix4x4.Scale(new Vector3(ex, ey, 1f));

        // Sem LockTransform o Tilemap ignora o espelhamento que acabamos de pedir.
        dados.flags = TileFlags.LockTransform | TileFlags.LockColor;
    }

    private uint Embaralhar(Vector3Int p)
    {
        unchecked
        {
            uint h = (uint)(p.x * 73856093) ^ (uint)(p.y * 19349663) ^ (uint)(semente * 83492791);
            h ^= h >> 13;
            h *= 0x85EBCA6B;
            h ^= h >> 16;
            return h;
        }
    }
}
