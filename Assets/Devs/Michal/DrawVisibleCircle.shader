Shader "Hidden/DrawVisibleCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _VisibilityRadius ("Visibility Radius", Float) = 5.0
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
                float4 worldPos : TEXCOORD1;
            };
            
            float4 _PlayerPos;
            float _VisibilityRadius;
            sampler2D _MainTex;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Convert UV coordinates back to world space
                o.worldPos = float4(
                    (o.uv.x * 100) - 50,  // Adjust these values based on your map size
                    (o.uv.y * 100) - 50,
                    0, 0
                );
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Get the current explored value
                fixed current = tex2D(_MainTex, i.uv).r;
                
                // Calculate distance from pixel to player
                float dist = distance(float2(i.worldPos.x, i.worldPos.y), float2(_PlayerPos.x, _PlayerPos.y));
                
                // If within visibility radius, set to explored
                float explored = dist < _VisibilityRadius ? 1 : 0;
                
                // Return maximum of current value and new explored value
                // This ensures areas remain explored once discovered
                return max(current, explored);
            }
            ENDCG
        }
    }
}