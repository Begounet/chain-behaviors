using AUE;
using UnityEngine;
using ChainBehaviors.Utils;
using TypeCodebase;

namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Event proxy whose the parameter type can be customized.
    /// Pretty cool to use any kind of type without recreating a new events proxy.
    /// However, it will add a little overhead (memory and CPU) so if you can you
    /// should favorize already implemented events proxies.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Generic Events Proxy")]
    public class GenericEventsProxy : BaseMethod, ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializableType _parameterType;

        [SerializeField]
        private CustomizableAUEEvent _executed;


        public void Execute(object arg)
        {
            Trace(("arg", arg));
            _executed.Invoke(arg);
        }

        public void OnBeforeSerialize()
        {
            if (_executed != null && _parameterType != null)
            {
                _executed.DefineParameterTypes(_parameterType.Type);
            }
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
