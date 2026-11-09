using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Close-up shared by the payphones in the world. Persistence stays in PayphoneCheckpoint.
public sealed class PayphoneJogoUI : MonoBehaviour
{
    private const string ArtPath = "UI/OrelhaoJogo/";
    private static readonly Color Amber = new Color(.95f, .76f, .44f);
    private static readonly Color Paper = new Color(.90f, .88f, .78f);
    private static readonly Color Steel = new Color(.95f, .95f, .91f);
    private static readonly Color LitKey = new Color(1f, .84f, .59f);
    private static readonly Vector2 StageSize = UIFactory.ReferenceResolution;

    private readonly ManualSaveData[] records = new ManualSaveData[CheckpointSystem.TotalSlots];
    private readonly Button[] keys = new Button[CheckpointSystem.TotalSlots];
    private RectTransform stage;
    private CanvasGroup group;
    private TextMeshProUGUI details, instruction, display;
    private Button save, load, confirm, confirmText;
    private RawImage hand;
    private MenuAmbienteGraphic rain;
    private Action<int, bool> onConfirm;
    private int selected = -1;
    private bool loading, waitingForScene, ownsCursor, oldCursorVisible;
    private CursorLockMode oldCursorLock;
    private float opening, rainTime;

    public static bool HasArt => Resources.Load<Texture2D>(ArtPath + "Rua") != null;
    public bool Busy => waitingForScene;

