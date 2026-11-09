using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A leitura acompanha o objeto no enquadramento da câmera ativa.
internal sealed class SantaRitaInspecao
{
    private readonly RectTransform tela, painel;
    private readonly CanvasGroup grupo, tituloGrupo;
    private readonly TextMeshProUGUI titulo, texto, ajuda;
    private Camera camera;
    private Bounds alvo;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private float lenteInicial, progresso, ateLetra;
    private int letras;
    private bool saindo, leuTudo;
    private Action aoFechar, acao;
    private string comando;
    private Vector2 tamanhoTela;

    internal bool Ativa { get; private set; }
    internal bool UsaCamera(Camera vista) => Ativa && camera == vista;

    internal SantaRitaInspecao(Canvas canvas)
    {
        tela = (RectTransform)canvas.transform;
        var fundo = UIFactory.CreateBox(tela, "LeituraAoLadoDoObjeto", null);
        fundo.color = MinigameDialogue.CorFundo;
        painel = fundo.rectTransform;
        AlinharNoTopo(painel);
        grupo = fundo.gameObject.AddComponent<CanvasGroup>();
        grupo.blocksRaycasts = grupo.interactable = false;
        titulo = CriarTexto("TituloDaLeitura", 15f, new Color32(231, 202, 147, 255));
        titulo.fontStyle = FontStyles.Bold;
        tituloGrupo = titulo.gameObject.AddComponent<CanvasGroup>();
        texto = CriarTexto("TextoDaLeitura", 15f, new Color32(247, 233, 208, 255));
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 10.5f;
        texto.fontSizeMax = 15f;
        ajuda = CriarTexto("ComandosDaLeitura", 11.5f, new Color32(220, 193, 149, 255));
        Ocultar();
    }

    private TextMeshProUGUI CriarTexto(string nome, float tamanho, Color cor)
    {
        var label = UIFactory.CreateLabel(painel, nome, tamanho);
        label.color = cor;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.textWrappingMode = TextWrappingModes.Normal;
        AlinharNoTopo(label.rectTransform);
        return label;
    }

    private static void AlinharNoTopo(RectTransform rect)
    {
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
    }

    internal void Abrir(Camera vista, Bounds objeto, string nome, string corpo,
        string pensamento, string etiqueta, Action terminou, string rotuloAcao, Action executar)
    {
        camera = vista;
        alvo = objeto;
        posicaoInicial = vista.transform.position;
        rotacaoInicial = vista.transform.rotation;
        lenteInicial = vista.fieldOfView;
        aoFechar = terminou;
        acao = executar;
        comando = rotuloAcao;
        saindo = leuTudo = false;
        progresso = ateLetra = 0f;
        letras = 0;
        tamanhoTela = Vector2.zero;
        Ativa = true;
        titulo.text = nome;
        texto.text = corpo;
        if (!string.IsNullOrEmpty(pensamento)) texto.text += "\n\n<i>" + pensamento + "</i>";
        if (!string.IsNullOrEmpty(etiqueta)) texto.text += "\n\n<size=80%><color=#C7AC81>" + etiqueta + "</color></size>";
        texto.maxVisibleCharacters = 0;
        SantaRitaSound.Play("page", .16f, .88f);
    }

    internal bool Enquadramento2D(Camera vista, out Bounds objeto, out float intensidade)
    {
        objeto = alvo;
        intensidade = CurvaDaCamera(progresso);
        return Ativa && camera == vista && camera.orthographic;
    }

    // Velocidade e aceleração nulas nas duas pontas do movimento.
    internal static float CurvaDaCamera(float progresso) =>
        progresso * progresso * progresso * (progresso * (progresso * 6f - 15f) + 10f);

    internal void Avancar()
    {
        if (saindo) return;
        if (!leuTudo)
        {
            leuTudo = true;
            texto.maxVisibleCharacters = int.MaxValue;
            return;
        }
        if (progresso < 1f) return;
        saindo = true;
    }

    internal void Confirmar()
    {
        if (saindo || acao == null || progresso < 1f || !leuTudo) return;
        var executar = acao;
        acao = null;
        saindo = true;
        executar();
    }

    internal static float MoverProgresso(float progresso, bool saindo, float dt)
    {
        float duracao = MenuPausa.ReduzirEfeitosIntensos ? .22f : saindo ? .57f : .80f;
        return Mathf.MoveTowards(progresso, saindo ? 0f : 1f, dt / duracao);
    }

    internal void Atualizar(float dt)
    {
        progresso = MoverProgresso(progresso, saindo, dt);
        if (saindo && progresso == 0f)
        {
            var terminou = aoFechar;
            Cancelar();
            terminou?.Invoke();
        }
        if (Ativa && !saindo && progresso >= .66f && !leuTudo && painel.gameObject.activeSelf)
            Digitar(dt);
    }

