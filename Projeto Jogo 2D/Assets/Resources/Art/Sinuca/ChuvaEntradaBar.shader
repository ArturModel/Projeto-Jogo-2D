Shader "SantaRita/Bar/ChuvaDaEntrada"
{
    Properties
    {
        _Tempo ("Tempo da entrada", Float) = 0
        _ChuvaContinua ("Relógio do mundo", Float) = 0
        _Luz ("Luz sobre a porta", Vector) = (.40, 2.08, -7.64, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha, Zero One

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Tempo, _ChuvaContinua;
                float4 _Luz;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 canto : TEXCOORD0;
                float4 gota : TEXCOORD1; // velocidade, altura do traço, largura, intensidade
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 cor : COLOR;
                half cauda : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                float3 p = v.positionOS;
                float tempo = lerp(_Tempo, _Time.y, _ChuvaContinua);
                float altura = frac(p.y - tempo * v.gota.x) * 3.55;
                p.x -= altura * .18 + v.canto.y * v.gota.y * .18;
                p.x += v.canto.x * v.gota.z;
                p.y = altura + v.canto.y * v.gota.y;

                float3 mundo = TransformObjectToWorld(p);
                o.positionCS = TransformWorldToHClip(mundo);
                float distancia = distance(mundo, GetCameraPositionWS());
                half visibilidade = smoothstep(.22, .53, distancia) * smoothstep(0, .11, altura);
                half luz = 1.0h - smoothstep(.25, 2.25, distance(p, _Luz.xyz));
                o.cor = half4(lerp(half3(.44, .57, .68), half3(.83, .69, .46), luz),
                    v.gota.w * visibilidade * lerp(.68h, 1.0h, luz));
                o.cauda = 1.0h - v.canto.y;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                return half4(i.cor.rgb, i.cor.a * i.cauda);
            }
            ENDHLSL
        }
    }
}
