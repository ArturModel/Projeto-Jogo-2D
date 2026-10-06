using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleSettingsController : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField] private Toggle ativarLegendas;
    [SerializeField] private Slider tamanhoLegendas;
    [SerializeField] private Toggle fundoEscuro;

    [Header("Interface")]
    [SerializeField] private TMP_Text valorTamanho;
    [SerializeField] private TMP_Text textoPreview;
    [SerializeField] private Image fundoPreview;

    private float tamanhoBasePreview;

    public static bool LegendasAtivas =>
        PlayerPrefs.GetInt("legendas_ativas", 1) == 1;

    public static float MultiplicadorTamanho =>
        PlayerPrefs.GetFloat("legendas_tamanho", 100f) / 100f;

    public static bool FundoEscuroAtivo =>
        PlayerPrefs.GetInt("legendas_fundo", 1) == 1;

    private void Start()
    {
        tamanhoBasePreview = textoPreview.fontSize;

        bool ativas = LegendasAtivas;
        float tamanho = PlayerPrefs.GetFloat(
            "legendas_tamanho",
            100f
        );
        bool fundo = FundoEscuroAtivo;

        ativarLegendas.SetIsOnWithoutNotify(ativas);
        tamanhoLegendas.SetValueWithoutNotify(tamanho);
        fundoEscuro.SetIsOnWithoutNotify(fundo);

        AplicarPreview();

        ativarLegendas.onValueChanged.AddListener(
            AlterarAtivacao
        );

        tamanhoLegendas.onValueChanged.AddListener(
            AlterarTamanho
        );

        fundoEscuro.onValueChanged.AddListener(
            AlterarFundo
        );
    }

    public void AlterarAtivacao(bool ativado)
    {
        PlayerPrefs.SetInt(
            "legendas_ativas",
            ativado ? 1 : 0
        );

        AplicarPreview();
    }

    public void AlterarTamanho(float valor)
    {
        PlayerPrefs.SetFloat("legendas_tamanho", valor);
        AplicarPreview();
    }

    public void AlterarFundo(bool ativado)
    {
        PlayerPrefs.SetInt(
            "legendas_fundo",
            ativado ? 1 : 0
        );

        AplicarPreview();
    }

    // Para interfaces montadas por código (menu de pausa). Chamar antes do Start.
    public void Vincular(Toggle ativar, Slider tamanho, Toggle fundo,
                         TMP_Text valor, TMP_Text preview, Image fundoDoPreview)
    {
        ativarLegendas = ativar;
        tamanhoLegendas = tamanho;
        fundoEscuro = fundo;
        valorTamanho = valor;
        textoPreview = preview;
        fundoPreview = fundoDoPreview;
    }

    private void AplicarPreview()
    {
        float percentual = tamanhoLegendas.value;

        // Os três elementos de preview são opcionais. No MainMenu, valorTamanho
        // e fundoPreview estão sem ligação na cena: sem estas checagens o Start
        // quebrava aqui antes de registrar os listeners, e nada era salvo.
        if (valorTamanho != null)
        {
            valorTamanho.text =
                Mathf.RoundToInt(percentual) + "%";
        }

        if (textoPreview != null)
        {
            textoPreview.fontSize =
                tamanhoBasePreview * (percentual / 100f);

            textoPreview.alpha =
                ativarLegendas.isOn ? 1f : 0.35f;
        }

        if (fundoPreview != null)
        {
            fundoPreview.enabled =
                ativarLegendas.isOn && fundoEscuro.isOn;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}