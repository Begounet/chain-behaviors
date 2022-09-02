using ChainBehaviors.Processes;
using ScriptableObjectArchitecture;

namespace ChainBehaviors.SOArchitecture
{
    [System.Serializable]
	public sealed class TargetFramerateUpdateProcessorReference : BaseReference<TargetFramerateUpdateProcessor, TargetFramerateUpdateProcessorVariable, TargetFramerateUpdateProcessorVariableComponent>
	{
	    public TargetFramerateUpdateProcessorReference() : base() { }
	    public TargetFramerateUpdateProcessorReference(TargetFramerateUpdateProcessor value) : base(value) { }
	}
}