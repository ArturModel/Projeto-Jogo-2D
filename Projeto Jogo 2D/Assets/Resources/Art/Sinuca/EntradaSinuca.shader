Shader "SantaRita/Sinuca/Entrada"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Grao ("Madeira do bar", 2D) = "gray" {}
        _Foco ("Foco, aspecto e alcance", Vector) = (.5, .5, 1.78, 1)
        _Progresso ("Progresso", Range(0, 1)) = 0
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
                return half4(color, shade * input.opacity);
            }
            ENDHLSL
        }
    }
}
