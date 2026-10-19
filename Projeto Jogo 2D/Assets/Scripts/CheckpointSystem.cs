using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ManualSaveData
{
    public int versao = 2;
    public string sceneName;
    public string payphoneId;
    public float posX;
    public float posY;
    public string salvoEm;
    public GameStateSnapshot estado;
}

// Guarda o último orelhão usado manualmente (Regra R1).
// É o único ponto de persistência de progresso do jogo: cena + posição +
// ID do orelhão + snapshot completo do GameState (flags, pistas, diário, ficha).
// Possui validação de versão e recuperação automática contra arquivo corrompido.
public static class CheckpointSystem
{
    private const int SaveVersion = 2;
    private const string PrefKeyPrimary = "SR_ManualSave_V2";
    private const string PrefKeyBackup = "SR_ManualSave_V2_Bak";

    private const string KeyExistsLegacy = "checkpoint.exists";
    private const string KeySceneLegacy = "checkpoint.scene";
    private const string KeyXLegacy = "checkpoint.x";
    private const string KeyYLegacy = "checkpoint.y";

    private static bool pendingSpawn;

    private static string PrimaryFilePath => Path.Combine(Application.persistentDataPath, "santa_rita_save_v2.json");
    private static string BackupFilePath => Path.Combine(Application.persistentDataPath, "santa_rita_save_v2.bak");

    public static bool HasCheckpoint => TryLoadManualSave(out _);

    public static string CheckpointScene
    {
        get
        {
            if (TryLoadManualSave(out ManualSaveData data))
                return data.sceneName;
            return string.Empty;
        }
    }

    public static void Save(string sceneName, Vector2 position)
    {
        Save(sceneName, position, "orelhao_padrao");
    }

    public static void Save(string sceneName, Vector2 position, string payphoneId)
    {
        var data = new ManualSaveData
        {
            versao = SaveVersion,
            sceneName = sceneName,
            payphoneId = string.IsNullOrEmpty(payphoneId) ? "orelhao_padrao" : payphoneId,
            posX = position.x,
            posY = position.y,
            salvoEm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            estado = GameState.ExportarSnapshot()
        };

        string json = JsonUtility.ToJson(data, true);

        // Escrita segura em disco com backup técnico para recuperação de corrupção
        try
        {
            if (File.Exists(PrimaryFilePath))
                File.Copy(PrimaryFilePath, BackupFilePath, true);

            string tempPath = PrimaryFilePath + ".tmp";
            File.WriteAllText(tempPath, json);
            if (File.Exists(PrimaryFilePath))
                File.Delete(PrimaryFilePath);
            File.Move(tempPath, PrimaryFilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[CheckpointSystem] Aviso ao gravar arquivo de save manual: " + e.Message);
        }

        if (PlayerPrefs.HasKey(PrefKeyPrimary))
            PlayerPrefs.SetString(PrefKeyBackup, PlayerPrefs.GetString(PrefKeyPrimary));

        PlayerPrefs.SetString(PrefKeyPrimary, json);
        PlayerPrefs.Save();

        GameState.ConfirmarSaveManual();
    }

    public static void RequestSpawnAtCheckpoint()
    {
        if (TryLoadManualSave(out ManualSaveData data))
        {
            GameState.ImportarSnapshot(data.estado);
            pendingSpawn = true;
        }
        else
        {
            pendingSpawn = false;
        }
    }

    // Só devolve a posição uma vez: quem consumir, nasce lá com o estado restaurado.
    public static bool ConsumePendingSpawn(out Vector2 position)
    {
        position = Vector2.zero;

        if (!pendingSpawn || !TryLoadManualSave(out ManualSaveData data))
        {
            return false;
        }

        pendingSpawn = false;
        GameState.ImportarSnapshot(data.estado);
        position = new Vector2(data.posX, data.posY);
        return true;
    }

    public static bool RecarregarUltimoSaveManual()
    {
        if (!TryLoadManualSave(out ManualSaveData data))
            return false;

        GameState.ImportarSnapshot(data.estado);
        pendingSpawn = true;
        SceneFader.Instance.LoadScene(data.sceneName);
        return true;
    }

    public static bool TryLoadManualSave(out ManualSaveData data)
    {
        data = null;

        if (TryReadAndValidateFile(PrimaryFilePath, out data))
            return true;

        if (TryReadAndValidateFile(BackupFilePath, out data))
            return true;

        if (PlayerPrefs.HasKey(PrefKeyPrimary) && TryParseAndValidateJson(PlayerPrefs.GetString(PrefKeyPrimary), out data))
            return true;

        if (PlayerPrefs.HasKey(PrefKeyBackup) && TryParseAndValidateJson(PlayerPrefs.GetString(PrefKeyBackup), out data))
            return true;

        return false;
    }

    private static bool TryReadAndValidateFile(string path, out ManualSaveData data)
    {
        data = null;
        try
        {
            if (!File.Exists(path)) return false;
            return TryParseAndValidateJson(File.ReadAllText(path), out data);
        }
        catch
        {
            return false;
        }
    }

    private static bool TryParseAndValidateJson(string json, out ManualSaveData data)
    {
        data = null;
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            var parsed = JsonUtility.FromJson<ManualSaveData>(json);
            if (parsed == null) return false;
            if (parsed.versao != SaveVersion) return false;
            if (string.IsNullOrEmpty(parsed.sceneName)) return false;
            if (parsed.estado == null) parsed.estado = new GameStateSnapshot();
            data = parsed;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void Clear()
    {
        pendingSpawn = false;

        try
        {
            if (File.Exists(PrimaryFilePath)) File.Delete(PrimaryFilePath);
            if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath);
        }
        catch { }

        PlayerPrefs.DeleteKey(PrefKeyPrimary);
        PlayerPrefs.DeleteKey(PrefKeyBackup);
        PlayerPrefs.DeleteKey(KeyExistsLegacy);
        PlayerPrefs.DeleteKey(KeySceneLegacy);
        PlayerPrefs.DeleteKey(KeyXLegacy);
        PlayerPrefs.DeleteKey(KeyYLegacy);
        PlayerPrefs.Save();
    }
}
