#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public sealed partial class Chapter2RuntimeVerification
{
    private bool bochaForce;
    private Mouse bochaMouse;
#if UNITY_EDITOR
    public static void StartBochaInEditor(string output)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);
        var host=new GameObject("QA_Bocha_Perspectiva_Explicito");DontDestroyOnLoad(host);host.AddComponent<Chapter2RuntimeVerification>().bochaForce=true;
    }
#endif
    private IEnumerator PointerBocha(Vector2 position,bool held)
    {
        InputSystem.QueueStateEvent(bochaMouse,new MouseState{position=position}.WithButton(MouseButton.Left,held));
        yield return null;yield return null;yield return null;
    }
    private static Vector2 ScreenBocha(BochaCourtView view,Vector3 p)=>RectTransformUtility.WorldToScreenPoint(null,view.transform.TransformPoint(view.Project(p)));
    private IEnumerator NewBochaPractice()
    {
        var ui=BochaMinigameUI.Instance;ui.Abrir(null);yield return null;Click("PRATICAR");yield return null;Click("LER A CANCHA");yield return null;
    }
    private IEnumerator RunBochaPerspective()
    {
        report.Add("Fixture declarada: entrada direta na Sociedade e prática; mouse virtual pelo Input System/EventSystem real. Não é percurso de campanha.");
        Application.targetFrameRate=60;QualitySettings.vSyncCount=0;
        yield return new WaitForSecondsRealtime(1);
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.RecebeuFichaClubeZe);GameState.Marcar(Flags.MapaDoCarroObtido);GameState.Marcar(Flags.LanternaDoCarroObtida);
        SceneFader.Instance.LoadScene("SociedadeBeiraRio");yield return AwaitScene("SociedadeBeiraRio");yield return CloseText();
        Check(GameObject.Find("SalaCanchaFechada")!=null&&FindObjectsByType<BoxCollider2D>().Any(c=>c.name=="ParedeDivisoriaCancha"&&c.enabled),"Sala da cancha tem divisória visível e colisão, conectada ao salão.");
        // Fixture na base da escada rasa; travessia usa movimento real na mesma cena.
        yield return Place(new Vector2(4.7f,6.3f));yield return Capture("sala-entrada");
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.W));yield return new WaitForSecondsRealtime(1.15f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.15f);
        var actor=FindAnyObjectByType<PlayerController>();Check(actor.transform.position.y>8.1f,"Marcus atravessa a escada do salão para a sala por movimento real.");
        yield return Capture("sala-cabeceira");yield return Place(new Vector2(17,16));
        Check(actor.GetComponent<SpriteRenderer>().sortingOrder>GameObject.Find("ParedeNorte").GetComponent<SpriteRenderer>().sortingOrder,"Marcus permanece inteiro à frente da parede do fundo.");
        yield return Capture("sala-bancos");yield return Place(new Vector2(26,16));yield return Capture("sala-fundo");
        bochaMouse=InputSystem.AddDevice<Mouse>("BochaRegressionMouse");
        yield return NewBochaPractice();var ui=BochaMinigameUI.Instance;var view=Field<BochaCourtView>(ui,"vista");var map=Field<BochaMapGraphic>(ui,"minimapa");
        Check(view.Project(new Vector3(-1,0,10)).x<view.Project(new Vector3(1,0,10)).x,"Câmera: esquerda física aparece à esquerda.");
        Check(map.MapPoint(new Vector2(-1,10)).x<map.MapPoint(new Vector2(1,10)).x&&map.MapPoint(new Vector2(0,18)).y>map.MapPoint(new Vector2(0,3)).y,"Minimapa: mesma lateral da cabeceira e fundo no alto.");
        view.SetView(BochaCameraView.Superior);Check(view.Project(new Vector3(0,0,1)).x<view.Project(new Vector3(0,0,19)).x,"Vista geral: lançamento da esquerda para a direita.");view.SetView(BochaCameraView.Lancamento);
        Check(!ui.GetComponentsInChildren<Button>(true).Any(b=>b.name=="LANÇAR")&&ui.GetComponentsInChildren<Slider>(true).Length==0,"Sem botão de lançar ou ajuste de força concorrente.");
        var point=ScreenBocha(view,new Vector3(.6f,0,5));yield return PointerBocha(point,false);float near=view.AimDistance;
        point=ScreenBocha(view,new Vector3(.6f,0,18));yield return PointerBocha(point,false);
        Check(view.AimDistance>near+8,"Cursor mais ao fundo aumenta linha de mira.");
        var match=Field<BochaMatch>(ui,"partida");yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(.45f);
        float previous=Field<float>(ui,"forca"),max=previous,min=previous;int rising=0,falling=0,turns=0,lastSign=0;float endCharge=Time.realtimeSinceStartup+6.3f;
        while(Time.realtimeSinceStartup<endCharge)
        {
            yield return null;float current=Field<float>(ui,"forca");max=Mathf.Max(max,current);min=Mathf.Min(min,current);
            int sign=current>previous+.002f?1:current<previous-.002f?-1:0;
            if(sign>0)rising++;if(sign<0)falling++;if(sign!=0&&lastSign!=0&&sign!=lastSign)turns++;if(sign!=0)lastSign=sign;previous=current;
        }
        report.Add("Carga parada: mínimo "+min+", máximo "+max+", subidas "+rising+", descidas "+falling+", inversões "+turns);
        Check(rising>1&&max>.85f&&match.usadas[0]==0,"Clique parado aumenta força sem lançar nem gastar bocha.");
        Check(falling>1&&min<.25f,"Segurar além do máximo faz força descer.");
        Check(turns>=3,"Segurar continuamente repete subidas e descidas.");yield return Capture("carga-segundo-ciclo");
        yield return PointerBocha(point,false);
        Check(match.usadas[0]==1&&Field<object>(ui,"fase").ToString()=="Rolagem","Soltar clique parado lança exatamente uma bocha.");yield return null;yield return null;
        Check(match.usadas[0]==1,"Release não duplica lançamento.");
        Invoke(ui,"Pausar");var s=Field<BochaSimulation>(ui,"sim");var frozen=s.bolas.Last().pos;yield return new WaitForSecondsRealtime(.3f);
        Check(s.bolas.Last().pos==frozen,"Pausa realmente congela a física.");Click("CONTINUAR");
        yield return NewBochaPractice();match=Field<BochaMatch>(ui,"partida");point=ScreenBocha(view,new Vector3(0,0,13));
        yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(.3f);Invoke(ui,"OnApplicationFocus",false);yield return PointerBocha(point,false);
        Check(match.usadas[0]==0&&ActiveButton("CONTINUAR")&&!Field<bool>(ui,"arrastando"),"Perder foco cancela carga e release tardio, sem consumir.");Click("CONTINUAR");
        yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(.2f);Invoke(ui,"MudarVista",BochaCameraView.Alvo);yield return PointerBocha(point,false);
        Check(match.usadas[0]==0&&view.View==BochaCameraView.Alvo,"Troca de vista durante carga cancela sem lançamento.");yield return Capture("vista-alvo");
        Invoke(ui,"MudarVista",BochaCameraView.Superior);yield return Capture("vista-superior");
        yield return PointerBocha(ScreenBocha(view,new Vector3(0,0,10)),true);yield return new WaitForSecondsRealtime(.2f);yield return PointerBocha(point,false);
        Check(match.usadas[0]==0,"Consulta superior não arma nem lança.");Invoke(ui,"MudarVista",BochaCameraView.Lancamento);
        point=ScreenBocha(view,new Vector3(0,0,13));yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(.3f);yield return PointerBocha(new Vector2(2,2),false);
        Check(match.usadas[0]==0&&!Field<bool>(ui,"arrastando"),"Soltar fora da cancha cancela sem gastar.");
        yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(.25f);Invoke(ui,"EscolherTipo",BochaThrowType.Rafa);yield return PointerBocha(point,false);
        Check(match.usadas[0]==0&&!Field<bool>(ui,"arrastando"),"Trocar tipo cancela carga sem release fantasma.");
        float zoom=view.Zoom;view.ChangeZoom(2);Check(view.Zoom>zoom,"Zoom aproxima sem mudar coordenadas físicas.");view.ChangeZoom(100);Check(view.Zoom<=1.65f,"Zoom limitado mantém consulta legível.");view.SetView(BochaCameraView.Lancamento);
