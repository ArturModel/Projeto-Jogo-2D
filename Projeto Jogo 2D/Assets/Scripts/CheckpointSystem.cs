using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ManualSaveData
{
    public int versao = 2;
    public int slotIndex = 0;
    public string sceneName;
    public string payphoneId;
    public string localDescricao;
    public float posX;
    public float posY;
    public string salvoEm;
    public GameStateSnapshot estado;
}

// Guarda os saves manuais feitos nos Orelhões (Regra R1), com suporte a 5 Slots independentes.
// Grava cena + posição + ID do orelhão + nome do local + snapshot completo do GameState.
public static class CheckpointSystem
{
    public const int TotalSlots = 5;
    private const int SaveVersion = 2;
    private const string PrefKeyPrimary = "SR_ManualSave_V2";
    private const string PrefKeyBackup = "SR_ManualSave_V2_Bak";
    private const string PrefKeySlotPrefix = "SR_ManualSave_V2_Slot_";
    private const string PrefKeyLastSlot = "SR_ManualSave_V2_LastSlot";

    private const string KeyExistsLegacy = "checkpoint.exists";
    private const string KeySceneLegacy = "checkpoint.scene";
    private const string KeyXLegacy = "checkpoint.x";
    private const string KeyYLegacy = "checkpoint.y";

    private static bool pendingSpawn;
    private static int pendingSlotIndex = -1;

    private static string PrimaryFilePath => Path.Combine(Application.persistentDataPath, "santa_rita_save_v2.json");
    private static string BackupFilePath => Path.Combine(Application.persistentDataPath, "santa_rita_save_v2.bak");
    private static string SlotFilePath(int slot) => Path.Combine(Application.persistentDataPath, $"santa_rita_save_v2_slot_{slot}.json");

    public static bool HasCheckpoint
    {
        get
        {
            if (TryLoadManualSave(out _)) return true;
            for (int i = 0; i < TotalSlots; i++)
            {
                if (TryLoadSlot(i, out _)) return true;
            }
            return false;
        }
    }

    public static int UltimoSlotUsado => Mathf.Clamp(PlayerPrefs.GetInt(PrefKeyLastSlot, 0), 0, TotalSlots - 1);

    public static string CheckpointScene
    {
        get
        {
            if (TryLoadManualSave(out ManualSaveData data))
                return data.sceneName;
            return string.Empty;
        }
    }

    public static string ObterNomeAmigavelOrelhao(string payphoneId, string sceneName)
    {
        if (!string.IsNullOrEmpty(payphoneId))
        {
            string id = payphoneId.ToLowerInvariant();
            if (id.Contains("rodovia") || id.Contains("entrada")) return "Rodovia de Entrada";
            if (id.Contains("posto")) return "Avenida Sul (Posto)";
            if (id.Contains("encruzilhada") || id.Contains("mercado")) return "Encruzilhada Central";
            if (id.Contains("bairro") || id.Contains("avo") || id.Contains("adilio")) return "Bairro das Casas";
            if (id.Contains("bar") || id.Contains("norte") || id.Contains("ze")) return "Rua Norte (Bar do Zé)";
        }
        if (sceneName == "CasaDoAvo") return "Casa do Avô Adílio";
        if (sceneName == "BarDoZe") return "Bar do Zé";
        return "Santa Rita (Orelhão)";
    }

    public static void Save(string sceneName, Vector2 position)
    {
        Save(sceneName, position, "orelhao_padrao");
    }

    public static void Save(string sceneName, Vector2 position, string payphoneId)
    {
        SaveSlot(UltimoSlotUsado, sceneName, position, payphoneId);
    }

