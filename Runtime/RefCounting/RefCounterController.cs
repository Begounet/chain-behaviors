using ChainBehaviors.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors.RefCounting
{
    /// <summary>
    /// Keep reference count and trigger event in consequences
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleRefCounting + "Ref Counter Controller")]
    public class RefCounterController : BaseMethod
    {
        [ShowInInspector, ReadOnly, PropertyOrder(-1)]
        public int ReferencesCount { get; private set; } = 0;

        [SerializeField, PropertyTooltip("Called once when the reference count is zero")]
        private UnityEvent _refCountZero = new UnityEvent();

        [SerializeField, PropertyTooltip("Called once when the reference count is over zero")]
        private UnityEvent _refCountOverZero = new UnityEvent();

#if UNITY_EDITOR
        [ShowInInspector, ListDrawerSettings(DraggableItems = false, IsReadOnly = true), ReadOnly, HideInEditorMode]
        private Queue<string> _stackTraces = new Queue<string>();
#endif

        private void Start()
        {
            // If no reference count has been set during start, call the event
            if (ReferencesCount == 0)
            {
                TraceCustomMethodName("Init Reference (0)");
                _refCountZero.Invoke();
            }
        }

        public void AddReference()
        {
            TraceCustomMethodName("Add Reference");

#if UNITY_EDITOR
            _stackTraces.Enqueue("[Add] " + StackTraceUtility.ExtractStackTrace());
#endif

            ++ReferencesCount;
            if (ReferencesCount == 1)
            {
                TraceCustomMethodName("ref count > zero");
                _refCountOverZero.Invoke();
            }
        }

        public void RemoveReference()
        {
            TraceCustomMethodName("Remove Reference");

            if (ReferencesCount > 0)
            {
#if UNITY_EDITOR
                _stackTraces.Enqueue("[Remove] " + StackTraceUtility.ExtractStackTrace());
#endif

                --ReferencesCount;
                if (ReferencesCount == 0)
                {
                    TraceCustomMethodName("ref count == zero");
                    _refCountZero.Invoke();
                }
            }
        }
    }
}