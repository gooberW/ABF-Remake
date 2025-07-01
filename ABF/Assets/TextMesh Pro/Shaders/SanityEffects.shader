Shader "Hidden/SanityEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0
        _VignetteRoundness ("Vignette Roundness", Range(1, 10)) = 2
        _Desaturation ("Desaturation", Range(0, 1)) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _VignetteIntensity;
            float _VignetteRoundness;
            float _Desaturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Rounded vignette calculation
                float2 uvCenter = i.uv - 0.5;
                float vignette = dot(uvCenter, uvCenter); // Distance from center
                vignette = pow(vignette, _VignetteRoundness); // Apply roundness
                vignette = 1.0 - vignette * _VignetteIntensity * 4.0; // Scale intensity
                vignette = saturate(vignette); // Clamp to [0, 1]
                col.rgb *= vignette;

                // Desaturation
                float luminance = Luminance(col.rgb);
                col.rgb = lerp(col.rgb, luminance.xxx, _Desaturation);
                
                return col;
            }
            ENDCG
        }
    }
}