using System.Diagnostics;
using UnityEngine;

namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Proxy of an UnityEngine.Object.
    /// Can be bound only to an UnityEngine.Object that implements <see cref="IProxyObjectHolder{T}"/>.
    /// </summary>
    /// <example>
    /// [SerializedField]
    /// private UnityObjectProxy<AudioSource> _audioSourceProxy;
    /// ...
    /// AudioSource audioSource = _audioSourceProxy.Proxy;
    /// </example>
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DrawWithUnity]
#endif
    [DebuggerDisplay("{Proxy}")]
    [System.Serializable]
    public class UnityObjectProxy<TObject> where TObject : Object
    {
        [SerializeField]
        private Object _proxy;

        public TObject Proxy
        {
            get => (_proxy != null ? (_proxy as IProxyObjectHolder<TObject>).Proxy : null);
            set
            {
                if (_proxy != null)
                {
                    (_proxy as IProxyObjectHolder<TObject>).Proxy = value;
                }
            }
        }

        public UnityObjectProxy() { }
        public UnityObjectProxy(TObject value) => _proxy = value;
    }
}
