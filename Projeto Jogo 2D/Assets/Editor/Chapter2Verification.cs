using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Verificações de regressão do capítulo, preservadas como comando do projeto.
// Dados de save sempre isolados; não depende dos slots do autor.
public static class Chapter2Verification
{
    [MenuItem("Santa Rita/Capítulo 2/Auditar retornos antigos (isolado)")]
    public static void AuditOldFeedback()
    {
        if (EditorApplication.isPlaying) throw new Exception("Execute fora de Play; dados pessoais são preservados.");
        var results = new System.Collections.Generic.List<string>();
        void Verify(string name, Action test)
        {
            try { test(); results.Add("PASS " + name); checks++; }
            catch (Exception ex) { results.Add("FAIL " + name + ": " + ex.GetBaseException().Message); }
        }
        void Require(bool value, string message) { if (!value) throw new Exception(message); }
        using (CheckpointSystem.BeginIsolatedTests())
        {
            Verify("Atualização do diário", () =>
            {
                GameState.RegistrarDiario("audit", "pista", "Antes", "Antes");
                GameState.ConfirmarSaveManual(); int notifications = 0;
                Action listener = () => notifications++;
                GameState.OnEstadoAlterado += listener;
                try
                {
                    GameState.RegistrarDiario("audit", "pista", "Depois", "Depois");
                    Require(GameState.TemProgressoNaoSalvo && notifications == 1, "Nota alterada não marcou progresso/notificou UI.");
                }
                finally { GameState.OnEstadoAlterado -= listener; }
            });
            Verify("Conversa genérica sem testemunho", () =>
            {
                GameState.ImportarSnapshot(new GameStateSnapshot());
                GameState.Marcar(Flags.FalouAtendenteMercadinho);
                Require(!GameState.ObterEntradasDiario().Any(d => d.id == "pista_relato_mercadinho"), "Relato revelado sem ouvi-lo.");
            });
            Verify("Slot moderno não duplica no primeiro", () =>
            {
                CheckpointSystem.SaveSlot(2, "EntradaDaCidade", Vector2.zero, "orelhao_bar");
                Require(CheckpointSystem.TryLoadSlot(2, out _) && !CheckpointSystem.TryLoadSlot(0, out _), "Save do slot 3 apareceu no slot 1.");
                CheckpointSystem.Clear();
                Require(CheckpointSystem.TryLoadSlot(2, out _), "Clear não pode apagar save.");
            });
            Verify("Legado sem slot continua disponível", () =>
            {
                var path = (string)typeof(CheckpointSystem).GetProperty("PrimaryFilePath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                File.WriteAllText(path, "{\"versao\":2,\"sceneName\":\"EntradaDaCidade\",\"estado\":{\"versao\":2}}");
                Require(CheckpointSystem.TryLoadSlot(0, out var old) && old.slotIndex == -1, "Compatibilidade sem slot foi perdida.");
            });
            Verify("Recibo e testemunho separados", () =>
            {
                GameState.ImportarSnapshot(new GameStateSnapshot());
                GameState.Marcar(Flags.ChegouCasaAvo);
                InteracaoCapitulo1.ConfirmarTestemunhoAtendente();
                Require(!GameState.Tem(Flags.OuviuRelatoAtendente), "Casa visitada se passou por cupom coletado.");
                GameState.Marcar(Flags.AchouReciboMercadinho);
                Require(!GameState.Tem(Flags.OuviuRelatoAtendente), "Coleta se passou por testemunho ouvido.");
                InteracaoCapitulo1.ConfirmarTestemunhoAtendente();
                Require(GameState.Tem(Flags.ApresentouReciboAtendente) && GameState.Tem(Flags.OuviuRelatoAtendente) &&
                    GameState.ObterEntradasDiario().Count(d => d.id == "pista_relato_mercadinho") == 1, "Relato legítimo não persistiu.");
                InteracaoCapitulo1.ConfirmarTestemunhoAtendente();
                Require(GameState.ObterEntradasDiario().Count(d => d.id == "pista_relato_mercadinho") == 1, "Revisita duplicou relato.");
            });
            Verify("Casa fora de ordem não inventa bilhete", () =>
            {
                GameState.ImportarSnapshot(new GameStateSnapshot());
                GameState.Marcar(Flags.ExaminouCafeCasa); GameState.Marcar(Flags.ExaminouCartasCasa);
                GameState.Marcar(Flags.ExaminouDesordemCasa);
                InteracaoCapitulo1.VerificarConclusaoPistasCasaAvo();
                Require(!GameState.Tem(Flags.Pista_RumoBarDoZe), "Três pistas sem bilhete revelaram o Bar.");
                Require(SantaRitaGeography.ObjetivoAtual() == SantaRitaGeography.UV("casa_avo"), "Objetivo sem documento saiu da casa.");
            });
        }
        var host = new GameObject("QA_Sinuca_Retornos");
        try
        {
            var ui = host.AddComponent<SinucaMinigameUI>(); ui.IniciarDuelo(null);
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var type = typeof(SinucaMinigameUI);
            void Set(string name, object value) => type.GetField(name, flags).SetValue(ui, value);
            object Get(string name) => type.GetField(name, flags).GetValue(ui);
            object Call(string name, params object[] args) => type.GetMethod(name, flags).Invoke(ui, args);
            Verify("Tabela anterior ao contato não valida", () =>
            {
                var balls = (System.Collections.IList)Get("bolas");
                var ball = balls[0]; var bt = ball.GetType();
                bt.GetField("pos").SetValue(ball, new Vector2(202, 0));
                bt.GetField("vel").SetValue(ball, new Vector2(30, 0));
                Call("SimularFisicaMesa", .001f);
                Require(!(bool)Get("houveToqueTabelaOuCacapa"), "Tabela antes do primeiro contato foi contabilizada.");
            });
            Verify("Regras durante preparação final da IA", () =>
            {
                ((GameObject)Get("painelModal")).SetActive(false);
                Set("brancaNaMao", false); Set("turnoDeMarcus", false);
                var stateType = Get("estado").GetType();
                Set("estado", Enum.Parse(stateType, "TurnoZePensando"));
                var ai = (System.Collections.IEnumerator)Call("ExecutarTurnoZeCoroutine");
                ai.MoveNext(); ai.MoveNext(); // espera inicial e preparação do taco
                Call("AbrirModalRegrasDuranteJogo");
                ai.MoveNext();
                Require(Get("estado").ToString() != "BolasEmMovimento", "IA bateu com regras abertas; fechar restaura estado ultrapassado.");
            });
            Verify("Resolver real da Sinuca", () =>
            { Require(ui.SimularTesteDeInvariantes(out string details), details); results.Add(details); });
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
        Verify("Geografia aprovada e objetivo sem antecipação", () =>
        {
            Require(SantaRitaGeography.UV("capela").x > .45f && SantaRitaGeography.UV("capela").x < .6f, "Capela fora do meio inferior.");
            Require(SantaRitaGeography.UV("prefeitura").x > SantaRitaGeography.UV("escola").x, "Troca Prefeitura/Escola não preservada.");
        });
        Verify("Áudio da segunda e terceira mãos", () =>
        {
            Require(SantaRitaBarAudio.VolumeDaCamada(0, 1) > 0 && SantaRitaBarAudio.VolumeDaCamada(0, 2) == 0 &&
                SantaRitaBarAudio.VolumeDaCamada(1, 2) > 0 && SantaRitaBarAudio.VolumeDaCamada(2, 3) > 0, "Eventos de áudio não acompanham as mãos.");
            for (int layer = 0; layer < 3; layer++)
            {
                var clip = SantaRitaBarAudio.CriarTextura(layer);
                try
                {
                    var data = new float[clip.samples]; clip.GetData(data, 0);
                    float peak = data.Max(v => Mathf.Abs(v));
                    Require(clip.length == 16 && peak > .02f && peak < .9f && Mathf.Abs(data[0]) < .001f && Mathf.Abs(data[data.Length - 1]) < .001f,
                        "Textura vazia, com clipping ou borda descontínua: " + layer);
                }
                finally { UnityEngine.Object.DestroyImmediate(clip); }
            }
        });
        string report = string.Join("\n", results);
        File.WriteAllText(Path.Combine(Application.dataPath, "../Library/sr-feedback-audit.txt"), report);
        Debug.Log(report);
        if (results.Any(r => r.StartsWith("FAIL"))) throw new Exception("Retornos antigos: há regressões. Library/sr-feedback-audit.txt");
    }

    private static int checks;
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); checks++; }

