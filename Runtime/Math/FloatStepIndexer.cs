using UnityEngine;
using Sirenix.OdinInspector;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Math
{
    /// <summary>
    /// Tell which threshold (index) has been reached by a floating value.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleMath + "Float Step Indexer")] 
    public class FloatStepIndexer : BaseMethod
    {
        [SerializeField]
        private float[] _thresholds = null;

        [SerializeField, HideInInspector]
        private bool _needsSort = false;

        [SerializeField]
        [Tooltip("Pass the index of the threshold reached as argument")]
        [HideLabel, HorizontalGroup]
        private AUEEvent<int> _onThresholdReached = null;
        public AUEEvent<int> OnThresholdReached => _onThresholdReached;

        private float[] SortedThresholds 
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
            _needsSort = !ThresholdUtils.IsSortedAscending(_thresholds);
        }

        public void Execute(int value)
        {
            Execute((float)value);
        }

        public void Execute(float value)
        {
            float[] sThresholds = SortedThresholds;
            for (int i = 0; i < sThresholds.Length; ++i)
            {
                bool isEqualsThreshold = Mathf.Approximately(value, sThresholds[i]);
                bool isAboveThreshold = value > sThresholds[i];
                bool isBelowNextThreshold = (i + 1 < sThresholds.Length ? value < sThresholds[i + 1] : true);

                if (isEqualsThreshold || (isAboveThreshold && isBelowNextThreshold))
                {
                    Trace(("value", value), ("threshold step index", sThresholds[i]));
                    OnThresholdReached.Invoke(i);
                    break;
                }
            }
        }

        [ContextMenu("Sort"), Button("Sort")]
        private void SortThresholds()
        {
            _needsSort = false;
            _thresholds = ThresholdUtils.Sort(_thresholds);
        }
    }
}
