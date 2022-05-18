using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Rendering;
#endif

namespace Soco.ShaderVariantsStripper
{
    public enum ShaderVariantsDataShaderType
    {
        Vertex = 1,
        Fragment = 2,
        Geometry = 3,
        Hull = 4,
        Domain = 5,
        Surface = 6,
        Count = 7,
        RayTracing = 7,
    }

    public struct ShaderVariantsData
    {
        //ShaderSnipperData
        public ShaderVariantsDataShaderType shaderType;
        public UnityEngine.Rendering.PassType passType;
        public string passName;

        //ShaderCompilerData
        public UnityEngine.Rendering.ShaderKeywordSet shaderKeywordSet;
        public UnityEngine.Rendering.PlatformKeywordSet platformKeywordSet;

        #if UNITY_EDITOR
        public static ShaderVariantsData GetShaderVariantsData(ShaderSnippetData shaderSnippetData, ShaderCompilerData data)
        {
            return new ShaderVariantsData()
            {
                shaderType = (ShaderVariantsDataShaderType)shaderSnippetData.shaderType,
                passType = shaderSnippetData.passType,
                passName = shaderSnippetData.passName,
                shaderKeywordSet = data.shaderKeywordSet,
                platformKeywordSet = data.platformKeywordSet
            };
        }
        #endif
        
    }
}