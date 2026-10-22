using UnityEngine;

public static class SantaRitaWorldLighting
{
    public static void Apply()
    {
        var material=Resources.Load<Material>("Art/WorldLit");if(material==null)return;
        foreach(var sprite in Object.FindObjectsByType<SpriteRenderer>()) sprite.sharedMaterial=material;
        var player=Object.FindAnyObjectByType<PlayerController>();
        if(player!=null&&player.GetComponent<SantaRitaRimLight>()==null)player.gameObject.AddComponent<SantaRitaRimLight>();
        if(player!=null && player.GetComponent<SantaRitaAtmosphere>()==null)player.gameObject.AddComponent<SantaRitaAtmosphere>();
    }
}
