Shader "Custom/Layers/RegionType" {

	Properties {
		_Land ("Land Color", Color) = (0, 1, 0, 1)
		_Ocean ("Ocean Color", Color) = (0, 0, 1, 1)
		_Mountain ("Mountain Color", Color) = (0.5, 0.5, 0.5, 1)
		_Altitude ("Altitude", float) = 0
		_OceanLandThreshold ("Ocean Land Threshold", float) = 0.45
		_LandMountainThreshold ("Land Mountain Threshold", float) = 0.55
	}

	SubShader {

		Pass {

			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			float4 _Ocean, _Land, _Mountain;
			float _Altitude, _OceanLandThreshold, _LandMountainThreshold;
			
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
				
				if(_Altitude < _OceanLandThreshold) {
					return _Ocean;
				}
				else if(_Altitude < _LandMountainThreshold) {
					return _Land;
				}
				else {
					return _Mountain;
				}
			}

			ENDCG

		}

	}

}