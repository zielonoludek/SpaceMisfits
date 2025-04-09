Shader "Custom/FogOfWar" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _CircleRadius ("Circle Radius", Float) = 5.0
        _FogColor ("Fog Color", Color) = (1,0,0,0.7)
        [Enum(XZ,0,YZ,1,XY,2)] _PlaneAxis ("Plane Axis", Int) = 2 // Default to XY (2)
    }
    
    SubShader {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }
        LOD 100
        
        // No culling or depth
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float4 worldPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _PlayerPos;
            float _CircleRadius;
            float4 _FogColor;
            int _PlaneAxis;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float2 playerPosPlane;
                float2 worldPosPlane;
                
                // Select the correct plane based on your map orientation
                if (_PlaneAxis == 0) { // XZ plane
                    playerPosPlane = float2(_PlayerPos.x, _PlayerPos.z);
                    worldPosPlane = float2(i.worldPos.x, i.worldPos.z);
                }
                else if (_PlaneAxis == 1) { // YZ plane
                    playerPosPlane = float2(_PlayerPos.y, _PlayerPos.z);
                    worldPosPlane = float2(i.worldPos.y, i.worldPos.z);
                }
                else { // XY plane (default)
                    playerPosPlane = float2(_PlayerPos.x, _PlayerPos.y);
                    worldPosPlane = float2(i.worldPos.x, i.worldPos.y);
                }
                
                // Calculate distance in the selected plane
                float dist = distance(worldPosPlane, playerPosPlane);
                
                // Inside the circle: fully transparent, outside: fog color
                // No transition/outline - sharp edge
                if (dist <= _CircleRadius) {
                    return fixed4(1,1,1,0); // Fully transparent inside circle
                } else {
                    return _FogColor; // Fog color outside
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}