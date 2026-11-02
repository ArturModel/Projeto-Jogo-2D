#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public sealed partial class Chapter2RuntimeVerification
{
    private IEnumerator Ref7Checks(SinucaMinigameUI ui,SinucaTableView view)
    {
        yield return new WaitForSecondsRealtime(2);
        foreach(var prop in SantaRitaBarLayout.Props)
        {
            var native=SantaRitaBarLayout.SpriteFor(prop);var live=GameObject.Find(prop.id).GetComponent<SpriteRenderer>();
            Check(native!=null&&view.Room.PixelSources[prop.id]==native,"Arte 2D é a mesma fonte da perspectiva: "+prop.id);
            Check(live.sharedMaterial==Resources.Load<Material>("Art/WorldLit"),"Material lit do restante do jogo: "+prop.id);
            if(prop.kind!=SantaRitaBarLayout.Kind.Actor)Check(live.sprite==native,"Exploração preserva sprite nativo: "+prop.id);
        }
        Check(GameObject.Find("NPC_Ze_Interior").GetComponent<SpriteRenderer>().sprite==Resources.Load<Sprite>("Art/Characters/Npc_Ze"),"Zé mantém pixel art original junto de Marcus");
        Check(view.Room.FloorTile==SantaRitaBarLayout.NativeSurface(false).texture&&view.Room.WallTile==SantaRitaBarLayout.NativeSurface(true).texture,"Piso e parede compartilham exatamente as texturas 2D");
        Check(view.Room!=null&&view.texture is RenderTexture,"Sala 3D persistente com RenderTexture real");
        Check(view.Room.Target.width==1280&&view.Room.Target.height==560,"Renderização da mesa em 1280x560, independente do pixel canvas");
        var method=typeof(SinucaTableView).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null);var vh=new VertexHelper();
        for(int j=0;j<10;j++)method.Invoke(view,new object[]{vh});
        var watch=System.Diagnostics.Stopwatch.StartNew();for(int j=0;j<200;j++)method.Invoke(view,new object[]{vh});watch.Stop();
        double average=watch.Elapsed.TotalMilliseconds/200;report.Add("PERF quad: "+average.ToString("F6",System.Globalization.CultureInfo.InvariantCulture)+" ms/rebuild; "+vh.currentVertCount+" vertices; 200 invocações reais após aquecimento.");
        Check(vh.currentVertCount==4&&average<2,"Quad de UI não reconstrói a sala por frame (<2 ms no host atual)");vh.Dispose();
        var times=new List<float>();for(int j=0;j<180;j++){yield return null;times.Add(Time.unscaledDeltaTime*1000);}
        times.Sort();report.Add("PERF frames reais: p50="+times[90].ToString("F2")+"ms; p95="+times[171].ToString("F2")+"ms; max="+times[179].ToString("F2")+"ms; foco="+Application.isFocused+"; vsync="+QualitySettings.vSyncCount+"; limite="+Application.targetFrameRate+"; resolução="+Screen.width+"x"+Screen.height+". Não equivale a certificação em outras GPUs.");
        Check(view.Room.root.GetComponentsInChildren<Camera>(true).Length==1,"Uma câmera 3D da sala, sem duplicatas");
        for(int j=0;j<6;j++)
        {
            var net=view.Room.root.transform.Find("ElementosMoveis/RedeExterna_"+j);
            Check(net!=null&&net.childCount>50,"Caçapa "+j+" possui rede externa volumétrica");
            var mouth=view.Room.root.GetComponentsInChildren<Transform>(true).First(t=>t.name=="BocaExterna_"+j);
            Check(Mathf.Abs(mouth.localPosition.z)>.71f,"Boca "+j+" fica além do pano");
        }
        Check(view.Room.ArtModels.ContainsKey("Inter1994")&&view.Room.ArtModels.ContainsKey("RadioBalcao"),"Bandeira histórica e rádio fazem parte da mesma planta");
        Check(Field<UnityEngine.UI.Button>(ui,"btnToggleEfeito")==null,"Sem botão de efeito: contato selecionado exclusivamente na branca");
        var balls=Field<System.Collections.IList>(ui,"bolas");Vector2 at=(Vector2)balls[0].GetType().GetField("pos").GetValue(balls[0]);
        foreach(var mode in new[]{SinucaCameraView.Mesa3D,SinucaCameraView.LinhaTaco,SinucaCameraView.VistaSuperior})
        {
            view.DefinirCamera(mode);Invoke(ui,"AtualizarBotaoVistaCamera");yield return null;
            var point=ScreenSinuca(view,new Vector3(at.x+35,SinucaTableView.BallRadius*.4f,at.y+12));
            yield return PointerSinuca(point,false);float a=Field<float>(ui,"anguloMiraRad");
            yield return PointerSinuca(point,false);yield return null;
            Check(Mathf.Abs(Mathf.DeltaAngle(a*Mathf.Rad2Deg,Field<float>(ui,"anguloMiraRad")*Mathf.Rad2Deg))<.1f,"Mouse absoluto estável, sem realimentar câmera: "+mode);
            yield return Capture("ref7-"+mode);
        }
        view.DefinirCamera(SinucaCameraView.Mesa3D);Invoke(ui,"AtualizarBotaoVistaCamera");
        Invoke(ui,"AbrirEscolhaAlvo");yield return null;
        Check(Field<GameObject>(ui,"painelAlvo").activeSelf,"Seleção de alvo abre explicação contextual");yield return Capture("ref7-escolher-alvo");Invoke(ui,"AbrirEscolhaAlvo");
    }
}
#endif
