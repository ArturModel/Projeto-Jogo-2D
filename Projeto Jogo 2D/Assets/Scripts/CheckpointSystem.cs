using UnityEngine;

// Guarda o último orelhão usado. É o "save" do jogo: cena + posição.
public static class CheckpointSystem
{
    private const string KeyExists = "checkpoint.exists";
    private const string KeyScene = "checkpoint.scene";
    private const string KeyX = "checkpoint.x";
    private const string KeyY = "checkpoint.y";

    // Marcado ao clicar em "Continuar": faz o player nascer no checkpoint
    // em vez da posição padrão da cena.
    private static bool pendingSpawn;

    public static bool HasCheckpoint => PlayerPrefs.GetInt(KeyExists, 0) == 1;

    public static string CheckpointScene => PlayerPrefs.GetString(KeyScene, string.Empty);

    public static void Save(string sceneName, Vector2 position)
    {
        PlayerPrefs.SetInt(KeyExists, 1);
        PlayerPrefs.SetString(KeyScene, sceneName);
        PlayerPrefs.SetFloat(KeyX, position.x);
        PlayerPrefs.SetFloat(KeyY, position.y);
        PlayerPrefs.Save();
    }

    public static void RequestSpawnAtCheckpoint()
    {
        pendingSpawn = HasCheckpoint;
    }

    // Só devolve a posição uma vez: quem consumir, nasce lá.
    public static bool ConsumePendingSpawn(out Vector2 position)
    {
        position = Vector2.zero;

        if (!pendingSpawn || !HasCheckpoint)
        {
            return false;
        }

        pendingSpawn = false;
        position = new Vector2(
            PlayerPrefs.GetFloat(KeyX, 0f),
            PlayerPrefs.GetFloat(KeyY, 0f)
        );

        return true;
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(KeyExists);
        PlayerPrefs.DeleteKey(KeyScene);
        PlayerPrefs.DeleteKey(KeyX);
        PlayerPrefs.DeleteKey(KeyY);
        PlayerPrefs.Save();
    }
}
