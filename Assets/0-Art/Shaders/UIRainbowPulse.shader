Shader "Custom/ImageRainbowShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 10)) = 1
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.5
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 1
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
            float _Speed;
            float _PulseAmount;
            float _PulseSpeed;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // ограничение координат UV
                o.uv = v.uv * _MainTex_TexelSize.xy;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float t = _Time.y * _Speed;
                float3 c = float3(0, 0, 0);
                c.x = sin(t) * 0.5 + 0.5;
                c.y = sin(t + 2) * 0.5 + 0.5;
                c.z = sin(t + 4) * 0.5 + 0.5;
                
                c *= _Color.rgb;
                
                // добавление эффекта пульсации
                float pulse = sin(t * _PulseSpeed) * 0.5 + 0.5;
                c = lerp(c, _Color.rgb * _PulseAmount, pulse);
                
                return fixed4(c, _Color.a);
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
