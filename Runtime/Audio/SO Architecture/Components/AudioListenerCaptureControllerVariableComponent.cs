using ScriptableObjectArchitecture;
using UnityEngine.Events;

namespace ChainBehaviors.Audio
{
    public class AudioListenerCaptureControllerVariableComponent : BaseObservableVariableComponent<AudioListenerCaptureController, AudioListenerCaptureControllerVariableComponent.UnityEvent> 
    {
        [System.Serializable]
        public class UnityEvent : UnityEvent<AudioListenerCaptureController> {}
    }
}