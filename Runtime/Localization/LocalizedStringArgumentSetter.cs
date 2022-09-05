#if USE_UNITY_LOCALIZATION
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Events;
using UnityEngine.Localization;
using System.Collections.Generic;
using AUE;
using ChainBehaviors.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ChainBehaviors.Localization
{
    /// <summary>
    /// Set an argument to a LocalizedString
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleLocalization + "LocalizedString Argument Setter")]
    public class LocalizedStringArgumentSetter : BaseMethod
    {
        [SerializeField]
        private AUEEvent<LocalizedString> _localizeStringEvent;

#pragma warning disable 414 // Used in Editor
        [SerializeField, Tooltip("If false, serialized arguments can be set in edit mode")]
        private bool _isRuntimeOnly = true;
#pragma warning restore

        [SerializeField]
        private int _argumentIndex;

        [SerializeField]
        private AUEEvent _onArgumentSet;


        public void SetArgument(object argument)
        {
            Trace(
                ("argument index", _argumentIndex),
                ("argument", argument));

#if UNITY_EDITOR
            if (!_isRuntimeOnly && !UnityEngine.Application.isPlaying && argument is UnityEngine.Object argumentUObj)
            {
                SerializedObject so = new SerializedObject(_localizeStringEvent);
                SerializedProperty argumentsSP = so.FindProperty("m_FormatArguments");
                if (_argumentIndex >= argumentsSP.arraySize)
                {
                    argumentsSP.arraySize = _argumentIndex + 1;
                }

                SerializedProperty argumentSP = argumentsSP.GetArrayElementAtIndex(_argumentIndex);
                argumentSP.objectReferenceValue = argumentUObj;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
#endif

            if (UnityEngine.Application.isPlaying)
            {
                LocalizedString ls = _localizeStringEvent.StringReference;
                if (ls.Arguments == null)
                {
                    ls.Arguments = new List<object>();
                }

                while (_argumentIndex >= ls.Arguments.Count)
                {
                    ls.Arguments.Add(null);
                }

                ls.Arguments[_argumentIndex] = argument;
            }

            _onArgumentSet.Invoke();
        }
    }
}
#endif