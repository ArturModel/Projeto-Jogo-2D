Shader "SantaRita/Sinuca/Entrada"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Grao ("Madeira do bar", 2D) = "gray" {}
        _Foco ("Foco, aspecto e alcance", Vector) = (.5, .5, 1.78, 1)
        _Progresso ("Progresso", Range(0, 1)) = 0
        [HideInInspector] _MascaraBar ("Máscara do bar", Float) = 0
        [HideInInspector] _MascaraPosto ("Névoa do posto", Float) = 0
        [HideInInspector] _TempoNevoa ("Tempo da névoa", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            float4 _Foco;
            float _Progresso;
            float _MascaraBar;
            float _MascaraPosto;
            float _TempoNevoa;
            sampler2D _Grao;

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                half opacity : TEXCOORD1;
            };

            Varyings Vert(appdata_full input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = input.texcoord.xy;
                output.opacity = input.color.a;
                return output;
            }

            float HashNevoa(float2 p)
            {
                float3 q = frac(float3(p.xyx) * .1031);
                q += dot(q, q.yzx + 33.33);
                return frac((q.x + q.y) * q.z);
            }

            float Nevoa(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 t = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(HashNevoa(i), HashNevoa(i + float2(1, 0)), t.x),
                    lerp(HashNevoa(i + float2(0, 1)), HashNevoa(i + 1), t.x),
                    t.y);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 delta = (input.uv - _Foco.xy) * float2(_Foco.z, 1);
                float distanceFromLamp = length(delta);
                float radius = lerp(_Foco.w + .32, -.32, _Progresso);
                float distanceFromEdge = distanceFromLamp - radius;
                half shade = smoothstep(-.24, .24, distanceFromEdge);
                half penumbra = smoothstep(-.24, -.04, distanceFromEdge) * (1 - smoothstep(-.04, .24, distanceFromEdge));
                half ember = (1 - smoothstep(.0, .15, distanceFromLamp)) * smoothstep(.7, 1, _Progresso);

                // Reuse the room's point-filtered floor as the grain of the darkened image.
                float2 pixel = floor(input.uv * float2(288 * _Foco.z, 288));
                float2 grainUV = (pixel + .5) / 108.0;
                half3 wood = tex2D(_Grao, grainUV).rgb;
                half weathering = tex2D(_Grao, grainUV.yx * .61 + .27).g;
                half grain = dot(wood, half3(.29, .55, .16)) * .72 + weathering * .28;
                half soot = smoothstep(.12, 1.15, distanceFromLamp);
                half3 dark = half3(.011, .012, .015) + grain * half3(.083, .066, .049);
                half3 color = dark * (1 - soot * .63);
                color += (half3(.115, .080, .045) * penumbra + half3(.26, .168, .079) * ember) * (.55 + grain);
                if (_MascaraPosto > .5)
                {
                    float2 p = input.uv * float2(_Foco.z, 1);
                    float tempo = _TempoNevoa;
                    float ampla = Nevoa(p * 4.3 + float2(-.31, .11) * tempo);
                    float fina = Nevoa(p * 10.7 + float2(.37, -.19) * tempo);
                    float veios = Nevoa((p + ampla * .13) * float2(7, 15) + float2(-.18, .16) * tempo);
                    float fluxo = .54 * ampla + .29 * fina + .17 * veios;
                    float frente = lerp(_Foco.w + .37, -.39, _Progresso);
                    float borda = distanceFromLamp - frente + (fluxo - .5) * .27;
                    shade = smoothstep(-.25, .21, borda);
                    float penumbraFria = exp(-24 * borda * borda) * shade;
                    color = lerp(half3(.008,.012,.016), half3(.038,.051,.062), fluxo);
                    color += half3(.055,.069,.080) * penumbraFria * (.30 + .64 * fina);
                    // A cobertura total preserva o movimento da névoa entre as duas câmeras.
                }
                else if (_MascaraBar > .5)
                {
                    float2 uv = input.uv * float2(_Foco.z * 2.3, 5.0);
                    float t = _TempoNevoa;
                    float ampla = Nevoa(uv + float2(-.34, .05) * t);
                    float fina = Nevoa(uv * 2.4 + float2(.43, -.11) * t);
                    half nevoa = smoothstep(.30, .80, .74 * ampla + .26 * fina);
                    // Ondulação leve na borda. Nas pontas da curva o quadro fica
                    // totalmente visível ou totalmente coberto, mesmo com névoa.
                    float borda = distanceFromEdge + (nevoa - .5) * .13;
                    shade = smoothstep(-.24, .24, borda);
                    half beirada = smoothstep(-.12, .06, borda) * (1 - smoothstep(.06, .27, borda));
                    color = half3(.007, .010, .013) + grain * half3(.023, .019, .016);
                    color += nevoa * half3(.023, .029, .035);
                    color += beirada * half3(.054, .044, .036) * (.38 + nevoa);
                }
                return half4(color, shade * input.opacity);
            }
            ENDHLSL
        }
    }
}
