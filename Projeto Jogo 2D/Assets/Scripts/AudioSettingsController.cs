using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sliders")]
    [SerializeField] private Slider volumeGeral;
    [SerializeField] private Slider volumeMusica;
    [SerializeField] private Slider volumeEfeitos;
    [SerializeField] private Slider volumeFala;
    [SerializeField] private Slider volumeChuva;
    [SerializeField] private Slider volumeImpactos;

    public static float VolumeFala => Mathf.Clamp01(PlayerPrefs.GetFloat("audio_fala", 1f));
    public static float VolumeChuva => Mathf.Clamp01(PlayerPrefs.GetFloat("audio_chuva", .55f));
    public static float VolumeImpactos => Mathf.Clamp01(PlayerPrefs.GetFloat("audio_impactos", 1f));

    private const string ChaveMaster = "audio_master";
    private const string ChaveMusica = "audio_musica";
    private const string ChaveEfeitos = "audio_efeitos";
    private const float MusicaPadrao = 0.35f;
    private static event System.Action VolumesAlterados;

    private bool sinucaAnterior;
    private bool reaplicarVolumes;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InicializarAudioGlobal()
    {
        GarantirValoresPadraoAudiveis();
        AplicarVolumesGlobais();

        SceneManager.sceneLoaded -= AoCarregarCena;
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo) => AplicarVolumesGlobais();

    private static void GarantirValoresPadraoAudiveis()
    {
        // Preferências já escolhidas, inclusive zero, nunca são elevadas por migração.
        bool criou = false;
        if (!PlayerPrefs.HasKey(ChaveMaster))
        {
            PlayerPrefs.SetFloat(ChaveMaster, 1f); criou = true;
        }
        if (!PlayerPrefs.HasKey(ChaveMusica))
        {
            PlayerPrefs.SetFloat(ChaveMusica, MusicaPadrao); criou = true;
        }
        if (!PlayerPrefs.HasKey(ChaveEfeitos))
        {
            PlayerPrefs.SetFloat(ChaveEfeitos, 1f); criou = true;
        }
        if (!PlayerPrefs.HasKey("audio_fala")) { PlayerPrefs.SetFloat("audio_fala", 1f); criou = true; }
        if (!PlayerPrefs.HasKey("audio_chuva")) { PlayerPrefs.SetFloat("audio_chuva", .55f); criou = true; }
        if (!PlayerPrefs.HasKey("audio_impactos")) { PlayerPrefs.SetFloat("audio_impactos", 1f); criou = true; }
        if (criou) PlayerPrefs.Save();
    }

    public static void AplicarVolumesGlobais(AudioMixer mixerExplicito = null)
    {
        GarantirValoresPadraoAudiveis();

        float master = PlayerPrefs.GetFloat(ChaveMaster, 1f);
        float musica = PlayerPrefs.GetFloat(ChaveMusica, 1f);
        float efeitos = PlayerPrefs.GetFloat(ChaveEfeitos, 1f);

        AudioListener.volume = Mathf.Clamp01(master);
        AtualizarChuvaMenu();

        // No treino aberto sobre MainMenu, sua música continua no mixer do menu.
        // Fontes diretas do bar têm seu próprio fade em Capitulo1Director.
        float ganhoMusica = musica * 0.5f * (SinucaMinigameUI.IsShowing ? 0.35f : 1f);

        if (mixerExplicito != null)
        {
            AplicarVolumeEmMixer(mixerExplicito, "VolumeMaster", 1f);
            AplicarVolumeEmMixer(mixerExplicito, "VolumeMusic", ganhoMusica);
            AplicarVolumeEmMixer(mixerExplicito, "VolumeSFX", efeitos);
        }

        AudioMixer[] mixers = Resources.FindObjectsOfTypeAll<AudioMixer>();
        if (mixers != null)
        {
            for (int i = 0; i < mixers.Length; i++)
            {
                if (mixers[i] == null) continue;
                // Master é aplicado só no AudioListener; não multiplicar duas vezes no menu.
                AplicarVolumeEmMixer(mixers[i], "VolumeMaster", 1f);
                AplicarVolumeEmMixer(mixers[i], "VolumeMusic", ganhoMusica);
                AplicarVolumeEmMixer(mixers[i], "VolumeSFX", efeitos);
            }
        }
    }

    private static void AplicarVolumeEmMixer(AudioMixer alvo, string parametro, float valor)
    {
        if (alvo == null) return;
        valor = Mathf.Max(valor, 0.0001f);
        float decibeis = Mathf.Log10(valor) * 20f;
        alvo.SetFloat(parametro, decibeis);
    }

    private void OnEnable()
    {
        ConstruirCategoriasMenu();
        VolumesAlterados -= SincronizarSliders;
        VolumesAlterados += SincronizarSliders;
        VincularSliders();
        SincronizarSliders();
        sinucaAnterior = SinucaMinigameUI.IsShowing;
        reaplicarVolumes = true;
    }

    private void Update()
    {
        if (!reaplicarVolumes && sinucaAnterior == SinucaMinigameUI.IsShowing) return;
        reaplicarVolumes = false;
        sinucaAnterior = SinucaMinigameUI.IsShowing;
        AplicarVolumesGlobais(mixer);
    }

    private void SincronizarSliders()
    {
        GarantirValoresPadraoAudiveis();
        if (volumeGeral != null) volumeGeral.SetValueWithoutNotify(PlayerPrefs.GetFloat(ChaveMaster, 1f));
        if (volumeMusica != null) volumeMusica.SetValueWithoutNotify(PlayerPrefs.GetFloat(ChaveMusica, MusicaPadrao));
        if (volumeEfeitos != null) volumeEfeitos.SetValueWithoutNotify(PlayerPrefs.GetFloat(ChaveEfeitos, 1f));
        if (volumeFala != null) volumeFala.SetValueWithoutNotify(VolumeFala);
        if (volumeChuva != null) volumeChuva.SetValueWithoutNotify(VolumeChuva);
        if (volumeImpactos != null) volumeImpactos.SetValueWithoutNotify(VolumeImpactos);
    }

    private void DesvincularSliders()
    {
        if (volumeGeral != null) volumeGeral.onValueChanged.RemoveListener(AlterarVolumeGeral);
        if (volumeMusica != null) volumeMusica.onValueChanged.RemoveListener(AlterarVolumeMusica);
        if (volumeEfeitos != null) volumeEfeitos.onValueChanged.RemoveListener(AlterarVolumeEfeitos);
        if (volumeFala != null) volumeFala.onValueChanged.RemoveListener(AlterarVolumeFala);
        if (volumeChuva != null) volumeChuva.onValueChanged.RemoveListener(AlterarVolumeChuva);
        if (volumeImpactos != null) volumeImpactos.onValueChanged.RemoveListener(AlterarVolumeImpactos);
    }

    private void VincularSliders()
    {
        DesvincularSliders();
        // O menu legado tinha mínimo 0,0001; ambos devem permitir o mesmo zero real.
        foreach (var slider in new[] { volumeGeral, volumeMusica, volumeEfeitos, volumeFala, volumeChuva, volumeImpactos })
            if (slider != null) { slider.minValue = 0f; slider.maxValue = 1f; }
        if (volumeGeral != null) volumeGeral.onValueChanged.AddListener(AlterarVolumeGeral);
        if (volumeMusica != null) volumeMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        if (volumeEfeitos != null) volumeEfeitos.onValueChanged.AddListener(AlterarVolumeEfeitos);
        if (volumeFala != null) volumeFala.onValueChanged.AddListener(AlterarVolumeFala);
        if (volumeChuva != null) volumeChuva.onValueChanged.AddListener(AlterarVolumeChuva);
        if (volumeImpactos != null) volumeImpactos.onValueChanged.AddListener(AlterarVolumeImpactos);
    }

    // Vinculação funciona mesmo com a pausa ainda inativa; não depende de Start.
    public void Vincular(AudioMixer mixer, Slider geral, Slider musica, Slider efeitos,
        Slider fala = null, Slider chuva = null, Slider impactos = null)
    {
        DesvincularSliders();
        this.mixer = mixer;
        volumeGeral = geral;
        volumeMusica = musica;
        volumeEfeitos = efeitos;
        volumeFala = fala; volumeChuva = chuva; volumeImpactos = impactos;
        SincronizarSliders();
        if (isActiveAndEnabled) VincularSliders();
        AplicarVolumesGlobais(mixer);
    }

    public void AlterarVolumeGeral(float valor)
    {
        PlayerPrefs.SetFloat(ChaveMaster, Mathf.Clamp01(valor));
        PublicarAlteracao();
    }

    public void AlterarVolumeMusica(float valor)
    {
        PlayerPrefs.SetFloat(ChaveMusica, Mathf.Clamp01(valor));
        PublicarAlteracao();
    }

    public void AlterarVolumeEfeitos(float valor)
    {
        PlayerPrefs.SetFloat(ChaveEfeitos, Mathf.Clamp01(valor));
        PublicarAlteracao();
    }

    public void AlterarVolumeFala(float valor)
    {
        PlayerPrefs.SetFloat("audio_fala", Mathf.Clamp01(valor)); PublicarAlteracao();
    }
    public void AlterarVolumeChuva(float valor)
    {
        PlayerPrefs.SetFloat("audio_chuva", Mathf.Clamp01(valor)); PublicarAlteracao();
    }
    public void AlterarVolumeImpactos(float valor)
    {
        PlayerPrefs.SetFloat("audio_impactos", Mathf.Clamp01(valor)); PublicarAlteracao();
    }

    private static void AtualizarChuvaMenu()
    {
        // A fonte legada estava no SFX, que agora controla só os outros efeitos.
        foreach (var fonte in Resources.FindObjectsOfTypeAll<AudioSource>())
            if (fonte.gameObject.scene.IsValid() && fonte.name == "MenuChuva")
            {
                fonte.outputAudioMixerGroup = null;
                fonte.volume = .25f * VolumeChuva;
            }
    }

    private void ConstruirCategoriasMenu()
    {
        if (volumeMusica == null || volumeMusica.transform.parent.name != "AudioPanel") return;
        var pai = volumeMusica.transform.parent;
        var modeloRotulo = pai.Find("label_musica").GetComponent<TMPro.TMP_Text>();
        if (volumeFala == null) volumeFala = ClonarCategoria(volumeMusica, modeloRotulo, "fala", "FALA");
        if (volumeChuva == null) volumeChuva = ClonarCategoria(volumeMusica, modeloRotulo, "chuva", "CHUVA");
        if (volumeImpactos == null) volumeImpactos = ClonarCategoria(volumeMusica, modeloRotulo, "impactos", "TACO E BOLAS");
        var sliders = new[] { volumeGeral, volumeMusica, volumeFala, volumeChuva, volumeImpactos, volumeEfeitos };
        var labels = new[] { "label_volumeGeral", "label_musica", "label_fala", "label_chuva", "label_impactos", "label_efeitos" };
        for (int i=0;i<sliders.Length;i++)
        {
            float y=94-i*38;
            var r=sliders[i].GetComponent<RectTransform>();r.anchoredPosition=new Vector2(r.anchoredPosition.x,y);
            var label=pai.Find(labels[i]).GetComponent<TMPro.TMP_Text>();
            var lr=label.rectTransform;lr.anchoredPosition=new Vector2(lr.anchoredPosition.x,y);
            lr.sizeDelta=new Vector2(lr.sizeDelta.x,34);
            label.fontSize=24;label.enableAutoSizing=false;
            if(i==5)label.text="OUTROS EFEITOS";
        }
    }

    private Slider ClonarCategoria(Slider modelo, TMPro.TMP_Text rotulo, string id, string texto)
    {
        var clone=Instantiate(modelo.gameObject,modelo.transform.parent);
        clone.name="Slider_"+id;
        var slider=clone.GetComponent<Slider>();slider.onValueChanged=new Slider.SliderEvent();
        var label=Instantiate(rotulo.gameObject,rotulo.transform.parent);label.name="label_"+id;
        label.GetComponent<TMPro.TMP_Text>().text=texto;
        return slider;
    }

    private void PublicarAlteracao()
    {
        AplicarVolumesGlobais(mixer);
        VolumesAlterados?.Invoke();
        PlayerPrefs.Save();
    }

    private void OnDisable()
    {
        DesvincularSliders();
        VolumesAlterados -= SincronizarSliders;
        PlayerPrefs.Save();
    }
}
