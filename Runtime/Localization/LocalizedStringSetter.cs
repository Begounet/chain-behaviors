#if USE_UNITY_LOCALIZATION
using AUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace ChainBehaviors
{
    public class LocalizedStringSetter : BaseMethod
    {
        private static readonly LocalizedString EmptyMessage = new LocalizedString();

        [SerializeField]
        private AUEEvent<LocalizedString> _setLocalizeString;

        [SerializeField]
        private AUEEvent _onEmptyTextSet;

        public void Set(LocalizedString str)
        {
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