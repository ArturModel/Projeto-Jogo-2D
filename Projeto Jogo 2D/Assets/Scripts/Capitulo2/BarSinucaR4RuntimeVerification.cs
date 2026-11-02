#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public sealed partial class Chapter2RuntimeVerification
{
    private bool sinucaR4Force;
    private Keyboard sinucaR4Keyboard;
#if UNITY_EDITOR
    private InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
    private InputSettings.BackgroundBehavior previousBackgroundInput;
    private bool ref7InputOverride;
    private void OnDestroy(){RestoreAudioC2Prefs();if(ref7InputOverride){InputSystem.settings.editorInputBehaviorInPlayMode=previousEditorInput;InputSystem.settings.backgroundBehavior=previousBackgroundInput;}}
#endif
#if UNITY_EDITOR
    public static void StartSinucaR4InEditor(string output)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);
        var host=new GameObject("QA_BarSinucaR4_Explicit");DontDestroyOnLoad(host);
        host.AddComponent<Chapter2RuntimeVerification>().sinucaR4Force=true;
    }
#endif
    private IEnumerator ClickR4(Button button)
    {
        Check(button!=null,"Botão de ação existe");
        var r=button.GetComponent<RectTransform>();Vector2 point=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));
        yield return PointerSinuca(point,false);yield return PointerSinuca(point,true);yield return PointerSinuca(point,false);
        yield return new WaitForSecondsRealtime(.4f);
    }
    private void BrazilianRuleChecks()
    {
        var empty=new List<int>();
        var r=new SinucaBrasileiraRules(true);
        var replay=r.Resolve(1,1,new List<int>{1},false,-1);
        Check(replay.replayOpening&&r.MarcusPoints==0&&replay.respot.Count==7,"Saída convertida repõe formação e não concede pontos");
        r.Resolve(1,1,empty,false,-1);
        Check(!r.Opening&&!r.MarcusTurn,"Saída legal alterna ação");
        r.Resolve(1,1,new List<int>{1},false,-1);
        Check(!r.Present[1]&&r.ZePoints==1&&r.OnBall==2,"Da vez removida e valor creditado");
        var color=r.Resolve(7,7,new List<int>{7},false,-1);
        Check(color.respot.Contains(7)&&r.Present[7]&&r.ZePoints==8,"Colorida livre vale seu valor e retorna");
        var foul=r.Resolve(6,6,empty,false,-1);
        Check(foul.foul&&r.MarcusPoints==7&&r.MayRefuse&&!foul.cueInHand,"Castigo concede sete e não inventa branca na mão");
        r.Refuse();Check(!r.MarcusTurn&&!r.MayRefuse,"Devolução da ação é única");
        var scratch=r.Resolve(2,2,empty,true,-1);Check(scratch.foul&&scratch.cueInHand,"Branca convertida retorna no D");
        r=new SinucaBrasileiraRules(true);r.Resolve(1,1,empty,false,-1);
        var wrong=r.Resolve(1,2,new List<int>{2},false,-1);Check(wrong.foul&&r.MarcusPoints==7&&r.Present[2],"Contato errado e bola indevida: uma penalidade, reposição");
        r=new SinucaBrasileiraRules(true);r.Resolve(1,1,empty,false,-1);
        var marked=r.Resolve(7,7,new List<int>{7},false,7);Check(marked.foul&&r.MarcusPoints==7&&r.Present[7],"Exceção narrativa da bola marcada preservada");
        r=new SinucaBrasileiraRules(true);r.Resolve(1,1,empty,false,-1);
        var multiple=r.Resolve(1,1,new List<int>{1,2},false,-1);Check(multiple.foul&&r.MarcusPoints==7&&r.Present[1],"Duas conversões não somam pontos nem faltas múltiplas");
        r=new SinucaBrasileiraRules(true);r.Resolve(1,1,empty,false,-1);
        int steps=0;
        while(!r.Finished&&steps++<35)
        {
            int on=r.OnBall;
            if(r.Phase==SinucaBrasileiraRules.Sequence.FreeColor)r.Resolve(on,on,empty,false,-1);
            else r.Resolve(on,on,new List<int>{on},false,-1);
        }
        Check(r.Finished&&r.Winner>=0&&steps<35,"Sequência completa encerra por pontuação brasileira, não meta quatro");
    }
    private IEnumerator ExitMatChecksR4()
    {
#if UNITY_EDITOR
        UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
#endif
        var director=Capitulo1Director.Instance;
        foreach(var zone in new[]{Capitulo1Director.ZonaId.Interior_Posto,Capitulo1Director.ZonaId.Interior_Mercadinho,Capitulo1Director.ZonaId.Interior_CasaDoAvo,Capitulo1Director.ZonaId.Interior_BarDoZe})
        {
            Capitulo1Director.InfoZona info;Capitulo1Director.TryObterInfoZona(zone,out info);
            director.TransicionarPara(zone,(info.min+info.max)*.5f,0);yield return new WaitForSecondsRealtime(1.6f);yield return CloseText();
            var mat=FindObjectsByType<SantaRitaExitThreshold>().OrderBy(x=>Vector2.Distance(x.transform.position,(info.min+info.max)*.5f)).First();
            var exit=FindObjectsByType<TransicaoDeZonaTrigger>().Where(x=>x.Prompt=="Sair").OrderBy(x=>Vector2.Distance(x.transform.position,mat.transform.position)).First();
            var bounds=mat.GetComponent<SpriteRenderer>().bounds;Vector2 feet=bounds.center;
            Check(!exit.CanInteractAt(feet+Vector2.up*1.1f),"Saída exige pés no tapete: "+zone);
            yield return Place(feet);Check(Interactable.FindNearest(feet)==exit,"Tapete seleciona saída C1: "+zone);
            yield return new WaitForSecondsRealtime(.7f);
            yield return TapR2(UnityEngine.InputSystem.Key.E);yield return new WaitForSecondsRealtime(1.5f);
            Check(Capitulo1Director.ZonaAtual!=zone,"E no tapete sai: "+zone+"; foco="+Application.isFocused+"; pausa="+MenuPausa.Aberto+"; texto="+GameMessageUI.IsShowing+"; sinuca="+SinucaMinigameUI.IsShowing+"; posição="+FindAnyObjectByType<PlayerController>().transform.position);yield return CloseText();
        }
        foreach(string room in new[]{"SociedadeBeiraRio","IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira"})
        {
            SceneTransition.Ir(room,"centro");yield return AwaitScene(room);yield return CloseText();
            var exit=FindObjectsByType<Capitulo2Portal>().First(x=>x.saidaInterior);
            var mat=FindObjectsByType<SantaRitaExitThreshold>().OrderBy(x=>Vector2.Distance(x.transform.position,exit.transform.position)).First();
            Vector2 feet=mat.GetComponent<SpriteRenderer>().bounds.center;
            Check(!exit.CanInteractAt(feet+Vector2.up*1.1f),"Saída exige pés no tapete: "+room);
            yield return Place(feet);Check(Interactable.FindNearest(feet)==exit,"Tapete seleciona saída C2: "+room);
            string destination=exit.cenaDestino;
            yield return TapR2(UnityEngine.InputSystem.Key.E);yield return AwaitScene(destination);yield return CloseText();
            Check(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name==destination,"E no tapete respeita destino "+destination+": "+room);
        }
    }
    private IEnumerator IntegratedFrameR4(SinucaMinigameUI ui)
    {
        ui.IniciarDuelo(null);ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",1);Field<GameObject>(ui,"painelModal").SetActive(false);
        var balls=Field<System.Collections.IList>(ui,"bolas");
        int shots=0;
        while(!Field<SinucaBrasileiraRules>(ui,"regrasBrasileiras").Finished&&shots<180)
        {
            Invoke(ui,"IniciarTurnoAtual");ui.StopAllCoroutines();Field<GameObject>(ui,"painelModal").SetActive(false);
            if(Field<bool>(ui,"brancaNaMao"))
            {
                balls[0].GetType().GetField("pos").SetValue(balls[0],(Vector2)ui.GetType().GetMethod("EscolherPosicaoBrancaZe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(ui,null));Set(ui,"brancaNaMao",false);
            }
            object[] choice={Vector2.zero,0f};ui.GetType().GetMethod("EscolherTacadaZe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(ui,choice);
            Invoke(ui,"ExecutarTacadaComEfeito",(Vector2)choice[0],(float)choice[1],SinucaCueSpin.Normal);
            int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<2400)Invoke(ui,"SimularFisicaMesa",1f/120);
            ui.StopAllCoroutines();Check(steps<2400&&ui.ValidarInvarianteSeteBolas(),"IA/física/resolução coerentes na tacada "+(++shots));
            if(shots%10==0){var r=Field<SinucaBrasileiraRules>(ui,"regrasBrasileiras");report.Add("DIAGNÓSTICO "+shots+": "+r.MarcusPoints+" x "+r.ZePoints+"; da vez "+r.OnBall+"; saída "+r.Opening);yield return null;}
        }
        var rules=Field<SinucaBrasileiraRules>(ui,"regrasBrasileiras");
        Check(rules.Finished,"Mão completa por IA e física, sem injetar pontos ("+shots+" tacadas, "+rules.MarcusPoints+" x "+rules.ZePoints+")");
        yield return Capture("sinuca-fim-mao-fisica");
        ui.Fechar();
    }
    private IEnumerator SeriesFixtureR4(SinucaMinigameUI ui)
    {
        report.Add("Fixture separada de fim de série: sete isolada junto à caçapa para executar duas conversões físicas e o callback real, sem simular a campanha.");
        bool victory=false;ui.IniciarDuelo(()=>victory=true);
        for(int frame=1;frame<=2;frame++)
        {
            ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",frame);Field<GameObject>(ui,"painelModal").SetActive(false);
            var rules=Field<SinucaBrasileiraRules>(ui,"regrasBrasileiras");
            rules.GetType().GetProperty("Opening").SetValue(rules,false);
            rules.GetType().GetProperty("MarcusTurn").SetValue(rules,true);
            var balls=Field<System.Collections.IList>(ui,"bolas");
            for(int n=1;n<=7;n++){rules.Present[n]=n==7;balls[n].GetType().GetField("ativa").SetValue(balls[n],n==7);}
            balls[0].GetType().GetField("pos").SetValue(balls[0],new Vector2(0,55));
            balls[7].GetType().GetField("pos").SetValue(balls[7],new Vector2(0,85));
            Set(ui,"turnoDeMarcus",true);Set(ui,"brancaNaMao",false);Set(ui,"cantadaManual",7);
            Invoke(ui,"AtualizarRegraBolaMarcadaAntesDoTurno");
            Check(Field<int>(ui,"numeroBolaMarcada")==-1,"Última bola da vez não fica bloqueada pela marca na mão "+frame);
            Invoke(ui,"ExecutarTacadaComEfeito",Vector2.up,120f,SinucaCueSpin.Normal);
            int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<2000)Invoke(ui,"SimularFisicaMesa",1f/120);
            ui.StopAllCoroutines();Check(steps<2000&&Field<int>(ui,"maosVencidasMarcus")==frame,"Conversão física fecha mão e acumula série "+frame);
            yield return null;
        }
        Check(Field<object>(ui,"estado").ToString()=="FimDeSerieVitoria"&&!victory,"Série aguarda ouvir Zé antes do callback");
        yield return Capture("sinuca-vitoria-serie");
        yield return ClickR4(Field<Button>(ui,"btnModalPrimario"));
        Check(victory&&!SinucaMinigameUI.IsShowing,"OUVIR ZÉ conclui callback sem autosave");
    }
    private IEnumerator RunSinucaR4()
    {
        report.Add("Fixture: aproximações/reposicionamento e pré-condições C1/C2 em RAM, isolamento de save. Cliques e carga via EventSystem/Input System reais. Não equivale a campanha legítima ou aprovação artística.");
        Application.targetFrameRate=60;QualitySettings.vSyncCount=0;
        #if UNITY_EDITOR
        previousEditorInput=InputSystem.settings.editorInputBehaviorInPlayMode;previousBackgroundInput=InputSystem.settings.backgroundBehavior;ref7InputOverride=true;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
        report.Add("Editor QA: roteamento de dispositivo virtual liberado para GameView em segundo plano; configuração anterior restaurada ao encerrar. Player usa configuração normal.");
#endif
        BrazilianRuleChecks();
        SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");yield return CloseText();
        sinucaMouse=InputSystem.AddDevice<Mouse>("R4RegressionMouse");
        sinucaR4Keyboard=InputSystem.AddDevice<Keyboard>("R4RegressionKeyboard");
        sinucaR4Keyboard.MakeCurrent();
        var car=FindObjectsByType<InteracaoCapitulo1>().First(x=>x.Tipo==InteracaoCapitulo1.TipoInteracao.CarroMarcus);
        car.Interact();yield return CloseText();
        Check(GameState.Tem(Flags.LanternaDoCarroObtida)&&!GameState.Tem(Flags.MapaDoCarroObtido),"Carro entrega lanterna sem mapa em sessão nova");
        Check(SantaRitaInventory.SlotCount==5&&SantaRitaInventory.HasItem(0)&&!SantaRitaInventory.HasItem(1),"Cinco slots com propriedade derivada do snapshot");
        MapaInterativoUI.EnsureInstance().Abrir();yield return null;
        Check(MapaInterativoUI.IsShowing&&Field<int>(MapaInterativoUI.Instance,"abaAtiva")==1,"Diário acessível antes de obter mapa");MapaInterativoUI.FecharSeAberto();
        var receipt=FindObjectsByType<InteracaoCapitulo1>().First(x=>x.Tipo==InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004);
        receipt.Interact();yield return new WaitForSecondsRealtime(.3f);
        Check(!GameState.Tem(Flags.AchouReciboMercadinho),"Examinar não coleta recibo automaticamente");
        yield return Capture("recibo-antes-coleta");
        yield return ClickR4(GameObject.Find("AcaoDoObjeto").GetComponentInChildren<Button>());
        yield return CloseText();
        Check(SantaRitaInventory.HasItem(2),"Clique PEGAR coloca recibo no inventário");
        var merchant=FindObjectsByType<InteracaoCapitulo1>().First(x=>x.Tipo==InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho);
        merchant.Interact();yield return new WaitForSecondsRealtime(.3f);
        yield return ClickR4(GameObject.Find("AcaoDoObjeto").GetComponentInChildren<Button>());yield return CloseText();
        Check(GameState.Tem(Flags.ApresentouReciboAtendente)&&!SantaRitaInventory.HasItem(2),"Entrega explícita conduz testemunho e retira original");
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.RecebeuFichaClubeZe);
        SceneTransition.Ir("RodoviariaSantaRita","centro");yield return AwaitScene("RodoviariaSantaRita");yield return CloseText();
        Capitulo2Director.Instance.Interagir("horarios");yield return new WaitForSecondsRealtime(.3f);
        yield return Capture("mapa-rodoviaria");
        yield return ClickR4(GameObject.Find("AcaoDoObjeto").GetComponentInChildren<Button>());yield return CloseText();
        Check(GameState.Tem(Flags.MapaDoCarroObtido)&&SantaRitaInventory.HasItem(1),"Mapa é coletado na rodoviária");
        var snapshot=JsonUtility.FromJson<GameStateSnapshot>(JsonUtility.ToJson(GameState.ExportarSnapshot()));GameState.ImportarSnapshot(snapshot);
        Check(SantaRitaInventory.HasItem(0)&&SantaRitaInventory.HasItem(1)&&!SantaRitaInventory.HasItem(2),"Snapshot preserva equipamento e entrega");
        Check(!CheckpointSystem.HasCheckpoint,"Aquisições e entrega não gravam autosave");
        SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");yield return CloseText();
        var d=Capitulo1Director.Instance;
        foreach(var zone in new[]{Capitulo1Director.ZonaId.Zona2_Posto,Capitulo1Director.ZonaId.Zona3_Encruzilhada,Capitulo1Director.ZonaId.Zona4_Bairro,Capitulo1Director.ZonaId.Interior_Posto,Capitulo1Director.ZonaId.Interior_Mercadinho,Capitulo1Director.ZonaId.Interior_CasaDoAvo})
        {
            Capitulo1Director.InfoZona info;Capitulo1Director.TryObterInfoZona(zone,out info);
            d.TransicionarPara(zone,(info.min+info.max)*.5f,0);yield return new WaitForSecondsRealtime(2.7f);
            var music=Field<AudioSource>(d,"musicaBairro");bool inside=zone.ToString().StartsWith("Interior");
            Check(music!=null&&music.loop&&(PlayerPrefs.GetFloat("audio_musica",1)<=0||music.isPlaying),"Trilha contínua em "+zone);
            Check(music.GetComponent<AudioLowPassFilter>().cutoffFrequency<(inside?2500:23000)&&(!inside?music.GetComponent<AudioLowPassFilter>().cutoffFrequency>18000:true),"Filtro contextual em "+zone);
        }
        d.TransicionarPara(Capitulo1Director.ZonaId.Interior_BarDoZe,new Vector2(509,-54.8f),1);yield return new WaitForSecondsRealtime(1.5f);yield return CloseText();
        Check(GameObject.Find("BarCoerente")!=null,"Bar usa planta compartilhada");
        foreach(var prop in SantaRitaBarLayout.Props.Where(x=>x.kind!=SantaRitaBarLayout.Kind.Actor))
        {var g=GameObject.Find(prop.id);Check(g!=null&&Vector2.Distance(g.transform.position,SantaRitaBarLayout.Origin+prop.position)<.02f,"Âncora compartilhada: "+prop.id);}
        yield return Capture("bar-recomposto");
        var ui=SinucaMinigameUI.Instance;ui.IniciarPraticaOpcional();yield return new WaitForSecondsRealtime(.3f);
        var view=FindAnyObjectByType<SinucaTableView>();
        yield return Ref7Checks(ui,view);
        Check(GameObject.Find("FundoEscuroBar").GetComponent<Image>().color.a==1,"Projeção tem fundo opaco, sem duplicar o bar 2D");
        foreach(var camera in new[]{SinucaCameraView.Mesa3D,SinucaCameraView.LinhaTaco,SinucaCameraView.VistaSuperior})
        {
            view.DefinirCamera(camera);Invoke(ui,"AtualizarBotaoVistaCamera");yield return null;
            Vector3 onTable=new Vector3(0,SinucaTableView.BallRadius*.4f,0);
            Vector2 screen=ScreenSinuca(view,onTable),back;
            Check(view.ScreenToTablePoint(screen,null,out back)&&back.magnitude<.03f,"Projeção e mira inversa: "+camera);
            yield return Capture("sinuca-"+camera);
        }
        view.DefinirCamera(SinucaCameraView.Mesa3D);yield return null;
        var selector=FindAnyObjectByType<SinucaSpinSelector>();var sr=selector.rectTransform;
        Vector2 contactScreen=RectTransformUtility.WorldToScreenPoint(null,sr.TransformPoint(new Vector2(sr.rect.width*.25f,sr.rect.height*.15f)));
        yield return PointerSinuca(contactScreen,false);yield return PointerSinuca(contactScreen,true);yield return PointerSinuca(contactScreen,false);
        Check(Field<Vector2>(ui,"contatoTaco").x>.2f&&Field<Vector2>(ui,"contatoTaco").y>.1f,"Disco recebe contato lateral/longitudinal por clique real");
        ui.SelecionarEfeitoBranca(SinucaCueSpin.Normal);
        Check(selector.Contact==Vector2.zero&&Field<Vector2>(ui,"contatoTaco")==Vector2.zero,"Reset interno centraliza contato sem botão de preset");
        var balls=Field<System.Collections.IList>(ui,"bolas");
        var cue=balls[0];var pos=(Vector2)cue.GetType().GetField("pos").GetValue(cue);
        Vector2 aim=ScreenSinuca(view,new Vector3(pos.x+30,SinucaTableView.BallRadius*.4f,pos.y));
        yield return PointerSinuca(aim,false);yield return PointerSinuca(aim,true);yield return new WaitForSecondsRealtime(.2f);
        Invoke(ui,"OnApplicationFocus",false);yield return PointerSinuca(aim,false);
        Check(Field<object>(ui,"estado").ToString()=="MirandoJogador","Perda de foco cancela carga sem tacada fantasma");
        yield return PointerSinuca(aim,false);yield return PointerSinuca(aim,true);yield return new WaitForSecondsRealtime(.3f);
        Check(Field<object>(ui,"estado").ToString()=="CarregandoForca","Clique real inicia carga");
        float initial=Field<float>(ui,"forcaNormalizada");yield return new WaitForSecondsRealtime(.6f);
        Check(Mathf.Abs(initial-Field<float>(ui,"forcaNormalizada"))>.02f,"Força muda enquanto segura");
        yield return PointerSinuca(aim,false);yield return null;
        Check(Field<object>(ui,"estado").ToString()=="BolasEmMovimento","Soltar clique lança sem botão extra");
        float end=Time.realtimeSinceStartup+20;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&Time.realtimeSinceStartup<end)yield return null;
        Check(Time.realtimeSinceStartup<end,"Física alcança repouso sem deadlock");
        ui.Fechar();
        Check(!CheckpointSystem.HasCheckpoint,"Partida e exploração permanecem sem autosave");
        yield return IntegratedFrameR4(ui);
        yield return SeriesFixtureR4(ui);
        yield return ExitMatChecksR4();
        Check(!CheckpointSystem.HasCheckpoint,"Regressão integrada e saídas sem autosave");
        InputSystem.RemoveDevice(sinucaMouse);InputSystem.RemoveDevice(sinucaR4Keyboard);sinucaR4Keyboard=null;
        Finish();
    }
}
#endif
