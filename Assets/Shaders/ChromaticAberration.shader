Shader "Custom/ChromaticAberrationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amount("Amount", Range(0.0, 2)) = 1.1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
  
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"
  
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
  
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
  
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
  
            sampler2D _MainTex;
            float _Amount;
  
            fixed4 frag (v2f i) : SV_Target
            {
                float2 iuv = (i.uv * 2) - 1;
                iuv /= _Amount;
                iuv = (iuv + 1) * 0.5;

                float colR = tex2D(_MainTex, iuv).r;
                float colG = tex2D(_MainTex, i.uv).g;

                iuv = (i.uv * 2) - 1;
                iuv /= (1 / _Amount);
                iuv = (iuv + 1) * 0.5;

                float colB = tex2D(_MainTex, iuv).b;
                return fixed4(colR, colG, colB, 1);
            }
            ENDCG
        }
    }
}