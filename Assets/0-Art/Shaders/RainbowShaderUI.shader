Shader "UI/Rainbow" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Speed ("Speed", Range(0, 10)) = 1
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass {
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv);
                col.r = sin(_Time.y * _Speed + i.uv.y * 2 * 3.14) * 0.5 + 0.5;
                col.g = sin(_Time.y * _Speed + i.uv.y * 2 * 3.14 + 2) * 0.5 + 0.5;
                col.b = sin(_Time.y * _Speed + i.uv.y * 2 * 3.14 + 4) * 0.5 + 0.5;
                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
