Shader "Custom/BorderTile" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_BorderColor ("Border Color", Color) = (1, 1, 1, 1)
		_BorderTex ("Border mask (R)", 2D) = "white" {}
		_GridColor ("Grid Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags {"RenderType" = "Opaque"}
		LOD 200


		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BorderTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _BorderColor;
		fixed4 _GridColor;
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 baseC = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 borderC = tex2D(_BorderTex, IN.uv_MainTex);
			o.Albedo = baseC.rgb * (1 - (borderC.r * _BorderColor.a)) + _BorderColor * borderC.r * _BorderColor.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = baseC.a;
			float4 emission = {0,0,0,0};
			float power = 32;
			emission += pow(_GridColor * IN.uv_MainTex.r, power) + pow(_GridColor * (1-IN.uv_MainTex.r), power);
			emission += pow(_GridColor * IN.uv_MainTex.g, power) + pow(_GridColor * (1-IN.uv_MainTex.g), power);
			o.Emission = emission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}