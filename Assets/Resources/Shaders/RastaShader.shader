Shader "EgorShaders/RastaShader"
{
	Properties
	{
		_OffsetX("OffsetX",Range(0,0.5)) = 0.1
		_OffsetY("OffsetY",Range(0,0.5)) = 0.1
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			float _OffsetX;
			float _OffsetY;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				float2 offsetPosRight = i.uv;
				float2 offsetPosLeft = i.uv;

				float diff = abs(0.5 - offsetPosRight.x);

				float alpha = 0.5 - diff*diff;
				offsetPosRight.x += _OffsetX;
				offsetPosLeft.x -= _OffsetX;

				float4 offsetColLeft = tex2D(_MainTex, offsetPosLeft.xy);
				float4 offsetColRight = tex2D(_MainTex, offsetPosRight.xy);
				col = lerp(col, offsetColLeft, alpha);
				col = lerp(col, offsetColRight, alpha);
				return col;
			}
			ENDCG
		}
	}
}
