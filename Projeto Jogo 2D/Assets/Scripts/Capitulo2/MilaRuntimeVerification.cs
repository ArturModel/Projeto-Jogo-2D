#if UNITY_EDITOR || DEBUG
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
public sealed class MilaRuntimeVerification:MonoBehaviour
{
    #if UNITY_EDITOR
    private UnityEngine.InputSystem.InputSettings oldInput,qaInput;private HideFlags oldFlags;
#endif
    private UnityEngine.InputSystem.Keyboard testKeyboard;
    private void Update(){if(testKeyboard!=null)testKeyboard.MakeCurrent();}
    private IDisposable scope;private string folder;private List<string> report=new List<string>();private bool failed;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Init(){if(!Environment.GetCommandLineArgs().Contains("-srMilaQA")&&!Environment.GetCommandLineArgs().Contains("-srMilaEditorQA"))return;var go=new GameObject("MilaQA");DontDestroyOnLoad(go);go.AddComponent<MilaRuntimeVerification>();}
    private void Start(){folder=Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR")??Path.Combine(Application.persistentDataPath,"MilaQA");Directory.CreateDirectory(folder);scope=CheckpointSystem.BeginIsolatedTests();
#if UNITY_EDITOR
        oldInput=UnityEngine.InputSystem.InputSystem.settings;oldFlags=oldInput.hideFlags;oldInput.hideFlags|=HideFlags.DontUnloadUnusedAsset;qaInput=Instantiate(oldInput);UnityEngine.InputSystem.InputSystem.settings=qaInput;
        qaInput.editorInputBehaviorInPlayMode=UnityEngine.InputSystem.InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        qaInput.backgroundBehavior=UnityEngine.InputSystem.InputSettings.BackgroundBehavior.IgnoreFocus;
#endif
Application.runInBackground=true;Screen.SetResolution(1280,720,FullScreenMode.Windowed);Application.logMessageReceived+=Log;StartCoroutine(Guard(Run()));}
    private void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception){failed=true;report.Add(m+"\n"+trace);}}
    private void Check(bool ok,string s){report.Add((ok?"PASS ":"FAIL ")+s);if(!ok)throw new Exception(s);}
    private static T Field<T>(object o,string n)=>(T)o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(o);
    private static void Set(object o,string n,object value)=>o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(o,value);
    private IEnumerator Guard(IEnumerator r){while(true){bool more=false;object v=null;try{more=r.MoveNext();if(more)v=r.Current;}catch(Exception e){report.Add(e.ToString());failed=true;break;}if(!more)break;yield return v;if(failed)break;}Finish();}
    private void Finish(){Application.logMessageReceived-=Log;scope?.Dispose();scope=null;File.WriteAllLines(Path.Combine(folder,"runtime-qa.txt"),report.Concat(new[]{failed?"FAIL runtime":"PASS runtime"}));
#if UNITY_EDITOR
        if(Application.isEditor){if(oldInput!=null){UnityEngine.InputSystem.InputSystem.settings=oldInput;oldInput.hideFlags=oldFlags;}if(qaInput!=null)DestroyImmediate(qaInput);UnityEditor.EditorSettings.enterPlayModeOptionsEnabled=UnityEditor.SessionState.GetBool("MilaQA.EnterPlayOptions",true);UnityEditor.EditorApplication.Exit(failed?1:0);return;}
#endif
        Application.Quit(failed?1:0);}
    private IEnumerator Run()
    {
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.MapaDoCarroObtido);GameState.Marcar(Flags.LanternaDoCarroObtida);GameState.Marcar(Flags.AchouReciboMercadinho);GameState.Marcar(Flags.CasaAvo_LeuBilhete);
        SceneManager.LoadScene("OficinaRadio");yield return new WaitForSecondsRealtime(4);
        var deck=SantaRitaTapeDeck.Ensure();var inv=SantaRitaInventory.Instance;
        Check(deck.Records.Length==4,"Quatro fitas atuais, sem fita final futura.");
        foreach(var t in deck.Records){Check(t.lines.Length>0&&t.lines.All(l=>l.seconds>0),"Transcrição temporizada "+t.id);Check(inv.Icon(t.id)!=null,"Arte de fita "+t.id);}
        Check(!SantaRitaInventory.Owns("tocador"),"Não recebe tocador só por visitar oficina.");
        deck.Repair();deck.PlayPause();Check(!SantaRitaInventory.Owns("tocador"),"Ajuste incorreto não conclui reparo.");Set(deck,"speed",1);deck.PlayPause();
        Check(SantaRitaInventory.Owns("tocador")&&SantaRitaInventory.Owns("fita02"),"Reparo regular concede aparelho e fita de procedência da oficina.");
        int guard=0;while(GameMessageUI.IsShowing&&guard++<40){GameMessageUI.Instance.Avancar();yield return new WaitForSecondsRealtime(.05f);}
        foreach(var id in new[]{"fita01","fita03","fita_sociedade"})GameState.Marcar("item."+id);
        Check(SantaRitaInventory.Contents().Length>5,"Bolsa guarda mais de cinco itens.");Check(!SantaRitaInventory.Contents().Any(id=>id.Contains("ficha")),"Fichas continuam exclusivamente no mapa.");
        SantaRitaInventory.Assign(0,"fita03");Check(SantaRitaInventory.Quick(0)=="fita03","Atalho reorganizável.");SantaRitaInventory.Assign(1,"fita03");Check(SantaRitaInventory.Quick(1)=="fita03"&&SantaRitaInventory.Quick(0)!="fita03","Mover atalho troca sem duplicar referência.");
        var before=SantaRitaInventory.Contents();SantaRitaInventory.Swap(before[0],before[1]);Check(SantaRitaInventory.Contents()[0]==before[1],"Ordem da bolsa alterada.");
        inv.Toggle();yield return new WaitForSecondsRealtime(.5f);Check(SantaRitaInventory.IsOpen,"Bolsa abre em tela própria.");
        var keyboard=UnityEngine.InputSystem.InputSystem.AddDevice<UnityEngine.InputSystem.Keyboard>();keyboard.MakeCurrent();testKeyboard=keyboard;
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard,new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.F));yield return null;yield return null;
        Check(!GameState.LanternaLigada,"Bolsa bloqueia equipamento F.");
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard,new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.Escape));yield return null;yield return null;
        Check(!SantaRitaInventory.IsOpen&&!MenuPausa.Aberto,"Esc fecha bolsa sem abrir pausa por baixo. Bolsa="+SantaRitaInventory.IsOpen+" Pausa="+MenuPausa.Aberto);
        UnityEngine.InputSystem.InputSystem.RemoveDevice(keyboard);testKeyboard=null;inv.Toggle();yield return null;ScreenCapture.CaptureScreenshot(Path.Combine(folder,"bolsa.png"));yield return new WaitForSecondsRealtime(.3f);inv.Close();
        var map=MapaInterativoUI.EnsureInstance();GameState.RegistrarNotaDiario("pista_cafe_hoje","Café","pista","Café ainda morno.");map.Abrir(1);var book=map.GetComponent<MarcusNotebookUI>();yield return null;
        book.ToggleBookmark();string marked=book.MarkedEntry;Check(marked!="","Marcador ligado a ID de anotação.");book.Turn(1);yield return new WaitForSecondsRealtime(.5f);map.Fechar();map.Abrir(1);Check(book.Spread==0,"Reabrir retorna à anotação marcada.");
        book.OpenIndex("Fitas");yield return null;Check(book.GetComponentsInChildren<Transform>().Any(t=>t.name=="IndiceManuscrito"),"Índice em papel abre com filtro.");ScreenCapture.CaptureScreenshot(Path.Combine(folder,"indice.png"));yield return new WaitForSecondsRealtime(.3f);map.Fechar();
        deck.Open("fita01");yield return null;Check(SantaRitaTapeDeck.IsOpen,"Aparelho acessível pela fita.");deck.PlayPause();yield return new WaitForSecondsRealtime(.3f);deck.PlayPause();float pos=deck.Position;yield return new WaitForSecondsRealtime(.2f);Check(Mathf.Approximately(pos,deck.Position),"Pausa mantém posição da fita.");deck.Rewind();Check(deck.Position==0&&!deck.Playing,"Rebobinar retorna ao início sem concluir.");
        ScreenCapture.CaptureScreenshot(Path.Combine(folder,"tocador.png"));yield return new WaitForSecondsRealtime(.3f);deck.PlayPause();SceneManager.LoadScene("IgrejaMatriz");yield return new WaitForSecondsRealtime(4);Check(!SantaRitaTapeDeck.IsOpen&&!GameState.Tem("fita.consultada.fita01"),"Troca de cena interrompe sem registrar escuta completa.");
        Check(FindObjectsByType<SantaRitaTapePickup>().Any(t=>t.tapeId=="fita03"),"Fita física anexada ao material de som da Igreja.");
        SceneManager.LoadScene("EntradaDaCidade");yield return new WaitForSecondsRealtime(5);
        Check(FindObjectsByType<SantaRitaTapePickup>().Any(t=>t.tapeId=="fita01"),"Fita inicial existe junto dos retratos da casa do avô.");
        Check(!CheckpointSystem.HasCheckpoint,"Reparo, coleta e organização não criam autosave.");CheckpointSystem.SaveSlot(0,"IgrejaMatriz",Vector2.zero,"MILA_QA");GameState.ImportarSnapshot(new GameStateSnapshot());CheckpointSystem.TryLoadSlot(0,out var saved);GameState.ImportarSnapshot(saved.estado);
        Check(SantaRitaInventory.Owns("tocador")&&SantaRitaInventory.Quick(1)=="fita03"&&book.MarkedEntry==marked,"Save manual restaura aparelho, atalhos e marcador.");
        GameState.Marcar(Flags.ApresentouReciboAtendente);Check(!SantaRitaInventory.Contents().Contains("recibo"),"Recibo entregue sai da bolsa sem apagar outros itens.");
        report.Add("Fixtures e transcrições; vozes humanas não produzidas nem aprovadas. Áudio final pendente.");
    }
}
#endif
