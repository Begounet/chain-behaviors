using System.Diagnostics;
using UnityEngine;

namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Either a direct reference either a proxy to an UnityEngine.Object.
    /// </summary>
    /// <example>
    /// [SerializedField]
    /// private UnityObjectProxyable<AudioSource> _audioSource;
    /// ...
    /// AudioSource audioSource = _audioSource.Value;
    /// float audioSourceTime = _audioSource.time;
    /// </example>
    /// <typeparam name="TObject">Any type, as long it is UnityEngine.Object</typeparam>
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DrawWithUnity]
#endif
    [DebuggerDisplay("{Value} (use proxy:{_useProxy})")]
    [System.Serializable]
    public class UnityObjectProxyable<TObject> 
        where TObject : Object
    {
        [SerializeField]
        private bool _useProxy = false;
        public bool UseProxy 
        {
            get => _useProxy;
            set => _useProxy = value;
        }

        [SerializeField]
        private Object _object;

        [SerializeField]
        private UnityObjectProxy<TObject> _proxy;

        public TObject Value
        {
            get => (_useProxy ? _proxy.Proxy : _object as TObject);
            set
            {
                if (_useProxy)
                {
                    _proxy.Proxy = value;
                }
                else
                {
                    _object = value;
                }
            }
        }

        public UnityObjectProxyable() { }

        public UnityObjectProxyable(TObject obj)
        {
            _object = obj;
            _useProxy = false;
        }

        public void SetDirectValue(Object value)
        {
            _object = value;
            _useProxy = false;
        }

        public static implicit operator TObject(UnityObjectProxyable<TObject> obj) => obj.Value;
    }
}
