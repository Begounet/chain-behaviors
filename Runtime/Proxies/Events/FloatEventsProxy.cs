#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Float Events Proxy")]
    public class FloatEventsProxy : BaseEventsProxy
    {
        [SerializeField]
        private AUEEvent<float> _executed = null;

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Execute(float value)
        {
            Trace(value);
            _executed.Invoke(value);   
        }
    }
}