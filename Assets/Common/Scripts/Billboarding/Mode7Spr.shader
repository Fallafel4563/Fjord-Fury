// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hedgehog Arts/Mode7_Spr"
{
	Properties
	{
		_Color ("Color Tint", Color) = (1,1,1,1)
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Cutoff("Alpha Cutout", Range(0,1)) = 0.5
		[PerRendererData] _Order("Rendering Order", Float) = 129
		_flip("flip X axis", int) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "PreviewType" = "Plane" }
		Pass
		{
			Cull Off ZWrite On ZTest LEqual

			Stencil {
				Ref[_Order]
				Comp GEqual
				Pass Replace
				Fail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0			
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
			half4 _MainTex_ST;
			half _Cutoff;
			uniform float _flip;
			uniform float _horizon;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
				
				return o;
			}
			
			fixed4 _Color;
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= _Color.rgb;
				clip(col.a - _Cutoff);
				
				return col;
			}
			ENDCG
		}
	}
}