    public static void SaveSlot(int slotIndex, string sceneName, Vector2 position, string payphoneId, string localExplicito = null)
    {
        slotIndex = Mathf.Clamp(slotIndex, 0, TotalSlots - 1);
        string idFinal = string.IsNullOrEmpty(payphoneId) ? "orelhao_padrao" : payphoneId;
        string localFinal = !string.IsNullOrEmpty(localExplicito)
            ? localExplicito
            : ObterNomeAmigavelOrelhao(idFinal, sceneName);

        var data = new ManualSaveData
        {
            versao = SaveVersion,
            slotIndex = slotIndex,
            sceneName = sceneName,
            payphoneId = idFinal,
            localDescricao = localFinal,
            posX = position.x,
            posY = position.y,
            salvoEm = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            estado = GameState.ExportarSnapshot()
        };

        string json = JsonUtility.ToJson(data, true);

        // Grava no arquivo específico do Slot (0..4)
        try
        {
            string slotPath = SlotFilePath(slotIndex);
            string tempSlot = slotPath + ".tmp";
            File.WriteAllText(tempSlot, json);
            if (File.Exists(slotPath))
                File.Delete(slotPath);
            File.Move(tempSlot, slotPath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[CheckpointSystem] Aviso ao gravar arquivo do slot " + slotIndex + ": " + e.Message);
        }

        // Também atualiza o último save geral com backup
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

        PlayerPrefs.SetString(PrefKeySlotPrefix + slotIndex, json);
        PlayerPrefs.SetString(PrefKeyPrimary, json);
        PlayerPrefs.SetInt(PrefKeyLastSlot, slotIndex);
        PlayerPrefs.Save();

        GameState.ConfirmarSaveManual();
    }

    public static bool TryLoadSlot(int slotIndex, out ManualSaveData data)
    {
        data = null;
        if (slotIndex < 0 || slotIndex >= TotalSlots) return false;

        string slotPath = SlotFilePath(slotIndex);
        if (TryReadAndValidateFile(slotPath, out data))
            return true;

        string prefKey = PrefKeySlotPrefix + slotIndex;
        if (PlayerPrefs.HasKey(prefKey) && TryParseAndValidateJson(PlayerPrefs.GetString(prefKey), out data))
            return true;

        // Compatibilidade: se for o slot 0 e existir um save antigo sem slot separado, espelha no Slot 1
        if (slotIndex == 0 && TryLoadPrimaryOnly(out data))
            return true;

        return false;
    }

    public static void RequestSpawnAtSlot(int slotIndex)
    {
        if (TryLoadSlot(slotIndex, out ManualSaveData data))
        {
            GameState.ImportarSnapshot(data.estado);
            pendingSpawn = true;
            pendingSlotIndex = slotIndex;
            PlayerPrefs.SetInt(PrefKeyLastSlot, slotIndex);
        }
    }

    public static void RequestSpawnAtCheckpoint()
    {
        if (TryLoadManualSave(out ManualSaveData data))
        {
            GameState.ImportarSnapshot(data.estado);
            pendingSpawn = true;
            pendingSlotIndex = -1;
        }
        else
        {
            pendingSpawn = false;
            pendingSlotIndex = -1;
        }
    }

    // Só devolve a posição uma vez: quem consumir, nasce lá com o estado restaurado.
    public static bool ConsumePendingSpawn(out Vector2 position)
    {
        position = Vector2.zero;

        if (!pendingSpawn)
            return false;

        ManualSaveData data = null;
        bool ok = (pendingSlotIndex >= 0 && TryLoadSlot(pendingSlotIndex, out data)) ||
                  TryLoadManualSave(out data);

        pendingSpawn = false;
        pendingSlotIndex = -1;

        if (!ok || data == null)
            return false;

        GameState.ImportarSnapshot(data.estado);
        position = new Vector2(data.posX, data.posY);
        return true;
    }

    public static bool CarregarSlot(int slotIndex)
    {
        if (!TryLoadSlot(slotIndex, out ManualSaveData data))
            return false;

        GameState.ImportarSnapshot(data.estado);
        pendingSpawn = true;
        pendingSlotIndex = slotIndex;
        PlayerPrefs.SetInt(PrefKeyLastSlot, slotIndex);
        SceneFader.Instance.LoadScene(data.sceneName);
        return true;
    }

    public static bool RecarregarUltimoSaveManual()
    {
        if (!TryLoadManualSave(out ManualSaveData data))
            return false;

        GameState.ImportarSnapshot(data.estado);
        pendingSpawn = true;
        pendingSlotIndex = -1;
        SceneFader.Instance.LoadScene(data.sceneName);
        return true;
    }

    private static bool TryLoadPrimaryOnly(out ManualSaveData data)
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

    public static bool TryLoadManualSave(out ManualSaveData data)
    {
        if (TryLoadPrimaryOnly(out data))
            return true;

        for (int i = 0; i < TotalSlots; i++)
        {
            if (TryLoadSlot(i, out data))
                return true;
        }

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
            if (string.IsNullOrEmpty(parsed.localDescricao))
                parsed.localDescricao = ObterNomeAmigavelOrelhao(parsed.payphoneId, parsed.sceneName);
            data = parsed;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void ClearPendingOnly()
    {
        pendingSpawn = false;
        pendingSlotIndex = -1;
    }

    public static void Clear()
    {
        pendingSpawn = false;
        pendingSlotIndex = -1;
    }
}
