using AppTools;
using AUE;
using UnityEngine;

namespace ChainBehaviors.Processes
{
    public class TargetFramerateUpdateProcessor : BaseProcess
    {
        public int Framerate 
        { 
            get => _framerate.Framerate;
            set => _framerate.Framerate = value;
        }

        [SerializeField]
        private TargetFramerateInvoker _framerate = new TargetFramerateInvoker(60);

        [SerializeField]
        private AUEEvent<float> _update = null;
        
        public override void UpdateProcess(float deltaTime)
        {
            TraceCustomMethodName("Update Process", ("framerate", _framerate.Framerate), ("delta time", deltaTime));
            _framerate.Invoke(deltaTime, (dt) => _update.Invoke(dt));
        }
    }
}
