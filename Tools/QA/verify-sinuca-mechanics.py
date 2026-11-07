"""Execute the current C# numerical kernel under .NET, without Unity native state.

Only presentation, event audio dispatch, and end-of-turn adjudication are stubbed.
Quaternion rendering is skipped; position, velocity, spin, pockets and jaws are real source.
Requires the installed Unity managed math DLL and .NET SDK. No save files are accessed.
"""
from pathlib import Path
import os
import re
import subprocess
import sys

root = Path(__file__).resolve().parents[2]
source = (root / 'Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs').read_text(encoding='utf-8-sig')
out = Path(os.environ.get('TEMP', 'C:/Temp')) / 'SantaRitaSinucaMechanicsQA'
out.mkdir(parents=True, exist_ok=True)
unity = Path(sys.argv[1] if len(sys.argv) > 1 else 'C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll')

def method(name):
    match = re.search(r'^    (?:private|public) (?:static )?[^\n]+\b' + name + r'\([^\n]*\)\s*\{', source, re.M)
    if not match:
        raise RuntimeError('Missing C# method: ' + name)
    start = source.index('{', match.start())
    level = 1
    end = start + 1
    while level:
        level += (source[end] == '{') - (source[end] == '}')
        end += 1
    return source[match.start():end]

constants = '\n'.join(re.findall(r'^    (?:public|private) const float [^\n]+', source, re.M))
methods = '\n'.join(method(n) for n in ['InicializarCacapas', 'IntegrarPano', 'RebaterTabela', 'CruzaCapturaCacapa', 'SimularFisicaMesa', 'PertoDeCacapa', 'RegistrarTabela', 'RegistrarQuedaNaCacapa', 'CalcularForcaPuxada', 'SuavizarMira', 'OscilarCarga', 'GarantirClipsAudio'])
methods = methods.replace('b.rotacao = Quaternion.AngleAxis(anguloGraus, eixoRolagem) * b.rotacao;', '// presentation-only quaternion update excluded')
methods = methods.replace('Application.isPlaying', 'false').replace('AudioClip', 'SampleClip')
prefix = '''using System;
using System.Collections.Generic;
using UnityEngine;
class SampleClip {
 public float[] data; public int hz;
 public static SampleClip Create(string n,int count,int channels,int hz,bool stream) => new SampleClip{hz=hz};
 public void SetData(float[] d,int offset){data=d;}
}
class Kernel {
 class BolaMesa { public int numero; public Vector2 pos,vel,cacapaAlvo,rolagem; public bool ativa=true,caiuNestaTacada,caindoNaCacapa; public float progressoQueda,spinLongitudinal,spinLateral; }
 struct BicoCacapa { public Vector2 pos; public float raio; }
 List<BolaMesa> bolas=new List<BolaMesa>();
 Vector2[] cacapas=new Vector2[6]; List<BicoCacapa> bicosCacapas=new List<BicoCacapa>();
 bool houvePrimeiroContatoBolaObjeto,houveToqueTabelaOuCacapa,brancaCaiuNestaTacada;
 int primeiroContatoNumero=-1,settled; List<int> bolasObjetoCaidasNaTacada=new List<int>();
 static SampleClip clipTacada,clipTabela,clipCacapa,clipGiz;
 void SincronizarTransformBolas(){} void AtualizarVisualTacoEMira(){} void ResolverFimDaTacada(){settled++;}
 void TocarSomTabela(float v){} void TocarSomColisaoBola(float v){} void TocarSomCacapa(){}
 static int checks;
 static void Check(bool ok,string msg){if(!ok)throw new Exception("FAIL: "+msg);checks++;Console.WriteLine("PASS: "+msg);}
 static Kernel Make(params (Vector2 p,Vector2 v)[] balls){var k=new Kernel();k.InicializarCacapas();foreach(var b in balls)k.bolas.Add(new BolaMesa{numero=k.bolas.Count,pos=b.p,vel=b.v});return k;}
 float Energy(){float e=0;foreach(var b in bolas)e+=b.vel.sqrMagnitude+.4f*(b.rolagem.sqrMagnitude+b.spinLateral*b.spinLateral);return e;}
 public static void Main(){
  var back=Vector2.left;
  float tiny=SuavizarMira(0,.00001f,1f/60);
  Check(tiny>0&&tiny<.00001f,"subpixel aim adjustment survives smoothing");
  Check(SuavizarMira(0,1,0)==0,"zero elapsed time preserves aim");
  Check(SuavizarMira(0,1,1f/60)>0&&SuavizarMira(0,1,1f/60)<1,"aim approaches target without snapping");
  float thirty=0,fast=0;for(int i=0;i<3;i++)thirty=SuavizarMira(thirty,1,1f/30);for(int i=0;i<24;i++)fast=SuavizarMira(fast,1,1f/240);
  Check(Math.Abs(thirty-fast)<.00001,"aim smoothing is independent of frame rate");
  float wrap=SuavizarMira(359*Mathf.Deg2Rad,1*Mathf.Deg2Rad,1f/60)*Mathf.Rad2Deg;
  Check(wrap>359&&wrap<361,"aim crosses angle seam along shortest arc");
  Check(CalcularForcaPuxada(Vector2.zero,back)==0,"click without pull has zero power");
  Check(CalcularForcaPuxada(new Vector2(-2,0),back)==0,"dead zone rejects small tremor");
  Check(CalcularForcaPuxada(new Vector2(100,0),back)==0,"forward push has zero power");
  Check(CalcularForcaPuxada(new Vector2(0,200),back)==0,"lateral movement has zero power");
  float half=CalcularForcaPuxada(new Vector2(-51.5f,0),back);
  Check(Math.Abs(half-.5f)<.0001,"half pull is predictable");
  Check(CalcularForcaPuxada(new Vector2(-51.5f,200),back)==half,"lateral jitter preserves power");
  Check(CalcularForcaPuxada(new Vector2(-500,0),back)==1,"maximum pull is capped");
  Check(CalcularForcaPuxada(new Vector2(-20,0),back)<half,"returning pointer reduces power");
  Check(Math.Abs(CalcularForcaPuxada(Vector2.down*51.5f,Vector2.down)-half)<.0001,"rotated view axis uses same power");
  bool up=false;float power=0;for(int i=0;i<600;i++)power=OscilarCarga(power,1f/60,ref up);
  Check(power==1,"keyboard charge stays at maximum without oscillating");
  var slow=Make((new Vector2(-100,30),new Vector2(MinShotPower,0)));
  for(int i=0;i<120;i++)slow.SimularFisicaMesa(FixedStep);
  Check(slow.settled>0&&slow.bolas[0].pos.x>-100&&slow.bolas[0].pos.x<-95,"soft shot travels a few units and stops");
  var coast=Make((new Vector2(-150,30),new Vector2(100,0)));
  coast.bolas[0].rolagem=coast.bolas[0].vel;
  for(int i=0;i<60;i++)coast.SimularFisicaMesa(FixedStep);
  Check(Math.Abs(coast.bolas[0].vel.x-(100-BallFriction*.5f))<.03,"rolling loses speed gradually");
  var hit=Make((new Vector2(0,30),new Vector2(100,0)),(new Vector2(BallRadius*2+.1f,30),Vector2.zero));
  float before=hit.Energy();hit.SimularFisicaMesa(FixedStep);
  Check(hit.bolas[1].vel.x>90&&hit.bolas[0].vel.x<7,"head-on collision transfers impulse to target");
  Check(hit.Energy()<before,"collision dissipates energy");
  Check(hit.primeiroContatoNumero==1,"first object contact is recorded");
  Vector2 sliding=new Vector2(100,0),rotation=Vector2.zero;
  IntegrarPano(ref sliding,ref rotation,.1f);
  Check(sliding.x<100 && rotation.x>0 && rotation.x<=sliding.x+.001f,"cloth develops forward roll without reversing slip");
  for(int i=0;i<100;i++)IntegrarPano(ref sliding,ref rotation,.01f);
  Check(Vector2.Distance(sliding,rotation)<.001f,"sliding converges to pure rolling");
  var draw=Make((new Vector2(0,30),new Vector2(100,0)),(new Vector2(BallRadius*2+.1f,30),Vector2.zero));
  var follow=Make((new Vector2(0,30),new Vector2(100,0)),(new Vector2(BallRadius*2+.1f,30),Vector2.zero));
  draw.bolas[0].rolagem=new Vector2(-80,0);follow.bolas[0].rolagem=new Vector2(120,0);
  float drawEnergy=draw.Energy(),followEnergy=follow.Energy();
  for(int i=0;i<30;i++){draw.SimularFisicaMesa(FixedStep);follow.SimularFisicaMesa(FixedStep);}
  Check(draw.bolas[0].vel.x<-5,"draw comes from retained reverse spin and cloth contact");
  Check(follow.bolas[0].vel.x>5,"follow develops after impact through cloth contact");
  Check(draw.Energy()<drawEnergy && follow.Energy()<followEnergy,"follow and draw do not create kinetic energy");
  var cut=Make((new Vector2(0,30),new Vector2(100,40)),(new Vector2(BallRadius*2+.1f,30),Vector2.zero));
  cut.SimularFisicaMesa(FixedStep);
  Check(cut.bolas[0].vel.y>35 && Math.Abs(cut.bolas[1].vel.y)<5,"cut preserves cue tangent instead of redirecting the entire velocity");
  var spinFrameA=Make((new Vector2(-150,30),new Vector2(100,0)));var spinFrameB=Make((new Vector2(-150,30),new Vector2(100,0)));
  spinFrameA.bolas[0].rolagem=spinFrameB.bolas[0].rolagem=new Vector2(-80,0);
  for(int i=0;i<120;i++)spinFrameA.SimularFisicaMesa(1f/120);for(int i=0;i<60;i++)spinFrameB.SimularFisicaMesa(1f/60);
  Check(Vector2.Distance(spinFrameA.bolas[0].pos,spinFrameB.bolas[0].pos)<.01,"spin dynamics are stable at different call rates");
  Check(Math.Abs(MaxShotPower*BallMassKg*(2.84f/TableWidth)-.60f)<.0001f && MaxShotPower>500,"maximum cue impulse is calibrated for 170 gram balls");
  var rail=Make((new Vector2(TableWidth*.5f-BallRadius-.1f,30),new Vector2(100,0)));
  rail.SimularFisicaMesa(FixedStep);
  Check(rail.bolas[0].vel.x<0&&Math.Abs(rail.bolas[0].vel.x)<75,"cushion rebounds with energy loss");
  var pocket=Make((new Vector2(0,90),new Vector2(0,100)));
  for(int i=0;i<120;i++)pocket.SimularFisicaMesa(FixedStep);
  Check(!pocket.bolas[0].ativa&&pocket.brancaCaiuNestaTacada,"cue ball enters middle pocket and scratch is recorded");
  var a=Make((new Vector2(-150,30),new Vector2(100,0)));var b=Make((new Vector2(-150,30),new Vector2(100,0)));
  for(int i=0;i<120;i++)a.SimularFisicaMesa(1f/120);for(int i=0;i<60;i++)b.SimularFisicaMesa(1f/60);
  Check(Vector2.Distance(a.bolas[0].pos,b.bolas[0].pos)<.01,"substeps preserve trajectory at different call rates");
  var rack=Make((new Vector2(-110,0),new Vector2(MaxShotPower,0)));
  for(int row=0;row<5;row++)for(int col=0;col<=row;col++)rack.bolas.Add(new BolaMesa{numero=rack.bolas.Count,pos=new Vector2(65+row*BallRadius*1.75f,(col-row*.5f)*BallRadius*2.02f)});
  int steps=0;while(rack.settled==0&&steps++<2400)rack.SimularFisicaMesa(FixedStep);
  Check(rack.settled>0,"fifteen-ball break settles within 20 seconds");
  Check(rack.houvePrimeiroContatoBolaObjeto,"break reaches rack");
  foreach(var ball in rack.bolas)Check(!float.IsNaN(ball.pos.x)&&!float.IsNaN(ball.pos.y)&&ball.vel==Vector2.zero,"finite position and rest: ball "+ball.numero);
  foreach(float sy in new[]{-1f,1f})foreach(float speed in new[]{80f,220f,MaxShotPower})foreach(float offset in new[]{-6f,-4f,-2f,0f,2f,4f,6f}){
   var side=Make((new Vector2(offset,sy*60),new Vector2(0,sy*speed)));
   for(int i=0;i<480&&side.bolas[0].ativa;i++)side.SimularFisicaMesa(FixedStep);
   Check(!side.bolas[0].ativa,"visible side mouth accepts clear shot speed="+speed+" offset="+offset);
  }
  foreach(float sx in new[]{-1f,1f})foreach(float sy in new[]{-1f,1f})foreach(float speed in new[]{80f,220f,MaxShotPower})foreach(float offset in new[]{-3f,0f,3f}){
   Vector2 centre=new Vector2(sx*206,sy*101),dir=new Vector2(sx,sy).normalized,tangent=new Vector2(-dir.y,dir.x);
   var corner=Make((centre-dir*40+tangent*offset,dir*speed));
   for(int i=0;i<480&&corner.bolas[0].ativa;i++)corner.SimularFisicaMesa(FixedStep);
   Check(!corner.bolas[0].ativa,"corner mouth accepts clear diagonal "+sx+","+sy+" speed="+speed+" offset="+offset);
  }
  foreach(float sign in new[]{-1f,1f}){
   var miss=Make((new Vector2(sign*18,60),new Vector2(0,220)));
   bool bounced=false;for(int i=0;i<80;i++){miss.SimularFisicaMesa(FixedStep);if(miss.bolas[0].vel.y<0)bounced=true;}
   Check(miss.bolas[0].ativa && bounced,"outside-mouth shot rebounds instead of being magnetically captured");
  }
  Check(CruzaCapturaCacapa(new Vector2(-20,103),new Vector2(20,103),new Vector2(0,103)),"swept capture catches a fast pass through the throat");
  float maxRatio=0;
  for(int angle=0;angle<360;angle+=5)foreach(float spin in new[]{-200f,0f,200f}){
   Vector2 normal=Vector2.up,velocity=new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad)*220,-Math.Abs(Mathf.Sin(angle*Mathf.Deg2Rad)*220));
   float sideSpin=spin,beforeEnergy=velocity.sqrMagnitude+.4f*sideSpin*sideSpin;
   RebaterTabela(ref velocity,ref sideSpin,normal);
   float afterEnergy=velocity.sqrMagnitude+.4f*sideSpin*sideSpin;
   maxRatio=Math.Max(maxRatio,afterEnergy/beforeEnergy);
   if(afterEnergy>beforeEnergy+.02f)throw new Exception("Cushion creates energy at angle "+angle);
  }
  Check(maxRatio<=1.00001f,"216 glancing cushion/side-spin combinations never increase energy");
  for(int angle=-60;angle<=60;angle+=10){
   float y=BallRadius*2*Mathf.Sin(angle*Mathf.Deg2Rad);
   var impact=Make((new Vector2(0,30),new Vector2(220,0)),(new Vector2(BallRadius*2+.1f,30+y),Vector2.zero));
   float beforeEnergy=impact.Energy();
   for(int i=0;i<36;i++){
    impact.SimularFisicaMesa(FixedStep);
    float afterEnergy=impact.Energy();
    if(afterEnergy>beforeEnergy+.02f)throw new Exception("Ball impact creates energy at cut "+angle);
    foreach(var ball in impact.bolas)if(ball.vel.magnitude>220.01f)throw new Exception("Impact exceeds input speed");
    beforeEnergy=afterEnergy;
   }
  }
  Check(true,"13 cut angles dissipate energy on every step and never exceed incoming speed");
  GarantirClipsAudio();
  foreach(var clip in new[]{clipTacada,clipTabela,clipCacapa,clipGiz}){
   float peak=0;foreach(float x in clip.data){CheckSample(x);peak=Math.Max(peak,Math.Abs(x));}
   Check(peak>0&&peak<1,"synthesized event audio has signal and no clipping");
  }
  Console.WriteLine("TOTAL: "+checks+" PASS. Numerical harness only; no native Unity input, rendering, listening, or Player build.");
 }
 static void CheckSample(float x){if(float.IsNaN(x)||float.IsInfinity(x))throw new Exception("Invalid audio sample");}
'''
(out/'Program.cs').write_text(prefix+constants+'\n'+methods+'\n}', encoding='utf-8')
(out/'Harness.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>0649;0169</NoWarn></PropertyGroup><ItemGroup><Reference Include="UnityEngine.CoreModule"><HintPath>{unity.as_posix()}</HintPath></Reference></ItemGroup></Project>''', encoding='utf-8')
env = dict(os.environ, DOTNET_CLI_HOME=str(out/'dotnet-home'), DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Harness.csproj'),'--source',str(out),'--nologo','-v:q'],check=True,env=env)
subprocess.run(['dotnet','run','--project',str(out/'Harness.csproj'),'--no-restore','-v:q'],check=True,env=env)
