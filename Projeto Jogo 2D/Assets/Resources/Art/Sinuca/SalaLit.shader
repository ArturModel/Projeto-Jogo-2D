Shader "SantaRita/Sinuca/SalaLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Surface",2D)="white" {}
        [NoScaleOffset] _SurfaceMap("Normal XY, roughness, metallic",2D)="white" {}
        [MainColor] _BaseColor("Tint",Color)=(1,1,1,1)
        _Smoothness("Polish",Range(0,1))=.15
        _ActorLighting("Rounded sprite lighting",Float)=0
        _RoomSurface("Architectural surface",Float)=0
        _WetThreshold("Wet entrance surface",Range(0,1))=0
        [HideInInspector] _BochaAbertura("Opening in the bocce court",Vector)=(0,0,0,0)
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
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
            #define SPRITE_SURFACE_ALBEDO _BaseMap
            #define SPRITE_SURFACE_SAMPLER sampler_BaseMap
            #include "../SpriteSurface.hlsl"
            float4 _BaseMap_TexelSize;

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST,_BochaAbertura;half4 _BaseColor;half _Smoothness,_AlphaClip,_Cull,_ActorLighting,_RoomSurface,_WetThreshold;
            CBUFFER_END
            struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct V {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;half3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;float2 localXY:TEXCOORD3;UNITY_VERTEX_INPUT_INSTANCE_ID};
            V Vert(A v)
            {
                V o;UNITY_SETUP_INSTANCE_ID(v);UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.positionWS=TransformObjectToWorld(v.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.positionWS);
                o.normalWS=TransformObjectToWorldNormal(v.normalOS);o.uv=TRANSFORM_TEX(v.uv,_BaseMap);o.localXY=v.positionOS.xy;return o;
            }
            half3 Shade(Light light,half3 baseColor,half3 normal,half3 view,SpriteSurface material,bool textured)
            {
                half ndl=saturate((dot(normal,light.direction)+_ActorLighting*.18)/(1+_ActorLighting*.18));
                half3 halfway=SafeNormalize(light.direction+view);
                half spec=pow(saturate(dot(normal,halfway)),lerp(12,160,_Smoothness))*_Smoothness*.32;
                half3 brdf = baseColor * ndl + spec;
                if (textured)
                {
                    float3 f0 = lerp(.04, baseColor, material.metallic);
                    float roughness = lerp(1 - _Smoothness, material.roughness, .74);
                    brdf = baseColor * ndl * (1 - .28 * material.metallic)
                        + SpriteSpecular(normal, light.direction, view, roughness, f0);
                }
                return brdf * light.color * light.distanceAttenuation * light.shadowAttenuation;
            }
            float SurfaceGrain(float3 cell)
            {
                float3 p = frac(cell * .1031);
                p += dot(p, p.yzx + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            half4 Frag(V i,FRONT_FACE_TYPE face:FRONT_FACE_SEMANTIC):SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
                half4 surface = albedo * _BaseColor;
                if (_BochaAbertura.z > 0)
                {
                    float2 offset = i.positionWS.xz - _BochaAbertura.xy;
                    float angle = atan2(offset.y, offset.x);
                    float contour = 1 + .10 * sin(7 * angle) + .065 * cos(13 * angle);
                    float edge = length(offset) / _BochaAbertura.z - contour;
                    clip(edge);
                    surface.rgb *= lerp(half3(.18,.10,.10), half3(1,1,1), smoothstep(0,.23,edge));
                }
                clip(surface.a-(_AlphaClip>.5?.5:0));
                half3 normal=normalize(i.normalWS)*IS_FRONT_VFACE(face,1,-1);
                // Gentle cylindrical lighting keeps cutout art grounded as the camera turns.
                // Geometry, UVs and feet remain untouched; no rubbery sprite deformation.
                if(_ActorLighting>.5)
                {
                    half3 right=normalize(UNITY_MATRIX_M._m00_m10_m20);
                    normal=normalize(normal+right*clamp(i.localXY.x,-.55,.55)*.65+half3(0,.32,0));
                }
                SpriteSurface material = (SpriteSurface)0;
                bool textured = any(_BaseMap_TexelSize.zw > 4);
                [branch] if (textured)
                {
                    material = ReadSpriteSurface(i.uv, _BaseMap_TexelSize.xy, albedo);
                    // Reconstruct the tangent basis for procedural meshes and cutouts.
                    float3 dx = ddx(i.positionWS), dy = ddy(i.positionWS);
                    float2 ux = ddx(i.uv), uy = ddy(i.uv);
                    float3 across = cross(dy, normal), up = cross(normal, dx);
                    float3 tangent = across * ux.x + up * uy.x;
                    float3 bitangent = across * ux.y + up * uy.y;
                    float scale = rsqrt(max(max(dot(tangent, tangent), dot(bitangent, bitangent)), 1e-12));
                    normal = normalize(normal + (tangent * material.slope.x + bitangent * material.slope.y) * (.72 * scale));
                }
                // Quantized wear sits in world space so it stays on the walls as Marcus moves.
                float3 cell = i.positionWS * 38 + abs(normal) * .47;
                half grain = SurfaceGrain(floor(cell));
                half damp = SurfaceGrain(floor(cell * .09));
                surface.rgb *= lerp(1, .86 + .13 * grain + .07 * damp, _RoomSurface);

                half3 view=SafeNormalize(GetWorldSpaceViewDir(i.positionWS));
                if (_WetThreshold > 0)
                {
                    half puddle = smoothstep(.42, .72, damp) * saturate(normal.y) * _WetThreshold;
                    material.roughness = lerp(material.roughness, .23, puddle);
                    surface.rgb *= 1 - puddle * .32;
                }
                // Room-local bounce: independent from the 2D world's ambient probe.
                half3 bounce=half3(.049,.056,.059)+_ActorLighting*half3(.040,.035,.026);
                half wallContact=lerp(.55,1,smoothstep(0,.36,i.positionWS.y));
                half ceilingContact=lerp(1,.70,smoothstep(2.60,3.04,i.positionWS.y));
                // Low frequency plaster variation; native pixel textures keep Point sampling.
                half age=1-_RoomSurface*(.035+.035*sin(i.positionWS.x*3.1+i.positionWS.z*2.7)*sin(i.positionWS.y*4.3));
                half3 color=surface.rgb*bounce;
                color+=Shade(GetMainLight(TransformWorldToShadowCoord(i.positionWS)),surface.rgb,normal,view,material,textured);
                uint count=GetAdditionalLightsCount();
                for(uint n=0;n<count;n++)color+=Shade(GetAdditionalLight(n,i.positionWS,half4(1,1,1,1)),surface.rgb,normal,view,material,textured);
                color*=lerp(1,wallContact*ceilingContact*age,_RoomSurface);
                // Cool, low haze gives the far end depth without veiling nearby clues.
                float distanceToCamera = distance(i.positionWS, GetCameraPositionWS());
                half haze = smoothstep(2.5, 19.0, distanceToCamera) * .23;
                haze *= lerp(.55, 1, 1 - smoothstep(.30, 1.70, i.positionWS.y));
                return half4(lerp(color, half3(.025,.031,.034), haze),1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Simple Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Simple Lit/DepthOnly"
    }
}
