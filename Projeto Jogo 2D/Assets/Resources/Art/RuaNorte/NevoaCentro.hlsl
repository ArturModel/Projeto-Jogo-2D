#ifndef SANTA_RITA_NEVOA_CENTRO_INCLUDED
#define SANTA_RITA_NEVOA_CENTRO_INCLUDED

float HashDaNevoaCentro(float3 celula)
{
    celula = frac(celula * float3(.1031, .11369, .13787));
    celula += dot(celula, celula.yxz + 19.19);
    return frac((celula.x + celula.y) * celula.z);
}

float RuidoDaNevoaCentro(float3 posicao)
{
    float3 base = floor(posicao);
    float3 t = frac(posicao);
    t = t * t * (3 - 2 * t);
    float frente = lerp(
        lerp(HashDaNevoaCentro(base), HashDaNevoaCentro(base + float3(1, 0, 0)), t.x),
        lerp(HashDaNevoaCentro(base + float3(0, 1, 0)), HashDaNevoaCentro(base + float3(1, 1, 0)), t.x), t.y);
    base.z += 1;
    float fundo = lerp(
        lerp(HashDaNevoaCentro(base), HashDaNevoaCentro(base + float3(1, 0, 0)), t.x),
        lerp(HashDaNevoaCentro(base + float3(0, 1, 0)), HashDaNevoaCentro(base + float3(1, 1, 0)), t.x), t.y);
    return lerp(frente, fundo, t.z);
}

float AreaDaPracaDaMatriz(float2 p)
{
    return 1 - smoothstep(.72, 1.22, length((p - float2(46, 55.5)) / float2(12.5, 17)));
}

float AreaDaNevoaCentro(float2 p)
{
    // O ar cruza o asfalto e perde densidade gradualmente entre os primeiros troncos.
    float oeste = (1 - smoothstep(2.7, 9.4, abs(p.x - 31.5))) *
        smoothstep(19, 25, p.y) * (1 - smoothstep(80, 86, p.y));
    float leste = (1 - smoothstep(2.7, 9.4, abs(p.x - 60.5))) *
        smoothstep(17, 23, p.y) * (1 - smoothstep(77, 83, p.y));
    float travessa = (1 - smoothstep(2.5, 7.4, abs(p.y - 37.5))) *
        smoothstep(26, 32, p.x) * (1 - smoothstep(76, 82, p.x));
    float adro = (1 - smoothstep(.56, 1.15, length((p - float2(46, 58.5)) / float2(15, 12))));
    return max(adro, max(max(oeste, leste), travessa) * .82);
}

float DensidadeDaNevoaCentro(float3 superficie, float tempo, float intensidade)
{
    float cobertura = AreaDaNevoaCentro(superficie.xy) * (1 - smoothstep(.18, 1.48, superficie.z));
    // A praça continua mais limpa que as estradas, mas a bacia e o piso ao redor
    // recebem a mesma camada baixa de ar úmido que atravessa o restante do Centro.
    cobertura *= lerp(1, .72, AreaDaPracaDaMatriz(superficie.xy));
    if (cobertura <= .002) return 0;

    // O receptor em pé encontra o ar que passa na sua frente, antes da sua própria base.
    float3 p = superficie + float3(-tempo * .43, tempo * .13 - superficie.z * .56, 0);
    float desvio = RuidoDaNevoaCentro(p * float3(.15, .20, .7) + 27.2) - .5;
    p.xy += desvio * float2(2.4, 1.6);
    p *= float3(.33, .53, 1.48);

    // Três alturas da camada baixa. O deslocamento em Y acompanha a vista oblíqua do jogo.
    float perto = RuidoDaNevoaCentro(p + float3(0, -.16, .28));
    float meio = RuidoDaNevoaCentro(p + float3(.08, -.38, .72));
    float alto = RuidoDaNevoaCentro(p + float3(.22, -.66, 1.29));
    float massa = perto * .52 + meio * .32 + alto * .16;
    float borda = RuidoDaNevoaCentro(p * 2.27 + 11.6);
    float densidade = smoothstep(.22, .83, massa * .82 + borda * .18);
    return 1 - exp2(-intensidade * cobertura * (.055 + .84 * densidade));
}

half3 CorDaNevoaCentro(half3 luzLocal)
{
    // Cinza frio no escuro, com a cor de cada luminária já atenuada pela sua sombra.
    return half3(.123, .139, .153) + min(luzLocal, 1.45) * half3(.38, .37, .36);
}

#endif
