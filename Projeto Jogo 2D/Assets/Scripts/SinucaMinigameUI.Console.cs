using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventoSinuca = SinucaPerceptionSchedule.Event;

public partial class SinucaMinigameUI
{
    private static readonly (string comando, string nome, EventoSinuca evento)[] ComandosSinuca =
    {
        ("event_apagao", "Apagão", EventoSinuca.Blackout),
        ("event_ze", "Zé assombrado", EventoSinuca.HauntedZe),
        ("event_batidas", "Batidas sob a mesa", EventoSinuca.UnderTable),
        ("event_bola_fantasma", "Bola fantasma", EventoSinuca.FalseBall),
        ("event_janela", "Batidas na janela", EventoSinuca.WindowKnock)
    };

    private GameObject consoleSinuca;
    private TMP_InputField entradaConsoleSinuca;
    private TextMeshProUGUI retornoConsoleSinuca;
    private EventoSinuca eventoConsolePendente;
    private bool ConsoleSinucaAberto => consoleSinuca != null && consoleSinuca.activeSelf;

    private bool AtualizarConsoleSinuca()
    {
        if (!Debug.isDebugBuild || !IsShowing || modoPratica) return false;
        var teclado = Keyboard.current;
        if (teclado == null) return false;
        bool aspas = teclado.quoteKey.wasPressedThisFrame || teclado.backquoteKey.wasPressedThisFrame;

        if (ConsoleSinucaAberto)
        {
            if (!aspas && !teclado.escapeKey.wasPressedThisFrame) return false;
            FecharConsoleSinuca();
            AtualizarCapturaCursor();
            return true;
        }

        if (!aspas || estado == EstadoPartida.Inativo || estado == EstadoPartida.FimDeMao ||
            EntradaMesaBloqueada(true)) return false;

        CancelarCargaAoSuspender();
        if (consoleSinuca == null) ConstruirConsoleSinuca();
        consoleSinuca.SetActive(true);
        AtualizarCapturaCursor();
        entradaConsoleSinuca.SetTextWithoutNotify("");
        entradaConsoleSinuca.Select();
        entradaConsoleSinuca.ActivateInputField();
        return true;
    }

    private void ConstruirConsoleSinuca()
    {
        consoleSinuca = CriarPainelHUD(raiz.transform, "ConsoleEventosSinuca", Vector2.zero, Vector2.zero);
        UIFactory.Stretch(consoleSinuca.GetComponent<RectTransform>());
        consoleSinuca.GetComponent<Image>().color = new Color32(0, 0, 0, 104);

        var painel = CriarPainelHUD(consoleSinuca.transform, "Terminal", new Vector2(0, 69), new Vector2(492, 186));
        painel.GetComponent<Image>().color = new Color32(12, 20, 17, 250);
        var titulo = CriarTexto("Titulo", painel.transform, "CONSOLE DA SINUCA", 12, Color.white, TextAlignmentOptions.Left);
        Posicionar(titulo.rectTransform, new Vector2(0, 75), new Vector2(464, 16));
        for (int i = 0; i < ComandosSinuca.Length; i++)
        {
            var opcao = ComandosSinuca[i];
            var posicao = new Vector2(i % 2 == 0 ? -118 : 118, 47 - i / 2 * 28);
            var botao = CriarBotaoHUD(painel.transform, opcao.comando, opcao.nome + " · " + opcao.comando,
                posicao, new Vector2(228, 24), new Color32(33, 48, 40, 255), out var rotulo);
            rotulo.color = new Color32(205, 230, 214, 255);
            botao.onClick.AddListener(() => SolicitarEventoConsoleSinuca(opcao.evento));
        }

        var campo = CriarPainelHUD(painel.transform, "CampoComando", new Vector2(0, -43), new Vector2(464, 24));
        campo.GetComponent<Image>().color = new Color32(33, 48, 40, 255);
        var area = new GameObject("AreaDeTexto", typeof(RectTransform), typeof(RectMask2D));
        area.transform.SetParent(campo.transform, false);
        var rect = area.GetComponent<RectTransform>();
        UIFactory.Stretch(rect);
        rect.offsetMin = new Vector2(8, 1);
        rect.offsetMax = new Vector2(-8, -1);
        var texto = CriarTexto("Comando", rect, "", 13, Color.white, TextAlignmentOptions.MidlineLeft);
        UIFactory.Stretch(texto.rectTransform);
        texto.raycastTarget = false;
        var dica = CriarTexto("Dica", rect, "Digite um comando…", 13, new Color32(171, 193, 178, 255), TextAlignmentOptions.MidlineLeft);
        UIFactory.Stretch(dica.rectTransform);
        dica.raycastTarget = false;

        entradaConsoleSinuca = campo.AddComponent<TMP_InputField>();
        entradaConsoleSinuca.targetGraphic = campo.GetComponent<Image>();
        entradaConsoleSinuca.textViewport = rect;
        entradaConsoleSinuca.textComponent = texto;
        entradaConsoleSinuca.placeholder = dica;
        entradaConsoleSinuca.characterLimit = 64;
        entradaConsoleSinuca.richText = false;
        entradaConsoleSinuca.onSubmit.AddListener(ExecutarComandoConsoleSinuca);

        retornoConsoleSinuca = CriarTexto("Retorno", painel.transform, "Enter executa · Esc fecha",
            10, new Color32(180, 222, 198, 255), TextAlignmentOptions.Left);
        Posicionar(retornoConsoleSinuca.rectTransform, new Vector2(0, -73), new Vector2(464, 20));
    }

