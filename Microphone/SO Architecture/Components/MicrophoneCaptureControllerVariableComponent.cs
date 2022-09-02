using UnityEngine.Events;
using ScriptableObjectArchitecture;
using ChainBehaviors.Microphone;

namespace ChainBehaviors.ScriptableObject
{
    public class MicrophoneCaptureControllerVariableComponent : BaseObservableVariableComponent<MicrophoneCaptureController, MicrophoneCaptureControllerVariableComponent.UnityEvent> 
    {
        [System.Serializable]
        public class UnityEvent : UnityEvent<MicrophoneCaptureController> {}
    }
}