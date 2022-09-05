#if USE_UNITY_LOCALIZATION
using AUE;
using ChainBehaviors.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace ChainBehaviors.Localization
{
    /// <summary>
    /// Set a <see cref="LocalizedString"/>.
    /// If string is empty, an other event is also triggered
    /// (so you can hide UI label for example).
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleLocalization + "LocalizedString Setter")]
    public class LocalizedStringSetter : BaseMethod
    {
        private static readonly LocalizedString EmptyMessage = new LocalizedString();

        [SerializeField]
        private AUEEvent<LocalizedString> _setLocalizeString;

        [SerializeField]
        private AUEEvent _onEmptyTextSet;

        public void Set(LocalizedString str)
        {
            Trace();

            str ??= EmptyMessage;
            _setLocalizeString.Invoke(str);
            if (str.IsEmpty)
            {
                _onEmptyTextSet.Invoke();
            }
        }
    }
}
#endif