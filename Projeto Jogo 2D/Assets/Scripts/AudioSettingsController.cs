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

    // Os dois menus compartilham as preferências de volume.
    // A posição de cada controle é persistida separadamente do ganho legado.
    private const string Perfil = "audio_perfil_sinuca_";
    private const string ChaveMaster = "master";
    private const string ChaveMusica = "musica";
    internal const string PreferenciaMusica = Perfil + ChaveMusica;
    private const string ChaveEfeitos = "efeitos";
    private const string ChaveFala = "fala";
    private const string ChaveChuva = "chuva";
    private const string ChaveImpactos = "impactos";

    private const float GanhoMusicaExploracao = .35f;
    private const float GanhoChuvaSinuca = .10f;
    private const float GanhoOutrosEfeitosSinuca = .078125f;

    public static float VolumeMusica => EmMinigame ? 0f : Nivel(ChaveMusica) * GanhoMusicaExploracao;
    public static float VolumeEfeitos => Nivel(ChaveEfeitos) * GanhoOutrosEfeitosSinuca;
    public static float VolumeFala => Nivel(ChaveFala);
    public static float VolumeChuva => Nivel(ChaveChuva) * GanhoChuvaSinuca;
    internal static float VolumeChuvaAmbiente => EmMinigame ? 0f : VolumeChuva;
    public static float VolumeImpactos => Nivel(ChaveImpactos);

    internal const float TrilhaDiretaMinigame = .10f;
    internal const float IntervaloColisaoBola = .018f;
    internal static float GanhoTacada(float intensidade) => Mathf.Lerp(.28f, .66f, Mathf.Clamp01(intensidade));
    internal static float GanhoColisaoBola(float intensidade) => .70f * Mathf.Sqrt(Mathf.Clamp01(intensidade));

    private static bool EmMinigame => SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing;

    private static float Nivel(string categoria) =>
        Mathf.Clamp01(PlayerPrefs.GetFloat(Perfil + categoria, 1f));

    private void AlterarNivel(string categoria, float valor)
    {
        PlayerPrefs.SetFloat(Perfil + categoria, Mathf.Clamp01(valor));
        PublicarAlteracao();
    }

    private static event System.Action VolumesAlterados;

    private bool minigameAnterior;
    private bool reaplicarVolumes;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InicializarAudioGlobal()
    {
        AplicarVolumesGlobais();

        SceneManager.sceneLoaded -= AoCarregarCena;
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo) => AplicarVolumesGlobais();

    public static void AplicarVolumesGlobais(AudioMixer mixerExplicito = null)
    {

        AudioListener.volume = Nivel(ChaveMaster);
        AtualizarChuvaMenu();

        // A trilha no mixer segue o mesmo contexto das fontes diretas.
        float ganhoMusica = VolumeMusica * .5f;
        float efeitos = VolumeEfeitos;

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
        minigameAnterior = EmMinigame;
        reaplicarVolumes = true;
    }

    private void Update()
    {
        if (!reaplicarVolumes && minigameAnterior == EmMinigame) return;
        reaplicarVolumes = false;
        minigameAnterior = EmMinigame;
        AplicarVolumesGlobais(mixer);
    }

    private void SincronizarSliders()
    {
        if (volumeGeral != null) volumeGeral.SetValueWithoutNotify(Nivel(ChaveMaster));
        if (volumeMusica != null) volumeMusica.SetValueWithoutNotify(Nivel(ChaveMusica));
        if (volumeEfeitos != null) volumeEfeitos.SetValueWithoutNotify(Nivel(ChaveEfeitos));
        if (volumeFala != null) volumeFala.SetValueWithoutNotify(Nivel(ChaveFala));
        if (volumeChuva != null) volumeChuva.SetValueWithoutNotify(Nivel(ChaveChuva));
        if (volumeImpactos != null) volumeImpactos.SetValueWithoutNotify(Nivel(ChaveImpactos));
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

    public void AlterarVolumeGeral(float valor) => AlterarNivel(ChaveMaster, valor);
    public void AlterarVolumeMusica(float valor) => AlterarNivel(ChaveMusica, valor);
    public void AlterarVolumeEfeitos(float valor) => AlterarNivel(ChaveEfeitos, valor);
    public void AlterarVolumeFala(float valor) => AlterarNivel(ChaveFala, valor);
    public void AlterarVolumeChuva(float valor) => AlterarNivel(ChaveChuva, valor);
    public void AlterarVolumeImpactos(float valor) => AlterarNivel(ChaveImpactos, valor);

    internal static void AtualizarChuvaMenu()
    {
        // A fonte legada estava no SFX, que agora controla só os outros efeitos.
        foreach (var fonte in Resources.FindObjectsOfTypeAll<AudioSource>())
            if (fonte.gameObject.scene.IsValid() && fonte.name == "MenuChuva")
            {
                fonte.outputAudioMixerGroup = null;
                fonte.volume = .25f * VolumeChuvaAmbiente;
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
