using AppTools;
using AUE;
using UnityEngine;

namespace ChainBehaviors.Processes
{
    public class TargetTimeIntervalUpdateProcessor : BaseProcess
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
