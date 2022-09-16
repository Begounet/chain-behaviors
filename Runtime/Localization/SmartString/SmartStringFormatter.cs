#if USE_UNITY_LOCALIZATION
using AUE;
using ChainBehaviors.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Localization.SmartFormat.Core.Formatting;

namespace ChainBehaviors.Localization.SmartString
{
    /// <summary>
    /// Allow to format a string from one or multiple sources.
    /// Use SmartString under the hood so format must respect it.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleSmartString + "Smart String Formatter")]
    public class SmartStringFormatter : BaseMethod
    {
        [SerializeField, TextArea]
        private string _format = string.Empty;
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                ResetFormatCache();
                RefreshString();
            }
        }

        [SerializeField]
        private UnityEngine.Object[] _args = null;

        private List<object> _arguments = null;
        public List<object> Arguments 
        {
            get => _arguments;
            set => _arguments = value;
        }

        [SerializeField]
        private AUEEvent<string> _updateString = null;

        private FormatCache _formatCache = null;

        private void OnValidate() => ResetFormatCache();

        [ContextMenu("Refresh String")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("Refresh")]
#endif
        private void EditorRefreshString()
        {
            ResetFormatCache();
            RefreshString();
        }

        public void RefreshString()
        {
            string str = string.Empty;

            int argCount = (_args?.Length ?? 0) + (_arguments?.Count ?? 0);
            if (argCount > 0)
            {
                object[] arguments = new object[argCount];
                if (_args != null)
                {
                    for (int i = 0; i < _args.Length; ++i)
                    {
                        arguments[i] = _args[i];
                    }
                }
                if (_arguments != null)
                {
                    for (int i = 0; i < _arguments.Count; ++i)
                    {
                        arguments[i + _args.Length] = _arguments[i];
                    }
                }

                try
                {
                    // Try to use localization formatters since we may added formatter extensions on it
                    if (LocalizationSettings.HasSettings && LocalizationSettings.StringDatabase != null)
                    {
                        IFormatProvider formatProvider = null;
                        if (LocalizationSettings.SelectedLocale != null)
                        {
                            formatProvider = LocalizationSettings.SelectedLocale.Formatter;
                        }
                        str = LocalizationSettings.StringDatabase.SmartFormatter.FormatWithCache(ref _formatCache, _format, formatProvider, arguments);
                    }
                    else
                    {
                        str = Smart.Format(_format, arguments);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                str = _format;
            }

            Trace(("format", Format), ("value", str));

            _updateString.Invoke(str);
        }

        public void AddArgument(object arg)
        {
            _arguments.Add(arg);
        }

        public void SetArgument(object arg)
            => SetArgument(arg, 0);

        public void SetArgument(float arg)
            => SetArgument(arg, 0);

        public void SetArgument(int arg)
            => SetArgument(arg, 0);

        public void SetArgument(string arg)
            => SetArgument(arg, 0);

        public void SetArgument(object arg, int index)
        {
            UpArgumentContainerSize(index + 1);
            _arguments[index] = arg;
        }

        private void UpArgumentContainerSize(int count)
        {
            if (_arguments == null)
            {
                _arguments = new List<object>();
            }

            if (count <= _arguments.Count)
            {
                return;
            }

            _arguments.AddRange(Enumerable.Repeat<object>(null, count - _arguments.Count));
        }

        private void ResetFormatCache() => _formatCache = null;
    }
}
#endif