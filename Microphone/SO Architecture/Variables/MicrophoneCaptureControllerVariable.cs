using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors.Microphone
{
	[System.Serializable]
	public class MicrophoneCaptureControllerEvent : UnityEvent<MicrophoneCaptureController> { }

	[CreateAssetMenu(
	    fileName = "MicrophoneCaptureControllerVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Audio/Microphone Capture Controller",
	    order = 120)]
	public class MicrophoneCaptureControllerVariable : BaseVariable<MicrophoneCaptureController, MicrophoneCaptureControllerEvent>
	{
		public void StartCapture()
		{
			if (Value == null)
			{
				return;
			}
			Value.StartCapture();
		}

		public void StopCapture()
		{
			if (Value == null)
			{
				return;
			}
			
			Value.StopCapture();
		}
	}
}