using ChainBehaviors.Processes;
using ScriptableObjectArchitecture;

namespace ChainBehaviors.SOArchitecture
{
    [System.Serializable]
	public sealed class TargetFramerateUpdateProcessorReference : BaseReference<TargetFramerateUpdateProcess, TargetFramerateUpdateProcessorVariable, TargetFramerateUpdateProcessorVariableComponent>
	{
	    public TargetFramerateUpdateProcessorReference() : base() { }
	    public TargetFramerateUpdateProcessorReference(TargetFramerateUpdateProcess value) : base(value) { }
	}
}