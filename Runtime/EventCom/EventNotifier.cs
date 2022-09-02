using System;

namespace ChainBehaviors.EventCom
{
    /// <summary>
    /// Raise an C# event when <see cref="Raise"/> is called.
    /// Use <see cref="EventNotifier"/> to subscribe and reroute the event notification.
    /// </summary>
    /// <seealso cref="EventNotifier"/>
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
