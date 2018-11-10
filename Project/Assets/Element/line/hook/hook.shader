Shader "Custom/hook"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
		Cull Off
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
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			fixed4 _Color1;
			fixed4 _Color2;
			float4 _Offset;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.xy = mul(unity_ObjectToWorld,v.vertex).xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = lerp(_Color1,_Color2,step(i.uv.y,_Offset.x));
				float temp = i.uv.y - _Offset.x;
				col.a = step(0,temp) + step(temp,0) * max(1+temp*3,0) * 0.5;
				col *= tex2D(_MainTex, i.uv.zw);
				return col;
			}
			ENDCG
		}
	}
}
