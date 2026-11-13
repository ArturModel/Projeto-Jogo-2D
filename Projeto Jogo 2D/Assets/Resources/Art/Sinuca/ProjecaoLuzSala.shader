Shader "SantaRita/Sinuca/ProjecaoLuzSala"
{
    Properties
    {
        [MainColor] _BaseColor("Projected Tint", Color) = (.28, .44, .78, .22)
        _Bars("Vertical Bars", Float) = 0
        _CrossH("Horizontal Crossbar", Float) = 1
        _CrossV("Vertical Crossbar", Float) = 1
        _Softness("Edge Softness", Range(.02, .45)) = .16
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-9" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend SrcAlpha One, Zero One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half _Bars, _CrossH, _CrossV, _Softness;
            CBUFFER_END
            struct A { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct V { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            V Vert(A i)
            {
                V o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = i.uv;
                return o;
            }
            half4 Frag(V i) : SV_Target
            {
                float2 u = i.uv;
                half edgeX = smoothstep(0, _Softness, u.x) * smoothstep(1, 1 - _Softness, u.x);
                half edgeNear = smoothstep(0, .14, u.y);
                half edgeFar = smoothstep(1, .28, u.y);
                half mask = edgeX * edgeNear * edgeFar;

                if (_CrossV > .5)
                    mask *= smoothstep(.018, .048, abs(u.x - .5));
                if (_CrossH > .5)
                    mask *= smoothstep(.018, .048, abs(u.y - .46));
                if (_Bars > .5)
                {
                    half barPhase = abs(frac(u.x * _Bars + .5) - .5);
                    mask *= smoothstep(.07, .16, barPhase);
                }
                // Perspective-stretched light stays crisp near sill and softens along the floor.
                return half4(_BaseColor.rgb, _BaseColor.a * mask);
            }
            ENDHLSL
        }
    }
}
