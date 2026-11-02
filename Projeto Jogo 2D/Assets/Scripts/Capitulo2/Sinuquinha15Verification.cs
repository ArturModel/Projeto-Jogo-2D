#if UNITY_EDITOR || DEBUG
using System;using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;using UnityEngine.InputSystem;using UnityEngine.InputSystem.LowLevel;
public sealed partial class Chapter2RuntimeVerification
{
 private bool sinuquinhaForce;
#if UNITY_EDITOR
 public static void StartSinuquinhaEditor(string output){Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);var g=new GameObject("QA_Sinuquinha15");DontDestroyOnLoad(g);g.AddComponent<Chapter2RuntimeVerification>().sinuquinhaForce=true;}
#endif
 private IEnumerator RunSinuquinha15()
 {
  Application.targetFrameRate=60;QualitySettings.vSyncCount=0;
#if UNITY_EDITOR
  previousEditorInput=InputSystem.settings.editorInputBehaviorInPlayMode;previousBackgroundInput=InputSystem.settings.backgroundBehavior;ref7InputOverride=true;
  InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
  UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
#endif
  var r=new SinuquinhaRules(true);int count=0;for(int n=1;n<16;n++)if(r.Present[n])count++;Check(count==15,"Quinze bolas de objeto, preta incluida");
  r.Resolve(1,1,new List<int>{1},false,-1);Check(r.MarcusGroup==1&&r.MarcusTurn&&!r.Present[1],"Primeira conversao define lisas e mantem vez");
  var foul=r.Resolve(2,9,new List<int>(),false,-1);Check(foul.foul&&foul.cueInHand&&!r.MarcusTurn,"Contato errado concede branca na mao");
  r=new SinuquinhaRules(true);r.Resolve(1,1,new List<int>(),false,-1);r.Resolve(8,8,new List<int>{8},false,-1);Check(r.Finished&&r.Winner==0,"Preta antecipada perde a mao");
  r=new SinuquinhaRules(true);r.Resolve(1,1,new List<int>{1},false,-1);for(int n=2;n<=7;n++)r.Resolve(n,n,new List<int>{n},false,-1);Check(r.CanNominate(8),"Preta liberada apos limpar grupo");r.Resolve(8,8,new List<int>{8},false,-1);Check(r.Finished&&r.Winner==0,"Preta legal encerra mao");
  r=new SinuquinhaRules(true);var annulled=r.Resolve(1,1,new List<int>{1,9},false,9);Check(annulled.revertShot&&r.Present[1]&&r.Present[9]&&r.MarcusGroup==0&&!r.MarcusTurn,"Bola marcada anula conversoes e passa branca na mao");
  SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");yield return CloseText();
  Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Interior_BarDoZe,new Vector2(509,-54.8f),1);yield return new WaitForSecondsRealtime(2);yield return CloseText();
  sinucaMouse=InputSystem.AddDevice<Mouse>("SinuquinhaMouse");sinucaR4Keyboard=InputSystem.AddDevice<Keyboard>("SinuquinhaKeyboard");sinucaR4Keyboard.MakeCurrent();
  var player=FindAnyObjectByType<PlayerController>();int stepsHeard=0;bool aligned=true;Action contact=()=>{stepsHeard++;aligned&=player.QuadroCaminhada==0||player.QuadroCaminhada==4;};player.PeApoiou+=contact;
  Vector3 start=player.transform.position;InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState(Key.D));yield return new WaitForSecondsRealtime(.5f);yield return Capture("marcus-andando");InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState());yield return null;player.PeApoiou-=contact;
  Check(Vector3.Distance(start,player.transform.position)>.2f&&stepsHeard>0&&aligned,"Marcus anda com eventos sonoros nos quadros de apoio 0/4");
  var flag=GameObject.Find("Inter1994").GetComponent<SantaRitaBarProp>();flag.Interact();yield return new WaitForSecondsRealtime(.5f);yield return Capture("bandeira-exame");yield return CloseText();
  GameMessageUI.Instance.ShowInspection("RECIBO DO MERCADINHO","Bilhete","Querosene: 1 litro. Entrega recebida no balcão.");yield return null;Check(GameObject.Find("DocumentoLegivel")!=null,"Documentos ampliados usam texto tipografico legivel");yield return new WaitForSecondsRealtime(.5f);yield return Capture("documento-legivel");yield return CloseText();
  Check(GameObject.Find("TapeteSinuca")==null,"Sem tapete sob a mesa");
  var mat=GameObject.Find("Interior4_BarDoZe").GetComponentInChildren<SantaRitaExitThreshold>();Check(mat!=null&&mat.GetComponent<SpriteRenderer>().sortingOrder>-5,"Capacho aparece acima do piso");yield return Capture("bar");
  var ui=SinucaMinigameUI.Instance;ui.IniciarDuelo(null);yield return null;Check(Field<GameObject>(ui,"painelModal").activeSelf,"Ze oferece os dois modos de identificacao");yield return Capture("escolha-ze");yield return ClickR4(Field<Button>(ui,"btnModalPrimario"));yield return null;
  Check(Field<System.Collections.IList>(ui,"bolas").Count==16&&!Field<bool>(ui,"brancaNaMao"),"Branca pronta e quinze numeradas, sem posicionamento na saida");
  var view=FindAnyObjectByType<SinucaTableView>();foreach(var mode in new[]{SinucaCameraView.Mesa3D,SinucaCameraView.LinhaTaco,SinucaCameraView.VistaSuperior}){view.DefinirCamera(mode);Invoke(ui,"AtualizarBotaoVistaCamera");yield return null;for(int i=0;i<15;i++)view.Orbit(25);yield return Capture("mesa-"+mode);}
  yield return Ref7Checks(ui,view);
  yield return SinuquinhaKeyboard(ui,view);
  yield return SinuquinhaMarked(ui,view);
  ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",1);Field<GameObject>(ui,"painelModal").SetActive(false);Invoke(ui,"IniciarTurnoAtual");
  Check(ui.SimularTesteDeInvariantes(out var kernelReport),"Seam publico atualizado para quinze bolas");
  view.DefinirCamera(SinucaCameraView.LinhaTaco);Invoke(ui,"AtualizarBotaoVistaCamera");var direction=view.Room.camera.transform.forward;view.Orbit(180);Check(Vector3.Dot(direction,view.Room.camera.transform.forward)<-.6f,"Camera contorna para o lado oposto sem limite angular");yield return Capture("camera-oposta");view.Orbit(180);Check(Vector3.Dot(direction,view.Room.camera.transform.forward)>.999f,"Orbita completa de 360 graus retorna a orientacao");
  view.DefinirCamera(SinucaCameraView.Mesa3D);Invoke(ui,"ExecutarTacadaComEfeito",Vector2.right,440f,SinucaCueSpin.Normal);int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<4000)Invoke(ui,"SimularFisicaMesa",1f/120);ui.StopAllCoroutines();Check(steps<4000,"Saida fisica de 16 bolas chega ao repouso");Check(ui.ValidarInvarianteSeteBolas(),"Presentes e convertidas coerentes apos saida");yield return Capture("apos-saida");ui.Fechar();ui.IniciarDuelo(null);yield return null;yield return ClickR4(Field<Button>(ui,"btnModalSecundario"));yield return null;Check(Field<bool>(ui,"gruposPorNumero")&&!Field<bool>(ui,"brancaNaMao"),"Escolha numerada abre partida pronta");yield return SinuquinhaPhysicalHand(ui);ui.Fechar();yield return SinuquinhaSeriesFixture(ui);Check(!CheckpointSystem.HasCheckpoint,"Sem autosave");yield return PauseLoadFiveSlots();InputSystem.RemoveDevice(sinucaMouse);InputSystem.RemoveDevice(sinucaR4Keyboard);sinucaR4Keyboard=null;Finish();
 }

 private IEnumerator SinuquinhaKeyboard(SinucaMinigameUI ui,SinucaTableView view)
 {
  Check(GameObject.Find("CameraEsquerda")==null&&GameObject.Find("CameraDireita")==null,"Sem botoes de contorno na tela");
  foreach(var mode in new[]{SinucaCameraView.Mesa3D,SinucaCameraView.LinhaTaco,SinucaCameraView.VistaSuperior})
  {
   view.DefinirCamera(mode);float aim=Field<float>(ui,"anguloMiraRad");var rotation=view.Room.camera.transform.rotation;var before=view.Room.camera.transform.position;var cameraRight=view.Room.camera.transform.right;
   InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState(Key.A));yield return new WaitForSecondsRealtime(.18f);InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState());yield return null;
   Check(Quaternion.Angle(rotation,view.Room.camera.transform.rotation)>2&&Mathf.Abs(aim-Field<float>(ui,"anguloMiraRad"))<.001f&&(mode==SinucaCameraView.VistaSuperior||Vector3.Dot(view.Room.camera.transform.position-before,cameraRight)<0),"A contorna sem mudar mira: "+mode);
   rotation=view.Room.camera.transform.rotation;InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState(Key.RightArrow));yield return new WaitForSecondsRealtime(.18f);InputSystem.QueueStateEvent(sinucaR4Keyboard,new KeyboardState());yield return null;
   Check(Mathf.Abs(aim-Field<float>(ui,"anguloMiraRad"))>.01f&&Quaternion.Angle(rotation,view.Room.camera.transform.rotation)<.1f,"Seta direciona sem contornar: "+mode);
  }
 }
 private IEnumerator SinuquinhaMarked(SinucaMinigameUI ui,SinucaTableView view)
 {
  Invoke(ui,"ConfigurarMao",2);Invoke(ui,"IniciarTurnoAtual");yield return null;int marked=Field<int>(ui,"numeroBolaMarcada");Check(marked>0&&marked!=8,"Segunda mao seleciona marcada sem bloquear a preta");
  yield return Capture("regra-segunda-mao");if(Field<GameObject>(ui,"painelModal").activeSelf)yield return ClickR4(Field<Button>(ui,"btnModalPrimario"));
  yield return null;Check(view.Room.root.transform.Find("ElementosMoveis/MarcaGiz_"+marked).gameObject.activeSelf,"X de giz visivel na bola marcada");yield return Capture("bola-marcada");
  var balls=Field<System.Collections.IList>(ui,"bolas");balls[0].GetType().GetField("pos").SetValue(balls[0],new Vector2(0,55));balls[marked].GetType().GetField("pos").SetValue(balls[marked],new Vector2(0,85));
  Invoke(ui,"ExecutarTacadaComEfeito",Vector2.up,120f,SinucaCueSpin.Normal);int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<4000)Invoke(ui,"SimularFisicaMesa",1f/120);ui.StopAllCoroutines();
  var r=Field<SinuquinhaRules>(ui,"regrasBrasileiras");Check(steps<4000&&r.Present[marked]&&Field<bool>(ui,"brancaNaMao")&&Vector2.Distance((Vector2)balls[marked].GetType().GetField("pos").GetValue(balls[marked]),new Vector2(0,85))<.01f,"Queda fisica da marcada restaura a formacao e concede branca na mao");
  Invoke(ui,"ConfigurarMao",3);Invoke(ui,"IniciarTurnoAtual");ui.StopAllCoroutines();Check(Field<int>(ui,"numeroBolaMarcada")>0,"Terceira mao tambem aplica marcada");
 }
 private IEnumerator PauseLoadFiveSlots()
 {
  // Only the already isolated QA namespace is written. Save exclusively remains a payphone action in production.
  GameState.DefinirNumero("qa_slot_escolhido",4);CheckpointSystem.SaveSlot(4,"EntradaDaCidade",new Vector2(509,-55),"qa_bar");
  GameState.DefinirNumero("qa_slot_escolhido",0);CheckpointSystem.SaveSlot(0,"EntradaDaCidade",new Vector2(119,12),"qa_inicio");
  var menu=FindAnyObjectByType<MenuPausa>();menu.Abrir();yield return Capture("menu-carregar");yield return ClickR4(GameObject.Find("Carregar Jogo").GetComponent<Button>());yield return null;
  var slots=(Button[])typeof(PayphoneCheckpoint).GetField("s_botoesSlot",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).GetValue(null);
  Check(PayphoneCheckpoint.IsSaveUIOpen&&slots.Length==5&&slots[0].interactable&&slots[4].interactable&&!slots[1].interactable,"Pausa lista cinco slots e desabilita vazios");yield return Capture("cinco-slots");
  PayphoneCheckpoint.FecharModalSlots();Check(MenuPausa.Aberto&&Time.timeScale==0,"Cancelar carga retorna a pausa sem retomar partida");
  menu.AbrirCarregar();yield return null;yield return ClickR4(slots[4]);yield return new WaitForSecondsRealtime(2);yield return AwaitScene("EntradaDaCidade");
  Check(GameState.Numero("qa_slot_escolhido")==4&&!MenuPausa.Aberto&&Time.timeScale==1,"Carrega slot escolhido 5 em vez do ultimo slot 1");
  Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Zona5_RuaNorteBar,new Vector2(513.5f,12.3f),1);yield return new WaitForSecondsRealtime(1.5f);var chair=GameObject.Find("Cadeira_Calcada_1").GetComponent<SpriteRenderer>();Check(chair.sprite==Resources.Load<Sprite>("Art/Props/CadeiraPlasticaBar")&&chair.bounds.size.x>.6f,"Cadeira da calcada deixa escala miniatura e compartilha arte do interior");yield return Capture("calcada-escala");
 }
 private IEnumerator SinuquinhaPhysicalHand(SinucaMinigameUI ui)
 {
  report.Add("Mão completa: decisões da IA para ambos os lados, física fixa real; não há injeção de pontuação.");
  var balls=Field<System.Collections.IList>(ui,"bolas");int shots=0;
  while(!Field<SinuquinhaRules>(ui,"regrasBrasileiras").Finished&&shots<180)
  {
   Invoke(ui,"IniciarTurnoAtual");ui.StopAllCoroutines();Field<GameObject>(ui,"painelModal").SetActive(false);
   if(Field<bool>(ui,"brancaNaMao")){balls[0].GetType().GetField("pos").SetValue(balls[0],ui.GetType().GetMethod("EscolherPosicaoBrancaZe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(ui,null));Set(ui,"brancaNaMao",false);}
   object[] choice={Vector2.zero,0f};ui.GetType().GetMethod("EscolherTacadaZe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(ui,choice);
   Invoke(ui,"ExecutarTacadaComEfeito",(Vector2)choice[0],(float)choice[1],SinucaCueSpin.Normal);
   int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<4000)Invoke(ui,"SimularFisicaMesa",1f/120);
   ui.StopAllCoroutines();Check(steps<4000&&ui.ValidarInvarianteSeteBolas(),"IA e fisica de quinze bolas coerentes na tacada "+(++shots));if(shots%5==0)yield return null;
  }
  Check(Field<SinuquinhaRules>(ui,"regrasBrasileiras").Finished,"Mao completa com quinze bolas por IA/fisica: "+shots+" tacadas");yield return Capture("fim-mao-fisica");
 }
 private IEnumerator SinuquinhaSeriesFixture(SinucaMinigameUI ui)
 {
  report.Add("Fixture separada de série: grupos já limpos e preta perto da caçapa. Exercita duas conversões físicas e callback, não campanha legítima.");
  bool victory=false;ui.IniciarDuelo(()=>victory=true);
  for(int hand=1;hand<=2;hand++)
  {
   ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",hand);Field<GameObject>(ui,"painelModal").SetActive(false);var r=Field<SinuquinhaRules>(ui,"regrasBrasileiras");
   r.GetType().GetProperty("Opening").SetValue(r,false);r.GetType().GetProperty("MarcusTurn").SetValue(r,true);r.GetType().GetProperty("MarcusGroup").SetValue(r,1);
   var balls=Field<System.Collections.IList>(ui,"bolas");for(int n=1;n<16;n++){r.Present[n]=n==8;balls[n].GetType().GetField("ativa").SetValue(balls[n],n==8);}
   balls[0].GetType().GetField("pos").SetValue(balls[0],new Vector2(0,55));balls[8].GetType().GetField("pos").SetValue(balls[8],new Vector2(0,85));Set(ui,"turnoDeMarcus",true);Set(ui,"brancaNaMao",false);Set(ui,"cantadaManual",8);
   Invoke(ui,"ExecutarTacadaComEfeito",Vector2.up,120f,SinucaCueSpin.Normal);int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<4000)Invoke(ui,"SimularFisicaMesa",1f/120);ui.StopAllCoroutines();Check(steps<4000&&Field<int>(ui,"maosVencidasMarcus")==hand,"Preta fisica fecha a mao "+hand);yield return null;
  }
  Check(Field<object>(ui,"estado").ToString()=="FimDeSerieVitoria"&&!victory,"Serie aguarda ouvir Ze antes de concluir");yield return Capture("vitoria-serie");yield return ClickR4(Field<Button>(ui,"btnModalPrimario"));Check(victory&&!SinucaMinigameUI.IsShowing,"Ouvir Ze conclui callback da nova regra");
 }

}
#endif
