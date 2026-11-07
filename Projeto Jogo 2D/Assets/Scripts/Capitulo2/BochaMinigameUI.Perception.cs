using System;
using UnityEngine;

public sealed partial class BochaMinigameUI
{
    private enum Alucinacao { Nenhuma, FiguraSentada, Buraco }

    private const float TempoQueda = 1.85f;
    private readonly MinigamePerceptionSchedule<Alucinacao> agendaBocha =
        new MinigamePerceptionSchedule<Alucinacao>(Guid.NewGuid().GetHashCode(), Alucinacao.FiguraSentada, Alucinacao.Buraco);
    private MinigameDialogue dialogoMarcus;
    private AudioSource audioAlucinacao;
    private AudioClip coracao;
    private float focoFigura, tempoFigura, medo, faseTremor, ateBatida, tremorMira, tempoBuraco;
    private BochaBall bochaAlucinada;
    private bool caindo, tremorPendente;

    private void ConstruirPercepcoes()
    {
        dialogoMarcus = new MinigameDialogue(raiz.transform, "LegendaMarcus", AlturaLegendaBocha);
        audioAlucinacao = CriarFonte();
    }

    private void MarcusComenta(string texto, float duracao) =>
        dialogoMarcus.Dizer("Marcus", texto, MinigameDialogue.MarcusPitch, Mathf.Max(2.2f, duracao));

    private void ReiniciarPercepcoes()
    {
        EncerrarPercepcao();
        agendaBocha.Reset();
        medo = faseTremor = ateBatida = tremorMira = 0;
        tremorPendente = false;
        dialogoMarcus.Limpar();
        audioAlucinacao.Stop();
    }

    private void EncerrarPercepcao()
    {
        agendaBocha.Finish();
        focoFigura = tempoFigura = tempoBuraco = 0;
        bochaAlucinada = null;
        caindo = false;
        vista.Room?.MostrarFigura(false);
        vista.Room?.FecharBuraco();
    }

    private void AtualizarPercepcoes(float dt)
    {
        if (pratica) return;

        var inicio = agendaBocha.Tick(dt, fase == Fase.Mira && vista.View == BochaCameraView.Caminhando,
            dialogoMarcus.Ocupada || medo > 0 || tremorPendente);
        if (inicio == Alucinacao.FiguraSentada) vista.Room.MostrarFigura(true);

        if (agendaBocha.Active == Alucinacao.FiguraSentada)
        {
            if (tempoFigura > 0)
            {
                tempoFigura -= dt;
                if (tempoFigura <= 0)
                {
                    EncerrarPercepcao();
                    MarcusComenta("Sumiu... Tonão nem virou a cabeça.", 3.2f);
                }
            }
            else
            {
                focoFigura = vista.View == BochaCameraView.Caminhando && vista.Room.FiguraEmFoco
                    ? focoFigura + dt : 0;
                if (focoFigura >= .42f)
                {
                    tempoFigura = 2.45f;
                    medo = 24;
                    tremorPendente = true;
                    ateBatida = 0;
                    MarcusComenta("Quem... quem tá sentado ali?", 2.45f);
                }
                else if (agendaBocha.Age >= 26) EncerrarPercepcao();
            }
        }

        AtualizarMedo(dt);
    }

    private void AtualizarMedo(float dt)
    {
        tremorMira = 0;
        medo = Mathf.Max(0, medo - dt);
        float intensidade = IntensidadeTremor();
        if (intensidade > 0)
        {
            faseTremor += dt;
            tremorMira = Mathf.Sin(faseTremor * 17) * .012f * intensidade;
        }
        if (medo <= 0) return;

        ateBatida -= dt;
        if (ateBatida > 0) return;

        ateBatida = Mathf.Lerp(.78f, .44f, Mathf.InverseLerp(0, 24, medo));
        if (coracao == null) coracao = MinigamePerceptionAudio.CriarCoracao("SR_Bocha_Coracao_Original");
        TocarAlucinacao(coracao, Mathf.Lerp(.22f, .54f, Mathf.InverseLerp(0, 24, medo)), 1);
    }

    private float IntensidadeTremor()
    {
        if (MenuPausa.ReduzirEfeitosIntensos) return 0;
        return Mathf.Max(Mathf.Clamp01(medo / 3), tremorPendente && arrastando ? .38f : 0);
    }

    private Vector2 TremorMao() =>
        new Vector2(Mathf.Sin(faseTremor * 25) * 3.1f, Mathf.Sin(faseTremor * 32) * 2.2f) * IntensidadeTremor();

    private void PrepararPercepcaoDoLancamento(BochaSide lado, BochaBall bola)
    {
        if (lado != BochaSide.Marcus) return;
        tremorPendente = false;
        if (agendaBocha.Active == Alucinacao.Buraco && bola.valida)
            bochaAlucinada = bola;
    }

    // Called after each normal physics step. The illusion holds the live state at this instant.
    private bool TentarAbrirBuraco()
    {
        if (bochaAlucinada == null) return false;
        if (!bochaAlucinada.valida)
        {
            bochaAlucinada = null;
            return false;
        }

        bool noChao = bochaAlucinada.altura < .018f && Mathf.Abs(bochaAlucinada.vertical) < .8f;
        if (!noChao || (bochaAlucinada.pos.y < 1.6f && sim.IsMoving)) return false;

        caindo = true;
        tempoBuraco = 0;
        acumulador = 0;
        vista.Room.AbrirBuraco(bochaAlucinada);
        PararFoley();
        TocarAlucinacao(madeira, .52f, .62f, PanBocha(bochaAlucinada.pos.x), impacto: true);
        MarcusComenta("O chão abriu... a minha bocha caiu!", 1.8f);
        return true;
    }

    private void AtualizarQueda(float dt)
    {
        float anterior = tempoBuraco;
        tempoBuraco += dt;
        if (anterior < .65f && tempoBuraco >= .65f)
            TocarAlucinacao(saibro, .70f, .56f, PanBocha(bochaAlucinada.pos.x), impacto: true);

        if (tempoBuraco < TempoQueda)
        {
            vista.Room.AnimarQueda(tempoBuraco);
            return;
        }

        EncerrarPercepcao();
        MarcusComenta("Tá no mesmo lugar... Tonão nem viu.", 3.5f);
    }

    private void TocarAlucinacao(AudioClip clip, float volume, float pitch, float pan = 0, bool impacto = false)
    {
        if (clip == null) return;
        audioAlucinacao.pitch = pitch;
        audioAlucinacao.panStereo = pan;
        audioAlucinacao.volume = volume * (impacto
            ? AudioSettingsController.VolumeImpactos
            : AudioSettingsController.VolumeEfeitos);
        audioAlucinacao.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        if (coracao != null) Destroy(coracao);
    }
}
