using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class LayerMaskRenderFeature : ScriptableRendererFeature
{
    [SerializeField] LayerMaskRenderFeatureSettings settings;

    private LayerMaskRenderFeaturePass mainPass;

    private RTHandle handle;
    private RenderTexture tex;
    private Camera hiddenCam;
    private GameObject hiddenCamGO;
    private bool isRendering = false;
    private Action<ScriptableRenderContext, Camera> renderHandler;
    private int lastRenderedFrame = -1;

    void SyncCamera(Camera hidden, Camera main)
    {
        hidden.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
        hidden.orthographic = main.orthographic;
        hidden.orthographicSize = main.orthographicSize;
        hidden.fieldOfView = main.fieldOfView;
        hidden.aspect = main.aspect;
        hidden.nearClipPlane = main.nearClipPlane;
        hidden.farClipPlane = main.farClipPlane;
        hidden.aspect = main.aspect;

        if (hidden.targetTexture != null &&
            (hidden.targetTexture.width != main.pixelWidth || hidden.targetTexture.height != main.pixelHeight))
        {
            hidden.targetTexture.Release();
            hidden.targetTexture.width = main.pixelWidth / settings.downscaling;
            hidden.targetTexture.height = main.pixelHeight / settings.downscaling;
            hidden.targetTexture.Create();
        }
    }

    private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
    {


        if (cam == hiddenCam) return;

        bool isGameCamera =
            cam.cameraType == CameraType.Game &&
            cam.CompareTag("MainCamera");

        bool isSceneView =
            cam.cameraType == CameraType.SceneView;

        if (!isGameCamera && !isSceneView)
            return;

        if (isSceneView)
        {
            if (settings.renderInSceneView)
            {
                cam.cullingMask = ~settings.hiddenLayers;
            }
            else
            {
                return;
            }
        }

        if (isRendering) return;
        if (hiddenCam == null) return;
        if (handle == null) return;
        if (hiddenCam.targetTexture == null) return;
        if (Time.frameCount == lastRenderedFrame) return;
        lastRenderedFrame = Time.frameCount;



        hiddenCam.cullingMask = settings.captureLayers;

        isRendering = true;
        try
        {
            SyncCamera(hiddenCam, cam);

            var request = new UniversalRenderPipeline.SingleCameraRequest();
            request.destination = hiddenCam.targetTexture;
            RenderPipeline.SubmitRenderRequest(hiddenCam, request);
        }
        finally
        {
            isRendering = false;
        }
    }



    public override void Create()
    {
        if (settings.tex != null)
        {
            SetUpManualRendering();
            return;
        }

        var w = Mathf.Max(1, Screen.width / settings.downscaling);
        var h = Mathf.Max(1, Screen.height / settings.downscaling);

        bool needsRealloc = tex == null || !tex.IsCreated() || tex.width != w || tex.height != h;

        if (renderHandler != null)
        {
            RenderPipelineManager.beginCameraRendering -= renderHandler;
            renderHandler = null;
        }

        if (hiddenCamGO != null || hiddenCam != null)
        {
            SafeDestroy(hiddenCamGO);
            hiddenCamGO = null;
            hiddenCam = null;
        }

        if (needsRealloc)
        {
            handle?.Release();
            handle = null;



            if (tex != null) { tex.Release(); tex = null; }

            var desc = new RenderTextureDescriptor(w, h, RenderTextureFormat.ARGB32, 0)
            {
                depthBufferBits = 0,
                depthStencilFormat = GraphicsFormat.None,
                msaaSamples = 1,
                sRGB = true
            };
            tex = new RenderTexture(desc) { name = $"_{settings.passName}_{System.Guid.NewGuid()}" };

            RenderingUtils.ReAllocateHandleIfNeeded(ref handle,
                Vector2.one / settings.downscaling,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_CustomMaskTex");


            mainPass = new LayerMaskRenderFeaturePass(settings, handle);
            mainPass.InvalidateRTInfo();
        }

        if (mainPass != null)
        {
            mainPass.renderPassEvent = settings.renderingOrder;
        }




        hiddenCamGO = new GameObject("HiddenCamGO") { hideFlags = HideFlags.HideAndDontSave };
        hiddenCam = hiddenCamGO.AddComponent<Camera>();
        hiddenCam.cullingMask = settings.captureLayers;
        hiddenCam.depthTextureMode = DepthTextureMode.None;
        hiddenCam.targetTexture = handle.rt;
        hiddenCam.forceIntoRenderTexture = true;
        hiddenCam.enabled = false;
        hiddenCam.clearFlags = CameraClearFlags.SolidColor;
        hiddenCam.backgroundColor = new Color(0, 0, 0, 0);
        hiddenCam.allowHDR = false;
        hiddenCam.allowMSAA = false;
        hiddenCam.useOcclusionCulling = false;
        hiddenCam.allowDynamicResolution = false;

        var urpData = hiddenCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        urpData.SetRenderer(settings.rendererIndex);
        urpData.renderPostProcessing = false;
        urpData.renderShadows = false;
        urpData.requiresDepthTexture = false;
        urpData.requiresColorTexture = false;
        urpData.antialiasing = AntialiasingMode.None;

        renderHandler = OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering += renderHandler;
    }

    private void SetUpManualRendering()
    {
        Dispose();

        if (settings.tex != null)
        {
            handle = RTHandles.Alloc(settings.tex);
        }
        else
        {
            handle = null;
        }

        mainPass = new LayerMaskRenderFeaturePass(settings, handle);
        mainPass.InvalidateRTInfo();
        mainPass.renderPassEvent = settings.renderingOrder;

    }


    protected override void Dispose(bool disposing)
    {
        if (renderHandler != null)
        {
            RenderPipelineManager.beginCameraRendering -= renderHandler;
            renderHandler = null;
        }

        handle?.Release();
        handle = null;

        if (tex != null) { tex.Release(); tex = null; }

        if (hiddenCamGO != null)
        {
            SafeDestroy(hiddenCamGO);
            hiddenCamGO = null;
            hiddenCam = null;
        }
    }

    void SafeDestroy(UnityEngine.Object obj)
    {
        if (!obj) return;
        if (Application.isPlaying) Destroy(obj);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (mainPass == null) return;
        renderer.EnqueuePass(mainPass);
    }

    [Serializable]
    public class LayerMaskRenderFeatureSettings
    {
        public string passName = "CustomMaskPass";
        public int rendererIndex = 0;

        [Tooltip("Select BeforeRenderingOpaques to render behind the main camera. \nSelect AfterRenderingTransparents to render on top of the main camera.")]
        public RenderPassEvent renderingOrder = RenderPassEvent.BeforeRenderingOpaques;

        [Tooltip("Select a custom render texture here to render manually from your own camera. \n\nRecommended for post-processing effects.")]
        public RenderTexture tex;

        [Tooltip("The material used to render the selected layers.\nSelect any fullscreen shader that reads from _BlitTexture.")]
        public List<Material> materials = new List<Material>();
        public LayerMask captureLayers;
        public LayerMask hiddenLayers;
        [Range(1, 8)] public int downscaling = 1;
        //public bool renderBehindScene = false;
        public bool renderInSceneView = false;
    }

    class LayerMaskRenderFeaturePass : ScriptableRenderPass
    {
        private readonly RTHandle rt;
        readonly LayerMaskRenderFeatureSettings settings;

        private RenderTargetInfo rtInfo;
        private ImportResourceParams _importParams;
        private bool rtInfoDirty = true;

        public LayerMaskRenderFeaturePass(LayerMaskRenderFeatureSettings settings, RTHandle handle)
        {
            this.settings = settings;
            this.rt = handle;
        }

        class PassData
        {
            public TextureHandle source;
            public Material blitMaterial;
        }

        public void InvalidateRTInfo() => rtInfoDirty = true;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (rt == null || rt.rt == null || !rt.rt.IsCreated()) return;

            if (rtInfoDirty)
            {
                _importParams = new ImportResourceParams
                {
                    clearOnFirstUse = false,
                    discardOnLastUse = false
                };
                rtInfo = new RenderTargetInfo
                {
                    width = rt.rt.width,
                    height = rt.rt.height,
                    volumeDepth = 1,
                    msaaSamples = 1,
                    format = rt.rt.graphicsFormat
                };
                rtInfoDirty = false;
            }

            settings.downscaling = Mathf.Clamp(settings.downscaling, 1, 8);

            var camData = frameData.Get<UniversalCameraData>();
            if (camData.camera.cameraType != CameraType.Game && camData.camera.cameraType != CameraType.SceneView) return;
            if (camData.camera.cameraType == CameraType.SceneView)
            {
                if (!settings.renderInSceneView) return;
            }

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;

            TextureHandle sourceHandle = renderGraph.ImportTexture(rt, rtInfo, _importParams);

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

            TextureHandle tempB = renderGraph.CreateTexture(tempDesc);

            TextureHandle currentSource = sourceHandle;

            for (int i = 0; i < settings.materials.Count; i++)
            {

                TextureHandle destination;

                if (i == settings.materials.Count - 1)
                {
                    destination = cameraColor;
                }
                else
                {
                    destination = (i % 2 == 0) ? tempA : tempB;
                }

                Material material = settings.materials[i];
                if (material == null)
                {
                    Debug.Log("null material!");
                    break;
                }

                using (var builder = renderGraph.AddRasterRenderPass<PassData>($"{settings.passName}_{i}", out var passData))
                {

                    passData.blitMaterial = material;
                    builder.AllowPassCulling(true);
                    builder.SetRenderAttachment(destination, 0, AccessFlags.Write);
                    passData.source = currentSource;
                    builder.UseTexture(passData.source, AccessFlags.Read);


                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
                    });
                }

                currentSource = destination;
            }
        }
    }
}