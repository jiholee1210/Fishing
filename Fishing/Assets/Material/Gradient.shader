Shader "Custom/Gradient"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Color2 ("Color 2", Color) = (0,0,0,0)
        _BlackWidth ("Black Width", Float) = 0.3
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
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

            fixed4 _Color;
            fixed4 _Color2;
            float _BlackWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = smoothstep(0.0, _BlackWidth, i.uv.x);
                return lerp(_Color, _Color2, t);
            }
            ENDCG
        }
    }
}
