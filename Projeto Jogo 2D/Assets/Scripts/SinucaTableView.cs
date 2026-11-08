using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SinucaCameraView
{
    Mesa3D = 0,
    VistaSuperior = 1,
    LinhaTaco = 2,
    Caminhando3D = 3
}

public enum SinucaCueSpin
{
    Normal = 0,
    Avancar = 1,
    Recuar = 2
}

public struct SinucaVisualBall
{
    public int numero;
    public Vector2 pos;
    public Vector2 vel;
    public bool ativa;
    public bool caindo;
    public float quedaProgresso;
    public Vector2 cacapaAlvo;
    public Quaternion rotacao;
    public bool marcada;
}

public struct SinucaAimGuideData
{
    public bool ativa;
    public Vector2 origem;
    public Vector2 pontoContato;
    public bool atingeBola;
    public int numeroBolaAlvo;
    public bool alvoEhMarcada;
    public Vector2 posBolaAlvo;
    public Vector2 direcaoBolaAlvo;
    public Vector2 direcaoBrancaPosContato;
}

/// <summary>GPU room view; one persistent UI quad, true depth and perspective-correct textures.</summary>
public sealed class SinucaTableView : RawImage, IPointerDownHandler,IPointerMoveHandler,IPointerUpHandler,IDragHandler,IScrollHandler
{
    public const float TableWidth=420,TableHeight=210,HalfW=210,HalfH=105,BallRadius=5.4f,PocketRadius=SinucaMinigameUI.PocketRadius;
    public SinucaCameraView CameraModo{get;private set;}=SinucaCameraView.Caminhando3D;
    public float Zoom{get;private set;}=1;
    public Action<Vector2,PointerEventData.InputButton> AoPressionarMesa;
    public Action<Vector2> AoMoverMesa;
    public Action<float> AoAjustarMira;
    public Action<Vector2,bool,PointerEventData.InputButton> AoSoltarMesa;
    public Action<float> AoScrollZoom;
    public SinucaRoom3D Room{get;private set;}
    private float lastAngle;
    private bool ready;
    private Camera worldCamera;
    private int worldMask;
    private Camera pointerCamera;
    private Vector2 lastPointerLocal;
    private bool hasPointerReference;
    public bool ScreenToViewPoint(Vector2 screen, out Vector2 local)
        => RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen, pointerCamera, out local);
    public void ReiniciarReferenciaPonteiro() { hasPointerReference = false; }
    protected override void Awake(){base.Awake();raycastTarget=true;}
    private void EnsureRoom()
    {
        if(ready&&Room!=null&&Room.root!=null)return;if(ready&&(Room==null||Room.root==null)){var old=GameObject.Find("SalaSinuca3D_Persistente");if(old!=null)Destroy(old);}Room=new SinucaRoom3D(1280,720);texture=Room.Target;Room.SetMode(CameraModo,lastAngle);ready=true;Room.SetActive(isActiveAndEnabled);
    }
    public void Preaquecer(){EnsureRoom();}
    protected override void OnEnable(){base.OnEnable();worldCamera=Camera.main;if(worldCamera!=null){worldMask=worldCamera.cullingMask;worldCamera.cullingMask=0;}if(ready){EnsureRoom();Room.SetActive(true);}}
    protected override void OnDisable(){if(worldCamera!=null)worldCamera.cullingMask=worldMask;if(Room!=null && !BarDoZe3D.Aberto)Room.SetActive(false);base.OnDisable();}
    protected override void OnDestroy(){if(Room!=null)Room.Dispose();base.OnDestroy();}
    // The explicit override is also a stable profiling seam: only a 4-vertex quad now.
    protected override void OnPopulateMesh(VertexHelper vh){base.OnPopulateMesh(vh);}
    public void InicializarOverlays(TMP_FontAsset font){}
    public void DefinirCamera(SinucaCameraView value){CameraModo=value;if(ready)Room.SetMode(value,lastAngle);}
    public void AlternarCamera(){DefinirCamera(CameraModo==SinucaCameraView.Mesa3D?SinucaCameraView.LinhaTaco:CameraModo==SinucaCameraView.LinhaTaco?SinucaCameraView.VistaSuperior:SinucaCameraView.Mesa3D);}
    public void DefinirZoom(float value){Zoom=Mathf.Clamp(value,.8f,1.35f);if(ready)Room.SetZoom(Zoom);}
    public void AjustarZoom(float amount)=>DefinirZoom(Zoom+amount);
    public void Orbit(float degrees){if(ready)Room.Orbit(degrees);}
    public void RecentrarCamera(){if(ready)Room.Recenter(lastAngle);}
    public Vector2 ProjetarPontoMesaParaLocalUI(Vector3 point)
    {
        EnsureRoom();Vector2 v=Room.Project(point);var r=rectTransform.rect;return new Vector2(r.xMin+v.x*r.width,r.yMin+v.y*r.height);
    }
    public bool ScreenToTablePoint(Vector2 screen,Camera cam,out Vector2 point)
    {
        point=Vector2.zero;EnsureRoom();if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,screen,cam,out var p))return false;var r=rectTransform.rect;return Room.RayToTable(new Vector2((p.x-r.xMin)/r.width,(p.y-r.yMin)/r.height),out point);
    }
    public float AngularScreenSign(Vector2 at,float angle)
    {
        Vector2 a=at+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*50,b=at+new Vector2(Mathf.Cos(angle+.01f),Mathf.Sin(angle+.01f))*50;
        float dx=ProjetarPontoMesaParaLocalUI(new Vector3(b.x,0,b.y)).x-ProjetarPontoMesaParaLocalUI(new Vector3(a.x,0,a.y)).x;return Mathf.Abs(dx)<.0001f?-1:Mathf.Sign(dx);
    }
    public void AtualizarEstadoRender(IList<SinucaVisualBall> balls,Vector2[] pockets,SinucaAimGuideData guide,bool showCue,Vector2 white,float angle,float power,SinucaCueSpin spin,bool hand,bool valid,int frame,int marked,bool reduced,float light,bool chalk,bool silence)
    {EnsureRoom();var rect=rectTransform.rect;if(rect.height>0)Room.SetViewportAspect(rect.width/rect.height);lastAngle=angle;Room.Sync(balls,white,angle,power,showCue,guide);}
    public void OnPointerDown(PointerEventData e)
    {
        if(Cursor.lockState==CursorLockMode.Locked)return;
        pointerCamera=e.pressEventCamera;
        if(CameraModo==SinucaCameraView.LinhaTaco){AoPressionarMesa?.Invoke(Vector2.zero,e.button);return;}
        if(ScreenToTablePoint(e.position,e.pressEventCamera,out var p))AoPressionarMesa?.Invoke(p,e.button);
    }
    public void OnPointerMove(PointerEventData e)
    {
        if(Cursor.lockState==CursorLockMode.Locked)return;
        pointerCamera=e.enterEventCamera;
        if(!ScreenToViewPoint(e.position,out var local))return;
        if(!hasPointerReference){lastPointerLocal=local;hasPointerReference=true;return;}
        // Reject only duplicate positions. Subpixel motion must remain available for fine aim.
        if((local-lastPointerLocal).sqrMagnitude<0.000001f)return;
        var movement=local-lastPointerLocal;
        lastPointerLocal=local;
        if(CameraModo==SinucaCameraView.Caminhando3D)return;
        if(CameraModo==SinucaCameraView.LinhaTaco){AoAjustarMira?.Invoke(movement.x*.003f);return;}
        if(ScreenToTablePoint(e.position,e.enterEventCamera,out var p))AoMoverMesa?.Invoke(p);
    }
    public void OnDrag(PointerEventData e){if(e.button==PointerEventData.InputButton.Middle)return;OnPointerMove(e);}
    public void OnPointerUp(PointerEventData e){if(Cursor.lockState==CursorLockMode.Locked)return;bool inside=RectTransformUtility.RectangleContainsScreenPoint(rectTransform,e.position,e.pressEventCamera);ScreenToTablePoint(e.position,e.pressEventCamera,out var p);AoSoltarMesa?.Invoke(p,inside,e.button);}
    public void OnScroll(PointerEventData e){} // no camera zoom while walking or leaning over the table
    public static Color CorDaBola(int n)
    {
        if(n>8)n-=8;
        switch(n){case 0:return new Color32(237,235,218,255);case 1:return new Color32(230,187,30,255);case 2:return new Color32(35,79,161,255);case 3:return new Color32(190,38,29,255);case 4:return new Color32(107,56,135,255);case 5:return new Color32(225,114,29,255);case 6:return new Color32(37,117,64,255);case 7:return new Color32(127,37,29,255);default:return new Color32(22,24,21,255);}
    }
}
