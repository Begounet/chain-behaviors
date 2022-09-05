using AppTools;
using AUE;
using ChainBehaviors.Utils;
using UnityEngine;

namespace ChainBehaviors.Processes
{
    /// <summary>
    /// Execute event each {interval} seconds
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleProcessors + "Target Time Interval Update Process")]
    public class TargetTimeIntervalUpdateProcess : BaseProcess
    {
        [SerializeField]
        private TargetTimeIntervalInvoker _timeIntervalInvoker = new TargetTimeIntervalInvoker(1);

        [SerializeField]
        private AUEEvent<float> _update = null;

        public override void UpdateProcess(float deltaTime)
        {
            TraceCustomMethodName("Update Process", ("interval", _timeIntervalInvoker.Interval), ("delta time", deltaTime));
            _timeIntervalInvoker.Invoke(deltaTime, (dt) => _update.Invoke(dt));
        }
    }
}
