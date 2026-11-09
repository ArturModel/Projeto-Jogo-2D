Shader "Hidden/SantaRita/RuaNorte/ProjetarRecorte"
{
    SubShader
    {
        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always
            Blend One One
            BlendOp Max
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_ShadowSprite);
            SAMPLER(sampler_ShadowSprite);
            float4 _ShadowUV;
            float4 _ShadowFoot;
            float4 _ShadowHeight;
            float4 _ShadowArea;
            float4 _ShadowLamp;
            float4 _ShadowChannel;

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
            };

            Varyings Vert(float3 vertex : POSITION)
            {
                Varyings output;
                float height = vertex.y * _ShadowHeight.x;
                float w = max(_ShadowLamp.z - height, .14 * _ShadowLamp.z);
                float2 basePoint = float2(lerp(_ShadowFoot.x, _ShadowFoot.y, vertex.x),
                    _ShadowFoot.z + vertex.y * _ShadowFoot.w);
                // Interseção do raio lâmpada–sprite com z = 0 em coordenadas homogêneas.
                float2 ground = _ShadowLamp.xy * w + (basePoint - _ShadowLamp.xy) * _ShadowLamp.z;
                float2 clip = ground * (2 / _ShadowArea.xy) - w;
                #if UNITY_UV_STARTS_AT_TOP
                    clip.y = -clip.y;
                #endif
                output.positionCS = float4(clip, 0, w);
                output.uv = _ShadowUV.xy + vertex.xy * _ShadowUV.zw;
                output.height = height;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(_ShadowSprite, sampler_ShadowSprite, input.uv).a;
                float withinLight = 1 - smoothstep(.76, .89, input.height / _ShadowLamp.z);
                return _ShadowChannel * (smoothstep(.28, .81, alpha) * withinLight * _ShadowHeight.y);
            }
            ENDHLSL
        }
    }
}
