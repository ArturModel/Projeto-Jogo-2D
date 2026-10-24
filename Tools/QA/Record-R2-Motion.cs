// Corpo para unity-mcp code execute -f. Executar em Play no menu, sem outra suíte ativa no Editor.
// Fixture declarada; captura a câmera de gameplay em tempo real, sem fabricar quadros de animação.
if (!UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Abra Play no menu antes de gravar.");
UnityEngine.Application.runInBackground=true;
var isolation=CheckpointSystem.BeginIsolatedTests();
GameState.Limpar();
foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida})GameState.Marcar(flag);
var folder=System.IO.Path.GetFullPath(UnityEngine.Application.dataPath+"/../../Docs/Evidencias/C2R2/MovimentoEditor");System.IO.Directory.CreateDirectory(folder);
SceneTransition.Ir("SociedadeBeiraRio","centro");
int stage=0,frame=0;float last=0;bool positioned=false;
var evidence=new System.Text.StringBuilder("Fixture isolada no Editor; câmera de gameplay, 15 capturas/s, sem faixa de áudio.\nsegmento,quadro,tempo,x,y,pose,lanterna\n");
UnityEditor.EditorApplication.CallbackFunction record=null;
record=()=>
{
    try
    {
        if(!UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Play encerrado durante a gravação.");
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!="SociedadeBeiraRio"||SceneFader.IsTransitioning)return;
        var player=UnityEngine.Object.FindAnyObjectByType<PlayerController>();if(player==null)return;
        if(!positioned)
        {
            positioned=true;
            player.TeleportarPara(stage==0?new UnityEngine.Vector2(5,11):stage==1?new UnityEngine.Vector2(5,4):new UnityEngine.Vector2(18,4),1);
            GameState.DefinirNumero("equipamento.lanterna_ligada",stage>=2?1:0);
            UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());
        }
        float time=UnityEngine.Time.realtimeSinceStartup;if(time-last<1f/15)return;last=time;
        string segment=stage==0?"tonao":stage==1?"nair":"marcus-dir"+(stage-2);
        if(stage>=2)
        {
            var keys=new[]{UnityEngine.InputSystem.Key.W,UnityEngine.InputSystem.Key.S,UnityEngine.InputSystem.Key.A,UnityEngine.InputSystem.Key.D};
            if(frame==0)UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState(keys[stage-2]));
            if(frame==18)UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());
            // Alternâncias reais do input F depois de parar; não escrever a preferência para fabricar o quadro.
            if(frame==24||frame==30)UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.F));
            if(frame==26||frame==32)UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());
        }
        UnityEngine.ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folder,segment+"-"+frame.ToString("D3")+".png"));
        evidence.AppendLine(segment+","+frame+","+time.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+","+player.transform.position.x.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+","+player.transform.position.y.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+","+player.QuadroCaminhada+","+player.LanternaVisivel);
        frame++;
        if(frame>=(stage==0?240:stage==1?150:36)){stage++;frame=0;positioned=false;}
        if(stage==6)
        {
            UnityEditor.EditorApplication.update-=record;
            UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());
            System.IO.File.WriteAllText(System.IO.Path.Combine(folder,"captura.csv"),evidence.ToString());isolation.Dispose();
            UnityEditor.EditorApplication.isPlaying=false;
        }
    }
    catch(System.Exception error)
    {
        UnityEditor.EditorApplication.update-=record;isolation.Dispose();System.IO.File.WriteAllText(System.IO.Path.Combine(folder,"erro.txt"),error.ToString());UnityEditor.EditorApplication.isPlaying=false;
    }
};
UnityEditor.EditorApplication.update+=record;
return "Captura real agendada: Tonão, Nair, quatro direções com F e repouso. "+folder;
