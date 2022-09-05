using ChainBehaviors.Processes;
using ScriptableObjectArchitecture;
using UnityEngine.Events;

namespace ChainBehaviors.SOArchitecture
{
    public class TargetFramerateUpdateProcessorVariableComponent : BaseObservableVariableComponent<TargetFramerateUpdateProcess, TargetFramerateUpdateProcessorVariableComponent.UnityEvent> 
    {
        [System.Serializable]
        public class UnityEvent : UnityEvent<TargetFramerateUpdateProcess> {}
    }
}