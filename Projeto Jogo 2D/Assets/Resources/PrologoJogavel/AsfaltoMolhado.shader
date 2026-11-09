Shader "SantaRita/PrologoAsfaltoMolhado"
{
    Properties
    {
        [PerRendererData] _MainTex ("Terreno da Rua Norte", 2D) = "white" {}
        _EntranceCells ("Células da Rodovia", 2D) = "white" {}
        _Arvore ("Árvore", 2D) = "white" {}
        _Araucaria ("Araucária", 2D) = "white" {}
        _Bracatinga ("Bracatinga", 2D) = "white" {}
        _Pitangueira ("Pitangueira", 2D) = "white" {}
        _Arbusto ("Arbusto", 2D) = "white" {}
        _Poste ("Poste", 2D) = "white" {}
        _Entrada ("Placa e parada", 2D) = "white" {}
        _Orelhao ("Orelhão", 2D) = "white" {}
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="False" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata { float4 vertex:POSITION; fixed4 color:COLOR; float3 uv:TEXCOORD0; float2 art:TEXCOORD1; };
            struct v2f { float4 vertex:SV_POSITION; fixed4 color:COLOR; float3 uv:TEXCOORD0; float4 local:TEXCOORD1; float art:TEXCOORD2; };
            sampler2D _MainTex, _EntranceCells, _Arvore, _Araucaria, _Bracatinga, _Pitangueira, _Arbusto, _Poste, _Entrada, _Orelhao;
            float4 _MainTex_TexelSize, _EntranceCells_TexelSize, _ClipRect;
            float4 _View, _EntranceMap, _EntranceLamps;
            float _EntranceStart;

            v2f vert(appdata v)
            {
                v2f o;
                o.local = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.art = v.art.x;
                return o;
            }

            float Hash(float2 cell)
            {
                return frac(sin(dot(cell, float2(127.1, 311.7))) * 43758.5453);
            }

            // Mesma organização de Terreno.png usada por SantaRitaRuaNorte.
            half3 Tile(float2 uv, float column, float row)
            {
                float2 pixel = float2(column, row) * 64.0 + clamp(uv * 64.0, .5, 63.5);
                return tex2D(_MainTex, pixel * _MainTex_TexelSize.xy).rgb;
            }

            half3 EntranceGround(float2 world, out float road)
            {
                float2 pos = float2(_EntranceMap.x + (world.y - _EntranceMap.y) / _EntranceMap.z,
                                    10.0 - world.x / _EntranceMap.w);
                float2 cell = floor(pos);
                float2 uv = frac(pos);
                float2 bounds = _EntranceCells_TexelSize.zw;
                // Repete o padrão do passeio e da pista depois do limite visível da zona.
                if (cell.x >= bounds.x) cell.x = 16.0 + fmod(cell.x - 16.0, 20.0);
                half4 data = tex2D(_EntranceCells, (clamp(cell, 0.0, bounds - 1.0) + .5) / bounds);
                float2 index = floor(data.rg * 255.0 + .5);
                uv = lerp(uv, 1.0 - uv, data.ba);
                float row = _MainTex_TexelSize.w / 64.0 - 1.0 - index.y;
                road = (row == 1.0 || row == 7.0 || row == 9.0) ? 1.0 : 0.0;
                return Tile(uv, index.x, index.y);
            }

            half3 RuralGround(float2 world, out float asphalt)
            {
                float dirt = smoothstep(140.0, 166.0, world.y) * (1.0 - smoothstep(348.0, 400.0, world.y));
                float edge = lerp(3.8, 3.0, dirt) + sin(world.y * .61 + world.x) * .065 + sin(world.y * 1.71) * .045;
                float lane = 1.0 - smoothstep(edge - .07, edge + .07, abs(world.x));
                asphalt = lane * (1.0 - dirt);
                float2 pos = world / float2(1.9, 3.2);
                float2 cell = floor(pos);
                float2 uv = frac(pos);
                float random = Hash(cell);
                float variant = floor(random * 4.0);
                float top = _MainTex_TexelSize.w / 64.0 - 1.0;
                half3 grass = Tile(uv, variant, top);
                half3 gravel = Tile(uv, variant, top - 3.0);
                float shoulder = 1.0 - smoothstep(edge + .08, edge + .82 + sin(world.y * .26) * .22, abs(world.x));
                half3 verge = lerp(grass, gravel * .82, shoulder * .88);
                float wear = random < .17 ? 7.0 : (random > .92 ? 8.0 : 1.0);
                half3 pavement = Tile(uv, variant, top - wear);
                float ruts = 1.0 - smoothstep(.10, .31, abs(abs(world.x) - 1.18 + sin(world.y * .15) * .05));
                gravel *= 1.12 - .24 * ruts;
                half3 color = lerp(verge, lerp(pavement, gravel, dirt), lane);
                float stripe = 1.0 - smoothstep(.018, .064, abs(abs(world.x) - .09));
                float edgePaint = 1.0 - smoothstep(.015, .06, abs(abs(world.x) - 3.69));
                color = lerp(color, half3(.52, .36, .12), asphalt * stripe * (.67 + random * .24));
                return lerp(color, half3(.37, .36, .28), asphalt * edgePaint * .54);
            }

            half4 Ground(float2 world)
            {
                float asphalt;
                float approach = smoothstep(_EntranceStart - 8.0, _EntranceStart, world.y);
                half3 color = 0;
                asphalt = 0;
                if (approach < 1.0) color = RuralGround(world, asphalt);
                if (approach > 0.0)
                {
                    float road;
                    half3 entrance = EntranceGround(world, road);
                    color = lerp(color, entrance, approach);
                    asphalt = lerp(asphalt, road, approach);
                }

                float z = world.y - _View.y;
                float lateral = world.x - _View.x;
                float left = 1.0 - smoothstep(.55, 1.35 + z * .060, abs(lateral - .65));
                float right = 1.0 - smoothstep(.55, 1.35 + z * .069, abs(lateral + .65));
                float beam = (1.0 - smoothstep(9.0, lerp(48.0, 89.0, _View.z), z)) * (.58 * left + .42 * right);
                float3 lampDistance = abs(_EntranceLamps.xyz - world.y);
                float closest = min(lampDistance.x, min(lampDistance.y, lampDistance.z));
                float lamp = exp2(-.008 * closest * closest - .13 * pow(world.x - _EntranceLamps.w, 2.0)) * approach;
                float ruralZ = (frac((world.y + 22.5) / 45.0) - .5) * 45.0;
                lamp += exp2(-.016 * ruralZ * ruralZ - .10 * pow(world.x - 4.9, 2.0)) * (1.0 - approach);
                float streak = exp2(-.023 * closest * closest) * (1.0 - smoothstep(.05, 1.3, abs(world.x + 2.65)));
                float broken = .48 + .52 * sin(world.y * 9.1 + world.x * 12.0);
                float reflection = asphalt * (.046 * lamp + .037 * streak * approach * broken);
                float fog = 1.0 - exp2(-.027 * max(0.0, z - 7.0));
                color *= half3(.28, .36, .49) + beam * half3(.41, .38, .32) + lamp * half3(.68, .48, .28);
                color += reflection * half3(.87, .57, .27);
                color = lerp(color, half3(.035, .055, .081), fog);
                return half4(color, 1.0 - smoothstep(150.0, 204.0, z));
            }

            float Noise(float2 p)
            {
                float2 cell = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(Hash(cell), Hash(cell + float2(1.0, 0.0)), f.x),
                            lerp(Hash(cell + float2(0.0, 1.0)), Hash(cell + 1.0), f.x), f.y);
            }

            half4 Mist(float2 uv)
            {
                float2 drift = float2(_View.w * .085 + _View.y * .006, 0.0);
                float clouds = .67 * Noise(uv * float2(6.0, 8.0) + drift) +
                               .33 * Noise(uv * float2(14.0, 17.0) - drift * .43);
                float horizon = .52 + (clouds - .5) * .13;
                float low = 1.0 - smoothstep(.03, .31, abs(uv.y - horizon));
                float wisps = smoothstep(.18, .72, clouds);
                float glass = .07 * (1.0 - smoothstep(.1, .9, uv.y));
                return half4(half3(.092, .128, .17), low * (.30 + wisps * .33) + glass);
            }

            fixed4 SampleProp(float art, float2 uv)
            {
                if (art > 8.5) return tex2D(_Orelhao, uv);
                if (art > 7.5) return tex2D(_Entrada, uv);
                if (art > 6.5) return tex2D(_Poste, uv);
                if (art > 5.5) return tex2D(_Arbusto, uv);
                if (art > 4.5) return tex2D(_Pitangueira, uv);
                if (art > 3.5) return tex2D(_Bracatinga, uv);
                if (art > 2.5) return tex2D(_Araucaria, uv);
                return tex2D(_Arvore, uv);
            }

            fixed4 frag(v2f i):SV_Target
            {
                fixed4 c = i.color;
                float2 uv = i.uv.xy / max(i.uv.z, .0001);
                if (i.art < -1.5)
                    c.a *= SampleProp(-i.art, uv).a;
                else if (i.art > 9.5) c *= Mist(uv);
                else if (i.art > 1.5) c *= SampleProp(i.art, uv);
                else if (i.art > .5) c *= Ground(uv);
                #ifdef UNITY_UI_CLIP_RECT
                c.a *= UnityGet2DClipping(i.local.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(c.a - .001);
                #endif
                return c;
            }
            ENDCG
        }
    }
}