    public void Build(Action<int, bool> commit, Action hangUp)
    {
        onConfirm = commit;
        stage = new GameObject("CenaDoOrelhao", typeof(RectTransform), typeof(CanvasGroup), typeof(RawImage)).GetComponent<RectTransform>();
        stage.SetParent(transform, false);
        SantaRitaInventory.Place(stage, Vector2.zero, StageSize);
        group = stage.GetComponent<CanvasGroup>();
        var background = stage.GetComponent<RawImage>();
        background.texture = Resources.Load<Texture2D>(ArtPath + "Rua");
        background.raycastTarget = false;
        var weather = new GameObject("ChuvaNoOrelhao", typeof(RectTransform), typeof(CanvasGroup));
        weather.GetComponent<CanvasGroup>().alpha = .22f;
        rain = weather.AddComponent<MenuAmbienteGraphic>();
        rain.transform.SetParent(stage, false);
        rain.Tipo = MenuAmbienteGraphic.Camada.ChuvaOrelhao;
        rain.raycastTarget = false;
        UIFactory.Stretch(rain.rectTransform);

        var caption = Text("LinhaLocal", "O R E L H Ã O   /   L I N H A   L O C A L", 116, 118, 184, 20, 10, Amber);
        caption.alignment = TextAlignmentOptions.MidlineLeft;
        Box("Filete", 66, 131, 84, 1, new Color(.76f, .57f, .31f, .67f));
        details = Text("DetalhesRegistro", "", 116, 158, 184, 42, 15, Paper);
        details.alignment = TextAlignmentOptions.MidlineLeft;
        details.overflowMode = TextOverflowModes.Ellipsis;
        instruction = Text("Instrucao", "", 116, 192, 184, 24, 11, new Color(.77f, .79f, .74f));
        instruction.alignment = TextAlignmentOptions.MidlineLeft;
        var hangUpKey = Key("BtnDesligarFone", "Desligar  [Esc]", 64, 224, 80, 19, () => hangUp());
        hangUpKey.image.color = new Color(.92f, .89f, .81f, .56f);

        Box("BordaDoVisor", 503, 132, 55, 18, new Color(.052f, .053f, .039f));
        Box("VidroDoVisor", 503, 132, 51, 15, new Color(.052f, .104f, .061f));
        Box("ReflexoNoVisor", 503, 139, 50, 1, new Color(.33f, .43f, .24f, .46f));
        display = Text("Visor", "", 503, 132, 52, 16, 9, new Color(.68f, .83f, .50f));
        save = Key("ModoSalvar", "SALVAR", 489, 152, 26, 14, () => SetMode(false), 8.5f);
        load = Key("ModoCarregar", "CARGA", 517, 152, 26, 14, () => SetMode(true), 8.5f);
        for (int i = 0; i < keys.Length; i++)
        {
            int slot = i;
            keys[i] = Key("BtnSlot_" + (i + 1), (i + 1).ToString(), 485 + i % 3 * 18,
                173 + i / 3 * 19, 16, 17, () => SelectRecord(slot), 13);
        }
        confirm = Key("ConfirmarRegistro", "#", 521, 192, 16, 17, Commit, 12);
        confirmText = Key("ConfirmarTexto", "CONFIRMAR", 503, 211, 54, 14, Commit, 8.5f);

        var texture = Resources.Load<Texture2D>(ArtPath + "Mao");
        if (texture != null)
        {
            hand = new GameObject("MaoDeMarcus", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            hand.transform.SetParent(stage, false);
            hand.texture = texture;
            hand.raycastTarget = false;
            // Trim transparent margins in UV space. The tip is the click hotspot.
            hand.uvRect = new Rect(160f / 1568, 143f / 1003, 1275f / 1568, 706f / 1003);
            SantaRitaInventory.Place(hand.rectTransform, Vector2.zero, new Vector2(36, 20));
            hand.rectTransform.pivot = new Vector2(1, .67f);
            hand.enabled = false;
        }
    }

    public void Open()
    {
        opening = 0;
        group.alpha = 0;
        group.interactable = true;
        waitingForScene = false;
        for (int i = 0; i < records.Length; i++)
            CheckpointSystem.TryLoadSlot(i, out records[i]);
        SetMode(false);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void SetMode(bool loadMode)
    {
        loading = loadMode;
        selected = -1;
        Refresh();
    }

    private void Refresh()
    {
        save.image.color = loading ? Steel : LitKey;
        load.image.color = loading ? LitKey : Steel;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].interactable = !loading || records[i] != null;
            keys[i].image.color = i == selected ? LitKey : Steel;
            keys[i].GetComponentInChildren<TMP_Text>().color =
                !keys[i].interactable ? new Color(.53f, .54f, .46f) : i == selected ? Amber : Paper;
        }

        bool hasSelection = selected >= 0;
        confirm.interactable = confirmText.interactable = hasSelection;
        confirmText.GetComponentInChildren<TMP_Text>().color = hasSelection ? Amber : new Color(.52f, .51f, .46f);
        display.text = hasSelection ? "REG " + (selected + 1).ToString("00") : "LINHA";
        if (!hasSelection)
        {
            details.text = loading ? "Carregar partida" : "Salvar progresso";
            instruction.text = loading && GameState.TemProgressoNaoSalvo
                ? "A carga descarta o progresso não salvo." : "Selecione um registro de 1 a 5 no aparelho.";
            return;
        }

        var record = records[selected];
        if (record == null)
            details.text = $"Registro {selected + 1:00}  /  {CheckpointSystem.DescreverSlotIndisponivel(selected)}";
        else
        {
            string local = string.IsNullOrEmpty(record.localDescricao)
                ? CheckpointSystem.ObterNomeAmigavelOrelhao(record.payphoneId, record.sceneName)
                : record.localDescricao;
            details.text = $"{local}\n{record.salvoEm}";
        }
        instruction.text = loading
            ? (GameState.TemProgressoNaoSalvo ? "Confirmar descarta o progresso não salvo." : "Confirme para carregar este registro.")
            : (record == null ? "Confirme com # ou Enter para gravar." : "Confirme para substituir este registro.");
    }

    public void SelectRecord(int slot)
    {
        if (Busy || slot < 0 || slot >= keys.Length || !keys[slot].interactable) return;
        if (slot != selected) PayphoneCheckpoint.TocarTeclaRegistro(slot);
        selected = slot;
        Refresh();
        EventSystem.current?.SetSelectedGameObject(confirm.gameObject);
    }

