using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlurRenderFeature : ScriptableRendererFeature
{
    ZoomBlurPass zoomBlurPass;

    // Create()初始化Feature资源
    public override void Create()
    {
        zoomBlurPass = new ZoomBlurPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        zoomBlurPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(zoomBlurPass); //入队
    }
}