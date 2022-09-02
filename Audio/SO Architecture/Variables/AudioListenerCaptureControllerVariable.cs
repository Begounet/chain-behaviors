using AppTools.Audio;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors.Audio
{
	[System.Serializable]
	public class AudioListenerCaptureControllerEvent : UnityEvent<AudioListenerCaptureController> { }

	[CreateAssetMenu(
	    fileName = "AudioListenerCaptureControllerVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Audio/Audio Listener Capture",
	    order = 120)]
	public class AudioListenerCaptureControllerVariable : BaseVariable<AudioListenerCaptureController, AudioListenerCaptureControllerEvent>
	{
	}
}