    private void ExecutarComandoConsoleSinuca(string texto)
    {
        if (!ConsoleSinucaAberto || !Application.isFocused) return;
        string comando = texto.Trim().ToLowerInvariant().Replace('ã', 'a').Replace('é', 'e');
        foreach (var opcao in ComandosSinuca)
        {
            if (opcao.comando != comando) continue;
            SolicitarEventoConsoleSinuca(opcao.evento);
            return;
        }

        retornoConsoleSinuca.text = "Use um comando ou clique em uma das opções.";
        entradaConsoleSinuca.ActivateInputField();
    }

    private void SolicitarEventoConsoleSinuca(EventoSinuca evento)
    {
        if (!ConsoleSinucaAberto || !Application.isFocused) return;
        eventoConsolePendente = evento;
        retornoConsoleSinuca.text = "Comando em fila. O evento começa quando Marcus puder jogar.";
        interfaceLivre = false;
        FecharConsoleSinuca();
        AtualizarCapturaCursor();
    }

    private EventoSinuca ConsumirEventoConsoleSinuca()
    {
        if (eventoConsolePendente == EventoSinuca.None || estado != EstadoPartida.MirandoJogador || !turnoDeMarcus)
            return EventoSinuca.None;

        var evento = eventoConsolePendente;
        eventoConsolePendente = EventoSinuca.None;
        EncerrarPercepcaoDaMao();
        LimparFalaZe();
        if (!EmPe) LevantarDaMesa();

        retornoConsoleSinuca.text = evento == EventoSinuca.HauntedZe
            ? "Zé armado. Olhe para longe dele e depois olhe de volta."
            : "Comando executado.";
        return agendaPercepcoes.StartForced(evento);
    }

    private void FecharConsoleSinuca()
    {
        if (!ConsoleSinucaAberto) return;
        entradaConsoleSinuca.DeactivateInputField();
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == entradaConsoleSinuca.gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        consoleSinuca.SetActive(false);
    }

    private void ReiniciarConsoleSinuca()
    {
        eventoConsolePendente = EventoSinuca.None;
        FecharConsoleSinuca();
        if (retornoConsoleSinuca != null) retornoConsoleSinuca.text = "Enter executa · Esc fecha";
    }
}
