using UnityEngine;
using UnityEngine.Audio;
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

    private void Start()
    {
        float master = PlayerPrefs.GetFloat(ChaveMaster, 1f);
        float musica = PlayerPrefs.GetFloat(ChaveMusica, 1f);
        float efeitos = PlayerPrefs.GetFloat(ChaveEfeitos, 1f);

        volumeGeral.SetValueWithoutNotify(master);
        volumeMusica.SetValueWithoutNotify(musica);
        volumeEfeitos.SetValueWithoutNotify(efeitos);

        AplicarVolume("VolumeMaster", master);
        AplicarVolume("VolumeMusic", musica);
        AplicarVolume("VolumeSFX", efeitos);

        volumeGeral.onValueChanged.AddListener(AlterarVolumeGeral);
        volumeMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        volumeEfeitos.onValueChanged.AddListener(AlterarVolumeEfeitos);
    }

    // Para interfaces montadas por código (menu de pausa). Chamar antes do Start.
    public void Vincular(AudioMixer mixer, Slider geral, Slider musica, Slider efeitos)
    {
        this.mixer = mixer;
        volumeGeral = geral;
        volumeMusica = musica;
        volumeEfeitos = efeitos;
    }

    public void AlterarVolumeGeral(float valor)
    {
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
        valor = Mathf.Max(valor, 0.0001f);
        float decibeis = Mathf.Log10(valor) * 20f;
        mixer.SetFloat(parametro, decibeis);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}