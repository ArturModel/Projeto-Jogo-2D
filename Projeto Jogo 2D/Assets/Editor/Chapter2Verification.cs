using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Verificações de regressão do capítulo, preservadas como comando do projeto.
// Dados de save sempre isolados; não depende dos slots do autor.
public static class Chapter2Verification
{
    private static int checks;
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); checks++; }

    [MenuItem("Santa Rita/Capítulo 2/Validar regras e saves (isolado)")]
    public static void Run()
    {
        checks = 0;
        try
        {
            Rules(); Physics(); Gait(); Narrative(); Saves(); Matches();
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
        Check(match.Lancar(sim, BochaSide.Marcus, 0, new Vector2(-.32f, 6.9f)), "Lançamento normal.");
        sim.RepousarParaPrevisao();
        Check(!sim.IsMoving && sim.bolas[1].valida, "Repouso e validade.");
        Check(BochaMatch.MelhorDistancia(sim, BochaSide.Marcus) < 2f, "Aproximação alcançável.");
        var dry = new BochaSimulation(); dry.bolas.Add(BochaBall.Balim(new Vector2(2, 18)));
        dry.bolas.Add(new BochaBall { lado = BochaSide.Marcus, massa = 1, raio = .18f,
            pos = new Vector2(0, .65f), vel = new Vector2(0, 7) });
        var wet = dry.Clone(); wet.faixaUmida = new Rect(-3, 5, 6, 5);
        dry.RepousarParaPrevisao(); wet.RepousarParaPrevisao();
        Check(wet.bolas[1].pos.y < dry.bolas[1].pos.y - .5f, "Piso úmido desacelera.");
        var clone = wet.Clone();
        Check(clone.faixaUmida == wet.faixaUmida && clone.Impacto == null, "IA compartilha piso, sem efeitos reais.");
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
                    Check(match.Lancar(sim,side,shot.x,shot.velocidade),"Partida completa: lançamento válido.");shots++;sim.RepousarParaPrevisao();
                    if(shots>250)throw new Exception("Partida de Bocha sem progressão.");
                }
                match.Resolver(sim);
            }
            Check(match.Vencedor==(skilled?BochaSide.Marcus:BochaSide.Tonao),"IA pode "+(skilled?"perder":"vencer")+" em partida real de "+shots+" lançamentos / "+hands+" mãos.");
        }
    }
}
