Shader "Custom/FogOfWar"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0, 0, 0, 0.8)
        _ExploredColor ("Explored Color", Color) = (0, 0, 0, 0.4)
        _VisibilityRadius ("Visibility Radius", Float) = 5.0
        _QuestRadius ("Quest Radius", Float) = 2.5
        _FadeEdgeSize ("Fade Edge Size", Range(0, 1)) = 0.2
        _ExploredTex ("Explored Texture", 2D) = "black" {}
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
                float4 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FogColor;
            float4 _ExploredColor;
            float _VisibilityRadius;
            float _QuestRadius;
            float _FadeEdgeSize;
            sampler2D _ExploredTex;
            float4 _PlayerPos;
            
            // Array of quest positions (x,y,0,0), maximum 10
            float4 _QuestPositions[10];
            int _QuestCount;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from current fragment to player
                float dist = distance(float2(i.worldPos.x, i.worldPos.y), float2(_PlayerPos.x, _PlayerPos.y));
                
                // Determine if this position is within player's visibility
                float inRadius = 1.0 - smoothstep(_VisibilityRadius * (1.0 - _FadeEdgeSize), _VisibilityRadius, dist);
                
                // Check if position is within any quest radius
                float inQuestRadius = 0;
                for (int idx = 0; idx < _QuestCount; idx++) 
                {
                    float questDist = distance(float2(i.worldPos.x, i.worldPos.y), 
                                             float2(_QuestPositions[idx].x, _QuestPositions[idx].y));
                    
                    // If within quest visibility radius, mark as visible
                    if (questDist < _QuestRadius) {
                        inQuestRadius = 1.0 - (questDist / _QuestRadius);
                        break;
                    }
                }
                
                // Sample the explored map texture
                // Convert world position to texture coordinates
                float2 texUV = float2(
                    (i.worldPos.x + 50) / 100,
                    (i.worldPos.y + 50) / 100
                );
                float explored = tex2D(_ExploredTex, texUV).r;
                
                // Either visible around player, quest, or previously explored
                float visibility = max(max(inRadius, explored), inQuestRadius);
                
                // Determine final fog color and alpha
                float4 finalColor = _FogColor;
                float fogAlpha = 0;
                
                if (visibility <= 0)
                {
                    fogAlpha = _FogColor.a;
                }
                
                return float4(finalColor.rgb, fogAlpha);
            }
            ENDCG
        }
    }
}