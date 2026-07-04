using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;


#if UNITY_EDITOR
using UnityEditorInternal;
#endif


namespace Myth0Games.LayerFX
{
    public class LayerFXRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] LayerMaskRenderFeatureSettings settings;

        private LayerMaskRenderFeaturePass mainPass;

        private RenderTexture camTargetTexture;
        private RenderTexture colorTexture;
        private RTHandle handle;
        private RTHandle camTargetHandle;

        private Camera hiddenCam;
        private UniversalAdditionalCameraData hiddenCamData;
        private GameObject hiddenCamGO;
        private bool isRendering = false;
        private Action<ScriptableRenderContext, Camera> renderHandler;
        private int lastRenderedFrame = -1;
        private bool validTag;

        //Called every frame to sync the internal camera to the main camera. Calls a resize of the RT if the current dimensions are invalid.
        void SyncCamera(Camera hidden, Camera main)
        {
            hidden.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
            hidden.orthographic = main.orthographic;
            hidden.orthographicSize = main.orthographicSize;
            hidden.fieldOfView = main.fieldOfView;
            hidden.aspect = main.aspect;
            hidden.nearClipPlane = main.nearClipPlane;
            hidden.farClipPlane = main.farClipPlane;
            hidden.layerCullDistances = main.layerCullDistances;

            hidden.usePhysicalProperties = main.usePhysicalProperties;
            hidden.sensorSize = main.sensorSize;
            hidden.lensShift = main.lensShift;
            hidden.focalLength = main.focalLength;

            int w = Mathf.Max(1, main.pixelWidth / Mathf.Max(1, settings.downscaling));
            int h = Mathf.Max(1, main.pixelHeight / Mathf.Max(1, settings.downscaling));

            if (hidden.targetTexture == null ||
                hidden.targetTexture.width != w ||
                hidden.targetTexture.height != h)
            {
                EnsureRenderTarget(w, h);
            }
        }

        //Creates the internal camera and defines basic settings.
        private void ConfigureHiddenCamera()
        {
            hiddenCam.cullingMask = settings.captureLayers;
            hiddenCam.depthTextureMode = DepthTextureMode.None;
            hiddenCam.targetTexture = camTargetTexture;
            hiddenCam.forceIntoRenderTexture = true;
            hiddenCam.enabled = false;
            hiddenCam.clearFlags = CameraClearFlags.SolidColor;
            hiddenCam.backgroundColor = Color.clear;
            hiddenCam.allowHDR = settings.colorBufferFormat == BufferFormatMode.HDR;
            hiddenCam.allowMSAA = false;
            hiddenCam.useOcclusionCulling = false;
            hiddenCam.allowDynamicResolution = false;
            if (hiddenCamData == null)
            {
                hiddenCamData = hiddenCamGO.AddComponent<UniversalAdditionalCameraData>();
            }


            hiddenCamData.SetRenderer(settings.rendererIndex);
            hiddenCamData.renderPostProcessing = false;
            hiddenCamData.renderShadows = false;
            hiddenCamData.requiresDepthTexture = false;
            hiddenCamData.requiresColorTexture = false;
            hiddenCamData.antialiasing = AntialiasingMode.None;
        }

        //Called every frame. Checks to make sure the current frame and camera is valid for rendering. Submits a render request after passing checks.
        private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam == hiddenCam) return;
            if (isRendering) return;
            if (hiddenCam == null) return;
            if (handle == null) return;
            if (hiddenCam.targetTexture == null) return;

            if (!ShouldRender(cam)) return;

            if (Time.frameCount == lastRenderedFrame) return;

            lastRenderedFrame = Time.frameCount;

            isRendering = true;

            try
            {
                SyncCamera(hiddenCam, cam);

                var requestData = new UniversalRenderPipeline.StandardRequest
                {
                    destination = camTargetTexture
                };

                if (RenderPipeline.SupportsRenderRequest(hiddenCam, requestData))
                {
                    RenderPipeline.SubmitRenderRequest(hiddenCam, requestData);

                    //Strip the only the color from the rendered layers for importing into Render Graph.
                    CommandBuffer cmd = CommandBufferPool.Get();
                    Blitter.BlitCameraTexture(cmd, camTargetHandle, handle);
                    ctx.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            finally
            {
                isRendering = false;
            }
        }

        private bool IsCameraTagValid()
        {
#if UNITY_EDITOR
            return Array.Exists(InternalEditorUtility.tags, t => t == settings.cameraTag);
#else
        return true;
#endif
        }

        public override void Create()
        {
            if (settings == null) return;

            for (int i = 0; i < settings.materials.Count; i++)
            {
                if (settings.materials[i] == null)
                {
                    Debug.LogWarning("[LayerFX] Material is null at index " + i + " on Render Feature with pass name " + settings.passName + "!");
                }
            }

            if (settings.renderTexture != null)
            {
                SetUpManualRendering();
                return;
            }

            validTag = IsCameraTagValid();

            if (!validTag)
            {
                Debug.LogWarning("Please enter a valid tag!");
                return;
            }

            var w = Mathf.Max(1, Screen.width / Mathf.Max(1, settings.downscaling));
            var h = Mathf.Max(1, Screen.height / Mathf.Max(1, settings.downscaling));

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
                hiddenCamData = null;
            }

            EnsureRenderTarget(w, h);

            if (mainPass != null)
            {
                mainPass.renderPassEvent = settings.renderingOrder;
            }

            hiddenCamGO = new GameObject("HiddenCamGO") { hideFlags = HideFlags.HideAndDontSave };
            hiddenCam = hiddenCamGO.AddComponent<Camera>();

            ConfigureHiddenCamera();
            renderHandler = OnBeginCameraRendering;
            RenderPipelineManager.beginCameraRendering += renderHandler;
        }

        //Reallocates the RT if nessecary.
        private void EnsureRenderTarget(int width, int height)
        {
            bool needsRealloc =
                handle == null ||
                handle.rt == null ||
                !handle.rt.IsCreated() ||
                handle.rt.width != width ||
                handle.rt.height != height;


            if (needsRealloc)
            {
                handle?.Release();
                handle = null;

                camTargetHandle?.Release();
                camTargetHandle = null;

                GraphicsFormat format = settings.colorBufferFormat switch
                {
                    BufferFormatMode.LDR => GraphicsFormat.R8G8B8A8_SRGB,
                    BufferFormatMode.HDR => GraphicsFormat.B10G11R11_UFloatPack32,
                    _ => GraphicsFormat.B10G11R11_UFloatPack32
                };

                if (camTargetTexture != null)
                {
                    camTargetTexture.Release();
                    SafeDestroy(camTargetTexture);
                    camTargetTexture = null;
                }

                if (colorTexture != null)
                {
                    colorTexture.Release();
                    SafeDestroy(colorTexture);
                    colorTexture = null;
                }

                camTargetTexture = new RenderTexture(width, height, 24, format);
                camTargetTexture.depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;
                camTargetTexture.Create();

                var desc = new RenderTextureDescriptor(width, height)
                {
                    graphicsFormat = format,
                    depthStencilFormat = GraphicsFormat.None,
                    msaaSamples = 1
                };

                colorTexture = new RenderTexture(desc);
                colorTexture.Create();

                camTargetTexture.name = "_LayerFXCameraTarget";
                colorTexture.name = "_LayerFXColorTexture";

                camTargetTexture.filterMode = settings.filterMode;
                colorTexture.filterMode = settings.filterMode;

                camTargetTexture.wrapMode = TextureWrapMode.Clamp;
                colorTexture.wrapMode = TextureWrapMode.Clamp;

                camTargetHandle = RTHandles.Alloc(camTargetTexture);
                handle = RTHandles.Alloc(colorTexture);

                if (mainPass == null)
                    mainPass = new LayerMaskRenderFeaturePass(settings, handle);
                else
                {
                    mainPass.SetHandle(handle);
                    mainPass.InvalidateRTInfo();
                }


                if (hiddenCam != null)
                    hiddenCam.targetTexture = camTargetTexture;

            }
        }

        //Sets up rendering from a RT inputted by user
        private void SetUpManualRendering()
        {

            if (settings.renderTexture != null && settings.renderTexture.depthStencilFormat == GraphicsFormat.None)
            {
                Debug.LogWarning("[LayerFX] Assigned Render Texture has no depth/stencil format. Please set Depth Stencil Format to a value other than None.");
            }



            var src = settings.renderTexture;

            var desc = new RenderTextureDescriptor(
                src.width,
                src.height,
                src.graphicsFormat,
                0);

            desc.depthStencilFormat = GraphicsFormat.None;

            colorTexture = new RenderTexture(desc);
            colorTexture.Create();

            handle = RTHandles.Alloc(colorTexture);
            camTargetHandle = RTHandles.Alloc(settings.renderTexture);

            mainPass = new LayerMaskRenderFeaturePass(settings, handle);
            mainPass.InvalidateRTInfo();
            mainPass.renderPassEvent = settings.renderingOrder;
        }




        private bool ShouldRender(Camera cam)
        {

#if UNITY_EDITOR
            if (cam.cameraType == CameraType.SceneView)
                return settings.renderInSceneView && !Application.isPlaying;
#endif

            if (cam.cameraType != CameraType.Game)
                return false;

            return cam.CompareTag(settings.cameraTag) && validTag;

        }

        protected override void Dispose(bool disposing)
        {
            if (renderHandler != null)
            {
                RenderPipelineManager.beginCameraRendering -= renderHandler;
                renderHandler = null;
            }

            //Releasing the handle here prevents recursive rendering and bleeding artifacts
            handle?.Release();
            handle = null;

            camTargetHandle?.Release();
            camTargetHandle = null;

            if (hiddenCamGO != null)
            {
                SafeDestroy(hiddenCamGO);
                hiddenCamGO = null;
                hiddenCam = null;
                hiddenCamData = null;
            }

            if (camTargetTexture != null)
            {
                camTargetTexture.Release();
                SafeDestroy(camTargetTexture);
                camTargetTexture = null;
            }

            if (colorTexture != null)
            {
                colorTexture.Release();
                SafeDestroy(colorTexture);
                colorTexture = null;
            }
        }

        void SafeDestroy(UnityEngine.Object obj)
        {
            if (!obj)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (obj)
                        DestroyImmediate(obj);
                };
                return;
            }
