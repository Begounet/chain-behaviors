using AppTools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors.RefCounting
{
    /// <summary>
    /// Like <see cref="RefCounterController"/> but use handles,
    /// so you cannot add/delete multiple times the same reference.
    /// </summary>
    /// <seealso cref="ObjectRefProxyHandler"/>
    public class ObjectRefCounterController : BaseMethod
    {
        [ShowInInspector, ReadOnly, PropertyOrder(-1)]
        public int ReferencesCount => _handlers.Count;

        [SerializeField, PropertyTooltip("Called once when the reference count is zero")]
        private UnityEvent _refCountZero = new UnityEvent();

        [SerializeField, PropertyTooltip("Called once when the reference count is over zero")]
        private UnityEvent _refCountOverZero = new UnityEvent();

#if UNITY_EDITOR
        [ShowInInspector, ListDrawerSettings(DraggableItems = false, IsReadOnly = true), MultiLineProperty, ReadOnly, HideInEditorMode]
        private Queue<string> _stackTraces = new Queue<string>();
#endif

        private List<Handler> _handlers = new List<Handler>();

        private void Start()
        {
            // If no reference count has been set during start, call the event
            if (ReferencesCount == 0)
            {
                _refCountZero.Invoke();
            }
        }

        public bool AddReference(Handler handler)
        {
            TraceCustomMethodName("Add Reference", ("handler", handler));

            if (_handlers.Contains(handler))
            {
                return false;
            }

            _handlers.Add(handler);
#if UNITY_EDITOR
            _stackTraces.Enqueue($"[Add] Ref count {ReferencesCount}\n{StackTraceUtility.ExtractStackTrace()}");
#endif

            if (ReferencesCount == 1)
            {
                _refCountOverZero.Invoke();
            }

            return true;
        }

        public bool RemoveReference(Handler handler)
        {
            TraceCustomMethodName("Remove Reference", ("handler", handler));

            int index = _handlers.IndexOf(handler);
            if (index >= 0)
            {
                _handlers.RemoveAt(index);
#if UNITY_EDITOR
                _stackTraces.Enqueue($"[Remove] Ref count {ReferencesCount}\n{StackTraceUtility.ExtractStackTrace()}");
#endif
                if (ReferencesCount == 0)
                {
                    _refCountZero.Invoke();
                }
                return true;
            }
            return false;
        }
    }
}