"""Execute the real walking/reach/room-clearance code without native rendering or input.
No scene or save is accessed. Furniture definitions are extracted from the source layout.
"""
from pathlib import Path
import os,re,subprocess
root=Path(__file__).resolve().parents[2]
room=(root/'Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.cs').read_text(encoding='utf-8-sig')
layout=(root/'Projeto Jogo 2D/Assets/Scripts/SantaRitaBarLayout.cs').read_text(encoding='utf-8-sig')
def block(source,token):
 start=source.index(token);a=source.index('{',start);depth=1;b=a+1
 while depth:
  depth+=(source[b]=='{')-(source[b]=='}');b+=1
 return source[start:b]
methods='\n'.join(block(room,token) for token in ['    public static float BridgeRailLift(', '    public float ApproachAngle(', '    private bool Walkable(', '    public void MovePlayer(', '    public static Vector3 Plan('])
# Expression-bodied methods have no brace, so extract their actual source expressions.
reach=re.search(r'    public static bool InsideTableFootprint\(Vector2 p\)\s*=>[^;]+;',room).group()+'\n'+re.search(r'    public bool CanReachWhite\(Vector2 ball\)\s*=>[^;]+;',room).group()
prop=block(layout,'    public sealed class Prop')
props=block(layout,'    public static readonly Prop[] Props=')+';'
kind=re.search(r'    public enum Kind[^\n]+',layout).group()
program='''using System;using System.Collections.Generic;using UnityEngine;
static class Application {public static bool isFocused=true;}
static class MenuPausa {public static bool Aberto;}
static class SantaRitaSound {public static int events; public static void Play(string id,float v,float pitch,float interval){events++;if(v>.07f)throw new Exception("Footstep too loud");}}
static class SantaRitaBarLayout {
 public static readonly Vector2 TableCenter=new Vector2(9,6.5f);
'''+kind+prop+props+'''}
class WalkingQA {
 const float MetresPerUnit=2.84f/420f;
 Vector3 standingPosition=new Vector3(-2.1f,1.62f,-1.25f);
 float standingYaw=58f,standingPitch=22f;
 float distanciaPasso;int variantePasso;
 class Actor { public Transform root; }
 Actor opponent;
 bool Walking=true;
 Vector2 StandingXZ=>new Vector2(standingPosition.x,standingPosition.z);
'''+methods+reach+'''
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception("FAIL: "+label);checks++;Console.WriteLine("PASS: "+label);}
 static WalkingQA At(float x,float z,float yaw=0)=>new WalkingQA{standingPosition=new Vector3(x,1.62f,z),standingYaw=yaw};
 public static int Main(){try{Run();return 0;}catch(Exception e){Console.Error.WriteLine(e.Message);return 1;}}
 static void Run(){
 Check(BridgeRailLift(Vector2.zero,Vector2.up)==0,"centre bridge rests on cloth");
 foreach(var b in new[]{new Vector2(200,0),new Vector2(-200,0),new Vector2(0,100),new Vector2(0,-100)})Check(BridgeRailLift(b,-b.normalized)>0,"bridge and rear cue lift at rail "+b);
 var k=new WalkingQA();Check(k.Walkable(k.StandingXZ),"entry point has clearance from all furniture");
 var foot=At(0,-2);int before=SantaRitaSound.events;
 for(int i=0;i<30;i++)foot.MovePlayer(Vector2.zero,new Vector2(1,0),1f/60);
 Check(SantaRitaSound.events==before,"looking without walking has no footsteps");
 foot=At(0,-2);for(int i=0;i<25;i++)foot.MovePlayer(Vector2.up,Vector2.zero,1f/60);
 Check(SantaRitaSound.events==before+1,"one quiet wood step per real stride");
 foot=At(0,-1.065f);before=SantaRitaSound.events;for(int i=0;i<100;i++)foot.MovePlayer(Vector2.up,Vector2.zero,1f/60);
 Check(SantaRitaSound.events==before,"walking against table is silent");
 foot=At(0,-2);Application.isFocused=false;for(int i=0;i<25;i++)foot.MovePlayer(Vector2.up,Vector2.zero,1f/60);
 Check(SantaRitaSound.events==before,"unfocused room does not emit footsteps");Application.isFocused=true;
 Check(!k.Walkable(Vector2.zero),"table blocks walking");
 foreach(var p in new[]{new Vector2(6,0),new Vector2(-6,0),new Vector2(0,4),new Vector2(0,-8)})Check(!k.Walkable(p),"room wall blocks "+p);
 var m=At(0,-2);for(int i=0;i<120;i++)m.MovePlayer(Vector2.up,Vector2.zero,1f/60);
 Check(m.standingPosition.z<=-1.06f && m.standingPosition.z>-1.1f,"walking stops at near cushion clearance");
 m=At(1.9f,0,-90);for(int i=0;i<120;i++)m.MovePlayer(Vector2.up,Vector2.zero,1f/60);
 Check(m.standingPosition.x>=1.78f && m.standingPosition.x<1.82f,"walking stops at end of table");
 m=At(0,-2);m.MovePlayer(new Vector2(1,1),Vector2.zero,1f/60);
 Check(Math.Abs(Vector2.Distance(m.StandingXZ,new Vector2(0,-2))-1.65f/60)<.00001,"diagonal walking has same speed");
 m=At(0,-2);m.MovePlayer(Vector2.up,Vector2.zero,5);
 Check(m.standingPosition.z>-1.92,"frame stall cannot tunnel across furniture");
 m=At(0,-2);m.Walking=false;m.MovePlayer(Vector2.up,new Vector2(100,100),1f/60);
 Check(m.standingPosition.z==-2 && m.standingYaw==0,"stance blocks walking and room look");
 m=At(0,-2);m.MovePlayer(Vector2.zero,new Vector2(0,100000),1f/60);Check(m.standingPitch==-42,"look clamps upward");
 m.MovePlayer(Vector2.zero,new Vector2(0,-100000),1f/60);Check(m.standingPitch==68,"look clamps downward");
 m=At(0,-1.1f);Check(m.CanReachWhite(Vector2.zero),"white in table centre can be reached from rail");
 Check(!m.CanReachWhite(new Vector2(200,100)),"distant white requires walking around table");
 Check(Math.Abs(m.ApproachAngle(Vector2.zero)-Mathf.PI*.5f)<.00001,"shot direction follows player's side of approach");
 // Flood-fill real clearance, then test that each sampled white position has a reachable stance.
 const float cell=.15f;const int w=69,h=72;var seen=new bool[w,h];var queue=new Queue<(int x,int y)>();
 Func<int,int,Vector2> pos=(x,y)=>new Vector2(-5.1f+x*cell,-7.15f+y*cell);
 int sx=(int)Math.Round((k.standingPosition.x+5.1)/cell),sy=(int)Math.Round((k.standingPosition.z+7.15)/cell);
 seen[sx,sy]=true;queue.Enqueue((sx,sy));var walk=new List<Vector2>();
 while(queue.Count>0){var at=queue.Dequeue();walk.Add(pos(at.x,at.y));foreach(var d in new[]{(1,0),(-1,0),(0,1),(0,-1)}){int x=at.x+d.Item1,y=at.y+d.Item2;if(x<0||y<0||x>=w||y>=h||seen[x,y]||!k.Walkable(pos(x,y)))continue;seen[x,y]=true;queue.Enqueue((x,y));}}
 Check(walk.Count>2000,"room circulation remains connected beyond the table");
 foreach(var corner in new[]{new Vector2(-2,-1.3f),new Vector2(2,-1.3f),new Vector2(-2,1.3f),new Vector2(2,1.3f)})Check(walk.Exists(p=>Vector2.Distance(p,corner)<.25f),"both ends and rails can be reached: "+corner);
 for(int x=-200;x<=200;x+=50)for(int y=-90;y<=90;y+=45){var ball=new Vector2(x,y)*MetresPerUnit;Check(walk.Exists(p=>Vector2.Distance(p,ball)<=1.9f),"reachable stance for white "+x+","+y);}
 Console.WriteLine("TOTAL: "+checks+" PASS. Numerical room checks; no native input, camera rendering or visual hand alignment validation.");
 }
}
'''
out=Path(os.environ.get('TEMP','C:/Temp'))/'SantaRitaSinucaWalkingQA';out.mkdir(exist_ok=True)
(out/'Program.cs').write_text(program,encoding='utf-8')
unity='C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
(out/'Walking.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Reference Include="UnityEngine.CoreModule"><HintPath>{unity}</HintPath></Reference></ItemGroup></Project>')
env=dict(os.environ,DOTNET_CLI_HOME=str(out/'dotnet-home'),DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Walking.csproj'),'--source',str(out),'-v:q'],env=env,check=True)
subprocess.run(['dotnet','run','--project',str(out/'Walking.csproj'),'--no-restore','-v:q'],env=env,check=True)



