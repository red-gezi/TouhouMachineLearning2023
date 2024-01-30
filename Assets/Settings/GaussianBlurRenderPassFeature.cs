using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderPassFeature : ScriptableRendererFeature
{
    static readonly string k_RenderTag = "Render GaussianBlur";                             // 显示在frame debug里的名称
    static readonly int TempTargetId0 = Shader.PropertyToID("_TempTargetColorTint0");       // 用来暂存纹理
    static readonly int TempTargetId1 = Shader.PropertyToID("_TempTargetColorTint1");       // 用来暂存纹理
    public Material postMaterial;                      // 后处理材质

    class GaussianBlurRenderPass : ScriptableRenderPass
    {
        public GaussianBlurParameter parameter;            // 参数类
        public Material postMaterial;                      // 后处理材质
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (postMaterial == null) return;
            if (!renderingData.cameraData.postProcessEnabled) return;  // 相机有没有打开后处理
            var stack = VolumeManager.instance.stack;
            if (parameter == null) return;
            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            void Render(CommandBuffer cmd, ref RenderingData renderingData)
            {
                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                int destination0 = TempTargetId0;
                int destination1 = TempTargetId1;
                ref var cameraData = ref renderingData.cameraData;
                var data = renderingData.cameraData.cameraTargetDescriptor;
                var width = data.width / parameter.downSample.value;
                var height = data.height / parameter.downSample.value;

                postMaterial.SetFloat("_Blur", parameter.blurSpread.value);
                cmd.GetTemporaryRT(destination0, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                cmd.Blit(source, destination0, postMaterial, 0);
                int tempId = 0;
                int targetId = 0;
                for (int i = 0; i < parameter.iterations.value; ++i)
                {
                    tempId = i % 2 == 0 ? destination0 : destination1;
                    targetId = i % 2 == 0 ? destination1 : destination0;
                    cmd.GetTemporaryRT(targetId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                    cmd.Blit(tempId, targetId, postMaterial, 0);
                    cmd.ReleaseTemporaryRT(tempId);
                }
                cmd.Blit(targetId, source);
                cmd.ReleaseTemporaryRT(targetId);
            }
        }
    }
    GaussianBlurRenderPass gaussianBlurPass;
    public RenderPassEvent renderPassEvent;
    /// <inheritdoc/>
    public override void Create()
    {
        var stack = VolumeManager.instance.stack;
        var parameter = stack.GetComponent<GaussianBlurParameter>();
        gaussianBlurPass = new GaussianBlurRenderPass
        {
            renderPassEvent = renderPassEvent,
            parameter = parameter,
            postMaterial = postMaterial,
        };
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(gaussianBlurPass);
    }
}
//挂在Volume上的参数
public class GaussianBlurParameter : VolumeComponent
{
    public ClampedIntParameter iterations = new ClampedIntParameter(3, 0, 8);
    public ClampedFloatParameter blurSpread = new ClampedFloatParameter(0.6f, 0.2f, 3.0f);
    public ClampedIntParameter downSample = new ClampedIntParameter(2, 1, 20);
}