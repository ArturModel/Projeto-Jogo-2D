using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Boss 1 — Minigame de Sinuca contra Zé (Melhor de 3 Mãos).
/// Implementa a especificação R1: "A Bola Marcada / Essa Fica na Mesa".
/// - Branca + 7 bolas numeradas compartilhadas (1..7).
/// - 1 ponto por bola legalmente encaçapada; 4 pontos vencem a mão.
/// - Vence a série quem ganhar 2 mãos (2-0 ou 2-1).
/// - Contabilidade estrita: pontosMarcus + pontosZe + bolasEmJogo == 7.
/// - Faltas apuradas somente após repouso total: branca encaçapada/fora, sem primeiro contato,
///   nenhuma bola atinge tabela/caçapa após contato, ou encaçapar a Bola Marcada.
/// - Em falta: zera apenas os pontos da tacada, repõe bolas caídas na tacada sem sobreposição,
///   concede bola na mão ao oponente e passa a vez.
/// </summary>
public class SinucaMinigameUI : MonoBehaviour
{
    public static SinucaMinigameUI Instance
    {
        get
        {
            EnsureInstance();
            return instance;
        }
    }
    private static SinucaMinigameUI instance;
    public static bool IsShowing => instance != null && instance.raiz != null && instance.raiz.activeSelf;

    private const float TableWidth = 420f;
    private const float TableHeight = 210f;
    private const float BallRadius = 8.5f;
    private const float PocketRadius = 15.5f;
    private const float CushionRestitution = 0.80f;
    private const float BallFriction = 95f;
    private const float StopSpeedSq = 4f;

    private enum EstadoPartida
    {
        Inativo,
        TutorialRegras,
        PosicionandoBranca,
        MirandoJogador,
        CarregandoForca,
        TurnoZePensando,
        BolasEmMovimento,
        ResolvendoTacada,
        FimDeMao,
        FimDeSerieDerrota,
        FimDeSerieVitoria
    }

    private class BolaMesa
    {
        public int numero;
        public Vector2 pos;
        public Vector2 vel;
        public bool ativa;
        public bool caiuNestaTacada;
        public RectTransform rect;
        public Image img;
        public TextMeshProUGUI label;
        public GameObject marcaVisual;
    }

    private GameObject raiz;
    private RectTransform mesaRect;
    private RectTransform linhaMiraRect;
    private RectTransform tacoRect;

    private TextMeshProUGUI txtPlacarSerie;
    private TextMeshProUGUI txtPlacarMao;
    private TextMeshProUGUI txtTurnoStatus;
    private TextMeshProUGUI txtRegraBolaMarcada;
    private TextMeshProUGUI txtPercepcaoSubtitulo;
    private TextMeshProUGUI txtControlesRodape;
    private Image barraForcaFill;
    private Image overlayPercepcao;

    private GameObject painelModal;
    private TextMeshProUGUI txtModalTitulo;
    private TextMeshProUGUI txtModalCorpo;
    private Button btnModalPrimario;
    private TextMeshProUGUI txtBtnPrimario;
    private Button btnModalSecundario;
    private TextMeshProUGUI txtBtnSecundario;

    private readonly List<BolaMesa> bolas = new List<BolaMesa>();
    private readonly Vector2[] cacapas = new Vector2[6];

    private EstadoPartida estado = EstadoPartida.Inativo;
    public static int MaoEmCurso => IsShowing ? instance.maoAtual : 0;

    private int maoAtual = 1;
    private int maosVencidasMarcus = 0;
    private int maosVencidasZe = 0;

    private int pontosMarcus = 0;
    private int pontosZe = 0;
    private bool turnoDeMarcus = true;
    private bool brancaNaMao = false;

    private int numeroBolaMarcada = -1;
    private bool demonstracaoMao2Feita = false;
    private int contadorTurnosMao = 0;

    private float anguloMiraRad = 0f;
    private float forcaNormalizada = 0f;
    private bool carregandoDirectionUp = true;

    private bool houvePrimeiroContatoBolaObjeto = false;
    private bool houveToqueTabelaOuCacapa = false;
    private bool brancaCaiuNestaTacada = false;
    private bool bolaMarcadaCaiuNestaTacada = false;
    private readonly List<int> bolasObjetoCaidasNaTacada = new List<int>();

    private Action onVictoryCallback;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatic()
    {
        instance = null;
    }

    public static void IniciarConfronto(Action aoVencerSerie)
    {
        EnsureInstance();
        instance.ComecarNovaSerie(aoVencerSerie);
    }

    public void IniciarDuelo(Action aoVencer, Action aoPerder = null)
    {
        if (raiz == null)
        {
            InicializarCacapas();
            ConstruirUI();
        }
        ComecarNovaSerie(aoVencer);
    }

    public void Fechar()
    {
        StopAllCoroutines();
        onVictoryCallback = null;
        if (painelModal != null) painelModal.SetActive(false);
        if (raiz != null) raiz.SetActive(false);
        estado = EstadoPartida.Inativo;
    }

    public void CancelarCargaAoSuspender()
    {
        if (estado != EstadoPartida.CarregandoForca) return;
        forcaNormalizada = 0;
        if (barraForcaFill != null) barraForcaFill.fillAmount = 0;
        estado = EstadoPartida.MirandoJogador;
    }

    private void OnApplicationFocus(bool focus)
    { if (!focus) CancelarCargaAoSuspender(); }

