Shader "Custom/Minimal Tile" {
    Properties {
        _BorderColor ("Main Color", Color) = (1,1,1,1)
        _GridColor ("Grid Color", Color) = (0, 0, 0, 1)
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader {
        Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        fixed4 _BorderColor;
        fixed4 _GridColor;
        sampler2D _MainTex;

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        float4 _MainTex_ST;

        v2f vert (appdata_base v)
        {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 texcol = _BorderColor;
            texcol *= clamp((1-fmod(i.uv.r + 0.95f,1)) / (1-i.uv.r), 0, 1);
            return texcol;
        }
        ENDCG

        }
    }
}