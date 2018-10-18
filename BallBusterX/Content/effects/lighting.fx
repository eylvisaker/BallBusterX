#define NUM_LIGHTS 10

float4x4 World;
float4x4 ViewProjection;

texture DiffuseTexture;

float3 AmbientLightColor;


// Light Types:
// 0 - point light
// 1 - directional light
float1 LightType[NUM_LIGHTS];
float3 LightPosition[NUM_LIGHTS];
float3 LightColor[NUM_LIGHTS];
float3 LightAttenuation[NUM_LIGHTS];

sampler texsampler = sampler_state
{
    Texture = <DiffuseTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoords : TEXCOORD0;
    float4 WorldPos : TEXCOORD1;
};

struct PS_RenderOutput
{
    float4 Color : COLOR0;
};

float3 ColorToVector(float3 color);
float3 VectorToColor(float3 v);


////////////////////////////////////////////////////////////////////
//  Standard Vertex Shader
////////////////////////////////////////////////////////////////////

VertexShaderOutput vs_Render(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.WorldPos = mul(input.Position, World);

    output.Position = mul(output.WorldPos, ViewProjection);
    output.TexCoords = input.TexCoords;

    output.Color = input.Color;

    return output;
}

////////////////////////////////////////////////////////////////////
//  Lighting 
////////////////////////////////////////////////////////////////////


float4 ps_WithLights(VertexShaderOutput input) : COLOR0
{
    float4 texel = tex2D(texsampler, input.TexCoords);
    float3 resultColor = AmbientLightColor;

    [unroll] for (uint i = 0; i < NUM_LIGHTS; i++)
    {
        float3 lightVector = LightPosition[i] - input.WorldPos;
        float3 lightDir = normalize(lightVector);
        float lightDistance = length(lightVector);
        float attenuation = 1 /
            (LightAttenuation[i].x
                + LightAttenuation[i].y * lightDistance
                + LightAttenuation[i].z * lightDistance * lightDistance);
        //float normaldot = saturate(dot(lightDir, normal));
        attenuation = saturate(attenuation);

        resultColor += LightColor[i] * attenuation * LightType[i];
    }

    resultColor = saturate(resultColor);
    //resultColor = saturate(LightPosition[0].x * LightAttenuation[0].x * 100000 * resultColor);

    return float4(texel.xyz * resultColor * input.Color.xyz, texel.a * input.Color.a);

}


////////////////////////////////////////////////////////////////////
//  No Lighting
////////////////////////////////////////////////////////////////////

float4 ps_NoLighting(VertexShaderOutput input) : COLOR0
{
    return tex2D(texsampler, input.TexCoords) * input.Color;
}

////////////////////////////////////////////////////////////////////
//  DEBUG SHADERS
////////////////////////////////////////////////////////////////////

//float4 DEBUG_ps_Normal(VertexShaderOutput input) : COLOR0
//{
//    float4 texel = tex2D(texsampler, input.TexCoords);
//    float3 normal = SampleNormal(input.TexCoords);
//    float3 resultColor = AmbientLightColor;
//
//    normal = VectorToColor(normal);
//
//    return float4(normal, texel.a);
//}
//
//float4 DEBUG_ps_LightDir(VertexShaderOutput input) : COLOR0
//{
//    float4 texel = tex2D(texsampler, input.TexCoords);
//    float3 normal = SampleNormal(input.TexCoords);
//    float3 resultColor = AmbientLightColor;
//    float resultAlpha;
//
//    [unroll] for (int i = 0; i < 1; i++)
//    {
//        float3 lightVector = LightPosition[i] - input.WorldPos;
//        float3 lightDir = normalize(lightVector);
//        float lightDistance = length(lightVector);
//        float attenuation = 1 /
//            (LightAttenuation[i].x
//                + LightAttenuation[i].y * lightDistance
//                + LightAttenuation[i].z * lightDistance * lightDistance);
//        float normaldot = saturate(dot(lightDir, normal));
//
//        resultColor = VectorToColor(lightDir);
//
//        resultAlpha = texel.a * attenuation;
//    }
//
//    //resultColor = saturate(resultColor);
//
//    return float4(resultColor, resultAlpha);
//}
//
//float4 DEBUG_ps_LightDistance(VertexShaderOutput input) : COLOR0
//{
//    float4 texel = tex2D(texsampler, input.TexCoords);
//    float3 normal = SampleNormal(input.TexCoords);
//    float3 resultColor = AmbientLightColor;
//    float resultAlpha;
//
//    [unroll] for (int i = 0; i < 1; i++)
//    {
//        float3 lightVector = LightPosition[i] - input.WorldPos;
//        float3 lightDir = normalize(lightVector);
//        float lightDistance = length(lightVector);
//        float attenuation = 1 /
//            (LightAttenuation[i].x
//                + LightAttenuation[i].y * lightDistance
//                + LightAttenuation[i].z * lightDistance * lightDistance);
//
//        return float4(attenuation, attenuation, attenuation, texel.a);
//    }
//}


