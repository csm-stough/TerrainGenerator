Shader "Custom/Layers/Temperature" {

	Properties {
		_Cold ("Cold Color", Color) = (0, 0, 1, 1)
		_Hot ("Hot Color", Color) = (1, 0, 0, 1)
		_MaxTemperature ("Max Temperature", float) = 100
		_Temperature ("Temperature", float) = 0
	}

	SubShader {

		Pass {

			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			float4 _Cold, _Hot;
			float _MaxTemperature, _Temperature;
			
			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvDetail : TEXCOORD1;
			};

			struct VertexData {
				float4 position: POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators MyVertexProgram (VertexData v) {
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				return i;
			}

			float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				
				return lerp(_Cold, _Hot, _Temperature / _MaxTemperature);

			}

			ENDCG

		}

	}

}