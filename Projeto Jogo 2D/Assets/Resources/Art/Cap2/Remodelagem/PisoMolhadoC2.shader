Shader "SantaRita/Cap2/PisoMolhado"
{
    Properties
    {
        [PerRendererData] _SilhouetteTex("Silhueta", 2D) = "white" {}
        _Tint("Cor", Color) = (0,0,0,.2)
        _Wet("Reflexo", Float) = 0
        _Feather("Suavizar ponta da sombra", Float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
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
            TEXTURE2D(_SilhouetteTex);
            SAMPLER(sampler_SilhouetteTex);
            float4 _Tint;
            float _Wet;
            float _Feather;
            struct Input { float3 pos : POSITION; float2 uv : TEXCOORD0; float depth : TEXCOORD1; };
            struct Output { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float depth : TEXCOORD1; };
            Output Vert(Input v)
            {
                Output o;
                o.pos = TransformObjectToHClip(v.pos);
                o.uv = v.uv;
                o.depth = v.depth;
                return o;
            }
            half4 Frag(Output i) : SV_Target
            {
                float2 grid = floor(i.uv * float2(60,106));
                float2 p = (grid + .5) / float2(60,106) * 2 - 1;
                float ripple = sin(grid.y * 1.1 - _Time.y * 3.6 + sin(grid.y * .33));
                float scatter = frac(sin(dot(grid,float2(67.2,89.1))) * 13154.1);
                float wet = (1 - smoothstep(.01,.68,abs(p.x + .10 * ripple)))
                    * (1 - smoothstep(.17,.99,abs(p.y)))
                    * smoothstep(.35,.99,scatter) * (.56 + .31 * ripple);
                float mask = lerp(SAMPLE_TEXTURE2D(_SilhouetteTex,sampler_SilhouetteTex,i.uv).a,wet,_Wet);
                float fade = 1 - .78 * smoothstep(.12,.99,i.depth) * _Feather;
                return half4(_Tint.rgb,_Tint.a * mask * fade);
            }
            ENDHLSL
        }
    }
}