    public static void EnsureInstance()
    {
        if (instance != null) return;
        var go = new GameObject("SinucaMinigameUI");
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(go);
        }
        instance = go.AddComponent<SinucaMinigameUI>();
        instance.InicializarCacapas();
        instance.ConstruirUI();
    }

    private void InicializarCacapas()
    {
        float hw = TableWidth * 0.5f - 4f;
        float hh = TableHeight * 0.5f - 4f;
        cacapas[0] = new Vector2(-hw, hh);
        cacapas[1] = new Vector2(0f, hh + 2f);
        cacapas[2] = new Vector2(hw, hh);
        cacapas[3] = new Vector2(-hw, -hh);
        cacapas[4] = new Vector2(0f, -hh - 2f);
        cacapas[5] = new Vector2(hw, -hh);
    }

    private void ConstruirUI()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "SinucaCanvas", 8800);
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        raiz = new GameObject("RaizSinuca", typeof(RectTransform), typeof(Image));
        raiz.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(raiz.GetComponent<RectTransform>());
        raiz.GetComponent<Image>().color = new Color(0.03f, 0.04f, 0.05f, 0.96f);

        var ovGo = new GameObject("OverlayPercepcao", typeof(RectTransform), typeof(Image));
        ovGo.transform.SetParent(raiz.transform, false);
        UIFactory.Stretch(ovGo.GetComponent<RectTransform>());
        overlayPercepcao = ovGo.GetComponent<Image>();
        overlayPercepcao.color = new Color(0.12f, 0.22f, 0.32f, 0f);
        overlayPercepcao.raycastTarget = false;

        txtPlacarSerie = CriarTexto("PlacarSerie", raiz.transform, "", 20, new Color(0.95f, 0.84f, 0.48f), TextAlignmentOptions.Center);
        Posicionar(txtPlacarSerie.rectTransform, new Vector2(0f, 158f), new Vector2(460f, 24f));

        txtPlacarMao = CriarTexto("PlacarMao", raiz.transform, "", 16, new Color(0.90f, 0.88f, 0.82f), TextAlignmentOptions.Center);
        Posicionar(txtPlacarMao.rectTransform, new Vector2(0f, 136f), new Vector2(460f, 22f));

        txtRegraBolaMarcada = CriarTexto("RegraBolaMarcada", raiz.transform, "", 15, new Color(0.98f, 0.42f, 0.36f), TextAlignmentOptions.Center);
        Posicionar(txtRegraBolaMarcada.rectTransform, new Vector2(0f, 116f), new Vector2(540f, 20f));

        var btnRegrasGo = new GameObject("BtnRegras", typeof(RectTransform), typeof(Image), typeof(Button));
        btnRegrasGo.transform.SetParent(raiz.transform, false);
        Posicionar(btnRegrasGo.GetComponent<RectTransform>(), new Vector2(265f, 154f), new Vector2(88f, 24f));
        btnRegrasGo.GetComponent<Image>().color = new Color(0.20f, 0.17f, 0.14f, 0.95f);
        var lblRegras = CriarTexto("Lbl", btnRegrasGo.transform, "[?] REGRAS", 14, new Color(0.95f, 0.84f, 0.52f), TextAlignmentOptions.Center);
        UIFactory.Stretch(lblRegras.rectTransform);
        btnRegrasGo.GetComponent<Button>().onClick.AddListener(AbrirModalRegrasDuranteJogo);

        var bordaMadeira = new GameObject("BordaMadeira", typeof(RectTransform), typeof(Image));
        bordaMadeira.transform.SetParent(raiz.transform, false);
        Posicionar(bordaMadeira.GetComponent<RectTransform>(), new Vector2(0f, -4f), new Vector2(TableWidth + 28f, TableHeight + 28f));
        bordaMadeira.GetComponent<Image>().color = new Color(0.28f, 0.15f, 0.08f, 1f);

        var feltroGo = new GameObject("MesaFeltro", typeof(RectTransform), typeof(Image));
        feltroGo.transform.SetParent(raiz.transform, false);
        mesaRect = feltroGo.GetComponent<RectTransform>();
        Posicionar(mesaRect, new Vector2(0f, -4f), new Vector2(TableWidth, TableHeight));
        feltroGo.GetComponent<Image>().color = new Color(0.09f, 0.32f, 0.20f, 1f);

        Sprite pocketSprite = CriarSpriteCirculo(32, new Color(0.04f, 0.04f, 0.05f, 1f), Color.clear);
        for (int i = 0; i < cacapas.Length; i++)
        {
            var pGo = new GameObject("Cacapa_" + i, typeof(RectTransform), typeof(Image));
            pGo.transform.SetParent(mesaRect, false);
            Posicionar(pGo.GetComponent<RectTransform>(), cacapas[i], new Vector2(PocketRadius * 2f, PocketRadius * 2f));
            var pImg = pGo.GetComponent<Image>();
            pImg.sprite = pocketSprite;
            pImg.raycastTarget = false;
        }

        var miraGo = new GameObject("LinhaMira", typeof(RectTransform), typeof(Image));
        miraGo.transform.SetParent(mesaRect, false);
        linhaMiraRect = miraGo.GetComponent<RectTransform>();
        linhaMiraRect.pivot = new Vector2(0f, 0.5f);
        linhaMiraRect.sizeDelta = new Vector2(95f, 2f);
        var miraImg = miraGo.GetComponent<Image>();
        miraImg.color = new Color(1f, 0.92f, 0.55f, 0.65f);
        miraImg.raycastTarget = false;

        var tacoGo = new GameObject("Taco", typeof(RectTransform), typeof(Image));
        tacoGo.transform.SetParent(mesaRect, false);
        tacoRect = tacoGo.GetComponent<RectTransform>();
        tacoRect.pivot = new Vector2(1f, 0.5f);
        tacoRect.sizeDelta = new Vector2(110f, 4f);
        var tacoImg = tacoGo.GetComponent<Image>();
        tacoImg.color = new Color(0.76f, 0.54f, 0.30f, 0.95f);
        tacoImg.raycastTarget = false;

        CriarBolasUI();

        var barraBg = new GameObject("BarraForcaBg", typeof(RectTransform), typeof(Image));
        barraBg.transform.SetParent(raiz.transform, false);
        Posicionar(barraBg.GetComponent<RectTransform>(), new Vector2(-248f, -4f), new Vector2(16f, 180f));
        barraBg.GetComponent<Image>().color = new Color(0.14f, 0.13f, 0.12f, 0.95f);

        var barraFillGo = new GameObject("BarraForcaFill", typeof(RectTransform), typeof(Image));
        barraFillGo.transform.SetParent(barraBg.transform, false);
        var bfRect = barraFillGo.GetComponent<RectTransform>();
        UIFactory.Stretch(bfRect);
        bfRect.offsetMin = new Vector2(2f, 2f);
        bfRect.offsetMax = new Vector2(-2f, -2f);
        barraForcaFill = barraFillGo.GetComponent<Image>();
        barraForcaFill.type = Image.Type.Filled;
        barraForcaFill.fillMethod = Image.FillMethod.Vertical;
        barraForcaFill.fillOrigin = (int)Image.OriginVertical.Bottom;
        barraForcaFill.fillAmount = 0f;
        barraForcaFill.color = new Color(0.95f, 0.72f, 0.25f, 1f);

        txtTurnoStatus = CriarTexto("TurnoStatus", raiz.transform, "", 16, new Color(0.92f, 0.90f, 0.84f), TextAlignmentOptions.Center);
        Posicionar(txtTurnoStatus.rectTransform, new Vector2(0f, -124f), new Vector2(560f, 22f));

        txtPercepcaoSubtitulo = CriarTexto("PercepcaoSubtitulo", raiz.transform, "", 14, new Color(0.68f, 0.84f, 0.96f), TextAlignmentOptions.Center);
        Posicionar(txtPercepcaoSubtitulo.rectTransform, new Vector2(0f, -144f), new Vector2(580f, 20f));

        txtControlesRodape = CriarTexto("ControlesRodape", raiz.transform,
            "Mouse ou [A/D]: Mirar   |   Segurar [Espaço] ou [Botão Esq]: Força   |   [Botão Dir] ou [C]: Cancelar Força",
            13, new Color(0.72f, 0.70f, 0.64f), TextAlignmentOptions.Center);
        Posicionar(txtControlesRodape.rectTransform, new Vector2(0f, -166f), new Vector2(600f, 18f));

        ConstruirModal();
        raiz.SetActive(false);
    }

    private void CriarBolasUI()
    {
        bolas.Clear();
        Color[] coresBolas =
        {
            new Color(0.96f, 0.95f, 0.90f),
            new Color(0.95f, 0.78f, 0.18f),
            new Color(0.20f, 0.48f, 0.88f),
            new Color(0.88f, 0.24f, 0.20f),
            new Color(0.58f, 0.28f, 0.78f),
            new Color(0.94f, 0.48f, 0.16f),
            new Color(0.18f, 0.68f, 0.36f),
            new Color(0.56f, 0.18f, 0.18f)
        };

        for (int i = 0; i <= 7; i++)
        {
            var bGo = new GameObject("Bola_" + i, typeof(RectTransform), typeof(Image));
            bGo.transform.SetParent(mesaRect, false);
            var r = bGo.GetComponent<RectTransform>();
            r.sizeDelta = new Vector2(BallRadius * 2f, BallRadius * 2f);

            var img = bGo.GetComponent<Image>();
            img.sprite = CriarSpriteCirculo(24, coresBolas[i], new Color(0.08f, 0.08f, 0.10f, 1f));
            img.raycastTarget = false;

            var marcaGo = new GameObject("MarcaSimbolo", typeof(RectTransform), typeof(Image));
            marcaGo.transform.SetParent(bGo.transform, false);
            var mr = marcaGo.GetComponent<RectTransform>();
            mr.anchorMin = new Vector2(0.5f, 0.5f);
            mr.anchorMax = new Vector2(0.5f, 0.5f);
            mr.sizeDelta = new Vector2(BallRadius * 2.7f, BallRadius * 2.7f);
            var mImg = marcaGo.GetComponent<Image>();
            mImg.sprite = CriarSpriteCirculo(28, Color.clear, new Color(1f, 0.22f, 0.20f, 0.95f));
            mImg.raycastTarget = false;
            marcaGo.SetActive(false);

            var lbl = CriarTexto("Num", bGo.transform, i == 0 ? "" : i.ToString(), 12, Color.white, TextAlignmentOptions.Center);
            UIFactory.Stretch(lbl.rectTransform);
            lbl.raycastTarget = false;

            bolas.Add(new BolaMesa
            {
                numero = i,
                rect = r,
                img = img,
                label = lbl,
                marcaVisual = marcaGo,
                ativa = true
            });
        }
    }

    private void ConstruirModal()
    {
        painelModal = new GameObject("ModalSinuca", typeof(RectTransform), typeof(Image));
        painelModal.transform.SetParent(raiz.transform, false);
        Posicionar(painelModal.GetComponent<RectTransform>(), Vector2.zero, new Vector2(460f, 260f));
        painelModal.GetComponent<Image>().color = new Color(0.12f, 0.10f, 0.09f, 0.98f);

        txtModalTitulo = CriarTexto("TituloModal", painelModal.transform, "", 22, new Color(0.95f, 0.82f, 0.45f), TextAlignmentOptions.Center);
        Posicionar(txtModalTitulo.rectTransform, new Vector2(0f, 98f), new Vector2(420f, 28f));

        txtModalCorpo = CriarTexto("CorpoModal", painelModal.transform, "", 15, new Color(0.90f, 0.87f, 0.80f), TextAlignmentOptions.TopLeft);
        Posicionar(txtModalCorpo.rectTransform, new Vector2(0f, 6f), new Vector2(416f, 144f));
        txtModalCorpo.textWrappingMode = TextWrappingModes.Normal;

        btnModalPrimario = CriarBotaoModal("BtnPrimario", painelModal.transform, "CONTINUAR", new Vector2(-95f, -96f));
        txtBtnPrimario = btnModalPrimario.GetComponentInChildren<TextMeshProUGUI>();

        btnModalSecundario = CriarBotaoModal("BtnSecundario", painelModal.transform, "MENU PRINCIPAL", new Vector2(95f, -96f));
        txtBtnSecundario = btnModalSecundario.GetComponentInChildren<TextMeshProUGUI>();

        painelModal.SetActive(false);
    }

    private Button CriarBotaoModal(string nome, Transform pai, string texto, Vector2 pos)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar(go.GetComponent<RectTransform>(), pos, new Vector2(174f, 30f));
        go.GetComponent<Image>().color = new Color(0.24f, 0.18f, 0.15f, 1f);

        var lbl = CriarTexto("Lbl", go.transform, texto, 15, new Color(0.95f, 0.88f, 0.72f), TextAlignmentOptions.Center);
        UIFactory.Stretch(lbl.rectTransform);

        return go.GetComponent<Button>();
    }

    private void ComecarNovaSerie(Action aoVencer)
    {
        StopAllCoroutines();
        onVictoryCallback = aoVencer;
        maoAtual = 1;
        maosVencidasMarcus = 0;
        maosVencidasZe = 0;
        demonstracaoMao2Feita = false;

        raiz.SetActive(true);
        ConfigurarMao(1);

        MostrarModal(
            "BAR DO ZÉ — MELHOR DE TRÊS MÃOS",
            "• <b>Objetivo da Série:</b> Vence quem ganhar <b>2 mãos</b> (2-0 ou 2-1).\n" +
            "• <b>Pontuação da Mão:</b> Branca + 7 bolas numeradas compartilhadas (1 a 7). Cada bola legalmente encaçapada vale 1 ponto. <b>4 pontos vencem a mão.</b>\n" +
            "• <b>Turnos:</b> Encaçapou legalmente, continua jogando. Errou sem falta, passa a vez.\n" +
            "• <b>Faltas:</b> Derrubar a branca, errar todas as bolas, não tocar em tabela/caçapa após o impacto, ou derrubar a <b>Bola Marcada [X]</b> (a partir da 2ª mão). Falta anula os pontos da tacada, repõe as bolas caídas e dá bola na mão ao adversário.",
            "INICIAR 1ª MÃO",
            () =>
            {
                painelModal.SetActive(false);
                IniciarTurnoAtual();
            },
            null, null
        );
    }

    private void AbrirModalRegrasDuranteJogo()
    {
        if (painelModal.activeSelf) return;
        // Painel é uma sobreposição. Não substituir o estado da mesa por um
        // estado de tutorial que uma coroutine poderia restaurar fora de hora.
        MostrarModal(
            "REGRAS — A BOLA MARCADA",
            "• <b>7 Bolas Compartilhadas:</b> Qualquer bola numerada (1 a 7) vale 1 ponto. Quem fizer <b>4 pontos</b> vence a mão.\n" +
            "• <b>Bola Marcada [X] ('Essa fica na mesa'):</b> A partir da 2ª mão, Zé marca uma bola por turno (indicada por anel vermelho e símbolo [X]). Ela pode ser tocada ou usada em carambola, mas <b>encaçapá-la é FALTA</b> para qualquer um dos dois. (Desativa se restar só 1 bola).\n" +
            "• <b>Controles:</b> Mova o mouse ou [A/D] para mirar. Segure [Espaço] ou [Botão Esq] para carregar a força e solte para bater. Clique [Botão Dir] ou [C] para cancelar a tacada.",
            "VOLTAR À MESA",
            () =>
            {
                painelModal.SetActive(false);
            },
            null, null
        );
    }

    private void ConfigurarMao(int numeroMao)
    {
        maoAtual = numeroMao;
        pontosMarcus = 0;
        pontosZe = 0;
        contadorTurnosMao = 0;
        numeroBolaMarcada = -1;
        brancaNaMao = false;
        forcaNormalizada = 0f;
        if (barraForcaFill != null) barraForcaFill.fillAmount = 0f;

        turnoDeMarcus = (maoAtual != 2);
        PosicionarBolasDeterministico(maoAtual);
        AtualizarTextosHUD();

        if (maoAtual == 1)
        {
            txtPercepcaoSubtitulo.text = "[Som ambiente: chuva lá fora, rádio baixo e copos no balcão]";
            overlayPercepcao.color = new Color(0.12f, 0.22f, 0.32f, 0f);
        }
        else if (maoAtual == 2)
        {
            txtPercepcaoSubtitulo.text = "[As conversas do bar cessam. O som de água represada ecoa sob o feltro da mesa.]";
            float alpha = MenuPausa.ReduzirEfeitosIntensos ? 0.05f : 0.14f;
            overlayPercepcao.color = new Color(0.08f, 0.18f, 0.28f, alpha);
        }
        else
        {
            txtPercepcaoSubtitulo.text = "[03:17 — O barulho de chuva pesada no para-brisa se mistura ao impacto das bolas.]";
            float alpha = MenuPausa.ReduzirEfeitosIntensos ? 0.08f : 0.20f;
            overlayPercepcao.color = new Color(0.16f, 0.12f, 0.22f, alpha);
        }
    }

    private void PosicionarBolasDeterministico(int indiceMao)
    {
        bolas[0].ativa = true;
        bolas[0].vel = Vector2.zero;
        bolas[0].pos = new Vector2(-115f, (indiceMao == 2) ? 12f : 0f);

        Vector2[] layoutBase =
        {
            new Vector2(45f, 0f),
            new Vector2(72f, 24f),
            new Vector2(72f, -24f),
            new Vector2(102f, 44f),
            new Vector2(100f, 0f),
            new Vector2(102f, -44f),
            new Vector2(132f, 16f)
        };

        float offsetMaoY = (indiceMao == 2) ? -8f : (indiceMao == 3 ? 8f : 0f);

        for (int i = 1; i <= 7; i++)
        {
            bolas[i].ativa = true;
            bolas[i].vel = Vector2.zero;
            bolas[i].caiuNestaTacada = false;
            Vector2 p = layoutBase[i - 1];
            p.y = Mathf.Clamp(p.y + offsetMaoY, -TableHeight * 0.38f, TableHeight * 0.38f);
            bolas[i].pos = p;
        }

        SincronizarTransformBolas();
    }

    private void IniciarTurnoAtual()
    {
        contadorTurnosMao++;
        AtualizarRegraBolaMarcadaAntesDoTurno();
        AtualizarTextosHUD();

        if (maoAtual == 2 && !demonstracaoMao2Feita)
        {
            demonstracaoMao2Feita = true;
            MostrarModal(
                "2ª MÃO — \"ESSA FICA NA MESA\"",
                "Zé apoia o giz na borda da tabela e aponta para a bola marcada com <b>[X]</b>:\n\n" +
                "<i>\"Nem tudo o que cai na caçapa resolve a noite, Marcus. Em Santa Rita, sempre tem aquilo que todo mundo combina de deixar onde está.\"</i>\n\n" +
                "• A bola marcada com <b>[X]</b> não pode ser encaçapada nem por você nem por Zé neste turno.\n" +
                "• Você pode tocá-la ou usá-la de tabela, mas derrubá-la é <b>FALTA</b>.",
                "ENTENDIDO",
                () =>
                {
                    painelModal.SetActive(false);
                    ContinuarInicioTurno();
                },
                null, null
            );
            return;
        }

        ContinuarInicioTurno();
    }

    private void ContinuarInicioTurno()
    {
        if (turnoDeMarcus)
        {
            if (brancaNaMao)
            {
                estado = EstadoPartida.PosicionandoBranca;
                bolas[0].ativa = true;
                bolas[0].vel = Vector2.zero;
                bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-115f, 0f), 0);
                SincronizarTransformBolas();
                txtTurnoStatus.text = "SUA VEZ (BOLA NA MÃO): Mova com Mouse/[WASD] e pressione [E] ou [Clique Esq] para posicionar.";
            }
            else
            {
                estado = EstadoPartida.MirandoJogador;
                txtTurnoStatus.text = "SUA VEZ: Mire e segure [Espaço] ou [Botão Esq] para definir a força.";
            }
        }
        else
        {
            estado = EstadoPartida.TurnoZePensando;
            txtTurnoStatus.text = "VEZ DE ZÉ: Zé está avaliando a mesa...";
            if (Application.isPlaying) StartCoroutine(ExecutarTurnoZeCoroutine());
        }
    }

    private void AtualizarRegraBolaMarcadaAntesDoTurno()
    {
        if (maoAtual <= 1)
        {
            numeroBolaMarcada = -1;
            AtualizarVisualBolasMarcadas();
            return;
        }

        List<int> ativas = new List<int>();
        for (int i = 1; i <= 7; i++)
        {
            if (bolas[i].ativa) ativas.Add(i);
        }

        if (ativas.Count < 2)
        {
            numeroBolaMarcada = -1;
            AtualizarVisualBolasMarcadas();
            return;
        }

        int idx = (contadorTurnosMao + maoAtual) % ativas.Count;
        numeroBolaMarcada = ativas[idx];
        AtualizarVisualBolasMarcadas();
    }

    private void AtualizarVisualBolasMarcadas()
    {
        for (int i = 1; i <= 7; i++)
        {
            bool marcada = (bolas[i].numero == numeroBolaMarcada) && bolas[i].ativa;
            if (bolas[i].marcaVisual != null)
                bolas[i].marcaVisual.SetActive(marcada);
            if (bolas[i].label != null)
                bolas[i].label.text = marcada ? (i + "X") : i.ToString();
        }
    }

    void Update()
    {
        if (!IsShowing || PayphoneCheckpoint.IsSaveUIOpen) return;
        if (MenuPausa.Aberto) return;
        if (painelModal != null && painelModal.activeSelf) return;

        switch (estado)
        {
            case EstadoPartida.PosicionandoBranca:
                AtualizarPosicionamentoBranca();
                break;
            case EstadoPartida.MirandoJogador:
            case EstadoPartida.CarregandoForca:
                AtualizarMiraEDisparoJogador();
                break;
            case EstadoPartida.BolasEmMovimento:
                SimularFisicaMesa(Time.deltaTime);
                break;
        }

        AtualizarVisualTacoEMira();
    }

    private void AtualizarPosicionamentoBranca()
    {
        Vector2 candidato = bolas[0].pos;

        if (Mouse.current != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mesaRect, Mouse.current.position.ReadValue(), null, out Vector2 localMouse))
        {
            candidato = localMouse;
        }

        if (Keyboard.current != null)
        {
            Vector2 move = Vector2.zero;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
            if (move.sqrMagnitude > 0.01f)
                candidato = bolas[0].pos + move.normalized * 140f * Time.deltaTime;
        }

        float limX = TableWidth * 0.5f - BallRadius - 4f;
        float limY = TableHeight * 0.5f - BallRadius - 4f;
        candidato.x = Mathf.Clamp(candidato.x, -limX, limX);
        candidato.y = Mathf.Clamp(candidato.y, -limY, limY);

        if (PosicaoValidaSemColisao(candidato, 0))
        {
            bolas[0].pos = candidato;
            SincronizarTransformBolas();
        }

        bool confirmar = false;
        if (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            confirmar = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && MouseDentroDaMesa())
            confirmar = true;

        if (confirmar && PosicaoValidaSemColisao(bolas[0].pos, 0))
        {
            brancaNaMao = false;
            estado = EstadoPartida.MirandoJogador;
            txtTurnoStatus.text = "SUA VEZ: Mire e segure [Espaço] ou [Botão Esq] para carregar a tacada.";
        }
    }

    private void AtualizarMiraEDisparoJogador()
    {
        if (estado == EstadoPartida.MirandoJogador)
        {
            if (Mouse.current != null && MouseDentroDaMesa() &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    mesaRect, Mouse.current.position.ReadValue(), null, out Vector2 localMouse))
            {
                Vector2 dir = localMouse - bolas[0].pos;
                if (dir.sqrMagnitude > 4f)
                    anguloMiraRad = Mathf.Atan2(dir.y, dir.x);
            }

            if (Keyboard.current != null)
            {
                float rot = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) rot += 1.6f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) rot -= 1.6f;
                anguloMiraRad += rot * Time.deltaTime;
            }

            bool iniciouCarga = false;
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                iniciouCarga = true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && MouseDentroDaMesa())
                iniciouCarga = true;

            if (iniciouCarga)
            {
                estado = EstadoPartida.CarregandoForca;
                forcaNormalizada = 0.08f;
                carregandoDirectionUp = true;
                txtTurnoStatus.text = "CARREGANDO FORÇA... Solte para tacar ou pressione [Botão Dir] / [C] para cancelar.";
            }
        }
        else if (estado == EstadoPartida.CarregandoForca)
        {
            bool cancelar = false;
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
                cancelar = true;
            if (Keyboard.current != null && (Keyboard.current.cKey.wasPressedThisFrame || Keyboard.current.backspaceKey.wasPressedThisFrame))
                cancelar = true;

            if (cancelar)
            {
                forcaNormalizada = 0f;
                barraForcaFill.fillAmount = 0f;
                estado = EstadoPartida.MirandoJogador;
                txtTurnoStatus.text = "Tacada cancelada. Ajuste sua mira.";
                return;
            }

            float delta = 1.15f * Time.deltaTime;
            forcaNormalizada += carregandoDirectionUp ? delta : -delta;
            if (forcaNormalizada >= 1f) { forcaNormalizada = 1f; carregandoDirectionUp = false; }
            if (forcaNormalizada <= 0.08f) { forcaNormalizada = 0.08f; carregandoDirectionUp = true; }
            barraForcaFill.fillAmount = forcaNormalizada;

            bool disparar = false;
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame)
                disparar = true;
            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
                disparar = true;

            if (disparar)
            {
                Vector2 direcao = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
                ExecutarTacada(direcao, Mathf.Lerp(170f, 540f, forcaNormalizada));
            }
        }
    }

    private bool MouseDentroDaMesa()
    {
        if (Mouse.current == null) return false;
        Vector2 screenPos = Mouse.current.position.ReadValue();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mesaRect, screenPos, null, out Vector2 local))
            return false;
        return Mathf.Abs(local.x) <= TableWidth * 0.56f && Mathf.Abs(local.y) <= TableHeight * 0.56f;
    }

    private IEnumerator ExecutarTurnoZeCoroutine()
    {
        yield return new WaitForSeconds(1.1f);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
            yield return null;

        if (brancaNaMao)
        {
            bolas[0].ativa = true;
            bolas[0].vel = Vector2.zero;
            bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-110f, 15f), 0);
            SincronizarTransformBolas();
            brancaNaMao = false;
            yield return new WaitForSeconds(0.45f);
            while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
                yield return null;
        }

        EscolherTacadaZe(out Vector2 direcaoZe, out float velocidadeZe);
        anguloMiraRad = Mathf.Atan2(direcaoZe.y, direcaoZe.x);
        yield return new WaitForSeconds(0.55f);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
            yield return null;
        if (estado != EstadoPartida.TurnoZePensando) yield break;

        ExecutarTacada(direcaoZe, velocidadeZe);
    }

    private void EscolherTacadaZe(out Vector2 direcaoFinal, out float velocidadeFinal)
    {
        Vector2 cuePos = bolas[0].pos;
        float melhorScore = -9999f;
        Vector2 melhorAlvo = Vector2.right;
        float melhorVel = 340f;

        for (int i = 1; i <= 7; i++)
        {
            if (!bolas[i].ativa) continue;
            if (bolas[i].numero == numeroBolaMarcada) continue;

            for (int p = 0; p < cacapas.Length; p++)
            {
                Vector2 pocketPos = cacapas[p];
                Vector2 ballToPocket = pocketPos - bolas[i].pos;
                float distBallPocket = ballToPocket.magnitude;
                Vector2 dirBallPocket = ballToPocket / Mathf.Max(1f, distBallPocket);

                Vector2 ghostBallPos = bolas[i].pos - dirBallPocket * (BallRadius * 2f);
                Vector2 cueToGhost = ghostBallPos - cuePos;
                float distCueGhost = cueToGhost.magnitude;
                Vector2 dirCueGhost = cueToGhost / Mathf.Max(1f, distCueGhost);

                float dot = Vector2.Dot(dirCueGhost, dirBallPocket);
                if (dot < 0.25f) continue;

                float penalidadeMarcada = 0f;
                if (numeroBolaMarcada > 0 && bolas[numeroBolaMarcada].ativa)
                {
                    float dMarcada = Vector2.Distance(bolas[numeroBolaMarcada].pos, bolas[i].pos);
                    if (dMarcada < BallRadius * 4.5f) penalidadeMarcada = 60f;
                }

                float score = dot * 150f - distBallPocket * 0.35f - distCueGhost * 0.2f - penalidadeMarcada;
                if (score > melhorScore)
                {
                    melhorScore = score;
                    melhorAlvo = dirCueGhost;
                    melhorVel = Mathf.Clamp(250f + (distCueGhost + distBallPocket) * 0.65f, 240f, 470f);
                }
            }
        }

        if (melhorScore < -500f)
        {
            for (int i = 1; i <= 7; i++)
            {
                if (bolas[i].ativa && bolas[i].numero != numeroBolaMarcada)
                {
                    melhorAlvo = (bolas[i].pos - cuePos).normalized;
                    melhorVel = 310f;
                    break;
                }
            }
        }

        float erroGraus = UnityEngine.Random.Range(-2.6f, 2.6f);
        float ang = Mathf.Atan2(melhorAlvo.y, melhorAlvo.x) + erroGraus * Mathf.Deg2Rad;
        direcaoFinal = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
        velocidadeFinal = melhorVel;
    }

    private void ExecutarTacada(Vector2 direcao, float velocidade)
    {
        houvePrimeiroContatoBolaObjeto = false;
        houveToqueTabelaOuCacapa = false;
        brancaCaiuNestaTacada = false;
        bolaMarcadaCaiuNestaTacada = false;
        bolasObjetoCaidasNaTacada.Clear();

        for (int i = 0; i < bolas.Count; i++)
            bolas[i].caiuNestaTacada = false;

        bolas[0].vel = direcao.normalized * velocidade;
        forcaNormalizada = 0f;
        if (barraForcaFill != null) barraForcaFill.fillAmount = 0f;

        estado = EstadoPartida.BolasEmMovimento;
        txtTurnoStatus.text = turnoDeMarcus ? "Bolas em movimento..." : "Zé executou a tacada...";
    }

    private void SimularFisicaMesa(float dt)
    {
        const int subSteps = 6;
        float stepDt = Mathf.Min(dt, 0.033f) / subSteps;

        float limX = TableWidth * 0.5f - BallRadius;
        float limY = TableHeight * 0.5f - BallRadius;

        for (int s = 0; s < subSteps; s++)
        {
            for (int i = 0; i < bolas.Count; i++)
            {
                var b = bolas[i];
                if (!b.ativa) continue;

                b.pos += b.vel * stepDt;

                bool encacapou = false;
                for (int p = 0; p < cacapas.Length; p++)
                {
                    if (Vector2.Distance(b.pos, cacapas[p]) <= PocketRadius)
                    {
                        encacapou = true;
                        break;
                    }
                }

                if (encacapou)
                {
                    b.ativa = false;
                    b.vel = Vector2.zero;
                    b.caiuNestaTacada = true;
                    if (houvePrimeiroContatoBolaObjeto) houveToqueTabelaOuCacapa = true;

                    if (b.numero == 0)
                    {
                        brancaCaiuNestaTacada = true;
                    }
                    else
                    {
                        bolasObjetoCaidasNaTacada.Add(b.numero);
                        if (b.numero == numeroBolaMarcada)
                            bolaMarcadaCaiuNestaTacada = true;
                    }
                    continue;
                }

                if (b.pos.x < -limX) { b.pos.x = -limX; b.vel.x = -b.vel.x * CushionRestitution; RegistrarTabela(); }
                else if (b.pos.x > limX) { b.pos.x = limX; b.vel.x = -b.vel.x * CushionRestitution; RegistrarTabela(); }

                if (b.pos.y < -limY) { b.pos.y = -limY; b.vel.y = -b.vel.y * CushionRestitution; RegistrarTabela(); }
                else if (b.pos.y > limY) { b.pos.y = limY; b.vel.y = -b.vel.y * CushionRestitution; RegistrarTabela(); }
            }

            for (int i = 0; i < bolas.Count; i++)
            {
                if (!bolas[i].ativa) continue;
                for (int j = i + 1; j < bolas.Count; j++)
                {
                    if (!bolas[j].ativa) continue;

                    Vector2 delta = bolas[j].pos - bolas[i].pos;
                    float dist = delta.magnitude;
                    float minDist = BallRadius * 2f;

                    if (dist > 0.0001f && dist < minDist)
                    {
                        if (i == 0 || j == 0)
                            houvePrimeiroContatoBolaObjeto = true;

                        Vector2 normal = delta / dist;
                        float overlap = minDist - dist;
                        bolas[i].pos -= normal * (overlap * 0.5f);
                        bolas[j].pos += normal * (overlap * 0.5f);

                        Vector2 relVel = bolas[j].vel - bolas[i].vel;
                        float velAlongNormal = Vector2.Dot(relVel, normal);
                        if (velAlongNormal < 0f)
                        {
                            float impulse = -velAlongNormal * 0.96f;
                            if (impulse > 5) SantaRitaSound.Play("sinuca", Mathf.Clamp(impulse * .001f, .025f, .2f), 1.1f, .065f);
                            bolas[i].vel -= normal * impulse;
                            bolas[j].vel += normal * impulse;
                        }
                    }
                }
            }

            for (int i = 0; i < bolas.Count; i++)
            {
                var b = bolas[i];
                if (!b.ativa) continue;
                float speed = b.vel.magnitude;
                if (speed > 0f)
                {
                    float newSpeed = Mathf.Max(0f, speed - BallFriction * stepDt);
                    b.vel = (newSpeed * newSpeed <= StopSpeedSq) ? Vector2.zero : b.vel * (newSpeed / speed);
                }
            }
        }

        SincronizarTransformBolas();

        bool algumaMovendo = false;
        for (int i = 0; i < bolas.Count; i++)
        {
            if (bolas[i].ativa && bolas[i].vel.sqrMagnitude > StopSpeedSq)
            {
                algumaMovendo = true;
                break;
            }
        }

        if (!algumaMovendo)
        {
            for (int i = 0; i < bolas.Count; i++)
                bolas[i].vel = Vector2.zero;
            ResolverFimDaTacada();
        }
    }

    private void ResolverFimDaTacada()
    {
        estado = EstadoPartida.ResolvendoTacada;

        bool falta = false;
        string motivoFalta = "";

        if (brancaCaiuNestaTacada)
        {
            falta = true;
            motivoFalta = "Bola branca encaçapada";
        }
        else if (bolaMarcadaCaiuNestaTacada)
        {
            falta = true;
            motivoFalta = "A Bola Marcada [" + numeroBolaMarcada + "X] caiu na caçapa ('Essa fica na mesa!')";
        }
        else if (!houvePrimeiroContatoBolaObjeto)
        {
            falta = true;
            motivoFalta = "A branca não atingiu nenhuma bola numerada";
        }
        else if (!houveToqueTabelaOuCacapa)
        {
            falta = true;
            motivoFalta = "Nenhuma bola tocou em tabela ou caçapa após o impacto";
        }

        if (falta)
        {
            for (int i = 0; i < bolasObjetoCaidasNaTacada.Count; i++)
            {
                int num = bolasObjetoCaidasNaTacada[i];
                BolaMesa b = bolas[num];
                b.ativa = true;
                b.vel = Vector2.zero;
                b.caiuNestaTacada = false;
                b.pos = EncontrarPosicaoLivreDeterministica(new Vector2(75f + i * 22f, 0f), num);
            }

            bolas[0].ativa = true;
            bolas[0].vel = Vector2.zero;
            bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-115f, 0f), 0);

            SincronizarTransformBolas();

            turnoDeMarcus = !turnoDeMarcus;
            brancaNaMao = true;
            AtualizarTextosHUD();

            txtTurnoStatus.text = "FALTA! (" + motivoFalta + ") — Bola na mão para " + (turnoDeMarcus ? "Marcus." : "Zé.");
            AgendarProximoTurno(1.6f);
            return;
        }

        int pontosFeitos = bolasObjetoCaidasNaTacada.Count;
        if (turnoDeMarcus)
            pontosMarcus += pontosFeitos;
        else
            pontosZe += pontosFeitos;

        ValidarInvarianteSeteBolas();
        AtualizarTextosHUD();

        if (pontosMarcus >= 4 || pontosZe >= 4)
        {
            bool marcusVenceuMao = pontosMarcus >= 4;
            ConcluirMao(marcusVenceuMao);
            return;
        }

        if (pontosFeitos > 0)
        {
            txtTurnoStatus.text = (turnoDeMarcus ? "Marcus" : "Zé") + " pontuou (+" + pontosFeitos + ") e continua na mesa!";
        }
        else
        {
            turnoDeMarcus = !turnoDeMarcus;
            txtTurnoStatus.text = "Sem caçapa. Vez de " + (turnoDeMarcus ? "Marcus." : "Zé.");
        }

        brancaNaMao = false;
        AgendarProximoTurno(1.1f);
    }

    private void RegistrarTabela()
    {
        if (houvePrimeiroContatoBolaObjeto) houveToqueTabelaOuCacapa = true;
    }

    private void AgendarProximoTurno(float segundos)
    {
        // EditMode executa o mesmo resolver síncrono sem iniciar uma rotina.
        if (Application.isPlaying) StartCoroutine(AguardarEIniciarProximoTurno(segundos));
    }

    public bool ValidarInvarianteSeteBolas()
    {
        int emJogo = 0;
        for (int i = 1; i <= 7; i++)
            if (bolas[i].ativa) emJogo++;

        return (pontosMarcus + pontosZe + emJogo) == 7;
    }

    private IEnumerator AguardarEIniciarProximoTurno(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
            yield return null;
        if (estado != EstadoPartida.ResolvendoTacada) yield break;
        IniciarTurnoAtual();
    }

    private void ConcluirMao(bool marcusVenceu)
    {
        estado = EstadoPartida.FimDeMao;
        if (marcusVenceu) maosVencidasMarcus++;
        else maosVencidasZe++;

        AtualizarTextosHUD();

        if (maosVencidasMarcus >= 2)
        {
            ConcluirSerieVitoria();
            return;
        }

        if (maosVencidasZe >= 2)
        {
            ConcluirSerieDerrota();
            return;
        }

        int proximaMao = maoAtual + 1;
        string titulo = marcusVenceu
            ? ("FIM DA " + maoAtual + "ª MÃO — VITÓRIA DE MARCUS")
            : ("FIM DA " + maoAtual + "ª MÃO — VITÓRIA DE ZÉ");

        string falaZe = (proximaMao == 2)
            ? "Zé passa o pano na beirada da mesa:\n<i>\"Você ainda lembra como segura o taco. Vamos ver na segunda mão, quando nem toda bola puder cair.\"</i>"
            : "O placar empata em 1 a 1. A chuva engrossa no telhado de zinco:\n<i>\"Última mão, Marcus. Quem fizer 4 pontos agora leva a conversa até o fim.\"</i>";

        MostrarModal(
            titulo,
            "Placar da Série: <b>MARCUS " + maosVencidasMarcus + " x " + maosVencidasZe + " ZÉ</b>\n\n" + falaZe,
            "IR PARA A " + proximaMao + "ª MÃO",
            () =>
            {
                painelModal.SetActive(false);
                ConfigurarMao(proximaMao);
                IniciarTurnoAtual();
            },
            null, null
        );
    }

    private void ConcluirSerieVitoria()
    {
        estado = EstadoPartida.FimDeSerieVitoria;
        MostrarModal(
            "VITÓRIA NA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ")",
            "Zé encosta o taco no suporte da parede e fica em silêncio por alguns segundos, ouvindo a chuva nos fundos do bar.\n\n" +
            "Ele tira do bolso do colete uma <b>Ficha Metálica do Clube</b> e a coloca sobre o feltro verde.",
            "OUVIR ZÉ",
            () =>
            {
                painelModal.SetActive(false);
                raiz.SetActive(false);
                estado = EstadoPartida.Inativo;
                var cb = onVictoryCallback;
                onVictoryCallback = null;
                cb?.Invoke();
            },
            null, null
        );
    }

    private void ConcluirSerieDerrota()
    {
        estado = EstadoPartida.FimDeSerieDerrota;
        bool temSaveManual = CheckpointSystem.HasCheckpoint;

        if (temSaveManual)
        {
            MostrarModal(
                "FIM DA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ") — ZÉ ENCERRA A CONVERSA",
                "Zé apaga a lâmpada sobre a mesa de sinuca:\n" +
                "<i>\"Você está com a cabeça em 1998, Marcus. Volte quando tiver firmeza para ouvir o que aconteceu hoje.\"</i>\n\n" +
                "Escolha qual <b>registro manual</b> deseja carregar, ou volte ao Menu Principal.",
                "ESCOLHER SAVE",
                () =>
                {
                    painelModal.SetActive(false);
                    PayphoneCheckpoint.AbrirSelecaoCarregar(ConcluirSerieDerrota, () =>
                    { raiz.SetActive(false); estado = EstadoPartida.Inativo; });
                },
                "MENU PRINCIPAL",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    SceneFader.Instance.LoadScene("MainMenu");
                }
            );
        }
        else
        {
            MostrarModal(
                "FIM DA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ") — SEM SAVE MANUAL",
                "Zé apaga a lâmpada sobre a mesa de sinuca e fecha a conversa.\n\n" +
                "<b>Nenhum Orelhão foi utilizado para salvar manualmente nesta sessão.</b>\n" +
                "Escolha reiniciar a jornada ou retornar ao Menu Principal.",
                "NOVO JOGO",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    CheckpointSystem.Clear();
                    GameState.Limpar();
                    SceneFader.Instance.LoadScene("Prologo");
                },
                "MENU PRINCIPAL",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    SceneFader.Instance.LoadScene("MainMenu");
                }
            );
        }
    }

    private Vector2 EncontrarPosicaoLivreDeterministica(Vector2 preferida, int ignorarIndice)
    {
        float limX = TableWidth * 0.5f - BallRadius - 8f;
        float limY = TableHeight * 0.5f - BallRadius - 8f;

        preferida.x = Mathf.Clamp(preferida.x, -limX, limX);
        preferida.y = Mathf.Clamp(preferida.y, -limY, limY);

        if (PosicaoValidaSemColisao(preferida, ignorarIndice))
            return preferida;

        for (int passo = 1; passo <= 18; passo++)
        {
            float raio = passo * (BallRadius * 2.1f);
            for (int angIdx = 0; angIdx < 12; angIdx++)
            {
                float ang = angIdx * (Mathf.PI * 2f / 12f);
                Vector2 cand = preferida + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * raio;
                cand.x = Mathf.Clamp(cand.x, -limX, limX);
                cand.y = Mathf.Clamp(cand.y, -limY, limY);
                if (PosicaoValidaSemColisao(cand, ignorarIndice))
                    return cand;
            }
        }

        return Vector2.zero;
    }

    private bool PosicaoValidaSemColisao(Vector2 pos, int ignorarIndice)
    {
        for (int p = 0; p < cacapas.Length; p++)
        {
            if (Vector2.Distance(pos, cacapas[p]) < PocketRadius + BallRadius * 0.8f)
                return false;
        }

        for (int i = 0; i < bolas.Count; i++)
        {
            if (i == ignorarIndice || !bolas[i].ativa) continue;
            if (Vector2.Distance(pos, bolas[i].pos) < BallRadius * 2.08f)
                return false;
        }

        return true;
    }

    private void SincronizarTransformBolas()
    {
        for (int i = 0; i < bolas.Count; i++)
        {
            var b = bolas[i];
            if (b.rect != null)
            {
                b.rect.gameObject.SetActive(b.ativa);
                b.rect.anchoredPosition = b.pos;
            }
        }
    }

    private void AtualizarVisualTacoEMira()
    {
        bool mostrarMira = (estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.CarregandoForca) && bolas[0].ativa;
        if (linhaMiraRect != null) linhaMiraRect.gameObject.SetActive(mostrarMira);
        if (tacoRect != null) tacoRect.gameObject.SetActive(mostrarMira);

        if (!mostrarMira) return;

        float graus = anguloMiraRad * Mathf.Rad2Deg;
        linhaMiraRect.anchoredPosition = bolas[0].pos;
        linhaMiraRect.localEulerAngles = new Vector3(0f, 0f, graus);

        Vector2 dir = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
        float recuo = 12f + forcaNormalizada * 26f;
        tacoRect.anchoredPosition = bolas[0].pos - dir * recuo;
        tacoRect.localEulerAngles = new Vector3(0f, 0f, graus);
    }

    private void AtualizarTextosHUD()
    {
        if (txtPlacarSerie != null)
            txtPlacarSerie.text = $"SÉRIE MELHOR DE 3 — MARCUS [{maosVencidasMarcus}] x [{maosVencidasZe}] ZÉ  (Mão {maoAtual}/3)";
        if (txtPlacarMao != null)
            txtPlacarMao.text = $"PONTOS NA MÃO (Meta: 4 pts):   MARCUS {pontosMarcus}/4   |   ZÉ {pontosZe}/4";

        if (txtRegraBolaMarcada != null)
        {
            if (numeroBolaMarcada > 0 && bolas[numeroBolaMarcada].ativa)
                txtRegraBolaMarcada.text = $"REGRA ATIVA — \"ESSA FICA NA MESA\": Bola [{numeroBolaMarcada}X] NÃO pode ser encaçapada!";
            else if (maoAtual >= 2)
                txtRegraBolaMarcada.text = "REGRA \"ESSA FICA NA MESA\": Desativada (menos de 2 bolas restantes).";
            else
                txtRegraBolaMarcada.text = "1ª MÃO: Sinuca Franca — Encaçape 4 bolas numeradas para vencer a mão.";
        }
    }

    private void MostrarModal(string titulo, string corpo, string txtPrimario, UnityEngine.Events.UnityAction acaoPrimaria,
        string txtSecundario, UnityEngine.Events.UnityAction acaoSecundaria)
    {
        painelModal.SetActive(true);
        txtModalTitulo.text = titulo;
        txtModalCorpo.text = corpo;

        btnModalPrimario.onClick.RemoveAllListeners();
        txtBtnPrimario.text = txtPrimario;
        btnModalPrimario.onClick.AddListener(acaoPrimaria);

        if (!string.IsNullOrEmpty(txtSecundario) && acaoSecundaria != null)
        {
            btnModalSecundario.gameObject.SetActive(true);
            btnModalPrimario.GetComponent<RectTransform>().anchoredPosition = new Vector2(-95f, -96f);
            txtBtnSecundario.text = txtSecundario;
            btnModalSecundario.onClick.RemoveAllListeners();
            btnModalSecundario.onClick.AddListener(acaoSecundaria);
        }
        else
        {
            btnModalSecundario.gameObject.SetActive(false);
            btnModalPrimario.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -96f);
        }
    }

    private Sprite CriarSpriteCirculo(int size, Color fill, Color border)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        float r = size * 0.5f - 1f;
        Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                if (d > r) tex.SetPixel(x, y, Color.clear);
                else if (d > r - 2.1f && border.a > 0.01f) tex.SetPixel(x, y, border);
                else tex.SetPixel(x, y, fill);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    private TextMeshProUGUI CriarTexto(string nome, Transform pai, string conteudo, float tamanho, Color cor, TextAlignmentOptions alinhamento)
    {
        TextMeshProUGUI t = UIFactory.CreateLabel(pai, nome, tamanho);
        t.text = conteudo;
        t.color = cor;
        t.alignment = alinhamento;
        return t;
    }

    private void Posicionar(RectTransform r, Vector2 pos, Vector2 tam)
    {
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta = tam;
    }

