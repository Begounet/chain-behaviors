using UnityEngine;

namespace ChainBehaviors.Math
{
    public class TransformMutator : BaseMethod
    {
        [SerializeField]
        private Transform _target;

        public float EulerAngleX
        {
            set
            {
                TraceCustomMethodName("eulerAngleX", ("value", value));
                Vector3 rotation = _target.eulerAngles;
                _target.rotation = Quaternion.Euler(value, rotation.y, rotation.z);
            }
        }

        public float EulerAngleY
        {
            set
            {
                TraceCustomMethodName("eulerAngleY", ("value", value));
                Vector3 rotation = _target.eulerAngles;
                _target.rotation = Quaternion.Euler(rotation.x, value, rotation.z);
            }
        }

        public float EulerAngleZ
        {
            set
            {
                TraceCustomMethodName("eulerAngleZ", ("value", value));
                Vector3 rotation = _target.eulerAngles;
                _target.rotation = Quaternion.Euler(rotation.x, rotation.y, value);
            }
        }
    }
}