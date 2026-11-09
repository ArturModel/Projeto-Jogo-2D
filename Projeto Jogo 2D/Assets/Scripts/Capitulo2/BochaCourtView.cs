using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum BochaCameraView { Lancamento, Alvo, Superior, Caminhando }

// A persistent room rendered by the same URP 3D renderer used by the sinuca.
public sealed class BochaCourtView : RawImage, IPointerDownHandler, IDragHandler,
    IPointerUpHandler, IPointerMoveHandler, IScrollHandler
{
    public Action<PointerEventData> Pressed, Dragged, Released, Hovered;
    public Action<float> Scrolled;
    public Action WalkingClicked;
    public BochaCameraView View { get; private set; } = BochaCameraView.Caminhando;
    public float Zoom { get; private set; } = 1;
    public int TrackCount => Room == null ? 0 : Room.TrackCount;
    public float AimDistance { get; private set; } = 12.65f;
    public BochaRoom3D Room { get; private set; }
    private BochaSimulation simulation;
    private BochaShot preview;
    private bool aiming;
    private Camera worldCamera;
    private int worldMask;
    private void EnsureRoom()
    {
        if(Room!=null&&Room.root!=null)return;
        Room=new BochaRoom3D();texture=Room.Target;Room.SetActive(isActiveAndEnabled);
    }
    protected override void Awake(){base.Awake();raycastTarget=true;}
    internal void Preaquecer() => EnsureRoom();
    internal void OlharPendente(float peso)
    {
        Room.SetEntryLook(peso);
        Sync();
    }
    protected override void OnEnable()
    {
        base.OnEnable();worldCamera=Camera.main;
        EnsureRoom();
        if (worldCamera != null)
        {
            worldMask = worldCamera.cullingMask;
            worldCamera.cullingMask = 0;
        }
        Room.SetActive(true);
    }
    protected override void OnDisable()
    {
        ReleaseMouse();if(worldCamera!=null)worldCamera.cullingMask=worldMask;
        if(Room!=null)Room.SetActive(false);base.OnDisable();
    }
    protected override void OnDestroy(){if(Room!=null)Room.Dispose();base.OnDestroy();}
    public void SetState(BochaSimulation sim,BochaShot shot,bool canAim,float aimDistance=12.65f)
    {
        EnsureRoom();if(simulation!=sim)Room.ClearTracks();simulation=sim;preview=shot;aiming=canAim;
        AimDistance=Mathf.Clamp(aimDistance,.8f,19.15f);Sync();
    }
    private void Sync()
    {
        if(Room==null)return;var rect=rectTransform.rect;
        Room.UpdateCamera(View,preview.x,Zoom,simulation,rect.height>0?rect.width/rect.height:16f/9);
        Room.Sync(simulation,preview,aiming,AimDistance);
    }
    public void SetView(BochaCameraView view){EnsureRoom();View=view;Zoom=1;ReleaseMouse();Sync();}
    public void ResetWalking(){EnsureRoom();Room.ResetWalking();SetView(BochaCameraView.Caminhando);}
    public void ChangeZoom(float amount){if(View==BochaCameraView.Caminhando)return;Zoom=Mathf.Clamp(Zoom+amount*.08f,.8f,1.35f);Sync();}
    public void Walk(Vector2 axis,Vector2 look,float dt,bool fast){EnsureRoom();Room.Walk(axis,look,dt,fast);Sync();}
    public void Tick(float dt)=>Sync();
    public void RecordContacts(){if(Room!=null)Room.RecordContacts(simulation);}
    public void CaptureMouse(){if(View==BochaCameraView.Caminhando){Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;}}
    public void ReleaseMouse(){Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
    public Vector2 Project(Vector3 p)
    {
        EnsureRoom();Sync();var v=Room.camera.WorldToViewportPoint(p);var r=rectTransform.rect;
        return new Vector2(r.xMin+v.x*r.width,r.yMin+v.y*r.height);
    }
    public bool CourtPoint(Vector2 screen,Camera camera,out Vector2 point)
    {
        point=Vector2.zero;EnsureRoom();Sync();
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,screen,camera,out var local))return false;
        var r=rectTransform.rect;var ray=Room.camera.ViewportPointToRay(new Vector3((local.x-r.xMin)/r.width,(local.y-r.yMin)/r.height));
        if(!new Plane(Vector3.up,Vector3.zero).Raycast(ray,out var t)||t>160)return false;
        var p=ray.GetPoint(t);point=new Vector2(p.x,p.z);return true;
    }
    public static Color BallColor(BochaSide side)=>side==BochaSide.Marcus?new Color(.15f,.53f,.59f):side==BochaSide.Tonao?new Color(.73f,.32f,.14f):new Color(.91f,.85f,.63f);
    public void OnPointerDown(PointerEventData e){if(View==BochaCameraView.Caminhando){if(e.button==PointerEventData.InputButton.Left)WalkingClicked?.Invoke();return;}Pressed?.Invoke(e);}
    public void OnDrag(PointerEventData e){if(View!=BochaCameraView.Caminhando)Dragged?.Invoke(e);}
    public void OnPointerUp(PointerEventData e){if(View!=BochaCameraView.Caminhando)Released?.Invoke(e);}
    public void OnPointerMove(PointerEventData e){if(View!=BochaCameraView.Caminhando)Hovered?.Invoke(e);}
    public void OnScroll(PointerEventData e)=>Scrolled?.Invoke(e.scrollDelta.y);
}

public sealed class BochaMapGraphic : MaskableGraphic
{
    public BochaSimulation Simulation { get; set; }
    internal int BochaOculta { get; set; } = -1;
    public Vector2 MapPoint(Vector2 world)
    {
        var r=rectTransform.rect;
        return new Vector2(r.x+5+(world.x+3)/6*(r.width-10),r.y+7+world.y/20*(r.height-14));
    }
    public void Refresh() => SetVerticesDirty();
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); var r = rectTransform.rect; var inner = new Rect(r.x+5,r.y+7,r.width-10,r.height-14);
        Quad(vh,r,new Color(.29f,.22f,.14f));Quad(vh,inner,new Color(.27f,.18f,.10f));
        float baseY = MapPoint(new Vector2(0, 1.1f)).y;
        Quad(vh,new Rect(inner.xMin,baseY,inner.width,1),new Color(.62f,.49f,.30f));
        if(Simulation==null)return;
        foreach(var b in Simulation.bolas)if(b.valida && b.id != BochaOculta)
        {
            Vector2 p=MapPoint(b.pos);
            bool balim=b.lado==BochaSide.Nenhum;
            float radius=balim?2.2f:3f;
            Circle(vh,p,radius+1,new Color(.025f,.025f,.02f));
            Circle(vh,p,radius,Color.Lerp(BochaCourtView.BallColor(b.lado),Color.white,balim?0:.16f));
            if(b.altura>.05f)Circle(vh,p,.8f,Color.white);
        }
    }
    private static void Quad(VertexHelper vh,Rect r,Color c)
    {int i=vh.currentVertCount;vh.AddVert(new Vector2(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector2(r.xMin,r.yMax),c,Vector2.zero);vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);}
    private static void Circle(VertexHelper vh,Vector2 p,float r,Color c)
    {int start=vh.currentVertCount;vh.AddVert(p,c,Vector2.zero);for(int i=0;i<=16;i++){float a=i*Mathf.PI/8;vh.AddVert(p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r,c,Vector2.zero);if(i>0)vh.AddTriangle(start,start+i,start+i+1);}}
}
