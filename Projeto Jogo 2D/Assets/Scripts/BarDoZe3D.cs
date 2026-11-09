using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

internal sealed class BarDoZe3D : MonoBehaviour
{
    private static BarDoZe3D atual;
    internal static bool Aberto => atual != null;
    private SinucaRoom3D sala;
    private Canvas canvasBar;
    private TextMeshProUGUI dica;
    private InteracaoCapitulo1 interacao;
    private Camera cameraRua;
    private Chuva chuvaDaRua;
    private int mascaraRua;
    private CursorLockMode cursorAnterior;
    private bool cursorVisivel, jogando;
    private int frameCaptura;
    private Vector3 posicaoAntesDaPartida;
    private Vector2 olharAntesDaPartida;
    private Vector2 velocidade;

    internal static void Abrir(bool animar, Vector2 foco = default)
    {
        if (Aberto) return;
        var sala = SinucaMinigameUI.Instance.PrepararSala();
        var bar = new GameObject("ExploracaoBarDoZe3D").AddComponent<BarDoZe3D>();
        atual = bar;
        bar.Inicializar(sala, animar, foco);
    }

    private void Inicializar(SinucaRoom3D room, bool animar, Vector2 foco)
    {
        sala = room;
        cameraRua = Camera.main;
        if (cameraRua != null) mascaraRua = cameraRua.cullingMask;
        cursorAnterior = Cursor.lockState;
        cursorVisivel = Cursor.visible;
        InteractionPrompt.Instance.SetTarget(null);

        canvasBar = UIFactory.CreateCanvas(transform, "BarDoZeCanvas", 6400);
        var imagem = new GameObject("VistaDoSalao", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
        imagem.transform.SetParent(canvasBar.transform, false);
        UIFactory.Stretch(imagem.rectTransform);
        imagem.raycastTarget = false;
        imagem.texture = sala.Target;
        dica = UIFactory.CreateLabel(canvasBar.transform, "InteracaoNoBar", 13f);
        dica.raycastTarget = false;
        dica.alignment = TextAlignmentOptions.Center;
        dica.rectTransform.anchorMin = dica.rectTransform.anchorMax = new Vector2(.5f, .5f);
        dica.rectTransform.anchoredPosition = new Vector2(0f, -28f);
        dica.rectTransform.sizeDelta = new Vector2(460f, 42f);
        dica.text = "";
        interacao = gameObject.AddComponent<InteracaoCapitulo1>();
        interacao.enabled = false;

        sala.SetViewportAspect(Screen.width / (float)Mathf.Max(1, Screen.height));
        sala.IniciarExploracaoDoBar(animar, foco);
        sala.ConfigurarSomDoSalao(transform);
        sala.SetActive(true);
        if (cameraRua != null) cameraRua.cullingMask = 0;
        chuvaDaRua = FindAnyObjectByType<Chuva>();
        if (chuvaDaRua != null) chuvaDaRua.DefinirModoInterior(true, animar ? 0f : 1f);
        GameState.Marcar(Flags.EntrouNoBarDoZe);
    }

    internal static void AnimarEntrada(float segundos)
    {
        if (atual == null) return;
        float fechamento = atual.sala.AnimarEntradaDoBar(segundos);
        if (atual.chuvaDaRua != null) atual.chuvaDaRua.DefinirOclusaoPortaBar(fechamento);
    }

    internal static void PrepararSaida() => atual?.sala.PrepararSaidaDoBar();

    internal static void AnimarSaida(float segundos)
    {
        if (atual == null) return;
        float abertura = atual.sala.AnimarSaidaDoBar(segundos);
        if (atual.chuvaDaRua != null) atual.chuvaDaRua.DefinirOclusaoPortaBar(1f - abertura);
    }

    internal static Vector2 FocoDaPorta => atual == null ? Vector2.one * .5f : atual.sala.FocoDaPortaDoBar;

    internal static void GuardarLugarParaPartida()
    {
        if (atual == null) return;
        atual.posicaoAntesDaPartida = atual.sala.PosicaoExploracao;
        atual.olharAntesDaPartida = atual.sala.OlharExploracao;
        atual.jogando = true;
        atual.dica.text = "";
        atual.CapturarCursor(false);
    }

    internal static void VoltarDaPartida()
    {
        if (atual == null || !atual.jogando) return;
        atual.jogando = false;
        atual.sala.VoltarAExplorar(atual.posicaoAntesDaPartida, atual.olharAntesDaPartida);
    }

    internal static void Fechar()
    {
        if (atual == null) return;
        atual.LiberarSala();
        Destroy(atual.gameObject);
        atual = null;
    }

    private void LiberarSala()
    {
        if (cameraRua != null) cameraRua.cullingMask = mascaraRua;
        if (chuvaDaRua != null) chuvaDaRua.DefinirModoInterior(false);
        if (sala != null && sala.root != null) sala.SetActive(false);
        if (canvasBar != null) canvasBar.enabled = false;
        Cursor.lockState = cursorAnterior;
        Cursor.visible = cursorVisivel;
    }

    private void OnDestroy()
    {
        if (atual != this) return;
        LiberarSala();
        atual = null;
    }

    private void Update()
    {
        if (SinucaMinigameUI.IsShowing)
        {
            velocidade = Vector2.zero;
            return;
        }
        sala.SetViewportAspect(Screen.width / (float)Mathf.Max(1, Screen.height));
        if (SceneFader.IsTransitioning || MenuPausa.Aberto || MapaInterativoUI.IsShowing ||
            SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen || !Application.isFocused || Time.timeScale == 0f)
        {
            velocidade = Vector2.zero;
            CapturarCursor(false);
            dica.text = "";
            return;
        }

        var teclado = Keyboard.current;
        if (GameMessageUI.IsShowing)
        {
            velocidade = Vector2.zero;
            CapturarCursor(false);
            dica.text = "";
            if (teclado != null && teclado.eKey.wasPressedThisFrame) GameMessageUI.Instance.Avancar();
            return;
        }

        if (SantaRitaFalasEntrada.Interagindo)
        {
            velocidade = Vector2.zero;
            CapturarCursor(false);
            dica.text = "";
            if (teclado != null && teclado.eKey.wasPressedThisFrame) SantaRitaFalasEntrada.Avancar();
            return;
        }

        CapturarCursor(true);
        if (teclado == null)
        {
            velocidade = Vector2.zero;
            return;
        }

        const float caminhada = 1.65f;
        float dt = Mathf.Min(Time.deltaTime, .05f);
        float maximo = caminhada;
        if (PlayerController.ShiftPressionado(teclado)) maximo *= PlayerController.MultiplicadorCorrida;
        velocidade = PlayerController.AjustarVelocidade(velocidade, PlayerController.LerDirecao(teclado) * maximo, dt);
        var mouse = Mouse.current;
        var olhar = mouse != null && frameCaptura != Time.frameCount ? mouse.delta.ReadValue() : Vector2.zero;
        sala.MoverJogador(velocidade, olhar, dt);
        sala.AtualizarExploracaoDoBar();

        bool podeSair = sala.PodeSairDoBar();
        bool podeTocar = !podeSair && sala.PodeTocarCampainha();
        Bounds enquadramento = default;
        var alvo = podeSair || podeTocar ? null : sala.InteracaoEmFoco(out enquadramento);
        if (alvo.HasValue) interacao.Configurar(alvo.Value);
        dica.text = podeSair ? "·\n[E] Sair para a Rua Norte" :
            podeTocar ? "·\n[E] Tocar campainha do balcão" :
            alvo.HasValue ? "·\n[E] " + interacao.Prompt : "·";
        if (!teclado.eKey.wasPressedThisFrame && (mouse == null || !mouse.leftButton.wasPressedThisFrame || frameCaptura == Time.frameCount))
            return;
        if (podeSair)
        {
            CapturarCursor(false);
            Capitulo1Director.Instance.SairDoBar3D();
        }
        else if (podeTocar)
        {
            sala.TocarCampainha();
        }
        else if (alvo.HasValue)
        {
            CapturarCursor(false);
            interacao.DefinirFocoInterior(sala.camera, enquadramento);
            interacao.Interact();
        }
    }

    private void CapturarCursor(bool capturar)
    {
        var modo = capturar ? CursorLockMode.Locked : CursorLockMode.None;
        if (Cursor.lockState != modo)
        {
            Cursor.lockState = modo;
            if (capturar) frameCaptura = Time.frameCount;
        }
        Cursor.visible = !capturar;
    }
}
