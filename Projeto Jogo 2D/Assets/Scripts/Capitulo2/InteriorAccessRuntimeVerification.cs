#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public sealed partial class Chapter2RuntimeVerification
{
    private bool accessForce;
    private bool accessStairsOnly;
#if UNITY_EDITOR
    public static void StartAccessInEditor(string output,bool stairsOnly=false)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);
        var host=new GameObject("QA_Acessos_Explicito");DontDestroyOnLoad(host);var qa=host.AddComponent<Chapter2RuntimeVerification>();qa.accessForce=true;qa.accessStairsOnly=stairsOnly;
    }
#endif
    private IEnumerator WalkAccess(Key direction,float seconds)
    {
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(direction));yield return new WaitForSecondsRealtime(seconds);
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.18f);
    }
    private IEnumerator WalkAccessFilm(Key direction,float seconds,string prefix,float? endY=null)
    {
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(direction));float start=Time.realtimeSinceStartup;int frame=0;
        while(Time.realtimeSinceStartup-start<seconds)
        {
            var p=FindAnyObjectByType<PlayerController>();
            if(endY.HasValue&&(direction==Key.W?p.transform.position.y>=endY.Value:p.transform.position.y<=endY.Value))break;
            yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,prefix+"-"+frame.ToString("000")+".png"));
            File.AppendAllText(Path.Combine(folder,"captura.csv"),prefix+","+frame+","+(Time.realtimeSinceStartup-start).ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+","+p.transform.position.y.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\n");frame++;
            yield return new WaitForSecondsRealtime(.09f);
        }
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.18f);
    }
    private IEnumerator RunAccesses()
    {
        report.Add("Acessos: fixture de progressão em RAM; posições de aproximação declaradas; E e movimento pelo Input System real. Nenhum save pessoal.");
        Application.targetFrameRate=60;QualitySettings.vSyncCount=0;
        foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida,Flags.ChegouCasaAvo})GameState.Marcar(flag);
        if(!accessStairsOnly)
        {
        foreach(string room in new[]{"SociedadeBeiraRio","IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira","AbrigoCemiterio"})
        {
            SceneFader.Instance.LoadScene(room);yield return AwaitScene(room);yield return CloseText();
            var portal=FindObjectsByType<Capitulo2Portal>().First(p=>p.name=="Porta_Saida");var target=portal.cenaDestino;
            var marks=FindObjectsByType<SantaRitaExitThreshold>();
            Check(marks.Length==(room=="MercadinhoDeposito"?2:1)&&marks.All(m=>m.GetComponent<SpriteRenderer>().sprite.name.StartsWith("Saida")&&m.GetComponent<Collider2D>()==null),room+": soleiras de chão sem obstáculo nem DoorFrame.");
            Check(!portal.automatico&&portal.Prompt=="Sair",room+": saída deliberada, sem revelar destino pelo prompt.");
            yield return Place((Vector2)portal.transform.position+Vector2.up*.45f);yield return new WaitForSecondsRealtime(.8f);
            Check(SceneManager.GetActiveScene().name==room,room+": aproximação não dispara transição involuntária.");
            Check(Interactable.FindNearest(FindAnyObjectByType<PlayerController>().transform.position)==portal,room+": E alcança a saída.");
            yield return Capture("saida-"+room);yield return TapR2(Key.E);yield return AwaitScene(target);
            Check(SceneManager.GetActiveScene().name==target,room+": E preserva destino e retorno.");
        }
        SceneFader.Instance.LoadScene("MercadinhoDeposito");yield return AwaitScene("MercadinhoDeposito");yield return CloseText();
        var service=FindObjectsByType<Capitulo2Portal>().Single(p=>p.cenaDestino=="EntradaDaCidade");
        yield return Place(service.transform.position+Vector3.up*.25f);yield return Capture("saida-servico-deposito");yield return TapR2(Key.E);yield return AwaitScene("EntradaDaCidade");yield return CloseText();
        Check(Vector2.Distance(FindAnyObjectByType<PlayerController>().transform.position,new Vector2(316.1f,-51.5f))<.8f,"Depósito preserva a porta de serviço e o ponto de retorno à loja C1.");
        var zones=new[]{Capitulo1Director.ZonaId.Interior_Posto,Capitulo1Director.ZonaId.Interior_Mercadinho,Capitulo1Director.ZonaId.Interior_CasaDoAvo,Capitulo1Director.ZonaId.Interior_BarDoZe};
        var outside=new[]{Capitulo1Director.ZonaId.Zona2_Posto,Capitulo1Director.ZonaId.Zona3_Encruzilhada,Capitulo1Director.ZonaId.Zona4_Bairro,Capitulo1Director.ZonaId.Zona5_RuaNorteBar};
        for(int i=0;i<zones.Length;i++)
        {
            float x=new[]{208f,309,409,509}[i];Capitulo1Director.Instance.TransicionarPara(zones[i],new Vector2(x,-57.8f),0);
            yield return new WaitForSecondsRealtime(1.7f);yield return CloseText();
            var portal=FindObjectsByType<TransicaoDeZonaTrigger>().First(t=>t.name==new[]{"Saida_Interior_Posto","Saida_Interior_Mercadinho","Saida_Interior_CasaAvo","Saida_Interior_BarDoZe"}[i]);
            Check(FindObjectsByType<SantaRitaExitThreshold>().Length>=1&&portal.Prompt=="Sair",zones[i]+": capacho nativo e saída contextual.");
            yield return Place(portal.transform.position);yield return Capture("saida-C1-"+i);yield return TapR2(Key.E);yield return new WaitForSecondsRealtime(1.8f);
            Check(Capitulo1Director.ZonaAtual==outside[i],zones[i]+": E devolve à região original.");
        }
        }
        SceneFader.Instance.LoadScene("SociedadeBeiraRio");yield return AwaitScene("SociedadeBeiraRio");yield return CloseText();
        var actor=FindAnyObjectByType<PlayerController>();var stair=FindAnyObjectByType<SantaRitaShallowStairs>();
        Check(stair!=null&&stair.GetComponent<Capitulo2Portal>()==null&&GameObject.Find("PortalAbertoDaCancha")==null,"Cancha: escada aberta de três degraus na mesma cena, sem porta.");
        Check(SantaRitaShallowStairs.SpeedAt(new Vector2(4.7f,7.3f))<1&&SantaRitaShallowStairs.SpeedAt(new Vector2(7,4))==1,"Cadência reduz apenas na escada; passada segue deslocamento real.");
        var cut=FindAnyObjectByType<SantaRitaRoomCutaway>();var elevation=actor.GetComponent<SantaRitaActorElevation>();
        yield return Place(new Vector2(13,4.7f));yield return new WaitForSecondsRealtime(.65f);yield return Capture("parede-distante");
        Check(cut.Opacity>.99f,"Longe da escada, parede e cobertura da sala permanecem visíveis.");
        yield return Place(new Vector2(4.7f,6.0f));yield return Capture("escada-antes");yield return WalkAccessFilm(Key.W,4,"escada-descida",9.35f);
        Check(elevation!=null&&elevation.VisualDepth>.42f,"Desnível de três espelhos projeta Marcus 15 pixels abaixo, sem trocar quadros da caminhada.");
        Check(elevation.VisualScale<.92f&&actor.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit>35,"Marcus reduz 10% de escala ao descer, sem encolher colisões do cenário.");
        Check(cut.Opacity<.01f&&cut.walls.All(w=>w.color.a<.01f),"Aproximação abre visão apenas da cancha; colisões permanecem.");
        Check(FindObjectsByType<BoxCollider2D>().Where(c=>c.name=="ParedeDivisoriaCancha").All(c=>c.enabled),"Recorte visual não remove as paredes físicas.");
        Check(actor.transform.position.y>9&&SceneManager.GetActiveScene().name=="SociedadeBeiraRio","Desce a escada por movimento, sem corte de cena.");yield return Capture("escada-cabeceira");
        yield return WalkAccessFilm(Key.S,4,"escada-subida",5.95f);Check(actor.transform.position.y<6.3f,"Sobe de volta ao salão sem bloqueio. y="+actor.transform.position.y);yield return Capture("escada-retorno");
        yield return new WaitForSecondsRealtime(.6f);Check(elevation.VisualDepth<.01f,"Ao subir, corpo e colisão retornam ao nível do salão.");
        var ends=FindObjectsByType<BoxCollider2D>().Where(c=>c.name=="CabeceiraDeMadeira").OrderBy(c=>c.transform.position.x).ToArray();
        Check(ends.Length==2&&Mathf.Abs(ends[1].bounds.min.x-ends[0].bounds.max.x-19)<.01f,"Cancha explorável reduzida de 22 para 19 de comprimento; partida preservada.");
        yield return Place(new Vector2(17,16));yield return Capture("cancha-compacta");
        Check(FindObjectsByType<SpriteRenderer>().Count(r=>r.name=="BancoCanchaSul")==4&&FindObjectsByType<SpriteRenderer>().Count(r=>r.name=="BancoCanchaNorte")==4,"Bancos nas duas laterais, correspondentes à sala da partida.");
        Check(!CheckpointSystem.HasCheckpoint,"Revisão de acessos e escada não cria save automático.");
        if(Application.isEditor){Finish();yield break;}
        yield return RunBochaPerspective();
    }
}
#endif
