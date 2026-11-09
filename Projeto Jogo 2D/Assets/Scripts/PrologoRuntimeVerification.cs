#if UNITY_EDITOR || UNITY_ENABLE_CHECKS
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

// Apenas o executável Development com -prologo-qa executa esta fixture.
// Dispositivos virtuais exercitam o mesmo caminho de input do jogador, sem saves.
public sealed class PrologoRuntimeVerification : MonoBehaviour
{
    private Keyboard teclado;
    private Mouse mouse;
    private string pasta="C:/Temp/SantaRitaPrologo/Evidencias";
    private int testes;
    private bool falhou;
    private bool focoFixture=true;
    private void Update()
    {
        if(focoFixture)
        {
            var p=FindAnyObjectByType<PrologoJogavel>();
            if(p!=null)p.SendMessage("OnApplicationFocus",true);
        }
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Instalar()
    {
        if(Array.IndexOf(Environment.GetCommandLineArgs(),"-prologo-qa")<0)return;
        var go=new GameObject("PrologoQA"); DontDestroyOnLoad(go); go.AddComponent<PrologoRuntimeVerification>();
    }
    private void Check(bool ok,string nome)
    {
        testes++; if(!ok)falhou=true;
        File.AppendAllText(pasta+"/runtime.txt",(ok?"PASS ":"FAIL ")+nome+"\n");
    }
    private IEnumerator Tecla(Key key,float duracao=.15f)
    {
        InputSystem.QueueStateEvent(teclado,new KeyboardState(key));
        yield return new WaitForSecondsRealtime(duracao);
        InputSystem.QueueStateEvent(teclado,new KeyboardState());
        yield return null;
    }
    private IEnumerator Captura(string nome)
    {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(pasta+"/"+nome+".png");
        yield return null;
    }
    private IEnumerator Start()
    {
        Application.runInBackground=true;
        Directory.CreateDirectory(pasta); File.WriteAllText(pasta+"/runtime.txt","Fixture Development; dispositivos virtuais; foco explícito em execução batch; nenhum save pessoal.\n");
        teclado=InputSystem.AddDevice<Keyboard>(); mouse=InputSystem.AddDevice<Mouse>();
        SceneManager.LoadScene("Prologo"); yield return null; yield return null;
        var p=FindAnyObjectByType<PrologoJogavel>();
        Check(p!=null,"Cena possui diretor jogável"); if(p==null){Application.Quit(1);yield break;}
        p.SendMessage("OnApplicationFocus",true);
        yield return new WaitForSecondsRealtime(3.5f);
        Check(p.Tempo>2.8f,"Abertura avança em tempo real");
        Check(FindAnyObjectByType<UnityEngine.Video.VideoPlayer>()==null,"Nenhum VideoPlayer no prólogo");
        yield return Captura("01-frente");
        yield return Tecla(Key.D,2f); Check(p.Lateral>.1f && p.Lateral<=.65f,"Direção direita desloca dentro da faixa");
        yield return Captura("02-direita");
        yield return Tecla(Key.A,3f); Check(p.Lateral<-.1f && p.Lateral>=-.65f,"Direção esquerda limitada");
        yield return Tecla(Key.E,1.3f); Check(p.Olhar.x>15f && p.Olhar.x<=26f,"Olhar lateral limitado");
        yield return Captura("03-olhar");
        yield return Tecla(Key.R); yield return new WaitForSecondsRealtime(.8f); Check(p.Olhar.magnitude<1,"Recentralizar olhar");
        yield return Tecla(Key.Digit1); Check(p.RadioLigado && GameState.Tem(Flags.PrologoOuviuRadio),"Rádio alterna e registra flag RAM");
        yield return Tecla(Key.Digit1); Check(!p.RadioLigado,"Rádio desliga");
        yield return Tecla(Key.Digit2); Check(p.Limpador==2,"Limpador rápido");
        yield return Tecla(Key.Digit2); Check(p.Limpador==0,"Limpador desligado");
        yield return Tecla(Key.Digit2); Check(p.Limpador==1,"Limpador intermitente");
        yield return Tecla(Key.Digit3); Check(p.FarolAlto,"Farol alto");
        yield return Tecla(Key.Digit4); Check(p.PortaLuvasAberto && GameState.Tem(Flags.PrologoAbriuPortaLuvas),"Porta-luvas abre");
        yield return Captura("04-porta-luvas");
        yield return Tecla(Key.Digit4); Check(GameState.Tem(Flags.PrologoViuFotoAvo),"Fotografia opcional");
        yield return Tecla(Key.Digit4); Check(!p.PortaLuvasAberto,"Porta-luvas fecha");
        yield return Tecla(Key.Digit5); Check(GameState.Tem(Flags.PrologoOlhouCelular),"Telefone examinado");
        yield return Tecla(Key.Digit5); Check(GameState.Tem(Flags.PrologoTentouLigarAvo),"Tentativa de ligação opcional");
        yield return Tecla(Key.Escape); float tempo=p.Tempo;
        yield return new WaitForSecondsRealtime(.8f); Check(MenuPausa.Aberto && Mathf.Abs(tempo-p.Tempo)<.1f,"ESC pausa relógio e input");
        yield return Tecla(Key.Escape); yield return new WaitForSecondsRealtime(.3f); Check(!MenuPausa.Aberto && p.Tempo>tempo,"ESC retoma viagem");
        focoFixture=false; p.SendMessage("OnApplicationFocus",false); tempo=p.Tempo;
        yield return new WaitForSecondsRealtime(.4f); Check(Mathf.Abs(tempo-p.Tempo)<.02f,"Perda de foco suspende viagem");
        focoFixture=true; p.SendMessage("OnApplicationFocus",true);
        while(p!=null && p.Tempo<PrologoJogavel.InicioManobra+1)yield return null;
        Check(p!=null && p.Estacionamento>0,"Manobra inicia antes da parada");
        yield return Captura("05-aproximacao");
        while(p!=null && p.Tempo<PrologoJogavel.Parado+.3f)yield return null;
        Check(p!=null && p.Velocidade==0 && p.Estacionamento==1,"Carro estaciona no recuo");
        Check(p!=null && Mathf.Abs(p.PosicaoLateral - PrologoJogavel.LateralEntrada(Capitulo1Director.CarroEntrada.y))<.01f &&
            Mathf.Abs(p.Distancia - PrologoJogavel.DistanciaParada)<.01f && p.Terra==0,
            "Carro para no recuo asfaltado da Rodovia nas coordenadas da cena seguinte");
        if(p!=null)
        {
            float d=p.Distancia;
            yield return new WaitForSecondsRealtime(.4f);
            Check(Mathf.Abs(d-p.Distancia)<.01f,"Cenário para junto com o veículo");
        }
        yield return Captura("06-estacionado");
        float limite=Time.realtimeSinceStartup+70;
        while(SceneManager.GetActiveScene().name=="Prologo" && Time.realtimeSinceStartup<limite)yield return null;
        Check(SceneManager.GetActiveScene().name=="EntradaDaCidade","Chegada natural sem tarefa obrigatória");
        Check(GameState.Tem(Flags.PrologoConcluido),"Conclusão registrada em RAM");
        yield return new WaitForSecondsRealtime(3); yield return Captura("07-chegada-exterior");
        // Segunda passagem sem interações: testa o salto deliberado, não ESC.
        SceneManager.LoadScene("Prologo"); yield return null; yield return null;
        p=FindAnyObjectByType<PrologoJogavel>(); p.SendMessage("OnApplicationFocus",true);
        yield return new WaitForSecondsRealtime(3.2f);
        yield return Tecla(Key.Space,.3f); Check(SceneManager.GetActiveScene().name=="Prologo","Toque breve não pula");
        yield return Tecla(Key.Space,1.6f); yield return new WaitForSecondsRealtime(3);
        Check(SceneManager.GetActiveScene().name=="EntradaDaCidade","Segurar espaço conclui pelo mesmo fluxo");
        File.AppendAllText(pasta+"/runtime.txt",$"{testes} checks; resultado={(falhou?"FAIL":"PASS")}\n");
        Application.Quit(falhou?1:0);
    }
}
#endif
