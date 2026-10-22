using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class BochaMinigameUI : MonoBehaviour
{
    private static BochaMinigameUI instance;
    public static bool IsShowing => instance != null && instance.raiz != null && instance.raiz.activeSelf;
    public static BochaMinigameUI Instance
    {
        get
        {
            if (instance != null) return instance;
            var host = new GameObject("BochaMinigameUI");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<BochaMinigameUI>();
            instance.Construir();
            return instance;
        }
    }

    private enum Fase { Mira, Carregando, Rolagem, Tonando, Resultado }
    private Fase fase;
    private GameObject raiz, modal, faixaGo;
    private RectTransform cancha, mira, marcadorLancamento, forcaFill;
    private TextMeshProUGUI placar, estado, estoque, medida, rodape, percepcao;
    private readonly List<RectTransform> bolasUI = new List<RectTransform>();
    private BochaMatch partida;
    private BochaSimulation sim;
    private readonly System.Random rng = new System.Random();
    private Action aoVencer;
    private bool pratica, subindo, aiPreparada, efeitoMao;
    private float xLancamento, forca, acumulador, quieto, esperaIA, ultimoImpacto;
    private Vector2 direcao = Vector2.up;
    private BochaShot shotIA;
    private AudioSource efeitos;
    private AudioClip impacto;
    private int larguraTela, alturaTela;

    public void Abrir(Action vitoria)
    {
        larguraTela=Screen.width;alturaTela=Screen.height;
        aoVencer = vitoria;
        raiz.SetActive(true);
        MostrarModal("SOCIEDADE BEIRA-RIO — BOCHA",
            "Tonão põe quatro bochas de cada lado da cancha.\n\n" +
            "Encoste no balim, proteja sua posição ou retire a bocha rival. O balim também pode se mover.\n" +
            "Você pode experimentar uma mão antes do duelo. A prática não grava progresso.",
            ("PRATICAR", () => Comecar(true)), ("DUELAR ATÉ 6 PONTOS", () => Comecar(false)), ("VOLTAR AO SALÃO", Fechar));
    }

    private void Construir()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "BochaCanvas", 8850);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        raiz = Painel(canvas.transform, "CanchaRaiz", Vector2.zero, new Vector2(640, 360), new Color(.055f, .055f, .045f, .99f)).gameObject;
        Label(raiz.transform, "Titulo", "SOCIEDADE BEIRA-RIO", new Vector2(0, 163), new Vector2(420, 25), 19);
        placar = Label(raiz.transform, "Placar", "", new Vector2(0, 138), new Vector2(480, 25), 17);
        estado = Label(raiz.transform, "Estado", "", new Vector2(0, 111), new Vector2(590, 28), 15);
        cancha = Painel(raiz.transform, "Cancha", Vector2.zero, new Vector2(476, 156), new Color(.24f, .15f, .09f));
        var areia = Painel(cancha, "Piso", Vector2.zero, new Vector2(460, 140), Color.white).GetComponent<Image>();
        areia.sprite = CriarPiso();
        foreach (float x in new[] { -234f, 234f })
        {
            var parede = Painel(cancha, x > 0 ? "ParedeFundo" : "ParedeLancamento", new Vector2(x, 0), new Vector2(8, 156), new Color(.28f, .18f, .10f));
            Painel(parede, "MadeiraLuz", new Vector2(-2, 0), new Vector2(1, 150), new Color(.58f, .40f, .23f));
            for (int y = -66; y <= 66; y += 22)
                Painel(parede, "Junta", new Vector2(0, y), new Vector2(8, 1), new Color(.12f, .09f, .06f));
        }
        for (int i = 1; i <= 4; i++)
            Painel(cancha, "Medida_" + i, new Vector2(-230 + i * 92, 0), new Vector2(1, 140), new Color(.65f, .56f, .39f, .35f));
        faixaGo = Painel(cancha, "FaixaUmida", Vector2.zero, Vector2.zero, new Color(.26f, .35f, .31f, .65f)).gameObject;
        faixaGo.AddComponent<RectMask2D>();
        for (int i = 0; i < 20; i++)
        {
            var traco = Painel(faixaGo.transform, "Traco_" + i, new Vector2(0, -66 + i * 7), new Vector2(50, 1), new Color(.68f, .69f, .54f, .8f));
            traco.localRotation = Quaternion.Euler(0, 0, 15);
        }
        faixaGo.SetActive(false);
        Painel(cancha, "LinhaLancamento", new Vector2(-215, 0), new Vector2(2, 140), new Color(.96f, .86f, .56f));
        marcadorLancamento = Painel(cancha, "PosicaoLancamento", new Vector2(-215, 0), new Vector2(10, 10), Color.white);
        marcadorLancamento.GetComponent<Image>().sprite = Circulo(new Color(.9f, .85f, .62f));
        mira = Painel(cancha, "DirecaoSemPrevisao", Vector2.zero, new Vector2(45, 1.5f), new Color(1, .9f, .6f));
        mira.pivot = new Vector2(0, .5f);
        estoque = Label(raiz.transform, "Estoque", "", new Vector2(0, -91), new Vector2(590, 22), 14);
        medida = Label(raiz.transform, "Medida", "", new Vector2(0, 89), new Vector2(590, 18), 12);
        percepcao = Label(raiz.transform, "Percepcao", "", new Vector2(0, -111), new Vector2(590, 21), 13);
        percepcao.color = new Color(.7f, .81f, .74f);
        Painel(raiz.transform, "BarraForca", new Vector2(-52, -134), new Vector2(226, 9), new Color(.2f, .19f, .14f));
        forcaFill = Painel(raiz.transform, "Forca", new Vector2(-165, -134), new Vector2(0, 7), new Color(.86f, .67f, .29f));
        forcaFill.pivot = new Vector2(0, .5f);
        Label(raiz.transform, "ForcaLbl", "FORÇA", new Vector2(-206, -134), new Vector2(60, 20), 12);
        Botao(raiz.transform, "REGRAS", new Vector2(186, -134), new Vector2(90, 22), () => Regras(0));
        rodape = Label(raiz.transform, "Controles", "A/D: posição • Mouse: direção • Segure e solte clique/Espaço • C/Direito: cancelar • ESC: pausa",
            new Vector2(0, -164), new Vector2(620, 25), 11.8f);
        efeitos = gameObject.AddComponent<AudioSource>(); efeitos.playOnAwake = false;
        impacto = Resources.Load<AudioClip>("Audio/Foley/bocha") ?? Capitulo2Audio.CriarImpactoMadeira();
        raiz.SetActive(false);
    }

    private void Comecar(bool treino)
    {
        pratica = treino;
        partida = new BochaMatch();
        NovaMao();
        MostrarModal(treino ? "PRÁTICA — SEM APOSTA" : "PRIMEIRA MÃO",
            "O balim claro, marcado B, está visível antes da jogada. Marcus usa bochas M; Tonão, T.\n\n" +
            "A/D escolhem a posição na faixa. Mire com o mouse; segure e solte o clique ou Espaço para lançar. C ou botão direito cancelam sem gastar bocha.\n\n" +
            "A direção desenhada é só a saída da bola, não uma promessa de trajetória. Quem está mais longe joga de novo enquanto tiver bochas.",
            ("LER A CANCHA", () => modal.SetActive(false)), ("REGRAS COMPLETAS", () => Regras(0)));
    }

    private void NovaMao()
    {
        foreach (var r in bolasUI) if (r != null) Destroy(r.gameObject);
        bolasUI.Clear();
        sim = partida.CriarMao(); sim.Impacto = ImpactoReal;
        acumulador = quieto = 0f; xLancamento = 0; efeitoMao = false;
        percepcao.text = sim.faixaUmida.width > 0 ? "RASTROS D'ÁGUA: faixa riscada desacelera TODAS as bolas nesta mão." : "Piso normal. Observe a rolagem antes de aumentar a força.";
        faixaGo.SetActive(sim.faixaUmida.width > 0);
        if (sim.faixaUmida.width > 0)
        {
            Rect r = sim.faixaUmida;
            var rt = (RectTransform)faixaGo.transform;
            rt.anchoredPosition = Ponto(new Vector2(r.center.x, r.center.y));
            rt.sizeDelta = new Vector2(r.height * 23f, r.width * (140f / 6f));
            foreach (RectTransform traco in rt) traco.sizeDelta = new Vector2(rt.sizeDelta.x - 4, 1);
        }
        ProximoTurno(); AtualizarTela();
    }

    private void ProximoTurno()
    {
        quieto = 0; forca = .25f; subindo = true; aiPreparada = false;
        if (!sim.bolas[0].valida || partida.Proximo(sim) == BochaSide.Nenhum) { ResolverMao(); return; }
        fase = partida.Proximo(sim) == BochaSide.Marcus ? Fase.Mira : Fase.Tonando;
        esperaIA = .9f;
    }

    private void Update()
    {
        if (!IsShowing || Keyboard.current == null) return;
        if(larguraTela!=Screen.width||alturaTela!=Screen.height)
        {
            larguraTela=Screen.width;alturaTela=Screen.height;
            if(modal==null||!modal.activeSelf)
            {
                if(fase==Fase.Carregando)fase=Fase.Mira;
                Pausar();return;
            }
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (modal != null && modal.activeSelf) return;
            if (fase == Fase.Carregando) fase = Fase.Mira;
            Pausar(); return;
        }
        if (modal != null && modal.activeSelf) return;
        if (fase == Fase.Mira || fase == Fase.Carregando) EntradaLancamento();
        else if (fase == Fase.Rolagem)
        {
            acumulador += Mathf.Min(Time.deltaTime, .12f);
            while (acumulador >= BochaSimulation.Passo)
            { sim.Step(); acumulador -= BochaSimulation.Passo; }
            quieto = sim.IsMoving ? 0 : quieto + Time.deltaTime;
            if (quieto >= .4f) ProximoTurno();
        }
        else if (fase == Fase.Tonando)
        {
            esperaIA -= Time.deltaTime;
            if (esperaIA <= 0)
            {
                if (!aiPreparada)
                {
                    shotIA = BochaAI.Escolher(sim, 4 - partida.usadas[1], rng);
                    aiPreparada = true; esperaIA = .7f; percepcao.text = shotIA.intencao;
                }
                else Lancar(BochaSide.Tonao, shotIA.x, shotIA.velocidade);
            }
        }
        AtualizarTela();
    }

    private void EntradaLancamento()
    {
        var k = Keyboard.current; var mouse = Mouse.current;
        float eixo = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0) -
            (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0);
        xLancamento = Mathf.Clamp(xLancamento + eixo * Time.deltaTime * 2f, -2.7f, 2.7f);
        bool dentro = mouse != null && RectTransformUtility.RectangleContainsScreenPoint(cancha, mouse.position.ReadValue());
        if (dentro && RectTransformUtility.ScreenPointToLocalPointInRectangle(cancha, mouse.position.ReadValue(), null, out var local))
        {
            Vector2 alvo = new Vector2(local.y / (140f / 6f), (local.x + 230) / 23f);
            Vector2 delta = alvo - new Vector2(xLancamento, .65f);
            if (delta.y > .15f) direcao = delta.normalized;
        }
        bool inicia = k.spaceKey.wasPressedThisFrame || (dentro && mouse.leftButton.wasPressedThisFrame);
        bool solta = k.spaceKey.wasReleasedThisFrame || (mouse != null && mouse.leftButton.wasReleasedThisFrame);
        bool cancela = k.cKey.wasPressedThisFrame || (mouse != null && mouse.rightButton.wasPressedThisFrame);
        if (cancela) { fase = Fase.Mira; forca = .25f; return; }
        if (fase == Fase.Mira && inicia) { fase = Fase.Carregando; subindo = true; }
        if (fase != Fase.Carregando) return;
        forca += (subindo ? 1 : -1) * Time.deltaTime * .62f;
        if (forca >= 1) { forca = 1; subindo = false; }
        if (forca <= .1f) { forca = .1f; subindo = true; }
        if (solta) Lancar(BochaSide.Marcus, xLancamento, direcao * (2.6f + forca * 10.4f));
    }

    private void Lancar(BochaSide lado, float x, Vector2 vel)
    {
        if (!partida.Lancar(sim, lado, x, vel)) return;
        fase = Fase.Rolagem; quieto = acumulador = 0;
    }

    private void ResolverMao()
    {
        fase = Fase.Resultado;
        BochaHandResult r = partida.Resolver(sim);
        AtualizarTela();
        string detalhe = r.resumo + "\n\nMARCUS " + partida.pontos[0] + " × " + partida.pontos[1] + " TONÃO\n" +
            "Medida entre as superfícies; tolerância de empate: 1 cm na escala da cancha.";
        if (pratica)
            MostrarModal("FIM DA PRÁTICA", detalhe + "\nO treino não salva nem concede ficha.",
                ("COMEÇAR DUELO", () => Comecar(false)), ("VOLTAR AO SALÃO", Fechar));
        else if (partida.Vencedor == BochaSide.Marcus)
            MostrarModal("MARCUS VENCE A PARTIDA", detalhe + "\n\nTonão deixa a bocha no suporte. Pela primeira vez, não propõe outra tentativa.",
                ("OUVIR TONÃO", () => { var cb = aoVencer; aoVencer = null; Fechar(); cb?.Invoke(); }));
        else if (partida.Vencedor == BochaSide.Tonao) Derrota();
        else
            MostrarModal(r.anulada ? "MÃO ANULADA" : "CONTAGEM DA MÃO", detalhe +
                (partida.Mao > 0 ? "\n\nAntes da próxima jogada: observe a faixa riscada de umidade. Ela não muda durante a mão e desacelera os dois lados." : ""),
                (r.anulada ? "REPETIR A MÃO" : "PRÓXIMA MÃO", () => { modal.SetActive(false); NovaMao(); }), ("REVER REGRAS", () => Regras(0, true)));
    }

    private void Derrota()
    {
        var opts = new List<(string, Action)>();
        if (CheckpointSystem.HasCheckpoint)
            opts.Add(("CARREGAR ÚLTIMO ORELHÃO", () => { Fechar(); CheckpointSystem.RecarregarUltimoSaveManual(); }));
        opts.Add(("MENU PRINCIPAL", () => { Fechar(); SceneFader.Instance.LoadScene("MainMenu"); }));
        MostrarModal("TONÃO VENCE — A CONVERSA SE FECHA",
            "A chuva volta a ocupar o silêncio. Marcus não consegue sustentar a pergunta.\n\n" +
            (CheckpointSystem.HasCheckpoint ? "Você pode carregar o último save manual. A partida não criou checkpoint." :
                "Não existe save manual válido. Nenhum ponto de recuperação foi criado."), opts.ToArray());
    }

    private void Pausar()
    {
        MostrarModal("BOCHA PAUSADA", "Bolas, força e turno ficam parados. Nenhum lançamento é consumido pela pausa.",
            ("CONTINUAR", () => modal.SetActive(false)), ("REGRAS", () => Regras(0)),
            (pratica ? "SAIR DA PRÁTICA" : "ENCERRAR DUELO", () => { if (pratica) Fechar(); else Derrota(); }));
    }

    private void OnApplicationFocus(bool focus)
    { if (!focus && IsShowing && (modal == null || !modal.activeSelf)) { if (fase == Fase.Carregando) fase = Fase.Mira; Pausar(); } }

    private void Regras(int pagina, bool resultado = false)
    {
        if (fase == Fase.Carregando) fase = Fase.Mira;
        string texto = pagina == 0 ?
            "4 bochas por lado; partida até 6 pontos. Lançamento usa uma bocha. Mão termina após até 8 lançamentos e repouso.\n\n" +
            "Depois da primeira bola, joga quem está mais distante, enquanto houver estoque. Se as melhores empatam (1 cm), joga quem não lançou por último.\n\n" +
            "Ao fim, vale 1 ponto por bocha estritamente mais próxima que a melhor rival. Empate das melhores: zero. Sem rival válido, contam suas válidas; sem válidas dos dois lados, zero." :
            "A/D: posição de saída; mouse: direção; segurar/soltar clique ou Espaço: força. C/direito cancelam antes de soltar. ESC pausa.\n\n" +
            "A força também permite retirar uma bocha ou deslocar o balim. Guias laterais rebatem pouco. As cabeceiras de madeira fecham a cancha: bochas e balim batem e voltam, perdendo velocidade. Encostar no fundo não elimina nenhuma bola.\n\n" +
            "Faixa riscada: atrito maior, anunciado antes da mão e igual para Marcus, Tonão e balim. Não muda durante uma jogada.";
        MostrarModal(pagina == 0 ? "REGRAS DA MÃO" : "CONTROLES E PISO", texto,
            (pagina == 0 ? "CONTROLES E PISO" : "PONTUAÇÃO", () => Regras(1 - pagina, resultado)),
            (resultado ? "PRÓXIMA MÃO" : "VOLTAR", () => { modal.SetActive(false); if (resultado) NovaMao(); }));
    }

    private void AtualizarTela()
    {
        if (sim == null) return;
        placar.text = (pratica ? "PRÁTICA" : "ATÉ 6") + "   MARCUS " + partida.pontos[0] + " × " + partida.pontos[1] + " TONÃO   •   MÃO " + (partida.Mao + (partida.Resolvida ? 0 : 1));
        estado.text = fase == Fase.Rolagem ? "Bolas em movimento — aguarde o repouso" : fase == Fase.Tonando ? "Vez de Tonão" :
            fase == Fase.Resultado ? "Mão resolvida" : "Sua vez — aproxime, proteja ou retire uma bocha";
        estoque.text = "MARCUS: " + (4 - partida.usadas[0]) + " bochas disponíveis     |     TONÃO: " + (4 - partida.usadas[1]) + "     •     B = balim";
        forcaFill.sizeDelta = new Vector2(226 * forca, 7);
        bool mirar = fase == Fase.Mira || fase == Fase.Carregando;
        mira.gameObject.SetActive(mirar); marcadorLancamento.gameObject.SetActive(mirar);
        Vector2 p = Ponto(new Vector2(xLancamento, .65f));
        marcadorLancamento.anchoredPosition = mira.anchoredPosition = p;
        mira.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direcao.x, direcao.y) * Mathf.Rad2Deg);
        while (bolasUI.Count < sim.bolas.Count)
        {
            var b = sim.bolas[bolasUI.Count];
            Color cor = b.lado == BochaSide.Marcus ? new Color(.36f, .66f, .71f) : b.lado == BochaSide.Tonao ? new Color(.83f, .51f, .29f) : new Color(.96f, .9f, .65f);
            var rt = Painel(cancha, "Bola_" + bolasUI.Count, Vector2.zero, Vector2.one * b.raio * 46f, Color.white);
            rt.GetComponent<Image>().sprite = Circulo(cor);
            Label(rt, "Lado", b.lado == BochaSide.Nenhum ? "B" : b.lado == BochaSide.Marcus ? "M" : "T", Vector2.zero, new Vector2(12, 12), 9.5f);
            bolasUI.Add(rt);
        }
        for (int i = 0; i < bolasUI.Count; i++) { bolasUI[i].gameObject.SetActive(sim.bolas[i].valida); bolasUI[i].anchoredPosition = Ponto(sim.bolas[i].pos); }
        if (!sim.IsMoving)
            medida.text = "MELHOR DISTÂNCIA — MARCUS " + TextoDistancia(BochaMatch.MelhorDistancia(sim, BochaSide.Marcus)) +
                "   •   TONÃO " + TextoDistancia(BochaMatch.MelhorDistancia(sim, BochaSide.Tonao));
        else medida.text = "Medida somente depois do repouso";
    }

    private static string TextoDistancia(float d) => float.IsPositiveInfinity(d) ? "—" : (d * 100f).ToString("F0") + " cm";
    private static Vector2 Ponto(Vector2 p) => new Vector2(-230 + p.y * 23f, p.x * (140f / 6f));
    public void Fechar() { if (modal != null) modal.SetActive(false); raiz.SetActive(false); }

    private void ImpactoReal(float forcaImpacto)
    {
        if (Time.unscaledTime - ultimoImpacto < .07f) return;
        ultimoImpacto = Time.unscaledTime;
        efeitos.PlayOneShot(impacto, Mathf.Clamp(forcaImpacto * .045f, .025f, .35f) * PlayerPrefs.GetFloat("audio_efeitos", 1));
        if (partida.Mao > 0 && !efeitoMao)
        { efeitoMao = true; percepcao.text = "Uma resposta abafada parece vir de outro salão. A cancha real permanece à vista."; }
    }

    private void MostrarModal(string titulo, string corpo, params (string texto, Action acao)[] opcoes)
    {
        if (modal != null) { modal.SetActive(false); Destroy(modal); }
        modal = Painel(raiz.transform, "ModalBocha", Vector2.zero, new Vector2(620, 344), new Color(.065f, .06f, .05f, .99f)).gameObject;
        modal.GetComponent<Image>().raycastTarget = true;
        Label(modal.transform, "Titulo", titulo, new Vector2(0, 145), new Vector2(570, 30), 20);
        var body = Label(modal.transform, "Texto", corpo, new Vector2(0, 20), new Vector2(560, 207), 15.5f);
        body.alignment = TextAlignmentOptions.TopLeft;
        for (int i = 0; i < opcoes.Length; i++)
        { var opt = opcoes[i]; Botao(modal.transform, opt.texto, new Vector2(0, -91 - i * 27), new Vector2(430, 23), () => opt.acao()); }
    }

    private static RectTransform Painel(Transform pai, string nome, Vector2 pos, Vector2 size, Color cor)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(Image)); go.transform.SetParent(pai, false);
        var r = (RectTransform)go.transform; r.anchoredPosition = pos; r.sizeDelta = size;
        var im = go.GetComponent<Image>(); im.color = cor; im.raycastTarget = false; return r;
    }
    private static TextMeshProUGUI Label(Transform pai, string nome, string texto, Vector2 pos, Vector2 size, float fonte)
    {
        var t = UIFactory.CreateLabel(pai, nome, fonte); t.text = texto; t.alignment = TextAlignmentOptions.Center;
        t.rectTransform.anchoredPosition = pos; t.rectTransform.sizeDelta = size; t.textWrappingMode = TextWrappingModes.Normal; return t;
    }
    private static void Botao(Transform pai, string texto, Vector2 pos, Vector2 size, Action click)
    {
        var r = Painel(pai, texto, pos, size, new Color(.24f, .21f, .15f)); var image = r.GetComponent<Image>(); image.raycastTarget = true;
        var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = image; b.onClick.AddListener(() => click());
        var label = Label(r, "Texto", texto, Vector2.zero, size, 13.5f); label.raycastTarget = false;
    }
    private static Sprite Circulo(Color cor)
    {
        const int n = 24; var tex = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        var px = new Color[n*n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        { float d = Vector2.Distance(new Vector2(x, y), new Vector2(11.5f, 11.5f)); px[y*n+x] = d > 11 ? Color.clear : d > 9 ? new Color(.1f,.1f,.09f) : cor * (.7f + y * .018f); }
        tex.SetPixels(px); tex.Apply(); return Sprite.Create(tex, new Rect(0,0,n,n), new Vector2(.5f,.5f), 32);
    }
    private static Sprite CriarPiso()
    {
        const int w = 460, h = 140; var tex = new Texture2D(w,h,TextureFormat.RGBA32,false) {filterMode=FilterMode.Point}; var p = new Color[w*h];
        var rng=new System.Random(1987);
        for(int y=0;y<h;y++) for(int x=0;x<w;x++) {float v=((float)rng.NextDouble()-.5f)*.035f; p[y*w+x]=new Color(.46f+v,.38f+v,.27f+v);}
        tex.SetPixels(p);tex.Apply();return Sprite.Create(tex,new Rect(0,0,w,h),new Vector2(.5f,.5f),32);
    }
}
