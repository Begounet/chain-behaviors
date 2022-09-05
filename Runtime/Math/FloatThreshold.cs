using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Math
{
    /// <summary>
    /// Triggers events according to a floating value reaching some thresholds.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleMath + "Float Threshold")]
    public class FloatThreshold : BaseMethod
    {
        [System.Serializable]
        [InlineProperty]
        private class ThresholdExecuteInfo
        {
            [SerializeField]
            [HideLabel, HorizontalGroup]
            private float _threshold = 0.0f;
            public float Threshold => _threshold;

            [SerializeField]
            [HideLabel, HorizontalGroup]
            private AUEEvent<float> _onThresholdReached = null;
            public AUEEvent<float> OnThresholdReached => _onThresholdReached;
        }

        [SerializeField]
        private ThresholdExecuteInfo[] _thresholds = null;

        [SerializeField, HideInInspector]
        private bool _needsSort = false;

        private ThresholdExecuteInfo[] SortedThresholds
        {
            get
            {
                if (_needsSort)
                {
                    SortThresholds();
                }
                return _thresholds;
            }
        }

        private void OnValidate()
        {
            _needsSort = (_thresholds != null ? !ThresholdUtils.IsSortedAscending(_thresholds.Select((t) => t.Threshold)) : false);
        }

        public void Execute(int value)
        {
            Execute((float)value);
        }

        public void Execute(float value)
        {
            ThresholdExecuteInfo[] sThresholds = SortedThresholds;
            for (int i = 0; i < sThresholds.Length; ++i)
            {
                bool isEqualsThreshold = Mathf.Approximately(value, sThresholds[i].Threshold);
                bool isAboveThreshold = value > sThresholds[i].Threshold;
                bool isBelowNextThreshold = (i + 1 < sThresholds.Length ? value < sThresholds[i + 1].Threshold : true);

                if (isEqualsThreshold || (isAboveThreshold && isBelowNextThreshold))
                {
                    Trace(("value", value), ("threshold", _thresholds[i].Threshold));
                    _thresholds[i].OnThresholdReached.Invoke(value);
                    break;
                }
            }
        }

        [ContextMenu("Sort"), Button("Sort")]
        private void SortThresholds()
        {
            _needsSort = false;
            _thresholds = ThresholdUtils.Sort(_thresholds, (t) => t.Threshold);
        }
    }
}