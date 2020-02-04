using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Normals
{
    sealed class NormalsRendererFeature : ScriptableRendererFeature
    {
#region Fields
        static NormalsRendererFeature s_Instance;
        readonly NormalsRenderPass m_NormalsRenderPass;
#endregion

#region Constructors
        internal NormalsRendererFeature()
        {
            // Set data
            s_Instance = this;
            m_NormalsRenderPass = new NormalsRenderPass();
        }
#endregion

#region Initialization
        public override void Create()
        {
            name = "Normals";
        }
#endregion
        
#region RenderPass
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Motion vector pass
            m_NormalsRenderPass.Setup();
            renderer.EnqueuePass(m_NormalsRenderPass);
        }
#endregion
    }
}
