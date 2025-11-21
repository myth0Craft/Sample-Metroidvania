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
        public Material mat2;
        public RenderTexture tex;
        public float blurAmount;
        

    }

    class CustomMaskRenderFeaturePass : ScriptableRenderPass
    {
        private readonly RTHandle rt;
        readonly CustomMaskRenderFeatureSettings settings;

        public CustomMaskRenderFeaturePass(CustomMaskRenderFeatureSettings settings, RTHandle handle)
        {
            this.settings = settings;
            this.rt = handle;
        }

        class PassData
        {
            public TextureHandle source;
            public Material blitMaterial;
            public Material mat2;
            public float blurAmount;
        }



        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            var camData = frameData.Get<UniversalCameraData>();
            if (camData.camera.cameraType != CameraType.Game)
                return;
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;
            //var depth = resourceData.activeDepthTexture;
            TextureHandle sourceHandle;
            if (rt != null)
            {
                sourceHandle = renderGraph.ImportTexture(rt);
            }
            else
            {
                sourceHandle = cameraColor;
            }

            TextureDesc desc = sourceHandle.GetDescriptor(renderGraph);
            desc.name = "BlurTempA";
            desc.clearBuffer = false;
            desc.dimension = TextureDimension.Tex2D;
            desc.useMipMap = false;
            desc.enableRandomWrite = false;
            TextureHandle tempA = renderGraph.CreateTexture(desc);

            var scale = rt.rtHandleProperties.rtHandleScale;
            float texelSizeX = 1.0f / (rt.referenceSize.x * scale.x);
            float texelSizeY = 1.0f / (rt.referenceSize.y * scale.y);

            /*Debug.Log(sourceHandle.GetDescriptor(renderGraph).colorFormat);
            
            Debug.Log(tempA.GetDescriptor(renderGraph).colorFormat);

            Debug.Log(tempA.GetDescriptor(renderGraph).width);
            Debug.Log(sourceHandle.GetDescriptor(renderGraph).width);
            Debug.Log(tempA.GetDescriptor(renderGraph).height);
            Debug.Log(sourceHandle.GetDescriptor(renderGraph).height);*/

            /*using (var builder = renderGraph.AddRasterRenderPass<PassData>(this.GetHashCode().ToString(), out var passData))
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
                    data.blitMaterial.SetVector("_Direction", new Vector2(1, 0));
                    //Vector2 viewportScale = rt.useScaling ? new Vector2(rt.rtHandleProperties.rtHandleScale.x, rt.rtHandleProperties.rtHandleScale.y) : Vector2.one;
                    //Blitter.BlitTexture2D(context.cmd, data.source, viewportScale, 0, true);
                    Blitter.BlitTexture(context.cmd, data.source, Vector2.one, data.blitMaterial, 0);
                });

            }*/


            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass1", out var passData))
            {
                builder.AllowPassCulling(false);

                builder.SetRenderAttachment(tempA, 0, AccessFlags.Write);

                passData.source = sourceHandle;
                passData.blitMaterial = settings.material;
                passData.mat2 = settings.mat2;
                passData.blurAmount = settings.blurAmount;

                builder.UseTexture(passData.source, AccessFlags.Read);


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    //data.blitMaterial.SetTexture("_MainTex", data.source);
                    data.blitMaterial.SetFloat("_BlurAmount", data.blurAmount);
                    data.blitMaterial.SetVector("_Direction", new Vector2(1, 0));
                    //data.blitMaterial.SetVector("_CustomTexelSize", new Vector4(texelSizeX, texelSizeY, 0, 0));
                    //Blitter.BlitTexture2D(context.cmd, data.source, viewportScale, 0, true);
                    Blitter.BlitTexture(context.cmd, data.source, Vector2.one, data.blitMaterial, 0);
                });

            }


            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass2", out var passData))
            {

                builder.AllowPassCulling(false);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                passData.source = tempA;
                passData.blitMaterial = settings.material;
                passData.mat2 = settings.mat2;
                passData.blurAmount = settings.blurAmount;

                builder.UseTexture(passData.source, AccessFlags.Read);


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    //data.blitMaterial.SetTexture("_MainTex", data.source);
                    data.mat2.SetFloat("_BlurAmount", data.blurAmount);
                    data.mat2.SetVector("_Direction", new Vector2(0, 1));
                    //data.mat2.SetVector("_CustomTexelSize", new Vector4(texelSizeX, texelSizeY, 0, 0));
                    Blitter.BlitTexture(context.cmd, data.source, Vector2.one, data.mat2, 0);
                });

            }
        }
    }
}
