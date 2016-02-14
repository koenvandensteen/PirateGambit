Shader "Custom/TiledWater" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextureSize("Texture Scale", float) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GridColor ("Grid Color", Color) = (1, 1, 1, 1)
		_GridPower ("Grid Power", float) = 8
		_BorderColor ("Border Color", Color) = (1,1,1,1)
		_BorderWidth ("border width", float) = 2
		[MaterialToggle] _IsActivated ("Is Activated", float) = 0
	}
	SubShader {
		Tags {"RenderType" = "Opaque"}
		LOD 200


		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _GridColor;
		fixed4 _BorderColor;
		float _GridPower;
		float _BorderWidth;
		float _IsActivated;
		float _TextureSize;
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float timeX = 0.25f;
			float timeY = 0.1f;
			float activeMod = 0.66;
			fixed4 baseC = tex2D(_MainTex, IN.worldPos.rb / _TextureSize) * _Color;

			float greyscale = dot(baseC.rgb, fixed3(.222, .707, .071));  // Convert to greyscale numbers with magic luminance numbers
			o.Albedo = lerp(float3(greyscale, greyscale, greyscale), baseC.rgb, activeMod + (_IsActivated * (1 - activeMod)));

			//o.Albedo = baseC.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = baseC.a;
			float4 emission = {0,0,0,0};
			
			emission += pow(_GridColor * (1-IN.uv_MainTex.r), _GridPower) * _GridColor.a * ((((sin(_Time.x * 20) + 1.0f))/4.0f) + 0.5f);
			float alpha = (pow(_GridColor * (1-fmod(IN.uv_MainTex.r + 0.85f,1)), 5));
			if(alpha > 0.5f){
				emission += _BorderColor * _BorderColor.a * 0.5f;
			}

			o.Emission = emission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}