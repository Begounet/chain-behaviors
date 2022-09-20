using ChainBehaviors.Processes;
using ChainBehaviors.Utils;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChainBehaviors.UI
{
    /// <summary>
    /// Fill the layout (horizontal/vertical) by repeating items and scrolling them.
    /// Can be used to make infinite scrolling information banner.
    /// </summary>
    /// <remarks>
    /// Does not support inserting new items dynamically.
    /// However, you can still disable the component, clear the layout children, 
    /// add new ones and re-enable the component to make a new list.
    /// </remarks>
    [AddComponentMenu(CBConstants.ModuleUIPath + "Scrollable Loop UI")]
    public class ScrollableLoopUI : BaseProcess
    {
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _root;

        [SerializeField, Tooltip("Determines the speed and the direction of the motion")]
        private float _speed = 1;

        private RectTransform _rootRect;
        private List<RectTransform> _items;
        private float _itemsAlignedSize;
        private bool _isHorizontal;
        private float _alignedPadding;
        private bool _isInitialized;

        private void Awake()
        {
            _isHorizontal = _root is HorizontalLayoutGroup;
        }

        private void OnEnable()
        {
            _rootRect = _root.GetComponent<RectTransform>();
            DelayedInitialization().Forget();
        }

        /// <summary>
        /// Delay the initialization because the rect transform may not have been
        /// set yet. Post pone the initialization to end of late update.
        /// </summary>
        private async UniTask DelayedInitialization()
        {
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            Initialize();
        }

        private void Initialize()
        {
            CacheItems();
            FillVisibilityWithItems();
            _isInitialized = true;
        }

        private void FillVisibilityWithItems()
        {
            float availableSpace = GetAlignedSize(_rootRect);
            availableSpace += GetBiggestItemAlignedSize();
            int idx = 0;
            
            RectTransform[] initialItems = new RectTransform[_items.Count];
            _items.CopyTo(initialItems);

            for (float x = _itemsAlignedSize; x < availableSpace; ++x)
            {
                for (int childIdx = 0; childIdx < initialItems.Length; ++childIdx)
                {
                    RectTransform itemToSpawn = initialItems[childIdx];
                    RectTransform itemInstance = Instantiate(itemToSpawn, _root.transform);
                    itemInstance.name = $"{itemToSpawn.name}:{idx++}";
                    x += GetAlignedSize(itemToSpawn) + _root.spacing;

                    _items.Add(itemInstance);
                }
            }
        }

        private float GetBiggestItemAlignedSize()
            => _items.Max((item) => item.rect.width);

        public override void UpdateProcess(float deltaTime)
        {
            if (!_isInitialized)
            {
                return;
            }

            UpdateItemsVisibility();
            UpdateItemsPositions();
        }

        private void UpdateItemsPositions()
        {
            _alignedPadding += _speed * Time.deltaTime;
            int moveBufferAsPixels = Mathf.RoundToInt(_alignedPadding);

            RectOffset offset = _root.padding;
            if (_isHorizontal)
            {
                offset.left = moveBufferAsPixels;
            }
            else
            {
                offset.top = moveBufferAsPixels;
            }
            LayoutRebuilder.MarkLayoutForRebuild(_rootRect);
        }

        private void UpdateItemsVisibility()
        {
            if (Mathf.Approximately(_speed, 0.0f))
            {
                return;
            }

            if (IsOutOfBounds(_items[0]))
            {
                // Move to end
                MoveItem(0, _items.Count);
            }
            else if (IsOutOfBounds(_items[_items.Count - 1]))
            {
                // Move to start
                MoveItem(_items.Count - 1, 0);
            }
        }

        private void MoveItem(int oldIdx, int newIdx)
        {
            RectTransform item = _items[oldIdx];
            _items.RemoveAt(oldIdx);
            if (newIdx > oldIdx)
            {
                --newIdx;
            }
            _items.Insert(newIdx, item);

            item.SetSiblingIndex(newIdx);

            float speedDir = GetAlignedDirection();
            float size = GetAlignedSize(item);
            _alignedPadding += -speedDir * (size + _root.spacing);
        }

        private bool IsOutOfBounds(RectTransform rect)
        {
            if (_isHorizontal)
            {
                if (_speed > 0)
                {
                    return (rect.offsetMin.x > _rootRect.rect.width);
                }
                else
                {
                    return (rect.offsetMax.x < 0);
                }
            }
            else
            {
                if (_speed > 0)
                {
                    return (rect.offsetMax.y < -_rootRect.rect.height);
                }
                else
                {
                    return (rect.offsetMin.y > 0);
                }
            }
        }

        private void CacheItems()
        {
            _alignedPadding = _isHorizontal ? _root.padding.left : _root.padding.top;
            _items = GetComponentsInChildren();
            _itemsAlignedSize = 0.0f;
            for (int i = 0; i < _items.Count; ++i)
            {
                _itemsAlignedSize += GetAlignedSize(_items[i]);
                if (i + 1 < _items.Count)
                {
                    _itemsAlignedSize += _root.spacing;
                }
            }
        }

        private float GetAlignedSize(RectTransform rect)
            => _isHorizontal ? rect.rect.width : rect.rect.height;

        private float GetAlignedDirection()
            => Mathf.Sign(_speed);

        private List<RectTransform> GetComponentsInChildren()
        {
            var rectTransformList = new List<RectTransform>();
            for (int i = 0; i < _root.transform.childCount; ++i)
            {
                Transform child = _root.transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    RectTransform rt = child.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rectTransformList.Add(rt);
                    }
                }
            }
            return rectTransformList;
        }
    }
}