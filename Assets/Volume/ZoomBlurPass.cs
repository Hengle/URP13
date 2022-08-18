using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlurPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Render ZoomBlur Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetZoomBlur");
    static readonly int FocusPowerId = Shader.PropertyToID("_FocusPower");
    static readonly int FocusDetailId = Shader.PropertyToID("_FocusDetail");
    static readonly int FocusScreenPositionId = Shader.PropertyToID("_FocusScreenPosition");
    static readonly int ReferenceResolutionXId = Shader.PropertyToID("_ReferenceResolutionX");


    private ZoomBlur zoomBlur;
    private Material zoomBlurMaterial;
    RenderTargetIdentifier currentCameraColorTarget;

    //ZoomBlurPass类的构造函数1个引用
    public ZoomBlurPass(RenderPassEvent evt)
    {
        //renderPassEvent的值为BeforeRenderingPostProcessing
        renderPassEvent = evt;
        //查找我们创建的名字为ZoomBlur的Shader
        var shader = Shader.Find("PostEffect/ZoomBlur");
        //如果为空的话，提示Shader not found，并返回
        if (shader == null)
        {
            Debug.LogError("Shader not found.");
            return;
        }

        //创建材质
        zoomBlurMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    //接着写一个接口，将currentTarget传进去
    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentCameraColorTarget = currentTarget;
    }

    public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination, Downsampling downsampling)
    {
    }

    //紧接着，Execute方法里执行CommandBuffer的方法
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var camera = renderingData.cameraData.camera;
        if (camera.cameraType != CameraType.Game)
            return;
        //环境准备是否创建材质
        if (zoomBlurMaterial == null)
        {
            Debug.LogError("Material not created.");
            return;
        }

        //后效是否生效
        if (!renderingData.cameraData.postProcessEnabled) return;

        //使用VolumeManager.instance.stack的GetComponent方法来获得我们的自定义Volume类的实例
        //并获取里面的属性变量来做具体的后处理。
        var stack = VolumeManager.instance.stack;
        zoomBlur = stack.GetComponent<ZoomBlur>();

        //这个IsActive返回的值，是我们在继承的VolumeComponent类里定义的
        //并非是VolumeComponent组件的启用和禁用决定的。
        if (zoomBlur == null)
        {
            return;
        }

        if (!zoomBlur.IsActive())
        {
            return;
        }

        //然后从命令缓存池中获取一个gl命令缓存，CommandBuffer主要用于收集一系列gl指令，然后之后执行。
        CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);

        Render(cmd, ref renderingData);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;

        //var source = new RenderTargetIdentifier(currentCameraColorTarget, 0, CubemapFace.Unknown, -1);
        //cmd.SetRenderTarget(source);
        cmd.SetRenderTarget(new RenderTargetIdentifier(currentCameraColorTarget, 0, CubemapFace.Unknown, -1));

        // int destination = TempTargetId;
        // var w = cameraData.camera.scaledPixelWidth;
        // var h = cameraData.camera.scaledPixelHeight;
        //
        // //设置
        // zoomBlurMaterial.SetFloat(FocusPowerId, zoomBlur.focusPower.value);
        // zoomBlurMaterial.SetInt(FocusDetailId, zoomBlur.focusDetail.value);
        // zoomBlurMaterial.SetVector(FocusScreenPositionId, zoomBlur.focusScreenPosition.value);
        // zoomBlurMaterial.SetInt(ReferenceResolutionXId, zoomBlur.referenceResolutionX.value);
        // //shader的第一个pass
        // int shaderPass = 0;
        //
        // cmd.SetGlobalTexture(MainTexId, source);
        // //在清理Rnendr Target前，如果存在后处理栈就需要申请一张临时的render texture。我们使用camera buffer的 CommandBuffer.CetTemporaryRT方法来申请这样一张texture。
        // //传入着色器属性ID以及与相机像素尺寸相匹配的纹理宽高。FilterMode、RenderTextureFormat。
        // //(大家可以对应修改下FilterMode，RenderTextureFormat，我们来看下效果)
        // cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        // cmd.Blit(source, destination);
        // cmd.Blit(destination, source, zoomBlurMaterial, shaderPass);
    }
}