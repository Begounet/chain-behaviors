using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.Tools
{
    public class BaseExtractSOReference<TBase, TVariable, TComponent, TVariableReference, TUnityEvent> : MonoBehaviour
        where TBase : new()
        where TVariable : BaseVariable<TBase>
        where TComponent : BaseVariableComponent<TBase>
        where TVariableReference : BaseReference<TBase, TVariable, TComponent>
        where TUnityEvent : UnityEvent<TBase>
    {
        [SerializeField]
        private TVariableReference _source = null;

        [SerializeField]
        private TUnityEvent _extracted = null;


        private void OnEnable()
        {
            _source?.AddListener(ProcessExtraction);
            ProcessExtraction();
        }

        private void ProcessExtraction()
        {
            ProcessExtraction(_source);
        }

        public void ProcessExtraction(TVariableReference source)
        {
            if (source != null && source.IsValueDefined && source.Value != null)
            {
                _extracted.Invoke(source.Value);
            }
        }

        private void OnDisable()
        {
            _source?.RemoveListener(ProcessExtraction);
        }
    }
}