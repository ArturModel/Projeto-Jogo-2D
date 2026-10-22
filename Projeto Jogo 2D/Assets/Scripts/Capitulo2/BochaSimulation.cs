using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class BochaBall
{
    public int id;
    public BochaSide lado;
    public Vector2 pos, vel;
    public float raio, massa;
    public bool valida = true;
    public BochaBall Clone() => (BochaBall)MemberwiseClone();
    public static BochaBall Balim(Vector2 pos) => new BochaBall
        { id = 0, lado = BochaSide.Nenhum, pos = pos, raio = 0.105f, massa = 0.32f };
}

// Um único kernel para a partida e para as previsões da IA. Não altera Physics2D global.
public sealed class BochaSimulation
{
    public const float MeiaLargura = 3f, Comprimento = 20f, RaioBocha = 0.18f;
    public const float Passo = 1f / 120f, VelocidadeMaxima = 14f;
    public readonly List<BochaBall> bolas = new List<BochaBall>();
    public Rect faixaUmida;
    public Action<float> Impacto;
    public bool IsMoving
    {
        get { foreach (var b in bolas) if (b.valida && b.vel.sqrMagnitude > 0.0004f) return true; return false; }
    }

    public BochaSimulation Clone()
    {
        var copia = new BochaSimulation { faixaUmida = faixaUmida };
        foreach (var b in bolas) copia.bolas.Add(b.Clone());
        return copia;
    }

    public void Step(float dt = Passo)
    {
        foreach (var b in bolas)
        {
            if (!b.valida) continue;
            float atrito = faixaUmida.width > 0 && faixaUmida.Contains(b.pos) ? 3.2f : 1.85f;
            Vector2 antes = b.vel;
            b.vel = Vector2.MoveTowards(b.vel, Vector2.zero, atrito * dt);
            b.pos += (antes + b.vel) * (0.5f * dt);
            ConterNaCancha(b);
        }
        // Duas passagens resolvem contatos múltiplos sem ruído aleatório.
        for (int iter = 0; iter < 2; iter++)
        {
            for (int i = 0; i < bolas.Count; i++)
                for (int j = i + 1; j < bolas.Count; j++) Colidir(bolas[i], bolas[j]);
            // Uma colisão pode empurrar a vizinha para dentro da parede.
            foreach (var b in bolas) if (b.valida) ConterNaCancha(b);
        }
    }

    private void ConterNaCancha(BochaBall b)
    {
        float x = MeiaLargura - b.raio;
        if (b.pos.x < -x) { b.pos.x = -x; Rebater(ref b.vel.x, 1, .22f); }
        if (b.pos.x > x) { b.pos.x = x; Rebater(ref b.vel.x, -1, .22f); }
        // Cabeceiras de madeira: bochas e balim permanecem na mão e voltam.
        if (b.pos.y < b.raio) { b.pos.y = b.raio; Rebater(ref b.vel.y, 1, .48f); }
        if (b.pos.y > Comprimento - b.raio)
        { b.pos.y = Comprimento - b.raio; Rebater(ref b.vel.y, -1, .48f); }
    }

    private void Rebater(ref float velocidade, int sentidoInterior, float restituicao)
    {
        if (velocidade * sentidoInterior >= 0) return;
        float impacto = Mathf.Abs(velocidade);
        velocidade = sentidoInterior * impacto * restituicao;
        if (impacto > .18f) Impacto?.Invoke(impacto);
    }

    private void Colidir(BochaBall a, BochaBall b)
    {
        if (!a.valida || !b.valida) return;
        Vector2 delta = b.pos - a.pos;
        float raios = a.raio + b.raio, d2 = delta.sqrMagnitude;
        if (d2 >= raios * raios) return;
        float d = Mathf.Sqrt(d2);
        Vector2 n = d > 0.00001f ? delta / d : Vector2.up;
        float invA = 1f / a.massa, invB = 1f / b.massa;
        float sobreposicao = raios - d + 0.0001f;
        a.pos -= n * sobreposicao * invA / (invA + invB);
        b.pos += n * sobreposicao * invB / (invA + invB);
        float relativa = Vector2.Dot(b.vel - a.vel, n);
        if (relativa >= 0f) return;
        float impulso = -(1f + 0.32f) * relativa / (invA + invB);
        a.vel -= impulso * invA * n;
        b.vel += impulso * invB * n;
        if (relativa < -0.18f) Impacto?.Invoke(-relativa);
    }

    public void RepousarParaPrevisao()
    {
        for (int i = 0; i < 3000 && IsMoving; i++) Step();
    }
}
