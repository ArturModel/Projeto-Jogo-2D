using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// O orelhão: único ponto de salvamento manual do jogo (Regra R1).
// Ao atender, toca o áudio autêntico de orelhão brasileiro (tirar do gancho, tom de linha 425 Hz,
// leitura de cartão/ficha, discagem e chiado de linha chuvosa) e abre a seleção de 5 Slots de Save.
public class PayphoneCheckpoint : Interactable
{
    public static bool IsSaveUIOpen { get; private set; }

    [Header("Identificador Estável do Orelhão")]
    [SerializeField] private string payphoneId = "orelhao_encruzilhada";

    [Header("Orelhão")]
    [TextArea]
    [SerializeField]
    private string[] falas =
    {
        "\"Alô? ... Só chiado de chuva na linha.\"",
        "\"Tento discar para a casa de Adilio de novo. Chama até cair.\"",
        "\"A linha interurbana está muda por causa do temporal na serra.\"",
        "\"Se eu ligar para o plantão agora dizendo só que ele não atendeu hoje, vão mandar esperar até amanhã.\"",
        "\"Preciso descobrir por conta própria onde Adilio esteve hoje.\""
    };

    [SerializeField] private string textoDeSave = "Progresso salvo manualmente no orelhão.";

    private static GameObject s_modalGo;
    private static AudioSource s_audioSource;
    private static AudioClip s_clipAtenderDiscar;
    private static AudioClip s_clipConfirmarGancho;
    private static PayphoneCheckpoint s_orelhaoAtivo;
    private static Vector2 s_savePosAtivo;
    private static bool s_modoApenasCarregar;
    private static readonly Button[] s_botoesSlot = new Button[CheckpointSystem.TotalSlots];
    private static readonly TextMeshProUGUI[] s_textosSlot = new TextMeshProUGUI[CheckpointSystem.TotalSlots];
    private static TextMeshProUGUI s_tituloModal;
    private static TextMeshProUGUI s_subtituloModal;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatic()
    {
        IsSaveUIOpen = false;
        s_modalGo = null;
        s_audioSource = null;
        s_orelhaoAtivo = null;
    }

    public string PayphoneId => payphoneId;

    public void ConfigurarId(string novoId)
    {
        if (!string.IsNullOrEmpty(novoId))
        {
            payphoneId = novoId;
        }
    }

    public override string Prompt => "Atender Orelhão / Salvar";

    public override void Interact()
    {
        if (IsSaveUIOpen) return;

        var player = FindAnyObjectByType<PlayerController>();
        s_savePosAtivo = player != null ? (Vector2)player.transform.position : (Vector2)transform.position;
        s_orelhaoAtivo = this;
        s_modoApenasCarregar = false;

        TocarSomAtenderOrelhao();
        AbrirModalSlots(false);
    }

    public static void AbrirModalCarregarDoMenu()
    {
        s_orelhaoAtivo = null;
        s_modoApenasCarregar = true;
        TocarSomAtenderOrelhao();
        AbrirModalSlots(true);
    }

    private static void GarantirAudioOrelhao()
    {
        if (s_audioSource == null)
        {
            var goAudio = new GameObject("OrelhaoAudioPlayer");
            if (Application.isPlaying)
                DontDestroyOnLoad(goAudio);
            s_audioSource = goAudio.AddComponent<AudioSource>();
            s_audioSource.playOnAwake = false;
            s_audioSource.spatialBlend = 0f;
            s_audioSource.ignoreListenerPause = true;
        }

        if (s_clipAtenderDiscar == null)
            s_clipAtenderDiscar = GerarClipAtenderEDiscarOrelhao();
        if (s_clipConfirmarGancho == null)
            s_clipConfirmarGancho = GerarClipConfirmarSaveGancho();
    }

    public static void TocarSomAtenderOrelhao()
    {
        GarantirAudioOrelhao();
        float vol = Mathf.Clamp01(PlayerPrefs.GetFloat("audio_master", 1f) * PlayerPrefs.GetFloat("audio_efeitos", 1f));
        s_audioSource.Stop();
        s_audioSource.clip = s_clipAtenderDiscar;
        s_audioSource.volume = Mathf.Max(0.25f, vol * 0.9f);
        s_audioSource.Play();
    }

    public static void TocarSomConfirmarGancho()
    {
        GarantirAudioOrelhao();
        float vol = Mathf.Clamp01(PlayerPrefs.GetFloat("audio_master", 1f) * PlayerPrefs.GetFloat("audio_efeitos", 1f));
        s_audioSource.Stop();
        s_audioSource.PlayOneShot(s_clipConfirmarGancho, Mathf.Max(0.3f, vol * 0.95f));
    }

