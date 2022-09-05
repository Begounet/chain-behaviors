#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using AUE;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Events Proxy")]
    public class EventsProxy : BaseEventsProxy
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_executedAUE")]
        private AUEEvent _executed = null;

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Execute()
        {
            Trace();
            _executed.Invoke();
        }
    }
}