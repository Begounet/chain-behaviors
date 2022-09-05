#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Int Events Proxy")]
    public class IntEventsProxy : BaseEventsProxy
    {
        [SerializeField]
        private AUEEvent<int> _executed = null;

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Execute(int value)
        {
            Trace(value);
            _executed.Invoke(value);
        }
    }
}