using ChainBehaviors.Proxy;
using System;
using UnityEditor;
using UnityEngine;

namespace ChainBehaviors
{
    [CustomPropertyDrawer(typeof(UnityObjectProxy<>), useForChildren: true)]
    public class BaseUnityObjectPropertyDrawer : PropertyDrawer
    {
        private const string ProxySPName = "_proxy";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type parameterType = fieldInfo.FieldType.GetGenericArguments()[0];
            var proxySP = property.FindPropertyRelative(ProxySPName);
            var proxyType = typeof(IProxyObjectHolder<>);
            proxyType = proxyType.MakeGenericType(parameterType);
            if (proxyType != null)
            {
                proxySP.objectReferenceValue = EditorGUI.ObjectField(position, label, proxySP.objectReferenceValue, proxyType, allowSceneObjects: true);
            }
        }
    }
}