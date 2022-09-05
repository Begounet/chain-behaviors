using ChainBehaviors.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ChainBehaviors
{
    /// <summary>
    /// Enable/disable easily multiple GameObjects
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleGameObjects + "GameObjects Enable Controller")]
    public class GameObjectsEnableController : BaseMethod
    {
        [SerializeField, FormerlySerializedAs("_targets")]
        private GameObject[] _onEnabled = null;

        [SerializeField]
        private GameObject[] _onDisabled = null;

        [SerializeField]
        private UnityEvent _executed = null;

        public void Execute(bool enable)
        {
            Trace(("enable", enable));
            for (int i = 0; i < _onEnabled.Length; ++i)
            {
#if UNITY_EDITOR
                if (_onEnabled[i] == null)
                {
                    Debug.LogWarning($"OnEnabled[{i}] should not be null", this);
                    continue;
                }
#endif
                _onEnabled[i].SetActive(enable);
            }
            for (int i = 0; i < _onDisabled.Length; ++i)
            {
#if UNITY_EDITOR
                if (_onDisabled[i] == null)
                {
                    Debug.LogWarning($"OnDisabled[{i}] should not be null", this);
                    continue;
                }
#endif
                _onDisabled[i].SetActive(!enable);
            }
            _executed.Invoke();
        }
    }
}
