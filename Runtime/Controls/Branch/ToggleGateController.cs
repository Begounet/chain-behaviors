using UnityEngine;
using AUE;
using System;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Act as a gate. It only call executed if the gate is opened.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Toggle Gate")]
    public class ToggleGateController : BaseMethod
    {
        [SerializeField]
        private bool _isOpen = false;
        public bool IsOpen { get => _isOpen; set => _isOpen = value; }

        [SerializeField]
        private bool _triggerEventAtStart = false;

        [SerializeField]
        private SerializableType _userDataType;

        [SerializeField]
        private AUEEvent<bool> _changed = null;

        [SerializeField]
        private CustomizableAUEEvent _executed;

        private void OnValidate() => UpdateExecutionUserDataType();
        private void Awake() => UpdateExecutionUserDataType();

        private void Start()
        {
            if (_triggerEventAtStart)
            {
                RaiseChangedEvent();
            }
        }

        public void Toggle()
        {
            _isOpen = !_isOpen;
            RaiseChangedEvent();
        }

        public void Execute(object userData)
        {
            if (_isOpen)
            {
                _executed.Invoke(userData);
            }
        }

        private void RaiseChangedEvent()
        {
            TraceCustomMethodName("Changed", ("value", _isOpen));
            _changed.Invoke(_isOpen);
        }

        private void UpdateExecutionUserDataType()
        {
            if (_executed == null || _userDataType == null)
            {
                // May happen when component has just been added
                return;
            }

            _executed.DefineParameterTypes(new Type[] { _userDataType.Type });
        }
    }
}