// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EgorShaders/FogShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IlluminationUV("Illumination Coords", Vector) = (0.5,0.5,0,0)
		_IlluminationRadius("Illumination Radius", Range(0,1)) = 0.1
		_FogColor("Fog Color", Range(0,1)) = 0.5
		_FogPower("Fog Power", Range (0,1)) = 0.5
	}

	SubShader {
		Pass {
		ZTest Always Cull Off ZWrite Off Fog { Mode off }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"
		#pragma target 3.0

		struct v2f { 
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		}; 

		uniform sampler2D _MainTex;
		uniform float _IlluminationRadius;
		uniform float _FogColor;
		uniform float _FogPower;
		uniform float4 _IlluminationUV;

		v2f vert (appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
			return o;
		}


		half4 frag (v2f i) : COLOR
		{
			float illumCoef = length(i.uv - _IlluminationUV.xy) /_IlluminationRadius;
			//if (illumCoef > 1)
				//illumCoef = 1;
				//illumCoef = illumCoef % 1;
			illumCoef = clamp(illumCoef,0, 1);
			illumCoef = (1-illumCoef) * _FogPower;
			illumCoef = illumCoef* illumCoef;
			float4 fogColor = float4(_FogColor,_FogColor,_FogColor,1);
			float4 screenColor = tex2D (_MainTex, i.uv);
			float4 finalColor = lerp(screenColor, fogColor, _FogPower - illumCoef);
			return finalColor;

		}

		ENDCG
		}
	}

	Fallback off

}

