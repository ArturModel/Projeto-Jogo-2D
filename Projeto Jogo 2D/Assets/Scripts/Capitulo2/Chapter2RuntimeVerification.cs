#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Somente Editor/Development e somente com argumento explícito. Não é entrada
// da campanha nem atalho de jogador. Saves possuem namespace temporário próprio.
public sealed partial class Chapter2RuntimeVerification : MonoBehaviour
{
    private readonly List<string> report=new List<string>();
    private IDisposable isolation;
    private string folder;
    private float deadline;
    private bool failed;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if(!Environment.GetCommandLineArgs().Contains("-srPortalQA")&&!Environment.GetCommandLineArgs().Contains("-srAccessQA")&&!Environment.GetCommandLineArgs().Contains("-srC2QA")&&!Environment.GetCommandLineArgs().Contains("-srC1LoadSmoke")&&!Environment.GetCommandLineArgs().Contains("-srC2PerfQA")&&!Environment.GetCommandLineArgs().Contains("-srSaveIsolationQA")&&!Environment.GetCommandLineArgs().Contains("-srC2R2QA")&&!Environment.GetCommandLineArgs().Contains("-srBochaQA"))return;
        var g=new GameObject("C2_Runtime_QA_Explicit");DontDestroyOnLoad(g);g.AddComponent<Chapter2RuntimeVerification>();
    }
    private void Start()
    {
        folder=Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR")??Path.Combine(Application.persistentDataPath,"C2_QA");Directory.CreateDirectory(folder);
        Application.runInBackground=true;deadline=Time.realtimeSinceStartup+1100;
        Application.logMessageReceived+=Error;
        isolation=CheckpointSystem.BeginIsolatedTests();
        File.WriteAllText(Path.Combine(folder,"runtime-progresso.txt"),"Início QA "+DateTime.UtcNow.ToString("O")+"\n");
        report.Add("QA automatizado em Development Build. Fixture C1 criada em RAM; não comprova T01/percurso legítimo. Nenhum slot pessoal utilizado.");
        StartCoroutine(Guard(portalForce||Environment.GetCommandLineArgs().Contains("-srPortalQA")?RunPortalRegression():accessForce||Environment.GetCommandLineArgs().Contains("-srAccessQA")?RunAccesses():bochaForce||Environment.GetCommandLineArgs().Contains("-srBochaQA")?RunBochaPerspective():r2Force||Environment.GetCommandLineArgs().Contains("-srC2R2QA")?RunR2():Environment.GetCommandLineArgs().Contains("-srSaveIsolationQA")?SaveIsolationProbe():Environment.GetCommandLineArgs().Contains("-srC1LoadSmoke")?C1LoadSmoke():Environment.GetCommandLineArgs().Contains("-srC2PerfQA")?PerformancePair():Run()));
    }
    private void Error(string msg,string stack,LogType type)
    {if(type==LogType.Exception||type==LogType.Error){failed=true;report.Add("ERROR "+msg+"\n"+stack);}}
    private void Update(){if(Time.realtimeSinceStartup>deadline){failed=true;Finish();}}
    private IEnumerator Guard(IEnumerator routine)
    {
        while(true)
        {
            bool more=false;object value=null;
            try{more=routine.MoveNext();if(more)value=routine.Current;}
            catch(Exception e){failed=true;report.Add(e.ToString());Finish();yield break;}
            if(!more)break;
            if(value is IEnumerator nested)yield return Guard(nested);else yield return value;
            if(failed){Finish();yield break;}
        }
    }
    private void Check(bool ok,string text){report.Add((ok?"PASS ":"FAIL ")+text);File.AppendAllText(Path.Combine(folder,"runtime-progresso.txt"),(ok?"PASS ":"FAIL ")+text+"\n");if(!ok)throw new Exception(text);}
    private IEnumerator SaveIsolationProbe()
    {
        yield return null;
        Check(!CheckpointSystem.HasCheckpoint,"Namespace novo não lê os slots pessoais.");
        CheckpointSystem.SaveSlot(4,"EntradaDaCidade",Vector2.zero,"qa_limpeza");
        Check(CheckpointSystem.TryLoadSlot(4,out _),"Save temporário realmente persistido antes de limpar.");
        Finish(); // A verificação externa confere os valores SRQA após encerrar o processo.
    }
    private IEnumerator C1LoadSmoke()
    {
        yield return new WaitForSecondsRealtime(2);SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");
        Check(FindAnyObjectByType<PlayerController>()!=null,"Cena C1 compilada carrega e cria Marcus.");
        yield return CloseText();yield return Capture("smoke-c1");yield return Performance("C1 início");
        yield return C1InvestigationRegression();
        yield return SinucaPauseRegression();
        var service=FindObjectsByType<Capitulo2Portal>().First(p=>p.cenaDestino=="MercadinhoDeposito");
        yield return Place(service.transform.position+Vector3.down*.6f);service.Interact();yield return CloseText();
        Check(SceneManager.GetActiveScene().name=="EntradaDaCidade","Porta de serviço não contorna bloqueio pré-Zé.");
        // Fixture explícita para exercitar portas/revisitas; não é save legítimo.
        foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida,Flags.ChegouCasaAvo,"c2_iniciado","c2_registro_manutencao",Capitulo2Director.Vitoria,Capitulo2Director.Ficha})GameState.Marcar(flag);
        yield return Travel("MercadinhoDeposito");yield return Travel("EntradaDaCidade");
        Check(Vector2.Distance(FindAnyObjectByType<PlayerController>().transform.position,new Vector2(316.1f,-51.5f))<.8f,"Porta de serviço retorna ao interior da loja original.");
        foreach(var zone in new[]{Capitulo1Director.ZonaId.Interior_Posto,Capitulo1Director.ZonaId.Interior_Mercadinho,Capitulo1Director.ZonaId.Interior_CasaDoAvo,Capitulo1Director.ZonaId.Interior_BarDoZe})
        {
            Vector2 center=zone==Capitulo1Director.ZonaId.Interior_Posto?new Vector2(208,-56):zone==Capitulo1Director.ZonaId.Interior_Mercadinho?new Vector2(309,-56):zone==Capitulo1Director.ZonaId.Interior_CasaDoAvo?new Vector2(409,-55):new Vector2(509,-56.8f);
            Capitulo1Director.Instance.TransicionarPara(zone,center,1);yield return new WaitForSecondsRealtime(2);yield return CloseText();
            Check(Capitulo1Director.ZonaAtual==zone,"Composição C1 ativa: "+zone);yield return Capture("c1-interior-"+zone);
        }
        foreach(var type in new[]{InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho,InteracaoCapitulo1.TipoInteracao.NpcZeBar})
        {
            var it=FindObjectsByType<InteracaoCapitulo1>().First(i=>i.Tipo==type);
            Capitulo1Director.Instance.TransicionarPara(type==InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho?Capitulo1Director.ZonaId.Interior_Mercadinho:Capitulo1Director.ZonaId.Interior_BarDoZe,it.transform.position+Vector3.down*.9f,1);
            yield return new WaitForSecondsRealtime(2);it.Interact();yield return Capture("smoke-revisita-"+type);yield return CloseText();
            Check(GameState.ObterObjetivo().Contains("Amaro"),"Revisita C1 conserva objetivo de C2: "+type);
        }
        var gate=FindObjectsByType<InteracaoCapitulo1>().First(i=>i.Tipo==InteracaoCapitulo1.TipoInteracao.LimiarCentro);
        yield return Place(gate.transform.position+Vector3.down*.5f);gate.Interact();yield return CloseText();yield return AwaitScene("CentroSantaRita");
        Check(GameState.Tem(Capitulo2Director.Ficha),"Limiar aprovado de C1 leva ao Centro mantendo consequência.");
        yield return Travel("RodoviariaSantaRita");yield return Place(new Vector2(18,10.6f));
        yield return Capture("revisao-guiche-aberto");yield return Use("dario");yield return ExitToCenter();
        yield return Travel("CemiterioSantaRita");yield return Travel("AbrigoCemiterio");yield return Place(new Vector2(8,7));
        yield return Capture("revisao-caderno-abrigo");yield return Use("caderno_velas");Finish();
    }
    private IEnumerator PerformancePair()
    {
        yield return new WaitForSecondsRealtime(2);
        foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida})GameState.Marcar(flag);
        SceneFader.Instance.LoadScene("EntradaDaCidade");yield return AwaitScene("EntradaDaCidade");yield return CloseText();yield return Place(new Vector2(525,12));
        yield return Performance("C1 Rua Norte / comparação controlada");
        SceneTransition.Ir("CentroSantaRita","c1_portao_norte");yield return AwaitScene("CentroSantaRita");
        yield return Place(new Vector2(44,48));yield return Performance("C2 Praça / comparação controlada");Finish();
    }
    private IEnumerator Run()
    {
        yield return new WaitForSecondsRealtime(2);
        QualitySettings.vSyncCount=0;Application.targetFrameRate=60;
        yield return Capture("00-menu");
        SinucaRegression();
        CheckpointSystem.SaveSlot(4,"EntradaDaCidade",new Vector2(525,12),"orelhao_bar_do_ze");
        CheckpointSystem.RequestSpawnAtSlot(4);CheckpointSystem.ClearPendingOnly();
        string before=SceneManager.GetActiveScene().name;Capitulo1Director.MostrarTelaFechamentoCapitulo1();yield return new WaitForSecondsRealtime(.5f);
        Check(SceneManager.GetActiveScene().name==before,"Save pré-Zé não abre conexão do Centro.");
        foreach(string flag in new[]{Flags.ChegouCasaAvo,Flags.ExaminouCafeCasa,Flags.ExaminouCartasCasa,Flags.ExaminouDesordemCasa,
            Flags.AchouReciboMercadinho,Flags.FalouAtendenteMercadinho,Flags.Pista_CasaAvoCompleta})GameState.Marcar(flag);
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.RecebeuFichaClubeZe);
        GameState.Marcar(Flags.MapaDoCarroObtido); GameState.Marcar(Flags.LanternaDoCarroObtida);
        CheckpointSystem.SaveSlot(0,"EntradaDaCidade",new Vector2(525,12),"orelhao_bar_do_ze");
        Capitulo1Director.MostrarTelaFechamentoCapitulo1();
        yield return AwaitScene("CentroSantaRita");
        Check(GameState.Tem(Flags.RecebeuFichaClubeZe)&&GameState.Tem("c2_iniciado"),"Transição C1 → C2 preserva ficha e inicia capítulo.");
        yield return PlanCapture();
        yield return Walking();
        yield return Performance("C2 Centro");
        yield return Place(new Vector2(44,48));yield return Capture("01-praca");
        yield return Use("avisos_praca");
        yield return Travel("IgrejaMatriz");yield return Place(new Vector2(11,13));yield return Capture("02-igreja");
        yield return Use("padre",true);yield return ExitToCenter();
        yield return Travel("CemiterioSantaRita");yield return Place(new Vector2(60,96));yield return Use("memorial",true);
        Check(GameState.Tem("c2_cemiterio_comparado"),"Cemitério antes da Prefeitura: comparação independente da ordem.");
        yield return Capture("03-cemiterio");
        CheckpointSystem.SaveSlot(3,"CentroSantaRita",new Vector2(60,96),"qa_save_cemiterio_antigo");
        Check(CheckpointSystem.CarregarSlot(3),"Save antigo do Cemitério aceita leitura sem regravar arquivo.");yield return new WaitForSecondsRealtime(3);yield return AwaitScene("CemiterioSantaRita");
        Check(Vector2.Distance(FindAnyObjectByType<PlayerController>().transform.position,new Vector2(60,96))<1,"Posição antiga é transferida à cena independente do Cemitério.");
        yield return Travel("AbrigoCemiterio");yield return Place(new Vector2(8,7));yield return Capture("04-abrigo");yield return Use("caderno_velas");yield return Travel("CemiterioSantaRita");yield return ExitToCenter();
        yield return Travel("SociedadeBeiraRio");yield return Use("tonao");yield return Use("bocha");
        Check(!BochaMinigameUI.IsShowing,"Tonão cedo fornece requisitos, sem liberar duelo.");yield return ExitToCenter();
        yield return Travel("PrefeituraSantaRita");yield return Use("prefeitura",true);yield return Use("protocolo",true);yield return Use("manutencao",true);yield return Use("placa_praca");
        yield return Place(new Vector2(12,9));yield return Capture("05-prefeitura");yield return ExitToCenter();
        Check(Capitulo2Director.ProntoParaBocha,"Caminho crítico pronto sem rádio/costura ou outras linhas opcionais.");
        yield return Travel("RodoviariaSantaRita");yield return Place(new Vector2(13,10));yield return Capture("06-rodoviaria");yield return Use("telefone_rodo");
        yield return Use("rodo_passageira");yield return Use("rodo_motorista");yield return Use("horarios");yield return Use("rodo_motorista");yield return Use("rodo_passageira");
        Check(GameState.Tem("c2_rodo_passageira_avisada"),"Pequena história da Rodoviária conclui por observação, conversa e retorno.");yield return ExitToCenter();
        yield return Travel("MercadinhoDeposito");yield return Place(new Vector2(10,9));yield return Capture("07-deposito");yield return Use("romaneio");yield return ExitToCenter();
        yield return Travel("OficinaRadio");yield return Place(new Vector2(9,8));yield return Capture("08-radio");yield return Use("mila");yield return Use("desenho_radio",true);yield return Use("radio_test");yield return Use("radio_test");
        Check(GameState.Tem("c2_radio_restaurado"),"Linha opcional rádio tem conclusão.");yield return Use("mila");
        Check(GameState.Tem("c2_radio_historia_concluida"),"Retorno à Mila leva em conta desenho e teste, com memória de Júlia em vida.");yield return ExitToCenter();
        yield return Travel("CasaCostureira");yield return Place(new Vector2(9,9));yield return Capture("09-costura");yield return Use("celina");
        Check(!GameState.Tem("c2_costura_reinterpretada"),"Celina não sabe o testemunho só porque Marcus o ouviu.");
        yield return Use("encomenda_faixa");yield return Use("celina");
        Check(GameState.Tem("c2_costura_reinterpretada"),"Marcus compartilha testemunho; Celina distingue a encomenda de um relato e anota a cópia de consulta.");yield return ExitToCenter();
        yield return Place(new Vector2(5,2.7f));yield return Capture("10-beira-rio");
        yield return Travel("SociedadeBeiraRio");yield return Place(new Vector2(17,7));yield return Capture("11-sociedade-cancha");
        yield return Place(new Vector2(26,16));yield return Capture("11-cancha-cabeceira");
        yield return Place(new Vector2(7,7.5f));yield return Capture("11-cancha-acesso-salao");
        yield return Use("nair");yield return Use("nair");Check(GameState.Tem("c2_nair_faixa_conversada"),"Consequência da costura chega à Nair somente por Marcus.");
        yield return Use("bocha");Check(BochaMinigameUI.IsShowing,"Diálogo inicia UI real da Bocha.");
        Click("DUELAR ATÉ 6 PONTOS");yield return null;Click("LER A CANCHA");yield return null;
        yield return Capture("12-bocha-primeira-mao");
        var ui=BochaMinigameUI.Instance;
        Invoke(ui,"Pausar");var sim=Field<BochaSimulation>(ui,"sim");var ball=sim.bolas[0].pos;
        yield return new WaitForSecondsRealtime(.5f);Check(sim.bolas[0].pos==ball,"Modal de pausa mantém física parada.");Click("CONTINUAR");
        int stock=Field<BochaMatch>(ui,"partida").usadas[0];
        Screen.SetResolution(Screen.width==1280?1024:1280,Screen.height==720?576:720,FullScreenMode.Windowed);yield return new WaitForSecondsRealtime(.5f);
        Check(ActiveButton("CONTINUAR")&&Field<BochaMatch>(ui,"partida").usadas[0]==stock&&sim.bolas[0].pos==ball,"Troca de resolução pausa sem gastar bocha ou mover balim.");
        if(ActiveButton("CONTINUAR"))Click("CONTINUAR");
        Set(ui,"fase",Enum.Parse(Field<object>(ui,"fase").GetType(),"Carregando"));Invoke(ui,"OnApplicationFocus",false);
        Check(ActiveButton("CONTINUAR")&&Field<object>(ui,"fase").ToString()=="Mira"&&Field<BochaMatch>(ui,"partida").usadas[0]==stock,"Perda de foco cancela carga e pausa sem consumir estoque.");Click("CONTINUAR");
        // QA usa lançamentos válidos pelo mesmo método de sessão, acelerados 6x.
        // Não posiciona bolas nem altera placar. IA real joga pelo Update.
        Time.timeScale=6;int shots=0;
        float end=Time.realtimeSinceStartup+300;
        while(BochaMinigameUI.IsShowing&&!GameState.Tem(Capitulo2Director.Vitoria)&&Time.realtimeSinceStartup<end)
        {
            string phase=Field<object>(ui,"fase").ToString();
            if(ActiveButton("OUVIR TONÃO")){Click("OUVIR TONÃO");break;}
            if(ActiveButton("PRÓXIMA MÃO")){Click("PRÓXIMA MÃO");yield return null;}
            else if(ActiveButton("REPETIR A MÃO")){Click("REPETIR A MÃO");yield return null;}
            else if(phase=="Mira")
            {
                sim=Field<BochaSimulation>(ui,"sim");var shot=BestShot(sim);
                Invoke(ui,"Lancar",BochaSide.Marcus,shot.x,shot.velocidade);shots++;
                if(shots==5){Time.timeScale=1;yield return Capture("13-bocha-piso-marcas");Time.timeScale=6;}
            }
            if(ActiveButton("ESCOLHER SAVE"))throw new Exception("QA perdeu duelo; ajustar análise, não placar.");
            yield return null;
        }
        Time.timeScale=1;
        Check(GameState.Tem(Capitulo2Director.Vitoria),"Vitória real da sessão após lançamentos/IA; "+shots+" lançamentos Marcus.");
        yield return Capture("14-tonao-consequencia");yield return CloseText();
        Check(GameState.Tem(Capitulo2Director.Ficha),"Segunda ficha concedida pela consequência.");
        int notes=GameState.ExportarSnapshot().diario.Count;yield return Use("bocha");
        Check(GameState.ExportarSnapshot().diario.Count==notes,"Revisita da Bocha não duplica recompensa/diário.");
        yield return ExitToCenter();
        var post=GameState.ExportarSnapshot();CheckpointSystem.RequestSpawnAtSlot(0);
        Check(!GameState.Tem(Capitulo2Director.Vitoria),"Vitória sem orelhão descartada ao carregar fixture anterior.");
        CheckpointSystem.ClearPendingOnly();GameState.ImportarSnapshot(post);
        yield return Place(new Vector2(12.7f,3.7f));
        var phone=FindObjectsByType<PayphoneCheckpoint>().First(p=>p.PayphoneId=="orelhao_bocha_beira_rio");phone.Interact();yield return null;
        PayphoneCheckpoint.AoClicarSlot(1);yield return CloseText();
        Check(CheckpointSystem.TryLoadSlot(1,out var saved)&&saved.estado.flags.Contains(Capitulo2Director.Ficha),"Orelhão real salva segunda ficha no slot isolado.");
        Check(CheckpointSystem.CarregarSlot(1),"Recarga pelo fluxo público do slot manual.");
        yield return new WaitForSecondsRealtime(3);yield return AwaitScene("CentroSantaRita");
        Check(GameState.Tem(Capitulo2Director.Ficha),"Recarga manual preserva consequência.");
        Check(Vector2.Distance(FindAnyObjectByType<PlayerController>().transform.position,new Vector2(saved.posX,saved.posY))<1,"Recarga restaura posição do orelhão na cena correta.");
        yield return Use("epilogo",true);yield return Capture("15-conclusao");
        MapaInterativoUI.Instance.Abrir(1);yield return null;yield return Capture("16-diario");MapaInterativoUI.Instance.Fechar();
        MapaInterativoUI.Instance.Abrir(3);yield return null;yield return Capture("17-legenda");MapaInterativoUI.Instance.Fechar();
        yield return Travel("EntradaDaCidade");
        Check(GameState.Tem(Capitulo2Director.Ficha)&&GameState.Tem(Flags.RecebeuFichaClubeZe),"Volta à Rua Norte preserva as duas fichas.");
        Check(FindObjectsByType<Chuva>().Length==1,"Retorno a C1 mantém uma instância de chuva.");
        yield return Capture("18-retorno-c1");yield return Performance("C1 Rua Norte (mesma build)");
        Capitulo1Director.MostrarTelaFechamentoCapitulo1();yield return AwaitScene("CentroSantaRita");
        yield return Travel("MercadinhoDeposito");yield return Travel("EntradaDaCidade");
        Check(Vector2.Distance(FindAnyObjectByType<PlayerController>().transform.position,new Vector2(316.1f,-51.5f))<1,"Segundo caminho retorna à loja de C1 pelo Mercadinho.");
        yield return Travel("MercadinhoDeposito");yield return ExitToCenter();
        Check(GameState.Tem(Capitulo2Director.Ficha)&&GameState.Tem(Flags.RecebeuFichaClubeZe),"Ida e volta pelos dois acessos conserva o mundo e as fichas.");
        yield return DefeatUI();
        report.Add("Capturas automáticas da build observam apresentação; não avaliam diversão, audição humana ou T01.");Finish();
    }

    private IEnumerator Walking()
    {
        yield return Place(new Vector2(31,45));yield return CloseText();
        var player=FindAnyObjectByType<PlayerController>();var keyboard=Keyboard.current;
        Vector2 initial=player.transform.position;var contacts=new List<int>();
        Action contact=()=>contacts.Add(player.QuadroCaminhada);player.PeApoiou+=contact;
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));
            for(int i=0;i<16;i++)
            {
                yield return new WaitForSecondsRealtime(.08f);
                ScreenCapture.CaptureScreenshot(Path.Combine(folder,"walk-"+i.ToString("D2")+".png"));
            }
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSecondsRealtime(.3f);
            Check(Vector2.Distance(initial,player.transform.position)>2,"Input real move Marcus com oito poses e interpolação.");
            Check(contacts.Count>=3 && contacts.All(q=>q==0||q==4),"Cada evento de passo coincide com quadro de apoio 0/4: "+string.Join(",",contacts));
            int count=contacts.Count;yield return new WaitForSecondsRealtime(.4f);
            Check(contacts.Count==count,"Repouso não dispara passos extras.");
        }
        finally {InputSystem.QueueStateEvent(keyboard,new KeyboardState());player.PeApoiou-=contact;}
    }
    private IEnumerator DefeatUI()
    {
        var ui=BochaMinigameUI.Instance;ui.Abrir(null);Click("DUELAR ATÉ 6 PONTOS");yield return null;Click("LER A CANCHA");
        Time.timeScale=6;float end=Time.realtimeSinceStartup+150;
        while(!ActiveButton("ESCOLHER SAVE")&&Time.realtimeSinceStartup<end)
        {
            if(ActiveButton("PRÓXIMA MÃO"))Click("PRÓXIMA MÃO");
            else if(ActiveButton("REPETIR A MÃO"))Click("REPETIR A MÃO");
            else if(Field<object>(ui,"fase").ToString()=="Mira")Invoke(ui,"Lancar",BochaSide.Marcus,2.3f,new Vector2(0,3.5f));
            yield return null;
        }
        Time.timeScale=1;
        Check(Field<BochaMatch>(ui,"partida").Vencedor==BochaSide.Tonao&&ActiveButton("ESCOLHER SAVE"),"Derrota real na UI oferece seleção explícita de save.");
        yield return Capture("19-derrota-com-save");
        // Clear() só cancela spawn pendente por contrato; não apaga saves.
        // Um novo escopo temporário vazio exercita o caso sem save pessoal.
        isolation.Dispose();isolation=CheckpointSystem.BeginIsolatedTests();Invoke(ui,"Derrota");yield return null;
        Check(!CheckpointSystem.HasCheckpoint&&!ActiveButton("ESCOLHER SAVE")&&ActiveButton("MENU PRINCIPAL"),"Derrota sem save oferece menu, sem inventar checkpoint.");
        yield return Capture("20-derrota-sem-save");ui.Fechar();
    }
    private IEnumerator Travel(string target)
    {
        var portal=FindObjectsByType<Capitulo2Portal>().First(p=>p.cenaDestino==target);
        yield return Place(portal.transform.position+Vector3.down*.6f);yield return new WaitForSecondsRealtime(1.6f);
        if(SceneManager.GetActiveScene().name!=target&&portal!=null)portal.Interact();yield return AwaitScene(target);
        Check(SceneManager.GetActiveScene().name==target,"Porta real para "+target);
        if (Capitulo2Director.Instance != null)
        {
            var map = MapaInterativoUI.EnsureInstance(); map.Abrir(0); yield return null;
            var player = FindAnyObjectByType<PlayerController>();
            Capitulo2Director.Instance.PosicaoNoMapa(player.transform.position, out Vector2 expected, out _);
            Check(MapaInterativoUI.IsShowing && Vector2.Distance(Field<RectTransform>(map,"playerMarkerRect").anchorMin, expected) < .0001f,
                "Mapa real marca Marcus na região explorada: " + target);
            map.Fechar();
        }
    }
    private IEnumerator ExitToCenter()=>Travel("CentroSantaRita");
    private IEnumerator AwaitScene(string name)
    {float end=Time.realtimeSinceStartup+20;while(SceneManager.GetActiveScene().name!=name&&Time.realtimeSinceStartup<end)yield return null;Check(SceneManager.GetActiveScene().name==name,"Cena carregada "+name);yield return new WaitForSecondsRealtime(1.7f);}
    private IEnumerator Place(Vector2 position)
    {var p=FindAnyObjectByType<PlayerController>();p.transform.position=position;var rb=p.GetComponent<Rigidbody2D>();rb.position=position;rb.linearVelocity=Vector2.zero;FindAnyObjectByType<CameraFollow>()?.SnapImediatoParaAlvo();yield return new WaitForSecondsRealtime(.3f);}
    private IEnumerator Use(string id,bool capture=false)
    {
        var item=FindObjectsByType<Capitulo2Interactable>().First(i=>i.interactionId==id);yield return Place(item.transform.position+Vector3.down*.7f);item.Interact();
        yield return new WaitForSecondsRealtime(.3f);
        if(capture)
        {
            if(Capitulo2Director.Dados(id).kind=="talk"){GameMessageUI.Instance.Avancar();yield return new WaitForSecondsRealtime(.06f);}
            yield return Capture("leitura-"+id);
            if(id=="manutencao")
            {
                var scroll=GameMessageUI.Instance.GetComponentsInChildren<ScrollRect>().First(s=>s.gameObject.name=="AreaLeitura");
                Check(scroll.content.rect.height>scroll.viewport.rect.height,"Documento longo possui conteúdo rolável, sem texto cortado no painel.");
                scroll.verticalNormalizedPosition=0;yield return null;yield return Capture("leitura-manutencao-final");
            }
        }
        yield return CloseText();
    }
    private IEnumerator CloseText(){int tries=0;while(GameMessageUI.IsShowing&&tries++<100){GameMessageUI.Instance.Avancar();yield return new WaitForSecondsRealtime(.06f);}Check(!GameMessageUI.IsShowing,"Leitura encerra e devolve controle.");}
    private IEnumerator Capture(string name){yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,name+".png"));yield return new WaitForSecondsRealtime(.15f);}
    private IEnumerator PlanCapture()
    {
        var camera=Camera.main;var pixel=camera.GetComponent<PixelPerfectCamera>();var follow=camera.GetComponent<CameraFollow>();
        var position=camera.transform.position;float ortho=camera.orthographicSize;Rect rect=camera.rect;
        var canvases=FindObjectsByType<Canvas>();var states=canvases.Select(c=>c.enabled).ToArray();foreach(var c in canvases)c.enabled=false;
        int width=Screen.width,height=Screen.height;var mode=Screen.fullScreenMode;
        follow.enabled=false;if(pixel!=null)pixel.enabled=false;
        Screen.SetResolution(1280,1320,FullScreenMode.Windowed);yield return new WaitForSecondsRealtime(.5f);
        camera.rect=new Rect(0,0,1,1);camera.orthographicSize=61;camera.transform.position=new Vector3(38,52,-10);
        yield return Capture("planta-centro");
        Screen.SetResolution(width,height,mode);yield return new WaitForSecondsRealtime(.5f);
        camera.transform.position=position;camera.orthographicSize=ortho;camera.rect=rect;
        if(pixel!=null)pixel.enabled=true;follow.enabled=true;
        for(int i=0;i<canvases.Length;i++)if(canvases[i]!=null)canvases[i].enabled=states[i];follow.SnapImediatoParaAlvo();
        report.Add("Planta Centro capturada com câmera de inspeção temporária; câmera normal restaurada.");
    }
    private IEnumerator Performance(string region)
    {
        QualitySettings.vSyncCount=0;Application.targetFrameRate=60;
        Screen.SetResolution(1280,720,FullScreenMode.Windowed);yield return new WaitForSecondsRealtime(.8f);
        double total=0;float max=0;
        for(int i=0;i<90;i++){yield return null;float d=Time.unscaledDeltaTime;total+=d;max=Mathf.Max(max,d);}
        report.Add("PERF "+region+": 90 frames, média "+(total/90*1000).ToString("F2")+" ms, máximo "+(max*1000).ToString("F2")+" ms; "+Screen.width+"x"+Screen.height+", vSync=0, target=60, janela QA; inclui carga do host.");
    }
    private static T Field<T>(object o,string n)=>(T)o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(o);
    private static void Set(object o,string n,object value)=>o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(o,value);
    private static void Invoke(object o,string n,params object[] args)=>o.GetType().GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(o,args);
    private static bool ActiveButton(string name)=>FindObjectsByType<Button>().Any(b=>b.gameObject.activeInHierarchy&&b.gameObject.name==name);
    private static void Click(string name)=>FindObjectsByType<Button>().First(b=>b.gameObject.activeInHierarchy&&b.gameObject.name==name).onClick.Invoke();

    private void SinucaRegression()
    {
        var ui=SinucaMinigameUI.Instance;ui.IniciarDuelo(null);
        Check(ui.SimularTesteDeInvariantes(out var oldReport),"Invariantes anteriores da Sinuca: "+oldReport);
        ui.IniciarDuelo(null);ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",1);Field<GameObject>(ui,"painelModal").SetActive(false);
        var balls=Field<IList>(ui,"bolas");
        // Tacada física real: branca alcança a 1, que cai na caçapa do meio.
        void Put(int i,string f,object val)=>balls[i].GetType().GetField(f).SetValue(balls[i],val);
        Put(0,"pos",new Vector2(0,42));Put(1,"pos",new Vector2(0,73));
        Invoke(ui,"ExecutarTacada",Vector2.up,180f);
        int steps=0;while(Field<object>(ui,"estado").ToString()=="BolasEmMovimento"&&steps++<1000)Invoke(ui,"SimularFisicaMesa",1f/120);
        Check(Field<int>(ui,"pontosMarcus")==1&&ui.ValidarInvarianteSeteBolas(),"Sinuca: tacada física conta ponto pelo resolver real.");
        ui.StopAllCoroutines();
        foreach(bool split in new[]{false,true})
        {
            ui.IniciarDuelo(null);ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",split?3:2);
            Set(ui,"maosVencidasMarcus",1);Set(ui,"maosVencidasZe",split?1:0);Set(ui,"pontosMarcus",3);Set(ui,"pontosZe",0);Set(ui,"turnoDeMarcus",true);
            for(int i=1;i<=4;i++)Put(i,"ativa",false);
            var fallen=Field<List<int>>(ui,"bolasObjetoCaidasNaTacada");fallen.Clear();fallen.Add(4);
            Set(ui,"brancaCaiuNestaTacada",false);Set(ui,"bolaMarcadaCaiuNestaTacada",false);Set(ui,"houvePrimeiroContatoBolaObjeto",true);Set(ui,"houveToqueTabelaOuCacapa",true);
            Invoke(ui,"ResolverFimDaTacada");
            Check(Field<int>(ui,"maosVencidasMarcus")==2&&Field<int>(ui,"maosVencidasZe")== (split?1:0)&&Field<object>(ui,"estado").ToString()=="FimDeSerieVitoria",
                "Sinuca: resolução da última caçapa encerra série "+(split?"2-1":"2-0")+" sem terceira mão forçada.");
        }
        ui.IniciarDuelo(null);ui.StopAllCoroutines();Invoke(ui,"ConfigurarMao",2);
        Set(ui,"turnoDeMarcus",true);Set(ui,"numeroBolaMarcada",1);Put(1,"ativa",false);Put(2,"ativa",false);
        var simultaneous=Field<List<int>>(ui,"bolasObjetoCaidasNaTacada");simultaneous.Clear();simultaneous.Add(1);simultaneous.Add(2);
        Set(ui,"bolaMarcadaCaiuNestaTacada",true);Set(ui,"brancaCaiuNestaTacada",false);Set(ui,"houvePrimeiroContatoBolaObjeto",true);Set(ui,"houveToqueTabelaOuCacapa",true);
        Invoke(ui,"ResolverFimDaTacada");
        Check(Field<int>(ui,"pontosMarcus")==0&&ui.ValidarInvarianteSeteBolas()&&Field<bool>(ui,"brancaNaMao"),"Sinuca: marcada + legal simultâneas anulam pontos e repõem pelo resolver real.");
        ui.StopAllCoroutines();ui.Fechar();
        Check(!CheckpointSystem.HasCheckpoint,"Regressão da Sinuca não cria save.");
    }

    private IEnumerator C1InvestigationRegression()
    {
        var director = Capitulo1Director.Instance;
        var items = FindObjectsByType<InteracaoCapitulo1>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var shop = items.First(i => i.Tipo == InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho);
        GameState.ImportarSnapshot(new GameStateSnapshot());
        GameState.Marcar(Flags.ChegouCasaAvo);
        director.TransicionarPara(Capitulo1Director.ZonaId.Interior_Mercadinho, shop.transform.position + Vector3.down, 1);
        yield return new WaitForSecondsRealtime(1.5f); yield return CloseText();
        shop.Interact(); yield return null;
        Check(!GameState.Tem(Flags.OuviuRelatoAtendente) && !GameState.ObterEntradasDiario().Any(d => d.id == "pista_relato_mercadinho"),
            "Atendente antes do recibo não transmite testemunho nem cria diário antecipado.");
        Check(!Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais").Any(l => l.texto.Contains("esta nota fiscal")),
            "Visitar a casa não apresenta cupom inexistente na conversa real.");
        yield return CloseText();
        var receipt = items.First(i => i.Tipo == InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004);
        director.TransicionarPara(Capitulo1Director.ZonaId.Interior_CasaDoAvo, receipt.transform.position + Vector3.down, 1);
        yield return new WaitForSecondsRealtime(1.5f); yield return CloseText();
        receipt.Interact(); yield return CloseText();
        Check(GameState.Tem(Flags.AchouReciboMercadinho) && !GameState.Tem(Flags.OuviuRelatoAtendente), "Coleta real do cupom precede testemunho.");
        director.TransicionarPara(Capitulo1Director.ZonaId.Interior_Mercadinho, shop.transform.position + Vector3.down, 1);
        yield return new WaitForSecondsRealtime(1.5f); shop.Interact(); yield return null;
        Check(!GameState.Tem(Flags.OuviuRelatoAtendente), "Abrir fala não equivale a ouvir o relato completo.");
        yield return CloseText();
        Check(GameState.Tem(Flags.ApresentouReciboAtendente) && GameState.Tem(Flags.OuviuRelatoAtendente), "Fechamento da conversa real persiste apresentação e testemunho.");
        shop.Interact(); yield return CloseText();
        Check(GameState.ObterEntradasDiario().Count(d => d.id == "pista_relato_mercadinho") == 1, "Revisita não duplica relato.");
        GameState.ImportarSnapshot(new GameStateSnapshot());
    }

    private IEnumerator SinucaPauseRegression()
    {
        Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Interior_BarDoZe, new Vector2(509,-56.8f), 1);
        yield return new WaitForSecondsRealtime(1.5f); yield return CloseText();
        var ui = SinucaMinigameUI.Instance; var keyboard = Keyboard.current;
        ui.IniciarDuelo(null); Field<GameObject>(ui,"painelModal").SetActive(false);
        Invoke(ui,"IniciarTurnoAtual");
        Invoke(ui,"ExecutarTacada", Vector2.right, 200f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape)); yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState()); yield return new WaitForSecondsRealtime(.15f);
        Check(MenuPausa.Aberto && Time.timeScale == 0, "Esc real abre pausa global sobre a Sinuca.");
        var balls = Field<IList>(ui,"bolas");
        Vector2 position = (Vector2)balls[0].GetType().GetField("pos").GetValue(balls[0]);
        yield return new WaitForSecondsRealtime(.35f);
        Check(position == (Vector2)balls[0].GetType().GetField("pos").GetValue(balls[0]), "Bola em movimento permanece congelada na pausa.");
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape)); yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState()); yield return new WaitForSecondsRealtime(.15f);
        Check(!MenuPausa.Aberto && Time.timeScale == 1, "Esc real retoma a Sinuca.");
        ui.Fechar();
        foreach (bool ballInHand in new[] { false, true })
        {
            ui.IniciarDuelo(null); Field<GameObject>(ui,"painelModal").SetActive(false);
            Set(ui,"turnoDeMarcus",false); Set(ui,"brancaNaMao",ballInHand);
            Invoke(ui,"ContinuarInicioTurno");
            yield return new WaitForSecondsRealtime(1.2f);
            Invoke(ui,"AbrirModalRegrasDuranteJogo");
            yield return new WaitForSecondsRealtime(.9f);
            Check(Field<object>(ui,"estado").ToString() == "TurnoZePensando", "IA aguarda regras durante " + (ballInHand ? "posicionamento" : "preparação final") + ".");
            Field<Button>(ui,"btnModalPrimario").onClick.Invoke();
            yield return new WaitForSecondsRealtime(.8f);
            Check(Field<object>(ui,"estado").ToString() != "TurnoZePensando", "Fechar regras permite uma tacada sem restaurar estado antigo.");
            ui.Fechar();
        }
        ui.IniciarDuelo(null); Invoke(ui,"ConfigurarMao",2);
        yield return new WaitForSecondsRealtime(1);
        var ambience = FindAnyObjectByType<SantaRitaBarAudio>(); var layers = Field<AudioSource[]>(ambience,"layers");
        bool effects = PlayerPrefs.GetFloat("audio_efeitos",1) > 0;
        Check(layers[0].volume < .001f && (effects ? layers[1].volume > 0 && layers[1].isPlaying : layers[1].volume == 0),
            "Segunda mão silencia murmúrio e toca água real, respeitando volume de efeitos.");
        Invoke(ui,"ConfigurarMao",3); yield return new WaitForSecondsRealtime(1);
        Check(layers[2].loop && layers[2].clip != null && (effects ? layers[2].volume > 0 : layers[2].volume == 0), "Terceira mão acrescenta camada de chuva.");
        ui.Fechar();
        Check(!CheckpointSystem.HasCheckpoint, "Investigação e pausa usam apenas RAM/namespace de QA.");
    }

    public static BochaShot BestShot(BochaSimulation table)
    {
        var best=new BochaShot();float score=float.NegativeInfinity;var aims=new List<Vector2>{table.bolas[0].pos};
        foreach(var b in table.bolas)if(b.valida&&b.lado==BochaSide.Tonao)aims.Add(b.pos);
        foreach(var aim in aims)foreach(float lane in new[]{-2.2f,-1.1f,0,1.1f,2.2f})
            for(float offset=-.65f;offset<=1.85f;offset+=.125f)
            {
                var delta=aim-new Vector2(lane,.65f);float power=Mathf.Sqrt(2*BochaSimulation.ResistenciaRolagem*delta.magnitude)+offset;
                Vector2 velocity=delta.normalized*power;
                var copy=table.Clone();copy.bolas.Add(new BochaBall{lado=BochaSide.Marcus,raio=.18f,massa=1,pos=new Vector2(lane,.65f),vel=velocity,giro=new Vector3(velocity.y,0,-velocity.x)/.18f});copy.RepousarParaPrevisao();
                if(!copy.bolas[0].valida)continue;
                float own=BochaMatch.MelhorDistancia(copy,BochaSide.Marcus),enemy=BochaMatch.MelhorDistancia(copy,BochaSide.Tonao);
                float val=-Mathf.Min(own,20)+(own<enemy?3:0)+Mathf.Min(enemy,3)*.4f;
                foreach(var b in copy.bolas)if(b.valida&&b.lado==BochaSide.Marcus&&BochaMatch.Distancia(b,copy.bolas[0])<enemy-.01f)val+=.3f;
                if(val>score){score=val;best=new BochaShot{x=lane,velocidade=delta.normalized*power};}
            }
        return best;
    }
    private void Finish()
    {
        if(isolation==null)return;Time.timeScale=1;Application.logMessageReceived-=Error;isolation.Dispose();isolation=null;
        report.Add((failed?"FAIL":"PASS")+" runtime "+DateTime.UtcNow.ToString("O"));File.WriteAllLines(Path.Combine(folder,"runtime-qa.txt"),report);Application.Quit(failed?1:0);
#if UNITY_EDITOR
        if(Application.isEditor)UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
}
#endif
