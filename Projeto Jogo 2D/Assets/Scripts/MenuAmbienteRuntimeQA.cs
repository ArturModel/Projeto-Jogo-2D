#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Fixture explícita por --menu-qa ou Begin() no Editor. Inexistente no player
// normal. Input passa pelo InputSystem/UIInputModule, nunca por Button.onClick.
public sealed class MenuAmbienteRuntimeQA : MonoBehaviour
{
    IDisposable saves;
    readonly System.Collections.Generic.List<string> resultados = new System.Collections.Generic.List<string>();
    string pasta;
    float brilhoOriginal;
    bool tinhaBrilho;
    readonly System.Collections.Generic.Dictionary<string, int?> preferenciasVideo = new System.Collections.Generic.Dictionary<string, int?>();
    InputSettings.BackgroundBehavior comportamentoOriginal;
    bool runInBackgroundOriginal;
    int falhas;
    public static bool Rodando { get; private set; }
    public static string Resultado { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Environment.GetCommandLineArgs().Contains("--menu-qa")) Begin();
    }

    public static void Begin()
    {
        if (Rodando || !Application.isPlaying) return;
        Rodando = true;
        var go = new GameObject("MenuAmbienteQA_OptIn");
        DontDestroyOnLoad(go);
        go.AddComponent<MenuAmbienteRuntimeQA>().StartCoroutine("ExecutarProtegido");
    }

    IEnumerator ExecutarProtegido()
    {
        pasta = "C:/Temp/SantaRitaMenu/Evidencias";
        Directory.CreateDirectory(pasta);
        comportamentoOriginal = InputSystem.settings.backgroundBehavior;
        runInBackgroundOriginal = Application.runInBackground;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        Application.runInBackground = true;
        tinhaBrilho = PlayerPrefs.HasKey("video_brilho");
        brilhoOriginal = PlayerPrefs.GetFloat("video_brilho", 1);
        foreach (var chave in new[] { "video_fullscreen", "video_largura", "video_altura" })
            preferenciasVideo[chave] = PlayerPrefs.HasKey(chave) ? PlayerPrefs.GetInt(chave) : (int?)null;
        if (!Application.isEditor)
        {
            PlayerPrefs.SetInt("video_fullscreen", 0);
            PlayerPrefs.SetInt("video_largura", Screen.width);
            PlayerPrefs.SetInt("video_altura", Screen.height);
        }
        saves = CheckpointSystem.BeginIsolatedTests();
        var pilha = new System.Collections.Generic.Stack<IEnumerator>();
        pilha.Push(Roteiro());
        while (pilha.Count > 0)
        {
            object atual = null; bool continua;
            var rotina = pilha.Peek();
            try { continua = rotina.MoveNext(); if (continua) atual = rotina.Current; }
            catch (Exception e) { Registrar(false, "Exceção: " + e); break; }
            if (!continua) { pilha.Pop(); continue; }
            if (atual is IEnumerator interna) pilha.Push(interna);
            else yield return atual;
        }
        Limpar();
        Resultado = falhas == 0 ? "PASS" : "FAIL";
        resultados.Add(Resultado + ": " + resultados.Count + " verificações; input injetado no InputSystem; saves isolados; preferências restauradas.");
        File.WriteAllLines(Path.Combine(pasta, "resultado.txt"), resultados);
        Debug.Log("MENU QA " + Resultado);
        Rodando = false;
        if (!Application.isEditor) Application.Quit(falhas == 0 ? 0 : 1);
        Destroy(gameObject);
    }

    IEnumerator Roteiro()
    {
        CheckpointSystem.SaveSlot(2, "EntradaDaCidade", Vector2.zero, "menu_qa", "QA — registro isolado");
        SceneManager.LoadScene("MainMenu");
        yield return Esperar(.8f);
        var a = FindAnyObjectByType<MenuAmbiente>();
        Registrar(a != null && a.Pronto, "Menu inicializado com arte nova");
        Registrar(CheckpointSystem.EmArmazenamentoDeTeste, "Armazenamento isolado ativo");
        Registrar(!FindObjectsByType<UnityEngine.Video.VideoPlayer>(FindObjectsInactive.Exclude).Any(v => v.enabled && v.isPlaying), "Vídeo original preservado e inativo");
        Registrar(Resources.Load<Texture2D>("MenuAmbiente/Rua").width >= 1600, "Imagem detalhada em resolução nativa superior ao vídeo original");
        Registrar(!GameObject.Find("VideoBackground").GetComponent<RawImage>().raycastTarget, "Fundo não intercepta UI");
        Registrar(GameObject.Find("OrelhaoPrimeiroPlano") != null, "Orelhão independente do cenário");
        yield return Hover("btn_novojogo");
        Registrar(a.FocoAtual == "Rua", "Novo jogo dirige atenção à rua");
        yield return Capturar("01-Principal");
        yield return Hover("btn_continuar");
        Registrar(a.FocoAtual == "Orelhao", "Hover em Continuar destaca o orelhão");
        yield return Capturar("02-Continuar");
        // Afastar ponteiro sem trocar a seleção, para isolar o evento de teclado.
        Mover(new Vector2(Screen.width * .4f, Screen.height * .9f));
        yield return null;
        yield return Tecla(Key.DownArrow);
        Registrar(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.name == "btn_opcoes", "Seta navega de Continuar a Configurações");
        yield return Tecla(Key.Enter);
        yield return Esperar(.5f);
        Registrar(Ativo("OptionsPanel"), "Enter abre configurações");
        yield return Click("btn_audio");
        Registrar(Ativo("AudioPanel") && a.FocoAtual == "Poste", "Áudio abre com foco no poste");
        yield return Capturar("03-Audio");
        yield return Tecla(Key.Escape);
        yield return Esperar(.3f);
        yield return Click("btn_video");
        Registrar(Ativo("VIdeoPanel") && a.FocoAtual == "Reflexos", "Vídeo abre com foco nos reflexos");
        var slider = GameObject.Find("VIdeoPanel").GetComponentsInChildren<Slider>().First();
        var rt = slider.GetComponent<RectTransform>();
        Vector2 p = RectTransformUtility.WorldToScreenPoint(null, rt.TransformPoint(new Vector2(rt.rect.xMin + rt.rect.width * .7f, rt.rect.center.y)));
        yield return ClickPonto(p);
        yield return Esperar(.2f);
        Registrar(Mathf.Abs(GameObject.Find("VideoBackground").GetComponent<RawImage>().color.r - slider.value) < .01f, "Brilho segue o controle existente");
        slider.value = brilhoOriginal;
        yield return Capturar("04-Video");
        yield return Tecla(Key.Escape);
        yield return Esperar(.3f);
        yield return Click("btn_legendas");
        Registrar(Ativo("LegendasPanel") && a.FocoAtual == "Clube", "Legendas abre integrado ao clube");
        yield return Capturar("05-Legendas");
        yield return Tecla(Key.Escape);
        yield return Esperar(.3f);
        yield return Tecla(Key.Escape);
        yield return Esperar(.5f);
        Registrar(Ativo("MainMenuPanel") && !Ativo("OptionsPanel"), "ESC retorna ao menu principal");
        // O ponto é derivado do sprite independente, sem usar callbacks de botão.
        var telefone = GameObject.Find("OrelhaoPrimeiroPlano").GetComponent<RectTransform>();
        Vector2 telefonePonto = RectTransformUtility.WorldToScreenPoint(null, telefone.TransformPoint(new Vector2(0, telefone.rect.height * .18f)));
        yield return ClickPonto(telefonePonto);
        yield return Esperar(.4f);
        Registrar(PayphoneCheckpoint.IsSaveUIOpen, "Clique no orelhão abre seleção explícita de slots");
        Registrar(SceneManager.GetActiveScene().name == "MainMenu", "Orelhão não carrega automaticamente");
        yield return Capturar("06-Registros");
        yield return Tecla(Key.Escape);
        yield return Esperar(.3f);
        Registrar(!PayphoneCheckpoint.IsSaveUIOpen, "ESC fecha registros sem carregar");
        yield return Click("btn_continuar");
        Registrar(PayphoneCheckpoint.IsSaveUIOpen, "Continuar mantém o mesmo modal de slots");
        yield return Tecla(Key.Escape);
        yield return Esperar(.3f);
        Registrar(CheckpointSystem.TryLoadSlot(2, out _), "Navegação preserva o save de teste");
        yield return Click("btn_novojogo", .12f);
        Registrar(a != null && a.Saindo, "Confirmar novo jogo inicia transição única");
        float limite = Time.realtimeSinceStartup + 7f;
        while (SceneManager.GetActiveScene().name != "Prologo" && Time.realtimeSinceStartup < limite)
            yield return null;
        yield return Esperar(.3f);
        Registrar(SceneManager.GetActiveScene().name == "Prologo", "Novo jogo chega ao prólogo");
        Registrar(FindAnyObjectByType<PrologoJogavel>() != null, "Diretor jogável do prólogo preservado");
        Registrar(CheckpointSystem.TryLoadSlot(2, out _), "Novo jogo preserva slots manuais existentes conforme contrato atual");
        saves.Dispose();
        saves = CheckpointSystem.BeginIsolatedTests();
        SceneManager.LoadScene("MainMenu");
        yield return Esperar(.8f);
        a = FindAnyObjectByType<MenuAmbiente>();
        yield return Hover("btn_continuar");
        Registrar(GameObject.Find("Contexto").GetComponent<TMP_Text>().text.Contains("Nenhum registro"), "Estado sem save é explicado");
        yield return Click("btn_continuar");
        Registrar(PayphoneCheckpoint.IsSaveUIOpen && SceneManager.GetActiveScene().name == "MainMenu", "Continuar sem save permite consultar o aparelho sem abrir partida");
        Registrar(GameObject.Find("AparelhoFrontal") != null, "Seletor aparece dentro do orelhão frontal");
        Registrar(GameObject.Find("BtnSlot_1") != null && !GameObject.Find("BtnSlot_1").GetComponent<Button>().interactable,
            "Registro vazio permanece indisponível para carregar");
        yield return Capturar("08-OrelhaoVazio");
        yield return Tecla(Key.Escape);
        yield return Esperar(.25f);

        // Fixture de interação: abre o orelhão real da cena, sem simular a
        // caminhada da campanha. A confirmação e a persistência usam o fluxo real.
        SceneManager.LoadScene("EntradaDaCidade");
        yield return Esperar(1.3f);
        var orelhao = FindAnyObjectByType<PayphoneCheckpoint>();
        Registrar(orelhao != null, "Orelhão da cena de chegada disponível para fixture de gravação");
        if (orelhao != null)
        {
            orelhao.Interact();
            yield return Esperar(.3f);
            var aparelho = FindAnyObjectByType<PayphoneRegistroUI>();
            Registrar(aparelho != null && !aparelho.SomenteCarregar, "Modo de gravação integrado ao mesmo aparelho");
            Registrar(!CheckpointSystem.TryLoadSlot(1, out _), "Abrir o aparelho não salva automaticamente");
            yield return Tecla(Key.Digit2);
            yield return Capturar("09-Gravando");
            yield return Esperar(.65f);
            Registrar(CheckpointSystem.TryLoadSlot(1, out var gravado) && gravado.sceneName == "EntradaDaCidade", "Tecla 2 grava somente no slot confirmado");
            Registrar(!PayphoneCheckpoint.IsSaveUIOpen, "Fim de gravação fecha o aparelho");
            SceneManager.LoadScene("MainMenu");
            yield return Esperar(1.3f);
            yield return Click("btn_continuar");
            Registrar(FindAnyObjectByType<PayphoneRegistroUI>().SomenteCarregar, "Continuar utiliza modo somente leitura/carga");
            Registrar(GameObject.Find("MonofoneAnimado") == null && GameObject.Find("CaboDoMonofone") == null,
                "Fone e cabo integram a imagem única do aparelho");
            yield return Tecla(Key.Digit2);
            float prazoCarga = Time.realtimeSinceStartup + 7f;
            bool semRecuo = true, escurecimentoContinuo = true, viuFade = false;
            float alphaAnterior = 0;
            while (SceneManager.GetActiveScene().name != "EntradaDaCidade" && Time.realtimeSinceStartup < prazoCarga)
            {
                if (SceneFader.IsTransitioning)
                {
                    viuFade = true;
                    var frontal = GameObject.Find("AparelhoFrontal");
                    semRecuo &= frontal != null && frontal.GetComponent<CanvasGroup>().alpha > .99f;
                    var preto = GameObject.Find("FadeCanvas").GetComponent<CanvasGroup>();
                    escurecimentoContinuo &= preto.alpha + .01f >= alphaAnterior;
                    alphaAnterior = preto.alpha;
                }
                yield return null;
            }
            Registrar(viuFade && semRecuo, "Carga mantém o orelhão enquadrado até trocar de cena");
            Registrar(viuFade && escurecimentoContinuo, "Carga escurece diretamente sem recuo ou piscadas");
            Registrar(SceneManager.GetActiveScene().name == "EntradaDaCidade", "Discagem reconecta ao registro escolhido");
            Registrar(CheckpointSystem.TryLoadSlot(1, out _), "Carregar preserva o registro salvo");
        }
        SceneManager.LoadScene("MainMenu");
        yield return Esperar(1.3f);
        a = FindAnyObjectByType<MenuAmbiente>();
        yield return Hover("btn_sair");
        Registrar(a.FocoAtual == "Despedida", "Sair responde com enquadramento de despedida");
        yield return Capturar("07-Sair");
        yield return Hover("btn_novojogo");
    }

    static IEnumerator Esperar(float t) { yield return new WaitForSecondsRealtime(t); }
    static bool Ativo(string n) { return GameObject.Find(n) != null; }
    static void Mover(Vector2 p)
    { InputSystem.QueueStateEvent(Mouse.current, new MouseState { position = p }); }
    static IEnumerator Hover(string n)
    {
        var r = GameObject.Find(n).GetComponent<RectTransform>();
        Mover(RectTransformUtility.WorldToScreenPoint(null, r.TransformPoint(r.rect.center)));
        yield return Esperar(.45f);
    }
    static IEnumerator Click(string n, float espera = .4f)
    {
        yield return Hover(n);
        var r = GameObject.Find(n).GetComponent<RectTransform>();
        yield return ClickPonto(RectTransformUtility.WorldToScreenPoint(null, r.TransformPoint(r.rect.center)));
        yield return Esperar(espera);
    }
    static IEnumerator ClickPonto(Vector2 p)
    {
        Mover(p); yield return null;
        InputSystem.QueueStateEvent(Mouse.current, new MouseState { position = p, buttons = 1 });
        yield return null; yield return null;
        Mover(p); yield return null;
    }
    static IEnumerator Tecla(Key k)
    {
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(k));
        yield return null; yield return null;
        InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState());
        yield return Esperar(.15f);
    }
    IEnumerator Capturar(string nome)
    {
        yield return new WaitForEndOfFrame();
        var tex = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(pasta, nome + ".png"), tex.EncodeToPNG());
        Destroy(tex);
    }
    void Registrar(bool ok, string descricao)
    {
        resultados.Add((ok ? "PASS " : "FAIL ") + descricao);
        if (!ok) { falhas++; Debug.LogError("MENU QA FAIL " + descricao); }
    }
    void Limpar()
    {
        saves?.Dispose(); saves = null;
        if (tinhaBrilho) PlayerPrefs.SetFloat("video_brilho", brilhoOriginal);
        else PlayerPrefs.DeleteKey("video_brilho");
        foreach (var par in preferenciasVideo)
            if (par.Value.HasValue) PlayerPrefs.SetInt(par.Key, par.Value.Value);
            else PlayerPrefs.DeleteKey(par.Key);
        PlayerPrefs.Save();
        InputSystem.settings.backgroundBehavior = comportamentoOriginal;
        Application.runInBackground = runInBackgroundOriginal;
    }
    void OnDestroy() { if (saves != null) Limpar(); Rodando = false; }
}
#endif
