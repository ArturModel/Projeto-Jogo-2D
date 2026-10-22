using System.Collections.Generic;
using UnityEngine;

public struct BochaShot { public float x; public Vector2 velocidade; public string intencao; }

public static class BochaAI
{
    // Avalia aproximação, retirada e balim na mesma física. O erro é aplicado
    // ao lançamento, antes da simulação real, e nunca corrige o resultado.
    public static BochaShot Escolher(BochaSimulation mesa, int estoque, System.Random rng)
    {
        var alvos = new List<Vector2> { mesa.bolas[0].pos };
        foreach (var b in mesa.bolas)
            if (b.valida && b.lado == BochaSide.Marcus) alvos.Add(b.pos);
        var melhor = new BochaShot();
        float melhorValor = float.NegativeInfinity;
        foreach (Vector2 alvo in alvos)
            foreach (float x in new[] { -1.8f, 0f, 1.8f })
                foreach (float extra in new[] { -0.16f, 0.14f, 1.8f })
                {
                    Vector2 origem = new Vector2(x, 0.65f), delta = alvo - origem;
                    float baseV = Mathf.Sqrt(2f * 1.85f * delta.magnitude);
                    bool atravessaFaixa = mesa.faixaUmida.width > 0 &&
                        mesa.faixaUmida.Contains(Vector2.Lerp(origem, alvo, 0.62f));
                    float v = Mathf.Clamp(baseV + extra + (atravessaFaixa ? 0.75f : 0f), 2f, 12.5f);
                    var sim = mesa.Clone();
                    sim.bolas.Add(new BochaBall { id = sim.bolas.Count, lado = BochaSide.Tonao,
                        raio = BochaSimulation.RaioBocha, massa = 1f, pos = origem, vel = delta.normalized * v });
                    sim.RepousarParaPrevisao();
                    if (!sim.bolas[0].valida) continue; // Anular não é estratégia preferida.
                    float nossa = BochaMatch.MelhorDistancia(sim, BochaSide.Tonao);
                    float rival = BochaMatch.MelhorDistancia(sim, BochaSide.Marcus);
                    float valor = -Mathf.Min(nossa, 12f) + (nossa < rival ? 2.4f : 0f);
                    if (!float.IsPositiveInfinity(rival)) valor += Mathf.Min(rival, 3f) * 0.45f;
                    foreach (var b in sim.bolas)
                        if (b.valida && b.lado == BochaSide.Tonao && BochaMatch.Distancia(b, sim.bolas[0]) < rival) valor += 0.12f;
                    if (valor <= melhorValor) continue;
                    melhorValor = valor;
                    melhor = new BochaShot { x = x, velocidade = delta.normalized * v,
                        intencao = extra > 1f ? "Tonão prepara uma retirada." : "Tonão procura encostar e proteger a posição." };
                }
        if (melhor.velocidade == Vector2.zero)
            melhor = new BochaShot { x = 0, velocidade = new Vector2(0, 7.4f), intencao = "Tonão mede a cancha." };
        float erroAngulo = ((float)rng.NextDouble() - 0.5f) * (estoque <= 1 ? 1.6f : 2.2f);
        melhor.velocidade = Quaternion.Euler(0, 0, erroAngulo) * melhor.velocidade;
        melhor.velocidade *= 0.975f + (float)rng.NextDouble() * 0.05f;
        return melhor;
    }
}
