Shader "Unlit/LineShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("Line Width", Range (0, 5)) = 1
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
				float2 v = i[1].vertex.xy - i[0].vertex.xy;
				float l = length (v);
				if (l == 0) {
					return;
				}

				v /= l;
				float2 v2 = float2 (-v.y, v.x);

				float width = _Width;
				float l2 = (l / width) * 0.5f + 1;

				{
					g2f o;
					o.vertex = i[0].vertex + float4 ((-v + v2) * width, 0, 0);
					o.vertex = mul (UNITY_MATRIX_P, o.vertex);
					o.color = i[0].color;
					o.params.x = 1;
					o.params.y = 0;
					o.params.z = l2;
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = i[1].vertex + float4 ((v + v2) * width, 0, 0);
					o.vertex = mul (UNITY_MATRIX_P, o.vertex);
					o.color = i[1].color;
					o.params.x = 1;
					o.params.y = l2;
					o.params.z = 0;
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = i[0].vertex + float4 ((-v - v2) * width, 0, 0);
					o.vertex = mul (UNITY_MATRIX_P, o.vertex);
					o.color = i[0].color;
					o.params.x = -1;
					o.params.y = 0;
					o.params.z = l2;
					outStream.Append (o);
				}
				{
					g2f o;
					o.vertex = i[1].vertex + float4 ((v - v2) * width, 0, 0);
					o.vertex = mul (UNITY_MATRIX_P, o.vertex);
					o.color = i[1].color;
					o.params.x = -1;
					o.params.y = l2;
					o.params.z = 0;
					outStream.Append (o);
				}
			}

			float4 frag (g2f i) : SV_Target
			{
				// こうしたい
				float u = abs (i.params.x);
				float p1 = tex2D(_MainTex, float2 (u, i.params.y)).r;
				float p2 = tex2D(_MainTex, float2 (u, 1-i.params.z)).r;
				float power = abs(p1 - p2);

				float t = smoothstep (0, p1 + p2, p1);

				float4 c = i.color;
				c.a *= power * _Power;
				return c;
			}
			ENDCG
		}
	}
}
