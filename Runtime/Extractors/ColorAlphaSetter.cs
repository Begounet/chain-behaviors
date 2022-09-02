using AUE;
using UnityEngine;

namespace ChainBehaviors.Extractors
{
    public class ColorAlphaSetter : BaseMethod
    {
        [SerializeField]
        private AUEGet<Color> _colorGetter;

        [SerializeField]
        private AUEEvent<Color> _extracted;

        public void SetAlpha(float alpha)
        {
            Color c = _colorGetter.Invoke();
            c.a = alpha;
            _extracted.Invoke(c);
        }
    }
}