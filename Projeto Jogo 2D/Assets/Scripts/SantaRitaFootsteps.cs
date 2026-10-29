using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public sealed class SantaRitaFootsteps : MonoBehaviour
{
    private PlayerController player;
    private int variant;
    private Tilemap[] maps;
    private void OnEnable()
    {
        player = GetComponent<PlayerController>();
        if (player != null) player.PeApoiou += ApoiarPe;
        maps = FindObjectsByType<Tilemap>();
    }
    private void OnDisable() { if (player != null) player.PeApoiou -= ApoiarPe; }
    private void ApoiarPe()
    {
        Vector2 now = transform.position;
        string scene = SceneManager.GetActiveScene().name;
        string surface = scene == "CasaDoAvo" || scene == "BarDoZe" ? "wood" : "concrete";
        if(Capitulo1Director.ZonaAtual==Capitulo1Director.ZonaId.Interior_CasaDoAvo||Capitulo1Director.ZonaAtual==Capitulo1Director.ZonaId.Interior_BarDoZe)surface="wood";
        foreach (var map in maps)
        {
            if (map == null) continue;
            if (map.name != "Terreno") continue; // Decals não substituem o material do chão.
            var tile = map.GetTile(map.WorldToCell(now)); if (tile == null) continue;
            string name = tile.name.ToLowerInvariant();
            if (name.Contains("grama") || name.Contains("terra") || name.Contains("areia")) surface = "grass";
            else if (name.Contains("madeira")) surface = "wood";
            break;
        }
        if(SantaRitaShallowStairs.SpeedAt(now)<1)surface="concrete";
        variant = (variant + 1) % 3;
        if(!Capitulo1Director.EstaEmInterior&&(Capitulo2Director.Instance==null||!Capitulo2Director.Instance.interior))
        {var rain=FindAnyObjectByType<Chuva>();if(rain!=null)rain.RespingoDePasso(now);}
        SantaRitaSound.Play("footstep_" + surface + "_00" + variant, surface == "grass" ? .13f : .18f,
            variant == 1 ? .97f : 1.03f, .12f);
    }
}
