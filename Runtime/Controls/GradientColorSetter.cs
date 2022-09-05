using AUE;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Call Set event with the color from a normalized position inside the gradient.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Gradient Color Setter")]
    public class GradientColorSetter : BaseMethod
    {
        [SerializeField]
        private Gradient _gradient = new Gradient()
        {
            colorKeys = new[]
            {
                new GradientColorKey(Color.black, 0.0f),
                new GradientColorKey(Color.white, 1.0f)
            },
            alphaKeys = new[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 0.0f),
            },
        };

        [SerializeField]
        private AUEEvent<Color> _set;


        public void SetColorFromGradientNormalizedValue(float value)
        {
            _set.Invoke(_gradient.Evaluate(Mathf.Clamp01(value)));
        }
    }
}
