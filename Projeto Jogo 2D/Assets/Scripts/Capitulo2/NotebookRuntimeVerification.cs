#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed partial class Chapter2RuntimeVerification
{
    private static bool notebookForce;
    private Mouse notebookMouse;
#if UNITY_EDITOR
    public static void RunNotebookInEditor()
    {
        if(!Application.isPlaying)throw new Exception("QA explícito requer fixture Play.");
        Application.runInBackground=true;notebookForce=true;var go=new GameObject("Caderno_QA_Explicit");DontDestroyOnLoad(go);go.AddComponent<Chapter2RuntimeVerification>();
    }
#endif
    private IEnumerator RunNotebook()
    {
        Screen.SetResolution(1280,720,FullScreenMode.Windowed);
        var mouse=InputSystem.AddDevice<Mouse>();notebookMouse=mouse;mouse.MakeCurrent();
        var kb=InputSystem.AddDevice<Keyboard>();kb.MakeCurrent();sinucaR4Keyboard=kb;
        InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(10,10)});
        InputSystem.QueueStateEvent(kb,new KeyboardState());yield return null;
        Check(Resources.Load<Texture2D>("Atlas/Paisagem")!=null,"Entorno artístico presente no Player.");
        var hand=Resources.Load<TMP_FontAsset>("Fonts/Kalam/MarcusHand");Check(hand!=null&&hand.HasCharacters("ãçéóáôíúÀÉÓÇ“”"),"Caligrafia possui acentos e aspas em português.");
        Check(MarcusNotebookContent.Read().Count==1,"Caderno novo só conhece a abertura pessoal, não futuras pistas.");
        var source=new EntradaDiario{id="pista_relato_mercadinho",categoria="pista"};
        Check(!MarcusNotebookContent.Project(source).body.Contains("entre os que levaram"),"Texto anterior não antecipa relato do Zé.");
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.MapaDoCarroObtido);GameState.Marcar(Flags.LanternaDoCarroObtida);
        SceneManager.LoadScene("CentroSantaRita");yield return new WaitForSecondsRealtime(2);yield return CloseText();
        var map=MapaInterativoUI.EnsureInstance();var notebook=map.GetComponent<MarcusNotebookUI>();
        Check(notebook!=null,"Caderno integrado à exploração.");
        Check(Field<TMP_Text>(map,"textoLocalAtual")!=null,"Painel geográfico preservado.");
        int updates=notebook.UpdateCount;
        GameState.RegistrarNotaDiario("pista_cafe_hoje","Café pela metade","pista","Café ainda morno.");
        Check(notebook.UpdateCount==updates+1&&notebook.HasPendingNotice,"Nova anotação pede aviso de caderno atualizado.");
        GameState.RegistrarNotaDiario("pista_cafe_hoje","Café pela metade","pista","Café ainda morno.");
        Check(notebook.UpdateCount==updates+1,"Repetição idêntica não repete aviso.");
        GameState.RegistrarNotaDiario("casa_adilio_entrada","Casa aberta","fato","A porta estava destrancada.");
        GameState.RegistrarNotaDiario("pista_desordem_porta","Casaco e banco","pista","Banco tombado e casaco no prego.");
        GameState.RegistrarNotaDiario("c2_protocolo","Pedido do vô","pista","Pedido público de Adílio.");
        yield return new WaitForSecondsRealtime(.3f);yield return Capture("aviso-caderno");
        var snapshot=GameState.ExportarSnapshot();updates=notebook.UpdateCount;GameState.ImportarSnapshot(snapshot);
        Check(notebook.UpdateCount==updates&&!notebook.HasPendingNotice,"Carga do snapshot não anuncia anotações antigas.");
        map.Abrir(1);yield return new WaitForSecondsRealtime(.2f);
        Check(notebook.IsOpen&&MapaInterativoUI.IsShowing&&!Field<GameObject>(map,"mapFrame").activeInHierarchy,"Caderno ocupa tela própria e mantém bloqueio da exploração.");
        Check(notebook.PageCount>=4,"Entradas adquiridas paginadas sem corte.");
        Check(notebook.GetComponentsInChildren<TMP_Text>().Where(t=>t.name.StartsWith("Pagina")).All(t=>t.font==hand),"Páginas usam escrita manuscrita, não a fonte digital do menu.");
        yield return Capture("caderno-aberto");
        notebook.Turn(1);yield return new WaitForSecondsRealtime(.5f);Check(notebook.Spread==1,"Virada chega à folha seguinte.");yield return Capture("caderno-segunda-folha");
        notebook.Turn(-1);yield return new WaitForSecondsRealtime(.5f);Check(notebook.Spread==0,"Pode voltar a ler folhas anteriores.");
        float zoom=Field<float>(map,"mapZoom");
        mouse.MakeCurrent();
        InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(Screen.width*.5f,Screen.height*.5f),scroll=new Vector2(0,-120)});
        yield return null;yield return new WaitForSecondsRealtime(.5f);
        Check(Field<float>(map,"mapZoom")==zoom,"Scroll do caderno não muda zoom do mapa oculto.");
        InputSystem.QueueStateEvent(kb,new KeyboardState(Key.F));yield return null;yield return null;
        Check(!GameState.LanternaLigada,"F não equipa lanterna enquanto lê o caderno.");
        InputSystem.QueueStateEvent(kb,new KeyboardState(Key.Escape));yield return null;yield return null;
        Check(!MapaInterativoUI.IsShowing&&!notebook.IsOpen,"Escape fecha o caderno.");
        InputSystem.QueueStateEvent(kb,new KeyboardState());yield return null;
        foreach(int tab in new[]{2,3})
        {
            map.Abrir(tab);yield return null;zoom=Field<float>(map,"mapZoom");
            var pane=(RectTransform)Field<GameObject>(map,"sidePanelObject").transform;
            var point=RectTransformUtility.WorldToScreenPoint(null,pane.position);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=point,scroll=new Vector2(0,-120)});yield return null;yield return null;
            Check(Field<float>(map,"mapZoom")==zoom,"Scroll do painel "+tab+" não aproxima ou afasta mapa.");
        }
        map.Abrir(2);string text=Field<TMP_Text>(map,"textoDiarioConteudo").text;
        Check(!text.Contains("nº 1")&&!text.Contains("nº 2")&&!text.Contains("Galpão"),"Fichas não adquiridas não são listadas.");
        GameState.Marcar(Capitulo2Director.Ficha);map.Abrir(2);text=Field<TMP_Text>(map,"textoDiarioConteudo").text;
        Check(text.Contains("nº 2")&&!text.Contains("nº 1"),"Só ficha 2 se só ela foi recebida.");
        GameState.Marcar(Flags.RecebeuFichaClubeZe);GameState.Marcar("ficha_clube_truco_03");GameState.Marcar(Flags.Pista_NotaFiscal2004);map.Abrir(2);
        text=Field<TMP_Text>(map,"textoDiarioConteudo").text;
        Check(text.IndexOf("nº 1")<text.IndexOf("nº 2")&&text.IndexOf("nº 2")<text.IndexOf("nº 3"),"Ordem numérica 1, 2, 3 independentemente da coleta.");
        Check(!text.Contains("Cupom")&&!text.Contains("Café")&&!text.Contains("Pendente"),"Sem recibo, prova solta ou ficha pendente nessa lista.");
        GameState.Marcar("ficha_clube_truco_03",false);map.Abrir(2);yield return Capture("fichas-adquiridas");
        map.Abrir(0);yield return null;Invoke(map,"ResetarZoomEPan");zoom=Field<float>(map,"mapZoom");
        var viewport=Field<RectTransform>(map,"mapViewportRect");var center=RectTransformUtility.WorldToScreenPoint(null,viewport.position);
        Check(!Field<GameObject>(map,"sidePanelObject").activeInHierarchy&&GameState.Tem(Flags.MapaDoCarroObtido),"Carta normal disponível para o teste de scroll.");
        InputSystem.QueueStateEvent(mouse,new MouseState{position=center,scroll=new Vector2(0,120)});yield return null;yield return null;
        Check(Field<float>(map,"mapZoom")>zoom,"Scroll continua funcionando na carta da cidade.");
        Invoke(map,"ResetarZoomEPan");yield return Capture("mapa-paisagem");
        Invoke(map,"SelecionarMarcoAtlas","prefeitura");zoom=Field<float>(map,"mapZoom");
        var panelPoint=RectTransformUtility.WorldToScreenPoint(null,Field<GameObject>(map,"sidePanelObject").transform.position);
        InputSystem.QueueStateEvent(mouse,new MouseState{position=panelPoint,scroll=new Vector2(0,120)});yield return null;yield return null;
        Check(Field<float>(map,"mapZoom")==zoom,"Painel contextual também intercepta scroll.");
        InputSystem.RemoveDevice(mouse);notebookMouse=null;InputSystem.RemoveDevice(kb);sinucaR4Keyboard=null;map.Fechar();
        Check(!CheckpointSystem.HasCheckpoint,"Atualizações de caderno não criam autosave.");
        CheckpointSystem.SaveSlot(0,"CentroSantaRita",Vector2.zero,"QA_CADERNO");
        GameState.ImportarSnapshot(new GameStateSnapshot());CheckpointSystem.TryLoadSlot(0,out var saved);GameState.ImportarSnapshot(saved.estado);
        Check(GameState.ObterEntradasDiario().Any(e=>e.id=="pista_cafe_hoje"),"Save manual isolado restaura anotações adquiridas.");
        Check(!notebook.HasPendingNotice,"Restaurar anotações não cria aviso falso.");
        Finish();
    }
}
#endif
