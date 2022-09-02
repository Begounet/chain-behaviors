#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Helper class to upgrade from a TObject variable to a <see cref="UnityObjectProxyable{TObject}"/> variable during serialization operation.
    /// </summary>
    public static class ObjectProxyUpgrader
    {
        public static UnityObjectProxy<T> FromVariableToProxy<T>(Object owner, T variable)
            where T : Object
        {
            EditorUtility.SetDirty(owner);
            return new UnityObjectProxy<T>(variable);
        }

        public static T FromProxyToVariable<T>(Object owner, UnityObjectProxy<T> variable)
            where T : Object
        {
            EditorUtility.SetDirty(owner);
            return variable.Proxy;
        }

        public static UnityObjectProxyable<T> FromVariableToProxyable<T>(Object owner, T variable) 
            where T : Object
        {
            EditorUtility.SetDirty(owner);
            return new UnityObjectProxyable<T>(variable);
        }

        public static T FromProxyableToVariable<T>(Object owner, UnityObjectProxyable<T> variable)
            where T : Object
        {
            EditorUtility.SetDirty(owner);
            return variable.Value;
        }
    }
}
#endif