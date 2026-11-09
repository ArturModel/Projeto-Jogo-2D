using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A fala da sinuca é a referência de apresentação para os minigames.
internal sealed class MinigameDialogue
{
    public const float MarcusPitch = .77f;
    public const float ZePitch = 1f;

    internal static readonly Color32 CorFundo = new Color32(10, 13, 11, 220);

    private const float TamanhoBase = 16f;
    private const float VolumeBase = .20f;
    private const float TempoFade = .3f;
    private static readonly AudioClip[] silabas = new AudioClip[4];

    private readonly CanvasGroup painel;
    private readonly Image fundo;
    private readonly TextMeshProUGUI texto;
    private readonly AudioSource voz;
    private float pitch, opacidade, ateLetra, ateSilaba, leitura;
    private int caractere, silaba;

    public bool Ocupada => !string.IsNullOrEmpty(texto.text);

    public MinigameDialogue(Transform pai, string nome, float altura = 64f)
    {
        var objeto = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        var rect = (RectTransform)objeto.transform;
        rect.SetParent(pai, false);
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 0);
        rect.anchoredPosition = new Vector2(0, altura);
        rect.sizeDelta = new Vector2(520, 48);

        fundo = objeto.GetComponent<Image>();
        fundo.color = CorFundo;
        fundo.raycastTarget = false;
        painel = objeto.GetComponent<CanvasGroup>();
        painel.blocksRaycasts = painel.interactable = false;
        painel.alpha = 0;

        texto = UIFactory.CreateLabel(rect, "Fala", TamanhoBase);
        texto.rectTransform.anchorMin = texto.rectTransform.anchorMax = new Vector2(.5f, .5f);
        texto.rectTransform.anchoredPosition = Vector2.zero;
        texto.rectTransform.sizeDelta = new Vector2(496, 42);
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color32(244, 233, 208, 255);
        texto.richText = true;
        texto.textWrappingMode = TextWrappingModes.Normal;
        texto.overflowMode = TextOverflowModes.Overflow;

        voz = objeto.AddComponent<AudioSource>();
        voz.playOnAwake = false;
        voz.spatialBlend = 0;
    }

    public void Dizer(string locutor, string fala, float tom = 1f, float leituraMinima = 2.2f)
    {
        voz.Stop();
        pitch = tom;
        string prefixo = locutor + ": ";
        texto.text = prefixo + fala;
        AplicarPreferencias();
        texto.ForceMeshUpdate();
        texto.maxVisibleCharacters = caractere = prefixo.Length;
        ateLetra = ateSilaba = opacidade = 0;
        leitura = Mathf.Max(leituraMinima, fala.Length * .035f);
    }

    public void Atualizar(float dt)
    {
        if (!Ocupada) return;
        AplicarPreferencias();
        voz.volume = VolumeBase * AudioSettingsController.VolumeFala;
        voz.UnPause();

        opacidade = Mathf.MoveTowards(opacidade, 1, dt * 8);
        ateSilaba -= dt;
        if (caractere < texto.textInfo.characterCount)
        {
            ateLetra -= dt;
            if (ateLetra <= 0)
            {
                char letra = texto.textInfo.characterInfo[caractere].character;
                texto.maxVisibleCharacters = ++caractere;
                ateLetra += char.IsPunctuation(letra) ? .16f : .032f;
                if (char.IsLetterOrDigit(letra) && ateSilaba <= 0)
                {
                    TocarSilaba();
                    ateSilaba = .095f;
                }
            }
        }
        else
        {
            leitura -= dt;
            if (leitura <= 0)
            {
                Limpar();
                return;
            }
            if (leitura < TempoFade) opacidade = leitura / TempoFade;
        }
        painel.alpha = SubtitleSettingsController.LegendasAtivas ? opacidade : 0;
    }

    public bool RevelarRestante()
    {
        if (!Ocupada || caractere >= texto.textInfo.characterCount) return false;
        texto.maxVisibleCharacters = caractere = texto.textInfo.characterCount;
        return true;
    }

    public void Suspender()
    {
        opacidade = 0;
        if (painel != null) painel.alpha = 0;
        if (voz != null) voz.Pause();
    }

    public void Limpar()
    {
        opacidade = 0;
        if (texto != null) texto.text = "";
        if (painel != null) painel.alpha = 0;
        if (voz != null) voz.Stop();
    }

    private void AplicarPreferencias()
    {
        texto.fontSize = TamanhoBase * SubtitleSettingsController.MultiplicadorTamanho;
        fundo.enabled = SubtitleSettingsController.FundoEscuroAtivo;
    }

    private void TocarSilaba()
    {
        if (voz.volume <= .001f || voz.isPlaying) return;
        // O Play Mode pode descartar o clipe e preservar o cache estático.
        var clip = silabas[silaba];
        if (clip == null)
        {
            clip = CriarSilaba(silaba);
            silabas[silaba] = clip;
        }
        silaba = (silaba + 3) % silabas.Length;
        voz.pitch = pitch;
        voz.clip = clip;
        voz.Play();
    }

    private static AudioClip CriarSilaba(int variante)
    {
        // Síntese original da sinuca. Cada locutor varia o tom da mesma base.
        const int hz = 22050;
        var amostras = new float[(int)(hz * .075f)];
        float fundamental = 112 + variante * 9;
        for (int i = 0; i < amostras.Length; i++)
        {
            float fase = 2 * Mathf.PI * fundamental * i / hz;
            float envelope = Mathf.Pow(Mathf.Max(0, Mathf.Sin(Mathf.PI * i / (amostras.Length - 1))), 1.5f);
            float onda = Mathf.Sin(fase) * .48f + Mathf.Sin(fase * 2) * .24f
                + Mathf.Sin(fase * (variante % 2 == 0 ? 5 : 6)) * .12f;
            amostras[i] = onda * envelope * .65f;
        }
        var clip = AudioClip.Create("SR_Minigame_Murmurio_" + variante, amostras.Length, 1, hz, false);
        clip.SetData(amostras, 0);
        return clip;
    }
}
