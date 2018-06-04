Shader "Custom/StandardVertexColor" {
	Properties {
		_MainTex ("Palette (RGB)", 2D) = "white" {}
		_PropTex ("Properties (RGB)", 2D) = "white" {}
		_NoiseVolume ("Noise Volume", 3D) = "white" {}
		_NoiseFilter ("Noise Filter", Range(0,1)) = 0.5
		_EmissionMultiplier ("Emission Multplier", Range(0,10)) = 1.5
		_SmoothnessNoise ("Smoothness Noise", Range(0,1)) = 0.5
		_MetallicNoise ("Metallic Noise", Range(0,1)) = 0.5
		_EmissionNoise ("Emission Noise", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _PropTex;
		sampler3D _NoiseVolume;

		struct Input {
			float4 color : COLOR;
			float3 localPos;
		};
		
		float _EmissionMultiplier, _NoiseFilter, _EmissionNoise, _MetallicNoise, _SmoothnessNoise;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.localPos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			fixed4 c = tex2D (_MainTex, IN.color.r);
			fixed4 p = tex2D (_PropTex, IN.color.r);
			fixed4 n = 0;
			n += tex3D (_NoiseVolume, IN.localPos*p.b);
			n += tex3D (_NoiseVolume, IN.localPos*p.b*2) * 0.75;
			n += tex3D (_NoiseVolume, IN.localPos*p.b*4) * 0.5;
			n = lerp(_NoiseFilter, 1, n);

			o.Albedo = c.rgb * n.r;
			// o.Albedo = IN.color.rgb;
			o.Emission = c * p.a * _EmissionMultiplier * (1-n.a*_EmissionNoise);
			o.Smoothness = p.r * (1-n.g*_SmoothnessNoise);
			o.Metallic = p.g * n.b * (1-n.g*_MetallicNoise);;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"

}
