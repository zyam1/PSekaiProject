Shader "UI/VerticalGradient"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (0,0,0,1)  // 위쪽 색 (검정)
        _ColorBottom ("Bottom Color", Color) = (0,0,0,0) // 아래쪽 색 (투명)
        _Sharpness ("Sharpness", Range(1, 5)) = 2 // 투명해지는 속도 조절
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha  // 알파 블렌딩 적용 (투명도)
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            float _Sharpness; // 투명해지는 속도

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // i.uv.y 값을 _Sharpness(2~5)제곱해서 급격하게 변화
                float gradientFactor = pow(i.uv.y, _Sharpness);
                return lerp(_ColorTop, _ColorBottom, gradientFactor);
            }
            ENDCG
        }
    }
}
