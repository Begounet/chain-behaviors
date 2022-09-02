using AUE;
using System;
using UnityEngine;

namespace ChainBehaviors.Math
{
    public class MathClampOperation : MonoBehaviour
    {
        [Flags]
        public enum EOperationMask
        {
            None = 0x00,
            Min = 0x01,
            Max = 0x02,
        }

        [SerializeField]
        private EOperationMask _clampOperation = EOperationMask.Min | EOperationMask.Max;

        [SerializeField]
        private float _minValue = 0.0f;

        [SerializeField]
        private float _maxValue = 1.0f;

        [SerializeField]
        private AUEEvent<float> _clampedFloat;

        [SerializeField]
        private AUEEvent<int> _clampedInteger;

        public void Clamp(float value)
        {
            if (_clampOperation.HasFlag(EOperationMask.Min))
            {
                value = Mathf.Max(value, _minValue);
            }
            if (_clampOperation.HasFlag(EOperationMask.Max))
            {
                value = Mathf.Min(value, _maxValue);
            }
            _clampedFloat.Invoke(value);
        }

        public void Clamp(int value)
        {
            if (_clampOperation.HasFlag(EOperationMask.Min))
            {
                value = Mathf.Min(value, (int) _minValue);
            }
            if (_clampOperation.HasFlag(EOperationMask.Max))
            {
                value = Mathf.Max(value, (int) _maxValue);
            }
            _clampedInteger.Invoke(value);
        }
    }
}