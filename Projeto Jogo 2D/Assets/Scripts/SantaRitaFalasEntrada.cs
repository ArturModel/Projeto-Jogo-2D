using System;
using UnityEngine;

internal sealed class SantaRitaFalasEntrada : MonoBehaviour
{
    private static SantaRitaFalasEntrada atual;
    private MinigameDialogue legenda;
    private DialogoLinha[] falas;
    private Capitulo1Director.ZonaId zonaDaFala;
    private const float TomFrentista = .96f;
    private const float TomMotorista = .62f;
    private int proxima;
    private Action aoTerminar;
    private bool interagindo;

    private static bool NaEntrada => atual != null &&
        Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona1_Rodovia;
    private static bool OutraInterface => GameMessageUI.IsShowing || MenuPausa.Aberto ||
        MapaInterativoUI.IsShowing || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen ||
        PayphoneCheckpoint.IsSaveUIOpen || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing;

    private static bool ZonaComFaixa => Capitulo1Director.ZonaAtual is
        Capitulo1Director.ZonaId.Zona2_Posto or Capitulo1Director.ZonaId.Interior_Posto or
        Capitulo1Director.ZonaId.Zona3_Encruzilhada or Capitulo1Director.ZonaId.Interior_Mercadinho or
        Capitulo1Director.ZonaId.Zona4_Bairro or Capitulo1Director.ZonaId.Interior_CasaDoAvo;

    public static bool Interagindo => atual != null && atual.interagindo &&
        atual.zonaDaFala == Capitulo1Director.ZonaAtual;
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
        atual.Iniciar(aoFechar, Array.ConvertAll(falas, fala => new DialogoLinha("Marcus", fala)));
    }

    public static bool TentarDizerNaZona(Action aoFechar, params DialogoLinha[] falas)
    {
        if (!ZonaComFaixa || atual == null) return false;
        atual.Iniciar(aoFechar, falas);
        return true;
    }

    private void Iniciar(Action aoFechar, DialogoLinha[] linhas)
    {
        zonaDaFala = Capitulo1Director.ZonaAtual;
        falas = linhas;
        proxima = 0;
        aoTerminar = aoFechar;
        interagindo = true;
        ProximaFala();
    }

    public static void Avancar()
    {
        if (Interagindo && !atual.legenda.RevelarRestante())
            atual.ProximaFala();
    }

    private void Update()
    {
        bool foraDaZona = interagindo ? !Interagindo : !NaEntrada;
        if (foraDaZona || SceneFader.IsTransitioning)
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
        if (proxima >= falas.Length)
        {
            Encerrar();
            return;
        }

        var fala = falas[proxima++];
        var (locutor, tom) = fala.falante switch
        {
            "FRENTISTA" => ("Frentista", TomFrentista),
            "MOTORISTA" => ("Motorista", TomMotorista),
            "ATENDENTE" => ("Atendente", 1.08f),
            "DONA LURDES" => ("Dona Lurdes", 1.01f),
            "VIZINHO" => ("Vizinho", .65f),
            "SEU RAMIRO" => ("Seu Ramiro", .65f),
            "MARCUS" => ("Marcus", MinigameDialogue.MarcusPitch),
            _ => (fala.falante, MinigameDialogue.MarcusPitch)
        };
        legenda.Dizer(locutor, fala.texto, tom);
    }

    private void Encerrar()
    {
        bool naZonaDaFala = Interagindo && !SceneFader.IsTransitioning;
        var callback = aoTerminar;
        legenda.Limpar();
        interagindo = false;
        aoTerminar = null;
        falas = null;
        if (naZonaDaFala) callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (atual == this) atual = null;
    }
}
