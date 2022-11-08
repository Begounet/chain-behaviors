#if ODIN_INSPECTOR
using AppTools;
using ChainBehaviors.Utils;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ChainBehaviors
{
    /// <summary>
    /// Execute the event only once in an application run.
    /// This data is saved among PlayerPref.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Once Per Application Run")]
    public class OncePerApplicationRun : BaseMethod
    {
        private const string OncePerApplicationRunVariablesPrefKey = "OncePerApplicationRunVariables";

        [Serializable]
        private struct PlayerCache
        {
            public string[] Keys;
        }
        
        public enum EKeyMode
        {
            BasedOnGameObjectPath,
            Constant
        }

        [DetailedInfoBox("Executed once by application run.", "Knowing if it has been executed once is based on a PlayerPref variable. So the key has to been unique in all the application.")]

        [SerializeField, OnValueChanged("CacheKey")]
        [Tooltip("BasedOnGameObjectPath: the key is generated from the GameObject path in hierarchy.\n" +
            "Constant: set your custom key")]
        private EKeyMode _keyMode = EKeyMode.BasedOnGameObjectPath;

        [SerializeField, ShowIf("_keyMode", EKeyMode.Constant), OnValueChanged("CacheKey")]
        private string _key = "Default";
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                ForceCacheKey();
            }
        }

        [SerializeField]
        private UnityEvent _executed = null;

        private string _cachedKey = string.Empty;
        private static bool _hasResetVariables = false;
        private bool _hasBeenExecutedCache = false;

        private void Awake() => CacheKey();

        private void ForceCacheKey()
        {
            _cachedKey = string.Empty;
            CacheKey();
        }

        private void CacheKey()
        {
            if (!string.IsNullOrEmpty(_cachedKey))
            {
                return;
            }

            _hasBeenExecutedCache = false;
            if (_keyMode == EKeyMode.Constant)
            {
                _cachedKey = _key;
            }
            else if (_keyMode == EKeyMode.BasedOnGameObjectPath)
            {
                _cachedKey = this.gameObject.scene.path + "/" + this.transform.GetHierarchyPath();
            }
        }

        public void Execute()
        {
            bool hasBeenExecuted = !HasBeenExecuted();
            Trace(("has already been executed?", hasBeenExecuted));
            if (hasBeenExecuted)
            {
                _hasBeenExecutedCache = true;
                _executed.Invoke();
            }
        }

        private bool HasBeenExecuted()
        {
            if (_hasBeenExecutedCache)
            {
                return true;
            }

            ResetApplicationVariablesIFN();

            CacheKey();
            if (PlayerPrefs.HasKey(OncePerApplicationRunVariablesPrefKey))
            {
                string jData = PlayerPrefs.GetString(OncePerApplicationRunVariablesPrefKey);
                PlayerCache pc =JsonUtility.FromJson<PlayerCache>(jData);
                string[] keys = pc.Keys;
                if (keys.FirstOrDefault((registeredKey) => registeredKey == _cachedKey) != null)
                {
                    return true;
                }

                Array.Resize(ref keys, keys.Length + 1);
                keys[keys.Length - 1] = _cachedKey;
                pc.Keys = keys;
                jData = JsonUtility.ToJson(pc);
                PlayerPrefs.SetString(OncePerApplicationRunVariablesPrefKey, jData);
            }
            else
            {
                string jData = JsonUtility.ToJson(new PlayerCache() { Keys = new [] { _cachedKey }});
                PlayerPrefs.SetString(OncePerApplicationRunVariablesPrefKey, jData);
            }
            return false;
        }

        /// <summary>
        /// Delete application variable key if it has not been done before
        /// </summary>
        private void ResetApplicationVariablesIFN()
        {
            if (_hasResetVariables)
            {
                return;
            }

            _hasResetVariables = true;
            PlayerPrefs.DeleteKey(OncePerApplicationRunVariablesPrefKey);
        }
    }
}
#endif