using UnityEngine;
using AUE;

namespace ChainBehaviors
{
    /// <summary>
    /// Execute isValid or isInvalid according if the object == null or not.
    /// Supports correctly <see cref="UnityEngine.Object"/>
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Validity Checker")]
    public class ValidityChecker : BaseMethod
    {
        [SerializeField]
        private AUEEvent<Object> _isValid;

        [SerializeField]
        private AUEEvent<Object> _isInvalid;

        [SerializeField]
        private AUEEvent<bool> _validityChecked;

        public void CheckNull(object obj)
        {
            // Ensure to use the Unity null check if obj is an UnityEngine.Object
            Object unityObj = obj as Object;
            bool isValid = (obj is Object ? unityObj != null : obj != null);
            if (isValid)
            {
                TraceCustomMethodName("isValid", ("object", obj));
                _isValid.Invoke(unityObj);
            }
            else
            {
                TraceCustomMethodName("invalid");
                _isInvalid.Invoke(unityObj);
            }
            TraceCustomMethodName("validity checked");
            _validityChecked.Invoke(isValid);
        }
    }
}