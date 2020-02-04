using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Normals
{
    sealed class NormalsRenderPass : ScriptableRenderPass
    {
#region Fields
        const string kShader = "Hidden/kNormals/ObjectNormals";
        const string kNormalsTexture = "_NormalsTexture";
        const string kProfilingTag = "Normals";

        static readonly string[] s_ShaderTags = new string[]
        {
            "UniversalForward",
            "LightweightForward",
        };

        RenderTargetHandle m_NormalsHandle;
        Material m_Material;
#endregion

#region Constructors
        internal NormalsRenderPass()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }
#endregion

#region State
        internal void Setup()
        {
            m_Material = new Material(Shader.Find(kShader));
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Configure Render Target
            m_NormalsHandle.Init(kNormalsTexture);
            cmd.GetTemporaryRT(m_NormalsHandle.id, cameraTextureDescriptor, FilterMode.Point);
            ConfigureTarget(m_NormalsHandle.Identifier(), m_NormalsHandle.Identifier());
            cmd.SetRenderTarget(m_NormalsHandle.Identifier(), m_NormalsHandle.Identifier());
                
            // TODO: Why do I have to clear here?
            cmd.ClearRenderTarget(true, true, Color.black, 1.0f);
        }
#endregion

#region Execution
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Get data
            var camera = renderingData.cameraData.camera;

            // Never draw in Preview
            if(camera.cameraType == CameraType.Preview)
                return;

            // Profiling command
            CommandBuffer cmd = CommandBufferPool.Get(kProfilingTag);
            using (new ProfilingSample(cmd, kProfilingTag))
            {
                ExecuteCommand(context, cmd);

                camera.depthTextureMode |= DepthTextureMode.Depth;

                // Drawing
                DrawObjectNormals(context, ref renderingData, cmd, camera);
            }
            ExecuteCommand(context, cmd);
        }

        DrawingSettings GetDrawingSettings(ref RenderingData renderingData)
        {
            // Drawing Settings
            var camera = renderingData.cameraData.camera;
            var sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
            var drawingSettings = new DrawingSettings(ShaderTagId.none, sortingSettings)
            {
                perObjectData = renderingData.perObjectData,
                mainLightIndex = renderingData.lightData.mainLightIndex,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = true,
            };

            // Shader Tags
            for (int i = 0; i < s_ShaderTags.Length; ++i)
            {
                drawingSettings.SetShaderPassName(i, new ShaderTagId(s_ShaderTags[i]));
            }
            
            // Material
            drawingSettings.overrideMaterial = m_Material;
            drawingSettings.overrideMaterialPassIndex = 0;
            return drawingSettings;
        }

        void DrawObjectNormals(ScriptableRenderContext context, ref RenderingData renderingData, CommandBuffer cmd, Camera camera)
        {
            // Get CullingParameters
            var cullingParameters = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out cullingParameters))
                return;

            // Culling Results
            var cullingResults = context.Cull(ref cullingParameters);

            var drawingSettings = GetDrawingSettings(ref renderingData);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, camera.cullingMask);
            var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            
            // Draw Renderers
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
        }
#endregion

#region Cleanup
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            // Reset Render Target
            if (m_NormalsHandle != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(m_NormalsHandle.id);
                m_NormalsHandle = RenderTargetHandle.CameraTarget;
            }
        }
#endregion

#region CommandBufer
        void ExecuteCommand(ScriptableRenderContext context, CommandBuffer cmd)
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
#endregion
    }
}
