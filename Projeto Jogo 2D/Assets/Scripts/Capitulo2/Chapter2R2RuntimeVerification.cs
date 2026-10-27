#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public sealed partial class Chapter2RuntimeVerification
{
    private bool r2Force;
#if UNITY_EDITOR
    public static void StartR2InEditor(string output)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);
        var g=new GameObject("QA_C2_R2_Explicito");DontDestroyOnLoad(g);g.AddComponent<Chapter2RuntimeVerification>().r2Force=true;
    }
#endif
    private IEnumerator TapR2(Key key)
    {
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return null;yield return null;
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;
    }
    private IEnumerator RunR2()
    {
        report.Add("R2: fixture em RAM + input e callbacks reais; reposicionamentos são explícitos. Não é campanha legítima.");
        QualitySettings.vSyncCount=0;Application.targetFrameRate=60;
        yield return new WaitForSecondsRealtime(2);GameState.Limpar();
        foreach(string f in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida})GameState.Marcar(f);
        SceneTransition.Ir("CentroSantaRita","c1_portao_norte");yield return AwaitScene("CentroSantaRita");
        yield return DialogueR2();
        yield return Place(new Vector2(44,48));yield return Capture("01-praca");
        yield return Travel("SociedadeBeiraRio");
        yield return Place(new Vector2(10,7));yield return Capture("11-sociedade-cancha");
        yield return Place(new Vector2(24,16));yield return Capture("12-cabeceira-horizontal");
        yield return Place(new Vector2(5,3));yield return Capture("13-entrada-convivencia");
        yield return RoutineR2();
        yield return ExitToCenter();
        foreach(var target in new[]{"IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira"})
        {
            yield return Travel(target);
            var layout=FindAnyObjectByType<SantaRitaRoomLayout>();
            Check(layout!=null&&layout.size.x>layout.size.y*.6f,"Planta e migração registradas: "+target);
            Vector2 view=target=="IgrejaMatriz"?new Vector2(9,12):target=="PrefeituraSantaRita"?new Vector2(10,5):target=="RodoviariaSantaRita"?new Vector2(11,7):target=="MercadinhoDeposito"?new Vector2(8,7):target=="OficinaRadio"?new Vector2(9,6):new Vector2(8,7);
            yield return Place(view);yield return Capture(target);
            if(target=="PrefeituraSantaRita")
            {
                yield return Use("protocolo");Check(!GameState.Tem("c2_protocolo_adilio"),"Protocolo não é entregue antes do atendimento.");
                yield return Use("prefeitura");yield return Use("protocolo");
            }
            foreach(var item in FindObjectsByType<Capitulo2Interactable>().Where(i=>Capitulo2Director.Dados(i.interactionId).kind!="bocha").ToArray())
                yield return Use(item.interactionId);
            if(target=="OficinaRadio")
            {if(!GameState.Tem("c2_radio_restaurado"))yield return Use("radio_test");Check(GameState.Tem("c2_radio_restaurado"),"Retorno LOCAL do rádio conclui interação real.");}
            yield return ExitToCenter();
        }
        yield return Travel("CemiterioSantaRita");yield return Place(new Vector2(60,96));yield return Capture("03-cemiterio");
        Check(Capitulo2Director.Instance.memorialPercepcao!=null,"Percepção ligada por referência serializada ao jazigo real.");
        yield return FlashlightR2("cemiterio",new Vector2(60,96));
        yield return Use("memorial");Check(GameState.Tem("c2_velas_percebidas"),"Leitura do jazigo dispara percepção no alvo serializado real.");
        yield return Travel("AbrigoCemiterio");yield return Place(new Vector2(7,5));yield return Capture("04-abrigo");yield return Use("caderno_velas");
        yield return Travel("CemiterioSantaRita");yield return ExitToCenter();
        yield return FlashlightR2("centro",new Vector2(31,45));
        yield return Walking();
        yield return FlashlightWalkR2();
        yield return Travel("SociedadeBeiraRio");yield return BochaAudioR2();yield return SlotsR2();
        AIPerformanceR2();
        report.Add("Não certifica aprovação artística, audição humana, balanceamento ou percurso sem fixture.");
