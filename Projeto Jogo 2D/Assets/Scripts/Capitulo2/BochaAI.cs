using System.Collections.Generic;
using UnityEngine;

public static class BochaAI
{
    public static double UltimaDecisaoMs { get; private set; }
    public static int UltimosCandidatos { get; private set; }
    // Orçamento limitado. Estima a força para gerar candidatos, mas pontua
    // cada um no MESMO kernel. Não puxa uma bola nem corrige a mira no ar.
    public static BochaShot Escolher(BochaSimulation mesa,int estoque,System.Random rng)
    {
        var timer=System.Diagnostics.Stopwatch.StartNew();UltimosCandidatos=0;
        Vector2 balim=mesa.bolas[0].pos,target=balim;
        float nearest=float.PositiveInfinity;
        foreach(var b in mesa.bolas)if(b.valida&&b.lado==BochaSide.Marcus)
        {float distance=BochaMatch.Distancia(b,mesa.bolas[0]);if(distance<nearest){nearest=distance;target=b.pos;}}
        var shots=new List<BochaShot>(25);
        foreach(float x in new[]{-1.7f,0,1.7f})
        {
            Vector2 delta=balim-new Vector2(x,.65f);float power=BochaLaunch.ForcaPonto(delta.magnitude);
            foreach(float offset in new[]{-.025f,0,.025f})shots.Add(BochaLaunch.Preparar(BochaThrowType.Ponto,x,delta,power+offset));
        }
        if(!float.IsPositiveInfinity(nearest))
        {
            foreach(float x in new[]{-1.4f,1.4f})
            {
                Vector2 delta=target-new Vector2(x,.65f);
                float p=(Mathf.Sqrt(2*BochaSimulation.ResistenciaRolagem*delta.magnitude)/.77f+1.2f-3.8f)/8.7f;
                foreach(float offset in new[]{-.025f,.035f})shots.Add(BochaLaunch.Preparar(BochaThrowType.Rafa,x,delta,p+offset));
                // Volo é uma opção quando uma bocha protege o caminho direto.
                float lo=0,hi=1;
                for(int i=0;i<12;i++)
                {
                    float mid=(lo+hi)*.5f;var s=BochaLaunch.Preparar(BochaThrowType.Volo,x,delta,mid);
                    float flight=(s.vertical+Mathf.Sqrt(s.vertical*s.vertical+2*BochaSimulation.Gravidade*s.altura))/BochaSimulation.Gravidade;
                    if(s.velocidade.magnitude*flight<delta.magnitude)lo=mid;else hi=mid;
                }
                shots.Add(BochaLaunch.Preparar(BochaThrowType.Volo,x,delta,(lo+hi)*.5f));
            }
        }
        BochaShot best=shots[0];float score=float.NegativeInfinity;
        foreach(var shot in shots)
        {
            var sim=mesa.Clone();sim.bolas.Add(new BochaBall{id=sim.bolas.Count,lado=BochaSide.Tonao,raio=.18f,massa=1,
                pos=new Vector2(shot.x,.65f),vel=shot.velocidade,altura=shot.altura,vertical=shot.vertical,giro=shot.giro});
            sim.RepousarParaPrevisao();UltimosCandidatos++;
            if(sim.IsMoving||!sim.bolas[0].valida)continue;
            float own=BochaMatch.MelhorDistancia(sim,BochaSide.Tonao),rival=BochaMatch.MelhorDistancia(sim,BochaSide.Marcus);
            float value=-Mathf.Min(own,15)+(own<rival?2.4f:0);
            if(!float.IsPositiveInfinity(rival))value+=Mathf.Min(rival,3)*.38f;
            foreach(var b in sim.bolas)if(b.valida&&b.lado==BochaSide.Tonao&&BochaMatch.Distancia(b,sim.bolas[0])<rival-.01f)value+=.18f;
            if(value<=score)continue;score=value;best=shot;
        }
        // Erro humano pequeno e explícito antes da partida simular o lançamento.
        float error=((float)rng.NextDouble()-.5f)*(estoque<=1?1.1f:1.7f);
        best.velocidade=Quaternion.Euler(0,0,error)*best.velocidade;
        best.velocidade*=.985f+(float)rng.NextDouble()*.03f;
        best.giro=new Vector3(best.velocidade.y,0,-best.velocidade.x)/.18f*(best.tipo==BochaThrowType.Ponto?1:best.tipo==BochaThrowType.Rafa?.2f:.1f);
        best.intencao=best.tipo==BochaThrowType.Ponto?"Tonão procura encostar no balim.":best.tipo==BochaThrowType.Rafa?"Tonão prepara uma batida rasteira.":"Tonão mede a queda de um volo.";
        UltimaDecisaoMs=timer.Elapsed.TotalMilliseconds;return best;
    }
}
