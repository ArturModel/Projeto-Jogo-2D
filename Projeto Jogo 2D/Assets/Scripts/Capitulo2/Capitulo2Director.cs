using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public sealed class C2InteractionData
{
    public string id, prompt, title, kind, art, flag, category, diary, source, ifFlag;
    public DialogoLinha[] lines, repeat, after, whenFlag;
    public C2DialogueBranch[] branches;
}
[Serializable] public sealed class C2DialogueBranch
{
    public string[] require, exclude;
    public DialogoLinha[] lines;
    public string flag, title, diary;
    public bool Disponivel => (require == null || Array.TrueForAll(require, GameState.Tem)) &&
        (exclude == null || Array.TrueForAll(exclude, f => !GameState.Tem(f)));
}
[Serializable] public sealed class C2ContentData { public C2InteractionData[] interactions; }

public sealed class Capitulo2Director : MonoBehaviour
{
    public const string Vitoria = "c2_venceu_tonao", Ficha = "ficha_clube_tonao_02";
    public static Capitulo2Director Instance { get; private set; }
    public string nomeLocal;
    public bool interior;
    public Vector2 limiteMin, limiteMax;
    public Vector2 uvNoMapa;
    public int assinaturaAudio;
    public SpriteRenderer memorialPercepcao;
    private PlayerController jogador;
    private static bool reconciliando;
    private static readonly Dictionary<string, C2InteractionData> conteudo = new Dictionary<string, C2InteractionData>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PrepararSessao()
    {
        Instance = null; reconciliando = false; conteudo.Clear();
        GameState.OnEstadoAlterado -= AtualizarInvestigacao;
        GameState.OnEstadoAlterado += AtualizarInvestigacao;
    }

    public static bool CenaDoCapitulo2(string nome) => SantaRitaGeography.CenaC2(nome);

    private void Awake()
    { Instance = this; CarregarConteudo(); }

