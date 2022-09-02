using ScriptableObjectArchitecture;
using ChainBehaviors.Microphone;

namespace ChainBehaviors.ScriptableObject
{
    [System.Serializable]
	public sealed class MicrophoneCaptureControllerReference : BaseReference<MicrophoneCaptureController, MicrophoneCaptureControllerVariable, MicrophoneCaptureControllerVariableComponent>
	{
	    public MicrophoneCaptureControllerReference() : base() { }
	    public MicrophoneCaptureControllerReference(MicrophoneCaptureController value) : base(value) { }
	}
}