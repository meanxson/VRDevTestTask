// Маркер зоны "встаньте сюда": мягкая заливка, бегущие наружу кольца и яркая кромка.
//
// Плоскому напольному диску обводка по контуру не подходит — её не видно сверху,
// а именно сверху игрок на зону и смотрит. Движение решает это лучше статичной
// линии: глаз цепляется за анимацию даже боковым зрением.
//
// Радиус считается от положения вершины в объектном пространстве, а не из UV:
// у цилиндра развёртка неудобная, а координаты крышки дают ровный радиус
// независимо от того, каким мешем нарисована зона.
Shader "App/ZoneMarker"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.15, 0.55, 0.75, 1)
        _RingColor ("Ring Color", Color) = (1, 0.85, 0.35, 1)
        _FillAlpha ("Fill Alpha", Range(0, 1)) = 0.22
        _RingCount ("Ring Count", Range(1, 6)) = 2
        _RingWidth ("Ring Width", Range(0.02, 0.6)) = 0.25
        _RingSpeed ("Ring Speed", Range(0, 3)) = 0.5
        _EdgeWidth ("Edge Width", Range(0.01, 0.4)) = 0.12

        // Яркость маркера: гасится, когда шаг не активен.
        _Intensity ("Intensity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "ZoneMarker"

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RingColor;
                float _FillAlpha;
                float _RingCount;
                float _RingWidth;
                float _RingSpeed;
                float _EdgeWidth;
                float _Intensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 planeOS : TEXCOORD0;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                // Примитивы Unity имеют радиус 0.5 в объектном пространстве.
                output.planeOS = input.positionOS.xz;

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                // 0 в центре зоны, 1 на её кромке.
                float radius = saturate(length(input.planeOS) * 2.0);

                // Заливка гуще к центру: подсказывает, куда именно вставать.
                float fill = _FillAlpha * (1.0 - smoothstep(0.0, 1.0, radius));

                // Кольца бегут от центра к краю и затухают у кромки,
                // чтобы не сливаться с ней в мигающую полосу.
                float wave = frac(radius * _RingCount - _Time.y * _RingSpeed);
                float ring = smoothstep(1.0 - _RingWidth, 1.0, wave);
                ring *= 1.0 - smoothstep(0.75, 1.0, radius);

                // Кромка держит границу зоны читаемой, даже когда кольцо далеко.
                float edge = smoothstep(1.0 - _EdgeWidth, 1.0, radius);

                float highlight = saturate(ring * 0.75 + edge);
                float alpha = saturate(fill + highlight) * _Intensity;

                half3 color = lerp(_BaseColor.rgb, _RingColor.rgb, highlight);

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
