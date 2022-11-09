#if USE_UNITY_INPUT_SYSTEM
using AUE;
using ChainBehaviors.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChainBehaviors.InputSystem
{
    [AddComponentMenu(CBConstants.ModuleInputSystemPath + "Input Action Event")]
    public class InputActionEvent : BaseMethod
    {
        [SerializeField]
        private InputActionReference _inputAction;

        [SerializeField]
        private AUEEvent<InputAction.CallbackContext> _onActionStarted;

        [SerializeField]
        private AUEEvent<InputAction.CallbackContext> _onActionPerformed;

        [SerializeField]
        private AUEEvent<InputAction.CallbackContext> _onActionCanceled;

        private void Awake()
        {
            _inputAction.action.started += OnActionPerformed;
            _inputAction.action.performed += OnActionPerformed;
            _inputAction.action.canceled += OnActionPerformed;
        }

        private void OnEnable()
        {
            _inputAction.action.Enable();
        }

        private void OnDisable()
        {
            _inputAction.action.Disable();
        }

        private void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            Trace(("Action", ctx.action.name), ("Phase", ctx.phase));

            if (ctx.performed)
            {
                _onActionPerformed.Invoke(ctx);
            }
            if (ctx.started)
            {
                _onActionStarted.Invoke(ctx);
            }
            if (ctx.canceled)
            {
                _onActionCanceled.Invoke(ctx);
            }
        }
    }
}
#endif