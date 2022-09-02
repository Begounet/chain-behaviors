using UnityEngine;
using AUE;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Color Events Proxy")]
    public class ColorEventsProxy : BaseMethod
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_executedAUE")]
        private AUEEvent<Color> _executed = null;

        public void Execute(Color color)
        {
            Trace(color);
            _executed.Invoke(color);
        }
    }
}