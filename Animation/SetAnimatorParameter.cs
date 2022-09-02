using UnityEngine;

namespace ChainBehaviors
{
    /// <summary>
    /// Provide a way to feed an Animator parameter via "Submit*" methods
    /// </summary>
    public class SetAnimatorParameter : BaseMethod
    {
        [SerializeField]
        private Animator _animator = null;
        public Animator Animator => _animator;

        [SerializeField]
        private string _parameterName = string.Empty;
        public string ParameterName
        {
            get => _parameterName;
            set
            {
                _parameterName = value;
                BuildParameterHashId();
            }
        }

        private int ParameterHashId { get; set; } = -1;

        private void Awake() => BuildParameterHashId();

        private void BuildParameterHashId()
            => ParameterHashId = Animator.StringToHash(_parameterName);

        public void SubmitFloat(float value)
        {
            TraceCustomMethodName(nameof(SubmitFloat), ("value", value));
            _animator.SetFloat(ParameterHashId, value);
        }

        public void SubmitBool(bool value)
        {
            TraceCustomMethodName(nameof(SubmitBool), ("value", value));
            _animator.SetBool(ParameterHashId, value);
        }

        public void SubmitInt(int value)
        {
            TraceCustomMethodName(nameof(SubmitInt), ("value", value));
            _animator.SetInteger(ParameterHashId, value);
        }

        public void SubmitTrigger()
        {
            TraceCustomMethodName(nameof(SubmitTrigger));
            _animator.SetTrigger(ParameterHashId);
        }
    }
}