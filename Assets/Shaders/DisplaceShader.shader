Shader "Custom/DisplaceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplaceTex ("Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", Range(0,1)) = 0.3
        _Shift ("Y Shift", Range(0,1)) = 0
        _Zoom ("Displace Texture Zoom", Range(0.0001, 2)) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            uniform sampler2D _DisplaceTex;
            uniform float _Magnitude;
            uniform float _Shift;
            uniform float _Zoom;

            float4 frag (v2f_img i) : COLOR
            {
                float2 displacementUv = float2(i.uv.x, i.uv.y + _Shift) * _Zoom;
                float4 displacement = tex2D(_DisplaceTex, displacementUv);
                displacement.rg = (displacement.rg * 2) - 1;

                i.uv.x += displacement.r * _Magnitude;
                i.uv.y += displacement.g * _Magnitude;
                i.uv = saturate(i.uv);

                float4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