//////////////////////////////////////////////////////////////////////////
//  Utility Functions
//////////////////////////////////////////////////////////////////////////

float3 ColorToVector(float3 color)
{
    color -= 0.5;
    color *= 2;
    color.y *= -1;
    color.z *= -1;

    return color;
}

float3 VectorToColor(float3 v)
{
    v.z *= -1;
    v /= 2;
    v += 0.5;

    return v;
}

//////////////////////////////////////////////////////////////////////////
//  Techniques
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////
//  Standard lighting shader
//////////////////////////////////////
technique Render
{
    pass Pass1
    {
#if HLSL
        VertexShader = compile vs_4_0 vs_Render();
        PixelShader = compile ps_4_0 ps_WithLights();
#else
        VertexShader = compile vs_3_0 vs_Render();
        PixelShader = compile ps_3_0 ps_WithLights();
#endif
    }
}
//
//technique DebugNormal
//{
//    pass Pass1
//    {
//#if HLSL
//        VertexShader = compile vs_4_0 vs_Render();
//        PixelShader = compile ps_4_0 DEBUG_ps_Normal();
//#else
//        VertexShader = compile vs_3_0 vs_Render();
//        PixelShader = compile ps_3_0 DEBUG_ps_Normal();
//#endif
//    }
//}
//
//technique DebugLightDistance
//{
//    pass Pass1
//    {
//#if HLSL
//        VertexShader = compile vs_4_0 vs_Render();
//        PixelShader = compile ps_4_0 DEBUG_ps_LightDistance();
//#else
//        VertexShader = compile vs_3_0 vs_Render();
//        PixelShader = compile ps_3_0 DEBUG_ps_LightDistance();
//#endif
//    }
//}
//
//technique DebugLightDir
//{
//    pass Pass1
//    {
//#if HLSL
//        VertexShader = compile vs_4_0 vs_Render();
//        PixelShader = compile ps_4_0 DEBUG_ps_LightDir();
//#else
//        VertexShader = compile vs_3_0 vs_Render();
//        PixelShader = compile ps_3_0 DEBUG_ps_LightDir();
//#endif
//    }
//}


// Reference material:


// Phong reflection is ambient + light-diffuse + spec highlights.
// I = Ia*ka*Oda + fatt*Ip[kd*Od(N.L) + ks(R.V)^n]
// Ref: http://www.whisqu.se/per/docs/graphics8.htm
// and http://en.wikipedia.org/wiki/Phong_shading

// Get light direction for this fragment
//float3 lightDir = normalize(input.WorldPos - LightPosition); // per pixel diffuse lighting

// Note: Non-uniform scaling not supported
//float diffuseLighting = saturate(dot(input.Normal, -lightDir));

// Introduce fall-off of light intensity
//diffuseLighting *= (LightDistanceSquared / dot(LightPosition - input.WorldPos, LightPosition - input.WorldPos));

// Using Blinn half angle modification for perofrmance over correctness
//float3 h = normalize(normalize(CameraPos - input.WorldPos) - lightDir);
//float specLighting = pow(saturate(dot(h, input.Normal)), SpecularPower);

//return float4(saturate(
//	AmbientLightColor +
//	(texel.xyz * DiffuseColor * LightDiffuseColor * diffuseLighting * 0.6) + // Use light diffuse vector as intensity multiplier
//	(SpecularColor * LightSpecularColor * specLighting * 0.5) // Use light specular vector as intensity multiplier
//	), texel.w);


