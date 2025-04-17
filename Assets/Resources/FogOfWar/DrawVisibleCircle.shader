Shader "Hidden/DrawVisibleCircles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _VisibilityRadius ("Visibility Radius", Float) = 5.0
        _QuestRadius ("Quest Radius", Float) = 2.5
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
            float _QuestRadius;
            sampler2D _MainTex;
            
            float4 _QuestPositions[10];
            int _QuestCount;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Convert UV coordinates back to world space
                o.worldPos = float4(
                    (o.uv.x * 100) - 50, 
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
                
                // If within player visibility radius, set to explored
                float explored = dist < _VisibilityRadius ? 1 : 0;
                
                // Check quest positions
                for (int idx = 0; idx < _QuestCount; idx++) 
                {
                    float questDist = distance(float2(i.worldPos.x, i.worldPos.y), 
                                              float2(_QuestPositions[idx].x, _QuestPositions[idx].y));
                    
                    // If within quest visibility radius, also mark as explored
                    if (questDist < _QuestRadius) {
                        explored = 1;
                        break;
                    }
                }
                
                // Return maximum of current value and new explored value
                // This ensures areas remain explored once discovered
                return max(current, explored);
            }
            ENDCG
        }
    }
}