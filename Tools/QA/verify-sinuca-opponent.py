"""Execute the opponent's real route planner against the live bar layout, without scenes/saves."""
from pathlib import Path
import os, re, subprocess
root=Path(__file__).resolve().parents[2]
src=(root/'Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.Opponent.cs').read_text(encoding='utf-8-sig')
layout=(root/'Projeto Jogo 2D/Assets/Scripts/SantaRitaBarLayout.cs').read_text(encoding='utf-8-sig')
def block(source, token):
    a=source.index(token); b=source.index('{',a); depth=1; end=b+1
    while depth:
        depth+=(source[end]=='{')-(source[end]=='}'); end+=1
    return source[a:end]
methods='\n'.join(block(src,t) for t in ['    private bool OpponentWalkable(', '    private bool OpponentSegment(', '    public bool PrepareOpponentShot('])
program='''using System;using System.Collections.Generic;using UnityEngine;
static class SantaRitaBarLayout {
 public static readonly Vector2 TableCenter=new Vector2(9,6.5f);
'''+re.search(r'    public enum Kind[^\n]+',layout).group()+block(layout,'    public sealed class Prop')+block(layout,'    public static readonly Prop[] Props=')+''';}
class OpponentQA {
 const float MetresPerUnit=2.84f/420f;
 class Model {public Vector3 localPosition; public void SetActive(bool b){} public Model gameObject=>this;}
 class Actor {public Model root=new Model();}
 Actor opponent=new Actor(); Model opponentCue=new Model(); object[] opponentSprites={new object()};
 Vector2 opponentBall,opponentDirection; int opponentWaypoint;
 readonly List<Vector2> opponentPath=new List<Vector2>();
 void CancelOpponentShot(){opponentPath.Clear();opponentWaypoint=0;}
 static Vector3 Plan(Vector2 p){p=(p-SantaRitaBarLayout.TableCenter)*1.15f;return new Vector3(p.x,0,p.y);}
'''+methods+'''
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;}
 static int Main(){try{Run();return 0;}catch(Exception e){Console.Error.WriteLine("FAIL: "+e.Message);return 1;}}
 static void Run(){
 var q=new OpponentQA(); q.opponent.root.localPosition=Plan(new Vector2(9.6f,7.95f));
 Check(!q.PrepareOpponentShot(Vector2.zero,Vector2.zero),"zero direction rejected");
 for(int x=-190;x<=190;x+=95)for(int y=-90;y<=90;y+=45)for(int angle=0;angle<360;angle+=30){
 var ball=new Vector2(x,y);var dir=new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad));
 Check(q.PrepareOpponentShot(ball,dir),"route missing "+ball+" angle "+angle);
 var from=new Vector2(q.opponent.root.localPosition.x,q.opponent.root.localPosition.z);
 foreach(var p in q.opponentPath){Check(q.OpponentSegment(from,p),"route crosses table/furniture");from=p;}
 var end=q.opponentPath[q.opponentPath.Count-1];var delta=ball*MetresPerUnit-end;
 Check(Vector2.Dot(delta.normalized,dir)>.9999f,"stance must line up behind actual shot");
 Check(!q.OpponentWalkable(Vector2.zero),"table blocks path");
 q.opponent.root.localPosition=new Vector3(end.x,0,end.y);
 }
 q.CancelOpponentShot();Check(q.opponentPath.Count==0,"cancel clears route");
 Console.WriteLine("TOTAL: "+checks+" PASS; 300 consecutive shot approaches, all route segments and shot alignment. No native rendering or visual approval.");
 }
}
'''
out=Path('C:/Temp/SantaRitaSinucaOpponentQA');out.mkdir(exist_ok=True)
(out/'Program.cs').write_text(program,encoding='utf-8')
unity='C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
(out/'Opponent.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>0649;0414</NoWarn></PropertyGroup><ItemGroup><Reference Include="UnityEngine.CoreModule"><HintPath>{unity}</HintPath></Reference></ItemGroup></Project>')
env=dict(os.environ,DOTNET_CLI_HOME='C:/Temp/dotnet-home',DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Opponent.csproj'),'--source',str(out),'-v:q'],env=env,check=True)
subprocess.run(['dotnet','run','--project',str(out/'Opponent.csproj'),'--no-restore','-v:q'],env=env,check=True)
