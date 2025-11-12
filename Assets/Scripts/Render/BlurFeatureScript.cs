using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEditor.AssetImporters;

public class BlurFeatureScript : ScriptableRendererFeature
{
    [SerializeField] BlurFeatureScriptSettings settings;
    BlurFeatureScriptPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new BlurFeatureScriptPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        // You can request URP color texture and depth buffer as inputs by uncommenting the line below,
        // URP will ensure copies of these resources are available for sampling before executing the render pass.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        //m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);

        // You can request URP to render to an intermediate texture by uncommenting the line below.
        // Use this option for passes that do not support rendering directly to the backbuffer.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        //m_ScriptablePass.requiresIntermediateTexture = true;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    // Use this class to pass around settings from the feature to the pass
    [Serializable]
    public class BlurFeatureScriptSettings
    {
        public Material blurMaterial;
        public LayerMask layersToBlur;
    }

    class BlurFeatureScriptPass : ScriptableRenderPass
    {
        readonly BlurFeatureScriptSettings settings;

        public BlurFeatureScriptPass(BlurFeatureScriptSettings settings)
        {
            this.settings = settings;
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            public UniversalResourceData resourceData;
        }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Layered Blur Pass";

            

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                TextureHandle colorHandle = resourceData.activeColorTexture;
                var tempRT = builder.CreateTransientTexture(colorHandle);


                builder.SetRenderAttachment(tempRT, 0);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    if (settings.blurMaterial != null)
                    {
                        Blitter.Cleanup();
                        Blitter.BlitTexture(context.cmd, tempRT, new Vector4(1, 1, 1, 1), settings.blurMaterial, 0);

                    }
                });
            }

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {

                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();


                TextureHandle colorHandle = resourceData.activeColorTexture;
                var tempRT = builder.CreateTransientTexture(colorHandle);

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    if (settings.blurMaterial != null)
                    {
                        Blitter.Cleanup();
                        Blitter.BlitTexture(context.cmd, tempRT, new Vector4(1, 1, 1, 1), settings.blurMaterial, 0);

                    }
                });
            }
        }
    }
}
