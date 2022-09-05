#if USE_UNITY_LOCALIZATION
using AUE;
using ChainBehaviors.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ChainBehaviors.Localization
{
    /// <summary>
    /// Allow to act on the Localization system (like changing localization culture)
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleLocalization + "Localization Controller")]
    public class LocalizationController : BaseMethod
    {
        [SerializeField]
        private AUEEvent _onLocalizationCultureChanged;

        private bool _lockLocaleChangedNotification = false;

        private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        private void OnLocaleChanged(Locale _)
        {
            if (!_lockLocaleChangedNotification)
            {
                RaiseLocaleChanged();
            }
        }

        public void SetLocalizationCulture(string cultureId)
            => SetLocalizationCulture(new LocaleIdentifier(cultureId));

        public void SetLocalizationCulture(LocaleIdentifier cultureIdentifier)
            => SetLocalizationCultureAsync(cultureIdentifier).Forget();

        public async UniTask SetLocalizationCultureAsync(LocaleIdentifier cultureIdentifier)
        {
            TraceCustomMethodName("SetLocalizationCultureAsync", 
                ("culture", cultureIdentifier.Code), 
                ("LocalizationSettings.InitializationOperation status", LocalizationSettings.InitializationOperation.Status));

            await LocalizationSettings.InitializationOperation;
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(cultureIdentifier);
            if (locale == null)
            {
                Debug.LogError($"Cannot load culture {cultureIdentifier.Code}");
                return;
            }

            // Block any localization locale change because we force the notification update
            // even if the locale not really changed but has been initialized
            _lockLocaleChangedNotification = true;
            LocalizationSettings.SelectedLocale = locale;
            RaiseLocaleChanged();
            _lockLocaleChangedNotification = false;
        }

        private void RaiseLocaleChanged()
        {
            TraceCustomMethodName("Raise Locale Changed", ("New Locale", LocalizationSettings.SelectedLocale?.LocaleName ?? "<unknown>"));
            _onLocalizationCultureChanged.Invoke();
        }
    }
}
#endif