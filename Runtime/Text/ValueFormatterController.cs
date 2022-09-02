using UnityEngine;
using AUE;

namespace ChainBehaviors.Text
{
    /// <summary>
    /// Convert a value to string according to the specific format, and call "Executed" when completed.
    /// </summary>
    public class ValueFormatterController : BaseMethod
    {
        [SerializeField]
        private string _format = "{0}";

        [SerializeField, Tooltip("Called after Format is called")]
        private AUEEvent<string> _executed = null;


        public void Format(int value) => InternalFormat(value);
        public void Format(float value) => InternalFormat(value);
        public void Format(double value) => InternalFormat(value);
        public void Format(bool value) => InternalFormat(value);
        public void Format(string value) => InternalFormat(value);
        public void Format(object value) => InternalFormat(value);

        private void InternalFormat(object value)
        {
            string result = string.Format(_format, value);
            Trace(("format", _format), ("value", value), ("result", result));
            _executed.Invoke(result);
        }
    }
}