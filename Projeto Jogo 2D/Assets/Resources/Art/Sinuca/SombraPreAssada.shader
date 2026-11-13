Shader "SantaRita/Sinuca/SombraPreAssada"
{
    Properties
    {
        [MainTexture] _BaseMap("Baked Shadow Map", 2D) = "white" {}
        _Strength("Shadow Darkness", Range(0, 1)) = .88
        _Radial("Use Radial Falloff", Float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-10" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend DstColor Zero, Zero One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half _Strength, _Radial;
            CBUFFER_END
            struct A { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct V { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            V Vert(A i)
            {
                V o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = TRANSFORM_TEX(i.uv, _BaseMap);
                return o;
            }
            half4 Frag(V i) : SV_Target
            {
                half a;
                if (_Radial > .5)
                {
                    float2 d = (i.uv - .5) * 2;
                    half falloff = saturate(1 - dot(d, d));
                    a = falloff * falloff;
                }
                else
                {
                    a = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).a;
                }
                half k = saturate(a * _Strength);
                half3 mult = lerp(half3(1, 1, 1), half3(.14, .14, .15), k);
                return half4(mult, 1);
            }
            ENDHLSL
        }
    }
}
