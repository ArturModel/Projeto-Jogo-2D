using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class SinucaSpinSelector : MaskableGraphic, IPointerDownHandler, IDragHandler
{
    public Action<Vector2> Changed;
    public Func<bool> CanSelect;
    public void SetContact(Vector2 value){Contact=Vector2.ClampMagnitude(value,.75f);SetVerticesDirty();}
    public Vector2 Contact { get; private set; }
    public void OnPointerDown(PointerEventData e){Select(e);}
    public void OnDrag(PointerEventData e){Select(e);}
    private void Select(PointerEventData e)
    {
        if(CanSelect!=null&&!CanSelect())return;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,e.position,e.pressEventCamera,out var p))return;
        Contact=Vector2.ClampMagnitude(p/(Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.5f),.75f);
        Changed?.Invoke(Contact);SetVerticesDirty();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();float radius=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.5f;
        Disc(vh,Vector2.zero,radius,new Color(.84f,.84f,.76f));
        Disc(vh,Vector2.zero,radius*.94f,new Color(.96f,.95f,.88f));
        Disc(vh,Contact*radius,2.8f,new Color(.75f,.20f,.12f));
    }
    private void Disc(VertexHelper vh,Vector2 c,float r,Color color)
    {
        int start=vh.currentVertCount;vh.AddVert(c,color,Vector2.zero);
        for(int i=0;i<=32;i++)vh.AddVert(c+new Vector2(Mathf.Cos(i*Mathf.PI/16),Mathf.Sin(i*Mathf.PI/16))*r,color,Vector2.zero);
        for(int i=1;i<=32;i++)vh.AddTriangle(start,start+i,start+i+1);
    }
}
