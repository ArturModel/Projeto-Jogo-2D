Shader "SantaRita/Sinuca/JanelaMolhada"
{
    Properties
    {
        [MainTexture] _BaseMap("Atlas exterior", 2D) = "black" {}
        _ViewRow("Metade do atlas", Float) = 0
        _RainAmount("Intensidade da chuva", Range(0, 1)) = .66
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "Vidro"
            Tags { "LightMode"="UniversalForward" }
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_TexelSize;
                float _ViewRow;
                half _RainAmount;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 rayOS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.rayOS = TransformWorldToObject(GetCameraPositionWS()) - input.positionOS.xyz;
                output.uv = input.uv;
                return output;
            }

            half4 SampleLayer(float2 uv, float column)
            {
                // Clamp each tile independently so grazing angles never bleed into the other window.
                float2 inset = .5 * _BaseMap_TexelSize.xy;
                float2 atlasUV = float2(column, _ViewRow) * .5 + lerp(inset, .5 - inset, saturate(uv));
                return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, atlasUV);
            }

            float Hash(float2 cell)
            {
                float3 p = frac(cell.xyx * .1031);
                p += dot(p, p.yzx + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            half FallingRain(float2 uv, float time)
            {
                float2 grid = float2(uv.x + uv.y * .10, uv.y) * float2(36, 7);
                float column = floor(grid.x);
                grid.y += time * (5.2 + 3.2 * Hash(float2(column, 9.7)));
                float seed = Hash(floor(grid) + 11.3);
                float2 cell = frac(grid);
                float width = 1 - smoothstep(.018, .072, abs(cell.x - .5));
                float streak = smoothstep(.04, .14, cell.y) *
                    (1 - smoothstep(.16, lerp(.25, .48, seed), cell.y));
                return width * streak * step(.74, seed);
            }

            half2 GlassDrops(float2 uv, float time)
            {
                float2 grid = uv * float2(13, 9);
                float2 id = floor(grid);
                float seed = Hash(id + 32.9);
                float phase = frac(time * (.24 + .10 * seed) + seed * 7.17);
                float slide = smoothstep(.38, .93, phase);
                float2 p = frac(grid) - float2(.22 + .55 * Hash(id + 2.8), .77 - .56 * slide);
                float2 drop = p * float2(1.35, lerp(1.8, .78, slide));
                float life = smoothstep(.03, .13, phase) * (1 - smoothstep(.86, 1, phase));
                float wet = life * step(.81, seed);

                half body = 1 - smoothstep(.060, .117, length(drop));
                half shine = 1 - smoothstep(.016, .056, length(drop - float2(-.034, .041)));
                half trail = (1 - smoothstep(.012, .033, abs(p.x))) *
                    smoothstep(.06, .12, p.y) * (1 - smoothstep(.20, .45, p.y)) * slide;
                return half2(body + .4 * trail, shine + .27 * trail) * wet;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 angle = clamp(input.rayOS.xy / max(.23, abs(input.rayOS.z)), -1.30, 1.30);
                float2 farUV = .5 + (uv - .5) * float2(.60, .76) - angle * float2(.151, .091);
                float2 nearUV = .5 + (uv - .5) * float2(.88, .91) - angle * float2(.045, .030);

                half3 distanceColor = SampleLayer(farUV, 0).rgb;
                half4 foreground = SampleLayer(nearUV, 1);
                float time = _Time.y + _ViewRow * 8.2;
                float rainVisibility = saturate(1.65 - 54 * max(fwidth(uv.x), fwidth(uv.y)));
                half rain = _RainAmount * rainVisibility;
                distanceColor += half3(.045, .059, .079) * FallingRain(farUV, time) * rain;
                half3 scene = lerp(distanceColor, foreground.rgb * .82, foreground.a);
                half2 drops = GlassDrops(uv, time) * rain;

                // Dim the recess and keep a narrow, subdued glint on the water.
                half edge = smoothstep(.29, .51, max(abs(uv.x - .5), abs(uv.y - .5)));
                scene *= (.71 - .11 * edge) * (1 - .23 * drops.x);
                scene += half3(.083, .109, .128) * drops.y;
                return half4(scene, 1);
            }
            ENDHLSL
        }
    }
}
