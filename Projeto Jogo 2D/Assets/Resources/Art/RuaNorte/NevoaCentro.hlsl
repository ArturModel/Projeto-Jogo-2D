#ifndef SANTA_RITA_NEVOA_CENTRO_INCLUDED
#define SANTA_RITA_NEVOA_CENTRO_INCLUDED

float HashDaNevoaCentro(float2 celula)
{
    celula = frac(celula * float2(.1031, .11369));
    celula += dot(celula, celula.yx + 33.33);
    return frac((celula.x + celula.y) * celula.x);
}

float Ruido2DDaNevoaCentro(float2 posicao)
{
    float2 base = floor(posicao);
    float2 t = frac(posicao);
    t = t * t * (3 - 2 * t);
    float a = HashDaNevoaCentro(base);
    float b = HashDaNevoaCentro(base + float2(1, 0));
    float c = HashDaNevoaCentro(base + float2(0, 1));
    float d = HashDaNevoaCentro(base + float2(1, 1));
    return lerp(lerp(a, b, t.x), lerp(c, d, t.x), t.y);
}

float AreaDaPracaDaMatriz(float2 p)
{
    return 1 - smoothstep(.72, 1.28, length((p - float2(46, 55.5)) / float2(12.5, 17)));
}

float AreaDaNevoaCentro(float2 p)
{
    // Cobertura contínua no mapa inteiro, sem bordas recortadas por rua.
    // A névoa apenas rarefaz de modo orgânico ao redor da praça iluminada e nas casas ao centro.
    float macro = Ruido2DDaNevoaCentro(p * .045 + float2(4.1, 1.7));
    float praca = AreaDaPracaDaMatriz(p);
    return lerp(.74, 1.0, macro) * lerp(1.0, .62, praca);
}

float DensidadeDaNevoaCentro(float3 superficie, float tempo, float intensidade)
{
    float altura = 1 - smoothstep(.18, 2.85, superficie.z);
    if (altura <= .002 || intensidade <= .001) return 0;

    float cobertura = AreaDaNevoaCentro(superficie.xy) * altura;
    if (cobertura <= .002) return 0;

    // Duas oitavas 2D leves com deriva e leve cisalhamento pela altura em Y.
    float2 p = superficie.xy + float2(-tempo * .36, tempo * .11 - superficie.z * .44);
    float macro = Ruido2DDaNevoaCentro(p * .16 + float2(7.3, 2.9));
    float detalhe = Ruido2DDaNevoaCentro(p * .44 + float2(macro * 1.15 - tempo * .08, 5.1));
    float massa = macro * .64 + detalhe * .36;
    float densidade = smoothstep(.24, .88, massa);
    return 1 - exp2(-intensidade * cobertura * (.035 + .68 * densidade));
}

half3 CorDaNevoaCentro(half3 luzLocal)
{
    // Azul-ardósia profundo na sombra para manter contraste cinemático, aquecendo sob cada luminária.
    return half3(.068, .086, .116) + min(luzLocal, 2.00) * half3(.48, .41, .34);
}

#endif