    private void Digitar(float dt)
    {
        ateLetra -= dt;
        int total = texto.textInfo.characterCount;
        while (ateLetra <= 0f && letras < total)
        {
            char caractere = texto.textInfo.characterInfo[letras++].character;
            ateLetra += caractere switch
            {
                '.' or '!' or '?' => .16f,
                ',' or ';' or ':' => .075f,
                '\n' => .10f,
                ' ' => .005f,
                _ => .022f
            };
        }
        texto.maxVisibleCharacters = letras;
        leuTudo = letras >= total && total > 0;
    }

    internal void Compor(bool visivel)
    {
        if (!Ativa || camera == null) return;
        if (!camera.orthographic)
        {
            float t = CurvaDaCamera(progresso);
            Vector3 direcao = alvo.center - posicaoInicial;
            Vector3 posicao = posicaoInicial + direcao.normalized * Mathf.Min(.27f, direcao.magnitude * .16f) * t;
            float fov = lenteInicial * .76f;
            float deslocamento = Vector3.Distance(posicao, alvo.center) *
                Mathf.Tan(fov * .5f * Mathf.Deg2Rad) * camera.aspect * .44f;
            Vector3 direita = rotacaoInicial * Vector3.right;
            Quaternion olhar = Quaternion.LookRotation(alvo.center + direita * deslocamento - posicao, Vector3.up);
            camera.transform.SetPositionAndRotation(posicao, Quaternion.Slerp(rotacaoInicial, olhar, t));
            camera.fieldOfView = Mathf.Lerp(lenteInicial, fov, t);
        }

        if (!visivel || progresso < .43f)
        {
            Ocultar();
            return;
        }

        painel.gameObject.SetActive(true);
        Vector2 dimensoes = tela.rect.size;
        if (tamanhoTela != dimensoes) AjustarPainel(dimensoes);
        var centro = camera.WorldToViewportPoint(alvo.center);
        float x = dimensoes.x * Mathf.Clamp(centro.x + .18f, .47f, .51f);
        float meio = dimensoes.y * Mathf.Clamp(1f - centro.y, .44f, .58f);
        float altura = painel.rect.height;
        float topo = Mathf.Clamp(meio - altura * .5f, dimensoes.y * .09f, dimensoes.y * .92f - altura);
        float entrada = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.43f, .88f, progresso));
        painel.anchoredPosition = new Vector2(x + (1f - entrada) * 7f, -topo);
        grupo.alpha = entrada;
        tituloGrupo.alpha = Mathf.InverseLerp(.47f, .72f, progresso);
        ajuda.text = leuTudo && acao != null ? "[R] " + comando + "   [E] VOLTAR" :
            leuTudo ? "[E] VOLTAR" : "[E] COMPLETAR TEXTO";
    }

    private void AjustarPainel(Vector2 dimensoes)
    {
        tamanhoTela = dimensoes;
        const float margem = 12f;
        float largura = dimensoes.x * .38f;
        float interna = largura - 2f * margem;
        float alturaTitulo = Mathf.Max(18f, titulo.GetPreferredValues(titulo.text, interna, 0f).y);
        float corpoY = margem + alturaTitulo + 7f;
        float limite = dimensoes.y * .80f - corpoY - 27f;
        float alturaCorpo = Mathf.Clamp(texto.GetPreferredValues(texto.text, interna, 0f).y + 3f, 23f, limite);
        painel.sizeDelta = new Vector2(largura, corpoY + alturaCorpo + 27f);
        titulo.rectTransform.anchoredPosition = new Vector2(margem, -margem);
        titulo.rectTransform.sizeDelta = new Vector2(interna, alturaTitulo);
        texto.rectTransform.anchoredPosition = new Vector2(margem, -corpoY);
        texto.rectTransform.sizeDelta = new Vector2(interna, alturaCorpo);
        ajuda.rectTransform.anchoredPosition = new Vector2(margem, -corpoY - alturaCorpo - 7f);
        ajuda.rectTransform.sizeDelta = new Vector2(interna, 18f);
        texto.ForceMeshUpdate();
    }

    private void Ocultar() => painel.gameObject.SetActive(false);

    internal void Cancelar()
    {
        if (!Ativa) return;
        if (camera != null && !camera.orthographic)
        {
            camera.transform.SetPositionAndRotation(posicaoInicial, rotacaoInicial);
            camera.fieldOfView = lenteInicial;
        }
        Ativa = false;
        camera = null;
        aoFechar = acao = null;
        Ocultar();
    }
}
