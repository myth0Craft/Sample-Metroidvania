using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class BlitFromRenderTexture : ScriptableRendererFeature
{
    [SerializeField] CustomMaskRenderFeatureSettings settings;
    [SerializeField] RenderPassEvent pass;
    CustomMaskRenderFeaturePass m_ScriptablePass;
    private RTHandle handle;
    /// <inheritdoc/>
    public override void Create()
    {
        if (settings.tex != null)
        {
            handle = RTHandles.Alloc(settings.tex);
        }
        else
        {
            handle = null;
        }
        //settings.material = new Material(settings.material);

        m_ScriptablePass = new CustomMaskRenderFeaturePass(settings, handle);

        if (pass != null)
        {
            m_ScriptablePass.renderPassEvent = pass;
        }
        else
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
        //public Material mat2;
        public RenderTexture tex;
        //public float blurAmount;


    }

    class CustomMaskRenderFeaturePass : ScriptableRenderPass
    {
        private readonly RTHandle rt;
        readonly CustomMaskRenderFeatureSettings settings;
        private RenderTargetInfo _rtInfo;
        private ImportResourceParams _importParams;
        private bool _rtInfoDirty = true;

        public CustomMaskRenderFeaturePass(CustomMaskRenderFeatureSettings settings, RTHandle handle)
        {
            this.settings = settings;
            this.rt = handle;
        }

        class PassData
        {
            public TextureHandle source;
            public Material blitMaterial;
        }



        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (rt == null || rt.rt == null || !rt.rt.IsCreated()) return;

            if (_rtInfoDirty)
            {
                _importParams = new ImportResourceParams
                {
                    clearOnFirstUse = false,
                    discardOnLastUse = false
                };
                _rtInfo = new RenderTargetInfo
                {
                    width = rt.rt.width,
                    height = rt.rt.height,
                    volumeDepth = 1,
                    msaaSamples = 1,
                    format = rt.rt.graphicsFormat
                };
                _rtInfoDirty = false;
            }

            var camData = frameData.Get<UniversalCameraData>();
            if (camData.camera.cameraType != CameraType.Game)
                return;
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;
            //var depth = resourceData.activeDepthTexture;

            /*var scale = rt.rtHandleProperties.rtHandleScale;
            float texelSizeX = 1.0f / (rt.referenceSize.x * scale.x);
            float texelSizeY = 1.0f / (rt.referenceSize.y * scale.y);*/


            TextureHandle sourceHandle = renderGraph.ImportTexture(rt, _rtInfo, _importParams);

            var tempDesc = new TextureDesc(rt.rt.width, rt.rt.height)
            {
                name = "BlurTempA",
                colorFormat = GraphicsFormat.R8G8B8A8_SRGB,
                clearBuffer = false,
                clearColor = Color.clear,
                depthBufferBits = DepthBits.None,
                dimension = TextureDimension.Tex2D,
                useMipMap = false,
                enableRandomWrite = false,
                msaaSamples = MSAASamples.None
            };

            TextureHandle tempA = renderGraph.CreateTexture(tempDesc);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlitRenderTexturePass", out var passData))
            {
                builder.AllowPassCulling(true);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);
                passData.source = sourceHandle;
                passData.blitMaterial = settings.material;
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
                });
            }
        }
    }
}