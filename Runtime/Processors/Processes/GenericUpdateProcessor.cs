using UnityEngine;
using AUE;
using UnityEngine.Serialization;

namespace ChainBehaviors.Processes
{
    public class GenericUpdateProcessor : BaseProcess
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