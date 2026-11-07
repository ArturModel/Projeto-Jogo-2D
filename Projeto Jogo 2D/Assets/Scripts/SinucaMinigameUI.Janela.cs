using UnityEngine;
using UnityEngine.InputSystem;

public partial class SinucaMinigameUI
{
    private const float TempoParaInvestigarJanela = 19f;
    private const float DuracaoMarcaJanela = 4.5f;
    private int janelaPercepcao = -1;
    private float janelaExaminadaEm = -1f;
    private AudioClip clipJanela;

    private void IniciarPercepcaoJanela()
    {
        janelaPercepcao = sorteioCenario.Next(2);
        GarantirFontePercepcao();
        if (clipJanela == null) clipJanela = CriarBatidasJanela();
        audioSobMesa.pitch = 1f;
        AtualizarVolumeSobMesa();
        audioSobMesa.PlayOneShot(clipJanela);
        FalaHistoriaEnfileirar("Bateram na janela. Tem alguém lá fora?");
    }

    private bool AtualizarPercepcaoJanela(SinucaRoom3D sala, Keyboard teclado)
    {
        if (janelaPercepcao < 0) return false;
        float idade = agendaPercepcoes.Age;
        bool janelaExaminada = janelaExaminadaEm >= 0f;
        bool saiuDaVez = estado != EstadoPartida.MirandoJogador || !turnoDeMarcus;
        if (saiuDaVez || (janelaExaminada ? idade - janelaExaminadaEm >= DuracaoMarcaJanela : idade >= TempoParaInvestigarJanela))
        {
            EncerrarPercepcaoDaMao(preservarFalas: true);
            return false;
        }

        if (janelaExaminada) return false;
        bool podeExaminar = EmPe && sala.CanInspectWindow(janelaPercepcao);
        string janela = janelaPercepcao == 0 ? "na lateral esquerda" : "na lateral direita";
        txtControlesRodape.text = podeExaminar ? "E: investigar o vidro" : "As batidas vieram da janela " + janela + ".";
        if (!podeExaminar || teclado == null || !teclado.eKey.wasPressedThisFrame) return false;

        janelaExaminadaEm = idade;
        sala.ShowWindowHandprint(janelaPercepcao, true);
        FalaHistoriaEnfileirar("Uma marca de mão... ainda tá escorrendo. Mas cadê a pessoa?");
        AtualizarVisualTacoEMira();
        return true;
    }

    private void LimparPercepcaoJanela()
    {
        if (janelaPercepcao >= 0) tableView3D?.Room?.ShowWindowHandprint(janelaPercepcao, false);
        janelaPercepcao = -1;
        janelaExaminadaEm = -1f;
    }

    private static AudioClip CriarBatidasJanela()
    {
        const int hz = 22050;
        var amostras = new float[(int)(hz * .86f)];
        float[] batidas = { 0f, .19f, .43f };
        for (int i = 0; i < amostras.Length; i++)
        {
            float t = (float)i / hz;
            for (int n = 0; n < batidas.Length; n++)
            {
                float idade = t - batidas[n];
                if (idade < 0f) break;
                float grave = Mathf.Sin(2f * Mathf.PI * 218f * idade) * Mathf.Exp(-idade * 47f);
                float vidro = Mathf.Sin(2f * Mathf.PI * (640f + n * 28f) * idade) * Mathf.Exp(-idade * 33f);
                float toque = Mathf.Sin(2f * Mathf.PI * 1843f * idade) * Mathf.Exp(-idade * 160f);
                amostras[i] += (.37f * grave + .27f * vidro + .11f * toque) *
                    Mathf.Clamp01(idade * 760f) * Mathf.Clamp01((.86f - t) * 25f);
            }
        }
        var clip = AudioClip.Create("SR_Sinuca_BatidasVidro_Original", amostras.Length, 1, hz, false);
        clip.SetData(amostras, 0);
        return clip;
    }
}
