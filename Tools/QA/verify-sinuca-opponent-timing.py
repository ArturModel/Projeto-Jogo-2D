"""Run the real opponent coroutine with controlled frames; rendering is stubbed."""
from pathlib import Path
import os, subprocess
root=Path(__file__).resolve().parents[2]
src=(root/'Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs').read_text(encoding='utf-8-sig')
start=src.index('    private IEnumerator ExecutarTurnoZeCoroutine()')
end=src.index('    private Vector2 EscolherPosicaoBrancaZe()',start)
program='''using System;using System.Collections;using UnityEngine;
static class Time {public static float deltaTime=.01666667f,timeScale=1;}
static class Application {public static bool isFocused=true;}
static class MenuPausa {public static bool Aberto;}
class WaitForSeconds {public WaitForSeconds(float seconds){}}
static class Debug {public static void LogWarning(string text){}}
enum SinucaCueSpin {Normal}
class Panel {public bool activeSelf;}
class Ball {public Vector2 pos;}
class Room {public int walks,poses,cancels;public bool ready=true;public float progress;
 public bool PrepareOpponentShot(Vector2 b,Vector2 d){return ready;}
 public bool OpponentAtShot=>walks>=3;
 public void TickOpponentWalk(float dt){walks++;}
 public void PoseOpponentShot(float p){poses++;progress=p;}
 public void CancelOpponentShot(){cancels++;}}
class View {public Room Room=new Room();}
class TimingQA {
 enum EstadoPartida {TurnoZePensando,Simulando,Inativo}
 EstadoPartida estado=EstadoPartida.TurnoZePensando;
 bool IsShowing=true,brancaNaMao; Panel painelModal,painelOpcoes,painelAlvo;
 Ball[] bolas={new Ball()}; View tableView3D=new View(); int shots;float contact;
 Vector2 EscolherPosicaoBrancaZe()=>Vector2.zero;
 void SincronizarTransformBolas(){} void AtualizarVisualTacoEMira(){}
 void EscolherTacadaZe(out Vector2 d,out float p){d=Vector2.right;p=300;}
 void ExecutarTacadaComEfeito(Vector2 d,float p,SinucaCueSpin spin){shots++;contact=tableView3D.Room.progress;estado=EstadoPartida.Simulando;}
'''+src[start:end]+'''
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS: "+label);}
 static void Frames(IEnumerator c,int n){for(int i=0;i<n;i++)c.MoveNext();}
 static int Main(){try{Run();return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Run(){
 var q=new TimingQA();var c=q.ExecutarTurnoZeCoroutine();c.MoveNext();
 MenuPausa.Aberto=true;Frames(c,20);Check(q.tableView3D.Room.walks==0&&q.shots==0,"pause before approach blocks walking/shot");
 MenuPausa.Aberto=false;Frames(c,2);int w=q.tableView3D.Room.walks;
 Application.isFocused=false;Frames(c,20);Check(q.tableView3D.Room.walks==w,"focus loss freezes approach");
 Application.isFocused=true;Frames(c,4);int poses=q.tableView3D.Room.poses;
 q.painelModal=new Panel{activeSelf=true};Frames(c,20);Check(q.tableView3D.Room.poses==poses,"modal freezes shot pose");q.painelModal.activeSelf=false;
 Frames(c,20);Check(q.shots==0,"white stays still during preparation");
 q.painelOpcoes=new Panel{activeSelf=true};poses=q.tableView3D.Room.poses;Frames(c,20);Check(q.tableView3D.Room.poses==poses,"options freeze shot pose");q.painelOpcoes.activeSelf=false;
 Frames(c,180);Check(q.shots==1,"one physical shot per animation");
 Check(q.contact>=.82f&&q.contact<.84f,"physical impulse matches contact frame");Check(q.tableView3D.Room.cancels==1,"recovery clears presentation");
 q=new TimingQA();c=q.ExecutarTurnoZeCoroutine();Frames(c,2);q.estado=EstadoPartida.Inativo;Frames(c,200);Check(q.shots==0,"cancelled turn cannot shoot later");
 q=new TimingQA();q.tableView3D.Room.ready=false;c=q.ExecutarTurnoZeCoroutine();Frames(c,200);Check(q.shots==1,"missing presentation cannot strand rules on AI turn");
 Console.WriteLine("TOTAL: "+checks+" PASS; actual coroutine, controlled frame clock; no native Unity rendering.");
 }
}
'''
out=Path('C:/Temp/SantaRitaSinucaTimingQA');out.mkdir(exist_ok=True)
(out/'Program.cs').write_text(program,encoding='utf-8')
unity='C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
(out/'Timing.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>0649;0414</NoWarn></PropertyGroup><ItemGroup><Reference Include="UnityEngine.CoreModule"><HintPath>{unity}</HintPath></Reference></ItemGroup></Project>')
env=dict(os.environ,DOTNET_CLI_HOME='C:/Temp/dotnet-home',DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Timing.csproj'),'--source',str(out),'-v:q'],env=env,check=True)
subprocess.run(['dotnet','run','--project',str(out/'Timing.csproj'),'--no-restore','-v:q'],env=env,check=True)
