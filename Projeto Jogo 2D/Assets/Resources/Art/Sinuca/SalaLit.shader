Shader "SantaRita/Sinuca/SalaLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Surface",2D)="white" {}
        [NoScaleOffset] _SurfaceMap("Normal XY, roughness, metallic",2D)="white" {}
        [MainColor] _BaseColor("Tint",Color)=(1,1,1,1)
        _Smoothness("Polish",Range(0,1))=.15
        _Metallic("Metallic",Range(0,1))=0
        _ActorLighting("Rounded sprite lighting",Float)=0
        _RoomSurface("Architectural surface",Float)=0
        _CeilingFade("Ceiling contact height",Float)=3.04
        _WetThreshold("Wet entrance surface",Range(0,1))=0
        _AmbientColor("Interior Ambient",Color)=(.049,.056,.059,1)
        _HazeColor("Atmospheric Haze Color",Color)=(.025,.031,.034,1)
        _HazeStrength("Atmospheric Haze Strength",Range(0,1))=.23
        _FilmicTonemap("Filmic Tonemap",Float)=0
        [HideInInspector] _BochaAbertura("Opening in the bocce court",Vector)=(0,0,0,0)
        _AlphaClip("Cutout",Float)=0
        _Cutoff("Cutout threshold",Range(0,1))=.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull",Float)=2
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
        float4 _BaseMap_TexelSize;
        CBUFFER_START(UnityPerMaterial)
        float4 _BaseMap_ST,_BochaAbertura;half4 _BaseColor,_AmbientColor,_HazeColor;half _Smoothness,_Metallic,_AlphaClip,_Cull,_ActorLighting,_RoomSurface,_CeilingFade,_WetThreshold,_HazeStrength,_FilmicTonemap;half _Cutoff;
        CBUFFER_END
        void ClipSalaSurface(float2 uv, float3 positionWS, out half4 albedo, out half4 surface)
        {
            albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
            surface = albedo * _BaseColor;
            if (_BochaAbertura.z > 0)
            {
                float2 offset = positionWS.xz - _BochaAbertura.xy;
                float angle = atan2(offset.y, offset.x);
                float contour = 1 + .10 * sin(7 * angle) + .065 * cos(13 * angle);
                clip(length(offset) / _BochaAbertura.z - contour);
            }
            clip(surface.a - (_AlphaClip > .5 ? _Cutoff : 0));
        }
        ENDHLSL
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
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #define SPRITE_SURFACE_ALBEDO _BaseMap
            #define SPRITE_SURFACE_SAMPLER sampler_BaseMap
            #include "../SpriteSurface.hlsl"

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
                half3 brdf;
                if (textured)
                {
                    float3 f0 = lerp(.04, baseColor, max(material.metallic, _Metallic));
                    float roughness = clamp(lerp(1 - _Smoothness, material.roughness, .74), .06, .98);
                    half metal = max(material.metallic, _Metallic);
                    brdf = baseColor * ndl * (1 - .45 * metal)
                        + SpriteSpecular(normal, light.direction, view, roughness, f0);
                }
                else
                {
                    float3 f0 = lerp(.04, baseColor, _Metallic);
                    float roughness = clamp(1 - _Smoothness, .08, .96);
                    brdf = baseColor * ndl * (1 - .55 * _Metallic)
                        + SpriteSpecular(normal, light.direction, view, roughness, f0) * lerp(.45, 1.0, saturate(_Smoothness + _Metallic));
                }
                return brdf * light.color * light.distanceAttenuation * light.shadowAttenuation;
            }
            half3 FilmicACES(half3 x)
            {
                // Preserve darks and midtones 1:1 while smoothly compressing highlights above 0.55.
                half3 excess = max(x - .55h, 0.h);
                return min(x, .55h) + .45h * (excess / (excess + .45h));
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

                if (!textured && _RoomSurface > .1)
                {
                    // Subtle plaster/masonry micro-normal variation so flat surfaces catch grazing light naturally.
                    float3 p = i.positionWS * 4.2;
                    half3 plasterPerturb = half3(
                        sin(p.y * 1.7 + p.z * 2.3) * .028,
                        sin(p.x * 2.1 - p.z * 1.9) * .022,
                        cos(p.x * 1.9 + p.y * 2.5) * .028);
                    plasterPerturb -= normal * dot(plasterPerturb, normal);
                    normal = normalize(normal + plasterPerturb * _RoomSurface);
                }

                half3 view=SafeNormalize(GetWorldSpaceViewDir(i.positionWS));
                if (_WetThreshold > 0)
                {
                    half puddle = smoothstep(.42, .72, damp) * saturate(normal.y) * _WetThreshold;
                    material.roughness = lerp(material.roughness, .23, puddle);
                    surface.rgb *= 1 - puddle * .32;
                }
                // Room-local bounce with hemisphere directionality so undersides/ceilings read distinctly from floors.
                half hemi = lerp(.62, 1.15, saturate(normal.y * .5 + .5));
                half3 bounce = (_AmbientColor.rgb * hemi) + _ActorLighting * half3(.040, .035, .026);
                half wallContact=lerp(.55,1,smoothstep(0,.36,i.positionWS.y));
                half ceilingContact=_CeilingFade>.1?lerp(1,.70,smoothstep(_CeilingFade-.44,_CeilingFade,i.positionWS.y)):1;
                // Low frequency plaster variation; native pixel textures keep Point sampling.
                half age=1-_RoomSurface*(.035+.035*sin(i.positionWS.x*3.1+i.positionWS.z*2.7)*sin(i.positionWS.y*4.3));
                float2 screenUV = GetNormalizedScreenSpaceUV(i.positionCS);
                AmbientOcclusionFactor ao = GetScreenSpaceAmbientOcclusion(screenUV);
                half aoIndirect = lerp(ao.indirectAmbientOcclusion, 1, _ActorLighting * .65);
                half aoDirect = lerp(ao.directAmbientOcclusion, 1, _ActorLighting * .80);
                half3 color=surface.rgb*bounce*aoIndirect;
                color+=Shade(GetMainLight(TransformWorldToShadowCoord(i.positionWS)),surface.rgb,normal,view,material,textured)*aoDirect;
                #if defined(_ADDITIONAL_LIGHTS)
                InputData inputData = (InputData)0;
                inputData.positionWS = i.positionWS;
                inputData.normalizedScreenSpaceUV = screenUV;
                uint count=GetAdditionalLightsCount();
                #if USE_CLUSTER_LIGHT_LOOP
                [loop] for (uint dirIdx = 0; dirIdx < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); dirIdx++)
                    color+=Shade(GetAdditionalLight(dirIdx,i.positionWS,half4(1,1,1,1)),surface.rgb,normal,view,material,textured)*aoDirect;
                #endif
                LIGHT_LOOP_BEGIN(count)
                    color+=Shade(GetAdditionalLight(lightIndex,i.positionWS,half4(1,1,1,1)),surface.rgb,normal,view,material,textured)*aoDirect;
                LIGHT_LOOP_END
                #endif
                color*=lerp(1,wallContact*ceilingContact*age,_RoomSurface);
                // Cool, low haze gives the far end depth without veiling nearby clues.
                float distanceToCamera = distance(i.positionWS, GetCameraPositionWS());
                half haze = smoothstep(2.5, 19.0, distanceToCamera) * _HazeStrength;
                haze *= lerp(.55, 1, 1 - smoothstep(.30, 1.70, i.positionWS.y));
                color = lerp(color, _HazeColor.rgb, haze);
                if (_FilmicTonemap > .5) color = FilmicACES(color);
                return half4(color,1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Simple Lit/ShadowCaster"
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            ZWrite On
            ColorMask R
            Cull [_Cull]
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex VertDepth
            #pragma fragment FragDepth
            #pragma multi_compile_instancing
            struct AD {float4 positionOS:POSITION;float2 uv:TEXCOORD0;UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct VD {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;float2 uv:TEXCOORD1;UNITY_VERTEX_INPUT_INSTANCE_ID};
            VD VertDepth(AD v)
            {
                VD o;UNITY_SETUP_INSTANCE_ID(v);UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.positionWS=TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS=TransformWorldToHClip(o.positionWS);
                o.uv=TRANSFORM_TEX(v.uv,_BaseMap);
                return o;
            }
            half FragDepth(VD i):SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half4 a,s; ClipSalaSurface(i.uv,i.positionWS,a,s);
                return i.positionCS.z;
            }
            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormals" }
            ZWrite On
            Cull [_Cull]
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex VertDN
            #pragma fragment FragDN
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            #pragma multi_compile_instancing
            struct ADN {float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct VDN {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;half3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;UNITY_VERTEX_INPUT_INSTANCE_ID};
            VDN VertDN(ADN v)
            {
                VDN o;UNITY_SETUP_INSTANCE_ID(v);UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.positionWS=TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS=TransformWorldToHClip(o.positionWS);
                o.normalWS=TransformObjectToWorldNormal(v.normalOS);
                o.uv=TRANSFORM_TEX(v.uv,_BaseMap);
                return o;
            }
            half4 FragDN(VDN i,FRONT_FACE_TYPE face:FRONT_FACE_SEMANTIC):SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half4 a,s; ClipSalaSurface(i.uv,i.positionWS,a,s);
                float3 n=normalize(i.normalWS)*IS_FRONT_VFACE(face,1,-1);
                #if defined(_GBUFFER_NORMALS_OCT)
                    float2 oct=PackNormalOctQuadEncode(n);
                    return half4(PackFloat2To888(saturate(oct*.5+.5)),0);
                #else
                    return half4(n,0);
                #endif
            }
            ENDHLSL
        }
    }
}
