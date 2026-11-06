#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public sealed partial class Chapter2RuntimeVerification
{
    private static bool atlasForce;
#if UNITY_EDITOR
    public static void RunAtlasInEditor()
    {
        if(!Application.isPlaying)throw new Exception("Entrar na fixture Play antes do QA.");
        atlasForce=true;var go=new GameObject("Atlas_QA_Explicit");DontDestroyOnLoad(go);go.AddComponent<Chapter2RuntimeVerification>();
    }
#endif
    private IEnumerator RunAtlas()
    {
        Screen.SetResolution(1280,720,FullScreenMode.Windowed);
        yield return null;
        Check(SantaRitaAtlas.Survey.regions.Count==19,"19 regiões levantadas, sem capítulos futuros.");
        foreach(var r in SantaRitaAtlas.Survey.regions)
        {
            Check(r.local.width>0&&r.local.height>0,"Limites não degenerados "+r.id);
            foreach(var p in new[]{r.local.min,r.local.max,r.local.center,Vector2.Lerp(r.local.min,r.local.max,.17f)})
                Check(Vector2.Distance(p,r.ToLocal(r.ToAtlas(p)))<.001f,"Transformação inversível "+r.id+" "+p);
            for(int lod=0;lod<3;lod++)Check(Resources.Load<Texture2D>("Atlas/"+r.id+"_"+lod)!=null,"Camada "+lod+" "+r.id+" presente na build");
        }
        foreach(var a in SantaRitaAtlas.Survey.accesses)
            Check(a.resolved&&SantaRitaAtlas.Region(a.destination)!=null,"Acesso resolvido "+a.region+" → "+a.destination);
        Check(SantaRitaAtlas.Annotation("prefeitura")=="","Sem anotação de protocolo antes de descobrir.");
        GameState.Marcar("c2_protocolo_adilio");
        Check(SantaRitaAtlas.Annotation("prefeitura").Contains("protocolo"),"Anotação deriva da pista real.");
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.ChegouCasaAvo);GameState.Marcar(Flags.LanternaDoCarroObtida);
        SceneManager.LoadScene("CentroSantaRita");yield return new WaitForSecondsRealtime(2);
        var map=MapaInterativoUI.EnsureInstance();map.Abrir(0);
        Check((int)typeof(MapaInterativoUI).GetField("abaAtiva",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(map)==1,"Sem mapa, M abre diário sem antecipar atlas.");map.Fechar();
        GameState.Marcar(Flags.MapaDoCarroObtido);
        map.Abrir(0);yield return new WaitForSecondsRealtime(.5f);yield return Capture("01-cidade");
        var type=typeof(MapaInterativoUI);var binding=BindingFlags.Instance|BindingFlags.NonPublic;
        var mouse=InputSystem.AddDevice<Mouse>();mouse.MakeCurrent();
        var viewport=(RectTransform)type.GetField("mapViewportRect",binding).GetValue(map);
        Vector2 screenPoint=RectTransformUtility.WorldToScreenPoint(null,viewport.position);
        InputSystem.QueueStateEvent(mouse,new MouseState{position=screenPoint,scroll=new Vector2(0,120)});yield return null;yield return null;
        Check((float)type.GetField("mapZoom",binding).GetValue(map)>1,"Scroll real aproxima atlas.");InputSystem.RemoveDevice(mouse);
        type.GetMethod("AplicarZoomIncremental",binding).Invoke(map,new object[]{100f,Vector2.zero});
        Check((float)type.GetField("mapZoom",binding).GetValue(map)==12,"Zoom máximo limitado a 12x.");
        var area=(RectTransform)type.GetField("mapAreaRect",binding).GetValue(map);var pan=area.anchoredPosition;
        type.GetMethod("SelecionarAba",binding).Invoke(map,new object[]{1});type.GetMethod("SelecionarAba",binding).Invoke(map,new object[]{0});
        Check(area.anchoredPosition==pan&&(float)type.GetField("mapZoom",binding).GetValue(map)==12,"Abas preservam zoom e posição.");
        type.GetMethod("ResetarZoomEPan",binding).Invoke(map,null);
        foreach(string focus in new[]{"prefeitura","igreja","canto_se"})
        {
            var door=SantaRitaAtlas.Survey.landmarks.Find(l=>l.region=="CentroSantaRita"&&SantaRitaAtlas.Building(l)==focus);
            Check(door!=null,"Entrada cartográfica identificada "+focus);
            FindAnyObjectByType<PlayerController>().TeleportarPara(door.local+Vector2.down*2);yield return new WaitForSecondsRealtime(.2f);
            type.GetMethod("ResetarZoomEPan",binding).Invoke(map,null);type.GetMethod("CentralizarNoJogador",binding).Invoke(map,null);
            type.GetMethod("AplicarZoomIncremental",binding).Invoke(map,new object[]{2f,Vector2.zero});yield return new WaitForSecondsRealtime(.15f);
            if(focus=="prefeitura")
            {
                Vector2 noteUV=SantaRitaAtlas.UV(SantaRitaAtlas.Region(door.region).ToAtlas(door.local)+Vector2.up*2);
                Check(map.GetComponentsInChildren<RectTransform>(true).Any(t=>t.name=="Nome_Anotação de Marcus"&&Vector2.Distance(t.anchorMin,noteUV)<.001f),"Anotação de protocolo situada na Prefeitura real, inclusive quando acesso tem alias.");
            }
            yield return Capture("foco-"+focus);
            if(focus=="prefeitura")
            {
                type.GetMethod("SelecionarMarcoAtlas",binding).Invoke(map,new object[]{door.id});yield return Capture("anotacao-prefeitura");
                type.GetField("pontoSelecionadoId",binding).SetValue(map,"");type.GetMethod("SelecionarAba",binding).Invoke(map,new object[]{0});
            }
            if(focus=="igreja")
            {type.GetMethod("AplicarZoomIncremental",binding).Invoke(map,new object[]{100f,Vector2.zero});yield return new WaitForSecondsRealtime(.1f);yield return Capture("zoom-maximo");}
        }
        type.GetMethod("ResetarZoomEPan",binding).Invoke(map,null);
        foreach(var scene in new[]{"CentroSantaRita","CemiterioSantaRita","SociedadeBeiraRio","IgrejaMatriz"})
        {
            map.Fechar();SceneManager.LoadScene(scene);yield return new WaitForSecondsRealtime(1.5f);yield return CloseText();
            var player=FindAnyObjectByType<PlayerController>();var region=SantaRitaAtlas.Current();
            Check(region!=null&&region.scene==scene,"Região atual "+scene);
            yield return Capture("cena-"+scene);
            map.Abrir(0);var view=FindAnyObjectByType<SantaRitaAtlasView>();
            foreach(var sample in new[]{region.local.center,region.local.min+Vector2.one*3,region.local.max-Vector2.one*3})
            {
                player.TeleportarPara(sample);yield return null;
                var uv=SantaRitaAtlas.Player(sample,out _);var marker=(RectTransform)typeof(MapaInterativoUI).GetField("playerMarkerRect",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(map);
                Check(region.interior||Vector2.Distance(marker.anchorMin,uv)<.001f,"Marcador segue a transformação em "+scene);
            }
            player.TeleportarPara(region.local.center);yield return null;
            if(region.interior)
            {
                Check(SantaRitaAtlas.Known(region),"Interior visitado disponível.");view.ShowInterior(region.id);
                var marker=(RectTransform)type.GetField("playerMarkerRect",binding).GetValue(map);Check(marker.GetSiblingIndex()==marker.parent.childCount-1,"Marcador permanece acima da planta após trocar visualização.");
                typeof(MapaInterativoUI).GetMethod("ResetarZoomEPan",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(map,null);
            }
            else
            {
                typeof(MapaInterativoUI).GetMethod("CentralizarNoJogador",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(map,null);
            }
            yield return Capture("mapa-"+scene);
            typeof(MapaInterativoUI).GetMethod("AplicarZoomIncremental",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(map,new object[]{2f,Vector2.zero});
            yield return Capture("detalhe-"+scene);
            view.ShowInterior(null);map.Fechar();
        }
        SceneManager.LoadScene("EntradaDaCidade");yield return new WaitForSecondsRealtime(2);yield return CloseText();
        foreach(var zone in new[]{Capitulo1Director.ZonaId.Zona4_Bairro,Capitulo1Director.ZonaId.Zona5_RuaNorteBar,Capitulo1Director.ZonaId.Interior_CasaDoAvo})
        {
            map.Fechar();Capitulo1Director.TryObterInfoZona(zone,out var info);Capitulo1Director.Instance.TransicionarPara(zone,info.min+Vector2.one*4,0);
            yield return new WaitForSecondsRealtime(1);yield return CloseText();
            var r=SantaRitaAtlas.Current();Check(r.zone==(int)zone,"Zona C1 corretamente identificada "+zone);
            map.Abrir(0);typeof(MapaInterativoUI).GetMethod("CentralizarNoJogador",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(map,null);
            yield return Capture("mapa-"+zone);
        }
        var kb=InputSystem.AddDevice<Keyboard>();kb.MakeCurrent();InputSystem.QueueStateEvent(kb,new KeyboardState(Key.Escape));yield return null;yield return null;
        Check(!MapaInterativoUI.IsShowing,"Escape fecha mapa com input real.");InputSystem.RemoveDevice(kb);
        Check(!CheckpointSystem.HasCheckpoint,"QA não gravou slot pessoal ou isolado.");
        GameState.Marcar("atlas.visit.casa_interior");CheckpointSystem.SaveSlot(0,"EntradaDaCidade",new Vector2(420,4),"QA_ATLAS");
        GameState.Marcar("atlas.visit.casa_interior",false);CheckpointSystem.TryLoadSlot(0,out var saved);GameState.ImportarSnapshot(saved.estado);
        Check(GameState.Tem("atlas.visit.casa_interior"),"Save manual isolado restaura descobertas.");
        GameState.Marcar("atlas.visit.bairro",false);GameState.ImportarSnapshot(saved.estado);
        Check(GameState.Tem("atlas.visit.bairro"),"Alteração não salva é descartada ao recarregar snapshot.");
        GameState.ImportarSnapshot(new GameStateSnapshot());Check(!GameState.Tem("atlas.visit.casa_interior"),"Snapshot antigo sem campos atlas continua compatível.");
        Finish();
    }
}
#endif
