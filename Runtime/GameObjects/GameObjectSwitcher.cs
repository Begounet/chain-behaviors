using System;
using UnityEngine;
using Sirenix.OdinInspector;
using AppTools;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Enable/disable GameObject child according to an index.
    /// Only one child can be activated. 
    /// Useful to make a tab system in UI.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleGameObjects + "GameObject Switcher")]
    public class GameObjectSwitcher : BaseMethod
    {
        [SerializeField]
        private int _startIndex = 0;

#pragma warning disable 414 // Used in Editor
        [SerializeField, Tooltip("If true, you can switch GameObject by selecting child GameObject")]
        private bool _editorSwitch = true;
#pragma warning restore

        private int _currentIndex = -1;

        [ShowInInspector, ReadOnly]
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                {
                    TraceCustomMethodName("Set Index", 
                        ("old index", _currentIndex), 
                        ("new index", value));

                    _currentIndex = value;
                    SetVisibilityOn(_currentIndex);
                }
            }
        }

        public int ClampedCurrentIndex
        {
            get => _currentIndex;
            set => CurrentIndex = Mathf.Clamp(value, 0, this.transform.childCount - 1);
        }

#if UNITY_EDITOR

        // Manage activation changes when selecting children

        private static event Action<GameObjectSwitcher, Transform> OnUISwitcherChildSelected;

        [UnityEditor.InitializeOnLoadMethod]
        private static void OnEnterPlayMode()
        {
            UnityEditor.Selection.selectionChanged -= OnSelectionChanged;
            UnityEditor.Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            if (UnityEditor.Selection.activeGameObject == null)
            {
                return;
            }

            GameObject selectedGo = UnityEditor.Selection.activeGameObject;

            GameObjectSwitcher[] closeParents = selectedGo.GetComponentsInParents<GameObjectSwitcher>();
            if (closeParents != null)
            {
                Array.ForEach(closeParents, (closeParent) =>
                {
                    Transform childTrans = UnityEditor.Selection.activeTransform;
                    if (closeParent.transform != childTrans)
                    {
                        OnUISwitcherChildSelected(closeParent, childTrans);
                    }
                });
            }
        }

        private void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                OnUISwitcherChildSelected -= HandleUISwitchChildSelected;
                OnUISwitcherChildSelected += HandleUISwitchChildSelected;
            }
        }

        private void HandleUISwitchChildSelected(GameObjectSwitcher uiSwitcher, Transform child)
        {
            if (this == null)
            {
                // May happen sometimes, if the object has been destroyed
                // but did not unsubscribe correctly
                OnUISwitcherChildSelected -= HandleUISwitchChildSelected;
                return;
            }

            if (this == uiSwitcher && isActiveAndEnabled && _editorSwitch)
            {
                SetVisibilityOn(FindSwitchIndexByChild(child));
            }
        }

        private void OnDestroy()
        {
            OnUISwitcherChildSelected -= HandleUISwitchChildSelected;
        }

#endif

        private void Start()
        {
            // Only assign the start index if no index has already been set
            if (_currentIndex < 0)
            {
                ClampedCurrentIndex = _startIndex;
            }
        }

        private void SetVisibilityOn(int index)
        {
            Trace(
                ("UI index", index),
                ("UI visible child", (index >= 0 && index < transform.childCount) ? transform.GetChild(index).name : "none (index of bounds)"));

            for (int childIndex = 0; childIndex < this.transform.childCount; ++childIndex)
            {
                Transform child = this.transform.GetChild(childIndex);
                child.gameObject.SetActive(childIndex == index);
            }
        }

        public int FindSwitchIndexByChild(Transform target)
        {
            if (target == null)
            {
                return -1;
            }

            for (int childIndex = 0; childIndex < this.transform.childCount; ++childIndex)
            {
                Transform child = this.transform.GetChild(childIndex);
                if (child == target || child.ContainsChild(target))
                {
                    return childIndex;
                }
            }
            return -1;
        }

        public void SetVisibilityOnTarget(Transform target)
        {
            CurrentIndex = FindSwitchIndexByChild(target);
        }
    }
}