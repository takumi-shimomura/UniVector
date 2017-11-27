Shader "Unlit/PreproccessBrightnessMapShader"
{
	Properties
	{
//		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vsIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vsOut
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			vsOut vert (vsIn v)
			{
				vsOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag (vsOut i) : SV_Target
			{
				float2 v = float2((i.uv.x - 0.5f) * 2.f, i.uv.y);
				float4 col;
				col.r = length (v);
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}
