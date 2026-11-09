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
        _DoorReflection("Reflexo sob a porta", Float) = 1
        _WetRoad("Reflexo nas poças", Float) = 0
        _Bairro("Ruas de terra do bairro", Float) = 0
        _Crosswalk("Faixa de pedestres", Float) = 0
        _Entrance("Rodovia de entrada", Float) = 0
        [NoScaleOffset] _EntranceWater("Poças da rodovia", 2D) = "black" {}
        _Lightning("Clarão", Float) = 0
        _Startle("Reação do Marcus", Float) = 0
        [HideInInspector] _MirrorBounds("Limites do reflexo", Vector) = (0,0,0,0)
        [NoScaleOffset] _MarcusFrame("Quadro refletido", 2D) = "black" {}
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
            float _DoorReflection;
            float _Entrance;
            float _Crosswalk;
            float _Bairro;
            float _WetRoad;
            TEXTURE2D(_EntranceWater);
            SAMPLER(sampler_EntranceWater);
            float4 _EntranceWaterRect;
            float _Lightning;
            float _Startle;
            TEXTURE2D(_MarcusFrame);
            SAMPLER(sampler_MarcusFrame);
            float4 _MirrorUV;
            float4 _MirrorBounds;
            float _MirrorFeet;
            float4 _Receiver;
            float4 _Wind;
            float _WindStrength;
            float4 _Origin;
            float4 _ViewFocus;
            float4 _Lamps[3];
            float4 _FourthLamp;
            float4 _FifthLamp;
            float4 _PlayerLamp;
            float2 _PlayerLampDirection;
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
                output.positionCS = TransformWorldToHClip(world + float3(0, _Startle, 0));
                output.frame = output.positionCS.xy / output.positionCS.w;
                output.uv = input.uv;
                output.world = world.xy;
                output.color = input.color * unity_SpriteColor;
                return output;
            }

            float SombraElipsoide(float3 surfacePosition, float3 ray, float3 center, float3 radius)
            {
                // Distância do segmento de luz ao centro no espaço normalizado do elipsoide.
                float3 offset = (surfacePosition - center) / radius;
                float3 segment = ray / radius;
                float t = clamp(-dot(offset, segment) / max(dot(segment, segment), .0001), .015, .985);
                float3 closest = offset + t * segment;
                return 1 - smoothstep(.64, 1.14, dot(closest, closest));
            }

            float VisibilidadeDaLuz(float3 surfacePosition, float3 ray, int owner, bool handheld)
            {
                // Segmento em XYZ entre a superfície e a fonte, em unidades do mundo.
                float visibility = 1;
                float3 signRay = step(0.0, ray) * 2.0 - 1.0;
                float3 inverse = signRay / max(abs(ray), 0.0001);
                [loop] for (int i = 0; i < _CasterCount; i++)
                {
                    if (i == owner || (handheld && i == _CasterCount - 1)) continue;
                    float4 lower = _CasterMin[i];
                    float4 upper = _CasterMax[i];
                    if (lower.w > 2.5)
                    {
                        float3 size = upper.xyz - lower.xyz;
                        float2 center = (lower.xy + upper.xy) * .5;
                        if (lower.w > 3.5)
                        {
                            float occlusion = SombraElipsoide(surfacePosition, ray,
                                float3(center, size.z * .40), size * float3(.50, .53, .53));
                            visibility = min(visibility, 1 - .66 * occlusion);
                            continue;
                        }
                        center.y -= .22;
                        float body = SombraElipsoide(surfacePosition, ray,
                            float3(center, size.z * .31), size * float3(.50, .54, .31));
                        float roof = SombraElipsoide(surfacePosition, ray,
                            float3(center, size.z * .62), size * float3(.30, .41, .30));
                        // Carroceria e cabine formam uma única projeção.
                        visibility = min(visibility, 1 - .88 * max(body, roof));
                        continue;
                    }
                    if (lower.w > .5) continue;
                    float3 a = (lower.xyz - surfacePosition) * inverse;
                    float3 b = (upper.xyz - surfacePosition) * inverse;
                    float3 nearT = min(a, b);
                    float3 farT = max(a, b);
                    float enter = max(max(nearT.x, nearT.y), nearT.z);
                    float exit = min(min(farT.x, farT.y), farT.z);
                    if (max(enter, 0.015) < min(exit, 0.985)) return 0;
                }
                return visibility;
            }

            struct GroundSample
            {
                float4 albedo;
                float4 surface;
                float4 relief;
                float asphalt;
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
                float atlasY = uv.y * _MainTex_TexelSize.w / GroundPixelsPerUnit;
                float row = floor(atlasY);
                // O tile de meio-fio tem calçada na metade superior do atlas e asfalto na inferior.
                result.asphalt = row == 7 ? 1 - smoothstep(.47, .53, frac(atlasY)) :
                    (row == 11 || (row >= 3 && row <= 5) ? 1 : 0);
                result.surface.rg = (result.surface.rg * 2 - 1) * axis;
                result.relief.gb = (result.relief.gb * 255 - 128) / 127 * axis;
                return result;
            }

            GroundSample BlendGround(GroundSample a, GroundSample b, float weight)
            {
                a.albedo = lerp(a.albedo, b.albedo, weight);
                a.surface = lerp(a.surface, b.surface, weight);
                a.relief = lerp(a.relief, b.relief, weight);
                a.asphalt = lerp(a.asphalt, b.asphalt, weight);
                return a;
            }

            GroundSample AmostraTrilha(float2 p, float side)
            {
                float variant = fmod(floor(p.x) + floor(p.y) * 3, 4);
                float dirtRow = _MainTex_TexelSize.w / GroundPixelsPerUnit - 4;
                float2 pixel = (float2(variant, dirtRow) + frac(p)) * GroundPixelsPerUnit;
                GroundSample track = SampleGround((floor(pixel) + .5) * _MainTex_TexelSize.xy, float2(1, 1));
                float ruts = 1 - smoothstep(.12, .38, abs(abs(side) - .83));
                float grain = frac(sin(dot(floor(p * 32), float2(39.45, 17.23))) * 46293.1);
                float gravel = smoothstep(.965, .997, grain) * (1 - ruts);
                track.albedo.rgb *= lerp(1.13, .73, ruts) * lerp(.88, 1.12, grain);
                track.albedo.rgb += gravel * float3(.13, .10, .067);
                track.relief.r *= 1 - ruts * .32;
                track.surface.b = lerp(track.surface.b, .56, ruts);
                return track;
            }

            GroundSample TrilhaDaEncruzilhada(float2 p, GroundSample grass, out float dirt)
            {
                float distanceUp = p.y - 14;
                float center = 20.5 + .48 * sin(distanceUp * .48) - .075 * distanceUp;
                float side = p.x - center;
                float width = lerp(2.55, 1.70, smoothstep(0, 3.8, distanceUp));
                float edge = .14 * sin(p.y * 6.3 + side * 7.2) + .08 * sin(p.y * 15.7 - side * 9.6);
                dirt = 1 - smoothstep(width - .43, width + .22, abs(side) + edge);
                return BlendGround(grass, AmostraTrilha(p, side), dirt);
            }

            void CaminhoDeBarro(float2 p, float2 start, float2 end, float width, inout float dirt, inout float side)
            {
                float2 axis = normalize(end - start);
                float2 relative = p - start;
                float lengthAlong = dot(relative, axis);
                float lengthTotal = length(end - start);
                float bend = sin(lengthAlong * .57) * .21 + sin(lengthAlong * 1.26) * .10;
                float across = dot(relative, float2(-axis.y, axis.x)) + bend;
                float beyond = max(max(-lengthAlong, lengthAlong - lengthTotal), 0);
                float rough = .11 * sin(p.x * 12.6 + p.y * 6.9) + .09 * sin(p.x * 4.9 - p.y * 7.1);
                float distance = length(float2(across, beyond)) + rough;
                float amount = 1 - smoothstep(width - .31, width + .30, distance);
                if (amount > dirt || (amount > .95 && abs(across) < abs(side)))
                {
                    dirt = max(dirt, amount);
                    side = across;
                }
            }

            GroundSample TrilhaDoBairro(float2 p, GroundSample ground, out float dirt)
            {
                dirt = 0;
                float side = 10;
                CaminhoDeBarro(p, float2(20, -1), float2(20.1, 9.5), 1.7, dirt, side);
                CaminhoDeBarro(p, float2(6.4, 9.35), float2(36.0, 9.7), 1.40, dirt, side);
                CaminhoDeBarro(p, float2(19.3, 9.5), float2(18.45, 21.4), 1.85, dirt, side);
                CaminhoDeBarro(p, float2(6.25, 21.45), float2(35.45, 21.55), 1.45, dirt, side);
                CaminhoDeBarro(p, float2(35.05, 21.55), float2(35.55, 35), 2.12, dirt, side);
                CaminhoDeBarro(p, float2(15.0, 9.55), float2(15.0, 12.35), .87, dirt, side);
                CaminhoDeBarro(p, float2(25.85, 12.1), float2(24.5, 9.65), .98, dirt, side);
                if (dirt > 0) return BlendGround(ground, AmostraTrilha(p, side), dirt);
                return ground;
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
                if (_Entrance > .5) span *= .40;
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

            float EntranceHeight(float2 position, float2 fallbackPixel, float2 fallbackTile, float2 cell)
            {
                float2 nextCell = floor(position);
                if (all(nextCell == cell) || any(nextCell < 0) || any(nextCell >= _GroundCells_TexelSize.zw))
                    return SampleHeight(fallbackPixel, fallbackTile);

                float4 tile = SAMPLE_TEXTURE2D_LOD(_GroundCells, sampler_GroundCells,
                    (nextCell + .5) * _GroundCells_TexelSize.xy, 0);
                float2 local = lerp(frac(position), 1 - frac(position), step(.5, tile.ba));
                float2 pixel = round(tile.rg * 255) * GroundPixelsPerUnit +
                    min(floor(local * GroundPixelsPerUnit), GroundPixelsPerUnit - 1) + .5;
                return SAMPLE_TEXTURE2D_LOD(_GroundRelief, sampler_GroundRelief,
                    RoadUnderPaint(pixel * _GroundRelief_TexelSize.xy), 0).r;
            }

            float HeightExposure(float2 pixel, float2 tileMin, float2 position, float groundHeight,
                float3 light, float2 worldDirection)
            {
                float horizontal = max(length(light.xy), .01);
                float2 direction = light.xy / horizontal;
                float rise = light.z / (horizontal * GroundPixelsPerUnit * GroundHeightScale);
                float nearBank, farBank;
                if (_Entrance > .5)
                {
                    float2 cell = floor(position);
                    float2 stepWorld = worldDirection / (horizontal * GroundPixelsPerUnit);
                    nearBank = EntranceHeight(position + stepWorld * 2, pixel + direction * 2, tileMin, cell);
                    farBank = EntranceHeight(position + stepWorld * 5, pixel + direction * 5, tileMin, cell);
                }
                else
                {
                    nearBank = SampleHeight(pixel + direction * 2, tileMin);
                    farBank = SampleHeight(pixel + direction * 5, tileMin);
                }
                nearBank -= 2 * rise;
                farBank -= 5 * rise;
                return 1 - smoothstep(.045, .25, max(nearBank, farBank) - groundHeight);
            }

            float ContactOcclusion(float2 xy)
            {
                float contact = 0;
                [loop] for (int i = 0; i < _CasterCount; i++)
                {
                    // Contato elíptico para objetos pequenos e veículos.
                    float mode = _CasterMin[i].w;
                    if (mode > 1.5 && mode < 2.5) continue;
                    if (mode > .5)
                    {
                        float2 center = (_CasterMin[i].xy + _CasterMax[i].xy) * .5;
                        if (mode > 2.5) center.y = lerp(_CasterMin[i].y, _CasterMax[i].y, mode > 3.5 ? .22 : .18);
                        float2 radius = (_CasterMax[i].xy - _CasterMin[i].xy) * .5 + float2(.14, .13);
                        float2 delta = (xy - center) / radius;
                        float ellipse = saturate(1 - dot(delta, delta));
                        contact = max(contact, .62 * ellipse * ellipse);
                        continue;
                    }
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
                float asphalt = 0;
                float water = 0;
                float2 waterLook = 0;
                float4 ground = 0;
                float4 detail = 0;
                bool groundRelief = groundPbr && _GroundReliefStrength > 0;
                float2 axis = sign(float2(ddx(input.uv.x) * ddx(position.x), ddy(input.uv.y) * ddy(position.y)));
                float seam = 0;
                [branch] if (groundPbr)
                {
                    GroundSample terrain = JoinGround(position, input.uv, axis, seam);
                    float dirt = 0;
                    if (_Crosswalk > .5 && xy.y >= 14 && xy.x >= 16 && xy.x < 25)
                        terrain = TrilhaDaEncruzilhada(xy, terrain, dirt);
                    if (_Bairro > .5)
                        terrain = TrilhaDoBairro(xy, terrain, dirt);
                    float2 roadUV = RoadUnderPaint(input.uv);
                    float pixelY = input.uv.y * _MainTex_TexelSize.w;
                    float3 paint = 0;
                    if (roadUV.y != input.uv.y && fmod(pixelY, GroundPixelsPerUnit) < 4)
                        paint = sprite.rgb - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, roadUV).rgb * input.color.rgb;
                    sprite = terrain.albedo * input.color;
                    sprite.rgb = saturate(sprite.rgb + paint);
                    ground = terrain.surface;
                    detail = terrain.relief;
                    asphalt = terrain.asphalt;
                    if (_Crosswalk > .5 && xy.x >= 10 && xy.x < 14 && xy.y > 8.23 && xy.y < 11.77)
                    {
                        float barra = step(frac((xy.x - 10) / .8), .54);
                        float desgaste = smoothstep(.06, .32, detail.r);
                        sprite.rgb = lerp(sprite.rgb, float3(.63, .59, .48), barra * lerp(.55, .89, desgaste));
                    }
                    if (_WetRoad > .5 && max(asphalt, dirt) > 0)
                    {
                        float2 waterUV = (position - _EntranceWaterRect.xy) / _EntranceWaterRect.zw;
                        if (all(waterUV >= 0) && all(waterUV < 1))
                        {
                            float3 pool = SAMPLE_TEXTURE2D(_EntranceWater, sampler_EntranceWater, waterUV).rgb;
                            water = pool.r * max(asphalt, dirt);
                            waterLook = pool.gb;
                            ground.b = lerp(ground.b, .34, water);
                            detail.gb *= 1 - .92 * water;
                        }
                    }
                }
                float terrainOcclusion = 1;
                float terrainHeight = 0;
                float3 ambientNormal = normal;
                float2 detailSlope = 0;
                float2 reliefUV = _Entrance > .5 ? RoadUnderPaint(input.uv) : input.uv;
                float2 reliefPixel = reliefUV * _GroundRelief_TexelSize.zw;
                float2 tileMin = floor(reliefPixel / GroundPixelsPerUnit) * GroundPixelsPerUnit;
                [branch] if (groundRelief)
                {
                    terrainHeight = detail.r;
                    surfacePosition.z = max(.005, surfacePosition.z
                        + (terrainHeight - .31) * GroundHeightScale * _GroundReliefStrength);
                    detailSlope = detail.gb * (1.32 * _GroundReliefStrength);
                    if (_Entrance > .5)
                    {
                        // Microtextura contínua no mundo, inclusive onde as normais dos tiles se misturam.
                        float2 grain = frac(sin(float2(dot(xy, float2(137.5, 23.7)),
                            dot(xy, float2(47.3, 197.1)))) * 8191) - .5;
                        detailSlope += grain * (.34 * asphalt * (1 - water));
                    }
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
                if (_Entrance > .5)
                {
                    // Preserve os marcos no passeio e aprofunde a penumbra do bosque.
                    light *= lerp(1, .43, smoothstep(14.3, 18.7, upright ? baseY : xy.y));

                }
                light += _Lightning * half3(.66, .80, 1.00);
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
                [unroll] for (int l = 0; l < 6; l++)
                {
                    bool handheld = l == 3;
                    float4 lamp = handheld ? _PlayerLamp :
                        l == 4 ? _FourthLamp : l == 5 ? _FifthLamp : _Lamps[min(l, 2)];
                    if (lamp.w <= 0) continue;
                    float3 ray = lamp.xyz - surfacePosition;
                    float distanceToLamp = length(ray);
                    float fade = saturate(1.0 - distanceToLamp / lamp.w);
                    if (handheld)
                    {
                        float cone = dot(-ray.xy / max(length(ray.xy), .01), _PlayerLampDirection);
                        fade *= smoothstep(.69, .94, cone);
                    }
                    if (fade <= 0) continue;
                    float3 toLight = ray / max(distanceToLamp, .01);
                    float facing = saturate(dot(normal, toLight) * .87 + .13);
                    float shadow = VisibilidadeDaLuz(surfacePosition, ray, owner, handheld);
                    half3 lampColor = handheld ? half3(2.6, 2.25, 1.63) : _LampColors[min(l, 2)].rgb;
                    if (_Entrance > .5 && !handheld && xy.y > lamp.y)
                        shadow *= lerp(.79, 1, shadow * shadow);
                    half3 incident = lampColor * (fade * fade * shadow * (groundPbr ? 1.22 : 1));
                    float exposure = 1;
                    [branch] if (groundRelief && shadow > 0)
                    {
                        float3 lightUV = float3(axis * toLight.xy, toLight.z);
                        float seamExposure = _Entrance > .5 ? 1 : 1 - 2 * seam;
                        exposure = lerp(1, HeightExposure(reliefPixel, tileMin, xy, terrainHeight, lightUV, toLight.xy),
                            .23 * _GroundReliefStrength * seamExposure);
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
                        if (l == 1 && _DoorReflection > .5)
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
                // Projeção planar do quadro atual, recortada pelo material da poça.
                [branch] if (_WetRoad > .5 && groundPbr && _MirrorBounds.z > 0)
                {
                    float separation = _MirrorFeet - xy.y;
                    float wet = water * (1 - smoothstep(.65, 1.1, separation));
                    if (separation > 0 && wet > 0)
                    {
                        float ripple = sin(xy.y * 49 - _Time.y * 6.1) * .032;
                        float2 projected = float2(xy.x + ripple, _MirrorFeet + separation * 1.8);
                        float2 reflectedUV = (projected - _MirrorBounds.xy) / _MirrorBounds.zw;
                        if (all(reflectedUV > 0) && all(reflectedUV < 1))
                        {
                            half4 frame = SAMPLE_TEXTURE2D(_MarcusFrame, sampler_MarcusFrame,
                                _MirrorUV.xy + reflectedUV * _MirrorUV.zw);
                            half3 image = frame.rgb * (half3(.39, .50, .64) + light * .55);
                            reflection = lerp(reflection, image * .58, frame.a * wet * .76);
                        }
                    }
                }
                float contact = upright ? 1 : ContactOcclusion(xy);
                // Nas bordas o olhar perde luz aos poucos. Luzes do cenário continuam
                // marcando as referências mesmo quando a fachada sai do centro da câmera.
                float edge = smoothstep(.50, 1.30, length(input.frame * float2(1, .86)));
                float vignette = 1 - edge * .48 * _ViewFocus.z * (1 - .64 * saturate(practicalLight));
                float entranceGrade = 1;
                if (_Entrance > .5)
                {
                    float far = smoothstep(14.2, 18.5, upright ? baseY : xy.y);
                    entranceGrade = (1 - .20 * far - .47 * asphalt) * (1 - .13 * (1 - saturate(practicalLight)));
                    reflection *= 1 - .18 * asphalt * (1 - water);
                    // O brilho frio destaca a água sem clarear o asfalto que a circunda.
                    float ripple = .5 + .5 * sin(xy.y * 47 + sin(xy.x * 12) * 1.1 - _Time.y * 2.5);
                    half3 waterColor = lerp(half3(.53, .55, .56), half3(.24, .48, .73), waterLook.x);
                    reflection += waterColor * water * waterLook.y *
                        (.012 + .012 * ripple + .023 * saturate(practicalLight));
                    entranceGrade = lerp(entranceGrade, 1, _Lightning * .85);
                }
                return half4((sprite.rgb * light * entranceGrade + reflection) * contact * vignette, sprite.a);
            }
            ENDHLSL
        }
    }
}
