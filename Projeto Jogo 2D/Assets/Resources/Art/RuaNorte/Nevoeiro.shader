Shader "SantaRita/RuaNorte/Nevoeiro"
{
    Properties
    {
        _Front("Faixa de primeiro plano", Float) = 0
        _Entrance("Rodovia de entrada", Float) = 0
        _BeamStrength("Intensidade do cone", Float) = 1
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
            float _Front;
            float _Entrance;
            float _BeamStrength;
            float4 _FireflyPatches[4];
            float4 _EntranceLamps[9];
            int _EntranceLampCount;
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

            float Fireflies(float2 local, float time)
            {
                const float2 cellSize = float2(1.46, 1.36);
                float2 cell = floor(local / cellSize);
                float2 id = cell + _Ground.xy;
                float seed = Hash(id + 3.9);
                if (seed < .19) return 0;

                float2 jitter = float2(Hash(id + 8.3), Hash(id + 11.7));
                float2 center = (cell + .26 + .48 * jitter) * cellSize;
                bool inside = false;
                [unroll] for (int i = 0; i < 4; i++)
                {
                    float4 patch = _FireflyPatches[i];
                    inside = inside || (all(center >= patch.xy) && all(center < patch.zw));
                }
                if (!inside) return 0;

                float phase = jitter.y * 6.283;
                float pulse = smoothstep(.12, .98, sin(time * lerp(.56, 1.15, seed) + phase));
                float2 drift = float2(sin(time * .83 + phase), cos(time * .67 + phase)) * float2(.15, .13);
                float2 pointAt = (floor((center + drift) * 32) + .5) / 32;
                float d = length(local - pointAt);
                float spark = 1 - smoothstep(.008, .030, d);
                float halo = (1 - smoothstep(.020, .061, d)) * .08;
                return pulse * (spark + halo);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 local = input.world - _Ground.xy;
                float2 border = min(local, _Ground.zw - local);
                float limit = smoothstep(0, 2, min(border.x, border.y));
                float2 p = floor(input.world * 32.0 + .5) / 32.0;
                float t = _Time.y;
                float breeze = t * .31 + sin(t * .73) * .16;
                float2 advected = p - float2(breeze, sin(p.x * .52 - t * .55) * .24);
                float broad = Noise(advected * float2(.37, .91));
                float filament = Noise(advected * float2(.91, 2.5) + 27.4);
                float density = smoothstep(.35, .79, broad * .72 + filament * .28);

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

                if (_Front > .5)
                {
                    float firefly = Fireflies(local, t) * .39;
                    if (_Entrance < .5) return half4(.80, .89, .37, limit * firefly);
                    float halo = 0;
                    float scatter = 0;
                    float core = 0;
                    [loop] for (int i = 0; i < _EntranceLampCount; i++)
                    {
                        float4 lamp = _EntranceLamps[i];
                        if (lamp.w <= 0) continue;
                        float2 bulb = _Ground.xy + lamp.xy;
                        float2 delta = input.world - bulb;
                        float glow = 1 - smoothstep(.035, .41, length(delta * float2(1, 1.55)));
                        halo = max(halo, glow * glow);

                        float down = -delta.y;
                        float width = .13 + pow(max(down, 0), 1.14) * .68;
                        float lateral = (delta.x + down * .06) / width;
                        float edge = exp2(- lateral * lateral * 2.35) * (1 - smoothstep(.18, 1.08, abs(lateral)));
                        float reach = smoothstep(-.10, .06, down) * (1 - smoothstep(1.05, 3.25, down));
                        if (edge * reach <= .001) continue;

                        // A densidade varia em filamentos que abrem a partir do vidro.
                        float rays = Noise(float2(lateral * 4.2 + lamp.x, down * .58 - t * .14));
                        float pockets = lerp(.76, 1.20, smoothstep(.12, .90, rays));
                        scatter = max(scatter, edge * reach * pockets * exp2(-max(down, 0) * .38));

                        // O miolo quente se alarga e perde energia antes das bordas.
                        float shaft = exp2(- lateral * lateral * 5.2) * (1 - smoothstep(.04, .52, abs(lateral + .035 * (rays - .5))));
                        float concentration = exp2(-max(down, 0) * 1.85);
                        core = max(core, shaft * reach * concentration * lerp(.89, 1.08, rays));
                    }

                    float amber = scatter * lerp(.13, .24, density);
                    float golden = core * .39 + halo * .25;
                    float total = amber + golden;
                    half3 fogColor = lerp(half3(1.00, .53, .21), half3(1.42, .92, .46),
                        golden / max(total, .001));
                    fogColor = lerp(fogColor, flash, saturate(strike + _Lightning.z * .62));
                    float opacity = saturate(total * _BeamStrength);
                    half3 color = lerp(fogColor, half3(.80, .89, .37), firefly / max(opacity + firefly, .001));
                    return half4(color, limit * saturate(opacity + firefly));
                }

                return half4(flash, limit * strike * .92);
            }
            ENDHLSL
        }
    }
}
