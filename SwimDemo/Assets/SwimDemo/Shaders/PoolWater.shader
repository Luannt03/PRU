Shader "SwimDemo/PoolWater"
{
    Properties { _Color ("Water colour", Color) = (0.05,0.48,0.6,0.27) _WaveTime ("Simulation time", Float) = 0 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            float4 _Color; float _WaveTime;
            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 position : SV_POSITION; float3 world : TEXCOORD0; float3 normal : TEXCOORD1; UNITY_FOG_COORDS(2) };
            v2f vert(appdata v)
            {
                v2f o;
                float ax = v.vertex.x * .8 + _WaveTime * 1.2;
                float az = v.vertex.z * 1.3 + _WaveTime * 1.9;
                v.vertex.y += .035 * sin(ax) + .020 * sin(az);
                o.normal = UnityObjectToWorldNormal(normalize(float3(-.028 * cos(ax), 1, -.026 * cos(az))));
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz; o.position = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.position); return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 eye = normalize(_WorldSpaceCameraPos - i.world);
                float3 normal = normalize(i.normal); if (dot(eye,normal) < 0) normal = -normal;
                float fresnel = pow(1 - saturate(dot(eye, normal)), 3);
                float3 light = normalize(UnityWorldSpaceLightDir(i.world));
                float specular = pow(saturate(dot(reflect(-light, normal), eye)), 100);
                float shimmer = .015 * sin(i.world.x * 15 + i.world.z * 9 + _WaveTime * 3);
                fixed4 colour = fixed4(_Color.rgb + fresnel * float3(.12,.2,.23) + specular * _LightColor0.rgb * .65 + shimmer, min(.55, _Color.a + fresnel * .18));
                UNITY_APPLY_FOG(i.fogCoord,colour); return colour;
            }
            ENDCG
        }
    }
}
