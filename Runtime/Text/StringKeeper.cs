using UnityEngine;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Text
{
    /// <summary>
    /// Simple string holder. Allow to reuse the result later.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleTextPath + "String Keeper")]
    public class StringKeeper : MonoBehaviour
    {
        [SerializeField, TextArea]
        private string _string = string.Empty;
        public string String
        {
            get => _string;
            set
            {
                _string = value;
                if (_notifyChanges)
                {
                    RefreshString();
                }
            }
        }

        [SerializeField]
        private bool _notifyChanges = true;
        public bool NotifyChanges
        {
            get => _notifyChanges;
            set => _notifyChanges = value;
        }

        [SerializeField]
        private AUEEvent<string> _updateString = null;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif

            RefreshString();
        }

        public void RefreshString()
        {
            if (_notifyChanges)
            {
                _updateString?.Invoke(_string);
            }
        }
    }
}