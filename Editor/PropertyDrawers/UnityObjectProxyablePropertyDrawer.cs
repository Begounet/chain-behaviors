using ChainBehaviors.Proxy;
using UnityEditor;
using UnityEngine;

namespace ChainBehaviors
{
    [CustomPropertyDrawer(typeof(UnityObjectProxyable<>), useForChildren: true)]
    public class UnityObjectProxyablePropertyDrawer : PropertyDrawer
    {
        private const float ModePopupWith = 60;
        private const float SeparationSpace = 2;
        private static readonly string[] ModeOptions = new string[] { "Direct", "Proxy" };

        private const string UseProxySPName = "_useProxy";
        private const string ObjectSPName = "_object";
        private const string ProxySPName = "_proxy";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var paramType = fieldInfo.FieldType.GetGenericArguments()[0];

            Rect valueRect = EditorGUI.PrefixLabel(position, label);
            float toggleButtonWidth = ModePopupWith;
            Rect toggleButtonRect = new Rect(valueRect.x + valueRect.width - toggleButtonWidth + SeparationSpace, valueRect.y, toggleButtonWidth, valueRect.height);
            Rect objectFieldRect = new Rect(valueRect.x, valueRect.y, valueRect.width - (toggleButtonRect.width + SeparationSpace), valueRect.height);

            var useProxySP = property.FindPropertyRelative(UseProxySPName);
            if (useProxySP.boolValue)
            {
                var proxySP = property.FindPropertyRelative(ProxySPName);
                EditorGUI.PropertyField(objectFieldRect, proxySP, GUIContent.none);
            }
            else
            {
                var objSP = property.FindPropertyRelative(ObjectSPName);
                objSP.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, GUIContent.none, objSP.objectReferenceValue, paramType, allowSceneObjects: true);
            }

            useProxySP.boolValue = (EditorGUI.Popup(toggleButtonRect, useProxySP.boolValue ? 1 : 0, ModeOptions) == 1);
        }
    }
}