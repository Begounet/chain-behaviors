using AUE;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChainBehaviors.EventCom
{
    /// <summary>
    /// Subscribe to one single <see cref="EventListener"/> and call <see cref="_onNotificationReceived"/> when event is raised.
    /// Subscription is based on the active state of the MonoBehavior.
    /// </summary>
    /// <seealso cref="EventNotifier"/>
    public class EventListener : BaseMethod
    {
        [SerializeField]
        private EventNotifier _notifier;

        [SerializeField, Tooltip("Auto disable when the event is raised")]
        private bool _listenOnce = false;

        [SerializeField]
        private AUEEvent _onNotificationReceived;

        [ShowInInspector, ReadOnly]
        public bool IsBound { get; private set; }

        private EventNotifier _cachedNotifier;

        private void OnValidate()
        {
            Disable(_cachedNotifier);
            IsBound = false;

            if (this.gameObject.activeInHierarchy && enabled)
            {
                Enable();
            }
        }

        private void OnEnable() => Enable();

        private void OnDisable() => Disable();
        private void OnDestroy() => Disable();

        public void Listen()
        {
            if (!IsBound)
            {
                Enable();
            }
        }

        private void OnExecuted()
        {
            Trace();
            _onNotificationReceived.Invoke();
            if (Application.isPlaying && _listenOnce)
            {
                Disable();
            }
        }

        private void Enable()
        {
            if (_notifier != null)
            {
                IsBound = true;
                _notifier.OnRaised += OnExecuted;
#if UNITY_EDITOR
                _cachedNotifier = _notifier;
#endif
            }
        }

        private void Disable()
        {
            Disable(_notifier);
            IsBound = false;
        }

        private void Disable(EventNotifier notifier)
        {
            if (notifier != null)
            {
                notifier.OnRaised -= OnExecuted;
            }
        }
    }
}
