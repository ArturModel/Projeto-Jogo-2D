using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
internal sealed class SantaRitaFalasEntrada : MonoBehaviour
{
    private static SantaRitaFalasEntrada atual;
    private MinigameDialogue legenda;
    private SantaRitaInspecao inspecao;
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

    private static bool Suspensa => OutraInterface || !Application.isFocused || Time.timeScale == 0f;
    public static bool Interagindo => atual != null &&
        (atual.inspecao.Ativa || (atual.interagindo && atual.zonaDaFala == Capitulo1Director.ZonaAtual));
    public static float Zoom => NaEntrada && !OutraInterface && atual.legenda.Ocupada
        ? (Interagindo ? .06f : .025f) : 0f;

    internal static bool UsaCamera(Camera vista) => atual != null && atual.inspecao.UsaCamera(vista);

    internal static bool Enquadramento2D(Camera vista, out Bounds objeto, out float intensidade)
    {
        objeto = default;
        intensidade = 0f;
        return atual != null && atual.inspecao.Enquadramento2D(vista, out objeto, out intensidade);
    }

    internal static bool Examinar(Camera vista, Bounds objeto, string titulo, string corpo,
        string pensamento = null, string etiqueta = null, Action terminou = null,
        string comando = null, Action acao = null)
    {
        if (atual == null || vista == null) return false;
        atual.legenda.Limpar();
        atual.zonaDaFala = Capitulo1Director.ZonaAtual;
        atual.inspecao.Abrir(vista, objeto, titulo, corpo, pensamento, etiqueta, terminou, comando, acao);
        return true;
    }

    private void Awake()
    {
        atual = this;
        var canvas = UIFactory.CreateCanvas(transform, "FalasDaEntrada", 8700);
        legenda = new MinigameDialogue(canvas.transform, "LegendaDaExploracao");
        inspecao = new SantaRitaInspecao(canvas);
    }

    public static void DizerEspontanea(string fala, float leitura)
    {
        if (atual != null && !Interagindo && !OutraInterface && !SceneFader.IsTransitioning && !atual.legenda.Ocupada)
        {
            atual.zonaDaFala = Capitulo1Director.ZonaAtual;
            atual.legenda.Dizer("Marcus", fala, MinigameDialogue.MarcusPitch, leitura);
        }
    }

    public static void DizerInteracao(Action aoFechar, params string[] falas)
    {
        if (!NaEntrada) return;
        atual.Iniciar(aoFechar, Array.ConvertAll(falas, fala => new DialogoLinha("Marcus", fala)));
    }

    public static bool TentarDizerNaZona(Action aoFechar, params DialogoLinha[] falas)
    {
        if (atual == null || falas.Length == 0) return false;
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
        if (!Interagindo || Suspensa) return;
        if (atual.inspecao.Ativa) atual.inspecao.Avancar();
        else if (!atual.legenda.RevelarRestante()) atual.ProximaFala();
    }

    private void Update()
    {
        if (inspecao == null) return;
        if (zonaDaFala != Capitulo1Director.ZonaAtual || SceneFader.IsTransitioning)
        {
            inspecao.Cancelar();
            if (interagindo || legenda.Ocupada) Encerrar();
            return;
        }

        if (Suspensa)
        {
            legenda.Suspender();
            return;
        }

        if (inspecao.Ativa)
        {
            if (Keyboard.current?.rKey.wasPressedThisFrame == true) inspecao.Confirmar();
            inspecao.Atualizar(Time.deltaTime);
            return;
        }

        legenda.Atualizar(Time.deltaTime);
        if (interagindo && !legenda.Ocupada) ProximaFala();
    }

    private void LateUpdate() => inspecao?.Compor(!Suspensa && !SceneFader.IsTransitioning);

    private void ProximaFala()
    {
        if (proxima >= falas.Length)
        {
            Encerrar();
            return;
        }

        var fala = falas[proxima++];
        var (locutor, tom) = fala.falante.ToUpperInvariant() switch
        {
            "FRENTISTA" => ("Frentista", TomFrentista),
            "MOTORISTA" => ("Motorista", TomMotorista),
            "ATENDENTE" => ("Atendente", 1.08f),
            "DONA LURDES" => ("Dona Lurdes", 1.01f),
            "VIZINHO" => ("Vizinho", .65f),
            "SEU RAMIRO" => ("Seu Ramiro", .65f),
            "ZÉ" => ("Zé", MinigameDialogue.ZePitch),
            "FREGUÊS" => ("Freguês", .86f),
            "MARCUS" => ("Marcus", MinigameDialogue.MarcusPitch),
            "PADRE" or "PADRE BENTO" or "BENTO" => ("Padre Bento", .68f),
            "CLARICE" => ("Clarice", 1.12f),
            "MILA" => ("Mila", 1.18f),
            "CELINA" => ("Celina", 1.04f),
            "DÁRIO" or "DARIO" => ("Dário", .84f),
            "NAIR" => ("Nair", 1.06f),
            "TONÃO" or "TONAO" => ("Tonão", .64f),
            "PASSAGEIRA" => ("Passageira", 1.14f),
            "CHICO" => ("Chico", .76f),
            "AMARO" => ("Amaro", .72f),
            _ => (fala.falante, MinigameDialogue.MarcusPitch)
        };
        legenda.Dizer(locutor, fala.texto, tom);
    }

    private void Encerrar()
    {
        bool naZonaDaFala = interagindo && zonaDaFala == Capitulo1Director.ZonaAtual && !SceneFader.IsTransitioning;
        var callback = aoTerminar;
        legenda.Limpar();
        interagindo = false;
        aoTerminar = null;
        falas = null;
        if (naZonaDaFala) callback?.Invoke();
    }

    private void OnDestroy()
    {
        inspecao?.Cancelar();
        if (atual == this) atual = null;
    }
}
