Shader "Custom/Waves" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_WaterFogColor ("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity ("Water Fog Density", Range(0, 2)) = 0.1
		_RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.25
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_Amplitude ("Amplitude", Float) = 1
		_WaveA ("Wave A (dir, steepness, wavelength)", Vector) = (1,0,0.5,10)
		_WaveB ("Wave B", Vector) = (0,1,0.25,20)
		_WaveC ("Wave C", Vector) = (1,1,0.15,10)
		_WaveD ("Wave B", Vector) = (0,1,0.25,20)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent+2" }
		LOD 200

		GrabPass {"_WaterBackground"}

		CGPROGRAM

		#pragma surface surf Standard alpha vertex:vert addshadow
		#pragma target 3.0

		#include "LookingThroughWater.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _WaveA, _WaveB, _WaveC, _WaveD;

		float3 GerstnerWave (
			float4 wave, float3 p, inout float3 tangent, inout float3 binormal
		) {
		    float steepness = wave.z;
		    float wavelength = wave.w;
		    float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, p.xz) - c * _Time.y);
			float a = steepness / k;
			
			//p.x += d.x * (a * cos(f));
			//p.y = a * sin(f);
			//p.z += d.y * (a * cos(f));

			tangent += float3(
				-d.x * d.x * (steepness * sin(f)),
				d.x * (steepness * cos(f)),
				-d.x * d.y * (steepness * sin(f))
			);
			binormal += float3(
				-d.x * d.y * (steepness * sin(f)),
				d.y * (steepness * cos(f)),
				-d.y * d.y * (steepness * sin(f))
			);
			return float3(
				d.x * (a * cos(f)),
				a * sin(f),
				d.y * (a * cos(f))
			);
		}

		void vert(inout appdata_full vertexData) {
			float3 gridPoint = vertexData.vertex.xyz;
			float3 tangent = float3(1, 0, 0);
			float3 binormal = float3(0, 0, 1);
			float3 p = gridPoint;
			p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
			p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
			p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
			p += GerstnerWave(_WaveD, gridPoint, tangent, binormal);
			float3 normal = normalize(cross(binormal, tangent));
			vertexData.vertex.xyz = p;
			vertexData.normal = normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
			
			o.Albedo = ColorBelowWater(IN.screenPos, o.Normal);
			o.Alpha = 1;
		}
		ENDCG
	}
	//FallBack "Diffuse"
}