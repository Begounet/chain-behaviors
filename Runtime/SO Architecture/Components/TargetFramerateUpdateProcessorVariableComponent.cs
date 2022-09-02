using ChainBehaviors.Processes;
using ScriptableObjectArchitecture;
using UnityEngine.Events;

namespace ChainBehaviors.SOArchitecture
{
    public class TargetFramerateUpdateProcessorVariableComponent : BaseObservableVariableComponent<TargetFramerateUpdateProcessor, TargetFramerateUpdateProcessorVariableComponent.UnityEvent> 
    {
        [System.Serializable]
        public class UnityEvent : UnityEvent<TargetFramerateUpdateProcessor> {}
    }
}