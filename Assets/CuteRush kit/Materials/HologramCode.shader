Shader "Custom/HologramCode"
{
    Properties
    {
        _MainColor ("Color", Color) = (0, 1, 1, 1)
        _RimPower ("Rim Power", Float) = 3.0
        _ScanSpeed ("Scan Speed", Float) = 1.0
        _ScanFrequency ("Scan Freq", Float) = 50.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
            };

            float4 _MainColor;
            float _RimPower;
            float _ScanSpeed;
            float _ScanFrequency;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dotProduct = 1.0 - saturate(dot(normalize(i.normal), normalize(i.viewDir)));
                float rim = pow(dotProduct, _RimPower);
                float scanline = sin(i.worldPos.y * _ScanFrequency + _Time.y * _ScanSpeed);
                scanline = (scanline + 1.0) * 0.5;
                return _MainColor * rim * (0.5 + scanline);
            }
            ENDCG
        }
    }
}