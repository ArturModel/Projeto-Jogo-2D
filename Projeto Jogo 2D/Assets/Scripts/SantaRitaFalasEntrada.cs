using System;
using UnityEngine;

internal sealed class SantaRitaFalasEntrada : MonoBehaviour
{
    private static SantaRitaFalasEntrada atual;
    private MinigameDialogue legenda;
    private string[] falas;
    private int proxima;
    private Action aoTerminar;
    private bool interagindo;

    private static bool NaEntrada => atual != null &&
        Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona1_Rodovia;
    private static bool OutraInterface => GameMessageUI.IsShowing || MenuPausa.Aberto ||
        MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen ||
        PayphoneCheckpoint.IsSaveUIOpen || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing;

    public static bool Interagindo => NaEntrada && atual.interagindo;
    public static float Zoom => NaEntrada && !OutraInterface && atual.legenda.Ocupada
        ? (Interagindo ? .06f : .025f) : 0f;

    private void Awake()
    {
        atual = this;
        var canvas = UIFactory.CreateCanvas(transform, "FalasDaEntrada", 8700);
        legenda = new MinigameDialogue(canvas.transform, "LegendaMarcus");
    }

    public static void DizerEspontanea(string fala, float leitura)
    {
        if (NaEntrada && !OutraInterface && !SceneFader.IsTransitioning && !atual.legenda.Ocupada)
            atual.legenda.Dizer("Marcus", fala, MinigameDialogue.MarcusPitch, leitura);
    }

    public static void DizerInteracao(Action aoFechar, params string[] falas)
    {
        if (!NaEntrada) return;
        atual.falas = falas;
        atual.proxima = 0;
        atual.aoTerminar = aoFechar;
        atual.interagindo = true;
        atual.ProximaFala();
    }

    public static void Avancar()
    {
        if (Interagindo && !atual.legenda.RevelarRestante())
            atual.ProximaFala();
    }

    private void Update()
    {
        if (!NaEntrada || SceneFader.IsTransitioning)
        {
            if (interagindo || legenda.Ocupada) Encerrar();
            return;
        }

        if (OutraInterface || !Application.isFocused || Time.timeScale == 0f)
        {
            legenda.Suspender();
            return;
        }

        legenda.Atualizar(Time.deltaTime);
        if (interagindo && !legenda.Ocupada) ProximaFala();
    }

    private void ProximaFala()
    {
        if (proxima < falas.Length)
            legenda.Dizer("Marcus", falas[proxima++], MinigameDialogue.MarcusPitch);
        else
            Encerrar();
    }

    private void Encerrar()
    {
        var callback = aoTerminar;
        legenda.Limpar();
        interagindo = false;
        aoTerminar = null;
        falas = null;
        if (NaEntrada && !SceneFader.IsTransitioning) callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (atual == this) atual = null;
    }
}
