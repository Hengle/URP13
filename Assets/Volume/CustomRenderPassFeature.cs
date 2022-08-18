using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        // 这个方法在执行渲染通道之前被调用。
        // 它可以用来配置渲染目标和它们的清除状态。同时创建临时渲染目标纹理。
        // 当此渲染通道为空时，将渲染到活动的相机渲染目标。
        // 你永远不应该调用CommandBuffer.SetRenderTarget. 相反，调用 <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // 渲染管道将确保目标的设置和清除以一种性能的方式发生。
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        //Execute（）是这个类的核心方法，定义我们的执行规则；包含渲染逻辑，设置渲染状态，绘制渲染器或绘制程序网格，调度计算等等。
        // 在这里你可以实现渲染逻辑。
        // 使用 <c>ScriptableRenderContext</c> 来发出绘图命令或执行命令缓冲区
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // 你不需要调用 ScriptableRenderContext.submit, 渲染管道将在管道中的特定点调用它。
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }

        // 清除在执行此呈现过程中创建的所有已分配资源。
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


