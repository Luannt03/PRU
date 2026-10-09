Shader "SwimDemo/Bubbles"
{
    Properties { _Color ("Colour", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _Color;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 colour : COLOR; };
            struct v2f { float4 position : SV_POSITION; float2 uv : TEXCOORD0; float4 colour : COLOR; };
            v2f vert(appdata v) { v2f o; o.position=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.colour=v.colour*_Color; return o; }
            fixed4 frag(v2f i) : SV_Target
            {
                float radius=length(i.uv*2-1); float alpha=saturate((1-radius)*8);
                float ring=exp(-pow((radius-.65)*8,2));
                return fixed4(i.colour.rgb*(.5+ring*.5),i.colour.a*alpha);
            }
            ENDCG
        }
    }
}
