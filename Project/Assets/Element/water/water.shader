Shader "Custom/water"
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
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
			
			fixed4 _Color1;
			fixed4 _Color2;
			float4 _Offset;

			v2f vert (appdata v)
			{
				v2f o;
				v.uv.x = v.uv.x * step(v.uv.x,0.99);
				float temp = sin(v.uv.x * _Offset.z + _Time.z)*_Offset.x + cos(v.uv.x * _Offset.w - _Time.y*0.37)*_Offset.y;
				o.vertex = UnityObjectToClipPos(v.vertex * (1 + temp));
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = lerp(_Color1,_Color2,1.0 - pow(1.0 - i.uv.y,3));
				return col;
			}
			ENDCG
		}
	}
}
