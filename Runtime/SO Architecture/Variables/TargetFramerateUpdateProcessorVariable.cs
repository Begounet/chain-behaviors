using ChainBehaviors.Processes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors.SOArchitecture
{
	[System.Serializable]
	public class TargetFramerateUpdateProcessorEvent : UnityEvent<TargetFramerateUpdateProcessor> { }

	[CreateAssetMenu(
	    fileName = "TargetFramerateUpdateProcessorVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "ChainBehaviors/Target Framerate Update Processor",
	    order = 120)]
	public class TargetFramerateUpdateProcessorVariable : BaseVariable<TargetFramerateUpdateProcessor, TargetFramerateUpdateProcessorEvent>
	{
	}
}