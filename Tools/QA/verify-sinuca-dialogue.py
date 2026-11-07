"""Run real C# reactions, subtitle timing and murmur synthesis with UI/audio stubs.

No Unity native rendering/listening or personal saves. Requires local .NET SDK.
"""
from pathlib import Path
import os
import re
import subprocess

root = Path(__file__).resolve().parents[2]
source = (root/'Projeto Jogo 2D/Assets/Scripts/SinucaMinigameUI.cs').read_text(encoding='utf-8-sig')
out = Path('C:/Temp/SantaRitaSinucaDialogueQA')
out.mkdir(parents=True, exist_ok=True)

def method(name):
    match = re.search(r'^    private (?:static )?[^\n]+\b'+name+r'\([^\n]*\)\s*\{', source, re.M)
    start = source.index('{', match.start())
    end, level = start+1, 1
    while level:
        level += (source[end] == '{') - (source[end] == '}')
        end += 1
    return source[match.start():end]

methods = '\n'.join(method(n) for n in ['EnfileirarFalaZe','LimparFalaZe','AtualizarFalaZe','TocarSilabaZe']).replace('AudioClip','SampleClip')
model = source[source.index('internal sealed class SinucaZeComments'):]
rules = (root/'Projeto Jogo 2D/Assets/Scripts/SinuquinhaRules.cs').read_text(encoding='utf-8-sig')
prefix = r'''
using System;
using System.Collections.Generic;
using UnityEngine;
class Time { public static float unscaledDeltaTime=.02f; }
class PlayerPrefs { public static float effects=1; public static float GetFloat(string key,float fallback)=>effects; }
class AudioSettingsController {public static float VolumeFala=>PlayerPrefs.effects;}
class SampleClip {
 public float[] data; public static SampleClip Create(string n,int length,int c,int hz,bool stream)=>new SampleClip();
 public void SetData(float[] d,int offset){data=d;}
}
class AudioSource {
 public bool playOnAwake,isPlaying,paused; public float spatialBlend,volume,pitch; public SampleClip clip;
 public void Play(){isPlaying=true;paused=false;} public void Stop(){isPlaying=paused=false;}
 public void Pause(){if(isPlaying){isPlaying=false;paused=true;}}
 public void UnPause(){if(paused){isPlaying=true;paused=false;}}
}
class Node { public bool activeSelf; public T AddComponent<T>() where T:new()=>new T(); }
class Group { public float alpha; }
class CharInfo { public char character; }
class TextInfo { public int characterCount; public CharInfo[] characterInfo; }
class Label {
 public string text=""; public int maxVisibleCharacters; public TextInfo textInfo;
 public void ForceMeshUpdate(){textInfo=new TextInfo{characterCount=text.Length,characterInfo=new CharInfo[text.Length]};for(int i=0;i<text.Length;i++)textInfo.characterInfo[i]=new CharInfo{character=text[i]};}
}
class Harness {
 enum EstadoPartida { Inativo,MirandoJogador }
 EstadoPartida estado=EstadoPartida.MirandoJogador;
 bool IsShowing=true,modoPratica;
 bool locutorAtualMarcus,locutorPendenteMarcus,OlhandoDebaixo,batidasOuvidas,inspecaoConcluida;
 int batidasRestantes; readonly Queue<string> falasHistoria=new Queue<string>();
 Node painelModal=new Node(),painelOpcoes=new Node(),painelAlvo=new Node(),gameObject=new Node();
 Label txtFalaZe=new Label(); Group legendaZe=new Group(); AudioSource vozFalaZe; SampleClip[] silabasZe;
 string falaZePendente; bool falaZePendenteUrgente;
 float tempoLetraZe,tempoLeituraZe,intervaloSilabaZe,intervaloFalaZe; int caractereZe,silabaZe;
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);Console.WriteLine("PASS "+message);checks++;}
 void Tick(int count=1,bool paused=false){for(int i=0;i<count;i++){if(vozFalaZe!=null&&!vozFalaZe.paused)vozFalaZe.isPlaying=false;AtualizarFalaZe(paused);}}
 static SinuquinhaRules Open(){var r=new SinuquinhaRules(true);r.Resolve(1,1,new List<int>{1},false,-1);return r;}
 public static void Main(){
  var c=new SinucaZeComments(); var r=Open();
  var a=c.React(r,new SinuquinhaRules.Result{credited=1},true);
  var b=c.React(r,new SinuquinhaRules.Result{credited=1},true);
  Check(a.Text!=null&&a.Text!=b.Text&&!a.Urgent,"pot reactions vary");
  for(int n=2;n<=5;n++)r.Present[n]=false;
  a=c.React(r,new SinuquinhaRules.Result{credited=1},true);
  Check(a.Urgent&&a.Text.Contains("chegando"),"Marcus with two balls gets near-win reaction");
  Check(!c.React(r,new SinuquinhaRules.Result{credited=1},true).Urgent,"near-win is not repeated");
  r.Present[6]=r.Present[7]=false;
  Check(c.React(r,new SinuquinhaRules.Result{credited=1},true).Text.Contains("preta"),"Marcus on black gets correct line");
  for(int n=9;n<=13;n++)r.Present[n]=false;
  Check(c.React(r,new SinuquinhaRules.Result{credited=1},false).Urgent,"Ze with two balls gets near-win reaction");
  r.Present[14]=r.Present[15]=false;
  Check(c.React(r,new SinuquinhaRules.Result{credited=1},false).Text.Contains("preta"),"Ze on black gets correct line");
  Check(c.React(r,new SinuquinhaRules.Result{foul=true,credited=1},true).Text.Contains("pressa"),"foul suppresses pot praise");
  Check(c.React(r,new SinuquinhaRules.Result{revertShot=true,foul=true},true).Urgent,"marked-ball foul takes priority");
  c.ResetHand();
  Check(c.React(r,new SinuquinhaRules.Result{credited=1},true).Urgent,"new hand resets milestones");
  var blank=new SinuquinhaRules(true);
  c.ResetHand();
  Check(c.React(blank,new SinuquinhaRules.Result(),true).Text==null&&c.React(blank,new SinuquinhaRules.Result(),true).Text==null,"silent gaps on dry shots");
  Check(c.React(blank,new SinuquinhaRules.Result(),false).Text!=null,"occasional dry-shot reaction");
  r.Resolve(8,8,new List<int>{8},false,-1);
  Check(c.React(r,new SinuquinhaRules.Result{credited=1},true).Text==null,"no stale reactions after hand ends");

  var h=new Harness(); h.EnfileirarFalaZe("Boa bola. Ainda tem mesa.");h.Tick();
  Check(h.caractereZe>4&&h.caractereZe<h.txtFalaZe.text.Length,"text types progressively");
  Check(h.vozFalaZe!=null&&h.vozFalaZe.clip!=null,"letters trigger dedicated NPC murmur");
  int visible=h.caractereZe;h.Tick(100,true);
  Check(h.caractereZe==visible&&h.legendaZe.alpha==0&&!h.vozFalaZe.isPlaying,"pause freezes text and sound");
  h.painelModal.activeSelf=true;h.Tick(100);
  Check(h.caractereZe==visible,"modal freezes subtitles");h.painelModal.activeSelf=false;
  h.Tick(10);Check(h.caractereZe>visible&&h.legendaZe.alpha>0,"resume continues subtitle");
  PlayerPrefs.effects=0;h.Tick();Check(h.vozFalaZe.volume==0,"effect mute applies live while subtitle remains");PlayerPrefs.effects=1;
  h.Tick(50);Check(h.caractereZe==h.txtFalaZe.text.Length&&h.legendaZe.alpha>0,"complete text remains for reading");
  h.Tick(130);Check(h.txtFalaZe.text==""&&h.legendaZe.alpha==0,"subtitle disappears after reading");
  h.EnfileirarFalaZe("Pendente");h.EnfileirarFalaZe("SÃ³ a preta.",true);h.EnfileirarFalaZe("Comum");
  Check(h.falaZePendente=="SÃ³ a preta.","important reaction replaces pending common line");
  h.LimparFalaZe();h.Tick(100);Check(h.txtFalaZe.text==""&&h.falaZePendente==null&&!h.vozFalaZe.isPlaying,"close/reset cancels text, queue and voice");
  h.modoPratica=true;h.EnfileirarFalaZe("NÃ£o deve aparecer");h.Tick(100);Check(h.txtFalaZe.text=="","practice has no opponent speech");
  foreach(var clip in h.silabasZe){float peak=0;foreach(float sample in clip.data){if(float.IsNaN(sample)||float.IsInfinity(sample))throw new Exception("Invalid sample");peak=Math.Max(peak,Math.Abs(sample));}Check(peak>0&&peak<.6f&&Math.Abs(clip.data[0])<.0001f&&Math.Abs(clip.data[clip.data.Length-1])<.0001f,"murmur finite, non-clipping, soft edges");}
  var pending=new Harness{batidasOuvidas=true,inspecaoConcluida=false};
  pending.EnfileirarFalaZe("Boa bola.");pending.Tick();
  Check(pending.txtFalaZe.text.StartsWith("Zé: ")&&pending.legendaZe.alpha>0,"unresolved table inspection does not silence Ze");
  pending.LimparFalaZe();pending.batidasRestantes=3;pending.EnfileirarFalaZe("Aguarda");
  Check(pending.falaZePendente==null,"knock audio retains priority during actual knocks");
  pending.batidasRestantes=0;pending.OlhandoDebaixo=true;pending.EnfileirarFalaZe("Aguarda");
  Check(pending.falaZePendente==null,"under-table investigation retains narrative priority");
  pending.OlhandoDebaixo=false;pending.falasHistoria.Enqueue("Historia");pending.EnfileirarFalaZe("Aguarda",true);
  Check(pending.falaZePendente==null,"queued narrative retains priority");
  pending.falasHistoria.Clear();pending.falaZePendente="Marcus historia";pending.falaZePendenteUrgente=true;pending.locutorPendenteMarcus=true;
  pending.EnfileirarFalaZe("Urgente",true);
  Check(pending.falaZePendente=="Marcus historia"&&pending.locutorPendenteMarcus,"dequeued Marcus story line is not overwritten or relabelled");
  Console.WriteLine("TOTAL "+checks+" PASS. C# source with UI/audio stubs; no native render/listening/Player.");
 }
'''
(out/'Program.cs').write_text(prefix+methods+'\n}\n'+model,encoding='utf-8')
(out/'Rules.cs').write_text(rules,encoding='utf-8')
unity='C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
(out/'Harness.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>0649;0169</NoWarn></PropertyGroup><ItemGroup><Reference Include="UnityEngine.CoreModule"><HintPath>{unity}</HintPath></Reference></ItemGroup></Project>',encoding='utf-8')
env=dict(os.environ,DOTNET_CLI_HOME=str(out/'dotnet-home'),DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Harness.csproj'),'--source',str(out),'--nologo','-v:q'],check=True,env=env)
subprocess.run(['dotnet','run','--project',str(out/'Harness.csproj'),'--no-restore','-v:q'],check=True,env=env)
