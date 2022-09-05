using AUE;
using ChainBehaviors.Utils;
using UnityEngine;

namespace ChainBehaviors.Extractors
{
    /// <summary>
    /// Set the alpha of a color
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleMutatorsPath + "Color Alpha Mutator")]
    public class ColorAlphaMutator : BaseMethod
    {
        [SerializeField]
        private AUEGet<Color> _colorGetter;

        [SerializeField]
        private AUEEvent<Color> _extracted;

        public void SetAlpha(float alpha)
        {
            Trace(("alpha", alpha));

            Color c = _colorGetter.Invoke();
            c.a = alpha;
            _extracted.Invoke(c);
        }
    }
}