Shader "Custom/ZoomInterpolate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", Range(0, 2)) = 0.3
        _Interpolation ("Interpolation", Range(0, 2)) = 0.3
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
            uniform float _Magnitude;
            uniform float _Interpolation;

            fixed4 frag (v2f_img i) : COLOR
            {
                fixed colBase = tex2D(_MainTex, i.uv);
                i.uv = (i.uv * 2) - 1;
                i.uv /= _Magnitude;
                i.uv = (i.uv + 1) * 0.5;
                //i.uv = saturate(i.uv);
                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(colBase, col, _Interpolation);
            }
            ENDCG
        }
    }
}
