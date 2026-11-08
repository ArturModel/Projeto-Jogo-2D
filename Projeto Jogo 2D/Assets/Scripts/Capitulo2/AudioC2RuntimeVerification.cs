#if UNITY_EDITOR || DEBUG
using UnityEngine.InputSystem;using UnityEngine.InputSystem.LowLevel;
using System;using System.Collections;using System.Linq;using UnityEngine;using UnityEngine.UI;
public sealed partial class Chapter2RuntimeVerification
{
 private bool audioC2Force,audioPrefsChanged,audioPrefExisted;private float audioPrefOriginal;
#if UNITY_EDITOR
 public static void StartAudioC2Editor(string output){Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);var go=new GameObject("QA_AudioC2");DontDestroyOnLoad(go);go.AddComponent<Chapter2RuntimeVerification>().audioC2Force=true;}
#endif
 private void RestoreAudioC2Prefs(){if(!audioPrefsChanged)return;if(audioPrefExisted)PlayerPrefs.SetFloat(AudioSettingsController.PreferenciaMusica,audioPrefOriginal);else PlayerPrefs.DeleteKey(AudioSettingsController.PreferenciaMusica);audioPrefsChanged=false;}
 private IEnumerator RunMarcusQA()
 {
  yield return new WaitForSecondsRealtime(2);
  var sheet=new Texture2D(2,2,TextureFormat.RGBA32,false);
  ImageConversion.LoadImage(sheet,System.IO.File.ReadAllBytes(System.IO.Path.Combine(Application.streamingAssetsPath,"MarcusWalkFluido.png")));
  var validation=typeof(PlayerController).GetMethod("ValidarFolhaMarcus",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);
  Check((bool)validation.Invoke(null,new object[]{sheet,3,4,false}),"Os 12 quadros da folha de Marcus têm corpo e bordas transparentes.");
  Destroy(sheet);
  GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.LanternaDoCarroObtida);
  SceneFader.Instance.LoadScene("CentroSantaRita");yield return AwaitScene("CentroSantaRita");yield return CloseText();yield return new WaitForSecondsRealtime(1);
#if !UNITY_EDITOR
  Screen.SetResolution(1280,720,FullScreenMode.Windowed);yield return null;yield return new WaitForSecondsRealtime(1);
