Shader "SwimDemo/Solid"
{
    Properties { _Color ("Colour", Color) = (1,1,1,1) _MainTex ("Texture", 2D) = "white" {} _Gloss ("Gloss", Range(8,128)) = 32 }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            sampler2D _MainTex; float4 _MainTex_ST, _Color; float _Gloss;
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float2 uv : TEXCOORD0; };
            struct v2f { float4 position : SV_POSITION; float3 normal : TEXCOORD0; float3 world : TEXCOORD1; float2 uv : TEXCOORD2; UNITY_FOG_COORDS(3) };
            v2f vert(appdata v)
            {
                v2f o; o.position = UnityObjectToClipPos(v.vertex); o.normal = UnityObjectToWorldNormal(v.normal);
                o.world = mul(unity_ObjectToWorld,v.vertex).xyz; o.uv = TRANSFORM_TEX(v.uv,_MainTex); UNITY_TRANSFER_FOG(o,o.position); return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal), light = normalize(UnityWorldSpaceLightDir(i.world));
                float diffuse = saturate(dot(normal,light));
                float3 eye = normalize(_WorldSpaceCameraPos-i.world);
                float specular = pow(saturate(dot(normal,normalize(light+eye))),_Gloss);
                float3 albedo = tex2D(_MainTex,i.uv).rgb * _Color.rgb;
                fixed4 colour = fixed4(albedo * (UNITY_LIGHTMODEL_AMBIENT.rgb + .25 + diffuse * _LightColor0.rgb * .7) + specular * .22,1);
                UNITY_APPLY_FOG(i.fogCoord,colour); return colour;
            }
            ENDCG
        }
    }
}
