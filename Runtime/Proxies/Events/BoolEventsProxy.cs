#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Bool Events Proxy")]
    public class BoolEventsProxy : BaseEventsProxy
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_executedAUE")]
        private AUEEvent<bool> _executed = null;

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Execute(bool value)
        {
            Trace(value);
            _executed.Invoke(value);
        }
    }
}
