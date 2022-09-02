using UnityEngine;

namespace ChainBehaviors.Processes
{
    public abstract class BaseProcess : BaseMethod
    {
        [SerializeField]
        private bool _isActive = true;
        public bool IsActive { get => _isActive; set => _isActive = value; }
        
        public void ExecuteProcess()
        {
            TraceCustomMethodName("Execute Process");
            UpdateProcess(0.0f);
        }

        public abstract void UpdateProcess(float deltaTime);
    }
}