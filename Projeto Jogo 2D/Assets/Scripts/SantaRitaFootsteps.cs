using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public sealed class SantaRitaFootsteps : MonoBehaviour
{
    private PlayerController player;
    private int variant;
    private Tilemap[] maps;
    private Chuva rain;

    private void OnEnable()
    {
        player = GetComponent<PlayerController>();
        if (player != null) player.PeApoiou += ApoiarPe;
    }

    private void OnDisable()
    {
        if (player != null) player.PeApoiou -= ApoiarPe;
        maps = null;
        rain = null;
    }

    private void ApoiarPe()
    {
        Vector2 posicao = transform.position;
        bool exterior = !Capitulo1Director.EstaEmInterior &&
            (Capitulo2Director.Instance == null || !Capitulo2Director.Instance.interior);
        string cena = SceneManager.GetActiveScene().name;
        string superficie = cena == "CasaDoAvo" || cena == "BarDoZe" ||
            Capitulo1Director.ZonaAtual is Capitulo1Director.ZonaId.Interior_CasaDoAvo or
            Capitulo1Director.ZonaId.Interior_BarDoZe ? "wood" : "concrete";

        float agua = 0f;
        float ganhoSuperficie = -1f;
        float pitchBase = 1f;
        if (SantaRitaShallowStairs.SpeedAt(posicao) >= 1f)
            superficie = MaterialDoPiso(posicao, superficie, exterior, out agua, out ganhoSuperficie, out pitchBase);
        else
            superficie = "concrete";

        if (exterior)
        {
            if (cena == "CentroSantaRita" && agua < .35f)
                agua = Mathf.Max(agua, CentroImersaoAmbiente.CoberturaDePocaCentro(posicao));
            if (rain == null) rain = FindAnyObjectByType<Chuva>();
            if (rain != null)
            {
                if (rain.DentroDePoca(posicao)) agua = 1f;
                rain.RespingoDePasso(posicao, agua >= .35f);
            }
            if (cena == "CentroSantaRita" && agua < .35f && superficie == "wood")
                CentroImersaoAmbiente.NotificarPassoMadeiraCentro(posicao);
        }

        float volume = ganhoSuperficie >= 0f ? ganhoSuperficie : (superficie == "grass" ? .13f : .18f);
        if (agua >= .35f)
        {
            superficie = "puddle";
            pitchBase = 1f;
            volume = Mathf.Lerp(.63f, .90f, Mathf.InverseLerp(.35f, 1f, agua));
        }
        else if (ganhoSuperficie < 0f && superficie == "asphalt")
            volume = .76f;
        else if (ganhoSuperficie < 0f && superficie == "sidewalk")
            volume = .55f;

        variant = (variant + 1) % 3;
        float pitch = pitchBase * (variant == 0 ? 1f : variant == 1 ? .97f : 1.03f);
        SantaRitaSound.Play("footstep_" + superficie + "_00" + variant, volume, pitch, .12f);
    }

    private Tilemap mapaAguaMadeiraCentro;
    private Tilemap mapaPavimentosCentro;

    private string MaterialDoPiso(Vector2 posicao, string fallback, bool exterior, out float agua,
        out float ganhoCustom, out float pitchBase)
    {
        agua = 0f;
        ganhoCustom = -1f;
        pitchBase = 1f;
        // O cenário do Capítulo 1 monta os tilemaps ao carregar a cena.
        if (maps == null)
        {
            maps = FindObjectsByType<Tilemap>();
            foreach (var m in maps)
            {
                if (m == null) continue;
                if (m.name == "Água e madeira do Centro") mapaAguaMadeiraCentro = m;
                else if (m.name == "Pavimentos do Centro") mapaPavimentosCentro = m;
            }
        }

        if (exterior && mapaAguaMadeiraCentro != null && mapaAguaMadeiraCentro.gameObject.activeInHierarchy)
        {
            var tilePonte = mapaAguaMadeiraCentro.GetTile(mapaAguaMadeiraCentro.WorldToCell(posicao));
            if (tilePonte != null && tilePonte.name == "Madeira")
            {
                ganhoCustom = .46f;
                pitchBase = .91f;
                return "wood";
            }
        }

        if (exterior && mapaPavimentosCentro != null && mapaPavimentosCentro.gameObject.activeInHierarchy)
        {
            var tileCentro = mapaPavimentosCentro.GetTile(mapaPavimentosCentro.WorldToCell(posicao));
            if (tileCentro != null)
            {
                switch (tileCentro.name)
                {
                    case "Asfalto":
                        return "asphalt";
                    case "Fissura":
                        pitchBase = .94f;
                        return "asphalt";
                    case "Calcada":
                        return "sidewalk";
                    case "Adro":
                        ganhoCustom = .58f;
                        pitchBase = .92f;
                        return "sidewalk";
                    case "Pedregulho":
                        ganhoCustom = .36f;
                        pitchBase = 1.14f;
                        return "concrete";
                    case "Terra":
                        ganhoCustom = .22f;
                        pitchBase = .88f;
                        return "grass";
                    case "Grama":
                        return "grass";
                }
            }
        }

        foreach (var map in maps)
        {
            if (map == null || !map.gameObject.activeInHierarchy) continue;
            bool rua = exterior && map.name.StartsWith("Piso_Zona", System.StringComparison.Ordinal);
            if (!rua && map.name != "Terreno") continue; // Detalhes decorativos ficam em outra camada.
            var tile = map.GetTile(map.WorldToCell(posicao));
            if (tile == null) continue;

            if (rua)
            {
                var terreno = map.GetComponentInParent<SantaRitaRuaNorte>();
                switch (tile.name)
                {
                    case "Road":
                    case "Crack":
                    case "Puddle":
                    case "Stripe":
                        if (terreno != null) agua = terreno.CoberturaDePoca(posicao);
                        return "asphalt";
                    case "Walk":
                    case "Curb":
                    case "Drain":
                        return "sidewalk";
                    default:
                        if (terreno != null && tile.name is "Dirt" or "DirtRoad")
                            agua = terreno.CoberturaDePoca(posicao);
                        return "grass";
                }
            }

            string nome = tile.name.ToLowerInvariant();
            if (nome.Contains("grama") || nome.Contains("terra") || nome.Contains("areia")) return "grass";
            if (nome.Contains("madeira")) return "wood";
            if (exterior && nome.Contains("asfalto")) return "asphalt";
            if (exterior && (nome.Contains("calcada") || nome.Contains("calçada"))) return "sidewalk";
        }
        return fallback;
    }
}
