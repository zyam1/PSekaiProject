Shader "UI/RadialGradient"
{
    Properties
    {
        _Color ("Center Color", Color) = (0,0,0,1) // 중앙 색상
        _EdgeColor ("Edge Color", Color) = (0,0,0,0) // 가장자리 색상 (투명)
        _Radius ("Radius", Range(0, 1)) = 0.5 // 그라데이션 범위 조절
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // UV 좌표를 -1~1 범위로 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.uv); // 중심에서의 거리 계산
                float alpha = smoothstep(_Radius, _Radius + 0.1, 1.0 - dist);
                return lerp(_EdgeColor, _Color, alpha);
            }
            ENDCG
        }
    }
}
