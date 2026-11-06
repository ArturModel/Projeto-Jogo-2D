"""Execute the whole AudioSettingsController C# against isolated Unity doubles.

Tests menu/pause lifecycle, slider events, settings persistence and mixer gains.
No access to real PlayerPrefs, personal saves, native audio or user sessions.
"""
from pathlib import Path
import os
import subprocess
import re

root = Path(__file__).resolve().parents[2]
out = Path('C:/Temp/SantaRitaAudioSettingsQA')
out.mkdir(parents=True, exist_ok=True)
source = (root/'Projeto Jogo 2D/Assets/Scripts/AudioSettingsController.cs').read_text(encoding='utf-8-sig')
# UI creation and native rain source discovery are validated in Unity separately.
for name in ['ConstruirCategoriasMenu', 'ClonarCategoria', 'AtualizarChuvaMenu']:
    match = re.search(r'^    private (?:static )?[^\n]+\b'+name+r'\([^\n]*\)\s*\{',source,re.M)
    start = source.index('{',match.start()); end,level=start+1,1
    while level:
        level += (source[end]=='{')-(source[end]=='}');end+=1
    source=source[:match.start()]+('' if name=='ClonarCategoria' else '    private static void '+name+'() {}')+source[end:]
(out/'Controller.cs').write_text(source,encoding='utf-8')
(out/'Stubs.cs').write_text(r'''
using System;
using System.Collections.Generic;
namespace UnityEngine {
 public class MonoBehaviour {public bool isActiveAndEnabled=true;}
 public class HeaderAttribute:Attribute {public HeaderAttribute(string t){}}
 public class SerializeField:Attribute {}
 public enum RuntimeInitializeLoadType {AfterSceneLoad}
 public class RuntimeInitializeOnLoadMethodAttribute:Attribute {public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t){}}
 public static class PlayerPrefs {
  public static Dictionary<string,float> data=new Dictionary<string,float>(); public static int saves;
  public static bool HasKey(string k)=>data.ContainsKey(k);
  public static float GetFloat(string k,float d)=>data.TryGetValue(k,out var v)?v:d;
  public static void SetFloat(string k,float v){data[k]=v;}
  public static void Save(){saves++;}
 }
 public static class AudioListener {public static float volume;public static bool pause;}
 public static class Mathf {public static float Clamp01(float v)=>Math.Clamp(v,0,1);public static float Max(float a,float b)=>Math.Max(a,b);public static float Log10(float v)=>(float)Math.Log10(v);}
 public static class Resources {public static Audio.AudioMixer[] mixers;public static T[] FindObjectsOfTypeAll<T>()=>mixers as T[];}
}
namespace UnityEngine.Audio {public class AudioMixer {public Dictionary<string,float> gains=new Dictionary<string,float>(); public bool SetFloat(string p,float v){gains[p]=v;return true;}}}
namespace UnityEngine.SceneManagement {
 public struct Scene {} public enum LoadSceneMode {Single}
 public static class SceneManager {public static event Action<Scene,LoadSceneMode> sceneLoaded;public static void Load(){sceneLoaded?.Invoke(new Scene(),LoadSceneMode.Single);}}
}
namespace UnityEngine.UI {
 public class Slider {
  public float value,minValue,maxValue; public Changed onValueChanged=new Changed();
  public void SetValueWithoutNotify(float v){value=v;}
  public void Drag(float v){value=v;onValueChanged.Invoke(v);}
 }
 public class Changed {
  public List<Action<float>> listeners=new List<Action<float>>();
  public void AddListener(Action<float> f){listeners.Add(f);}public void RemoveListener(Action<float> f){listeners.RemoveAll(x=>x==f);}
  public void Invoke(float v){foreach(var f in listeners.ToArray())f(v);}
 }
}
public static class SinucaMinigameUI {public static bool IsShowing;}
''',encoding='utf-8')
(out/'Program.cs').write_text(r'''
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
class Program {
 static int checks;
 static void Check(bool ok,string msg){if(!ok)throw new Exception(msg);Console.WriteLine("PASS "+msg);checks++;}
 static void Call(object o,string m){typeof(AudioSettingsController).GetMethod(m,BindingFlags.Instance|BindingFlags.Static|BindingFlags.NonPublic).Invoke(o,null);}
 static bool Near(float a,float b)=>Math.Abs(a-b)<.00001f;
 static float Gain(AudioMixer m,string p)=>(float)Math.Pow(10,m.gains[p]/20);
 static void Main(){
  var mixer=new AudioMixer();Resources.mixers=new[]{mixer};
  AudioSettingsController.AplicarVolumesGlobais();
  Check(Near(PlayerPrefs.GetFloat("audio_musica",0),.35f),"fresh music default is 35 percent");
  Check(PlayerPrefs.GetFloat("audio_efeitos",0)==1&&AudioListener.volume==1,"fresh effects full and master full");
  PlayerPrefs.SetFloat("audio_master",0);PlayerPrefs.SetFloat("audio_musica",0);PlayerPrefs.SetFloat("audio_efeitos",.05f);
  AudioSettingsController.AplicarVolumesGlobais();
  Check(AudioListener.volume==0&&PlayerPrefs.GetFloat("audio_musica",1)==0&&PlayerPrefs.GetFloat("audio_efeitos",1)==.05f,"legacy low and muted preferences never reset");
  var menu=new AudioSettingsController();var master=new Slider();var music=new Slider();var effects=new Slider();
  var speech=new Slider();var rain=new Slider();var hits=new Slider();
  menu.Vincular(mixer,master,music,effects,speech,rain,hits);Call(menu,"OnEnable");Call(menu,"Update");
  Check(speech.value==1&&rain.value==.55f&&hits.value==1,"new categories have independent defaults despite old effects at five percent");
  Check(master.value==0&&music.value==0&&effects.value==.05f,"main menu sliders load saved values");
  master.Drag(.6f);music.Drag(.2f);effects.Drag(.8f);
  Check(Near(AudioListener.volume,.6f)&&Near(Gain(mixer,"VolumeMaster"),1),"master applied once at listener");
  Check(Near(Gain(mixer,"VolumeMusic"),.1f)&&Near(Gain(mixer,"VolumeSFX"),.8f),"main menu sliders apply actual mixer gains");
  var pause=new AudioSettingsController{isActiveAndEnabled=false};var pm=new Slider();var pu=new Slider();var pe=new Slider();
  pause.Vincular(null,pm,pu,pe);
  Check(pm.value==.6f&&pu.value==.2f&&pe.value==.8f,"inactive pause receives same settings before first opening");
  pause.isActiveAndEnabled=true;Call(pause,"OnEnable");Call(pause,"Update");
  pu.Drag(.4f);
  Check(Near(Gain(mixer,"VolumeMusic"),.2f),"pause without local mixer updates loaded menu mixer");
  Check(music.value==.4f,"pause change synchronizes main menu slider");
  music.Drag(.15f);Check(pu.value==.15f,"menu change synchronizes active pause slider");
  pu.Drag(0);Check(Gain(mixer,"VolumeMusic")<.0002f&&music.value==0,"music zero mutes menu mixer from pause");
  pe.Drag(0);Check(Gain(mixer,"VolumeSFX")<.0002f&&effects.value==0,"effects zero updates mixer and all panels");
  Call(pause,"OnDisable");pause.isActiveAndEnabled=false;music.Drag(.3f);
  pause.isActiveAndEnabled=true;Call(pause,"OnEnable");Check(pu.value==.3f,"reopening pause reloads latest menu value");
  pause.Vincular(null,pm,pu,pe);pause.Vincular(null,pm,pu,pe);
  Check(pu.onValueChanged.listeners.Count==1,"rebinding does not duplicate slider listeners");
  Call(pause,"OnDisable");Call(pause,"OnEnable");Check(pu.onValueChanged.listeners.Count==1,"reopening does not duplicate listeners");
  SinucaMinigameUI.IsShowing=true;Call(menu,"Update");
  Check(Near(Gain(mixer,"VolumeMusic"),.3f*.5f*.35f),"practice ducks real menu music without chapter director");
  Check(music.value==.3f&&pu.value==.3f,"practice attenuation never overwrites preferences or sliders");
  SinucaMinigameUI.IsShowing=false;Call(menu,"Update");Check(Near(Gain(mixer,"VolumeMusic"),.15f),"closing practice restores selected menu level");
  AudioListener.pause=true;AudioSettingsController.AplicarVolumesGlobais();Check(AudioListener.pause,"volume change preserves audio pause");
  menu.AlterarVolumeMusica(4);menu.AlterarVolumeEfeitos(-2);Check(music.value==1&&effects.value==0,"out of range changes clamped consistently");
  Call(null,"InicializarAudioGlobal");Call(null,"InicializarAudioGlobal");
  UnityEngine.SceneManagement.SceneManager.Load();
  Check(PlayerPrefs.GetFloat("audio_musica",0)==1&&PlayerPrefs.GetFloat("audio_efeitos",1)==0,"scene load preserves settings");
  Check(PlayerPrefs.saves>0,"settings changes are persisted immediately");
  speech.Drag(.7f);rain.Drag(.6f);hits.Drag(.9f);
  menu.AlterarVolumeEfeitos(0);
  Check(AudioSettingsController.VolumeFala==.7f&&AudioSettingsController.VolumeChuva==.6f&&AudioSettingsController.VolumeImpactos==.9f,"other effects mute leaves speech rain and impacts independent");
  speech.Drag(0);Check(AudioSettingsController.VolumeFala==0&&AudioSettingsController.VolumeChuva==.6f&&AudioSettingsController.VolumeImpactos==.9f,"speech mute leaves rain and impacts unchanged");
  rain.Drag(0);Check(AudioSettingsController.VolumeChuva==0&&AudioSettingsController.VolumeImpactos==.9f,"rain mute leaves impacts unchanged");
  hits.Drag(0);menu.AlterarVolumeFala(.8f);Check(AudioSettingsController.VolumeImpactos==0&&AudioSettingsController.VolumeFala==.8f,"impact mute leaves speech independent");
  var ps=new Slider();var pr=new Slider();var ph=new Slider();pause.Vincular(null,pm,pu,pe,ps,pr,ph);
  Check(ps.value==.8f&&pr.value==0&&ph.value==0,"pause loads all category preferences");
  pr.Drag(.4f);Check(rain.value==.4f&&AudioSettingsController.VolumeFala==.8f,"category sliders synchronize across menu and pause");
  Console.WriteLine("TOTAL "+checks+" PASS. Entire C# controller with isolated Unity doubles; no native audio or user preferences touched.");
 }
}
''',encoding='utf-8')
(out/'Harness.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>',encoding='utf-8')
env=dict(os.environ,DOTNET_CLI_HOME=str(out/'dotnet-home'),DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
subprocess.run(['dotnet','restore',str(out/'Harness.csproj'),'--source',str(out),'--nologo','-v:q'],check=True,env=env)
subprocess.run(['dotnet','run','--project',str(out/'Harness.csproj'),'--no-restore','-v:q'],check=True,env=env)
