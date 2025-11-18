using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static Unity.VisualScripting.Member;

public class CustomMaskRenderFeature : ScriptableRendererFeature
{
    [SerializeField] CustomMaskRenderFeatureSettings settings;
    [SerializeField] RenderPassEvent pass;
    CustomMaskRenderFeaturePass m_ScriptablePass;
    private RTHandle handle;
    /// <inheritdoc/>
    public override void Create()
    {
        handle = RTHandles.Alloc(
        settings.tex
        );

        m_ScriptablePass = new CustomMaskRenderFeaturePass(settings, handle);

        if (pass != null) {
            m_ScriptablePass.renderPassEvent = pass;
        } else 
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

        
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    [Serializable]
    public class CustomMaskRenderFeatureSettings
    {
        public Material material;
        public RenderTexture tex;
        public float blurAmount;
        

    }

    class CustomMaskRenderFeaturePass : ScriptableRenderPass
    {
        private readonly RTHandle handle;
        readonly CustomMaskRenderFeatureSettings settings;

        public CustomMaskRenderFeaturePass(CustomMaskRenderFeatureSettings settings, RTHandle handle)
        {
            this.settings = settings;
            this.handle = handle;
        }

        class PassData
        {
            public TextureHandle source;
            public Material blitMaterial;
            public float blurAmount;
        }



        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            var camData = frameData.Get<UniversalCameraData>();
            if (camData.camera.cameraType != CameraType.Game)
                return;
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;
            RTHandle rt = RTHandles.Alloc(settings.tex);
            TextureHandle sourceHandle = renderGraph.ImportTexture(handle);
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Blur Blit Pass", out var passData))
            {
                builder.SetRenderAttachment(cameraColor, 0);

                passData.source = sourceHandle;
                passData.blitMaterial = settings.material;
                passData.blurAmount = settings.blurAmount;

                builder.UseTexture(passData.source);
                


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    
                    data.blitMaterial.SetTexture("_MainTex", data.source);
                    data.blitMaterial.SetFloat("_BlurAmount", data.blurAmount);
                    Vector2 viewportScale = rt.useScaling ? new Vector2(rt.rtHandleProperties.rtHandleScale.x, rt.rtHandleProperties.rtHandleScale.y) : Vector2.one;
                    //Blitter.BlitTexture2D(context.cmd, data.source, viewportScale, 0, true);
                    Blitter.BlitTexture(context.cmd, data.source, viewportScale, data.blitMaterial, 0);

                });
                
            }
        }
    }
}
