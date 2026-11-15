Shader "Custom/Lava"
{
    Properties
    {
        _MainTex ("Sprite (needed by the Sprite Renderer)", 2D) = "white" {}
        _ColorDark ("Dark Color", Color) = (0.55, 0.02, 0, 1)
        _ColorBright ("Bright Color", Color) = (1, 0.55, 0, 1)
        _Speed ("Speed", Float) = 1
        _Scale ("Pattern Size", Float) = 2
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _ColorDark;
            float4 _ColorBright;
            float _Speed;
            float _Scale;

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz).xy;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 p = input.worldPos / _Scale;
                float t = _Time.y * _Speed;

                float wave = sin(p.x * 3 + t) + sin(p.y * 4 - t * 1.3) + sin((p.x + p.y) * 2 + t * 0.7);

                float blend = wave / 6 + 0.5;
                return lerp(_ColorDark, _ColorBright, blend);
            }
            ENDHLSL
        }
    }
}
