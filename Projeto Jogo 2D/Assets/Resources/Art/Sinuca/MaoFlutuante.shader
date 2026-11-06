Shader "SantaRita/Sinuca/MaoFlutuante"
{
    Properties
    {
        _BaseMap("Pixel art", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth test", Float) = 4
        _BaseColor("Tint", Color) = (0.87,0.84,0.77,1)
        _WristOrigin("Wrist ending in UV", Vector) = (0,0,0,0)
        _WristDirection("Toward palm", Vector) = (0,1,0,0)
        _BridgeLayer("Bridge: full / back / thumb", Float) = 0
        _BridgeSplit("Thumb seam: x, y, slope", Vector) = (0.2845,0.662,0.14,0)
        _WristFade("Pixel dissolve length", Float) = 0.12
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Cull Off
            ZWrite On
            ZTest [_ZTest]
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _WristOrigin, _WristDirection;
                float _WristFade, _BridgeLayer;
                float4 _BridgeSplit;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float3 positionWS:TEXCOORD1; half3 normalWS:TEXCOORD2; };
            Varyings Vert(Attributes v)
            {
                Varyings o; o.positionWS=TransformObjectToWorld(v.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.positionWS);
                // The billboard faces the camera, but the back of the physical hand faces
                // the pendant. Keep that lighting normal stable while the camera orbits.
                o.normalWS=normalize(half3(0,.85,0)+TransformObjectToWorldNormal(v.normalOS)*.15);
                o.uv=v.uv;return o;
            }
            half4 Frag(Varyings i):SV_Target
            {
                half4 color=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv)*_BaseColor;
                clip(color.a-0.5);
                if(_BridgeLayer>0.5)
                {
                    float boundary=_BridgeSplit.x+(i.uv.y-_BridgeSplit.y)*_BridgeSplit.z;
                    bool thumb=i.uv.x>=boundary;
                    if((_BridgeLayer<1.5 && thumb) || (_BridgeLayer>=1.5 && !thumb)) discard;
                }
                float distance=dot(i.uv-_WristOrigin.xy,normalize(_WristDirection.xy));
                float presence=saturate(distance/max(_WristFade,0.001));
                // Stable, texture-locked pixel dissolve; no temporal noise or soft blurry edge.
                float threshold=frac(dot(floor(i.uv*96),float2(0.754877666,0.569840296)));
                clip(presence-(threshold*0.999+0.0005));
                color.rgb*=lerp(0.76,1.0,presence);
                half3 illumination=half3(.035,.042,.052);
                half3 normal=normalize(i.normalWS);
                Light main=GetMainLight();
                illumination+=main.color*main.distanceAttenuation*saturate(dot(normal,main.direction));
                uint count=GetAdditionalLightsCount();
                for(uint n=0;n<count;n++)
                {
                    Light light=GetAdditionalLight(n,i.positionWS);
                    illumination+=light.color*light.distanceAttenuation*saturate(dot(normal,light.direction));
                }
                color.rgb*=illumination;
                return color;
            }
            ENDHLSL
        }
    }
}
