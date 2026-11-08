Shader "SantaRita/RuaNorte/Nevoeiro"
{
    Properties { _Front("Faixa junto à fachada", Float) = 0 }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Ground; // origem XY e extensão
            float4 _Bar;    // base XY, meia largura e profundidade
            float _Front;

            struct Attributes { float3 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 world : TEXCOORD0; };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 world = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(world);
                output.world = world.xy;
                return output;
            }

            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(43.27, 107.13))) * 4375.45);
            }

            float Noise(float2 p)
            {
                float2 cell = floor(p);
                float2 uv = frac(p);
                uv = uv * uv * (3.0 - 2.0 * uv);
                return lerp(lerp(Hash(cell), Hash(cell + float2(1, 0)), uv.x),
                            lerp(Hash(cell + float2(0, 1)), Hash(cell + 1), uv.x), uv.y);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 local = input.world - _Ground.xy;
                float2 border = min(local, _Ground.zw - local);
                float limit = smoothstep(0, 2, min(border.x, border.y));
                float2 p = floor((input.world - _Bar.xy) * 32.0 + .5) / 32.0;
                float t = _Time.y;
                float breeze = t * .31 + sin(t * .73) * .16;
                float2 advected = p - float2(breeze, sin(p.x * .52 - t * .55) * .24);
                float broad = Noise(advected * float2(.37, .91));
                float filament = Noise(advected * float2(.91, 2.5) + 27.4);
                float density = smoothstep(.35, .79, broad * .72 + filament * .28);
                float shoulder = smoothstep(_Bar.z - .35, _Bar.z + .6, abs(p.x));
                float overHouse = (1 - shoulder) * smoothstep(-.1, .28, p.y)
                             * (1 - smoothstep(_Bar.w - .2, _Bar.w + .65, p.y));

                if (_Front > .5)
                {
                    // À frente, fica baixa. Nas laterais, sobe um pouco e contorna as paredes.
                    float height = lerp(.60, 1.35, shoulder);
                    float band = smoothstep(-2.45, -.72, p.y) * (1 - smoothstep(height - .45, height, p.y));
                    float spread = 1 - smoothstep(_Bar.z + .7, _Bar.z + 5.1, abs(p.x));
                    float doorway = 1 - .72 * (1 - smoothstep(.65, 1.6, abs(p.x)));
                    float glow = (1 - smoothstep(.4, 3.2, abs(p.x))) * (1 - smoothstep(.1, 1.9, abs(p.y)));
                    half3 color = lerp(half3(.20, .27, .30), half3(.34, .29, .22), glow);
                    return half4(color, limit * spread * band * density * doorway * (1 - overHouse * .36) * .062);
                }

                // O véu mais largo passa também sobre copas e paredes, com menos densidade na fachada.
                float back = smoothstep(-2.5, 2.3, p.y) * (1 - smoothstep(5.0, 8.0, p.y));
                float flank = 1 - smoothstep(9.0, 16.0, abs(p.x));
                return half4(.16, .22, .25, limit * flank * back * density * (1 - overHouse * .42) * .085);
            }
            ENDHLSL
        }
    }
}