#if UNITY_EDITOR
        if(Application.isEditor){Finish();UnityEditor.EditorApplication.isPlaying=false;yield break;}
#endif
        Finish();
    }
    private IEnumerator DialogueR2()
    {
        var campaign=GameState.ExportarSnapshot();
        foreach(string id in new[]{"mila","celina","alcina","nair","rodo_motorista","rodo_passageira"})
        foreach(bool late in new[]{false,true})
        {
            GameState.Limpar();var d=Capitulo2Director.Dados(id);var branch=d.branches[0];
            if(late)GameState.Marcar(Capitulo2Director.Vitoria);
            // A primeira visita continua sendo apresentação mesmo com requisitos obtidos em outra ordem.
            foreach(string f in branch.require.Where(f=>f!=d.flag))GameState.Marcar(f);
            Capitulo2Director.Instance.Interagir(id);yield return null;
            var pages=Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais");
            Check(pages!=null&&pages[0].texto==d.lines[0].texto,"Apresentação real "+id+"; vitória prévia="+late);
            Check(!GameState.Tem(branch.flag),"Consequência não antecede leitura "+id);
            yield return CloseText();Check(GameState.Tem(d.flag),"Callback da apresentação "+id);
            Capitulo2Director.Instance.Interagir(id);yield return null;pages=Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais");
            Check(pages!=null&&pages[0].texto==branch.lines[0].texto,"Opcional real alcançável após apresentação "+id+"; vitória="+late);
            yield return CloseText();Check(GameState.Tem(branch.flag),"Consequência do ramo executada "+id);
            GameState.Marcar(Capitulo2Director.Vitoria);
            if(d.after?.Length>0)
            {
                Capitulo2Director.Instance.Interagir(id);yield return null;pages=Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais");
                Check(pages[0].texto==d.after[0].texto,"Reação depois do opcional "+id);yield return CloseText();
                Check(GameState.Tem("c2_reacao_vitoria_"+id),"Reação única registrada somente após leitura "+id);
                Capitulo2Director.Instance.Interagir(id);yield return null;pages=Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais");
                Check(pages[0].texto!=d.after[0].texto,"Reação não repete para sempre "+id);yield return CloseText();
            }
        }
        GameState.Limpar();GameState.Marcar(Capitulo2Director.Vitoria);
        Capitulo2Director.Instance.Interagir("telefone_rodo");yield return null;
        Check(Field<DialogoLinha[]>(GameMessageUI.Instance,"paginasAtuais")[0].texto==Capitulo2Director.Dados("telefone_rodo").lines[0].texto,"Telefone tardio começa pela primeira comunicação, não atualização inexistente.");
        yield return CloseText();Check(GameState.Tem("c2_ajuda_comunicada"),"Primeira comunicação conclui callback.");
        GameState.ImportarSnapshot(campaign);
    }
    private IEnumerator RoutineR2()
    {
        var ton=FindObjectsByType<SantaRitaNpcWork>().First(w=>w.name=="NPC_tonao");
        yield return Place(new Vector2(5,8));
        var initial=ton.transform.position;var stations=new HashSet<int>();
        for(int i=0;i<75;i++) {stations.Add(ton.StationIndex);if(i%3==0)yield return Capture("rotina-"+i.ToString("D2"));else yield return new WaitForSecondsRealtime(.2f);}
        Check(stations.Count>=2&&Vector2.Distance(initial,ton.transform.position)>.2f,"Tonão percorre postos reais, com poses e alvo seguindo o corpo.");
        var target=ton.GetComponentInChildren<Capitulo2Interactable>();Check(target!=null&&Vector2.Distance(target.transform.position,ton.transform.position)<1,"Interação segue Tonão.");
        yield return Place(ton.transform.position+Vector3.down*.8f);GameMessageUI.Instance.Show("Teste de interrupção da rotina.","MARCUS","marcus");yield return null;
        var stopped=ton.transform.position;yield return new WaitForSecondsRealtime(.6f);Check(ton.transform.position==stopped&&!ton.Working,"Conversa interrompe trabalho/movimento.");yield return CloseText();
    }
    private IEnumerator FlashlightR2(string place,Vector2 p)
    {
        yield return Place(p);FindAnyObjectByType<PlayerController>().TeleportarPara(p,1);
        GameState.DefinirNumero("equipamento.lanterna_ligada",0);yield return null;yield return Capture(place+"-lanterna-off");
        float wait=Time.realtimeSinceStartup+30;while(!Application.isFocused&&Time.realtimeSinceStartup<wait)yield return null;
        Check(Application.isFocused,"Janela com foco para input F: "+place);
        yield return TapR2(Key.F);Check(GameState.LanternaLigada,"F liga uma vez: "+place);yield return null;
        var player=FindAnyObjectByType<PlayerController>();Check(player.LanternaVisivel,"Feixe visível após F: "+place);
        Check(player.GetComponentsInChildren<Light2D>(true).Count(l=>l.lightType==Light2D.LightType.Point)==1,"Uma fonte de equipamento em "+place);
        yield return Capture(place+"-lanterna-on");
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.F));yield return new WaitForSecondsRealtime(.5f);
        bool held=GameState.LanternaLigada;yield return new WaitForSecondsRealtime(.5f);Check(GameState.LanternaLigada==held,"Segurar F não alterna repetidamente.");
        InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;
        if(!held){yield return TapR2(Key.F);}
        GameMessageUI.Instance.Show("A lanterna aguarda o fim da leitura.","MARCUS","marcus");yield return null;
        Check(!player.LanternaVisivel&&GameState.LanternaLigada,"Leitura suprime apresentação e conserva preferência.");yield return TapR2(Key.F);Check(GameState.LanternaLigada,"F bloqueado na leitura.");yield return CloseText();
        MapaInterativoUI.Instance.Abrir(0);yield return TapR2(Key.F);Check(GameState.LanternaLigada,"F bloqueado no mapa.");MapaInterativoUI.Instance.Fechar();
        PayphoneCheckpoint.AbrirSelecaoCarregar(null,null);yield return TapR2(Key.F);Check(GameState.LanternaLigada,"F bloqueado na seleção de saves.");PayphoneCheckpoint.FecharModalSlots();
        yield return TapR2(Key.Escape);Check(MenuPausa.Aberto,"Pausa real aberta para teste de equipamento.");yield return TapR2(Key.F);Check(GameState.LanternaLigada,"F bloqueado na pausa.");yield return TapR2(Key.Escape);
        var snapshot=GameState.ExportarSnapshot();GameState.Marcar(Flags.LanternaDoCarroObtida,false);yield return TapR2(Key.F);Check(!GameState.LanternaLigada&&!player.LanternaVisivel,"F antes da aquisição não fornece equipamento.");
        GameState.ImportarSnapshot(snapshot);var old=GameState.ExportarSnapshot();old.numeros.RemoveAll(v=>v.chave=="equipamento.lanterna_ligada");GameState.ImportarSnapshot(old);Check(!GameState.LanternaLigada,"Save antigo com equipamento e sem preferência começa apagado.");GameState.ImportarSnapshot(snapshot);
    }
    private IEnumerator FlashlightWalkR2()
    {
        var player=FindAnyObjectByType<PlayerController>();GameState.DefinirNumero("equipamento.lanterna_ligada",1);
        foreach(var pair in new[]{(Key.W,1),(Key.S,0),(Key.A,2),(Key.D,3),(Key.W,1),(Key.S,0)})
        {
            yield return Place(new Vector2(31,45));InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(pair.Item1));
            for(int i=0;i<16;i++){yield return new WaitForSecondsRealtime(.08f);yield return Capture("lanterna-dir"+pair.Item2+"-"+i.ToString("D2"));}
            InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.3f);
            Check(player.DirecaoAtual==pair.Item2&&player.LanternaVisivel,"Pose equipada andar/parar na direção "+pair.Item2);
            Vector2 pixel=pair.Item2==1?new Vector2(40,28):pair.Item2==0?new Vector2(40,46):new Vector2(16,36);
            Vector2 expected=player.transform.TransformPoint(new Vector2((pixel.x-32)/32,(56-pixel.y)/32));
            Check(Vector2.Distance(expected,Field<Transform>(player,"flashlight").position)<.045f,"Lente e origem coincidem ao virar/parar, inclusive perfil → vertical: "+pair.Item2);
            yield return Capture("lanterna-idle-"+pair.Item2);
        }
    }
    private IEnumerator SlotsR2()
    {
        var layout=FindAnyObjectByType<SantaRitaRoomLayout>();var snapshot=GameState.ExportarSnapshot();
        GameState.Marcar("qa_slot0");CheckpointSystem.SaveSlot(0,"SociedadeBeiraRio",new Vector2(5,3),"qa_r2_0");GameState.Marcar("qa_slot0",false);
        GameState.Marcar("qa_slot4");CheckpointSystem.SaveSlot(4,"CentroSantaRita",new Vector2(31,45),"qa_r2_4");GameState.Marcar("qa_slot4",false);
        GameState.DefinirNumero("equipamento.lanterna_ligada",0);CheckpointSystem.TryLoadSlot(0,out var stored);
        Check(stored.estado.numeros.Any(n=>n.chave=="equipamento.lanterna_ligada"&&n.valor==1),"F em RAM não regrava a preferência do slot manual anterior.");
        var ui=BochaMinigameUI.Instance;ui.Abrir(null);Invoke(ui,"Derrota");Click("ESCOLHER SAVE");yield return null;
        Check(PayphoneCheckpoint.IsSaveUIOpen&&BochaMinigameUI.IsShowing,"Derrota Bocha abre escolha explícita de cinco slots.");yield return Capture("derrota-bocha-slots");
        PayphoneCheckpoint.FecharModalSlots();Check(ActiveButton("ESCOLHER SAVE"),"Cancelar volta à derrota Bocha.");
        Click("ESCOLHER SAVE");PayphoneCheckpoint.AoClicarSlot(0);yield return new WaitForSecondsRealtime(3);yield return AwaitScene("SociedadeBeiraRio");
        Check(GameState.Tem("qa_slot0")&&!GameState.Tem("qa_slot4"),"Escolha carrega slot 1 mesmo sendo slot 5 o último gravado.");
        var sinuca=SinucaMinigameUI.Instance;sinuca.IniciarDuelo(null);sinuca.StopAllCoroutines();Invoke(sinuca,"ConcluirSerieDerrota");Field<UnityEngine.UI.Button>(sinuca,"btnModalPrimario").onClick.Invoke();yield return null;
        Check(PayphoneCheckpoint.IsSaveUIOpen&&SinucaMinigameUI.IsShowing,"Derrota Sinuca abre o mesmo seletor.");yield return Capture("derrota-sinuca-slots");
        PayphoneCheckpoint.FecharModalSlots();Check(Field<GameObject>(sinuca,"painelModal").activeSelf&&Field<UnityEngine.UI.Button>(sinuca,"btnModalPrimario").GetComponentInChildren<TMPro.TMP_Text>().text=="ESCOLHER SAVE","Cancelar volta à derrota Sinuca.");Field<UnityEngine.UI.Button>(sinuca,"btnModalPrimario").onClick.Invoke();PayphoneCheckpoint.AoClicarSlot(4);
        yield return new WaitForSecondsRealtime(3);yield return AwaitScene("CentroSantaRita");Check(GameState.Tem("qa_slot4")&&!GameState.Tem("qa_slot0"),"Sinuca carrega exatamente o slot escolhido.");
        GameState.ImportarSnapshot(snapshot);GameState.Marcar("layout_r2_SociedadeBeiraRio",false);
        CheckpointSystem.SaveSlot(1,"SociedadeBeiraRio",new Vector2(12,34),"qa_sociedade_antiga");CheckpointSystem.CarregarSlot(1);
        yield return new WaitForSecondsRealtime(3);yield return AwaitScene("SociedadeBeiraRio");
        var pos=(Vector2)FindAnyObjectByType<PlayerController>().transform.position;
        Check(pos.y<17&&pos.x<29&&!(pos.x>6&&pos.y>9&&pos.y<15),"Save antigo da cancha é adaptado para área conectada à saída: "+pos);
        Check(GameState.Tem("layout_r2_SociedadeBeiraRio"),"Migração espacial registrada em RAM sem regravar save.");
        // O sistema proíbe escopos sobrepostos. Troca por outro namespace vazio no fim da suíte.
        isolation.Dispose();isolation=CheckpointSystem.BeginIsolatedTests();
        {
            ui.Abrir(null);Invoke(ui,"Derrota");Check(!ActiveButton("ESCOLHER SAVE")&&ActiveButton("MENU PRINCIPAL"),"Bocha sem save oferece menu, sem retorno automático.");ui.Fechar();
            sinuca.IniciarDuelo(null);sinuca.StopAllCoroutines();Invoke(sinuca,"ConcluirSerieDerrota");
            Check(Field<UnityEngine.UI.Button>(sinuca,"btnModalPrimario").GetComponentInChildren<TMPro.TMP_Text>().text=="NOVO JOGO","Sinuca sem save não inventa checkpoint.");sinuca.Fechar();
        }
    }
    private IEnumerator BochaAudioR2()
    {
        var ui=BochaMinigameUI.Instance;ui.Abrir(null);Click("DUELAR ATÉ 6 PONTOS");yield return null;Click("LER A CANCHA");
        var match=Field<BochaMatch>(ui,"partida");var first=Field<BochaSimulation>(ui,"sim");
        match.usadas[0]=match.usadas[1]=4;match.Resolver(first);Invoke(ui,"NovaMao");
        Set(ui,"fase",Enum.Parse(Field<object>(ui,"fase").GetType(),"Resultado"));
        var sim=Field<BochaSimulation>(ui,"sim");sim.bolas.Add(new BochaBall{id=1,lado=BochaSide.Marcus,pos=new Vector2(0,19.8f),vel=new Vector2(0,5),raio=.18f,massa=1});
        sim.Step();Check(sim.bolas[1].vel.y<0&&sim.bolas[1].valida,"Colisão física na cabeceira rebate antes do evento sonoro.");
        yield return new WaitForSecondsRealtime(.9f);
        var source=Field<AudioSource>(ui,"efeitos");var response=Capitulo2Audio.RespostaAbafada();var samples=new float[response.samples];response.GetData(samples,0);
        float rms=Mathf.Sqrt(samples.Sum(v=>v*v)/samples.Length),peak=samples.Max(v=>Mathf.Abs(v));
        report.Add("Áudio resposta: "+response.length.ToString("F2")+" s, RMS "+rms.ToString("F4")+", pico "+peak.ToString("F3"));
        Check(Field<bool>(ui,"efeitoMao")&&source.isPlaying&&Field<TMPro.TextMeshProUGUI>(ui,"percepcao").text.Contains("sopro"),"Resposta abafada audível agendada após impacto real, distinta da colisão.");
        Check(response!=Field<AudioClip>(ui,"impacto")&&rms>.001f&&peak<1,"Resposta usa clip próprio não vazio e sem clipping.");ui.Fechar();
    }
    private void AIPerformanceR2()
    {
        foreach(int count in new[]{0,4,7})
        {
            var sim=new BochaSimulation();sim.bolas.Add(BochaBall.Balim(new Vector2(.3f,15)));
            for(int i=0;i<count;i++)sim.bolas.Add(new BochaBall{id=i+1,lado=i%2==0?BochaSide.Marcus:BochaSide.Tonao,pos=new Vector2((i%3-1)*.6f,11+i*.5f),raio=.18f,massa=1});
            var shot=BochaAI.Escolher(sim,2,new System.Random(317));
            report.Add("AI síncrona: "+count+" bochas + balim, "+BochaAI.UltimaDecisaoMs.ToString("F2")+" ms; velocidade "+shot.velocidade.magnitude.ToString("F2"));
            Check(BochaAI.UltimaDecisaoMs<500&&shot.velocidade.magnitude>0,"Decisão AI medida, sem travamento excessivo (limite 500 ms).");
        }
    }
}
#endif
