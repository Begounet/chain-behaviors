using ScriptableObjectArchitecture;

namespace ChainBehaviors.Audio
{
    [System.Serializable]
    public sealed class AudioListenerCaptureControllerReference : BaseReference<AudioListenerCaptureController, AudioListenerCaptureControllerVariable, AudioListenerCaptureControllerVariableComponent>
    {
        public AudioListenerCaptureControllerReference() : base() { }
        public AudioListenerCaptureControllerReference(AudioListenerCaptureController value) : base(value) { }
    }
}