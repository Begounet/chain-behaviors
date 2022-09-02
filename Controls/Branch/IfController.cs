using AUE;
using UnityEngine;

namespace ChainBehaviors
{
    /// <summary>
    /// From a boolean, execute true or false then executed in all cases.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "If")]
    public class IfController : BaseMethod
    {
        [SerializeField]
        private AUEEvent _true;

        [SerializeField]
        private AUEEvent _false;

        [SerializeField]
        private AUEEvent<bool> _executed;

        public void Execute(bool value)
        {
            Trace(value);
            if (value)
            {
                _true.Invoke();
            }
            else
            {
                _false.Invoke();
            }
            _executed.Invoke(value);
        }
    }
}
