using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class SinucaRoom3D
{
    private readonly List<Light> blackoutLights = new List<Light>();
    private readonly Plane[] opponentFrustum = new Plane[6];
    private Sprite zeAssombradoSprite;
    private bool zeAssombrado;
    private Vector2 startleLook;
    public float GripShake { get; set; }

    public void SetStartleLook(Vector2 degrees)
    {
        if (startleLook == degrees) return;
        startleLook = degrees;
        UpdateCamera();
        SyncFirstPersonCamera();
    }

    public void SetBlackout(bool active)
    {
        if (active)
        {
            if (blackoutLights.Count > 0) return;
            foreach (var light in root.GetComponentsInChildren<Light>())
            {
                if (!light.enabled) continue;
                blackoutLights.Add(light);
                light.enabled = false;
            }
        }
        else
        {
            foreach (var light in blackoutLights)
                if (light != null) light.enabled = true;
            blackoutLights.Clear();
        }
    }

    private void BuildHauntedOpponent()
    {
        var texture = Resources.Load<Texture2D>("Art/Characters/Npc_Ze_Sinuca_Assombrado");
        if (texture == null) return;
        zeAssombradoSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(.5f, .12f), texture.width / 2f, 0, SpriteMeshType.FullRect);
        zeAssombradoSprite.name = "ZeSinuca_Assombrado";
        owned.Add(zeAssombradoSprite);
        NativeMesh(zeAssombradoSprite);
        PixelMaterial(zeAssombradoSprite);
    }

    public bool SetHauntedOpponent(bool active)
    {
        if (opponent == null || zeAssombradoSprite == null) return false;
        if (active != zeAssombrado)
        {
            zeAssombrado = active;
            UpdateOpponentView();
        }
        return true;
    }

    public bool OpponentInFrame
    {
        get
        {
            if (opponent == null) return false;
            GeometryUtility.CalculateFrustumPlanes(camera, opponentFrustum);
            return GeometryUtility.TestPlanesAABB(opponentFrustum, opponent.renderer.bounds);
        }
    }

    public bool HauntedOpponentInFocus
    {
        get
        {
            if (!zeAssombrado) return false;
            var upperBody = opponent.root.position + Vector3.up * 1.13f;
            var point = camera.WorldToViewportPoint(upperBody);
            return point.z > 0f && Mathf.Abs(point.x - .5f) < .19f && Mathf.Abs(point.y - .5f) < .29f;
        }
    }


    private Transform falseBall,underMemory,wetMarks,underBoards;
    private readonly Transform[] underDrops=new Transform[3];
    private Light inspectionLight;
    private bool inspectUnder;
    private Vector3 inspectionPosition;
    private float inspectionYaw,inspectionPitch;
    public bool InspectingUnderTable => inspectUnder;
    public bool FalseBallVisible => falseBall!=null && falseBall.gameObject.activeSelf;
    public bool CanInspectUnderTable => Walking && Mathf.Abs(StandingXZ.x)<2.05f && Mathf.Abs(StandingXZ.y)<1.75f;
    public bool LookingAtFalseBall
    {
        get
        {
            if(!FalseBallVisible)return false;
            var delta=falseBall.localPosition-camera.transform.localPosition;
            return delta.magnitude<3.4f && Vector3.Dot(camera.transform.forward,delta.normalized)>.996f;
        }
    }
    public void SetFalseBall(bool visible,int number,Vector2 at)
    {
        if(!visible){if(falseBall!=null)falseBall.gameObject.SetActive(false);return;}
        if(number<1 || number>15 || number==8)return;
        if(falseBall==null)
        {
            falseBall=Sphere("BolaPercebida_SemCorpo",Vector3.zero,SinucaTableView.BallRadius*2*MetresPerUnit,BallMaterial(number),dynamic).transform;
            falseBall.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        }
        // Exact same texture, geometry, scale, lighting and number as the genuine ball.
        falseBall.GetComponent<Renderer>().sharedMaterial=balls[number].GetComponent<Renderer>().sharedMaterial;
        falseBall.localRotation=balls[number].localRotation;
        falseBall.localPosition=new Vector3(at.x*MetresPerUnit,TableY+SinucaTableView.BallRadius*MetresPerUnit,at.y*MetresPerUnit);
        falseBall.gameObject.SetActive(!inspectUnder);
    }
    public void BeginUnderTable()
    {
        if(inspectUnder)return;
        BuildUnderTableMemory();inspectUnder=true;
        float side=standingPosition.z>=0?1:-1;
        inspectionPosition=new Vector3(Mathf.Clamp(standingPosition.x,-.82f,.82f),.32f,side*1.10f);
        var direction=new Vector3(0,.58f,0)-inspectionPosition;
        var rotation=Quaternion.LookRotation(direction).eulerAngles;
        inspectionYaw=rotation.y;inspectionPitch=Mathf.DeltaAngle(0,rotation.x);
        underBoards.gameObject.SetActive(true);inspectionLight.gameObject.SetActive(true);
        transitionPosition=camera.transform.localPosition;transitionRotation=camera.transform.localRotation;transitionStarted=Time.unscaledTime;
        if(falseBall!=null)falseBall.gameObject.SetActive(false);
        UpdateCamera();SyncFirstPersonCamera();
    }
    public void LookUnderTable(Vector2 delta)
    {
        inspectionYaw+=delta.x*.10f;
        inspectionPitch=Mathf.Clamp(inspectionPitch-delta.y*.09f,-48,22);
    }
    public void SetUnderTableMemory(bool visible,float seconds,bool reduced)
    {
        if(underMemory==null)return;
        underMemory.gameObject.SetActive(visible && inspectUnder);
        if(!visible)return;
        // Three small, staggered droplets. The existing wooden floor stays fully visible.
        for(int i=0;i<underDrops.Length;i++)
        {
            float age=Mathf.Repeat(seconds-i*.43f,1.7f);
            underDrops[i].gameObject.SetActive(age<.34f);
            underDrops[i].localPosition=new Vector3((i-1)*.19f,.625f-4.905f*age*age,i==1?.08f:-.04f);
        }
        wetMarks.gameObject.SetActive(seconds>=3);
    }
    public void EndUnderTable()
    {
        if(!inspectUnder)return;
        inspectUnder=false;
        if(underMemory!=null)underMemory.gameObject.SetActive(false);
        if(underBoards!=null)underBoards.gameObject.SetActive(false);
        if(inspectionLight!=null)inspectionLight.gameObject.SetActive(false);
        transitionPosition=camera.transform.localPosition;transitionRotation=camera.transform.localRotation;transitionStarted=Time.unscaledTime;
        UpdateCamera();SyncFirstPersonCamera();
    }
    private void AplicarCameraInvestigacao()
    {
        if(!inspectUnder)return;
        camera.orthographic=false;camera.fieldOfView=66;
        var position=inspectionPosition;var rotation=Quaternion.Euler(inspectionPitch,inspectionYaw,0);
        float blend=Mathf.SmoothStep(0,1,(Time.unscaledTime-transitionStarted)/.42f);
        camera.transform.localPosition=Vector3.Lerp(transitionPosition,position,blend);
        camera.transform.localRotation=Quaternion.Slerp(transitionRotation,rotation,blend);
    }
    private void BuildUnderTableMemory()
    {
        if(underMemory!=null)return;
        underBoards=new GameObject("ForroInferiorDaMesa").transform;underBoards.SetParent(dynamic,false);
        // The existing tabletop is a top-only mesh: give the crouching view a real underside.
        for(int i=0;i<9;i++)Box("TabuaInferior",new Vector3((i-4)*.30f,.65f,0),new Vector3(.294f,.018f,1.36f),DarkWood,underBoards);
        for(int i=-1;i<=1;i+=2)Box("TravessaInferior",new Vector3(i*.88f,.60f,0),new Vector3(.075f,.09f,1.28f),Wood,underBoards);
        underMemory=new GameObject("MemoriaCisterna_Transitoria").transform;underMemory.SetParent(dynamic,false);
        var dropMat=Mat("gotasDiscretas",new Color(.40f,.48f,.46f));dropMat.SetFloat("_Smoothness",.65f);
        for(int i=0;i<underDrops.Length;i++)
        {
            underDrops[i]=Sphere("GotaSobMesa_"+i,Vector3.zero,1f,dropMat,underMemory).transform;
            underDrops[i].localScale=new Vector3(.007f,.012f,.007f);
            underDrops[i].GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        }
        wetMarks=new GameObject("MarcasDeDedosNaMadeira").transform;wetMarks.SetParent(underMemory,false);
        var stain=Mat("madeiraMolhadaMemoria",new Color(.12f,.095f,.063f));
        for(int hand=0;hand<2;hand++)
        {
            float x=hand==0?-.24f:.21f;
            var palm=Sphere("MarcaPalma",new Vector3(x,.638f,.05f),.10f,stain,wetMarks);
            palm.transform.localScale=new Vector3(.10f,.003f,.115f);
            for(int finger=0;finger<4;finger++)
            {
                float fx=x+(finger-1.5f)*.024f;
                Tube("RastroDedo",new Vector3(fx,.637f,.08f),new Vector3(fx+.008f,.637f,.22f-finger*.012f),.008f,stain,wetMarks);
                Tube("Escorrido",new Vector3(fx,.636f,.02f),new Vector3(fx-.008f,.636f,-.17f-finger*.017f),.0025f,stain,wetMarks);
            }
        }
        inspectionLight=new GameObject("LuzRebatidaSobMesa").AddComponent<Light>();inspectionLight.transform.SetParent(dynamic,false);
        inspectionLight.transform.localPosition=new Vector3(0,.30f,0);inspectionLight.type=LightType.Point;
        inspectionLight.color=new Color(.68f,.75f,.66f);inspectionLight.intensity=.65f;inspectionLight.range=2.5f;
        inspectionLight.cullingMask=1<<Layer;inspectionLight.shadows=LightShadows.None;
        underMemory.gameObject.SetActive(false);underBoards.gameObject.SetActive(false);inspectionLight.gameObject.SetActive(false);
    }
}
