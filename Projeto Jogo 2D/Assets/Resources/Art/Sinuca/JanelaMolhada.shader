Shader "SantaRita/Sinuca/JanelaMolhada"
{
    Properties
    {
        _RainAmount("Intensidade da chuva", Range(0, 1)) = .66
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent+15" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half _RainAmount;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 ray : TEXCOORD1; };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.ray = TransformWorldToObject(GetCameraPositionWS()) - input.positionOS.xyz;
                return output;
            }

            float Hash(float2 id)
            {
                float3 p = frac(id.xyx * .1031);
                p += dot(p, p.yzx + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 grid = input.uv * float2(17,10);
                float2 id = floor(grid);
                float seed = Hash(id);
                float phase = frac(_Time.y * (.20 + .10 * seed) + seed * 7.17);
                float slide = smoothstep(.28,.92,phase);
                float2 p = frac(grid) - float2(.20 + .60 * Hash(id + 9), .83 - .62 * slide);
                float2 drop = p * float2(1.25, lerp(1.7,.65,slide));
                half life = smoothstep(.02,.12,phase) * (1 - smoothstep(.80,1,phase));
                half rain = life * step(.80,seed) * _RainAmount * saturate(1.65 - 56 * max(fwidth(input.uv.x), fwidth(input.uv.y)));
                half water = (1 - smoothstep(.045,.12,length(drop))) * rain;
                half brilho = (1 - smoothstep(.014,.052,length(drop - float2(-.034,.047)))) * rain;
                half edge = smoothstep(.31,.50,max(abs(input.uv.x-.5),abs(input.uv.y-.5)));
                half reflexo = pow(1 - abs(normalize(input.ray).z), 3);
                half3 cor = lerp(half3(.04,.053,.054),half3(.38,.31,.23),saturate(brilho*2 + reflexo*.2));
                return half4(cor,.05 + edge*.11 + reflexo*.09 + water*.26 + brilho*.42);
            }
            ENDHLSL
        }
    }
}
