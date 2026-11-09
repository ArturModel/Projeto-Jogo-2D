Shader "SantaRita/RuaNorte/LuzVolumetrica"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        [PerRendererData] _Receiver("Base, altura e oclusor", Vector) = (0, 0, 0, 0)
        [PerRendererData] _Wind("Amplitude, altura, fase e folhagem", Vector) = (0, 0, 0, 0)
        [PerRendererData] _CanopyTone("Compensação da copa e da casca", Vector) = (1, 1, 1, 1)
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
        [HideInInspector] _BarStreet("Rua do Bar do Zé", Float) = 0
        _Crosswalk("Faixa de pedestres", Float) = 0
        _Entrance("Rodovia de entrada", Float) = 0
        [NoScaleOffset] _EntranceWater("Poças da rodovia", 2D) = "black" {}
        _Arrival("Composição da chegada", Float) = 0
        [NoScaleOffset] _ArrivalShadows("Sombras dos postes", 2D) = "black" {}
        [PerRendererData] [NoScaleOffset] _PropSurface("Relevo e rugosidade do sprite", 2D) = "gray" {}
        [PerRendererData] [NoScaleOffset] _SurfaceMap("Normal XY, rugosidade e metal", 2D) = "white" {}
        [PerRendererData] _PropSurfaceEnabled("Relevo do sprite ativo", Float) = 0
        [NoScaleOffset] _ArrivalCar("Carro refletido", 2D) = "black" {}
        [HideInInspector] _FourthLamp("Quarto poste", Vector) = (0, 0, 0, 0)
        [HideInInspector] _FifthLamp("Quinto poste", Vector) = (0, 0, 0, 0)
        [HideInInspector] _AccentLamp("Luz de destaque", Vector) = (0, 0, 0, 0)
        [HideInInspector] _AccentColor("Cor da luz de destaque", Vector) = (0, 0, 0, 0)
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
            #include "../SpriteSurface.hlsl"
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
            float _BarStreet;
            float _WetRoad;
            TEXTURE2D(_EntranceWater);
            SAMPLER(sampler_EntranceWater);
            float4 _EntranceWaterRect;
            float _Arrival;
            TEXTURE2D(_ArrivalShadows);
            SAMPLER(sampler_ArrivalShadows);
            TEXTURE2D(_PlayerShadows);
            SAMPLER(sampler_PlayerShadows);
            float2 _ArrivalShadowSize;
            float _ArrivalShadowEnabled;
            float _PlayerShadowEnabled;
            int _ArrivalPlayerCaster;
            TEXTURE2D(_PropSurface);
            SAMPLER(sampler_PropSurface);
            float _PropSurfaceEnabled;
            TEXTURE2D(_ArrivalCar);
            SAMPLER(sampler_ArrivalCar);
            float4 _ArrivalCarUV;
            float4 _ArrivalCarBounds;
            float _ArrivalCarFeet;
            float _Lightning;
            float _Startle;
            TEXTURE2D(_MarcusFrame);
            SAMPLER(sampler_MarcusFrame);
            float4 _MarcusFrame_TexelSize;
            float4 _MirrorUV;
            float4 _MirrorBounds;
            float _MirrorFeet;
            float4 _Receiver;
            float4 _Wind;
            float4 _CanopyTone;
            float _WindStrength;
            float4 _Origin;
            float4 _ViewFocus;
            float4 _Lamps[3];
            float4 _FourthLamp;
            float4 _FifthLamp;
            float4 _MarkerLamps[4];
            float4 _BairroPostes[4];
            float4 _BairroTrechos[15];
            float4 _BairroMargens[15];
            float4 _AccentLamp;
            float4 _AccentColor;
            float4 _PlayerLamp;
            float2 _PlayerLampDirection;
            float4 _LampColors[3];
            float4 _CasterMin[48];
            float4 _CasterMax[48];
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

            float RecorteDoMarcus(float3 surface, float3 ray)
            {
                if (_MirrorBounds.z <= 0 || _MirrorBounds.w <= 0) return 0;

                // A penumbra curta conecta a projeção aos pés mesmo entre dois texels do sprite.
                float2 direction = ray.xy / max(length(ray.xy), .001);
                float2 feet = float2(_MirrorBounds.x + _MirrorBounds.z * .5, _MirrorFeet);
                float2 delta = surface.xy - feet;
                float along = (dot(delta, direction) + .11) / .31;
                float acrossFoot = dot(delta, float2(-direction.y, direction.x)) / .20;
                float join = .55 * (1 - smoothstep(.14, 1, along * along + acrossFoot * acrossFoot));

                // Interseção do raio do piso com o plano do sprite, inclinado a partir dos pés.
                const float heightScale = .94;
                float height = max(.01, (_MirrorBounds.y + _MirrorBounds.w - _MirrorFeet) * heightScale);
                float slope = .17 / height;
                float across = ray.y - ray.z * slope;
                if (abs(across) < .001) return join;
                float t = (_MirrorFeet + surface.z * slope - surface.y) / across;
                if (t <= .001 || t >= .985) return join;
                float3 hit = surface + ray * t;
                float spriteY = _MirrorFeet + hit.z / heightScale;
                float2 uv = (float2(hit.x, spriteY) - _MirrorBounds.xy) / _MirrorBounds.zw;
                if (any(uv <= 0) || any(uv >= 1)) return join;

                // Suaviza a penumbra sem perder o recorte do quadro animado.
                float2 texel = _MarcusFrame_TexelSize.xy / abs(_MirrorUV.zw);
                float2 offset = texel * lerp(.40, 1.20, uv.y);
                float2 margin = texel * .5;
                float2 low = clamp(uv - offset, margin, 1 - margin);
                float2 high = clamp(uv + offset, margin, 1 - margin);
                float center = SAMPLE_TEXTURE2D_LOD(_MarcusFrame, sampler_MarcusFrame,
                    _MirrorUV.xy + uv * _MirrorUV.zw, 0).a;
                float a = SAMPLE_TEXTURE2D_LOD(_MarcusFrame, sampler_MarcusFrame,
                    _MirrorUV.xy + low * _MirrorUV.zw, 0).a;
                float b = SAMPLE_TEXTURE2D_LOD(_MarcusFrame, sampler_MarcusFrame,
                    _MirrorUV.xy + high * _MirrorUV.zw, 0).a;
                return max(join, (center * .60 + (a + b) * .20) * .70);
            }

            float VisibilidadeDaLuz(float3 surfacePosition, float3 ray, int owner, bool handheld, bool ground, bool projectedGround)
            {
                // Segmento em XYZ entre a superfície e a fonte, em unidades do mundo.
                float visibility = 1;
                float3 signRay = step(0.0, ray) * 2.0 - 1.0;
                float3 inverse = signRay / max(abs(ray), 0.0001);
                [loop] for (int i = 0; i < _CasterCount; i++)
                {
                    if (i == owner || (handheld && i == _CasterCount - 1)) continue;
                    if (ground && i == _ArrivalPlayerCaster)
                    {
                        visibility = min(visibility, 1 - RecorteDoMarcus(surfacePosition, ray));
                        continue;
                    }
                    float4 lower = _CasterMin[i];
                    // No piso, cada poste já tem o recorte dos props estáticos.
                    if (projectedGround && lower.w != 3) continue;
                    float4 upper = _CasterMax[i];
                    if (lower.w > 4.5)
                    {
                        // Volume de sombra macia, inclusive componentes elevados como telhado e defesa.
                        float3 a = (lower.xyz - surfacePosition) * inverse;
                        float3 b = (upper.xyz - surfacePosition) * inverse;
                        float3 nearT = min(a, b);
                        float3 farT = max(a, b);
                        float enter = max(max(nearT.x, nearT.y), nearT.z);
                        float exit = min(min(farT.x, farT.y), farT.z);
                        float atravessamento = min(exit, .985) - max(enter, .015);
                        float penumbra = .035 / max(length(ray), .25);
                        visibility = min(visibility, 1 - .78 * smoothstep(0, penumbra, atravessamento));
                        continue;
                    }
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
                track.albedo.rgb *= _Bairro > .5 ? lerp(1.07, .88, ruts) * lerp(.93, 1.07, grain)
                    : lerp(1.13, .73, ruts) * lerp(.88, 1.12, grain);
                track.albedo.rgb += gravel * float3(.13, .10, .067) * (_Bairro > .5 ? .39 : 1);
                track.relief.r *= 1 - ruts * .32;
                track.surface.b = lerp(track.surface.b, .56, ruts);
                return track;
            }

            GroundSample PisoDaMataSul(float2 p, GroundSample grass)
            {
                // As manchas de terra se perdem na grama antes do passeio.
                float2 pixel = floor(p * 32) / 32;
                float lobes = sin(pixel.x * .71 + sin(pixel.y * 1.34))
                    + .46 * sin(pixel.x * 1.42 - pixel.y * 2.03);
                float roughEdge = .17 * sin(pixel.x * 12.7 + pixel.y * 7.1)
                    + .12 * sin(pixel.y * 18.3 - pixel.x * 9.7);
                float verge = 1 - smoothstep(4.72, 5.82, pixel.y + roughEdge);
                float soilPatch = smoothstep(.18, 1.05, lobes + roughEdge) * verge;
                GroundSample soil = AmostraTrilha(p, 2);
                soil.albedo.rgb *= float3(.68, .64, .54);
                grass.albedo.rgb *= 1 - .17 * verge * smoothstep(-.9, .66, lobes);
                return BlendGround(grass, soil, soilPatch * .75);
            }

            GroundSample TrilhaDaEncruzilhada(float2 p, GroundSample grass, out float dirt)
            {
                float distanceUp = p.y - 14;
                float center = 20.5 + .48 * sin(distanceUp * .48) - .075 * distanceUp;
                float side = p.x - center;
                float width = lerp(2.55, 1.70, smoothstep(0, 3.8, distanceUp));
                float edge = .14 * sin(p.y * 6.3 + side * 7.2) + .08 * sin(p.y * 15.7 - side * 9.6);
                dirt = (1 - smoothstep(width - .43, width + .22, abs(side) + edge))
                    * smoothstep(12.52, 14.25, p.y);
                return BlendGround(grass, AmostraTrilha(p, side), dirt);
            }

            float3 PinturaDaTravessia(float3 albedo, float2 xy, float asphalt, float relief)
            {
                // A faixa e os dois rebaixos ficam no eixo da porta do mercadinho.
                float2 pixel = (floor(xy * 32) + .5) / 32;
                float fromCenter = abs(pixel.x - 11);
                float bar = step(abs(frac((pixel.x - 11) / .72 + .5) - .5), .27);
                float ends = smoothstep(7.74, 7.82, pixel.y) * (1 - smoothstep(12.20, 12.28, pixel.y));
                float salt = frac(sin(dot(floor(pixel * 21), float2(79.1, 41.7))) * 15843.72);
                float wear = lerp(.68, 1, smoothstep(.07, .35, relief)) * lerp(.58, .83, smoothstep(.12, .45, salt));
                float paint = bar * step(fromCenter, 1.65) * ends * asphalt * wear;
                albedo = lerp(albedo, float3(.55, .53, .46), paint);

                float lip = min(abs(pixel.y - 7.36), abs(pixel.y - 12.65));
                float ramp = (1 - smoothstep(.83, .94, fromCenter)) * (1 - smoothstep(.19, .30, lip));
                float joint = smoothstep(.11, .15, lip) * (1 - smoothstep(.19, .21, lip));
                float grooves = step(.68, frac(pixel.x * 8)) * step(.62, frac(pixel.y * 9));
                return lerp(albedo, albedo * float3(1.14, 1.10, 1.00) - joint * (.014 + grooves * .014),
                    ramp * (1 - asphalt));
            }

            GroundSample TerraDoBloqueio(float2 p, GroundSample ground)
            {
                float2 pixel = floor(p * 32) / 32;
                float axis = lerp(34.70, 36.76, saturate((pixel.y - 6.6) / 6.8));
                float across = abs(pixel.x - axis);
                float lengthMask = smoothstep(5.80, 7.05, p.y) * (1 - smoothstep(13.6, 15.40, p.y));
                float scatter = .28 * sin(pixel.y * 4.1) + .14 * sin(pixel.y * 14.0 + pixel.x * 8.6);
                float mud = (1 - smoothstep(.25, 1.07, across + scatter)) * lengthMask;
                GroundSample earth = AmostraTrilha(p, 2);
                earth.albedo.rgb *= float3(.61, .54, .48);
                earth.surface.b = lerp(earth.surface.b, .53, .6);
                return BlendGround(ground, earth, mud * .78);
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

            GroundSample ChegadaDoBairro(float2 p, GroundSample ground, out float dirt)
            {
                float side = p.y - (10.0 + .15 * sin(p.x * .8));
                float edge = .10 * sin(p.x * 7.4 + p.y * 11.3);
                dirt = (1 - smoothstep(1.53, 2.16, abs(side) + edge))
                    * (1 - smoothstep(4.45, 5.93, p.x));
                GroundSample track = AmostraTrilha(p, side);
                track.albedo.rgb *= float3(.76, .75, .70);
                return BlendGround(ground, track, dirt);
            }

            GroundSample CurvaDoCentro(float2 p, GroundSample ground, out float dirt)
            {
                float pavement = 0;
                float side = 0;
                CaminhoDeBarro(p, float2(29.7, 10.05), float2(32.12, 10.8), 1.85, pavement, side);
                CaminhoDeBarro(p, float2(32.12, 10.8), float2(33.0, 13.85), 1.78, pavement, side);
                float2 pixel = (float2(fmod(floor(p.x) + floor(p.y), 4),
                    _MainTex_TexelSize.w / GroundPixelsPerUnit - 2) + frac(p)) * GroundPixelsPerUnit;
                GroundSample asphalt = SampleGround((floor(pixel) + .5) * _MainTex_TexelSize.xy, float2(1, 1));
                ground = BlendGround(ground, asphalt, pavement * (1 - smoothstep(13.85, 14.30, p.y)));

                // A abertura entre os pilares é o eixo dos dois sulcos de pneu.
                dirt = 0;
                side = 0;
                CaminhoDeBarro(p, float2(32.96, 12.52), float2(33.0, 15.52), 1.61, dirt, side);
                CaminhoDeBarro(p, float2(33.0, 15.52), float2(33.70, 22.8), 1.67, dirt, side);
                dirt *= smoothstep(12.65, 14.85, p.y);
                GroundSample track = AmostraTrilha(p, side);
                track.albedo.rgb *= float3(.76, .77, .74);
                return BlendGround(ground, track, dirt);
            }

            GroundSample TrilhaDoBairro(float2 p, GroundSample ground, out float dirt)
            {
                dirt = 0;
                float side = 10;
                [loop] for (int i = 0; i < 15; i++)
                {
                    float4 trecho = _BairroTrechos[i];
                    CaminhoDeBarro(p, trecho.xy, trecho.zw, _BairroMargens[i].x, dirt, side);
                }
                if (dirt > 0)
                {
                    GroundSample track = AmostraTrilha(p, side);
                    float2 pixel = floor(p * 32) / 32;
                    float sediment = sin(pixel.x * 1.73 + sin(pixel.y * 1.16) * 1.7)
                        + .55 * sin(pixel.y * 2.14 - pixel.x * 1.32);
                    float runoff = smoothstep(.20, 1.24, sediment) * smoothstep(.52, 1.5, abs(side));
                    track.albedo.rgb *= lerp(float3(1.08, 1.03, .95), float3(.90, .92, .94), runoff);
                    track.surface.b = lerp(track.surface.b, .46, runoff * .72);
                    track.relief.r -= runoff * .071;
                    return BlendGround(ground, track, dirt);
                }
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
                    if (i == _ArrivalPlayerCaster)
                    {
                        float2 feet = (_CasterMin[i].xy + _CasterMax[i].xy) * .5;
                        float2 delta = (xy - feet) / float2(.30, .18);
                        float ellipse = saturate(1 - dot(delta, delta));
                        contact = max(contact, .38 * ellipse * ellipse);
                        continue;
                    }
                    // Contato elíptico para objetos pequenos e veículos.
                    float mode = _CasterMin[i].w;
                    if (mode > 1.5 && mode < 2.5) continue;
                    if (mode > 5.5) continue;
                    if (mode > 4.5 && _CasterMin[i].z > .12) continue;
                    if (mode > .5 && mode < 4.5)
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
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 sprite = albedo * input.color;
                clip(sprite.a - .001);
                // A caixa da baliza tem vidro emissivo e não projeta recorte próprio.
                if (_Receiver.w > 1.5) return half4(sprite.rgb * .72, sprite.a);

                float2 position = input.world - _Origin.xy;
                float2 xy = (floor(position * GroundPixelsPerUnit) + .5) / GroundPixelsPerUnit;
                float baseY = _Receiver.x - _Origin.y;
                bool upright = _Receiver.y > 0;
                // A base da copa representa a profundidade no cenário 2D.
                float focusY = lerp(12.5, _ViewFocus.y, saturate(_ViewFocus.z));
                float softness = smoothstep(5.5, 12.0, baseY - focusY) * .30;
                [branch] if (_Entrance > .5 && upright && _Wind.w > .5 && softness > .001)
                {
                    float2 texel = _MainTex_TexelSize.xy * 1.35;
                    half4 left = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(texel.x, 0));
                    half4 right = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(texel.x, 0));
                    half4 down = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(0, texel.y));
                    half4 up = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, texel.y));
                    half3 average = (albedo.rgb * albedo.a * 2 + left.rgb * left.a + right.rgb * right.a
                        + down.rgb * down.a + up.rgb * up.a) / max(2 * albedo.a + left.a + right.a + down.a + up.a, .001);
                    sprite.rgb = lerp(sprite.rgb, average * input.color.rgb, softness);
                }
                float height = upright ? max(0, (xy.y - baseY) * _Receiver.y) : 0;
                float3 surfacePosition = float3(xy.x, upright ? baseY : xy.y, height + .02);
                int owner = (int)_Receiver.z - 1;
                float proxyHeight = owner >= 0 && owner < _CasterCount ? _CasterMax[clamp(owner, 0, 47)].z : 1.5;
                float top = smoothstep(proxyHeight * .30, proxyHeight * .95, height);
                float3 normal = upright
                    ? normalize(lerp(float3(0, -.79, .61), float3(0, 0, 1), top * .85))
                    : float3(0, 0, 1);
                bool propSurface = _PropSurfaceEnabled > .5;
                bool generatedSurface = abs(_Receiver.y) > .001;
                float propRoughness = 1;
                float propMetallic = 0;
                float foliage = 0;
                [branch] if (propSurface)
                {
                    float4 material = SAMPLE_TEXTURE2D(_PropSurface, sampler_PropSurface, input.uv);
                    float2 slope = material.rg * 2 - 1;
                    slope.x *= sign(ddx(input.uv.x) * ddx(position.x));
                    float3 front = float3(0, -.92, .39);
                    normal = upright ? normalize(lerp(front, float3(0, 0, 1), material.b)) : float3(0, 0, 1);
                    float3 side = float3(1, 0, 0);
                    normal = normalize(normal + side * slope.x + cross(normal, side) * slope.y);
                    propRoughness = material.a;
                    foliage = _Wind.w * smoothstep(.25, .65, material.b);
                    if (_Wind.w > .5)
                    {
                        sprite.rgb *= lerp(_CanopyTone.www, _CanopyTone.rgb, foliage);
                        surfacePosition.y += normal.y * foliage * .36;
                    }
                }
                [branch] if (generatedSurface)
                {
                    SpriteSurface material = ReadSpriteSurface(input.uv, _MainTex_TexelSize.xy, albedo);
                    float2 axis = sign(float2(ddx(input.uv.x) * ddx(position.x),
                                              ddy(input.uv.y) * ddy(position.y)));
                    float3 tangent = float3(1, 0, 0);
                    float3 bitangent = cross(normal, tangent);
                    normal = normalize(normal + (material.slope.x * axis.x * tangent
                        + material.slope.y * axis.y * bitangent) * (propSurface ? .32 : 1));
                    propRoughness = propSurface ? lerp(propRoughness, material.roughness, .34) : material.roughness;
                    propMetallic = material.metallic;
                }
                bool groundPbr = !upright && _Receiver.y >= 0 && _GroundSurfaceEnabled > .5;
                float asphalt = 0;
                float water = 0;
                float dirt = 0;
                float2 waterLook = 0;
                float4 ground = 0;
                float4 detail = 0;
                bool groundRelief = groundPbr && _GroundReliefStrength > 0;
                float2 axis = sign(float2(ddx(input.uv.x) * ddx(position.x), ddy(input.uv.y) * ddy(position.y)));
                float seam = 0;
                [branch] if (groundPbr)
                {
                    GroundSample terrain = JoinGround(position, input.uv, axis, seam);
                    if ((_Arrival > .5 || _BarStreet > .5) && xy.y < 5.82)
                        terrain = PisoDaMataSul(xy, terrain);
                    if (_Crosswalk > .5 && xy.x > 33.3 && xy.y > 5.8 && xy.y < 15.4)
                        terrain = TerraDoBloqueio(xy, terrain);
                    if (_Crosswalk > .5 && xy.y >= 12.52 && xy.x >= 16 && xy.x < 25)
                        terrain = TrilhaDaEncruzilhada(xy, terrain, dirt);
                    if (_Bairro > .5)
                        terrain = TrilhaDoBairro(xy, terrain, dirt);
                    if (_BarStreet > .5 && xy.x < 5.94 && xy.y > 7.55 && xy.y < 12.45)
                        terrain = ChegadaDoBairro(xy, terrain, dirt);
                    if (_BarStreet > .5 && xy.x > 28.4 && xy.y > 7.5)
                        terrain = CurvaDoCentro(xy, terrain, dirt);
                    float2 roadUV = RoadUnderPaint(input.uv);
                    float pixelY = input.uv.y * _MainTex_TexelSize.w;
                    float3 paint = 0;
                    if (roadUV.y != input.uv.y && fmod(pixelY, GroundPixelsPerUnit) < 4)
                        paint = sprite.rgb - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, roadUV).rgb * input.color.rgb;
                    sprite = terrain.albedo * input.color;
                    sprite.rgb = saturate(sprite.rgb + paint * terrain.asphalt);
                    ground = terrain.surface;
                    detail = terrain.relief;
                    asphalt = terrain.asphalt;
                    if (_Crosswalk > .5 && xy.x > 9.2 && xy.x < 12.8 && xy.y > 7 && xy.y < 13)
                        sprite.rgb = PinturaDaTravessia(sprite.rgb, xy, asphalt, detail.r);
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
                float receiverY = upright ? baseY : xy.y;
                // No Bairro a rua sobe para dentro da mata. A penumbra acompanha essa subida.
                float nightY = receiverY;
                if (_Bairro > .5)
                    nightY -= max(0, _ViewFocus.y - 13.7);
                float depth = smoothstep(13.7, 20.8, nightY);
                half3 light = lerp(half3(.61, .70, .77), half3(.26, .36, .43), depth);
                float2 fromMarcus = (xy - _ViewFocus.xy) * float2(.75, 1);
                float falloff = smoothstep(2.3, 7.0, length(fromMarcus));
                light *= 1 - .32 * falloff * _ViewFocus.z;
                // Perfil noturno da Rodovia, compartilhado por piso, cenário e personagens.
                float forestDepth = smoothstep(14.3, 18.7, nightY);
                light *= half3(.7743, .7308, .7614) * lerp(1, .43, forestDepth);
                if (_Wind.w > .5)
                {
                    // A luz difusa do céu dá contorno às copas para lá dos postes.
                    float distantCanopy = smoothstep(17.0, 23.4, baseY) * (.48 + .52 * foliage);
                    light += half3(.054, .077, .078) * distantCanopy * (1 - .28 * falloff * _ViewFocus.z);
                }
                // A mata densa recebe menos céu. Os postes ainda iluminam as folhas diretamente.
                if (_Bairro > .5 && _Wind.w > .5) light *= lerp(.55, .66, foliage);
                // O ar úmido atenua a luz ambiente no fundo, antes das luzes locais.
                float hazeStart = max(15.8, _ViewFocus.y + 3.5);
                float haze = smoothstep(hazeStart, hazeStart + 8, upright ? baseY : xy.y) * _WindStrength;
                half luma = dot(sprite.rgb, half3(.2126, .7152, .0722));
                sprite.rgb = lerp(sprite.rgb, luma.xxx, haze * .24);
                light *= lerp(half3(1, 1, 1), half3(.67, .77, .86), haze);
                light += _Lightning * half3(.66, .80, 1.00);
                if (groundPbr)
                {
                    // Luz do céu rasante, visível nos relevos mesmo entre dois postes.
                    const float3 sky = float3(-.56, -.51, .652);
                    float relief = clamp(1 + (dot(ambientNormal, sky) - sky.z) * .39, .78, 1.21);
                    float moisture = sin(xy.x * 1.62 + xy.y * .69) * sin(xy.y * 1.41 - xy.x * .54);
                    light *= lerp(1, ground.a, .62) * relief * terrainOcclusion * (1 + .07 * moisture);
                }
                if (propSurface || generatedSurface)
                    light *= .84 + .24 * saturate(dot(normal, float3(-.41, -.52, .75)));
                half3 reflection = 0;
                float practicalLight = 0;
                bool useArrivalShadows = !upright && _ArrivalShadowEnabled > .5;
                float3 arrivalOcclusion = 0;
                [branch] if (useArrivalShadows)
                    arrivalOcclusion = SAMPLE_TEXTURE2D(_ArrivalShadows, sampler_ArrivalShadows,
                        xy / _ArrivalShadowSize).rgb;
                bool usePlayerShadows = !upright && _PlayerShadowEnabled > .5;
                float playerOcclusion = 0;
                [branch] if (usePlayerShadows)
                    playerOcclusion = SAMPLE_TEXTURE2D(_PlayerShadows, sampler_PlayerShadows,
                        xy / _ArrivalShadowSize).r;
                int lampCount = _Bairro > .5 ? 15 : (_Crosswalk > .5 ? 11 : 7);
                [loop] for (int l = 0; l < lampCount; l++)
                {
                    bool handheld = l == 3;
                    bool accent = l == 6;
                    bool marker = l >= 7 && l < 11;
                    float4 lamp = l >= 11 ? _BairroPostes[clamp(l - 11, 0, 3)] :
                        marker ? _MarkerLamps[clamp(l - 7, 0, 3)] :
                        accent ? _AccentLamp : handheld ? _PlayerLamp :
                        l == 4 ? _FourthLamp : l == 5 ? _FifthLamp : _Lamps[min(l, 2)];
                    if (lamp.w <= 0) continue;
                    float3 ray = lamp.xyz - surfacePosition;
                    float distanceToLamp = length(ray);
                    float fade = marker
                        ? 1 - smoothstep(.12, lamp.w, length(ray.xy))
                        : saturate(1.0 - distanceToLamp / lamp.w);
                    float handheldHotspot = 0;
                    if (handheld)
                    {
                        float dist2D = length(ray.xy);
                        float2 dirFromLamp = -ray.xy + _PlayerLampDirection * .18;
                        float cone = dot(dirFromLamp, _PlayerLampDirection) / max(length(dirFromLamp), .01);
                        float outerSpill = smoothstep(.48, .83, cone) * (1.0 - smoothstep(0.0, lamp.w * .78, dist2D));
                        float mainBeam = smoothstep(.69, .93, cone);
                        handheldHotspot = smoothstep(.89, .99, cone) * (1.0 - smoothstep(0.3, lamp.w * .85, dist2D));
                        fade *= saturate(outerSpill * .16 + mainBeam * .44 + handheldHotspot * .24);
                    }
                    float3 toLight = ray / max(distanceToLamp, .01);
                    // Os postes da avenida têm a fonte a 3,18 m; as fachadas ficam abaixo de 3 m.
                    bool streetLamp = !handheld && !accent && !marker && lamp.z > 3;
                    if (streetLamp)
                    {
                        const float3 beamAxis = float3(0, .60, .80);
                        float beam = smoothstep(.40, .83, dot(toLight, beamAxis));
                        fade *= lerp(.20, 1.34, beam);
                    }
                    if (fade <= 0) continue;
                    float facing = saturate(dot(normal, toLight) * .87 + .13);
                    bool projectedGround = (useArrivalShadows && l < 3) || (usePlayerShadows && handheld);
                    float shadow = VisibilidadeDaLuz(surfacePosition, ray, owner, handheld, !upright, projectedGround);
                    if (useArrivalShadows && l < 3)
                        shadow = min(shadow, 1 - .91 * arrivalOcclusion[min(l, 2)]);
                    else if (usePlayerShadows && handheld)
                        shadow = min(shadow, 1 - .91 * playerOcclusion);
                    half3 handheldColor = lerp(half3(1.05, 1.10, 1.18), half3(1.36, 1.42, 1.50), handheldHotspot);
                    half3 lampColor = marker ? half3(.69, .46, .23) : handheld ? handheldColor : (accent ? _AccentColor.rgb : _LampColors[min(l, 2)].rgb);
                    if (handheld) lampColor *= 1.65;
                    if (streetLamp) lampColor *= half3(1.05, .91, .69);
                    if ((_Entrance > .5 || streetLamp) && !handheld && xy.y > lamp.y)
                        shadow *= lerp(.79, 1, shadow * shadow);
                    float lampFalloff = lerp(fade * fade, fade * sqrt(fade), foliage * .62);
                    half3 incident = lampColor * (lampFalloff * shadow * (groundPbr ? 1.22 : 1));
                    float exposure = 1;
                    [branch] if (groundRelief && shadow > 0)
                    {
                        float3 lightUV = float3(axis * toLight.xy, toLight.z);
                        float seamExposure = _Entrance > .5 ? 1 : 1 - 2 * seam;
                        exposure = lerp(1, HeightExposure(reliefPixel, tileMin, xy, terrainHeight, lightUV, toLight.xy),
                            .23 * _GroundReliefStrength * seamExposure);
                    }
                    light += incident * facing * exposure;
                    // Folhas finas deixam passar um pouco da luz rasante; madeira só recebe luz na superfície.
                    float backlight = saturate(dot(-normal, toLight)) * .36 + (1 - abs(toLight.z)) * .49;
                    light += incident * half3(1.00, .88, .49) * (foliage * (.12 + backlight) * .61);
                    practicalLight += incident.r * facing;
                    if (generatedSurface)
                    {
                        float3 f0 = lerp(.04, sprite.rgb, propMetallic);
                        reflection += incident * SpriteSpecular(normal, toLight, GroundView, propRoughness, f0)
                            * lerp(.74, .23, propRoughness);
                    }
                    else if (propSurface)
                    {
                        float specular = GroundSpecular(normal, toLight, propRoughness);
                        reflection += incident * specular * lerp(.70, .16, propRoughness);
                    }
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
                        if (streetLamp || (_Arrival > .5 && !handheld && !accent && !marker))
                        {
                            // A lâmina d'água devolve a lâmpada numa coluna de reflexos curtos.
                            float distanceDown = lamp.y + .62 - xy.y;
                            float2 ripplePixel = floor(xy * float2(32, 34));
                            float ripple = sin(ripplePixel.y * 1.29 + sin(ripplePixel.y * .24) * 2.4 - _Time.y * 3.9);
                            float shimmer = frac(sin(dot(floor(xy * float2(23, 43)), float2(73.1, 31.7))) * 32719.3);
                            float beamWidth = .19 + .17 * saturate(distanceDown * .44) + .18 * water;
                            float drift = .07 * ripple + .06 * sin(xy.y * 5.3);
                            float streak = 1 - smoothstep(.045, beamWidth, abs(xy.x - lamp.x + drift));
                            float reach = smoothstep(-.35, .31, distanceDown) * (1 - smoothstep(2.6, 5.2, distanceDown));
                            float glints = smoothstep(.32, 1, shimmer) * (.70 + .30 * ripple);
                            float surface = _Bairro > .5 ? (.19 + .56 * water) * saturate(dirt + water)
                                : lerp(.44, .96, saturate(asphalt + water)) * (1 - smoothstep(12.95, 14.1, xy.y));
                            reflection += lampColor * streak * reach * glints * surface * (.052 + .085 * water) * lerp(.78, 1, shadow);
                        }
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
                [branch] if (_Arrival > .5 && groundPbr)
                {
                    float ripple = sin(xy.y * 53 - _Time.y * 5.4) * .041;
                    float fromCar = _ArrivalCarFeet - xy.y;
                    if (_ArrivalCarBounds.z > 0 && fromCar > 0 && fromCar < 1.25 && asphalt > .2)
                    {
                        float2 mirrored = float2(xy.x + ripple, _ArrivalCarFeet + fromCar * 1.64);
                        float2 uvCar = (mirrored - _ArrivalCarBounds.xy) / _ArrivalCarBounds.zw;
                        if (all(uvCar > 0) && all(uvCar < 1))
                        {
                            half4 car = SAMPLE_TEXTURE2D(_ArrivalCar, sampler_ArrivalCar,
                                _ArrivalCarUV.xy + uvCar * _ArrivalCarUV.zw);
                            float broken = saturate(.55 + .43 * sin(xy.y * 91 - _Time.y * 3));
                            float opacity = car.a * (1 - smoothstep(.27, 1.25, fromCar)) * (.11 + .29 * water) * broken;
                            reflection += car.rgb * opacity * half3(.47, .34, .31);
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
                float far = smoothstep(14.2, 18.5, nightY);
                float nightGrade = (1 - .20 * far - .47 * asphalt)
                    * (1 - .13 * (1 - saturate(practicalLight)));
                nightGrade = lerp(nightGrade, 1, _Lightning * .85);
                if (_WetRoad > .5)
                {
                    reflection *= 1 - .18 * asphalt * (1 - water);
                    // O brilho frio destaca a água sem clarear o asfalto que a circunda.
                    float ripple = .5 + .5 * sin(xy.y * 47 + sin(xy.x * 12) * 1.1 - _Time.y * 2.5);
                    half3 waterColor = lerp(half3(.53, .55, .56), half3(.24, .48, .73), waterLook.x);
                    reflection += waterColor * water * waterLook.y *
                        (.012 + .012 * ripple + .023 * saturate(practicalLight));
                }
                half3 color = (sprite.rgb * light * nightGrade + reflection) * contact;
                [branch] if (_Entrance > .5 && groundPbr && _WindStrength > 0 && practicalLight > .10)
                {
                    // A umidade se acumula perto das sarjetas e só ganha corpo sob luz direta.
                    float gutter = min(abs(xy.y - 12.48), abs(xy.y - 7.45));
                    float lowMist = 1 - smoothstep(.06, .67, gutter);
                    float2 p = xy + _Origin.xy;
                    float drift = p.x * 1.35 - _Time.y * .48;
                    float filament = sin(drift + sin(p.y * 9.2 + drift * .34) * .84);
                    float patch = sin(p.x * .73 + p.y * 3.7 - _Time.y * .21);
                    float mist = lowMist * smoothstep(.15, .95, filament * .68 + patch * .32);
                    mist *= smoothstep(.10, .65, practicalLight) * .17 * _WindStrength;
                    color = lerp(color, half3(.46, .34, .23), mist);
                }
                return half4(color * vignette, sprite.a);
            }
            ENDHLSL
        }
    }
}
