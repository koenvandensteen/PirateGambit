Shader "Custom/TiledWaterMinimal" {
	Properties {
		_Color ("Water Color", Color) = (1,1,1,1)
		_BorderColor ("Border Color", Color) = (1,1,1,1)
		[MaterialToggle] _IsActivated ("Is Activated", float) = 0
		_EmphasisColor ("Emphasis Color", Color) = (1,1,1,1)
		_EmphasisFlickerSpeed("Flicker Speed", float) = 1
		_EmphasisTime("Emphasis Time", float) = 0
		[MaterialToggle] _IsEmphasis ("Is Emphasis", float) = 0
		_HighLightColor ("Highlight Color", Color) = (1,1,1,1)
		[MaterialToggle] _IsHighlighted ("Is Highlighted", float) = 0		
	}
	SubShader {
		Tags {"RenderType" = "Opaque"}
		LOD 200


		Pass {
            Tags { "LightMode" = "Always" }
           
            Fog { Mode Off }
            ZWrite On
            ZTest LEqual
            Cull Back
            Lighting Off
   
            CGPROGRAM
            	#include "UnityCG.cginc"
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
               
                fixed4 _Color;
                fixed4 _BorderColor;
                float _IsActivated;
                fixed4 _EmphasisColor;
                float _EmphasisFlickerSpeed;
                float _IsEmphasis;
                float _EmphasisTime;
                fixed4 _HighLightColor;
                float _IsHighlighted;
               
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
               
                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }
               
                fixed4 frag (v2f i) : COLOR {

                	float3 finalColor = {0,0,0};

                    float3 baseColor = _Color;

					
                    //Add greyscale
					float activeMod = 0.33;

    				float greyscale = dot(_BorderColor.rgb, fixed3(.222, .707, .071));  // Convert to greyscale numbers with magic luminance numbers
					float3 borderColor = lerp(float3(greyscale, greyscale, greyscale), _BorderColor.rgb, activeMod + (_IsActivated * (1 - activeMod)));


					//Colored Border
					float alpha = fmod(floor(clamp(floor((i.uv.r * 10) * 0.75) , 0, 2)), 2);
					alpha *= _BorderColor.a;

					finalColor = borderColor * alpha + baseColor * (1-alpha);


					//Emphasis
					float borderWidth = 3;
					float timeMult = round(fmod(_Time.y * _EmphasisFlickerSpeed,1));
					float emphasisAlpha = fmod(floor(clamp(floor((i.uv.r * borderWidth)), borderWidth - 2, borderWidth) - (borderWidth-2) ), 2) * _IsEmphasis * timeMult;
					
					//_EmphasisTime += _Time.y * _EmphasisFlickerSpeed * _IsEmphasis;
					//float emphasisAlpha = fmod(floor(clamp(floor((i.uv.r * 10) - (9 - _EmphasisTime)) , 0, 2)), 2);

					emphasisAlpha *= _EmphasisColor.a;

					finalColor = _EmphasisColor * emphasisAlpha + finalColor * (1-emphasisAlpha);


					//Highlight
					borderWidth = 2;
					float highlightAlpha = fmod(floor(clamp(floor((i.uv.r * borderWidth)), borderWidth - 2, borderWidth) - (borderWidth-2) ), 2) * _IsHighlighted;
					highlightAlpha *= _HighLightColor.a;


					finalColor = _HighLightColor * highlightAlpha + finalColor * (1-highlightAlpha);

					return float4(finalColor, 1);
                }
            ENDCG
   
        }
	}
	FallBack "Diffuse"
}