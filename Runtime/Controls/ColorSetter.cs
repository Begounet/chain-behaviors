using UnityEngine;
using AUE;

namespace ChainBehaviors
{
    /// <summary>
    /// Set a color.
    /// Auxiliary colors are available if you want to set from a color database.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Color Setter")]
    public class ColorSetter : BaseMethod
    {
        [SerializeField]
        private Color _color;

        [SerializeField]
        private Color[] _auxColors;

        [SerializeField]
        private AUEEvent<Color> _set;


        public void Set()
        {
            TraceCustomMethodName("execute", ("color", _color));
            _set.Invoke(_color);
        }

        public void SetAuxColor(int colorIdx)
        {
            Color auxColor = _auxColors[colorIdx];
            TraceCustomMethodName("execute aux color", ("index", colorIdx), ("color", auxColor));
            _set.Invoke(auxColor);
        }
    }
}