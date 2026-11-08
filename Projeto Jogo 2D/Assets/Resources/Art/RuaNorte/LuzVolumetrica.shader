Shader "SantaRita/RuaNorte/LuzVolumetrica"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        [PerRendererData] _Receiver("Base, altura e oclusor", Vector) = (0, 0, 0, 0)
        [PerRendererData] _Wind("Amplitude, copa e fase", Vector) = (0, 0, 0, 0)
        _WindStrength("Vento", Float) = 0
        _GroundCells("Células do terreno", 2D) = "black" {}
        _GroundSurface("Normal XY, rugosidade e cavidades", 2D) = "gray" {}
        _GroundSurfaceEnabled("Relevo do terreno", Float) = 0
        _GroundRelief("Altura, gradientes XY e oclusão", 2D) = "black" {}
        _GroundReliefStrength("Intensidade do relevo adicional", Range(0, 1)) = 0
        _ContactStrength("Sombras de contato", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.5
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GroundCells);
            SAMPLER(sampler_GroundCells);
            float4 _GroundCells_TexelSize;
            float4 _MainTex_TexelSize;
            TEXTURE2D(_GroundSurface);
            SAMPLER(sampler_GroundSurface);
            TEXTURE2D(_GroundRelief);
            SAMPLER(sampler_GroundRelief);
            float4 _GroundRelief_TexelSize;
            float _GroundSurfaceEnabled;
            float _GroundReliefStrength;
            float _ContactStrength;
            float4 _Receiver;
            float4 _Wind;
            float _WindStrength;
            float4 _Origin;
            float4 _ViewFocus;
            float4 _Lamps[3];
            float4 _LampColors[3];
            float4 _CasterMin[32];
            float4 _CasterMax[32];
            int _CasterCount;

            static const float GroundPixelsPerUnit = 64.0;
            static const float GroundHeightScale = .22;
            static const float3 GroundView = float3(0, -.46, .888);

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 world : TEXCOORD1;
                float2 frame : TEXCOORD2;
                half4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                float3 world = TransformObjectToWorld(input.positionOS);
                if (_Wind.x > 0)
                {
                    float crown = smoothstep(.18, .94, (world.y - _Receiver.x) / max(_Wind.y, .1));
                    float gust = .55 + .45 * sin(_Time.y * .73 + world.x * .19);
                    float sway = sin(_Time.y * 1.28 + _Wind.z) * .72
                               + sin(_Time.y * 2.11 + _Wind.z * .63) * .28;
                    world.x += _WindStrength * _Wind.x * crown * gust * sway;
                }
                output.positionCS = TransformWorldToHClip(world);
                output.frame = output.positionCS.xy / output.positionCS.w;
                output.uv = input.uv;
                output.world = world.xy;
                output.color = input.color * unity_SpriteColor;
                return output;
            }

            bool Occluded(float3 surfacePosition, float3 ray, int owner)
            {
                // Segmento em XYZ entre a superfície e a fonte, em unidades do mundo.
                float3 signRay = step(0.0, ray) * 2.0 - 1.0;
                float3 inverse = signRay / max(abs(ray), 0.0001);
                [loop] for (int i = 0; i < _CasterCount; i++)
                {
                    if (i == owner) continue;
                    float3 a = (_CasterMin[i].xyz - surfacePosition) * inverse;
                    float3 b = (_CasterMax[i].xyz - surfacePosition) * inverse;
                    float3 nearT = min(a, b);
                    float3 farT = max(a, b);
                    float enter = max(max(nearT.x, nearT.y), nearT.z);
                    float exit = min(min(farT.x, farT.y), farT.z);
                    if (max(enter, 0.015) < min(exit, 0.985)) return true;
                }
                return false;
            }

            struct GroundSample
            {
                float4 albedo;
                float4 surface;
                float4 relief;
            };

            float2 RoadUnderPaint(float2 uv)
            {
                // A faixa viária é um decalque de cor sobre o asfalto. Mescle o
                // substrato nas juntas e aplique a tinta na posição original.
                const float stripeRow = 3;
                const float roadRow = 11;
                float row = floor(uv.y * _MainTex_TexelSize.w / GroundPixelsPerUnit);
                return uv + float2(0, (row == stripeRow ? roadRow - stripeRow : 0)
                    * GroundPixelsPerUnit * _MainTex_TexelSize.y);
            }

            GroundSample SampleGround(float2 uv, float2 axis)
            {
                uv = RoadUnderPaint(uv);
                GroundSample result;
                result.albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                result.surface = SAMPLE_TEXTURE2D(_GroundSurface, sampler_GroundSurface, uv);
                result.relief = SAMPLE_TEXTURE2D(_GroundRelief, sampler_GroundRelief, uv);
                result.surface.rg = (result.surface.rg * 2 - 1) * axis;
                result.relief.gb = (result.relief.gb * 255 - 128) / 127 * axis;
                return result;
            }

            GroundSample BlendGround(GroundSample a, GroundSample b, float weight)
            {
                a.albedo = lerp(a.albedo, b.albedo, weight);
                a.surface = lerp(a.surface, b.surface, weight);
                a.relief = lerp(a.relief, b.relief, weight);
                return a;
            }

            GroundSample NeighborGround(float2 cell, float2 local, float2 offset)
            {
                float4 tile = SAMPLE_TEXTURE2D(_GroundCells, sampler_GroundCells,
                    (cell + offset + .5) * _GroundCells_TexelSize.xy);
                float2 flip = step(.5, tile.ba);
                float2 mirrored = lerp(local, 1 - local, abs(offset));
                float2 pixel = floor(lerp(mirrored, 1 - mirrored, flip) * GroundPixelsPerUnit);
                float2 uv = (round(tile.rg * 255) * GroundPixelsPerUnit + pixel + .5) * _MainTex_TexelSize.xy;
                return SampleGround(uv, 1 - 2 * flip);
            }

            GroundSample JoinGround(float2 position, float2 uv, float2 axis, out float seam)
            {
                float2 cell = floor(position);
                float2 local = (floor(frac(position) * GroundPixelsPerUnit) + .5) / GroundPixelsPerUnit;
                float2 side = step(.5, local) * 2 - 1;
                float2 neighbor = cell + side;
                float2 valid = step(0, neighbor) * step(neighbor, _GroundCells_TexelSize.zw - 1);

                // Juntas largas o bastante para casar o tom e o relevo. A borda dos
                // materiais continua texturizada porque as duas amostras são Point.
                float2 edgeDistance = min(local, 1 - local);
                // Varie a largura ao longo de cada junta, em coordenadas do mundo.
                // As duas células usam a mesma faixa, inclusive em cantos com quatro tiles.
                float2 span = .27 + .075 * sin(position.yx * 5.4 + round(position) * 2.3);
                float2 blend = .5 * (1 - smoothstep(0, span, edgeDistance)) * valid;
                seam = max(blend.x, blend.y);
                GroundSample a = SampleGround(uv, axis);
                [branch] if (blend.x > 0)
                    a = BlendGround(a, NeighborGround(cell, local, float2(side.x, 0)), blend.x);
                [branch] if (blend.y > 0)
                {
                    GroundSample b = NeighborGround(cell, local, float2(0, side.y));
                    [branch] if (blend.x > 0)
                        b = BlendGround(b, NeighborGround(cell, local, side), blend.x);
                    a = BlendGround(a, b, blend.y);
                }
                return a;
            }

            float SampleHeight(float2 pixel, float2 tileMin)
            {
                // A busca para na última amostra do tile, antes de entrar em outro material do atlas.
                float2 safePixel = clamp(pixel, tileMin + .5, tileMin + GroundPixelsPerUnit - .5);
                return SAMPLE_TEXTURE2D_LOD(_GroundRelief, sampler_GroundRelief,
                    safePixel * _GroundRelief_TexelSize.xy, 0).r;
            }

            float HeightExposure(float2 pixel, float2 tileMin, float groundHeight, float3 light)
            {
                float horizontal = max(length(light.xy), .01);
                float2 direction = light.xy / horizontal;
                float rise = light.z / (horizontal * GroundPixelsPerUnit * GroundHeightScale);
                float nearBank = SampleHeight(pixel + direction * 2, tileMin) - 2 * rise;
                float farBank = SampleHeight(pixel + direction * 5, tileMin) - 5 * rise;
                return 1 - smoothstep(.045, .25, max(nearBank, farBank) - groundHeight);
            }

            float ContactOcclusion(float2 xy)
            {
                float contact = 0;
                [loop] for (int i = 0; i < _CasterCount; i++)
                {
                    float2 nearest = clamp(xy, _CasterMin[i].xy, _CasterMax[i].xy);
                    float2 gap = (nearest - xy) * float2(1, .78);
                    float spread = .13 + min(.22, _CasterMax[i].z * .055);
                    float distanceSquared = dot(gap, gap);
                    float fade = saturate(1 - distanceSquared / (spread * spread));
                    contact = max(contact, fade * fade);
                }
                return 1 - contact * .58 * _ContactStrength;
            }

            float GroundSpecular(float3 normal, float3 toLight, float roughness)
            {
                float3 halfway = normalize(GroundView + toLight);
                float nh = saturate(dot(normal, halfway));
                float nv = saturate(dot(normal, GroundView));
                float nl = saturate(dot(normal, toLight));
                float vh = saturate(dot(GroundView, halfway));
                float a2 = roughness * roughness;
                a2 *= a2;
                float d = nh * nh * (a2 - 1) + 1;
                float distribution = a2 / max(3.14159 * d * d, .001);
                float k = (roughness + 1) * (roughness + 1) * .125;
                float visibility = rcp(max(4 * (nl * (1 - k) + k) * (nv * (1 - k) + k), .001));
                float fresnel = .04 + .96 * pow(1 - vh, 5);
                return min(.65, distribution * visibility * fresnel) * nl;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                clip(sprite.a - .001);

                float2 position = input.world - _Origin.xy;
                float2 xy = (floor(position * GroundPixelsPerUnit) + .5) / GroundPixelsPerUnit;
                float baseY = _Receiver.x - _Origin.y;
                bool upright = _Receiver.y > 0;
                float height = upright ? max(0, (xy.y - baseY) * _Receiver.y) : 0;
                float3 surfacePosition = float3(xy.x, upright ? baseY : xy.y, height + .02);
                int owner = (int)_Receiver.z - 1;
                float proxyHeight = owner >= 0 && owner < _CasterCount ? _CasterMax[clamp(owner, 0, 31)].z : 1.5;
                float top = smoothstep(proxyHeight * .30, proxyHeight * .95, height);
                float3 normal = upright
                    ? normalize(lerp(float3(0, -.79, .61), float3(0, 0, 1), top * .85))
                    : float3(0, 0, 1);
                bool groundPbr = !upright && _GroundSurfaceEnabled > .5;
                float4 ground = 0;
                float4 detail = 0;
                bool groundRelief = groundPbr && _GroundReliefStrength > 0;
                float2 axis = sign(float2(ddx(input.uv.x) * ddx(position.x), ddy(input.uv.y) * ddy(position.y)));
                float seam = 0;
                [branch] if (groundPbr)
                {
                    GroundSample terrain = JoinGround(position, input.uv, axis, seam);
                    float2 roadUV = RoadUnderPaint(input.uv);
                    float pixelY = input.uv.y * _MainTex_TexelSize.w;
                    float3 paint = 0;
                    if (roadUV.y != input.uv.y && fmod(pixelY, GroundPixelsPerUnit) < 4)
                        paint = sprite.rgb - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, roadUV).rgb * input.color.rgb;
                    sprite = terrain.albedo * input.color;
                    sprite.rgb = saturate(sprite.rgb + paint);
                    ground = terrain.surface;
                    detail = terrain.relief;
                }
                float terrainOcclusion = 1;
                float terrainHeight = 0;
                float3 ambientNormal = normal;
                float2 detailSlope = 0;
                float2 reliefPixel = input.uv * _GroundRelief_TexelSize.zw;
                float2 tileMin = floor(reliefPixel / GroundPixelsPerUnit) * GroundPixelsPerUnit;
                [branch] if (groundRelief)
                {
                    terrainHeight = detail.r;
                    surfacePosition.z = max(.005, surfacePosition.z
                        + (terrainHeight - .31) * GroundHeightScale * _GroundReliefStrength);
                    detailSlope = detail.gb * (1.32 * _GroundReliefStrength);
                    terrainOcclusion = lerp(1, detail.a, .16 * _GroundReliefStrength);
                }
                if (groundPbr)
                {
                    float2 slope = ground.rg;
                    float3 presetNormal = float3(slope * 1.65, sqrt(saturate(1 - dot(slope, slope))));
                    float3 reliefNormal = float3(detailSlope, 0);
                    normal = normalize(presetNormal + reliefNormal);
                    ambientNormal = normalize(presetNormal + reliefNormal * .14);
                }
                // A mesma luz ambiente fria alcança terreno e sprites. O bosque ao fundo fica em penumbra.
                float depth = smoothstep(13.7, 20.8, upright ? baseY : xy.y);
                half3 light = lerp(half3(.61, .70, .77), half3(.26, .36, .43), depth);
                float2 fromMarcus = (xy - _ViewFocus.xy) * float2(.75, 1);
                float falloff = smoothstep(2.3, 7.0, length(fromMarcus));
                light *= 1 - .32 * falloff * _ViewFocus.z;
                if (groundPbr)
                {
                    // Luz do céu rasante, visível nos relevos mesmo entre dois postes.
                    const float3 sky = float3(-.56, -.51, .652);
                    float relief = clamp(1 + (dot(ambientNormal, sky) - sky.z) * .39, .78, 1.21);
                    float moisture = sin(xy.x * 1.62 + xy.y * .69) * sin(xy.y * 1.41 - xy.x * .54);
                    light *= lerp(1, ground.a, .62) * relief * terrainOcclusion * (1 + .07 * moisture);
                }
                half3 reflection = 0;
                float practicalLight = 0;
                [unroll] for (int l = 0; l < 3; l++)
                {
                    float3 ray = _Lamps[l].xyz - surfacePosition;
                    float distanceToLamp = length(ray);
                    float fade = saturate(1.0 - distanceToLamp / _Lamps[l].w);
                    if (fade <= 0) continue;
                    float3 toLight = ray / max(distanceToLamp, .01);
                    float facing = saturate(dot(normal, toLight) * .87 + .13);
                    float shadow = Occluded(surfacePosition, ray, owner) ? .0 : 1.0;
                    half3 incident = _LampColors[l].rgb * (fade * fade * shadow * (groundPbr ? 1.22 : 1));
                    float exposure = 1;
                    [branch] if (groundRelief && shadow > 0)
                    {
                        float3 lightUV = float3(axis * toLight.xy, toLight.z);
                        exposure = lerp(1, HeightExposure(reliefPixel, tileMin, terrainHeight, lightUV),
                            .23 * _GroundReliefStrength * (1 - 2 * seam));
                    }
                    light += incident * facing * exposure;
                    practicalLight += incident.r * facing;
                    if (groundPbr)
                    {
                        // A água suaviza a microtextura. A fonte, a normal e a rugosidade
                        // determinam o reflexo, inclusive na transição entre poça e asfalto.
                        float wet = (1 - smoothstep(.50, .82, ground.b)) * _GroundReliefStrength;
                        float3 wetNormal = normalize(lerp(normal, float3(0, 0, 1), wet * .38));
                        float roughness = lerp(max(.20, ground.b), max(.39, ground.b), wet);
                        float specular = GroundSpecular(wetNormal, toLight, roughness);
                        float softLimit = lerp(specular, specular / (1 + 3 * specular), _GroundReliefStrength);
                        reflection += incident * softLimit * 1.25 * exposure;
                        if (l == 1)
                        {
                            // Brilho quebrado pela água sob a porta. O desenho segue o piso,
                            // com trechos curtos e horizontais entre os grãos do asfalto.
                            float2 fromDoor = xy - _Lamps[l].xy;
                            float streak = frac(sin(dot(floor(xy * float2(18, 51)), float2(73.1, 31.7))) * 32719.3);
                            float wash = 1 - smoothstep(.12, 1.65, abs(fromDoor.x));
                            wash *= smoothstep(-4.2, -.5, fromDoor.y) * (1 - smoothstep(.1, .65, fromDoor.y));
                            reflection += incident * wash * smoothstep(.61, 1, streak) * (.021 + .024 * wet);
                        }
                    }
                }
                float contact = upright ? 1 : ContactOcclusion(xy);
                // Nas bordas o olhar perde luz aos poucos. Luzes do cenário continuam
                // marcando as referências mesmo quando a fachada sai do centro da câmera.
                float edge = smoothstep(.50, 1.30, length(input.frame * float2(1, .86)));
                float vignette = 1 - edge * .48 * _ViewFocus.z * (1 - .64 * saturate(practicalLight));
                return half4((sprite.rgb * light + reflection) * contact * vignette, sprite.a);
            }
            ENDHLSL
        }
    }
}
