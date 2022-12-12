Shader "Custom/Layers/Moisture" {

	Properties {
		_Dry ("Dry Color", Color) = (0, 0, 1, 1)
		_Wet ("Wet Color", Color) = (1, 0, 0, 1)
		_MaxMoisture ("Max Moisture", float) = 100
		_Moisture ("Moisture", float) = 0
	}

	SubShader {

		Pass {

			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			float4 _Dry, _Wet;
			float _MaxMoisture, _Moisture;
			
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
				
				return lerp(_Dry, _Wet, _Moisture / _MaxMoisture);

			}

			ENDCG

		}

	}

}