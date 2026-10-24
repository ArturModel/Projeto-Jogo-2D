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
    private static string PrefKeyPrimary => TestPrefix + "SR_ManualSave_V2";
    private static string PrefKeyBackup => TestPrefix + "SR_ManualSave_V2_Bak";
    private static string PrefKeySlotPrefix => TestPrefix + "SR_ManualSave_V2_Slot_";
    private static string PrefKeyLastSlot => TestPrefix + "SR_ManualSave_V2_LastSlot";

    private const string KeyExistsLegacy = "checkpoint.exists";
    private const string KeySceneLegacy = "checkpoint.scene";
    private const string KeyXLegacy = "checkpoint.x";
    private const string KeyYLegacy = "checkpoint.y";

    private static bool pendingSpawn;
    private static int pendingSlotIndex = -1;

    private static string PrimaryFilePath => Path.Combine(StorageDirectory, "santa_rita_save_v2.json");
    private static string BackupFilePath => Path.Combine(StorageDirectory, "santa_rita_save_v2.bak");
    private static string SlotFilePath(int slot) => Path.Combine(StorageDirectory, $"santa_rita_save_v2_slot_{slot}.json");
    private static string StorageDirectory => testDirectory ?? Application.persistentDataPath;
    private static string TestPrefix => testDirectory == null ? "" : "SRQA_" + Path.GetFileName(testDirectory) + "_";
    private static string testDirectory;
    public static bool EmArmazenamentoDeTeste => testDirectory != null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // Os testes do save usam arquivo e namespace de preferências próprios.
    // Não apontar uma execução de QA para os slots pessoais do autor.
    public static IDisposable BeginIsolatedTests()
    {
        if (testDirectory != null) throw new InvalidOperationException("Teste de save já isolado.");
        var scope = new IsolatedSaveScope(GameState.ExportarSnapshot());
        testDirectory = Path.Combine(Path.GetTempPath(), "SantaRitaC2SaveTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testDirectory);
        ClearPendingOnly();
        GameState.ImportarSnapshot(new GameStateSnapshot());
        return scope;
    }

    private sealed class IsolatedSaveScope : IDisposable
    {
        private readonly GameStateSnapshot anterior;
        private readonly bool progressoPendente = GameState.TemProgressoNaoSalvo;
        private readonly bool spawnPendente = pendingSpawn;
        private readonly int slotPendente = pendingSlotIndex;
        public IsolatedSaveScope(GameStateSnapshot estado) { anterior = estado; }
        public void Dispose()
        {
            if (testDirectory == null) return;
            PlayerPrefs.DeleteKey(PrefKeyPrimary);
            PlayerPrefs.DeleteKey(PrefKeyBackup);
            PlayerPrefs.DeleteKey(PrefKeyLastSlot);
            for (int i = 0; i < TotalSlots; i++) PlayerPrefs.DeleteKey(PrefKeySlotPrefix + i);
            PlayerPrefs.Save(); // Persistir também a remoção do namespace temporário.
            string criadaPeloTeste = testDirectory;
            testDirectory = null;
            ClearPendingOnly();
            GameState.ImportarSnapshot(anterior);
            GameState.RestaurarIndicadorDeProgresso(progressoPendente);
            pendingSpawn = spawnPendente; pendingSlotIndex = slotPendente;
            if (Path.GetFileName(criadaPeloTeste).StartsWith("SantaRitaC2SaveTests_"))
                Directory.Delete(criadaPeloTeste, true);
        }
    }
#endif

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
            if (id.Contains("bocha") || id.Contains("beira_rio")) return "Sociedade Beira-Rio (Bocha)";
            if (id.Contains("centro") || id.Contains("matriz")) return "Centro — Praça da Matriz";
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

        // Somente o formato anterior aos slots pode ocupar o primeiro slot.
        // O arquivo geral moderno é a cópia do último slot usado, não outro slot.
        if (slotIndex == 0 && TryLoadPrimaryOnly(out data, legacyOnly: true))
            return true;

        return false;
    }

    // Consulta sem apagar ou regravar saves. As cópias de recuperação continuam
    // sendo tentadas pelo leitor; só descreve o problema se nenhuma for válida.
    public static string DescreverSlotIndisponivel(int slotIndex)
    {
        string path = SlotFilePath(slotIndex);
        string json = null;
        try { if (File.Exists(path)) json = File.ReadAllText(path); }
        catch { return "Arquivo sem acesso — preservado"; }
        if (json == null && PlayerPrefs.HasKey(PrefKeySlotPrefix + slotIndex))
            json = PlayerPrefs.GetString(PrefKeySlotPrefix + slotIndex);
        if (json == null) return "Slot vazio";
        try
        {
            var d = JsonUtility.FromJson<ManualSaveData>(json);
            if (d != null && d.versao != SaveVersion) return "Versão incompatível — arquivo preservado";
        }
        catch { }
        return "Save inválido — arquivo preservado";
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

    private static bool TryLoadPrimaryOnly(out ManualSaveData data, bool legacyOnly = false)
    {
        data = null;
        if (TryReadAndValidateFile(PrimaryFilePath, out data) && (!legacyOnly || data.slotIndex == -1))
            return true;
        if (TryReadAndValidateFile(BackupFilePath, out data) && (!legacyOnly || data.slotIndex == -1))
            return true;
        if (PlayerPrefs.HasKey(PrefKeyPrimary) && TryParseAndValidateJson(PlayerPrefs.GetString(PrefKeyPrimary), out data) && (!legacyOnly || data.slotIndex == -1))
            return true;
        if (PlayerPrefs.HasKey(PrefKeyBackup) && TryParseAndValidateJson(PlayerPrefs.GetString(PrefKeyBackup), out data) && (!legacyOnly || data.slotIndex == -1))
            return true;
        data = null;
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
            var parsed = new ManualSaveData { slotIndex = -1, versao = 0 };
            JsonUtility.FromJsonOverwrite(json, parsed);
            if (parsed.versao != SaveVersion) return false;
            if (parsed.slotIndex < -1 || parsed.slotIndex >= TotalSlots) return false;
            if (parsed.sceneName != "EntradaDaCidade" && parsed.sceneName != "CasaDoAvo" && parsed.sceneName != "BarDoZe" &&
                !Capitulo2Director.CenaDoCapitulo2(parsed.sceneName)) return false;
            if (float.IsNaN(parsed.posX) || float.IsInfinity(parsed.posX) ||
                float.IsNaN(parsed.posY) || float.IsInfinity(parsed.posY)) return false;
            if (parsed.estado == null) parsed.estado = new GameStateSnapshot();
            if (parsed.estado.versao > 2) return false;
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
