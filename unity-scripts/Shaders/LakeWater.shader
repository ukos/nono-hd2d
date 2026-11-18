// 試作用の軽量な湖面シェーダー（URP）。
// リアルタイム反射は使わない（WebGL 負荷対策＝試作仕様 §3）。
// ゆらぎ（sin 波の疑似法線）＋空色へのフレネル＋太陽のグリントで「らしく」見せる。
Shader "NONO/LakeWater"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (0.05, 0.16, 0.22, 1)
        _SkyColor ("Sky / Reflection Color", Color) = (0.65, 0.78, 0.85, 1)
        _SunColor ("Sun Glint Color", Color) = (1, 0.75, 0.45, 1)
        _Opacity ("Opacity", Range(0, 1)) = 0.88
        _WaveSpeed ("Wave Speed", Float) = 0.6
        _WaveScale ("Wave Scale", Float) = 2.2
        _GlintSharpness ("Glint Sharpness", Range(8, 256)) = 90
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float4 _DeepColor, _SkyColor, _SunColor;
            float _Opacity, _WaveSpeed, _WaveScale, _GlintSharpness;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float3 positionWS : TEXCOORD0; float2 uv : TEXCOORD1; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // 2方向の sin 波を重ねた疑似法線
            float3 WaveNormal(float2 p, float t)
            {
                float e = 0.35;
                float h  = sin(p.x * 1.7 + t * 1.3) * 0.5 + sin(p.y * 2.3 - t * 1.7) * 0.35 + sin((p.x + p.y) * 3.1 + t * 0.8) * 0.25;
                float hx = sin((p.x + e) * 1.7 + t * 1.3) * 0.5 + sin(p.y * 2.3 - t * 1.7) * 0.35 + sin((p.x + e + p.y) * 3.1 + t * 0.8) * 0.25;
                float hz = sin(p.x * 1.7 + t * 1.3) * 0.5 + sin((p.y + e) * 2.3 - t * 1.7) * 0.35 + sin((p.x + p.y + e) * 3.1 + t * 0.8) * 0.25;
                return normalize(float3((h - hx) * 0.6, 1.0, (h - hz) * 0.6));
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _WaveSpeed;
                float3 n = WaveNormal(IN.positionWS.xz * _WaveScale, t);
                float3 viewDir = normalize(GetWorldSpaceViewDir(IN.positionWS));
                float fresnel = pow(1.0 - saturate(dot(viewDir, float3(0, 1, 0))), 2.0);

                float3 col = lerp(_DeepColor.rgb, _SkyColor.rgb, fresnel * 0.85);

                // 太陽のグリント（ゆらぎで散らす）
                Light mainLight = GetMainLight();
                float3 halfDir = normalize(viewDir + mainLight.direction);
                float spec = pow(saturate(dot(n, halfDir)), _GlintSharpness);
                col += _SunColor.rgb * spec * 1.2;

                // ゆらぎの明暗を少しだけ足す
                col += n.x * 0.03 + n.z * 0.02;

                return half4(col, _Opacity);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
