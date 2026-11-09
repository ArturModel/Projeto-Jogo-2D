Shader "SantaRita/RuaNorte/Nevoeiro"
{
    Properties
    {
        _Front("Faixa de primeiro plano", Float) = 0
        _Entrance("Rodovia de entrada", Float) = 0
    }
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
            float _Entrance;
            float4 _EntranceLamps[3];
            float4 _Lightning;

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

                float y = input.world.y;
                float strike = 0;
                if (_Front < .5 && _Lightning.z > .30)
                {
                    float level = (y - _Lightning.w) * 1.65;
                    float path = _Lightning.x + lerp(Hash(float2(floor(level), _Lightning.y)),
                        Hash(float2(floor(level) + 1, _Lightning.y)), frac(level)) * .92 - .46;
                    float d = abs(input.world.x - path);
                    strike = smoothstep(0, .25, level) * (1 - smoothstep(.025, .10, d)) * _Lightning.z;
                }
                half3 flash = half3(.79, .88, 1);

                if (_Entrance > .5)
                {
                    float back = smoothstep(14.1, 16.9, y) * (1 - smoothstep(19.3, 22.5, y));
                    float low = smoothstep(9.8, 12.7, y) * (1 - smoothstep(15.5, 18.2, y));
                    float halo = 0;
                    float scatter = 0;
                    if (_Front > .5)
                    {
                        [unroll] for (int i = 0; i < 3; i++)
                        {
                            // A cabeça do poste é desenhada acima da altura do solo.
                            float2 bulb = _Ground.xy + _EntranceLamps[i].xy + float2(0, _EntranceLamps[i].z + .24);
                            float2 delta = input.world - bulb;
                            float glow = 1 - smoothstep(.06, .64, length(delta * float2(1, 1.40)));
                            halo = max(halo, glow * glow);
                            scatter = max(scatter, 1 - smoothstep(.18, 2.65, length(delta * float2(1, .84))));
                        }
                    }
                    half3 fogColor = lerp(half3(.065, .105, .14), half3(.72, .43, .19), saturate(scatter * .88 + halo));
                    fogColor = lerp(fogColor, flash, saturate(strike + _Lightning.z * .62));
                    float fog = density * (_Front > .5 ? low * (.024 + .11 * scatter) : back * .12);
                    return half4(fogColor, limit * saturate(fog + halo * .21 + strike * .92));
                }

                if (_Front > .5)
                {
                    // À frente, fica baixa. Nas laterais, sobe um pouco e contorna as paredes.
                    float height = lerp(.60, 1.35, shoulder);
                    float band = smoothstep(-2.45, -.72, p.y) * (1 - smoothstep(height - .45, height, p.y));
                    float spread = 1 - smoothstep(_Bar.z + .7, _Bar.z + 5.1, abs(p.x));
                    float doorway = 1 - .72 * (1 - smoothstep(.65, 1.6, abs(p.x)));
                    float glow = (1 - smoothstep(.4, 3.2, abs(p.x))) * (1 - smoothstep(.1, 1.9, abs(p.y)));
                    half3 color = lerp(half3(.20, .27, .30), half3(.34, .29, .22), glow);
                    return half4(lerp(color, flash, _Lightning.z * .62), limit * spread * band * density * doorway * (1 - overHouse * .36) * .062);
                }

                // O véu mais largo passa também sobre copas e paredes, com menos densidade na fachada.
                float back = smoothstep(-2.5, 2.3, p.y) * (1 - smoothstep(5.0, 8.0, p.y));
                float flank = 1 - smoothstep(9.0, 16.0, abs(p.x));
                half3 backColor = lerp(half3(.16, .22, .25), flash, saturate(strike + _Lightning.z * .62));
                float opacity = flank * back * density * (1 - overHouse * .42) * .085;
                return half4(backColor, limit * saturate(opacity + strike * .92));
            }
            ENDHLSL
        }
    }
}