#if UNITY_EDITOR || DEBUG
    public bool SimularTesteDeInvariantes(out string relatorio)
    {
        // 1. Verifica Mão 1: 7 bolas na mesa, 0 pontos, sem bola marcada
        ConfigurarMao(1);
        AtualizarRegraBolaMarcadaAntesDoTurno();
        if (!ValidarInvarianteSeteBolas())
        {
            relatorio = "Mão 1 falhou na invariante 7 bolas (marcus + ze + emJogo != 7).";
            return false;
        }
        if (numeroBolaMarcada != -1)
        {
            relatorio = "Mão 1 não deveria ter Bola Marcada.";
            return false;
        }

        // 2. Verifica Mão 2: Bola Marcada ativa quando há >= 2 bolas
        ConfigurarMao(2);
        AtualizarRegraBolaMarcadaAntesDoTurno();
        if (numeroBolaMarcada < 1 || numeroBolaMarcada > 7)
        {
            relatorio = "Mão 2 deveria marcar uma bola [X] entre 1 e 7.";
            return false;
        }

        // 3. Mesmo resolver da partida: falta e bola comum caem na mesma tacada.
        int marcada = numeroBolaMarcada;
        int comum = marcada == 1 ? 2 : 1;
        turnoDeMarcus = true;
        bolas[marcada].ativa = false;
        bolas[comum].ativa = false;
        bolas[marcada].caiuNestaTacada = true;
        bolasObjetoCaidasNaTacada.Clear();
        bolasObjetoCaidasNaTacada.Add(marcada);
        bolasObjetoCaidasNaTacada.Add(comum);
        bolaMarcadaCaiuNestaTacada = true;
        brancaCaiuNestaTacada = false;
        houvePrimeiroContatoBolaObjeto = true;
        houveToqueTabelaOuCacapa = true;

        ResolverFimDaTacada();
        if (!ValidarInvarianteSeteBolas() || !bolas[marcada].ativa || !bolas[comum].ativa ||
            pontosMarcus != 0 || pontosZe != 0 || turnoDeMarcus || !brancaNaMao ||
            !PosicaoValidaSemColisao(bolas[marcada].pos, marcada))
        {
            relatorio = "Falta na Bola Marcada não repôs a bola na mesa preservando soma == 7.";
            return false;
        }

        // 4. Verifica desativação da Bola Marcada quando restam < 2 bolas na mesa
        pontosMarcus = 3;
        pontosZe = 3;
        for (int i = 1; i <= 6; i++) bolas[i].ativa = false;
        bolas[7].ativa = true;
        if (!ValidarInvarianteSeteBolas())
        {
            relatorio = "Estado 3x3 com 1 bola na mesa violou invariante de 7 bolas.";
            return false;
        }
        AtualizarRegraBolaMarcadaAntesDoTurno();
        if (numeroBolaMarcada != -1)
        {
            relatorio = "Bola Marcada deveria desativar quando resta apenas 1 bola na mesa.";
            return false;
        }

        // Quarta bola e branca caem juntas: não pode declarar vitória.
        ConfigurarMao(1); pontosMarcus = 3;
        for (int i = 1; i <= 4; i++) bolas[i].ativa = false;
        bolasObjetoCaidasNaTacada.Clear(); bolasObjetoCaidasNaTacada.Add(4);
        brancaCaiuNestaTacada = true; bolaMarcadaCaiuNestaTacada = false;
        ResolverFimDaTacada();
        if (pontosMarcus != 3 || maosVencidasMarcus != 0 || !bolas[4].ativa || !ValidarInvarianteSeteBolas())
        { relatorio = "Quarta bola + branca não anulou pontuação antes de decidir vencedor."; return false; }

        // Séries completas pelos resultados do resolver (entradas controladas, sem alegar tacadas humanas).
        foreach (var winners in new[] { new[] { true, true }, new[] { true, false, true }, new[] { false, false } })
        {
            maosVencidasMarcus = maosVencidasZe = 0;
            for (int h = 0; h < winners.Length; h++)
            {
                ConfigurarMao(h + 1); turnoDeMarcus = winners[h];
                numeroBolaMarcada = -1;
                bolasObjetoCaidasNaTacada.Clear();
                for (int i = 1; i <= 4; i++) { bolas[i].ativa = false; bolasObjetoCaidasNaTacada.Add(i); }
                brancaCaiuNestaTacada = bolaMarcadaCaiuNestaTacada = false;
                houvePrimeiroContatoBolaObjeto = houveToqueTabelaOuCacapa = true;
                ResolverFimDaTacada();
            }
            if (estado != (winners[winners.Length - 1] ? EstadoPartida.FimDeSerieVitoria : EstadoPartida.FimDeSerieDerrota))
            { relatorio = "Resolver não concluiu série 2–0/2–1/derrota."; return false; }
        }
        relatorio = "Resolver real: falta + bola comum; quarta bola + branca; reposição sem sobreposição; série 2–0/2–1 e derrota; invariante 7 bolas; marcada desativa com uma bola.";
        return true;
    }
#endif
}
