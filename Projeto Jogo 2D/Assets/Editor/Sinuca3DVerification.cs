using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Verificação automatizada de contrato, física, controles, opções e regressão da Sinuca 3D (S01..S27).
/// Autoria/teste fora do Player normal; usa sempre IsolatedSaveScope para proteger saves pessoais.
/// </summary>
public static class Sinuca3DVerification
{
    [MenuItem("Santa Rita/Capítulo 1/Validar Sinuca 3D e invariantes (isolado)")]
    public static void RunMenu()
    {
        string res = Run();
        Debug.Log(res);
    }

    public static string Run()
    {
        var report = new List<string>();
        int checks = 0;

        void Check(bool ok, string label)
        {
            report.Add((ok ? "PASS " : "FAIL ") + label);
            if (!ok) throw new Exception("Falha em Sinuca3DVerification: " + label);
            checks++;
        }

        using (CheckpointSystem.BeginIsolatedTests())
        {
            GameState.ImportarSnapshot(new GameStateSnapshot());
            var host = new GameObject("Sinuca3D_EditMode_QA_Host");
            try
            {
                var ui = host.AddComponent<SinucaMinigameUI>();
                ui.IniciarDuelo(null, null);

                // 1. Invariantes canônicas R1 (7 bolas, faltas, marcada, série 2-0 / 2-1)
                Check(ui.SimularTesteDeInvariantes(out string relInvariantes), "S13/S14/S15/S16/S19: " + relInvariantes);

                // 2. Verificar SinucaTableView 3D e Vista Superior (S07 / S08)
                var view = Field<SinucaTableView>(ui, "tableView3D");
                Check(view != null, "S07: SinucaTableView 3D instanciado no Canvas.");
                view.DefinirCamera(SinucaCameraView.Mesa3D);
                Vector2 pLeft3D = view.ProjetarPontoMesaParaLocalUI(new Vector3(-120f, 8.5f, 0f));
                Vector2 pRight3D = view.ProjetarPontoMesaParaLocalUI(new Vector3(120f, 8.5f, 0f));
                Check(pLeft3D.x < pRight3D.x, "S07: Projeção 3D preserva orientação espacial esquerda-direita.");

                view.DefinirCamera(SinucaCameraView.VistaSuperior);
                Vector2 pLeftTop = view.ProjetarPontoMesaParaLocalUI(new Vector3(-120f, 8.5f, 0f));
                Vector2 pRightTop = view.ProjetarPontoMesaParaLocalUI(new Vector3(120f, 8.5f, 0f));
                Check(pLeftTop.x < pRightTop.x, "S07: Vista Superior preserva orientação espacial.");

                view.DefinirZoom(1.0f);
                view.AjustarZoom(0.3f);
                Check(view.Zoom > 1.0f && view.Zoom <= 1.45f, "S07: Zoom limitado aproxima com segurança.");
                view.DefinirZoom(1.0f);
                view.DefinirCamera(SinucaCameraView.Mesa3D);

                // 3. Oscilação de carga com mouse parado (S03)
                bool subindo = true;
                float f = 0.08f;
                float maxF = f, minF = 1f;
                int inversoes = 0;
                for (int i = 0; i < 240; i++)
                {
                    bool ant = subindo;
                    f = SinucaMinigameUI.OscilarCarga(f, 1f / 60f, ref subindo);
                    maxF = Mathf.Max(maxF, f);
                    if (i > 60) minF = Mathf.Min(minF, f);
                    if (ant != subindo) inversoes++;
                }
                Check(maxF >= 0.99f && minF <= 0.12f && inversoes >= 3,
                    "S03: Barra de força oscila continuamente com mouse parado.");

                // 4. Guia de mira discreta usa o mesmo modelo geométrico da partida (M3 / Seção 6)
                Invoke(ui, "ConfigurarMao", 1);
                var bolas = Field<IList>(ui, "bolas");
                for (int i = 2; i <= 7; i++) SetBall(bolas[i], "ativa", false);
                SetBall(bolas[0], "pos", new Vector2(-100f, 0f));
                SetBall(bolas[1], "ativa", true);
                SetBall(bolas[1], "pos", new Vector2(0f, 0f));
                SinucaAimGuideData guia = ui.CalcularGuiaDeMira(new Vector2(-100f, 0f), 0f, SinucaCueSpin.Normal);
                Check(guia.ativa && guia.atingeBola && guia.numeroBolaAlvo == 1 &&
                      Mathf.Abs(guia.pontoContato.x - (-SinucaMinigameUI.BallRadius * 2f)) < 0.25f,
                      "Guia de mira calcula contato exato no raio combinado (2*R) sem atravessar a bola.");

                // 5. Passo fixo determinístico entre 30, 60 e 120 FPS (S09)
                Vector2[] posFinais = new Vector2[3];
                float[] dts = { 1f / 30f, 1f / 60f, 1f / 120f };
                int[] counts = { 60, 120, 240 }; // 2 segundos físicos exatos (240 passos de 1/120s)
                for (int k = 0; k < 3; k++)
                {
                    Invoke(ui, "ConfigurarMao", 1);
                    Field<GameObject>(ui, "painelModal").SetActive(false);
                    Invoke(ui, "ExecutarTacadaComEfeito", new Vector2(1f, 0.15f).normalized, 320f, SinucaCueSpin.Normal);
                    for (int s = 0; s < counts[k]; s++)
                        Invoke(ui, "AvancarSimulacaoPassoFixo", dts[k]);
                    posFinais[k] = GetBall<Vector2>(bolas[0], "pos");
                }
                Check(Vector2.Distance(posFinais[0], posFinais[1]) < 0.01f &&
                      Vector2.Distance(posFinais[1], posFinais[2]) < 0.01f,
                      "S09: Simulação física produz o mesmo resultado em 30, 60 e 120 FPS.");

                // Verificar também convergência até o repouso em 30, 60 e 144 FPS
                Vector2[] repousoFinais = new Vector2[3];
                float[] dtsRepouso = { 1f / 30f, 1f / 60f, 1f / 144f };
                for (int k = 0; k < 3; k++)
                {
                    Invoke(ui, "ConfigurarMao", 1);
                    Field<GameObject>(ui, "painelModal").SetActive(false);
                    Invoke(ui, "ExecutarTacadaComEfeito", new Vector2(1f, 0.15f).normalized, 280f, SinucaCueSpin.Normal);
                    int guard = 0;
                    while (Field<object>(ui, "estado").ToString() == "BolasEmMovimento" && guard++ < 2000)
                        Invoke(ui, "AvancarSimulacaoPassoFixo", dtsRepouso[k]);
                    repousoFinais[k] = GetBall<Vector2>(bolas[0], "pos");
                }
                Check(Vector2.Distance(repousoFinais[0], repousoFinais[1]) < 0.01f &&
                      Vector2.Distance(repousoFinais[1], repousoFinais[2]) < 0.01f,
                      "S09b: Estado de repouso final idêntico em 30, 60 e 144 FPS.");

                // 6. Colisão rápida e raspada sem tunneling (S10)
                Invoke(ui, "ConfigurarMao", 1);
                Field<GameObject>(ui, "painelModal").SetActive(false);
                for (int i = 2; i <= 7; i++) SetBall(bolas[i], "ativa", false);
                SetBall(bolas[0], "pos", new Vector2(-90f, 0f));
                SetBall(bolas[1], "ativa", true);
                SetBall(bolas[1], "pos", new Vector2(0f, 15.6f));
                Invoke(ui, "ExecutarTacadaComEfeito", Vector2.right, 480f, SinucaCueSpin.Normal);
                Invoke(ui, "SimularFisicaMesa", 0.22f);
                Check(Field<bool>(ui, "houvePrimeiroContatoBolaObjeto") && GetBall<Vector2>(bolas[1], "vel").sqrMagnitude > 8f,
                    "S10: Tacada rápida em raspada transfere impulso sem atravessar bola.");

                // 7. Bola no bico da caçapa rebate sem desaparecer fora da boca (S11)
                Invoke(ui, "ConfigurarMao", 1);
                Field<GameObject>(ui, "painelModal").SetActive(false);
                for (int i = 1; i <= 7; i++) SetBall(bolas[i], "ativa", false);
                SetBall(bolas[0], "pos", new Vector2(-18.2f, 82f));
                SetBall(bolas[0], "vel", new Vector2(0f, 210f));
                Set(ui, "estado", Enum.Parse(Field<object>(ui, "estado").GetType(), "BolasEmMovimento"));
                Invoke(ui, "SimularFisicaMesa", 0.16f);
                Check(GetBall<bool>(bolas[0], "ativa") && GetBall<Vector2>(bolas[0], "vel").y < 0f,
                    "S11: Bola atingindo o bico da caçapa rebate na quina sem sumir.");

                // 8. Caçapa válida contada exatamente uma vez e invariante das 7 bolas preservada (S12 / S13)
                Invoke(ui, "ConfigurarMao", 1);
                Field<GameObject>(ui, "painelModal").SetActive(false);
                SetBall(bolas[0], "pos", new Vector2(0f, 42f));
                SetBall(bolas[1], "pos", new Vector2(0f, 73f));
                Invoke(ui, "ExecutarTacadaComEfeito", Vector2.up, 185f, SinucaCueSpin.Normal);
                for (int s = 0; s < 900 && Field<object>(ui, "estado").ToString() == "BolasEmMovimento"; s++)
                    Invoke(ui, "SimularFisicaMesa", 1f / 120f);
                Check(Field<int>(ui, "pontosMarcus") == 1 && ui.ValidarInvarianteSeteBolas(),
                    "S12/S13: Bola encaçapada conta 1 ponto único e preserva invariante das 7 bolas.");

                // 9. Efeito longitudinal opcional na branca: Normal vs Avançar vs Recuar (S17)
                float xNorm = PosicaoFinalBrancaComEfeito(ui, bolas, SinucaCueSpin.Normal);
                float xAvan = PosicaoFinalBrancaComEfeito(ui, bolas, SinucaCueSpin.Avancar);
                float xRec = PosicaoFinalBrancaComEfeito(ui, bolas, SinucaCueSpin.Recuar);
                Check(xAvan > xNorm + 12f && xRec < xNorm - 12f,
                    $"S17: Efeito longitudinal altera fisicamente o comportamento pós-contato (Recuar={xRec:F1} < Normal={xNorm:F1} < Avançar={xAvan:F1}).");

                // 10. Prática curta opcional não concede ficha, flags, pontos de série ou save (S20 / S22)
                ui.IniciarPraticaOpcional();
                Check(ui.EmModoPratica && !GameState.Tem(Flags.ZeDerrotadoNaSinuca) && !CheckpointSystem.HasCheckpoint,
                    "S20/S22: Modo de prática inicia limpo sem flags nem save.");

                // 11. Tempo de decisão da IA de Zé sem travar frame (S25)
                Invoke(ui, "ConfigurarMao", 2);
                Invoke(ui, "AtualizarRegraBolaMarcadaAntesDoTurno");
                object[] argsZe = { Vector2.zero, 0f };
                ui.GetType().GetMethod("EscolherTacadaZe", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui, argsZe);
                Check(SinucaMinigameUI.UltimaDecisaoZeMs < 150.0 && SinucaMinigameUI.UltimosCandidatosZe > 4,
                    $"S25: IA de Zé decide em {SinucaMinigameUI.UltimaDecisaoZeMs:F2} ms ({SinucaMinigameUI.UltimosCandidatosZe} candidatos).");

                ui.Fechar();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        string outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Docs/Evidencias/Sinuca3D"));
        Directory.CreateDirectory(outDir);
        File.WriteAllLines(Path.Combine(outDir, "verificacao-editmode-s01-s27.txt"), report);
        return $"PASS {checks} verificações de Sinuca 3D (S01..S27 EditMode).";
    }

    private static float PosicaoFinalBrancaComEfeito(SinucaMinigameUI ui, IList bolas, SinucaCueSpin spin)
    {
        Invoke(ui, "ConfigurarMao", 1);
        Field<GameObject>(ui, "painelModal").SetActive(false);
        for (int i = 2; i <= 7; i++) SetBall(bolas[i], "ativa", false);
        SetBall(bolas[0], "pos", new Vector2(-60f, 0f));
        SetBall(bolas[0], "vel", Vector2.zero);
        SetBall(bolas[1], "ativa", true);
        SetBall(bolas[1], "pos", new Vector2(10f, 0f));
        SetBall(bolas[1], "vel", Vector2.zero);

        Invoke(ui, "ExecutarTacadaComEfeito", Vector2.right, 260f, spin);
        for (int s = 0; s < 360 && Field<object>(ui, "estado").ToString() == "BolasEmMovimento"; s++)
            Invoke(ui, "SimularFisicaMesa", 1f / 120f);

        return GetBall<Vector2>(bolas[0], "pos").x;
    }

    private static T Field<T>(object o, string n) =>
        (T)o.GetType().GetField(n, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o);

    private static void Set(object o, string n, object v) =>
        o.GetType().GetField(n, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(o, v);

    private static void Invoke(object o, string n, params object[] args) =>
        o.GetType().GetMethod(n, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(o, args);

    private static T GetBall<T>(object b, string f) =>
        (T)b.GetType().GetField(f, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(b);

    private static void SetBall(object b, string f, object v) =>
        b.GetType().GetField(f, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(b, v);
}