    /// <summary>
    /// Sintetiza o som autêntico de um telefone público (orelhão) brasileiro dos anos 2000:
    /// 1) Batida metálica de tirar o monofone do gancho;
    /// 2) Tom contínuo de linha telefônica brasileira (425 Hz filtrado);
    /// 3) Bip duplo de leitura do cartão/ficha + discagem DTMF;
    /// 4) Tom de chamada (425 Hz modulado) com chiado característico de linha sob chuva.
    /// </summary>
    private static AudioClip GerarClipAtenderEDiscarOrelhao()
    {
        const int sampleRate = 22050;
        const float duracao = 2.65f;
        int totalSamples = Mathf.CeilToInt(sampleRate * duracao);
        float[] samples = new float[totalSamples];

        // Sequência DTMF de 7 dígitos (par de frequências por dígito)
        float[,] dtmf =
        {
            { 697f, 1209f }, { 697f, 1336f }, { 770f, 1336f }, { 852f, 1209f },
            { 770f, 1477f }, { 852f, 1336f }, { 941f, 1336f }
        };

        uint rng = 0x5A17C9E3u;
        for (int i = 0; i < totalSamples; i++)
        {
            float t = i / (float)sampleRate;
            rng = rng * 1664525u + 1013904223u;
            float noise = ((rng & 0xFFFF) / 32768f) - 1f;

            float s = 0f;

            // Chiado de linha telefônica antiga durante toda a ligação
            if (t > 0.10f)
            {
                s += noise * 0.032f;
                s += Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.015f; // hum 60Hz da rede
            }

            // 0.00s - 0.13s: Clack metálico de tirar o fone do gancho do orelhão
            if (t < 0.13f)
            {
                float env = Mathf.Exp(-t * 42f);
                float click1 = Mathf.Sin(2f * Mathf.PI * 310f * t) + 0.6f * Mathf.Sin(2f * Mathf.PI * 1180f * t);
                s += (click1 * 0.45f + noise * 0.55f) * env * 0.85f;
            }
            // 0.06s - 0.11s: Segundo estalo da mola do gancho
            if (t >= 0.065f && t < 0.12f)
            {
                float dt = t - 0.065f;
                float env = Mathf.Exp(-dt * 55f);
                s += (Mathf.Sin(2f * Mathf.PI * 820f * dt) * 0.5f + noise * 0.4f) * env * 0.65f;
            }

            // 0.16s - 0.78s: Tom de discar padrão brasileiro (425 Hz contínuo)
            if (t >= 0.16f && t < 0.78f)
            {
                float tone = Mathf.Sin(2f * Mathf.PI * 425f * t) + 0.18f * Mathf.Sin(2f * Mathf.PI * 850f * t);
                s += tone * 0.28f;
            }

            // 0.80s - 0.94s: Bip metálico de leitura de cartão indutivo / queda de ficha do orelhão
            if (t >= 0.80f && t < 0.86f)
            {
                s += Mathf.Sin(2f * Mathf.PI * 1780f * t) * 0.25f;
            }
            else if (t >= 0.88f && t < 0.94f)
            {
                s += Mathf.Sin(2f * Mathf.PI * 2120f * t) * 0.25f;
            }

            // 0.98s - 1.75s: Discagem rápida nas teclas metálicas do orelhão (7 tons DTMF)
            if (t >= 0.98f && t < 1.75f)
            {
                float rel = t - 0.98f;
                int digito = Mathf.FloorToInt(rel / 0.11f);
                float dentro = rel - digito * 0.11f;
                if (digito >= 0 && digito < 7 && dentro < 0.075f)
                {
                    float fLow = dtmf[digito, 0];
                    float fHigh = dtmf[digito, 1];
                    float clickTecla = dentro < 0.008f ? noise * 0.35f : 0f;
                    s += (Mathf.Sin(2f * Mathf.PI * fLow * t) + Mathf.Sin(2f * Mathf.PI * fHigh * t)) * 0.18f + clickTecla;
                }
            }

            // 1.85s - 2.65s: Tom de chamando na linha (425 Hz com cadência vibrante de 25 Hz) + estática de tempestade
            if (t >= 1.85f)
            {
                float mod = 0.75f + 0.25f * Mathf.Sin(2f * Mathf.PI * 25f * t);
                float ring = Mathf.Sin(2f * Mathf.PI * 425f * t) * mod * 0.26f;
                float fadeOut = Mathf.Clamp01((duracao - t) / 0.25f);
                s += (ring + noise * 0.055f) * fadeOut;
            }

            samples[i] = Mathf.Clamp(s, -0.95f, 0.95f);
        }

        AudioClip clip = AudioClip.Create("Orelhao_Atender_Discar", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    /// <summary>
    /// Sintetiza o som de confirmação de gravação no orelhão (sinal de validação + batida do fone no gancho).
    /// </summary>
    private static AudioClip GerarClipConfirmarSaveGancho()
    {
        const int sampleRate = 22050;
        const float duracao = 0.52f;
        int totalSamples = Mathf.CeilToInt(sampleRate * duracao);
        float[] samples = new float[totalSamples];

        uint rng = 0x9E3779B9u;
        for (int i = 0; i < totalSamples; i++)
        {
            float t = i / (float)sampleRate;
            rng = rng * 1664525u + 1013904223u;
            float noise = ((rng & 0xFFFF) / 32768f) - 1f;
            float s = 0f;

            // 0.00s - 0.18s: Duplo bip telefônico de confirmação da gravação
            if (t < 0.08f)
                s += Mathf.Sin(2f * Mathf.PI * 960f * t) * 0.30f;
            else if (t >= 0.10f && t < 0.20f)
                s += Mathf.Sin(2f * Mathf.PI * 1440f * t) * 0.32f;

            // 0.23s - 0.45s: Batida mecânica do fone voltando ao gancho metálico do orelhão
            if (t >= 0.23f && t < 0.42f)
            {
                float dt = t - 0.23f;
                float env = Mathf.Exp(-dt * 38f);
                float thud = Mathf.Sin(2f * Mathf.PI * 240f * dt) * 0.55f + Mathf.Sin(2f * Mathf.PI * 760f * dt) * 0.35f;
                s += (thud + noise * 0.45f) * env * 0.9f;
            }

            samples[i] = Mathf.Clamp(s, -0.95f, 0.95f);
        }

        AudioClip clip = AudioClip.Create("Orelhao_Gancho_Save", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static void AbrirModalSlots(bool apenasCarregar)
    {
        GarantirModalCriado();
        s_modoApenasCarregar = apenasCarregar;

        if (s_tituloModal != null)
        {
            s_tituloModal.text = apenasCarregar
                ? "ORELHÃO — ESCOLHA O SLOT PARA CARREGAR"
                : "ORELHÃO TELEFÔNICO — ESCOLHA UM SLOT PARA SALVAR";
        }

        if (s_subtituloModal != null)
        {
            s_subtituloModal.text = apenasCarregar
                ? "Clique em um dos 5 registros salvos (ou pressione [1]-[5]). [ESC] Voltar"
                : "Linha chuvosa (425 Hz)... Escolha em qual dos 5 Slots deseja gravar seu progresso ([1]-[5] ou Clique):";
        }

        AtualizarTextosDosSlots();
        s_modalGo.SetActive(true);
        IsSaveUIOpen = true;
    }

    public static void FecharModalSlots()
    {
        if (s_modalGo != null)
            s_modalGo.SetActive(false);
        IsSaveUIOpen = false;
    }

    private static void GarantirModalCriado()
    {
        if (s_modalGo != null) return;

        var host = new GameObject("PayphoneSaveSlotsUI");
        if (Application.isPlaying)
            DontDestroyOnLoad(host);
        host.AddComponent<PayphoneSlotInputWatcher>();

        Canvas canvas = UIFactory.CreateCanvas(host.transform, "PayphoneSaveCanvas", 9600);
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        s_modalGo = new GameObject("ModalRoot", typeof(RectTransform), typeof(Image));
        s_modalGo.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(s_modalGo.GetComponent<RectTransform>());
        s_modalGo.GetComponent<Image>().color = new Color(0.01f, 0.02f, 0.03f, 0.86f);

        Image caixa = UIFactory.CreateBox(s_modalGo.transform, "CaixaSlotsOrelhao", "DialogueBox");
        caixa.color = new Color(0.11f, 0.13f, 0.16f, 0.99f);
        RectTransform cRect = caixa.rectTransform;
        Posicionar(cRect, Vector2.zero, new Vector2(460f, 294f));

        s_tituloModal = CriarTexto(cRect, "Titulo", "ORELHÃO TELEFÔNICO — ESCOLHA UM SLOT PARA SALVAR", 16f,
            new Color(0.98f, 0.86f, 0.44f, 1f), TextAlignmentOptions.Center);
        Posicionar(s_tituloModal.rectTransform, new Vector2(0f, 122f), new Vector2(430f, 22f));

        s_subtituloModal = CriarTexto(cRect, "Subtitulo", "", 11.5f,
            new Color(0.80f, 0.86f, 0.90f, 0.95f), TextAlignmentOptions.Center);
        Posicionar(s_subtituloModal.rectTransform, new Vector2(0f, 99f), new Vector2(430f, 18f));

        for (int i = 0; i < CheckpointSystem.TotalSlots; i++)
        {
            int slotCapturado = i;
            float y = 62f - i * 38f;

            var btnGo = new GameObject("BtnSlot_" + (i + 1), typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(cRect, false);
            Posicionar(btnGo.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(416f, 32f));

            var img = btnGo.GetComponent<Image>();
            img.color = new Color(0.17f, 0.21f, 0.25f, 0.98f);

            var txt = CriarTexto(btnGo.transform, "TxtSlot", "", 13f, new Color(0.95f, 0.93f, 0.86f, 1f), TextAlignmentOptions.Left);
            Posicionar(txt.rectTransform, new Vector2(6f, 0f), new Vector2(392f, 28f));

            var btn = btnGo.GetComponent<Button>();
            btn.targetGraphic = img;
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1f, 0.92f, 0.65f, 1f);
            cb.pressedColor = new Color(0.82f, 0.95f, 0.78f, 1f);
            btn.colors = cb;

            btn.onClick.AddListener(() => AoClicarSlot(slotCapturado));

            s_botoesSlot[i] = btn;
            s_textosSlot[i] = txt;
        }

        var btnFecharGo = new GameObject("BtnDesligarFone", typeof(RectTransform), typeof(Image), typeof(Button));
        btnFecharGo.transform.SetParent(cRect, false);
        Posicionar(btnFecharGo.GetComponent<RectTransform>(), new Vector2(0f, -124f), new Vector2(190f, 24f));
        var imgFechar = btnFecharGo.GetComponent<Image>();
        imgFechar.color = new Color(0.34f, 0.16f, 0.15f, 0.98f);

        var txtFechar = CriarTexto(btnFecharGo.transform, "TxtFechar", "Desligar / Cancelar [ESC]", 12f,
            new Color(0.98f, 0.88f, 0.84f, 1f), TextAlignmentOptions.Center);
        UIFactory.Stretch(txtFechar.rectTransform);

        var btnFechar = btnFecharGo.GetComponent<Button>();
        btnFechar.targetGraphic = imgFechar;
        btnFechar.onClick.AddListener(() =>
        {
            TocarSomConfirmarGancho();
            FecharModalSlots();
        });

        s_modalGo.SetActive(false);
    }

    private static void AtualizarTextosDosSlots()
    {
        for (int i = 0; i < CheckpointSystem.TotalSlots; i++)
        {
            if (s_textosSlot[i] == null) continue;

            if (CheckpointSystem.TryLoadSlot(i, out ManualSaveData data))
            {
                string local = string.IsNullOrEmpty(data.localDescricao)
                    ? CheckpointSystem.ObterNomeAmigavelOrelhao(data.payphoneId, data.sceneName)
                    : data.localDescricao;
                string acao = s_modoApenasCarregar ? "[CARREGAR]" : "[SOBRESCREVER]";
                s_textosSlot[i].text = $"<b>[{i + 1}] SLOT {i + 1}:</b> <color=#88E8A0>{local}</color>  <size=88%><color=#C8C2B4>({data.salvoEm})</color></size>  <color=#FFD868>{acao}</color>";
            }
            else
            {
                string status = CheckpointSystem.DescreverSlotIndisponivel(i);
                string acao = s_modoApenasCarregar ? "<color=#88847C>[INDISPONÍVEL]</color>" : "<color=#78D8FF>[GRAVAR AQUI]</color>";
                s_textosSlot[i].text = $"<b>[{i + 1}] SLOT {i + 1}:</b> <color=#98948C>{status}</color>   {acao}";
            }
        }
    }

    internal static void AoClicarSlot(int slotIndex)
    {
        if (!IsSaveUIOpen) return;

        if (s_modoApenasCarregar)
        {
            if (!CheckpointSystem.TryLoadSlot(slotIndex, out _))
            {
                return;
            }
            TocarSomConfirmarGancho();
            FecharModalSlots();
            CheckpointSystem.CarregarSlot(slotIndex);
            return;
        }

        // Modo Salvar no Orelhão
        TocarSomConfirmarGancho();

        GameState.DefinirNumero(
            Flags.OrelhaoUsos,
            GameState.Numero(Flags.OrelhaoUsos) + 1
        );

        string idOrelhao = s_orelhaoAtivo != null ? s_orelhaoAtivo.payphoneId : "orelhao_padrao";
        string falaEscolhida = s_orelhaoAtivo != null
            ? s_orelhaoAtivo.ProximaFalaContextual()
            : "\"Alô? ... Só chiado de chuva na linha.\"";

        CheckpointSystem.SaveSlot(
            slotIndex,
            SceneManager.GetActiveScene().name,
            s_savePosAtivo,
            idOrelhao
        );

        FecharModalSlots();

        string nomeLocal = CheckpointSystem.ObterNomeAmigavelOrelhao(idOrelhao, SceneManager.GetActiveScene().name);
        GameMessageUI.Instance.Show(
            $"{falaEscolhida}\n<size=75%><color=#88E098>[ Salvo no SLOT {slotIndex + 1} — {nomeLocal} ]</color></size>",
            "Marcus",
            "marcus"
        );
    }

    private string ProximaFalaContextual()
    {
        if (Capitulo2Director.CenaDoCapitulo2(SceneManager.GetActiveScene().name))
        {
            if (GameState.Tem(Capitulo2Director.Vitoria))
                return "\"Registro a segunda ficha e o relato de Tonão. O recado do vô menciona Amaro; ainda não sei para onde o levaram.\"";
            if (GameState.Tem("c2_ajuda_comunicada"))
                return "\"O plantão recebeu meu relato pelo telefone da Rodoviária. Organizo aqui as pistas antes de continuar.\"";
            return "\"A linha está funcionando. Guardo no orelhão o que já consegui confirmar sobre a consulta do vô.\"";
        }
        if (GameState.Tem(Flags.VenceuZeSinuca))
        {
            return "\"A linha continua chiando. Agora tenho a ficha do Clube e o que o Zé deixou escapar: Adilio não saiu de casa por vontade própria.\"";
        }

        if (GameState.Tem(Flags.FalouAtendenteMercadinho))
        {
            return "\"Só estática na linha. Se um desconhecido perguntou por Adilio no mercadinho e esteve com o Zé antes, a resposta está no bar da rua norte.\"";
        }

        if (GameState.Tem(Flags.AchouReciboMercadinho) || GameState.Tem(Flags.ExaminouDesordemCasa))
        {
            return "\"Sem linha limpa para fora da cidade. Com o café passado hoje e o tapete revirado na sala, não posso dar meia-volta.\"";
        }

        if (falas == null || falas.Length == 0)
        {
            return "\"Chiado de chuva na linha.\"";
        }

        int index = Mathf.Clamp(
            GameState.Numero(Flags.OrelhaoVozIndice),
            0,
            falas.Length - 1
        );

        string fala = falas[index];
        if (fala != null && (fala.Contains("clube vivo") || fala.Contains("Clube vivo")))
        {
            fala = "\"A linha interurbana está muda por causa do temporal na serra. Preciso verificar a cidade pessoalmente.\"";
        }

        if (index < falas.Length - 1)
        {
            GameState.DefinirNumero(Flags.OrelhaoVozIndice, index + 1);
        }

        return fala;
    }

    private static TextMeshProUGUI CriarTexto(Transform pai, string nome, string conteudo, float tamanho, Color cor, TextAlignmentOptions alinhamento)
    {
        TextMeshProUGUI t = UIFactory.CreateLabel(pai, nome, tamanho);
        t.text = conteudo;
        t.color = cor;
        t.alignment = alinhamento;
        return t;
    }

    private static void Posicionar(RectTransform r, Vector2 pos, Vector2 tam)
    {
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta = tam;
    }

    private sealed class PayphoneSlotInputWatcher : MonoBehaviour
    {
        private void Update()
        {
            if (!IsSaveUIOpen || Keyboard.current == null) return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TocarSomConfirmarGancho();
                FecharModalSlots();
                return;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
                AoClicarSlot(0);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
                AoClicarSlot(1);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
                AoClicarSlot(2);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
                AoClicarSlot(3);
            else if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
                AoClicarSlot(4);
        }
    }
}
