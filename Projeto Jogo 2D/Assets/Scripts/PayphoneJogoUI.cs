using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Painel frontal diegético de orelhão brasileiro clássico (aço inox, visor LCD verde-fósforo,
// cartão telefônico indutivo e teclas metálicas de discagem).
[DefaultExecutionOrder(100)]
public sealed class PayphoneJogoUI : MonoBehaviour
{
    private static readonly Color Amber = new Color(.96f, .82f, .44f);
    private static readonly Color Paper = new Color(.92f, .90f, .82f);
    private static readonly Color SteelNormal = new Color32(121, 112, 95, 255);
    private static readonly Color SteelLit = new Color32(154, 91, 45, 255);
    private static readonly Color SteelDisabled = new Color32(84, 78, 66, 255);
    private static Sprite paintedShell, beveledFace;
    private static readonly Color LcdStatusCor = new Color(.60f, .90f, .55f);
    private static readonly Color LcdTituloCor = new Color(.78f, 1.0f, .65f);
    private static readonly Color LcdDetalheCor = new Color(.62f, .88f, .58f);

    private static readonly Vector2 Tamanho = new Vector2(298f, 268f);

    private readonly ManualSaveData[] records = new ManualSaveData[CheckpointSystem.TotalSlots];
    private readonly Button[] keys = new Button[CheckpointSystem.TotalSlots];
    private readonly TMP_Text[] keyLabels = new TMP_Text[CheckpointSystem.TotalSlots];
    private readonly TMP_Text[] keySlots = new TMP_Text[CheckpointSystem.TotalSlots];

    private RectTransform stage;
    private CanvasGroup group;
    private TextMeshProUGUI displayStatus, display, details, instruction;
    private Button save, load, confirm, hook;
    private RectTransform handset;
    private TMP_Text hookLabel;
    private Action onHangUp;
    private bool offHook;
    private TMP_Text saveLabel, loadLabel;
    private Action<int, bool> onConfirm;
    private Action afterClose;
    private Camera vista;
    private Bounds aparelho;
    private int selected;
    private bool loading, waitingForScene, closing;
    private float opening;

    public bool Busy => closing || waitingForScene;
    internal bool Ready => !Busy && opening >= .70f;

    public void Build(Action<int, bool> commit, Action hangUp)
    {
        onConfirm = commit;
        onHangUp = hangUp;
        var background = UIFactory.CreateBox(transform, "GabineteDoOrelhao", null);
        background.sprite = CriarSuperficie(ref paintedShell, true);
        background.color = Color.white;
        stage = background.rectTransform;
        SantaRitaInventory.Place(stage, Vector2.zero, Tamanho);
        group = stage.gameObject.AddComponent<CanvasGroup>();

        CriarPainel("PlacaDeAco", 86, 27, 200, 192, new Color32(104, 97, 82, 255));
        CriarPainel("PlacaDeIdentificacao", 92, 8, 189, 19, new Color32(70, 41, 28, 255));
        CriarPainel("PlacaDeInstrucoes", 88, 221, 194, 37, new Color32(65, 40, 28, 255));
        CriarParafuso("ParafusoTL", 7, 7);
        CriarParafuso("ParafusoTR", 283, 7);
        CriarParafuso("ParafusoBL", 7, 253);
        CriarParafuso("ParafusoBR", 283, 253);
        CriarRotulo("Marca", "TELEFONIA  /  SANTA RITA", 96, 12, 181, 14, 11f, Amber, TextAlignmentOptions.Left, true);

        CriarPainel("Berco", 17, 30, 63, 194, new Color32(77, 45, 31, 255));
        CriarPainel("GanchoSuperior", 23, 39, 51, 26, new Color32(136, 113, 86, 255));
        CriarPainel("GanchoInferior", 23, 181, 51, 26, new Color32(136, 113, 86, 255));
        CriarPainel("Cabo", 44, 202, 9, 35, new Color32(62, 53, 43, 255));
        for (int i = 0; i < 8; i++)
            CriarPainel("Elo_" + i, 44, 205 + i * 4, 9, 1, new Color32(141, 118, 86, 255));

        handset = CriarPainel("Monofone", 33, 51, 31, 150, new Color32(79, 73, 63, 255)).rectTransform;
        foreach (float y in new[] { 35f, -35f })
        {
            var cap = UIFactory.CreateBox(handset, y > 0 ? "Auricular" : "Microfone", null);
            cap.sprite = CriarSuperficie(ref beveledFace, false);
            cap.color = new Color32(94, 86, 73, 255);
            cap.raycastTarget = false;
            SantaRitaInventory.Place(cap.rectTransform, new Vector2(0, y * 1.55f), new Vector2(45, 37));
            for (int i = -1; i <= 1; i++)
            {
                var slit = UIFactory.CreateBox(cap.transform, "Ranhura", null);
                slit.color = new Color32(41, 34, 28, 255);
                slit.raycastTarget = false;
                SantaRitaInventory.Place(slit.rectTransform, new Vector2(0, i * 4), new Vector2(19, 2));
            }
        }
        hook = Key("Gancho", "", 17, 30, 63, 194, AcionarGancho, 10f);
        hook.image.color = Color.clear;
        hook.transition = Selectable.Transition.None;
        CriarPainel("PlacaDoGancho", 12, 237, 74, 19, new Color32(74, 41, 26, 255));
        hookLabel = CriarRotulo("AcaoGancho", "", 8, 235, 82, 22, 9f, Amber, TextAlignmentOptions.Center, true);

        CriarPainel("MolduraLCD", 91, 30, 190, 60, new Color32(48, 49, 39, 255));
        CriarPainel("VidroLCD", 95, 34, 182, 52, new Color32(35, 62, 37, 255));
        displayStatus = CriarRotulo("StatusLinha", "", 101, 37, 170, 11, 8.5f, LcdStatusCor, TextAlignmentOptions.Left);
        display = CriarRotulo("VisorRegistro", "", 101, 49, 170, 17, 11.5f, LcdTituloCor, TextAlignmentOptions.Left, true);
        details = CriarRotulo("DetalhesSlot", "", 101, 66, 170, 16, 8.5f, LcdDetalheCor, TextAlignmentOptions.Left);
        display.overflowMode = details.overflowMode = TextOverflowModes.Ellipsis;

        CriarPainel("FendaDoCartao", 118, 97, 136, 6, new Color(.07f, .065f, .053f));
        CriarPainel("BordaDaFenda", 118, 103, 136, 1, new Color(.55f, .47f, .33f));
        save = KeyModo("ModoSalvar", "GRAVAR", 97, 112, 85, 19, () => SetMode(false), out saveLabel);
        load = KeyModo("ModoCarregar", "CARREGAR", 190, 112, 85, 19, () => SetMode(true), out loadLabel);

        CriarPainel("BercoDoTeclado", 106, 136, 160, 80, new Color32(54, 48, 39, 255));
        for (int i = 0; i < keys.Length; i++)
        {
            int slot = i;
            keys[i] = KeyTeclado("BtnSlot_" + (i + 1), (i + 1).ToString(),
                111 + i % 3 * 51, 141 + i / 3 * 36, 47, 32, () => SelectRecord(slot),
                out keyLabels[i], out keySlots[i]);
        }
        confirm = Key("ConfirmarRegistro", "#", 213, 177, 47, 32, Commit, 16f);
        instruction = CriarRotulo("InstrucaoRodape", "", 94, 223, 181, 34, 9.5f, Paper,
            TextAlignmentOptions.MidlineLeft);
    }

    internal void Open(Camera camera, Bounds foco)
    {
        vista = camera;
        aparelho = foco;
        opening = 0f;
        group.alpha = 0f;
        group.interactable = group.blocksRaycasts = false;
        waitingForScene = closing = offHook = false;
        handset.localRotation = Quaternion.identity;
        handset.anchoredPosition = new Vector2(-100.5f, 8f);
        afterClose = null;
        for (int i = 0; i < records.Length; i++)
            CheckpointSystem.TryLoadSlot(i, out records[i]);
        SetMode(false);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    internal bool Enquadramento2D(Camera camera, out Bounds foco, out float intensidade)
    {
        foco = aparelho;
        intensidade = SantaRitaInspecao.CurvaDaCamera(opening);
        return isActiveAndEnabled && vista == camera && camera.orthographic;
    }

    private void SetMode(bool loadMode)
    {
        loading = loadMode;
        selected = -1;
        Refresh();
    }

    private void Refresh()
    {
        save.interactable = load.interactable = offHook;
        save.image.color = offHook && !loading ? SteelLit : SteelNormal;
        load.image.color = offHook && loading ? SteelLit : SteelNormal;
        saveLabel.color = offHook && !loading ? Amber : Paper;
        loadLabel.color = offHook && loading ? Amber : Paper;

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].interactable = offHook && (!loading || records[i] != null);
            keys[i].image.color = i == selected ? SteelLit : keys[i].interactable ? SteelNormal : SteelDisabled;
            keyLabels[i].color = keys[i].interactable ? Paper : new Color32(221, 208, 177, 255);
            keySlots[i].text = records[i] != null ? "●" : "○";
            keySlots[i].color = records[i] != null ? LcdStatusCor : Paper;
        }

        confirm.interactable = offHook && loading && selected >= 0;
        confirm.image.color = confirm.interactable ? SteelLit : SteelDisabled;
        hookLabel.text = !offHook ? "PEGAR  [E]" : !loading && selected >= 0 ? "GRAVAR  [E]" : "REPOR  [E]";
        displayStatus.text = !offHook ? "●  NO GANCHO" : loading ? "●  LINHA / CARGA" : "●  LINHA / GRAVAÇÃO";
        if (!offHook)
        {
            display.text = "AGUARDANDO";
            details.text = "";
            instruction.text = "Pegue o fone para usar a linha.\n[Esc] Sair";
            return;
        }

        if (selected < 0)
        {
            display.text = "DISQUE UM REGISTRO";
            details.text = loading ? "Escolha um número gravado." : "Escolha um número de 1 a 5.";
            instruction.text = "[1–5] Discar   [Esc] Cancelar";
            return;
        }

        var record = records[selected];
        display.text = $"REGISTRO {selected + 1:00}  /  " + (record == null ? "LIVRE" : "GRAVADO");
        details.text = record == null ? "" : string.IsNullOrEmpty(record.localDescricao)
            ? CheckpointSystem.ObterNomeAmigavelOrelhao(record.payphoneId, record.sceneName)
            : record.localDescricao;
        instruction.text = loading
            ? GameState.TemProgressoNaoSalvo ? "[# / Enter] Carregar\nDescarta o progresso não salvo." : "[# / Enter] Carregar   [Esc] Cancelar"
            : record == null ? "Reponha o fone [E] para gravar.\n[Esc] Cancelar"
                : "Reponha o fone [E] para substituir.\n[Esc] Cancelar";
    }

    internal void AcionarGancho()
    {
        if (!Ready || SceneFader.IsTransitioning) return;
        if (!offHook)
        {
            offHook = true;
            PayphoneCheckpoint.TocarFoneRetirado();
            Refresh();
            EventSystem.current?.SetSelectedGameObject(null);
            return;
        }

        if (!loading && selected >= 0) onConfirm(selected, false);
        else onHangUp();
    }

    internal bool FoneForaDoGancho => offHook;

    public void SelectRecord(int slot)
    {
        if (!Ready || slot < 0 || slot >= keys.Length || !keys[slot].interactable) return;
        if (slot != selected) PayphoneCheckpoint.TocarTeclaRegistro(slot);
        selected = slot;
        Refresh();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    internal void Commit()
    {
        if (!Ready || !offHook || !loading || selected < 0 || SceneFader.IsTransitioning) return;
        onConfirm(selected, true);
    }

    internal void Close(Action completed)
    {
        if (Busy) return;
        closing = true;
        offHook = false;
        afterClose = completed;
        group.interactable = group.blocksRaycasts = false;
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void AwaitScene()
    {
        waitingForScene = true;
        group.interactable = group.blocksRaycasts = false;
        display.text = "CHAMANDO...";
        details.text = "Linha interurbana conectando...";
        instruction.text = "Reconectando ao registro gravado…";
    }

    public void LoadFailed()
    {
        waitingForScene = false;
        for (int i = 0; i < records.Length; i++)
            CheckpointSystem.TryLoadSlot(i, out records[i]);
        SetMode(true);
        instruction.text = "Registro indisponível. Tente outro número.";
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        afterClose = null;
        vista = null;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single) PayphoneCheckpoint.FecharModalSlots();
    }

    private void Update()
    {
        opening = SantaRitaInspecao.MoverProgresso(opening, closing, Time.deltaTime);
        if (!closing || opening > 0f) return;
        var completed = afterClose;
        afterClose = null;
        completed?.Invoke();
    }

    private void LateUpdate()
    {
        Vector2 phoneTarget = new Vector2(offHook ? -158f : -100.5f, offHook ? 15f : 8f);
        float movement = 1f - Mathf.Exp(-20f * Time.unscaledDeltaTime);
        handset.anchoredPosition = Vector2.Lerp(handset.anchoredPosition, phoneTarget, movement);
        handset.localRotation = Quaternion.Slerp(handset.localRotation,
            Quaternion.Euler(0, 0, offHook ? -18f : 0f), movement);

        Vector2 area = ((RectTransform)transform).rect.size;
        float scale = Mathf.Min(1f, area.x * .49f / Tamanho.x, area.y * .86f / Tamanho.y);
        stage.localScale = Vector3.one * scale;

        Vector3 pos = vista != null ? vista.WorldToViewportPoint(aparelho.center) : new Vector3(.33f, .5f);
        float x = area.x * (Mathf.Clamp(pos.x + .09f, .40f, .44f) - .5f);
        float halfHeight = (area.y - Tamanho.y * scale) * .5f - 8f;
        float y = Mathf.Clamp((pos.y - .5f) * area.y, -halfHeight, halfHeight);

        float opacity = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.20f, .85f, opening));
        stage.anchoredPosition = new Vector2(x + Tamanho.x * scale * .5f + (1f - opacity) * 7f, y);
        group.alpha = opacity;
        group.interactable = group.blocksRaycasts = Ready;
    }

    // A superfície neutra recebe a cor do controle. O casco usa o laranja da cúpula.
    private static Sprite CriarSuperficie(ref Sprite cache, bool casco)
    {
        if (cache != null) return cache;

        int largura = casco ? 96 : 20;
        int altura = casco ? 80 : 20;
        var textura = new Texture2D(largura, altura, TextureFormat.RGBA32, false)
        {
            name = casco ? "Orelhao_FibraLaranja" : "Orelhao_Relevo",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        var pixels = new Color32[largura * altura];
        for (int y = 0; y < altura; y++)
        {
            for (int x = 0; x < largura; x++)
            {
                int margemX = Mathf.Min(x, largura - 1 - x);
                int margemY = Mathf.Min(y, altura - 1 - y);
                int chanfro = casco ? 4 : 2;
                if (margemX + margemY < chanfro) continue;

                float relevo = 1f;
                if (margemX == 0 || margemY == 0 || margemX + margemY == chanfro)
                    relevo = .37f;
                else if (y == altura - 2 || x == 1)
                    relevo = casco ? 1.21f : 1f;
                else if (y <= 2 || x >= largura - 3)
                    relevo = .65f;

                if (casco)
                {
                    float desgaste = Mathf.PerlinNoise(x * .13f, y * .16f);
                    var pintura = Color32.Lerp(new Color32(131, 58, 29, 255),
                        new Color32(203, 111, 57, 255), desgaste * .7f + .30f * y / altura);
                    if (desgaste > .70f && margemX > 3 && margemY > 3)
                        pintura = Color32.Lerp(pintura, new Color32(102, 102, 83, 255),
                            Mathf.Clamp01((desgaste - .70f) * 8f));
                    pixels[y * largura + x] = new Color(pintura.r / 255f * relevo,
                        pintura.g / 255f * relevo, pintura.b / 255f * relevo);
                }
                else
                {
                    if (relevo == 1f && y < altura - 2 && x > 1) relevo = .90f;
                    pixels[y * largura + x] = new Color(relevo, relevo, relevo);
                }
            }
        }

        textura.SetPixels32(pixels);
        textura.Apply(false, true);
        float borda = casco ? 6f : 4f;
        cache = Sprite.Create(textura, new Rect(0, 0, largura, altura), new Vector2(.5f, .5f),
            100, 0, SpriteMeshType.FullRect, new Vector4(borda, borda, borda, borda));
        return cache;
    }

    private Image CriarPainel(string nome, float x, float y, float w, float h, Color cor)
    {
        var img = UIFactory.CreateBox(stage, nome, null);
        if (w >= 12 && h >= 10) img.sprite = CriarSuperficie(ref beveledFace, false);
        img.color = cor;
        img.raycastTarget = false;
        float posX = (x + w * .5f) - Tamanho.x * .5f;
        float posY = Tamanho.y * .5f - (y + h * .5f);
        SantaRitaInventory.Place(img.rectTransform, new Vector2(posX, posY), new Vector2(w, h));
        return img;
    }

    private TextMeshProUGUI CriarRotulo(string nome, string texto, float x, float y, float w, float h, float fontSize,
        Color cor, TextAlignmentOptions alinhamento, bool negrito = false)
    {
        float posX = (x + w * .5f) - Tamanho.x * .5f;
        float posY = Tamanho.y * .5f - (y + h * .5f);
        var lbl = SantaRitaInventory.Label(stage, nome, new Vector2(posX, posY), new Vector2(w, h), fontSize);
        lbl.text = texto;
        lbl.color = cor;
        lbl.alignment = alinhamento;
        if (negrito) lbl.fontStyle = FontStyles.Bold;
        return lbl;
    }

    private void CriarParafuso(string nome, float x, float y)
    {
        var img = CriarPainel(nome, x, y, 8, 8, new Color(.70f, .75f, .78f, .95f));
        img.raycastTarget = false;
    }

    private Button Key(string name, string title, float x, float y, float w, float h,
        UnityEngine.Events.UnityAction click, float fontSize)
    {
        var face = CriarPainel(name, x, y, w, h, SteelNormal);
        face.raycastTarget = true;
        var button = face.gameObject.AddComponent<Button>();
        button.targetGraphic = face;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = colors.selectedColor = new Color(1.25f, 1.15f, .90f);
        colors.pressedColor = new Color(.70f, .60f, .40f);
        colors.disabledColor = new Color(.86f, .83f, .77f);
        colors.colorMultiplier = 1.2f;
        colors.fadeDuration = .06f;
        button.colors = colors;

        var label = SantaRitaInventory.Label(face.transform, "Legenda", Vector2.zero, new Vector2(w, h), fontSize);
        label.text = title;
        label.color = Paper;
        label.alignment = TextAlignmentOptions.Center;

        button.onClick.AddListener(click);
        return button;
    }

    private Button KeyModo(string name, string title, float x, float y, float w, float h,
        UnityEngine.Events.UnityAction click, out TMP_Text labelOut)
    {
        var face = CriarPainel(name, x, y, w, h, SteelNormal);
        face.raycastTarget = true;
        var button = face.gameObject.AddComponent<Button>();
        button.targetGraphic = face;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = colors.selectedColor = new Color(1.20f, 1.10f, .85f);
        colors.pressedColor = new Color(.70f, .60f, .40f);
        colors.fadeDuration = .06f;
        button.colors = colors;

        var label = SantaRitaInventory.Label(face.transform, "Legenda", Vector2.zero, new Vector2(w, h), 10.5f);
        label.text = title;
        label.color = Paper;
        label.alignment = TextAlignmentOptions.Center;
        labelOut = label;

        button.onClick.AddListener(click);
        return button;
    }

    private Button KeyTeclado(string name, string digito, float x, float y, float w, float h,
        UnityEngine.Events.UnityAction click, out TMP_Text numLabel, out TMP_Text slotLabel)
    {
        var face = CriarPainel(name, x, y, w, h, SteelNormal);
        face.raycastTarget = true;
        var button = face.gameObject.AddComponent<Button>();
        button.targetGraphic = face;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = colors.selectedColor = new Color(1.30f, 1.20f, .90f);
        colors.pressedColor = new Color(.65f, .55f, .35f);
        colors.disabledColor = new Color(.88f, .85f, .79f);
        colors.fadeDuration = .06f;
        button.colors = colors;

        var num = SantaRitaInventory.Label(face.transform, "Digito", new Vector2(0, 2), new Vector2(28, h), 14f);
        num.text = digito;
        num.fontStyle = FontStyles.Bold;
        num.color = Paper;
        num.alignment = TextAlignmentOptions.Center;
        numLabel = num;

        var st = SantaRitaInventory.Label(face.transform, "Status", new Vector2(16, 8), new Vector2(10, 10), 8f);
        st.text = "○ LIVRE";
        st.color = new Color(.60f, .65f, .62f);
        st.alignment = TextAlignmentOptions.Center;
        slotLabel = st;

        button.onClick.AddListener(click);
        return button;
    }
}
