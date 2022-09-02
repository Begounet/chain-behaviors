using AUE;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ChainBehaviors.Events
{
    public class MonoBehaviorLifeCycleEvents : BaseMethod
    {
        [Flags]
        public enum EEventType
        {
            Start = 0x01,
            OnEnable = 0x02,
            OnDisable = 0x04,
            OnDestroy = 0x08,
            OnPreRender = 0x10,
            OnPostRender = 0x20,
        }

        [SerializeField]
        private EEventType _eventsFilter = EEventType.Start;

        [SerializeField, ShowIf("PassStartFilter")]
        private AUEEvent _onStart = null;

        [SerializeField, ShowIf("PassOnEnableFilter")]
        private AUEEvent _onEnable = null;

        [SerializeField, ShowIf("PassOnDisableFilter")]
        private AUEEvent _onDisable = null;

        [SerializeField, ShowIf("PassOnDestroyFilter")]
        private AUEEvent _onDestroy = null;

        [SerializeField, ShowIf("PassOnPreRenderFilter")]
        private AUEEvent _onPreRender = null;

        [SerializeField, ShowIf("PassOnPostRenderFilter")]
        private AUEEvent _onPostRender = null;

        private bool PassStartFilter => _eventsFilter.HasFlag(EEventType.Start);
        private bool PassOnEnableFilter => _eventsFilter.HasFlag(EEventType.OnEnable);
        private bool PassOnDisableFilter => _eventsFilter.HasFlag(EEventType.OnDisable);
        private bool PassOnDestroyFilter => _eventsFilter.HasFlag(EEventType.OnDestroy);
        private bool PassOnPreRenderFilter => _eventsFilter.HasFlag(EEventType.OnPreRender);
        private bool PassOnPostRenderFilter => _eventsFilter.HasFlag(EEventType.OnPostRender);

        private void Start() => FilteredTrigger(EEventType.Start, _onStart);
        private void OnEnable() => FilteredTrigger(EEventType.OnEnable, _onEnable);
        private void OnDisable() => FilteredTrigger(EEventType.OnDisable, _onDisable);
        private void OnDestroy() => FilteredTrigger(EEventType.OnDestroy, _onDestroy);
        private void OnPreRender() => FilteredTrigger(EEventType.OnPreRender, _onPreRender);
        private void OnPostRender() => FilteredTrigger(EEventType.OnPostRender, _onPostRender);

        private void FilteredTrigger(EEventType type, AUEEvent evt)
        {
            if (_eventsFilter.HasFlag(type))
            {
                TraceCustomMethodName($"Event '{type}' triggered.");
                evt.Invoke();
            }
        }
    }
}