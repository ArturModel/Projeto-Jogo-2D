Shader "SantaRita/Exterior/Nevoa3D"
{
    Properties
    {
        _PosteA("Poste esquerdo", Vector) = (0,4,0,0)
        _PosteB("Poste direito", Vector) = (0,4,0,0)
        _ForcaPostes("Espalhamento âmbar", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-15" "RenderType"="Transparent" }
        Cull Back
        ZTest LEqual
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Pass
        {
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _PosteA, _PosteB;
                float _ForcaPostes;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            float Feixe(float3 p, float3 lamp)
            {
                float3 d = p - lamp;
                float h = -d.y;
                float abertura = .20 + max(h, 0) * .67;
                float perfil = 1 - smoothstep(.14, 1, length(d.xz) / abertura);
                return perfil * smoothstep(0, .12, h) * (1 - smoothstep(2.1, 4.5, h));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 origem = GetCameraPositionWS();
                float3 raio = normalize(input.positionWS - origem);
                float3 ro = TransformWorldToObject(origem);
                float3 rd = mul((float3x3)UNITY_MATRIX_I_M, raio);
                float3 inv = (step(0, rd) * 2 - 1) / max(abs(rd), .00001);
                float3 a = (-.5 - ro) * inv, b = (.5 - ro) * inv;
                float3 mins = min(a,b), maxs = max(a,b);
                float inicio = max(0, max(mins.x, max(mins.y,mins.z)));
                float fim = min(maxs.x, min(maxs.y,maxs.z));
                float2 uv = input.positionCS.xy / _ScaledScreenParams.xy;
                float depth = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
                float conversao = max(.01, -mul((float3x3)UNITY_MATRIX_V, raio).z);
                fim = min(fim, depth / conversao);
                clip(fim - inicio - .001);
                float passo = (fim - inicio) / 16.0;
                half transparencia = 1;
                half3 cor = 0;
                [loop] for (int i = 0; i < 16; i++)
                {
                    float3 p = origem + raio * (inicio + (i + .5) * passo);
                    float h = mul(UNITY_MATRIX_I_M, float4(p,1)).y + .5;
                    float ondula = sin(p.x*.63 + p.z*.87 + _Time.y*.31)
                        * cos(p.x*.27 - p.z*.38 - _Time.y*.25);
                    float d = (.015 + .051 * exp(-max(0,h)*9)) * (.81 + .19 * ondula);
                    float lamp = saturate(Feixe(p, _PosteA.xyz) + Feixe(p, _PosteB.xyz)) * _ForcaPostes;
                    half a = 1 - exp(-passo * (d + lamp*.14));
                    half3 luz = lerp(half3(.075,.091,.105), half3(.68,.39,.17), saturate(lamp*1.75));
                    cor += transparencia * a * luz;
                    transparencia *= 1 - a;
                }
                return half4(cor, 1-transparencia);
            }
            ENDHLSL
        }
    }
}
