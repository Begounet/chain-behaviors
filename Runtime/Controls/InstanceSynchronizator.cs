using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using AUE;

namespace ChainBehaviors
{
    /// <summary>
    /// Instantiate one GameObject from a prefab to a specific place.
    /// Automatically update the instance if the prefab is changed.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Instance Synchronizator")]
    public class InstanceSynchronizator : BaseMethod
    {
        [SerializeField]
        private UnityEventCallState _callState = UnityEventCallState.EditorAndRuntime;
        public UnityEventCallState CallState
        {
            get => _callState;
            set
            {
                _callState = value;
                Synchronize();
            }
        }

        [SerializeField]
        private GameObject _prefab;
        public GameObject Prefab
        {
            get => _prefab;
            set
            {
                _prefab = value;
                Synchronize();
            }
        }

        [SerializeField]
        private Transform _rootTarget;

        [SerializeField]
        private string _prefixName = "(Generated) ";

        [SerializeField, Tooltip("Instantiate as a prefab instance, while in edit mode")]
        private bool _asPrefabInEditMode = false;
        public bool AsPrefabInEditMode 
        { 
            get => _asPrefabInEditMode;
            set
            {
                if (_asPrefabInEditMode != value)
                {
                    _asPrefabInEditMode = value;
                    Synchronize();
                }
            }
        }

        #region Caches

        [SerializeField, HideInInspector]
        private GameObject _cachedPrefabInstanced;

        [SerializeField, HideInInspector]
        private bool _cachedInstancedAsPrefab = false;

        #endregion

        [SerializeField]
        private AUEEvent<GameObject> _onInstanced;


        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
#else
        [HideInInspector]
#endif
        private GameObject _instance;
        public GameObject Instance => _instance;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                // Ensure object has not been destroyed in the delay
                if (this != null)
                    {
                        Synchronize();
                    }
                };
            }
        }
#endif

        private void OnEnable() => Synchronize();

        private void Synchronize()
        {
            Trace(
                ("prefab", (_prefab != null ? _prefab.name : "null")),
                ("destination", (_rootTarget != null ? _rootTarget.gameObject.name : "null")));

            bool hasPrefabChanged = (_cachedPrefabInstanced != _prefab);
            bool hasPrefabModeChanged = _cachedInstancedAsPrefab != _asPrefabInEditMode;
            if (hasPrefabChanged || hasPrefabModeChanged)
            {
                DestroyInstance();
            }

            if (ShouldSynchronize())
            {
                if (_instance == null)
                {
                    Instantiate();
                }
            }
            else
            {
                DestroyInstance();
            }
        }

        private void Instantiate()
        {
            if (_prefab == null)
            {
                return;
            }

            bool shouldInstantiateAsPrefab = (_asPrefabInEditMode && !UnityEngine.Application.isPlaying);
            if (!shouldInstantiateAsPrefab)
            {
                _instance = Instantiate(_prefab);
            }
#if UNITY_EDITOR
            else
            {
                _instance = UnityEditor.PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
            }
#endif

            // Could not instantiate prefab
            if (_instance == null)
            {
                throw new UnityException($"Could not instantiate prefab {_prefab}");
            }

            _instance.name = $"{_prefixName}{_prefab.name}";

            Transform instanceTrans = _instance.transform;
            Transform prefabTrans = _prefab.transform;

            if (_rootTarget != null)
            {
                if (_rootTarget.gameObject.scene.IsValid())
                {
                    SceneManager.MoveGameObjectToScene(_instance.gameObject, _rootTarget.gameObject.scene);
                }
                instanceTrans.SetParent(_rootTarget, worldPositionStays: false);
            }
            instanceTrans.localPosition = prefabTrans.localPosition;
            instanceTrans.localRotation = prefabTrans.localRotation;
            instanceTrans.localScale = prefabTrans.localScale;

            _cachedPrefabInstanced = _prefab;
            _cachedInstancedAsPrefab = _asPrefabInEditMode;
            _onInstanced.Invoke(_instance);
        }

        private void DestroyInstance()
        {
            if (_instance != null)
            {
                if (UnityEngine.Application.isPlaying)
                {
                    Destroy(_instance);
                }
#if UNITY_EDITOR
                else if (!UnityEditor.PrefabUtility.IsPartOfPrefabInstance(_instance))
                {
                    DestroyImmediate(_instance, allowDestroyingAssets: false);
                }
#endif
                _instance = _cachedPrefabInstanced = null;
            }
        }

        private bool ShouldSynchronize()
        {
            bool isCallStateValid =
                (_callState == UnityEventCallState.EditorAndRuntime) ||
            (_callState == UnityEventCallState.RuntimeOnly && UnityEngine.Application.isPlaying);

            bool isPrefabValid = (_prefab != null);

            return (isCallStateValid && isPrefabValid);
        }
            
    }
}