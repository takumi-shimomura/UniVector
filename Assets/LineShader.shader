﻿Shader "Unlit/LineShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("Line Width", Range (0.1, 5)) = 1
		_Power ("Power", Range (0, 5)) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma target 4.0

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct vsIn
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float3 params : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _Width;
			float _Power;
			float4 ScreenParams;
			
			v2g vert (vsIn i)
			{
				v2g o;
				o.vertex = float4 (UnityObjectToViewPos(i.vertex), 1);
				o.color = i.color;
				return o;
			}

			[maxvertexcount(4)]
			void geom (line v2g i[2], inout TriangleStream<g2f> outStream)
			{
				float4 p1 = i[0].vertex;
				float4 p2 = i[1].vertex;

				float4 vx = float4 (p2.xy - p1.xy, 0, 0);
				float l = length (vx);
				if (l == 0) {
					return;
				}

				vx = vx * _Width / l;
				float4 vy = float4 (-vx.y, vx.x, 0, 0);
				float l2 = (l / _Width) * 0.5f + 1;

				fixed4 color = i[0].color;
				{
					g2f o;
					o.vertex = mul (UNITY_MATRIX_P, p1 - vx + vy);
					o.color = color;
					o.params = float3 (1, 0, 1 - l2);
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = mul (UNITY_MATRIX_P, p2 + vx + vy);
					o.color = color;
					o.params = float3 (1, l2, 1 - 0);
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = mul (UNITY_MATRIX_P, p1 - vx - vy);
					o.color = color;
					o.params = float3 (-1, 0, 1 - l2);
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = mul (UNITY_MATRIX_P, p2 + vx - vy);
					o.color = color;
					o.params = float3 (-1, l2, 1 - 0);
					outStream.Append (o);
				}
			}

			float4 frag (g2f i) : SV_Target
			{
				float p1 = tex2D(_MainTex, i.params.xy).r;
				float p2 = tex2D(_MainTex, i.params.xz).r;
				float power = p1 - p2;

				float4 c = i.color;
				c.a *= power * _Power;
				return c;
			}
			ENDCG
		}
	}
}