    [MenuItem("Santa Rita/Capítulo 2/Validar regras e saves (isolado)")]
    public static void Run()
    {
        checks = 0;
        try
        {
            Rules(); Physics(); Gait(); Narrative(); Saves(); Matches(); AuditOldFeedback();
            string report = "PASS " + checks + " verificações: Bocha/regras/física e save isolado. " + DateTime.UtcNow.ToString("O");
            Debug.Log(report);
            string evidencias = Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR") ??
                Path.GetFullPath(Path.Combine(Application.dataPath, "../../Docs/Evidencias/Capitulo2"));
            Directory.CreateDirectory(evidencias);
            File.WriteAllText(Path.Combine(evidencias, "regras-save.txt"), report);
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }

    private static void Rules()
    {
        var match = new BochaMatch(); var sim = match.CriarMao();
        Check(match.Proximo(sim) == BochaSide.Marcus, "Marcus começa.");
        Check(!match.Lancar(sim, BochaSide.Tonao, 0, Vector2.up * 7), "Não jogar fora da vez.");
        Check(match.Lancar(sim, BochaSide.Marcus, 99, Vector2.up), "Inválida consome.");
        Check(match.usadas[0] == 1 && !sim.bolas[1].valida, "Estoque da inválida.");
        Check(match.Proximo(sim) == BochaSide.Tonao, "Empate sem válidas passa a vez.");
        var measure = new BochaSimulation(); measure.bolas.Add(BochaBall.Balim(new Vector2(0, 10)));
        void Ball(BochaSide s, float d) => measure.bolas.Add(new BochaBall { lado = s, massa = 1,
            raio = 0.18f, pos = new Vector2(d + 0.285f, 10) });
        Ball(BochaSide.Marcus, .12f); Ball(BochaSide.Marcus, .3f); Ball(BochaSide.Tonao, .6f);
        var r = BochaMatch.Medir(measure);
        Check(r.vencedor == BochaSide.Marcus && r.pontos == 2, "Contagem múltipla.");
        Check(Mathf.Abs(r.distanciaMarcus - .12f) < .0001f, "Medida entre superfícies.");
        measure.bolas[3].pos = measure.bolas[1].pos + Vector2.right * .005f;
        Check(BochaMatch.Medir(measure).pontos == 0, "Empate por tolerância.");
        measure.bolas[3].valida = false;
        Check(BochaMatch.Medir(measure).pontos == 2, "Sem adversárias válidas.");
        measure.bolas[1].valida = measure.bolas[2].valida = false;
        Check(BochaMatch.Medir(measure).pontos == 0, "Ambas sem válidas.");
        measure.bolas[0].valida = false;
        Check(BochaMatch.Medir(measure).anulada, "Balim fora anula.");
        sim.bolas[0].valida = false;
        r = match.Resolver(sim); int hand = match.Mao;
        Check(r.anulada && hand == 0 && match.Iniciador == BochaSide.Marcus, "Anulação mantém iniciador/disposição.");
        Check(match.CriarMao().bolas[0].pos == new BochaMatch().CriarMao().bolas[0].pos, "Reposição determinística.");
        match.pontos[0] = 5;
        Check(match.Vencedor == BochaSide.Nenhum, "Vitória não vem de vantagem provisória.");
        bool premature = false; try { match.Resolver(match.CriarMao()); } catch (InvalidOperationException) { premature = true; }
        Check(premature, "Não resolver com estoque.");
    }

    private static void Physics()
    {
        var sim = new BochaMatch().CriarMao(); var match = new BochaMatch();
        Check(match.Lancar(sim, BochaSide.Marcus, 0, new Vector2(-.23f, 4.9f)), "Lançamento normal.");
        sim.RepousarParaPrevisao();
        Check(!sim.IsMoving && sim.bolas[1].valida, "Repouso e validade.");
        Check(BochaMatch.MelhorDistancia(sim, BochaSide.Marcus) < 2f, "Aproximação alcançável.");
        var dry = new BochaSimulation(); dry.bolas.Add(BochaBall.Balim(new Vector2(2, 18)));
        dry.bolas.Add(new BochaBall { lado = BochaSide.Marcus, massa = 1, raio = .18f,
            pos = new Vector2(0, .65f), vel = new Vector2(0, 7) });
        var other = dry.Clone();
        dry.RepousarParaPrevisao(); other.RepousarParaPrevisao();
        Check(other.bolas[1].pos == dry.bolas[1].pos, "Piso uniforme e previsão determinística.");
        var clone = other.Clone();
        Check(clone.Impacto == null && clone.Contato == null, "IA compartilha física sem efeitos reais.");
        var hit = new BochaSimulation(); hit.bolas.Add(BochaBall.Balim(new Vector2(0, 10)));
        hit.bolas.Add(new BochaBall { lado = BochaSide.Marcus, massa = 1, raio = .18f,
            pos = new Vector2(0, 1), vel = new Vector2(0, 14) });
        hit.RepousarParaPrevisao();
        Check(hit.bolas[0].pos.y > 10.2f, "Colisão rápida desloca balim sem tunneling.");
        Check(hit.bolas[0].valida, "Balim permanece válido após atingir o fundo.");
        foreach (bool balim in new[] { false, true })
        {
            var walls = new BochaSimulation();
            var ball = balim ? BochaBall.Balim(new Vector2(0, 19.8f)) : new BochaBall { raio=.18f, massa=1, pos=new Vector2(0,19.8f) };
            ball.vel=Vector2.up*14;walls.bolas.Add(ball);int impacts=0;walls.Impacto=_=>impacts++;
            walls.Step(.1f);
            Check(ball.valida && ball.vel.y<0 && ball.pos.y<=20-ball.raio+.00001f && impacts==1,"Parede do fundo devolve "+(balim?"balim":"bocha")+" sem apagar nem duplicar impacto.");
            ball.pos=new Vector2(2.95f,.05f);ball.vel=new Vector2(5,-8);walls.Step();
            Check(ball.valida && ball.vel.x<0 && ball.vel.y>0,"Canto e cabeceira de lançamento contêm a bola.");
            walls.RepousarParaPrevisao();
            Check(!walls.IsMoving && ball.pos.y>=ball.raio && ball.pos.y<=20-ball.raio+.00001f,"Bola repousa dentro da cancha fechada.");
        }
    }

    private static void Gait()
    {
        var gait=new MarcusGait();int steps=0;
        gait.Avancar(.081f,()=>steps++);
        Check(gait.Quadro==0 && steps==1,"Primeiro som ocorre no contato, quadro zero.");
        gait.Avancar(.8f,()=>steps++);
        Check(gait.Quadro==4 && steps==2,"Segundo som coincide com o pé oposto.");
        for(int i=0;i<60;i++)gait.Avancar(0,()=>steps++);
        Check(steps==2 && gait.Quadro==0,"Parado/contra parede não dá passos.");
        gait.Avancar(5,()=>steps++);
        Check(steps==2,"Teleporte não dispara passos.");
        var a=new MarcusGait();var b=new MarcusGait();int sa=0,sb=0;
        for(int i=0;i<100;i++)a.Avancar(.016f,()=>sa++);
        for(int i=0;i<20;i++)b.Avancar(.08f,()=>sb++);
        Check(a.Quadro==b.Quadro && sa==sb,"Cadência independe do framerate para a mesma distância.");
        var hitch=new MarcusGait();int hc=0;hitch.Avancar(.4f,()=>hc++);
        Check(hc==1 && hitch.Quadro==0,"Frame demorado conserva o contato visível antes de soar o passo.");
    }

    private static void Narrative()
    {
        var content=JsonUtility.FromJson<C2ContentData>(Resources.Load<TextAsset>("Cap2/Investigation").text);
        var branches=content.interactions.Where(d=>d.branches!=null).SelectMany(d=>d.branches).ToArray();
        Check(branches.Length>=6 && branches.Select(b=>b.flag).Distinct().Count()==branches.Length,"Retornos opcionais têm consequências distintas.");
        using(CheckpointSystem.BeginIsolatedTests())
        {
            foreach(var branch in branches)
            {
                GameState.ImportarSnapshot(new GameStateSnapshot());
                Check(!branch.Disponivel,"Sem fontes não surge conhecimento: "+branch.flag);
                foreach(string required in branch.require)GameState.Marcar(required);
                Check(branch.Disponivel,"Fontes e apresentação abrem retorno: "+branch.flag);
                GameState.Marcar(branch.flag);
                Check(!branch.Disponivel,"Retorno concluído não repete descoberta: "+branch.flag);
            }
        }
    }

    private static void Saves()
    {
        using (CheckpointSystem.BeginIsolatedTests())
        {
            Check(!CheckpointSystem.HasCheckpoint, "Namespace de teste vazio.");
            GameState.Marcar(Flags.ZeDerrotadoNaSinuca); GameState.Marcar(Flags.RecebeuFichaClubeZe);
            CheckpointSystem.SaveSlot(0, "EntradaDaCidade", new Vector2(525, 12), "orelhao_bar_do_ze");
            Check(CheckpointSystem.TryLoadSlot(0, out var old) && old.versao == 2, "Save v2.");
            GameState.Marcar("c2_venceu_tonao");
            CheckpointSystem.RequestSpawnAtSlot(0);
            Check(!GameState.Tem("c2_venceu_tonao") && GameState.Tem(Flags.RecebeuFichaClubeZe), "Vitória sem save é descartada; ficha C1 mantida.");
            GameState.Marcar("c2_venceu_tonao"); GameState.Marcar("ficha_clube_tonao_02");
            CheckpointSystem.SaveSlot(1, "CentroSantaRita", new Vector2(5, 8), "orelhao_bocha");
            GameState.ImportarSnapshot(new GameStateSnapshot());
            CheckpointSystem.RequestSpawnAtSlot(1);
            Check(GameState.Tem("ficha_clube_tonao_02") && GameState.Tem("c2_venceu_tonao"), "Save após vitória mantém C2.");
            Check(CheckpointSystem.ConsumePendingSpawn(out var pos) && pos == new Vector2(5, 8), "Spawn consumido uma vez.");
            Check(!CheckpointSystem.ConsumePendingSpawn(out _), "Sem spawn automático repetido.");
            string dir=(string)typeof(CheckpointSystem).GetProperty("StorageDirectory",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).GetValue(null);
            string bad=Path.Combine(dir,"santa_rita_save_v2_slot_3.json");
            File.WriteAllText(bad,"{invalid");
            Check(!CheckpointSystem.TryLoadSlot(3,out _)&&CheckpointSystem.DescreverSlotIndisponivel(3).Contains("inválido"),"Arquivo malformado é descrito e preservado.");
            Check(File.ReadAllText(bad)=="{invalid","Leitura não destrói arquivo ruim.");
            File.WriteAllText(bad,"{\"versao\":99,\"sceneName\":\"CentroSantaRita\"}");
            Check(!CheckpointSystem.TryLoadSlot(3,out _)&&CheckpointSystem.DescreverSlotIndisponivel(3).Contains("incompatível"),"Versão futura não migra silenciosamente.");
            Check(CheckpointSystem.TryLoadSlot(1,out _),"Save inválido de outro slot não destrói válido.");
            File.WriteAllText(bad,"{\"versao\":2,\"sceneName\":\"CenaInexistente\"}");
            Check(!CheckpointSystem.TryLoadSlot(3,out _),"Cena desconhecida não é carregada por um save inválido.");
        }
    }

    private static void Matches()
    {
        foreach(bool skilled in new[]{false,true})
        {
            var match=new BochaMatch();var rng=new System.Random(93);int hands=0,shots=0;
            while(match.Vencedor==BochaSide.Nenhum&&hands++<24)
            {
                var sim=match.CriarMao();
                while(match.Proximo(sim)!=BochaSide.Nenhum)
                {
                    var side=match.Proximo(sim);
                    BochaShot shot=side==BochaSide.Tonao?BochaAI.Escolher(sim,4-match.usadas[1],rng):skilled?Chapter2RuntimeVerification.BestShot(sim):new BochaShot{x=2.3f,velocidade=new Vector2(0,3.5f)};
                    Check(match.Lancar(sim,side,shot),"Partida completa: lançamento válido.");shots++;sim.RepousarParaPrevisao();
                    if(shots>250)throw new Exception("Partida de Bocha sem progressão.");
                }
                match.Resolver(sim);
            }
            Check(match.Vencedor==(skilled?BochaSide.Marcus:BochaSide.Tonao),"IA pode "+(skilled?"perder":"vencer")+" em partida real de "+shots+" lançamentos / "+hands+" mãos.");
        }
    }
}
