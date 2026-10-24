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
    // Base da esfera acima do saibro; o centro inclui o raio.
    public float altura, vertical;
    public Vector3 giro;
    public Quaternion rotacao = Quaternion.identity;
    public bool valida = true;
    public BochaBall Clone() => (BochaBall)MemberwiseClone();
    public Vector3 Centro => new Vector3(pos.x, altura + raio, pos.y);
    public Vector3 Velocidade => new Vector3(vel.x, vertical, vel.y);
    public static BochaBall Balim(Vector2 pos) => new BochaBall
        { id = 0, lado = BochaSide.Nenhum, pos = pos, raio = .105f, massa = .32f };
}

public enum BochaContactType { Bochas, Madeira, Saibro }
public struct BochaContact
{
    public BochaContactType tipo;
    public Vector3 ponto;
    public float velocidade;
}

// Kernel único da partida e IA. Contato varrido entre esferas, guias e piso;
// não depende do FPS, de Physics2D global ou da projeção usada para desenhar.
public sealed class BochaSimulation
{
    public const float MeiaLargura = 3f, Comprimento = 20f, RaioBocha = .18f;
    public const float Passo = 1f / 120f, VelocidadeMaxima = 14f;
    public const float ResistenciaRolagem = .92f, Gravidade = 9.81f;
    private const float Epsilon = .00001f;
    public readonly List<BochaBall> bolas = new List<BochaBall>();
    public Action<float> Impacto;
    public Action<BochaContact> Contato;
    public bool IsMoving
    {
        get
        {
            foreach (var b in bolas)
                if (b.valida && (b.vel.sqrMagnitude > .000025f || b.altura > .0001f || Mathf.Abs(b.vertical) > .01f)) return true;
            return false;
        }
    }
    public BochaSimulation Clone()
    {
        var copy = new BochaSimulation();
        foreach (var b in bolas) copy.bolas.Add(b.Clone());
        return copy; // Previsão nunca reproduz áudio/efeitos da partida.
    }
    public void Step(float dt = Passo)
    {
        if (float.IsNaN(dt) || float.IsInfinity(dt) || dt < 0 || dt > .25f)
            throw new ArgumentOutOfRangeException(nameof(dt));
        while (dt > .0000001f)
        {
            float h = Mathf.Min(dt, Passo);
            CorrigirContatos(); Forcas(h * .5f); AvancarComContatos(h); Forcas(h * .5f); CorrigirContatos();
            dt -= h;
        }
    }
    private void Forcas(float dt)
    {
        foreach (var b in bolas)
        {
            if (!b.valida) continue;
            if(b.altura==0&&b.vertical==0&&b.vel==Vector2.zero&&b.giro==Vector3.zero)continue;
            if (b.altura > Epsilon || b.vertical > .001f)
            {
                b.vertical -= Gravidade * dt; b.vel *= Mathf.Exp(-.015f * dt); continue;
            }
            b.altura = b.vertical = 0;
            // Atrito do ponto de contato converte deslizamento em rolamento,
            // considerando I=2/5 mr², sem corrigir a mira depois de lançar.
            Vector3 slip = b.Velocidade + Vector3.Cross(b.giro, Vector3.down * b.raio); slip.y = 0;
            if (slip.sqrMagnitude > .000001f)
            {
                float magnitude = Mathf.Min(2.8f * b.massa * dt, slip.magnitude * b.massa / 3.5f);
                Aplicar(b, -slip.normalized * magnitude, Vector3.down * b.raio);
            }
            if (b.vel.sqrMagnitude > 0)
            {
                b.vel = Vector2.MoveTowards(b.vel, Vector2.zero, ResistenciaRolagem * dt);
                if (slip.sqrMagnitude < .0025f) b.giro = new Vector3(b.vel.y / b.raio, b.giro.y, -b.vel.x / b.raio);
            }
            b.giro.y = Mathf.MoveTowards(b.giro.y, 0, dt * 3);
            if (b.vel.sqrMagnitude < .000025f) { b.vel = Vector2.zero; b.giro = Vector3.zero; }
        }
    }
    private struct Evento { public float tempo; public int a, b, eixo; public float sentido; }
    private void AvancarComContatos(float remaining)
    {
        // Resolver no instante de contato, antes do restante do passo, impede
        // que uma raspada estreita atravesse a esfera entre duas amostras.
        for (int iteration = 0; remaining > .0000001f && iteration < 96; iteration++)
        {
            Evento next = new Evento { tempo = remaining + 1, a = -1, b = -1 };
            for (int i = 0; i < bolas.Count; i++)
            {
                var b = bolas[i]; if (!b.valida) continue;
                TestarPlano(ref next, i, b.pos.x, b.vel.x, -MeiaLargura + b.raio, 0, 1, remaining);
                TestarPlano(ref next, i, b.pos.x, b.vel.x, MeiaLargura - b.raio, 0, -1, remaining);
                TestarPlano(ref next, i, b.pos.y, b.vel.y, b.raio, 2, 1, remaining);
                TestarPlano(ref next, i, b.pos.y, b.vel.y, Comprimento - b.raio, 2, -1, remaining);
                if (b.vertical < -.001f) TestarPlano(ref next, i, b.altura, b.vertical, 0, 1, 1, remaining);
                for (int j = i + 1; j < bolas.Count; j++)
                {
                    var other = bolas[j]; if (!other.valida) continue;
                    if((other.vel-b.vel).sqrMagnitude+(other.vertical-b.vertical)*(other.vertical-b.vertical)<1e-10f)continue;
                    Vector3 p = other.Centro - b.Centro, v = other.Velocidade - b.Velocidade;
                    float vv = v.sqrMagnitude, pv = Vector3.Dot(p, v), r = b.raio + other.raio;
                    if (vv < 1e-10f || pv >= 0) continue;
                    float c = p.sqrMagnitude - r * r, discriminant = pv * pv - vv * c;
                    if (discriminant < 0) continue;
                    float t = Mathf.Max(0, (-pv - Mathf.Sqrt(discriminant)) / vv);
                    if (t <= remaining && t < next.tempo) next = new Evento { tempo = t, a = i, b = j };
                }
            }
            if (next.a < 0) { Mover(remaining); return; }
            Mover(next.tempo); remaining -= next.tempo;
            if (next.b >= 0) Colidir(bolas[next.a], bolas[next.b]); else Rebater(bolas[next.a], next.eixo, next.sentido);
        }
        // Pilhas num canto têm um limite de iterações; a correção geométrica
        // evita explosão numérica/espera infinita sem invalidar nenhuma bola.
        if (remaining > 0) Mover(remaining);
    }
    private static void TestarPlano(ref Evento e, int i, float p, float v, float limite, int eixo, float sentido, float restante)
    {
        if (v * sentido >= -.00001f) return;
        float t = Mathf.Max(0, (limite - p) / v);
        if (t <= restante && t < e.tempo) e = new Evento { a = i, b = -1, eixo = eixo, sentido = sentido, tempo = t };
    }
    private void Mover(float dt)
    {
        foreach (var b in bolas)
        {
            if (!b.valida) continue;
            b.pos += b.vel * dt; b.altura = Mathf.Max(0, b.altura + b.vertical * dt);
            float angle = b.giro.magnitude * dt * Mathf.Rad2Deg;
            if (angle > .0001f) b.rotacao = Quaternion.AngleAxis(angle, b.giro.normalized) * b.rotacao;
        }
    }
    private static void Velocidade(BochaBall b, Vector3 v) { b.vel = new Vector2(v.x, v.z); b.vertical = v.y; }
    private static void Posicao(BochaBall b, Vector3 p) { b.pos = new Vector2(p.x, p.z); b.altura = Mathf.Max(0, p.y - b.raio); }
    private static void Aplicar(BochaBall b, Vector3 impulse, Vector3 arm)
    {
        Velocidade(b, b.Velocidade + impulse / b.massa);
        b.giro += Vector3.Cross(arm, impulse) / (.4f * b.massa * b.raio * b.raio);
    }
    private void Colidir(BochaBall a, BochaBall b)
    {
        Vector3 n = (b.Centro - a.Centro).normalized; if (n.sqrMagnitude < .5f) n = Vector3.forward;
        float closing = Vector3.Dot(b.Velocidade - a.Velocidade, n); if (closing >= -.00001f) return;
        float inverse = 1 / a.massa + 1 / b.massa;
        float impulse = -(1 + .68f) * closing / inverse;
        Aplicar(a, -n * impulse, n * a.raio); Aplicar(b, n * impulse, -n * b.raio);
        Vector3 contact = (b.Velocidade + Vector3.Cross(b.giro, -n * b.raio)) - (a.Velocidade + Vector3.Cross(a.giro, n * a.raio));
        Vector3 tangent = contact - n * Vector3.Dot(contact, n);
        if (tangent.sqrMagnitude > 1e-9f)
        {
            Vector3 friction = -tangent.normalized * Mathf.Min(tangent.magnitude / (3.5f * inverse), impulse * .14f);
            Aplicar(a, -friction, n * a.raio); Aplicar(b, friction, -n * b.raio);
        }
        Notificar(BochaContactType.Bochas, a.Centro + n * a.raio, -closing);
    }
    private void Rebater(BochaBall b, int eixo, float sentido)
    {
        Vector3 v = b.Velocidade; float impact = -v[eixo] * sentido; if (impact <= 0) return;
        if (eixo == 1)
        {
            b.altura = 0; v.y = impact > 1 ? impact * .19f : 0; if (v.y < .28f) v.y = 0; Velocidade(b, v);
            Vector3 slip = b.Velocidade + Vector3.Cross(b.giro, Vector3.down * b.raio); slip.y = 0;
            if (slip.sqrMagnitude > 1e-9f)
                Aplicar(b, -slip.normalized * Mathf.Min(slip.magnitude * b.massa / 3.5f, impact * b.massa * .28f), Vector3.down * b.raio);
            Notificar(BochaContactType.Saibro, b.Centro - Vector3.up * b.raio, impact);
        }
        else
        {
            v[eixo] = sentido * impact * (eixo == 0 ? .48f : .54f); Velocidade(b, v); b.giro *= .82f;
            Notificar(BochaContactType.Madeira, b.Centro, impact);
        }
    }
    private void Notificar(BochaContactType tipo, Vector3 p, float v)
    {
        if (v < .06f) return;
        Impacto?.Invoke(v); Contato?.Invoke(new BochaContact { tipo = tipo, ponto = p, velocidade = v });
    }
    private void CorrigirContatos()
    {
        for (int iteration = 0; iteration < 4; iteration++)
        {
            bool overlap=false;
            foreach (var b in bolas)
            {
                if (!b.valida) continue;
                b.pos = new Vector2(Mathf.Clamp(b.pos.x, -MeiaLargura + b.raio, MeiaLargura - b.raio), Mathf.Clamp(b.pos.y, b.raio, Comprimento - b.raio));
                if (b.altura <= Epsilon && b.vertical < 0) { b.altura = 0; b.vertical = 0; }
            }
            for (int i = 0; i < bolas.Count; i++) for (int j = i + 1; j < bolas.Count; j++)
            {
                var a = bolas[i]; var b = bolas[j]; if (!a.valida || !b.valida) continue;
                Vector3 d = b.Centro - a.Centro; float r = a.raio + b.raio, length = d.magnitude;
                if (length >= r - .00001f) continue;
                overlap=true;
                Vector3 n = length > Epsilon ? d / length : Vector3.forward;
                float invA = 1 / a.massa, invB = 1 / b.massa;
                Vector3 correction = n * (r - length + .00001f) / (invA + invB);
                Posicao(a, a.Centro - correction * invA); Posicao(b, b.Centro + correction * invB);
            }
            if(!overlap)break;
        }
    }
    public void RepousarParaPrevisao() { for (int i = 0; i < 4200 && IsMoving; i++) Step(); }
}
