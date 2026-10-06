using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsController : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField] private Toggle telaCheia;
    [SerializeField] private TMP_Dropdown resolucaoDropdown;
    [SerializeField] private Toggle vSync;
    [SerializeField] private Slider brilho;
    [SerializeField] private TMP_Text valorBrilho;

    [Header("Fundo do menu")]
    [SerializeField] private RawImage videoBackground;

    private readonly List<Resolution> resolucoes = new();

    private void Start()
    {
        PrepararResolucoes();

        bool fullscreenSalvo =
            PlayerPrefs.GetInt("video_fullscreen", 1) == 1;

        bool vsyncSalvo =
            PlayerPrefs.GetInt("video_vsync", 1) == 1;

        float brilhoSalvo =
            PlayerPrefs.GetFloat("video_brilho", 1f);

        telaCheia.SetIsOnWithoutNotify(fullscreenSalvo);
        vSync.SetIsOnWithoutNotify(vsyncSalvo);
        brilho.SetValueWithoutNotify(brilhoSalvo);

        QualitySettings.vSyncCount = vsyncSalvo ? 1 : 0;
        AplicarBrilho(brilhoSalvo);

        telaCheia.onValueChanged.AddListener(AlterarTelaCheia);
        resolucaoDropdown.onValueChanged.AddListener(AlterarResolucao);
        vSync.onValueChanged.AddListener(AlterarVSync);
        brilho.onValueChanged.AddListener(AlterarBrilho);

        AplicarResolucaoAtual();
    }

    // Para interfaces montadas por código (menu de pausa). Chamar antes do Start.
    public void Vincular(Toggle telaCheia, TMP_Dropdown resolucao, Toggle vSync,
                         Slider brilho, TMP_Text valorBrilho, RawImage fundo)
    {
        this.telaCheia = telaCheia;
        resolucaoDropdown = resolucao;
        this.vSync = vSync;
        this.brilho = brilho;
        this.valorBrilho = valorBrilho;
        videoBackground = fundo;
    }

    private void PrepararResolucoes()
    {
        resolucoes.Clear();
        resolucaoDropdown.ClearOptions();

        foreach (Resolution resolucao in Screen.resolutions)
        {
            bool jaExiste = resolucoes.Exists(r =>
                r.width == resolucao.width &&
                r.height == resolucao.height);

            if (!jaExiste)
                resolucoes.Add(resolucao);
        }

        List<string> opcoes = new();

        foreach (Resolution resolucao in resolucoes)
        {
            opcoes.Add($"{resolucao.width} x {resolucao.height}");
        }

        resolucaoDropdown.AddOptions(opcoes);

        int larguraSalva = PlayerPrefs.GetInt(
            "video_largura",
            Screen.currentResolution.width
        );

        int alturaSalva = PlayerPrefs.GetInt(
            "video_altura",
            Screen.currentResolution.height
        );

        int indice = resolucoes.FindIndex(r =>
            r.width == larguraSalva &&
            r.height == alturaSalva);

        if (indice < 0)
            indice = Mathf.Max(0, resolucoes.Count - 1);

        resolucaoDropdown.SetValueWithoutNotify(indice);
        resolucaoDropdown.RefreshShownValue();
    }

    public void AlterarTelaCheia(bool ativado)
    {
        PlayerPrefs.SetInt(
            "video_fullscreen",
            ativado ? 1 : 0
        );

        AplicarResolucaoAtual();
        PlayerPrefs.Save();
    }

    public void AlterarResolucao(int indice)
    {
        if (indice < 0 || indice >= resolucoes.Count)
            return;

        Resolution selecionada = resolucoes[indice];

        PlayerPrefs.SetInt(
            "video_largura",
            selecionada.width
        );

        PlayerPrefs.SetInt(
            "video_altura",
            selecionada.height
        );

        AplicarResolucaoAtual();
        PlayerPrefs.Save();
    }

    private void AplicarResolucaoAtual()
    {
        int indice = resolucaoDropdown.value;

        if (indice < 0 || indice >= resolucoes.Count)
            return;

        Resolution selecionada = resolucoes[indice];

        FullScreenMode modo = telaCheia.isOn
            ? FullScreenMode.ExclusiveFullScreen
            : FullScreenMode.Windowed;

        Screen.SetResolution(
            selecionada.width,
            selecionada.height,
            modo
        );
    }

    public void AlterarVSync(bool ativado)
    {
        QualitySettings.vSyncCount = ativado ? 1 : 0;
        PlayerPrefs.SetInt("video_vsync", ativado ? 1 : 0);
    }

    public void AlterarBrilho(float valor)
    {
        AplicarBrilho(valor);
        PlayerPrefs.SetFloat("video_brilho", valor);
    }

    private void AplicarBrilho(float valor)
    {
        if (videoBackground != null)
        {
            Color cor = videoBackground.color;
            cor.r = valor;
            cor.g = valor;
            cor.b = valor;
            videoBackground.color = cor;
        }

        if (valorBrilho != null)
        {
            valorBrilho.text =
                Mathf.RoundToInt(valor * 100f) + "%";
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}