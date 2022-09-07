#if USE_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Graphics
{
    [AddComponentMenu(CBConstants.ModuleGraphicsPath + "Graphics Settings")]
    public class GraphicsSettingsController : BaseMethod
    {
        public void SetMSAASampleCount(int msaaSampleCount)
        {
            TraceCustomMethodName(nameof(SetMSAASampleCount), ("msaa sample count", msaaSampleCount));

            var renderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (renderPipelineAsset != null)
            {
                renderPipelineAsset.msaaSampleCount = msaaSampleCount;
            }
        }
    }
}

#endif