    private void Commit()
    {
        if (Busy || selected < 0 || SceneFader.IsTransitioning) return;
        onConfirm(selected, loading);
    }

    public void AwaitScene()
    {
        waitingForScene = true;
        group.interactable = false;
        display.text = "CHAMANDO";
        instruction.text = "Reconectando ao registro…";
    }

    public void LoadFailed()
    {
        waitingForScene = false;
        group.interactable = true;
        for (int i = 0; i < records.Length; i++)
            CheckpointSystem.TryLoadSlot(i, out records[i]);
        SetMode(true);
        instruction.text = "Registro indisponível. Tente outro.";
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        RestoreCursor();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;
        if (waitingForScene) gameObject.SetActive(false);
        else PayphoneCheckpoint.FecharModalSlots();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) RestoreCursor();
    }

    private void LateUpdate()
    {
        var area = ((RectTransform)transform).rect.size;
        stage.localScale = Vector3.one * Mathf.Min(area.x / StageSize.x, area.y / StageSize.y);
        opening = Mathf.MoveTowards(opening, 1, Time.unscaledDeltaTime / .19f);
        group.alpha = Mathf.SmoothStep(0, 1, opening);
        rainTime += Time.unscaledDeltaTime;
        rain.Tempo = rainTime;
        rain.Brilho = MenuPausa.ReduzirEfeitosIntensos ? .72f : 1.38f;
        rain.SetVerticesDirty();

        if (hand == null) return;
        if (Mouse.current == null || !Application.isFocused)
        {
            RestoreCursor();
            return;
        }
        if (!ownsCursor)
        {
            oldCursorVisible = Cursor.visible;
            oldCursorLock = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            ownsCursor = true;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, Mouse.current.position.ReadValue(), null, out var point);
        bool inside = stage.rect.Contains(point) && !Busy;
        hand.enabled = inside;
        Cursor.visible = !inside;
        hand.rectTransform.anchoredPosition = point;
        hand.rectTransform.localScale = Mouse.current.leftButton.isPressed ? Vector3.one * .88f : Vector3.one;
    }

    private void RestoreCursor()
    {
        if (hand != null) hand.enabled = false;
        if (!ownsCursor) return;
        Cursor.lockState = oldCursorLock;
        Cursor.visible = oldCursorVisible;
        ownsCursor = false;
    }

    private Image Box(string name, float x, float y, float w, float h, Color color)
    {
        var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        image.transform.SetParent(stage, false);
        SantaRitaInventory.Place(image.rectTransform, new Vector2(x - 320, 180 - y), new Vector2(w, h));
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI Text(string name, string text, float x, float y, float w, float h, float size, Color color)
    {
        var label = SantaRitaInventory.Label(stage, name, new Vector2(x - 320, 180 - y), new Vector2(w, h), size);
        label.text = text;
        label.color = color;
        return label;
    }

    private Button Key(string name, string title, float x, float y, float w, float h, UnityEngine.Events.UnityAction click, float fontSize = 11)
    {
        var face = Box(name, x, y, w, h, Steel);
        UIFactory.ApplyWeatheredSurface(face, 1);
        face.raycastTarget = true;
        var button = face.gameObject.AddComponent<Button>();
        button.targetGraphic = face;
        var colors = button.colors;
        colors.highlightedColor = colors.selectedColor = new Color(1, .90f, .63f);
        colors.pressedColor = new Color(.65f, .52f, .29f);
        colors.disabledColor = new Color(.47f, .48f, .48f);
        colors.colorMultiplier = 1.35f;
        colors.fadeDuration = .08f;
        button.colors = colors;
        var label = SantaRitaInventory.Label(face.transform, "Legenda", Vector2.zero, new Vector2(w, h), fontSize);
        label.text = title;
        label.color = Paper;
        button.onClick.AddListener(click);
        return button;
    }
}
