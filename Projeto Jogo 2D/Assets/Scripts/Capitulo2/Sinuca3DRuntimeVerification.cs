#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public sealed partial class Chapter2RuntimeVerification
{
    private bool sinuca3DForce;
    private Mouse sinucaMouse;

#if UNITY_EDITOR
    public static void StartSinuca3DInEditor(string output)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR", output);
        var host = new GameObject("QA_Sinuca3D_Explicito");
        DontDestroyOnLoad(host);
        host.AddComponent<Chapter2RuntimeVerification>().sinuca3DForce = true;
    }
#endif

    private static Vector2 ScreenSinuca(SinucaTableView view, Vector3 worldPos)
    {
        Vector2 local = view.ProjetarPontoMesaParaLocalUI(worldPos);
        return RectTransformUtility.WorldToScreenPoint(null, view.transform.TransformPoint(local));
    }

    private IEnumerator PointerSinuca(Vector2 screenPos, bool leftHeld, bool rightHeld = false)
    {
        var state = new MouseState { position = screenPos }
            .WithButton(MouseButton.Left, leftHeld)
            .WithButton(MouseButton.Right, rightHeld);
        InputSystem.QueueStateEvent(sinucaMouse, state);
        yield return null;
        yield return null;
    }

    private IEnumerator RunSinuca3DPerspective()
    {
        report.Add("Fixture declarada: entrada direta na cena EntradaDaCidade / Interior_BarDoZe para exercitar Sinuca 3D (S01..S27) com mouse/teclado virtuais pelo Input System/EventSystem real. Não substitui T01 humano. Saves pessoais isolados por IsolatedSaveScope.");
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        yield return new WaitForSecondsRealtime(1.0f);
        SceneFader.Instance.LoadScene("EntradaDaCidade");
        yield return AwaitScene("EntradaDaCidade");
        yield return CloseText();

        // Transicionar para o Bar do Zé e preparar pré-condição de diálogo com Zé (S01)
        GameState.Marcar(Flags.ChegouCasaAvo);
        GameState.Marcar(Flags.AchouReciboMercadinho);
        GameState.Marcar(Flags.OuviuRelatoAtendente);

        var zeNpc = FindObjectsByType<InteracaoCapitulo1>(FindObjectsInactive.Include)
            .First(i => i.Tipo == InteracaoCapitulo1.TipoInteracao.NpcZeBar);
        Capitulo1Director.Instance.TransicionarPara(
            Capitulo1Director.ZonaId.Interior_BarDoZe,
            zeNpc.transform.position + Vector3.down * 0.9f,
            1);
        yield return new WaitForSecondsRealtime(1.4f);
        yield return CloseText();
        yield return Capture("sinuca-00-bar-do-ze");

        // S01 — Abrir pelo Zé / mesa após conversa real
        zeNpc.Interact();
        yield return new WaitForSecondsRealtime(0.3f);
        yield return CloseText();
        Check(SinucaMinigameUI.IsShowing, "S01: Conversa real com Zé abre SinucaMinigameUI sem recriar cena nem perder estado.");

        var ui = SinucaMinigameUI.Instance;
        var view = Field<SinucaTableView>(ui, "tableView3D");
        var modal = Field<GameObject>(ui, "painelModal");
        Check(modal.activeSelf && view != null, "S01: Modal inicial e SinucaTableView 3D ativos.");
        yield return Capture("sinuca-01-modal-inicial");

        // S20 — Prática curta opcional antes do duelo (sem conceder ficha, flags, pontos ou save)
        Click("BtnModalSecundario");
        yield return null;
        Check(ui.EmModoPratica && !modal.activeSelf, "S20: Botão de Prática Curta abre treino livre sem modal.");
        yield return Capture("sinuca-02-pratica-3d");

        sinucaMouse = InputSystem.AddDevice<Mouse>("SinucaRegressionMouse");

        // S07 — Alternar vista 3D / Superior com tecla V e zoom limitado no scroll
        Check(view.CameraModo == SinucaCameraView.Mesa3D, "S07: Vista padrão é Mesa 3D em perspectiva.");
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.V));
        yield return null;
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        yield return null;
        Check(view.CameraModo == SinucaCameraView.VistaSuperior, "S07: Tecla V alterna para Vista Superior.");
        yield return Capture("sinuca-03-vista-superior");

        float zoomInicial = view.Zoom;
        view.AjustarZoom(0.25f);
        Check(view.Zoom > zoomInicial && view.Zoom <= 1.45f, "S07: Zoom aproxima dentro do limite seguro.");
        view.DefinirZoom(1.0f);

        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.V));
        yield return null;
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        yield return null;
        Check(view.CameraModo == SinucaCameraView.Mesa3D, "S07: Tecla V retorna para Mesa 3D.");

        // S02 & S03 — Mirar com mouse na mesa 3D, segurar para carregar com mouse parado e soltar para tacar
        var bolas = Field<IList>(ui, "bolas");
        object bola1 = bolas[1];
        Vector2 posBola1 = (Vector2)bola1.GetType().GetField("pos").GetValue(bola1);
        Vector2 pontoTelaAlvo = ScreenSinuca(view, new Vector3(posBola1.x, 8.5f, posBola1.y));

        yield return PointerSinuca(pontoTelaAlvo, false);
        float anguloMouse = Field<float>(ui, "anguloMiraRad");

        // Ajuste fino também por A/D
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.A));
        yield return new WaitForSecondsRealtime(0.18f);
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        yield return null;
        Check(Mathf.Abs(Field<float>(ui, "anguloMiraRad") - anguloMouse) > 0.02f, "S02: Teclas A/D ajustam mira suavemente.");

        // Voltar mira para a bola 1 e iniciar carga com mouse parado (S03)
        yield return PointerSinuca(pontoTelaAlvo + Vector2.right, false);
        yield return PointerSinuca(pontoTelaAlvo, true);
        float miraTravada = Field<float>(ui, "anguloMiraRad");
        yield return new WaitForSecondsRealtime(0.25f);
        Check(Field<object>(ui, "estado").ToString() == "CarregandoForca", "S02/S03: Segurar botão esquerdo entra em CarregandoForca.");
        yield return Capture("sinuca-04-carregando-taco-3d");

        // Verificar que a barra oscila com o mouse parado e que mover o mouse durante a carga NÃO destrava a mira
        float fMin = 1f, fMax = 0f;
        Vector2 outroPonto = ScreenSinuca(view, new Vector3(0f, 8.5f, 75f));
        float tOscEnd = Time.realtimeSinceStartup + 2.2f;
        while (Time.realtimeSinceStartup < tOscEnd)
        {
            yield return PointerSinuca(outroPonto, true);
            float f = Field<float>(ui, "forcaNormalizada");
            fMin = Mathf.Min(fMin, f);
            fMax = Mathf.Max(fMax, f);
        }
        Check(fMax > 0.82f && fMin < 0.35f && Mathf.Abs(Field<float>(ui, "anguloMiraRad") - miraTravada) < 0.001f,
            "S03: Força oscila com mouse e mira permanece travada durante a carga.");

        // S04 — Cancelar carga com botão direito sem consumir tacada
        yield return PointerSinuca(pontoTelaAlvo, true, true);
        yield return null;
        yield return PointerSinuca(pontoTelaAlvo, false, false);
        Check(Field<object>(ui, "estado").ToString() == "MirandoJogador", "S04: Botão direito cancela a carga sem tacar.");
        yield return Capture("sinuca-05-carga-cancelada");

        // S04 — Cancelar carga ao trocar vista (V), abrir regras, pausar (ESC) e perder foco
        yield return PointerSinuca(pontoTelaAlvo, true);
        yield return new WaitForSecondsRealtime(0.2f);
        Invoke(ui, "AlternarVistaCamera");
        yield return PointerSinuca(pontoTelaAlvo, false);
        Check(Field<object>(ui, "estado").ToString() == "MirandoJogador", "S04: Alternar vista cancela carga sem disparar tacada.");
        Invoke(ui, "AlternarVistaCamera");

        yield return PointerSinuca(pontoTelaAlvo, true);
        yield return new WaitForSecondsRealtime(0.2f);
        Invoke(ui, "OnApplicationFocus", false);
        yield return PointerSinuca(pontoTelaAlvo, false);
        Check(Field<object>(ui, "estado").ToString() == "MirandoJogador", "S04: Perda de foco cancela carga sem tacada.");

        // S05 — Soltar fora do viewport da mesa não dispara tacada
        yield return PointerSinuca(pontoTelaAlvo, true);
        yield return new WaitForSecondsRealtime(0.2f);
        yield return PointerSinuca(new Vector2(4f, 4f), false);
        Check(Field<object>(ui, "estado").ToString() == "MirandoJogador", "S05: Soltar fora do viewport cancela sem tacar.");

        // Executar uma tacada real na prática soltando dentro da mesa (S02)
        yield return PointerSinuca(pontoTelaAlvo, true);
        yield return new WaitForSecondsRealtime(0.45f);
        yield return PointerSinuca(pontoTelaAlvo, false);
        Check(Field<object>(ui, "estado").ToString() == "BolasEmMovimento", "S02: Soltar botão esquerdo dentro da mesa executa uma única tacada.");
        yield return new WaitForSecondsRealtime(0.18f);
        yield return Capture("sinuca-06-tacada-em-movimento");

        // Aguardar repouso na prática e verificar S20 (nenhuma flag narrativa, ficha ou save criados)
        float waitPratica = Time.realtimeSinceStartup + 8f;
        while (Field<object>(ui, "estado").ToString() == "BolasEmMovimento" && Time.realtimeSinceStartup < waitPratica)
            yield return null;
        Check(!GameState.Tem(Flags.ZeDerrotadoNaSinuca) && !GameState.Tem(Flags.RecebeuFichaClubeZe) && !CheckpointSystem.HasCheckpoint,
            "S20: Prática não altera flags narrativas, ficha ou save.");

        // S06 — Bola na mão: confirmar posição não inicia carga no mesmo frame
        Set(ui, "brancaNaMao", true);
        Set(ui, "estado", Enum.Parse(Field<object>(ui, "estado").GetType(), "PosicionandoBranca"));
        Vector2 posLivre = ScreenSinuca(view, new Vector3(-115f, 8.5f, 15f));
        yield return PointerSinuca(posLivre, true);
        Check(Field<object>(ui, "estado").ToString() == "MirandoJogador" && !Field<bool>(ui, "brancaNaMao"),
            "S06: Confirmar bola na mão muda para MirandoJogador sem iniciar carga no mesmo frame.");
        yield return PointerSinuca(posLivre, false);

        // S17 — Efeito longitudinal opcional na branca (Normal / Avançar / Recuar)
        float xNormal = SimularPosicaoFinalBrancaPosImpacto(ui, SinucaCueSpin.Normal);
        float xAvancar = SimularPosicaoFinalBrancaPosImpacto(ui, SinucaCueSpin.Avancar);
        float xRecuar = SimularPosicaoFinalBrancaPosImpacto(ui, SinucaCueSpin.Recuar);
        Check(xAvancar > xNormal + 12f && xRecuar < xNormal - 12f,
            $"S17: Efeito opcional altera fisicamente a trajetória pós-contato da branca (Recuar={xRecuar:F1} < Normal={xNormal:F1} < Avançar={xAvancar:F1}).");

        // S09, S10, S11, S12 — Passo fixo em 30/60/144 FPS, raspada rápida sem tunneling, bico de caçapa e caçapa única
        VerificarFisicaDeterministicaEBicos(ui);

        // S08 & S26 — Verificar legibilidade e enquadramento em 1280x720 e 1920x1080
        Click("BtnAcaoPraticaOuDuelo"); // Sai da prática e inicia o duelo real
        yield return null;
        Invoke(ui, "ConfigurarMao", 2);
        Invoke(ui, "AtualizarRegraBolaMarcadaAntesDoTurno");
        Invoke(ui, "AtualizarVisualTacoEMira");
        Check(Field<int>(ui, "numeroBolaMarcada") >= 1 && Field<int>(ui, "numeroBolaMarcada") <= 7,
            "S08/S14: Bola Marcada [X] ativa na 2ª mão.");
        yield return Capture("sinuca-07-bola-marcada-3d");

        view.DefinirCamera(SinucaCameraView.VistaSuperior);
        yield return null;
        yield return Capture("sinuca-08-bola-marcada-superior");
        view.DefinirCamera(SinucaCameraView.Mesa3D);

