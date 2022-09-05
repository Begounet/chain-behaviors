using AUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleEventsProxyPath + "Object Events Proxy")]
    public class ObjectEventsProxy : BaseEventsProxy
    {
        [SerializeField]
        private AUEEvent<Object> _executed;

        public void Execute(Object obj)
        {
            Trace(obj);
            _executed.Invoke(obj);
        }
    }
}
