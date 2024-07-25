Shader "FX/SeeThrough" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OccludeColor ("Occlusion Color", Color) = (0,0,1,1)
        _Emission("Emission", float) = 0
         [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0)
        _EmissionMap ("Emission Map", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue"="Geometry+5"}
        // occluded pass
        Pass {
            ZWrite Off
            Blend One Zero
            ZTest Greater
            Color [_OccludeColor]
        }
        // Vertex lights
        Pass {
            Tags {"LightMode" = "Vertex"}
            ZWrite On
            Lighting On
            SeparateSpecular On
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Emission [_Emission]
            }
            SetTexture [_MainTex] {
                ConstantColor [_Color]
                Combine texture * primary DOUBLE, texture * constant
            }
        }
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _EmissionMap;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // Emission
            o.Emission = tex2D(_EmissionMap, IN.uv_MainTex) * _EmissionColor;
        }
        ENDCG
    }
    FallBack "Diffuse", 1
}