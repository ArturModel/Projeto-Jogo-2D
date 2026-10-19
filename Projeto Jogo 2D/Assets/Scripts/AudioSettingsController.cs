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

    private const string ChaveMaster = "audio_master";
    private const string ChaveMusica = "audio_musica";
    private const string ChaveEfeitos = "audio_efeitos";
    private const string ChaveAudioInicializadoV2 = "audio_init_v2";

    private static bool s_eventoCenaRegistrado;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InicializarAudioGlobal()
    {
        GarantirValoresPadraoAudiveis();
        AplicarVolumesGlobais();

        if (!s_eventoCenaRegistrado)
        {
            s_eventoCenaRegistrado = true;
            SceneManager.sceneLoaded += (_, __) => AplicarVolumesGlobais();
        }
    }

    private static void GarantirValoresPadraoAudiveis()
    {
        if (PlayerPrefs.GetInt(ChaveAudioInicializadoV2, 0) == 0)
        {
            PlayerPrefs.SetInt(ChaveAudioInicializadoV2, 1);
            if (PlayerPrefs.GetFloat(ChaveMaster, 1f) < 0.15f)
                PlayerPrefs.SetFloat(ChaveMaster, 1f);
            if (PlayerPrefs.GetFloat(ChaveMusica, 1f) < 0.15f)
                PlayerPrefs.SetFloat(ChaveMusica, 1f);
            if (PlayerPrefs.GetFloat(ChaveEfeitos, 1f) < 0.15f)
                PlayerPrefs.SetFloat(ChaveEfeitos, 1f);
            PlayerPrefs.Save();
        }
    }

    public static void AplicarVolumesGlobais(AudioMixer mixerExplicito = null)
    {
        GarantirValoresPadraoAudiveis();

        float master = PlayerPrefs.GetFloat(ChaveMaster, 1f);
        float musica = PlayerPrefs.GetFloat(ChaveMusica, 1f);
        float efeitos = PlayerPrefs.GetFloat(ChaveEfeitos, 1f);

        AudioListener.pause = false;
        AudioListener.volume = Mathf.Clamp01(master);

        if (mixerExplicito != null)
        {
            AplicarVolumeEmMixer(mixerExplicito, "VolumeMaster", master);
            AplicarVolumeEmMixer(mixerExplicito, "VolumeMusic", musica);
            AplicarVolumeEmMixer(mixerExplicito, "VolumeSFX", efeitos);
        }

        AudioMixer[] mixers = Resources.FindObjectsOfTypeAll<AudioMixer>();
        if (mixers != null)
        {
            for (int i = 0; i < mixers.Length; i++)
            {
                if (mixers[i] == null) continue;
                AplicarVolumeEmMixer(mixers[i], "VolumeMaster", master);
                AplicarVolumeEmMixer(mixers[i], "VolumeMusic", musica);
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

    private void Start()
    {
        GarantirValoresPadraoAudiveis();

        float master = PlayerPrefs.GetFloat(ChaveMaster, 1f);
        float musica = PlayerPrefs.GetFloat(ChaveMusica, 1f);
        float efeitos = PlayerPrefs.GetFloat(ChaveEfeitos, 1f);

        if (volumeGeral != null) volumeGeral.SetValueWithoutNotify(master);
        if (volumeMusica != null) volumeMusica.SetValueWithoutNotify(musica);
        if (volumeEfeitos != null) volumeEfeitos.SetValueWithoutNotify(efeitos);

        AplicarVolume("VolumeMaster", master);
        AplicarVolume("VolumeMusic", musica);
        AplicarVolume("VolumeSFX", efeitos);

        if (volumeGeral != null) volumeGeral.onValueChanged.AddListener(AlterarVolumeGeral);
        if (volumeMusica != null) volumeMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        if (volumeEfeitos != null) volumeEfeitos.onValueChanged.AddListener(AlterarVolumeEfeitos);
    }

    // Para interfaces montadas por código (menu de pausa). Chamar antes do Start.
    public void Vincular(AudioMixer mixer, Slider geral, Slider musica, Slider efeitos)
    {
        this.mixer = mixer;
        volumeGeral = geral;
        volumeMusica = musica;
        volumeEfeitos = efeitos;
        AplicarVolumesGlobais(mixer);
    }

    public void AlterarVolumeGeral(float valor)
    {
        AudioListener.volume = Mathf.Clamp01(valor);
        AplicarVolume("VolumeMaster", valor);
        PlayerPrefs.SetFloat(ChaveMaster, valor);
    }

    public void AlterarVolumeMusica(float valor)
    {
        AplicarVolume("VolumeMusic", valor);
        PlayerPrefs.SetFloat(ChaveMusica, valor);
    }

    public void AlterarVolumeEfeitos(float valor)
    {
        AplicarVolume("VolumeSFX", valor);
        PlayerPrefs.SetFloat(ChaveEfeitos, valor);
    }

    private void AplicarVolume(string parametro, float valor)
    {
        AplicarVolumeEmMixer(mixer, parametro, valor);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}