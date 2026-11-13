Shader "SantaRita/Centro/AguaCorrente"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Color("Cor", Color) = (1, 1, 1, 1)
        _ShoreMap("Mapa da margem", 2D) = "black" {}
        _ShoreRect("Retângulo da margem", Vector) = (0, 0, 1, 1)
        _TownGroundMist("Névoa rente ao piso", Range(0, 1)) = 0
        _WindStrength("Intensidade do clima", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex AguaVertex
            #pragma fragment AguaFragment
            #pragma target 3.5

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_LIT_OUTPUTS
                half4 color : COLOR;
                float2 worldPos : TEXCOORD7;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Lit2DCommon.hlsl"
            #include "NevoaCentro.hlsl"

            TEXTURE2D(_ShoreMap);
            SAMPLER(sampler_ShoreMap);

            float4 _Origin;
            float4 _ViewFocus;
            int _TownLampCount;
            float4 _TownLamps[88];
            float4 _TownLampColors[88];
            float4 _PlayerLamp;
            float2 _PlayerLampDirection;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _ShoreRect;
                float _TownGroundMist;
                float _WindStrength;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            Varyings AguaVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonLitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                o.worldPos = TransformObjectToWorld(input.positionOS).xy;
                return o;
            }

            half4 AguaFragment(Varyings input) : SV_Target
            {
                float2 world = input.worldPos;
                float2 wp = floor(world * 32.0) * 0.03125;
                float t = _Time.y;

                // Amostra o campo de distância suavizado da margem (R), ponte (G) e máscara de pavimento (B)
                float2 shoreUV = (world - _ShoreRect.xy) / max(_ShoreRect.zw, float2(1.0, 1.0));
                float3 shoreSample = float3(0.0, 1.0, 0.0);
                if (all(shoreUV >= 0.0) && all(shoreUV <= 1.0))
                {
                    shoreSample = SAMPLE_TEXTURE2D(_ShoreMap, sampler_ShoreMap, shoreUV).rgb;
                }

                float sdRaw = (shoreSample.r - 0.5) * 1.5;
                float bridgeDist = shoreSample.g * 1.5;
                float hasLand = shoreSample.b;
                float bridgeKeep = smoothstep(0.10, 0.55, bridgeDist);

                // Ondulação orgânica da linha d'água para formar curvas naturais de barranco
                float coastWave = 0.14 * sin(wp.x * 0.72 + wp.y * 0.55 + 1.3)
                                + 0.075 * sin(wp.x * 1.85 - wp.y * 1.55 + 0.7)
                                + 0.03 * sin(wp.x * 4.20 + wp.y * 3.40);
                float sd = sdRaw + coastWave * bridgeKeep;

                if (hasLand > 0.50 && (sd > 0.30 || (sd > 0.0 && bridgeKeep < 0.08)))
                    clip(-1);

                half3 surfaceColor = 0;
                float3 normalTS = float3(0.0, 0.0, 1.0);
                float alpha = input.color.a;
                float wavelet = 0.0;
                float bridgeShadow = 1.0;

                [branch] if (sd >= 0.0)
                {
                    // --- Divisória orgânica da terra/barranco para a água (sd >= 0) ---
                    float soilHash = Hash21(floor(wp * 32.0));
                    float clump = sin(wp.x * 7.8 + wp.y * 5.4) * cos(wp.x * 4.5 - wp.y * 8.2);
                    float wetMud = 1.0 - smoothstep(0.02, 0.15, sd);
                    half3 mudCol = half3(0.068, 0.052, 0.038) * lerp(0.82, 1.15, soilHash);
                    half3 mossyEarth = lerp(half3(0.076, 0.066, 0.046), half3(0.056, 0.074, 0.044),
                        smoothstep(0.09, 0.27, sd) * (0.5 + 0.5 * clump));
                    half3 bankCol = lerp(mossyEarth, mudCol, wetMud);

                    // Pequenas pedras e seixos na borda do barranco
                    float pebble = step(0.87, soilHash) * smoothstep(0.01, 0.06, sd) * (1.0 - smoothstep(0.16, 0.24, sd));
                    bankCol = lerp(bankCol, half3(0.120, 0.122, 0.114), pebble * 0.75);
                    bankCol *= lerp(0.55, 1.0, smoothstep(0.0, 0.045, sd));

                    float bankFeather = 1.0 - smoothstep(0.13, 0.29, sd + (soilHash - 0.5) * 0.055);
                    alpha = (hasLand > 0.50 ? bankFeather * bridgeKeep : 1.0) * input.color.a;
                    clip(alpha - 0.001);
                    surfaceColor = bankCol;
                }
                else
                {
                    // --- Superfície da água (azul noturno profundo + correnteza suave) ---
                    float2 flowP = wp + float2(-t * 0.26, sin(wp.x * 0.38 - t * 0.30) * 0.06);
                    float p1 = flowP.x * 1.15 + flowP.y * 3.10 + sin(flowP.y * 1.65 + t * 0.24) * 0.90;
                    float p2 = wp.x * 2.35 - t * 0.62 - wp.y * 4.45 + sin(wp.x * 1.20 - t * 0.34) * 0.80;
                    float w1 = sin(p1);
                    float w2 = sin(p2);

                    // Filetes horizontais finos e afilados nas cristas (estilo pixel art)
                    float2 wUv = float2(wp.x * 2.2 - t * 0.58 + sin(wp.y * 1.7 + t * 0.35) * 0.22, wp.y * 14.0);
                    float wRow = floor(wUv.y);
                    float wShift = Hash21(float2(wRow, 17.3)) * 13.0;
                    float wSpan = wUv.x + wShift;
                    float wCellId = floor(wSpan);
                    float wMask = step(0.68, Hash21(float2(wRow, wCellId)));
                    float wTaper = sin(frac(wSpan) * 3.14159);
                    float wThin = 1.0 - abs(frac(wUv.y) * 2.0 - 1.0);
                    wavelet = wMask * smoothstep(0.32, 0.96, wTaper) * smoothstep(0.30, 0.90, wThin)
                            * (0.40 + 0.60 * sin(t * 1.55 + wCellId * 2.3 + wRow));

                    // Pequenos pontinhos/folhinhas sutis à deriva na correnteza
                    float2 speckUv = float2(wp.x * 2.8 - t * 0.54, wp.y * 3.8 + sin(wp.x * 0.7 + t * 0.28) * 0.5);
                    float speckRand = Hash21(floor(speckUv) + 29.1);
                    float2 speckFrac = frac(speckUv);
                    float speck = step(0.982, speckRand) * step(abs(speckFrac.x - 0.5), 0.14) * step(abs(speckFrac.y - 0.5), 0.14);

                    // Normal sutil das ondas para iluminação 2D
                    float dHx = cos(p1) * 0.38 + cos(p2) * 0.28;
                    float dHy = cos(p1) * 0.68 - cos(p2) * 0.58;
                    normalTS = normalize(float3(-dHx * 0.16, -dHy * 0.20, 1.0));

                    float depth = saturate(-sd * 0.8);
                    float waveShade = 0.5 + 0.5 * (w1 * 0.64 + w2 * 0.36);

                    half3 deepTrough = half3(0.010, 0.026, 0.068);
                    half3 deepCrest = half3(0.024, 0.056, 0.132);
                    half3 shallowEdge = half3(0.009, 0.022, 0.052);
                    half3 waterCol = lerp(shallowEdge, lerp(deepTrough, deepCrest, waveShade), smoothstep(0.0, 0.72, depth));

                    waterCol += half3(0.018, 0.044, 0.102) * wavelet * (0.25 + 0.75 * depth);
                    waterCol = lerp(waterCol, half3(0.036, 0.054, 0.060), speck * 0.45 * smoothstep(0.08, 0.45, -sd));

                    // Sombra de oclusão do barranco + filete animado da linha d'água
                    float bankAO = lerp(0.42, 1.0, smoothstep(-0.01, -0.22, sd));
                    waterCol *= bankAO;

                    float lap = sin(t * 1.45 + wp.x * 2.1 + wp.y * 1.5) * 0.022;
                    float meniscus = (1.0 - smoothstep(0.0, 0.040, abs(sd + 0.022 + lap))) * bridgeKeep;
                    waterCol += half3(0.024, 0.046, 0.086) * meniscus * 0.68;

                    // Sombra e marolinhas perto da ponte de madeira
                    bridgeShadow = lerp(0.44, 1.0, smoothstep(0.04, 0.58, bridgeDist));
                    float bridgeWake = (1.0 - smoothstep(0.0, 0.48, bridgeDist))
                                     * smoothstep(0.62, 0.96, sin((wp.x - t * 0.42) * 13.0 - abs(wp.y + 3.5) * 3.8));
                    surfaceColor = waterCol * bridgeShadow + half3(0.016, 0.036, 0.078) * bridgeWake * 0.42;
                }

                // --- 3. Iluminação URP 2D (Global Light 2D + Light2D dos postes/balizadores) ---
                SurfaceData2D surface;
                InputData2D lightInput;
                InitializeSurfaceData(surfaceColor, alpha,
                    SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv), normalTS, surface);
                InitializeInputData(input.uv, input.lightingUV, lightInput);
                #if defined(DEBUG_DISPLAY)
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(lightInput, input.positionWS, input.positionCS, _MainTex);
                surface.normalWS = input.normalWS;
                #endif
                half4 lit = CombinedShapeLightShared(surface, lightInput);

                // --- 4. Iluminação e reflexo suave dos postes (_TownLamps) e Lanterna (_PlayerLamp) ---
                float2 relXY = world - _Origin.xy;
                float3 surfPos = float3(relXY, 0.02);
                half3 lampReflect = 0;
                half3 mistLight = 0;

                float rRow = 0.0;
                float rippleWave = 0.0;
                float refMask = 0.0;
                if (sd < 0.0)
                {
                    rRow = floor(wp.y * 24.0);
                    rippleWave = sin(rRow * 1.15 + sin(rRow * 0.23) * 1.8 - t * 2.5);
                    float rDrift = wp.x * 2.4 - t * 0.35 + sin(rRow * 0.65 - t * 1.4) * 0.35;
                    float segHash = Hash21(float2(floor(rDrift), rRow));
                    float dashBreak = smoothstep(0.28, 0.88, segHash);
                    float rowPulse = 0.55 + 0.45 * sin(rRow * 1.85 - t * 2.9 + wp.x * 1.4);
                    refMask = smoothstep(-0.02, -0.26, sd) * bridgeShadow * lerp(0.22, 1.0, dashBreak) * rowPulse;
                }

                int lampCount = min(_TownLampCount, 88);
                [loop] for (int l = 0; l < lampCount; l++)
                {
                    float4 lamp = _TownLamps[l];
                    if (lamp.w <= 0.01) continue;

                    float3 ray = lamp.xyz - surfPos;
                    float reflectRadius = max(lamp.w * 1.55, 13.5);
                    if (dot(ray.xy, ray.xy) >= reflectRadius * reflectRadius) continue;

                    float dist3D = length(ray);
                    float3 toLight = ray / max(dist3D, 0.01);

                    float3 customCol = _TownLampColors[l].rgb;
                    half3 lampCol = dot(customCol, customCol) > 0.01 ? (half3)customCol : half3(1.75, 1.31, 0.82);

                    // Banho direto suave na margem e na água próxima ao poste (sem manchas pontudas)
                    float spillFade = saturate(1.0 - dist3D / max(lamp.w * 1.35, 0.1));
                    float ndl = saturate(dot(normalTS, toLight) * 0.35 + 0.65);
                    half3 spill = lampCol * (spillFade * spillFade * ndl);
                    lit.rgb += surfaceColor * spill * 1.40;
                    mistLight += lampCol * (spillFade * spillFade * 0.60);

                    // Coluna delicada de reflexos fragmentados em pixel art sob cada poste
                    if (sd < 0.0)
                    {
                        float distDown = lamp.y + 0.60 - relXY.y;
                        if (distDown > -0.15 && distDown < 10.5)
                        {
                            float beamWidth = 0.24 + 0.28 * saturate(distDown * 0.12);
                            float dx = abs(relXY.x - lamp.x + 0.06 * rippleWave);
                            if (dx < beamWidth)
                            {
                                float horizProfile = 1.0 - dx / beamWidth;
                                horizProfile *= horizProfile;
                                float verticalReach = smoothstep(-0.15, 0.95, distDown) * (1.0 - smoothstep(3.2, 10.2, distDown));
                                float glintCell = Hash21(float2(floor((wp.x - t * 0.22) * 5.0), rRow + l * 5.1));
                                float glints = smoothstep(0.25, 0.94, glintCell);
                                float colGlow = horizProfile * verticalReach * (0.14 + 0.52 * glints + 0.32 * wavelet) * refMask;
                                lampReflect += lampCol * colGlow * 0.26;
                            }
                        }
                    }
                }

                // Feixe da lanterna de Marcus sobre a água e o barranco
                if (_PlayerLamp.w > 0.01)
                {
                    float3 rayF = _PlayerLamp.xyz - surfPos;
                    if (dot(rayF, rayF) < _PlayerLamp.w * _PlayerLamp.w)
                    {
                        float distF = length(rayF);
                        float dist2D = length(rayF.xy);
                        float2 dirFromLamp = -rayF.xy + _PlayerLampDirection * 0.18;
                        float cone = dot(dirFromLamp, _PlayerLampDirection) / max(length(dirFromLamp), 0.01);
                        float outerSpill = smoothstep(0.48, 0.83, cone) * (1.0 - smoothstep(0.0, _PlayerLamp.w * 0.78, dist2D));
                        float mainBeam = smoothstep(0.69, 0.93, cone);
                        float hotspot = smoothstep(0.89, 0.99, cone) * (1.0 - smoothstep(0.3, _PlayerLamp.w * 0.85, dist2D));
                        float beam = saturate(outerSpill * 0.18 + mainBeam * 0.46 + hotspot * 0.26);
                        float fadeF = saturate(1.0 - distF / _PlayerLamp.w) * beam;

                        if (fadeF > 0.001)
                        {
                            float3 toFlash = rayF / max(distF, 0.01);
                            half3 flashCol = lerp(half3(1.05, 1.10, 1.18), half3(1.36, 1.42, 1.50), hotspot) * 1.55;
                            float ndlF = saturate(dot(normalTS, toFlash) * 0.42 + 0.58);
                            lit.rgb += surfaceColor * flashCol * (fadeF * ndlF * 1.35);
                            mistLight += flashCol * (fadeF * 0.50);

                            if (sd < 0.0)
                            {
                                float rippleFlash = wavelet * 0.55 + refMask * 0.35;
                                lampReflect += flashCol * (fadeF * rippleFlash * bridgeShadow * 0.18);
                            }
                        }
                    }
                }

                lit.rgb += lampReflect;

                // --- 5. Névoa baixa do Centro integrada (mais suave sobre o espelho d'água fundo) ---
                [branch] if (_TownGroundMist > 0.001 && _WindStrength > 0.001)
                {
                    float nevoa = DensidadeDaNevoaCentro(float3(world, 0.02), t, _TownGroundMist * _WindStrength);
                    float mistWeight = lerp(0.38, 0.78, saturate(sd * 0.6666667 + 1.0));
                    if (nevoa > 0.001)
                    {
                        lit.rgb = lerp(lit.rgb, CorDaNevoaCentro(mistLight), nevoa * mistWeight);
                    }
                }

                lit.a = alpha;
                return lit;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode"="NormalsRendering" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_NORMALS_INPUTS
                float4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_NORMALS_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _ShoreRect;
                float _TownGroundMist;
                float _WindStrength;
            CBUFFER_END

            Varyings NormalsRenderingVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                Varyings o = CommonNormalsVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                SetUpSpriteInstanceProperties();
                half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                return NormalsRenderingShared(main, float3(0, 0, 1), input.tangentWS, input.bitangentWS, input.normalWS);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _ShoreRect;
                float _TownGroundMist;
                float _WindStrength;
            CBUFFER_END

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                return CommonUnlitFragment(input, input.color);
            }
            ENDHLSL
        }
    }
}
