Shader "SantaRita/Sinuca/SalaLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Surface",2D)="white" {}
        [MainColor] _BaseColor("Tint",Color)=(1,1,1,1)
        _Smoothness("Polish",Range(0,1))=.15
        _ActorLighting("Rounded sprite lighting",Float)=0
        _RoomSurface("Architectural surface",Float)=0
        _AlphaClip("Cutout",Float)=0
        _Cutoff("Cutout threshold",Range(0,1))=.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull",Float)=2
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Cull [_Cull]
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;half4 _BaseColor;half _Smoothness,_AlphaClip,_Cull,_ActorLighting,_RoomSurface;
            CBUFFER_END
            struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct V {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;half3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;float2 localXY:TEXCOORD3;UNITY_VERTEX_INPUT_INSTANCE_ID};
            V Vert(A v)
            {
                V o;UNITY_SETUP_INSTANCE_ID(v);UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.positionWS=TransformObjectToWorld(v.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.positionWS);
                o.normalWS=TransformObjectToWorldNormal(v.normalOS);o.uv=TRANSFORM_TEX(v.uv,_BaseMap);o.localXY=v.positionOS.xy;return o;
            }
            half3 Shade(Light light,half3 baseColor,half3 normal,half3 view)
            {
                half ndl=saturate((dot(normal,light.direction)+_ActorLighting*.18)/(1+_ActorLighting*.18));
                half3 halfway=SafeNormalize(light.direction+view);
                half spec=pow(saturate(dot(normal,halfway)),lerp(12,160,_Smoothness))*_Smoothness*.32;
                return (baseColor*ndl+spec)*light.color*light.distanceAttenuation*light.shadowAttenuation;
            }
            half4 Frag(V i,FRONT_FACE_TYPE face:FRONT_FACE_SEMANTIC):SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half4 surface=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv)*_BaseColor;
                clip(surface.a-(_AlphaClip>.5?.5:0));
                half3 normal=normalize(i.normalWS)*IS_FRONT_VFACE(face,1,-1);
                // Gentle cylindrical lighting keeps cutout art grounded as the camera turns.
                // Geometry, UVs and feet remain untouched; no rubbery sprite deformation.
                if(_ActorLighting>.5)
                {
                    half3 right=normalize(UNITY_MATRIX_M._m00_m10_m20);
                    normal=normalize(normal+right*clamp(i.localXY.x,-.55,.55)*.65+half3(0,.32,0));
                }
                half3 view=SafeNormalize(GetWorldSpaceViewDir(i.positionWS));
                // Room-local bounce: independent from the 2D world's ambient probe.
                half3 bounce=half3(.065,.072,.083)+_ActorLighting*half3(.032,.029,.022);
                half wallContact=lerp(.55,1,smoothstep(0,.36,i.positionWS.y));
                half ceilingContact=lerp(1,.70,smoothstep(2.60,3.04,i.positionWS.y));
                // Low frequency plaster variation; native pixel textures keep Point sampling.
                half age=1-_RoomSurface*(.035+.035*sin(i.positionWS.x*3.1+i.positionWS.z*2.7)*sin(i.positionWS.y*4.3));
                half3 color=surface.rgb*bounce;
                color+=Shade(GetMainLight(TransformWorldToShadowCoord(i.positionWS)),surface.rgb,normal,view);
                uint count=GetAdditionalLightsCount();
                for(uint n=0;n<count;n++)color+=Shade(GetAdditionalLight(n,i.positionWS,half4(1,1,1,1)),surface.rgb,normal,view);
                color*=lerp(1,wallContact*ceilingContact*age,_RoomSurface);
                return half4(color,1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Simple Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Simple Lit/DepthOnly"
    }
}
