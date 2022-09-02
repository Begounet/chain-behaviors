#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Diagnostics;
using UnityEngine;

namespace ChainBehaviors
{
    /// <summary>
    /// Base class for all chain behavior methods.
    /// </summary>
    /// <remarks>
    /// <see cref="MethodLogger"/> is a weird class to inherit from,
    /// however, it ensures that the logging system is correctly initialized
    /// at anytime, and stay easy to use without being too much verbose in all
    /// <see cref="BaseMethod"/>'s child classes. 
    /// </remarks>
    public class BaseMethod : MethodLogger
    {
#if ODIN_INSPECTOR
        [PropertyOrder(1001)]
#if !CHAINBEHAVIOR_METHOD_TRACE
        [HideInInspector]
#endif
#endif
        [SerializeField, Tooltip("If true, the attached IDE will break when Trace is called.")]
        private bool _debugBreak = false;

        protected override void Trace(string tagName, params (string key, object value)[] args)
        {
            base.Trace(tagName, args);
            if (_debugBreak)
            {
                Debugger.Break();
            }
        }
    }
}