#endif
  var player=FindAnyObjectByType<PlayerController>();var body=player.GetComponent<SpriteRenderer>();
  var previous=Keyboard.current;var keyboard=InputSystem.AddDevice<Keyboard>();
  try
  {
   foreach(bool equipped in new[]{false,true})
   {
    if(GameState.LanternaLigada!=equipped)GameState.AlternarLanterna();
    foreach(var key in new[]{Key.A,Key.D,Key.W,Key.S})
    {
     yield return Place(new Vector2(31,45));yield return null;
     var poses=new System.Collections.Generic.HashSet<Sprite>();
     for(int i=0;i<12;i++)
     {
      keyboard.MakeCurrent();InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return new WaitForSecondsRealtime(.09f);poses.Add(body.sprite);
      if(i%4==0)yield return Capture("marcus-"+key+"-"+equipped+"-"+i);
     }
     keyboard.MakeCurrent();InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSecondsRealtime(.4f);
     int row = key == Key.S ? 0 : key == Key.W ? 1 : key == Key.A ? 2 : 3;
     var atlas = Field<Sprite[,]>(player,"quadrosCaminhada");
     Check(poses.Count==3 && poses.All(p=>p.texture==atlas[0,0].texture && p.rect.y==(3-row)*64),"Três poses da linha correta: "+key+" lanterna="+equipped);
     Check(body.sprite==atlas[row,1] && body.sprite.texture.width==192 && body.sprite.rect.width==64,"Repouso mantém a pose central: "+key+" lanterna="+equipped);
     Check(player.LanternaVisivel==equipped,"Feixe independente da folha: "+key+" lanterna="+equipped);
     yield return Capture("repouso-"+key+"-"+equipped);
    }
   }
  }
  finally{InputSystem.RemoveDevice(keyboard);if(previous!=null)previous.MakeCurrent();}
  if(GameState.LanternaLigada)GameState.AlternarLanterna();
  yield return Walking();Finish();
 }
 private IEnumerator RunAudioC2()
 {
  yield return new WaitForSecondsRealtime(2);
#if !UNITY_EDITOR
  Screen.SetResolution(1280,720,FullScreenMode.Windowed);yield return null;yield return new WaitForSecondsRealtime(1);
#endif
  audioPrefExisted=PlayerPrefs.HasKey(AudioSettingsController.PreferenciaMusica);audioPrefOriginal=PlayerPrefs.GetFloat(AudioSettingsController.PreferenciaMusica,1);audioPrefsChanged=true;PlayerPrefs.SetFloat(AudioSettingsController.PreferenciaMusica,1);
  float ganhoMusica = AudioSettingsController.VolumeMusica;
  foreach(string n in new[]{"Centro","Cemiterio","Bocha"}){var c=Resources.Load<AudioClip>("Audio/"+n+"_loop");Check(c!=null&&c.length>80&&c.frequency==48000,"OGG importado corretamente: "+n);}
  foreach(var prop in SantaRitaBarLayout.Props)Check(SantaRitaBarLayout.SpriteFor(prop)!=null,"Sprite real do bar: "+prop.id);
  SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");yield return CloseText();
#if !UNITY_EDITOR
  Screen.SetResolution(1280,720,FullScreenMode.Windowed);yield return null;yield return new WaitForSecondsRealtime(1);
#endif
  foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.LanternaDoCarroObtida,Flags.MapaDoCarroObtido,Flags.AchouReciboMercadinho,Flags.RecebeuFichaClubeZe,Capitulo2Director.Ficha})GameState.Marcar(flag);
  var inventory=FindAnyObjectByType<SantaRitaInventory>();yield return null;var pictures=Field<Image[]>(inventory,"pictures");Check(pictures.Take(3).All(i=>i.sprite!=null&&i.enabled)&&pictures.Skip(3).All(i=>!i.enabled),"Tres itens reais; fichas ausentes da barra apesar de obtidas");
  Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Interior_BarDoZe,new Vector2(509,-55.5f),1);yield return new WaitForSecondsRealtime(2);yield return CloseText();yield return Capture("bar-e-inventario");
  var barTable=GameObject.Find("MesaChimarrao").GetComponent<SpriteRenderer>();
  foreach(string prop in new[]{"CuiaMesa","TermicaMesa"})Check(GameObject.Find(prop).GetComponent<SpriteRenderer>().sortingOrder>barTable.sortingOrder,"Objeto apoiado desenhado acima do tampo: "+prop);
  foreach(var def in SantaRitaBarLayout.Props.Where(p=>p.kind==SantaRitaBarLayout.Kind.Actor&&!p.key.Contains("Ze")))
  {
   Check(Enumerable.Range(0,240).Select(i=>SantaRitaBarLayout.PatronPose(def.key,def.id,i*.1f)).Where(x=>x!=null).Distinct().Count()==8,"Oito poses de rotina disponiveis: "+def.id);
   Check(SantaRitaBarLayout.PatronPose(def.key,def.id,0)!=null,"Rotina sentada carregada: "+def.id);
   Check(FindObjectsByType<SantaRitaBarPose>().Any(x=>x.actorId==def.id),"Ator usa rotina compartilhada: "+def.id);
  }
  var pool=SinucaMinigameUI.Instance;pool.IniciarPraticaOpcional();yield return new WaitForSecondsRealtime(2);yield return Capture("bar-na-sinuca");pool.Fechar();
  Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Zona5_RuaNorteBar,new Vector2(525,12),1);yield return new WaitForSecondsRealtime(3);yield return CloseText();
  var bairro=Field<AudioSource>(Capitulo1Director.Instance,"musicaBairro");Check(bairro!=null && (ganhoMusica > 0 ? bairro.isPlaying && bairro.volume > 0 : !bairro.isPlaying && bairro.volume == 0),"Musica do bairro toca na Rua Norte exterior do bar");

  AudioSource centro=null;
  foreach(string scene in new[]{"CentroSantaRita","IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira","CemiterioSantaRita","AbrigoCemiterio","SociedadeBeiraRio","CentroSantaRita"})
  {
   SceneFader.Instance.LoadScene(scene);yield return new WaitForSecondsRealtime(1);yield return AwaitScene(scene);yield return CloseText();yield return new WaitForSecondsRealtime(2);
   int region=scene=="CemiterioSantaRita"||scene=="AbrigoCemiterio"?1:scene=="SociedadeBeiraRio"?2:0;var audio=FindAnyObjectByType<Capitulo2Audio>();Check(audio!=null,"Gerenciador de audio ativo: "+scene);var sources=audio.GetComponentsInChildren<AudioSource>();var f=sources.Single(x=>x.gameObject.name=="TrilhaC2_"+region);
   string name=new[]{"Centro","Cemiterio","Bocha"}[region];bool inside=Capitulo2Director.Instance.interior;
   Check(f.clip!=null&&f.clip.name==name+"_loop"&&f.loop && f.isPlaying == (ganhoMusica > 0),"Trilha exclusiva em loop: "+scene);
   Check(Mathf.Abs(f.volume-(inside?.1625f:.25f)*ganhoMusica)<.01f,"Volume contextual: "+scene);
   Check(Mathf.Abs(f.GetComponent<AudioLowPassFilter>().cutoffFrequency-(inside?1800:22000))<5,"Filtro contextual: "+scene);
   Check(sources.Where(x=>x!=f).All(x=>x.volume<.001f),"Sem sobreposicao de outra regiao: "+scene);
   if(region==0){if(centro!=null)Check(f==centro,"Mesma fonte continua entre cenas do Centro");centro=f;}
   if(scene=="CemiterioSantaRita"||scene=="SociedadeBeiraRio"||scene=="IgrejaMatriz")yield return Capture(scene);
   if(scene=="SociedadeBeiraRio")
   {
    BochaMinigameUI.Instance.Abrir(null);yield return new WaitForSecondsRealtime(1.5f);Check(Mathf.Abs(f.volume-AudioSettingsController.TrilhaDiretaMinigame*ganhoMusica)<.01f,"Bocha reduz musica ao entrar no jogo");yield return Capture("bocha-musica-reduzida");BochaMinigameUI.Instance.Fechar();yield return new WaitForSecondsRealtime(1.5f);Check(Mathf.Abs(f.volume-.1625f*ganhoMusica)<.01f,"Bocha devolve volume ao sair do jogo");
   }
   if (ganhoMusica > 0)
   {
    f.time=f.clip.length-.3f;yield return new WaitForSecondsRealtime(1);Check(f.isPlaying&&f.time<3,"Loop atravessa fim do OGG sem encerrar: "+scene);
   }
  }
  PlayerPrefs.SetFloat(AudioSettingsController.PreferenciaMusica,.5f);yield return new WaitForSecondsRealtime(1.5f);Check(Mathf.Abs(centro.volume-.125f*ganhoMusica)<.01f,"Volume de musica do usuario respeitado");
  yield return Walking();
  Check(!CheckpointSystem.HasCheckpoint,"Nenhum autosave durante transicoes e minigame");Finish();
 }
}
#endif
