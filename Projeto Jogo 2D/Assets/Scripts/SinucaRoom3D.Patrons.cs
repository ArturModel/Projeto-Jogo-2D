using System.Collections.Generic;
using UnityEngine;

public sealed partial class SinucaRoom3D
{
    private sealed class PatronVisual
    {
        public Sprite[] frames;
        public Vector2 heading;
        public int facing;
    }
    private readonly Dictionary<Actor,PatronVisual> salonPatrons=new Dictionary<Actor,PatronVisual>();
    private void BuildSalonPatrons()
    {
        var sheet=Resources.Load<Texture2D>("Art/Characters/FrequentadoresSinuca");if(sheet==null)return;
        string[] keys={"Npc_Frequentador","Npc_Chico","Npc_Ramiro","Npc_Motorista"};
        for(int row=0;row<keys.Length;row++)
        {
            var actor=actors.Find(a=>a.definition.key==keys[row]);if(actor==null)continue;
            var visual=new PatronVisual{frames=new Sprite[8],heading=row==0||row==2?Vector2.right:Vector2.left};
            for(int pose=0;pose<8;pose++)
            {
                var sprite=Sprite.Create(sheet,new Rect(pose*128,(3-row)*128,128,128),new Vector2(.5f,13f/128f),80,0,SpriteMeshType.FullRect);
                sprite.name=keys[row]+"_Salao_"+pose;owned.Add(sprite);visual.frames[pose]=sprite;NativeMesh(sprite);PixelMaterial(sprite);
            }
            salonPatrons.Add(actor,visual);actor.rest=visual.frames[0];PixelSources[actor.definition.id]=actor.rest;
            actor.filter.sharedMesh=NativeMesh(actor.rest);actor.renderer.sharedMaterial=PixelMaterial(actor.rest);
        }
    }
    private bool UpdateSalonPatron(Actor actor)
    {
        if(!salonPatrons.TryGetValue(actor,out var visual))return false;
        var delta=camera.transform.localPosition-actor.root.localPosition;
        var viewer=new Vector2(delta.x,delta.z).normalized;
        float alignment=Vector2.Dot(visual.heading,viewer);
        float front=visual.facing==0?.48f:.60f,back=visual.facing==2?-.48f:-.60f;
        visual.facing=alignment>front?0:alignment<back?2:1;
        bool mirror=visual.facing==1&&Vector2.Dot(visual.heading,new Vector2(-viewer.y,viewer.x))<0;
        var scale=actor.root.localScale;scale.x=Mathf.Abs(scale.x)*(mirror?-1:1);actor.root.localScale=scale;
        int routine=SantaRitaBarLayout.PatronFrame(actor.definition.id,Time.time);
        // Original routine timing is retained; independent drawings show the action.
        bool sip=routine>=1&&routine<=5;
        bool talk=routine>=6;
        int frame=visual.facing==2?7:visual.facing==1?(sip?6:talk?5:4):sip?3:talk?((int)(Time.time*3.5f)%2==0?1:2):0;
        actor.filter.sharedMesh=NativeMesh(visual.frames[frame]);actor.renderer.sharedMaterial=PixelMaterial(visual.frames[frame]);
        return true;
    }
    private bool SalonPropVisible(string id)
    {
        // The new mate drawings hold the gourd; the thermos stays on its real table.
        if(salonPatrons.Count>0&&id=="CuiaMesa")return false;
        if(salonPatrons.Count>0&&id=="TermicaMesa")return true;
        if(salonPatrons.Count>0&&id=="CopoJanela")return false;
        if(salonPatrons.Count>0&&id=="CopoConversa")
        {int frame=SantaRitaBarLayout.PatronFrame("NPC_Frequentador_Interior",Time.time);return frame==0||frame>=6;}
        return SantaRitaBarLayout.TablePropVisible(id,Time.time);
    }
}
