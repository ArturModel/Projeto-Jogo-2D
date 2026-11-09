#ifndef SANTA_RITA_SPRITE_SURFACE_INCLUDED
#define SANTA_RITA_SPRITE_SURFACE_INCLUDED

#ifndef SPRITE_SURFACE_ALBEDO
#define SPRITE_SURFACE_ALBEDO _MainTex
#define SPRITE_SURFACE_SAMPLER sampler_MainTex
#endif

TEXTURE2D(_SurfaceMap);
SAMPLER(sampler_SurfaceMap);

struct SpriteSurface
{
    float2 slope;
    float roughness;
    float metallic;
};

float SurfaceHeight(float4 neighbor, float4 center)
{
    // Ignore RGB in transparent padding and leave a small bevel at the silhouette.
    return dot(lerp(center.rgb, neighbor.rgb, neighbor.a), float3(.299, .587, .114))
        + .15 * neighbor.a;
}

SpriteSurface ReadSpriteSurface(float2 uv, float2 texel, float4 albedo)
{
    float4 packed = SAMPLE_TEXTURE2D(_SurfaceMap, sampler_SurfaceMap, uv);
    SpriteSurface result;
    result.slope = packed.rg * 2 - 1;
    result.roughness = packed.b;
    result.metallic = packed.a;

    // White means no secondary texture, as with animation sheets created at runtime.
    [branch] if (packed.a > .98)
    {
        float2 x = float2(texel.x, 0);
        float2 y = float2(0, texel.y);
        float l = SurfaceHeight(SAMPLE_TEXTURE2D(SPRITE_SURFACE_ALBEDO, SPRITE_SURFACE_SAMPLER, uv - x), albedo);
        float r = SurfaceHeight(SAMPLE_TEXTURE2D(SPRITE_SURFACE_ALBEDO, SPRITE_SURFACE_SAMPLER, uv + x), albedo);
        float b = SurfaceHeight(SAMPLE_TEXTURE2D(SPRITE_SURFACE_ALBEDO, SPRITE_SURFACE_SAMPLER, uv - y), albedo);
        float t = SurfaceHeight(SAMPLE_TEXTURE2D(SPRITE_SURFACE_ALBEDO, SPRITE_SURFACE_SAMPLER, uv + y), albedo);
        result.slope = float2(l - r, b - t) * .42;
        result.roughness = lerp(.88, .66, dot(albedo.rgb, float3(.299, .587, .114)));
        result.metallic = 0;
    }

    return result;
}

float3 SpriteSpecular(float3 normal, float3 lightDir, float3 viewDir, float roughness, float3 f0)
{
    float3 halfway = SafeNormalize(lightDir + viewDir);
    float nh = saturate(dot(normal, halfway));
    float nv = saturate(dot(normal, viewDir));
    float nl = saturate(dot(normal, lightDir));
    float vh = saturate(dot(viewDir, halfway));
    float a2 = roughness * roughness;
    a2 *= a2;
    float d = nh * nh * (a2 - 1) + 1;
    float distribution = a2 / max(3.14159 * d * d, .001);
    float k = (roughness + 1) * (roughness + 1) * .125;
    float visibility = rcp(max(4 * lerp(k, 1, nl) * lerp(k, 1, nv), .001));
    float3 fresnel = f0 + (1 - f0) * pow(1 - vh, 5);
    return min(.65, distribution * visibility * fresnel) * nl;
}

#endif