#if !UNITY_EDITOR
        foreach(var resolution in new[]{new Vector2Int(1024,768),new Vector2Int(1920,1080),new Vector2Int(1280,720)})
        {
            point=ScreenBocha(view,new Vector3(0,0,13));yield return PointerBocha(point,true);
            Screen.SetResolution(resolution.x,resolution.y,FullScreenMode.Windowed);yield return new WaitForSecondsRealtime(.8f);
            Check(ActiveButton("CONTINUAR")&&match.usadas[0]==0&&!Field<bool>(ui,"arrastando"),"Resolução cancela carga e pausa: "+resolution);
            yield return PointerBocha(point,false);Click("CONTINUAR");yield return null;
            var bounds=new Vector3[4];Field<GameObject>(ui,"raiz").GetComponent<RectTransform>().GetWorldCorners(bounds);
            Check(bounds[0].x>=-.1f&&bounds[0].y>=-.1f&&bounds[2].x<=Screen.width+.1f&&bounds[2].y<=Screen.height+.1f,"Interface completa dentro da tela "+resolution);
            yield return Capture("resolucao-"+resolution.x+"x"+resolution.y);
        }
#endif
        // Teclado entra pelo mesmo loop usado na partida, sem alterar coordenadas por reflexão.
        yield return PointerBocha(new Vector2(1,1),false);float startX=Field<float>(ui,"xLancamento");
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.A));yield return new WaitForSecondsRealtime(.25f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;
        float left=Field<float>(ui,"xLancamento");Check(left<startX-.1f&&view.Project(new Vector3(left,.18f,.65f)).x<view.Project(new Vector3(startX,.18f,.65f)).x,"A move posição e bocha para a esquerda visível.");
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.D));yield return new WaitForSecondsRealtime(.5f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;
        Check(Field<float>(ui,"xLancamento")>startX+.1f,"D move para a direita.");
        foreach(var mode in new[]{BochaThrowType.Ponto,BochaThrowType.Rafa,BochaThrowType.Volo})
        {
            yield return NewBochaPractice();Invoke(ui,"EscolherTipo",mode);point=ScreenBocha(view,new Vector3(.8f,0,14));
            yield return PointerBocha(point,true);yield return new WaitForSecondsRealtime(1.1f);yield return PointerBocha(point,false);
            s=Field<BochaSimulation>(ui,"sim");Check(s.bolas.Count==2&&s.bolas[1].valida,"Input completo lança modo "+mode);
            Check(mode==BochaThrowType.Volo?s.bolas[1].altura>.5f:s.bolas[1].altura<.01f,"Altura inicial correta em "+mode);
            yield return new WaitForSecondsRealtime(.35f);yield return Capture("lancamento-"+mode);
            if(mode==BochaThrowType.Volo)Check(view.TrackCount==0,"Voo não desenha rastro no ar.");
            else Check(view.TrackCount>0,"Rolagem deixa marca real no saibro: "+mode);
        }
        ui.Fechar();yield return null;
        Check(!BochaMinigameUI.IsShowing&&!ui.GetComponentsInChildren<Graphic>().Any(g=>g.gameObject.activeInHierarchy),"Sair remove também fundo opaco da Bocha.");
        yield return FullBochaPerspective();
        yield return BochaAudioR2();GameState.Marcar(Flags.LanternaDoCarroObtida);GameState.DefinirNumero("equipamento.lanterna_ligada",1);
        yield return SlotsR2();AIPerformanceR2();
        Check(!CheckpointSystem.HasCheckpoint,"Prática e UI não criaram checkpoint.");
        if(bochaMouse!=null)InputSystem.RemoveDevice(bochaMouse);bochaMouse=null;
        report.Add("Aprovação de diversão, arte e audição pelo autor permanece pendente.");Finish();
    }
    private IEnumerator FullBochaPerspective()
    {
        foreach(string flag in new[]{"c2_protocolo_adilio","c2_registro_manutencao","c2_padre_testemunho","c2_cemiterio_comparado"})GameState.Marcar(flag);
        GameState.Marcar(Capitulo2Director.Vitoria,false);GameState.Marcar(Capitulo2Director.Ficha,false);
        yield return Use("bocha");Check(BochaMinigameUI.IsShowing,"Interação e diálogo reais abrem a Bocha na sala.");
        Click("DUELAR ATÉ 6 PONTOS");yield return null;Click("LER A CANCHA");yield return null;
        var ui=BochaMinigameUI.Instance;Time.timeScale=8;int throws=0;float end=Time.realtimeSinceStartup+300;
        while(BochaMinigameUI.IsShowing&&!GameState.Tem(Capitulo2Director.Vitoria)&&Time.realtimeSinceStartup<end)
        {
            if(ActiveButton("OUVIR TONÃO")){Click("OUVIR TONÃO");break;}
            if(ActiveButton("PRÓXIMA MÃO"))Click("PRÓXIMA MÃO");
            else if(ActiveButton("REPETIR A MÃO"))Click("REPETIR A MÃO");
            else if(Field<object>(ui,"fase").ToString()=="Mira")
            {var shot=BestShot(Field<BochaSimulation>(ui,"sim"));Invoke(ui,"Lancar",BochaSide.Marcus,shot.x,shot.velocidade);throws++;}
            if(ActiveButton("ESCOLHER SAVE"))throw new Exception("QA perdeu duelo: revisar análise, nunca modificar placar.");
            yield return null;
        }
        Time.timeScale=1;Check(GameState.Tem(Capitulo2Director.Vitoria),"Sessão completa até seis com IA e física reais; Marcus lançou "+throws+" vezes. Apenas tempo/decisão de mira automatizados.");
        yield return Capture("vitoria-consequencia");yield return CloseText();Check(GameState.Tem(Capitulo2Director.Ficha),"Consequência entrega a ficha depois da vitória.");
        int notes=GameState.ExportarSnapshot().diario.Count;yield return Use("bocha");Check(GameState.ExportarSnapshot().diario.Count==notes,"Revisita não duplica recompensa ou diário.");
        Check(!CheckpointSystem.HasCheckpoint,"Vitória não salva automaticamente.");
    }
}
#endif
