Shader "Unlit/UIBorderShader"
{
     Properties
    {
        _BorderColor("Border Color", Color) = (1,1,1,1)
        _BorderThickness("Border Thickness", Range(0.0, 0.1)) = 0.05
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BorderColor;
            float _BorderThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                color.rgb = _BorderColor.rgb;

                float border = _BorderThickness;
                if (i.uv.x > border && i.uv.x < 1.0 - border && i.uv.y > border && i.uv.y < 1.0 - border)
                {
                    color.a = 0; // Transparent fill
                }

                return color;
            }
            ENDCG
        }
    }
}
