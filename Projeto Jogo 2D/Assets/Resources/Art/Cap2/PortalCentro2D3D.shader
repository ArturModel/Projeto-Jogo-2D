Shader "SantaRita/Cap2/PortalCentro3D"
{
    Properties
    {
        _PortalTex ("Vista 3D do Interior", 2D) = "black" {}
        _PixelAmount ("Quantizacao Pixel Art", Range(0, 1)) = 0.85
        _PixelGrid ("Resolucao Base da Grade", Vector) = (320, 180, 0, 0)
        _ArchTop ("Arco Superior", Range(0, 0.5)) = 0
        _WarmRim ("Brilho Quente da Soleira", Color) = (1, 0.78, 0.45, 1)
        _Alpha ("Opacidade", Range(0, 1)) = 1
        _FrameShadow ("Sombra do Batente", Range(0, 1)) = 0.55
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_PortalTex);
            SAMPLER(sampler_PortalTex);

            float _PixelAmount;
            float4 _PixelGrid;
            float _ArchTop;
            half4 _WarmRim;
            float _Alpha;
            float _FrameShadow;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(worldPos);
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float mask = 1.0;

                // Recorte opcional em arco para a porta da Igreja Matriz
                if (_ArchTop > 0.001)
                {
                    float archStart = 1.0 - _ArchTop;
                    if (uv.y > archStart)
                    {
                        float2 d = float2((uv.x - 0.5) * 2.0, (uv.y - archStart) / _ArchTop);
                        float r = dot(d, d);
                        mask = 1.0 - smoothstep(0.94, 1.03, r);
                    }
                }

                float2 screenUV = input.screenPos.xy / max(0.0001, input.screenPos.w);
                #if UNITY_UV_STARTS_AT_TOP
                // Em RenderTexture capturada de câmera, alinhamos verticalmente com ComputeScreenPos
                #endif

                float2 grid = max(float2(64.0, 36.0), _PixelGrid.xy);
                float2 snappedUV = (floor(screenUV * grid) + 0.5) / grid;
                float2 sampleUV = lerp(screenUV, snappedUV, saturate(_PixelAmount));

                half3 roomCol = SAMPLE_TEXTURE2D(_PortalTex, sampler_PortalTex, saturate(sampleUV)).rgb;

                // Sombra interna dos batentes (lateral e verga superior) diminuindo conforme o mergulho avança
                float sideDist = min(uv.x, 1.0 - uv.x);
                float topDist = 1.0 - uv.y;
                float sideShade = smoothstep(0.0, 0.16, sideDist);
                float topShade = smoothstep(0.0, 0.18, topDist);
                float frameOcclusion = lerp(1.0, sideShade * topShade, _FrameShadow);
                roomCol *= lerp(0.42, 1.0, frameOcclusion);

                // Leve brilho âmbar na soleira inferior para integrar a luz do interior
                float sillGlow = (1.0 - smoothstep(0.0, 0.22, uv.y)) * (0.28 * _FrameShadow);
                roomCol += _WarmRim.rgb * sillGlow;

                return half4(roomCol, saturate(_Alpha * mask));
            }
            ENDHLSL
        }
    }
}
