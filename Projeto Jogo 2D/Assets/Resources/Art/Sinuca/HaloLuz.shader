Shader "SantaRita/HaloLuz"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.78, 0.42, 1)
        _Intensity ("Intensity", Float) = 0.55
        _Falloff ("Falloff", Float) = 5.5
        _Core ("Core Boost", Float) = 0.45
        _ViewBias ("Camera Pull Bias", Float) = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+50"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "HaloAdditive"
            Tags { "LightMode" = "UniversalForward" }
            Blend One One
            ZWrite Off
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Intensity;
                float _Falloff;
                float _Core;
                float _ViewBias;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 centerWS = TransformObjectToWorld(float3(0.0, 0.0, 0.0));
                float scaleX = length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x));
                float scaleY = length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y));

                float3 centerVS = TransformWorldToView(centerWS);
                float3 posVS = centerVS + float3(v.positionOS.x * scaleX, v.positionOS.y * scaleY, _ViewBias);
                o.positionCS = TransformWViewToHClip(posVS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 d = i.uv * 2.0 - 1.0;
                float r2 = dot(d, d);
                clip(1.0 - r2);

                float edge = saturate(1.0 - r2);
                edge *= edge;
                float aura = exp(-r2 * _Falloff) * edge;
                float core = exp(-r2 * (_Falloff * 3.0)) * _Core * edge;
                half3 col = _Color.rgb * ((aura + core) * max(0.0, _Intensity));
                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
