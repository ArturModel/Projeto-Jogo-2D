using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SinucaMinigameUI
{
    private const float DuracaoApagao = 2.2f;
    private const float InicioPiscada = .72f;
    private const float FimPiscada = 1.27f;

    private readonly System.Random sorteioCenario = new System.Random(System.Guid.NewGuid().GetHashCode());
    private AudioClip clipApagao;
    private AudioSource audioMedoSinuca;
    private AudioClip clipImpactoSusto, clipCoracaoSinuca;
    private RectTransform palpebraSuperior, palpebraInferior;
    private bool apagao, bolasMudaram, zeForaDoQuadro, zeVisto, tremorSinuca;
    private float tempoSustoZe, faseTremorSinuca;
    private float tempoMedoSinuca, proximaBatidaCoracao;
    private float IntensidadeTremor => tremorSinuca && !MenuPausa.ReduzirEfeitosIntensos ? 1f : 0f;
    private float AnguloMiraComTremor => anguloMiraRad + Mathf.Sin(faseTremorSinuca * 17f) * .012f * IntensidadeTremor;
    private float TremorEmpunhadura => Mathf.Sin(faseTremorSinuca * 25f) * .008f * IntensidadeTremor;
    private bool SustosSinucaBloqueiamMesa => apagao || zeVisto;

    private void IniciarApagaoSinuca()
    {
        apagao = true;
        tableView3D.Room.SetBlackout(true);
        CobrirVisaoSinuca(1f);
        GarantirFontePercepcao();
        if (clipApagao == null) clipApagao = CriarEstaloApagao();
        audioSobMesa.pitch = 1f;
        AtualizarVolumeSobMesa();
        audioSobMesa.PlayOneShot(clipApagao);
    }

    private bool AtualizarSustosSinuca(SinucaRoom3D sala, float dt)
    {
        AtualizarMedoSinuca(sala, dt);

        var evento = agendaPercepcoes.Active;
        float idade = agendaPercepcoes.Age;
        if (evento == SinucaPerceptionSchedule.Event.Blackout)
        {
            if (!bolasMudaram && idade >= .85f)
            {
                bolasMudaram = true;
                RedistribuirBolasNoApagao();
                ApontarMiraParaBolaMaisProxima();
                AtualizarVisualTacoEMira();
                FalaHistoriaEnfileirar("Ué... que que é isso? Apagou tudo!");
            }

            if (idade < DuracaoApagao) return true;
            EncerrarPercepcaoDaMao(true);
            FalaHistoriaEnfileirar("Zé, tu viu? Até as bolas mudaram de lugar.");
            FalaHistoriaEnfileirar("Não aconteceu nada, Marcus. Tá tudo do jeito que tava. Joga.", false);
            return true;
        }

        if (evento != SinucaPerceptionSchedule.Event.HauntedZe) return false;
        if (!zeVisto)
        {
            if (!EmPe || estado != EstadoPartida.MirandoJogador || !turnoDeMarcus || idade >= 24f)
            {
                EncerrarPercepcaoDaMao(true);
                return false;
            }

            if (!zeForaDoQuadro && !sala.OpponentInFrame)
                zeForaDoQuadro = sala.SetHauntedOpponent(true);
            if (agendaPercepcoes.IsForced)
                txtControlesRodape.text = zeForaDoQuadro ? "Olhe de volta para Zé." : "Tire Zé do campo de visão.";
            if (!zeForaDoQuadro || !sala.HauntedOpponentInFocus) return false;

            zeVisto = true;
            IniciarMedoSinuca();
            FalaHistoriaEnfileirar("Ah! Zé... que que foi isso?");
            return true;
        }

        // Exibe ao menos um quadro com a visão fechada antes de reabrir.
        tempoSustoZe += Mathf.Min(dt, .05f);
        float fechamento = Mathf.InverseLerp(InicioPiscada, .9f, tempoSustoZe);
        float abertura = Mathf.InverseLerp(.97f, FimPiscada, tempoSustoZe);
        CobrirVisaoSinuca(Mathf.SmoothStep(0f, 1f, fechamento - abertura));
        if (tempoSustoZe >= .9f) sala.SetHauntedOpponent(false);
        if (tempoSustoZe < FimPiscada) return true;

        EncerrarPercepcaoDaMao(preservarFalas: true, preservarMedo: true);
        FalaHistoriaEnfileirar("Calma... só mais uma tacada.");
        return true;
    }

    private void RedistribuirBolasNoApagao()
    {
        // A malha tem 55 pontos separados por mais de dois diâmetros. Cada posição
        // antiga invalida no máximo um ponto, então sempre há espaço para as 16 bolas.
        var livres = new List<Vector2>(55);
        for (int x = -5; x <= 5; x++)
        for (int y = -2; y <= 2; y++)
        {
            var ponto = new Vector2(x * 34f, y * 34f);
            if (PosicaoValidaSemColisao(ponto, -1)) livres.Add(ponto);
        }

        foreach (var bola in bolas)
        {
            if (!bola.ativa) continue;
            int sorteada = sorteioCenario.Next(livres.Count);
            bola.pos = livres[sorteada];
            livres.RemoveAt(sorteada);
        }
    }

    private void RestaurarSustosSinuca(bool preservarMedo)
    {
        if (apagao) tableView3D?.Room?.SetBlackout(false);
        if (zeForaDoQuadro) tableView3D?.Room?.SetHauntedOpponent(false);
        CobrirVisaoSinuca(0f);
        apagao = bolasMudaram = zeForaDoQuadro = zeVisto = false;
        tempoSustoZe = 0f;
        if (!preservarMedo) PararTremorSinuca();
    }

    private void IniciarMedoSinuca()
    {
        tremorSinuca = true;
        proximaBatidaCoracao = .22f;
        if (audioMedoSinuca == null)
        {
            audioMedoSinuca = gameObject.AddComponent<AudioSource>();
            audioMedoSinuca.playOnAwake = false;
        }
        if (clipImpactoSusto == null) clipImpactoSusto = CriarImpactoSusto();
        if (clipCoracaoSinuca == null) clipCoracaoSinuca = MinigamePerceptionAudio.CriarCoracao("SR_Sinuca_Coracao_Original");
        audioMedoSinuca.PlayOneShot(clipImpactoSusto, .78f * AudioSettingsController.VolumeImpactos);
    }

    private void AtualizarMedoSinuca(SinucaRoom3D sala, float dt)
    {
        if (!tremorSinuca) return;
        tempoMedoSinuca += dt;
        if (estado != EstadoPartida.BolasEmMovimento) faseTremorSinuca += dt;
        float urgencia = Mathf.Lerp(1f, .32f, Mathf.Clamp01(tempoMedoSinuca / 9f));
        Vector2 tremorCamera = Vector2.zero;
        if (EmPe && IntensidadeTremor > 0f)
        {
            float t = tempoMedoSinuca;
            float recuo = t < .33f ? Mathf.Sin(Mathf.PI * t / .33f) : 0f;
            tremorCamera = new Vector2(.46f * Mathf.Sin(t * 19f), .36f * Mathf.Sin(t * 27f)) * urgencia
                + new Vector2(.6f, -3.7f) * recuo;
        }
        sala.SetStartleLook(tremorCamera);

        proximaBatidaCoracao -= dt;
        if (proximaBatidaCoracao > 0f) return;
        proximaBatidaCoracao = Mathf.Lerp(.91f, .48f, urgencia);
        audioMedoSinuca.PlayOneShot(clipCoracaoSinuca,
            Mathf.Lerp(.23f, .55f, urgencia) * AudioSettingsController.VolumeEfeitos);
    }

    private void PararTremorSinuca()
    {
        if (!tremorSinuca) return;
        tremorSinuca = false;
        faseTremorSinuca = tempoMedoSinuca = proximaBatidaCoracao = 0f;
        audioMedoSinuca.Stop();
        tableView3D?.Room?.SetStartleLook(Vector2.zero);
    }

    private void CobrirVisaoSinuca(float cobertura)
    {
        if (palpebraSuperior == null)
        {
            if (cobertura <= 0f) return;
            palpebraSuperior = CriarPalpebraSinuca("PalpebraSuperior");
            palpebraInferior = CriarPalpebraSinuca("PalpebraInferior");
        }

        bool visivel = cobertura > 0f;
        palpebraSuperior.gameObject.SetActive(visivel);
        palpebraInferior.gameObject.SetActive(visivel);
        if (!visivel) return;
        float metade = cobertura * .5f;
        palpebraSuperior.anchorMin = new Vector2(0f, 1f - metade);
        palpebraSuperior.anchorMax = Vector2.one;
        palpebraInferior.anchorMin = Vector2.zero;
        palpebraInferior.anchorMax = new Vector2(1f, metade);
    }

    private RectTransform CriarPalpebraSinuca(string nome)
    {
        var objeto = new GameObject(nome, typeof(RectTransform), typeof(Image));
        objeto.transform.SetParent(tableView3D.transform, false);
        var imagem = objeto.GetComponent<Image>();
        imagem.color = Color.black;
        imagem.raycastTarget = false;
        imagem.rectTransform.offsetMin = imagem.rectTransform.offsetMax = Vector2.zero;
        return imagem.rectTransform;
    }

    private static AudioClip CriarEstaloApagao()
    {
        const int hz = 22050;
        var amostras = new float[(int)(hz * 1.05f)];
        float[] estalos = { 0f, .092f, .188f, .294f };
        var aleatorio = new System.Random(317);
        float grave = 0f;
        for (int i = 0; i < amostras.Length; i++)
        {
            float t = (float)i / hz;
            float ruido = 2f * (float)aleatorio.NextDouble() - 1f;
            grave = Mathf.Lerp(grave, ruido, .09f);
            float impulso = 0f;
            foreach (float inicio in estalos)
            {
                float idade = t - inicio;
                if (idade < 0f) break;
                impulso += Mathf.Min(1f, idade * 950f) * Mathf.Exp(-idade * 53f);
            }
            float ronco = Mathf.Sin(2f * Mathf.PI * (124f * t - 51f * t * t));
            float cauda = Mathf.Min(1f, t * 80f) * Mathf.Exp(-t * 7.5f) * Mathf.Clamp01((1.05f - t) * 20f);
            amostras[i] = .55f * impulso * (ruido - grave * .45f) + cauda * (ronco * .29f + grave * .25f);
        }
        var clip = AudioClip.Create("SR_Sinuca_Apagao_Original", amostras.Length, 1, hz, false);
        clip.SetData(amostras, 0);
        return clip;
    }

    private static AudioClip CriarImpactoSusto()
    {
        const int hz = 22050;
        var amostras = new float[(int)(hz * .46f)];
        var ruido = new System.Random(1990);
        for (int i = 0; i < amostras.Length; i++)
        {
            float t = (float)i / hz;
            float ataque = Mathf.Clamp01(t * 580f);
            float batida = Mathf.Sin(2f * Mathf.PI * (114f * t - 87f * t * t)) * Mathf.Exp(-17f * t);
            float estalo = (2f * (float)ruido.NextDouble() - 1f) * Mathf.Exp(-52f * t);
            amostras[i] = ataque * (.62f * batida + .31f * estalo) * Mathf.Clamp01((.46f - t) * 20f);
        }
        var clip = AudioClip.Create("SR_Sinuca_ImpactoSusto_Original", amostras.Length, 1, hz, false);
        clip.SetData(amostras, 0);
        return clip;
    }
}
