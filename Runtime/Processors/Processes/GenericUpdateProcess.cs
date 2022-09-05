using UnityEngine;
using AUE;
using UnityEngine.Serialization;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Processes
{
    /// <summary>
    /// Forward an update, passing by the delta time.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleProcessors + "Generic Update Process")]
    public class GenericUpdateProcess : BaseProcess
    {
        [SerializeField, FormerlySerializedAs("_updateAUE")]
        private AUEEvent<float> _update = null;

        public override void UpdateProcess(float deltaTime)
        {
            TraceCustomMethodName("Update Process", ("deltaTime", deltaTime));

            _update?.Invoke(deltaTime);
        }
    }
}