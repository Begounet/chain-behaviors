using AUE;
using ChainBehaviors.Utils;
using UnityEngine;

namespace ChainBehaviors.Math
{
    /// <summary>
    /// Remap a float from a range to another one.
    /// If min = -1, max = 1 and newMin = 0 and newMax = 1,
    /// then when value = 0, result = 0.5f
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleMath + "Float Remapper")]
    public class FloatRemapper : BaseMethod
    {
        [SerializeField]
        private float _min = 0;

        [SerializeField]
        private float _max = 1;

        [SerializeField]
        private float _newMin = 0;

        [SerializeField]
        private float _newMax = 1;

        [SerializeField]
        private bool _clamped = false;

        [SerializeField]
        private AUEEvent<float> _onValueRemapped = null;


        public void Remap(float value)
        {
            float normal = Mathf.InverseLerp(_min, _max, value);
            float newValue = Mathf.Lerp(_newMin, _newMax, normal);

            if (_clamped)
            {
                newValue = Mathf.Clamp(newValue, _newMin, _newMax);
            }

            Trace(
                ("value", value), 
                ("min-max", $"{_min} - {_max}"), 
                ("new min-max", $"{_newMin} - {_newMax}"), 
                ("clamped", _clamped),
                ("result", newValue));

            _onValueRemapped.Invoke(newValue);
        }
    }
}

