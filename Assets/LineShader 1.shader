Shader "Unlit/LineShader_BAK"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("lineWidth", Float) = 1
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
				float2 uv : TEXCOORD0;
			};

			struct vsOut
			{
				float4 vertex : SV_POSITION;
				float3 params : TEXCOORD0;
			};

			sampler2D _MainTex;
//			float4 _MainTex_ST;
			float4 ScreenParams;
			
			vsOut vert (vsIn v)
			{
				vsOut o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = float4 (UnityObjectToViewPos(v.vertex), 1);
				o.vertex = mul (UNITY_MATRIX_P, o.vertex);
//				o.vertex.xyz /= o.vertex.w;
//				o.vertex.w = 1;
				o.params = float3 (0,0,0);
				return o;
			}

			[maxvertexcount(4)]
			void geom (line vsOut i[2], inout TriangleStream<vsOut> outStream)
			{
				float2 v = i[1].vertex.xy - i[0].vertex.xy;
				float l = length (v);
				if (l == 0) {
					return;
				}

				v /= l;
				float2 v2 = float2 (-v.y, v.x);

				float width = 0.01f;
				float l2 = l / width + width + width;

				{
					vsOut o = i[0];
					o.vertex.xy += (-v + v2) * width;
					o.params.x = 1;
					o.params.y = 0;
					o.params.z = l2;
					outStream.Append (o);
				}
				{
					vsOut o = i[1];
					o.vertex.xy += (v + v2) * width;
					o.params.x = 1;
					o.params.y = l2;
					o.params.z = 0;
					outStream.Append (o);
				}
				{
					vsOut o = i[0];
					o.vertex.xy += (-v - v2) * width;
					o.params.x = -1;
					o.params.y = 0;
					o.params.z = l2;
					outStream.Append (o);
				}
				{
					vsOut o = i[1];
					o.vertex.xy += (v - v2) * width;
					o.params.x = -1;
					o.params.y = l2;
					o.params.z = 0;
					outStream.Append (o);
				}
			}

			float4 frag (vsOut i) : SV_Target
			{
				// こうしたい
				float u = abs (i.params.x);
				float power = 
					tex2D(_MainTex, float2 (u, i.params.y)).r +
					tex2D(_MainTex, float2 (u, i.params.z)).r -
					tex2D(_MainTex, float2 (u, 1)).r;

				return float4 (1,1,1, power);
			}
			ENDCG
		}
	}
}
