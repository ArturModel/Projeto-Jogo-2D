Shader "SantaRita/Cap2/LuzSoleira2D"
{
    Properties
    {
        _Color ("Cor da Luz", Color) = (1, 0.72, 0.36, 0.55)
        _Open ("Abertura da Porta", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha One

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            half4 _Color;
            float _Open;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(worldPos);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // uv.y = 1 na soleira da porta, 0 na calçada abaixo
                float dist = 1.0 - input.uv.y;
                float qDist = floor(dist * 24.0) / 24.0;
                float halfWidth = lerp(0.24, 0.48, qDist) * lerp(0.35, 1.0, _Open);
                float xOff = abs(input.uv.x - 0.5);
                float cone = 1.0 - smoothstep(halfWidth * 0.55, halfWidth, xOff);
                float falloff = pow(saturate(1.0 - qDist), 1.45);
                float crackMask = (1.0 - smoothstep(0.0, 0.08, dist)) * (1.0 - smoothstep(0.16, 0.25, xOff));
                float crack = crackMask * smoothstep(0.02, 0.20, _Open) * 0.18;
                float intensity = max(crack, cone * falloff * _Open);
                return half4(_Color.rgb, _Color.a * intensity);
            }
            ENDHLSL
        }
    }
}