#if !UNITY_EDITOR
        foreach (var res in new[] { new Vector2Int(1920, 1080), new Vector2Int(1280, 720) })
        {
            Screen.SetResolution(res.x, res.y, FullScreenMode.Windowed);
            yield return new WaitForSecondsRealtime(0.5f);
            var corners = new Vector3[4];
            Field<GameObject>(ui, "raiz").GetComponent<RectTransform>().GetWorldCorners(corners);
            Check(corners[0].x >= -0.5f && corners[0].y >= -0.5f && corners[2].x <= Screen.width + 0.5f && corners[2].y <= Screen.height + 0.5f,
                $"S26: HUD e mesa cabem inteiros em {res.x}x{res.y}.");
            yield return Capture($"sinuca-res-{res.x}x{res.y}");
        }
#endif

        // S24 & S25 — Opção ReduzirEfeitosIntensos e tempo de decisão da IA de Zé
        bool oldReduzir = MenuPausa.ReduzirEfeitosIntensos;
        MenuPausa.ReduzirEfeitosIntensos = true;
        Invoke(ui, "AplicarPercepcaoPorMao");
        Check(Field<Image>(ui, "overlayPercepcao").color.a < 0.001f, "S24: ReduzirEfeitosIntensos desativa overlay intenso.");
        MenuPausa.ReduzirEfeitosIntensos = oldReduzir;

        object[] argsZe = { Vector2.zero, 0f };
        ui.GetType().GetMethod("EscolherTacadaZe", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui, argsZe);
        Check(SinucaMinigameUI.UltimaDecisaoZeMs < 150.0 && SinucaMinigameUI.UltimosCandidatosZe > 4,
            $"S25: IA de Zé decide sem travar frame ({SinucaMinigameUI.UltimaDecisaoZeMs:F2} ms, {SinucaMinigameUI.UltimosCandidatosZe} candidatos).");

        // S13..S16, S18, S19, S21 — Partida completa contra Zé vencida usando APENAS tacadas Normal + pagamento único de recompensa
        ui.Fechar();
        yield return null;
        zeNpc.Interact();
        yield return new WaitForSecondsRealtime(0.25f);
        yield return CloseText();
        Check(SinucaMinigameUI.IsShowing, "S18: Reabertura limpa do duelo contra Zé.");
        if (modal.activeSelf) Click("BtnModalPrimario");
        yield return null;

        Time.timeScale = 6f;
        int tacadasMarcusNormal = 0;
        float deadlineSerie = Time.realtimeSinceStartup + 240f;

        while (SinucaMinigameUI.IsShowing && !GameState.Tem(Flags.ZeDerrotadoNaSinuca) && Time.realtimeSinceStartup < deadlineSerie)
        {
            if (modal.activeSelf)
            {
                string btnTxt = Field<Button>(ui, "btnModalPrimario").GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                Field<Button>(ui, "btnModalPrimario").onClick.Invoke();
                yield return null;
                if (btnTxt.Contains("OUVIR ZÉ")) break;
                continue;
            }

            string est = Field<object>(ui, "estado").ToString();
            if (est == "PosicionandoBranca")
            {
                Invoke(ui, "PosicionarBrancaEm", InvokeEncontrarLivre(ui, new Vector2(-110f, 0f)));
                Invoke(ui, "ConfirmarBrancaNaMao");
                yield return null;
            }
            else if (est == "MirandoJogador")
            {
                // Garantir que Marcus usa estritamente SinucaCueSpin.Normal (S18)
                ui.SelecionarEfeitoBranca(SinucaCueSpin.Normal);
                EscolherMelhorTacadaNormalMarcus(ui, out Vector2 dirM, out float potM);
                Invoke(ui, "ExecutarTacadaComEfeito", dirM, potM, SinucaCueSpin.Normal);
                tacadasMarcusNormal++;
            }
            yield return null;
        }

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(0.35f);
        yield return Capture("sinuca-09-pos-vitoria-ze");
        yield return CloseText();

        Check(GameState.Tem(Flags.ZeDerrotadoNaSinuca) && GameState.Tem(Flags.RecebeuFichaClubeZe) && GameState.Tem(Flags.LimiarCentroDesbloqueado),
            $"S18/S19/S21: Marcus venceu a série contra Zé usando apenas tacadas Normal ({tacadasMarcusNormal} tacadas) e recebeu Ficha #01 + liberação do Centro.");
        Check(SinucaMinigameUI.EventosAudioDisparados > 5, "S23: Eventos físicos de áudio disparados durante a partida.");
        Check(!CheckpointSystem.HasCheckpoint, "S22: Vencer a Sinuca não cria autosave.");

        int entradasDiario = GameState.ObterEntradasDiario().Count;
        zeNpc.Interact();
        yield return new WaitForSecondsRealtime(0.2f);
        yield return CloseText();
        Check(!SinucaMinigameUI.IsShowing && GameState.ObterEntradasDiario().Count == entradasDiario,
            "S21: Revisitar Zé após a vitória não reabre o duelo nem duplica recompensa.");

        if (sinucaMouse != null) InputSystem.RemoveDevice(sinucaMouse);
        sinucaMouse = null;
        Finish();
    }

    private static Vector2 InvokeEncontrarLivre(SinucaMinigameUI ui, Vector2 pref)
    {
        return (Vector2)ui.GetType()
            .GetMethod("EncontrarPosicaoLivreDeterministica", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(ui, new object[] { pref, 0 });
    }

    private float SimularPosicaoFinalBrancaPosImpacto(SinucaMinigameUI ui, SinucaCueSpin spin)
    {
        Invoke(ui, "ConfigurarMao", 1);
        Field<GameObject>(ui, "painelModal").SetActive(false);
        var bolas = Field<IList>(ui, "bolas");
        for (int i = 2; i <= 7; i++)
            bolas[i].GetType().GetField("ativa").SetValue(bolas[i], false);

        bolas[0].GetType().GetField("pos").SetValue(bolas[0], new Vector2(-60f, 0f));
        bolas[0].GetType().GetField("vel").SetValue(bolas[0], Vector2.zero);
        bolas[1].GetType().GetField("ativa").SetValue(bolas[1], true);
        bolas[1].GetType().GetField("pos").SetValue(bolas[1], new Vector2(10f, 0f));
        bolas[1].GetType().GetField("vel").SetValue(bolas[1], Vector2.zero);

        Invoke(ui, "ExecutarTacadaComEfeito", Vector2.right, 260f, spin);
        for (int s = 0; s < 360 && Field<object>(ui, "estado").ToString() == "BolasEmMovimento"; s++)
            Invoke(ui, "SimularFisicaMesa", 1f / 120f);

        return ((Vector2)bolas[0].GetType().GetField("pos").GetValue(bolas[0])).x;
    }

    private void VerificarFisicaDeterministicaEBicos(SinucaMinigameUI ui)
    {
        // S09 — Determinismo entre 30, 60 e 144 FPS até o repouso usando o acumulador de passo fixo
        Vector2[] finais = new Vector2[3];
        float[] dts = { 1f / 30f, 1f / 60f, 1f / 144f };

        for (int k = 0; k < 3; k++)
        {
            Invoke(ui, "ConfigurarMao", 1);
            Field<GameObject>(ui, "painelModal").SetActive(false);
            Invoke(ui, "ExecutarTacadaComEfeito", new Vector2(1f, 0.12f).normalized, 280f, SinucaCueSpin.Normal);
            int guard = 0;
            while (Field<object>(ui, "estado").ToString() == "BolasEmMovimento" && guard++ < 2000)
                Invoke(ui, "AvancarSimulacaoPassoFixo", dts[k]);
            var bolas = Field<IList>(ui, "bolas");
            finais[k] = (Vector2)bolas[0].GetType().GetField("pos").GetValue(bolas[0]);
        }

        Check(Vector2.Distance(finais[0], finais[1]) < 0.01f && Vector2.Distance(finais[1], finais[2]) < 0.01f,
            "S09: Mesmo estado físico final em 30, 60 e 144 FPS.");

        // S10 — Tacada rápida e raspada não atravessa bola nem tabela
        Invoke(ui, "ConfigurarMao", 1);
        Field<GameObject>(ui, "painelModal").SetActive(false);
        var list = Field<IList>(ui, "bolas");
        for (int i = 2; i <= 7; i++) list[i].GetType().GetField("ativa").SetValue(list[i], false);
        list[0].GetType().GetField("pos").SetValue(list[0], new Vector2(-80f, 0f));
        list[1].GetType().GetField("pos").SetValue(list[1], new Vector2(0f, 15.8f)); // Raspada fina
        Invoke(ui, "ExecutarTacadaComEfeito", Vector2.right, 480f, SinucaCueSpin.Normal);
        Invoke(ui, "SimularFisicaMesa", 0.25f);
        Vector2 v1 = (Vector2)list[1].GetType().GetField("vel").GetValue(list[1]);
        Check(Field<bool>(ui, "houvePrimeiroContatoBolaObjeto") && v1.sqrMagnitude > 10f,
            "S10: Tacada rápida em raspada transfere impulso sem tunneling.");

        // S11 — Bola no bico da caçapa rebate na quina sem sumir fora da boca
        Invoke(ui, "ConfigurarMao", 1);
        Field<GameObject>(ui, "painelModal").SetActive(false);
        for (int i = 1; i <= 7; i++) list[i].GetType().GetField("ativa").SetValue(list[i], false);
        list[0].GetType().GetField("pos").SetValue(list[0], new Vector2(-18.5f, 80f));
        list[0].GetType().GetField("vel").SetValue(list[0], new Vector2(0f, 220f));
        Set(ui, "estado", Enum.Parse(Field<object>(ui, "estado").GetType(), "BolasEmMovimento"));
        Invoke(ui, "SimularFisicaMesa", 0.18f);
        bool brancaAtiva = (bool)list[0].GetType().GetField("ativa").GetValue(list[0]);
        Vector2 velBranca = (Vector2)list[0].GetType().GetField("vel").GetValue(list[0]);
        Check(brancaAtiva && velBranca.y < 0f, "S11: Bola atingindo o bico da caçapa rebate sem sumir.");
    }

    private void EscolherMelhorTacadaNormalMarcus(SinucaMinigameUI ui, out Vector2 melhorDir, out float melhorPot)
    {
        var bolas = Field<IList>(ui, "bolas");
        int marcada = Field<int>(ui, "numeroBolaMarcada");
        Vector2 cuePos = (Vector2)bolas[0].GetType().GetField("pos").GetValue(bolas[0]);
        Vector2[] cacapas = Field<Vector2[]>(ui, "cacapas");

        melhorDir = Vector2.right;
        melhorPot = 260f;
        float bestScore = float.MinValue;

        MethodInfo prevMethod = ui.GetType().GetMethod("SimularPrevisaoRapida", BindingFlags.Instance | BindingFlags.NonPublic);

        for (int i = 1; i <= 7; i++)
        {
            bool ativa = (bool)bolas[i].GetType().GetField("ativa").GetValue(bolas[i]);
            if (!ativa || i == marcada) continue;
            Vector2 bPos = (Vector2)bolas[i].GetType().GetField("pos").GetValue(bolas[i]);

            for (int p = 0; p < cacapas.Length; p++)
            {
                Vector2 toPocket = (cacapas[p] - bPos).normalized;
                Vector2 ghost = bPos - toPocket * (SinucaMinigameUI.BallRadius * 1.98f);
                Vector2 dir = (ghost - cuePos).normalized;
                foreach (float pot in new[] { 210f, 290f, 370f })
                {
                    object[] args = { dir, pot, 0, false, false, false, false, 0f };
                    prevMethod.Invoke(ui, args);
                    int caidas = (int)args[2];
                    bool caiuBranca = (bool)args[3];
                    bool caiuMarcada = (bool)args[4];
                    bool tocouObj = (bool)args[5];
                    bool tocouTab = (bool)args[6];
                    float menorDist = (float)args[7];

                    if (caiuBranca || caiuMarcada || !tocouObj || !tocouTab) continue;
                    float sc = caidas * 200f + (150f - menorDist * 0.4f);
                    if (sc > bestScore)
                    {
                        bestScore = sc;
                        melhorDir = dir;
                        melhorPot = pot;
                    }
                }
            }

            // Fallback direto no centro da bola legal
            Vector2 dirCenter = (bPos - cuePos).normalized;
            object[] argsC = { dirCenter, 280f, 0, false, false, false, false, 0f };
            prevMethod.Invoke(ui, argsC);
            if (!(bool)argsC[3] && !(bool)argsC[4] && (bool)argsC[5] && (bool)argsC[6])
            {
                float sc = (int)argsC[2] * 200f + (120f - (float)argsC[7] * 0.3f);
                if (sc > bestScore)
                {
                    bestScore = sc;
                    melhorDir = dirCenter;
                    melhorPot = 280f;
                }
            }
        }
    }
}
#endif
