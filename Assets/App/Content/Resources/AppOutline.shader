// Обводка методом вывернутой оболочки: объект рисуется второй раз, увеличенным
// вдоль нормалей и с отсечением лицевых граней, поэтому наружу выходит только
// контур. Готовые ассеты подсветки заданием запрещены, а это решение целиком
// на средствах URP и не тянет за собой зависимостей.
Shader "App/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 0.75, 0.1, 1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.02
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
            Name "Outline"

            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;

                // Раздвигаем вершины в МИРОВОМ пространстве, а не в объектном:
                // иначе толщина контура умножается на масштаб объекта и у сплющенных
                // или сильно вытянутых мешей получается разной по осям — вплоть
                // до полностью невидимой.
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 centerWS = TransformObjectToWorld(float3(0, 0, 0));

                // Направление берём от центра объекта, а не из нормали вершины.
                // У примитивов Unity нормали жёсткие: на кубе их шесть, по одной
                // на грань, и раздвигание вдоль них разносит грани в стороны —
                // оболочка рвётся по рёбрам, контур виден только местами.
                // Радиальное направление для выпуклого меша всегда даёт замкнутую
                // оболочку и не требует сглаженных нормалей в самом меше.
                float3 outward = positionWS - centerWS;
                float lengthSq = dot(outward, outward);
                float3 direction = lengthSq > 1e-8
                    ? outward * rsqrt(lengthSq)
                    : normalize(TransformObjectToWorldNormal(input.normalOS));

                output.positionCS = TransformWorldToHClip(positionWS + direction * _OutlineWidth);

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
