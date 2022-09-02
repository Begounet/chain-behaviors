using AppTools;
using UnityEngine;

namespace ChainBehaviors.RefCounting
{
    /// <summary>
    /// Bound to a ref counter, it allows to add/delete a reference on it,
    /// without caring if the reference is already set or not.
    /// More safe than to use a simple number like for the <see cref="RefCounterController"/>.
    /// </summary>
    /// <seealso cref="ObjectRefCounterController"/>
    public class ObjectRefProxyHandler : BaseMethod
    {
        [SerializeField]
        private ObjectRefCounterController _refCounter = null;

        private Handler _handler = null;

        public void AddReference()
        {
            TraceCustomMethodName("Add Reference");
            if (_handler == null)
            {
                _handler = new Handler();
            }

            _refCounter.AddReference(_handler);
        }

        public void RemoveReference()
        {
            TraceCustomMethodName("Remove Reference");
            if (_refCounter.RemoveReference(_handler))
            {
                _handler = null;
            }
        }

        public void SetReference(bool shouldSetReference)
        {
            TraceCustomMethodName("Set Reference", ("set?", shouldSetReference));
            if (shouldSetReference)
            {
                AddReference();
            }
            else
            {
                RemoveReference();
            }
        }
    }
}