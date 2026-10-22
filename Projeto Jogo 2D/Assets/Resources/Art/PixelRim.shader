Shader "SantaRita/PixelRim"
{
    Properties { [PerRendererData] _MainTex("Sprite",2D)="white" {} _RimColor("Color",Color)=(1,.8,.5,1) _Strength("Strength",Float)=0 _LightDir("Direction",Vector)=(1,1,0,0) }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            CBUFFER_START(UnityPerMaterial)
            float4 _RimColor; float4 _LightDir; float _Strength;
            CBUFFER_END
            struct Input { float3 positionOS:POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
            struct Output { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
            Output vert(Input i) { Output o;o.positionCS=TransformObjectToHClip(i.positionOS);o.uv=i.uv;o.color=i.color;return o; }
            half4 frag(Output i):SV_Target
            {
                half alpha=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).a;
                half neighbor=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv+_MainTex_TexelSize.xy*_LightDir.xy).a;
                return half4(_RimColor.rgb,saturate(alpha-neighbor)*_Strength*i.color.a);
            }
            ENDHLSL
        }
    }
}
