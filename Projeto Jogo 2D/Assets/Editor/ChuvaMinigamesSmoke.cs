using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
internal static class ChuvaMinigamesSmoke
{
    private static int fase;
    private static double horario;
    private static AudioSource chuva;
    private static AudioSource menu;
    private static AudioSource exterior;
    private static int amostraPausa;

    static ChuvaMinigamesSmoke()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-srRainSmoke") < 0) return;
        if (EditorApplication.isPlaying) EditorApplication.update += Atualizar;
        else EditorApplication.playModeStateChanged += estado =>
        {
            if (estado == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.update -= Atualizar;
                EditorApplication.update += Atualizar;
            }
        };
    }

    public static void Run()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        EditorApplication.isPlaying = true;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Debug.Log("RAIN_QA PASS " + message);
    }

    private static void Atualizar()
    {
        if (EditorApplication.timeSinceStartup < horario) return;
        horario = EditorApplication.timeSinceStartup + 1.0;
        try
        {
            switch (fase++)
            {
                case 0:
                    menu = GameObject.Find("MenuChuva").GetComponent<AudioSource>();
                    Check(menu.volume > 0f, "chuva do menu antes da prática");
                    UnityEngine.Object.FindAnyObjectByType<SinucaMinigameUI>().IniciarPraticaOpcional();
                    break;
                case 1:
                    chuva = GameObject.Find("RaizSinuca").GetComponent<AudioSource>();
                    Check(chuva.isPlaying && chuva.loop && chuva.clip.name == "ChuvaMinigames" && chuva.volume > menu.volume,
                        "sinuca prática toca a gravação em loop");
                    Check(menu.volume == 0f, "prática substitui chuva do menu");
                    Time.timeScale = 0f;
                    break;
                case 2:
                    Check(!chuva.isPlaying, "chuva pausa com o jogo");
                    amostraPausa = chuva.timeSamples;
                    Time.timeScale = 1f;
                    break;
                case 3:
                    Check(chuva.isPlaying && chuva.timeSamples > amostraPausa, "chuva retoma o loop");
                    UnityEngine.Object.FindAnyObjectByType<SinucaMinigameUI>().Fechar();
                    break;
                case 4:
                    Check(!chuva.isPlaying && menu.volume > 0f, "fechar treino encerra gravação e restaura chuva do menu");
                    SceneManager.LoadScene("SociedadeBeiraRio");
                    horario += 1;
                    break;
                case 5:
                    exterior = GameObject.Find("SomChuvaA").GetComponent<AudioSource>();
                    Check(exterior.isPlaying && exterior.volume > 0, "chuva da Sociedade antes da bocha");
                    BochaMinigameUI.Instance.Abrir(null);
                    horario += 2;
                    break;
                case 6:
                    chuva = GameObject.Find("CanchaRaiz").GetComponent<AudioSource>();
                    Check(chuva.isPlaying && chuva.loop && chuva.clip.name == "ChuvaMinigames" && chuva.volume > .09f,
                        "bocha toca a gravação em loop");
                    Check(exterior.volume < .001f, "chuva da Sociedade cede à gravação");
                    BochaMinigameUI.Instance.Fechar();
                    break;
                case 7:
                    Check(!chuva.isPlaying && exterior.volume > .01f, "fechar bocha restaura chuva da Sociedade");
                    EditorApplication.Exit(0);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("RAIN_QA FAIL focus=" + Application.isFocused + " " + ex);
            EditorApplication.Exit(1);
        }
    }
}
