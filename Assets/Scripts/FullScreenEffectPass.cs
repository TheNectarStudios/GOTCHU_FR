using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectRenderer : ScriptableRendererFeature
{
    class FullScreenEffectPass : ScriptableRenderPass
    {
        public Material effectMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle temporaryRT;

        public FullScreenEffectPass(Material material)
        {
            this.effectMaterial = material;
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("FullScreenEffect");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(temporaryRT.id, opaqueDesc);

            Blit(cmd, source, temporaryRT.Identifier(), effectMaterial);
            Blit(cmd, temporaryRT.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryRT.id);
        }
    }

    FullScreenEffectPass fullScreenEffectPass;

    public override void Create()
    {
        fullScreenEffectPass = new FullScreenEffectPass(null);
        fullScreenEffectPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (fullScreenEffectPass.effectMaterial != null)
        {
            fullScreenEffectPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(fullScreenEffectPass);
        }
    }

    public static void SetActiveEffect(Material material)
    {
        FullScreenEffectRenderer rendererFeature = FindObjectOfType<FullScreenEffectRenderer>();
        if (rendererFeature != null)
        {
            rendererFeature.fullScreenEffectPass.effectMaterial = material;
        }
    }

    public static void RemoveActiveEffect()
    {
        FullScreenEffectRenderer rendererFeature = FindObjectOfType<FullScreenEffectRenderer>();
        if (rendererFeature != null)
        {
            rendererFeature.fullScreenEffectPass.effectMaterial = null;
        }
    }
}
