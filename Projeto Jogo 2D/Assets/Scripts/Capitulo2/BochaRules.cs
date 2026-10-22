using System;
using System.Linq;
using UnityEngine;

// Variante autoral do capítulo 2, não regulamento de campeonato.
public enum BochaSide { Nenhum = -1, Marcus = 0, Tonao = 1 }

public struct BochaHandResult
{
    public bool anulada;
    public BochaSide vencedor;
    public int pontos;
    public float distanciaMarcus, distanciaTonao;
    public string resumo;
}

public sealed class BochaMatch
{
    public const int BochasPorLado = 4;
    public const int PontosParaVencer = 6;
    public const float ToleranciaMedida = 0.01f;
    public readonly int[] pontos = new int[2];
    public readonly int[] usadas = new int[2];
    public int Mao { get; private set; }
    public BochaSide Iniciador { get; private set; } = BochaSide.Marcus;
    public BochaSide UltimoLancador { get; private set; } = BochaSide.Nenhum;
    public bool Resolvida { get; private set; }
    public BochaSide Vencedor => pontos[0] >= PontosParaVencer ? BochaSide.Marcus :
        pontos[1] >= PontosParaVencer ? BochaSide.Tonao : BochaSide.Nenhum;

    public BochaSimulation CriarMao()
    {
        usadas[0] = usadas[1] = 0;
        Resolvida = false;
        UltimoLancador = BochaSide.Nenhum;
        var sim = new BochaSimulation();
        Vector2[] posicoes = { new Vector2(-0.6f, 13.3f), new Vector2(0.55f, 14.1f),
            new Vector2(0f, 12.6f), new Vector2(-1.0f, 14.6f) };
        sim.bolas.Add(BochaBall.Balim(posicoes[Mao % posicoes.Length]));
        if (Mao > 0)
        {
            // Anunciada antes da mira e imutável até a resolução da mão.
            sim.faixaUmida = Mao % 2 == 1 ? new Rect(-2.85f, 7.4f, 3.2f, 2.1f) :
                new Rect(-0.35f, 8.8f, 3.2f, 2.2f);
        }
        return sim;
    }

    public bool Lancar(BochaSimulation sim, BochaSide lado, float x, Vector2 velocidade)
    {
        if (Resolvida || sim.IsMoving || Vencedor != BochaSide.Nenhum ||
            lado == BochaSide.Nenhum || usadas[(int)lado] >= BochasPorLado || Proximo(sim) != lado)
            return false;
        usadas[(int)lado]++;
        UltimoLancador = lado;
        bool valido = Mathf.Abs(x) <= BochaSimulation.MeiaLargura - BochaSimulation.RaioBocha &&
            velocidade.y > 0f && velocidade.magnitude <= BochaSimulation.VelocidadeMaxima;
        var bola = new BochaBall
        {
            id = sim.bolas.Count, lado = lado, raio = BochaSimulation.RaioBocha, massa = 1f,
            pos = new Vector2(x, 0.65f), vel = valido ? velocidade : Vector2.zero, valida = valido
        };
        sim.bolas.Add(bola);
        return true; // Um lançamento inválido também gasta estoque.
    }

    public BochaSide Proximo(BochaSimulation sim)
    {
        if (Resolvida || Vencedor != BochaSide.Nenhum || !sim.bolas[0].valida) return BochaSide.Nenhum;
        if (usadas[0] >= BochasPorLado && usadas[1] >= BochasPorLado) return BochaSide.Nenhum;
        if (usadas[0] >= BochasPorLado) return BochaSide.Tonao;
        if (usadas[1] >= BochasPorLado) return BochaSide.Marcus;
        if (UltimoLancador == BochaSide.Nenhum) return Iniciador;
        float a = MelhorDistancia(sim, BochaSide.Marcus), b = MelhorDistancia(sim, BochaSide.Tonao);
        if ((float.IsPositiveInfinity(a) && float.IsPositiveInfinity(b)) || Mathf.Abs(a - b) <= ToleranciaMedida)
            return Outro(UltimoLancador);
        return a > b ? BochaSide.Marcus : BochaSide.Tonao;
    }

    public static BochaSide Outro(BochaSide lado) => lado == BochaSide.Marcus ? BochaSide.Tonao : BochaSide.Marcus;

    public static float Distancia(BochaBall bola, BochaBall balim) =>
        Mathf.Max(0f, Vector2.Distance(bola.pos, balim.pos) - bola.raio - balim.raio);

    public static float MelhorDistancia(BochaSimulation sim, BochaSide lado)
    {
        float menor = float.PositiveInfinity;
        foreach (var bola in sim.bolas)
            if (bola.valida && bola.lado == lado) menor = Mathf.Min(menor, Distancia(bola, sim.bolas[0]));
        return menor;
    }

    public static BochaHandResult Medir(BochaSimulation sim)
    {
        if (sim.IsMoving) throw new InvalidOperationException("Medida antes do repouso.");
        var r = new BochaHandResult { vencedor = BochaSide.Nenhum };
        if (!sim.bolas[0].valida)
        {
            r.anulada = true;
            r.resumo = "Balim fora: mão anulada. Mesma disposição e mesmo iniciador; nenhum ponto.";
            return r;
        }
        r.distanciaMarcus = MelhorDistancia(sim, BochaSide.Marcus);
        r.distanciaTonao = MelhorDistancia(sim, BochaSide.Tonao);
        if ((float.IsPositiveInfinity(r.distanciaMarcus) && float.IsPositiveInfinity(r.distanciaTonao)) ||
            Mathf.Abs(r.distanciaMarcus - r.distanciaTonao) <= ToleranciaMedida)
        {
            r.resumo = "Mão sem pontos: melhores bochas empatadas (tolerância 0,01), ou nenhuma bocha válida.";
            return r;
        }
        r.vencedor = r.distanciaMarcus < r.distanciaTonao ? BochaSide.Marcus : BochaSide.Tonao;
        float rival = r.vencedor == BochaSide.Marcus ? r.distanciaTonao : r.distanciaMarcus;
        r.pontos = sim.bolas.Count(b => b.valida && b.lado == r.vencedor &&
            Distancia(b, sim.bolas[0]) < rival - ToleranciaMedida);
        r.pontos = Mathf.Clamp(r.pontos, 0, BochasPorLado);
        r.resumo = (r.vencedor == BochaSide.Marcus ? "Marcus" : "Tonão") + " marca " + r.pontos +
            (r.pontos == 1 ? " ponto" : " pontos") + ". Contam as bochas mais próximas que a melhor adversária.";
        return r;
    }

    public BochaHandResult Resolver(BochaSimulation sim)
    {
        if (Resolvida) throw new InvalidOperationException("Mão já contabilizada.");
        if (sim.bolas[0].valida && usadas[0] + usadas[1] < BochasPorLado * 2)
            throw new InvalidOperationException("Ainda há lançamentos disponíveis.");
        var r = Medir(sim);
        if (!r.anulada && r.vencedor != BochaSide.Nenhum) pontos[(int)r.vencedor] += r.pontos;
        Resolvida = true;
        if (!r.anulada) { Mao++; Iniciador = Outro(Iniciador); }
        return r;
    }
}
