#if USE_UNITY_LOCALIZATION
using ChainBehaviors.Utils;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace ChainBehaviors.Localization
{
    /// <summary>
    /// Provide a fast access to the localized string from a <see cref="LocalizeStringEvent"/>.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleLocalization + "Localized String Accessor")]
    public class LocalizeStringAccessor : MonoBehaviour
    {
        [SerializeField]
        private LocalizeStringEvent _localizeStringEvent = null;

        public string String => _localizeStringEvent.StringReference.GetLocalizedString();
    }
}
#endif