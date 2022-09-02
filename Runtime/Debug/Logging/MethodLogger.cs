using Sirenix.OdinInspector;
using System;
using System.Diagnostics;
using System.Text;

using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.Pool;
using AppTools;

namespace ChainBehaviors
{
    /// <summary>
    /// Utility to log trace on chain behaviors methods.
    /// </summary>
    /// <remarks>Define "CHAINBEHAVIOR_METHOD_TRACE" to be able to log/></remarks>
    /// <remarks>Define "CHAINBEHAVIOR_METHOD_FORCE_LOG" to force all MethodLogger to log their trace.</remarks>
    public class MethodLogger : MonoBehaviour
    {
        private const string MethodsTraceDefine = "CHAINBEHAVIOR_METHOD_TRACE";

        /// <summary>
        /// It is the responsibility of the author's method implementer to use <see cref="Log"/>.  
        /// </summary>
#if !CHAINBEHAVIOR_METHOD_TRACE
        [HideInInspector]
#endif
        [PropertyOrder(1000)]
        [SerializeField, Tooltip("If true, the executed method will be logged.")]
        private bool _logTrace = false;

        private string TypeName => GetType().Name;
        private string OwnerHierarchyPath => transform.GetHierarchyPath();

        [Conditional(MethodsTraceDefine)]
        protected void Log(string message)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif
            Debug.Log(message, this);
        }

        [Conditional(MethodsTraceDefine)]
        protected void LogLines(params string[] messages)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif

            Log(string.Join(Environment.NewLine, messages));
        }

        protected virtual void Trace(string tagName, params (string key, object value)[] args)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif

            var sb = UnsafeGenericPool<StringBuilder>.Get();
            sb.Clear();
            sb.AppendLine(tagName);
            sb.AppendLine(OwnerHierarchyPath);
            if (args.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("<color=#e3763b>~ Details ~</color>");
                foreach ((string key, object value) in args)
                {
                    if (value != null && value is UnityEngine.Object unityObjValue)
                    {
                        sb.AppendLine($"<color=#67a8e0>{key}</color>: {GenerateUnityObjHyperlink(unityObjValue)}");
                    }
                    else
                    {
                        sb.AppendLine($"<color=#67a8e0>{key}</color>: <color=#55e69d>{value}</color>");
                    }
                }
            }
            Log(sb.ToString());
            UnsafeGenericPool<StringBuilder>.Release(sb);
        }

        private static string GenerateUnityObjHyperlink(UnityEngine.Object unityObjValue)
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(unityObjValue);
            return $"{unityObjValue.ToString()} ({path})";
#else
            return unityObjValue.ToString();
#endif
        }

        [Conditional(MethodsTraceDefine)]
        protected void Trace(params (string key, object value)[] args)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif
            Trace($"{gameObject.name}({TypeName})", args);
        }

        [Conditional(MethodsTraceDefine)]
        protected void Trace(object argument)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif
            Trace(("value", argument));
        }

        [Conditional(MethodsTraceDefine)]
        protected void TraceCustomMethodName(string customMethodName, params (string key, object value)[] args)
        {
#if !CHAINBEHAVIOR_METHOD_FORCE_LOG
            if (!_logTrace) return;
#endif
            Trace($"{gameObject.name}({TypeName}):{customMethodName}", args);
        }
    }
}