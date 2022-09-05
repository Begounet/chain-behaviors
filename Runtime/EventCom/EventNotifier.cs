using ChainBehaviors.Utils;
using System;
using UnityEngine;

namespace ChainBehaviors.EventCom
{
    /// <summary>
    /// Raise an C# event when <see cref="Raise"/> is called.
    /// Use <see cref="EventNotifier"/> to subscribe and reroute the event notification.
    /// </summary>
    /// <seealso cref="EventNotifier"/>
    [AddComponentMenu(CBConstants.ModuleEventComPath + "Event Nofitier")]
    public class EventNotifier : BaseMethod
    {
        public event Action OnRaised;

        public void Raise()
        {
            Trace();
            OnRaised?.Invoke();
        }
    }
}
