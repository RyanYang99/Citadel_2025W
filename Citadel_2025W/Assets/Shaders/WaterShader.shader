Shader "Custom/WaterShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveAmplitude ("Wave Amplitude", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct attributes
            {
                float4 position_os : POSITION;
            };

            struct varyings
            {
                float4 position_hcs : SV_POSITION;
                float3 world_position : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _WaveSpeed;
                float _WaveAmplitude;
            CBUFFER_END

            float rand(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898,78.233))) * 43758.5453);
            }
            
            varyings vert(attributes input)
            {
                float3 position = input.position_os.xyz;
                position.y += sin((_WaveSpeed * _Time.y) + (rand(position.xz) * 100.0)) * _WaveAmplitude;
                
                varyings output;
                output.position_hcs = TransformObjectToHClip(position);
                output.world_position = TransformObjectToWorld(position);
                return output;
            }

            half4 frag(varyings input) : SV_Target
            {
                float3 world_normal = normalize(cross(ddy(input.world_position), ddx(input.world_position)));
                
                Light main_light = GetMainLight();
                float direct = _BaseColor.rgb * main_light.color * saturate(dot(world_normal, main_light.direction));
                
                return half4(_BaseColor.rgb * (direct + SampleSH(world_normal)), 1.0);
            }
            
            ENDHLSL
        }
    }
}