#endif

            Destroy(obj);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {

            if (handle == null)
                return;

            if (handle.rt == null)
                return;

            if (settings.renderTexture != null)
            {
                if (camTargetHandle == null)
                    return;

                if (camTargetHandle.rt == null)
                    return;
            }

            if (settings == null || mainPass == null) return;

            if (settings.renderTexture != null)
            {
                //Strip only the color from the attached render texture for importing into Render Graph.

                //Cameras must render to RTs with a depth attachment,
                //but RTs imported into Render Graph can only have either depth or color, making this step nessecary in the custom rendering pipeline.
                //Here it is executed directly from Graphics because ScriptableRenderingContext is not accessible in this method.
                CommandBuffer cmd = CommandBufferPool.Get();
                Blitter.BlitCameraTexture(cmd, camTargetHandle, handle);
                Graphics.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

            }
            else if (!ShouldRender(renderingData.cameraData.camera)) return;

            renderer.EnqueuePass(mainPass);
        }

        [Serializable]
        public class LayerMaskRenderFeatureSettings
        {
            [Tooltip("The name the pass will be logged with. \n\nEach material selected will create a new pass called <Pass Name>_0, <Pass Name>_1, etc.")]
            public string passName = "LayerFXRenderPass";

            [Tooltip("Determines which renderer asset to render the render feature with. \n\nFind your project's list of renderer assets from the Renderer List field in your URP asset.")]
            public int rendererIndex = 0;

            [Tooltip("Where the pass will be enqueued in the rendering pipeline." +
                "\n\nSelect BeforeRenderingOpaques to render behind the main camera. \nSelect AfterRenderingTransparents to render on top of the main camera.")]
            public RenderPassEvent renderingOrder = RenderPassEvent.BeforeRenderingOpaques;

            [Tooltip("Select a custom render texture here to render manually from your own camera. \n\nRecommended for including post-processing effects." +
                "\n\nLeave this field blank to render from the selected Layer Mask.")]
            public RenderTexture renderTexture;

            [Tooltip("The material used to render the selected layers.\nSelect any fullscreen shader that reads from _BlitTexture." +
                "\n\nTo learn more about recommended settings for custom materials, please visit the documentation.")]
            public List<Material> materials = new List<Material>();

            [Tooltip("The Layer Mask to render with the selected material(s).\n\nFor best results, remove these layers from the Main Camera's culling mask." +
                "\n\nIf Render Texture is set to a value other than (None), then this field will be ignored.")]
            public LayerMask captureLayers;

            [Tooltip("Downscaling drastically improves performance, but reduces image quality.")]
            [Range(1, 8)] public int downscaling = 1;

            [Tooltip("Render the selected layers with the selected materials in Scene View. For best results, remove your selected layers from the Scene View culling mask.")]
            public bool renderInSceneView = false;

            [Tooltip("Selects the color format the render feature will render with.\n\nMatch Camera will automatically select the project's current color format." +
                "\n\nForce LDR will render using R8G8B8A8_SRGB for low-end devices.\n\nForce HDR will render using B10G11R11_UFloatPack32 for higher-end devices.")]
            public BufferFormatMode colorBufferFormat = BufferFormatMode.HDR;

            public FilterMode filterMode = FilterMode.Bilinear;

            [Tooltip("The internal camera will sync itself to the first camera it finds with this tag.")]
            public string cameraTag = "MainCamera";
        }
        public enum BufferFormatMode { LDR, HDR }



        class LayerMaskRenderFeaturePass : ScriptableRenderPass
        {
            private RTHandle rt;
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

            public void SetHandle(RTHandle newHandle)
            {
                rt = newHandle;
                rtInfoDirty = true;
            }

            public void InvalidateRTInfo() => rtInfoDirty = true;

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (rt == null || rt.rt == null || !rt.rt.IsCreated()) return;

                if (settings.materials.Count == 0)
                    return;

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

                var camData = frameData.Get<UniversalCameraData>();
                if (camData.camera.cameraType != CameraType.Game && camData.camera.cameraType != CameraType.SceneView) return;
                if (camData.camera.cameraType == CameraType.SceneView)
                {
                    if (!settings.renderInSceneView) return;
                }

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraColor = resourceData.activeColorTexture;

                TextureHandle sourceHandle = renderGraph.ImportTexture(rt, rtInfo, _importParams);


                GraphicsFormat finalFormat;

                if (settings.colorBufferFormat == BufferFormatMode.LDR)
                {
                    finalFormat = GraphicsFormat.R8G8B8A8_SRGB;
                }
                else
                {
                    finalFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                }

                var tempDesc = new TextureDesc(rt.rt.width, rt.rt.height)
                {
                    name = "BlurTempA",
                    colorFormat = finalFormat,
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

                //For each material inputted by the user, enqueues another render pass.
                for (int i = 0; i < settings.materials.Count; i++)
                {

                    Material material = settings.materials[i];
                    if (material == null)
                    {
                        continue;
                    }



                    TextureHandle destination;

                    if (i == settings.materials.Count - 1)
                    {
                        destination = cameraColor;
                    }
                    else
                    {
                        destination = (i % 2 == 0) ? tempA : tempB;
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
}