Shader "Custom/line"
{
	Properties
	{
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)
		_Offset ("Offset", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		ZWrite Off
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
			
			fixed4 _Color1;
			fixed4 _Color2;
			float4 _Offset;

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = mul(unity_ObjectToWorld,v.vertex).xy;

				v.vertex.x += (sin(v.vertex.y + _Time.z*0.17) - cos(v.vertex.y + _Time.z*0.33)*2) * v.uv.y * _Offset.y;
				v.vertex.z += (sin(v.vertex.y + _Time.z * 0.21) - cos(v.vertex.y + _Time.z*0.71)*2) * v.uv.y * _Offset.y;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = lerp(_Color1,_Color2,step(i.uv.y,_Offset.x));
				float temp = i.uv.y - _Offset.x;
				col.a = step(0,temp) + step(temp,0) * max(1+temp*3,0) * 0.5;
				return col;
			}
			ENDCG
		}
	}
}
