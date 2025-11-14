// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FuzzyLogic Studios/Mode7_Spr_Shadow"
{
	Properties
	{
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Order("Rendering Order", int) = 3
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "PreviewType" = "Plane" }
		Pass
		{
			ZTest Off Cull Off ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha
			Stencil {
				Ref[_Order]
				Comp GEqual
				Pass Replace
				Fail Keep
				ZFail Replace
			}
			
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
				fixed4 col = _Color;
				clip(tex.a - 0.35);
				return col;
			}
			ENDCG
		}
	}
}
