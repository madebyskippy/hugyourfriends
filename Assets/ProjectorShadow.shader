// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProjectorShadow"{
    Properties {
        _ShadowTex("Projected Image", 2D) = "white"{}
    }
    Subshader{
        Pass{
            Blend Zero OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            uniform sampler2D _ShadowTex;

            uniform float4x4 unity_Projector;

            struct vertexInput{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct vertexOutput{
                float4 pos : SV_POSITION;
                float4 posProj : TEXCOORD0;

            };

            vertexOutput vert(vertexInput input){
                vertexOutput output;
                output.posProj = mul(unity_Projector, input.vertex);
                output.pos = UnityObjectToClipPos(input.vertex);
                return output;
            }

            float4 frag(vertexOutput input) : COLOR{
                if(input.posProj.w > 0.0){
                    return tex2D(_ShadowTex, float2(input.posProj) / input.posProj.w);

                } else{
                    return float4(0.0);
                }

            }

            ENDCG
        }
    }
}