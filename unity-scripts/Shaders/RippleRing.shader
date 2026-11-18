// 足元の波紋：広がるリングをフェードアウトさせる（URP、Unlit 透明）。
// _Progress（0→1）を FootstepRipple が進める。
Shader "NONO/RippleRing"
{
    Properties
    {
        _Color ("Color", Color) = (0.9, 0.97, 1, 1)
        _Progress ("Progress", Range(0, 1)) = 0
        _Width ("Ring Width", Range(0.02, 0.4)) = 0.12
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            float4 _Color;
            float _Progress, _Width;
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };
            Varyings vert(Attributes IN) { Varyings OUT; OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz); OUT.uv = IN.uv; return OUT; }
            half4 frag(Varyings IN) : SV_Target
            {
                float d = length(IN.uv - 0.5) * 2.0; // 0=中心, 1=端
                float outer = 1.0 - smoothstep(_Progress - _Width, _Progress, d);
                float inner = smoothstep(_Progress - _Width * 2.0, _Progress - _Width, d);
                float ring = outer * inner;
                float fade = 1.0 - _Progress;
                return half4(_Color.rgb, ring * fade * 0.8);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