    private IEnumerator Start()
    {
        yield return null; // Player.Start consome porta/checkpoint antes do enquadramento.
        jogador = FindAnyObjectByType<PlayerController>();
        if(jogador!=null&&jogador.GetComponent<SantaRitaRimLight>()==null)jogador.gameObject.AddComponent<SantaRitaRimLight>();
        if (SceneManager.GetActiveScene().name == "CentroSantaRita" && jogador != null &&
            jogador.transform.position.y > 83 && jogador.transform.position.x > 43)
        {
            // Saves antigos gravavam o cemitério dentro do Centro. Não mover nem sobrescrever o arquivo.
            SceneTransition.IrParaPosicao("CemiterioSantaRita", jogador.transform.position); yield break;
        }
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.Show("Zé ainda guarda a passagem da Rua Norte. Preciso falar com ele antes de seguir.", "MARCUS", "marcus",
                () => SceneTransition.Ir("EntradaDaCidade", "saida_bar_ze"));
            yield break;
        }
        GameState.Marcar("c2_iniciado");
        var camera = FindAnyObjectByType<CameraFollow>();
        if (camera != null) camera.DefinirLimitesRegiao(limiteMin, limiteMax, interior ? 4.7f : 5.5f, true);
        var chuva = FindAnyObjectByType<Chuva>(); if (chuva != null) chuva.DefinirModoInterior(interior);
        Capitulo2Audio.Definir(assinaturaAudio);
        MapaInterativoUI.EnsureInstance();
        // Veste os interiores de C2 com props autenticos existentes (Art/Props/).
        // So executa em cenas de interior; inofensivo se GameObjects esperados nao existirem.
        if (interior) SantaRitaPlaceDressing.ApplyC2();
        AtualizarInvestigacao();
    }

    private void OnDestroy()
    { if (Instance == this) Instance = null; }

    private void Update()
    {
        if (interior || SceneManager.GetActiveScene().name != "CentroSantaRita") return;
        if (jogador == null) return;
        Vector2 p = jogador.transform.position;
        int assinatura = p.y > 80 && p.x > 42 ? 1 : p.y < 17 && p.x < 25 ? 2 : 0;
        Capitulo2Audio.Definir(assinatura);
    }

    public static C2InteractionData Dados(string id)
    { CarregarConteudo(); return conteudo.TryGetValue(id, out var d) ? d : null; }
    private static void CarregarConteudo()
    {
        if (conteudo.Count > 0) return;
        var arquivo = Resources.Load<TextAsset>("Cap2/Investigation");
        if (arquivo == null) return;
        var dados = JsonUtility.FromJson<C2ContentData>(arquivo.text);
        if (dados?.interactions != null) foreach (var d in dados.interactions) conteudo[d.id] = d;
    }

    public static bool ProntoParaBocha => GameState.Tem("c2_protocolo_adilio") &&
        GameState.Tem("c2_registro_manutencao") && GameState.Tem("c2_padre_testemunho") && GameState.Tem("c2_cemiterio_comparado");

    public void Interagir(string id, Sprite arteExame = null)
    {
        if (GameMessageUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen) return;
        C2InteractionData d = Dados(id);
        if (d == null) { Debug.LogError("Interação C2 sem conteúdo: " + id); return; }
        if(id=="protocolo"&&!GameState.Tem("c2_prefeitura_atendimento"))
        {GameMessageUI.Instance.Show("Os papéis estão voltados para o lado da funcionária. Vou pedir para consultar o pedido do vô.","MARCUS","marcus");return;}
        if (d.kind == "bocha")
        {
            if (GameState.Tem(Vitoria)) { Dialogo(d.after); return; }
            if (!ProntoParaBocha)
            {
                string falta = !GameState.Tem("c2_protocolo_adilio") ?
                    "Ainda não sei nem o que meu avô pediu. Desafiar Tonão agora não vai tornar minha pergunta mais clara." :
                    !GameState.Tem("c2_registro_manutencao") ?
                    "O pedido de cópias do meu avô menciona manutenção. Preciso ler esse anexo antes de confrontar a versão de Tonão." :
                    !GameState.Tem("c2_padre_testemunho") ?
                    "O registro conta as tentativas, mas não diz quem conseguia ouvir Júlia. Preciso encontrar alguém que estivesse perto dela." :
                    "Ouvi o padre e li o registro. Quero passar pelo jazigo e organizar o que lembro antes de voltar a Tonão.";
                GameMessageUI.Instance.Show(falta, "MARCUS", "marcus"); return;
            }
            Dialogo(d.lines, () => BochaMinigameUI.Instance.Abrir(ConcluirBocha)); return;
        }
        if (d.kind == "radio")
        {
            int faixa = (GameState.Numero("c2_radio_faixa") + 1) % 3;
            GameState.DefinirNumero("c2_radio_faixa", faixa);
            Capitulo2Audio.TocarRadio(faixa);
            string[] opcoes = { "FM / sinal distante", "AM / estática", "LOCAL / retorno de teste" };
            GameMessageUI.Instance.ShowInspection("SELETOR DE FAIXA", "Radio", "Faixa selecionada: " + opcoes[faixa] +
                "\n\nA etiqueta de Mila pede FAIXA LOCAL: o retorno de teste deve soar em dois tons curtos.",
                () => { if (faixa == 2) { GameState.Marcar("c2_radio_restaurado"); Nota(d); } }, arteExame);
            return;
        }
        if (d.kind == "epilogo")
        {
            if (!GameState.Tem(Vitoria)) { Dialogo(d.lines); return; }
            GameMessageUI.Instance.Show(GameState.TemProgressoNaoSalvo ?
                "Tonão me deu a segunda ficha e uma pergunta para Amaro. O orelhão está ao lado da Sociedade. Preciso salvar manualmente se quiser retomar daqui." :
                "O relato de Tonão está no diário. Adílio buscava quem escolheu as palavras do registro. Preciso preparar a pergunta a Amaro. Posso continuar explorando a cidade e voltar pelos caminhos que já conheço.", "MARCUS", "marcus");
            return;
        }
        bool visto = !string.IsNullOrEmpty(d.flag) && GameState.Tem(d.flag);
        C2DialogueBranch ramo = null;
        if (d.branches != null) foreach (var b in d.branches)
            if (b.Disponivel) { ramo = b; break; }
        if (d.kind == "phone") PayphoneCheckpoint.TocarSomAtenderOrelhao();
        string reacao = "c2_reacao_vitoria_" + id;
        bool usarRamo = visto && ramo != null;
        bool usarReacao = visto && !usarRamo && GameState.Tem(Vitoria) && !GameState.Tem(reacao) && d.after?.Length > 0;
        DialogoLinha[] linhas = !visto ? d.lines : usarRamo ? ramo.lines : usarReacao ? d.after :
            visto && !string.IsNullOrEmpty(d.ifFlag) && GameState.Tem(d.ifFlag) && d.whenFlag?.Length > 0 ? d.whenFlag :
            visto && d.repeat?.Length > 0 ? d.repeat : d.lines;
        bool primeiraChamadaTardia=id=="telefone_rodo"&&!visto&&GameState.Tem(Vitoria);
        if(primeiraChamadaTardia)
        {
            var chamada=new List<DialogoLinha>(linhas);
            chamada.Add(new DialogoLinha("MARCUS","Leio também a pergunta que o vô deixou para Amaro. O plantonista anota junto do primeiro relato e confirma o número do guichê para retorno.","marcus"));
            linhas=chamada.ToArray();
        }
        Action concluir = () =>
        {
            if (!string.IsNullOrEmpty(d.flag)) GameState.Marcar(d.flag);
            Nota(d); AtualizarInvestigacao();
            if (usarReacao||primeiraChamadaTardia) GameState.Marcar(reacao);
            if (usarRamo)
            {
                if (!string.IsNullOrEmpty(ramo.flag)) GameState.Marcar(ramo.flag);
                if (!string.IsNullOrEmpty(ramo.diary)) GameState.RegistrarNotaDiario(ramo.flag,
                    ramo.title, "interpretacao", ramo.diary);
            }
            if (id == "padre") Ocorrencia("c2_sino_percebido", "Um sino soa baixo, mas o cordão está imóvel. O padre espera que Marcus retome a conversa.");
            if (id == "memorial" && GameState.Tem("c2_padre_testemunho"))
                Ocorrencia("c2_velas_percebidas", "No vidro, por um instante, as velas parecem apagadas. Ao olhar diretamente, as chamas continuam acesas.");
        };
        if (d.kind == "examine" || d.kind == "compare")
        {
            string texto = "";
            if (linhas != null) foreach (var l in linhas) texto += l.texto + "\n\n";
            GameMessageUI.Instance.ShowInspection(d.title, string.IsNullOrEmpty(d.art) ? "Bilhete" : d.art, texto.Trim(), concluir, arteExame);
        }
        else Dialogo(linhas, concluir);
    }

    private static void Nota(C2InteractionData d)
    {
        if (string.IsNullOrEmpty(d.diary)) return;
        GameState.RegistrarNotaDiario("c2_" + d.id, d.title, d.category,
            d.diary + (string.IsNullOrEmpty(d.source) ? "" : "\n\nFonte: " + d.source));
    }
    private static void Dialogo(DialogoLinha[] lines, Action cb = null)
    { if (lines == null || lines.Length == 0) cb?.Invoke(); else GameMessageUI.Instance.Show(lines, cb); }
    private static void Ocorrencia(string flag, string texto)
    {
        if (GameState.Tem(flag)) return;
        GameState.Marcar(flag);
        Capitulo2Perception.Tocar(flag);
        Legenda.Dizer(texto, 5.5f, Legenda.PrioridadeCritica);
        GameState.RegistrarNotaDiario(flag, "Percepção sem prova", "interpretação", texto + " Não encontrei uma alteração material que confirme a impressão.");
    }

    public static void AtualizarInvestigacao()
    {
        if (reconciliando || !GameState.Tem("c2_iniciado")) return;
        reconciliando = true;
        try
        {
            if (GameState.Tem("c2_memorial_lido") && GameState.Tem("c2_padre_testemunho"))
            {
                GameState.Marcar("c2_cemiterio_comparado");
                GameState.RegistrarNotaDiario("c2_comparacao_memoria", "Presença não foi socorro", "interpretação",
                    "O jazigo registra a morte, sem narrar as tentativas. O padre lembra de conversar com Júlia enquanto acreditava que alguém já havia chamado. A inscrição não confirma a cronologia; o testemunho contradiz uma inconsciência desde a queda, sem identificar quem deixou de telefonar.");
            }
            string objetivo;
            if (GameState.Tem(Vitoria)) objetivo = "Salvar na Beira-Rio. Preparar a pergunta a Amaro sobre as frases do registro.";
            else if (!GameState.Tem("c2_protocolo_adilio")) objetivo = GameState.Tem("c2_prefeitura_atendimento") ? "Procurar o pedido de Adílio entre as pastas indicadas no atendimento." : GameState.Tem("c2_conheceu_chico") ? "O jardineiro mencionou o atendimento público. Posso perguntar por Adílio." : "Descobrir por onde Adílio passou. Observar a praça e conversar com quem ainda está trabalhando.";
            else if (!GameState.Tem("c2_registro_manutencao")) objetivo = "Conferir os anexos disponíveis junto ao pedido do vô.";
            else if (!GameState.Tem("c2_padre_testemunho")) objetivo = "O registro não conta o que Júlia conseguia dizer. Preciso encontrar alguém que esteve com ela.";
            else if (!GameState.Tem("c2_cemiterio_comparado")) objetivo = "Passar pelo Cemitério e separar o que lembro de Júlia do que os relatos confirmam.";
            else objetivo = GameState.Tem("c2_conheceu_nair") || GameState.Tem("c2_conheceu_tonao") ? "Perguntar a Tonão sobre as tentativas descritas nos registros." : "Descobrir quem fazia os reparos e confrontar as tentativas com o registro.";
            GameState.DefinirObjetivo(objetivo);
        }
        finally { reconciliando = false; }
    }

    private void ConcluirBocha()
    {
        if (GameState.Tem(Vitoria)) return;
        GameState.Marcar(Vitoria); GameState.Marcar(Ficha); GameState.Marcar("c2_adilio_recado");
        GameState.RegistrarNotaDiario(Ficha, "Ficha da Sociedade Beira-Rio — Tonão", "item",
            "Segunda ficha: metal gasto com o nome da Sociedade e marcas de uso. Tonão a entregou depois da Bocha como apresentação entre os antigos jogadores; não é chave mágica nem moeda para salvar.");
        GameState.RegistrarNotaDiario("c2_revelacao_tentativas", "Júlia respondeu entre as tentativas", "fato",
            "Tonão admitiu que ouviu Júlia pedir ajuda, voltou a trocar cordas e insistiu no resgate próprio. Ela continuava consciente. O relato converge com o testemunho do padre e com as reposições registradas na manutenção. A cisterna ficava sob o Clube, não sob esta cancha.");
        GameState.RegistrarNotaDiario("c2_adilio_recado", "O recado anterior de Adílio", "pista",
            "Tonão entrega a via que guardou de um recado escrito a pedido de Adílio, antes do desaparecimento: 'Perguntar a Amaro quem escolheu as frases da versão pública'. Tonão conversou com Adílio na Sociedade e não testemunhou sua saída posterior. O recado indica uma intenção, não confirma que ele chegou ao Galpão ou onde está agora.");
        Dialogo(new[] {
            new DialogoLinha("TONÃO", "Eu ouvia ela. Perguntava se a água já chegava no degrau. Eu mudava a corda e dizia que agora ia.", "tonao"),
            new DialogoLinha("MARCUS", "Dava para tentar e chamar ajuda ao mesmo tempo.", "marcus"),
            new DialogoLinha("TONÃO", "Eu sei o que dava. Naquela hora, admitir que eu não conseguia parecia perder a única coisa que eu sabia fazer.", "tonao"),
            new DialogoLinha("MARCUS", "E o meu avô? Você falou com ele hoje.", "marcus"),
            new DialogoLinha("TONÃO", "Aqui, antes de sumir. Pediu que eu escrevesse de novo as palavras que aceitei no registro. Guardei esta via. Ele queria perguntar ao Amaro quem tinha escolhido aquelas frases. Não vi para onde foi depois.", "tonao"),
            new DialogoLinha("MARCUS", "Zé diz que o levaram pelo portão. Este papel não me diz quem fez isso. Mas explica o que o vô estava tentando comparar.", "marcus"),
            new DialogoLinha("TONÃO", "Leva a ficha e a via. Não vou te pedir outra mão para adiar a resposta.", "tonao")
        }, AtualizarInvestigacao);
    }

    public bool PosicaoNoMapa(Vector3 pos, out Vector2 uv, out string local)
    {
        local = nomeLocal;
        string scene = SceneManager.GetActiveScene().name;
        string landmark = SantaRitaGeography.IdDaCena(scene);
        uv = landmark != null ? SantaRitaGeography.UV(landmark) : uvNoMapa;
        if (!interior)
        {
            uv = SantaRitaGeography.MundoC2ParaUV(pos);
            local = SceneManager.GetActiveScene().name == "CemiterioSantaRita" ? "Cemitério municipal" : pos.y < 17 && pos.x < 25 ? "Sociedade Beira-Rio — Margem urbana" : nomeLocal;
        }
        return true;
    }
}
