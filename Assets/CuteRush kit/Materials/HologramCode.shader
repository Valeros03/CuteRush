Shader "Custom/MedikitHologram"
{
    Properties
    {
        // QUESTA È LA MAGIA: Ora lo shader può ricevere la tua immagine!
        _MainTex ("Base Texture", 2D) = "white" {}
        
        _MainColor ("Hologram Tint", Color) = (0, 0.8, 1, 1) // Colore del bagliore (es. Ciano)
        _RimPower ("Rim Power", Float) = 3.0
        _ScanSpeed ("Scan Speed", Float) = 5.0
        _ScanFrequency ("Scan Freq", Float) = 50.0
        
        // Ho aggiunto un controllo per l'opacità generale
        _Opacity ("Overall Opacity", Range(0, 1)) = 0.6 
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            // Usiamo il blending Standard per mantenere i colori originali (Bianco e Rosso)
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
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // Aggiunto per le coordinate dell'immagine
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; // Aggiunto per le coordinate dell'immagine
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float4 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float _RimPower;
            float _ScanSpeed;
            float _ScanFrequency;
            float _Opacity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Passiamo i dati della texture
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1. Leggiamo l'immagine originale della cassetta (la croce rossa)
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 2. Calcoliamo i bordi luminosi (Rim Lighting)
                float dotProduct = 1.0 - saturate(dot(normalize(i.normal), normalize(i.viewDir)));
                float rim = pow(dotProduct, _RimPower);

                // 3. Calcoliamo le linee di scansione (effetto TV vecchia)
                float scanline = sin(i.worldPos.y * _ScanFrequency + _Time.y * _ScanSpeed);
                scanline = (scanline + 1.0) * 0.5;

                // 4. COMBINIAMO TUTTO
                // Prendiamo la texture originale, le aggiungiamo un alone luminoso sui bordi,
                // e applichiamo le linee di scansione sopra.
                float3 finalRGB = (texColor.rgb + (_MainColor.rgb * rim)) * (0.5 + scanline * 0.5);

                // 5. Impostiamo la trasparenza generale
                float finalAlpha = texColor.a * _Opacity;

                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}